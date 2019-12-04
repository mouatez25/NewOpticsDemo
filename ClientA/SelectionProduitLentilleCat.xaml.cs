﻿
using NewOptics.Administrateur;
using NewOptics.Commande;
using NewOptics.Stock;
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

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for SelectionProduitLentilleCat.xaml
    /// </summary>
    public partial class SelectionProduitLentilleCat :Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MemberUser;
        ICallback callback;
        SVC.Produit Produit;
        SVC.Produit ancienproduit;
        SVC.Param selectedparam;
        int interfacenew;
        int Lentillelocalisation;
        bool selectionproduit = false;
        SVC.LentilleClient SelectedLentille;
        private delegate void FaultedInvokerSelectionProduitLentilleCat();
        SVC.ClientV CLIENTV;
        public SelectionProduitLentilleCat(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, int verrrerecu, SVC.LentilleClient LentilleRecu, int interfcerecu,SVC.ClientV clientrecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                callback = callbackrecu;

                MemberUser = memberrecu;

                SelectedLentille = LentilleRecu;
                Lentillelocalisation = verrrerecu;
                CLIENTV = clientrecu;
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

                ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                ListeMarqueCombo.SelectedIndex = -1;
                if (verrrerecu == 1 || verrrerecu == 2)
                {
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                    FamilleCombo.ItemsSource = testmedecin;

                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 10).OrderBy(n => n.Id).ToList();
                    FamilleCombo.SelectedItem = tte.First();
                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == 10 ).OrderBy(n => n.design));
                }
                else
                {

                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1).ToList();
                    FamilleCombo.ItemsSource = testmedecin;

                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == 1).OrderBy(n => n.Id).ToList();
                    FamilleCombo.SelectedItem = tte.First();
                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == 1 ).OrderBy(n => n.design));

                }

                selectedparam = proxy.GetAllParamétre();


                
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
                /*  switch (Lentillelocalisation)
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
                  }*/




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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduitLentilleCat(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionProduitLentilleCat(HandleProxy));
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
                        SVC.Produit selectedprodf = PatientDataGrid.SelectedItem as SVC.Produit;
                        Produit = selectedprodf;
                        griddetail.DataContext = Produit;
                        selectionproduit = true;
                        btnselection.IsEnabled = true;

                        switch (Lentillelocalisation)
                        {
                            case 1:
                                SelectedLentille.IdDroiteLentille = selectedprodf.Id;
                                SelectedLentille.DroiteLentilleDesignation = selectedprodf.design;
                                SelectedLentille.DroitPrixLentille = selectedprodf.PrixVente;
                                SelectedLentille.DroiteStatutLentille = 1;
                                SelectedLentille.DroitQuantiteLentille = 1;
                                break;
                            case 2:
                                SelectedLentille.IdGaucheLentille = selectedprodf.Id;
                                SelectedLentille.GaucheLentilleDesignation = selectedprodf.design;
                                SelectedLentille.GauchePrixLentille = selectedprodf.PrixVente;
                                SelectedLentille.GaucheStatutLentille = 1;
                                SelectedLentille.GaucheQuantiteLentille = 1;
                                break;
                            case 7:
                                SelectedLentille.IdAccessoires1 = selectedprodf.Id;
                                SelectedLentille.Accessoires1 = selectedprodf.design;
                                SelectedLentille.AccessoiresPrix1 = selectedprodf.PrixVente;
                                SelectedLentille.DroiteStatutAccessoires1 = 1;
                                SelectedLentille.AccessoiresQuantite1 = 1;
                                break;
                            case 8:
                                SelectedLentille.IdAccessoires2 = selectedprodf.Id;
                                SelectedLentille.Accessoires2 = selectedprodf.design;
                                SelectedLentille.AccessoiresPrix2 = selectedprodf.PrixVente;
                                SelectedLentille.DroiteStatutAccessoires2 = 1;
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
                    SVC.Produit selectedprodf = PatientDataGrid.SelectedItem as SVC.Produit;
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
                ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                ListeMarqueCombo.SelectedIndex = -1;
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                FamilleCombo.SelectedIndex = -1;
                txtRecherche.Text = "";
                PatientDataGrid.ItemsSource = proxy.GetAllProduit().OrderBy(n => n.design);

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

    
         
        
        private void PatientDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          /*  try
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

            }*/
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

        private void btnstock_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SelectionProduitLentille cl = new SelectionProduitLentille(proxy, MemberUser, callback, Lentillelocalisation, SelectedLentille, interfacenew);
                this.Close();
                cl.Show();

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
                txtRecherche.Text = "";
                PatientDataGrid.ItemsSource =  proxy.GetAllProduit().OrderBy(n => n.design);

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

        private void ListeMarqueCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListeMarqueCombo.SelectedItem != null && FamilleCombo.SelectedItem == null)
                {
                    SVC.Marque selectedmarque = ListeMarqueCombo.SelectedItem as SVC.Marque;
                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdMarque == selectedmarque.Id).OrderBy(n => n.design));

                }
                else
                {
                    if (ListeMarqueCombo.SelectedItem != null && FamilleCombo.SelectedItem != null)
                    {
                        SVC.Marque selectedmarque = ListeMarqueCombo.SelectedItem as SVC.Marque;
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdMarque == selectedmarque.Id && n.IdFamille == selectedfamille.Id).OrderBy(n => n.design));
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
                if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem == null)
                {
                    SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                    PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == t.Id).OrderBy(n => n.design));


                }
                else
                {
                    if (FamilleCombo.SelectedItem != null && ListeMarqueCombo.SelectedItem != null)
                    {
                        SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        SVC.Marque tmarque = ListeMarqueCombo.SelectedItem as SVC.Marque;
                        PatientDataGrid.ItemsSource = (proxy.GetAllProduit().Where(n => n.IdFamille == t.Id && n.IdMarque == tmarque.Id).OrderBy(n => n.design));


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
