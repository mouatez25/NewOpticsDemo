
using Microsoft.Win32;
using NewOptics.Administrateur;
using NewOptics.SVC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
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
    /// Interaction logic for AjouterProduit.xaml
    /// </summary>
    public partial class AjouterCatalogueVerre : Window
    {
        SVC.Produit special;
        SVC.ServiceCliniqueClient proxy;

        private delegate void FaultedInvokerAjouterCatalogueVerre();
        SVC.MembershipOptic MembershipOptic;
        string title;
        System.Drawing.Brush titlebrush;
        int créationOrdonnace = 0;
        ICallback callback;
        OpenFileDialog op = new OpenFileDialog();
        bool imageok = false;
        string serverfilepath, filepath;
        List<SVC.Tcab> listcodeabarre;
        /**********Verre*************/
        SVC.Verre CatalogueVerre;
        SVC.TarifVerre TarifVerre;
        List<string> ListDiam;
        List<string> ListDiamLentille;
        List<string> ListCourbureLentille;
        int interfacecatalogue = 0;
        bool tarifexiste = false;
        bool déjaverre = false;
        List<SVC.TarifVerre> listtarif = new List<TarifVerre>();
        List<SVC.TarifVerre> listtarifanciennelist = new List<TarifVerre>();
        /***************Lentille*************************/
        SVC.Lentille CatalogueLentille;
        bool déjalentille = false;
        List<SVC.Verre> verreajouter;
        List<SVC.Produit> produitajouter;
        public AjouterCatalogueVerre(SVC.ServiceCliniqueClient proxyrecu, SVC.Produit spécialtiérecu, SVC.MembershipOptic membershirecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                special = spécialtiérecu;
                MembershipOptic = membershirecu;
                callback = callbackrecu;


               /* if (special != null)
                {
                    FournVousGrid.DataContext = special;
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1).ToList();
                    ListeDciCombo.ItemsSource = testmedecin;
                    if (special.IdFamille != 0)
                    {
                        List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == special.IdFamille).OrderBy(n => n.Id).ToList();
                        ListeDciCombo.SelectedItem = tte.First();
                    }
                    List<SVC.Marque> testmarque = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit).ToList();
                    ListeMarqueCombo.ItemsSource = testmarque;

                    if (special.IdMarque != 0)
                    {
                        List<SVC.Marque> tte = testmarque.Where(n => n.Id == special.IdMarque).OrderBy(n => n.Id).ToList();
                        ListeMarqueCombo.SelectedItem = tte.First();
                    }
                    if (special.IdFamille == 19)
                    {
                        déjaverre = true;
                        // CatalogueVerre = proxy.GetAllVerrebycode(special.clecab).First();
                        CatalogueVerre = proxy.GetAllVerrebycode(special.cab).First();
                        txtRef.IsEnabled = true;
                        GridVerre.DataContext = CatalogueVerre;
                        if (CatalogueVerre.LimiteConnu == true)
                        {
                            Limit1.IsChecked = false;
                            Limit2.IsChecked = true;
                        }
                        else
                        {
                            Limit1.IsChecked = true;
                            Limit2.IsChecked = false;
                        }
                        txtdiam.Visibility = Visibility.Collapsed;
                        switch (CatalogueVerre.Matière)
                        {
                            case "Minéral":
                                radMat1.IsChecked = true;
                                break;
                            case "Organique":
                                radMat2.IsChecked = true;
                                break;
                            case "Polycarbonate":
                                radMat3.IsChecked = true;
                                break;
                        }

                        interfacecatalogue = 1;
                        CATitem.IsEnabled = true;
                        CATitem.Visibility = Visibility.Visible;

                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = CatalogueVerre.IndiceVerre,
                        };
                        CdompteComboBox.Items.Add(item);

                        ComboBoxItem item1 = new ComboBoxItem
                        {
                            Content = CatalogueVerre.Surface,
                        };
                        SurfaceComboBox.Items.Add(item1);

                        ComboBoxItem item2 = new ComboBoxItem
                        {
                            Content = CatalogueVerre.TypeVerre,
                        };
                        TypeVerreComboBox.Items.Add(item2);
                        bool existe = proxy.GetAllTarifVerre().Any(n => n.Cletarif == CatalogueVerre.Cletarif);
                        if (existe == true)
                        {
                            listtarif = proxy.GetAllTarifVerrebycode(CatalogueVerre.Cletarif);
                            listtarifanciennelist = proxy.GetAllTarifVerrebycode(CatalogueVerre.Cletarif);
                            tarifexiste = true;
                            string line = CatalogueVerre.Diamètre.Trim();
                            ListDiam = line.Split(',').ToList();
                            DiamComboBox.ItemsSource = ListDiam;
                            DiamComboBox.DataContext = ListDiam;
                            PrixComboBox.IsEnabled = true;
                            DiamComboBox.IsEnabled = true;

                        }
                        else
                        {
                            tarifexiste = false;
                            PrixComboBox.IsEnabled = false;
                            DiamComboBox.IsEnabled = false;
                        }




                    }
                    else
                    {
                        if (special.IdFamille == 10)
                        {
                            déjalentille = true;
                            CatalogueLentille = proxy.GetAllLentillebycode(special.cab).First();
                            //  CatalogueLentille = proxy.GetAllLentillebycode(special.clecab).First();
                            txtdiamlentille.Visibility = Visibility.Collapsed;
                            GridLentille.DataContext = CatalogueLentille;
                            interfacecatalogue = 2;
                            LENTitem.IsEnabled = true;
                            LENTitem.Visibility = Visibility.Visible;
                            if (CatalogueLentille.LimiteConnu == true)
                            {
                                LimitLentille1.IsChecked = false;
                                LimitLentille2.IsChecked = true;
                            }
                            else
                            {
                                LimitLentille1.IsChecked = true;
                                LimitLentille2.IsChecked = false;
                            }

                            ComboBoxItem item = new ComboBoxItem
                            {
                                Content = CatalogueLentille.Packaging,
                            };
                            TypePackagingComboBox.Items.Add(item);
                            ComboBoxItem item1 = new ComboBoxItem
                            {
                                Content = CatalogueLentille.TypeLentille,
                            };
                            TypeLentilleComboBox.Items.Add(item1);
                            ComboBoxItem item3 = new ComboBoxItem
                            {
                                Content = CatalogueLentille.Durrée,
                            };
                            TypeDuréeComboBox.Items.Add(item3);
                            ComboBoxItem item4 = new ComboBoxItem
                            {
                                Content = CatalogueLentille.CodeLPP,
                            };
                            TypeLPPComboBox.Items.Add(item4);
                            ComboBoxItem item5 = new ComboBoxItem
                            {
                                Content = CatalogueLentille.Matière,
                            };
                            MatièreComboBox.Items.Add(item5);
                        }
                        else
                        {
                            déjaverre = false;
                            déjalentille = false;
                        }

                    }
                    créationOrdonnace = 0;
                    bool codeexiste = proxy.GetAllTcab().Any(n => n.cleproduit == special.clecab);
                    if (codeexiste)
                    {
                        listcodeabarre = proxy.GetAllTcab().Where(n => n.cleproduit == special.clecab).ToList();
                        CodeDataGrid.ItemsSource = listcodeabarre;
                        CodeDataGrid.DataContext = listcodeabarre;
                    }
                    else
                    {
                        listcodeabarre = new List<SVC.Tcab>();
                        CodeDataGrid.ItemsSource = listcodeabarre;
                        CodeDataGrid.DataContext = listcodeabarre;
                    }



                    if (special.photo.ToString() != "")
                    {

                        if (proxy.DownloadDocumentIsHere(special.photo.ToString()) == true)
                        {
                            imgPhoto.Source = LoadImage(proxy.DownloadDocument(special.photo.ToString()));
                            imageok = true;

                            //  op.FileName = special.photo;
                            btnCreer.IsEnabled = true;
                            //  btnCreer.Content = "Modifier";
                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention la photo du produit n'existe plus dans le serveur", "Medicus", MessageBoxButton.OK, MessageBoxImage.Error);
                            op.FileName = "";
                            imageok = false;
                            // patient = patientrecu;
                            btnCreer.IsEnabled = true;
                            //btnCreer.Content = "Modifier";
                        }
                    }


                    callbackrecu.InsertFamilleProduitCallbackevent += new ICallback.CallbackEventHandler58(callbackDci_Refresh);
                    callbackrecu.InsertMarqueCallbackevent += new ICallback.CallbackEventHandler57(callbackMarque_Refresh);

                }*/
              //  else
              //  {
                    special = new SVC.Produit
                    {
                        IdFamille=19,
                        famille= "Verres",
                    };
                    FournVousGrid.DataContext = special;
                    listcodeabarre = new List<SVC.Tcab>();
                    CodeDataGrid.ItemsSource = listcodeabarre;
                    CodeDataGrid.DataContext = listcodeabarre;
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1).ToList();
                    ListeDciCombo.ItemsSource = testmedecin;
                    List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == special.IdFamille).OrderBy(n => n.Id).ToList();
                    ListeDciCombo.SelectedItem = tte.First();
                ListeDciCombo.IsEnabled = false;




                /****-***********************/
                interfacecatalogue = 1;
                    CATitem.IsEnabled = true;
                    CATitem.Visibility = Visibility.Visible;
                    LENTitem.IsEnabled = false;
                    LENTitem.Visibility = Visibility.Collapsed;
                    if (créationOrdonnace == 0 && déjaverre == false)
                    {
                        Rdesc.IsChecked = true;
                        CatalogueVerre = new SVC.Verre
                        {
                            LimiteConnu = false,

                        };
                        radMat1.IsChecked = true;
                        Limit1.IsChecked = true;
                        GridVerre.DataContext = CatalogueVerre;
                        tarifexiste = false;
                        TarifVerre = new SVC.TarifVerre
                        {
                            A1 = 0,
                            A2 = 0,
                            A3 = 0,
                            A4 = 0,
                            B1 = 0,
                            B2 = 0,
                            B3 = 0,
                            B4 = 0,
                            C1 = 0,
                            C2 = 0,
                            C3 = 0,
                            C4 = 0,
                            D1 = 0,
                            D2 = 0,
                            D3 = 0,
                            D4 = 0,
                            E1 = 0,
                            E2 = 0,
                            E3 = 0,
                            E4 = 0,
                            F1 = 0,
                            F2 = 0,
                            F3 = 0,
                            F4 = 0,
                            G1 = 0,
                            G2 = 0,
                            G3 = 0,
                            G4 = 0,
                            H1 = 0,
                            H2 = 0,
                            H3 = 0,
                            H4 = 0,
                        };
                        GridTarification.DataContext = TarifVerre;
                    }
                    /******************************/
                    //  ListeDciCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                    // ListeDciCombo.SelectedIndex = -1; 

                    ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                    ListeMarqueCombo.SelectedIndex = -1;

                    btnCreer.IsEnabled = true;
                    créationOrdonnace = 1;


                    /***************catalogue verre*********************/
                    Rdesc.IsChecked = true;
                    CatalogueVerre = new SVC.Verre
                    {
                        LimiteConnu = false,

                    };
                    radMat1.IsChecked = true;
                    Limit1.IsChecked = true;
                    GridVerre.DataContext = CatalogueVerre;
                    tarifexiste = false;
                  
                    /****************************************************/
                   /* CatalogueLentille = new SVC.Lentille
                    {
                        LimiteConnu = false,
                    };
                    GridLentille.DataContext = CatalogueLentille;*/
                    /********************************************************/
                 //   callbackrecu.InsertFamilleProduitCallbackevent += new ICallback.CallbackEventHandler58(callbackDci_Refresh);
                    callbackrecu.InsertMarqueCallbackevent += new ICallback.CallbackEventHandler57(callbackMarque_Refresh);

              //  }
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }


        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                op = new OpenFileDialog();
                op.Title = "Selectionnez image";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";
                if (op.ShowDialog() == true)
                {
                    imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }






        }
        void callbackDci_Refresh(object source, CallbackEventInsertFamilleProduit e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefresh(List<SVC.FamilleProduit> listMembershipOptic)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        if (créationOrdonnace == 1)
                        {


                            ListeDciCombo.ItemsSource = listMembershipOptic;
                        }
                        else
                        {
                            if (créationOrdonnace == 0)
                            {

                                List<SVC.FamilleProduit> testmedecin = listMembershipOptic;
                                ListeDciCombo.ItemsSource = testmedecin;
                                List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == special.IdFamille).ToList();
                                ListeDciCombo.SelectedItem = tte.First();
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
        void callbackMarque_Refresh(object source, CallbackEventInsertMarque e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshMarque(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefreshMarque(List<SVC.Marque> listMembershipOptic)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        if (créationOrdonnace == 1)
                        {


                            ListeMarqueCombo.ItemsSource = listMembershipOptic;
                        }
                        else
                        {
                            if (créationOrdonnace == 0)
                            {

                                List<SVC.Marque> testmedecin = listMembershipOptic;
                                ListeMarqueCombo.ItemsSource = testmedecin;
                                List<SVC.Marque> tte = testmedecin.Where(n => n.Id == special.IdMarque).ToList();
                                ListeMarqueCombo.SelectedItem = tte.First();
                            }

                        }

                    }
                }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAjouterCatalogueVerre(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAjouterCatalogueVerre(HandleProxy));
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
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region
                if (MembershipOptic.CreationFichier == true && créationOrdonnace == 1 && txtDesign.Text != "")
                {
                    int interfacecreation = 0;
                    bool creeerproduit = false;
                    bool crfeercab = false;
                    bool crverre = false;
                    bool crtarif = false;
                    serverfilepath = op.FileName;

                    filepath = "";
                    if (serverfilepath != "")
                    {

                        filepath = op.FileName;

                        serverfilepath = @"Produit\PhotoProduit\" + (txtDesign.Text.Trim()) + ".png";
                        byte[] buffer = null;

                        // read the file and return the byte[
                        using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, (int)fs.Length);
                        }
                        if (buffer != null)
                        {
                            proxy.UploadDocument(serverfilepath, buffer);
                        }


                    }
                    else
                    {
                        special.photo = "";
                    }
                    special.clecab = txtDesign.Text.Trim() + DateTime.Now;
                    special.cab = txtDesign.Text.Trim() + DateTime.Now;


                    if (ListeDciCombo.SelectedItem != null)
                    {
                        SVC.FamilleProduit selecdci = ListeDciCombo.SelectedItem as SVC.FamilleProduit;

                        special.IdFamille = selecdci.Id;
                        special.famille = selecdci.FamilleProduit1;

                    }
                    else
                    {
                        special.IdFamille = 0;
                        special.famille = "";
                    }

                    if (ListeMarqueCombo.SelectedItem != null)
                    {
                        SVC.Marque selecdci = ListeMarqueCombo.SelectedItem as SVC.Marque;

                        special.IdMarque = selecdci.Id;
                        special.marque = selecdci.MarqueProduit;

                    }
                    else
                    {
                        special.IdMarque = 0;
                        special.marque = "";
                    }
                    if (interfacecatalogue == 1)
                    {
                        CatalogueVerre.Design = special.design + " " + CdompteComboBox.Text;
                        string matiereverre = "";
                        if (radMat1.IsChecked == true)
                        {
                            matiereverre = "Minéral";
                        }
                        else if (radMat2.IsChecked == true)
                        {
                            matiereverre = "Organique";
                        }
                        else if (radMat3.IsChecked == true)
                        {
                            matiereverre = "Polycarbonate";
                        }
                        if (Limit1.IsChecked == true)
                        {
                            CatalogueVerre.LimiteConnu = false;
                        }
                        else if (Limit2.IsChecked == true)
                        {
                            CatalogueVerre.LimiteConnu = true;
                        }

                        CatalogueVerre.IndiceVerre = CdompteComboBox.Text;
                        CatalogueVerre.Surface = SurfaceComboBox.Text;
                        CatalogueVerre.TypeVerre = TypeVerreComboBox.Text;
                        CatalogueVerre.cleproduit = special.cab;
                        CatalogueVerre.Matière = matiereverre;
                        CatalogueVerre.Marque = special.marque;
                        CatalogueVerre.Ref = special.design + " " + CdompteComboBox.Text;

                        if (listtarif.Count > 0)
                        {
                            List<string> listObjects = (from obj in listtarif
                                                        select obj.Diamètre).Distinct().ToList();
                            produitajouter= new List<Produit>();
                            verreajouter= new List<SVC.Verre>();
                            foreach (var item in listObjects)
                            {

                              
                                /***************************************************************/
                                bool existeA1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.A1 != 0 && n.A1 != null);
                                bool existeA1Achat = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == false && n.PrixAchat == true && n.A1 != 0 && n.A1 != null);
                                int numberno = 0;
                                if (existeA1Vente || existeA1Achat)
                                {

                                    double pat = 0.25;
                                    double debut = -2;
                                    double fin = 2;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {

                                        SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                       
                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,

                                        };
                                        if (existeA1Vente)
                                        {
                                            var existeA1ValueVente = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == true && n.A1 != 0 && n.PrixAchat == false && n.A1 != null);
                                            dd.PrixVente = existeA1ValueVente.A1;
                                            ll.PrixVente = existeA1ValueVente.A1;
                                        }
                                        if (existeA1Achat)
                                        {
                                            var existeA1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.A1 != 0 && n.PrixAchat == true && n.A1 != null);
                                            dd.PrixRevient = existeA1ValueAchat.A1;
                                            ll.PrixRevient = existeA1ValueAchat.A1;
                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                       
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /****************************************/
                                bool existeB1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.B1 != 0 && n.B1 != null);
                                bool existeB1Achat = listtarif.Any(n => n.Diamètre == item
                                       && n.PrixVente == false && n.PrixAchat == true && n.B1 != 0 && n.B1 != null);
                                if (existeB1Vente || existeB1Achat)
                                {


                                    double pat = 0.25;
                                    double debut = -4;
                                    double fin = 4;

                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i<-2 || i >2)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };
                                        if (existeB1Vente)
                                        {
                                            var existeB1ValueVente = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == true && n.B1 != 0 && n.PrixAchat == false && n.B1 != null);
                                            dd.PrixVente = existeB1ValueVente.B1;
                                            ll.PrixVente = existeB1ValueVente.B1;


                                        }

                                        if (existeB1Achat)
                                        {
                                            var existeB1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.B1 != 0 && n.PrixAchat == true && n.B1 != null);
                                            dd.PrixRevient = existeB1ValueAchat.B1;
                                            ll.PrixRevient = existeB1ValueAchat.B1;

                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeC1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.C1 != 0 && n.C1 != null);
                                bool existeC1Achat = listtarif.Any(n => n.Diamètre == item
                                   && n.PrixVente == false && n.PrixAchat == true && n.C1 != 0 && n.C1 != null);
                                if (existeC1Vente || existeC1Achat)
                                {



                                    double pat = 0.25;
                                    double debut = -6;
                                    double fin = 6;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };
                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };

                                        if (existeC1Vente)
                                        {
                                            var existeC1ValueVente = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == true && n.C1 != 0 && n.PrixAchat == false && n.C1 != null);
                                            dd.PrixVente = existeC1ValueVente.C1;
                                            ll.PrixVente = existeC1ValueVente.C1;

                                        }
                                        if (existeC1Achat)
                                        {
                                            var existeC1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.C1 != 0 && n.PrixAchat == true && n.C1 != null);
                                            dd.PrixRevient = existeC1ValueAchat.C1;
                                            ll.PrixRevient = existeC1ValueAchat.C1;

                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /**********************************************/
                                bool existeD1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.D1 != 0 && n.D1 != null);
                                bool existeD1Achat = listtarif.Any(n => n.Diamètre == item
                                       && n.PrixVente == false && n.PrixAchat == true && n.D1 != 0 && n.D1 != null);
                                if (existeD1Vente || existeD1Achat)
                                {

                                    double pat = 0.25;
                                    double debut = -8;
                                    double fin = 8;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i < -6 || i > 6)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };

                                        if (existeD1Vente)
                                        {
                                            var existeD1ValueVente = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == true && n.D1 != 0 && n.PrixAchat == false && n.D1 != null);
                                            dd.PrixVente = existeD1ValueVente.D1;
                                            ll.PrixVente = existeD1ValueVente.D1;

                                        }
                                        if (existeD1Achat)
                                        {
                                            var existeD1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.D1 != 0 && n.PrixAchat == true && n.D1 != null);
                                            dd.PrixRevient = existeD1ValueAchat.D1;
                                            ll.PrixRevient = existeD1ValueAchat.D1;

                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*******************************************/
                                bool existeE1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.E1 != 0 && n.E1 != null);
                                bool existeE1Achat = listtarif.Any(n => n.Diamètre == item
                                        && n.PrixVente == false && n.PrixAchat == true && n.E1 != 0 && n.E1 != null);
                                if (existeE1Vente || existeE1Achat)
                                {

                                    double pat = 0.25;
                                    double debut = -10;
                                    double fin = 10;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i < -8 || i > 8)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };

                                        if (existeE1Vente)
                                        {
                                            var existeE1ValueVente = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == true && n.E1 != 0 && n.PrixAchat == false && n.E1 != null);
                                            dd.PrixVente = existeE1ValueVente.E1;
                                            ll.PrixVente = existeE1ValueVente.E1;

                                        }
                                        if (existeE1Achat)
                                        {
                                            var existeE1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.E1 != 0 && n.PrixAchat == true && n.E1 != null);
                                            dd.PrixRevient = existeE1ValueAchat.E1;
                                            ll.PrixRevient = existeE1ValueAchat.E1;
                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*****************************************/
                                bool existeF1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.F1 != 0 && n.F1 != null);
                                bool existeF1Achat = listtarif.Any(n => n.Diamètre == item
                                       && n.PrixVente == false && n.PrixAchat == true && n.F1 != 0 && n.F1 != null);
                                if (existeF1Vente || existeF1Achat)
                                {

                                    double pat = 0.25;
                                    double debut = -12;
                                    double fin = 12;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i < -10 || i > 10)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };

                                        if (existeF1Vente)
                                        {
                                            var existeF1ValueVente = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == true && n.F1 != 0 && n.PrixAchat == false && n.F1 != null);
                                            dd.PrixVente = existeF1ValueVente.F1;
                                            ll.PrixVente = existeF1ValueVente.F1;

                                        }
                                        if (existeF1Achat)
                                        {
                                            var existeF1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.F1 != 0 && n.PrixAchat == true && n.F1 != null);
                                            dd.PrixRevient = existeF1ValueAchat.F1;
                                            ll.PrixRevient = existeF1ValueAchat.F1;
                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /***********************************************/
                                bool existeG1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.G1 != 0 && n.G1 != null);
                                bool existeG1Achat = listtarif.Any(n => n.Diamètre == item
                                       && n.PrixVente == false && n.PrixAchat == true && n.G1 != 0 && n.G1 != null);
                                if (existeG1Vente || existeG1Achat)
                                {

                                    double pat = 0.25;
                                    double debut = -14;
                                    double fin = 14;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i < -12 || i > 12)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };
                                        if (existeG1Vente)
                                        {
                                            var existeG1ValueVente = listtarif.First(n => n.Diamètre == item
                                          && n.PrixVente == true && n.G1 != 0 && n.PrixAchat == false && n.G1 != null);
                                            dd.PrixVente = existeG1ValueVente.G1;
                                            ll.PrixVente = existeG1ValueVente.G1;

                                        }

                                        if (existeG1Achat)
                                        {
                                            var existeG1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.G1 != 0 && n.PrixAchat == true && n.G1 != null);
                                            dd.PrixRevient = existeG1ValueAchat.G1;
                                            ll.PrixRevient = existeG1ValueAchat.G1;

                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*******************************************/
                                bool existeH1Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.H1 != 0 && n.H1 != null);
                                bool existeH1Achat = listtarif.Any(n => n.Diamètre == item
                                       && n.PrixVente == false && n.PrixAchat == true && n.H1 != 0 && n.H1 != null);
                                if (existeH1Vente || existeH1Achat)
                                {

                                    double pat = 0.25;
                                    double debut = -16;
                                    double fin = 16;
                                    for (double i = debut; i <= fin; i = i + pat)
                                    {
                                        if (i < -14 || i > 14)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                        {
                                            Aphaque = CatalogueVerre.Aphaque,
                                            cleproduit = CatalogueVerre.cleproduit + item + i.ToString(),
                                            Cletarif = CatalogueVerre.cleproduit + item,
                                            Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Diamètre = item,
                                            Fournisseur = CatalogueVerre.Fournisseur,
                                            IndiceVerre = CatalogueVerre.IndiceVerre,
                                            LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                            LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                            LimiteConnu = CatalogueVerre.LimiteConnu,
                                            LimiteFab0 = CatalogueVerre.LimiteFab0,
                                            LimiteFab1 = CatalogueVerre.LimiteFab1,
                                            Marque = CatalogueVerre.Marque,
                                            Matière = CatalogueVerre.Matière,
                                            Photochromique = CatalogueVerre.Photochromique,
                                            Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            Surface = CatalogueVerre.Surface,
                                            Teinte = CatalogueVerre.Teinte,
                                            TypeVerre = CatalogueVerre.TypeVerre,
                                            VerreTeinté = CatalogueVerre.VerreTeinté,
                                            Sph = Convert.ToDecimal(i),
                                        };

                                        SVC.Produit dd = new Produit
                                        {
                                            cab = CatalogueVerre.cleproduit + item + i.ToString(),
                                            design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString(),
                                            clecab = special.clecab,
                                            dates = special.dates,
                                            famille = special.famille,
                                            IdFamille = special.IdFamille,
                                            IdMarque = special.IdMarque,
                                            marque = special.marque,
                                            photo = special.photo,
                                            StockAlert = special.StockAlert,
                                            typevente = special.typevente,
                                            Catalogue = true,
                                        };
                                        if (existeH1Vente)
                                        {
                                            var existeH1ValueVente = listtarif.First(n => n.Diamètre == item
                                       && n.PrixVente == true && n.H1 != 0 && n.PrixAchat == false && n.H1 != null);
                                            dd.PrixVente = existeH1ValueVente.H1;
                                            ll.PrixVente = existeH1ValueVente.H1;

                                        }

                                        if (existeH1Achat)
                                        {
                                            var existeH1ValueAchat = listtarif.First(n => n.Diamètre == item
                                            && n.PrixVente == false && n.H1 != 0 && n.PrixAchat == true && n.H1 != null);
                                            dd.PrixRevient = existeH1ValueAchat.H1;
                                            ll.PrixRevient = existeH1ValueAchat.H1;

                                        }
                                        produitajouter.Add(dd);
                                        verreajouter.Add(ll);
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /************************************cylindre************************************/
                                /*******************************************/
                                bool existeA2Vente = listtarif.Any(n => n.Diamètre == item
                                && n.PrixVente == true && n.A2 != 0 && n.A2 != null);
                                bool existeA2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.A2 != 0 && n.A2 != null);
                                if (existeA2Vente || existeA2Achat)
                                {

                                    /*  double pat = 0.25;
                                      double debutsph = 0;
                                      double finsph = 2;
                                      double debutcyl = 0.25;
                                      double fincyl = 2;*/
                                    double pat = 0.25;
                                    double debutsph = -2;
                                    double finsph = 2;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i !=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeA2Vente)
                                            {
                                                var existeA2ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.A2 != 0 && n.PrixAchat == false && n.A2 != null);
                                                dd.PrixVente = existeA2ValueVente.A2;
                                                ll.PrixVente = existeA2ValueVente.A2;

                                            }
                                            if (existeA2Achat)
                                            {
                                                var existeA2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.A2 != 0 && n.PrixAchat == true && n.A2 != null);
                                                dd.PrixRevient = existeA2ValueAchat.A2;
                                                ll.PrixRevient = existeA2ValueAchat.A2;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*******************************************/
                                bool existeA3Vente = listtarif.Any(n => n.Diamètre == item
                              && n.PrixVente == true && n.A3 != 0 && n.A3 != null);
                                bool existeA3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.A3 != 0 && n.A3 != null);
                                if (existeA3Vente || existeA3Achat)
                                {

                                    double pat = 0.25;
                                    /*   double debutsph = 0;
                                       double finsph = 2;
                                       double debutcyl = 2.25;
                                       double fincyl = 4;*/
                                    double debutsph = -2;
                                    double finsph = 2;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    { 
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeA3Vente)
                                            {
                                                var existeA3ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.A3 != 0 && n.PrixAchat == false && n.A3 != null);
                                                dd.PrixVente = existeA3ValueVente.A3;
                                                ll.PrixVente = existeA3ValueVente.A3;

                                            }
                                            if (existeA3Achat)
                                            {
                                                var existeA3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.A3 != 0 && n.PrixAchat == true && n.A3 != null);
                                                dd.PrixRevient = existeA3ValueAchat.A3;
                                                ll.PrixRevient = existeA3ValueAchat.A3;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*******************************************/
                                bool existeA4Vente = listtarif.Any(n => n.Diamètre == item
                              && n.PrixVente == true && n.A4 != 0 && n.A4 != null);
                                bool existeA4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.A4 != 0 && n.A4 != null);
                                if (existeA4Vente || existeA4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -2;
                                    double finsph = 2;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeA4Vente)
                                            {
                                                var existeA4ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.A4 != 0 && n.PrixAchat == false && n.A4 != null);
                                                dd.PrixVente = existeA4ValueVente.A4;
                                                ll.PrixVente = existeA4ValueVente.A4;

                                            }
                                            if (existeA4Achat)
                                            {
                                                var existeA4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.A4 != 0 && n.PrixAchat == true && n.A4 != null);
                                                dd.PrixRevient = existeA4ValueAchat.A4;
                                                ll.PrixRevient = existeA4ValueAchat.A4;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*******************************************/
                                bool existeB2Vente = listtarif.Any(n => n.Diamètre == item
                              && n.PrixVente == true && n.B2 != 0 && n.B2 != null);
                                bool existeB2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.B2 != 0 && n.B2 != null);
                                if (existeB2Vente || existeB2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -4;
                                    double finsph = 4;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i !=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                            {
                                                if (j < -2 || j > 2)
                                                {
                                                    SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeB2Vente)
                                                {
                                                    var existeB2ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.B2 != 0 && n.PrixAchat == false && n.B2 != null);
                                                    dd.PrixVente = existeB2ValueVente.B2;
                                                    ll.PrixVente = existeB2ValueVente.B2;

                                                }
                                                if (existeB2Achat)
                                                {
                                                    var existeB2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.B2 != 0 && n.PrixAchat == true && n.B2 != null);
                                                    dd.PrixRevient = existeB2ValueAchat.B2;
                                                    ll.PrixRevient = existeB2ValueAchat.B2;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /**********************************************************
                                 * /
                                 *                            /*******************************************/
                                bool existeB3Vente = listtarif.Any(n => n.Diamètre == item
                              && n.PrixVente == true && n.B3 != 0 && n.B3 != null);
                                bool existeB3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.B3 != 0 && n.B3 != null);
                                if (existeB3Vente || existeB3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -4;
                                    double finsph = 4;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                            {
                                                if (j < -2 || j > 2)
                                                {
                                                    SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeB3Vente)
                                                {
                                                    var existeB3ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.B3 != 0 && n.PrixAchat == false && n.B3 != null);
                                                    dd.PrixVente = existeB3ValueVente.B3;
                                                    ll.PrixVente = existeB3ValueVente.B3;

                                                }
                                                if (existeB3Achat)
                                                {
                                                    var existeB3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.B3 != 0 && n.PrixAchat == true && n.B3 != null);
                                                    dd.PrixRevient = existeB3ValueAchat.B3;
                                                    ll.PrixRevient = existeB3ValueAchat.B3;
                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /******************************************/
                                bool existeB4Vente = listtarif.Any(n => n.Diamètre == item
                            && n.PrixVente == true && n.B4 != 0 && n.B4 != null);
                                bool existeB4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.B4 != 0 && n.B4 != null);
                                if (existeB4Vente || existeB4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -4;
                                    double finsph = 4;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                            {
                                                if (j < -2 || j > 2)
                                                {
                                                    SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeB4Vente)
                                                {
                                                    var existeB4ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.B4 != 0 && n.PrixAchat == false && n.B4 != null);
                                                    dd.PrixVente = existeB4ValueVente.B4;
                                                    ll.PrixVente = existeB4ValueVente.B4;

                                                }
                                                if (existeB4Achat)
                                                {
                                                    var existeB4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.B4 != 0 && n.PrixAchat == true && n.B4 != null);
                                                    dd.PrixRevient = existeB4ValueAchat.B4;
                                                    ll.PrixRevient = existeB4ValueAchat.B4;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*************************************************************/
                                bool existeC2Vente = listtarif.Any(n => n.Diamètre == item
                                   && n.PrixVente == true && n.C2 != 0 && n.C2 != null);
                                bool existeC2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.C2 != 0 && n.C2 != null);
                                if (existeC2Vente || existeC2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -6;
                                    double finsph = 6;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i !=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -4 || j > 4)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeC2Vente)
                                                {
                                                    var existeC2ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.C2 != 0 && n.PrixAchat == false && n.C2 != null);
                                                    dd.PrixVente = existeC2ValueVente.C2;
                                                    ll.PrixVente = existeC2ValueVente.C2;

                                                }
                                                if (existeC2Achat)
                                                {
                                                    var existeC2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.C2 != 0 && n.PrixAchat == true && n.C2 != null);
                                                    dd.PrixRevient = existeC2ValueAchat.C2;
                                                    ll.PrixRevient = existeC2ValueAchat.C2;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*************************************************************/
                                bool existeC3Vente = listtarif.Any(n => n.Diamètre == item
                                   && n.PrixVente == true && n.C3 != 0 && n.C3 != null);
                                bool existeC3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.C3 != 0 && n.C3 != null);
                                if (existeC3Vente || existeC3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -6;
                                    double finsph = 6;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                            {
                                                if (j < -4 || j > 4)
                                                {
                                                    SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeC3Vente)
                                                {
                                                    var existeC3ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.C3 != 0 && n.PrixAchat == false && n.C3 != null);
                                                    dd.PrixVente = existeC3ValueVente.C3;
                                                    ll.PrixVente = existeC3ValueVente.C3;

                                                }
                                                if (existeC3Achat)
                                                {
                                                    var existeC3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.C3 != 0 && n.PrixAchat == true && n.C3 != null);
                                                    dd.PrixRevient = existeC3ValueAchat.C3;
                                                    ll.PrixRevient = existeC3ValueAchat.C3;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /**************************************************/
                                bool existeC4Vente = listtarif.Any(n => n.Diamètre == item
                                 && n.PrixVente == true && n.C4 != 0 && n.C4 != null);
                                bool existeC4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.C4 != 0 && n.C4 != null);
                                if (existeC4Vente || existeC4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -6;
                                    double finsph = 6;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                            {
                                                if (j < -4 || j > 4)
                                                {
                                                    SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };


                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeC4Vente)
                                                {
                                                    var existeC4ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.C4 != 0 && n.PrixAchat == false && n.C4 != null);
                                                    dd.PrixVente = existeC4ValueVente.C4;
                                                    ll.PrixVente = existeC4ValueVente.C4;

                                                }
                                                if (existeC4Achat)
                                                {
                                                    var existeC4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.C4 != 0 && n.PrixAchat == true && n.C4 != null);
                                                    dd.PrixRevient = existeC4ValueAchat.C4;
                                                    ll.PrixRevient = existeC4ValueAchat.C4;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeD2Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.D2 != 0 && n.D2 != null);
                                bool existeD2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.D2 != 0 && n.D2 != null);
                                if (existeD2Vente || existeD2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -8;
                                    double finsph = 8;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i!=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -6 || j > 6)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeD2Vente)
                                                {
                                                    var existeD2ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.D2 != 0 && n.PrixAchat == false && n.D2 != null);
                                                    dd.PrixVente = existeD2ValueVente.D2;
                                                    ll.PrixVente = existeD2ValueVente.D2;

                                                }
                                                if (existeD2Achat)
                                                {
                                                    var existeD2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.D2 != 0 && n.PrixAchat == true && n.D2 != null);
                                                    dd.PrixRevient = existeD2ValueAchat.D2;
                                                    ll.PrixRevient = existeD2ValueAchat.D2;
                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }  /*********************************************/
                                bool existeD3Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.D3 != 0 && n.D3 != null);
                                bool existeD3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.D3 != 0 && n.D3 != null);
                                if (existeD3Vente || existeD3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -8;
                                    double finsph = 8;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -6 || j > 6)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeD3Vente)
                                                {
                                                    var existeD3ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.D3 != 0 && n.PrixAchat == false && n.D3 != null);
                                                    dd.PrixVente = existeD3ValueVente.D3;
                                                    ll.PrixVente = existeD3ValueVente.D3;

                                                }
                                                if (existeD3Achat)
                                                {
                                                    var existeD3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.D3 != 0 && n.PrixAchat == true && n.D3 != null);
                                                    dd.PrixRevient = existeD3ValueAchat.D3;
                                                    ll.PrixRevient = existeD3ValueAchat.D3;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeD4Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.D4 != 0 && n.D4 != null);
                                bool existeD4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.D4 != 0 && n.D4 != null);
                                if (existeD4Vente || existeD4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -8;
                                    double finsph = 8;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -6 || j > 6)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeD4Vente)
                                                {
                                                    var existeD4ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.D4 != 0 && n.PrixAchat == false && n.D4 != null);
                                                    dd.PrixVente = existeD4ValueVente.D4;
                                                    ll.PrixVente = existeD4ValueVente.D4;

                                                }
                                                if (existeD4Achat)
                                                {
                                                    var existeD4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.D4 != 0 && n.PrixAchat == true && n.D4 != null);
                                                    dd.PrixRevient = existeD4ValueAchat.D4;
                                                    ll.PrixRevient = existeD4ValueAchat.D4;

                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeE2Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.E2 != 0 && n.E2 != null);
                                bool existeE2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.E2 != 0 && n.E2 != null);
                                if (existeE2Vente || existeE2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -10;
                                    double finsph = 10;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i !=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -8 || j > 8)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                                {
                                                    Aphaque = CatalogueVerre.Aphaque,
                                                    cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    Cletarif = CatalogueVerre.cleproduit + item,
                                                    Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Diamètre = item,
                                                    Fournisseur = CatalogueVerre.Fournisseur,
                                                    IndiceVerre = CatalogueVerre.IndiceVerre,
                                                    LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                    LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                    LimiteConnu = CatalogueVerre.LimiteConnu,
                                                    LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                    LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                    Marque = CatalogueVerre.Marque,
                                                    Matière = CatalogueVerre.Matière,
                                                    Photochromique = CatalogueVerre.Photochromique,
                                                    Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    Surface = CatalogueVerre.Surface,
                                                    Teinte = CatalogueVerre.Teinte,
                                                    TypeVerre = CatalogueVerre.TypeVerre,
                                                    VerreTeinté = CatalogueVerre.VerreTeinté,
                                                    Sph = Convert.ToDecimal(j),
                                                    Cyl = Convert.ToDecimal(i),
                                                };

                                                SVC.Produit dd = new Produit
                                                {
                                                    cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                    design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                    clecab = special.clecab,
                                                    dates = special.dates,
                                                    famille = special.famille,
                                                    IdFamille = special.IdFamille,
                                                    IdMarque = special.IdMarque,
                                                    marque = special.marque,
                                                    photo = special.photo,
                                                    StockAlert = special.StockAlert,
                                                    typevente = special.typevente,
                                                    Catalogue = true,
                                                };
                                                if (existeE2Vente)
                                                {
                                                    var existeE2ValueVente = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == true && n.E2 != 0 && n.PrixAchat == false && n.E2 != null);
                                                    dd.PrixVente = existeE2ValueVente.E2;
                                                    ll.PrixVente = existeE2ValueVente.E2;

                                                }
                                                if (existeE2Achat)
                                                {
                                                    var existeE2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                    && n.PrixVente == false && n.E2 != 0 && n.PrixAchat == true && n.E2 != null);
                                                    dd.PrixRevient = existeE2ValueAchat.E2;
                                                    ll.PrixRevient = existeE2ValueAchat.E2;
                                                }
                                                produitajouter.Add(dd);
                                                verreajouter.Add(ll);
                                            }
                                        }
                                    }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeE3Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.E3 != 0 && n.E3 != null);
                                bool existeE3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.E3 != 0 && n.E3 != null);
                                if (existeE3Vente || existeE3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -10;
                                    double finsph = 10;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -8 || j > 8)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeE3Vente)
                                            {
                                                var existeE3ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.E3 != 0 && n.PrixAchat == false && n.E3 != null);
                                                dd.PrixVente = existeE3ValueVente.E3;
                                                ll.PrixVente = existeE3ValueVente.E3;

                                            }
                                            if (existeE3Achat)
                                            {
                                                var existeE3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.E3 != 0 && n.PrixAchat == true && n.E3 != null);
                                                dd.PrixRevient = existeE3ValueAchat.E3;
                                                ll.PrixRevient = existeE3ValueAchat.E3;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeE4Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.E4 != 0 && n.E4 != null);
                                bool existeE4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.E4 != 0 && n.E4 != null);
                                if (existeE4Vente || existeE4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -10;
                                    double finsph = 10;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -8 || j > 8)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeE4Vente)
                                            {
                                                var existeE4ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.E4 != 0 && n.PrixAchat == false && n.E4 != null);
                                                dd.PrixVente = existeE4ValueVente.E4;
                                                ll.PrixVente = existeE4ValueVente.E4;

                                            }
                                            if (existeE4Achat)
                                            {
                                                var existeE4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.E4 != 0 && n.PrixAchat == true && n.E4 != null);
                                                dd.PrixRevient = existeE4ValueAchat.E4;
                                                ll.PrixRevient = existeE4ValueAchat.E4;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }    /*********************************************/
                                bool existeF2Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.F2 != 0 && n.F2 != null);
                                bool existeF2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.F2 != 0 && n.F2 != null);
                                if (existeF2Vente || existeF2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -12;
                                    double finsph = 12;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i!=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -10 || j > 10)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeF2Vente)
                                            {
                                                var existeF2ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.F2 != 0 && n.PrixAchat == false && n.F2 != null);
                                                dd.PrixVente = existeF2ValueVente.F2;
                                                ll.PrixVente = existeF2ValueVente.F2;

                                            }
                                            if (existeF2Achat)
                                            {
                                                var existeF2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.F2 != 0 && n.PrixAchat == true && n.F2 != null);
                                                dd.PrixRevient = existeF2ValueAchat.F2;
                                                ll.PrixRevient = existeF2ValueAchat.F2;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeF3Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.F3 != 0 && n.F3 != null);
                                bool existeF3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.F3 != 0 && n.F3 != null);
                                if (existeF3Vente || existeF3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -12;
                                    double finsph = 12;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -10 || j > 10)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeF3Vente)
                                            {
                                                var existeF3ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.F3 != 0 && n.PrixAchat == false && n.F3 != null);
                                                dd.PrixVente = existeF3ValueVente.F3;
                                                ll.PrixVente = existeF3ValueVente.F3;

                                            }
                                            if (existeF3Achat)
                                            {
                                                var existeF3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.F3 != 0 && n.PrixAchat == true && n.F3 != null);
                                                dd.PrixRevient = existeF3ValueAchat.F3;
                                                ll.PrixRevient = existeF3ValueAchat.F3;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeF4Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.F4 != 0 && n.F4 != null);
                                bool existeF4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.F4 != 0 && n.F4 != null);
                                if (existeF4Vente || existeF4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -12;
                                    double finsph = 12;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -10 || j > 10)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeF4Vente)
                                            {
                                                var existeF4ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.F4 != 0 && n.PrixAchat == false && n.F4 != null);
                                                dd.PrixVente = existeF4ValueVente.F4;
                                                ll.PrixVente = existeF4ValueVente.F4;

                                            }
                                            if (existeF4Achat)
                                            {
                                                var existeF4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.F4 != 0 && n.PrixAchat == true && n.F4 != null);
                                                dd.PrixRevient = existeF4ValueAchat.F4;
                                                ll.PrixRevient = existeF4ValueAchat.F4;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeG2Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.G2 != 0 && n.G2 != null);
                                bool existeG2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.G2 != 0 && n.G2 != null);
                                if (existeG2Vente || existeG2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -14;
                                    double finsph = 14;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i!=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -12 || j > 12)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeG2Vente)
                                            {
                                                var existeG2ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.G2 != 0 && n.PrixAchat == false && n.G2 != null);
                                                dd.PrixVente = existeG2ValueVente.G2;
                                                ll.PrixVente = existeG2ValueVente.G2;

                                            }
                                            if (existeG2Achat)
                                            {
                                                var existeG2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.G2 != 0 && n.PrixAchat == true && n.G2 != null);
                                                dd.PrixRevient = existeG2ValueAchat.G2;
                                                ll.PrixRevient = existeG2ValueAchat.G2;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeG3Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.G3 != 0 && n.G3 != null);
                                bool existeG3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.G3 != 0 && n.G3 != null);
                                if (existeG3Vente || existeG3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -14;
                                    double finsph = 14;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -12 || j > 12)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeG3Vente)
                                            {
                                                var existeG3ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.G3 != 0 && n.PrixAchat == false && n.G3 != null);
                                                dd.PrixVente = existeG3ValueVente.G3;
                                                ll.PrixVente = existeG3ValueVente.G3;

                                            }
                                            if (existeG3Achat)
                                            {
                                                var existeG3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.G3 != 0 && n.PrixAchat == true && n.G3 != null);
                                                dd.PrixRevient = existeG3ValueAchat.G3;
                                                ll.PrixRevient = existeG3ValueAchat.G3;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeG4Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.G4 != 0 && n.G4 != null);
                                bool existeG4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.G4 != 0 && n.G4 != null);
                                if (existeG4Vente || existeG4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -14;
                                    double finsph = 14;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -12 || j > 12)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeG4Vente)
                                            {
                                                var existeG4ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.G4 != 0 && n.PrixAchat == false && n.G4 != null);
                                                dd.PrixVente = existeG4ValueVente.G4;
                                                ll.PrixVente = existeG4ValueVente.G4;

                                            }
                                            if (existeG4Achat)
                                            {
                                                var existeG4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.G4 != 0 && n.PrixAchat == true && n.G4 != null);
                                                dd.PrixRevient = existeG4ValueAchat.G4;
                                                ll.PrixRevient = existeG4ValueAchat.G4;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeH2Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.H2 != 0 && n.H2 != null);
                                bool existeH2Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.H2 != 0 && n.H2 != null);
                                if (existeH2Vente || existeH2Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -16;
                                    double finsph = 16;
                                    double debutcyl = -2;
                                    double fincyl = 2;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i!=0)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -14 || j > 14)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeH2Vente)
                                            {
                                                var existeH2ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.H2 != 0 && n.PrixAchat == false && n.H2 != null);
                                                dd.PrixVente = existeH2ValueVente.H2;
                                                ll.PrixVente = existeH2ValueVente.H2;

                                            }
                                            if (existeH2Achat)
                                            {
                                                var existeH2ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.H2 != 0 && n.PrixAchat == true && n.H2 != null);
                                                dd.PrixRevient = existeH2ValueAchat.H2;
                                                ll.PrixRevient = existeH2ValueAchat.H2;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }

                                /*********************************************/
                                bool existeH3Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.H3 != 0 && n.H3 != null);
                                bool existeH3Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.H3 != 0 && n.H3 != null);
                                if (existeH3Vente || existeH3Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -16;
                                    double finsph = 16;
                                    double debutcyl = -4;
                                    double fincyl = 4;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -2 || i > 2)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -14 || j > 14)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeH3Vente)
                                            {
                                                var existeH3ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.H3 != 0 && n.PrixAchat == false && n.H3 != null);
                                                dd.PrixVente = existeH3ValueVente.H3;
                                                ll.PrixVente = existeH3ValueVente.H3;

                                            }
                                            if (existeH3Achat)
                                            {
                                                var existeH3ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.H3 != 0 && n.PrixAchat == true && n.H3 != null);
                                                dd.PrixRevient = existeH3ValueAchat.H3;
                                                ll.PrixRevient = existeH3ValueAchat.H3;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                /*********************************************/
                                bool existeH4Vente = listtarif.Any(n => n.Diamètre == item
                               && n.PrixVente == true && n.H4 != 0 && n.H4 != null);
                                bool existeH4Achat = listtarif.Any(n => n.Diamètre == item
                                           && n.PrixVente == false && n.PrixAchat == true && n.H4 != 0 && n.H4 != null);
                                if (existeH4Vente || existeH4Achat)
                                {

                                    double pat = 0.25;
                                    double debutsph = -16;
                                    double finsph = 16;
                                    double debutcyl = -6;
                                    double fincyl = 6;
                                    for (double i = debutcyl; i <= fincyl; i = i + pat)
                                    {
                                        if (i < -4 || i > 4)
                                        {
                                            for (double j = debutsph; j <= finsph; j = j + pat)
                                        {
                                            if (j < -14 || j > 14)
                                            {
                                                SVC.Verre ll = new SVC.Verre
                                            {
                                                Aphaque = CatalogueVerre.Aphaque,
                                                cleproduit = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                Cletarif = CatalogueVerre.cleproduit + item,
                                                Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Diamètre = item,
                                                Fournisseur = CatalogueVerre.Fournisseur,
                                                IndiceVerre = CatalogueVerre.IndiceVerre,
                                                LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                                LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                                LimiteConnu = CatalogueVerre.LimiteConnu,
                                                LimiteFab0 = CatalogueVerre.LimiteFab0,
                                                LimiteFab1 = CatalogueVerre.LimiteFab1,
                                                Marque = CatalogueVerre.Marque,
                                                Matière = CatalogueVerre.Matière,
                                                Photochromique = CatalogueVerre.Photochromique,
                                                Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                Surface = CatalogueVerre.Surface,
                                                Teinte = CatalogueVerre.Teinte,
                                                TypeVerre = CatalogueVerre.TypeVerre,
                                                VerreTeinté = CatalogueVerre.VerreTeinté,
                                                Sph = Convert.ToDecimal(j),
                                                Cyl = Convert.ToDecimal(i),
                                            };

                                            SVC.Produit dd = new Produit
                                            {
                                                cab = CatalogueVerre.cleproduit + item + i.ToString() + j.ToString(),
                                                design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre + " " + i.ToString() + " " + j.ToString(),
                                                clecab = special.clecab,
                                                dates = special.dates,
                                                famille = special.famille,
                                                IdFamille = special.IdFamille,
                                                IdMarque = special.IdMarque,
                                                marque = special.marque,
                                                photo = special.photo,
                                                StockAlert = special.StockAlert,
                                                typevente = special.typevente,
                                                Catalogue = true,
                                            };
                                            if (existeH4Vente)
                                            {
                                                var existeH4ValueVente = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == true && n.H4 != 0 && n.PrixAchat == false && n.H4 != null);
                                                dd.PrixVente = existeH4ValueVente.H4;
                                                ll.PrixVente = existeH4ValueVente.H4;

                                            }
                                            if (existeH4Achat)
                                            {
                                                var existeH4ValueAchat = listtarif.First(n => n.Diamètre == item
                                                && n.PrixVente == false && n.H4 != 0 && n.PrixAchat == true && n.H4 != null);
                                                dd.PrixRevient = existeH4ValueAchat.H4;
                                                ll.PrixRevient = existeH4ValueAchat.H4;

                                            }
                                            produitajouter.Add(dd);
                                            verreajouter.Add(ll);
                                        }
                                    }
                                }
                                    }
                                }
                                else
                                {
                                    numberno = numberno + 1;
                                }
                                if (numberno == 32)
                                {

                                    
                                    //   special.design = special.design + " " + CdompteComboBox.Text;
                                    SVC.Produit dd = new Produit
                                    {
                                        cab = CatalogueVerre.cleproduit + item,
                                        design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre,
                                        clecab = special.clecab,
                                        dates = special.dates,
                                        famille = special.famille,
                                        IdFamille = special.IdFamille,
                                        IdMarque = special.IdMarque,
                                        marque = special.marque,
                                        photo = special.photo,
                                        StockAlert = special.StockAlert,
                                        typevente = special.typevente,
                                        PrixVente = 0,
                                        PrixRevient = 0,
                                        Catalogue = false,
                                    };
                                   
                                    

                                    SVC.Verre ll = new SVC.Verre
                                    {
                                        Aphaque = CatalogueVerre.Aphaque,
                                        cleproduit = CatalogueVerre.cleproduit + item,
                                        Cletarif = CatalogueVerre.cleproduit + item,
                                        //  Design = CatalogueVerre.Design + " " + item,
                                        Design = special.design + " " + item + " " + CatalogueVerre.IndiceVerre,
                                        Diamètre = item,
                                        Fournisseur = CatalogueVerre.Fournisseur,
                                        IndiceVerre = CatalogueVerre.IndiceVerre,
                                        LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                        LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                        LimiteConnu = CatalogueVerre.LimiteConnu,
                                        LimiteFab0 = CatalogueVerre.LimiteFab0,
                                        LimiteFab1 = CatalogueVerre.LimiteFab1,
                                        Marque = CatalogueVerre.Marque,
                                        Matière = CatalogueVerre.Matière,
                                        Photochromique = CatalogueVerre.Photochromique,
                                        // Ref = CatalogueVerre.Design + " " + item,
                                        Ref = special.design + " " + item + " " + CatalogueVerre.IndiceVerre,
                                        Surface = CatalogueVerre.Surface,
                                        Teinte = CatalogueVerre.Teinte,
                                        TypeVerre = CatalogueVerre.TypeVerre,
                                        VerreTeinté = CatalogueVerre.VerreTeinté,
                                    };
                                    produitajouter.Add(dd);
                                    verreajouter.Add(ll);
                                    //MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                               
                                /******************************************/
                                /*  creeerproduit = false;
                                  SVC.Produit dd = new Produit
                                  {
                                      cab = CatalogueVerre.cleproduit + item,
                                      design = special.design + " " + item,
                                      clecab = special.clecab,
                                      dates = special.dates,
                                      famille = special.famille,
                                      IdFamille = special.IdFamille,
                                      IdMarque = special.IdMarque,
                                      marque = special.marque,
                                      photo = special.photo,
                                      StockAlert = special.StockAlert,
                                      typevente = special.typevente,

                                  };

                                  proxy.InsertProduit(dd);
                                  creeerproduit = true;
                                  /***********************************/
                            }
                           
                        }

                    }
                    else
                    {
                        if (interfacecatalogue == 2)
                        {
                            CatalogueLentille.Design = special.design;
                            CatalogueLentille.cleproduit = special.cab;
                            CatalogueLentille.CodeLPP = TypeLPPComboBox.Text;
                            CatalogueLentille.Durrée = TypeDuréeComboBox.Text;
                            CatalogueLentille.FiltreUV = ckfiltreUV.IsChecked;
                            if (LimitLentille1.IsChecked == true)
                            {
                                CatalogueLentille.LimiteConnu = false;
                            }
                            else if (LimitLentille2.IsChecked == true)
                            {
                                CatalogueLentille.LimiteConnu = true;
                            }
                            CatalogueLentille.Matière = MatièreComboBox.Text;
                            CatalogueLentille.Packaging = TypePackagingComboBox.Text;
                            CatalogueLentille.TypeLentille = TypeLentilleComboBox.Text;
                            CatalogueLentille.Marque = special.marque;
                            CatalogueLentille.Ref = special.design;
                        }
                    }
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        special.photo = serverfilepath;
                        if (listcodeabarre.Count > 0)
                        {

                            foreach (SVC.Tcab item in listcodeabarre)
                            {
                                if (item.cab != "")
                                {
                                    crfeercab = false;
                                    item.cleproduit = special.clecab;
                                    proxy.InsertTcab(item);
                                    crfeercab = true;
                                }
                            }

                            interfacecreation = 1;

                        }
                        else
                        {
                            interfacecreation = 0;

                        }

                        if (interfacecatalogue == 1)
                        {

                            if(verreajouter.Count>0)
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Attention ! le systéme va générer " + produitajouter.Count, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    foreach (var itemverre in verreajouter)
                                    {
                                        crverre = true;

                                        proxy.InsertVerre(itemverre);
                                        crverre = true;

                                    }
                                    foreach (var itemprodfuit in produitajouter)
                                    {

                                        creeerproduit = false;

                                        proxy.InsertProduit(itemprodfuit);
                                        creeerproduit = true;
                                    }

                                    foreach (var item in listtarif)
                                    {
                                        crtarif = false;
                                        item.Cletarif = CatalogueVerre.cleproduit + item.Diamètre;
                                        item.cleproduit = CatalogueVerre.cleproduit + item.Diamètre;
                                        proxy.InsertTarifVerre(item);
                                        crtarif = true;
                                    }
                                }

                            }
                            else
                            {
                             //   MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                crverre = true;
                                CatalogueVerre.Design = special.design + " " + CdompteComboBox.Text;
                                CatalogueVerre.Ref = special.design + " " + CdompteComboBox.Text;
                                proxy.InsertVerre(CatalogueVerre);
                                crverre = true;
                                crtarif = true;
                                creeerproduit = false;
                                special.Catalogue = false;
                                special.design = special.design + " " + CdompteComboBox.Text;
                                proxy.InsertProduit(special);
                                creeerproduit = true;
                            }
                        }
                      



                        if (interfacecatalogue == 0)
                        {
                            if (creeerproduit == true && interfacecreation == 0)
                            {
                                ts.Complete();
                            }
                            else
                            {
                                if (creeerproduit == true && interfacecreation == 1 && crfeercab == true)
                                {
                                    ts.Complete();
                                }
                            }
                        }
                        else
                        {
                            if (interfacecatalogue == 1)
                            {
                                if (creeerproduit == true && interfacecreation == 0 && crverre == true && crtarif == true)
                                {
                                    ts.Complete();
                                }
                                else
                                {
                                    if (creeerproduit == true && interfacecreation == 1 && crfeercab == true && crverre == true && crtarif == true)
                                    {
                                        ts.Complete();
                                    }
                                }
                            }
                            else
                            {
                                if (interfacecatalogue == 2)
                                {
                                    if (creeerproduit == true && interfacecreation == 0 && crverre == true)
                                    {
                                        ts.Complete();
                                    }
                                    else
                                    {
                                        if (creeerproduit == true && interfacecreation == 1 && crfeercab == true && crverre == true)
                                        {
                                            ts.Complete();
                                        }
                                    }
                                }
                            }
                        }

                    }

                    if (interfacecatalogue == 0)
                    {
                        /*************creation sans code à barre produit simple**************************/
                        if (creeerproduit == true && interfacecreation == 0)
                        {
                            proxy.AjouterProduitStockRefresh();

                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            /****************creation avec code à bare produit simple************/
                            if (creeerproduit == true && interfacecreation == 1 && crfeercab == true)
                            {
                                proxy.AjouterProduitStockRefresh();
                                proxy.AjouterTcabRefresh(special.clecab);
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        if (interfacecatalogue == 1)
                        {
                            /************************creation verre produit sans code à barre*********/
                            if (creeerproduit == true && interfacecreation == 0 && crverre == true && crtarif == true)
                            {
                                proxy.AjouterProduitStockRefresh();
                                proxy.AjouterVerreRefresh();
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                            else
                            {
                                /***********************creation verre produit avec code à barre*******/
                                if (creeerproduit == true && interfacecreation == 1 && crfeercab == true && crverre == true && crtarif == true)
                                {
                                    proxy.AjouterProduitStockRefresh();
                                    proxy.AjouterTcabRefresh(special.clecab);
                                    proxy.AjouterVerreRefresh();
                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.Close();
                                }
                            }
                        }
                        else
                        {
                            if (interfacecatalogue == 2)
                            {
                                if (creeerproduit == true && interfacecreation == 0 && crverre == true)
                                {
                                    proxy.AjouterProduitStockRefresh();
                                    proxy.AjouterLentilleRefresh();
                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.Close();
                                }
                                else
                                {
                                    if (creeerproduit == true && interfacecreation == 1 && crfeercab == true && crverre == true)
                                    {
                                        proxy.AjouterProduitStockRefresh();
                                        proxy.AjouterTcabRefresh(special.clecab);
                                        proxy.AjouterLentilleRefresh();
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        this.Close();
                                    }
                                }
                            }
                        }
                    }
                    btnCreer.IsEnabled = false;
                }
                #endregion
                else
                {
                    if (MembershipOptic.ModificationFichier == true && créationOrdonnace == 0 && txtDesign.Text != "")
                    {
                        int interfacemodf = 0;
                        bool cremodicab = false;
                        bool modifprodui = false;
                        bool modifprodf = false;
                        bool crverre = false;
                        bool crtarif = false;
                        if (op.FileName != "")
                        {
                            serverfilepath = op.FileName;

                            filepath = "";
                            if (special.photo == "")
                            {
                                if (serverfilepath != "")
                                {


                                    filepath = op.FileName;

                                    serverfilepath = @"Produit\PhotoProduit\" + (txtDesign.Text.Trim()) + ".png";

                                    byte[] buffer = null;

                                    // read the file and return the byte[
                                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, (int)fs.Length);
                                    }
                                    if (buffer != null)
                                    {
                                        proxy.UploadDocument(serverfilepath, buffer);
                                    }
                                    special.photo = serverfilepath;
                                }
                                else
                                {

                                    special.photo = "";
                                }
                            }
                            else
                            {
                                if (serverfilepath == "")
                                {
                                    special.photo = "";
                                }
                                else
                                {

                                    filepath = op.FileName;

                                    serverfilepath = @"Produit\PhotoProduit\" + (txtDesign.Text.Trim()) + ".png";

                                    byte[] buffer = null;

                                    // read the file and return the byte[
                                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, (int)fs.Length);
                                    }
                                    if (buffer != null)
                                    {
                                        proxy.UploadDocument(serverfilepath, buffer);
                                    }
                                    special.photo = serverfilepath;
                                }
                            }
                        }
                        else
                        {
                            if (op.FileName == "" && special.photo != "" && imageok == false)
                            {
                                special.photo = "";
                            }

                        }
                        if (ListeDciCombo.SelectedItem != null)
                        {
                            SVC.FamilleProduit SelectMedecin = ListeDciCombo.SelectedItem as SVC.FamilleProduit;
                            special.famille = SelectMedecin.FamilleProduit1;
                            special.IdFamille = SelectMedecin.Id;
                        }
                        else
                        {
                            special.IdFamille = 0;
                            special.famille = "";
                        }

                        if (ListeMarqueCombo.SelectedItem != null)
                        {
                            SVC.Marque SelectMedecin = ListeMarqueCombo.SelectedItem as SVC.Marque;
                            special.marque = SelectMedecin.MarqueProduit;
                            special.IdMarque = SelectMedecin.Id;
                        }
                        else
                        {
                            special.marque = "";
                            special.IdMarque = 0;
                        }
                        if (interfacecatalogue == 1)
                        {
                            string matiereverre = "";
                            if (radMat1.IsChecked == true)
                            {
                                matiereverre = "Minéral";
                            }
                            else if (radMat2.IsChecked == true)
                            {
                                matiereverre = "Organique";
                            }
                            else if (radMat3.IsChecked == true)
                            {
                                matiereverre = "Polycarbonate";
                            }
                            if (Limit1.IsChecked == true)
                            {
                                CatalogueVerre.LimiteConnu = false;
                            }
                            else if (Limit2.IsChecked == true)
                            {
                                CatalogueVerre.LimiteConnu = true;
                            }

                            CatalogueVerre.IndiceVerre = CdompteComboBox.Text;
                            CatalogueVerre.Surface = SurfaceComboBox.Text;
                            CatalogueVerre.TypeVerre = TypeVerreComboBox.Text;
                            CatalogueVerre.Matière = matiereverre;
                            CatalogueVerre.Marque = special.marque;


                        }
                        else
                        {
                            if (interfacecatalogue == 2)
                            {
                                CatalogueLentille.cleproduit = special.clecab;
                                CatalogueLentille.CodeLPP = TypeLPPComboBox.Text;
                                CatalogueLentille.Durrée = TypeDuréeComboBox.Text;
                                CatalogueLentille.FiltreUV = ckfiltreUV.IsChecked;
                                if (LimitLentille1.IsChecked == true)
                                {
                                    CatalogueLentille.LimiteConnu = false;
                                }
                                else if (LimitLentille2.IsChecked == true)
                                {
                                    CatalogueLentille.LimiteConnu = true;
                                }
                                CatalogueLentille.Matière = MatièreComboBox.Text;
                                CatalogueLentille.Packaging = TypePackagingComboBox.Text;
                                CatalogueLentille.TypeLentille = TypeLentilleComboBox.Text;
                                CatalogueLentille.Marque = special.marque;
                                CatalogueLentille.cleproduit = special.cab;
                            }
                        }

                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            #region code à barre
                            bool cabexiste = proxy.GetAllTcab().Any(n => n.cleproduit == special.clecab);
                            List<SVC.Tcab> ancienlist = new List<SVC.Tcab>();
                            if (cabexiste == true && listcodeabarre.Count > 0)
                            {
                                ancienlist = proxy.GetAllTcab().Where(n => n.cleproduit == special.clecab).ToList();
                                interfacemodf = 1;


                                foreach (SVC.Tcab itemnew in listcodeabarre)
                                {

                                    var found = (ancienlist).Find(itemf => itemf.Id == itemnew.Id);


                                    if (found == null && itemnew.cab != "")
                                    {
                                        cremodicab = false;
                                        itemnew.cleproduit = special.clecab;
                                        proxy.InsertTcab(itemnew);
                                        cremodicab = true;
                                    }
                                    else
                                    {
                                        if (found != null && itemnew.cab != "")
                                        {
                                            cremodicab = false;
                                            found.cleproduit = special.clecab;
                                            proxy.UpdateTcab(found);
                                            cremodicab = true;
                                        }

                                    }

                                }
                                /* for (int i = ancienlist.Count - 1; i >= 0; i--)

                                 {
                                     var item = ancienlist.ElementAt(i);
                                     var founddeleted = (listcodeabarre).Find(itemf => itemf.Id == item.Id);
                                     if (founddeleted == null)
                                     {
                                         /*  cremodicab = false;
                                           proxy.DeleteTcab(founddeleted);
                                           cremodicab = true;*/
                                /*       MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(item.cab, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                   }
                               }*/
                                foreach (SVC.Tcab itemdelete in ancienlist)
                                {
                                    var founddeleted = (listcodeabarre).Find(itemf => itemf.Id == itemdelete.Id);
                                    if (founddeleted == null)
                                    {
                                        cremodicab = false;
                                        //itemnew.cleproduit = special.clecab;
                                        proxy.DeleteTcab(itemdelete);
                                        cremodicab = true;
                                    }
                                }




                            }
                            else
                            {
                                if (listcodeabarre.Count > 0 && cabexiste == false)
                                {
                                    interfacemodf = 1;

                                    foreach (var itemnew in listcodeabarre)
                                    {
                                        if (itemnew.cab != "")
                                        {
                                            cremodicab = false;
                                            itemnew.cleproduit = special.clecab;
                                            proxy.InsertTcab(itemnew);
                                            cremodicab = true;
                                        }
                                        //    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(itemnew.cab, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                }
                                else
                                {
                                    if (cabexiste == true && listcodeabarre.Count == 0)
                                    {
                                        interfacemodf = 1;

                                        ancienlist = proxy.GetAllTcab().Where(n => n.cleproduit == special.clecab).ToList();

                                        foreach (var itemdelete in ancienlist)
                                        {
                                            cremodicab = false;
                                            proxy.DeleteTcab(itemdelete);
                                            cremodicab = true;
                                        }
                                    }
                                }

                            }
                            #endregion


                            if (interfacecatalogue == 1)
                            {
                                if (déjaverre == true)
                                {
                                    proxy.UpdateVerre(CatalogueVerre);
                                    crverre = true;
                                }
                                else
                                {
                                    if (déjaverre == false)
                                    {
                                        CatalogueVerre.Cletarif = special.cab;
                                        CatalogueVerre.cleproduit = special.cab;
                                        proxy.InsertVerre(CatalogueVerre);
                                        crverre = true;
                                    }
                                }

                                if (tarifexiste == false)
                                {
                                    if (listtarif.Count > 0)
                                    {
                                        foreach (var item in listtarif)
                                        {
                                            crtarif = false;
                                            item.Cletarif = CatalogueVerre.Cletarif;
                                            item.cleproduit = CatalogueVerre.cleproduit;
                                            proxy.InsertTarifVerre(item);
                                            crtarif = true;
                                        }
                                    }
                                    else
                                    {
                                        crtarif = true;
                                    }
                                }
                                else
                                {
                                    if (tarifexiste == true)
                                    {

                                        /*******************SUPPRIMER L'INCIENNE LIST)*/////////
                                        foreach (var item in listtarifanciennelist)
                                        {
                                            crtarif = false;
                                            proxy.DeleteTarifVerre(item);
                                            crtarif = true;
                                        }
                                        /***********ajouter new list*//////////////
                                        if (listtarif.Count > 0)
                                        {
                                            foreach (var item in listtarif)
                                            {
                                                crtarif = false;
                                                item.Cletarif = CatalogueVerre.Cletarif;
                                                item.cleproduit = CatalogueVerre.cleproduit;
                                                proxy.InsertTarifVerre(item);
                                                crtarif = true;
                                            }
                                        }
                                        else
                                        {
                                            crtarif = true;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (interfacecatalogue == 2)
                                {
                                    if (déjalentille == true)
                                    {
                                        proxy.UpdateLentille(CatalogueLentille);
                                        crverre = true;
                                    }
                                    else
                                    {
                                        if (déjalentille == false)
                                        {
                                            CatalogueLentille.cleproduit = special.cab;
                                            proxy.InsertLentille(CatalogueLentille);
                                            crverre = true;
                                        }
                                    }
                                }
                            }
                            if (interfacecatalogue != 2 && déjalentille == true || interfacecatalogue != 1 && déjaverre == true)
                            {
                                if (déjalentille == true)
                                {
                                    proxy.DeleteLentille(CatalogueLentille);
                                    crverre = true;
                                }
                                if (déjaverre == true)
                                {
                                    proxy.DeleteVerre(CatalogueVerre);
                                    crverre = true;
                                }
                            }
                            proxy.UpdateProduit(special);
                            modifprodui = true;


                            if (interfacecatalogue == 0)
                            {
                                if (modifprodui == true && interfacemodf == 0)
                                {
                                    ts.Complete();
                                }
                                else
                                {
                                    if (modifprodui == true && interfacemodf == 1 && cremodicab == true)
                                    {
                                        ts.Complete();

                                    }

                                }
                            }
                            else
                            {
                                if (interfacecatalogue == 1)
                                {
                                    if (modifprodui == true && interfacemodf == 0 && crverre == true && crtarif == true)
                                    {
                                        ts.Complete();
                                    }
                                    else
                                    {
                                        if (modifprodui == true && interfacemodf == 1 && cremodicab == true && crverre == true && crtarif == true)
                                        {
                                            ts.Complete();

                                        }

                                    }
                                }

                                else
                                {
                                    if (interfacecatalogue == 2)
                                    {
                                        if (modifprodui == true && interfacemodf == 0 && crverre == true)
                                        {
                                            ts.Complete();
                                        }
                                        else
                                        {
                                            if (modifprodui == true && interfacemodf == 1 && cremodicab == true && crverre == true)
                                            {
                                                ts.Complete();

                                            }

                                        }
                                    }
                                }
                            }
                        }
                        if (interfacecatalogue == 0)
                        {
                            if (modifprodui == true && interfacemodf == 0)
                            {
                                proxy.AjouterProduitStockRefresh();

                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                            else
                            {
                                if (modifprodui == true && interfacemodf == 1 && cremodicab == true)
                                {
                                    proxy.AjouterProduitStockRefresh();
                                    proxy.AjouterTcabRefresh(special.clecab);

                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.Close();

                                }
                            }
                        }
                        else
                        {
                            if (interfacecatalogue == 1)
                            {

                                if (modifprodui == true && interfacemodf == 0 && crverre == true && crtarif == true)
                                {
                                    proxy.AjouterProduitStockRefresh();
                                    proxy.AjouterVerreRefresh();

                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.Close();
                                }
                                else
                                {
                                    if (modifprodui == true && interfacemodf == 1 && cremodicab == true && crverre == true && crtarif == true)
                                    {
                                        proxy.AjouterProduitStockRefresh();
                                        proxy.AjouterTcabRefresh(special.clecab);
                                        proxy.AjouterVerreRefresh();

                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        this.Close();

                                    }
                                }

                            }
                            else
                            {
                                if (interfacecatalogue == 2)
                                {

                                    if (modifprodui == true && interfacemodf == 0 && crverre == true)
                                    {
                                        proxy.AjouterProduitStockRefresh();
                                        proxy.AjouterLentilleRefresh();

                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        this.Close();
                                    }
                                    else
                                    {
                                        if (modifprodui == true && interfacemodf == 1 && cremodicab == true && crverre == true)
                                        {
                                            proxy.AjouterProduitStockRefresh();
                                            proxy.AjouterTcabRefresh(special.clecab);
                                            proxy.AjouterLentilleRefresh();

                                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                            this.Close();

                                        }
                                    }

                                }
                            }
                        }
                    }


                    else
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }



        private void BTNCLOSTE_Click(object sender, RoutedEventArgs e)
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

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.CreationFichier == true)
                {
                    AjouterFamilleProduit cl = new AjouterFamilleProduit(proxy, null, MembershipOptic);
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

        private void btnMarque_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.CreationFichier == true)
                {
                    AjouterMarque cl = new AjouterMarque(proxy, null, MembershipOptic);
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rdesc.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Visible;
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Visible;
                    GridVerre.Visibility = Visibility.Collapsed;

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Rdesc_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rdesc.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Visible;
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Visible;
                    GridVerre.Visibility = Visibility.Collapsed;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Rtar_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rtar.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Collapsed;
                    GridVerreTarification.Visibility = Visibility.Visible;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                    GridVerre.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Rtar_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rtar.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Collapsed;
                    GridVerreTarification.Visibility = Visibility.Visible;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                    GridVerre.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PrixComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (PrixComboBox.SelectedItem != null && DiamComboBox.SelectedItem != null)
                {
                    GridTarification.IsEnabled = true;
                    ComboBoxItem cbItem = (ComboBoxItem)PrixComboBox.SelectedItem;
                    string prix = cbItem.Content.ToString();
                    var selectedDiamettre = DiamComboBox.SelectedItem as string;
                    if (prix == "Prix de vente")
                    {
                        TarifVerre = listtarif.Find(n => n.PrixVente == true && n.PrixAchat == false && n.Diamètre == selectedDiamettre);
                    }
                    else
                    {
                        if (prix == "Prix d'achat")
                        {
                            TarifVerre = listtarif.Find(n => n.PrixVente == false && n.PrixAchat == true && n.Diamètre == selectedDiamettre);
                        }
                    }


                    GridTarification.DataContext = TarifVerre;
                }
                else
                {
                    GridTarification.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void DiamComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (PrixComboBox.SelectedItem != null && DiamComboBox.SelectedItem != null)
                {
                    GridTarification.IsEnabled = true;
                    ComboBoxItem cbItem = (ComboBoxItem)PrixComboBox.SelectedItem;
                    string prix = cbItem.Content.ToString();
                    var selectedDiamettre = DiamComboBox.SelectedItem as string;
                    if (prix == "Prix de vente")
                    {
                        TarifVerre = listtarif.Find(n => n.PrixVente == true && n.PrixAchat == false && n.Diamètre == selectedDiamettre);
                    }
                    else
                    {
                        if (prix == "Prix d'achat")
                        {
                            TarifVerre = listtarif.Find(n => n.PrixVente == false && n.PrixAchat == true && n.Diamètre == selectedDiamettre);
                        }
                    }


                    GridTarification.DataContext = TarifVerre;
                }
                else
                {
                    GridTarification.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtDiam_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtDiam_KeyDown(object sender, KeyEventArgs e)
        {
            /*  try
              {

                  if (txtDiam.Text != "")
                  {

                      string line = txtDiam.Text;

                        /*  string[] col = line.Split(',');
                          for (int i = col.Count() - 1; i >= 0; i--)
                          {
                              string tt = col[i];
                              ListDiam.Add(tt);
                          }*/
            /*    ListDiam = line.Split(' ').ToList();
                DiamComboBox.ItemsSource = ListDiam;
                DiamComboBox.DataContext = ListDiam;
                DiamComboBox.IsEnabled = true;
            }
            else
            {
                DiamComboBox.IsEnabled = false;
            }
        }
        catch (Exception ex)
        {
            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

        }*/
        }



        private void Limit2_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Limit2.IsChecked == true)
                {
                    txtLimDe.IsEnabled = true;
                    txtLimA.IsEnabled = true;
                    txtLimiteAddFab0.IsEnabled = true;
                    txtLimiteAddFab1.IsEnabled = true;
                }
                else
                {
                    txtLimDe.IsEnabled = false;
                    txtLimA.IsEnabled = false;
                    txtLimDe.Text = "";
                    txtLimA.Text = "";
                    txtLimiteAddFab0.IsEnabled = false;
                    txtLimiteAddFab1.IsEnabled = false;
                    txtLimiteAddFab0.Text = "";
                    txtLimiteAddFab1.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Limit1_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Limit1.IsChecked == true)
                {
                    txtLimDe.IsEnabled = false;
                    txtLimA.IsEnabled = false;
                    txtLimiteAddFab0.IsEnabled = false;
                    txtLimiteAddFab1.IsEnabled = false;
                    txtLimDe.Text = "";
                    txtLimA.Text = "";
                }
                else
                {
                    txtLimDe.IsEnabled = true;
                    txtLimA.IsEnabled = true;
                    txtLimiteAddFab0.IsEnabled = true;
                    txtLimiteAddFab1.IsEnabled = true;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnsauv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var objectmodifed = listtarif.Find(n => n.Diamètre == TarifVerre.Diamètre && n.PrixAchat == TarifVerre.PrixAchat && n.PrixVente == TarifVerre.PrixVente);
                // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(objectmodifed.Diamètre + " " + objectmodifed.PrixAchat + " " + objectmodifed.PrixVente, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                var index = listtarif.IndexOf(objectmodifed);
                if (index != -1)
                    listtarif[index] = TarifVerre;

                /*  foreach(var item in listtarif)
                  {
                      MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(item.Diamètre+ " "+item.PrixAchat+ " "+item.PrixVente, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                  }*/
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void LimitLentille1_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LimitLentille1.IsChecked == true)
                {
                    txtLimLentilleDe.IsEnabled = false;
                    txtLimLentilleA.IsEnabled = false;
                    txtLimLentilleDe.Text = "";
                    txtLimLentilleA.Text = "";
                }
                else
                {
                    txtLimLentilleDe.IsEnabled = true;
                    txtLimLentilleA.IsEnabled = true;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void LimitLentille2_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LimitLentille2.IsChecked == true)
                {
                    txtLimLentilleDe.IsEnabled = true;
                    txtLimLentilleA.IsEnabled = true;
                }
                else
                {
                    txtLimLentilleDe.IsEnabled = false;
                    txtLimLentilleA.IsEnabled = false;
                    txtLimLentilleDe.Text = "";
                    txtLimLentilleA.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }


   /*     private void txtDesign_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try
            {
                if (txtDesign.EditValue != null && créationOrdonnace == 1)
                {

                    var query = from c in proxy.GetAllProduit()
                                select new { c.design };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.design.Trim().ToUpper() == txtDesign.Text.Trim().ToUpper()).FirstOrDefault();

                    if (disponible != null)
                    {


                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;


                    }
                    else
                    {
                        //   if (txtCodeCnas.Text.Trim() != "")
                        //  {

                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;

                        //}
                    }
                }
                else
                {

                    btnCreer.IsEnabled = true;
                    btnCreer.Opacity = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
*/
       
 

       

        private void txtStockAlert_KeyDown(object sender, KeyEventArgs e)
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
        

        private void btnCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string _numbers = Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond) + Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute);
                var LISTITEM11 = CodeDataGrid.ItemsSource as IEnumerable<SVC.Tcab>;
                listcodeabarre = LISTITEM11.ToList();
                if (proxy.GetAllTcab().Any(n => n.cab == _numbers) == false)
                {
                    SVC.Tcab add = new Tcab
                    {
                        cab = (DoWork(_numbers)).ToString(),

                    };
                    listcodeabarre.Add(add);
                }
                CodeDataGrid.ItemsSource = listcodeabarre;
                CodeDataGrid.DataContext = listcodeabarre;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        System.Int64 DoWork(string _numbers)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(13);
            string numberAsString = "";
            System.Int64 numberAsNumber = 0;

            for (var i = 0; i < 13; i++)
            {
                builder.Append(_numbers[random.Next(0, _numbers.Length)]);
            }

            numberAsString = builder.ToString();
            numberAsNumber = System.Int64.Parse(numberAsString);
            return numberAsNumber;
        }
        private Bitmap CreateBitmapImage(string sImageText)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);

            int intWidth = 0;
            int intHeight = 0;

            // Create the Font object for the image text drawing.
            Font objFont = new Font("C39HrP24DhTt", 80, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // This is where the bitmap size is determined.
            intWidth = (int)objGraphics.MeasureString(sImageText, objFont).Width;
            intHeight = (int)objGraphics.MeasureString(sImageText, objFont).Height;

            // Create the bmpImage again with the correct size for the text and font.
            objBmpImage = new Bitmap(objBmpImage, new System.Drawing.Size(intWidth, intHeight));

            // Add the colors to the new bitmap.
            objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color
            // objGraphics.Clear(System.Drawing.Color.White);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.DrawString(sImageText, objFont, new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)), 0, 0);
            objGraphics.Flush();

            return (objBmpImage);
        }
        private void btnImpressionCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CodeDataGrid.SelectedItem != null)
                {
                    SVC.Tcab selected = CodeDataGrid.SelectedItem as SVC.Tcab;

                    // Convert_Text_to_Image("*"+selected.cab+"*", "Code EAN13", 40).Save("code.png");

                    CreateBitmapImage("*" + selected.cab.Trim() + "*").Save("code.jpeg");



                    System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();
                    procInfo.FileName = ("mspaint.exe");
                    String photolocation = "code.jpeg";
                    procInfo.Arguments = @"""" + photolocation;
                    System.Diagnostics.Process.Start(procInfo);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ListeDciCombo_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (ListeDciCombo.SelectedItem != null)
                {
                    var selected = ListeDciCombo.SelectedItem as SVC.FamilleProduit;
                    if (selected.FamilleProduit1 == "Verres")
                    {
                        interfacecatalogue = 1;
                        CATitem.IsEnabled = true;
                        CATitem.Visibility = Visibility.Visible;
                        LENTitem.IsEnabled = false;
                        LENTitem.Visibility = Visibility.Collapsed;
                        if (créationOrdonnace == 0 && déjaverre == false)
                        {
                            Rdesc.IsChecked = true;
                            CatalogueVerre = new SVC.Verre
                            {
                                LimiteConnu = false,

                            };
                            radMat1.IsChecked = true;
                            Limit1.IsChecked = true;
                            GridVerre.DataContext = CatalogueVerre;
                            tarifexiste = false;
                            TarifVerre = new SVC.TarifVerre
                            {
                                A1 = 0,
                                A2 = 0,
                                A3 = 0,
                                A4 = 0,
                                B1 = 0,
                                B2 = 0,
                                B3 = 0,
                                B4 = 0,
                                C1 = 0,
                                C2 = 0,
                                C3 = 0,
                                C4 = 0,
                                D1 = 0,
                                D2 = 0,
                                D3 = 0,
                                D4 = 0,
                                E1 = 0,
                                E2 = 0,
                                E3 = 0,
                                E4 = 0,
                                F1 = 0,
                                F2 = 0,
                                F3 = 0,
                                F4 = 0,
                                G1 = 0,
                                G2 = 0,
                                G3 = 0,
                                G4 = 0,
                                H1 = 0,
                                H2 = 0,
                                H3 = 0,
                                H4 = 0,
                            };
                            GridTarification.DataContext = TarifVerre;
                        }



                    }
                    else
                    {
                        if (selected.FamilleProduit1 == "Lentilles")
                        {
                            interfacecatalogue = 2;
                            LENTitem.IsEnabled = true;
                            LENTitem.Visibility = Visibility.Visible;
                            CATitem.IsEnabled = false;
                            CATitem.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            if (selected.FamilleProduit1 != "Lentilles" && selected.FamilleProduit1 != "Verres")
                            {
                                interfacecatalogue = 0;
                                LENTitem.IsEnabled = false;
                                LENTitem.Visibility = Visibility.Collapsed;
                                CATitem.IsEnabled = false;
                                CATitem.Visibility = Visibility.Collapsed;
                            }
                        }

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
                if (ListeMarqueCombo.SelectedItem != null)
                {
                    var selected = ListeMarqueCombo.SelectedItem as SVC.Marque;
                    txtRefMarque.Text = selected.MarqueProduit;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtDiam_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tarifexiste == false)
                {
                    if (!string.IsNullOrEmpty(txtDiam.Text))
                    {
                        if (txtDiam.Text != null)
                        {

                            string line = txtDiam.Text.Trim();
                            ListDiam = line.Split(',').ToList();

                            listtarif = new List<SVC.TarifVerre>();
                            foreach (var item in ListDiam)
                            {
                                TarifVerre nn = new TarifVerre
                                {
                                    Diamètre = item,
                                    PrixAchat = true,
                                    PrixVente = false,

                                };
                                listtarif.Add(nn);
                            }
                            foreach (var item in ListDiam)
                            {
                                TarifVerre nn = new TarifVerre
                                {
                                    Diamètre = item,
                                    PrixAchat = false,
                                    PrixVente = true,

                                };
                                listtarif.Add(nn);
                            }
                            DiamComboBox.IsEnabled = true;
                            PrixComboBox.IsEnabled = true;

                            DiamComboBox.ItemsSource = ListDiam;
                            DiamComboBox.DataContext = ListDiam;
                        }
                        else
                        {
                            listtarif = new List<SVC.TarifVerre>();
                            DiamComboBox.IsEnabled = false;
                            PrixComboBox.IsEnabled = false;
                        }
                    }
                    else
                    {
                        listtarif = new List<SVC.TarifVerre>();
                        DiamComboBox.IsEnabled = false;
                        PrixComboBox.IsEnabled = false;
                    }

                }
                else
                {
                    if (tarifexiste == true)
                    {

                        //CatalogueVerre
                        string line = txtDiam.Text.Trim();
                        string ancienneline = CatalogueVerre.Diamètre;
                        List<string> ListDiamAncinne = ancienneline.Split(',').ToList();
                        ListDiam = line.Split(',').ToList();


                        foreach (var item in ListDiam)
                        {
                            if (ListDiamAncinne.Contains(item) == false)
                            {
                                TarifVerre nn = new TarifVerre
                                {
                                    Diamètre = item,
                                    PrixAchat = true,
                                    PrixVente = false,

                                };
                                listtarif.Add(nn);
                            }
                        }
                        foreach (var item in ListDiam)
                        {
                            if (ListDiamAncinne.Contains(item) == false)
                            {
                                TarifVerre nn = new TarifVerre
                                {
                                    Diamètre = item,
                                    PrixAchat = false,
                                    PrixVente = true,

                                };
                                listtarif.Add(nn);
                            }
                        }
                        DiamComboBox.IsEnabled = true;
                        PrixComboBox.IsEnabled = true;

                        DiamComboBox.ItemsSource = ListDiam;
                        DiamComboBox.DataContext = ListDiam;
                    }
                    else
                    {
                        DiamComboBox.IsEnabled = false;
                        PrixComboBox.IsEnabled = false;
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtDiamLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (déjalentille == false)
                {
                    if (!string.IsNullOrEmpty(txtDiamLentille.Text))
                    {
                        string line = txtDiamLentille.Text.Trim();
                        ListDiamLentille = line.Split(',').ToList();
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

        private void txtCourbureLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> ListDiamLentille;
            List<string> ListCourbureLentille;
        }

        
    }
}
