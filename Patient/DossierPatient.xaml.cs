using GestionClinique.Administrateur;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Word = Microsoft.Office.Interop.Word;
using System.Text;
using GestionClinique.Caisse;

using GestionClinique.RendezVous;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using DevExpress.Xpf.Core;
using GestionClinique.Stock;

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for DossierPatient.xaml
    /// </summary>
    public partial class DossierPatient : DXWindow  
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Patient PATIENT;
        ICallback callback;
        SVC.Membership memberuser;
        bool modif = false,modifcas=false,modifvisite=false;
        private delegate void FaultedInvokerDossierPatient();
        public enum ImageBitsPerPixel { Eight, Sixteen, TwentyFour };
        public enum ViewSettings { Zoom1_1, ZoomToFit };
        DicomDecoder dd;
        List<byte> pixels8;
        List<ushort> pixels16;
        List<byte> pixels24; // 30 July 2010
        int imageWidth;
        int imageHeight;
        int bitDepth;
        int samplesPerPixel;  // Updated 30 July 2010
        bool imageOpened;
        BinaryReader readBinary;
        double winCentre;
        double winWidth;
        bool signedImage;
        int maxPixelValue;    // Updated July 2012
        int minPixelValue;
        Microsoft.Win32.OpenFileDialog op,ofd=null;
        string serverfilepath, filepath;
        SVC.BesoinCalorique BesoinCaloriqueCalculé;
        List<SVC.Repa> PetitDejList;
        List<SVC.Repa> CollationList;
        List<SVC.Repa> DéjeunerList;
        List<SVC.Repa> GouterList;
        List<SVC.Repa> DinerList;
        List<SVC.Repa> LeSoirList;
        int TabInterface=0;
        string NumRendezVous;
        SVC.Medecin MedecinConsult;
        SVC.EnteteOrdonnace EnteteOrdonnanceSelect;
        bool OrdonnanceOuvert = false;
        List<SVC.OrdonnancePatient> OrdonnancePatientList;
        SVC.Constante constanteCréer;
        SVC.Constante constanteCréerVisiteASSI;
        bool constanteAnamnéseExiste = false;
        bool InterogatoireAnamnéseExiste = false;
        SVC.Diagnostic DiagnosticCréer;
        bool tabitemhygiene = false;
        bool newcastraiter, modifcastraiter = false, newprohèse, modifprothèse = false;
        bool simplevisite = false;
        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        SVC.autosurveillance autoSelected;
        DXDialog dialog1;
        SVC.Diagnostic dia;
        bool visitewithdiag = false;
        List<SVC.AutreBilan> autrebilanlist;
        public DossierPatient(SVC.ServiceCliniqueClient proxyrecu,SVC.Patient patientrecu,ICallback callbackrecu,SVC.Membership memberrecu,string NrendezVous,SVC.Medecin MedecinRecu,SVC.Client localclient)
        {
            try
            {
                InitializeComponent();
                if (MedecinRecu != null)
                {
                    MedecinConsult = MedecinRecu;

               
                }
                             
                    proxy = proxyrecu;
                    PATIENT = patientrecu;
                    callback = callbackrecu;
                    memberuser = memberrecu;
                    /*******visite**************/
                callbackrecu.InsertVisiteCallbackEvent += new ICallback.CallbackEventHandler13(callbackrecu_Refresh);
                txtMotifVisite.ItemsSource = proxy.GetAllTraitement().Where(n => n.CodePatient == PATIENT.Id && n.Devis == false  /*&& n.ModeFacturationint!=0*/ ).ToList();
                TypeDeVisite.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                SIMPLEDataGrid.ItemsSource = proxy.GetAllVisiteByVisite(PATIENT.Id).ToList();
                TypeDeActesTraitement.ItemsSource = proxy.GetAllActe().OrderBy(n => n.Libelle);
                callback.InsertActeCallbackevent += new ICallback.CallbackEventHandler35(callbackrecuActe_Refresh);
                /******************cas a traiter*********************/
                callbackrecu.InsertCasTraitementCallbackEvent += new ICallback.CallbackEventHandler12(callbackrecucastraitement_Refresh);
                 /*   CasDataGrid.ItemsSource = proxy.GetAllTraitement().Where(n => n.CodePatient == PATIENT.Id).ToList();
                    txtDepuis.SelectedDate = DateTime.Now;*/

                    /******************Constante***********************/
                    callbackrecu.InsertConstanteCallbackEvent += new ICallback.CallbackEventHandler14(callbackrecuConstante_Refresh);
                    DatagridContstante.ItemsSource = proxy.GetAllConstantes().Where(n => n.CodePatient == PATIENT.Id).ToList();
                    MemoryStream memLicStream3 = new MemoryStream(ASCIIEncoding.Default.GetBytes(GestionClinique.Properties.Resources.IMC));
                    this.richTextBoxIMC.Selection.Load(memLicStream3, System.Windows.DataFormats.Rtf);
                    /********************dicomfichier*****************/
                    callbackrecu.InsertDicomFichierCallbackEvent += new ICallback.CallbackEventHandler15(callbackrecuDicomFichier_Refresh);
                    DatagridImagerie.ItemsSource = proxy.GetAllDicomFichier().Where(n => n.CodePatient == PATIENT.Id && n.Image == false && n.Dicom == true).ToList();
                    btndéfiltrerDicom.IsEnabled = false;



                /**************************détail patient dans ************************************/

                PatientGrid.DataContext = PATIENT;
                PatientDataGrid.ItemsSource = proxy.GetAllVisiteByVisite(PATIENT.Id).ToList();
                if (PATIENT.CheminPhoto.ToString() != "")
                {
                    imgPhoto.Source = LoadImage(proxy.DownloadDocument(PATIENT.CheminPhoto.ToString()));

                }
                else
                {

                    BitmapImage image = new BitmapImage(new Uri("/Diabetosoft;component/Images/PatientInconu.jpg", UriKind.Relative));
                    imgPhoto.Source = image;
                }



                /***************************Ordonnance****************************/
                OrdonnanceDatagGrid.ItemsSource = proxy.GetAllEnteteOrdonnace().Where(n => n.CodePatient == PATIENT.Id).ToList();
                callbackrecu.InsertEnteteOrdonnaceCallbackevent += new ICallback.CallbackEventHandler37(callbackrecuEntete_Refresh);
                txtDateOrdonnace.SelectedDate = DateTime.Now;
                chimprimerOrdo.IsChecked = false;
                chimprimerA4.IsChecked = false;
                /*   MedicamentsDataGrid.ItemsSource = (proxy.GetAllProduitOrdonnance()).OrderBy(r => r.Design);
                         cbDci.ItemsSource = proxy.GetAllDci().OrderBy(r => r.Dci1);
                         callbackrecu.InsertOrdonnancePatientCallbackevent += new ICallback.CallbackEventHandler38(callbackrecuOrdonnancePatient_Refresh);


                  


                           /*********************************réglment patient***************************************/



                txtLabelTotalAregler.Content = Convert.ToString((proxy.GetAllVisiteAll().Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle)).AsEnumerable().Sum(o => o.Montant));
                txtLabelTotalversement.Content = Convert.ToString((proxy.GetAllVisiteAll().Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle)).AsEnumerable().Sum(o => o.Versement));
                txtLabelTotalReste.Content = Convert.ToString((proxy.GetAllVisiteAll().Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle)).AsEnumerable().Sum(o => o.Reste));

                HistoriqueDesDataGrid.ItemsSource = proxy.GetAllVisiteAll().Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle).OrderBy(r => r.Date);


                /*******************************galerie image************************************/
                DatagridImage1.ItemsSource = proxy.GetAllDicomFichier().Where(n => n.CodePatient == PATIENT.Id && n.Image == true && n.Dicom == false).ToList();


                    /***************************Hygiéne alimentaire*******************************************************/
                    List<KeyValuePair<string, int>> MyValue = new List<KeyValuePair<string, int>>();
                    MyValue.Add(new KeyValuePair<string, int>("Administration", 20));
                    MyValue.Add(new KeyValuePair<string, int>("Management", 36));
                    MyValue.Add(new KeyValuePair<string, int>("Development", 89));
                    MyValue.Add(new KeyValuePair<string, int>("Support", 270));
                    MyValue.Add(new KeyValuePair<string, int>("Sales", 140));

                    BarChart1.DataContext = MyValue;


                /*****************************faire comentaire********************************************/
                    /*  FamilleAlimentDataGrid.ItemsSource = proxy.GetAllAliment();
                     cbFamille.ItemsSource = (proxy.GetAllFamilleAliment()).OrderBy(r => r.FamilleAlimentDesign);*/
                     BesoinCaloriqueGrid.ItemsSource = proxy.GetAllBesoinCalorique().Where(n => n.CodePatient == PATIENT.Id).ToList();
                     TabPetitDéj.IsEnabled = false;
                     TabDéjeuner.IsEnabled = false;
                     TabCollation.IsEnabled = false;
                     TabGouter.IsEnabled = false;
                     TabDiner.IsEnabled = false;
                     TabLeSoir.IsEnabled = false;
                     if (tabitemhygiene == false)
                     {
                         MemoryStream memLicStream = new MemoryStream(ASCIIEncoding.Default.GetBytes(GestionClinique.Properties.Resources.CalculeCalorique));

                         this.richTextBox1.Selection.Load(memLicStream, System.Windows.DataFormats.Rtf);
                         MemoryStream memLicStream2 = new MemoryStream(ASCIIEncoding.Default.GetBytes(GestionClinique.Properties.Resources.L_index_glycémique));

                         this.richTextBox2.Selection.Load(memLicStream2, System.Windows.DataFormats.Rtf);

                         callback.InsertAlimentCallbackevent += new ICallback.CallbackEventHandler28(callbackrecuInsertAliment_Refresh);
                         callback.InsertFamilleAlimentCallbackevent += new ICallback.CallbackEventHandler29(callbackrecufamille_Refresh);
                         callback.InsertBesoinCaloriqueCallbackevent += new ICallback.CallbackEventHandler30(callbackrecuBesoinCalorique_Refresh);
                         callback.InsertRepaCallbackevent += new ICallback.CallbackEventHandler31(callbackrecuRepa_Refresh);
                     }
                     tabitemhygiene = true;
                /****************examen bucall*********************************/

                AutoDatagGrid.ItemsSource = proxy.GetAllautosurveillance().Where(n => n.CodePatient == PATIENT.Id);

                callback.InsertautosurveillanceCallbackevent += new ICallback.CallbackEventHandler52(callbackrecuautosurveillance_Refresh);




                /*********************************Diagnostic***********************************/
                DiagnosticCréer = proxy.GetAllDiagnostic().Find(n => n.CodePatient == PATIENT.Id && n.IdVisite==0);


                    if (DiagnosticCréer != null)
                    {
                        InterogatoireAnamnéseExiste = true;
                        txtAnamneseAdulte.DataContext = DiagnosticCréer;
                        txtConsultationAdulte.DataContext = DiagnosticCréer;
                        txtConsultationclinique.DataContext = DiagnosticCréer;
                        string caseSwitch = DiagnosticCréer.SituationFamiliale;
                        switch (caseSwitch)
                        {
                            case "Célibataire":
                                comboCélibataire.IsChecked = true;
                                break;
                            case "Marié (e)":
                                comboMarié.IsChecked = true;
                                break;
                            case "Veuf (veuve)":
                                comboVeuf.IsChecked = true;
                                break;
                            case "Divorcé (e)":
                                comboDivorce.IsChecked = true;
                                break;

                        }
                    if( DiagnosticCréer.AutreSymptomatologie!="")
                    {
                        txtSympAutre.Visibility = Visibility.Visible;
                        chautresymp.IsChecked = true;
                    }
                    if(DiagnosticCréer.AutreAntécédentsmédicaux!="")
                    {
                        txtMedicauxAutre.Visibility = Visibility.Visible;
                        chautreatecedentmed.IsChecked = true;
                       
                    }
                    if (DiagnosticCréer.AutreTypealimentaire != "")
                    {
                        txtAlimentaireAutre.Visibility = Visibility.Visible;
                        autretypealimentation.IsChecked = true;
                    }
                    if (DiagnosticCréer.AutresSituationProfessionnele!="")
                    {
                        txtActivitéAutre.Visibility = Visibility.Visible;
                        chautreprofessiel.IsChecked = true;
                    }

                    if(DiagnosticCréer.autreatecedentfamille!="")
                    {
                        txtAntécédentFamiAutre.Visibility = Visibility.Visible;
                        autreatecedentfamille.IsChecked = true;
                    }
                    if(DiagnosticCréer.autrefacteurderisque!="")
                    {
                        txtFacteurAutre.Visibility = Visibility.Visible;
                        autrefacteurderisque.IsChecked = true;
                    }
                    if(DiagnosticCréer.chautrefemme!="")
                    {
                        txtFemmeAutre.Visibility = Visibility.Visible;
                        chautrefemme.IsChecked = true;
                    }



                    }
                    else
                    {
                        InterogatoireAnamnéseExiste = false;
                        DiagnosticCréer = new SVC.Diagnostic
                        {
                            Date = DateTime.Now,
                            CodePatient = PATIENT.Id,
                            NomPatient = PATIENT.Nom,
                            PrénomPatient = PATIENT.Prénom,
                            MemberUser = memberuser.UserName,
                            Remarque = "Dépistage",
                            Cle= "Dépistage",
                            Affectionsendocrines = false,
                            Affectionspancréatiques = false,
                            Alcoolisme = false,
                            Alimentairericheengraisse = false,
                            Asthénie = false,
                            Augmentationdelasoifetdelafaim = false,
                            Besoinfréquentduriner = false,
                            Chômage = false,
                            Coupuresetblessuresquicicatrisentlentement = false,
                            Desdémangeaisons = false,
                            Douleursaumollets = false,
                            DouleursHépatiques = false,
                            DouleursThoraciques = false,
                            Dyspnéesdefforts = false,
                            Enactivité = false,
                            Frèreousoeur = false,
                            Furonclesàrépétition = false,
                            Hypercalorique = false,
                            Hyperlipidiques = false,
                            Hypertensionartérielle = false,
                            InfectionsFemme = false,
                            InfectionsHomme = false,
                            Insensibilité = false,

                            Mère = false,
                            Pauvreenfibresalimentaires = false,
                            Prisedepoids = false,
                            Problèmecardiovasculaire = false,
                            Problèmedentaire = false,
                            Père = false,
                            Retraité = false,
                            SituationparticulièrefemmeAccouchement = false,
                            SituationparticulièrefemmeDiabète = false,
                            Sédentarité = false,
                            Tabagisme = false,
                            Traitementcorticothérapiecontraceptifs = false,
                            Troublesdelérection = false,
                            Visionfloue = false,
                            IdVisite=0,
                        };
                        if (MedecinRecu != null)
                        {
                            DiagnosticCréer.CodeMedecin = MedecinRecu.Id;
                            DiagnosticCréer.NomMedecin = MedecinRecu.Nom;
                            DiagnosticCréer.PrénomMedecin = MedecinRecu.Prénom;
                        }
                        txtAnamneseAdulte.DataContext = DiagnosticCréer;
                        txtConsultationAdulte.DataContext = DiagnosticCréer;
                        txtConsultationclinique.DataContext = DiagnosticCréer;
                    }

                    /*constanteDépistage*/
                    constanteCréer = proxy.GetAllConstantesByPatient(PATIENT.Id).Find(n=> n.Remarque == "DÉPISTAGE");
                    if (constanteCréer != null)
                    {
                        constanteAnamnéseExiste = true;
                        DatePriseAnamnese.SelectedDate = DateTime.Now;
                        txtTimeAnamnese.Text = Convert.ToString(constanteCréer.Date2);
                        CréerConstanteAnamnese.Content = "Modifier";
                        ConstaneAnamneseDétailGrid.DataContext = constanteCréer;

                    }
                    else
                    {
                        constanteCréer = new SVC.Constante
                        {
                            CodePatient = PATIENT.Id,
                            créatininesanguin = 0,
                            créatinineurinaire = 0,
                            Date1 = DatePrise.SelectedDate,
                            Date2 = DateTime.Now.TimeOfDay,
                            GlycémieAjeun = 0,
                            GlycémiePostprandiale = 0,
                            HbA1c = 0,
                            HDL_cholestérol = 0,
                            IMC = 0,
                            LDL_cholestérol = 0,
                            Micro_albuminurie = 0,
                            Nom = PATIENT.Nom,
                            PAD = 0,
                            PAS = 0,
                            Poid = 0,
                            Pouls = 0,
                            Prénom = PATIENT.Prénom,
                            Taille = 0,
                            Temp = 0,
                            triglycérides = 0,
                            Uréesanguine = 0,
                            Uréeurinaire = 0,
                            UserName = memberuser.UserName,
                            Remarque = "DÉPISTAGE",
                            Cholestéroltotal=0,
                        };
                        DatePriseAnamnese.SelectedDate = DateTime.Now;
                        txtTimeAnamnese.Text = Convert.ToString(constanteCréer.Date2);
                        ConstaneAnamneseDétailGrid.DataContext = constanteCréer;
                        CréerConstanteAnamnese.Content = "Créer";
                        constanteAnamnéseExiste = false;
                        /************la partie ou la mise a jours doit pas se faire pour garderle travaill*************/


                        /**********************/


                    }









                NumRendezVous = NrendezVous;


                /******************RendezVous*****************************/
                DateNOWRendezz.SelectedDate = DateTime.Now;
                RendezVousExisiteGridPatient.ItemsSource = (proxy.GetAllRendezVousParPatient(PATIENT.Id)).OrderBy(n => n.Date);
                if (MedecinConsult != null)
                {
                    /********************************/
                    RendezVousExisiteGrid.ItemsSource = (proxy.GetAllRendezVousParMedecin(MedecinConsult.Id)).Where(n => n.Date == DateTime.Now.Date);//.Where(n => n.MedecinNom == medecinSelected.Nom && n.MedecinPrénom==medecinSelected.Prénom && n.Date== DateTime.Now);
                    NBRENDEZVOUS.Text = ((proxy.GetAllRendezVousParMedecin(MedecinConsult.Id)).Where(n => n.Date == DateTime.Now.Date).AsEnumerable().Count()).ToString();
                    /************************************/
                    callbackrecu.InsertRendezVousCallbackEvent += new ICallback.CallbackEventHandler8(callbackrecuRendezVous_Refresh);

                }
                else
                {
                    RendezVousExisiteGrid.ItemsSource = (proxy.GetAllRendezVous(DateTime.Now.Date, DateTime.Now.Date));//.Where(n => n.MedecinNom == medecinSelected.Nom && n.MedecinPrénom==medecinSelected.Prénom && n.Date== DateTime.Now);
                    NBRENDEZVOUS.Text = ((proxy.GetAllRendezVous(DateTime.Now.Date, DateTime.Now.Date)).AsEnumerable().Count()).ToString();
                    callbackrecu.InsertRendezVousCallbackEvent += new ICallback.CallbackEventHandler8(callbackrecuRendezVous_Refresh);
                }



                if (localclient.Stock)
                {
                    btndestockage.Visibility = Visibility.Visible;
                    btndestockage.IsEnabled = true;
                }
                else
                {
                    btndestockage.Visibility = Visibility.Collapsed;
                    btndestockage.IsEnabled = false;
                }





                /****************droit et privilége pour création*******************/
                if (memberuser.CréationDossierPatient == true)
                    {
                        btnCréerCas.IsEnabled = true;
                        CréerConstante.IsEnabled = true;
                   //     btnClear.IsEnabled = true;
                        bntcreerSimpleVisite.IsEnabled = true;
                     //   bntcreerOrdonnance.IsEnabled = true;
                    }
                    else
                    {
                        btnCréerCas.IsEnabled = false;
                //        btnClear.IsEnabled = false;
                        CréerConstante.IsEnabled = false;
                        bntcreerSimpleVisite.IsEnabled = false;
                      //  bntcreerOrdonnance.IsEnabled = false;
                    }
                    proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                    proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

               
            }
            catch (Exception ex)
            {
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }
        void callbackrecuautosurveillance_Refresh(object source, CallbackEventInsertautosurveillance e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshautosurveillance(e.clientleav);
            }));
        }
        public void AddRefreshautosurveillance(List<SVC.autosurveillance> listmembership)
        {

            AutoDatagGrid.ItemsSource = listmembership.Where(n=>n.CodePatient==PATIENT.Id);

        }
        void callbackrecuActe_Refresh(object source, CallbackEventInsertActe e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshActe(e.clientleav);
            }));
        }
        public void AddRefreshActe(List<SVC.Acte> listmembership)
        {

            TypeDeActesTraitement.ItemsSource = listmembership;

        }

        void callbackrecuRendezVous_Refresh(object source, CallbackEventInsertRendezVous e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshRendezVous(e.clientleav,e.operleav);
            }));
        }
        public void AddRefreshRendezVous(SVC.RendezVou listmembership, int oper)
        {
            try
            {
                if (MedecinConsult != null && DateNOWRendezz.SelectedDate.Value != null)
                {
                    var LISTITEM1 = RendezVousExisiteGrid.ItemsSource as IEnumerable<SVC.RendezVou>;
                    List<SVC.RendezVou> LISTITEM = LISTITEM1.ToList();

                    if (listmembership.CodeMedecin == MedecinConsult.Id && listmembership.Date == DateNOWRendezz.SelectedDate.Value)
                    {
                        if (oper == 1)
                        {
                            LISTITEM.Add(listmembership);
                        }
                        else
                        {
                            if (oper == 2)
                            {

                                var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                                //objectmodifed = listmembership;
                                var index = LISTITEM.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM[index] = listmembership;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                                    LISTITEM.Remove(deleterendez);
                                }
                            }
                        }
                    }

                    RendezVousExisiteGrid.ItemsSource = LISTITEM;





                    NBRENDEZVOUS.Text = ((LISTITEM).Where(n => n.CodeMedecin == MedecinConsult.Id && n.Date == DateNOWRendezz.SelectedDate.Value).AsEnumerable().Count()).ToString();
                    //     RendezVousExisiteGridPatient.ItemsSource = (LISTITEM).Where(n => n.CodePatient == PATIENT.Id).OrderBy(n => n.Date);

                }
                else
                {
                    if (MedecinConsult == null && DateNOWRendezz.SelectedDate.Value != null)
                    {
                        var LISTITEM1 = RendezVousExisiteGrid.ItemsSource as IEnumerable<SVC.RendezVou>;
                        List<SVC.RendezVou> LISTITEM = LISTITEM1.ToList();


                        if (listmembership.Date == DateNOWRendezz.SelectedDate.Value)
                        {
                            if (oper == 1)
                            {
                                LISTITEM.Add(listmembership);
                            }
                            else
                            {
                                if (oper == 2)
                                {


                                    var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                                    //objectmodifed = listmembership;
                                    var index = LISTITEM.IndexOf(objectmodifed);
                                    if (index != -1)
                                        LISTITEM[index] = listmembership;
                                }
                                else
                                {
                                    if (oper == 3)
                                    {
                                        //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                        var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                                        LISTITEM.Remove(deleterendez);
                                    }
                                }
                            }
                        }

                        RendezVousExisiteGrid.ItemsSource = LISTITEM;




                        NBRENDEZVOUS.Text = ((LISTITEM).Where(n => n.Date == DateNOWRendezz.SelectedDate.Value).AsEnumerable().Count()).ToString();
                        //      RendezVousExisiteGridPatient.ItemsSource = (LISTITEM).Where(n => n.CodePatient == PATIENT.Id).OrderBy(n => n.Date);

                    }
                }
                /****************************************************************////
                var LISTITEMRendezVousExisiteGridPatient1 = RendezVousExisiteGridPatient.ItemsSource as IEnumerable<SVC.RendezVou>;
                List<SVC.RendezVou> LISTITEMRendezVousExisiteGridPatient = LISTITEMRendezVousExisiteGridPatient1.ToList();
                if (listmembership.CodePatient == PATIENT.Id)
                {
                    if (oper == 1)
                    {
                        LISTITEMRendezVousExisiteGridPatient.Add(listmembership);
                    }
                    else
                    {
                        if (oper == 2)
                        {


                            var objectmodifed = LISTITEMRendezVousExisiteGridPatient.Find(n => n.Id == listmembership.Id);
                            //objectmodifed = listmembership;
                            var index = LISTITEMRendezVousExisiteGridPatient.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEMRendezVousExisiteGridPatient[index] = listmembership;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                var deleterendez = LISTITEMRendezVousExisiteGridPatient.Where(n => n.Id == listmembership.Id).First();
                                LISTITEMRendezVousExisiteGridPatient.Remove(deleterendez);
                            }
                        }
                    }
                    RendezVousExisiteGridPatient.ItemsSource = LISTITEMRendezVousExisiteGridPatient;
                }
                /*****************************************************************/


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void Détail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try

            {
                if (tabfichepatient.IsSelected)
                {
                    PatientGrid.DataContext = PATIENT;
                    PatientDataGrid.ItemsSource = proxy.GetAllVisiteByVisite(PATIENT.Id).ToList();
                    if (PATIENT.CheminPhoto.ToString() != "")
                    {
                        imgPhoto.Source = LoadImage(proxy.DownloadDocument(PATIENT.CheminPhoto.ToString()));

                    }
                    else
                    {

                        BitmapImage image = new BitmapImage(new Uri("/GestionClinique;component/Images/PatientInconu.jpg", UriKind.Relative));
                        imgPhoto.Source = image;
                    }
                }




            }
            catch (Exception ex)
            {
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }





        
       
        void callbackrecuInsertAliment_Refresh(object source, CallbackEventInsertAliment e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav);
            }));
        }
        public void AddRefresh(List<SVC.Aliment> listmembership)
        {
            if(tabhygiéneALIMENTAIRE.IsSelected)
           {
                FamilleAlimentDataGrid.ItemsSource = listmembership;
            }
            }
      
       
        void callbackrecufamille_Refresh(object source, CallbackEventInsertFamilleAliment e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshFamille(e.clientleav);
            }));
        }
        public void AddRefreshFamille(List<SVC.FamilleAliment> listmembership)
        {
            if (tabhygiéneALIMENTAIRE.IsSelected)
            {
                cbFamille.ItemsSource = listmembership.OrderBy(r => r.FamilleAlimentDesign);
            }
        }
       
        void callbackrecuBesoinCalorique_Refresh(object source, CallbackEventInsertBesoinCalorique e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshBesoinCalorique(e.clientleav);
            }));
        }
        public void AddRefreshBesoinCalorique(List<SVC.BesoinCalorique> listmembership)
        {
            if (tabhygiéneALIMENTAIRE.IsSelected)
            {
                BesoinCaloriqueGrid.ItemsSource = listmembership.Where(n => n.CodePatient == PATIENT.Id).ToList();
            }
        }
      void callbackrecuRepa_Refresh(object source, CallbackEventInsertRepa e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshRepa(e.clientleav);
            }));
        }
        public void AddRefreshRepa(List<SVC.Repa> listmembership)
        {
            if (tabhygiéneALIMENTAIRE.IsSelected)
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    /* if (TabInterface == 1)
                     {
                         PetitDejList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.DésignationRepas == "Petit Déjeuner").ToList();
                         PetitDéjeunerDataGrid.ItemsSource = PetitDejList;
                     }*/

                    if (BesoinCaloriqueCalculé.RepasBoolPetitDéj == true || BesoinCaloriqueCalculé.RepasBoolCollation == true || BesoinCaloriqueCalculé.RepasBoolDéjeuner == true || BesoinCaloriqueCalculé.RepasBoolGouter == true || BesoinCaloriqueCalculé.RepasBoolDiner == true || BesoinCaloriqueCalculé.RepasBoolLesoir == true)
                    {
                       // if (TabInterface == 1)
                      //  {
                            PetitDejList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 1).ToList();
                            PetitDéjeunerDataGrid.ItemsSource = PetitDejList;
                     //   }
                     //   if (TabInterface == 2)
                     //   {
                            CollationList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 2).ToList();
                            CollationDataGrid.ItemsSource = CollationList;
                     //   }
                     //   if (TabInterface == 3)
                     //   {
                            DéjeunerList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 3).ToList();
                            DéjeunerDataGrid.ItemsSource = DéjeunerList;
                    //    }
                     //   if (TabInterface == 4)
                     //   {
                            GouterList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 4).ToList();
                            GouterDataGrid.ItemsSource = GouterList;
                    //    }
                     //   if (TabInterface == 5)
                    //    {
                            DinerList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 5).ToList();
                            DinerDataGrid.ItemsSource = DinerList;
                     //   }
                      //  if (TabInterface == 6)
                     //   {
                            LeSoirList = listmembership.Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 6).ToList();
                            LeSoirDataGrid.ItemsSource = LeSoirList;
                     //   }
                    }
                }

            }
        }
        void callbackrecu_Refresh(object source,CallbackEventInsertVisite e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav,e.operleav);
            }));
        }
        void callbackrecucastraitement_Refresh(object source, CallbackEventInsertCasTraitement e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshTraitement(e.clientleav);
            }));
        }
        void callbackrecuConstante_Refresh(object source, CallbackEventInsertConstante e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshConstante(e.clientleav);
            }));
        }

        void callbackrecuDicomFichier_Refresh(object source, CallbackEventInsertDicomFichier e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshDicomFichier(e.clientleav);
            }));
        }
        void callbackrecuEntete_Refresh(object source, CallbackEventInsertEnteteOrdonnace e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshEnteteOrdonnace(e.clientleav);
            }));
        }
        public void AddRefreshEnteteOrdonnace(List<SVC.EnteteOrdonnace> listmembership)
        {

            OrdonnanceDatagGrid.ItemsSource = proxy.GetAllEnteteOrdonnace().Where(n => n.CodePatient == PATIENT.Id );
        }
        void callbackrecuOrdonnancePatient_Refresh(object source, CallbackEventInsertOrdonnancePatient e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshOrdonnancePatient(e.clientleav);
            }));
        }
        public void AddRefreshOrdonnancePatient(List<SVC.OrdonnancePatient> listmembership)
        {

            MedicamentsDataGrid.ItemsSource = (proxy.GetAllProduitOrdonnance()).OrderBy(r => r.Design);
         }
        public void AddRefreshDicomFichier(List<SVC.DicomFichier> listmembership)
        {

            DatagridImagerie.ItemsSource = proxy.GetAllDicomFichier().Where(n => n.CodePatient== PATIENT.Id && n.Image == false && n.Dicom == true).ToList();
            DatagridImage1.ItemsSource = proxy.GetAllDicomFichier().Where(n => n.CodePatient == PATIENT.Id && n.Image == true && n.Dicom == false).ToList();
        }

        public void AddRefreshConstante(List<SVC.Constante> listmembership)
        {
          //  if (tabconstante.IsSelected == true)
          //  {
                DatagridContstante.ItemsSource = listmembership.Where(n => n.CodePatient == PATIENT.Id).ToList();
          //  }
           //     if (tabdiagnostic.IsSelected==true)
          //  {
                constanteCréer = listmembership.Find(n => n.CodePatient == PATIENT.Id && n.Remarque == "DÉPISTAGE");
                if (constanteCréer != null)
                {
                    constanteAnamnéseExiste = true;
                    DatePriseAnamnese.SelectedDate = DateTime.Now;
                    txtTimeAnamnese.Text = Convert.ToString(constanteCréer.Date2);
                    CréerConstanteAnamnese.Content = "Modifier";
                    ConstaneAnamneseDétailGrid.DataContext = constanteCréer;

                }
                else
                {
                    constanteCréer = new SVC.Constante
                    {
                        CodePatient = PATIENT.Id,
                        créatininesanguin = 0,
                        créatinineurinaire = 0,
                        Date1 = DatePrise.SelectedDate,
                        Date2 = DateTime.Now.TimeOfDay,
                        GlycémieAjeun = 0,
                        GlycémiePostprandiale = 0,
                        HbA1c = 0,
                        HDL_cholestérol = 0,
                        IMC = 0,
                        LDL_cholestérol = 0,
                        Micro_albuminurie = 0,
                        Nom = PATIENT.Nom,
                        PAD = 0,
                        PAS = 0,
                        Poid = 0,
                        Pouls = 0,
                        Prénom = PATIENT.Prénom,
                        Taille = 0,
                        Temp = 0,
                        triglycérides = 0,
                        Uréesanguine = 0,
                        Uréeurinaire = 0,
                        UserName = memberuser.UserName,
                        Remarque = "DÉPISTAGE",
                    };
                    DatePriseAnamnese.SelectedDate = DateTime.Now;
                    txtTimeAnamnese.Text = Convert.ToString(constanteCréer.Date2);
                    ConstaneAnamneseDétailGrid.DataContext = constanteCréer;
                    CréerConstanteAnamnese.Content = "Créer";
                    constanteAnamnéseExiste = false;

                //}
            }
        }
        public void AddRefresh(SVC.Visite listmembership,int oper)
        {

            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Visite>;
                List<SVC.Visite> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listmembership);
                }
                else
                {
                    if (oper == 2)
                    {



                        var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //objectmodifed = listmembership;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listmembership;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                PatientDataGrid.ItemsSource = LISTITEM;
                SIMPLEDataGrid.ItemsSource = LISTITEM;
                /*******************************************************************************************/

                var LISTITEM11 = HistoriqueDesDataGrid.ItemsSource as IEnumerable<SVC.Visite>;
                List<SVC.Visite> LISTITEM0 = LISTITEM11.ToList();
                if (listmembership.cle == PATIENT.cle || listmembership.CodePatient == PATIENT.Id)
                {
                    if (oper == 1)
                    {
                        LISTITEM0.Add(listmembership);
                    }
                    else
                    {
                        if (oper == 2)
                        {




                            var objectmodifed = LISTITEM0.Find(n => n.Id == listmembership.Id);
                            //objectmodifed = listmembership;
                            var index = LISTITEM0.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEM0[index] = listmembership;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                var deleterendez = LISTITEM0.Where(n => n.Id == listmembership.Id).First();
                                LISTITEM0.Remove(deleterendez);
                            }
                        }
                    }

                }
                HistoriqueDesDataGrid.ItemsSource = LISTITEM0.OrderBy(r => r.Date);
                txtLabelTotalAregler.Content = Convert.ToString((LISTITEM0.Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle)).AsEnumerable().Sum(o => o.Montant));
                txtLabelTotalversement.Content = Convert.ToString((LISTITEM0.Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle)).AsEnumerable().Sum(o => o.Versement));
                txtLabelTotalReste.Content = Convert.ToString((LISTITEM0.Where(n => n.CodePatient == PATIENT.Id || n.cle == PATIENT.cle)).AsEnumerable().Sum(o => o.Reste));

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }



        }
        public void AddRefreshTraitement(List<SVC.CasTraitement> listmembership)
        {
            CasDataGrid.ItemsSource = listmembership.Where(n => n.CodePatient == PATIENT.Id ).ToList();
            txtMotifVisite.ItemsSource = listmembership.Where(n => n.CodePatient == PATIENT.Id && n.Devis==false ).ToList();

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
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDossierPatient(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDossierPatient(HandleProxy));
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

    
        private void BtnRendezVousExiste_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCas(object sender, RoutedEventArgs e)
        {
            if (CasDataGrid.SelectedItem==null && memberuser.CréationDossierPatient==true && txtCas1.Text !="" && txtDepuis.SelectedDate!=null && ComboEtat.SelectedItem!=null && ComboFact.SelectedItem!=null && modifcas==false)
            {
                try
                {
                    bool succéesOpération = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                     //   SVC.TypeCa SelectTyepCAS = txtCas1.SelectedItem as SVC.TypeCa;
                        SVC.CasTraitement cas = new SVC.CasTraitement
                        {
                            Nom = PATIENT.Nom,
                            Prénom = PATIENT.Prénom,
                            Cas = txtCas1.Text,
                            Date = txtDepuis.SelectedDate,
                            Histoire = txtHistoire.Text.Trim(),
                            Remarques = txtRemarqueCas.Text.Trim(),
                            EtatActuel = ComboEtat.SelectionBoxItem.ToString(),
                            ModeFacturation = ComboFact.SelectionBoxItem.ToString(),
                            UserName = memberuser.UserName,
                            Versement = 0,
                            Devis=false,
                            CodePatient = PATIENT.Id,
                            //IdCas = SelectTyepCAS.Id,
                            ModeFacturationint = ComboFact.SelectedIndex,
                         //   cle = casdent.Cas + PATIENT.Id + Convert.ToString(txtDepuis.SelectedDate) + ComboFact.SelectedIndex + DateTime.Now.TimeOfDay,
                        };
                        cas.cle = cas.Cas + PATIENT.Id + Convert.ToString(txtDepuis.SelectedDate) + ComboFact.SelectedIndex + DateTime.Now.TimeOfDay;
                        if (ComboFact.SelectedIndex == 2)
                        {
                            if (TxtHonoraireCasTraiter.Text != "")
                            {
                                cas.Montant = Convert.ToDecimal(TxtHonoraireCasTraiter.Text);
                                cas.Reste = Convert.ToDecimal(TxtHonoraireCasTraiter.Text);
                                cas.Versement = 0;
                            }
                            else
                            {
                                cas.Montant = 0;
                                cas.Reste = 0;
                                cas.Versement = 0;
                            }
                        }
                        if (MedecinConsult != null)
                        {
                            cas.MedecinNom = MedecinConsult.Nom;
                            cas.MedecinPrénom = MedecinConsult.Prénom;

                        }
                        else
                        {
                            cas.MedecinNom = memberuser.UserName + " User";
                            cas.MedecinPrénom = memberuser.Prénom + " User";
                        }
                        proxy.InsertTypeTraitement(cas);
                        ts.Complete();
                            succéesOpération = true;
                    }
                    proxy.AjouterCasTraitementRefresh();
                  txtCas1.Text =null;
                txtDepuis.SelectedDate = null;
                txtHistoire.Text = "";
                txtRemarqueCas.Text = "";
                ComboEtat.SelectedIndex = -1;
                ComboFact.SelectedIndex = -1;
                TxtHonoraireCasTraiter.Text = "";
       
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie !", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    string Mess = "Error: " + ex.Message;
                    this.Title = Mess;
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(Mess, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (CasDataGrid.SelectedItem != null && memberuser.ModificationDossierPatient == true && modifcas==true)
                {
                    try
                    {
                        bool succéesOpération = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            SVC.CasTraitement SelectMedecin = CasDataGrid.SelectedItem as SVC.CasTraitement;
                           // SVC.TypeCa SelectTyepCAS = txtCas1.SelectedItem as SVC.TypeCa;
                            SelectMedecin.Cas = SelectMedecin.Cas;
                            SelectMedecin.EtatActuel = ComboEtat.SelectionBoxItem.ToString();
                            SelectMedecin.ModeFacturation = ComboFact.SelectionBoxItem.ToString();
                            SelectMedecin.Devis = false;
                            if (ComboFact.SelectedIndex == 2)
                            {
                                if (TxtHonoraireCasTraiter.Text != "")
                                {
                                    SelectMedecin.Montant = Convert.ToDecimal(TxtHonoraireCasTraiter.Text);
                                    SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                                }
                                else
                                {
                                    SelectMedecin.Montant = 0;
                                    SelectMedecin.Reste = 0;
                                }
                            }
                            else
                            {
                                if (ComboFact.SelectedIndex == 0)
                                {
                                    SelectMedecin.Montant =0;
                                    SelectMedecin.Reste = 0;
                                   
                                }
                                else
                                {
                                    /********actes**********************/
                                }
                            }


                           
                           // SelectMedecin.IdCas = SelectTyepCAS.Id;
                            SelectMedecin.CodePatient = PATIENT.Id;
                            SelectMedecin.Nom = PATIENT.Nom;
                            SelectMedecin.Prénom = PATIENT.Prénom;
                            SelectMedecin.UserName = memberuser.UserName;
                            if (MedecinConsult != null)
                            {
                                SelectMedecin.MedecinNom = MedecinConsult.Nom;
                                SelectMedecin.MedecinPrénom = MedecinConsult.Prénom;

                            }
                            else
                            {
                                SelectMedecin.MedecinNom = memberuser.UserName + " User";
                                SelectMedecin.MedecinPrénom = memberuser.Prénom + " User";
                            }
                            SelectMedecin.ModeFacturationint = ComboFact.SelectedIndex;
                            SelectMedecin.Histoire = txtHistoire.Text.Trim();
                            SelectMedecin.Remarques= txtRemarqueCas.Text.Trim();
                            


                            proxy.UpdateTypeTraitement(SelectMedecin);
                            succéesOpération = true;
                            if (succéesOpération)
                            {
                                ts.Complete();
                          
                                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        proxy.AjouterCasTraitementRefresh();



                

                    }
                    catch (Exception ex)
                    {
                        
                        MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                else
                {
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vousp pouvez pas faire cette opération ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void bntcreerSimpleVisite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtMotifVisite.SelectedItem != null /*&& txtInterrogatoire.Text != null && txtExamen.Text != null && txtConclusions.Text != null */&& modifvisite == false && TxtHonoraire.Text != "" && memberuser.CréationDossierPatient == true)
                {


                    SVC.CasTraitement SelectMedecin = txtMotifVisite.SelectedItem as SVC.CasTraitement;

                    if (SelectMedecin.ModeFacturationint == 0 && TypeDeVisite.SelectedItem != null && (SelectMedecin.Cas == "Consultation simple" || SelectMedecin.Cas == "Consultation assistée"))
                    {
                        SVC.MotifVisite SelectMotifVisite = TypeDeVisite.SelectedItem as SVC.MotifVisite;
                        bool succésvisite = false;
                        bool succéstraitgement = false;
                        bool insertdiag = false;
                        bool insertconstante = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            SVC.Visite cas = new SVC.Visite
                            {
                                Motif = SelectMotifVisite.Motif,
                                Interrogatoire = txtInterrogatoire.Text.Trim(),
                                Examen = txtExamen.Text.Trim(),
                                Conclusions = txtConclusions.Text.Trim(),
                                Date = DateTime.Now,
                                Datetime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                                CasTraite = SelectMedecin.Cas,
                                NuméroRendezVous = NumRendezVous,

                                UserName = memberuser.UserName,
                                NomPatient = PATIENT.Nom,
                                PrénomPatient = PATIENT.Prénom,

                                Montant = Convert.ToDecimal(TxtHonoraire.Text),
                                Versement = 0,
                                Reste = Convert.ToDecimal(TxtHonoraire.Text),
                                Soldé = false,
                                CodePatient = PATIENT.Id,

                                IdCas = SelectMedecin.Id,
                                IdMotif = SelectMotifVisite.Id,
                                ModeFacturation = SelectMedecin.ModeFacturationint,
                                cle = SelectMedecin.cle + DateTime.Now.TimeOfDay,
                                IdDiag = 0,
                                Diag = false,
                            };
                            if (MedecinConsult != null)
                            {
                                cas.VisiteParNom = MedecinConsult.Nom;
                                cas.VisiteParPrénom = MedecinConsult.Prénom;
                                cas.CodeMedecin = MedecinConsult.Id;
                            }
                            else
                            {
                                cas.VisiteParNom = memberuser.UserName + " " + "User";
                                cas.VisiteParPrénom = memberuser.Prénom + " " + "User";
                                cas.CodeMedecin = 1987;
                            }
                            //   SelectMedecin.Versement = SelectMedecin.Versement + cas.Montant;
                            // SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                            //proxy.UpdateTypeTraitement(SelectMedecin);
                            //succéstraitgement = true;


                           



                            if(visitewithdiag==true)
                            {
                                cas.Diag = true;
                                dia.Cle = cas.cle;
                                constanteCréerVisiteASSI.cle = cas.cle;
                                constanteCréerVisiteASSI.Remarque = cas.Motif;
                                proxy.InsertDiagnostic(dia);
                                insertdiag = true;
                                


                                proxy.InsertConstante(constanteCréerVisiteASSI,PATIENT.Id);
                                insertconstante = true;

                              
                            }
                            proxy.InsertVisite(cas);
                            succésvisite = true;

                            if (succésvisite == true && visitewithdiag == false)
                            {
                                ts.Complete();
                                LabelForfait.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                if (succésvisite == true && visitewithdiag == true && insertdiag==true && insertconstante==true)
                                {
                                    ts.Complete();
                                    LabelForfait.Visibility = Visibility.Collapsed;
                                }
                            }

                            SelectMedecin.Cas = "";
                            txtInterrogatoire.Text = "";
                            txtExamen.Text = "";
                            txtConclusions.Text = "";
                            TxtHonoraire.Text = "";
                            txtMotifVisite.SelectedIndex = -1;
                            TypeDeVisite.SelectedIndex = -1;
                            chactetraiter.IsChecked = false;

                            txtblockmotifvisite.Visibility = Visibility.Collapsed;
                            TypeDeVisite.Visibility = Visibility.Collapsed;
                            Grid.SetColumnSpan(txtConclusions, 3);
                            Grid.SetColumnSpan(txtExamen, 2);
                            TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                        }
                        proxy.AjouterSoldeVisiteRefresh();
                        proxy.AjouterCasTraitementRefresh();
                    } else
                    {

                        if (TypeDeVisite.SelectedItem != null && SelectMedecin.ModeFacturationint == 2)
                        {
                            SVC.MotifVisite SelectMotifVisite = TypeDeVisite.SelectedItem as SVC.MotifVisite;

                            bool succésvisite = false;
                            bool succéstraitgement = false;

                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                SVC.Visite cas = new SVC.Visite
                                {
                                    Motif = SelectMotifVisite.Motif,
                                    Interrogatoire = txtInterrogatoire.Text.Trim(),
                                    Examen = txtExamen.Text.Trim(),
                                    Conclusions = txtConclusions.Text.Trim(),
                                    Date = DateTime.Now,
                                    Datetime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                                    CasTraite = SelectMedecin.Cas,
                                    NuméroRendezVous = NumRendezVous,

                                    UserName = memberuser.UserName,
                                    NomPatient = PATIENT.Nom,
                                    PrénomPatient = PATIENT.Prénom,

                                    Montant = Convert.ToDecimal(TxtHonoraire.Text),
                                    Versement = 0,
                                    Reste = Convert.ToDecimal(TxtHonoraire.Text),
                                    Soldé = false,
                                    CodePatient = PATIENT.Id,
                                    IdDiag = 0,
                                    Diag = false,
                                    IdCas = SelectMedecin.Id,
                                    IdMotif = SelectMotifVisite.Id,
                                    ModeFacturation = SelectMedecin.ModeFacturationint,
                                    cle = SelectMedecin.cle + DateTime.Now.TimeOfDay,
                                };
                                if (MedecinConsult != null)
                                {
                                    cas.VisiteParNom = MedecinConsult.Nom;
                                    cas.VisiteParPrénom = MedecinConsult.Prénom;
                                    cas.CodeMedecin = MedecinConsult.Id;
                                }
                                else
                                {
                                    cas.VisiteParNom = memberuser.UserName + " " + "User";
                                    cas.VisiteParPrénom = memberuser.Prénom + " " + "User";
                                    cas.CodeMedecin = 1987;
                                }
                                SelectMedecin.Versement = SelectMedecin.Versement + cas.Montant;
                                SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                                proxy.UpdateTypeTraitement(SelectMedecin);
                                succéstraitgement = true;


                                proxy.InsertVisite(cas);
                                succésvisite = true;



                                if (succésvisite == true && succéstraitgement == true)
                                {
                                    ts.Complete();
                                    LabelForfait.Visibility = Visibility.Collapsed;
                                }

                                SelectMedecin.Cas = "";
                                txtInterrogatoire.Text = "";
                                txtExamen.Text = "";
                                txtConclusions.Text = "";
                                TxtHonoraire.Text = "";
                                txtMotifVisite.SelectedIndex = -1;
                                TypeDeVisite.SelectedIndex = -1;
                                chactetraiter.IsChecked = false;
                                txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                TypeDeVisite.Visibility = Visibility.Collapsed;
                                Grid.SetColumnSpan(txtConclusions, 3);
                                Grid.SetColumnSpan(txtExamen, 2);
                                TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                            }
                            proxy.AjouterSoldeVisiteRefresh();
                            proxy.AjouterCasTraitementRefresh();
                        } else
                        {
                            /*********************visite traitement***********************/
                            SVC.Acte SelectMotifVisite = TypeDeActesTraitement.SelectedItem as SVC.Acte;
                            bool succésvisite = false;
                            // bool succéstraitgement = false;
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                SVC.Visite cas = new SVC.Visite
                                {
                                    Motif = SelectMotifVisite.Libelle,
                                    Interrogatoire = txtInterrogatoire.Text.Trim(),
                                    Examen = txtExamen.Text.Trim(),
                                    Conclusions = txtConclusions.Text.Trim(),
                                    Date = DateTime.Now,
                                    Datetime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                                    CasTraite = SelectMedecin.Cas,
                                    NuméroRendezVous = NumRendezVous,

                                    UserName = memberuser.UserName,
                                    NomPatient = PATIENT.Nom,
                                    PrénomPatient = PATIENT.Prénom,

                                    Montant = Convert.ToDecimal(TxtHonoraire.Text),
                                    Versement = 0,
                                    Reste = Convert.ToDecimal(TxtHonoraire.Text),
                                    Soldé = false,
                                    CodePatient = PATIENT.Id,
                                    IdDiag = 0,
                                    Diag = false,
                                    IdCas = SelectMedecin.Id,
                                    IdMotif = SelectMotifVisite.Id,
                                    ModeFacturation = SelectMedecin.ModeFacturationint,
                                    cle = SelectMedecin.cle + DateTime.Now.TimeOfDay,
                                };
                                if (MedecinConsult != null)
                                {
                                    cas.VisiteParNom = MedecinConsult.Nom;
                                    cas.VisiteParPrénom = MedecinConsult.Prénom;
                                    cas.CodeMedecin = MedecinConsult.Id;
                                }
                                else
                                {
                                    cas.VisiteParNom = memberuser.UserName + " " + "User";
                                    cas.VisiteParPrénom = memberuser.Prénom + " " + "User";
                                    cas.CodeMedecin = 1987;
                                }
                                //   SelectMedecin.Versement = SelectMedecin.Versement + cas.Montant;
                                // SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                                //   proxy.UpdateTypeTraitement(SelectMedecin);
                                //   succéstraitgement = true;


                                proxy.InsertVisite(cas);
                                succésvisite = true;


                                if (succésvisite == true)
                                {
                                    ts.Complete();
                                    LabelForfait.Visibility = Visibility.Collapsed;
                                }

                                SelectMedecin.Cas = "";
                                txtInterrogatoire.Text = "";
                                txtExamen.Text = "";
                                txtConclusions.Text = "";
                                TxtHonoraire.Text = "";
                                txtMotifVisite.SelectedIndex = -1;
                                TypeDeVisite.SelectedIndex = -1;
                                TypeDeActesTraitement.SelectedIndex = -1;
                                chactetraiter.IsChecked = false; txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                TypeDeVisite.Visibility = Visibility.Collapsed;
                                Grid.SetColumnSpan(txtConclusions, 3);
                                Grid.SetColumnSpan(txtExamen, 2);
                                TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                            }
                            proxy.AjouterSoldeVisiteRefresh();
                            proxy.AjouterCasTraitementRefresh();

                            /*********************************************/
                        }
                    }
                
                /*  }
                catch (Exception ex)
                  {

                      MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                  }*/
            
            }
            else
            {
                    if (SIMPLEDataGrid.SelectedItem != null && txtMotifVisite.SelectedItem != null/* && txtInterrogatoire.Text != null && txtExamen.Text != null && txtConclusions.Text != null*/ && modifvisite == true && TxtHonoraire.Text != "" && memberuser.ModificationDossierPatient == true)
                    {
                        bool succésvisiteupdate = false;
                        bool succéscastraitementupdate = false;
                        bool succéesboucheactes = false;
                        bool insertdiag = false;
                        bool insertconstante = false;
                        SVC.Visite SelectMedecin = SIMPLEDataGrid.SelectedItem as SVC.Visite;

                        SVC.Visite anciennevisite = proxy.GetAllVisiteByVisite(PATIENT.Id).Find(n => n.Id == SelectMedecin.Id);
                        SVC.CasTraitement SelectMedecisn = txtMotifVisite.SelectedItem as SVC.CasTraitement;
                        if (SelectMedecisn.ModeFacturationint == 0 && TypeDeVisite.SelectedItem != null && (SelectMedecisn.Cas == "Consultation simple" || SelectMedecisn.Cas == "Consultation assistée")) 
                        {
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {

                                SVC.MotifVisite SelectMotifVisite = TypeDeVisite.SelectedItem as SVC.MotifVisite;
                                SelectMedecin.Motif = SelectMotifVisite.Motif;
                                SelectMedecin.CasTraite = SelectMedecisn.Cas;
                                SelectMedecin.Interrogatoire = txtInterrogatoire.Text;
                                SelectMedecin.Examen = txtExamen.Text;
                                SelectMedecin.Conclusions = txtConclusions.Text;
                                SelectMedecin.UserName = memberuser.UserName;
                                SelectMedecin.NomPatient = PATIENT.Nom;
                                SelectMedecin.PrénomPatient = PATIENT.Prénom;
                                SelectMedecin.IdCas = SelectMedecisn.Id;
                                SelectMedecin.IdMotif = SelectMotifVisite.Id;
                                SelectMedecin.ModeFacturation = SelectMedecisn.ModeFacturationint;
                                if (MedecinConsult != null)
                                {
                                    SelectMedecin.VisiteParNom = MedecinConsult.Nom;
                                    SelectMedecin.VisiteParPrénom = MedecinConsult.Prénom;
                                    SelectMedecin.CodeMedecin = MedecinConsult.Id;
                                }
                                SelectMedecin.Montant = Convert.ToDecimal(TxtHonoraire.Text);
                                SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                                if (SelectMedecin.Reste == 0)
                                {
                                    SelectMedecin.Soldé = true;
                                }
                                else
                                {
                                    SelectMedecin.Soldé = false;
                                }
                                if (visitewithdiag == true)
                                {
                                    SelectMedecin.Diag = true;
                                    dia.Cle = SelectMedecin.cle;


                                    proxy.UpdateDiagnostic(dia);
                                    insertdiag = true;

                                    constanteCréerVisiteASSI.Remarque = SelectMedecin.Motif;
                                    constanteCréerVisiteASSI.cle= SelectMedecin.cle;
                                    proxy.UpdateConstante(constanteCréerVisiteASSI,PATIENT.Id);
                                    insertconstante = true;
                                }
                                proxy.UpdateVisite(SelectMedecin);
                                succésvisiteupdate = true;

                                // SelectMedecisn.Versement = SelectMedecisn.Versement + SelectMedecin.Montant - anciennevisite.Montant;
                                //SelectMedecisn.Reste = SelectMedecisn.Montant - SelectMedecisn.Versement;
                                //  proxy.UpdateTypeTraitement(SelectMedecisn);
                                //succéscastraitementupdate = true;
                                if (/*succéscastraitementupdate == true && */succésvisiteupdate == true)
                                {
                                    ts.Complete();
                                    LabelForfait.Visibility = Visibility.Collapsed;
                                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    if (succésvisiteupdate == true && visitewithdiag == true && insertdiag == true && insertconstante == true)
                                    {
                                        ts.Complete();
                                        LabelForfait.Visibility = Visibility.Collapsed;
                                    }
                                }
                                txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                TypeDeVisite.Visibility = Visibility.Collapsed;
                                Grid.SetColumnSpan(txtConclusions, 3);
                                Grid.SetColumnSpan(txtExamen, 2);
                                TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                                TxtHonoraire.Text = "";
                            }
                            proxy.AjouterSoldeVisiteRefresh();
                            proxy.AjouterCasTraitementRefresh();
                            TypeDeVisite.Visibility = Visibility.Collapsed;
                        }
                       
                            else
                            {
                                if (TypeDeVisite.SelectedItem != null && SelectMedecisn.ModeFacturationint == 2)
                                {
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {

                                        SVC.MotifVisite SelectMotifVisite = TypeDeVisite.SelectedItem as SVC.MotifVisite;
                                        SelectMedecin.Motif = SelectMotifVisite.Motif;
                                        SelectMedecin.CasTraite = SelectMedecisn.Cas;
                                        SelectMedecin.Interrogatoire = txtInterrogatoire.Text;
                                        SelectMedecin.Examen = txtExamen.Text;
                                        SelectMedecin.Conclusions = txtConclusions.Text;
                                        SelectMedecin.UserName = memberuser.UserName;
                                        SelectMedecin.NomPatient = PATIENT.Nom;
                                        SelectMedecin.PrénomPatient = PATIENT.Prénom;
                                        SelectMedecin.IdCas = SelectMedecisn.Id;
                                        SelectMedecin.IdMotif = SelectMotifVisite.Id;
                                        SelectMedecin.ModeFacturation = SelectMedecisn.ModeFacturationint;
                                        if (MedecinConsult != null)
                                        {
                                            SelectMedecin.VisiteParNom = MedecinConsult.Nom;
                                            SelectMedecin.VisiteParPrénom = MedecinConsult.Prénom;
                                            SelectMedecin.CodeMedecin = MedecinConsult.Id;
                                        }
                                        SelectMedecin.Montant = Convert.ToDecimal(TxtHonoraire.Text);
                                        SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                                        if (SelectMedecin.Reste == 0)
                                        {
                                            SelectMedecin.Soldé = true;
                                        }
                                        else
                                        {
                                            SelectMedecin.Soldé = false;
                                        }

                                        proxy.UpdateVisite(SelectMedecin);
                                        succésvisiteupdate = true;

                                        SelectMedecisn.Versement = SelectMedecisn.Versement + SelectMedecin.Montant - anciennevisite.Montant;
                                        SelectMedecisn.Reste = SelectMedecisn.Montant - SelectMedecisn.Versement;
                                        proxy.UpdateTypeTraitement(SelectMedecisn);
                                        succéscastraitementupdate = true;



                                        if (succéscastraitementupdate == true && succésvisiteupdate == true)
                                        {
                                            ts.Complete();
                                            LabelForfait.Visibility = Visibility.Collapsed;
                                            MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }
                                    txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                    TypeDeVisite.Visibility = Visibility.Collapsed;
                                    Grid.SetColumnSpan(txtConclusions, 3);
                                    Grid.SetColumnSpan(txtExamen, 2);
                                    TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                                    TxtHonoraire.Text = "";
                                }
                                proxy.AjouterSoldeVisiteRefresh();
                                proxy.AjouterCasTraitementRefresh();
                                TypeDeVisite.Visibility = Visibility.Collapsed;
                                }

                                else
                                {
                                    if (SelectMedecisn.ModeFacturationint == 0 && TypeDeActesTraitement.SelectedItem != null && SelectMedecisn.Cas == "Traitement")
                                    {
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {

                                            SVC.Acte SelectMotifVisite = TypeDeActesTraitement.SelectedItem as SVC.Acte;
                                            SelectMedecin.Motif = SelectMotifVisite.Libelle;
                                            SelectMedecin.CasTraite = SelectMedecisn.Cas;
                                            SelectMedecin.Interrogatoire = txtInterrogatoire.Text;
                                            SelectMedecin.Examen = txtExamen.Text;
                                            SelectMedecin.Conclusions = txtConclusions.Text;
                                            SelectMedecin.UserName = memberuser.UserName;
                                            SelectMedecin.NomPatient = PATIENT.Nom;
                                            SelectMedecin.PrénomPatient = PATIENT.Prénom;
                                            SelectMedecin.IdCas = SelectMedecisn.Id;
                                            SelectMedecin.IdMotif = SelectMotifVisite.Id;
                                            SelectMedecin.ModeFacturation = SelectMedecisn.ModeFacturationint;
                                            if (MedecinConsult != null)
                                            {
                                                SelectMedecin.VisiteParNom = MedecinConsult.Nom;
                                                SelectMedecin.VisiteParPrénom = MedecinConsult.Prénom;
                                                SelectMedecin.CodeMedecin = MedecinConsult.Id;
                                            }
                                            SelectMedecin.Montant = Convert.ToDecimal(TxtHonoraire.Text);
                                            SelectMedecin.Reste = SelectMedecin.Montant - SelectMedecin.Versement;
                                            if (SelectMedecin.Reste == 0)
                                            {
                                                SelectMedecin.Soldé = true;
                                            }
                                            else
                                            {
                                                SelectMedecin.Soldé = false;
                                            }

                                            proxy.UpdateVisite(SelectMedecin);
                                            succésvisiteupdate = true;

                                            //  SelectMedecisn.Versement = SelectMedecisn.Versement + SelectMedecin.Montant - anciennevisite.Montant;
                                            //SelectMedecisn.Reste = SelectMedecisn.Montant - SelectMedecisn.Versement;
                                            //proxy.UpdateTypeTraitement(SelectMedecisn);
                                            //succéscastraitementupdate = true;
                                            if (/*succéscastraitementupdate == true && */succésvisiteupdate == true)
                                            {
                                                ts.Complete();
                                                LabelForfait.Visibility = Visibility.Collapsed;
                                                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }
                                       
                                        txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                        TypeDeVisite.Visibility = Visibility.Collapsed;
                                        Grid.SetColumnSpan(txtConclusions, 3);
                                        Grid.SetColumnSpan(txtExamen, 2);
                                        TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                                        TxtHonoraire.Text = "";
                                    }
                                    proxy.AjouterSoldeVisiteRefresh();
                                    proxy.AjouterCasTraitementRefresh();
                                    TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                                    }
                             
                            }
                        }
                    }
                    else
                    {
                       // proxy.AjouterCasVisiteRefresh();
                        MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez remplir les champs obligatoires ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
               }
               // proxy.AjouterCasVisiteRefresh();

            }
            catch (Exception ex)
                {

                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                  }
                }

        private void CréerConstante_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DatePrise.SelectedDate != null && txtTime.Text != "" && modif == false && memberuser.CréationDossierPatient == true)
                {

                    /*  SVC.Constante cas = new SVC.Constante
                      {
                          Date1 = DatePrise.SelectedDate,
                          Date2 = TimeSpan.Parse(txtTime.Text.Trim()),
                          Poid =Convert.ToDecimal(txtPoid.Text.Trim()),
                          Taille = Convert.ToDecimal(txtTaille.Text.Trim()),
                          PAS = txtPAS.Text.Trim(),
                          PAD = txtPAD.Text.Trim(),
                          Pouls = txtPouls.Text.Trim(),
                          Temp = Convert.ToDecimal(txtTemp.Text.Trim()),
                          GlycémieAjeun = Convert.ToDecimal(txtGlycémieAjeun.Text.Trim()),
                          GlycémiePostprandiale = Convert.ToDecimal(txtGlycémiePostpran.Text.Trim()),
                          UserName = memberuser.UserName,
                          Nom = PATIENT.Nom,
                          Prénom = PATIENT.Prénom,
                         IMC= Convert.ToDecimal(txtIMC.Text),
                         CodePatient=PATIENT.Id,

                      };*/
                    constanteCréer.Date1 = DatePrise.SelectedDate;
                    constanteCréer.Date2 = TimeSpan.Parse(txtTime.Text);
                    proxy.InsertConstante(constanteCréer,PATIENT.Id);
                    DatagridContstante.SelectedItem = null;
                    DatePrise.SelectedDate = null;
                    txtPoid.Text = "";
                    txtTaille.Text = "";
                    txtPAS.Text = "";
                    txtPAD.Text = "";
                    txtPouls.Text = "";
                    txtTemp.Text = "";
                    txtGlycémieAjeun.Text = "";
                    txtGlycémiePostpran.Text = "";
                    txtTime.Text = "";
                    txtIMC.Text = "";
                    txtHDLCHOLEST.Text = "";
                    txtHDLcholestérol.Text = "";
                    txttriglycérides.Text = "";
                    txttHBA1C.Text = "";
                    txtMicroalbuminurie.Text = "";
                    txtUréesanguine.Text = "";
                    txtUréeurinaire.Text = "";
                    txtcréatininesanguin.Text = "";
                    txtccréatinineurinaire.Text = "";
                 //   constanteCréer = null;
                    ConstaneDétailGrid.DataContext = null;
                    CréerConstante.Content = "Créer";
                    ConstaneDétailGrid.IsEnabled = false;

                    if (autrebilanlist.Count() > 0)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            foreach(var item in autrebilanlist)
                            {
                                item.cle = constanteCréer.cle;
                                item.CodePatient = constanteCréer.CodePatient;
                                item.Date1 = constanteCréer.Date1;
                                item.Date2 = constanteCréer.Date2;
                                item.Remarque = constanteCréer.Remarque;

                                proxy.InsertAutreBilan(item);

                            }
                        }
                    }
                    constanteCréer = null;
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }




                else
                {
                    if (DatagridContstante.SelectedItem != null && DatePrise.SelectedDate != null && txtTime.Text != null && modif == true && memberuser.ModificationDossierPatient == true)
                    {
                        constanteCréer = DatagridContstante.SelectedItem as SVC.Constante;
                        constanteCréer.Date1 = DatePrise.SelectedDate;
                        constanteCréer.Date2 = TimeSpan.Parse(txtTime.Text.Trim());
                        constanteCréer.UserName = memberuser.UserName;
                        proxy.UpdateConstante(constanteCréer,PATIENT.Id);
                        /*************************************/
                        var listinbdd = proxy.GetAllAutreBilan(constanteCréer.cle);
                        if (autrebilanlist.Count() > 0 && listinbdd.Count()==0)
                        {
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                foreach (var item in autrebilanlist)
                                {
                                    item.cle = constanteCréer.cle;
                                    item.CodePatient = constanteCréer.CodePatient;
                                    item.Date1 = constanteCréer.Date1;
                                    item.Date2 = constanteCréer.Date2;
                                    item.Remarque = constanteCréer.Remarque;

                                    proxy.InsertAutreBilan(item);

                                }
                            }
                        }
                        else
                        {
                            if(autrebilanlist.Count()==0 && listinbdd.Count()>0)
                            {
                                foreach (var item in listinbdd)
                                {
                                   

                                    proxy.DeleteAutreBilan(item);

                                }
                            }else
                            {
                                if(autrebilanlist.Count()>0 && listinbdd.Count() > 0)
                                {
                                    foreach (var newitem in autrebilanlist)
                                    {
                                        var found = (listinbdd).Find(itemf => itemf.Id == newitem.Id);
                                        if (found == null)
                                        {
                                            newitem.cle = constanteCréer.cle;
                                            newitem.CodePatient = constanteCréer.CodePatient;
                                            newitem.Date1 = constanteCréer.Date1;
                                            newitem.Date2 = constanteCréer.Date2;
                                            newitem.Remarque = constanteCréer.Remarque;
                                            proxy.InsertAutreBilan(newitem);
                                        }
                                        else
                                        {
                                            if(found!=null)
                                            {
                                                newitem.cle = constanteCréer.cle;
                                                newitem.CodePatient = constanteCréer.CodePatient;
                                                newitem.Date1 = constanteCréer.Date1;
                                                newitem.Date2 = constanteCréer.Date2;
                                                newitem.Remarque = constanteCréer.Remarque;
                                                proxy.UpdateAutreBilan(newitem);
                                            }
                                        }
                                    }
                                    foreach(var ancienitem in listinbdd)
                                    {
                                        var foundancieninnouveau = (autrebilanlist).Find(itemf => itemf.Id == ancienitem.Id);
                                        if(foundancieninnouveau==null)
                                        {
                                            proxy.DeleteAutreBilan(ancienitem);
                                        }
                                    }
                                }
                            }
                        }









                        /************************************************************/
                        MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Mise à jours réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                     
                        MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vousp pouvez pas faire cette opération ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;

                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(Mess, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatagridContstante_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

        }

     

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(FamilleAlimentDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Aliment p = o as SVC.Aliment;
                        if (t.Name == "txtid")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.DesignProduit.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
            
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
          /*  try
            {
                if (memberuser.ImpressionDossierPatient == true)
                {
                    if (DatagridImagerie.SelectedItem != null)
                    {


                    SVC.DicomFichier SelectMedecin = DatagridImagerie.SelectedItem as SVC.DicomFichier;
                       
                        ImpressionOneDicom sc = new ImpressionOneDicom(proxy, SelectMedecin);
                        sc.Show();


                    }
                    else
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Votre selection est vide ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        imagePanelControl.Invalidate();
                    }


                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(Mess, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DatagridImagerie.SelectedItem != null && memberuser.SuppressionDossierPatient == true)
                {
                   
                        SVC.DicomFichier SelectMedecin = DatagridImagerie.SelectedItem as SVC.DicomFichier;
                        proxy.DeleteDicomFichier(SelectMedecin);
                       
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Mise à jours réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    proxy.AjouterDicomImageRefresh();


            }
                else
                {

                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous pouvez pas faire cette opération ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(Mess, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.CréationDossierPatient==true)
            {
               ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "All DICOM Files(*.*)|*.*";
                if (ofd.ShowDialog() == true)
                {
                    if (ofd.FileName.Length > 0)
                    {
                        Cursor = System.Windows.Input.Cursors.Wait;
                        ReadAndDisplayDicomFile(ofd.FileName, ofd.SafeFileName);
                        imageOpened = true;
                        btndéfiltrerDicom.IsEnabled = true;
                        Cursor = System.Windows.Input.Cursors.Arrow;
                    }
                  
                }

            }
        }
        private void ReadAndDisplayDicomFile(string fileName, string fileNameOnly)
        {
            dd.DicomFileName = fileName;
           
            TypeOfDicomFile typeOfDicomFile = dd.typeofDicomFile;

            if (typeOfDicomFile == TypeOfDicomFile.Dicom3File ||
                typeOfDicomFile == TypeOfDicomFile.DicomOldTypeFile)
            {
                imageWidth = dd.width;
                imageHeight = dd.height;
                bitDepth = dd.bitsAllocated;
                winCentre = dd.windowCentre;
                winWidth = dd.windowWidth;
                samplesPerPixel = dd.samplesPerPixel;
                signedImage = dd.signedImage;

                imagePanelControl.NewImage = true;
              
                if (samplesPerPixel == 1 && bitDepth == 8)
                {
                    pixels8.Clear();
                    pixels16.Clear();
                    pixels24.Clear();
                    dd.GetPixels8(ref pixels8);

                    // This is primarily for debugging purposes, 
                    //  to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //               "C:\\imageSigned.txt");

                    //    for (int ik = 0; ik < pixels8.Count; ++ik)
                    //        file.Write(pixels8[ik] + "  ");

                    //    file.Close();
                    //}

                    minPixelValue = pixels8.Min();
                    maxPixelValue = pixels8.Max();

                    // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
                    // Thanks to Matias Montroull from Argentina for pointing this out.
                    if (dd.signedImage)
                    {
                        winCentre -= char.MinValue;
                    }

                    if (Math.Abs(winWidth) < 0.001)
                    {
                        winWidth = maxPixelValue - minPixelValue;
                    }

                    if ((winCentre == 0) ||
                        (minPixelValue > winCentre) || (maxPixelValue < winCentre))
                    {
                        winCentre = (maxPixelValue + minPixelValue) / 2;
                    }

                    imagePanelControl.SetParameters(ref pixels8, imageWidth, imageHeight,
                        winWidth, winCentre, samplesPerPixel, true, this);
                }

                if (samplesPerPixel == 1 && bitDepth == 16)
                {
                    pixels16.Clear();
                    pixels8.Clear();
                    pixels24.Clear();
                    dd.GetPixels16(ref pixels16);

                    // This is primarily for debugging purposes, 
                    //  to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //               "C:\\imageSigned.txt");

                    //    for (int ik = 0; ik < pixels16.Count; ++ik)
                    //        file.Write(pixels16[ik] + "  ");

                    //    file.Close();
                    //}

                    minPixelValue = pixels16.Min();
                    maxPixelValue = pixels16.Max();

                    // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
                    // Thanks to Matias Montroull from Argentina for pointing this out.
                    if (dd.signedImage)
                    {
                        winCentre -= short.MinValue;
                    }

                    if (Math.Abs(winWidth) < 0.001)
                    {
                        winWidth = maxPixelValue - minPixelValue;
                    }

                    if ((winCentre == 0) ||
                        (minPixelValue > winCentre) || (maxPixelValue < winCentre))
                    {
                        winCentre = (maxPixelValue + minPixelValue) / 2;
                    }

                    imagePanelControl.Signed16Image = dd.signedImage;

                    imagePanelControl.SetParameters(ref pixels16, imageWidth, imageHeight,
                        winWidth, winCentre, true, this);
                }

                if (samplesPerPixel == 3 && bitDepth == 8)
                {
                    // This is an RGB colour image
                    pixels8.Clear();
                    pixels16.Clear();
                    pixels24.Clear();
                    dd.GetPixels24(ref pixels24);

                    // This code segment is primarily for debugging purposes, 
                    //    to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //                      "C:\\image24.txt");

                    //    for (int ik = 0; ik < pixels24.Count; ++ik)
                    //        file.Write(pixels24[ik] + "  ");

                    //    file.Close();
                    //}

                    imagePanelControl.SetParameters(ref pixels24, imageWidth, imageHeight,
                        winWidth, winCentre, samplesPerPixel, true, this);
                }
            }
            else
            {
                if (typeOfDicomFile == TypeOfDicomFile.DicomUnknownTransferSyntax)
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, I can't read a DICOM file with this Transfer Syntax.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, I can't open this file. " +
                        "This file does not appear to contain a DICOM image.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            //    Text = "DICOM Image Viewer: ";
                // Show a plain grayscale image instead
                pixels8.Clear();
                pixels16.Clear();
                pixels24.Clear();
                samplesPerPixel = 1;

                imageWidth = imagePanelControl.Width - 25;   // 25 is a magic number
                imageHeight = imagePanelControl.Height - 25; // Same magic number
                int iNoPix = imageWidth * imageHeight;

                for (int i = 0; i < iNoPix; ++i)
                {
                    pixels8.Add(240);// 240 is the grayvalue corresponding to the Control colour
                }
                winWidth = 256;
                winCentre = 127;
                imagePanelControl.SetParameters(ref pixels8, imageWidth, imageHeight,
                    winWidth, winCentre, samplesPerPixel, true, this);
                imagePanelControl.Invalidate();
               
            }

        }
        private void ReadAndDisplayDicomFileBinaryReader(BinaryReader fileName, string fileNameOnly)
        {
            dd.DicomFileNameBinaryReader = fileName;

            TypeOfDicomFile typeOfDicomFile = dd.typeofDicomFile;

            if (typeOfDicomFile == TypeOfDicomFile.Dicom3File ||
                typeOfDicomFile == TypeOfDicomFile.DicomOldTypeFile)
            {
                imageWidth = dd.width;
                imageHeight = dd.height;
                bitDepth = dd.bitsAllocated;
                winCentre = dd.windowCentre;
                winWidth = dd.windowWidth;
                samplesPerPixel = dd.samplesPerPixel;
                signedImage = dd.signedImage;

                imagePanelControl.NewImage = true;
            
                if (samplesPerPixel == 1 && bitDepth == 8)
                {
                    pixels8.Clear();
                    pixels16.Clear();
                    pixels24.Clear();
                    dd.GetPixels8(ref pixels8);

                    // This is primarily for debugging purposes, 
                    //  to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //               "C:\\imageSigned.txt");

                    //    for (int ik = 0; ik < pixels8.Count; ++ik)
                    //        file.Write(pixels8[ik] + "  ");

                    //    file.Close();
                    //}

                    minPixelValue = pixels8.Min();
                    maxPixelValue = pixels8.Max();

                    // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
                    // Thanks to Matias Montroull from Argentina for pointing this out.
                    if (dd.signedImage)
                    {
                        winCentre -= char.MinValue;
                    }

                    if (Math.Abs(winWidth) < 0.001)
                    {
                        winWidth = maxPixelValue - minPixelValue;
                    }

                    if ((winCentre == 0) ||
                        (minPixelValue > winCentre) || (maxPixelValue < winCentre))
                    {
                        winCentre = (maxPixelValue + minPixelValue) / 2;
                    }

                    imagePanelControl.SetParameters(ref pixels8, imageWidth, imageHeight,
                        winWidth, winCentre, samplesPerPixel, true, this);
                }

                if (samplesPerPixel == 1 && bitDepth == 16)
                {
                    pixels16.Clear();
                    pixels8.Clear();
                    pixels24.Clear();
                    dd.GetPixels16(ref pixels16);

                    // This is primarily for debugging purposes, 
                    //  to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //               "C:\\imageSigned.txt");

                    //    for (int ik = 0; ik < pixels16.Count; ++ik)
                    //        file.Write(pixels16[ik] + "  ");

                    //    file.Close();
                    //}

                    minPixelValue = pixels16.Min();
                    maxPixelValue = pixels16.Max();

                    // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
                    // Thanks to Matias Montroull from Argentina for pointing this out.
                    if (dd.signedImage)
                    {
                        winCentre -= short.MinValue;
                    }

                    if (Math.Abs(winWidth) < 0.001)
                    {
                        winWidth = maxPixelValue - minPixelValue;
                    }

                    if ((winCentre == 0) ||
                        (minPixelValue > winCentre) || (maxPixelValue < winCentre))
                    {
                        winCentre = (maxPixelValue + minPixelValue) / 2;
                    }

                    imagePanelControl.Signed16Image = dd.signedImage;

                    imagePanelControl.SetParameters(ref pixels16, imageWidth, imageHeight,
                        winWidth, winCentre, true, this);
                }

                if (samplesPerPixel == 3 && bitDepth == 8)
                {
                    // This is an RGB colour image
                    pixels8.Clear();
                    pixels16.Clear();
                    pixels24.Clear();
                    dd.GetPixels24(ref pixels24);

                    // This code segment is primarily for debugging purposes, 
                    //    to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //                      "C:\\image24.txt");

                    //    for (int ik = 0; ik < pixels24.Count; ++ik)
                    //        file.Write(pixels24[ik] + "  ");

                    //    file.Close();
                    //}

                    imagePanelControl.SetParameters(ref pixels24, imageWidth, imageHeight,
                        winWidth, winCentre, samplesPerPixel, true, this);
                }
            }
            else
            {
                if (typeOfDicomFile == TypeOfDicomFile.DicomUnknownTransferSyntax)
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, I can't read a DICOM file with this Transfer Syntax.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, I can't open this file. " +
                        "This file does not appear to contain a DICOM image.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //    Text = "DICOM Image Viewer: ";
                // Show a plain grayscale image instead
                pixels8.Clear();
                pixels16.Clear();
                pixels24.Clear();
                samplesPerPixel = 1;

                imageWidth = imagePanelControl.Width - 25;   // 25 is a magic number
                imageHeight = imagePanelControl.Height - 25; // Same magic number
                int iNoPix = imageWidth * imageHeight;

                for (int i = 0; i < iNoPix; ++i)
                {
                    pixels8.Add(240);// 240 is the grayvalue corresponding to the Control colour
                }
                winWidth = 256;
                winCentre = 127;
                imagePanelControl.SetParameters(ref pixels8, imageWidth, imageHeight,
                    winWidth, winCentre, samplesPerPixel, true, this);
                imagePanelControl.Invalidate();

            }

        }
        private void btnNewVisite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void btnSuppVisite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SIMPLEDataGrid.SelectedItem != null && memberuser.SuppressionDossierPatient == true)
                {

                    SVC.Visite SelectMedecin = SIMPLEDataGrid.SelectedItem as SVC.Visite;
                    if (SelectMedecin.Versement == 0 && SelectMedecin.ModeFacturation == 0)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.DeleteVisite(SelectMedecin);
                            ts.Complete();
                        }
                        proxy.AjouterSoldeVisiteRefresh();
                        proxy.AjouterCasTraitementRefresh();
                        this.Title = "Mise à jours réussie ! ";

                    }
                   
                   
                }
                else
                {
                    MessageBoxResult resultcx = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultcx = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }


        private void btnImprimerVisite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionDossierPatient == true)
                {
                    if (SIMPLEDataGrid.SelectedItem == null)
                    {
                        /*  List<SVC.Visite> test = SIMPLEDataGrid.ItemsSource as List<SVC.Visite>;


                          ImpressionCasTraitement clsho = new ImpressionCasTraitement(proxy, test.ToList());
                          clsho.Show();*/
                    }
                    else
                    {
                        if (SIMPLEDataGrid.SelectedItem != null)
                        {
                            SVC.Visite SelectMedecin = SIMPLEDataGrid.SelectedItem as SVC.Visite;



                            ImpressionOneVisite clsho = new ImpressionOneVisite(proxy, SelectMedecin);
                            clsho.Show();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;

            }
        }

        private void btnNewCas_Click(object sender, RoutedEventArgs e)
        {

            if (memberuser.CréationDossierPatient == true)
            {
                btnCréerCas.ToolTip = "Créer";
                txtCas1.Text = "";
                txtCas1.IsEnabled = true;
                ComboEtat.SelectedItem = null;
                ComboFact.SelectedItem = null;
                ComboFact.IsEnabled = true;
                txtDepuis.SelectedDate =DateTime.Now;
                txtDepuis.IsEnabled = true;
                txtHistoire.Text = "";
                txtHistoire.IsEnabled = true;
                txtRemarqueCas.Text = "";
                txtRemarqueCas.IsEnabled = true;
                TxtHonoraireCasTraiter.IsEnabled = true;
                TxtHonoraireCasTraiter.Text = "0";
                modifcas = false;
            }
        }

        private void btnSuppCas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CasDataGrid.SelectedItem != null && memberuser.SuppressionDossierPatient == true)
                {
                    bool succescassupp = false;
                   
                    SVC.CasTraitement selectedcastaitement = CasDataGrid.SelectedItem as SVC.CasTraitement;
                    if (selectedcastaitement.ModeFacturationint != 0)
                    {

                        using (var ts = new TransactionScope())
                        {
                            if (selectedcastaitement.Versement == 0)
                            {
                                proxy.DeleteTypeTraitement(selectedcastaitement);
                                succescassupp = true;

                               
                               

                                if (succescassupp)
                                {
                                    ts.Complete();
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Cas à été supprimer aves succées", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous ne pouvez pas faire cette opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        proxy.AjouterCasTraitementRefresh();
                        txtCas1.Text = "";
                        txtCas1.IsEnabled = false;
                        txtDepuis.SelectedDate = null;
                        txtDepuis.IsEnabled = false;
                        txtHistoire.Text = "";
                        txtHistoire.IsEnabled = false;
                        txtRemarqueCas.Text = "";
                        txtRemarqueCas.IsEnabled = false;
                        TxtHonoraireCasTraiter.Text = "0";
                        TxtHonoraireCasTraiter.Visibility = Visibility.Collapsed;
                        ComboFact.SelectedIndex = -1;
                        ComboEtat.SelectedIndex = -1;
                    }
                    else
                    {
                        if (selectedcastaitement.ModeFacturationint == 0)
                        {

                            using (var ts = new TransactionScope())
                            {
                                proxy.DeleteTypeTraitement(selectedcastaitement);
                                ts.Complete();
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Cas à été supprimer aves succées", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            proxy.AjouterCasTraitementRefresh();
         
                        }
                    }


                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimerCas_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (memberuser.ImpressionDossierPatient == true)
            {
                if (CasDataGrid.SelectedItem == null)
                {
                    List<SVC.CasTraitement> test = CasDataGrid.ItemsSource as List<SVC.CasTraitement>;


                    ImpressionCasTraitement clsho = new ImpressionCasTraitement(proxy, test.ToList());
                    clsho.Show();
                }
                else
                {
                    if (CasDataGrid.SelectedItem != null)
                    {
                        SVC.CasTraitement SelectMedecin = CasDataGrid.SelectedItem as SVC.CasTraitement;



                        ImpressionOneCas clsho = new ImpressionOneCas(proxy, SelectMedecin);
                        clsho.Show();
                    }

                }
                }
            }catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
            }

        }

        

     

       

        private void txtImagerie_Click(object sender, RoutedEventArgs e)
        {
            /*MainForm dd = new MainForm();
            dd.Show();*/
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            dd = new DicomDecoder();
            pixels8 = new List<byte>();
            pixels16 = new List<ushort>();
            pixels24 = new List<byte>();
            imageOpened = false;
            signedImage = false;
            maxPixelValue = 0;
            minPixelValue = 65535;
        }

        private void btnNewCons_Click(object sender, RoutedEventArgs e)
        {
            ConstaneDétailGrid.IsEnabled = true;
            DatePrise.SelectedDate = DateTime.Now;
            autreexamentab.IsEnabled = true;
      
            constanteCréer = new SVC.Constante
            {
                CodePatient = PATIENT.Id,
                créatininesanguin = 0,
                créatinineurinaire = 0,
                Date1 = DatePrise.SelectedDate,
                Date2 = DateTime.Now.TimeOfDay,
                GlycémieAjeun = 0,
                GlycémiePostprandiale = 0,
                HbA1c = 0,
                HDL_cholestérol = 0,
                Cholestéroltotal=0,
                IMC = 0,
                LDL_cholestérol = 0,
                Micro_albuminurie = 0,
                Nom = PATIENT.Nom,
                PAD = 0,
                PAS = 0,
                Poid = 0,
                Pouls = 0,
                Prénom = PATIENT.Prénom,
                Taille = 0,
                Temp = 0,
                triglycérides = 0,
                Uréesanguine = 0,
                Uréeurinaire = 0,
                UserName = memberuser.UserName,
                Remarque = "Control simple",
            };
            txtTime.Text =Convert.ToString(constanteCréer.Date2);
            ConstaneDétailGrid.DataContext = constanteCréer;
            CréerConstante.Content = "Créer";
            modif = false;

            autrebilanlist = new List<SVC.AutreBilan>();
            GridDataGrid.DataContext = autrebilanlist;
            GridDataGrid.ItemsSource= autrebilanlist;
        }

        private void DatagridImagerie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DatagridImagerie.SelectedItem != null)
                {
                    SVC.DicomFichier SelectMedecin = DatagridImagerie.SelectedItem as SVC.DicomFichier;
                    //   ReadAndDisplayDownloadDicomFile((proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString())),SelectMedecin.NomFichierDicom);
                  //  Stream stream = new MemoryStream(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));
                    BinaryWriter binWriter = new BinaryWriter(new MemoryStream());
                    binWriter.Write(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));
                    //  readBinary = new BinaryReader(stream);
                    readBinary = new BinaryReader(binWriter.BaseStream);
                    ReadAndDisplayDicomFileBinaryReader(readBinary, SelectMedecin.NomFichierDicom);

                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
            }
        }

        private void btnNewDicom1_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.CréationDossierPatient == true)
            {
                op = new Microsoft.Win32.OpenFileDialog();
                op.Title = "Select a picture";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";
                if (op.ShowDialog() == true)
                {
                    imgPhotod.Source = new BitmapImage(new Uri(op.FileName));
                }


            }
        }

        private void btnSuppDicom1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImprimer1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btndéfiltrer1_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                bool ImageResultTransfer = false;
                bool InsertImageResult = false;
                using (var ts = new TransactionScope())
                {
                    serverfilepath = op.FileName;

                filepath = "";
                if (serverfilepath != "")
                {
                        string dada = op.SafeFileName;//+PATIENT.Id;
                    filepath = op.FileName;

                    serverfilepath = @"ImagesGalerie\" + dada;
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
                        ImageResultTransfer = true;
                            /*Il se peut que les informations dicom sont vide il faudrait traité ce cas*/
                            SVC.DicomFichier cas = new SVC.DicomFichier
                            {
                                PatientNameInDicom = dd.NomPatient,
                                AcquisitionDate = DateTime.Now.ToString(),
                                AcquisitionTime = DateTime.Now.TimeOfDay.ToString(),
                                Nom = PATIENT.Nom,
                                Prénom = PATIENT.Prénom,
                                NomFichierDicom = dada,
                                ChemainFichier = serverfilepath,
                                Dicom = false,
                                Image = true,
                            CodePatient=PATIENT.Id,
                        };
                           proxy.InsertDicomFichier(cas);
                            InsertImageResult = true;
                    }
                        if (ImageResultTransfer && InsertImageResult)
                        {
                            ts.Complete();
                            MessageBoxResult resul03s = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            op = null;
                        }
                        else
                        {
                            MessageBoxResult resul03d = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                        

                    }
                    
                   


                }
                proxy.AjouterDicomImageRefresh();

               
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;

            }
        }

       

      

      
        private void btnZOOM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionDossierPatient == true)
                {
                    if (DatagridImagerie.SelectedItem != null)
                    {


                        SVC.DicomFichier SelectMedecin = DatagridImagerie.SelectedItem as SVC.DicomFichier;

                        Dicom sc = new Dicom(proxy, SelectMedecin);
                        sc.Show();


                    }
                    else
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Votre selection est vide ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        imagePanelControl.Invalidate();
                    }


                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
            }
        }

      
        private void cbFamille_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cbFamille.ItemsSource = proxy.GetAllFamilleAliment();
            FamilleAlimentDataGrid.ItemsSource = proxy.GetAllAliment().OrderBy(r => r.FamilleAlimentDesign);
           
        }

        private void cbFamille_DropDownClosed(object sender, EventArgs e)
        {
            if (cbFamille.SelectedItem != null)

            {
                SVC.FamilleAliment selectaliment = cbFamille.SelectedItem as SVC.FamilleAliment;

                string filter = selectaliment.FamilleAlimentDesign;


                ICollectionView cv = CollectionViewSource.GetDefaultView(FamilleAlimentDataGrid.ItemsSource);

                cv.Filter = o =>
                {
                    SVC.Aliment p = o as SVC.Aliment;
                    return (p.FamilleAlimentDesign.ToUpper().Contains(filter.ToUpper()));
                };


            }
        }
        public void update_size(object sender, RoutedEventArgs e)
        {
            if (ofd != null && DatagridImagerie.SelectedItem == null)
            {
                if (ofd.FileName.Length > 0)
                {
                    Cursor = System.Windows.Input.Cursors.Wait;
                    ReadAndDisplayDicomFile(ofd.FileName, ofd.SafeFileName);
                    imageOpened = true;
                    Cursor = System.Windows.Input.Cursors.Arrow;
                }
            }
            else
            {
                if (DatagridImagerie.SelectedItem != null)
                {
                    Cursor = System.Windows.Input.Cursors.Wait;
                    SVC.DicomFichier SelectMedecin = DatagridImagerie.SelectedItem as SVC.DicomFichier;
                    //   ReadAndDisplayDownloadDicomFile((proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString())),SelectMedecin.NomFichierDicom);
                    //  Stream stream = new MemoryStream(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));
                    BinaryWriter binWriter = new BinaryWriter(new MemoryStream());
                    binWriter.Write(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));
                    //  readBinary = new BinaryReader(stream);
                    readBinary = new BinaryReader(binWriter.BaseStream);
                    ReadAndDisplayDicomFileBinaryReader(readBinary, SelectMedecin.NomFichierDicom);
                    Cursor = System.Windows.Input.Cursors.Arrow;
                }

            }

        }

        private void btndéfiltrerDicom_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                bool ImageResultTransfer = false;
                bool InsertImageResult = false;
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    serverfilepath = ofd.FileName;

                    filepath = "";
                    if (serverfilepath != "")
                    {
                        string dada = ofd.SafeFileName + PATIENT.Id;
                        filepath = ofd.FileName;

                        serverfilepath = @"ServerPacs\" + dada;
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
                            ImageResultTransfer = true;
                            /*Il se peut que les informations dicom sont vide il faudrait traité ce cas*/
                            SVC.DicomFichier cas = new SVC.DicomFichier
                            {
                                PatientNameInDicom = dd.NomPatient,
                                AcquisitionDate = dd.AcquisitionDateString,
                                AcquisitionTime = dd.AcquisitionTimeStirng,
                                Nom = PATIENT.Nom,
                                Prénom = PATIENT.Prénom,
                                NomFichierDicom = dada,
                                ChemainFichier = serverfilepath,
                                Dicom = true,
                                Image = false,
                                CodePatient=PATIENT.Id,
                            };
                            proxy.InsertDicomFichier(cas);
                            InsertImageResult = true;
                        }


                    }
                    if (ImageResultTransfer && InsertImageResult)
                    {
                        ts.Complete();
                        //ofd = null;
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                }

                proxy.AjouterDicomImageRefresh();

            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;

            }
        }

       

        private void CasDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CasDataGrid.SelectedItem != null)
            {
                SVC.CasTraitement SelectMedecin = CasDataGrid.SelectedItem as SVC.CasTraitement;
                //   txtCas.DataContext = SelectMedecin;
                txtDepuis.SelectedDate = SelectMedecin.Date;
                txtDepuis.IsEnabled = true;
                //  List<SVC.TypeCa> test = proxy.GetAllTypeCas();
                //txtCas1.ItemsSource = test;
                //List<SVC.TypeCa> tt = test.Where(n => n.Id== SelectMedecin.IdCas).ToList();
                //txtCas1.SelectedItem = tt.First();



                txtCas1.Text = SelectMedecin.Cas;
                txtCas1.IsEnabled = true;
                txtHistoire.Text = SelectMedecin.Histoire;
                txtHistoire.IsEnabled = true;
                txtRemarqueCas.Text = SelectMedecin.Remarques;
                txtRemarqueCas.IsEnabled = true;
                ComboEtat.SelectedValue = SelectMedecin.EtatActuel;
                ComboEtat.IsEnabled = true;
                ComboFact.SelectedValue = SelectMedecin.ModeFacturation;
                ComboFact.IsEnabled = true;
                if (SelectMedecin.ModeFacturationint== 2)
                {
                    labelhonoraire.Visibility = Visibility.Visible;
                    TxtHonoraireCasTraiter.Visibility = Visibility.Visible;
                    TxtHonoraireCasTraiter.IsEnabled = true;
                    TxtHonoraireCasTraiter.Text =Convert.ToString(SelectMedecin.Montant);
                }
                else
                {
                    labelhonoraire.Visibility = Visibility.Collapsed;
                    TxtHonoraireCasTraiter.Visibility = Visibility.Collapsed;
                    TxtHonoraireCasTraiter.Text = Convert.ToString(SelectMedecin.Montant);

                    TxtHonoraireCasTraiter.IsEnabled = true;
                }
                btnCréerCas.ToolTip = "Modifier";
                modifcas = true;

            }

        }


        /************************************Hygiéne Alimentaire****************************//// <summary>
    
        private void btncolor_Click(object sender, RoutedEventArgs e)
        {
            FamilleAlimentDataGrid.ItemsSource = proxy.GetAllAliment();
            cbFamille.ItemsSource = (proxy.GetAllFamilleAliment()).OrderBy(r => r.FamilleAlimentDesign);
         //   Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(ProcessRowsAlimentIG));
        }
        void CalculeDeCalorieDétail(SVC.BesoinCalorique selectaliment)
        {
          
            txtlabeltotalCalorique.Content = Convert.ToString(selectaliment.DEJ);
            txtlabeltotalProtéines.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.ProtéinesPourcentage) / 100);
            txtlabeltotalLipides.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.LipidesPourcentage) / 100);
            txtlabeltotalGlucides.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.GlucidesPourcentage) / 100);
            txtPourProt.Text = Convert.ToString(selectaliment.ProtéinesPourcentage);
            txtPourLipide.Text = Convert.ToString(selectaliment.LipidesPourcentage);
            txtPourGlicu.Text = Convert.ToString(selectaliment.GlucidesPourcentage);


            txtPetitDéje.Text = Convert.ToString(selectaliment.PetitDéjPourcentage);
            txtCollation.Text = Convert.ToString(selectaliment.CollationPourcentage);
            txtDéjeuner.Text = Convert.ToString(selectaliment.DéjeunerPourcentage);
            txtGouter.Text = Convert.ToString(selectaliment.GouterPourcentage);
            txtDiner.Text = Convert.ToString(selectaliment.DinerPourcentage);
            txtLeSoir.Text = Convert.ToString(selectaliment.LesoirPourcentage);

            labelLesoir.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.LesoirPourcentage) / 100);
            labelDiner.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.DinerPourcentage) / 100);
            Gouter16.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.GouterPourcentage) / 100);
            labelDéjeuner.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.DéjeunerPourcentage) / 100);
            Collation10.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.CollationPourcentage) / 100);
            labelPetid.Content = Convert.ToString(selectaliment.DEJ * Convert.ToDecimal(selectaliment.PetitDéjPourcentage) / 100);
        }
       
        private void txtDate_CalendarClosed(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewBesoinCalorique_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.CréationDossierPatient==true)
            {
                CalculeCalorique cl = new CalculeCalorique(proxy, memberuser,PATIENT);
                    cl.Show();
            }
        }

        private void btnSuppBesoinCalorique_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.SuppressionPatient == true && BesoinCaloriqueGrid.SelectedItem!=null)
            {
                SVC.BesoinCalorique selectaliment = BesoinCaloriqueGrid.SelectedItem as SVC.BesoinCalorique;
                var found = (proxy.GetAllRepa()).Find(itemf => itemf.Clé == selectaliment.Clé);
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    bool deleteCalorique = false;
                    bool deleteRepa = false;
             
                    proxy.DeletBesoinCaloriqueAsync(selectaliment);
                    deleteCalorique = true;
                  
                    int interfaceDelete = 0;
                    if (selectaliment.RepasBoolPetitDéj == true || selectaliment.RepasBoolCollation == true || selectaliment.RepasBoolDéjeuner == true || selectaliment.RepasBoolGouter == true || selectaliment.RepasBoolDiner == true || selectaliment.RepasBoolLesoir == true)
                    {
                        interfaceDelete = 1;
                       
                     
                        if (found != null)
                        {
                            var s = proxy.GetAllRepa().Where(n => n.Clé == selectaliment.Clé).ToList();
                            for (int i = s.Count - 1; i >= 0; i--)

                            {
                                deleteRepa = false;
                                var item = s.ElementAt(i);
                                proxy.DeleteRepa(item);
                                deleteRepa = true;

                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(item.DésignationRepas + item.Id, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            deleteRepa = true;
                        }
                    }
                    if (deleteCalorique && deleteRepa && interfaceDelete == 1)
                    {
                        ts.Complete();
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (deleteCalorique && interfaceDelete == 0)
                        {
                            ts.Complete();
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

            }
        }
        public void ChartRepartionRepas(SVC.BesoinCalorique BsCal)
            {
               /*    List<KeyValuePair<string, decimal>> MyValue2 = new List<KeyValuePair<string, decimal>>();
               MyValue2.Add(new KeyValuePair<string, decimal>("Le soir", Convert.ToDecimal(BsCal.kcalLesoir)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Diner", Convert.ToDecimal(BsCal.kcalDiner)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Gôuter", Convert.ToDecimal(BsCal.kcalGouter)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Déjeuner", Convert.ToDecimal(BsCal.kcalDéjeuner)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Collation(10 heures)", Convert.ToDecimal(BsCal.kcalCollation)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Petit Déjeuner", Convert.ToDecimal(BsCal.kcalPetitDéj)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Ration atteinte", Convert.ToDecimal(BsCal.Kcal)));
                MyValue2.Add(new KeyValuePair<string, decimal>("Besoin Calorique", Convert.ToDecimal(BsCal.DEJ)));*/

            //BarChart2.DataContext = MyValue2;
            List<KeyValuePair<string, decimal>> MyValue = new List<KeyValuePair<string, decimal>>();
            MyValue.Add(new KeyValuePair<string, decimal>("Glucides atteinte", Convert.ToDecimal(BsCal.GlucidesNewValeur)));
            MyValue.Add(new KeyValuePair<string, decimal>("Glucides", Convert.ToDecimal((BsCal.GlucidesPourcentage * BsCal.DEJ) / 100)));

            MyValue.Add(new KeyValuePair<string, decimal>("Lipides atteinte", Convert.ToDecimal(BsCal.LipidesNewValeur)));
            MyValue.Add(new KeyValuePair<string, decimal>("Lipides", Convert.ToDecimal((BsCal.LipidesPourcentage * BsCal.DEJ) / 100)));

            MyValue.Add(new KeyValuePair<string, decimal>("protéines atteinte", Convert.ToDecimal(BsCal.ProtéinesNewValeur)));
            MyValue.Add(new KeyValuePair<string, decimal>("protéines", Convert.ToDecimal((BsCal.ProtéinesPourcentage* BsCal.DEJ)/100)));
            BarChart1.DataContext = MyValue;
       
        }
        private void BtnPresentExiste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueGrid.SelectedItem != null)
                {
                    SVC.BesoinCalorique selectaliment = BesoinCaloriqueGrid.SelectedItem as SVC.BesoinCalorique;
                    

                        BesoinCaloriqueCalculé = selectaliment;
              
                    CalculeDeCalorieDétail(selectaliment);
                      detailcalculeGlobal.Visibility = Visibility.Visible;
                     GridSpliterCalcule.Visibility = Visibility.Visible;
                   RepasTabControl.Visibility = Visibility.Visible;
                    detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                  detailcalcule.DataContext = BesoinCaloriqueCalculé;
                        detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                    ChartRepartionRepas(BesoinCaloriqueCalculé);
                    BarChart1.Visibility = Visibility.Visible;
                    richTextBox1.Visibility = Visibility.Collapsed;
                    richTextBox2.Visibility = Visibility.Collapsed;
                    labelRepas.Visibility = Visibility.Visible;
                    cc.Visibility = Visibility.Visible;
                    labelRepasCalTotal.Visibility = Visibility.Visible;
                    labelRepasCalAteint.Visibility = Visibility.Visible;
                    labelRepasCalAteintPourcent.Visibility = Visibility.Visible;
                    PetitDéjeunerDataGrid.Visibility = Visibility.Visible;
                    btnDejuner.Visibility = Visibility.Visible;
                    btnDejunerImpr.Visibility = Visibility.Visible;
                    txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                    txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                    txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur; 



               
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }
        private void FamilleAlimentDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                switch (TabInterface)
                {
                    case 1:
                        if (TabPetitDéj.IsEnabled == true)
                            {
                            if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolPetitDéj == false)
                            {
                                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                SVC.Repa DD = new SVC.Repa
                                {
                                    CodeAliment = selectaliment.Id,
                                    FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                    DesignProduit = selectaliment.DesignProduit,
                                    Kcal = selectaliment.Kcal,
                                    Protéines = selectaliment.Protéines,
                                    Lipides = selectaliment.Lipides,
                                    Glucides = selectaliment.Glucides,
                                    ValeurGramme = selectaliment.ValeurGramme,
                                    IG = selectaliment.IG,
                                    Clé = BesoinCaloriqueCalculé.Clé,
                                    DésignationRepas = "Petit Déjeuner",
                                    PourcentageRepas = BesoinCaloriqueCalculé.PetitDéjPourcentage,
                                    InterfaceRepas = 1,
                                    Quantite = 0,
                                    ProtéinesNew = 0,
                                    LipidesNew = 0,
                                    GlucidesNew = 0,
                                    KcalNew = 0,
                                    CodePatient=PATIENT.Id,
                                };
                                var found = PetitDejList.Find(item => item.CodeAliment == selectaliment.Id);
                                if (found == null)
                                {
                                    PetitDejList.Add(DD);

                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }


                                PetitDéjeunerDataGrid.ItemsSource = PetitDejList;
                                CollectionViewSource.GetDefaultView(PetitDéjeunerDataGrid.ItemsSource).Refresh();

                                labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                labelRepasCalAteintPourcent.Content = string.Format("{0:0.##}", ((PetitDejList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                            }
                            else
                            {
                                if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolPetitDéj == true)
                                {
                                    SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                    SVC.Repa DD = new SVC.Repa
                                    {
                                        CodeAliment = selectaliment.Id,
                                        FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                        DesignProduit = selectaliment.DesignProduit,
                                        Kcal = selectaliment.Kcal,
                                        Protéines = selectaliment.Protéines,
                                        Lipides = selectaliment.Lipides,
                                        Glucides = selectaliment.Glucides,
                                        ValeurGramme = selectaliment.ValeurGramme,
                                        IG = selectaliment.IG,
                                        Clé = BesoinCaloriqueCalculé.Clé,
                                        DésignationRepas = "Petit Déjeuner",
                                        PourcentageRepas = BesoinCaloriqueCalculé.PetitDéjPourcentage,
                                        InterfaceRepas = 1,
                                        Quantite = 0,
                                        ProtéinesNew = 0,
                                        LipidesNew = 0,
                                        GlucidesNew = 0,
                                        KcalNew = 0,
                                        CodePatient = PATIENT.Id,
                                    };
                                    var found = PetitDejList.Find(item => item.CodeAliment == selectaliment.Id);
                                    if (found == null)
                                    {
                                        PetitDejList.Add(DD);

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                    PetitDéjeunerDataGrid.ItemsSource = PetitDejList;
                                    CollectionViewSource.GetDefaultView(PetitDéjeunerDataGrid.ItemsSource).Refresh();

                                    labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                    labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                    labelRepasCalAteintPourcent.Content = string.Format("{0:0.##}", ((PetitDejList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                                }

                            }
                        }
                        break;
                    case 2:
                        if (TabCollation.IsEnabled == true)
                        {
                            if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolCollation == false)
                            {
                                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                SVC.Repa DD = new SVC.Repa
                                {
                                    CodeAliment = selectaliment.Id,
                                    FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                    DesignProduit = selectaliment.DesignProduit,
                                    Kcal = selectaliment.Kcal,
                                    Protéines = selectaliment.Protéines,
                                    Lipides = selectaliment.Lipides,
                                    Glucides = selectaliment.Glucides,
                                    ValeurGramme = selectaliment.ValeurGramme,
                                    IG = selectaliment.IG,
                                    Clé = BesoinCaloriqueCalculé.Clé,
                                    DésignationRepas = "Collation",
                                    PourcentageRepas = BesoinCaloriqueCalculé.CollationPourcentage,
                                    InterfaceRepas = 2,
                                    Quantite = 0,
                                    ProtéinesNew = 0,
                                    LipidesNew = 0,
                                    GlucidesNew = 0,
                                    KcalNew = 0,
                                    CodePatient = PATIENT.Id,
                                };
                                var found = CollationList.Find(item => item.CodeAliment == selectaliment.Id);
                                if (found == null)
                                {
                                    CollationList.Add(DD);

                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }


                                CollationDataGrid.ItemsSource = CollationList;
                                CollectionViewSource.GetDefaultView(CollationDataGrid.ItemsSource).Refresh();

                                labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                labelRepasCalAteintPourcentCollation.Content = string.Format("{0:0.##}", ((CollationList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                            }
                            else
                            {
                                if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolCollation == true)
                                {
                                    SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                    SVC.Repa DD = new SVC.Repa
                                    {
                                        CodeAliment = selectaliment.Id,
                                        FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                        DesignProduit = selectaliment.DesignProduit,
                                        Kcal = selectaliment.Kcal,
                                        Protéines = selectaliment.Protéines,
                                        Lipides = selectaliment.Lipides,
                                        Glucides = selectaliment.Glucides,
                                        ValeurGramme = selectaliment.ValeurGramme,
                                        IG = selectaliment.IG,
                                        Clé = BesoinCaloriqueCalculé.Clé,
                                        DésignationRepas = "Collation",
                                        PourcentageRepas = BesoinCaloriqueCalculé.CollationPourcentage,
                                        InterfaceRepas = 2,
                                        Quantite = 0,
                                        ProtéinesNew = 0,
                                        LipidesNew = 0,
                                        GlucidesNew = 0,
                                        KcalNew = 0,
                                        CodePatient = PATIENT.Id,
                                    };
                                    var found = CollationList.Find(item => item.CodeAliment == selectaliment.Id);
                                    if (found == null)
                                    {
                                        CollationList.Add(DD);

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                    CollationDataGrid.ItemsSource = CollationList;
                                    CollectionViewSource.GetDefaultView(CollationDataGrid.ItemsSource).Refresh();

                                    labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                    labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                    labelRepasCalAteintPourcentCollation.Content = string.Format("{0:0.##}", ((CollationList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                                }

                            }
                        }
                        break;
                    case 3:
                        if (TabDéjeuner.IsEnabled == true)
                        {
                            if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDéjeuner == false)
                            {
                                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                SVC.Repa DD = new SVC.Repa
                                {
                                    CodeAliment = selectaliment.Id,
                                    FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                    DesignProduit = selectaliment.DesignProduit,
                                    Kcal = selectaliment.Kcal,
                                    Protéines = selectaliment.Protéines,
                                    Lipides = selectaliment.Lipides,
                                    Glucides = selectaliment.Glucides,
                                    ValeurGramme = selectaliment.ValeurGramme,
                                    IG = selectaliment.IG,
                                    Clé = BesoinCaloriqueCalculé.Clé,
                                    DésignationRepas = "Déjeuner",
                                    PourcentageRepas = BesoinCaloriqueCalculé.DéjeunerPourcentage,
                                    InterfaceRepas = 3,
                                    Quantite = 0,
                                    ProtéinesNew = 0,
                                    LipidesNew = 0,
                                    GlucidesNew = 0,
                                    KcalNew = 0,
                                    CodePatient = PATIENT.Id,
                                };
                                var found = DéjeunerList.Find(item => item.CodeAliment == selectaliment.Id);
                                if (found == null)
                                {
                                    DéjeunerList.Add(DD);

                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }


                                DéjeunerDataGrid.ItemsSource = DéjeunerList;
                                CollectionViewSource.GetDefaultView(DéjeunerDataGrid.ItemsSource).Refresh();

                                labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                labelRepasCalAteintPourcentDéjeuner.Content = string.Format("{0:0.##}", ((DéjeunerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                            }
                            else
                            {
                                if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDéjeuner == true)
                                {
                                    SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                    SVC.Repa DD = new SVC.Repa
                                    {
                                        CodeAliment = selectaliment.Id,
                                        FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                        DesignProduit = selectaliment.DesignProduit,
                                        Kcal = selectaliment.Kcal,
                                        Protéines = selectaliment.Protéines,
                                        Lipides = selectaliment.Lipides,
                                        Glucides = selectaliment.Glucides,
                                        ValeurGramme = selectaliment.ValeurGramme,
                                        IG = selectaliment.IG,
                                        Clé = BesoinCaloriqueCalculé.Clé,
                                        DésignationRepas = "Déjeuner",
                                        PourcentageRepas = BesoinCaloriqueCalculé.DéjeunerPourcentage,
                                        InterfaceRepas =3,
                                        Quantite = 0,
                                        ProtéinesNew = 0,
                                        LipidesNew = 0,
                                        GlucidesNew = 0,
                                        KcalNew = 0,
                                        CodePatient = PATIENT.Id,
                                    };
                                    var found = DéjeunerList.Find(item => item.CodeAliment == selectaliment.Id);
                                    if (found == null)
                                    {
                                        DéjeunerList.Add(DD);

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                    DéjeunerDataGrid.ItemsSource = DéjeunerList;
                                    CollectionViewSource.GetDefaultView(DéjeunerDataGrid.ItemsSource).Refresh();

                                    labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                    labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                    labelRepasCalAteintPourcentDéjeuner.Content = string.Format("{0:0.##}", ((DéjeunerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                                }

                            }
                        }
                        break;
                    case 4:
                        if (TabGouter.IsEnabled == true)
                        {
                            if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolGouter == false)
                            {
                                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                SVC.Repa DD = new SVC.Repa
                                {
                                    CodeAliment = selectaliment.Id,
                                    FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                    DesignProduit = selectaliment.DesignProduit,
                                    Kcal = selectaliment.Kcal,
                                    Protéines = selectaliment.Protéines,
                                    Lipides = selectaliment.Lipides,
                                    Glucides = selectaliment.Glucides,
                                    ValeurGramme = selectaliment.ValeurGramme,
                                    IG = selectaliment.IG,
                                    Clé = BesoinCaloriqueCalculé.Clé,
                                    DésignationRepas = "Gôuter",
                                    PourcentageRepas = BesoinCaloriqueCalculé.GouterPourcentage,
                                    InterfaceRepas = 4,
                                    Quantite = 0,
                                    ProtéinesNew = 0,
                                    LipidesNew = 0,
                                    GlucidesNew = 0,
                                    KcalNew = 0,
                                    CodePatient = PATIENT.Id,
                                };
                                var found = GouterList.Find(item => item.CodeAliment == selectaliment.Id);
                                if (found == null)
                                {
                                    GouterList.Add(DD);

                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }


                                GouterDataGrid.ItemsSource = GouterList;
                                CollectionViewSource.GetDefaultView(GouterDataGrid.ItemsSource).Refresh();

                                labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                labelRepasCalAteintPourcentGouter.Content = string.Format("{0:0.##}", ((GouterList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                            }
                            else
                            {
                                if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolGouter == true)
                                {
                                    SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                    SVC.Repa DD = new SVC.Repa
                                    {
                                        CodeAliment = selectaliment.Id,
                                        FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                        DesignProduit = selectaliment.DesignProduit,
                                        Kcal = selectaliment.Kcal,
                                        Protéines = selectaliment.Protéines,
                                        Lipides = selectaliment.Lipides,
                                        Glucides = selectaliment.Glucides,
                                        ValeurGramme = selectaliment.ValeurGramme,
                                        IG = selectaliment.IG,
                                        Clé = BesoinCaloriqueCalculé.Clé,
                                        DésignationRepas = "Gôuter",
                                        PourcentageRepas = BesoinCaloriqueCalculé.GouterPourcentage,
                                        InterfaceRepas = 4,
                                        Quantite = 0,
                                        ProtéinesNew = 0,
                                        LipidesNew = 0,
                                        GlucidesNew = 0,
                                        KcalNew = 0,
                                        CodePatient = PATIENT.Id,
                                    };
                                    var found = GouterList.Find(item => item.CodeAliment == selectaliment.Id);
                                    if (found == null)
                                    {
                                        GouterList.Add(DD);

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                    GouterDataGrid.ItemsSource = GouterList;
                                    CollectionViewSource.GetDefaultView(GouterDataGrid.ItemsSource).Refresh();

                                    labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                    labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                    labelRepasCalAteintPourcentGouter.Content = string.Format("{0:0.##}", ((GouterList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                                }

                            }
                        }
                        break;
                    case 5:
                        if (TabDiner.IsEnabled == true)
                        {
                            if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDiner == false)
                            {
                                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                SVC.Repa DD = new SVC.Repa
                                {
                                    CodeAliment = selectaliment.Id,
                                    FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                    DesignProduit = selectaliment.DesignProduit,
                                    Kcal = selectaliment.Kcal,
                                    Protéines = selectaliment.Protéines,
                                    Lipides = selectaliment.Lipides,
                                    Glucides = selectaliment.Glucides,
                                    ValeurGramme = selectaliment.ValeurGramme,
                                    IG = selectaliment.IG,
                                    Clé = BesoinCaloriqueCalculé.Clé,
                                    DésignationRepas = "Diner",
                                    PourcentageRepas = BesoinCaloriqueCalculé.DinerPourcentage,
                                    InterfaceRepas = 5,
                                    Quantite = 0,
                                    ProtéinesNew = 0,
                                    LipidesNew = 0,
                                    GlucidesNew = 0,
                                    KcalNew = 0,
                                    CodePatient = PATIENT.Id,
                                };
                                var found = DinerList.Find(item => item.CodeAliment == selectaliment.Id);
                                if (found == null)
                                {
                                    DinerList.Add(DD);

                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }


                                DinerDataGrid.ItemsSource = DinerList;
                                CollectionViewSource.GetDefaultView(DinerDataGrid.ItemsSource).Refresh();

                                labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                labelRepasCalAteintPourcentDiner.Content = string.Format("{0:0.##}", ((DinerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                            }
                            else
                            {
                                if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDiner == true)
                                {
                                    SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                    SVC.Repa DD = new SVC.Repa
                                    {
                                        CodeAliment = selectaliment.Id,
                                        FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                        DesignProduit = selectaliment.DesignProduit,
                                        Kcal = selectaliment.Kcal,
                                        Protéines = selectaliment.Protéines,
                                        Lipides = selectaliment.Lipides,
                                        Glucides = selectaliment.Glucides,
                                        ValeurGramme = selectaliment.ValeurGramme,
                                        IG = selectaliment.IG,
                                        Clé = BesoinCaloriqueCalculé.Clé,
                                        DésignationRepas = "Diner",
                                        PourcentageRepas = BesoinCaloriqueCalculé.DinerPourcentage,
                                        InterfaceRepas = 5,
                                        Quantite = 0,
                                        ProtéinesNew = 0,
                                        LipidesNew = 0,
                                        GlucidesNew = 0,
                                        KcalNew = 0,
                                        CodePatient = PATIENT.Id,
                                    };
                                    var found = DinerList.Find(item => item.CodeAliment == selectaliment.Id);
                                    if (found == null)
                                    {
                                        DinerList.Add(DD);

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                    DinerDataGrid.ItemsSource = DinerList;
                                    CollectionViewSource.GetDefaultView(DinerDataGrid.ItemsSource).Refresh();

                                    labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                    labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                    labelRepasCalAteintPourcentDiner.Content = string.Format("{0:0.##}", ((DinerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                                }

                            }
                        }
                        break;
                    case 6:
                        if (TabLeSoir.IsEnabled == true)
                        {
                            if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolLesoir == false)
                            {
                                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                SVC.Repa DD = new SVC.Repa
                                {
                                    CodeAliment = selectaliment.Id,
                                    FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                    DesignProduit = selectaliment.DesignProduit,
                                    Kcal = selectaliment.Kcal,
                                    Protéines = selectaliment.Protéines,
                                    Lipides = selectaliment.Lipides,
                                    Glucides = selectaliment.Glucides,
                                    ValeurGramme = selectaliment.ValeurGramme,
                                    IG = selectaliment.IG,
                                    Clé = BesoinCaloriqueCalculé.Clé,
                                    DésignationRepas = "Le Soir",
                                    PourcentageRepas = BesoinCaloriqueCalculé.LesoirPourcentage,
                                    InterfaceRepas = 6,
                                    Quantite = 0,
                                    ProtéinesNew = 0,
                                    LipidesNew = 0,
                                    GlucidesNew = 0,
                                    KcalNew = 0,
                                    CodePatient = PATIENT.Id,
                                };
                                var found = LeSoirList.Find(item => item.CodeAliment == selectaliment.Id);
                                if (found == null)
                                {
                                    LeSoirList.Add(DD);

                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }


                                LeSoirDataGrid.ItemsSource = LeSoirList;
                                CollectionViewSource.GetDefaultView(LeSoirDataGrid.ItemsSource).Refresh();

                                labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                labelRepasCalAteintPourcentLeSoir.Content = string.Format("{0:0.##}", ((LeSoirList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                            }
                            else
                            {
                                if (FamilleAlimentDataGrid.SelectedItem != null && BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolLesoir == true)
                                {
                                    SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                                    SVC.Repa DD = new SVC.Repa
                                    {
                                        CodeAliment = selectaliment.Id,
                                        FamilleAlimentDesign = selectaliment.FamilleAlimentDesign,
                                        DesignProduit = selectaliment.DesignProduit,
                                        Kcal = selectaliment.Kcal,
                                        Protéines = selectaliment.Protéines,
                                        Lipides = selectaliment.Lipides,
                                        Glucides = selectaliment.Glucides,
                                        ValeurGramme = selectaliment.ValeurGramme,
                                        IG = selectaliment.IG,
                                        Clé = BesoinCaloriqueCalculé.Clé,
                                        DésignationRepas = "Le Soir",
                                        PourcentageRepas = BesoinCaloriqueCalculé.LesoirPourcentage,
                                        InterfaceRepas = 6,
                                        Quantite = 0,
                                        ProtéinesNew = 0,
                                        LipidesNew = 0,
                                        GlucidesNew = 0,
                                        KcalNew = 0,
                                        CodePatient = PATIENT.Id,
                                    };
                                    var found = LeSoirList.Find(item => item.CodeAliment == selectaliment.Id);
                                    if (found == null)
                                    {
                                        LeSoirList.Add(DD);

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Aliment appartient déja au repas", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                    LeSoirDataGrid.ItemsSource = LeSoirList;
                                    CollectionViewSource.GetDefaultView(LeSoirDataGrid.ItemsSource).Refresh();

                                    labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                                    labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                                    labelRepasCalAteintPourcentLeSoir.Content = string.Format("{0:0.##}", ((LeSoirList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                                }

                            }
                        }
                        break;
                    default:

                        MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez Ouvrir un repas en mode création ou modification", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        break;
                }
            }catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void btnPetitDéj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    TabPetitDéj.IsEnabled = true;

                    if (BesoinCaloriqueCalculé.RepasBoolPetitDéj == false)
                    {
                        TabPetitDéj.IsEnabled = true;
                        txtPetitDéje.IsEnabled = false;
                        PetitDéjGrid.Visibility = Visibility.Visible;
                        PetitDejList = new List<SVC.Repa>();
                        labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                        labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => o.Kcal);
                        labelRepasCalAteintPourcent.Content = (PetitDejList.AsEnumerable().Sum(o => o.Kcal)) / BesoinCaloriqueCalculé.DEJ + "" + "%";
                        TabInterface = 1;
                        PetitDéjeunerDataGrid.ItemsSource = PetitDejList;
                        PetitDéjeunerDataGrid.DataContext = PetitDejList;

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé.RepasBoolPetitDéj== true)
                        {
                            TabPetitDéj.IsEnabled = true;
                            PetitDéjGrid.Visibility = Visibility.Visible;
                            txtPetitDéje.IsEnabled = false;
                            PetitDejList = proxy.GetAllRepa().Where(n => n.DésignationRepas== "Petit Déjeuner" && n.Clé==BesoinCaloriqueCalculé.Clé).ToList();
                            PetitDéjeunerDataGrid.ItemsSource = PetitDejList;
                            PetitDéjeunerDataGrid.DataContext = PetitDejList;
                            labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                            labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                            labelRepasCalAteintPourcent.Content = string.Format("{0:0.##}", ((PetitDejList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                        }
                    }
                }
            }catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void btnCollation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    TabPetitDéj.IsEnabled = true;

                    if (BesoinCaloriqueCalculé.RepasBoolCollation == false)
                    {
                        TabCollation.IsEnabled = true;
                        txtCollation.IsEnabled = false;
                        CollationGrid.Visibility = Visibility.Visible;
                        CollationList = new List<SVC.Repa>();
                        labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                        labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => o.Kcal);
                        labelRepasCalAteintPourcentCollation.Content = (CollationList.AsEnumerable().Sum(o => o.Kcal)) / BesoinCaloriqueCalculé.DEJ + "" + "%";
                        TabInterface = 2;
                        CollationDataGrid.ItemsSource = CollationList;
                        CollationDataGrid.DataContext = CollationList;

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé.RepasBoolPetitDéj == true)
                        {
                            TabCollation.IsEnabled = true;
                            CollationGrid.Visibility = Visibility.Visible;
                            txtCollation.IsEnabled = false;
                            CollationList = proxy.GetAllRepa().Where(n => n.DésignationRepas == "Collation" && n.Clé == BesoinCaloriqueCalculé.Clé).ToList();
                            CollationDataGrid.ItemsSource = CollationList;
                            CollationDataGrid.DataContext = CollationList;
                            labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                            labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                            labelRepasCalAteintPourcentCollation.Content = string.Format("{0:0.##}", ((CollationList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnDéj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    TabPetitDéj.IsEnabled = true;

                    if (BesoinCaloriqueCalculé.RepasBoolDéjeuner == false)
                    {
                        TabDéjeuner.IsEnabled = true;
                        txtDéjeuner.IsEnabled = false;
                        DéjeunerGrid.Visibility = Visibility.Visible;
                        DéjeunerList = new List<SVC.Repa>();
                        labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                        labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => o.Kcal);
                        labelRepasCalAteintPourcentDéjeuner.Content = (DéjeunerList.AsEnumerable().Sum(o => o.Kcal)) / BesoinCaloriqueCalculé.DEJ + "" + "%";
                        TabInterface = 3;
                        DéjeunerDataGrid.ItemsSource = DéjeunerList;
                        DéjeunerDataGrid.DataContext = DéjeunerList;

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé.RepasBoolDéjeuner == true)
                        {
                            TabDéjeuner.IsEnabled = true;
                            DéjeunerGrid.Visibility = Visibility.Visible;
                            txtDéjeuner.IsEnabled = false;
                            DéjeunerList = proxy.GetAllRepa().Where(n => n.DésignationRepas == "Déjeuner" && n.Clé == BesoinCaloriqueCalculé.Clé).ToList();
                            DéjeunerDataGrid.ItemsSource = DéjeunerList;
                            DéjeunerDataGrid.DataContext = DéjeunerList;
                            labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                            labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                            labelRepasCalAteintPourcentDéjeuner.Content = string.Format("{0:0.##}", ((DéjeunerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnGouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    TabGouter.IsEnabled = true;

                    if (BesoinCaloriqueCalculé.RepasBoolGouter == false)
                    {
                        TabGouter.IsEnabled = true;
                        txtGouter.IsEnabled = false;
                        GouterGrid.Visibility = Visibility.Visible;
                        GouterList = new List<SVC.Repa>();
                        labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                        labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => o.Kcal);
                        labelRepasCalAteintPourcentGouter.Content = (GouterList.AsEnumerable().Sum(o => o.Kcal)) / BesoinCaloriqueCalculé.DEJ + "" + "%";
                        TabInterface = 4;
                        GouterDataGrid.ItemsSource = GouterList;
                        GouterDataGrid.DataContext = GouterList;

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé.RepasBoolGouter == true)
                        {
                            TabGouter.IsEnabled = true;
                            GouterGrid.Visibility = Visibility.Visible;
                            txtGouter.IsEnabled = false;
                            GouterList = proxy.GetAllRepa().Where(n => n.DésignationRepas == "Gôuter" && n.Clé == BesoinCaloriqueCalculé.Clé).ToList();
                            GouterDataGrid.ItemsSource = GouterList;
                            GouterDataGrid.DataContext = GouterList;
                            labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                            labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                            labelRepasCalAteintPourcentGouter.Content = string.Format("{0:0.##}", ((GouterList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDiner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    TabDiner.IsEnabled = true;

                    if (BesoinCaloriqueCalculé.RepasBoolDiner == false)
                    {
                        TabDiner.IsEnabled = true;
                        txtDiner.IsEnabled = false;
                        DinerGrid.Visibility = Visibility.Visible;
                        DinerList = new List<SVC.Repa>();
                        labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                        labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => o.Kcal);
                        labelRepasCalAteintPourcentDiner.Content = (DinerList.AsEnumerable().Sum(o => o.Kcal)) / BesoinCaloriqueCalculé.DEJ + "" + "%";
                        TabInterface = 5;
                        DinerDataGrid.ItemsSource = DinerList;
                        DinerDataGrid.DataContext = DinerList;

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé.RepasBoolDiner == true)
                        {
                            TabDiner.IsEnabled = true;
                            DinerGrid.Visibility = Visibility.Visible;
                            txtDiner.IsEnabled = false;
                            DinerList = proxy.GetAllRepa().Where(n => n.DésignationRepas == "Diner" && n.Clé == BesoinCaloriqueCalculé.Clé).ToList();
                            DinerDataGrid.ItemsSource = DinerList;
                            DinerDataGrid.DataContext = DinerList;
                            labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                            labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                            labelRepasCalAteintPourcentDiner.Content = string.Format("{0:0.##}", ((DinerList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLeSoir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BesoinCaloriqueCalculé != null)
                {
                    TabLeSoir.IsEnabled = true;

                    if (BesoinCaloriqueCalculé.RepasBoolLesoir == false)
                    {
                        TabLeSoir.IsEnabled = true;
                        txtLeSoir.IsEnabled = false;
                        LeSoirGrid.Visibility = Visibility.Visible;
                        LeSoirList = new List<SVC.Repa>();
                        labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                        labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => o.Kcal);
                        labelRepasCalAteintPourcentLeSoir.Content = (LeSoirList.AsEnumerable().Sum(o => o.Kcal)) / BesoinCaloriqueCalculé.DEJ + "" + "%";
                        TabInterface = 6;
                        LeSoirDataGrid.ItemsSource = LeSoirList;
                        LeSoirDataGrid.DataContext = LeSoirList;

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé.RepasBoolLesoir == true)
                        {
                            TabLeSoir.IsEnabled = true;
                            LeSoirGrid.Visibility = Visibility.Visible;
                            txtLeSoir.IsEnabled = false;
                            LeSoirList = proxy.GetAllRepa().Where(n => n.DésignationRepas == "Le Soir" && n.Clé == BesoinCaloriqueCalculé.Clé).ToList();
                            LeSoirDataGrid.ItemsSource = LeSoirList;
                            LeSoirDataGrid.DataContext = LeSoirList;
                            labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
                            labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme));
                            labelRepasCalAteintPourcentLeSoir.Content = string.Format("{0:0.##}", ((LeSoirList.AsEnumerable().Sum(o => (o.Quantite * o.Kcal / o.ValeurGramme)) / ((BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtPetitDéje_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(BesoinCaloriqueCalculé!=null && txtPetitDéje.Text!="")
            {
                

              
                labelPetid.Content = Convert.ToString(BesoinCaloriqueCalculé.DEJ * Convert.ToDecimal(txtPetitDéje.Text) / 100);
            }
        }

        private void txtCollation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BesoinCaloriqueCalculé != null && txtCollation.Text != "")
            {
               
                Collation10.Content = Convert.ToString(BesoinCaloriqueCalculé.DEJ * Convert.ToDecimal(txtCollation.Text) / 100);
            
            }
        }

        private void RepasTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabPetitDéj.IsSelected)
            {
                TabInterface = 1;
            }
            else
            {
                if (TabCollation.IsSelected)
                {
                    TabInterface = 2;
                }
                else
                {
                    if (TabDéjeuner.IsSelected)
                    {
                        TabInterface = 3;
                    }
                    else
                    {
                        if (TabGouter.IsSelected)
                        {
                            TabInterface = 4;
                        }
                        else
                        {
                            if (TabDiner.IsSelected)
                            {
                                TabInterface = 5;
                            }
                            else
                            {
                                if (TabLeSoir.IsSelected)
                                {
                                    TabInterface = 6;
                                }
                            }
                        }


                    }
                }
            }
        }

        private void BesoinCaloriqueGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void PetitDéjeunerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in PetitDejList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcent.Content = string.Format("{0:0.##}", ((PetitDejList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void PetitDéjeunerDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            foreach (var row in PetitDejList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcent.Content = string.Format("{0:0.##}", ((PetitDejList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void PetitDéjeunerDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var row in PetitDejList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotal.Content = (BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteint.Content = PetitDejList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcent.Content = string.Format("{0:0.##}", ((PetitDejList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.PetitDéjPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

      

        private void btnDejuner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabPetitDéj.IsEnabled == true)
                {
                    if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolPetitDéj == false && TabInterface == 1)
                    {
                        bool insertrepa = false;
                        bool updatebesoincal = false;
                        /*******************    il faut métre la condition quantité supp a zero pour les somme      ********************/
                        BesoinCaloriqueCalculé.RepasBoolPetitDéj = true;
                        BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal + PetitDejList.AsEnumerable().Where(o =>o.Quantite>0).Sum(o => (o.ProtéinesNew * 4)) + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        BesoinCaloriqueCalculé.kcalPetitDéj = PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        /*************************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPetitDéj= PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew)); 
                        BesoinCaloriqueCalculé.LipidesPetitDéj= PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesPetitDéj= PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                        /*****************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur + PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));
                        /*************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.LipidesPourcentageNew = ((PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        /****************************************************************************************/
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = PetitDéjeunerDataGrid.ItemsSource as IEnumerable;
                         
                            foreach (SVC.Repa item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertRepa(item);
                                    insertrepa = true;
                                }
                            }


                            proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                            updatebesoincal = true;
                            if (insertrepa && updatebesoincal)
                            {
                                ts.Complete();
                                
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        if (insertrepa && updatebesoincal)
                        {
                            /***********************************************/////

                            proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                            detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                            detailcalcule.DataContext = BesoinCaloriqueCalculé;
                            detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                            ChartRepartionRepas(BesoinCaloriqueCalculé);
                            txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                            txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                            txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;

                        }
                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolPetitDéj == true && TabInterface == 1)
                        {

                            bool insertrepa = false;
                            bool updatebesoincal = false;
                            /***********************************         .Where(o =>o.Quantite>0)                             *//////////////
                            BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal-BesoinCaloriqueCalculé.kcalPetitDéj+ PetitDejList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + PetitDejList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + PetitDejList.AsEnumerable().Sum(o => (o.LipidesNew * 9));
                             BesoinCaloriqueCalculé.kcalPetitDéj = PetitDejList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + PetitDejList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + PetitDejList.AsEnumerable().Sum(o => (o.LipidesNew * 9));

       
                            

                            /*****************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur - BesoinCaloriqueCalculé.ProtéinesPetitDéj + PetitDejList.AsEnumerable().Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur -BesoinCaloriqueCalculé.LipidesPetitDéj+ PetitDejList.AsEnumerable().Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur-BesoinCaloriqueCalculé.GlucidesPetitDéj + PetitDejList.AsEnumerable().Sum(o => (o.GlucidesNew));
                             /************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesPetitDéj = PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesPetitDéj = PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesPetitDéj = PetitDejList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                            /*************************************************************************/
                            if (BesoinCaloriqueCalculé.Kcal != 0)
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((PetitDejList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = ((PetitDejList.AsEnumerable().Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((PetitDejList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                            }else
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = 0;
                            }
                            /****************************************************************************************/
                        
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                List<SVC.Repa> itemsSource0 = PetitDéjeunerDataGrid.ItemsSource as List<SVC.Repa>;

                                //    for (int i = itemsSource0.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                //    {
                                List<SVC.Repa> itemsSourcerecept = (proxy.GetAllRepa()).Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas==1).ToList();
                                foreach (SVC.Repa item in itemsSource0)
                                {
                                  //  var item = itemsSource0.ElementAt(i);
                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);
                                    if (found == null)
                                    {

                                        if (item.Quantite > 0)
                                        {
                                            proxy.InsertRepa(item);
                                            insertrepa = true;
                                        }

                                    }
                                    else
                                    {/*Le cas ou l'aliment existe déja*/
                                        if (found != null)
                                        {
                                            if (item.Quantite > 0)
                                            {
                                                proxy.UpdateRepa(item);
                                                insertrepa = true;
                                            }
                                            else
                                            {
                                                if (item.Quantite == 0)
                                                {
                                                    proxy.DeleteRepa(item);
                                                    insertrepa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                                updatebesoincal = true;
                                if (insertrepa && updatebesoincal)
                                {
                                    ts.Complete();
                                    // btnDejunerImpr.Visibility = Visibility.Visible;
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }

                            if (insertrepa && updatebesoincal)
                            {
                                /***********************************************/////

                                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                                detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                                detailcalcule.DataContext = BesoinCaloriqueCalculé;
                                detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                                ChartRepartionRepas(BesoinCaloriqueCalculé);
                                txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                                txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                                txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;

                            }

                        }
                      


                    }
                    }
                   
                
            }
            catch (Exception ex)
            {

                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnDejunerImpr_Click(object sender, RoutedEventArgs e)
        {
            if (TabPetitDéj.IsEnabled == true)
            {
                if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolPetitDéj == true && TabInterface == 1)
                {
                    ImpressionOneRepas cl = new ImpressionOneRepas(proxy, BesoinCaloriqueCalculé, PetitDejList);
                    cl.Show();
                }
            }
        }

        private void btnImpressionBesoinCalorique_Click(object sender, RoutedEventArgs e)
        {
           
                if (BesoinCaloriqueGrid.SelectedItem != null)
                {
                    SVC.BesoinCalorique selectaliment = BesoinCaloriqueGrid.SelectedItem as SVC.BesoinCalorique;

                ImpressionBesoinCalorique cl = new ImpressionBesoinCalorique(proxy, selectaliment);
                    cl.Show();
                }
            
        }

        private void CollationDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            foreach (var row in CollationList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentCollation.Content = string.Format("{0:0.##}", ((CollationList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void CollationDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var row in CollationList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentCollation.Content = string.Format("{0:0.##}", ((CollationList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void CollationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in CollationList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalCollation.Content = (BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintCollation.Content = CollationList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentCollation.Content = string.Format("{0:0.##}", ((CollationList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.CollationPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void DéjeunerDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var row in DéjeunerList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentDéjeuner.Content = string.Format("{0:0.##}", ((DéjeunerList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void DéjeunerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in DéjeunerList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentDéjeuner.Content = string.Format("{0:0.##}", ((DéjeunerList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void DéjeunerDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            foreach (var row in DéjeunerList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalDéjeuner.Content = (BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintDéjeuner.Content = DéjeunerList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentDéjeuner.Content = string.Format("{0:0.##}", ((DéjeunerList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.DéjeunerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void GouterDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            foreach (var row in GouterList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentGouter.Content = string.Format("{0:0.##}", ((GouterList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void GouterDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in GouterList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentGouter.Content = string.Format("{0:0.##}", ((GouterList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void GouterDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var row in GouterList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalGouter.Content = (BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintGouter.Content = GouterList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentGouter.Content = string.Format("{0:0.##}", ((GouterList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.GouterPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void DinerDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            foreach (var row in DinerList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentDiner.Content = string.Format("{0:0.##}", ((DinerList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void DinerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in DinerList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentDiner.Content = string.Format("{0:0.##}", ((DinerList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void DinerDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var row in DinerList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalDiner.Content = (BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintDiner.Content = DinerList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentDiner.Content = string.Format("{0:0.##}", ((DinerList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.DinerPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void LeSoirDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {

            foreach (var row in LeSoirList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentLeSoir.Content = string.Format("{0:0.##}", ((LeSoirList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void LeSoirDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in LeSoirList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentLeSoir.Content = string.Format("{0:0.##}", ((LeSoirList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void LeSoirDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var row in LeSoirList)
            {
                if (row.Quantite >= 0)
                {
                    row.LipidesNew = row.Quantite * row.Lipides / row.ValeurGramme;
                    row.GlucidesNew = row.Quantite * row.Glucides / row.ValeurGramme;
                    row.ProtéinesNew = row.Quantite * row.Protéines / row.ValeurGramme;
                    row.KcalNew = (row.LipidesNew * 9) + (row.ProtéinesNew * 4) + (row.GlucidesNew * 4);
                }
            }
            labelRepasCalTotalLeSoir.Content = (BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100;
            labelRepasCalAteintLeSoir.Content = LeSoirList.AsEnumerable().Sum(o => (o.KcalNew));
            labelRepasCalAteintPourcentLeSoir.Content = string.Format("{0:0.##}", ((LeSoirList.AsEnumerable().Sum(o => (o.KcalNew)) / ((BesoinCaloriqueCalculé.LesoirPourcentage * BesoinCaloriqueCalculé.DEJ) / 100)) * 100)) + "" + "%";
        }

        private void btnCollationNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabCollation.IsEnabled == true)
                {
                    if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolCollation == false && TabInterface == 2)
                    {
                        /*******************    il faut métre la condition quantité supp a zero pour les somme      ********************/
                        BesoinCaloriqueCalculé.RepasBoolCollation = true;
                        BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        BesoinCaloriqueCalculé.kcalCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        /*************************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                        /*****************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur + CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));
                        /*************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.LipidesPourcentageNew = ((CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        /****************************************************************************************/
                        bool insertrepa = false;
                        bool updatebesoincal = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = CollationDataGrid.ItemsSource as IEnumerable;
                     
                            foreach (SVC.Repa item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertRepa(item);
                                    insertrepa = true;
                                }
                            }


                            proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                            updatebesoincal = true;
                            if (insertrepa && updatebesoincal)
                            {
                                ts.Complete();

                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        if (insertrepa && updatebesoincal)
                        {
                            proxy.AjouterRepaBesoinRefresh(PATIENT.Id);
                            detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                            detailcalcule.DataContext = BesoinCaloriqueCalculé;
                            detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                            ChartRepartionRepas(BesoinCaloriqueCalculé);
                            txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                            txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                            txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                        }
                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolCollation == true && TabInterface == 2)
                        {

                            bool insertrepa = false;
                            bool updatebesoincal = false;
                            /***********************************         .Where(o =>o.Quantite>0)                             *//////////////
                            BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal - BesoinCaloriqueCalculé.kcalCollation + CollationList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + CollationList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + CollationList.AsEnumerable().Sum(o => (o.LipidesNew * 9));
                            BesoinCaloriqueCalculé.kcalCollation = CollationList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + CollationList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + CollationList.AsEnumerable().Sum(o => (o.LipidesNew * 9));




                            /*****************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur - BesoinCaloriqueCalculé.ProtéinesCollation + CollationList.AsEnumerable().Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur - BesoinCaloriqueCalculé.LipidesCollation + CollationList.AsEnumerable().Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur - BesoinCaloriqueCalculé.GlucidesCollation + CollationList.AsEnumerable().Sum(o => (o.GlucidesNew));
                            /************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesCollation = CollationList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                            /*************************************************************************/
                            if (BesoinCaloriqueCalculé.Kcal != 0)
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((CollationList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = ((CollationList.AsEnumerable().Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((CollationList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                            }
                            else
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = 0;
                            }
                            /****************************************************************************************/
                   
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                List<SVC.Repa> itemsSource0 = CollationDataGrid.ItemsSource as List<SVC.Repa>;

                                //    for (int i = itemsSource0.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                //    {
                                List<SVC.Repa> itemsSourcerecept = (proxy.GetAllRepa()).Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas==2).ToList();
                                foreach (SVC.Repa item in itemsSource0)
                                {
                                  
                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);
                                    if (found == null)
                                    {

                                        if (item.Quantite > 0)
                                        {
                                            proxy.InsertRepa(item);
                                            insertrepa = true;
                                        }

                                    }
                                    else
                                    {/*Le cas ou l'aliment existe déja*/
                                        if (found != null)
                                        {
                                            if (item.Quantite > 0)
                                            {
                                                proxy.UpdateRepa(item);
                                                insertrepa = true;
                                            }
                                            else
                                            {
                                                if (item.Quantite == 0)
                                                {
                                                    proxy.DeleteRepa(item);
                                                    insertrepa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                                updatebesoincal = true;
                                if (insertrepa && updatebesoincal)
                                {
                                    ts.Complete();
                                    // btnDejunerImpr.Visibility = Visibility.Visible;
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }
                            if (insertrepa && updatebesoincal)
                            {
                                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);
                                detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                                detailcalcule.DataContext = BesoinCaloriqueCalculé;
                                detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                                ChartRepartionRepas(BesoinCaloriqueCalculé);
                                txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                                txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                                txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                            }


                        }


                    }
                }


            }
            catch (Exception ex)
            {

                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnCollationImpr_Click(object sender, RoutedEventArgs e)
        {
            if (TabCollation.IsEnabled == true)
            {
                if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolCollation == true && TabInterface == 2)
                {
                    ImpressionOneRepas cl = new ImpressionOneRepas(proxy, BesoinCaloriqueCalculé, CollationList);
                    cl.Show();
                }
            }
        }

        private void btnDéjeunerImpr_Click(object sender, RoutedEventArgs e)
        {
            if (TabCollation.IsEnabled == true)
            {
                if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDéjeuner == true && TabInterface == 3)
                {
                    ImpressionOneRepas cl = new ImpressionOneRepas(proxy, BesoinCaloriqueCalculé, DéjeunerList);
                    cl.Show();
                }
            }
        }

        private void btnDéjeunerNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabDéjeuner.IsEnabled == true)
                {
                    if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDéjeuner == false && TabInterface == 3)
                    {
                        bool insertrepa = false;
                        bool updatebesoincal = false;
                        /*******************    il faut métre la condition quantité supp a zero pour les somme      ********************/
                        BesoinCaloriqueCalculé.RepasBoolDéjeuner = true;
                        BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        BesoinCaloriqueCalculé.kcalDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        /*************************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                        /*****************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur + DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));
                        /*************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.LipidesPourcentageNew = ((DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        /****************************************************************************************/
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = DéjeunerDataGrid.ItemsSource as IEnumerable;
                        
                            foreach (SVC.Repa item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertRepa(item);
                                    insertrepa = true;
                                }
                            }


                            proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                            updatebesoincal = true;
                            if (insertrepa && updatebesoincal)
                            {
                                ts.Complete();

                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        if (insertrepa && updatebesoincal)
                        {
                            proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                            detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                            detailcalcule.DataContext = BesoinCaloriqueCalculé;
                            detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                            ChartRepartionRepas(BesoinCaloriqueCalculé);
                            txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                            txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                            txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                        }


                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDéjeuner == true && TabInterface == 3)
                        {

                            bool insertrepa = false;
                            bool updatebesoincal = false;
                            /***********************************         .Where(o =>o.Quantite>0)                             *//////////////
                            BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal - BesoinCaloriqueCalculé.kcalDéjeuner + DéjeunerList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + DéjeunerList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + DéjeunerList.AsEnumerable().Sum(o => (o.LipidesNew * 9));
                            BesoinCaloriqueCalculé.kcalDéjeuner = DéjeunerList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + DéjeunerList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + DéjeunerList.AsEnumerable().Sum(o => (o.LipidesNew * 9));




                            /*****************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur - BesoinCaloriqueCalculé.ProtéinesDéjeuner + DéjeunerList.AsEnumerable().Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur - BesoinCaloriqueCalculé.LipidesDéjeuner + DéjeunerList.AsEnumerable().Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur - BesoinCaloriqueCalculé.GlucidesDéjeuner + DéjeunerList.AsEnumerable().Sum(o => (o.GlucidesNew));
                            /************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesDéjeuner = DéjeunerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                            /*************************************************************************/
                            if (BesoinCaloriqueCalculé.Kcal != 0)
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((DéjeunerList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = ((DéjeunerList.AsEnumerable().Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((DéjeunerList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                            }
                            else
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = 0;
                            }
                            /****************************************************************************************/
                           
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                List<SVC.Repa> itemsSource0 = DéjeunerDataGrid.ItemsSource as List<SVC.Repa>;

                                //    for (int i = itemsSource0.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                //    {
                                List<SVC.Repa> itemsSourcerecept = (proxy.GetAllRepa()).Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 3).ToList();
                                foreach (SVC.Repa item in itemsSource0)
                                {

                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);
                                    if (found == null)
                                    {

                                        if (item.Quantite > 0)
                                        {
                                            proxy.InsertRepa(item);
                                            insertrepa = true;
                                        }

                                    }
                                    else
                                    {/*Le cas ou l'aliment existe déja*/
                                        if (found != null)
                                        {
                                            if (item.Quantite > 0)
                                            {
                                                proxy.UpdateRepa(item);
                                                insertrepa = true;
                                            }
                                            else
                                            {
                                                if (item.Quantite == 0)
                                                {
                                                    proxy.DeleteRepa(item);
                                                    insertrepa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                                updatebesoincal = true;
                                if (insertrepa && updatebesoincal)
                                {
                                    ts.Complete();
                                    // btnDejunerImpr.Visibility = Visibility.Visible;
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }

                            if (insertrepa && updatebesoincal)
                            {
                                /***********************************************/////

                                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                                detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                                detailcalcule.DataContext = BesoinCaloriqueCalculé;
                                detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                                ChartRepartionRepas(BesoinCaloriqueCalculé);
                                txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                                txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                                txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;

                            }

                        }
                        


                    }
                }


            }
            catch (Exception ex)
            {

                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnGouterNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabGouter.IsEnabled == true)
                {
                    if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolGouter == false && TabInterface == 4)
                    {
                        bool insertrepa = false;
                        bool updatebesoincal = false;
                        /*******************    il faut métre la condition quantité supp a zero pour les somme      ********************/
                        BesoinCaloriqueCalculé.RepasBoolGouter = true;
                        BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        BesoinCaloriqueCalculé.kcalGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        /*************************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                        /*****************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur + GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));
                        /*************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.LipidesPourcentageNew = ((GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        /****************************************************************************************/
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = GouterDataGrid.ItemsSource as IEnumerable;
                       
                            foreach (SVC.Repa item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertRepa(item);
                                    insertrepa = true;
                                }
                            }


                            proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                            updatebesoincal = true;
                            if (insertrepa && updatebesoincal)
                            {
                                ts.Complete();

                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        if (insertrepa && updatebesoincal)
                        {
                            proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                            detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                            detailcalcule.DataContext = BesoinCaloriqueCalculé;
                            detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                            ChartRepartionRepas(BesoinCaloriqueCalculé);
                            txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                            txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                            txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                        }

                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolGouter == true && TabInterface == 4)
                        {


                            /***********************************         .Where(o =>o.Quantite>0)                             *//////////////
                            BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal - BesoinCaloriqueCalculé.kcalGouter + GouterList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + GouterList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + GouterList.AsEnumerable().Sum(o => (o.LipidesNew * 9));
                            BesoinCaloriqueCalculé.kcalGouter = GouterList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + GouterList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + GouterList.AsEnumerable().Sum(o => (o.LipidesNew * 9));




                            /*****************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur - BesoinCaloriqueCalculé.ProtéinesGouter + GouterList.AsEnumerable().Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur - BesoinCaloriqueCalculé.LipidesGouter + GouterList.AsEnumerable().Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur - BesoinCaloriqueCalculé.GlucidesGouter + GouterList.AsEnumerable().Sum(o => (o.GlucidesNew));
                            /************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesGouter = GouterList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                            /*************************************************************************/
                            if (BesoinCaloriqueCalculé.Kcal != 0)
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((GouterList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = ((GouterList.AsEnumerable().Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((GouterList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                            }
                            else
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = 0;
                            }
                            /****************************************************************************************/
                            bool insertrepa = false;
                            bool updatebesoincal = false;
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                List<SVC.Repa> itemsSource0 = GouterDataGrid.ItemsSource as List<SVC.Repa>;

                                //    for (int i = itemsSource0.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                //    {
                                List<SVC.Repa> itemsSourcerecept = (proxy.GetAllRepa()).Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 4).ToList();
                                foreach (SVC.Repa item in itemsSource0)
                                {

                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);
                                    if (found == null)
                                    {

                                        if (item.Quantite > 0)
                                        {
                                            proxy.InsertRepa(item);
                                            insertrepa = true;
                                        }

                                    }
                                    else
                                    {/*Le cas ou l'aliment existe déja*/
                                        if (found != null)
                                        {
                                            if (item.Quantite > 0)
                                            {
                                                proxy.UpdateRepa(item);
                                                insertrepa = true;
                                            }
                                            else
                                            {
                                                if (item.Quantite == 0)
                                                {
                                                    proxy.DeleteRepa(item);
                                                    insertrepa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                                updatebesoincal = true;
                                if (insertrepa && updatebesoincal)
                                {
                                    ts.Complete();
                                    // btnDejunerImpr.Visibility = Visibility.Visible;
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }

                            if (insertrepa && updatebesoincal)
                            {
                                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                                detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                                detailcalcule.DataContext = BesoinCaloriqueCalculé;
                                detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                                ChartRepartionRepas(BesoinCaloriqueCalculé);
                                txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                                txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                                txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                            }

                        }


                    }
                }


            } 
            catch (Exception ex)
            {

                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnGouterImpr_Click(object sender, RoutedEventArgs e)
        {
            if (TabGouter.IsEnabled == true)
            {
                if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolGouter == true && TabInterface == 4)
                {
                    ImpressionOneRepas cl = new ImpressionOneRepas(proxy, BesoinCaloriqueCalculé, GouterList);
                    cl.Show();
                }
            }
        }

        private void btnDinerNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabDiner.IsEnabled == true)
                {
                    if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDiner == false && TabInterface == 5)
                    {
                        bool insertrepa = false;
                        bool updatebesoincal = false;
                        /*******************    il faut métre la condition quantité supp a zero pour les somme      ********************/
                        BesoinCaloriqueCalculé.RepasBoolDiner = true;
                        BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        BesoinCaloriqueCalculé.kcalDiner = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        /*************************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesGouter = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesGouter = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesGouter = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                        /*****************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur + DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));
                        /*************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.LipidesPourcentageNew = ((DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        /****************************************************************************************/
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = DinerDataGrid.ItemsSource as IEnumerable;
                           
                            foreach (SVC.Repa item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertRepa(item);
                                    insertrepa = true;
                                }
                            }


                            proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                            updatebesoincal = true;
                            if (insertrepa && updatebesoincal)
                            {
                                ts.Complete();

                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        if (insertrepa && updatebesoincal)
                        {
                            proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                            detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                            detailcalcule.DataContext = BesoinCaloriqueCalculé;
                            detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                            ChartRepartionRepas(BesoinCaloriqueCalculé);
                            txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                            txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                            txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                        }


                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDiner == true && TabInterface == 5)
                        {


                            /***********************************         .Where(o =>o.Quantite>0)                             *//////////////
                            BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal - BesoinCaloriqueCalculé.kcalDiner + DinerList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + DinerList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + DinerList.AsEnumerable().Sum(o => (o.LipidesNew * 9));
                            BesoinCaloriqueCalculé.kcalDiner = DinerList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + DinerList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + DinerList.AsEnumerable().Sum(o => (o.LipidesNew * 9));




                            /*****************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur - BesoinCaloriqueCalculé.ProtéinesDiner + DinerList.AsEnumerable().Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur - BesoinCaloriqueCalculé.LipidesDiner + DinerList.AsEnumerable().Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur - BesoinCaloriqueCalculé.GlucidesDiner + DinerList.AsEnumerable().Sum(o => (o.GlucidesNew));
                            /************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesDiner = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesDiner = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesDiner = DinerList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                            /*************************************************************************/
                            if (BesoinCaloriqueCalculé.Kcal != 0)
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((DinerList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = ((DinerList.AsEnumerable().Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((DinerList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                            }
                            else
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = 0;
                            }
                            /****************************************************************************************/
                            bool insertrepa = false;
                            bool updatebesoincal = false;
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                List<SVC.Repa> itemsSource0 = DinerDataGrid.ItemsSource as List<SVC.Repa>;

                                //    for (int i = itemsSource0.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                //    {
                                List<SVC.Repa> itemsSourcerecept = (proxy.GetAllRepa()).Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 5).ToList();
                                foreach (SVC.Repa item in itemsSource0)
                                {

                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);
                                    if (found == null)
                                    {

                                        if (item.Quantite > 0)
                                        {
                                            proxy.InsertRepa(item);
                                            insertrepa = true;
                                        }

                                    }
                                    else
                                    {/*Le cas ou l'aliment existe déja*/
                                        if (found != null)
                                        {
                                            if (item.Quantite > 0)
                                            {
                                                proxy.UpdateRepa(item);
                                                insertrepa = true;
                                            }
                                            else
                                            {
                                                if (item.Quantite == 0)
                                                {
                                                    proxy.DeleteRepa(item);
                                                    insertrepa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                                updatebesoincal = true;
                                if (insertrepa && updatebesoincal)
                                {
                                    ts.Complete();
                                    // btnDejunerImpr.Visibility = Visibility.Visible;
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }

                            if (insertrepa && updatebesoincal)
                            {
                                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                                detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                                detailcalcule.DataContext = BesoinCaloriqueCalculé;
                                detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                                ChartRepartionRepas(BesoinCaloriqueCalculé);
                                txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                                txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                                txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                            }

                        }
                       


                    }
                }


            }
            catch (Exception ex)
            {

                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnDinerImpr_Click(object sender, RoutedEventArgs e)
        {
            if (TabDiner.IsEnabled == true)
            {
                if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolDiner == true && TabInterface == 5)
                {
                    ImpressionOneRepas cl = new ImpressionOneRepas(proxy, BesoinCaloriqueCalculé, DinerList);
                    cl.Show();
                }
            }
        }

        private void btnLeSoirNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LeSoirGrid.IsEnabled == true)
                {
                    if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolLesoir == false && TabInterface == 6)
                    {
                        bool insertrepa = false;
                        bool updatebesoincal = false;
                        /*******************    il faut métre la condition quantité supp a zero pour les somme      ********************/
                        BesoinCaloriqueCalculé.RepasBoolLesoir = true;
                        BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        BesoinCaloriqueCalculé.kcalLesoir = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9));
                        /*************************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesGouter = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesGouter = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesGouter = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                        /*****************************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                        BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                        BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur + LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));
                        /*************************************************************************/
                        BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.LipidesPourcentageNew = ((LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                        /****************************************************************************************/
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = LeSoirDataGrid.ItemsSource as IEnumerable;
                        
                            foreach (SVC.Repa item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertRepa(item);
                                    insertrepa = true;
                                }
                            }


                            proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                            updatebesoincal = true;
                            if (insertrepa && updatebesoincal)
                            {
                                ts.Complete();

                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        if (insertrepa && updatebesoincal)
                        {
                            proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                            detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                            detailcalcule.DataContext = BesoinCaloriqueCalculé;
                            detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                            ChartRepartionRepas(BesoinCaloriqueCalculé);
                            txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                            txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                            txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                        }


                    }
                    else
                    {
                        if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolLesoir == true && TabInterface == 6)
                        {


                            /***********************************         .Where(o =>o.Quantite>0)                             *//////////////
                            BesoinCaloriqueCalculé.Kcal = BesoinCaloriqueCalculé.Kcal - BesoinCaloriqueCalculé.kcalLesoir + LeSoirList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + LeSoirList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + LeSoirList.AsEnumerable().Sum(o => (o.LipidesNew * 9));
                            BesoinCaloriqueCalculé.kcalLesoir = LeSoirList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) + LeSoirList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) + LeSoirList.AsEnumerable().Sum(o => (o.LipidesNew * 9));




                            /*****************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesNewValeur = BesoinCaloriqueCalculé.ProtéinesNewValeur - BesoinCaloriqueCalculé.ProtéinesLesoir + LeSoirList.AsEnumerable().Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesNewValeur = BesoinCaloriqueCalculé.LipidesNewValeur - BesoinCaloriqueCalculé.LipidesLesoir + LeSoirList.AsEnumerable().Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesNewValeur = BesoinCaloriqueCalculé.GlucidesNewValeur - BesoinCaloriqueCalculé.GlucidesLesoir + LeSoirList.AsEnumerable().Sum(o => (o.GlucidesNew));
                            /************************************************************************************/
                            BesoinCaloriqueCalculé.ProtéinesLesoir = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.ProtéinesNew));
                            BesoinCaloriqueCalculé.LipidesLesoir = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.LipidesNew));
                            BesoinCaloriqueCalculé.GlucidesLesoir = LeSoirList.AsEnumerable().Where(o => o.Quantite > 0).Sum(o => (o.GlucidesNew));

                            /*************************************************************************/
                            if (BesoinCaloriqueCalculé.Kcal != 0)
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = ((LeSoirList.AsEnumerable().Sum(o => (o.ProtéinesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = ((LeSoirList.AsEnumerable().Sum(o => (o.LipidesNew * 9)) / BesoinCaloriqueCalculé.Kcal) * 100);
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = ((LeSoirList.AsEnumerable().Sum(o => (o.GlucidesNew * 4)) / BesoinCaloriqueCalculé.Kcal) * 100);
                            }
                            else
                            {
                                BesoinCaloriqueCalculé.ProtéinesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.LipidesPourcentageNew = 0;
                                BesoinCaloriqueCalculé.GlucidesPourcentageNew = 0;
                            }
                            /****************************************************************************************/
                            bool insertrepa = false;
                            bool updatebesoincal = false;
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                List<SVC.Repa> itemsSource0 = LeSoirDataGrid.ItemsSource as List<SVC.Repa>;

                                //    for (int i = itemsSource0.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                //    {
                                List<SVC.Repa> itemsSourcerecept = (proxy.GetAllRepa()).Where(n => n.Clé == BesoinCaloriqueCalculé.Clé && n.InterfaceRepas == 6).ToList();
                                foreach (SVC.Repa item in itemsSource0)
                                {

                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);
                                    if (found == null)
                                    {

                                        if (item.Quantite > 0)
                                        {
                                            proxy.InsertRepa(item);
                                            insertrepa = true;
                                        }

                                    }
                                    else
                                    {/*Le cas ou l'aliment existe déja*/
                                        if (found != null)
                                        {
                                            if (item.Quantite > 0)
                                            {
                                                proxy.UpdateRepa(item);
                                                insertrepa = true;
                                            }
                                            else
                                            {
                                                if (item.Quantite == 0)
                                                {
                                                    proxy.DeleteRepa(item);
                                                    insertrepa = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                proxy.UpdateBesoinCalorique(BesoinCaloriqueCalculé);
                                updatebesoincal = true;
                                if (insertrepa && updatebesoincal)
                                {
                                    ts.Complete();
                                    // btnDejunerImpr.Visibility = Visibility.Visible;
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Opération términée", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur Opération", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }


                            if (insertrepa && updatebesoincal)
                            {
                                proxy.AjouterRepaBesoinRefresh(PATIENT.Id);

                                detailcalculeRepas.DataContext = BesoinCaloriqueCalculé;
                                detailcalcule.DataContext = BesoinCaloriqueCalculé;
                                detailcalculeGlobal.DataContext = BesoinCaloriqueCalculé;
                                ChartRepartionRepas(BesoinCaloriqueCalculé);
                                txtlabeltotalProtéinesAteinte.Content = BesoinCaloriqueCalculé.ProtéinesNewValeur;
                                txtlabeltotalLipidesAteinte.Content = BesoinCaloriqueCalculé.LipidesNewValeur;
                                txtlabeltotalGlucidesAteinte.Content = BesoinCaloriqueCalculé.GlucidesNewValeur;
                            }
                        }
                       


                    }
                }


            }
            catch (Exception ex)
            {

                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnLesoirImpr_Click(object sender, RoutedEventArgs e)
        {
            if (TabLeSoir.IsEnabled == true)
            {
                if (BesoinCaloriqueCalculé != null && BesoinCaloriqueCalculé.RepasBoolLesoir == true && TabInterface == 6)
                {
                    ImpressionOneRepas cl = new ImpressionOneRepas(proxy, BesoinCaloriqueCalculé, LeSoirList);
                    cl.Show();
                }
            }
        }

        private void txtPoid_SelectionChanged(object sender, RoutedEventArgs e)
        { try
            {
                if (txtTaille.Text != "" && txtPoid.Text != "")
                {
                    if (txtTaille.Text != "0" && txtPoid.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        txtIMC.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text)));
                    }else
                    {
                        txtIMC.Text = "0";
                    }
                }
                else
                {
                    txtIMC.Text = "0";
                }
            }catch (Exception ex)
            {
                this.Title = ex.Message;
            }

         }

        private void txtTaille_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTaille.Text != "" && txtPoid.Text != "")
            {
                    if (txtTaille.Text != "0" && txtPoid.Text != "0")
                    {
                        txtIMC.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text)));
                    }
                    else
                    {
                        txtIMC.Text = "0";
                    }
                }
            else
            {
                txtIMC.Text = "0";
            }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void txtPoid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtTaille_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtPAS_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtPAD_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtPouls_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtTemp_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtGlycémie_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtCholést_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void TxtHonoraire_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void ComboFact_DropDownClosed(object sender, EventArgs e)
        {
            if (ComboFact.SelectedItem!=null)
            {
                if (ComboFact.SelectionBoxItem.ToString()== "forfait")
                {
                    TxtHonoraireCasTraiter.Visibility = Visibility.Visible;
                    labelhonoraire.Visibility = Visibility.Visible;
                }else
                {
                    if (ComboFact.SelectionBoxItem.ToString() == "actes")
                    {
                        TxtHonoraireCasTraiter.Visibility = Visibility.Visible;
                        TxtHonoraireCasTraiter.IsEnabled = false;

                    }
                    else
                    {
                        TxtHonoraireCasTraiter.Visibility = Visibility.Collapsed;
                        labelhonoraire.Visibility = Visibility.Collapsed;
                        TxtHonoraire.Text = "0";
                    }
                }
            }
        }

        private void TxtHonoraireCasTraiter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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


        private void SIMPLEDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SIMPLEDataGrid.Items.Count >= 1)
                {
                    SIMPLEDataGrid.SelectedItem = null;
                    txtMotifVisite.SelectedItem = -1;
                    //TypeDeVisite.SelectedItem = null;
                    //TypeDeActes.SelectedIndex = -1;
                    txtInterrogatoire.Text = "";
                    txtExamen.Text = "";
                    txtConclusions.Text = "";
                    TxtHonoraire.Text = "";
                    txtMotifVisite.IsEnabled = false;
                    txtblockmotifvisite.Visibility = Visibility.Collapsed;
                    TypeDeVisite.Visibility = Visibility.Collapsed;
                 
                    chactetraiter.Visibility = Visibility.Collapsed;
                    chactetraiter.IsChecked = false;

                    LabelForfait.Visibility = Visibility.Collapsed;
                    TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                    txtMotifVisite.ItemsSource = proxy.GetAllTraitement().Where(n => n.CodePatient == PATIENT.Id && n.Devis == false /*&& n.ModeFacturationint!=0*/).ToList();
                    Grid.SetColumnSpan(txtConclusions, 3);
                    Grid.SetColumnSpan(txtExamen, 2);
                    bntcreerSimpleVisite.ToolTip = "Créer";
                    modifvisite = false;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void CasDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void CasDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CasDataGrid.Items.Count >= 1)
            {
                CasDataGrid.SelectedItem = null;
                txtCas1.Text = "";
                txtCas1.IsEnabled = false;
                txtDepuis.SelectedDate = null;
                txtDepuis.IsEnabled = false;
                txtHistoire.Text = "";
                txtHistoire.IsEnabled = false;
                txtRemarqueCas.Text = "";
                txtRemarqueCas.IsEnabled = false;
                ComboFact.SelectedIndex = -1;
                ComboEtat.SelectedIndex = -1;
                ComboFact.IsEnabled = false;
                TxtHonoraireCasTraiter.Text = "";
                TxtHonoraireCasTraiter.Visibility = Visibility.Collapsed;
                TxtHonoraireCasTraiter.IsEnabled = false;
                btnCréerCas.ToolTip = "Créer";
            }
        }

        private void DatagridContstante_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DatagridContstante.Items.Count >= 1)
            {
                DatagridContstante.SelectedItem = null;
                DatePrise.SelectedDate = null;
                txtPoid.Text = "";
                txtTaille.Text = "";
                txtPAS.Text = "";
                txtPAD.Text = "";
                txtPouls.Text = "";
                txtTemp.Text = "";
                txtGlycémieAjeun.Text = "";
                txtGlycémiePostpran.Text = "";
                txtTime.Text = "";
                txtIMC.Text = "";
                txtHDLCHOLEST.Text = "";
                txtHDLcholestérol.Text = "";
                txttriglycérides.Text = "";
                txttHBA1C.Text = "";
                txtMicroalbuminurie.Text = "";
                txtUréesanguine.Text = "";
                txtUréeurinaire.Text = "";
                txtcréatininesanguin.Text = "";
                txtccréatinineurinaire.Text = "";
                constanteCréer = null;
                ConstaneDétailGrid.DataContext = null;
                CréerConstante.Content = "Créer";
                ConstaneDétailGrid.IsEnabled = false;
                autreexamentab.IsEnabled = false;
                autrebilanlist = null;
                GridDataGrid.DataContext = null;
                GridDataGrid.ItemsSource = null;
            }
        }

        private void txtMotifVisite_DropDownClosed(object sender, EventArgs e)
        {
            if (txtMotifVisite.SelectedItem!=null)
            {
                SVC.CasTraitement selectedCasTraitement = txtMotifVisite.SelectedItem as SVC.CasTraitement;
                if (selectedCasTraitement.ModeFacturationint == 2)
                {
                    LabelForfait.Visibility = Visibility.Visible;
                 //   LabelForfait.Text = " forfait à " + selectedCasTraitement.Montant + ", reste non facturé " + selectedCasTraitement.Reste + " Montant reste a payer :"+ ((proxy.GetAllVisite()).Where(n => n.CodePatient == PATIENT.Id && n.IdCas== selectedCasTraitement.Id).AsEnumerable().Sum(o => o.Reste)).ToString(); ;
                    LabelForfait.Text = " forfait à " + selectedCasTraitement.Montant + ", reste non facturé " + selectedCasTraitement.Reste + " Montant reste a payer :" + ((proxy.GetAllVisiteByVisite(PATIENT.Id)).Where(n => n.IdCas == selectedCasTraitement.Id).AsEnumerable().Sum(o => o.Reste)).ToString(); ;

                    TypeDeVisite.Visibility = Visibility.Visible;
                    txtblockmotifvisite.Visibility = Visibility.Visible;
                    chactetraiter.Visibility = Visibility.Collapsed;
                    TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                    //  TypeDeActes.ItemsSource = proxy.GetAllMotifVisite().Where(n => n.Cle == selectedCasTraitement.cle && n.Traité == false);
                  //  TypeDeActes.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                }
                else
                {
                    if (selectedCasTraitement.ModeFacturationint == 1)
                    {
                        LabelForfait.Visibility = Visibility.Visible;
                     //   LabelForfait.Text = "(totaux des actes s'élevent à " + selectedCasTraitement.Montant + " ,reste non facturé " + selectedCasTraitement.Reste + " Montant reste a payer :" + ((proxy.GetAllVisite()).Where(n => n.CodePatient == PATIENT.Id && n.IdCas == selectedCasTraitement.Id).AsEnumerable().Sum(o => o.Reste)).ToString(); ;
                        LabelForfait.Text = "(totaux des actes s'élevent à " + selectedCasTraitement.Montant + " ,reste non facturé " + selectedCasTraitement.Reste + " Montant reste a payer :" + ((proxy.GetAllVisiteByVisite(PATIENT.Id)).Where(n => n.CodePatient == PATIENT.Id && n.IdCas == selectedCasTraitement.Id).AsEnumerable().Sum(o => o.Reste)).ToString(); ;

                        TypeDeVisite.Visibility = Visibility.Collapsed;
                        txtblockmotifvisite.Visibility = Visibility.Collapsed;
                        chactetraiter.Visibility = Visibility.Visible;
                        TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (selectedCasTraitement.ModeFacturationint == 0 && selectedCasTraitement.Cas== "Consultation simple")
                        {
                            TypeDeVisite.Visibility = Visibility.Visible;
                            txtblockmotifvisite.Visibility = Visibility.Visible;
                            LabelForfait.Visibility = Visibility.Collapsed;
                            chactetraiter.Visibility = Visibility.Collapsed;
                            TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                        } else
                        {
                            if (selectedCasTraitement.ModeFacturationint == 0 && selectedCasTraitement.Cas == "Traitement")
                            {
                                TypeDeVisite.Visibility = Visibility.Collapsed;
                                txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                LabelForfait.Visibility = Visibility.Collapsed;
                                chactetraiter.Visibility = Visibility.Collapsed;
                                TypeDeActesTraitement.Visibility = Visibility.Visible;
                            }
                        }
                    }

                }

            }
        }

        private void MedicamentsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MedicamentsDataGrid.SelectedItem != null && OrdonnanceOuvert==true)
            {
                SVC.ProduitOrdonnance selectaliment = MedicamentsDataGrid.SelectedItem as SVC.ProduitOrdonnance;
                SVC.OrdonnancePatient DD = new SVC.OrdonnancePatient
                {
                   CodePatient=PATIENT.Id,
                   NomPatient=PATIENT.Nom,
                   PrénomPatient=PATIENT.Prénom,
                   Date= EnteteOrdonnanceSelect.Date,
                   CodeMedecin= EnteteOrdonnanceSelect.IdMedecin,
                   UserName=memberuser.UserName,
                   CodeCnas=selectaliment.CodeCnas,
                   Design=selectaliment.Design,
                   Quantite=0,
                   dci=selectaliment.dci,
                   Dosage=selectaliment.Dosage,
                   IdDci=selectaliment.IdDci,
                   Remarques= "La posologie c'est l'étude des doses auxquelles doivent être administrés les médicaments pour donner un effet thérapeutique donné.",
                   CodeOrdonnance =EnteteOrdonnanceSelect.Id,
                   CodeProduit=selectaliment.Id,
                   Colisage=selectaliment.Colisage,
                   UnitéDeMesure=selectaliment.UnitéDeMesure,
                };
                var found = OrdonnancePatientList.Find(item => item.CodeProduit == selectaliment.Id);
                if (found == null)
                {
                    OrdonnancePatientList.Add(DD);

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("ce Médicament appartient déja a l'ordonnance", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }


                PetitOrdonnanceGridDataGrid.ItemsSource = OrdonnancePatientList;
                CollectionViewSource.GetDefaultView(PetitOrdonnanceGridDataGrid.ItemsSource).Refresh();

               }
        }

        private void btnNewOrdonnance_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.CréationDossierPatient == true && txtDateOrdonnace.SelectedDate!=null)
            {
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                  if (MedecinConsult != null)
               {
                        SVC.EnteteOrdonnace neword = new SVC.EnteteOrdonnace
                    {
                        CodePatient = PATIENT.Id,
                        Date= txtDateOrdonnace.SelectedDate,
                        NomPatient=PATIENT.Nom,
                        PrénomPatient=PATIENT.Prénom,
                        UserName=memberuser.UserName,
                        créer=false,
                       
                    };
                        proxy.InsertEnteteOrdonnace(neword);
                        ts.Complete();

                  }else
                 {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("cette opération requière une session de medecin ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

               }


                }
                proxy.AjouterEnteteOrdonnaceOrdonnancePatientRefresh();
            }else
            {
                MessageBoxResult resul03E = Xceed.Wpf.Toolkit.MessageBox.Show("Nous sommes dans le else ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnSuppBesoinOrdonnance_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (memberuser.SuppressionDossierPatient == true && OrdonnanceDatagGrid.SelectedItem != null)
                {
                    SVC.EnteteOrdonnace selectaliment = OrdonnanceDatagGrid.SelectedItem as SVC.EnteteOrdonnace;
                    var found = (proxy.GetAllOrdonnancePatient()).Find(itemf => itemf.CodeOrdonnance == selectaliment.Id);
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        bool deleteEnteteOrdonnance = false;
                        bool deletePatientOrdonnance = false;
                        int interfacesuppordo = 0;
                        proxy.DeleteEnteteOrdonnace(selectaliment);
                        deleteEnteteOrdonnance = true;



                        if (found != null)
                        {
                            var s = proxy.GetAllOrdonnancePatient().Where(n => n.CodeOrdonnance == selectaliment.Id).ToList();
                            interfacesuppordo = 1;
                            for (int i = s.Count - 1; i >= 0; i--)

                            {
                                deletePatientOrdonnance = false;
                                var item = s.ElementAt(i);
                                proxy.DeleteOrdonnancePatient(item);
                                deletePatientOrdonnance = true;

                            }

                        }

                        if (interfacesuppordo == 0 && deleteEnteteOrdonnance == true)
                        {
                            ts.Complete();
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Suppression términé", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                        else
                        {
                            if (interfacesuppordo == 1 && deleteEnteteOrdonnance == true && deletePatientOrdonnance == true)
                            {
                                ts.Complete();
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Suppression términé", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Erreur", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                    }
                    proxy.AjouterEnteteOrdonnaceOrdonnancePatientRefresh();



                }
            }catch (Exception ex)
            {

                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

      

        private void txtRechercheMédicaments_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(MedicamentsDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.ProduitOrdonnance p = o as SVC.ProduitOrdonnance;
                        if (t.Name == "txtid")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Design.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void cbDci_DropDownClosed(object sender, EventArgs e)
        {
            if (cbDci.SelectedItem != null)

            {
                SVC.Dci selectaliment = cbDci.SelectedItem as SVC.Dci;

                string filter = selectaliment.Dci1;


                ICollectionView cv = CollectionViewSource.GetDefaultView(MedicamentsDataGrid.ItemsSource);

                cv.Filter = o =>
                {
                    SVC.ProduitOrdonnance p = o as SVC.ProduitOrdonnance;
                    return (p.dci.ToUpper().Contains(filter.ToUpper()));
                };


            }
        }

        private void cbDci_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cbDci.ItemsSource = proxy.GetAllDci().OrderBy(r => r.Dci1);
            MedicamentsDataGrid.ItemsSource = proxy.GetAllProduitOrdonnance();
        }

        private void BtnOrdonnacedétail_Click(object sender, RoutedEventArgs e)
        {
            //if (MedecinConsult != null)
            //{
                if (OrdonnanceDatagGrid.SelectedItem != null)
                {
                    SVC.EnteteOrdonnace selectordonnance = OrdonnanceDatagGrid.SelectedItem as SVC.EnteteOrdonnace;
                    if (selectordonnance.créer == true && memberuser.CréationDossierPatient == true)
                    {
                        EnteteOrdonnanceSelect = selectordonnance;
                        OrdonnanceOuvert = true;

                        gridPub.Visibility = Visibility.Collapsed;
                        DétailOrdonnanceGrid.Visibility = Visibility.Visible;
                        OrdonnancePatientList = proxy.GetAllOrdonnancePatient().Where(n => n.CodeOrdonnance == EnteteOrdonnanceSelect.Id).ToList();
                        PetitOrdonnanceGridDataGrid.ItemsSource = OrdonnancePatientList;
                        PetitOrdonnanceGridDataGrid.DataContext = OrdonnancePatientList;
                        PetitOrdonnanceGridDataGrid.Visibility = Visibility.Visible;
                        btnVALIDERORDO.Visibility = Visibility.Visible;
                        btnImprimerOrdonnance.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        OrdonnancePatientList = new List<SVC.OrdonnancePatient>();
                        EnteteOrdonnanceSelect = selectordonnance;
                        OrdonnanceOuvert = true;
                        PetitOrdonnanceGridDataGrid.ItemsSource = OrdonnancePatientList;
                        PetitOrdonnanceGridDataGrid.DataContext = OrdonnancePatientList;
                        PetitOrdonnanceGridDataGrid.Visibility = Visibility.Visible;
                        gridPub.Visibility = Visibility.Collapsed;
                        DétailOrdonnanceGrid.Visibility = Visibility.Visible;
                        btnVALIDERORDO.Visibility = Visibility.Visible;
                        btnImprimerOrdonnance.Visibility = Visibility.Visible;
                    }
                }
          //  }
        }

        private void OrdonnanceDatagGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        /*    if (OrdonnanceDatagGrid.SelectedItem != null)
            {
                SVC.EnteteOrdonnace selectordonnance = OrdonnanceDatagGrid.SelectedItem as SVC.EnteteOrdonnace;
                if (selectordonnance.créer == false && memberuser.CréationDossierPatient == true)
                {
                    EnteteOrdonnanceSelect = selectordonnance;
                    OrdonnanceOuvert = true;

                    gridPub.Visibility = Visibility.Collapsed;
                    DétailOrdonnanceGrid.Visibility = Visibility.Visible;
                    OrdonnancePatientList = proxy.GetAllOrdonnancePatient().Where(n => n.CodeOrdonnance == EnteteOrdonnanceSelect.Id).ToList();
                    PetitOrdonnanceGridDataGrid.ItemsSource = OrdonnancePatientList;
                    PetitOrdonnanceGridDataGrid.DataContext = OrdonnancePatientList;
                    PetitOrdonnanceGridDataGrid.Visibility = Visibility.Visible;
                    btnVALIDERORDO.Visibility = Visibility.Visible;
                    btnImprimerOrdonnance.Visibility = Visibility.Visible;
                }
                else
                {
                    OrdonnancePatientList = new List<SVC.OrdonnancePatient>();
                    EnteteOrdonnanceSelect = selectordonnance;
                    OrdonnanceOuvert = true;
                    PetitOrdonnanceGridDataGrid.ItemsSource = OrdonnancePatientList;
                    PetitOrdonnanceGridDataGrid.DataContext = OrdonnancePatientList;
                    PetitOrdonnanceGridDataGrid.Visibility = Visibility.Visible;
                    gridPub.Visibility = Visibility.Collapsed;
                    DétailOrdonnanceGrid.Visibility = Visibility.Visible;
                    btnVALIDERORDO.Visibility = Visibility.Visible;
                    btnImprimerOrdonnance.Visibility = Visibility.Visible;
                }
            }*/
        }

        private void btnVALIDERORDO_Click(object sender, RoutedEventArgs e)
        {
            try
            {   if (EnteteOrdonnanceSelect!=null)
                {
                    if (EnteteOrdonnanceSelect.créer==false && OrdonnancePatientList.Count()> 0 && MedecinConsult!=null)
                    {/****************************************************************************/
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            var itemsSource0 = PetitOrdonnanceGridDataGrid.ItemsSource as IEnumerable;
                            bool insertDétailOrdonnace = false;
                            bool updateEnteteOrdonnace = false;
                            foreach (SVC.OrdonnancePatient item in itemsSource0)
                            {
                                if (item.Quantite > 0)
                                {
                                    proxy.InsertOrdonnancePatient(item);
                                    insertDétailOrdonnace = true;
                                }
                            }

                            EnteteOrdonnanceSelect.créer = true;
                            proxy.UpdateEnteteOrdonnace(EnteteOrdonnanceSelect);
                            updateEnteteOrdonnace = true;
                            if (insertDétailOrdonnace && updateEnteteOrdonnace)
                            {
                                ts.Complete();

                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        proxy.AjouterEnteteOrdonnaceOrdonnancePatientRefresh();










                        /*************************************************************************/
                    } else
                    {

                    }





                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }


        }

        private void btnImprimerOrdonnance_Click(object sender, RoutedEventArgs e)
        {
            if (chimprimerOrdo.IsChecked == true)
            {
                if (OrdonnancePatientList.Count() > 0 && EnteteOrdonnanceSelect != null && memberuser.ImpressionDossierPatient == true )
                {
                    ImpressionOrdonnance cm = new ImpressionOrdonnance(proxy, OrdonnancePatientList, PATIENT, MedecinConsult, EnteteOrdonnanceSelect,Convert.ToBoolean(chimprimerA4.IsChecked));
                    cm.Show();
                }
                else
                {
                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                if (chimprimerOrdo.IsChecked == false)
                {
                    if (OrdonnancePatientList.Count() > 0 && EnteteOrdonnanceSelect != null && memberuser.ImpressionDossierPatient == true)
                    {
                        Run(PATIENT, MedecinConsult, OrdonnancePatientList, EnteteOrdonnanceSelect,Convert.ToBoolean(chimprimerA4.IsChecked));
                    }
                    else
                    {
                        MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
        }
        private void Run(SVC.Patient PatientRecu, SVC.Medecin SelectMedecinListe, List<SVC.OrdonnancePatient> OrdonnancePatientRecu, SVC.EnteteOrdonnace selectedEnteteOrdonnance,bool a4)
        {
            var Entetemededcin = new List<SVC.EnteteOrdonnace>();
            Entetemededcin.Add(selectedEnteteOrdonnance);

            var people = new List<SVC.Patient>();
            people.Add(PatientRecu);
            LocalReport reportViewer1 = new LocalReport();

            if (a4==false)
            {
                MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.Ordonnance), false);
               // LocalReport reportViewer1 = new LocalReport();
                reportViewer1.LoadReportDefinition(MyRptStream);
            }
            else
            {
                if(a4==true)
                {
                    MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.OrdonnanceA4), false);
                  //  LocalReport reportViewer1 = new LocalReport();
                    reportViewer1.LoadReportDefinition(MyRptStream);
                }
            }

            //reportViewer1.LoadReportDefinition(MyRptStream);

            ReportDataSource rds = new ReportDataSource();
            rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
            rds.Value = people;
            reportViewer1.DataSources.Add(rds);
            var selpara = new List<SVC.Param>();
            SVC.Param paramlocal = (proxy.GetAllParamétre());
            paramlocal.CheminLogo = "D/Logo.jpg";
            selpara.Add((paramlocal));

            var peoplemededcin = new List<SVC.Medecin>();
            peoplemededcin.Add(SelectMedecinListe);
           reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
            reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", OrdonnancePatientRecu));
            reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", peoplemededcin));
            reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", Entetemededcin));
            if (proxy.GetAllParamétre().CheminLogo != "")
            {

                if (proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().CheminLogo.ToString()) == true)
                {
                    reportViewer1.EnableExternalImages = true;
                   
                    String photolocation = System.Environment.CurrentDirectory + "/Logo.png";
                 
                    ReportParameter paramLogo = new ReportParameter();
                    paramLogo.Name = "ImagePath";
                    //  paramLogo.Values.Add(@"file:///C:\Logo.png");
                    paramLogo.Values.Add(@"file:///" + photolocation);
                    reportViewer1.SetParameters(paramLogo);
                    // reportViewer1.LocalReport.SetParameters(parameter);
                }

            }
            reportViewer1.Refresh();
            
            Export(reportViewer1);
            Print();
        }
        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>5,708661in</PageWidth>
                <PageHeight>8,26772in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        private Stream CreateStream(string name,
     string fileNameExtension, Encoding encoding,
     string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        private void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.DocumentName = PATIENT.NomPrenom;
                printDoc.Print();
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }
        private void HistoriqueDesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HistoriqueDesDataGrid.SelectedItem!=null)
            {
                SVC.Visite tasvisite= HistoriqueDesDataGrid.SelectedItem as SVC.Visite;
             //   if (tasvisite.Versement!=0)
             //   {
                    VersementPatientDataGrid.ItemsSource = (proxy.GetAllDepeiment()).Where(n=>n.cle==tasvisite.cle);
              //  }

            }
        }

        private void txtRechercheVersement_TextChanged(object sender, TextChangedEventArgs e)
        {

            System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(VersementPatientDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    SVC.Depeiment p = o as SVC.Depeiment;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.paiem.ToUpper().Contains(filter.ToUpper()));
                };
            }
        }

        private void btnNewRéglement_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.CréationCaisse == true && HistoriqueDesDataGrid.SelectedItem != null)
            {

                SVC.Visite SelectMedecin = HistoriqueDesDataGrid.SelectedItem as SVC.Visite;
                if (SelectMedecin.Reste != 0)
                {
                    AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, null, 3, null, SelectMedecin,null);
                    bn.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Facture déja soldé", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRechercheVisite_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnImprimerVersement_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.ImressionCaisse == true && VersementPatientDataGrid.SelectedItem != null && HistoriqueDesDataGrid.SelectedItem!=null)
            {
                SVC.Depeiment selectdepeiemt = VersementPatientDataGrid.SelectedItem as SVC.Depeiment;
                List<SVC.Visite> vs = new List<SVC.Visite>();
                SVC.Visite VisiteAregler = HistoriqueDesDataGrid.SelectedItem as SVC.Visite;
                vs.Add(VisiteAregler);
                List<SVC.Depeiment> dp = new List<SVC.Depeiment>();
                dp.Add(selectdepeiemt);
                ImpressionRecu cl = new ImpressionRecu(proxy, dp, vs);
                cl.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez sélectionner un réglement", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnSuppVersement_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.SuppréssionCaisse == true && VersementPatientDataGrid.SelectedItem != null)
            {
                bool SupDepaif = false;
                bool SupDepanse = false;
                bool UpdateRecouf = false;
                try
                {
                    SVC.Depeiment selectdepaief = VersementPatientDataGrid.SelectedItem as SVC.Depeiment;
                    var depense = (proxy.GetAllDepense()).Find(n => n.cle == selectdepaief.cle && n.Crédit == true && n.MontantCrédit == selectdepaief.montant);
                    var Recouf = (proxy.GetAllVisiteAll()).Find(n => n.cle == selectdepaief.cle);

                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {


                        /*suprimez l'écriture de depaief*/
                        proxy.DeleteDepeiment(selectdepaief);
                        SupDepaif = true;
                     //   MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("paiement succées", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        /*  suprimez l'écriture de depanse*/
                        proxy.DeleteDepense(depense);
                        SupDepanse = true;
                      //  MessageBoxResult result1g1 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepense succées", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        /*  update recouf c-a-d enlevez le montant */
                        Recouf.Reste = Recouf.Reste + selectdepaief.montant;
                        Recouf.Versement = Recouf.Versement - selectdepaief.montant;
                        Recouf.Soldé = false;

                        proxy.UpdateVisite(Recouf);
                        UpdateRecouf = true;
                     //   MessageBoxResult result1g1d = Xceed.Wpf.Toolkit.MessageBox.Show("Visite yes", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                        {
                            ts.Complete();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }


                    }
                    proxy.AjouterTransactionPaiementRefresh();
                    proxy.AjouterSoldeVisiteRefresh();
                    proxy.AjouterDepenseRefresh();
                }
                catch (FaultException ex)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                }
                catch (Exception ex)
                {
                    this.Title = ex.Message;
                }
            }else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

       
        private void txtDéjeuner_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BesoinCaloriqueCalculé != null && txtDéjeuner.Text != "")
            {
              labelDéjeuner.Content = Convert.ToString(BesoinCaloriqueCalculé.DEJ * Convert.ToDecimal(txtDéjeuner.Text) / 100);
            }
        }

        private void txtGouter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BesoinCaloriqueCalculé != null && txtGouter.Text != "")
            {
              
                Gouter16.Content = Convert.ToString(BesoinCaloriqueCalculé.DEJ * Convert.ToDecimal(txtGouter.Text) / 100);
       
            }
        }

        private void txtDiner_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BesoinCaloriqueCalculé != null && txtDiner.Text != "")
            {
             
                labelDiner.Content = Convert.ToString(BesoinCaloriqueCalculé.DEJ * Convert.ToDecimal(txtDiner.Text) / 100);
             
            }
        }
        /*************************examen buccal********************************/
        

     
     

      

     

        private void txtLeSoir_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BesoinCaloriqueCalculé != null && txtLeSoir.Text != "")
            {
                labelLesoir.Content = Convert.ToString(BesoinCaloriqueCalculé.DEJ * Convert.ToDecimal(txtLeSoir.Text) / 100);
                
            }
        }





















     



    











        













        private void btnValiderAnamnes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CréationDossierPatient == true && InterogatoireAnamnéseExiste == false)
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (comboCélibataire.IsChecked == true)
                        {
                            DiagnosticCréer.SituationFamiliale = "Célibataire";
                        }
                        else
                        {
                            if (comboMarié.IsChecked == true)
                            {
                                DiagnosticCréer.SituationFamiliale = "Marié (e)";

                            }
                            else
                            {
                                if (comboVeuf.IsChecked == true)
                                {
                                    DiagnosticCréer.SituationFamiliale = "Veuf (veuve)";
                                }
                                else
                                {
                                    if (comboDivorce.IsChecked == true)
                                    {
                                        DiagnosticCréer.SituationFamiliale = "Divorcé (e)";
                                    }
                                }
                            }
                        }
                        proxy.InsertDiagnostic(DiagnosticCréer);
                        ts.Complete();
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }





                }
                else
                {
                    if (memberuser.ModificationPatient == true && InterogatoireAnamnéseExiste == true)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (comboCélibataire.IsChecked == true)
                            {
                                DiagnosticCréer.SituationFamiliale = "Célibataire";
                            }
                            else
                            {
                                if (comboMarié.IsChecked == true)
                                {
                                    DiagnosticCréer.SituationFamiliale = "Marié (e)";

                                }
                                else
                                {
                                    if (comboVeuf.IsChecked == true)
                                    {
                                        DiagnosticCréer.SituationFamiliale = "Veuf (veuve)";
                                    }
                                    else
                                    {
                                        if (comboDivorce.IsChecked == true)
                                        {
                                            DiagnosticCréer.SituationFamiliale = "Divorcé (e)";
                                        }
                                    }
                                }
                            }
                            proxy.UpdateDiagnostic(DiagnosticCréer);
                            ts.Complete();
                            MessageBoxResult resultc1d = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }

                    }
                }
            }catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimerAnamnes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.ImpressionDossierPatient==true && DiagnosticCréer!=null)
                {
                    List<SVC.Diagnostic> listdig = new List<SVC.Diagnostic>();
                    listdig.Add(DiagnosticCréer);
                    ImpressionDiagnostic cl = new ImpressionDiagnostic(proxy, PATIENT, listdig,1);
                    cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void txtTaile_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtPois_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void CréerConstanteAnamnese_Click(object sender, RoutedEventArgs e)
        {
            if (DatePriseAnamnese.SelectedDate != null && txtTimeAnamnese.Text != "" && constanteAnamnéseExiste ==false && memberuser.CréationDossierPatient == true)
            {
                constanteCréer.Date1 = DatePriseAnamnese.SelectedDate;
                constanteCréer.Date2 = TimeSpan.Parse(txtTimeAnamnese.Text);
                proxy.InsertConstante(constanteCréer,PATIENT.Id);
                DatagridContstante.SelectedItem = null;
                DatePriseAnamnese.SelectedDate = null;
                txtPoidAnamnese.Text = "";
                txtTailleAnamnese.Text = "";
                txtPASAnamnese.Text = "";
                txtPADAnamnese.Text = "";
                txtPoulsAnamnese.Text = "";
                txtTempAnamnese.Text = "";
                txtGlycémieAjeunAnamnese.Text = "";
                txtGlycémiePostpranAnamnese.Text = "";
                txtTimeAnamnese.Text = "";
                txtIMCAnamnese.Text = "";
                txtHDLCHOLESTAnamnese.Text = "";
                txtHDLcholestérolAnamnese.Text = "";
                txttriglycéridesAnamnese.Text = "";
                txttHBA1CAnamnese.Text = "";
                txtMicroalbuminurieAnamnese.Text = "";
                txtUréesanguineAnamnese.Text = "";
                txtUréeurinaireAnamnese.Text = "";
                txtcréatininesanguinAnamnese.Text = "";
                txtccréatinineurinaireAnamnese.Text = "";
                // constanteCréer = null;
                constanteCréer = proxy.GetAllConstantesByPatient(PATIENT.Id).Find(n => n.Remarque == "DÉPISTAGE");
                constanteAnamnéseExiste = true;
                ConstaneAnamneseDétailGrid.DataContext = constanteCréer;
                CréerConstanteAnamnese.Content = "Modifier";
                ConstaneDétailGrid.IsEnabled = false;
                DatePriseAnamnese.SelectedDate = constanteCréer.Date1;
                txtTimeAnamnese.Text = Convert.ToString(constanteCréer.Date2);
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }else
            {
                if (DatePriseAnamnese.SelectedDate != null && txtTimeAnamnese.Text != "" && constanteAnamnéseExiste == true && memberuser.ModificationDossierPatient == true)
                {
                    proxy.UpdateConstante(constanteCréer,PATIENT.Id);
                    constanteCréer = proxy.GetAllConstantesByPatient(PATIENT.Id).Find(n => n.Remarque == "DÉPISTAGE");
                    ConstaneAnamneseDétailGrid.DataContext = constanteCréer;
                    DatePriseAnamnese.SelectedDate = constanteCréer.Date1;
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Opération réussie ! ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }

            }
        }

        private void txtTailleAnamnese_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTaille.Text != "0" && txtPoid.Text != "0")
                {
                    if (txtTaille.Text != "" && txtPoid.Text != "")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        txtIMC.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text)));
                    }
                }
                else
                {
                    txtIMC.Text = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }

        }

        private void txtPoidAnamnese_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTailleAnamnese.Text != "" && txtPoidAnamnese.Text != "")
                {
                    if (txtTailleAnamnese.Text != "0" && txtPoidAnamnese.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        txtIMCAnamnese.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoidAnamnese.Text) / (Convert.ToDecimal(txtTailleAnamnese.Text) * Convert.ToDecimal(txtTailleAnamnese.Text)));
                    }else
                    {
                        txtIMCAnamnese.Text = "0";
                    }
                }
                else
                {
                    txtIMCAnamnese.Text = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }

        }

        private void txtTailleAnamnese_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTailleAnamnese.Text != "" && txtPoidAnamnese.Text != "")
                {
                    if (txtTailleAnamnese.Text != "0" && txtPoidAnamnese.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        txtIMCAnamnese.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoidAnamnese.Text) / (Convert.ToDecimal(txtTailleAnamnese.Text) * Convert.ToDecimal(txtTailleAnamnese.Text)));
                    }else
                    {
                        txtIMCAnamnese.Text = "0";

                    }
                }
                else
                {
                    txtIMCAnamnese.Text = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void chautreprofessiel_Checked(object sender, RoutedEventArgs e)
        {
           if(chautreprofessiel.IsChecked==true)
            {
                txtActivitéAutre.Visibility=Visibility.Visible;
            }
           
        }

        private void chautreprofessiel_Unchecked(object sender, RoutedEventArgs e)
        {
        
                if (chautreprofessiel.IsChecked == false)
                {
                    txtActivitéAutre.Visibility = Visibility.Collapsed;
                }
            
        }

        private void autreatecedentfamille_Checked(object sender, RoutedEventArgs e)
        {
            if (autreatecedentfamille.IsChecked == true)
            {
                txtAntécédentFamiAutre.Visibility = Visibility.Visible;
            }
      }

        private void autreatecedentfamille_Unchecked(object sender, RoutedEventArgs e)
        {
            if (autreatecedentfamille.IsChecked == false)
            {
                txtAntécédentFamiAutre.Visibility = Visibility.Collapsed;
            }
        }

        private void autrefacteurderisque_Checked(object sender, RoutedEventArgs e)
        {
            if (autrefacteurderisque.IsChecked == true)
            {
                txtFacteurAutre.Visibility = Visibility.Visible;
            }
        }

        private void autrefacteurderisque_Unchecked(object sender, RoutedEventArgs e)
        {
            if (autrefacteurderisque.IsChecked == false)
            {
                txtFacteurAutre.Visibility = Visibility.Collapsed;
            }
        }

        private void chautrefemme_Checked(object sender, RoutedEventArgs e)
        {
            if (chautrefemme.IsChecked == true)
            {
                txtFemmeAutre.Visibility = Visibility.Visible;
            }
        }

        private void chautrefemme_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chautrefemme.IsChecked ==false)
            {
                txtFemmeAutre.Visibility = Visibility.Collapsed;
            }
        }

        private void chautreatecedentmed_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chautreatecedentmed.IsChecked == false)
            {
                txtMedicauxAutre.Visibility = Visibility.Collapsed;
            }
        }

        private void chautreatecedentmed_Checked(object sender, RoutedEventArgs e)
        {
            if (chautreatecedentmed.IsChecked == true)
            {
                txtMedicauxAutre.Visibility = Visibility.Visible;
            }
        }

        private void autretypealimentation_Checked(object sender, RoutedEventArgs e)
        {
            if (autretypealimentation.IsChecked == true)
            {
                txtAlimentaireAutre.Visibility = Visibility.Visible;
            }
        }

        private void autretypealimentation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (autretypealimentation.IsChecked == false)
            {
                txtAlimentaireAutre.Visibility = Visibility.Collapsed;
            }
        }

        private void chautresymp_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chautresymp.IsChecked == false)
            {
                txtSympAutre.Visibility = Visibility.Collapsed;
            }
        }

        private void chautresymp_Checked(object sender, RoutedEventArgs e)
        {
            if (chautresymp.IsChecked == true)
            {
                txtSympAutre.Visibility = Visibility.Visible;
            }
        }


       



        






        

        private void btnEditVisite_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SIMPLEDataGrid.SelectedItem != null)
                {

                    SVC.Visite SelectMedecin = SIMPLEDataGrid.SelectedItem as SVC.Visite;

                    gridsimplevisite.DataContext = SelectMedecin;


                    List<SVC.CasTraitement> testmedecin = proxy.GetAllTraitement().Where(n => n.CodePatient == PATIENT.Id && n.Devis == false).ToList();
                    txtMotifVisite.ItemsSource = testmedecin;
                    List<SVC.CasTraitement> tte = testmedecin.Where(n => n.Id == SelectMedecin.IdCas && n.Devis == false).ToList();
                    txtMotifVisite.SelectedItem = tte.First();
                    SVC.CasTraitement selectedtraitment = tte.First();
                    txtMotifVisite.IsEnabled = false;
                    if (SelectMedecin.ModeFacturation == 0 && (SelectMedecin.CasTraite == "Consultation simple" || SelectMedecin.CasTraite == "Consultation assistée"))
                    {
                        Grid.SetColumnSpan(txtConclusions, 2);
                        Grid.SetColumnSpan(txtExamen, 1);

                        TypeDeVisite.Visibility = Visibility.Visible;
                        txtblockmotifvisite.Visibility = Visibility.Visible;
                       
                        chactetraiter.Visibility = Visibility.Collapsed;
                        TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                        List<SVC.MotifVisite> testmedecin3 = proxy.GetAllMotifVisite().OrderBy(n => n.Motif).ToList();
                        TypeDeVisite.ItemsSource = testmedecin3;
                        List<SVC.MotifVisite> tte3 = testmedecin3.Where(n => n.Id == SelectMedecin.IdMotif).ToList();
                        TypeDeVisite.SelectedItem = tte3.First();


                        txtInterrogatoire.Text = SelectMedecin.Interrogatoire;
                        txtExamen.Text = SelectMedecin.Examen;
                        txtConclusions.Text = SelectMedecin.Conclusions;
                        TxtHonoraire.Text = Convert.ToString(SelectMedecin.Montant);
                        modifvisite = true;

                        TypeDeVisite.Visibility = Visibility.Visible;
                        txtblockmotifvisite.Visibility = Visibility.Visible;
                     
                        chactetraiter.Visibility = Visibility.Collapsed;
                        LabelForfait.Visibility = Visibility.Collapsed;
                        bntcreerSimpleVisite.ToolTip = "Modifier";
                        txtInterrogatoire.IsEnabled = true;
                        txtExamen.IsEnabled = true;
                        txtConclusions.IsEnabled = true;
                        TxtHonoraire.IsEnabled = true;
                        if(SelectMedecin.Diag==true)
                        {
                          //  bool hna = proxy.GetAllDiagnostic().Any(n => n.Cle.Trim() == SelectMedecin.cle.Trim() && n.Cle != "");
                          //  if(hna==true)
                         //   {
                                dia = proxy.GetAllDiagnostic().Find(n => n.Cle == SelectMedecin.cle);
                            constanteCréerVisiteASSI = proxy.GetAllConstantesByPatient(PATIENT.Id).Find(n=>n.cle == SelectMedecin.cle);
                            if (dia !=null)
                            { 
                                visitewithdiag = true;
                             //   MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Here", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                VisiteAs ll = new VisiteAs(proxy, dia, this, constanteCréerVisiteASSI);
                                ll.Show();
                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Attention détail de la consultation n'existe plus", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }

                        }
                    }
                    else
                    {
                      
                                if (SelectMedecin.ModeFacturation == 0 && SelectMedecin.CasTraite == "Traitement")
                                {
                                    Grid.SetColumnSpan(txtConclusions, 2);
                                    Grid.SetColumnSpan(txtExamen, 1);
                                    TypeDeVisite.Visibility = Visibility.Collapsed;
                                    txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                  
                                    chactetraiter.Visibility = Visibility.Collapsed;
                                    TypeDeActesTraitement.Visibility = Visibility.Visible;
                                    List<SVC.Acte> testmedecin3 = proxy.GetAllActe().OrderBy(n => n.Libelle).ToList();
                                    TypeDeActesTraitement.ItemsSource = testmedecin3;
                                    List<SVC.Acte> tte3 = testmedecin3.Where(n => n.Id == SelectMedecin.IdMotif).ToList();
                                    TypeDeActesTraitement.SelectedItem = tte3.First();


                                    txtInterrogatoire.Text = SelectMedecin.Interrogatoire;
                                    txtExamen.Text = SelectMedecin.Examen;
                                    txtConclusions.Text = SelectMedecin.Conclusions;
                                    TxtHonoraire.Text = Convert.ToString(SelectMedecin.Montant);
                                    modifvisite = true;

                                    TypeDeVisite.Visibility = Visibility.Collapsed;
                                    txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                 
                                    chactetraiter.Visibility = Visibility.Collapsed;
                                    LabelForfait.Visibility = Visibility.Collapsed;
                                    TypeDeActesTraitement.Visibility = Visibility.Visible;
                                    bntcreerSimpleVisite.ToolTip = "Modifier";
                                    txtInterrogatoire.IsEnabled = true;
                                    txtExamen.IsEnabled = true;
                                    txtConclusions.IsEnabled = true;
                                    TxtHonoraire.IsEnabled = true;

                                }
                            }
                        }

                   
             

            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                //this.Title = Mess;
                // this.WindowTitleBrush = Brushes.Red;
            }
        }












        private void MedicamentsDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                OrdonnanceDatagGrid.ItemsSource = proxy.GetAllEnteteOrdonnace().Where(n => n.CodePatient == PATIENT.Id).ToList();
                MedicamentsDataGrid.ItemsSource = (proxy.GetAllProduitOrdonnance()).OrderBy(r => r.Design);
                cbDci.ItemsSource = proxy.GetAllDci().OrderBy(r => r.Dci1);
                callback.InsertOrdonnancePatientCallbackevent += new ICallback.CallbackEventHandler38(callbackrecuOrdonnancePatient_Refresh);
                callback.InsertEnteteOrdonnaceCallbackevent += new ICallback.CallbackEventHandler37(callbackrecuEntete_Refresh);
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatagridImage1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (DatagridImage1.SelectedItem != null)
                {
                    SVC.DicomFichier SelectMedecin = DatagridImage1.SelectedItem as SVC.DicomFichier;

                    imgPhotod.Source = LoadImage(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));

                    op = null;

                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
            }
        }

        private void DatagridImagerie_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (DatagridImagerie.SelectedItem != null)
                {
                    SVC.DicomFichier SelectMedecin = DatagridImagerie.SelectedItem as SVC.DicomFichier;
                    //   ReadAndDisplayDownloadDicomFile((proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString())),SelectMedecin.NomFichierDicom);
                    //  Stream stream = new MemoryStream(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));
                    BinaryWriter binWriter = new BinaryWriter(new MemoryStream());
                    binWriter.Write(proxy.DownloadDocument(SelectMedecin.ChemainFichier.ToString()));
                    //  readBinary = new BinaryReader(stream);
                    readBinary = new BinaryReader(binWriter.BaseStream);
                    ReadAndDisplayDicomFileBinaryReader(readBinary, SelectMedecin.NomFichierDicom);

                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
            }
        }

        private void TypeDeActesTraitement_DropDownClosed(object sender, EventArgs e)
        {
            if(TypeDeActesTraitement.SelectedItem!=null)
            {
                SVC.Acte selectedacte = TypeDeActesTraitement.SelectedItem as SVC.Acte;
                TxtHonoraire.Text =Convert.ToString(selectedacte.Prix);
            }
        }

        private void btnImprimerCONSTAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DatagridContstante.SelectedItem != null && memberuser.ImpressionDossierPatient == true)
                {
                    SVC.Constante selectedconstante = DatagridContstante.SelectedItem as SVC.Constante;
                    ImpressionOneConstante cl = new ImpressionOneConstante(proxy, selectedconstante);
                    cl.Show();
                }
                else
                {
                    if (DatagridContstante.SelectedItem == null && memberuser.ImpressionDossierPatient == true )
                    {
                        List<SVC.Constante> selectedconstante = DatagridContstante.ItemsSource as List<SVC.Constante>;

                      
                      ImpressionListeConstante cl = new ImpressionListeConstante(proxy, selectedconstante);
                        cl.Show();
                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnSuppConstante_Click(object sender, RoutedEventArgs e)
        {
            if(DatagridContstante.SelectedItem!=null && memberuser.SuppressionDossierPatient==true)
            {
                SVC.Constante selectedconstante = DatagridContstante.SelectedItem as SVC.Constante;
                proxy.DeleteConstante(selectedconstante,PATIENT.Id);
                ConstaneDétailGrid.DataContext = null;
            }
        }

        private void txtRecherche12_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(RendezVousExisiteGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.RendezVou p = o as SVC.RendezVou;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Nom.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch
            {

            }
        }

        private void DateNOWRendez_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MedecinConsult != null)
            {

                RendezVousExisiteGrid.ItemsSource = (proxy.GetAllRendezVousParMedecin(MedecinConsult.Id)).Where(n => n.Date == DateNOWRendezz.SelectedDate.Value);
                NBRENDEZVOUS.Text = ((proxy.GetAllRendezVousParMedecin(MedecinConsult.Id)).Where(n => n.Date == DateNOWRendezz.SelectedDate.Value).AsEnumerable().Count()).ToString();

                //      Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(ProcessRows));
            }
            else
            {

                RendezVousExisiteGrid.ItemsSource = (proxy.GetAllRendezVous(DateNOWRendezz.SelectedDate.Value, DateNOWRendezz.SelectedDate.Value));
                NBRENDEZVOUS.Text = ((proxy.GetAllRendezVous(DateNOWRendezz.SelectedDate.Value, DateNOWRendezz.SelectedDate.Value)).AsEnumerable().Count()).ToString();

                //   Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(ProcessRows));
            }
        }

        private void txtRecherche5e_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(RendezVousExisiteGridPatient.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.RendezVou p = o as SVC.RendezVou;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Motif.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch
            {

            }
        }

        private void btnNewAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CréationDossierPatient==true)
                {
                    AjouterAuto cl = new AjouterAuto(proxy, callback, PATIENT, memberuser, null);
                    cl.Show();
                }else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch(Exception ex)

            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnSuppAuto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAutoDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(AutoDatagGrid.SelectedItem!=null)
                {
                    SVC.autosurveillance selectedauto = AutoDatagGrid.SelectedItem as SVC.autosurveillance;
                    autoSelected = selectedauto;
                    if (autoSelected.Existe == false && memberuser.CréationDossierPatient == true)
                    {
                        List<SVC.autosurveillanceDetail> listdetailauto = new List<SVC.autosurveillanceDetail>();

                        for (int day = 0; day <= 27; day++)
                        {

                            SVC.autosurveillanceDetail emp = new SVC.autosurveillanceDetail
                            {
                                Cle = autoSelected.Cle,
                                CodePatient = autoSelected.CodePatient,
                                Date = (autoSelected.débutdelafiche).Value.AddDays(day),


                            };
                            listdetailauto.Add(emp);
                         
                        }
                        gridAuto.Visibility = Visibility.Collapsed;
                        tabdetailaut.Visibility = Visibility.Visible;
                        AutoDetailGridDataGrid.ItemsSource = listdetailauto;
                        AutoDetailGridDataGrid.DataContext = listdetailauto;
                    }
                    else
                    {
                        if (autoSelected.Existe == true && memberuser.ModificationDossierPatient == true)
                        {
                            gridAuto.Visibility = Visibility.Collapsed;
                            tabdetailaut.Visibility = Visibility.Visible;
                           List<SVC.autosurveillanceDetail> OrdonnancePatientList = proxy.GetAllautosurveillanceDetail().Where(n => n.Cle == autoSelected.Cle).ToList();
                          
                            AutoDetailGridDataGrid.ItemsSource = OrdonnancePatientList;
                            AutoDetailGridDataGrid.DataContext = OrdonnancePatientList;

                        }
                    }





                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.ImpressionDossierPatient==true && AutoDatagGrid.SelectedItem!=null)
                {
                    SVC.autosurveillance selectedauto = AutoDatagGrid.SelectedItem as SVC.autosurveillance;
                    if (selectedauto.Type== "Type 1")
                    {
                        List<SVC.autosurveillanceDetail> listdetailauto = new List<SVC.autosurveillanceDetail>();

                        for (int day = 0; day <= 27; day++)
                        {

                            SVC.autosurveillanceDetail emp = new SVC.autosurveillanceDetail
                            {
                                Cle = selectedauto.Cle,
                                CodePatient = selectedauto.CodePatient,
                                Date = (selectedauto.débutdelafiche).Value.AddDays(day),


                            };
                            listdetailauto.Add(emp);
                          //  day++;
                        }
                        ImpressionAuto cl = new ImpressionAuto(1, proxy, PATIENT, selectedauto, listdetailauto,selectedauto.NbSemaine.Value);
                        cl.Show();
                    }else
                    {
                        if (selectedauto.Type == "Type 2")
                        {
                            List<SVC.autosurveillanceDetail> listdetailauto = new List<SVC.autosurveillanceDetail>();

                            for (int day = 0; day <= 27; day++)
                            {

                                SVC.autosurveillanceDetail emp = new SVC.autosurveillanceDetail
                                {
                                    Cle = selectedauto.Cle,
                                    CodePatient = selectedauto.CodePatient,
                                    Date = (selectedauto.débutdelafiche).Value.AddDays(day),


                                };
                                listdetailauto.Add(emp);
                                //  day++;
                            }
                            ImpressionAuto cl = new ImpressionAuto(2, proxy, PATIENT, selectedauto, listdetailauto, selectedauto.NbSemaine.Value);
                            cl.Show();
                        }
                    }

                }else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEditAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.ModificationDossierPatient==true && AutoDatagGrid.SelectedItem!=null)
                {
                    SVC.autosurveillance selectedauto = AutoDatagGrid.SelectedItem as SVC.autosurveillance;

                    AjouterAuto cl = new AjouterAuto(proxy, callback, PATIENT, memberuser, selectedauto);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }


            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimerFiche_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionDossierPatient == true && AutoDatagGrid.SelectedItem != null)
                {
                    SVC.autosurveillance selectedauto = AutoDatagGrid.SelectedItem as SVC.autosurveillance;
                    if (selectedauto.Type == "Type 1" && selectedauto.Existe == true)
                    {

             

                        List<SVC.autosurveillanceDetail> listdetailauto = proxy.GetAllautosurveillanceDetail().Where(n=>n.Cle==selectedauto.Cle).ToList();

                        ImpressionAuto cl = new ImpressionAuto(1, proxy, PATIENT, selectedauto, listdetailauto, selectedauto.NbSemaine.Value);
                        cl.Show();
                    }
                    else
                    {
                        if (selectedauto.Type == "Type 2" && selectedauto.Existe == true)
                        {
                            List<SVC.autosurveillanceDetail> listdetailauto = proxy.GetAllautosurveillanceDetail().Where(n => n.Cle == selectedauto.Cle).ToList();


                            ImpressionAuto cl = new ImpressionAuto(2, proxy, PATIENT, selectedauto, listdetailauto, selectedauto.NbSemaine.Value);
                            cl.Show();
                        }
                    }

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            } catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

      
        private void btnVALIDERFiche_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CréationDossierPatient == true && autoSelected.Existe == false)
                {
                    bool updateauto = false;
                    bool insertdetail = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        autoSelected.Existe = true;
                        proxy.Updateautosurveillance(autoSelected);
                        updateauto = true;
                        var LISTITEM1 = AutoDetailGridDataGrid.ItemsSource as List<SVC.autosurveillanceDetail>;
                        foreach(SVC.autosurveillanceDetail item in LISTITEM1)
                        {
                            insertdetail = false;
                            proxy.InsertautosurveillanceDetail(item);
                            insertdetail = true;
                        }
                        if(updateauto==true && insertdetail==true)
                        {
                            ts.Complete();
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    proxy.AjouterAutosurvRefresh(PATIENT.Id);
                }
                else
                {
                    if (memberuser.ModificationDossierPatient == true && autoSelected.Existe == true)
                    {
                        bool updateauto = false;
                        bool insertdetail = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            autoSelected.Existe = true;
                            proxy.Updateautosurveillance(autoSelected);
                            updateauto = true;
                            var LISTITEM1 = AutoDetailGridDataGrid.ItemsSource as List<SVC.autosurveillanceDetail>;
                            foreach (SVC.autosurveillanceDetail item in LISTITEM1)
                            {
                                insertdetail = false;
                                proxy.UpdateautosurveillanceDetail(item);
                                insertdetail = true;
                            }
                            if (updateauto == true && insertdetail == true)
                            {
                                ts.Complete();
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        proxy.AjouterAutosurvRefresh(PATIENT.Id);
                    }
                }

            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void YesButton3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                simplevisite = true;
                bool succéescasdent = false;
                SVC.CasTraitement CASSIMPLEVISITE = new SVC.CasTraitement
                {
                    Montant = 0,
                    Reste = 0,
                    Versement = 0,
                    Date = DateTime.Now,
                    EtatActuel = "Traité",
                    Devis = false,
                    Histoire = "",
                    Remarques = "",
                    Cas = "Consultation simple",
                    CodePatient = PATIENT.Id,
                    ModeFacturation = "visites",
                    Nom = PATIENT.Nom,
                    Prénom = PATIENT.Prénom,
                    ModeFacturationint = 0,
                    UserName = memberuser.UserName,
                };

                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {


                    /*****gérer les réglements***************/

                    if (MedecinConsult != null)
                    {
                        CASSIMPLEVISITE.MedecinNom = MedecinConsult.Nom;
                        CASSIMPLEVISITE.MedecinPrénom = MedecinConsult.Prénom;
                        CASSIMPLEVISITE.CodeMedecin = MedecinConsult.Id;

                    }
                    else
                    {
                        CASSIMPLEVISITE.MedecinNom = memberuser.UserName + " User";
                        CASSIMPLEVISITE.MedecinPrénom = memberuser.Prénom + " User";
                        CASSIMPLEVISITE.CodeMedecin = 0;
                    }

                    CASSIMPLEVISITE.cle = CASSIMPLEVISITE.Cas + PATIENT.Id + Convert.ToString(CASSIMPLEVISITE.Date) + "0" + DateTime.Now.TimeOfDay;

                    /***************************************/
                    proxy.InsertTypeTraitement(CASSIMPLEVISITE);
                    succéescasdent = true;






                    if (succéescasdent == true)
                    {
                        ts.Complete();
                        // MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMotifVisite.IsEnabled = false;
                        /*   List<SVC.CasTraitement> testmedecin = proxy.GetAllTraitement().Where(n => n.CodePatient == PATIENT.Id).ToList();
                           txtMotifVisite.ItemsSource = testmedecin;
                           List<SVC.CasTraitement> tte = testmedecin.Where(n => n.Date == CASSIMPLEVISITE.Date && n.Cas==CASSIMPLEVISITE.Cas ).ToList();
                           txtMotifVisite.SelectedItem = tte.First();*/

                    }
                }

                proxy.AjouterCasTraitementRefresh();
                var itemsource = txtMotifVisite.ItemsSource as List<SVC.CasTraitement>;
                txtMotifVisite.SelectedIndex = itemsource.Count();
                TypeDeVisite.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                TypeDeVisite.Visibility = Visibility.Visible;
                txtblockmotifvisite.Visibility = Visibility.Visible;
                TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                txtInterrogatoire.IsEnabled = true;
                txtInterrogatoire.Text = "";
                txtExamen.Text = "";
                txtConclusions.Text = "";
                TxtHonoraire.Text = "";
                txtExamen.IsEnabled = true;
                txtConclusions.IsEnabled = true;
                TxtHonoraire.IsEnabled = true;
                modifvisite = false;
                LabelForfait.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(txtConclusions, 2);
                Grid.SetColumnSpan(txtExamen, 1);
            }
            catch (Exception ex)

            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void NoButton_Click3(object sender, RoutedEventArgs e)
        {
            try
            {
                /**************************************/
                simplevisite = true;
                bool succéescasdent = false;
                SVC.CasTraitement CASSIMPLEVISITE = new SVC.CasTraitement
                {
                    Montant = 0,
                    Reste = 0,
                    Versement = 0,
                    Date = DateTime.Now,
                    EtatActuel = "Traité",
                    Devis = false,
                    Histoire = "",
                    Remarques = "",
                    Cas = "Traitement",
                    CodePatient = PATIENT.Id,
                    ModeFacturation = "visites",
                    Nom = PATIENT.Nom,
                    Prénom = PATIENT.Prénom,
                    ModeFacturationint = 0,
                    UserName = memberuser.UserName,

                };

                txtInterrogatoire.IsEnabled = true;
                txtExamen.IsEnabled = true;
                txtConclusions.IsEnabled = true;
                TxtHonoraire.IsEnabled = true;
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {


                    /*****gérer les réglements***************/

                    if (MedecinConsult != null)
                    {
                        CASSIMPLEVISITE.MedecinNom = MedecinConsult.Nom;
                        CASSIMPLEVISITE.MedecinPrénom = MedecinConsult.Prénom;
                        CASSIMPLEVISITE.CodeMedecin = MedecinConsult.Id;

                    }
                    else
                    {
                        CASSIMPLEVISITE.MedecinNom = memberuser.UserName + " User";
                        CASSIMPLEVISITE.MedecinPrénom = memberuser.Prénom + " User";
                        CASSIMPLEVISITE.CodeMedecin = 0;
                    }

                    CASSIMPLEVISITE.cle = CASSIMPLEVISITE.Cas + PATIENT.Id + Convert.ToString(CASSIMPLEVISITE.Date) + "0" + DateTime.Now.TimeOfDay;

                    /***************************************/
                    proxy.InsertTypeTraitement(CASSIMPLEVISITE);
                    succéescasdent = true;






                    if (succéescasdent == true)
                    {
                        ts.Complete();
                        // MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMotifVisite.IsEnabled = false;


                    }
                }

                proxy.AjouterCasTraitementRefresh();
                var itemsource = txtMotifVisite.ItemsSource as List<SVC.CasTraitement>;
                txtMotifVisite.SelectedIndex = itemsource.Count();
                TypeDeActesTraitement.ItemsSource = proxy.GetAllActe().OrderBy(n => n.Libelle);
                TypeDeActesTraitement.Visibility = Visibility.Visible;
                TypeDeVisite.Visibility = Visibility.Collapsed;
                txtblockmotifvisite.Visibility = Visibility.Collapsed;
                txtInterrogatoire.Text = "";
                txtExamen.Text = "";
                txtConclusions.Text = "";
                TxtHonoraire.Text = "";
                modifvisite = false;
                LabelForfait.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(txtConclusions, 2);
                Grid.SetColumnSpan(txtExamen, 1);
                /********************************************************/
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MedecinConsult != null && memberuser.ModuleAchat == true)
                {
                    MonStock cl = new MonStock(proxy, memberuser, callback, PATIENT);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtMotifVisite_PopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            try
            {
                if (txtMotifVisite.SelectedItem != null)
                {
                    SVC.CasTraitement selectedCasTraitement = txtMotifVisite.SelectedItem as SVC.CasTraitement;
                    if (selectedCasTraitement.ModeFacturationint == 2)
                    {
                        LabelForfait.Visibility = Visibility.Visible;
                        LabelForfait.Text = " forfait à " + selectedCasTraitement.Montant + ", reste non facturé " + selectedCasTraitement.Reste + " Montant reste a payer :" + ((proxy.GetAllVisiteByVisite(PATIENT.Id)).Where(n => n.IdCas == selectedCasTraitement.Id).AsEnumerable().Sum(o => o.Reste)).ToString(); ;
                        TypeDeVisite.Visibility = Visibility.Visible;
                        txtblockmotifvisite.Visibility = Visibility.Visible;
                     
                        chactetraiter.Visibility = Visibility.Collapsed;
                        TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                        Grid.SetColumnSpan(txtConclusions, 2);
                        Grid.SetColumnSpan(txtExamen, 1);
                    }
                    else
                    {
                        if (selectedCasTraitement.ModeFacturationint == 1)
                        {
                            LabelForfait.Visibility = Visibility.Visible;
                            LabelForfait.Text = "(totaux des actes s'élevent à " + selectedCasTraitement.Montant + " ,reste non facturé " + selectedCasTraitement.Reste + " Montant reste a payer :" + ((proxy.GetAllVisiteByVisite(PATIENT.Id)).Where(n => n.CodePatient == PATIENT.Id && n.IdCas == selectedCasTraitement.Id).AsEnumerable().Sum(o => o.Reste)).ToString(); ;
                            TypeDeVisite.Visibility = Visibility.Collapsed;
                            txtblockmotifvisite.Visibility = Visibility.Collapsed;
                          
                            chactetraiter.Visibility = Visibility.Visible;
                            TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                            Grid.SetColumnSpan(txtConclusions, 2);
                            Grid.SetColumnSpan(txtExamen, 1);
                        }
                        else
                        {
                            if (selectedCasTraitement.ModeFacturationint == 0 && selectedCasTraitement.Cas == "Consultation simple")
                            {
                                TypeDeVisite.Visibility = Visibility.Visible;
                                txtblockmotifvisite.Visibility = Visibility.Visible;
                                LabelForfait.Visibility = Visibility.Collapsed;
                                chactetraiter.Visibility = Visibility.Collapsed;
                                TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                                Grid.SetColumnSpan(txtConclusions, 2);
                                Grid.SetColumnSpan(txtExamen, 1);
                            }
                            else
                            {
                                if (selectedCasTraitement.ModeFacturationint == 0 && selectedCasTraitement.Cas == "Traitement")
                                {
                                    TypeDeVisite.Visibility = Visibility.Collapsed;
                                    txtblockmotifvisite.Visibility = Visibility.Collapsed;
                                    LabelForfait.Visibility = Visibility.Collapsed;
                                    chactetraiter.Visibility = Visibility.Collapsed;
                                    TypeDeActesTraitement.Visibility = Visibility.Visible;
                                    Grid.SetColumnSpan(txtConclusions, 2);
                                    Grid.SetColumnSpan(txtExamen, 1);
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void TypeDeActesTraitement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TypeDeActesTraitement.SelectedItem != null)
                {
                    SVC.Acte selectedacte = TypeDeActesTraitement.SelectedItem as SVC.Acte;
                    TxtHonoraire.Text = Convert.ToString(selectedacte.Prix);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void BTNPLAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               dia = new SVC.Diagnostic
                {
                    CodePatient=PATIENT.Id,
                    NomPatient=PATIENT.Nom,
                    PrénomPatient=PATIENT.Prénom,
                    MemberUser=memberuser.UserName,
                    
                };
                if (MedecinConsult!=null)
                {
                    dia.CodeMedecin = MedecinConsult.Id;
                    dia.NomMedecin = MedecinConsult.Nom;
                    dia.PrénomMedecin = MedecinConsult.Prénom;
                }else
                {
                    dia.CodeMedecin = 0;
                    dia.NomMedecin = memberuser.UserName;
                    dia.PrénomMedecin = memberuser.UserName;
                }
                constanteCréerVisiteASSI = new SVC.Constante
                {
                    CodePatient = PATIENT.Id,
                    créatininesanguin = 0,
                    créatinineurinaire = 0,
                    Date1 = DateTime.Now,
                    Date2 = DateTime.Now.TimeOfDay,
                    GlycémieAjeun = 0,
                    GlycémiePostprandiale = 0,
                    HbA1c = 0,
                    HDL_cholestérol = 0,
                    IMC = 0,
                    LDL_cholestérol = 0,
                    Micro_albuminurie = 0,
                    Nom = PATIENT.Nom,
                    PAD = 0,
                    PAS = 0,
                    Poid = 0,
                    Pouls = 0,
                    Prénom = PATIENT.Prénom,
                    Taille = 0,
                    Temp = 0,
                    triglycérides = 0,
                    Uréesanguine = 0,
                    Uréeurinaire = 0,
                    UserName = memberuser.UserName,
                    Remarque = "DÉPISTAGE",
                    Cholestéroltotal=0,
                };
                simplevisite = true;
                bool succéescasdent = false;
                SVC.CasTraitement CASSIMPLEVISITE = new SVC.CasTraitement
                {
                    Montant = 0,
                    Reste = 0,
                    Versement = 0,
                    Date = DateTime.Now,
                    EtatActuel = "Traité",
                    Devis = false,
                    Histoire = "",
                    Remarques = "",
                    Cas = "Consultation assistée",
                    CodePatient = PATIENT.Id,
                    ModeFacturation = "visites",
                    Nom = PATIENT.Nom,
                    Prénom = PATIENT.Prénom,
                    ModeFacturationint = 0,
                    UserName = memberuser.UserName,
                };

                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {


                    /*****gérer les réglements***************/

                    if (MedecinConsult != null)
                    {
                        CASSIMPLEVISITE.MedecinNom = MedecinConsult.Nom;
                        CASSIMPLEVISITE.MedecinPrénom = MedecinConsult.Prénom;
                        CASSIMPLEVISITE.CodeMedecin = MedecinConsult.Id;

                    }
                    else
                    {
                        CASSIMPLEVISITE.MedecinNom = memberuser.UserName + " User";
                        CASSIMPLEVISITE.MedecinPrénom = memberuser.Prénom + " User";
                        CASSIMPLEVISITE.CodeMedecin = 0;
                    }

                    CASSIMPLEVISITE.cle = CASSIMPLEVISITE.Cas + PATIENT.Id + Convert.ToString(CASSIMPLEVISITE.Date) + "0" + DateTime.Now.TimeOfDay;

                    /***************************************/
                    proxy.InsertTypeTraitement(CASSIMPLEVISITE);
                    succéescasdent = true;






                    if (succéescasdent == true)
                    {
                        ts.Complete();
                        // MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMotifVisite.IsEnabled = false;
                        /*   List<SVC.CasTraitement> testmedecin = proxy.GetAllTraitement().Where(n => n.CodePatient == PATIENT.Id).ToList();
                           txtMotifVisite.ItemsSource = testmedecin;
                           List<SVC.CasTraitement> tte = testmedecin.Where(n => n.Date == CASSIMPLEVISITE.Date && n.Cas==CASSIMPLEVISITE.Cas ).ToList();
                           txtMotifVisite.SelectedItem = tte.First();*/

                    }
                }

                proxy.AjouterCasTraitementRefresh();
                var itemsource = txtMotifVisite.ItemsSource as List<SVC.CasTraitement>;
                txtMotifVisite.SelectedIndex = itemsource.Count();
                TypeDeVisite.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                TypeDeVisite.Visibility = Visibility.Visible;
                txtblockmotifvisite.Visibility = Visibility.Visible;
                TypeDeActesTraitement.Visibility = Visibility.Collapsed;
                txtInterrogatoire.IsEnabled = true;
                txtInterrogatoire.Text = "";
                txtExamen.Text = "";
                txtConclusions.Text = "";
                TxtHonoraire.Text = "";
                txtExamen.IsEnabled = true;
                txtConclusions.IsEnabled = true;
                TxtHonoraire.IsEnabled = true;
                modifvisite = false;
                LabelForfait.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(txtConclusions, 2);
                Grid.SetColumnSpan(txtExamen, 1);
                visitewithdiag = true;
                dialog1.Close();
                VisiteAs cl = new VisiteAs(proxy,dia,this,constanteCréerVisiteASSI);
                cl.Show();
              
                 
                

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPourProt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(txtPourProt.Text!="" && txtPourLipide.Text!="" && txtPourGlicu.Text!="")
                {
                    txtlabeltotalProtéines.Content = Convert.ToDecimal((Convert.ToDecimal(txtlabeltotalCalorique.Content)*Convert.ToDecimal(txtPourProt.Text))/100);
                 
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtPourLipide_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtlabeltotalLipides.Content = Convert.ToDecimal((Convert.ToDecimal(txtlabeltotalCalorique.Content) * Convert.ToDecimal(txtPourLipide.Text)) / 100); ;
            
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtPourGlicu_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtlabeltotalGlucides.Content = Convert.ToDecimal((Convert.ToDecimal(txtlabeltotalCalorique.Content) * Convert.ToDecimal(txtPourGlicu.Text)) / 100); ;

            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void txtPourGlicu_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtPourLipide_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void txtPourProt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void btnImprimerBonExamen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.ImpressionDossierPatient==true)
                {
                    BonExamen ll = new BonExamen(proxy,callback,PATIENT);
                    ll.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void DatagridContstante_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void btnEditConstante_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (DatagridContstante.SelectedItem != null)
                {
                 //   autreexamentab.IsEnabled = true;
                    constanteCréer = DatagridContstante.SelectedItem as SVC.Constante;
                    ConstaneDétailGrid.DataContext = constanteCréer;
                    DatePrise.SelectedDate = constanteCréer.Date1;
                    txtTime.Text = Convert.ToString(constanteCréer.Date2);
                    ConstaneDétailGrid.IsEnabled = true;
                    CréerConstante.Content = "Modifier";
                    modif = true;
                    autrebilanlist = proxy.GetAllAutreBilan((DatagridContstante.SelectedItem as SVC.Constante).cle);
                    if (autrebilanlist.Count>0)
                    {
                        GridDataGrid.DataContext = autrebilanlist;
                        GridDataGrid.ItemsSource = autrebilanlist;
                        autreexamentab.IsEnabled = true;
                    }else
                    {
                        autrebilanlist = new List<SVC.AutreBilan>();
                        GridDataGrid.DataContext = autrebilanlist;
                        GridDataGrid.ItemsSource = autrebilanlist;
                        autreexamentab.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string Mess = "Error: " + ex.Message;
                this.Title = Mess;
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(Mess, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionDossierPatient == true)
                {
                    if (autrebilanlist.Count() > 0)
                    {
                        List<SVC.Patient> listpat = new List<SVC.Patient>();
                        listpat.Add(PATIENT);
                        ImpressionAutreBilan cl = new ImpressionAutreBilan(proxy, autrebilanlist, listpat);
                        cl.Show();
                    }
                }else
                {
                    MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch(Exception ex)
            {
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void PrendreRendezVous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CréationRendezVous == true )
                {
                    SVC.RendezVou rendezvous = new SVC.RendezVou
                    {
                        Nom =PATIENT.Nom,
                        Prénom = PATIENT.Prénom,
                        CodePatient = PATIENT.Id,
                        Adresse = PATIENT.Adresse,
                        Téléphone = PATIENT.Téléphone,
                        Email = PATIENT.Email,
                        PrisPar = memberuser.UserName,

                    };

                    PrendreRendezVous kl = new PrendreRendezVous(rendezvous, proxy, memberuser, callback, 4, PATIENT);

                    kl.Show();
                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnchargementproduit_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OrdonnanceDatagGrid.ItemsSource = proxy.GetAllEnteteOrdonnace().Where(n => n.CodePatient == PATIENT.Id).ToList();
                MedicamentsDataGrid.ItemsSource = (proxy.GetAllProduitOrdonnance()).OrderBy(r => r.Design);
                cbDci.ItemsSource = proxy.GetAllDci().OrderBy(r => r.Dci1);
                callback.InsertOrdonnancePatientCallbackevent += new ICallback.CallbackEventHandler38(callbackrecuOrdonnancePatient_Refresh);
                callback.InsertEnteteOrdonnaceCallbackevent += new ICallback.CallbackEventHandler37(callbackrecuEntete_Refresh);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtTaile_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if (txtTaile.Text != "" && txtPois.Text != "")
                {
                    if (txtTaile.Text != "0" && txtPois.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        ValeurImc.Content = string.Format("{0:0.##}", Convert.ToDecimal(txtPois.Text) / (Convert.ToDecimal(txtTaile.Text) * Convert.ToDecimal(txtTaile.Text)));
                    }
                    else
                    {
                        ValeurImc.Content = "0";

                    }
                }
                else
                {
                    ValeurImc.Content = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void txtPois_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTaile.Text != "" && txtPois.Text != "")
                {
                    if (txtTaile.Text != "0" && txtPois.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        ValeurImc.Content = string.Format("{0:0.##}", Convert.ToDecimal(txtPois.Text) / (Convert.ToDecimal(txtTaile.Text) * Convert.ToDecimal(txtTaile.Text)));
                    }
                    else
                    {
                        ValeurImc.Content = "0";

                    }
                }
                else
                {
                    ValeurImc.Content = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void btnImprimercONSULTATIONCLI02_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionDossierPatient == true && DiagnosticCréer != null)
                {
                    List<SVC.Diagnostic> listdig = new List<SVC.Diagnostic>();
                    listdig.Add(DiagnosticCréer);
                    ImpressionDiagnostic cl = new ImpressionDiagnostic(proxy, PATIENT, listdig, 2);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimerparaf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (constanteCréer != null && memberuser.ImpressionDossierPatient == true)
                {
                    ImpressionOneConstante cl = new ImpressionOneConstante(proxy, constanteCréer);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void FamilleAlimentDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {/**************
                 try
            { 
            SVC.RendezVou RowDataContaxt = e.Row.DataContext as SVC.RendezVou;
            if (RowDataContaxt != null)
            {
                    var converter = new System.Windows.Media.BrushConverter();

                    if (RowDataContaxt.Confirm == true)
                    e.Row.Background = (Brush)converter.ConvertFromString(RdvPresent);
                    else if (RowDataContaxt.Confirm == false)
                    e.Row.Background = (Brush)converter.ConvertFromString(RdvNoPresent);  
               
                }
             } catch (Exception ex)
            {

                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
             }************************/

                SVC.Aliment RowDataContaxt = e.Row.DataContext as SVC.Aliment;

                if (RowDataContaxt != null)
                {
                   // var converter = new System.Windows.Media.BrushConverter();
                    if (RowDataContaxt.IgBas == true && RowDataContaxt.IgMoy == false && RowDataContaxt.IgElv == false && RowDataContaxt.IgNotDef == false)
                        e.Row.Background = Brushes.LightGreen;
                    else if (RowDataContaxt.IgMoy == true && RowDataContaxt.IgBas == false && RowDataContaxt.IgElv == false && RowDataContaxt.IgNotDef == false)
                        e.Row.Background = Brushes.Orange;
                    else if (RowDataContaxt.IgElv == true && RowDataContaxt.IgBas == false && RowDataContaxt.IgMoy == false && RowDataContaxt.IgNotDef == false)
                        e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#008000") ;
                    else
                        e.Row.Background = Brushes.Cyan;

                }

           
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }






    }
}
