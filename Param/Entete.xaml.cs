using Microsoft.Win32;
using NewOptics.Administrateur;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Param
{
    /// <summary>
    /// Interaction logic for Param.xaml
    /// </summary>
    public partial class Entete : Page
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;
        SVC.Param parametre;
     
        SVC.Client localclient;
        OpenFileDialog op = new OpenFileDialog();
        private delegate void FaultedInvokerListeParamEntete();
      
        public Entete(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Client clientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;

                memberuser = memberrecu;

                callback = callbackrecu;
                localclient = clientrecu;
                parametre = proxy.GetAllParamétre();
                entetegrid.DataContext = parametre;

                if (parametre.logo.ToString() != "")
                {

                    if (proxy.DownloadDocumentIsHere(parametre.logo.ToString()) == true)
                    {
                        imgPhoto.Source = LoadImage(proxy.DownloadDocument(parametre.logo.ToString()));
                    }
                }
                callbackrecu.InsertParamCallbackEvent += new ICallback.CallbackEventHandler16(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.Param listmembership)
        {
            try
            {

              
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeParamEntete(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeParamEntete(HandleProxy));
                return;
            }
            HandleProxy();
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.ModuleParametreAcces==true)
                {
                    try
                    {
                        string serverfilepath;
                        parametre.logo = "Paramétre/Logo.png";
                      
                        serverfilepath = op.FileName;

                        string filepath = "";
                        if (serverfilepath != "")
                        {

                            filepath = op.FileName;

                            serverfilepath = @"Paramétre/Logo.png";
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
                        bool succes = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateParamétre(parametre);
                            ts.Complete();
                            succes = true;
                        }

                        if (succes == true)
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }

                    }
                    catch (Exception ex)

                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }



            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
