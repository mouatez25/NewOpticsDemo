using DevExpress.Xpf.Core;
using GestionClinique.SVC;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for NewPatient.xaml
    /// </summary>
    public partial class NewPatient : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerNewPatient();
        SVC.Membership member;
        SVC.Patient patient;
        bool Création = false;
        OpenFileDialog op=new OpenFileDialog();
        string serverfilepath, filepath;

        public NewPatient(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership memberrecu, SVC.Patient patientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                proxy.InnerDuplexChannel.Opened += new EventHandler(InnerDuplexChannel_Opened);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                member = memberrecu;
                if (patientrecu == null)
                {
                    Création = true;
                    btnCreer.IsEnabled = false;
                    txtMedecin.ItemsSource = proxy.GetAllMedecin();
                }
                else
                {
                    patient = patientrecu;
                    PatientGrid.DataContext = patient;



                    List<SVC.Medecin> testmedecin = proxy.GetAllMedecin();
                    txtMedecin.ItemsSource = testmedecin;
                    if (patientrecu.SuiviParCode != null/* && patientrecu.SuiviParPrénom != ""*/)
                    {
                        List<SVC.Medecin> tte = testmedecin.Where(n => n.Id==patientrecu.SuiviParCode).ToList();
                        txtMedecin.SelectedItem = tte.First();
                    }




                    if (patientrecu.CheminPhoto.ToString() != "")
                    {
                      
                        if (proxy.DownloadDocumentIsHere(patientrecu.CheminPhoto.ToString())==true)
                        {
                            imgPhoto.Source = LoadImage(proxy.DownloadDocument(patientrecu.CheminPhoto.ToString()));
                            Création = false;
                            patient = patientrecu;
                            op.FileName = patientrecu.CheminPhoto;
                            btnCreer.IsEnabled = true;
                            btnCreer.Content = "Modifier";
                        }else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention la photo du patient n'existe plus dans le serveur", "Medicus", MessageBoxButton.OK, MessageBoxImage.Error);
                            op.FileName = "";
                            Création = false;
                            patient = patientrecu;
                            btnCreer.IsEnabled = true;
                            btnCreer.Content = "Modifier";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewPatient(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewPatient(HandleProxy));
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
        public void rempliridsivisite(SVC.Patient pa)
        {
            try { 
            if (txtCrédit.Text != "")
            {
                var found = (proxy.GetAllPatient()).Find(n => n.Nom.Trim() == pa.Nom.Trim() && n.Prénom.Trim() == pa.Prénom.Trim());
                /***********************************************/
                if (found.Solde != 0)
                {
                    bool interfaceVisite = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {

                        SVC.Visite cas = new SVC.Visite
                        {
                            Motif = "Ancien Solde",
                            Interrogatoire = "Ancien Solde",
                            Examen = "Ancien Solde",
                            Conclusions = "Ancien Solde",
                            Date = DateTime.Now,
                            Datetime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                            CasTraite = "Ancien Solde",
                            NuméroRendezVous = "Ancien Solde",
                            UserName = member.UserName,
                            NomPatient = found.Nom,
                            PrénomPatient = found.Prénom,
                            Montant = found.Solde,
                            Versement = 0,
                            Reste = found.Solde,
                            Soldé = false,
                            //  CodePatient = 0,
                            CodePatient = found.Id,
                            IdCas = 0,
                            IdMotif = 0,
                            ModeFacturation = 0,
                            //   cle = "Ancien Solde" + 0,
                            cle = "Ancien Solde" + found.Id,
                        };

                        cas.VisiteParNom = member.UserName + " " + "User";
                        cas.VisiteParPrénom = member.Prénom + " " + "User";
                        cas.CodeMedecin = 1987;

                        proxy.InsertVisite(cas);
                        interfaceVisite = true;
                        if (interfaceVisite == true)
                        {
                            ts.Complete();
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try { 
                
            string nommedecin;
            string prénommedecin;
            int codemedecin;
            SVC.Patient pa;
            if (Création == true && member.CréationPatient == true && txtMedecin.SelectedItem != null && txtDate.SelectedDate != null && SexeCombo.SelectedValuePath != null && EtatCivicCombo.SelectedValuePath != null)
            {
                try {
                 

                    SVC.Medecin SelectMedecin = txtMedecin.SelectedItem as SVC.Medecin;
                    if (SelectMedecin != null)
                    {
                        nommedecin = SelectMedecin.Nom;
                        prénommedecin = SelectMedecin.Prénom;
                        codemedecin = SelectMedecin.Id;
                    }
                    else
                    {
                        nommedecin = "";
                        prénommedecin ="";
                        codemedecin = 0;
                    }

                    /*****************transfert***********************/
                    serverfilepath = op.FileName;

                    filepath = "";
                    if (serverfilepath != "")
                    {

                        filepath = op.FileName;

                        serverfilepath = @"Patient\PhotoPatient\" + (txtNom.Text.Trim() + txtPrenom.Text.Trim()) + ".png";
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
                        pa = new SVC.Patient
                        {
                            Nom = txtNom.Text.Trim(),
                            Prénom = txtPrenom.Text.Trim(),
                            Adresse = txtAdresse.Text.Trim(),
                            Téléphone = txtTelfixe.Text.Trim(),
                            EtatCiv = EtatCivicCombo.SelectionBoxItem.ToString(),
                            Sexe = SexeCombo.SelectionBoxItem.ToString(),
                            DateDeNaissance = txtDate.SelectedDate,
                            NuméroSS = txtSS.Text.Trim(),
                            Assurance = txtAssurance.Text.Trim(),
                            Fonction = txtFonction.Text.Trim(),
                            Mobile = txtTelPerso.Text.Trim(),
                            Remarques = txtRemarque.Text.Trim(),
                            Email = txtEmail.Text.Trim(),
                            SuiviParNom = nommedecin,
                            SuiviParPrénom = prénommedecin,
                            CheminPhoto = serverfilepath,
                            cle = txtNom.Text.Trim() + txtPrenom.Text.Trim() + DateTime.Now,
                            SuiviParCode = codemedecin,
                         GroupeSanguin=GroupeTXT.Text.Trim(),
                    };
                        if (DossierTXT.Text != "")
                        {
                            pa.Dossier = Convert.ToInt16 (DossierTXT.Text);


                            /********************************************************/
                        }
                        else
                        {
                            pa.Dossier = 0;
                        }
                        if (txtRéf.Text != "")
                        {
                            pa.Ref = Convert.ToInt16(txtRéf.Text);


                            /********************************************************/
                        }
                        else
                        {
                            pa.Ref = 0;
                        }
                        if (txtCrédit.Text != "")
                    {
                        pa.Solde = Convert.ToDecimal(txtCrédit.Text);


                        /********************************************************/
                    }
                    else
                    {
                        pa.Solde = 0;
                    }
                    bool succéespatient = false;
                    bool succéesvisite = false;
                    int interfaceoper;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.InsertPatient(pa);
                        interfaceoper = 0;
                        succéespatient = true;
                    if (pa.Solde != 0)
                        {
                         
                            interfaceoper = 1;
                            succéesvisite = true;
                            SVC.Visite cas = new SVC.Visite
                            {
                                Motif = "Ancien Solde",
                                Interrogatoire = "Ancien Solde",
                                Examen = "Ancien Solde",
                                Conclusions = "Ancien Solde",
                                Date = DateTime.Now,
                                Datetime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                                CasTraite = "Ancien Solde",
                                NuméroRendezVous = "Ancien Solde",

                                UserName = member.UserName,
                                NomPatient = pa.Nom,
                                PrénomPatient = pa.Prénom,

                                Montant = pa.Solde,
                                Versement = 0,
                                Reste = pa.Solde,
                                Soldé = false,
                                CodePatient = 0,
                                IdCas = 0,
                                IdMotif = 0,
                                ModeFacturation = 0,
                                cle = pa.cle,
                                VisiteParNom = nommedecin,
                                VisiteParPrénom= prénommedecin,
                                CodeMedecin = codemedecin,
                            };
                            proxy.InsertVisite(cas);
                        }
                        if (interfaceoper == 0 && succéespatient == true)
                        {
                            ts.Complete();
                            btnCreer.IsEnabled = false;
                            rempliridsivisite(pa);

                        }
                        else
                        {
                            if (interfaceoper == 1 && succéespatient == true && succéesvisite==true)
                            {

                                ts.Complete();

                       
                            }
                            else
                            {

                                btnCreer.IsEnabled = false;

                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                    }
                        if (interfaceoper == 0 && succéespatient == true)
                        {
                            proxy.AjouterPatientRefresh();
                           // proxy.AjouterSoldeVisiteRefresh();
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            if (interfaceoper == 1 && succéespatient == true && succéesvisite == true)
                            {
                                proxy.AjouterPatientRefresh();
                                proxy.AjouterSoldeVisiteRefresh();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                        }
                      /*      proxy.AjouterPatientRefresh();
                    proxy.AjouterSoldeVisiteRefresh();*/
                }
                catch (Exception ex)
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    if (Création == false && member.ModificationPatient == true && txtMedecin.SelectedItem != null && txtDate.SelectedDate != null && SexeCombo.SelectedValuePath != null && EtatCivicCombo.SelectedValuePath != null)
                    {
                        bool succesUpdatePatient = false;
                        bool succesUpdateVisite = false;
                     
                        SVC.Medecin SelectMedecin = txtMedecin.SelectedItem as SVC.Medecin;
                        if (SelectMedecin != null)
                        {
                            patient.SuiviParNom = SelectMedecin.Nom;
                            patient.SuiviParPrénom = SelectMedecin.Prénom;
                            patient.SuiviParCode = SelectMedecin.Id;
                        }
                            if (DossierTXT.Text != "")
                            {
                                patient.Dossier = Convert.ToInt16(DossierTXT.Text);


                                /********************************************************/
                            }
                            else
                            {
                                patient.Dossier = 0;
                            }
                            if (txtRéf.Text != "")
                            {
                                patient.Ref = Convert.ToInt16(txtRéf.Text);


                                /********************************************************/
                            }
                            else
                            {
                                patient.Ref = 0;
                            }
                            if (op.FileName != "")
                        {
                            serverfilepath = op.FileName;

                            filepath = "";
                            if (patient.CheminPhoto == "")
                            {
                                if (serverfilepath != "")
                                {


                                    filepath = op.FileName;

                                    serverfilepath = @"Patient\PhotoPatient\" + (txtNom.Text.Trim() + txtPrenom.Text.Trim()) + ".png";
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
                                    patient.CheminPhoto = serverfilepath;
                                }
                                else
                                {

                                    patient.CheminPhoto = "";
                                }
                            }
                            else
                            {
                                if (serverfilepath == "")
                                {
                                    patient.CheminPhoto = "";
                                }
                                else
                                {

                                    filepath = op.FileName;

                                    serverfilepath = @"Patient\PhotoPatient\" + (txtNom.Text.Trim() + txtPrenom.Text.Trim()) + ".png";
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
                                    patient.CheminPhoto = serverfilepath;
                                }
                            }
                        }else
                        {
                            patient.CheminPhoto = "";
                        }

                        if (txtCrédit.Text != "")
                        {
                            patient.Solde = Convert.ToDecimal(txtCrédit.Text);
                        }
                        else
                        {
                            patient.Solde = 0;
                        }
                        patient.DateDeNaissance = txtDate.SelectedDate;
                        var found = (proxy.GetAllVisiteByClepatient(patient.cle).Find(n=>n.cle==patient.cle));
                        int interfaceModif=3;
                      
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                      

                            if (found != null)
                            {
                                if (patient.Solde != 0)
                                {

                                   found.Montant = patient.Solde;
                                    found.Reste = patient.Solde - found.Versement;
                                        found.VisiteParNom = SelectMedecin.Nom;
                                        found.VisiteParPrénom = SelectMedecin.Prénom;
                                        found.CodeMedecin = SelectMedecin.Id;

                                    proxy.UpdateVisite(found);
                                    interfaceModif = 1;
                                    succesUpdateVisite = true;
                                    proxy.UpdatePatient(patient);
                                 
                                    succesUpdatePatient = true;
                                }
                                else
                                {
                                    if (patient.Solde == 0)
                                    {
                                        if (found.Versement == 0)
                                        {
                                            proxy.DeleteVisite(found);
                                            interfaceModif = 1;
                                            succesUpdateVisite = true;
                                            proxy.UpdatePatient(patient);
                                          
                                            succesUpdatePatient = true;
                                        }
                                        else
                                        {
                                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention erreur dans le solde patient qui contient un versement", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                        }
                                    }
                                }
                            }else
                            {
                                if (found == null)
                                {
                                    if (patient.Solde != 0)
                                    {
                                        SVC.Visite cas = new SVC.Visite
                                        {
                                            Motif = "Ancien Solde",
                                            Interrogatoire = "Ancien Solde",
                                            Examen = "Ancien Solde",
                                            Conclusions = "Ancien Solde",
                                            Date = DateTime.Now,
                                            Datetime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                                            CasTraite = "Ancien Solde",
                                            NuméroRendezVous = "Ancien Solde",

                                            UserName = member.UserName,
                                            NomPatient = patient.Nom,
                                            PrénomPatient = patient.Prénom,

                                            Montant = patient.Solde,
                                            Versement = 0,
                                            Reste = patient.Solde,
                                            Soldé = false,
                                            CodePatient = 0,
                                            IdCas = 0,
                                            IdMotif = 0,
                                            ModeFacturation = 0,
                                            cle = patient.cle,
                                            VisiteParNom = SelectMedecin.Nom,
                                            VisiteParPrénom = SelectMedecin.Prénom,
                                            CodeMedecin = SelectMedecin.Id,
                                        };
                                        proxy.InsertVisite(cas);
                                        interfaceModif = 1;
                                        succesUpdateVisite = true;
                                        proxy.UpdatePatient(patient);

                                        succesUpdatePatient = true;
                                    }else
                                    {
                                        if (patient.Solde == 0)
                                        {
                                            interfaceModif = 0;
                                            proxy.UpdatePatient(patient);
                                            succesUpdatePatient = true;
                                        }
                                    }
                                }
                            }
                            if (interfaceModif == 0 && succesUpdatePatient == true && succesUpdateVisite==false)
                            {
                                ts.Complete();
                            }
                            else
                            {
                                if (interfaceModif == 1 && succesUpdatePatient == true && succesUpdateVisite == true)
                                {
                                    ts.Complete();
                                }
                                else
                                {
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                            if (interfaceModif == 0 && succesUpdatePatient == true && succesUpdateVisite == false)
                            {
                               // proxy.AjouterSoldeVisiteRefresh();
                                proxy.AjouterPatientRefresh();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                if (interfaceModif == 1 && succesUpdatePatient == true && succesUpdateVisite == true)
                                {
                                    proxy.AjouterSoldeVisiteRefresh();
                                    proxy.AjouterPatientRefresh();
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }
                               
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
      
        private void txtPrenom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtNom.Text != "" && txtPrenom.Text != "" && Création == true)
                {

                
                    var disponible = proxy.GetAllPatienteEXISTE(txtNom.Text.Trim(), txtPrenom.Text.Trim());
                    if (disponible == true)
                    {

                        this.Title = "Ce Patient appartient déja a la base de donnée";

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;

                    }
                    else
                    {
                        this.Title = "Patient " + txtNom.Text.Trim().ToUpper() + "  " + txtPrenom.Text.ToUpper() + " est disponible pour création";
                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;

                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtNom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtPrenom.Text.Trim() != "")
                {
                   
                    var disponible = proxy.GetAllPatienteEXISTE(txtNom.Text.Trim(), txtPrenom.Text.Trim());
                    if (disponible == true)
                    {
                        this.Title = "Ce Patient appartient déja a la base de donnée";

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;
                    }
                    else
                    {
                        this.Title = "Patient " + txtNom.Text.Trim().ToUpper() + "  " + txtPrenom.Text.ToUpper() + " est disponible pour création";
                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;

                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

       
        private void txtCrédit_KeyDown(object sender, KeyEventArgs e)
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

        private void BTNCLOSTE_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRéf_KeyDown(object sender, KeyEventArgs e)
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
               


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void DossierTXT_KeyDown(object sender, KeyEventArgs e)
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



                    break;
                default:
                    e.Handled = true;
                    break;
            }
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }






        }
           
        }


    }

