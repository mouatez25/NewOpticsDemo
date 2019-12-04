using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for Entête.xaml
    /// </summary>
    public partial class Entête : Page
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.Param paramétre;
        private delegate void FaultedInvokerEntete();
        OpenFileDialog op ;
        public Entête(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrecu,SVC.Membership memberrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                paramétre = (proxy.GetAllParamétre());
                    if (paramétre.CheminLogo.ToString() != "")
                      {

                          if (proxy.DownloadDocumentIsHere(paramétre.CheminLogo.ToString()) == true)
                          {
                              imgLogo.Source = LoadImage(proxy.DownloadDocument(paramétre.CheminLogo.ToString()));
                          }
                     }

                callback = callbackrecu;
                callbackrecu.InsertParamCallbackEvent += new ICallback.CallbackEventHandler16(callbackrecu_Refresh);
                GridParam.DataContext = paramétre;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerEntete(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerEntete(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertParam e)
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
        public void AddRefresh(SVC.Param listmembership)
        {
            try
            { 
            paramétre = (proxy.GetAllParamétre());
            GridParam.DataContext = paramétre;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serverfilepath;
                paramétre.CheminLogo = "Logo.png";
                proxy.UpdateParamétre(paramétre);
                serverfilepath = op.FileName;

               string filepath = "";
                if (serverfilepath != "")
                {

                    filepath = op.FileName;

                    serverfilepath = @"Logo.png";
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)

            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            op = new OpenFileDialog();
            op.Title = "Selectionnez Image";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgLogo.Source = new BitmapImage(new Uri(op.FileName));
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
