
using Microsoft.Win32;
using NewOptics.Administrateur;
using NewOptics.SVC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class AjouterProduit : Window
    {
        SVC.Produit special;
        SVC.ServiceCliniqueClient proxy;

        private delegate void FaultedInvokerNewProduitOrdonnance();
        SVC.MembershipOptic MembershipOptic;
        string title;
        System.Windows.Media.Brush titlebrush;
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
        public AjouterProduit(SVC.ServiceCliniqueClient proxyrecu, SVC.Produit spécialtiérecu, SVC.MembershipOptic membershirecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                special = spécialtiérecu;
                MembershipOptic = membershirecu;
                callback = callbackrecu;


                if (special != null)
                {
                    FournVousGrid.DataContext = special;
                    /*****************************famille produit*******************/
                    List<SVC.FamilleProduit> testmedecin = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1).ToList();
                    ListeDciCombo.ItemsSource = testmedecin;
                    if (special.IdFamille != 0)
                    {
                        List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == special.IdFamille).OrderBy(n => n.Id).ToList();
                        ListeDciCombo.SelectedItem = tte.First();
                    }
                    else
                    {
                        ListeDciCombo.SelectedItem = null;
                    }
                    if (special.Catalogue == true)
                    {
                        ListeDciCombo.IsEnabled = false;
                    }
                    /***************************** marque ******************************/
                    List<SVC.Marque> testmarque = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit).ToList();
                    ListeMarqueCombo.ItemsSource = testmarque;

                    if (special.IdMarque != 0)
                    {
                        List<SVC.Marque> tte = testmarque.Where(n => n.Id == special.IdMarque).OrderBy(n => n.Id).ToList();
                        ListeMarqueCombo.SelectedItem = tte.First();
                    }
                    else
                    {
                        ListeMarqueCombo.SelectedItem = null;
                    }
                    /*********************************************************************/
                    if (special.IdFamille == 19)
                    {
                        déjaverre = true;
                        ListeDciCombo.IsEnabled = false;
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
                        /************tarif***************/
                        if (CatalogueVerre.Cletarif != null)
                        {
                            bool existe = proxy.GetAllTarifVerre().Any(n => n.Cletarif == CatalogueVerre.Cletarif);
                        if (existe == true)
                        {
                            listtarif = proxy.GetAllTarifVerrebycode(CatalogueVerre.Cletarif);
                            listtarifanciennelist = proxy.GetAllTarifVerrebycode(CatalogueVerre.Cletarif);
                            tarifexiste = true;
                            string line = CatalogueVerre.Diamètre.Trim();
                            ListDiam = line.Split(',').ToList();
                          

                        }
                        else
                        {
                            tarifexiste = false;

                            }
                        }
                        else
                        {
                            tarifexiste = false;
                        }



                        /**********************************/

                    }
                    else
                    {
                        if (special.IdFamille == 10)
                        {
                            déjalentille = true;
                            ListeDciCombo.IsEnabled = false;
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
                            txtRefLentille.IsEnabled = true;
                        }
                        else
                        {
                            déjaverre = false;
                            déjalentille = false;
                        }

                    }
                    /************************************************************************/
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
                      //      btnCreer.IsEnabled = true;
                            //  btnCreer.Content = "Modifier";
                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention la photo du produit n'existe plus dans le serveur", "Medicus", MessageBoxButton.OK, MessageBoxImage.Error);
                            op.FileName = "";
                            imageok = false;
                            // patient = patientrecu;
                         //   btnCreer.IsEnabled = true;
                            //btnCreer.Content = "Modifier";
                        }
                    }


                    callbackrecu.InsertFamilleProduitCallbackevent += new ICallback.CallbackEventHandler58(callbackDci_Refresh);
                    callbackrecu.InsertMarqueCallbackevent += new ICallback.CallbackEventHandler57(callbackMarque_Refresh);

                }
                else
                {
                    special = new SVC.Produit();
                    FournVousGrid.DataContext = special;
                    listcodeabarre = new List<SVC.Tcab>();
                    CodeDataGrid.ItemsSource = listcodeabarre;
                    CodeDataGrid.DataContext = listcodeabarre;
                    ListeDciCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                    ListeDciCombo.SelectedIndex = -1;

                    ListeMarqueCombo.ItemsSource = proxy.GetAllMarque().OrderBy(n => n.MarqueProduit);
                    ListeMarqueCombo.SelectedIndex = -1;

                  //  btnCreer.IsEnabled = false;
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
                  
                    /****************************************************/
                    CatalogueLentille = new SVC.Lentille
                    {
                        LimiteConnu = false,
                    };
                    GridLentille.DataContext = CatalogueLentille;
                    /********************************************************/
                    callbackrecu.InsertFamilleProduitCallbackevent += new ICallback.CallbackEventHandler58(callbackDci_Refresh);
                    callbackrecu.InsertMarqueCallbackevent += new ICallback.CallbackEventHandler57(callbackMarque_Refresh);

                }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewProduitOrdonnance(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewProduitOrdonnance(HandleProxy));
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
                        if(!string.IsNullOrEmpty(txtCyl.Text) && !string.IsNullOrEmpty(txtSph.Text))
                        {
                            CatalogueVerre.Design = special.design +" "+ CdompteComboBox.Text+" "+txtCyl.Text+" "+txtSph.Text;
                            CatalogueVerre.Ref = special.design + " " + CdompteComboBox.Text + " " + txtCyl.Text + " " + txtSph.Text;

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(txtCyl.Text) && string.IsNullOrEmpty(txtSph.Text))
                            {
                                CatalogueVerre.Design = special.design + " " + CdompteComboBox.Text + " " + txtCyl.Text;
                                CatalogueVerre.Ref = special.design + " " + CdompteComboBox.Text + " " + txtCyl.Text;

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(txtCyl.Text) && !string.IsNullOrEmpty(txtSph.Text))
                                {
                                    CatalogueVerre.Design = special.design + " " + CdompteComboBox.Text + " "  + txtSph.Text;
                                    CatalogueVerre.Ref = special.design + " " + CdompteComboBox.Text + " " + txtSph.Text;

                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(txtCyl.Text) && string.IsNullOrEmpty(txtSph.Text))
                                    {
                                        CatalogueVerre.Design = special.design + " " + CdompteComboBox.Text ;
                                        CatalogueVerre.Ref = special.design + " " + CdompteComboBox.Text;

                                    }
                                }
                            }
                        }
                        CatalogueVerre.IndiceVerre = CdompteComboBox.Text;
                        CatalogueVerre.Surface = SurfaceComboBox.Text;
                        CatalogueVerre.TypeVerre = TypeVerreComboBox.Text;
                        CatalogueVerre.cleproduit = special.cab;
                        CatalogueVerre.Matière = matiereverre;
                        CatalogueVerre.Marque = special.marque;
                        CatalogueVerre.PrixRevient = special.PrixRevient;
                        CatalogueVerre.PrixVente = special.PrixVente;
                        if(!string.IsNullOrEmpty(txtSph.Text))
                        {
                            CatalogueVerre.Sph = Convert.ToDecimal(txtSph.Text);
                        }
                        else
                        {
                            CatalogueVerre.Sph = null;
                        }
                        if (!string.IsNullOrEmpty(txtCyl.Text))
                        {
                            CatalogueVerre.Cyl = Convert.ToDecimal(txtCyl.Text);
                        }
                        else
                        {
                            CatalogueVerre.Cyl = null;
                        }
                    }
                    else
                    {
                        if (interfacecatalogue == 2)
                        {
                           // CatalogueLentille.Design = special.design;
                            if(!string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                            {
                                CatalogueLentille.Design = special.design + " " + txtCylLentille.Text + " " + txtSphLentille.Text;
                                CatalogueLentille.Ref= special.design + " " + txtCylLentille.Text + " " + txtSphLentille.Text;

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                {
                                    CatalogueLentille.Design = special.design + " " + txtSphLentille.Text;
                                    CatalogueLentille.Ref = special.design + " " + txtSphLentille.Text;

                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                    {
                                        CatalogueLentille.Design = special.design + " " + txtCylLentille.Text;
                                        CatalogueLentille.Ref = special.design + " " + txtCylLentille.Text;

                                    }
                                    else
                                    {

                                        if (string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                        {
                                            CatalogueLentille.Design = special.design;
                                            CatalogueLentille.Ref = special.design;

                                        }
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(txtSphLentille.Text))
                            {
                                CatalogueLentille.Sph = Convert.ToDecimal(txtSphLentille.Text);
                            }
                          /*  else
                            {
                                CatalogueLentille.Sph =null;
                            }*/
                            if (!string.IsNullOrEmpty(txtCylLentille.Text))
                            {
                                CatalogueLentille.Cyl = Convert.ToDecimal(txtCylLentille.Text);
                            }
                            /*  else
                             {
                                 CatalogueLentille.Cyl = null;
                             }*/
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
                           //  CatalogueLentille.Ref = special.design;
                             CatalogueLentille.PrixRevient = special.PrixRevient;
                             CatalogueLentille.PrixVente = special.PrixVente;


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

                             if (listtarif.Count > 0)
                             {
                                 List<string> listObjects = (from obj in listtarif
                                                             select obj.Diamètre).Distinct().ToList();
                                 foreach (var item in listObjects)
                                 {
                                     SVC.Verre ll = new SVC.Verre
                                     {
                                         Aphaque = CatalogueVerre.Aphaque,
                                         cleproduit = CatalogueVerre.cleproduit + item,
                                         Cletarif = CatalogueVerre.cleproduit + item,
                                         //  Design = CatalogueVerre.Design + " " + item,
                                         //Design = special.design + " " + item + " " + CdompteComboBox.Text,
                                         Diamètre = item,
                                         Fournisseur = CatalogueVerre.Fournisseur,
                                         IndiceVerre = CdompteComboBox.Text,
                                         LimiteAddFab0 = CatalogueVerre.LimiteAddFab0,
                                         LimiteAddFab1 = CatalogueVerre.LimiteAddFab1,
                                         LimiteConnu = CatalogueVerre.LimiteConnu,
                                         LimiteFab0 = CatalogueVerre.LimiteFab0,
                                         LimiteFab1 = CatalogueVerre.LimiteFab1,
                                         Marque = CatalogueVerre.Marque,
                                         Matière = CatalogueVerre.Matière,
                                         Photochromique = CatalogueVerre.Photochromique,
                                         // Ref = CatalogueVerre.Design + " " + item,
                                       //  Ref = special.design + " " + item + " " + CdompteComboBox.Text,
                                         Surface = CatalogueVerre.Surface,
                                         Teinte = CatalogueVerre.Teinte,
                                         TypeVerre = CatalogueVerre.TypeVerre,
                                         VerreTeinté = CatalogueVerre.VerreTeinté,
                                         Cyl=CatalogueVerre.Cyl,
                                         Sph=CatalogueVerre.Sph,
                                         PrixVente=special.PrixVente,
                                         PrixRevient=special.PrixRevient,
                                     };
                                     if (!string.IsNullOrEmpty(txtCyl.Text) && !string.IsNullOrEmpty(txtSph.Text))
                                     {
                                         ll.Design = special.design + " "+item+" " + CdompteComboBox.Text + " " + txtCyl.Text + " " + txtSph.Text;
                                         ll.Ref = special.design + " " +item+" "+ CdompteComboBox.Text + " " + txtCyl.Text + " " + txtSph.Text;

                                     }
                                     else
                                     {
                                         if (!string.IsNullOrEmpty(txtCyl.Text) && string.IsNullOrEmpty(txtSph.Text))
                                         {
                                             ll.Design = special.design + " "+item+" " + CdompteComboBox.Text + " " + txtCyl.Text;
                                             ll.Ref = special.design + " "+item+" " + CdompteComboBox.Text + " " + txtCyl.Text;

                                         }
                                         else
                                         {
                                             if (string.IsNullOrEmpty(txtCyl.Text) && !string.IsNullOrEmpty(txtSph.Text))
                                             {
                                                 ll.Design = special.design + " " +item+" "+ CdompteComboBox.Text + " " + txtSph.Text;
                                                 ll.Ref = special.design + " " +item+" "+ CdompteComboBox.Text + " " + txtSph.Text;

                                             }
                                             else
                                             {
                                                 if (string.IsNullOrEmpty(txtCyl.Text) && string.IsNullOrEmpty(txtSph.Text))
                                                 {
                                                     ll.Design = special.design + " "+item+" "+ CdompteComboBox.Text;
                                                     ll.Ref = special.design + " " +item+" "+ CdompteComboBox.Text;

                                                 }
                                             }
                                         }
                                     }

                                     crverre = false;
                                     proxy.InsertVerre(ll);
                                     crverre = true;
                                     creeerproduit = false;
                                     SVC.Produit dd = new Produit
                                     {
                                         cab = CatalogueVerre.cleproduit + item,
                                         design = ll.Design,
                                         clecab = special.clecab,
                                         dates = special.dates,
                                         famille = special.famille,
                                         IdFamille = special.IdFamille,
                                         IdMarque = special.IdMarque,
                                         marque = special.marque,
                                         photo = special.photo,
                                         StockAlert = special.StockAlert,
                                         typevente = special.typevente,
                                         PrixRevient=special.PrixRevient,
                                         PrixVente=special.PrixVente,
                                     };

                                     proxy.InsertProduit(dd);
                                     creeerproduit = true;
                                 }
                                 foreach (var item in listtarif)
                                 {
                                     crtarif = false;
                                    // item.Cletarif = CatalogueVerre.cleproduit + item.Diamètre;
                                     //item.cleproduit = CatalogueVerre.cleproduit + item.Diamètre;
                                     //proxy.InsertTarifVerre(item);
                                     crtarif = true;
                                 }
 //                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("1", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                             }
                             else
                             {
                                 crverre = true;
                                 proxy.InsertVerre(CatalogueVerre);
                                 crverre = true;
                                 crtarif = true;
                                 creeerproduit = false;
                                 special.design = CatalogueVerre.Design;
                                 proxy.InsertProduit(special);
                                 creeerproduit = true;
                               //  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("2", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                             }
                         }
                         else
                         {
                             if (interfacecatalogue == 2)
                             {
                                 if (txtDiamLentille.Text != null)
                                 {
                                     string line = txtDiamLentille.Text.Trim();
                                     ListDiamLentille = line.Split(',').ToList();
                                 }
                                 else
                                 {
                                     ListDiamLentille = new List<string>();
                                 }
                                 if (txtCourbureLentille.Text != null)
                                 {
                                     string line = txtCourbureLentille.Text.Trim();
                                     ListCourbureLentille = line.Split(',').ToList();
                                 }
                                 else
                                 {
                                     ListCourbureLentille = new List<string>();
                                 }
                                 if (ListDiamLentille.Count > 0)
                                 {
                                     foreach (var item in ListDiamLentille)
                                     {
                                         if (ListCourbureLentille.Count > 0)
                                         {
                                             foreach (var itemcour in ListCourbureLentille)
                                             {
                                                 crverre = false;
                                                SVC.Lentille ll = new SVC.Lentille
                                                {
                                                    cleproduit = CatalogueLentille.cleproduit + item + itemcour,
                                                    CodeLPP = CatalogueLentille.CodeLPP,
                                                    Couleur = CatalogueLentille.Couleur,
                                                    Courbure = itemcour,
                                                    //   Design = CatalogueLentille.Design + " " + "d: " + item + "/" + "c: " + itemcour,
                                                    ///////////   Design = special.design,// + " " + "d: " + item + "/" + "c: " + itemcour,

                                                    Diamètre = item,
                                                    Durrée = CatalogueLentille.Durrée,
                                                    FiltreUV = CatalogueLentille.FiltreUV,
                                                    Fournisseur = CatalogueLentille.Fournisseur,
                                                    LimiteConnu = CatalogueLentille.LimiteConnu,
                                                    LimiteFab0 = CatalogueLentille.LimiteFab0,
                                                    LimiteFab1 = CatalogueLentille.LimiteFab1,
                                                    Marque = CatalogueLentille.Marque,
                                                    Matière = CatalogueLentille.Matière,
                                                    Modele = CatalogueLentille.Modele,
                                                    Packaging = CatalogueLentille.Packaging,
                                                    PrixRevient = CatalogueLentille.PrixRevient,
                                                    PrixVente = CatalogueLentille.PrixVente,
                                                    //Ref = CatalogueLentille.Ref,
                                                    TypeLentille = CatalogueLentille.TypeLentille,
                                                    // Sph = Convert.ToDecimal(txtSphLentille.Text),
                                                    // Cyl = Convert.ToDecimal(txtCylLentille.Text),
                                                    Sph = CatalogueLentille.Sph,
                                                    Cyl = CatalogueLentille.Cyl,
                                                };
                                                 if (!string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                                 {
                                                     ll.Design = special.design + " " +"(D"+item+"/C"+itemcour+")"+" "+ txtCylLentille.Text + " " + txtSphLentille.Text;
                                                     ll.Ref = special.design + " " + "(D" + item + "/C" + itemcour + ")" +" "+ txtCylLentille.Text + " " + txtSphLentille.Text;

                                                 }
                                                 else
                                                 {
                                                     if (!string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                                     {
                                                         ll.Design = special.design + " " + "(D" + item + "/C" + itemcour + ")" +" "+ txtSphLentille.Text;
                                                         ll.Ref = special.design + " " + "(D" + item + "/C" + itemcour + ")" +" "+ txtSphLentille.Text;

                                                     }
                                                     else
                                                     {
                                                         if (string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                                         {
                                                             ll.Design = special.design + " " + "(D" + item + "/C" + itemcour + ")" +" "+ txtCylLentille.Text;
                                                             ll.Ref = special.design + " " + "(D" + item + "/C" + itemcour + ")" +" "+ txtCylLentille.Text;

                                                         }
                                                         else
                                                         {

                                                             if (string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                                             {
                                                                 ll.Design = special.design + " " + "(D" + item + "/C" + itemcour + ")";
                                                                 ll.Ref = special.design + " " + "(D" + item + "/C" + itemcour + ")";

                                                             }
                                                         }
                                                     }
                                                 }
                                                 proxy.InsertLentille(ll);
                                                 crverre = true;
                                                 creeerproduit = false;
                                                 SVC.Produit pp = new Produit
                                                 {
                                                     cab = CatalogueLentille.cleproduit + item + itemcour,
                                                     clecab = special.clecab,
                                                     dates = special.dates,
                                                     design = ll.Design,// + " " + "d: " + item + "/" + "c: " + itemcour,
                                                     famille = special.famille,
                                                     IdFamille = special.IdFamille,
                                                     IdMarque = special.IdMarque,
                                                     marque = special.marque,
                                                     photo = special.photo,
                                                     StockAlert = special.StockAlert,
                                                     typevente = special.typevente,
                                                     PrixRevient = special.PrixRevient,
                                                     PrixVente = special.PrixVente,

                                                 };
                                                 proxy.InsertProduit(pp);
                                                 creeerproduit = true;
                                             }
                                         }
                                         else
                                         {
                                             crverre = false;
                                             SVC.Lentille ll = new SVC.Lentille
                                             {
                                                 cleproduit = CatalogueLentille.cleproduit + item,
                                                 CodeLPP = CatalogueLentille.CodeLPP,
                                                 Couleur = CatalogueLentille.Couleur,
                                                 Courbure = CatalogueLentille.Courbure,
                                                 // Design = CatalogueLentille.Design + " " + "d: " + item,
                                              //   Design = CatalogueLentille.Design,//+ " " + "d: " + item,

                                                 Diamètre = item,
                                                 Durrée = CatalogueLentille.Durrée,
                                                 FiltreUV = CatalogueLentille.FiltreUV,
                                                 Fournisseur = CatalogueLentille.Fournisseur,
                                                 LimiteConnu = CatalogueLentille.LimiteConnu,
                                                 LimiteFab0 = CatalogueLentille.LimiteFab0,
                                                 LimiteFab1 = CatalogueLentille.LimiteFab1,
                                                 Marque = CatalogueLentille.Marque,
                                                 Matière = CatalogueLentille.Matière,
                                                 Modele = CatalogueLentille.Modele,
                                                 Packaging = CatalogueLentille.Packaging,
                                                 PrixRevient = CatalogueLentille.PrixRevient,
                                                 PrixVente = CatalogueLentille.PrixVente,
                                               //  Ref = CatalogueLentille.Ref,
                                                 TypeLentille = CatalogueLentille.TypeLentille,
                                                 //Sph = Convert.ToDecimal(txtSphLentille.Text),
                                                 // = Convert.ToDecimal(txtCylLentille.Text),
                                                 Sph = CatalogueLentille.Sph,
                                                 Cyl = CatalogueLentille.Cyl,
                                             };
                                             if (!string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                             {
                                                 ll.Design = special.design + " " + "(D" + item  + ")" + " " + txtCylLentille.Text + " " + txtSphLentille.Text;
                                                 ll.Ref = special.design + " " + "(D" + item +  ")" + " " + txtCylLentille.Text + " " + txtSphLentille.Text;

                                             }
                                             else
                                             {
                                                 if (!string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                                 {
                                                     ll.Design = special.design + " " + "(D" + item  + ")" + " " + txtSphLentille.Text;
                                                     ll.Ref = special.design + " " + "(D" + item + ")" + " " + txtSphLentille.Text;

                                                 }
                                                 else
                                                 {
                                                     if (string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                                     {
                                                         ll.Design = special.design + " " + "(D" + item +  ")" + " " + txtCylLentille.Text;
                                                         ll.Ref = special.design + " " + "(D" + item +  ")" + " " + txtCylLentille.Text;

                                                     }
                                                     else
                                                     {

                                                         if (string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                                         {
                                                             ll.Design = special.design + " " + "(D" + item + ")";
                                                             ll.Ref = special.design + " " + "(D" + item + ")";

                                                         }
                                                     }
                                                 }
                                             }
                                             proxy.InsertLentille(ll);
                                             crverre = true;
                                             creeerproduit = false;
                                             SVC.Produit pp = new Produit
                                             {
                                                 cab = CatalogueLentille.cleproduit + item,
                                                 clecab = special.clecab,
                                                 dates = special.dates,
                                                 design = ll.Design,// + " " + "d: " + item,
                                                 famille = special.famille,
                                                 IdFamille = special.IdFamille,
                                                 IdMarque = special.IdMarque,
                                                 marque = special.marque,
                                                 photo = special.photo,
                                                 StockAlert = special.StockAlert,
                                                 typevente = special.typevente,
                                                 PrixRevient = special.PrixRevient,
                                                 PrixVente = special.PrixVente,
                                             };
                                             proxy.InsertProduit(pp);
                                             creeerproduit = true;
                                         }
                                     }
                                 }
                                 else
                                 {
                                     if (ListCourbureLentille.Count > 0)
                                     {
                                         foreach (var itemcour in ListCourbureLentille)
                                         {
                                             crverre = false;
                                             SVC.Lentille ll = new SVC.Lentille
                                             {
                                                 cleproduit = CatalogueLentille.cleproduit + itemcour,
                                                 CodeLPP = CatalogueLentille.CodeLPP,
                                                 Couleur = CatalogueLentille.Couleur,
                                                 Courbure = itemcour,
                                                 // Design = CatalogueLentille.Design + " " + "c: " + itemcour,
                                                // Design = CatalogueLentille.Design,// + " " + "c: " + itemcour,

                                                 Diamètre = itemcour,
                                                 Durrée = CatalogueLentille.Durrée,
                                                 FiltreUV = CatalogueLentille.FiltreUV,
                                                 Fournisseur = CatalogueLentille.Fournisseur,
                                                 LimiteConnu = CatalogueLentille.LimiteConnu,
                                                 LimiteFab0 = CatalogueLentille.LimiteFab0,
                                                 LimiteFab1 = CatalogueLentille.LimiteFab1,
                                                 Marque = CatalogueLentille.Marque,
                                                 Matière = CatalogueLentille.Matière,
                                                 Modele = CatalogueLentille.Modele,
                                                 Packaging = CatalogueLentille.Packaging,
                                                 PrixRevient = CatalogueLentille.PrixRevient,
                                                 PrixVente = CatalogueLentille.PrixVente,
                                             //    Ref = CatalogueLentille.Ref,
                                                 TypeLentille = CatalogueLentille.TypeLentille,
                                                 //     Sph = Convert.ToDecimal(txtSphLentille.Text),
                                                 //   Cyl = Convert.ToDecimal(txtCylLentille.Text),
                                                 Sph = CatalogueLentille.Sph,
                                                 Cyl = CatalogueLentille.Cyl,
                                             };
                                             if (!string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                             {
                                                 ll.Design = special.design + " " + "(C" + itemcour + ")" + " " + txtCylLentille.Text + " " + txtSphLentille.Text;
                                                 ll.Ref = special.design + " " + "(C" + itemcour + ")" + " " + txtCylLentille.Text + " " + txtSphLentille.Text;

                                             }
                                             else
                                             {
                                                 if (!string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                                 {
                                                     ll.Design = special.design + " " + "(C" + itemcour + ")" + " " + txtSphLentille.Text;
                                                     ll.Ref = special.design + " " + "(C" + itemcour + ")" + " " + txtSphLentille.Text;

                                                 }
                                                 else
                                                 {
                                                     if (string.IsNullOrEmpty(txtSphLentille.Text) && !string.IsNullOrEmpty(txtCylLentille.Text))
                                                     {
                                                         ll.Design = special.design + " " + "(C" + itemcour + ")" + " " + txtCylLentille.Text;
                                                         ll.Ref = special.design + " " + "(C" + itemcour + ")" + " " + txtCylLentille.Text;

                                                     }
                                                     else
                                                     {

                                                         if (string.IsNullOrEmpty(txtSphLentille.Text) && string.IsNullOrEmpty(txtCylLentille.Text))
                                                         {
                                                             ll.Design = special.design + " " + "(C" + itemcour + ")";
                                                             ll.Ref = special.design + " " + "(C" + itemcour + ")";

                                                         }
                                                     }
                                                 }
                                             }
                                             proxy.InsertLentille(ll);
                                             crverre = true;
                                             creeerproduit = false;
                                             SVC.Produit pp = new Produit
                                             {
                                                 cab = CatalogueLentille.cleproduit + itemcour,
                                                 clecab = special.clecab,
                                                 dates = special.dates,
                                                 design = ll.Design,//+ " " + "c: " + itemcour,
                                                 famille = special.famille,
                                                 IdFamille = special.IdFamille,
                                                 IdMarque = special.IdMarque,
                                                 marque = special.marque,
                                                 photo = special.photo,
                                                 StockAlert = special.StockAlert,
                                                 typevente = special.typevente,
                                                 PrixRevient = special.PrixRevient,
                                                 PrixVente = special.PrixVente,
                                             };
                                             proxy.InsertProduit(pp);
                                             creeerproduit = true;
                                         }
                                     }
                                     else
                                     {
                                         //  MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("I'm here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                         crverre = false;
                                         SVC.Lentille ll = new SVC.Lentille
                                         {
                                             cleproduit = CatalogueLentille.cleproduit,
                                             CodeLPP = CatalogueLentille.CodeLPP,
                                             Couleur = CatalogueLentille.Couleur,
                                             Courbure = CatalogueLentille.Courbure,
                                             Design = CatalogueLentille.Design,
                                             Diamètre = CatalogueLentille.Diamètre,
                                             Durrée = CatalogueLentille.Durrée,
                                             FiltreUV = CatalogueLentille.FiltreUV,
                                             Fournisseur = CatalogueLentille.Fournisseur,
                                             LimiteConnu = CatalogueLentille.LimiteConnu,
                                             LimiteFab0 = CatalogueLentille.LimiteFab0,
                                             LimiteFab1 = CatalogueLentille.LimiteFab1,
                                             Marque = CatalogueLentille.Marque,
                                             Matière = CatalogueLentille.Matière,
                                             Modele = CatalogueLentille.Modele,
                                             Packaging = CatalogueLentille.Packaging,
                                             PrixRevient = CatalogueLentille.PrixRevient,
                                             PrixVente = CatalogueLentille.PrixVente,
                                             Ref = CatalogueLentille.Ref,
                                             TypeLentille = CatalogueLentille.TypeLentille,
                                             //Sph = Convert.ToDecimal(txtSphLentille.Text),
                                            // Cyl = Convert.ToDecimal(txtCylLentille.Text),
                                             Sph = CatalogueLentille.Sph,
                                             Cyl = CatalogueLentille.Cyl,
                                         };

                                         proxy.InsertLentille(ll);
                                         crverre = true;
                                         creeerproduit = false;
                                         SVC.Produit pp = new Produit
                                         {
                                             cab = CatalogueLentille.cleproduit,
                                             clecab = special.clecab,
                                             dates = special.dates,
                                             design =CatalogueLentille.Design,
                                             famille = special.famille,
                                             IdFamille = special.IdFamille,
                                             IdMarque = special.IdMarque,
                                             marque = special.marque,
                                             photo = special.photo,
                                             StockAlert = special.StockAlert,
                                             typevente = special.typevente,
                                             PrixRevient = special.PrixRevient,
                                             PrixVente = special.PrixVente,
                                         };
                                         proxy.InsertProduit(pp);
                                         creeerproduit = true;
                                     }
                                 }
                             }
                             else
                             {

                                 /*crverre = false;
                                 proxy.InsertLentille(CatalogueLentille);
                                 crverre = true;*/
                            creeerproduit = false;
                                ///special.design = CatalogueLentille.Design;
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

                                    MessageBoxResult resul0h3 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
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
                            CatalogueVerre.PrixRevient = special.PrixRevient;
                            CatalogueVerre.PrixVente = special.PrixVente;

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
                                CatalogueLentille.PrixVente = special.PrixVente;
                                CatalogueLentille.PrixRevient = special.PrixRevient;
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
                
                }
                else
                {
             
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
                  
                }
                else
                {
         
                    GridVerre.Visibility = Visibility.Collapsed;

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
       

        private void txtSph_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Add:
                case Key.Subtract:
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
                string _numbers =  Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond)+ Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute);
               var LISTITEM11 = CodeDataGrid.ItemsSource as IEnumerable<SVC.Tcab>;
                listcodeabarre = LISTITEM11.ToList();
                 if (proxy.GetAllTcab().Any(n=>n.cab==_numbers)==false)
                  {
                     SVC.Tcab add = new Tcab
                     {
                         cab= (DoWork(_numbers)).ToString(),

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
         public static Bitmap Convert_Text_to_Image(string txt, string fontname, int fontsize)
         {
             //creating bitmap image
             Bitmap bmp = new Bitmap(10, 10);

             //FromImage method creates a new Graphics from the specified Image.
             Graphics graphics = Graphics.FromImage(bmp);
            // Create the Font object for the image text drawing.
            Font font = new Font(fontname, fontsize);
            
             // Instantiating object of Bitmap image again with the correct size for the text and font.
             SizeF stringSize = graphics.MeasureString(txt, font);
             bmp = new Bitmap(bmp, (int)stringSize.Width, (int)stringSize.Height);
             graphics = Graphics.FromImage(bmp);

        //  It can also be a way
           // bmp = new Bitmap(bmp, new Size((int)graphics.MeasureString(txt, font).Width, (int)graphics.MeasureString(txt, font).Height));*/

        //Draw Specified text with specified format 
          graphics.DrawString(txt, font, System.Drawing.Brushes.Black, 0, 0);
             font.Dispose();
             graphics.Flush();
             graphics.Dispose();
             return bmp;     //return Bitmap Image 
        }
       private Bitmap CreateBitmapImage(string sImageText)
 {
    Bitmap objBmpImage = new Bitmap(1, 1);
  
     int intWidth = 0;
      int intHeight = 0;
   
     // Create the Font object for the image text drawing.
      Font objFont = new Font("C39HrP24DhTt",80, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
  
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
                if (CodeDataGrid.SelectedItem!=null)
                {
                    SVC.Tcab selected = CodeDataGrid.SelectedItem as SVC.Tcab;

                    // Convert_Text_to_Image("*"+selected.cab+"*", "Code EAN13", 40).Save("code.png");

                    CreateBitmapImage("*" + selected.cab.Trim()+ "*").Save("code.jpeg");



                    System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();
                    procInfo.FileName = ("mspaint.exe");
                    String photolocation = "code.jpeg";
                    procInfo.Arguments = @""""+photolocation;
                    System.Diagnostics.Process.Start(procInfo);

                }
            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtDesign_TextChanged(object sender, TextChangedEventArgs e)
        {

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
                    if (txtDiamLentille.Text!= null)
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
