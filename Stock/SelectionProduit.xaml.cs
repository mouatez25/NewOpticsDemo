
using NewOptics.Administrateur;
using System;
using System.Collections;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for SelectionProduit.xaml
    /// </summary>
    public partial class SelectionProduit : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MemberUser;
        ICallback callback;
        SVC.Prodf Produit;
        SVC.Prodf ancienproduit;
        SVC.Param selectedparam;
        int interfacenew;
        int verrelocalisation;
        bool selectionproduit = false;
        SVC.Monture SelectedMonture;
  
        private delegate void FaultedInvokerSelectionProduit();
        public SelectionProduit(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu,int verrrerecu, SVC.Monture MontureRecu,int interfcerecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                callback = callbackrecu;



                SelectedMonture = MontureRecu;
                verrelocalisation = verrrerecu;
                CompteComboBox.SelectedIndex = 0;
                interfacenew = interfcerecu;
                ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                ListeMarqueCombo.SelectedIndex = -1;
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
                if (verrrerecu== 1 || verrrerecu== 2 || verrrerecu== 3 || verrrerecu==4)
                {
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                    FamilleCombo.ItemsSource = testmedecin;

                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 19).OrderBy(n => n.Id).ToList();
                    FamilleCombo.SelectedItem = tte.First();
                    PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == 19 && n.quantite > 0).OrderBy(n => n.design));
                }
                else
                {
                    if (verrrerecu == 5 || verrrerecu == 6)
                    {
                        List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                        FamilleCombo.ItemsSource = testmedecin;

                        List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 13).OrderBy(n => n.Id).ToList();
                        FamilleCombo.SelectedItem = tte.First();
                        PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == 13 && n.quantite > 0).OrderBy(n => n.design));
                    }
                    else
                    {
                        List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                        FamilleCombo.ItemsSource = testmedecin;

                        List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 1).OrderBy(n => n.Id).ToList();
                        FamilleCombo.SelectedItem = tte.First();
                        PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == 1 && n.quantite > 0).OrderBy(n => n.design));
                    }
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
                if (selectedparam.ModiPrix== true)
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
                switch (verrelocalisation)
                {
                    case 1:
                        if(MontureRecu.IdDroiteVerreLoin!=0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection du verre DROIT";
                        break;
                    case 2:
                        if (MontureRecu.IdGaucheVerreLoin != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdGaucheVerreLoin)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdGaucheVerreLoin)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection du verre GAUCHE";
                        break;
                    case 3:
                        if (MontureRecu.IdDroiteVerrePres != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerrePres)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdDroiteVerrePres)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection du verre DROIT";
                        break;
                    case 4:
                        if (MontureRecu.IdGaucheVerrePres != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdGaucheVerrePres)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdGaucheVerrePres)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection du verre GAUCHE";

                        break;
                    case 5:
                        if (MontureRecu.IdMontureLoin != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdMontureLoin)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdMontureLoin)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection d'une monture";

                        break;
                    case 6:
                        if (MontureRecu.IdMonturePres != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdMontureLoin)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdMontureLoin)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection d'une monture";

                        break;
                    case 7:
                        if (MontureRecu.IdAccessoires1 != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdAccessoires1)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdAccessoires1)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection d'un accessoire";

                        break;
                    case 8:
                        if (MontureRecu.IdAccessoires2 != 0)
                        {
                            ancienproduit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdAccessoires2)).First();
                            Produit = proxy.GetAllProdfbyfiche(Convert.ToInt16(MontureRecu.IdAccessoires2)).First();

                            griddetail.DataContext = Produit;
                            btnselection.IsEnabled = true;
                        }
                        this.Title = "Sélection d'un accessoire";

                        break;
                }



                /****************supplement******************/
 

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch(Exception ex)
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduit(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduit(HandleProxy));
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

            }catch(Exception ex)
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
                        SVC.Prodf selectedprodf = PatientDataGrid.SelectedItem as SVC.Prodf;
                        Produit = selectedprodf;
                        griddetail.DataContext = Produit;
                        selectionproduit = true;
                        btnselection.IsEnabled = true;

                        switch (verrelocalisation)
                        {
                            case 1:
                                SelectedMonture.IdDroiteVerreLoin = selectedprodf.Id;
                                SelectedMonture.DroiteVerreLoinDesignation = selectedprodf.design;
                                SelectedMonture.DroitPrixVerreLoin = selectedprodf.privente;
                                SelectedMonture.DroiteStatutLoinVerre= 0;
                              
                                break;
                            case 2:
                                SelectedMonture.IdGaucheVerreLoin = selectedprodf.Id;
                                SelectedMonture.GaucheVerreLoinDesignation = selectedprodf.design;
                                SelectedMonture.GauchePrixVerreLoin = selectedprodf.privente;
                                SelectedMonture.GaucheStatutLoinVerre= 0;
                                break;
                            case 3:
                                SelectedMonture.IdDroiteVerrePres = selectedprodf.Id;
                                SelectedMonture.DroiteVerrePresDesignation = selectedprodf.design;
                                SelectedMonture.DroitPrixVerrePres = selectedprodf.privente;
                                SelectedMonture.DroiteStatutPresVerre = 0;
                                break;
                            case 4:
                                SelectedMonture.IdGaucheVerrePres = selectedprodf.Id;
                                SelectedMonture.GaucheVerrePresDesignation = selectedprodf.design;
                                SelectedMonture.GauchePrixVerrePres = selectedprodf.privente;
                                SelectedMonture.GaucheStatutPresVerre = 0;
                                break;
                            case 5:
                                SelectedMonture.IdMontureLoin = selectedprodf.Id;
                                SelectedMonture.MontureDesignationLoin = selectedprodf.design;
                                SelectedMonture.PrixMontureLoin = selectedprodf.privente;
                                SelectedMonture.ModeVenteLoin = "Vente";
                                SelectedMonture.DroiteStatutLoinMonture = 0;
                                break;
                            case 6:
                                SelectedMonture.IdMonturePres = selectedprodf.Id;
                                SelectedMonture.MontureDesignationPres = selectedprodf.design;
                                SelectedMonture.PrixMonturePres = selectedprodf.privente;
                                SelectedMonture.ModeVentePres = "Vente";
                                SelectedMonture.DroiteStatutPresMonture = 0;
                                break;
                            case 7:
                                SelectedMonture.IdAccessoires1 = selectedprodf.Id;
                                SelectedMonture.Accessoires1 = selectedprodf.design;
                                SelectedMonture.AccessoiresPrix1 = selectedprodf.privente;
                                SelectedMonture.Accessoires1Statut = 0;
                                SelectedMonture.AccessoiresQuantite1 = 1;
                                break;
                            case 8:
                                SelectedMonture.IdAccessoires2 = selectedprodf.Id;
                                SelectedMonture.Accessoires2 = selectedprodf.design;
                                SelectedMonture.AccessoiresPrix2 = selectedprodf.privente;
                                SelectedMonture.Accessoires2Statut = 0;
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
            }catch(Exception ex)
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
            }catch(Exception ex)
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
                PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(n => n.quantite > 0).OrderBy(n=>n.design);
                txtRecherche.Text = "";
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

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            } 
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            
        }

       
        private void ListeMarqueCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            { 
            ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
            ListeMarqueCombo.SelectedIndex = -1;

                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                FamilleCombo.SelectedIndex = -1;
                PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(n => n.quantite > 0).OrderBy(n => n.design); ;
                txtRecherche.Text = "";
            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void FamilleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem == null)
                {
                    SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                    PatientDataGrid.ItemsSource = (proxy.GetAllProdf().Where(n => n.IdFamille == t.Id && n.quantite > 0).OrderBy(n => n.design));


                }
                else
                {
                    if (ListeMarqueCombo.SelectedItem != null && FamilleCombo.SelectedItem != null)
                    {
                        SVC.Marque selectedmarque = ListeMarqueCombo.SelectedItem as SVC.Marque;
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        List<SVC.Prodf> listprodffiltre = new List<SVC.Prodf>();
                        var listprodf = proxy.GetAllProdf().Where(n => n.quantite > 0);

                        List<SVC.Produit> listproduit = proxy.GetAllProduit().Where(n => n.IdMarque == selectedmarque.Id && n.IdFamille == selectedfamille.Id).ToList();
                        foreach (var itemproduit in listproduit)
                        {
                            var existe = listprodf.Any(n => n.cp == itemproduit.Id);
                            if (existe)
                            {
                                var prodf = listprodf.Where(n => n.cp == itemproduit.Id);
                                foreach (var itemprodf in prodf)
                                {
                                    listprodffiltre.Add(itemprodf);
                                }
                            }
                        }

                        PatientDataGrid.ItemsSource = listprodffiltre.OrderBy(n => n.design); ;
                    }
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

        private void ListeMarqueCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListeMarqueCombo.SelectedItem != null && FamilleCombo.SelectedItem == null)
                {
                    SVC.Marque selectedmarque = ListeMarqueCombo.SelectedItem as SVC.Marque;
                    List<SVC.Prodf> listprodffiltre = new List<SVC.Prodf>();
                    var listprodf = proxy.GetAllProdf().Where(n => n.quantite > 0);

                    List<SVC.Produit> listproduit = proxy.GetAllProduit().Where(n => n.IdMarque == selectedmarque.Id).ToList();
                    foreach (var itemproduit in listproduit)
                    {
                        var existe = listprodf.Any(n => n.cp == itemproduit.Id);
                        if (existe)
                        {
                            var prodf = listprodf.Where(n => n.cp == itemproduit.Id);
                            foreach (var itemprodf in prodf)
                            {
                                listprodffiltre.Add(itemprodf);
                            }
                        }
                    }

                    PatientDataGrid.ItemsSource = listprodffiltre.OrderBy(n => n.design); ;
                }
                else
                {
                    if (ListeMarqueCombo.SelectedItem != null && FamilleCombo.SelectedItem != null)
                    {
                        SVC.Marque selectedmarque = ListeMarqueCombo.SelectedItem as SVC.Marque;
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        List<SVC.Prodf> listprodffiltre = new List<SVC.Prodf>();
                        var listprodf = proxy.GetAllProdf().Where(n => n.quantite > 0);

                        List<SVC.Produit> listproduit = proxy.GetAllProduit().Where(n => n.IdMarque == selectedmarque.Id && n.IdFamille == selectedfamille.Id).ToList();
                        foreach (var itemproduit in listproduit)
                        {
                            var existe = listprodf.Any(n => n.cp == itemproduit.Id);
                            if (existe)
                            {
                                var prodf = listprodf.Where(n => n.cp == itemproduit.Id);
                                foreach (var itemprodf in prodf)
                                {
                                    listprodffiltre.Add(itemprodf);
                                }
                            }
                        }

                        PatientDataGrid.ItemsSource = listprodffiltre.OrderBy(n => n.design);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void CompteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
    }
}
