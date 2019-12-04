using DevExpress.Xpf.Core;
using GestionClinique.SVC;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

namespace GestionClinique.Administrateur
{
    /// <summary>
      /// </summary>
    public partial class AjouterMedecin : DXWindow
    {
        string title=null;
        Brush titlebrush=null;
        SVC.Medecin SelectMedecinListe;
        SVC.Medecin selectmedecinforsession;
        SVC.ServiceCliniqueClient clientproxy;
       // SVC.ServiceCliniqueClient clientproxy;
        private delegate void FaultedInvokerMedecin();
        bool samedi, dimanche, lundi, mardi, mercredi, jeudi, vendredi, NewCréation;
        TimeSpan TEMPSVISITE, SAMEDID, SAMEDIF, DIMANCHED, DIMANCHEF, LUNDID, LUNDIF, MARDID, MARDIF, MERCREDID, MERCREDIF, JEUDID, JEUDIF, VENDREDID, VENDREDIF;
        SVC.Membership selectmedecinsession;
        int interfacechangement;
        public AjouterMedecin(SVC.Medecin test, SVC.ServiceCliniqueClient clientproxy1, SVC.Membership mem, ICallback callbackrecu)
        {
            try
            {
               
                if (clientproxy1 != null)
                {
                    if (clientproxy1.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        InitializeComponent();
                        /////////**********************************/

                        clientproxy = clientproxy1;





                        /*********************************/


                        //   clientproxy=clientproxy1;
                        clientproxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                        clientproxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                        callbackrecu.InsertSpécialitéCallbackEvent += new ICallback.CallbackEventHandler9(callbackrecu_Refresh);
                        TxtSpecialité.ItemsSource = clientproxy.GetAllSpécialité();
                        
                        if (test != null)
                        {
                            SelectMedecinListe = test;
                            NewCréation = false;
                            title = this.Title;
                         //   titlebrush = this.WindowTitleBrush;
                            this.InfGénéral.DataContext = SelectMedecinListe;
                            InfEnteteOrdonnace.DataContext = SelectMedecinListe;
                            //    Refreshdatagrid(SelectMedecinListe.Nom, SelectMedecinListe.Prénom);
                            TypeCombo.SelectedItem = SelectMedecinListe.Type;
                            tabitemMembership.IsEnabled = true;
                            interfacechangement = 2;
                            /*********************/
                            var peoplemededcin = new List<SVC.Medecin>();
                            //SVC.Medecin MedecinRecu = new SVC.Medecin { };
                            peoplemededcin.Add(SelectMedecinListe);
                            fCreermedecin.Content = "Modifier le Medecin";
                            var EnteteOrdoonancePatient = new List<SVC.EnteteOrdonnace>();
                            SVC.EnteteOrdonnace enteteRecu = new SVC.EnteteOrdonnace
                            {
                                CodePatient = 0,
                                créer = true,
                                Date = DateTime.Now,
                                NomPatient = "Nom du Patient",
                                PrénomPatient = "Prénom du Patient",
                                UserName = "Administrateur",

                            };
                            EnteteOrdoonancePatient.Add(enteteRecu);

                            var people = new List<SVC.Patient>();
                            SVC.Patient PatientRecu = new SVC.Patient
                            {
                                Nom = "Nom du Patient",
                                Prénom = "Prénom du Patient",
                                Adresse = "Adresse du cabinet medical",
                            };
                            people.Add(PatientRecu);
                            var LISTORDOOO = new List<SVC.OrdonnancePatient>();
                            SVC.OrdonnancePatient OrdonnancePatientRecu = new SVC.OrdonnancePatient
                            {
                                CodeCnas = 04936,
                                Design = "DOLIPRANE",
                                Dosage = "1000MG",
                                Colisage = "BTE / 8",
                                Quantite = 3,
                                Remarques = "Traitement symptomatique des douleurs d'intensité légère à modérée et/ou des états fébriles.",

                            };
                            LISTORDOOO.Add(OrdonnancePatientRecu);
                            SVC.OrdonnancePatient OrdonnancePatientRecu2 = new SVC.OrdonnancePatient
                            {
                                CodeCnas = 05138,
                                Design = "BETASONE",
                                Dosage = " 1MG ",
                                Colisage = " BTE/30",
                                Quantite = 3,
                                Remarques= "Traitement symptomatique des douleurs d'intensité légère à modérée et/ou des états fébriles.",
                            };
                            LISTORDOOO.Add(OrdonnancePatientRecu2);
                            clientproxy = clientproxy1;
                            // datable = datatablerecu;
                            MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.Ordonnance), false);
                            reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                         
                            // reportViewer1.LocalReport.ReportPath = "../../Patient/ReportOnePatient.rdlc";

                            ReportDataSource rds = new ReportDataSource();
                            rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                            rds.Value = people;
                            this.reportViewer1.LocalReport.DataSources.Add(rds);
                            var selpara = new List<SVC.Param>();
                            selpara.Add((clientproxy.GetAllParamétre()));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", LISTORDOOO));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", peoplemededcin));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", EnteteOrdoonancePatient));
                            reportViewer1.LocalReport.EnableExternalImages = true;
                            ReportParameter paramLogo = new ReportParameter();
                            paramLogo.Name = "ImagePath";
                            String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                            paramLogo.Values.Add(@"file:///" + photolocation);
                            reportViewer1.LocalReport.SetParameters(paramLogo);
                            reportViewer1.RefreshReport();






                            /*****************************/

                            txtTempsDevisite.Text = SelectMedecinListe.TempsVisite.ToString();
                            txtSamediD.Text = SelectMedecinListe.SamediD.ToString();
                            txtSamediF.Text = SelectMedecinListe.SamediF.ToString();
                            txtDimancheD.Text = SelectMedecinListe.DimancheD.ToString();
                            txtDimancheF.Text = SelectMedecinListe.DimancheF.ToString();
                            txtLundiD.Text = SelectMedecinListe.LundiD.ToString();
                            txtLundiF.Text = SelectMedecinListe.LundiF.ToString();
                            txtMardiD.Text = SelectMedecinListe.MardiD.ToString();
                            txtMardiF.Text = SelectMedecinListe.MardiF.ToString();
                            txtMercrediD.Text = SelectMedecinListe.MercrediD.ToString();
                            txtMercrediF.Text = SelectMedecinListe.MercrediF.ToString();
                            txtJeudiD.Text = SelectMedecinListe.JeudiD.ToString();
                            txtJeudiF.Text = SelectMedecinListe.JeudiF.ToString();
                            txtVendrediD.Text = SelectMedecinListe.VendrediD.ToString();
                            txtVendrediF.Text = SelectMedecinListe.VendrediF.ToString();

                            if(chSamedi.IsChecked==true)
                            {
                                txtSamediD.Enabled = true;
                                txtSamediF.Enabled = true;
                            }else
                            {
                                txtSamediD.Enabled = false;
                                txtSamediF.Enabled = false;
                            }
                                    
                            if(chDimanche.IsChecked==true)
                            {
                                txtDimancheD.Enabled = true;
                                txtDimancheF.Enabled = true;
                            }else
                            {
                                txtDimancheD.Enabled = false;
                                txtDimancheF.Enabled = false;
                            }

                            if(chLundi.IsChecked==true)
                            {
                                txtLundiD.Enabled = true;
                                txtLundiF.Enabled = true;
                            }else
                            {
                                txtLundiD.Enabled = false;
                                txtLundiF.Enabled = false;
                            }

                            if(chMardi.IsChecked==true)
                            {
                                txtMardiD.Enabled = true;
                                txtMardiF.Enabled = true;
                            }else
                            {
                                txtMardiD.Enabled = false;
                                txtMardiF.Enabled = false;
                            }
                            if(chMercredi.IsChecked==true)
                            {
                                txtMercrediD.Enabled = true;
                                txtMercrediF.Enabled = true;
                            }else
                            {
                                txtMercrediF.Enabled = false;
                                txtMercrediD.Enabled = false;

                            }
                            if(chJeudi.IsChecked==true)
                            {
                                txtJeudiD.Enabled = true;
                                txtJeudiF.Enabled = true;
                            }else
                            {
                                txtJeudiD.Enabled = false;
                                txtJeudiF.Enabled = false;
                            }
                             if(chVendredi.IsChecked==true)
                            {
                                txtVendrediD.Enabled = true;
                                txtVendrediF.Enabled = true;
                            }else
                            {
                                txtVendrediD.Enabled = false;
                                txtVendrediF.Enabled = false;
                            }
                        }
                        else
                        {
                            interfacechangement = 1;
                            SelectMedecinListe = new Medecin
                            {
                                Type = "Permanent",
                                EnteteCabinet01 = "CABINET MEDICAL DE LA PETITE FORET",
                                EnteteCabinet02 = "33, RUE DE combattant 3333 Ville",
                                EnteteCabinet03 = "Tél : 033 33 33 33  R.D.V ET URGENCES",
                                Entete01ar = "الحكيم الدكتور البروفيسور",
                                Entete02ar = "طبيب داخلي سابق مستشفيات ",
                                Entete03ar = "أستاد مساعد سابق المستشفى الجامعي قسنطينة",
                                Entete04ar = "أخصائي في جراحة العظام و المفاصل والكسور",
                                Entete01fr = "Docteur Professeur Medecin",
                                Entete02fr= "Spécialiste en Chirurgie Orthopédique",
                                Entete03fr= "Cité 00 rue du medecin. 5Bt A1 UV14",
                                Entete04fr= "Ordre des Médecins N°00/0000",

                            };
                            /**************************************/
                            var peoplemededcin = new List<SVC.Medecin>();
                            //SVC.Medecin MedecinRecu = new SVC.Medecin { };
                            peoplemededcin.Add(SelectMedecinListe);

                            var EnteteOrdoonancePatient = new List<SVC.EnteteOrdonnace>();
                            SVC.EnteteOrdonnace enteteRecu = new SVC.EnteteOrdonnace
                            {
                                CodePatient=0,
                                créer= true,
                                Date= DateTime.Now,
                                NomPatient="Nom du Patient",
                                PrénomPatient="Prénom du Patient",
                                UserName="Administrateu",

                            };
                            EnteteOrdoonancePatient.Add(enteteRecu);

                            var people = new List<SVC.Patient>();
                            SVC.Patient PatientRecu = new SVC.Patient
                            { Nom = "Nom du Patient",
                              Prénom = "Prénom du Patient",
                              Adresse="00 rue du medecin constantine",
                            };
                            people.Add(PatientRecu);
                            var LISTORDOOO = new List<SVC.OrdonnancePatient>();
                            SVC.OrdonnancePatient OrdonnancePatientRecu = new SVC.OrdonnancePatient
                            {
                               CodeCnas= 04936,
                               Design= "DOLIPRANE",
                               Dosage="1000MG",
                               Colisage="BTE / 8",
                               Quantite=3,
                               Remarques = "Traitement symptomatique des douleurs d'intensité légère à modérée et/ou des états fébriles.",

                            };
                            LISTORDOOO.Add(OrdonnancePatientRecu);
                            SVC.OrdonnancePatient OrdonnancePatientRecu2 = new SVC.OrdonnancePatient
                            {
                                CodeCnas = 05138,
                                Design = "BETASONE",
                                Dosage = " 1MG ",
                                Colisage = " BTE/30",
                                Quantite = 3,
                                Remarques = "Traitement symptomatique des douleurs d'intensité légère à modérée et/ou des états fébriles.",

                            };
                            LISTORDOOO.Add(OrdonnancePatientRecu2);

                            clientproxy = clientproxy1;
                            // datable = datatablerecu;

                            MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.Ordonnance), false);
                            reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                            // reportViewer1.LocalReport.ReportPath = "../../Patient/ReportOnePatient.rdlc";

                            ReportDataSource rds = new ReportDataSource();
                            rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                            rds.Value = people;
                            this.reportViewer1.LocalReport.DataSources.Add(rds);
                            var selpara = new List<SVC.Param>();
                            selpara.Add((clientproxy.GetAllParamétre()));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", LISTORDOOO));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", peoplemededcin));
                            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", EnteteOrdoonancePatient));
                            reportViewer1.LocalReport.EnableExternalImages = true;
                            ReportParameter paramLogo = new ReportParameter();
                            paramLogo.Name = "ImagePath";
                            String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                            paramLogo.Values.Add(@"file:///" + photolocation);
                            reportViewer1.LocalReport.SetParameters(paramLogo);
                            reportViewer1.RefreshReport();









                            /*****************************************/
                            InfGénéral.DataContext = SelectMedecinListe;
                            InfEnteteOrdonnace.DataContext = SelectMedecinListe;
                            NewCréation = true;
                            tabitemMembership.IsEnabled = false;
                            txtSamediD.Enabled = false;
                            txtSamediF.Enabled = false;
                            txtDimancheD.Enabled = false;
                            txtDimancheF.Enabled = false;
                            txtLundiD.Enabled = false;
                            txtLundiF.Enabled = false;
                            txtMardiD.Enabled = false;
                            txtMardiF.Enabled = false;
                            txtMercrediD.Enabled = false;
                            txtMercrediF.Enabled = false;
                            txtJeudiD.Enabled = false;
                            txtJeudiF.Enabled = false;
                            txtVendrediD.Enabled = false;
                            txtVendrediF.Enabled = false;
                            tabitemOrdonnance.IsEnabled = false;

                            btnValider.IsEnabled = false;
                            btnValider.Opacity = 0.2;

                            title = this.Title;
                         //   titlebrush = this.WindowTitleBrush;
                            TypeCombo.SelectedIndex = 0;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void btnAnnulerSession_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }else
                { 
                if (txtUserName.Text.Trim() != "" && interfacechangement==1 )
                {

                    var query = from c in clientproxy.GetAllMembership()
                                select new
                                {
                                    c.UserName
                                };

                    var resultsList = query.ToList();
                    var disponibleSession = resultsList.Where(list0 => list0.UserName.Trim().ToUpper() == txtUserName.Text.Trim().ToUpper()).FirstOrDefault();

                    if (disponibleSession != null)
                    {
                        this.Title = "Cette Session appartient déja a la base de donnée";
                     //   this.WindowTitleBrush = Brushes.Red;
                                MESSAGELABELSession.Content = "Cette Session appartient déja a la base de donnée";
                                MESSAGELABELSession.Foreground = Brushes.Red;
                                btnValiderSession.IsEnabled = false;
                                btnValiderSession.Opacity = 0.2;


                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;
                    }
                    else
                    {
                        if (txtUserName.Text.Trim() != "")
                        {
                            this.Title = title;
                        //    this.WindowTitleBrush = titlebrush;
                                    if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasseRe.Text.Trim()!="")
                                    {
                                        btnValiderSession.IsEnabled = true;
                                        btnValiderSession.Opacity = 1;
                                        MESSAGELABELSession.Content = "";
                                    }
                           txtMotDePasse.IsEnabled = true;
                            txtMotDePasseRe.IsEnabled = true;
                        }
                        else
                        {
                                    txtMotDePasse.IsEnabled = false;
                                    txtMotDePasseRe.IsEnabled = false;
                                    btnValiderSession.Opacity = 0.2;
                                    btnValiderSession.IsEnabled = false;
                        }
                    }
                }
                else
                {
                            if (txtUserName.Text.Trim() != "" && interfacechangement == 2)
                            {
                                txtMotDePasse.IsEnabled = true;
                                txtMotDePasseRe.IsEnabled = true;
                                if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasseRe.Text.Trim()!="")
                                {
                                    MESSAGELABELSession.Content = "";
                                    btnValiderSession.IsEnabled = true;
                                    btnValiderSession.Opacity =1;
                                }
                                else
                                {
                                  
                                    btnValiderSession.IsEnabled = false;
                                    btnValiderSession.Opacity = 0.2;
                                    //MESSAGELABELSession.Content = "dsfdsfdsfdsfds";
                                }
                            }else
                            {
                                if (txtUserName.Text.Trim() == "" )
                                {
                                    txtMotDePasse.IsEnabled = false;
                                    txtMotDePasseRe.IsEnabled = false;
                                    btnValiderSession.Opacity = 0.2;
                                    btnValiderSession.IsEnabled = false;
                                }
                 
                            }
                         
                 }
                }
            }
            }catch(Exception ex)
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                HandleProxy();


         
            }
        }

        private void txtMotDePasseRe_TextChanged(object sender, TextChangedEventArgs e)
        { try
            {
                if (clientproxy != null)
                {
                    if (clientproxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        if (txtMotDePasse.Text.Trim() != "" && txtUserName.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && interfacechangement==1)
                        {
                            if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim())
                            {
                                if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && NewCréation==true)

                                {
                                    this.Title = title;
                             //       this.WindowTitleBrush = titlebrush;
                                    MESSAGELABELSession.Content = "";
                                    btnValiderSession.IsEnabled = true;
                                    btnValiderSession.Opacity = 1;
                                    txtNom.IsEnabled = true;
                                    txtPrenom.IsEnabled = true;
                                    txtUserName.IsEnabled = true;

                                }
                                else
                                {
                                    if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && NewCréation==true)

                                    {
                                        this.Title = title;
                               //         this.WindowTitleBrush = titlebrush;
                                        MESSAGELABELSession.Content = "";
                                        btnValiderSession.IsEnabled = true;
                                        btnValiderSession.Opacity = 1;


                                    }
                                    else
                                    {
                                        btnValiderSession.IsEnabled = false;
                                        btnValiderSession.Opacity = 0.2;
                                    }
                                }
                            }
                            else
                            {
                                this.Title = title;
                              //  this.WindowTitleBrush = titlebrush;
                                this.Title = "Veuillez Confirmez le mot de passe";
                                MESSAGELABELSession.Content = "Veuillez Confirmez le mot de passe";
                                MESSAGELABELSession.Foreground = Brushes.Red;
                            //    this.WindowTitleBrush = Brushes.Red;

                                btnValiderSession.IsEnabled = false;
                                btnValiderSession.Opacity = 0.2;
                                txtNom.IsEnabled = false;
                                txtPrenom.IsEnabled = false;
                                txtUserName.IsEnabled = false;
                            }

                        }
                        else
                        {
                            if (txtMotDePasse.Text.Trim() != "" && txtUserName.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && interfacechangement == 2)
                            {
                              
                                    btnValiderSession.IsEnabled = true;
                                    btnValiderSession.Opacity = 1;
                               
                            }
                            else
                            {
                                btnValiderSession.IsEnabled = false;
                                btnValiderSession.Opacity = 0.2;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                HandleProxy();
            }
        }
        private void tabitemMembership_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (clientproxy != null)
                {
                    if (clientproxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        if (interfacechangement==2)
                        {
                           
                               selectmedecinforsession = SelectMedecinListe;
                            InfGénéral.DataContext = SelectMedecinListe;
                            var query = (from c in clientproxy.GetAllMembership()
                                         where c.UserName.Trim() == SelectMedecinListe.UserName.Trim()
                                         select c).FirstOrDefault();
                            if (query != null)
                            {
                                txtUserName.IsEnabled = false;
                                selectmedecinsession = query;
                                csessionusermedein.Content = "Modifier la session du medecin";
                                tabitemMembership.DataContext = selectmedecinsession;
                            }
                           
                        }
                       

                    }
                }
            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message,GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                HandleProxy();
            }
        }

    private void txtNom_TextChanged(object sender, TextChangedEventArgs e)
        {
         try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    if (txtPrenom.Text.Trim() != "" && txtNom.Text.Trim() != ""  && interfacechangement ==1)
                    {
                        var query = from c in clientproxy.GetAllMedecin()
                                    select new { c.Nom, c.Prénom, };

                        var results = query.ToList();
                        var disponible = results.Where(list1 => list1.Nom.ToUpper() == txtNom.Text.Trim().ToUpper() && list1.Prénom.ToUpper() == txtPrenom.Text.Trim().ToUpper()).FirstOrDefault();
                        if (disponible != null)
                        {
                            this.Title = "Ce Medecin appartient déja a la base de donnée";
                            //this.WindowTitleBrush = Brushes.Red;

                            btnValider.IsEnabled = false;
                            btnValider.Opacity = 0.2;
                        }
                        else
                        {
                            this.Title = "Medecin " + txtNom.Text.Trim().ToUpper() + "  " + txtPrenom.Text.ToUpper() + " est disponible pour création";
                         //   this.WindowTitleBrush = Brushes.Green;
                            btnValider.IsEnabled = true;
                            btnValider.Opacity = 1;

                        }
                    }else
                   {
                            if (txtPrenom.Text.Trim() != "" && txtNom.Text.Trim() != "" && interfacechangement == 2)
                            {
                                btnValider.IsEnabled = true;
                                btnValider.Opacity = 1;
                            }
                            else
                            {
                                btnValider.IsEnabled = false;
                                btnValider.Opacity = 0.2;
                            }
                        }
                }
                }
            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                HandleProxy();
            }
        }
        private void btnValiderSession_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            //SVC.Membership NewMembership;
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    try
                    { 

                             selectmedecinsession.Nom = txtNom.Text.Trim();
                             selectmedecinsession.Prénom = txtPrenom.Text.Trim();
                             selectmedecinsession.MotDePasse = txtMotDePasse.Text.Trim();
                             selectmedecinsession.Actif = chACTIF.IsChecked;

                        selectmedecinsession.ModuleAdministrateur = chadmin.IsChecked;
                             selectmedecinsession.CréationAdministrateur = chcreeradmin.IsChecked;
                             selectmedecinsession.ModificationAdministrateur = chmodifadmin.IsChecked;
                             selectmedecinsession.SuppressionAdministrateur = chsuppadmin.IsChecked;
                             selectmedecinsession.ImpressionAdministrateur = chimpradmin.IsChecked;

                   

                        selectmedecinsession.ModuleRendezVous = chaRendz.IsChecked;
                             selectmedecinsession.CréationRendezVous = chcreerRendz.IsChecked;
                             selectmedecinsession.ModificationRendezVous = chmodifRendz.IsChecked;
                             selectmedecinsession.SuppressionRendezVous = chsuppRendz.IsChecked;
                             selectmedecinsession.ImpressionRendezVous = chimprRendz.IsChecked;

                        selectmedecinsession.ModulePatient = chaPatient.IsChecked;
                             selectmedecinsession.CréationPatient = chcreerPatient.IsChecked;
                             selectmedecinsession.ModificationPatient = chmodifPatient.IsChecked;
                             selectmedecinsession.SuppressionPatient = chsuppPatient.IsChecked;
                             selectmedecinsession.ImpressionPatient = chimprPatient.IsChecked;

                        selectmedecinsession.ModuleDossierPatient = chaDossierPatient.IsChecked;
                             selectmedecinsession.CréationDossierPatient = chcreerDossierPatient.IsChecked;
                             selectmedecinsession.ModificationDossierPatient = chmodifDossierPatient.IsChecked;
                             selectmedecinsession.SuppressionDossierPatient = chsuppDossierPatient.IsChecked;
                             selectmedecinsession.ImpressionDossierPatient = chimprDossierPatient.IsChecked;
                             selectmedecinsession.AccèsToutLesDossierPatient = chimprAccèsToutLesDossierPatient.IsChecked;

                             selectmedecinsession.ModuleSalleAttente = chaDossierSalleAttente.IsChecked;
                             selectmedecinsession.CréationSalleAttente = chcreerSalleAttente.IsChecked;
                             selectmedecinsession.ModificationSalleAttente = chmodifSalleAttente.IsChecked;
                             selectmedecinsession.SupressionSalleAttente = chsuppSalleAttente.IsChecked; 
                             selectmedecinsession.ImpressionSalleAttente = chimprSalleAttente.IsChecked;

                             selectmedecinsession.ModuleCaisse = chaDepense.IsChecked;
                             selectmedecinsession.CréationCaisse = chcreerDepense.IsChecked;
                             selectmedecinsession.ModificationCaisse = chmodifDepense.IsChecked;
                             selectmedecinsession.SuppréssionCaisse = chsuppDepense.IsChecked;
                             selectmedecinsession.ImressionCaisse = chimprDepense.IsChecked;

                             selectmedecinsession.ModuleAchat = chaAchat.IsChecked;
                             selectmedecinsession.CréationAchat = chcreerAchat.IsChecked;
                             selectmedecinsession.ModuleAchat = chmodifAchat.IsChecked;
                             selectmedecinsession.SupressionAchat = chsuppAchat.IsChecked;
                             selectmedecinsession.ImpressionAchat = chimprAchat.IsChecked;

                        selectmedecinsession.ModuleChat = chaModuleChat.IsChecked;
                            selectmedecinsession.EnvoiReceptMessage = chcreerEnvoiReceptMessage.IsChecked;
                            selectmedecinsession.EnvoiReceptMessage = chmodifEnvoiRécéptFichier.IsChecked;
                        selectmedecinsession.DiscussionPrivé = chsuppDiscussionPrivé.IsChecked;

                             selectmedecinsession.CréerLe = DateTime.Now;
                             selectmedecinsession.UserName = txtUserName.Text.Trim();

                        
                        if (interfacechangement==1)
                        {
                           
                                bool insertmember = false;
                                bool updatemedecin = false;
                                try
                            {
                                var query = (from c in clientproxy.GetAllMedecin()
                                             where c.Nom == selectmedecinsession.Nom && c.Prénom == selectmedecinsession.Prénom
                                             select c).FirstOrDefault();
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        clientproxy.InsertMembership(selectmedecinsession);
                                        insertmember = true;
                                      
                                        SelectMedecinListe = query;
                                        SelectMedecinListe.UserName = selectmedecinsession.UserName;
                                        clientproxy.UpdateMedecin(SelectMedecinListe);
                                        updatemedecin = true;
                                        btnValiderSession.IsEnabled = false;
                                         if(insertmember==true && updatemedecin==true)
                                         {
                                            ts.Complete();
                                            tabitemOrdonnance.IsEnabled = true;
                                            tabitemOrdonnance.IsSelected = true;
                                            tabitemMembership.IsEnabled = false;
                                            InfEnteteOrdonnace.DataContext = SelectMedecinListe;
                                         }
                                        else
                                         {
                                           MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OpérationéchouéeTransaction, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                         }
                                     }
                                }
                                catch (Exception ex)
                                 {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                 }


                    }
                        else
                        {
                            try
                            {
                                if (interfacechangement == 2)
                                {
                                    if (selectmedecinsession != null)
                                    {
                                        bool updatemedecin = false;
                                        bool updatemember = false;
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            SelectMedecinListe.UserName = selectmedecinsession.UserName;
                                            clientproxy.UpdateMedecin(SelectMedecinListe);
                                            updatemedecin = true;
                                            clientproxy.UpdateMembership(selectmedecinsession);
                                            updatemember = true;
                                            if (updatemember == true && updatemedecin == true)
                                            {
                                                ts.Complete();
                                            }
                                            else
                                            {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OpérationéchouéeTransaction, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                            }
                                    }
                                    }
                                    else
                                    {

                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OpérationéchouéeTransaction, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }

                            }
                            }
                            catch (Exception ex)
                            {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


                        }
                    }

                    }
                    catch (Exception ex)
                    {



                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtMotDePasse_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && interfacechangement==1 )
                        {
                            if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim())
                            {
                                this.Title = title;
                               // this.WindowTitleBrush = titlebrush;
                                btnValiderSession.IsEnabled = true;
                                btnValiderSession.Opacity = 1;
                            }
                            else
                            {
                                btnValiderSession.IsEnabled = false;
                                btnValiderSession.Opacity = 0.2;
                            }
                    }
                    else
                    {
                        if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && interfacechangement==2)
                            {
                                if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim()!="")
                                {
                                    this.Title = title;
                                  //  this.WindowTitleBrush = titlebrush;
                                    btnValiderSession.IsEnabled = true;
                                    btnValiderSession.Opacity = 1;
                                }
                                else
                                {
                                    btnValiderSession.IsEnabled = false;
                                    btnValiderSession.Opacity = 0.2;
                                }
                            }else
                            {
                                btnValiderSession.IsEnabled = false;
                                btnValiderSession.Opacity = 0.2;
                            }
                      
                    }
                }
                }
            }
            catch
            {
                this.Title = "Une erreur de reseau veuillez vous déconnecter et vous reconnecter de nouveau";
             //   this.WindowTitleBrush = Brushes.Red;
                HandleProxy();
            }
        }

        private void btnValiderOrdonnace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                        clientproxy.UpdateMedecin(SelectMedecinListe);
                        ts.Complete();
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void chSamedi_Checked(object sender, RoutedEventArgs e)
        {
          try
            { 
                samedi = true;
                txtSamediD.Enabled = true;
                txtSamediF.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chSamedi_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            { 
            samedi = false;
            txtSamediD.Enabled = false;
            txtSamediF.Enabled = false;
                txtSamediD.Text = "00:00";
                txtSamediF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chDimanche_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            { 
            txtDimancheD.Enabled = false;
            txtDimancheF.Enabled = false;
            dimanche = false;
                txtDimancheD.Text = "00:00";
                txtDimancheF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chDimanche_Checked(object sender, RoutedEventArgs e)
        {
            try
            { 
                dimanche = true;
                txtDimancheD.Enabled = true;
                txtDimancheF.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chLundi_Checked(object sender, RoutedEventArgs e)
        {
          
            try
            { 
                txtLundiD.Enabled = true;
                txtLundiF.Enabled = true;
                lundi = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chLundi_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            { 
            lundi = false;
            txtLundiD.Enabled = false;
            txtLundiF.Enabled = false;
                txtLundiD.Text = "00:00";
                txtLundiF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chMardi_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            { 
            mardi = false;
            txtMardiD.Enabled = false;
            txtMardiF.Enabled = false;
                txtMardiD.Text = "00:00";
                txtMardiF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chMardi_Checked(object sender, RoutedEventArgs e)
        {
           try
            { 
                mardi = true;
                txtMardiD.Enabled = true;
                txtMardiF.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chMercredi_Checked(object sender, RoutedEventArgs e)
        {
            try
            { 
            mercredi = true;
            txtMercrediD.Enabled = true;
            txtMercrediF.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chMercredi_Unchecked(object sender, RoutedEventArgs e)
        {
           try
            { 
                mercredi = false;
                txtMercrediD.Enabled = false;
                txtMercrediF.Enabled = false;
                txtMercrediD.Text = "00:00";
                txtMercrediF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chJeudi_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            { 
            jeudi = false;
            txtJeudiD.Enabled = false;
            txtJeudiF.Enabled = false;
                txtJeudiD.Text = "00:00";
                txtJeudiF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chJeudi_Checked(object sender, RoutedEventArgs e)
        {
            try
            { 
                jeudi = true;
                txtJeudiD.Enabled = true;
                txtJeudiF.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chVendredi_Checked(object sender, RoutedEventArgs e)
        {
            try
            { 
            vendredi = true;
            txtVendrediD.Enabled = true;
            txtVendrediF.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chVendredi_Unchecked(object sender, RoutedEventArgs e)
        {
       try
            { 
            vendredi = false;
            txtVendrediD.Enabled = false;
            txtVendrediF.Enabled = false;
                txtVendrediD.Text = "00:00";
                txtVendrediF.Text = "00:00";
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMedecin(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMedecin(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void TypeCombo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            { 
            if (e.Key == System.Windows.Input.Key.Space)
            {

                TypeCombo.IsDropDownOpen = true;
            }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void HandleProxy()
        {
            if (clientproxy != null)
            {
                switch (this.clientproxy.State)
                {
                    case CommunicationState.Closed:
                        clientproxy.Abort();
                        clientproxy = null;
                        this.Close();
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                      clientproxy.Abort();
                       clientproxy = null;
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



        void callbackrecu_Refresh(object source, CallbackEventInsertSpécialité e)
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(List<SVC.Spécialité> listmembership)
        {
            try
            { 
            TxtSpecialité.ItemsSource = listmembership;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

     
  
    
        private void datepickerexclusion_CalendarClosed_1(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    try
                    {
                       // object item = exclusionDatagrid.SelectedItem;
                        //int m = int.Parse((exclusionDatagrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);

                        DatePicker dp = (DatePicker)sender;
                     //   clientproxy.UpdateExclusionDay(m, Convert.ToDateTime(dp.SelectedDate));
                        MessageBox.Show("enregistrement modifié avec succès");
                    }
                    catch (Exception ex)
                    {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            return;

                    }
                }
            }
            }
            catch
            {
                this.Title = "Une erreur de reseau veuillez vous déconnecter et vous reconnecter de nouveau";
              //  this.WindowTitleBrush = Brushes.Red;
                HandleProxy();
                this.IsEnabled = false;

            }
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void txtPrenom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    if (txtNom.Text != "" && txtPrenom.Text != "" &&  interfacechangement==1)
                    {

                        var query = from c in clientproxy.GetAllMedecin()
                                    select new { c.Nom, c.Prénom };

                        var results = query.ToList();
                        var disponible = results.Where(list1 => list1.Nom.ToUpper() == txtNom.Text.Trim().ToUpper() && list1.Prénom.ToUpper() == txtPrenom.Text.Trim().ToUpper()).FirstOrDefault();
                        if (disponible != null)
                        {
                            this.Title = "Ce Medecin appartient déja a la base de donnée";
            //                this.WindowTitleBrush = Brushes.Red;
                                MESSAGELABEL.Content= "Ce Medecin appartient déja a la base de donnée";
                                MESSAGELABEL.Foreground= Brushes.Red;
                                btnValider.IsEnabled = false;
                            btnValider.Opacity = 0.2;

                        }
                        else
                        {
                            this.Title = "Medecin " + txtNom.Text.Trim().ToUpper() + "  " + txtPrenom.Text.ToUpper() + " est disponible pour création";
                                MESSAGELABEL.Content = "Medecin " + txtNom.Text.Trim().ToUpper() + "  " + txtPrenom.Text.ToUpper() + " est disponible pour création";
                                MESSAGELABEL.Foreground = Brushes.Green;
                      //          this.WindowTitleBrush = Brushes.Green;
                            btnValider.IsEnabled = true;
                            btnValider.Opacity = 1;

                        }
                    }else
                        {
                            if (txtPrenom.Text.Trim() != "" && txtNom.Text.Trim() != "" && interfacechangement == 2)
                            {
                                btnValider.IsEnabled = true;
                                btnValider.Opacity = 1;
                            }
                            else
                            {
                                btnValider.IsEnabled = false;
                                btnValider.Opacity = 0.2;
                            }
                        }
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                HandleProxy();
           

            }
        }
     
        public string NomMedecin, PrénomMedecin,TypeMedecin, AdressMedecin,TelMedecin,EmailMedecin;

        private void chDimanche_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chDimanche.IsChecked == false)
            {

                txtDimancheD.Enabled = false;
                txtDimancheF.Enabled = false;
                dimanche = false;
            }
            else
            {
                dimanche = true;
                txtDimancheD.Enabled = true;
                txtDimancheF.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chLundi_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chLundi.IsChecked == false)
            {
                lundi = false;
                txtLundiD.Enabled = false;
                txtLundiF.Enabled = false;
            }
            else
            {

                txtLundiD.Enabled = true;
                txtLundiF.Enabled = true;
                lundi = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chMardi_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chMardi.IsChecked == false)
            {
                mardi = false;
                txtMardiD.Enabled = false;
                txtMardiF.Enabled = false;
            }
            else
            {
                mardi = true;
                txtMardiD.Enabled = true;
                txtMardiF.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chMercredi_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chMercredi.IsChecked == false)
            {
                mercredi = false;
                txtMercrediD.Enabled = false;
                txtMercrediF.Enabled = false;
            }
            else
            {
                mercredi = true;
                txtMercrediD.Enabled = true;
                txtMercrediF.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chJeudi_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chJeudi.IsChecked == false)
            {
                jeudi = false;
                txtJeudiD.Enabled = false;
                txtJeudiF.Enabled = false;
            }
            else
            {
                jeudi = true;
                txtJeudiD.Enabled = true;
                txtJeudiF.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chVendredi_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chVendredi.IsChecked == false)
            {
                vendredi = false;
                txtVendrediD.Enabled = false;
                txtVendrediF.Enabled = false;
            }
            else
            {
                vendredi = true;
                txtVendrediD.Enabled = true;
                txtVendrediF.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void TypeCombo_DropDownClosed(object sender, EventArgs e)
        {
            try
            { 

            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    if (TypeCombo.SelectionBoxItem.ToString() != "Permanent")
                    {
                        GridPermanent.Visibility = Visibility.Collapsed;
                        GridPassager.Visibility = Visibility.Visible;
                       // gridExc.Visibility = Visibility.Collapsed;
                        labelPrg.Text = "Programme du Mois";
                        labelPrg.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        if (TypeCombo.SelectionBoxItem.ToString() != "Passager")
                        {
                            GridPermanent.Visibility = Visibility.Visible;
                            GridPassager.Visibility = Visibility.Collapsed;
                     //       gridExc.Visibility = Visibility.Visible;
                            labelPrg.Text = "Programme de la semaine";
                            labelPrg.Visibility = Visibility.Visible;
                        }
                        else
                        {

                            GridPermanent.Visibility = Visibility.Collapsed;
                            GridPassager.Visibility = Visibility.Collapsed;
                         //   gridExc.Visibility = Visibility.Collapsed;
                            labelPrg.Visibility = Visibility.Collapsed;
                            labelPrg.Visibility = Visibility.Collapsed;



                        }
                    }
                }
                }
            }
            catch
            {
                this.Title = "Une erreur de reseau veuillez vous déconnecter et vous reconnecter de nouveau";
             //   this.WindowTitleBrush = Brushes.Red;
                HandleProxy();
                this.IsEnabled = false;

            }
        }

        private void chSamedi_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chSamedi.IsChecked == false)
            {
                samedi = false;
                txtSamediD.Enabled = false;
                txtSamediF.Enabled = false;
            }
            else
            {
                samedi = true;
                txtSamediD.Enabled = true;
                txtSamediF.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

  
      

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    this.Close();
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (clientproxy != null)
            {
                if (clientproxy.State == CommunicationState.Faulted)
                {
                    HandleProxy();
                }
                else
                {
                    try
                    {
                     

                      //  SVC.Medecin NewMedecin;
                      
                        NomMedecin = txtNom.Text.Trim();
                        PrénomMedecin = txtPrenom.Text.Trim();
                        if (TypeCombo.SelectedValue != null)
                        {



                            TypeMedecin = TypeCombo.SelectionBoxItem.ToString();
                        }
                        else
                        {
                            TypeMedecin = "Permanent";
                        }
                        AdressMedecin = txtAdresse.Text;
                        TelMedecin = txtTéléphone.Text;
                        EmailMedecin = txtemail.Text;
                        if (txtTempsDevisite.Text != null)
                        {

                            TEMPSVISITE = TimeSpan.Parse(txtTempsDevisite.Text);
                        }
                        else
                        {
                            // txtTempsDevisite.Text = "00:00:00";
                            TEMPSVISITE = TimeSpan.Parse(txtTempsDevisite.Text);

                        }
                        if (txtSamediD.Text != null)
                        {
                            SAMEDID = TimeSpan.Parse(txtSamediD.Text);
                        }
                        else
                        {
                            //   txtSamediD.Text = "00:00:00";
                            SAMEDID = TimeSpan.Parse(txtSamediD.Text);
                        }
                        if (txtSamediF.Text != null)
                        {
                            SAMEDIF = TimeSpan.Parse(txtSamediF.Text);
                        }
                        else
                        {
                            //  txtSamediF.Text = "00:00:00";
                            SAMEDIF = TimeSpan.Parse((txtSamediF.Text));
                        }
                        if (txtDimancheD.Text != null)
                        {
                            DIMANCHED = TimeSpan.Parse(txtDimancheD.Text);
                        }
                        else
                        {
                            //  txtDimancheD.Text = "00:00:00";
                            DIMANCHED = TimeSpan.Parse(txtDimancheD.Text);
                        }
                        if (txtDimancheF.Text != null)
                        {
                            DIMANCHEF = TimeSpan.Parse(txtDimancheF.Text);
                        }
                        else
                        {
                            //  txtDimancheF.Text = "00:00:00";

                            DIMANCHEF = TimeSpan.Parse(txtDimancheF.Text);
                        }
                        if (txtLundiD.Text != null)
                        {
                            LUNDID = TimeSpan.Parse(txtLundiD.Text);
                        }
                        else
                        {
                            // txtLundiD.Text = "00:00:00";
                            LUNDID = TimeSpan.Parse(txtLundiD.Text);
                        }
                        if (txtLundiF.Text != null)
                        {
                            LUNDIF = TimeSpan.Parse(txtLundiF.Text);
                        }
                        else
                        {
                            //  txtLundiF.Text = "00:00:00";
                            LUNDIF = TimeSpan.Parse(txtLundiF.Text);
                        }
                        if (txtMardiD.Text != null)
                        {
                            MARDID = TimeSpan.Parse(txtMardiD.Text);
                        }
                        else
                        {
                            // txtMardiD.Text = "00:00:00";
                            MARDID = TimeSpan.Parse(txtMardiD.Text);
                        }
                        if (txtMardiF.Text != null)
                        {
                            MARDIF = TimeSpan.Parse(txtMardiF.Text);
                        }
                        else
                        {
                            // txtMardiF.Text = "00:00:00";
                            MARDIF = TimeSpan.Parse(txtMardiF.Text);
                        }
                        if (txtMercrediD.Text != null)
                        {
                            MERCREDID = TimeSpan.Parse(txtMercrediD.Text);
                        }
                        else
                        {
                            // txtMercrediD.Text = "00:00:00";
                            MERCREDID = TimeSpan.Parse(txtMercrediD.Text);
                        }

                        if (txtMercrediF.Text != null)
                        {
                            MERCREDIF = TimeSpan.Parse(txtMercrediF.Text);
                        }
                        else
                        {
                            //  txtMercrediF.Text= "00:00:00";
                            MERCREDIF = TimeSpan.Parse(txtMercrediF.Text);
                        }
                        if (txtJeudiD.Text != null)
                        {
                            JEUDID = TimeSpan.Parse(txtJeudiD.Text);
                        }
                        else
                        {
                            //  txtJeudiD.Text="00:00:00";
                            JEUDID = TimeSpan.Parse(txtJeudiD.Text);
                        }
                        if (txtJeudiF.Text != null)
                        {
                            JEUDIF = TimeSpan.Parse(txtJeudiF.Text);
                        }
                        else
                        {
                            //  txtJeudiF.Text = "00:00:00";
                            JEUDIF = TimeSpan.Parse(txtJeudiF.Text);
                        }
                        if (txtVendrediD.Text != null)
                        {
                            VENDREDID = TimeSpan.Parse(txtVendrediD.Text);
                        }
                        else
                        {
                            //  txtVendrediD.Text= "00:00:00";
                            VENDREDID = TimeSpan.Parse(txtVendrediD.Text);
                        }
                        if (txtVendrediF.Text != null)
                        {
                            VENDREDIF = TimeSpan.Parse(txtVendrediF.Text);
                        }
                        else
                        {
                            //  txtVendrediF.Text="00:00:00";
                            VENDREDIF = TimeSpan.Parse(txtVendrediF.Text);
                        }
                        string specselecte;
                        if (TxtSpecialité.SelectedItem != null)
                        {
                            specselecte = TxtSpecialité.SelectedValue.ToString();
                        }
                        else
                        {
                            specselecte = "";
                        }


                        SelectMedecinListe.Nom = NomMedecin;
                        SelectMedecinListe.Prénom = PrénomMedecin;
                        SelectMedecinListe.Type = TypeMedecin;
                        SelectMedecinListe.Adresse = AdressMedecin;
                        SelectMedecinListe.Téléphone = TelMedecin;
                        SelectMedecinListe.Email = EmailMedecin;
                        SelectMedecinListe.Samedi = samedi;
                        SelectMedecinListe.Dimanche = dimanche;
                        SelectMedecinListe.Lundi = lundi;
                        SelectMedecinListe.Mardi = mardi;
                        SelectMedecinListe.Mercredi = mercredi;
                        SelectMedecinListe.Jeudi = jeudi;
                        SelectMedecinListe.Vendredi = vendredi;
                        SelectMedecinListe.TempsVisite = TEMPSVISITE;
                        SelectMedecinListe.SamediD = SAMEDID;
                        SelectMedecinListe.SamediF = SAMEDIF;
                        SelectMedecinListe.DimancheD = DIMANCHED;
                        SelectMedecinListe.DimancheF = DIMANCHEF;
                        SelectMedecinListe.LundiD = LUNDID;
                        SelectMedecinListe.LundiF = LUNDIF;
                        SelectMedecinListe.MardiD = MARDID;
                        SelectMedecinListe.MardiF = MARDIF;
                        SelectMedecinListe.MercrediD = MERCREDID;
                        SelectMedecinListe.MercrediF = MERCREDIF;
                        SelectMedecinListe.JeudiD = JEUDID;
                        SelectMedecinListe.JeudiF = JEUDIF;
                        SelectMedecinListe.VendrediD = VENDREDID;
                        SelectMedecinListe.VendrediF = VENDREDIF;
                        SelectMedecinListe.SpécialitéMedecin = specselecte;

                        if (NewCréation==true)
                        {

                            try
                            {
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    clientproxy.InsertMedecin(SelectMedecinListe);

                                    selectmedecinforsession = SelectMedecinListe;
                                    ts.Complete();
                                }
               
                             selectmedecinsession = new Membership();
                                InfGénéral1.DataContext = selectmedecinsession;
                               /*   SelectMedecinListe=new Medecin();

                                    var query = (from c in clientproxy.GetAllMedecin()
                                            where c.Nom == SelectMedecinListe.Nom && c.Prénom== SelectMedecinListe.Prénom
                                                select c).FirstOrDefault();
                                    SelectMedecinListe = query;
                               InfGénéral.DataContext=SelectMedecinListe;*/


                                btnValider.IsEnabled = false;
                                tabitemMembership.IsSelected = true;
                                tabitemMembership.IsEnabled = true;
                                tabitemOrdonnance.IsEnabled = false;
                                INFTABITEM.IsEnabled = false;
                                //  this.Title = "Opération réussie ! ";
                                // this.WindowTitleBrush = Brushes.Green;
                                /*   this.Title = selectmedecinforsession.Nom;
                                   this.WindowTitleBrush = Brushes.Green;*/
                                btnValiderSession.IsEnabled = false;
                                btnValiderSession.Opacity = 0.2;
                            }
                            catch (Exception ex)
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        else
                        {
                            try
                            {
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    SelectMedecinListe.TempsVisite = TimeSpan.Parse(txtTempsDevisite.Text);
                                    SelectMedecinListe.SamediD = TimeSpan.Parse(txtSamediD.Text);
                                    SelectMedecinListe.SamediF = TimeSpan.Parse(txtSamediF.Text);
                                    SelectMedecinListe.DimancheD = TimeSpan.Parse(txtDimancheD.Text);
                                    SelectMedecinListe.DimancheF = TimeSpan.Parse(txtDimancheF.Text);
                                    SelectMedecinListe.LundiD = TimeSpan.Parse(txtLundiD.Text);
                                    SelectMedecinListe.LundiF = TimeSpan.Parse(txtLundiF.Text);
                                    SelectMedecinListe.MardiD = TimeSpan.Parse(txtMardiD.Text);
                                    SelectMedecinListe.MardiF = TimeSpan.Parse(txtMardiF.Text);
                                    SelectMedecinListe.MercrediD = TimeSpan.Parse(txtMercrediD.Text);
                                    SelectMedecinListe.MercrediF = TimeSpan.Parse(txtMercrediF.Text);
                                    SelectMedecinListe.JeudiD = TimeSpan.Parse(txtJeudiD.Text);
                                    SelectMedecinListe.JeudiF = TimeSpan.Parse(txtJeudiF.Text);
                                    SelectMedecinListe.VendrediD = TimeSpan.Parse(txtVendrediD.Text);
                                    SelectMedecinListe.VendrediF = TimeSpan.Parse(txtVendrediF.Text);
                                    SelectMedecinListe.Type = TypeCombo.SelectionBoxItem.ToString();
                                    string specselecte1;
                                    if (TxtSpecialité.SelectedItem != null)
                                    {
                                        specselecte1 = TxtSpecialité.SelectedValue.ToString();
                                    }
                                    else
                                    {
                                        specselecte1 = "";
                                    }
                                    SelectMedecinListe.SpécialitéMedecin = specselecte1;

                                    clientproxy.UpdateMedecin(SelectMedecinListe);
                                    ts.Complete();
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }


                            }
                            catch (Exception ex)
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }





                        }



                    }
                    catch (Exception ex)
                    {


                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
