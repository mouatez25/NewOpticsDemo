
using NewOptics.Administrateur;
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

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for ImageProduit.xaml
    /// </summary>
    public partial class ImageProduit : Window
    {
        SVC.Produit special;
        SVC.ServiceCliniqueClient proxy;

        private delegate void FaultedInvokerImageProduitOrdonnance();
        SVC.MembershipOptic Membership;
        ICallback callback;
        public ImageProduit(SVC.ServiceCliniqueClient proxyrecu, SVC.Produit spécialtiérecu, SVC.MembershipOptic membershirecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                special = spécialtiérecu;
                Membership = membershirecu;
                callback = callbackrecu;

                if (special != null)
                {
                    FournVousGrid.DataContext = special;
                    /*****************************famille produit*******************/




                    if (special.photo.ToString() != "")
                    {

                        if (proxy.DownloadDocumentIsHere(special.photo.ToString()) == true)
                        {
                            imgPhoto.Source = LoadImage(proxy.DownloadDocument(special.photo.ToString()));

                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention la photo du produit n'existe plus dans le serveur", "Medicus", MessageBoxButton.OK, MessageBoxImage.Error);


                        }
                    }



                }
            }
            catch (Exception ex)
            {
                HandleProxy();
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImageProduitOrdonnance(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImageProduitOrdonnance(HandleProxy));
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
    }
}
