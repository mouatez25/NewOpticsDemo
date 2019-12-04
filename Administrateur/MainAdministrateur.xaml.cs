using DevExpress.Xpf.Core;
using GestionClinique.SVC;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for MainAdministrateur.xaml
    /// </summary>
    public partial class MainAdministrateur : DXWindow
    {



        SVC.ServiceCliniqueClient proxy = null;
        SVC.Membership MemBerShip;
        SVC.Client localClient = null;
        string username;
        public ICallback callback;
        public event EventHandler AddEvent;
        Dictionary<ListBoxItem, SVC.Client> OnlineClients = new Dictionary<ListBoxItem, Client>();
    
        bool ChatOpened = false;
        //List<SVC.Client> ListeClients = new List<Client>();
        string titlesucce;
        Brush colortitle;
        bool connectionok = false;
        private delegate void FaultedInvoker();
        public MainAdministrateur()
        {
            try
            { 
            InitializeComponent();
           
          
            textBlockss.Content = GestionClinique.Properties.Resources.Logiciel + ".Administrateur";
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
                loginTxtBoxIP.Text = "192.168.1.10";
                }
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
                  
                        Confirmer.IsEnabled = false;
                         break;
                    case CommunicationState.Opened:
                        gridhome.Visibility = Visibility.Visible;
                     
                        gridAuthentification.Visibility = Visibility.Collapsed;
                     

                         using (StreamWriter writer = new StreamWriter(@"IPTEXTE.txt", false))
                        {

                            writer.WriteLine(loginTxtBoxIP.Text);
                        }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           try
            { 
           FrameInterieur.NavigationService.Navigate(new RendezVous(proxy,callback, MemBerShip));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
            try
            { 
            FrameInterieur.NavigationService.Navigate(new ListeSession(proxy,callback, MemBerShip));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

     
    
      
        public bool authentification(SVC.ServiceCliniqueClient testproxy, string usernameTextbox)
        {

            var query = from c in testproxy.GetAllMembership()
                        select c;

            var results = query.ToList();
            var disponible = results.Where(list1 => list1.MotDePasse == usernameTextbox).FirstOrDefault();
            MemBerShip = disponible;
            if (disponible != null && disponible.Actif == true && disponible.ModuleAdministrateur==true)
            {
                MemBerShip = disponible;
                username = disponible.UserName;

                return true;

            }
            else
            {
                this.Title = "Cette Session n'existe pas ou inactif";
                 this.localClient = null;
                proxy.Close();
                return false;
            }

        }
        void proxy_ConnecttCompleted(object sender, ConnectCompletedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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

                     //   this.localClient.Status = true;

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
                //    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

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
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                proxy.DisconnectAsync(this.localClient);
            }catch(Exception ex)
            {

            }
           
          
        }

      

        private void spécialitemenu_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
           ListeSpécialitéMedecin CLSession = new ListeSpécialitéMedecin(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Motifemenu_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            ListeMotifVisite CLSession = new ListeMotifVisite(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void TyepCasemenu_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            ListeTypeCas CLSession = new ListeTypeCas(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void BtnEntête_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            FrameInterieur.NavigationService.Navigate(new Entête(proxy, callback, MemBerShip));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ColorSalleAttente_Click(object sender, RoutedEventArgs e)
        {
            try

            { 
           ParamétreColor CLSession = new ParamétreColor(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void MotifDepensemenu_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            
            ListMotifDepense CLSession = new ListMotifDepense(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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

                          //  this.localClient.Status = true;

                            proxy.TestConnect(localClient.UserName);

                            TestConnecttCompleted();




                        }


                    }
                    else
                    {
                        HandleProxy();
                    }



                }
                else
                {
                    HandleProxy();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

       

       

        private void Catalogue_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
           ListeDesCatalogue CLSession = new ListeDesCatalogue(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Acte_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            ListeActe CLSession = new ListeActe(proxy, MemBerShip, callback);

            CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Questionnaire_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnuser_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var imageBrush = (ImageBrush)btnuser.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnuser_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnuser.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnuser_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnuser.Background;
            imageBrush.Stretch = Stretch.UniformToFill;
        }

        private void btnMdecin_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnMdecin.Background;
            imageBrush.Stretch = Stretch.UniformToFill;
        }

        private void btnMdecin_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)btnMdecin.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void btnMdecin_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            var imageBrush = (ImageBrush)btnMdecin.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void BtnEntête_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            var imageBrush = (ImageBrush)BtnEntête.Background;
            imageBrush.Stretch = Stretch.Fill;
        }

        private void BtnEntête_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)BtnEntête.Background;
            imageBrush.Stretch = Stretch.UniformToFill;
        }

        private void BtnEntête_MouseMove(object sender, MouseEventArgs e)
        {
            var imageBrush = (ImageBrush)BtnEntête.Background;
            imageBrush.Stretch = Stretch.Uniform;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Authentification cl = new Authentification();
                cl.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void ParamOrdo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrdoParam CLSession = new OrdoParam(proxy, callback, MemBerShip);

                CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void quses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DictionnaireListe CL = new DictionnaireListe(proxy, MemBerShip, callback);
                CL.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
