
using NewOptics.Administrateur;
using NewOptics.Stock;
using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Tarif
{
    /// <summary>
    /// Interaction logic for ListVerre.xaml
    /// </summary>
    public partial class ListVerre : Page
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerVerre();
        bool filtre = false;
        public ListVerre(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;

                PatientDataGrid.ItemsSource = proxy.GetAllVerre().OrderBy(n => n.Design);
                MarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(x => x.MarqueProduit);



                callbackrecu.InsertVerreCallbackevent += new ICallback.CallbackEventHandler31(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertVerre e)
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



        public void AddRefresh(SVC.Verre listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Verre>;
                List<SVC.Verre> LISTITEM = LISTITEM1.ToList();

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVerre(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVerre(HandleProxy));
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
       
        private void MarqueCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                PatientDataGrid.ItemsSource = proxy.GetAllVerre().OrderBy(n => n.Design);
                MarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(x => x.MarqueProduit);
                chstockVerreTeinte.IsChecked = false;
                chstockVerreAphaque.IsChecked = false;
                chstockVerrePhotochromique.IsChecked = false;

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
                PatientDataGrid.ItemsSource = proxy.GetAllVerre().OrderBy(n => n.Design);

                MarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                chstockVerreTeinte.IsChecked = false;
                chstockVerreAphaque.IsChecked = false;
                chstockVerrePhotochromique.IsChecked = false;

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
                if(MemberUser.ImpressionFichier==true && PatientDataGrid.Items.Count>0)
                {

                    List<SVC.Verre> itemsSource0 = PatientDataGrid.Items.OfType<SVC.Verre>().ToList();

                    ImpressionVerre cl = new ImpressionVerre(proxy,itemsSource0.ToList());
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

     /*   private void chstockVerreTeinte_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool photo, teinté, aphaque = false;

                if (chstockVerreTeinte.IsChecked == true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.Items);
                if (chstockVerreTeinte.IsChecked == false)
                {
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Verre temp = item as SVC.Verre;
                        return /*(temp.VerreTeinté == true || temp.VerreTeinté == false) &&*/ //temp.Photochromique == photo && temp.Aphaque == aphaque; 


                /*    };

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }*/

      /*  private void chstockVerreTeinte_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool photo,teinté,aphaque = false;
               
                if (chstockVerreTeinte.IsChecked==true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }

                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.Items);
                if (chstockVerreTeinte.IsChecked == true)
                {
                    //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Verre temp = item as SVC.Verre;
                        return temp.VerreTeinté == true && temp.Photochromique== photo && temp.Aphaque== aphaque;


                    };

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }*/

     /*   private void chstockVerreAphaque_Unchecked(object sender, RoutedEventArgs e)
        {

            try
            {
                bool photo, teinté, aphaque = false;

                if (chstockVerreTeinte.IsChecked == true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.Items);
                if (chstockVerreAphaque.IsChecked == false)
                {
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Verre temp = item as SVC.Verre;
                        return /*(temp.Aphaque == true || temp.Aphaque == false) && *///temp.VerreTeinté== teinté && temp.Photochromique == photo;


             /*       };

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }*/

      /*  private void chstockVerreAphaque_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool photo, teinté, aphaque = false;

                if (chstockVerreTeinte.IsChecked == true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.Items);
                if (chstockVerreAphaque.IsChecked == true)
                {
                    //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Verre temp = item as SVC.Verre;
                        return temp.Aphaque == true && temp.VerreTeinté == teinté && temp.Photochromique ==photo;


                    };

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }*/

  /*      private void chstockVerrePhotochromique_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool photo, teinté, aphaque = false;

                if (chstockVerreTeinte.IsChecked == true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.Items);
                if (chstockVerrePhotochromique.IsChecked == false)
                {
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Verre temp = item as SVC.Verre;
                        return /*(temp.Photochromique == true || temp.Photochromique == false) && *///temp.Aphaque==aphaque && temp.VerreTeinté== teinté;


             /*       };

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }*/

       /* private void chstockVerrePhotochromique_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool photo, teinté, aphaque = false;

                if (chstockVerreTeinte.IsChecked == true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.Items);
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Verre temp = item as SVC.Verre;
                        return temp.Photochromique == true && temp.Aphaque == aphaque && temp.VerreTeinté == teinté ;


                    };

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }*/

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool photo, teinté, aphaque,marque = false;

                if (chstockVerreTeinte.IsChecked == true)
                {
                    teinté = true;
                }
                else
                {
                    teinté = false;
                }
                if (chstockVerreAphaque.IsChecked == true)
                {
                    aphaque = true;
                }
                else
                {
                    aphaque = false;
                }
                if (chstockVerrePhotochromique.IsChecked == true)
                {
                    photo = true;
                }
                else
                {
                    photo = false;
                }

                if(MarqueCombo.SelectedItem!=null)
                {
                    marque = true;
                }
                else
                {
                    marque = false;
                }
                if(marque==true)
                {
                    SVC.Marque SELECTEDMarque = MarqueCombo.SelectedItem as SVC.Marque;


                    PatientDataGrid.ItemsSource = proxy.GetAllVerre().Where(n => n.Marque == SELECTEDMarque.MarqueProduit && n.Aphaque == aphaque && n.VerreTeinté == teinté && n.Photochromique == photo);
                }
                else
                {
                    PatientDataGrid.ItemsSource = proxy.GetAllVerre().Where(n =>  n.Aphaque == aphaque && n.VerreTeinté == teinté && n.Photochromique == photo);

                }

            }
            catch(Exception ex)
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
                    SVC.Verre mm = PatientDataGrid.SelectedItem as SVC.Verre;
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
                    SVC.Verre mm = PatientDataGrid.SelectedItem as SVC.Verre;
                    SVC.Produit selectedproduit = proxy.GetAllProduitbycab(mm.cleproduit);
                    AjouterProduit cl = new AjouterProduit(proxy,selectedproduit,MemberUser,callback);
                    cl.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        

        private void btnGroupeStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("La création se fait par un seul groupe", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    AjouterCatalogueVerre cl = new AjouterCatalogueVerre(proxy, null, MemberUser, callback);
                    cl.Show();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
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
                    SVC.Verre p = o as SVC.Verre;
                    if (t.Name == "txtId")
                        return (p.Design == filter);
                    return (p.Design.ToUpper().Contains(filter.ToUpper()));
                };
            }

        }
    }
}
