
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
    /// Interaction logic for ListLentille.xaml
    /// </summary>
    public partial class ListLentille : Page
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerLentille();
        bool filtre = false;
        public ListLentille(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;

                PatientDataGrid.ItemsSource = proxy.GetAllLentille().OrderBy(n => n.Design);
                MarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(x => x.MarqueProduit);



                callbackrecu.InsertLentilleCallbackevent += new ICallback.CallbackEventHandler29(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertLentille e)
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



        public void AddRefresh(SVC.Lentille listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Lentille>;
                List<SVC.Lentille> LISTITEM = LISTITEM1.ToList();

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerLentille(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerLentille(HandleProxy));
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
       
     
        private void MarqueCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
        }
        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.ItemsSource = proxy.GetAllLentille().OrderBy(n => n.Design);
                chstockFiltreUV.IsChecked = false;
                MarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(x => x.MarqueProduit);
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
                    List<SVC.Lentille> itemsSource0 = PatientDataGrid.Items.OfType<SVC.Lentille>().ToList();
                   
                    ImpressionLentille cl = new ImpressionLentille(proxy, itemsSource0.ToList());
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

        private void chstockFiltreUV_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
               
                    bool filtreuv = false;

                    if (chstockFiltreUV.IsChecked == true)
                    {
                        filtreuv = true;
                    }
                    else
                    {
                        filtreuv = false;
                    }

                    string filterValue = filtreuv.ToString();
                  
                    PatientDataGrid.SelectedItem = null;
                

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chstockFiltreUV_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool filtreuv = false;

                if (chstockFiltreUV.IsChecked == true)
                {
                    filtreuv = true;
                }
                else
                {
                    filtreuv = false;
                }

               
                PatientDataGrid.SelectedItem = null;


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
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Lentille mm = PatientDataGrid.SelectedItem as SVC.Lentille;
                    SVC.Produit selectedproduit = proxy.GetAllProduitbycab(mm.cleproduit);
                    AjouterProduit cl = new AjouterProduit(proxy, selectedproduit, MemberUser, callback);
                    cl.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEditerStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Lentille mm = PatientDataGrid.SelectedItem as SVC.Lentille;
                    SVC.Produit selectedproduit = proxy.GetAllProduitbycab(mm.cleproduit);
                    AjouterProduit cl = new AjouterProduit(proxy, selectedproduit, MemberUser, callback);
                    cl.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                  

                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (PatientDataGrid.SelectedItem != null)
                    {
                        SVC.Lentille selectedlentille = PatientDataGrid.SelectedItem as SVC.Lentille;
                        proxy.DeleteLentille(selectedlentille);
                        ts.Complete();
                    }
                }
                }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Lentille p = o as SVC.Lentille;
                        if (t.Name == "txtId")
                            return (p.Design == filter);
                        return (p.Design.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MarqueCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {


                  if (MarqueCombo.SelectedItem != null)
                {
                    bool filtreuv = false;

                    if (chstockFiltreUV.IsChecked == true)
                    {
                        filtreuv = true;
                    }
                    else
                    {
                        filtreuv = false;
                    }
                    SVC.Marque t = MarqueCombo.SelectedItem as SVC.Marque;

                     var filter = (t.MarqueProduit).ToString();

                     ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                     if (filter == "")
                         cv.Filter = null;
                     else
                     {
                         cv.Filter = o =>
                         {
                             SVC.Lentille p = o as SVC.Lentille;
                             if (t.MarqueProduit == "txtId")
                                 return (p.Marque == filter);
                             return (p.Marque.Contains(filter.ToUpper()) && p.FiltreUV == filtreuv);
                         };

                     }

                 } 
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
