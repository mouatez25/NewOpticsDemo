using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
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

namespace NewOptics.Appointement
{
    /// <summary>
    /// Interaction logic for ColorPaleteStatus.xaml
    /// </summary>
    public partial class ColorPaleteStatus : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;

        private delegate void FaultedInvokerColorPaletteStatus();
        private WPFBrushList _brushes = new WPFBrushList();
        SVC.Statu motiftableclass;
        Button background;
        SVC.Param par;
        public ColorPaleteStatus(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, SVC.Statu motifrecu, Button backgroundrecu)
        {
            try
            {
                InitializeComponent();
                lsbBrushes.DataContext = _brushes;
                proxy = proxyrecu;
                memberuser = memberrecu;

                motiftableclass = motifrecu;
                background = backgroundrecu;
                par = proxy.GetAllParamétre();
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        private void CopyHex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lsbBrushes.SelectedIndex != -1)
                    Clipboard.SetText(((WPFBrush)lsbBrushes.SelectedItem).Hex);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string couleur = "";
                couleur = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                motiftableclass.Color = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                var converter = new System.Windows.Media.BrushConverter();
                background.Background = (Brush)converter.ConvertFromString(couleur);
                //    background.Background = ((Brush)lsbBrushes.SelectedItem);
                this.Close();
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerColorPaletteStatus(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerColorPaletteStatus(HandleProxy));
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
        private void CopyName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lsbBrushes.SelectedIndex != -1)
                    Clipboard.SetText(((WPFBrush)lsbBrushes.SelectedItem).Name);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

    }
}
