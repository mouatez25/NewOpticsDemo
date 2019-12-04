using NewOptics.Administrateur;
using NewOptics.Stock;
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

namespace NewOptics.Tarif
{
    /// <summary>
    /// Interaction logic for ListSupplément.xaml
    /// </summary>
    public partial class ListSupplément : Page
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerSupplément();
        bool filtre = false;
        SVC.Param selectedparam;
        public ListSupplément(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                selectedparam=proxy.GetAllParamétre();
                PatientDataGrid.ItemsSource = proxy.GetAllSupplement().OrderBy(n => n.Id);
                PatientDataGrid.SelectedItem = null;
               


                callbackrecu.InsertSupplementCallbackevent += new ICallback.CallbackEventHandler13(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSupplément(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSupplément(HandleProxy));
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
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                //  ;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Supplement p = o as SVC.Supplement;
                        if (t.Name == "txtId")
                            return (p.Libelle == filter);
                        return (p.Libelle.ToUpper().Contains(filter.ToUpper()));


                    };

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void MarqueCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                PatientDataGrid.ItemsSource = proxy.GetAllSupplement().OrderBy(n => n.Id);
          

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.ItemsSource = proxy.GetAllSupplement().OrderBy(n => n.Id);
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimerStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.ImpressionFichier == true && PatientDataGrid.Items.Count > 0)
                {
                    var itemsSource0 = PatientDataGrid.Items.OfType<SVC.Produit>();
                    ImpressionProduit cl = new ImpressionProduit(proxy, itemsSource0.ToList());
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

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(MemberUser.CreationFichier==true)
                {
                    AjouterSupplement cl = new AjouterSupplement(proxy,callback,MemberUser,null);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem!=null && MemberUser.SuppressionFichier==true)
                {
                    SVC.Supplement selectedsupp = PatientDataGrid.SelectedItem as SVC.Supplement;
                    bool succes = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.DeleteSupplement(selectedsupp);
                        succes = true;
                    }
                    if (succes == true)
                    {
                        proxy.AjouterSupplementRefresh();
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MemberUser.ModuleFichier == true && PatientDataGrid.SelectedItem!=null)
            {
                SVC.Supplement selectedsupp = PatientDataGrid.SelectedItem as SVC.Supplement;
                AjouterSupplement cl = new AjouterSupplement(proxy, callback, MemberUser, selectedsupp);
                cl.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.ImpressionFichier == true && PatientDataGrid.Items.Count > 0)
                {
                    if (PatientDataGrid.SelectedItem==null)
                    {
                        List<SVC.Supplement> itemsSource0 = PatientDataGrid.Items.OfType<SVC.Supplement>().ToList();
                        
                        ImpressionSupp cl = new ImpressionSupp(proxy, itemsSource0.ToList());
                        cl.Show();
                    }else
                    {
                        SVC.Supplement SELECTED = PatientDataGrid.SelectedItem as SVC.Supplement;
                        List<SVC.Supplement> itemsSource0 = new List<SVC.Supplement>();
                      //  for (int i = 0; i < PatientDataGrid.VisibleRowCount; i++)
                        //{
                           // SVC.Supplement p = PatientDataGrid.GetRow(PatientDataGrid.GetRowHandleByVisibleIndex(i)) as SVC.Supplement;

                            itemsSource0.Add(SELECTED);
                       // }
                        ImpressionOneSupplement cl = new ImpressionOneSupplement(proxy, itemsSource0.ToList());
                        cl.Show();
                    }

                   
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

        private void btnEdit_Click(object sender, MouseButtonEventArgs e)
        {
            if (MemberUser.ModuleFichier == true && PatientDataGrid.SelectedItem != null)
            {
                SVC.Supplement selectedsupp = PatientDataGrid.SelectedItem as SVC.Supplement;
                AjouterSupplement cl = new AjouterSupplement(proxy, callback, MemberUser, selectedsupp);
                cl.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if(MemberUser.ModuleFichier==true)
                {
                    ListeCategorieSupplement cl = new ListeCategorieSupplement(proxy,MemberUser,callback);
                    cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
