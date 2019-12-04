
using NewOptics.Administrateur;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for SelectionProduitLentille.xaml
    /// </summary>
    public partial class SelectionProduitLentille :Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MemberUser;
        ICallback callback;
        SVC.Prodf Produit;
        SVC.Prodf ancienproduit;
        SVC.Param selectedparam;
        int interfacenew;
        int Lentillelocalisation;
        bool selectionproduit = false;
        SVC.LentilleClient SelectedLentille;

        private delegate void FaultedInvokerSelectionProduitLentille();
        public SelectionProduitLentille(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, int verrrerecu, SVC.LentilleClient LentilleRecu, int interfcerecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                callback = callbackrecu;

                MemberUser = memberrecu;

                SelectedLentille = LentilleRecu;
                Lentillelocalisation = verrrerecu;
                CompteComboBox.SelectedIndex = 0;
                interfacenew = interfcerecu;
                switch (Lentillelocalisation)
                {
                    case 1:
                         this.Title = "Sélection de la lentille DROITE";
                        break;
                    case 2:
                       
                        this.Title = "Sélection de la lentille GAUCHE";
                        break;

                    case 7:
                       
                        this.Title = "Sélection d'un accessoire";

                        break;
                    case 8:
                      
                        this.Title = "Sélection d'un accessoire";

                        break;
                }


                if (verrrerecu == 1 || verrrerecu == 2 )
                {
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                    FamilleCombo.ItemsSource = testmedecin;

                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 10).OrderBy(n => n.Id).ToList();
                    FamilleCombo.SelectedItem = tte.First();
                    PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == 10 && n.quantite > 0).OrderBy(n => n.design));
                }
                else
                {

                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                    FamilleCombo.ItemsSource = testmedecin;

                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 1).OrderBy(n => n.Id).ToList();
                    FamilleCombo.SelectedItem = tte.First();
                    PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == 1 && n.quantite > 0).OrderBy(n => n.design));

                }

                selectedparam = proxy.GetAllParamétre();


                if (selectedparam.ModifTarif == true)
                {
                    CompteComboBox.IsEnabled = true;
                }
                else
                {
                    CompteComboBox.IsEnabled = false;
                }
                if (selectedparam.AffichPrixAchatVente == true)
                {
                    PatientDataGrid.Columns[3].Visibility = Visibility.Visible;
                }
                else
                {
                    PatientDataGrid.Columns[3].Visibility = Visibility.Collapsed;
                }
                if (selectedparam.ModiPrix == true)
                {
                    txtPrix.IsEnabled = true;
                }
                else
                {
                    txtPrix.IsEnabled = false;
                }
                /*   if(interfacenew==1 && MontureRecu.IdDroiteVerreLoin!=0)
                   {
                       ancienproduit=proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();
                       Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();

                       griddetail.DataContext = Produit;
                       btnselection.IsEnabled = true;
                   }
                   else
                   {
                       if (interfacenew == 1 && MontureRecu.IdDroiteVerreLoin != 0)
                       {
                           ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();
                           Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();

                           griddetail.DataContext = Produit;
                           btnselection.IsEnabled = true;
                       }
                   }*/
                switch (Lentillelocalisation)
                {
                    case 1:
                        if (LentilleRecu.IdDroiteLentille != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdDroiteLentille)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdDroiteLentille)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection de la lentille DROITE";
                        break;
                    case 2:
                        if (LentilleRecu.IdGaucheLentille != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdGaucheLentille)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdGaucheLentille)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection de la lentille GAUCHE";
                        break;
                  
                    case 7:
                        if (LentilleRecu.IdAccessoires1 != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdAccessoires1)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdAccessoires1)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection d'un accessoire";

                        break;
                    case 8:
                        if (LentilleRecu.IdAccessoires2 != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdAccessoires2)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(LentilleRecu.IdAccessoires2)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection d'un accessoire";

                        break;
                }






                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduitLentille(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduitLentille(HandleProxy));
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
                try
                {
                    if (PatientDataGrid.SelectedItem != null)
                    {
                        SVC.Prodf selectedprodf = PatientDataGrid.SelectedItem as SVC.Prodf;
                        Produit = selectedprodf;
                        griddetail.DataContext = Produit;
                        selectionproduit = true;
                        btnselection.IsEnabled = true;

                        switch (Lentillelocalisation)
                        {
                            case 1:
                                SelectedLentille.IdDroiteLentille = selectedprodf.Id;
                                SelectedLentille.DroiteLentilleDesignation = selectedprodf.design;
                                SelectedLentille.DroitPrixLentille = selectedprodf.privente;
                                SelectedLentille.DroiteStatutLentille = 0;
                                SelectedLentille.DroitQuantiteLentille = 1;
                                break;
                            case 2:
                                SelectedLentille.IdGaucheLentille = selectedprodf.Id;
                                SelectedLentille.GaucheLentilleDesignation = selectedprodf.design;
                                SelectedLentille.GauchePrixLentille = selectedprodf.privente;
                                SelectedLentille.GaucheStatutLentille = 0;
                                SelectedLentille.GaucheQuantiteLentille = 1;
                                break;
                            case 7:
                                SelectedLentille.IdAccessoires1 = selectedprodf.Id;
                                SelectedLentille.Accessoires1 = selectedprodf.design;
                                SelectedLentille.AccessoiresPrix1 = selectedprodf.privente;
                                SelectedLentille.DroiteStatutAccessoires1 = 0;
                                SelectedLentille.AccessoiresQuantite1 = 1;
                                break;
                            case 8:
                                SelectedLentille.IdAccessoires2 = selectedprodf.Id;
                                SelectedLentille.Accessoires2 = selectedprodf.design;
                                SelectedLentille.AccessoiresPrix2 = selectedprodf.privente;
                                SelectedLentille.DroiteStatutAccessoires2 = 0;
                                SelectedLentille.AccessoiresQuantite2 = 1;
                                break;

                        }


                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

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
               /* switch (Lentillelocalisation)
                {
                    case 1:
                        SelectedLentille.IdDroiteLentille = ancienproduit.Id;
                        SelectedLentille.DroiteLentilleDesignation = ancienproduit.design;
                        SelectedLentille.DroitPrixLentille = ancienproduit.privente;
                      
                        break;
                    case 2:
                        SelectedLentille.IdGaucheLentille = ancienproduit.Id;
                        SelectedLentille.GaucheLentilleDesignation = ancienproduit.design;
                        SelectedLentille.GauchePrixLentille = ancienproduit.privente;
                      

                        break;
                   
                  
                    case 7:
                        SelectedLentille.IdAccessoires1 = ancienproduit.Id;
                        SelectedLentille.Accessoires1 = ancienproduit.design;
                        SelectedLentille.AccessoiresPrix1 = ancienproduit.privente;
                      
                        break;
                    case 8:
                        SelectedLentille.IdAccessoires2 = ancienproduit.Id;
                        SelectedLentille.Accessoires2 = ancienproduit.design;
                        SelectedLentille.AccessoiresPrix2 = ancienproduit.privente;
                       
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
                    SVC.Prodf selectedprodf = PatientDataGrid.SelectedItem as SVC.Prodf;
                    Produit = selectedprodf;
                    griddetail.DataContext = Produit;
                    selectionproduit = true;
                    btnselection.IsEnabled = true;

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
                PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.quantite > 0).OrderBy(n => n.design));
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);

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
                if(PatientDataGrid.SelectedItem!=null)
                {
                    SVC.Prodf pp = PatientDataGrid.SelectedItem as SVC.Prodf;
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

            }catch (Exception ex)
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
                        SVC.Prodf p = o as SVC.Prodf;
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

        private void CompteComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {

                string ValueCompte = "";
                if (CompteComboBox.SelectedIndex >= 0)
                {

                    var test = PatientDataGrid.ItemsSource as IEnumerable;



                    ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                    switch (ValueCompte)
                    {
                        case "Prixa":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixa;
                            }

                            break;
                        case "Prixb":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixb;
                            }
                            break;

                        case "Prixc":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixc;
                            }
                            break;

                        default:


                            break;
                    }


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
                if (FamilleCombo.SelectedItem != null)
                {
                    SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                    PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == t.Id).OrderBy(n => n.design));


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}

