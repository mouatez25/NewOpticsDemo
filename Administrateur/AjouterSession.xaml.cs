using DevExpress.Xpf.Core;
using GestionClinique.SVC;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for AjouterSession.xaml
    /// </summary>
    public partial class AjouterSession : DXWindow
    {//
        SVC.ServiceCliniqueClient proxy;
        Membership SelectMembershiListe;
        string title;
        private delegate void FaultedInvokerMedecin();
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMedecin(HandleProxy));
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


        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMedecin(HandleProxy));
                return;
            }
            HandleProxy();
        }

        public AjouterSession(Membership mem, SVC.ServiceCliniqueClient proxyrecu, ListeSession mainSession)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                if (mem != null)
                {



                    InitializeComponent();

                   
                    proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                    proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                    var query = from c in proxy.GetAllMedecin()
                                select new { c.Nom, c.Prénom, c.UserName };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.UserName.Trim().ToUpper() == mem.UserName.Trim().ToUpper()).FirstOrDefault();

                    if (disponible == null)
                    {




                        if (mem.UserName == "Administrateur")
                        {
                            SelectMembershiListe = mem;
                            this.InfGénéral.DataContext = SelectMembershiListe;
                            this.Title = "Modification de Session";
                            title = this.Title;
                       //     titlebrush = this.WindowTitleBrush;
                            txtMotDePasseRe.Text = SelectMembershiListe.MotDePasse;
                            txtUserName.IsEnabled = false;
                            txtNom.IsEnabled = false;
                            txtPrenom.IsEnabled = false;
                            txtUserName.Background = Brushes.Gray;
                            txtNom.Background = Brushes.Gray;
                            txtPrenom.Background = Brushes.Gray;
                            chACTIF.IsEnabled = true;
                            chadmin.IsEnabled = false;
                            chaRendz.IsEnabled = false;

                            chcreeradmin.IsEnabled = false;
                            chmodifadmin.IsEnabled = false;
                            chsuppadmin.IsEnabled = false;
                            chimpradmin.IsEnabled = false;

                         

                            chcreerRendz.IsEnabled = false;
                            chmodifRendz.IsEnabled = false;
                            chsuppRendz.IsEnabled = false;
                            chimprRendz.IsEnabled = false;
                            chaPatient.IsEnabled = false;
                            chimprPatient.IsEnabled = false;
                            chcreerPatient.IsEnabled = false;
                            chmodifPatient.IsEnabled = false;
                            chsuppPatient.IsEnabled = false;



                            chimprAccèsToutLesDossierPatient.IsEnabled = false;
                            chaDossierPatient.IsEnabled = false;
                            chmodifDossierPatient.IsEnabled = false;
                            chcreerDossierPatient.IsEnabled = false;
                            chsuppDossierPatient.IsEnabled = false;
                            chimprDossierPatient.IsEnabled = false;

                            chaDossierSalleAttente.IsEnabled = false;
                            chcreerSalleAttente.IsEnabled = false;
                            chmodifSalleAttente.IsEnabled = false;
                            chsuppSalleAttente.IsEnabled = false;
                            chimprSalleAttente.IsEnabled = false;

                            chaCaisse.IsEnabled = false;
                            chimprCaisse.IsEnabled = false;
                            chcreerCaisse.IsEnabled = false;
                            chmodifCaisse.IsEnabled = false;
                            chsuppCaisse.IsEnabled = false;
                            chimprCaisse.IsEnabled = false;


                            chaAchat.IsEnabled = false;
                            chcreerAchat.IsEnabled = false;
                            chmodifAchat.IsEnabled = false;
                            chsuppAchat.IsEnabled = false;
                            chimprAchat.IsEnabled = false;

                            chaModuleChat.IsEnabled = false;
                            chcreerEnvoiReceptMessage.IsEnabled = false;
                            chmodifEnvoiRécéptFichier.IsEnabled = false;
                            chsuppDiscussionPrivé.IsEnabled = false;

                        }
                        else
                        {
                            SelectMembershiListe = mem;
                            this.InfGénéral.DataContext = SelectMembershiListe;
                            this.Title = "Modification de Session";
                            title = this.Title;
                      //      titlebrush = this.WindowTitleBrush;
                            txtMotDePasseRe.Text = SelectMembershiListe.MotDePasse;

                        }
                    }
                    else
                    {
                        SelectMembershiListe = mem;
                        this.InfGénéral.DataContext = SelectMembershiListe;
                        this.Title = "Modification de Session";
                        title = this.Title;
                   //     titlebrush = this.WindowTitleBrush;
                        txtMotDePasseRe.Text = SelectMembershiListe.MotDePasse;
                        txtUserName.IsEnabled = false;
                        txtNom.IsEnabled = false;
                        txtPrenom.IsEnabled = false;
                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;
                        txtUserName.Background = Brushes.Gray;
                        txtNom.Background = Brushes.Gray;
                        txtPrenom.Background = Brushes.Gray;
                        chACTIF.IsEnabled = false;
                        chadmin.IsEnabled = false;
                        chaRendz.IsEnabled = false;

                        chcreeradmin.IsEnabled = false;
                        chmodifadmin.IsEnabled = false;
                        chsuppadmin.IsEnabled = false;
                        chimpradmin.IsEnabled = false;

                     
                        chcreerRendz.IsEnabled = false;
                        chmodifRendz.IsEnabled = false;
                        chsuppRendz.IsEnabled = false;
                        chimprRendz.IsEnabled = false;
                        chaPatient.IsEnabled = false;
                        chimprPatient.IsEnabled = false;
                        chcreerPatient.IsEnabled = false;
                        chmodifPatient.IsEnabled = false;
                        chsuppPatient.IsEnabled = false;



                        chimprAccèsToutLesDossierPatient.IsEnabled = false;
                        chaDossierPatient.IsEnabled = false;
                        chmodifDossierPatient.IsEnabled = false;
                        chcreerDossierPatient.IsEnabled = false;
                        chsuppDossierPatient.IsEnabled = false;
                        chimprDossierPatient.IsEnabled = false;

                        chaDossierSalleAttente.IsEnabled = false;
                        chcreerSalleAttente.IsEnabled = false;
                        chmodifSalleAttente.IsEnabled = false;
                        chsuppSalleAttente.IsEnabled = false;
                        chimprSalleAttente.IsEnabled = false;

                        chaCaisse.IsEnabled = false;
                        chimprCaisse.IsEnabled = false;
                        chcreerCaisse.IsEnabled = false;
                        chmodifCaisse.IsEnabled = false;
                        chsuppCaisse.IsEnabled = false;
                        chimprCaisse.IsEnabled = false;


                        chaAchat.IsEnabled = false;
                        chcreerAchat.IsEnabled = false;
                        chmodifAchat.IsEnabled = false;
                        chsuppAchat.IsEnabled = false;
                        chimprAchat.IsEnabled = false;

                        chaModuleChat.IsEnabled = false;
                        chcreerEnvoiReceptMessage.IsEnabled = false;
                        chmodifEnvoiRécéptFichier.IsEnabled = false;
                        chsuppDiscussionPrivé.IsEnabled = false;


                        chACTIVserver.IsEnabled = false;

                        MESSAGELABELSession.Content = "Cette session apartient a un medecin modification impossible dans cette rubrique";
                        MESSAGELABELSession.Foreground = Brushes.Red;
                    }
                }
                else
                {
                    title = this.Title;
                  //  titlebrush = this.WindowTitleBrush;
                    btnValider.IsEnabled = false;
                    btnValider.Opacity = 0.2;


                }
                mainSession.AddEventSession += new System.EventHandler(addsessionWindow_AddEventSession);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        void addsessionWindow_AddEventSession(object sender, System.EventArgs e)
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


        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                SVC.Membership NewMembership;

                NewMembership = new SVC.Membership
                {
                    UserName = txtUserName.Text.Trim(),
                    Nom = txtNom.Text.Trim(),
                    Prénom = txtPrenom.Text.Trim(),
                    MotDePasse = txtMotDePasse.Text.Trim(),
                    Actif = chACTIF.IsChecked,
                    ModuleAdministrateur = chadmin.IsChecked,
                    CréationAdministrateur = chcreeradmin.IsChecked,
                    ModificationAdministrateur = chmodifadmin.IsChecked,
                    SuppressionAdministrateur = chsuppadmin.IsChecked,
                    ImpressionAdministrateur = chimpradmin.IsChecked,
                
                    ModuleRendezVous = chaRendz.IsChecked,
                    CréationRendezVous = chcreerRendz.IsChecked,
                    ModificationRendezVous = chmodifRendz.IsChecked,
                    SuppressionRendezVous = chsuppRendz.IsChecked,
                    ImpressionRendezVous = chimprRendz.IsChecked,

                    ModulePatient = chaPatient.IsChecked,
                    CréationPatient = chcreerPatient.IsChecked,
                    ModificationPatient = chmodifPatient.IsChecked,
                    SuppressionPatient = chsuppPatient.IsChecked,
                    ImpressionPatient = chimprPatient.IsChecked,

                    CréerLe = DateTime.Now,
                    ModuleDossierPatient = chaDossierPatient.IsChecked,
                    CréationDossierPatient = chcreerDossierPatient.IsChecked,
                    ModificationDossierPatient = chmodifDossierPatient.IsChecked,
                    SuppressionDossierPatient = chsuppDossierPatient.IsChecked,
                    ImpressionDossierPatient = chimprDossierPatient.IsChecked,
                    AccèsToutLesDossierPatient = chimprAccèsToutLesDossierPatient.IsChecked,

                    ModuleSalleAttente = chaDossierSalleAttente.IsChecked,
                    CréationSalleAttente = chcreerSalleAttente.IsChecked,
                    ModificationSalleAttente = chmodifSalleAttente.IsChecked,
                    SupressionSalleAttente = chsuppSalleAttente.IsChecked,
                    ImpressionSalleAttente = chimprSalleAttente.IsChecked,

                    ModuleCaisse = chaCaisse.IsChecked,
                    CréationCaisse = chcreerCaisse.IsChecked,
                    ModificationCaisse = chmodifCaisse.IsChecked,
                    SuppréssionCaisse = chsuppCaisse.IsChecked,
                    ImressionCaisse = chimprCaisse.IsChecked,

                    ModuleAchat = chaAchat.IsChecked,
                    CréationAchat = chcreerAchat.IsChecked,
                    ModificationAchat = chmodifAchat.IsChecked,
                    SupressionAchat = chsuppAchat.IsChecked,
                    ImpressionAchat = chimprAchat.IsChecked,

                     ModuleChat= chaModuleChat.IsChecked,
                     DiscussionPrivé= chsuppDiscussionPrivé.IsChecked,
                     EnvoiReceptMessage= chcreerEnvoiReceptMessage.IsChecked,
                     EnvoiRécéptFichier= chmodifEnvoiRécéptFichier.IsChecked,


                };
                if (SelectMembershiListe == null)
                {

                    try
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.InsertMembership(NewMembership);
                            btnValider.IsEnabled = false;
                            ts.Complete();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
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
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateMembership(SelectMembershiListe);
                            ts.Complete();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                       
                 //       this.WindowTitleBrush = Brushes.Green;

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



        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var query = from c in proxy.GetAllMembership()
                            select new
                            {
                                c.UserName                           };
                if (txtUserName.Text.Trim() != "" && SelectMembershiListe == null)
                {



                    var resultsList = query.ToList();
                    var disponibleSession = resultsList.Where(list0 => list0.UserName.Trim().ToUpper() == txtUserName.Text.Trim().ToUpper()).FirstOrDefault();
                    if (disponibleSession != null)
                    {
                        MESSAGELABELSession.Content = "Cette Session appartient déja a la base de donnée";
                     //   this.WindowTitleBrush = Brushes.Red;

                        btnValider.IsEnabled = false;
                        btnValider.Opacity = 0.2;

                        txtPrenom.IsEnabled = false;
                        txtNom.IsEnabled = false;
                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;
                    }
                    else
                    {
                        if (txtUserName.Text.Trim() != "")
                        {
                            MESSAGELABELSession.Content = "";

                            txtPrenom.IsEnabled = true;
                            txtNom.IsEnabled = true;
                            txtMotDePasse.IsEnabled = true;
                            txtMotDePasseRe.IsEnabled = true;
                            if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim() != "")
                            {
                                btnValider.IsEnabled = true;
                                btnValider.Opacity = 1;
                            }
                        }
                    }
                }
                else
                {

                    btnValider.IsEnabled = false;
                    btnValider.Opacity = 0.2;
                }
            } catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrenom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && SelectMembershiListe == null)
                {

                    var query = from c in proxy.GetAllMembership()
                                select new { c.Nom, c.Prénom, c.UserName };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.Nom.ToUpper() == txtNom.Text.Trim().ToUpper() && list1.Prénom.ToUpper() == txtPrenom.Text.Trim().ToUpper()).FirstOrDefault();
                    if (disponible != null)
                    {
                        MESSAGELABELSession.Content = "Ce Nom et Prénom existe déja avec une autre session";
                      //  this.WindowTitleBrush = Brushes.Red;

                        btnValider.IsEnabled = false;
                        btnValider.Opacity = 0.2;
                        txtUserName.IsEnabled = false;
                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;



                    }
                    else
                    {
                        if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim())

                        {
                            MESSAGELABELSession.Content = "";
                       //     this.WindowTitleBrush = titlebrush;
                            btnValider.IsEnabled = true;
                            btnValider.Opacity = 1;
                            txtUserName.IsEnabled = true;
                            txtMotDePasse.IsEnabled = true;
                            txtMotDePasseRe.IsEnabled = true;

                            if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim() != "")
                            {
                                btnValider.IsEnabled = true;
                                btnValider.Opacity = 1;
                            }

                        }
                        else
                        {
                            txtMotDePasse.IsEnabled = true;
                            txtMotDePasseRe.IsEnabled = true;
                            this.Title = title;
                          //  this.WindowTitleBrush = titlebrush;
                        }

                    }
                }
                else
                {
                    btnValider.IsEnabled = false;
                    btnValider.Opacity = 0.2;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private void txtMotDePasseRe_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (txtMotDePasse.Text.Trim() != "" && txtUserName.Text.Trim() != "")
            {
                if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim())
                {
                    if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && SelectMembershiListe == null)

                    {
                        MESSAGELABELSession.Content = "";
                   //     this.WindowTitleBrush = titlebrush;
                        btnValider.IsEnabled = true;
                        btnValider.Opacity = 1;
                        txtNom.IsEnabled = true;
                        txtPrenom.IsEnabled = true;
                        txtUserName.IsEnabled = true;

                    } else
                    {
                        if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && SelectMembershiListe != null)

                        {
                            MESSAGELABELSession.Content = "";
                        //    this.WindowTitleBrush = titlebrush;
                            if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim() != "")
                            {
                                btnValider.IsEnabled = true;
                                btnValider.Opacity = 1;
                            }


                        }
                    }
                }
                else
                {
                    this.Title = title;
                //    this.WindowTitleBrush = titlebrush;
                    MESSAGELABELSession.Content = "Veuillez Confirmez le mot de passe";
                    MESSAGELABELSession.Foreground= Brushes.Red;

                    btnValider.IsEnabled = false;
                    btnValider.Opacity = 0.2;
                    txtNom.IsEnabled = false;
                    txtPrenom.IsEnabled = false;
                    txtUserName.IsEnabled = false;
                }

            }
            else
            {
                btnValider.IsEnabled = false;
                btnValider.Opacity = 0.2;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtMotDePasse_TextChanged(object sender, TextChangedEventArgs e)
        {
            try {
            
            if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && SelectMembershiListe == null)

            {
                    MESSAGELABELSession.Content = "";
           //     this.WindowTitleBrush = titlebrush;
                if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim() != "")
                {
                    btnValider.IsEnabled = true;
                    btnValider.Opacity = 1;
                }


            }
            else
            {
                if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && SelectMembershiListe != null)

                {
                        MESSAGELABELSession.Content = "";
              //      this.WindowTitleBrush = titlebrush;
                    if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim() != "")
                    {
                        btnValider.IsEnabled = true;
                        btnValider.Opacity = 1;
                    }


                }
                else
                {
                    btnValider.IsEnabled = false;
                    btnValider.Opacity = 0.2;
                }

            }
        }
    
            catch (Exception ex)
            {
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Medicus", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtNom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtPrenom.Text.Trim() != "" && SelectMembershiListe == null)
                {
                    var query = from c in proxy.GetAllMembership()
                                select new { c.Nom, c.Prénom, };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.Nom.ToUpper() == txtNom.Text.Trim().ToUpper() && list1.Prénom.ToUpper() == txtPrenom.Text.Trim().ToUpper()).FirstOrDefault();
                    if (disponible != null)
                    {
                        MESSAGELABELSession.Content = "Ce Nom et Prénom existe dans la base de donnée";
               //         this.WindowTitleBrush = Brushes.Red;

                        btnValider.IsEnabled = false;
                        btnValider.Opacity = 0.2;
                        txtUserName.IsEnabled = false;
                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;
                    }
                    else
                    {
                        if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtNom.Text.Trim() != "" && txtPrenom.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim())

                        {

                            MESSAGELABELSession.Content="";
                   //         this.WindowTitleBrush = titlebrush;
                            if (txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && txtMotDePasse.Text.Trim() != "")
                            {
                                btnValider.IsEnabled = true;
                                btnValider.Opacity = 1;
                            }
                            txtNom.IsEnabled = true;
                            txtPrenom.IsEnabled = true;
                            txtUserName.IsEnabled = true;

                        }
                        else
                        {
                            this.Title = title;
                         //   this.WindowTitleBrush = titlebrush;
                            txtMotDePasse.IsEnabled = true;
                            txtMotDePasseRe.IsEnabled = true;
                            //   btnValider.IsEnabled = true;
                            // btnValider.Opacity = 1;
                        }




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
