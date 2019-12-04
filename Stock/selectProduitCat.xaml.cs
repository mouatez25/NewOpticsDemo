
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
    /// Interaction logic for selectProduitCat.xaml
    /// </summary>
    public partial class selectProduitCat : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MemberUser;
        ICallback callback;

        SVC.Param selectedparam;
        int interfacenew;
        int verrelocalisation;
        bool selectionproduit = false;
        SVC.Monture SelectedMonture;
        SVC.Produit Produit;
        SVC.Produit ancienproduit;
        private delegate void FaultedInvokerSelectionProduitCat();
        SVC.ClientV CLIENTV;
        public selectProduitCat(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, int verrrerecu, SVC.Monture MontureRecu, int interfcerecu,SVC.ClientV clienrecu)
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
                CLIENTV = clienrecu;
                switch (verrelocalisation)
                {
                    case 1:

                        this.Title = "Sélection du verre DROIT";
                        break;
                    case 2:

                        this.Title = "Sélection du verre GAUCHE";
                        break;
                    case 3:

                        this.Title = "Sélection du verre DROIT";
                        break;
                    case 4:

                        this.Title = "Sélection du verre GAUCHE";

                        break;
                    case 5:

                        this.Title = "Sélection d'une monture";

                        break;
                    case 6:

                        this.Title = "Sélection d'une monture";

                        break;
                    case 7:

                        this.Title = "Sélection d'un accessoire";

                        break;
                    case 8:

                        this.Title = "Sélection d'un accessoire";

                        break;
                }
                if (verrrerecu == 1 || verrrerecu == 2 || verrrerecu == 3 || verrrerecu == 4)
                {
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                    FamilleCombo.ItemsSource = testmedecin;

                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 19).OrderBy(n => n.Id).ToList();
                    FamilleCombo.SelectedItem = tte.First();
                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == 19).OrderBy(n => n.design));
                }
                else
                {
                    if (verrrerecu == 5 || verrrerecu == 6)
                    {
                      //  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == 13).OrderBy(n => n.design));

                        List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                        FamilleCombo.ItemsSource = testmedecin;

                        List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 13).OrderBy(n => n.Id).ToList();
                        FamilleCombo.SelectedItem = tte.First();
                    }
                    else
                    {
                        List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                        FamilleCombo.ItemsSource = testmedecin;

                        List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 1).OrderBy(n => n.Id).ToList();
                        FamilleCombo.SelectedItem = tte.First();
                        PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == 1 ).OrderBy(n => n.design));
                    }
                }

                selectedparam = proxy.GetAllParamétre();
                ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                ListeMarqueCombo.SelectedIndex = -1;

                if (selectedparam.AffichPrixAchatVente == true)
                {
                    PatientDataGrid.Columns[2].Visibility = Visibility.Visible;
                }
                else
                {
                    PatientDataGrid.Columns[2].Visibility = Visibility.Collapsed;
                }
                if (selectedparam.ModiPrix == true)
                {
                    txtPrix.IsEnabled = true;
                }
                else
                {
                    txtPrix.IsEnabled = false;
                }
  



                /****************supplement******************/

                callbackrecu.InsertProduitCallbackEvent += new ICallback.CallbackEventHandler22(callbackrecu_Refresh);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertProduit e)
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
        public void AddRefresh(SVC.Produit listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM11 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Produit>;
                List<SVC.Produit> LISTITEM0 = LISTITEM11.ToList();

                if (oper == 1)
                {
                    LISTITEM0.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {


                        var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;
                        var index = LISTITEM0.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM0[index] = listMembershipOptic;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM0.Where(n => n.Id == listMembershipOptic.Id).First();
                            LISTITEM0.Remove(deleterendez);
                        }
                    }
                }










                PatientDataGrid.ItemsSource = LISTITEM0;


            }
            catch (Exception ex)
            {

            }
        }

        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduitCat(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduitCat(HandleProxy));
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
        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }


        }
        private void btnselection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectionproduit == true/* && interfacenew == 0*/)
                {
                    if (PatientDataGrid.SelectedItem != null)
                    {
                        SVC.Produit selectedprodf = PatientDataGrid.SelectedItem as SVC.Produit;
                        Produit = selectedprodf;
                        griddetail.DataContext = Produit;
                        selectionproduit = true;
                        btnselection.IsEnabled = true;

                        switch (verrelocalisation)
                        {
                            case 1:
                                SelectedMonture.IdDroiteVerreLoin = selectedprodf.Id;
                                SelectedMonture.DroiteVerreLoinDesignation = selectedprodf.design;
                                SelectedMonture.DroitPrixVerreLoin = selectedprodf.PrixVente;
                                SelectedMonture.DroiteStatutLoinVerre = 1;

                                break;
                            case 2:
                                SelectedMonture.IdGaucheVerreLoin = selectedprodf.Id;
                                SelectedMonture.GaucheVerreLoinDesignation = selectedprodf.design;
                                SelectedMonture.GauchePrixVerreLoin = selectedprodf.PrixVente;
                                SelectedMonture.GaucheStatutLoinVerre = 1;
                                break;
                            case 3:
                                SelectedMonture.IdDroiteVerrePres = selectedprodf.Id;
                                SelectedMonture.DroiteVerrePresDesignation = selectedprodf.design;
                                SelectedMonture.DroitPrixVerrePres = selectedprodf.PrixVente;
                                SelectedMonture.DroiteStatutPresVerre = 1;
                                break;
                            case 4:
                                SelectedMonture.IdGaucheVerrePres = selectedprodf.Id;
                                SelectedMonture.GaucheVerrePresDesignation = selectedprodf.design;
                                SelectedMonture.GauchePrixVerrePres = selectedprodf.PrixVente;
                                SelectedMonture.GaucheStatutPresVerre = 1;
                                break;
                            case 5:
                                SelectedMonture.IdMontureLoin = selectedprodf.Id;
                                SelectedMonture.MontureDesignationLoin = selectedprodf.design;
                                SelectedMonture.PrixMontureLoin = selectedprodf.PrixVente;
                                SelectedMonture.ModeVenteLoin = "Vente";
                                SelectedMonture.DroiteStatutLoinMonture = 1;
                                break;
                            case 6:
                                SelectedMonture.IdMonturePres = selectedprodf.Id;
                                SelectedMonture.MontureDesignationPres = selectedprodf.design;
                                SelectedMonture.PrixMonturePres = selectedprodf.PrixVente;
                                SelectedMonture.ModeVentePres = "Vente";
                                SelectedMonture.DroiteStatutPresMonture = 1;
                                break;
                            case 7:
                                SelectedMonture.IdAccessoires1 = selectedprodf.Id;
                                SelectedMonture.Accessoires1 = selectedprodf.design;
                                SelectedMonture.AccessoiresPrix1 = selectedprodf.PrixVente;
                                SelectedMonture.Accessoires1Statut = 1;
                                SelectedMonture.AccessoiresQuantite1 = 1;
                                break;
                            case 8:
                                SelectedMonture.IdAccessoires2 = selectedprodf.Id;
                                SelectedMonture.Accessoires2 = selectedprodf.design;
                                SelectedMonture.AccessoiresPrix2 = selectedprodf.PrixVente;
                                SelectedMonture.Accessoires2Statut = 1;
                                SelectedMonture.AccessoiresQuantite2 = 1;
                                break;
                        }
                        this.Close();

                    }
                    else
                    {
                        this.Close();
                    }

                }
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
                /*  switch (verrelocalisation)
                  {
                      case 1:
                          SelectedMonture.IdDroiteVerreLoin = ancienproduit.Id;
                          SelectedMonture.DroiteVerreLoinDesignation = ancienproduit.design;
                          SelectedMonture.DroitPrixVerreLoin = ancienproduit.privente;
                          break;
                      case 2:
                          SelectedMonture.IdGaucheVerreLoin = ancienproduit.Id;
                          SelectedMonture.GaucheVerreLoinDesignation = ancienproduit.design;
                          SelectedMonture.GauchePrixVerreLoin = ancienproduit.privente;

                          break;
                      case 3:
                          SelectedMonture.IdDroiteVerrePres = ancienproduit.Id;
                          SelectedMonture.DroiteVerrePresDesignation = ancienproduit.design;
                          SelectedMonture.DroitPrixVerrePres = ancienproduit.privente;
                          break;
                      case 4:
                          SelectedMonture.IdGaucheVerrePres = ancienproduit.Id;
                          SelectedMonture.GaucheVerrePresDesignation = ancienproduit.design;
                          SelectedMonture.GauchePrixVerrePres = ancienproduit.privente;
                          break;
                      case 5:
                          SelectedMonture.IdMontureLoin = ancienproduit.Id;
                          SelectedMonture.MontureDesignationLoin = ancienproduit.design;
                          SelectedMonture.PrixMontureLoin = ancienproduit.privente;
                          break;
                      case 6:
                          SelectedMonture.IdMonturePres = ancienproduit.Id;
                          SelectedMonture.MontureDesignationPres = ancienproduit.design;
                          SelectedMonture.PrixMonturePres = ancienproduit.privente;
                          break;
                      case 7:
                          SelectedMonture.IdAccessoires1 = ancienproduit.Id;
                          SelectedMonture.Accessoires1 = ancienproduit.design;
                          SelectedMonture.AccessoiresPrix1 = ancienproduit.privente;
                          break;
                      case 8:
                          SelectedMonture.IdAccessoires2 = ancienproduit.Id;
                          SelectedMonture.Accessoires2 = ancienproduit.design;
                          SelectedMonture.AccessoiresPrix2 = ancienproduit.privente;
                          break;
                  }*/
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
        private void PatientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Produit selectedprodf = PatientDataGrid.SelectedItem as SVC.Produit;
                    Produit = selectedprodf;
                    griddetail.DataContext = Produit;
                    selectionproduit = true;
                    btnselection.IsEnabled = true;

                    /*     switch (verrelocalisation)
                         {
                             case 1:
                                 SelectedMonture.IdDroiteVerreLoin = selectedprodf.Id;
                                 SelectedMonture.DroiteVerreLoinDesignation = selectedprodf.design;
                                 SelectedMonture.DroitPrixVerreLoin = selectedprodf.privente;
                                 break;
                             case 2:
                                 SelectedMonture.IdGaucheVerreLoin = selectedprodf.Id;
                                 SelectedMonture.GaucheVerreLoinDesignation = selectedprodf.design;
                                 SelectedMonture.GauchePrixVerreLoin = selectedprodf.privente;

                                 break;
                             case 3:
                                 SelectedMonture.IdDroiteVerrePres = selectedprodf.Id;
                                 SelectedMonture.DroiteVerrePresDesignation = selectedprodf.design;
                                 SelectedMonture.DroitPrixVerrePres = selectedprodf.privente;
                                 break;
                             case 4:
                                 SelectedMonture.IdGaucheVerrePres = selectedprodf.Id;
                                 SelectedMonture.GaucheVerrePresDesignation = selectedprodf.design;
                                 SelectedMonture.GauchePrixVerrePres = selectedprodf.privente;
                                 break;
                             case 5:
                                 SelectedMonture.IdMontureLoin = selectedprodf.Id;
                                 SelectedMonture.MontureDesignationLoin = selectedprodf.design;
                                 SelectedMonture.PrixMontureLoin = selectedprodf.privente;
                                 SelectedMonture.ModeVenteLoin = "Vente";
                                 break;
                             case 6:
                                 SelectedMonture.IdMonturePres = selectedprodf.Id;
                                 SelectedMonture.MontureDesignationPres = selectedprodf.design;
                                 SelectedMonture.PrixMonturePres = selectedprodf.privente;
                                 SelectedMonture.ModeVentePres = "Vente";
                                 break;
                             case 7:
                                 SelectedMonture.IdAccessoires1 = selectedprodf.Id;
                                 SelectedMonture.Accessoires1 = selectedprodf.design;
                                 SelectedMonture.AccessoiresPrix1 = selectedprodf.privente;

                                 break;
                             case 8:
                                 SelectedMonture.IdAccessoires2 = selectedprodf.Id;
                                 SelectedMonture.Accessoires2 = selectedprodf.design;
                                 SelectedMonture.AccessoiresPrix2 = selectedprodf.privente;
                                 break;
                         }*/


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void FamilleCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
           try
            {
                ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                ListeMarqueCombo.SelectedIndex = -1;
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                FamilleCombo.SelectedIndex = -1;
                PatientDataGrid.ItemsSource = (proxy.GetAllProduit().OrderBy(n => n.design));
                txtRecherche.Text = "";

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }
       
      
        private void PatientDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /* try
             {
                 if (PatientDataGrid.SelectedItem != null)
                 {
                     SVC.Produit pp = PatientDataGrid.SelectedItem as SVC.Produit;
                     if (pp.IdFamille == 10)
                     {
                         bool exsite = proxy.GetAllLentillebycode(pp.cab).Any();
                         if (exsite == true)
                         {
                             SVC.Lentille ll = proxy.GetAllLentillebycode(pp.cab).First();
                             Lentille cl = new Lentille(proxy, ll);
                             cl.Show();
                         }
                     }
                     else
                     {
                         if (pp.IdFamille == 19)
                         {
                             bool exsite = proxy.GetAllVerrebycode(pp.cab).Any();
                             if (exsite == true)
                             {
                                 SVC.Verre ll = proxy.GetAllVerrebycode(pp.cab).First();
                                 Verre cl = new Verre(proxy, ll);
                                 cl.Show();
                             }
                         }
                     }
                 }
                 else
                 {

                 }

             }
             catch (Exception ex)
             {
                 MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

             }*/
        }

        private void btnstock_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SelectionProduit cl = new SelectionProduit(proxy, MemberUser, callback, verrelocalisation, SelectedMonture, interfacenew);
                this.Close();
                cl.Show();

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btncommande_Click(object sender, RoutedEventArgs e)
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

       

        private void ListeMarqueCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                ListeMarqueCombo.SelectedIndex = -1;
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                FamilleCombo.SelectedIndex = -1;
                PatientDataGrid.ItemsSource = (proxy.GetAllProduit().OrderBy(n => n.design));
                txtRecherche.Text = "";

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
                        SVC.Produit p = o as SVC.Produit;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
                    };
                }
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
               if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem != null)
                {
                    SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                    SVC.Marque marqueselected = ListeMarqueCombo.SelectedItem as SVC.Marque;

                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == t.Id && n.IdMarque == marqueselected.Id).OrderBy(n => n.design));

                }
                else
                {
                    if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem == null)
                    {
                        SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                        PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == t.Id).OrderBy(n => n.design));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ListeMarqueCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem != null)
                {
                    SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                    SVC.Marque marqueselected = ListeMarqueCombo.SelectedItem as SVC.Marque;

                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == t.Id && n.IdMarque == marqueselected.Id).OrderBy(n => n.design));

                }
                else
                {
                    if (ListeMarqueCombo.SelectedItem != null && FamilleCombo.SelectedItem == null)
                    {
                        SVC.Marque t = ListeMarqueCombo.SelectedItem as SVC.Marque;

                        PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdMarque == t.Id).OrderBy(n => n.design));


                    }
                    else
                    {
                        if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem == null)
                        {
                            SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                           

                            PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == t.Id ).OrderBy(n => n.design));

                        }
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
 
