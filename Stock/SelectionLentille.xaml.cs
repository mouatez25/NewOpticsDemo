
using NewOptics.Administrateur;
using NewOptics.Commande;
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
    /// Interaction logic for SelectionLentille.xaml
    /// </summary>
    public partial class SelectionLentille : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MemberUser;
        ICallback callback;
        SVC.Produit Produit;

        SVC.Param selectedparam;
        int interfacenew;
        public int verrelocalisation;
        SVC.LentilleClient SelectedMonture;
        private delegate void FaultedInvokerSelectionLentille();
        SVC.ClientV CLIENTV;
        public SelectionLentille(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, int verrrerecu, SVC.LentilleClient MontureRecu, int interfcerecu,SVC.ClientV CLIENTRECU)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = memberrecu;
                SelectedMonture = MontureRecu;
                verrelocalisation = verrrerecu;
                interfacenew = interfcerecu;
                CLIENTV = CLIENTRECU;
                selectedparam = proxy.GetAllParamétre();
                PatientDataGrid.ItemsSource = proxy.GetAllLentille().OrderBy(n => n.Design);
                if (interfcerecu == 0)
                {
                  switch (verrelocalisation)
                    {
                        case 1:
                            Produit = new SVC.Produit();
                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
       
                            this.Title = "Sélection de la lentille DROITE";
                            break;
                        case 2:
                           

                            Produit = new SVC.Produit();
                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                           
                            this.Title = "Sélection de la lentille GAUCHE";
                            break;
                     
                    }
                }
                else
                {
                    if (interfcerecu == 1)
                    {

                        List<SVC.Lentille> testmedecind = (proxy.GetAllLentille().OrderBy(n => n.Design)).ToList();
                        PatientDataGrid.ItemsSource = testmedecind;
                        switch (verrrerecu)
                        {
                            case 1:
                                if (MontureRecu.IdDroiteLentille!= null)
                                {
                                    if (MontureRecu.IdDroiteLentille != 0)
                                    {
                                        Produit = proxy.GetAllProduitbyid(Convert.ToInt16(MontureRecu.IdDroiteLentille)).First();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                     
                                    }
                                    else
                                    {
                                        Produit = new SVC.Produit();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                      
                                    }
                                }
                                else
                                {
                                    Produit = new SVC.Produit();
                                    griddetail.DataContext = Produit;
                                    btnselection.IsEnabled = true;
                                  
                                }
                                this.Title = "Sélection de la lentille DROITE";
                                break;
                            case 2:
                                if (MontureRecu.IdGaucheLentille != null)
                                {
                                    if (MontureRecu.IdGaucheLentille != 0)
                                    {
                                        Produit = proxy.GetAllProduitbyid(Convert.ToInt16(MontureRecu.IdGaucheLentille)).First();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                      
                                    }
                                    else
                                    {
                                        Produit = new SVC.Produit();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                       
                                    }
                                }
                                else
                                {
                                    Produit = new SVC.Produit();
                                    griddetail.DataContext = Produit;
                                    btnselection.IsEnabled = true;
                                }
                                this.Title = "Sélection de la lentille GAUCHE";
                                break;
                          
                        }
                    }
            }
                callbackrecu.InsertLentilleCallbackevent += new ICallback.CallbackEventHandler29(callbackrecu_Refresh);



                PatientDataGrid.SelectedItem = null;

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                HandleProxy();
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionLentille(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionLentille(HandleProxy));
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
                        this.Close();
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        this.Close();
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
        private void btnselection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             
                    switch (verrelocalisation)
                    {
                        case 1:
                            SelectedMonture.IdDroiteLentille = Produit.Id;
                            SelectedMonture.DroiteLentilleDesignation = Produit.design;
                            SelectedMonture.DroitPrixLentille = Convert.ToDecimal(txtPrix.Text);
                            SelectedMonture.DroiteStatutLentille = 1;
                            SelectedMonture.DroitQuantiteLentille = 1;
                            
                            break;
                        case 2:
                            SelectedMonture.IdGaucheLentille = Produit.Id;
                            SelectedMonture.GaucheLentilleDesignation = Produit.design;
                            SelectedMonture.GauchePrixLentille = Convert.ToDecimal(txtPrix.Text);
                            SelectedMonture.GaucheStatutLentille = 1;
                            SelectedMonture.GaucheQuantiteLentille = 1;
                            break;
                      
                    }
                    this.Close();

             
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void txtPrix_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
        private void FamilleCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.ItemsSource = (proxy.GetAllLentille().OrderBy(n => n.Design));

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
          
        
        

  /*      private void SphComboBox_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                if (SphComboBox.SelectedItem != null)
                {
                   
                    string filterValue = SphComboBox.Text;
                    if (!String.IsNullOrEmpty(filterValue))
                    {
                        PatientDataGrid.Columns[4].AutoFilterCondition = AutoFilterCondition.Contains;
                        PatientDataGrid.Columns[4].AutoFilterValue = filterValue;
                    }

                    else
                    {
                        PatientDataGrid.FilterString = "([Id]  >=0)";
                    }
                    PatientDataGrid.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        */
         

        private void btnstock_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SelectionProduitLentille cl = new SelectionProduitLentille(proxy, MemberUser, callback, verrelocalisation, SelectedMonture, interfacenew);
                this.Close();
                cl.Show();

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        

        private void btnCommander_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationCommande == true/* && Produit != null*/)
                {
                    AjouterCommande cl = new AjouterCommande(proxy, MemberUser, callback, null, Produit, CLIENTV);
                    cl.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        

       

       
        private void btnProduit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true)
                {
                    AjouterProduit cl = new AjouterProduit(proxy, null, MemberUser, callback);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PatientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (PatientDataGrid.SelectedItem != null)
                {

                    SVC.Lentille selectedverre = PatientDataGrid.SelectedItem as SVC.Lentille;

                    Produit = proxy.GetAllProduitbycab(selectedverre.cleproduit);
                    txtDesign.Text = selectedverre.Design;
                    txtPrix.Text = Convert.ToString(selectedverre.PrixVente);
                }
            }
            catch (Exception ex)
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

        private void txtRecherchefOURN_TextChanged(object sender, TextChangedEventArgs e)
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
                            return (p.Fournisseur == filter);
                        return (p.Fournisseur.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtType_TextChanged(object sender, TextChangedEventArgs e)
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
                            return (p.Marque == filter);
                        return (p.Marque.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtSph_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {


                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Lentille p = o as SVC.Lentille;
                        if (t.Name == "txtId")
                           
                        return (p.Sph.ToString() == filter);
                      
                        return (p.Sph.ToString().Contains(filter));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtCyl_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {


                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Lentille p = o as SVC.Lentille;
                        if (t.Name == "txtId")

                            return (p.Cyl.ToString() == filter);

                        return (p.Cyl.ToString().Contains(filter));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtCou_TextChanged(object sender, TextChangedEventArgs e)
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
                            return (p.Courbure == filter);
                        return (p.Courbure.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;

            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
