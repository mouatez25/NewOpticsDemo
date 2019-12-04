using GestionClinique.Administrateur;
using GestionClinique.FileDattente;
using GestionClinique.Patient;
using GestionClinique.RendezVous;
using GestionClinique.SVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerHome();
        SVC.Client localclient;
        public Home(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership memberrecu, ICallback callbackrecu,SVC.Client clienrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                localclient = clienrecu;
           //     LogicielName.Text = GestionClinique.Properties.Resources.Logiciel;
             //   SiteWebName.Text = GestionClinique.Properties.Resources.SiteWeb;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerHome(HandleProxy));
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
                        var wndlistsession = Window.GetWindow(this);

                        Grid test = (Grid)wndlistsession.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;
                        Button confirmer = (Button)wndlistsession.FindName("Confirmer");
                        confirmer.IsEnabled = false;
                        Grid tests = (Grid)wndlistsession.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerHome(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void txtNomPatient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNomPatient.Text.Trim() != "")
            {
                TxtCodePatient.IsEnabled = false;
            }
            else
            {
                if (txtNomPatient.Text.Trim() == "")
                {
                    TxtCodePatient.IsEnabled = true;
                }
            }
        }

        private void TxtCodePatient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtCodePatient.Text.Trim() != "")
            {
                txtNomPatient.IsEnabled = false;
            }
            else
            {
                if (TxtCodePatient.Text.Trim() == "")
                {
                    txtNomPatient.IsEnabled = true;
                }
            }
        }





        private void txtNomPatient_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtNomPatient.Text != "")
            {
                if (e.Key != System.Windows.Input.Key.Enter) return;

                // your event handler here
                e.Handled = true;
                List<SVC.Patient> tte = (proxy.GetAllPatient()).Where(n => n.Nom.ToUpper().StartsWith(txtNomPatient.Text.ToUpper().Trim())).ToList();

                if (tte.Count() != 0 )
                {
                    if (memberuser.ModulePatient == true)
                    {
                        ListePatientHome ch = new ListePatientHome(proxy, memberuser, callback, tte,localclient);
                        ch.Show();
                    }
                    else
                    {
                        MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        if (results == MessageBoxResult.OK)
                        {
                            txtNomPatient.Text = "";
                        }
                    }
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Patient n'existe pas"+"\n"+ "voulez vous créer un patient ? ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (memberuser.CréationPatient == true)
                        {
                            NewPatient CLMedecin = new NewPatient(proxy, memberuser, null);
                            CLMedecin.Show();

                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége,GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            if (results == MessageBoxResult.OK)
                            {
                                txtNomPatient.Text = "";
                            }
                        }

                    }else
                    {
                        txtNomPatient.Text = "";
                    }
                }
            }
        }

        private void TxtCodePatient_KeyUp(object sender, KeyEventArgs e)
        {
            if (TxtCodePatient.Text != "" )
            {
                if (e.Key != System.Windows.Input.Key.Enter) return;

                // your event handler here
                e.Handled = true;
                List<SVC.Patient> tte = (proxy.GetAllPatient()).Where(n => n.Id==Convert.ToInt16(TxtCodePatient.Text)).ToList();

                if (tte.Count() != 0 )
                {
                    if (memberuser.ModulePatient == true)
                    {
                        ListePatientHome ch = new ListePatientHome(proxy, memberuser, callback, tte,localclient);
                        ch.Show();
                    }
                    else
                    {
                        MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", "Medicus", MessageBoxButton.OK, MessageBoxImage.Stop);
                        if (results == MessageBoxResult.OK)
                        {
                            TxtCodePatient.Text = "";
                        }
                    }
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Patient n'existe pas"+"\n"+ "voulez vous créer un patient ? ", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (memberuser.CréationPatient == true)
                        {
                            NewPatient CLMedecin = new NewPatient(proxy, memberuser, null);
                            CLMedecin.Show();

                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb
                                , MessageBoxButton.OK, MessageBoxImage.Stop);
                            if (results == MessageBoxResult.OK)
                            {
                                TxtCodePatient.Text = "";
                            }
                        }

                    }else
                    {
                        TxtCodePatient.Text = "";
                    }
                }
            }
        }

        private void btnRendezVous_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.ModuleSalleAttente==true)
            {
                FileDattente.SalleDattente CLsalee = new FileDattente.SalleDattente(proxy, memberuser, callback,localclient);

                CLsalee.Show();
            }else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void TxtNumeroRendezVous_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (TxtNumeroRendezVous.Text != "" )
                {
                    if (e.Key != System.Windows.Input.Key.Enter) return;

                    // your event handler here
                    e.Handled = true;
                    List<SVC.RendezVou> tte = (proxy.GetAllRendezVousAll()).Where(n => n.NuméroRendezVous == TxtNumeroRendezVous.Text).ToList();

                    if (tte.Count != 0 )
                    {
                        if (memberuser.ModulePatient == true)
                        {
                            var tt1 = tte.First();
                            List<SVC.Patient> selectpatient = (proxy.GetAllPatient()).Where(n => n.Nom == tt1.Nom && n.Prénom == tt1.Prénom && n.Id == tt1.CodePatient).ToList();

                            ListePatientHome ch = new ListePatientHome(proxy, memberuser, callback, selectpatient,localclient);
                            ch.Show();
                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            if (results == MessageBoxResult.OK)
                            {
                                TxtNumeroRendezVous.Text = "";
                            }
                        }
                    }
                    else
                    {
                        
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° de Rendez vous inéxistant"+"\n"+"Voulez vous créer un rendez vous",GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);

                              if (result == MessageBoxResult.Yes)
                              {
                            if (memberuser.CréationRendezVous == true)
                            {

                                SVC.RendezVou SelectRendezVous = new RendezVou
                                {
                                    PrisPar = memberuser.UserName,

                                };


                                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, memberuser, callback, 3, null);
                                CLMedecin.Show();



                            }
                            else
                            {
                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action",GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                if (results == MessageBoxResult.OK)
                                {
                                    TxtNumeroRendezVous.Text = "";

                                }
                            }
                        }
                              else
                              {
                                  TxtCodePatient.Text = "";
                              }
                        
                    }
                }
            } catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
            }

        private void btnRendezVous_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnRendezVous.Background;
            imageBrush.Stretch = Stretch.None;
        }

        private void btnRendezVous_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnRendezVous.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnLogoDent_Click(object sender, RoutedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "http://www.medicalogitech.com";
            p.Start();
        }
    }
    }

