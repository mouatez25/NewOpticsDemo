using GestionClinique.Administrateur;
using GestionClinique.Caisse;
using GestionClinique.DocumentImportant;
using GestionClinique.EtatEtRapport;
using GestionClinique.FileDattente;

using GestionClinique.Patient;
using GestionClinique.RendezVous;
using GestionClinique.Stock;
using GestionClinique.SVC;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        SVC.ServiceCliniqueClient proxy = null;
        SVC.Membership MemBerShip;
        SVC.Client localClient = null;
        string username;
        public ICallback callback;
        public event EventHandler AddEvent;
        Dictionary<ListBoxItem, SVC.Client> OnlineClients = new Dictionary<ListBoxItem, Client>();
        Chat CL;
        bool ChatOpened = false;
        private delegate void FaultedInvoker();
        string titlesucce;
        Brush colortitle;
        bool connectionok = false;
        public MainWindow()
        {
            InitializeComponent();
            chatbutton.IsEnabled = false;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            // CultureInfo cci = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            // cci.NumberFormat.CurrencyDecimalSeparator = ",";
            MenuPrincipale.IsEnabled = false;
            titlesucce = this.WindowTitle;
            colortitle = this.WindowTitleBrush;

            textBlockss.Content = GestionClinique.Properties.Resources.Logiciel;
            if (File.Exists("IPTEXTE.txt"))
            {
                using (StreamReader sr = new StreamReader("IPTEXTE.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadLine();
                    loginTxtBoxIP.Text = line;
                }
            }
            else
            {
                loginTxtBoxIP.Text = "127.0.0.1";
            }
        }



        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void HandleProxy()
        {
            try
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
                            gridhome.Visibility = Visibility.Collapsed;
                            textBlock.Content = "Erreur de communication entre le Server et le client";
                            gridAuthentification.Visibility = Visibility.Visible;
                            chatbutton.IsEnabled = false;
                            MenuPrincipale.IsEnabled = false;
                            if (ChatOpened == true)
                            {
                                AddEvent(CL, new EventArgs());
                            }

                            Confirmer.IsEnabled = false;
                            labelSession.Text = "User Hors ligne ";
                            MenuStackPanel.Visibility = Visibility.Hidden;
                            chatbutton.Visibility = Visibility.Hidden;

                            break;
                        case CommunicationState.Opened:
                            if (connectionok == true)
                            {
                                FrameInterieur.NavigationService.Navigate(new Home(proxy, MemBerShip, callback));
                                gridhome.Visibility = Visibility.Visible;

                                using (StreamWriter writer = new StreamWriter(System.Environment.CurrentDirectory + "/IPTEXTE.txt", false))
                                //       using (StreamWriter writer = new StreamWriter("IPTEXTE.txt", false))

                                {

                                    writer.WriteLine(loginTxtBoxIP.Text);
                                }
                                gridAuthentification.Visibility = Visibility.Collapsed;
                                chatbutton.IsEnabled = true;
                                MenuPrincipale.IsEnabled = true;
                                labelSession.Text = "Bonjour " + localClient.UserName;
                                this.Title = titlesucce;
                                statusmenu.IsChecked = localClient.Status;
                                this.WindowTitleBrush = colortitle;
                                MenuStackPanel.Visibility = Visibility.Visible;
                                chatbutton.Visibility = Visibility.Visible;
                                //  callback.callbackMessageRecu += new ICallback.CallbackEventHandler3(callbackrecu_callbackReceive);
                                if (MemBerShip.ModuleChat == true)
                                {
                                    Chat cl = new Chat(localClient, callback, proxy, this, MemBerShip);
                                    cl.Show();
                                    cl.WindowState = WindowState.Minimized;
                                }
                                var image = LoadImage(proxy.DownloadDocument(proxy.GetAllParamétre().CheminLogo.ToString()));
                                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                                Guid photoID = System.Guid.NewGuid();
                                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";
                                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));
                                using (var filestream = new FileStream(photolocation, FileMode.Create))
                                    encoder.Save(filestream);
                            }
                            else
                            {
                                proxy.Abort();
                                proxy = null;
                            }
                            break;
                        case CommunicationState.Opening:



                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void objImage_DownloadCompleted(object sender, EventArgs e)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)sender));

            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);
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
        void callbackrecu_callbackReceive(object source, CallbackEventMessageRecu e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddMessageRecu(e.clientleav);
            }));
        }
        public void AddMessageRecu(Message msg)
        {
            
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {try
            { 
            if (MemBerShip.ModuleChat == true)
            {
                CL = new Chat(localClient, callback, proxy, this,MemBerShip);
                ChatOpened = true;
                CL.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége,GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }
        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proxy == null)
                {



                    callback = new ICallback();
                    InstanceContext cntx = new InstanceContext(callback);
                    proxy = new SVC.ServiceCliniqueClient(cntx);
                    string servicePath = proxy.Endpoint.ListenUri.AbsolutePath;
                    string serviceListenPort = proxy.Endpoint.Address.Uri.Port.ToString();

                    ////////////////////////*************a supprimer***************///////

                    //this.localClient = new SVC.Client();
                    //this.localClient.UserName = txtMotDePasse.Password;
                    proxy.Endpoint.Address = new EndpointAddress("net.tcp://" + loginTxtBoxIP.Text.ToString() + ":" + serviceListenPort + servicePath);
                    //

                    proxy.Open();

                    var oper = authentification(proxy, txtMotDePasse.Password);
                    if (oper == true)
                    {


                        this.localClient = new SVC.Client();
                        this.localClient.UserName = MemBerShip.UserName;
                        this.localClient.Actif = Convert.ToBoolean(MemBerShip.Actif);
                     /*   if (chboxstatus.IsChecked == true)
                        {
                            this.localClient.Status = true;
                        }
                        else
                        {
                            if (chboxstatus.IsChecked == false)
                            {
                                this.localClient.Status = false;
                            }
                        }*/
                      // this.localClient.Status = Convert.ToBoolean(chboxstatus.IsChecked);

                        proxy.TestConnect(localClient);
                        TestConnecttCompleted();



                    }


                }
                else
                {
                    HandleProxy();
                }
            }
            /////


            catch (Exception ex)
            {

                this.Title = "Serveur Invalide";//#3A8EBA
                this.WindowTitleBrush = Brushes.Red;
                // // loginLabelStatus.Content = "Offline";
                //Confirmer.IsEnabled = false;
                string Mess = "Error: " + ex.Message;
                textBlock.Content = Mess;
            }

        }
        void TestConnecttCompleted()
        {
            try
            {
                var disponible = (proxy.GetAllClient()).Where(list1 => list1.UserName == localClient.UserName).FirstOrDefault();


                if (disponible == null)
                {


                    proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                    proxy.InnerDuplexChannel.Opened += new EventHandler(InnerDuplexChannel_Opened);
                    proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                    //  proxy.ConnectAsync(this.localClient);

                    proxy.ConnectAsync(this.localClient);

                    //   MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(i.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    var listconnected = proxy.GetAllClient().Find(n => n.UserName == localClient.UserName);

                    if (listconnected != null)
                    {
                        connectionok = true;

                        proxy.ConnectCompleted += new EventHandler<ConnectCompletedEventArgs>(proxy_ConnecttCompleted);


                    }
                    else
                    {
                        this.Title = "Nombre de connécté a été ateint";//#3A8EBA
                        this.WindowTitleBrush = Brushes.Red;
                        proxy.Close();
                        connectionok = false;
                    }
                }
                else
                {

                    this.Title = "Cette session est déja connéctée";//#3A8EBA
                    this.WindowTitleBrush = Brushes.Red;
                    proxy.Close();
                    connectionok = false;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public bool authentification(SVC.ServiceCliniqueClient testproxy, string usernameTextbox)
        {

            var query = from c in testproxy.GetAllMembership()
                        select c;

            var results = query.ToList();
            var disponible = results.Where(list1 => list1.MotDePasse == usernameTextbox).FirstOrDefault();
            MemBerShip = disponible;
            if (disponible != null && disponible.Actif == true)
            {
                MemBerShip = disponible;
                username = disponible.UserName;

                return true;

            }
            else
            {
                this.Title = "Cette Session n'existe pas ou inactif";
                this.WindowTitleBrush = Brushes.Red;
                this.localClient = null;
                proxy.Close();
                return false;
            }

        }
        void proxy_ConnecttCompleted(object sender, ConnectCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                textBlock.Content = e.Error.Message;
                textBlock.Foreground = Brushes.Blue;
            }
            else if (e.Result)
            {
                HandleProxy();
            }
            else if (!e.Result)
            {
                textBlock.Content = e.Error.Message;
                textBlock.Foreground = Brushes.Black;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemBerShip.ModuleRendezVous == true)
                {
                    FrameInterieur.NavigationService.Navigate(new RendezVous.RendezVous(proxy, MemBerShip, callback));
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            } catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.ModuleRendezVous == true)
            {
                FrameInterieur.NavigationService.Navigate(new RendezVous.RendezVous(proxy, MemBerShip, callback));
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
           }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
    
             }
           }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.ModulePatient == true)
            {
                FrameInterieur.NavigationService.Navigate(new ListePatient(proxy, MemBerShip, callback));
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            try { 
            if (MemBerShip.ModulePatient == true)
            {
                FrameInterieur.NavigationService.Navigate(new ListePatient(proxy, MemBerShip, callback));
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

    }
}
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.ModuleAdministrateur == true)
            {
                proxy.Disconnect(localClient);
                MainAdministrateur test = new MainAdministrateur();
                test.Show();
                this.Close();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
             }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

               }
}

        private void btnListeRendezVous_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.ModuleRendezVous == true)
            {
                FrameInterieur.NavigationService.Navigate(new ListeRendezVous(proxy, MemBerShip, callback));
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            { 
            FrameInterieur.NavigationService.Navigate(new Home(proxy, MemBerShip, callback));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
            if (MemBerShip.CréationRendezVous == true)
            {
                SVC.RendezVou SelectRendezVous = new RendezVou
                {
                    PrisPar = MemBerShip.UserName,

                };


                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, MemBerShip, callback, 3, null);
                CLMedecin.Show();



                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (proxy != null)
            {
                if (proxy.State == CommunicationState.Faulted)
                {

                }
                else
                {
                    proxy.DisconnectAsync(this.localClient);
                }
            }
        }

        private void btnListeDepense_Click(object sender, RoutedEventArgs e)
        {  try
            { 
            if (MemBerShip.ModuleCaisse == true)
            {
                FrameInterieur.NavigationService.Navigate(new Caisse.Caisse(proxy, MemBerShip, callback, localClient));

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
                 }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
          }




        private void fichef_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.CréationAchat == true)
            {
                AjouterFournisseur CLSession = new AjouterFournisseur(proxy, null, MemBerShip);
                CLSession.Show();

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


            }
               }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
}

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.CréationAchat == true)
            {



                ListeFournisseur CLMedecin = new ListeFournisseur(proxy, MemBerShip, callback);
                CLMedecin.Show();
                
            }else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.ModificationAchat== true)
            {
                MonStock CLMedecin = new MonStock(proxy, MemBerShip, callback, localClient);
                CLMedecin.Show();
            }else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

    }
}

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.CréationAchat == true)
            {
                AjouterProduitOrdonnance CLSession = new AjouterProduitOrdonnance(proxy, null, MemBerShip,callback);
                CLSession.Show();

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


            }
        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

    }
}

        private void listachat_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.ModuleAchat == true)
            {
                MesAchat CLSession = new MesAchat(proxy, MemBerShip, callback, localClient);
                CLSession.Show();

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Achat_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.CréationAchat == true)
            {
                AjouterFactureAchat CLSession = new AjouterFactureAchat(proxy, MemBerShip, callback, null, localClient);
                CLSession.Show();

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

 

        private void LettretEST_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RichTextEditorSample CL = new RichTextEditorSample(proxy, callback);
                CL.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


            }
        }

        private void btnAgenda_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnAgenda.Background;
            imageBrush.Stretch = Stretch.Fill;

        }

        private void btnAgenda_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnAgenda.Background;
            imageBrush.Stretch = Stretch.Uniform;
            btnAgenda.Width = 80;
            btnAgenda.Height = 80;
        }

        private void btnListePatient_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnListePatient.Background;
            imageBrush.Stretch = Stretch.Fill;

        }

        private void btnListePatient_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnListePatient.Background;
            imageBrush.Stretch = Stretch.Uniform;
            btnListePatient.Width = 80;
            btnListePatient.Height = 80;
        }

        private void btnListeDepense_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnListeDepense.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnListeDepense_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnListeDepense.Background;
            imageBrush.Stretch = Stretch.Uniform;
            btnListeDepense.Width = 80;
            btnListeDepense.Height = 80;
        }

        private void btnHome_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnHome.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnHome.Background;
            imageBrush.Stretch = Stretch.UniformToFill;
        }

        private void btnListeRendezVous_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnListeRendezVous.Background;
            imageBrush.Stretch = Stretch.Uniform;
        }

        private void btnListeRendezVous_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnListeRendezVous.Background;
            imageBrush.Stretch = Stretch.UniformToFill;
        }

        private void btnHome_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var imageBrush = (ImageBrush)btnHome.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnecran_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            EcranFilleAttente cl = new EcranFilleAttente(proxy, MemBerShip, callback);
            cl.Show();
        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

        }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.CréationAchat == true)
            {



                ListeDeProduitPourOronnance CLMedecin = new ListeDeProduitPourOronnance(proxy, MemBerShip, callback);

                CLMedecin.Show();



            }else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

          }      }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemBerShip.CréationAchat == true)
            {
                
                ListeDesDci CLMedecin = new ListeDesDci(proxy, MemBerShip, callback);

                CLMedecin.Show();
                
            }else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

    }
}

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            try { 
            if (MemBerShip.CréationPatient == true)
            {

                NewPatient cl = new NewPatient(proxy, MemBerShip, null);
                cl.Show();

            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

      
       

      

       

      
       
      
        private void RéglementPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool sessionmedecin = false;
                var disponible = (proxy.GetAllMedecin()).Where(list1 => list1.UserName == MemBerShip.UserName).FirstOrDefault();
                if (disponible == null)
                {
                    sessionmedecin = false;
                }
                else
                {
                    sessionmedecin = true;
                }

                if (MemBerShip.ModuleAdministrateur == true || sessionmedecin == true)
                {

                    ReglementPatient cl = new ReglementPatient(proxy, MemBerShip, callback);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void CasatraiterPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool sessionmedecin = false;
                var disponible = (proxy.GetAllMedecin()).Where(list1 => list1.UserName == MemBerShip.UserName).FirstOrDefault();
                if (disponible == null)
                {
                    sessionmedecin = false;
                }
                else
                {
                    sessionmedecin = true;
                }
                if (MemBerShip.ModuleAdministrateur == true || sessionmedecin == true)
                {

                    CasAtraiterPatient cl = new CasAtraiterPatient(proxy, MemBerShip, callback);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Tableaudebord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool sessionmedecin = false;
                var disponible = (proxy.GetAllMedecin()).Where(list1 => list1.UserName == MemBerShip.UserName).FirstOrDefault();
                if (disponible == null)
                {
                    sessionmedecin = false;
                }
                else
                {
                    sessionmedecin = true;
                }
                    if (MemBerShip.ModuleAdministrateur == true || sessionmedecin==true)
                {

                    TableauDeBord cl = new TableauDeBord(proxy, MemBerShip, callback);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtMotDePasse_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (loginTxtBoxIP.Text != "")
                {

                    if (e.Key != System.Windows.Input.Key.Enter) return;

                    // your event handler here
                    e.Handled = true;
                    if (proxy == null)
                    {


                        callback = new ICallback();
                        InstanceContext cntx = new InstanceContext(callback);
                        proxy = new SVC.ServiceCliniqueClient(cntx);
                        string servicePath = proxy.Endpoint.ListenUri.AbsolutePath;
                        string serviceListenPort = proxy.Endpoint.Address.Uri.Port.ToString();

                        ////////////////////////*************a supprimer***************///////

                        //this.localClient = new SVC.Client();
                        //this.localClient.UserName = txtMotDePasse.Password;
                        proxy.Endpoint.Address = new EndpointAddress("net.tcp://" + loginTxtBoxIP.Text.ToString() + ":" + serviceListenPort + servicePath);
                        //

                        proxy.Open();

                        var oper = authentification(proxy, txtMotDePasse.Password);
                        if (oper == true)
                        {


                            this.localClient = new SVC.Client();
                            this.localClient.UserName = MemBerShip.UserName;
                            this.localClient.Actif = Convert.ToBoolean(MemBerShip.Actif);
                            proxy.TestConnect(localClient);
                            TestConnecttCompleted();



                        }


                    }
                    else
                    {
                        HandleProxy();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnLogoDent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNavigateur_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@".\Dent6\index.html");
        }

        private void AchatVVente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemBerShip.ModuleAchat==true)
                {
                    AchatEtVenteProduit cl = new AchatEtVenteProduit(proxy, callback,MemBerShip);
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

        private void Rapport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               CertificatRapport cl = new CertificatRapport(proxy, MemBerShip);
                    cl.Show();
               

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void test4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LivreViewer cl = new LivreViewer();
                cl.Show();
;
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proxy != null)
                {
                    this.localClient.Status = false;
                    proxy.SetAllClient(localClient);
                    proxy.ReConnect(localClient);
                }
            }
            catch
                  (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proxy != null)
                {
                    this.localClient.Status = true;
                    proxy.SetAllClient(localClient);
                    proxy.ReConnect(localClient);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            if (proxy != null)
            {
                if (proxy.State == CommunicationState.Faulted)
                {

                }
                else
                {
                    proxy.DisconnectAsync(this.localClient);
                }
            }
        }
        private void MenuItem_Click_17(object sender, RoutedEventArgs e)
        {
            if (proxy != null)
            {
                if (proxy.State == CommunicationState.Faulted)
                {

                }
                else
                {
                    proxy.DisconnectAsync(this.localClient);
                }
            }
        }
    }
    }
