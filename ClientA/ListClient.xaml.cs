
using NewOptics.Administrateur;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for ListClient.xaml
    /// </summary>
    public partial class ListClient : Page
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;
        SVC.ClientV MedecinEnCours;
        SVC.Client localclient;
        private delegate void FaultedInvokerListePatient();
        public ListClient(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Client clientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                PatientDataGrid.ItemsSource = proxy.GetAllClientV().Where(n => n.Id != 0).OrderByDescending(n => n.Id);
                PatientDataGrid.SelectedItem = null;
                memberuser = memberrecu;

                callback = callbackrecu;
                localclient = clientrecu;


                callbackrecu.InsertClientVCallbackEvent += new ICallback.CallbackEventHandler7(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtRecherche.Text))
                return true;
            else
                return ((item as SVC.ClientV).Raison.IndexOf(txtRecherche.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListePatient(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void txtRef_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.ClientV p = o as SVC.ClientV;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Ref.ToString().ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void HandleProxy()
        {
            if (proxy != null)
            {
                switch (this.proxy.State)
                {
                    case CommunicationState.Closed:
                        proxy = null;

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;


                        break;
                    case CommunicationState.Opened:


                        break;
                    case CommunicationState.Opening:
                        break;
                    default:
                        break;
                }
            }

        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListePatient(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertClientV e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.ClientV listmembership, int oper)
        {
            try
            {
                // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Mise à jours", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.ClientV>;
                List<SVC.ClientV> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Insert(0, listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                        //   var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //  objectmodifed = listmembership;

                        var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //objectmodifed = listmembership;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listmembership;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                PatientDataGrid.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    NewClient CLMedecin = new NewClient(proxy, memberuser, null);
                    CLMedecin.Show();

                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.SuppressionFichier == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;

                    if (SelectMedecin.Id != 0)
                    {
                        int nbvisite = proxy.GetAllF1Bycode(SelectMedecin.Id).Count();
                        if (nbvisite == 0)
                        {
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                proxy.DeleteClientV(SelectMedecin);
                                ts.Complete();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            proxy.AjouterClientVRefresh();
                        }
                        else
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModificationFichier == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;
                    NewClient CLMedecin = new NewClient(proxy, memberuser, SelectMedecin);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionFichier == true)
                {
                    if (PatientDataGrid.SelectedItem != null)
                    {
                        SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;



                        FichePatient clsho = new FichePatient(proxy, SelectMedecin);
                        clsho.Show();
                    }
                    else
                    {
                        List<SVC.ClientV> itemsSource0 = PatientDataGrid.Items.OfType<SVC.ClientV>().ToList();
                        
                        ReportListePatient clsho = new ReportListePatient(proxy, itemsSource0);
                        clsho.Show();


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }




        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               
                if (memberuser.ModificationFichier == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;
                    NewClient CLMedecin = new NewClient(proxy, memberuser, SelectMedecin);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnDossier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModuleDossierClient == true && PatientDataGrid.SelectedItem != null)
                {
                    var selectedclient = PatientDataGrid.SelectedItem as SVC.ClientV;
                    DossierClient cl = new DossierClient(proxy, callback, memberuser, selectedclient);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

     

        private void txtRecherche_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.Items.OfType<SVC.ClientV>().ToList());
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.ClientV p = o as SVC.ClientV;
                        if (t.Name == "txtId")
                            return (p.Raison == filter);
                        return (p.Raison.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRef_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.Items.OfType<SVC.ClientV>().ToList());
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.ClientV p = o as SVC.ClientV;
                        if (t.Name == "txtId")
                            return (p.Ref == filter);
                        return (p.Ref.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PatientDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (memberuser.ModificationFichier == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;
                    NewClient CLMedecin = new NewClient(proxy, memberuser, SelectMedecin);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModificationFichier == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;
                    NewClient CLMedecin = new NewClient(proxy, memberuser, SelectMedecin);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void FolderClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModuleDossierClient == true && PatientDataGrid.SelectedItem != null)
                {
                    var selectedclient = PatientDataGrid.SelectedItem as SVC.ClientV;
                    DossierClient cl = new DossierClient(proxy, callback, memberuser, selectedclient);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void supprimer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.SuppressionFichier == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.ClientV SelectMedecin = PatientDataGrid.SelectedItem as SVC.ClientV;

                    if (SelectMedecin.Id != 0)
                    {
                        int nbvisite = proxy.GetAllF1Bycode(SelectMedecin.Id).Count();
                        if (nbvisite == 0)
                        {
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                proxy.DeleteClientV(SelectMedecin);
                                ts.Complete();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            proxy.AjouterClientVRefresh();
                        }
                        else
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
