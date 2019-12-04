
using NewOptics.Administrateur;
using NewOptics.Tarif;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for ListeSupplement.xaml
    /// </summary>
    public partial class ListeSupplement : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerListeSupplement();
        SelectionVerre window;
        SVC.Param selectedparam;
        public ListeSupplement(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu,SelectionVerre windowrecu)
        {
            try
            {
           InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                selectedparam=proxy.GetAllParamétre();
                PatientDataGrid.ItemsSource = proxy.GetAllSupplement().OrderBy(n => n.Libelle);
               window = windowrecu;
             FamilleCombo.ItemsSource = proxy.GetAllCatSupp().OrderBy(x => x.Cat);

                if (selectedparam.AffichPrixAchatVente == true)
                {
                    PatientDataGrid.Columns[3].Visibility = Visibility.Visible;
                }
                else
                {
                    PatientDataGrid.Columns[3].Visibility = Visibility.Collapsed;
                }

                callbackrecu.InsertSupplementCallbackevent += new ICallback.CallbackEventHandler13(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true)
                {
                    AjouterSupplement cl = new AjouterSupplement(proxy, callback, MemberUser, null);
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
        void callbackrecu_Refresh(object source, CallbackEventInsertSupplement e)
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


        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeSupplement(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeSupplement(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void HandleProxy()
        {
            if (proxy != null)
            {
                switch (this.proxy.State)
                {
                    case CommunicationState.Closed:
                        proxy.Abort();
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

        public void AddRefresh(SVC.Supplement listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Supplement>;
                List<SVC.Supplement> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {
                        //  var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;


                        var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listMembershipOptic;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                PatientDataGrid.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnselection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedverre = PatientDataGrid.SelectedItems;
                var LISTITEM1 = window.SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;
                List<SVC.MontureSupplement> LISTITEM;
                if (LISTITEM1 != null)
                {
                   LISTITEM = LISTITEM1.ToList();
                }
                else
                {
                    LISTITEM = new List<SVC.MontureSupplement>();
                }
              if (selectedverre.Count > 0)
                {
                    if (LISTITEM!=null)
                    {

                        SVC.MontureSupplement cc;
                        foreach (var item in selectedverre)
                        {
                            var dog = item as SVC.Supplement;

                            cc = new SVC.MontureSupplement
                            {
                                Libelle = dog.Libelle,
                                PrixAchat = dog.PrixAchat,
                                PrixVente = dog.PrixVente,

                            };
                            LISTITEM.Add(cc);
                             switch (window.verrelocalisation)
                              {
                                  case 1:
                                      window.ListSupp1.Add(cc);

                                      break;
                                  case 2:
                                      window.ListSupp2.Add(cc);

                                      break;
                                  case 3:
                                      window.ListSupp3.Add(cc);

                                      break;
                                  case 4:
                                      window.ListSupp4.Add(cc);

                                      break;
                              }
                        }
                    }
                    else
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Valeur null", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    window.SuppDataGrid.ItemsSource = LISTITEM;
                }
                this.Close();


            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                PatientDataGrid.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void FournisseurCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

          try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);

                cv00.Filter = delegate (object item)
                {
                    SVC.Supplement temp = item as SVC.Supplement;
                    return temp.IdCat != -1;


                };

                FamilleCombo.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        

        private void FamilleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FamilleCombo.SelectedItem != null)
                {
                    SVC.CatSupp t = FamilleCombo.SelectedItem as SVC.CatSupp;

                    var filter = (t.Id).ToString();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid);
                    if (filter == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            SVC.Supplement p = o as SVC.Supplement;
                            if (t.Id.ToString() == "txtId")
                                return ((p.IdCat).ToString() == filter);
                            return (p.IdCat.ToString().ToUpper().Contains(filter.ToUpper()));
                        };

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtDesign_TextChanged(object sender, TextChangedEventArgs e)
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
                        SVC.Supplement p = o as SVC.Supplement;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Libelle.ToUpper().Contains(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}

 
 
