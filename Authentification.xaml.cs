using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Threading;
using System.Globalization;
using System.IO;
using System.Windows.Threading;
using System.ServiceModel;
using Madaa.Lib.Win.Services.Msdtc;
using NetFwTypeLib;
using NewOptics.SVC;
using NewOptics.Administrateur;

namespace NewOptics
{
    /// <summary>
    /// Interaction logic for Authentification.xaml
    /// </summary>
    public partial class Authentification : Window
    {
        ServiceCliniqueClient proxy = null;
        SVC.MembershipOptic   Membership;
        SVC.Client localClient = null;
        string username;
        public ICallback callback;
        public event EventHandler AddEvent;
        Dictionary<ListBoxItem, SVC.Client> OnlineClients = new Dictionary<ListBoxItem, Client>();
        bool connectionok = false;
        private MsdtcManager msdtcManager;
        DispatcherTimer dispatcherTimer;
        int count = 0;
        int countrasaction = 0;
        private delegate void FaultedInvoker();

        public Authentification()
        {
            try
            {
                InitializeComponent();

                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                // CultureInfo cci = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
                Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
                // cci.NumberFormat.CurrencyDecimalSeparator = ",";
                /*    msdtcManager = new MsdtcManager(false, 1000);
                    msdtcManager.EnableWindowsFirewallException(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL);
                    msdtcManager.EnableWindowsFirewallException(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN);
                    msdtcManager.EnableWindowsFirewallException(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE);
                    msdtcManager.EnableWindowsFirewallException(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC);
                    // if (msdtcManager.IsServiceInstalled() == false)
                    // {
                    msdtcManager.InstallService();

                    // }


                    //msdtcManager.EnableWindowsFirewallException(GetNetFwProfileType());
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();*/




                textBlockss.Content = NewOptics.Properties.Resources.Logiciel;
                /*   if (File.Exists("IPTEXTE.txt"))
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
                   }*/
                if (!Directory.Exists(System.Environment.CurrentDirectory + "/Produit/PhotoProduit"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(System.Environment.CurrentDirectory + "/Produit/PhotoProduit");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());

            }
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                count++;
                countrasaction++;



                if (countrasaction == Convert.ToInt32(10))
                {
                    if (msdtcManager.IsServiceInstalledAndRunning() != true)
                    {
                        msdtcManager.StartService();
                    }
                }
                else
                {
                    if (countrasaction == Convert.ToInt32(15))
                    {
                        msdtcManager.NetworkDtcAccess = NetworkDTCAccessStatus.On;
                        msdtcManager.AllowInbound = true;
                        msdtcManager.AllowOutbound = true;
                        msdtcManager.AllowClientDistant = true;
                        msdtcManager.AllowAdminDistant = true;


                        msdtcManager.AuthenticationRequired = AuthenticationRequiredType.NoAuthenticationRequired;
                        msdtcManager.EnableXaTransactions = true;
                        msdtcManager.EnableSnaLuTransactions = true;

                        if (msdtcManager.NeedRestart)
                        {
                            msdtcManager.RestartService();
                        }
                        dispatcherTimer.Stop();
                    }
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
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


                            Confirmer.IsEnabled = false;
                            textBlockss.Content = "User Hors ligne ";
                            textBlockss.Visibility = Visibility.Visible;

                            break;
                        case CommunicationState.Opened:
                            if (connectionok == true)
                            {
                                DXWindowMain cl = new DXWindowMain(proxy, Membership, callback, localClient);
                                cl.Show();


                                /*   using (StreamWriter writer = new StreamWriter(System.Environment.CurrentDirectory + "/IPTEXTE.txt", false))

                                   {

                                       writer.WriteLine(loginTxtBoxIP.Text);
                                   }*/


                                if (checkBoxImage.IsChecked == true)
                                {
                                    var image = LoadImage(proxy.DownloadDocument(proxy.GetAllParamétre().logo.ToString()));
                                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                                    Guid photoID = System.Guid.NewGuid();
                                    String photolocation = System.Environment.CurrentDirectory + "/Logo.png";
                                    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));
                                    using (var filestream = new FileStream(photolocation, FileMode.Create))
                                        encoder.Save(filestream);
                                    //System.Windows.MessageBox.Show((proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().logo.ToString())).ToString());
                                }
                                this.Close();
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            /* try
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
            /*   proxy.Endpoint.Address = new EndpointAddress("net.tcp://" + loginTxtBoxIP.Text.ToString() + ":" + serviceListenPort + servicePath);
               //

               proxy.Open();

               var oper = authentification(proxy, txtMotDePasse.Password);
               if (oper == true)
               {


                   this.localClient = new SVC.Client();
                   this.localClient.UserName = Membership.Username;
                   this.localClient.Actif = Convert.ToBoolean(Membership.Actif);

                   //      this.localClient.Status = true;

                   proxy.TestConnect(localClient.UserName);

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


           // // loginLabelStatus.Content = "Offline";
           //Confirmer.IsEnabled = false;
           string Mess = "Error: " + ex.Message;
           textBlockss.Content = Mess;
           textBlockss.Foreground = Brushes.Red;
           textBlockss.Visibility = Visibility.Visible;
           //    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

       }*/
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
                        this.localClient.UserName = Membership.Username;
                        this.localClient.Actif = Convert.ToBoolean(Membership.Actif);

                        //      this.localClient.Status = true;

                        proxy.TestConnect(localClient.UserName);

                        TestConnecttCompleted();




                    }
                    else
                    {
                        string Mess = "Vous ne pouvez pas connectez avec cette session";
                        textBlockss.Content = Mess;
                        textBlockss.Foreground = Brushes.Red;
                        textBlockss.Visibility = Visibility.Visible;
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


                // // loginLabelStatus.Content = "Offline";
                //Confirmer.IsEnabled = false;
                string Mess = "Error: " + ex.Message;
                textBlockss.Content = Mess;
                textBlockss.Foreground = Brushes.Red;
                textBlockss.Visibility = Visibility.Visible;
                //    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }
        /* void TestConnecttCompleted()
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

                     bool test = proxy.VerificationCle1();

                     if (test)
                     {
                         proxy.ConnectAsync(this.localClient);

                         //   MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(i.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                         var listconnected = proxy.GetAllClient().Find(n => n.UserName == localClient.UserName);

                         if (listconnected != null)
                         {
                             connectionok = true;
                             localClient = listconnected;
                             proxy.ConnectCompleted += new EventHandler<ConnectCompletedEventArgs>(proxy_ConnecttCompleted);


                         }
                         else
                         {
                             textBlockss.Content = "Nombre de connécté a été ateint ";//#3A8EBA
                             textBlockss.Foreground = Brushes.Red;
                             textBlockss.Visibility = Visibility.Visible;
                             proxy.Close();
                             connectionok = false;
                         }

                     }
                     else
                     {
                         textBlockss.Content = "la clé d'activation est absente ";//#3A8EBA
                         textBlockss.Foreground = Brushes.Red;
                         textBlockss.Visibility = Visibility.Visible;
                         proxy.Close();
                         connectionok = false;
                     }
                 }
                 else
                 {

                     textBlockss.Content = "Cette session est déja connéctée";//#3A8EBA
                     textBlockss.Foreground = Brushes.Red;
                     textBlockss.Visibility = Visibility.Visible;
                     proxy.Close();
                     connectionok = false;

                 }
             }
             catch (Exception ex)
             {
                 MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
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

                    //   MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(i.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    var listconnected = proxy.GetAllClient().Find(n => n.UserName == localClient.UserName);

                    if (listconnected != null)
                    {
                        connectionok = true;
                        localClient = listconnected;
                        proxy.ConnectCompleted += new EventHandler<ConnectCompletedEventArgs>(proxy_ConnecttCompleted);


                    }
                    else
                    {
                        textBlockss.Content = "Nombre de connécté a été ateint ou la clé d'activation est absente ";//#3A8EBA
                        textBlockss.Foreground = Brushes.Red;
                        textBlockss.Visibility = Visibility.Visible;
                        proxy.Close();
                        connectionok = false;
                    }
                }
                else
                {

                    textBlockss.Content = "Cette session est déja connéctée";//#3A8EBA
                    textBlockss.Foreground = Brushes.Red;
                    textBlockss.Visibility = Visibility.Visible;
                    proxy.Close();
                    connectionok = false;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public bool authentification(SVC.ServiceCliniqueClient testproxy, string usernameTextbox)
        {

            /*  var query = from c in testproxy.GetAllMembershipOptics()
                          select c;

              var results = query.ToList();
              var disponible = results.Where(list1 => list1.MotDePasse == usernameTextbox).FirstOrDefault();*/
            if (usernameTextbox != "Admin" && usernameTextbox != "Administrateur" && usernameTextbox != "ADMIN" && usernameTextbox != "ADMINISTRATEUR" && usernameTextbox != "administrateur" && usernameTextbox != "admin")
            {
                Membership = new MembershipOptic
                {
                    Username = usernameTextbox,
                    Actif = true,
                    ActiveServer = true,
                    CreationAchat = true,
                    CreationAms = true,
                    CreationCaisse = true,
                    CreationCommande = true,
                    CreationDossierClient = true,
                    CreationFichier = true,
                    CreationPlanning = true,
                    CreationRecouv = true,
                    DiscussionPrivé = true,
                    EnvoiReceptMessage = true,
                    EnvoiRécéptFichier = true,
                    ImpressionAchat = true,
                    ImpressionAms = true,
                    ImpressionCaisse = true,
                    ImpressionCommande = true,
                    ImpressionDossierClient = true,
                    ImpressionFichier = true,
                    ImpressionPlanning = true,
                    ImpressionRecouv = true,
                    ModificationAchat = true,
                    ModificationAms = true,
                    ModificationCaisse = true,
                    ModificationCommande = true,
                    ModificationDossierClient = true,
                    ModificationFichier = true,
                    ModificationPlanning = true,
                    ModificationRecouv = true,
                    ModuleAchat = true,
                    ModuleAms = true,
                    ModuleCaisse = true,
                    ModuleChat = true,
                    ModuleCommande = true,
                    ModuleDossierClient = true,
                    ModuleFichier = true,
                    ModuleParametreAcces = true,
                    ModulePlanning = true,
                    ModuleRecouv = true,
                    ModuleStatistiqueAcces = true,
                    ModuleVenteCompoirAcces = true,
                    SuppressinAchat = true,
                    SuppressionAms = true,
                    SuppressionCaisse = true,
                    SuppressionFichier = true,
                    SuppressionPlanning = true,
                    SuppressionRecouv = true,
                    SupressionCommande = true,
                    SupressionDossierClient = true,
                    
                };
                username = usernameTextbox;
                return true;
            }
            else
            {
                return false;
            }

            /*= disponible;
        if (disponible != null && disponible.Actif == true)
        {
            MembershipOptic = disponible;
            username = disponible.Username;

            return true;

        }
        else
        {
            textBlockss.Content = "Cette Session n'existe pas ou inactif";
            textBlockss.Foreground = Brushes.Red;
            textBlockss.Visibility = Visibility.Visible;
            this.localClient = null;
            proxy.Close();
            return false;
        }*/

        }
        void proxy_ConnecttCompleted(object sender, ConnectCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                textBlockss.Content = e.Error.Message;
                textBlockss.Foreground = Brushes.Red;
                textBlockss.Visibility = Visibility.Visible;
            }
            else if (e.Result)
            {
                HandleProxy();
            }
            else if (!e.Result)
            {
                textBlockss.Content = e.Error.Message;
                textBlockss.Foreground = Brushes.Red;
                textBlockss.Visibility = Visibility.Visible;
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
                            this.localClient.UserName = Membership.Username;
                            this.localClient.Actif = Convert.ToBoolean(Membership.Actif);

                            //      this.localClient.Status = true;

                            proxy.TestConnect(localClient.UserName);

                            TestConnecttCompleted();




                        }
                        else
                        {
                            string Mess = "Vous ne pouvez pas connectez avec cette session";
                            textBlockss.Content = Mess;
                            textBlockss.Foreground = Brushes.Red;
                            textBlockss.Visibility = Visibility.Visible;
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
                // // loginLabelStatus.Content = "Offline";
                //Confirmer.IsEnabled = false;
                string Mess = "Error: " + ex.Message;
                textBlockss.Content = Mess;
                textBlockss.Foreground = Brushes.Red;
                textBlockss.Visibility = Visibility.Visible;
                //    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void Proxy_TestConnectCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnLogoDent_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}