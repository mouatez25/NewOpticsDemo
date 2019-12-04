using NewOptics.Administrateur;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Param
{
    /// <summary>
    /// Interaction logic for Utilisateurs.xaml
    /// </summary>
    public partial class Utilisateurs : Page
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;

        SVC.Client localclient;
        SVC.MembershipOptic selectedsession;
        private delegate void FaultedInvokerListeParamUtilisateurs();
        bool creation = false;
        bool modification = false;
        public Utilisateurs(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Client clientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                localclient = clientrecu;
                MemberDataGrid.ItemsSource =proxy.GetAllMembershipOptics().OrderBy(n => n.Username);


                callbackrecu.InsertMmebershipCallbackEvent += new ICallback.CallbackEventHandler5(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertMembershipOptic e)
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
        public void AddRefresh(List<SVC.MembershipOptic> listmembership)
        {
            try
            {
                MemberDataGrid.ItemsSource = listmembership.OrderBy(n => n.Username);

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeParamUtilisateurs(HandleProxy));
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

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;


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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeParamUtilisateurs(HandleProxy));
                return;
            }
            HandleProxy();
        }

       

        
        private void txtMotDePasse_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && creation==true && modification==false)
                    

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
                    if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != ""  && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && creation==false && modification==true
                        )

                    {
                        MESSAGELABELSession.Content = "";
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
       

      

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (creation == true && modification == false && selectedsession!=null)
                {
                    selectedsession.DernierAccés = DateTime.Now;
                    selectedsession.DernierAccésHeure = DateTime.Now.TimeOfDay;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.InsertMembershipOptics(selectedsession);
                        ts.Complete();

                    }
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    creation = false;
                    modification = false;
                    selectedsession = null;
                    InfGénéral.DataContext = null;
                    windowBorderDatdagridCasTraiter.IsEnabled = false;
                    windowBorderDatsdagridCasTraiter.IsEnabled = false;
                    windowBorderDatagridCasTraiter.IsEnabled = false;
                    winddowBordderDatsdagridCasTraiter.IsEnabled = false;
                    winddowBorderDatsdagridCasTraiter.IsEnabled = false;
                    winddowBordderCommande.IsEnabled = false;
                    chACTIVserver.IsEnabled = false;
                    chimprModuleModuleStatistiqueAcces.IsEnabled = false;
                    btnValider.IsEnabled = false;
                    chimprModuleModuleVenteCompoirAcces.IsEnabled = false;
                    chimprModuleParametreAcces.IsEnabled = false;
                    chACTIF.IsEnabled = false;
                    txtUserName.IsEnabled = false;
                    txtMotDePasse.IsEnabled = false;
                    txtMotDePasseRe.IsEnabled = false;
                    txtMotDePasseRe.Text = "";
                }
                else
                {
                    if (creation == false && modification == true && selectedsession != null)
                {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateMembershipOptics(selectedsession);
                            ts.Complete();

                        }
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        creation = false;
                        modification = false;
                        selectedsession = null;
                        InfGénéral.DataContext = null;
                        windowBorderDatdagridCasTraiter.IsEnabled = false;
                        windowBorderDatsdagridCasTraiter.IsEnabled = false;
                        windowBorderDatagridCasTraiter.IsEnabled = false;
                        winddowBordderDatsdagridCasTraiter.IsEnabled = false;
                        winddowBorderDatsdagridCasTraiter.IsEnabled = false;
                        chACTIVserver.IsEnabled = false;
                        chimprModuleModuleStatistiqueAcces.IsEnabled = false;
                        btnValider.IsEnabled = false;
                        chimprModuleModuleVenteCompoirAcces.IsEnabled = false;
                        chimprModuleParametreAcces.IsEnabled = false;
                        chACTIF.IsEnabled = false;
                        txtUserName.IsEnabled = false;
                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;
                        txtMotDePasseRe.Text = "";
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModuleParametreAcces==true)
                {
                    selectedsession = new SVC.MembershipOptic
                    {
                        Actif=false,
                        ActiveServer = false,
                      
                        DiscussionPrivé = false,
                        CreationDossierClient = false,
                        CreationAchat = false,
                        CreationCommande = false,CreationRecouv = false,CreationAms = false,
                        SuppressinAchat = false,SuppressionCaisse = false,SuppressionFichier = false,SuppressionPlanning = false,SuppressionRecouv = false,SupressionCommande = false,
                        SupressionDossierClient = false,ModuleStatistiqueAcces = false,CreationCaisse = false,CreationFichier = false,
                        CreationPlanning = false,EnvoiReceptMessage = false,ImpressionCaisse = false,ImpressionCommande = false,EnvoiRécéptFichier = false,ImpressionFichier = false,ModificationFichier = false,
                        ImpressionAchat = false,ImpressionAms = false,ImpressionDossierClient = false,ModuleVenteCompoirAcces = false,ModificationCaisse = false,ModificationCommande = false,
                        ImpressionPlanning = false,ImpressionRecouv = false,ModificationAchat = false,ModuleFichier = false,ModificationDossierClient = false,ModificationAms = false,ModificationPlanning = false,ModuleAchat = false,
                      ModificationRecouv = false,ModuleAms = false,
                      ModuleCaisse = false,ModuleChat = false,ModuleCommande = false,ModuleDossierClient = false,ModuleParametreAcces = false,ModulePlanning = false,ModuleRecouv = false,MotDePasse ="",Username ="",
                        SuppressionAms = false,
                    };

                     InfGénéral.DataContext = selectedsession; 
                  creation = true;
                    modification = false;
                    txtMotDePasseRe.Text = "";
                    windowBorderDatdagridCasTraiter.IsEnabled = true;
                    windowBorderDatsdagridCasTraiter.IsEnabled = true;
                    windowBorderDatagridCasTraiter.IsEnabled = true;
                    winddowBordderDatsdagridCasTraiter.IsEnabled = true;
                    winddowBorderDatsdagridCasTraiter.IsEnabled = true;
                    chACTIVserver.IsEnabled = true;
                    chimprModuleModuleStatistiqueAcces.IsEnabled = true;
                    btnValider.IsEnabled = true;
                    chimprModuleModuleVenteCompoirAcces.IsEnabled = true;
                    chimprModuleParametreAcces.IsEnabled = true;
                    chACTIF.IsEnabled = true;
                    txtUserName.IsEnabled = true;
                    txtMotDePasse.IsEnabled = true;
                    txtMotDePasseRe.IsEnabled = true;
                    GridCommande.IsEnabled = true;
                    winddowBordderCommande.IsEnabled = true;
                    selection.IsEnabled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               if(MemberDataGrid.SelectedItem!=null && memberuser.ModuleParametreAcces==true)
                {
                    var selecteduser = MemberDataGrid.SelectedItem as SVC.MembershipOptic;
                    if(selecteduser.Id==1)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous ne pouvez pas supprimer la session administrateur", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }else
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.DeleteMembershipOptics(selecteduser);
                            ts.Complete();
                        }
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }


                }

            }
            catch(Exception ex)
            {

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberDataGrid.SelectedItem != null && memberuser.ModuleParametreAcces == true)
                {
                    selectedsession = MemberDataGrid.SelectedItem as SVC.MembershipOptic;
                    InfGénéral.DataContext = selectedsession;
                    creation = false;
                    modification = true;

                    if (selectedsession.Id != 1)
                    {
                        windowBorderDatdagridCasTraiter.IsEnabled = true;
                        windowBorderDatsdagridCasTraiter.IsEnabled = true;
                        windowBorderDatagridCasTraiter.IsEnabled = true;
                        winddowBordderDatsdagridCasTraiter.IsEnabled = true;
                        winddowBorderDatsdagridCasTraiter.IsEnabled = true;
                        chACTIVserver.IsEnabled = true;
                        chimprModuleModuleStatistiqueAcces.IsEnabled = true;
                        btnValider.IsEnabled = true;
                        chimprModuleModuleVenteCompoirAcces.IsEnabled = true;
                        chimprModuleParametreAcces.IsEnabled = true;
                        chACTIF.IsEnabled = true;
                        txtUserName.IsEnabled = true;
                        txtMotDePasse.IsEnabled = true;
                        txtMotDePasseRe.IsEnabled = true;
                        winddowBordderCommande.IsEnabled = true;
                        selection.IsEnabled = true;
                    }
                    else
                    {
                        windowBorderDatdagridCasTraiter.IsEnabled = false;
                        windowBorderDatsdagridCasTraiter.IsEnabled = false;
                        windowBorderDatagridCasTraiter.IsEnabled = false;
                        winddowBordderDatsdagridCasTraiter.IsEnabled = false;
                        winddowBorderDatsdagridCasTraiter.IsEnabled = false;
                        chACTIVserver.IsEnabled = false;
                        chimprModuleModuleStatistiqueAcces.IsEnabled = false;
                        btnValider.IsEnabled = false;
                        chimprModuleModuleVenteCompoirAcces.IsEnabled = false;
                        chimprModuleParametreAcces.IsEnabled = false;
                        chACTIF.IsEnabled = false;
                        txtUserName.IsEnabled = false;
                        txtMotDePasse.IsEnabled = true;
                        txtMotDePasseRe.IsEnabled = true;
                        winddowBordderCommande.IsEnabled = false;
                        selection.IsEnabled = false;
                    }
                    txtMotDePasseRe.Text = selectedsession.MotDePasse;
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez ", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MemberDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                 if (MemberDataGrid.SelectedItem != null && memberuser.ModuleParametreAcces==true)
                { 
                    selectedsession = MemberDataGrid.SelectedItem as SVC.MembershipOptic;
                    InfGénéral.DataContext = selectedsession;
                    creation = false;
                    modification = true;

                    if (selectedsession.Id != 1)
                    {
                        windowBorderDatdagridCasTraiter.IsEnabled = true;
                        windowBorderDatsdagridCasTraiter.IsEnabled = true;
                        windowBorderDatagridCasTraiter.IsEnabled = true;
                        winddowBordderDatsdagridCasTraiter.IsEnabled = true;
                        winddowBorderDatsdagridCasTraiter.IsEnabled = true;
                        chACTIVserver.IsEnabled = true;
                        chimprModuleModuleStatistiqueAcces.IsEnabled = true;
                        btnValider.IsEnabled = true;
                        chimprModuleModuleVenteCompoirAcces.IsEnabled = true;
                        chimprModuleParametreAcces.IsEnabled = true;
                        chACTIF.IsEnabled = true;
                        txtUserName.IsEnabled = true;
                        txtMotDePasse.IsEnabled = true;
                        txtMotDePasseRe.IsEnabled = true;
                        winddowBordderCommande.IsEnabled = true;
                        selection.IsEnabled = true;
                    }else
                    {
                        windowBorderDatdagridCasTraiter.IsEnabled = false;
                        windowBorderDatsdagridCasTraiter.IsEnabled = false;
                        windowBorderDatagridCasTraiter.IsEnabled = false;
                        winddowBordderDatsdagridCasTraiter.IsEnabled = false;
                        winddowBorderDatsdagridCasTraiter.IsEnabled = false;
                        chACTIVserver.IsEnabled = false;
                        chimprModuleModuleStatistiqueAcces.IsEnabled = false;
                        btnValider.IsEnabled = false;
                        chimprModuleModuleVenteCompoirAcces.IsEnabled = false;
                        chimprModuleParametreAcces.IsEnabled = false;
                        chACTIF.IsEnabled = false;
                        txtUserName.IsEnabled = false;
                        txtMotDePasse.IsEnabled = true;
                        txtMotDePasseRe.IsEnabled = true;
                        winddowBordderCommande.IsEnabled = false;
                        selection.IsEnabled = false;
                    }
                    txtMotDePasseRe.Text = selectedsession.MotDePasse;
                 }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez ", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                } 

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void MemberDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            creation = false;
            modification = false;
            selectedsession = null;
            InfGénéral.DataContext = null;
            windowBorderDatdagridCasTraiter.IsEnabled = false;
            windowBorderDatsdagridCasTraiter.IsEnabled = false;
            windowBorderDatagridCasTraiter.IsEnabled = false;
            winddowBordderDatsdagridCasTraiter.IsEnabled = false;
            winddowBorderDatsdagridCasTraiter.IsEnabled = false;
            chACTIVserver.IsEnabled = false;
            chimprModuleModuleStatistiqueAcces.IsEnabled = false;
            btnValider.IsEnabled = false;
            chimprModuleModuleVenteCompoirAcces.IsEnabled = false;
            chimprModuleParametreAcces.IsEnabled = false;
            chACTIF.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtMotDePasse.IsEnabled = false;
            txtMotDePasseRe.IsEnabled = false;
            winddowBordderCommande.IsEnabled = false;
            txtMotDePasseRe.Text = "";
        }

        
        

        private void selection_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                chACTIF.IsChecked = true;
                chimprPatient.IsChecked = true;
                chadmin.IsChecked = true;
                    chcreeradmin.IsChecked = true;
                chmodifadmin.IsChecked = true;
                chsuppadmin.IsChecked = true;
                chimpradmin.IsChecked = true;
                chaRendz.IsChecked = true;
                chcreerRendz.IsChecked = true;
                chmodifRendz.IsChecked = true;
                chsuppRendz.IsChecked = true;
                chimprRendz.IsChecked = true;
                chaDossierPatient.IsChecked = true;
                chcreerDossierPatient.IsChecked = true;
                chmodifDossierPatient.IsChecked = true;
                chsuppDossierPatient.IsChecked = true;
                chimprDossierPatient.IsChecked = true;
                chaPatient.IsChecked = true;
                chcreerPatient.IsChecked = true;
                chmodifPatient.IsChecked = true;
                chsuppPatient.IsChecked = true;
                chaDossierSalleAttente.IsChecked = true;
                chcreerSalleAttente.IsChecked = true;
                chmodifSalleAttente.IsChecked = true;
                chsuppSalleAttente.IsChecked = true;
                chimprSalleAttente.IsChecked = true;
                chaCommande.IsChecked = true;
                chcreerCommande.IsChecked = true;
                chmodifCommande.IsChecked = true;
                chsuppCommande.IsChecked = true;
                chimprCommande.IsChecked = true;

                chaAchat.IsChecked = true;
                chcreerAchat.IsChecked = true;
                chmodifAchat.IsChecked = true;
                chsuppAchat.IsChecked = true;
                chimprAchat.IsChecked = true;
                chaModuleChat.IsChecked = true;
                chcreerEnvoiReceptMessage.IsChecked = true;
                chmodifEnvoiRécéptFichier.IsChecked = true;
                chsuppDiscussionPrivé.IsChecked = true;
                chimprModuleModuleVenteCompoirAcces.IsChecked = true;
                chimprModuleModuleStatistiqueAcces.IsChecked = true;
                chimprModuleParametreAcces.IsChecked = true;
                chACTIVserver.IsChecked = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void selection_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                chACTIF.IsChecked = false;
                chimprPatient.IsChecked = false;
                chadmin.IsChecked = false;
                chcreeradmin.IsChecked = false;
                chmodifadmin.IsChecked = false;
                chsuppadmin.IsChecked = false;
                chimpradmin.IsChecked = false;
                chaRendz.IsChecked = false;
                chcreerRendz.IsChecked = false;
                chmodifRendz.IsChecked = false;
                chsuppRendz.IsChecked = false;
                chimprRendz.IsChecked = false;
                chaDossierPatient.IsChecked = false;
                chcreerDossierPatient.IsChecked = false;
                chmodifDossierPatient.IsChecked = false;
                chsuppDossierPatient.IsChecked = false;
                chimprDossierPatient.IsChecked = false;
                chaPatient.IsChecked = false;
                chcreerPatient.IsChecked = false;
                chmodifPatient.IsChecked = false;
                chsuppPatient.IsChecked = false;
                chaDossierSalleAttente.IsChecked = false;
                chcreerSalleAttente.IsChecked = false;
                chmodifSalleAttente.IsChecked = false;
                chsuppSalleAttente.IsChecked = false;
                chimprSalleAttente.IsChecked = false;
                chaCommande.IsChecked = false;
                chcreerCommande.IsChecked = false;
                chmodifCommande.IsChecked = false;
                chsuppCommande.IsChecked = false;
                chimprCommande.IsChecked = false;

                chaAchat.IsChecked = false;
                chcreerAchat.IsChecked = false;
                chmodifAchat.IsChecked = false;
                chsuppAchat.IsChecked = false;
                chimprAchat.IsChecked = false;
                chaModuleChat.IsChecked = false;
                chcreerEnvoiReceptMessage.IsChecked = false;
                chmodifEnvoiRécéptFichier.IsChecked = false;
                chsuppDiscussionPrivé.IsChecked = false;
                chimprModuleModuleVenteCompoirAcces.IsChecked = false;
                chimprModuleModuleStatistiqueAcces.IsChecked = false;
                chimprModuleParametreAcces.IsChecked = false;
                chACTIVserver.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var query = from c in proxy.GetAllMembershipOptics()
                            select new
                            {
                                c.Username
                            };
                if (txtUserName.Text.Trim() != "" && creation == true && modification == false)
                {



                    var resultsList = query.ToList();
                    var disponibleSession = resultsList.Where(list0 => list0.Username.Trim().ToUpper() == txtUserName.Text.Trim().ToUpper()).FirstOrDefault();
                    if (disponibleSession != null)
                    {
                        MESSAGELABELSession.Content = "Cette Session appartient déja a la base de donnée";
                        //   this.WindowTitleBrush = Brushes.Red;

                        btnValider.IsEnabled = false;
                        btnValider.Opacity = 0.2;


                        txtMotDePasse.IsEnabled = false;
                        txtMotDePasseRe.IsEnabled = false;
                    }
                    else
                    {
                        if (txtUserName.Text.Trim() != "")
                        {
                            MESSAGELABELSession.Content = "";


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
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                        if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && creation == true && modification == false)

                        {
                            MESSAGELABELSession.Content = "";
                            //     this.WindowTitleBrush = titlebrush;
                            btnValider.IsEnabled = true;
                            btnValider.Opacity = 1;

                            txtUserName.IsEnabled = true;

                        }
                        else
                        {
                            if (txtUserName.Text.Trim() != "" && txtMotDePasse.Text.Trim() != "" && txtMotDePasseRe.Text.Trim() != "" && txtMotDePasse.Text.Trim() == txtMotDePasseRe.Text.Trim() && modification == true && creation == false)

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

                        //    this.WindowTitleBrush = titlebrush;
                        MESSAGELABELSession.Content = "Veuillez Confirmez le mot de passe";
                        MESSAGELABELSession.Foreground = Brushes.Red;

                        btnValider.IsEnabled = false;
                        btnValider.Opacity = 0.2;

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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
