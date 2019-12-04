using DevExpress.Xpf.Core;
using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for ColorPalete.xaml
    /// </summary>
    public partial class ColorPalete : DXWindow

    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerColorPalette();
        private WPFBrushList _brushes = new WPFBrushList();
        string colorstring;
        int interfaced;
        SVC.Param par;
        public ColorPalete(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership memberrecu, ICallback callbackrecu, int interfacerecu, string valeurcolor)
        {try
            {
                InitializeComponent();
                lsbBrushes.DataContext = _brushes;
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                colorstring = valeurcolor;
                interfaced = interfacerecu;
                par = proxy.GetAllParamétre();
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void CopyName_Click(object sender, RoutedEventArgs e)
        {try
            { 
            if (lsbBrushes.SelectedIndex != -1)
                Clipboard.SetText(((WPFBrush)lsbBrushes.SelectedItem).Name);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lsbBrushes.SelectedItem != null && interfaced == 1)
                {
                    var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                    par.RdvPresent = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                    proxy.UpdateParamétreAsync(par);
                }
                else
                {
                    if (lsbBrushes.SelectedItem != null && interfaced == 2)
                    {
                        var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                        par.RdvNoPresent = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                        proxy.UpdateParamétreAsync(par);
                    }else
                    {
                        if (lsbBrushes.SelectedItem != null && interfaced == 3)
                        {
                            var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                            par.SalleAttenteTjr = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                            proxy.UpdateParamétreAsync(par);
                        }else
                        {
                            if (lsbBrushes.SelectedItem != null && interfaced == 4)
                            {
                                var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                par.SalleAttenteQuit = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                proxy.UpdateParamétreAsync(par);
                            }
                            else
                            {
                                if (lsbBrushes.SelectedItem != null && interfaced == 5)
                                {
                                    var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                    par.MedecinSalleTjr = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                    proxy.UpdateParamétreAsync(par);

                                }
                                else
                                {
                                    if (lsbBrushes.SelectedItem != null && interfaced == 6)
                                    {
                                        var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                        par.MedecinSalleNON = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                        proxy.UpdateParamétreAsync(par);

                                    }else
                                    {
                                        if (lsbBrushes.SelectedItem != null && interfaced == 7)
                                        {
                                            var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                            par.VisteRéglé = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                            proxy.UpdateParamétreAsync(par);

                                        }else
                                        {
                                            if (lsbBrushes.SelectedItem != null && interfaced == 8)
                                            {
                                                var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                                par.VisteNonRéglé = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                                proxy.UpdateParamétreAsync(par);

                                            }
                                            else
                                            {
                                                if (lsbBrushes.SelectedItem != null && interfaced == 9)
                                                {
                                                    var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                                    par.CasTraiter = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                                    proxy.UpdateParamétreAsync(par);

                                                }else
                                                {
                                                    if (lsbBrushes.SelectedItem != null && interfaced ==10)
                                                    {
                                                        var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                                        par.CasNonTraiter= ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                                        proxy.UpdateParamétreAsync(par);

                                                    }
                                                    else
                                                    {
                                                        if (lsbBrushes.SelectedItem != null && interfaced == 11)
                                                        {
                                                            var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                                            par.ProthèseCommandé = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                                            proxy.UpdateParamétreAsync(par);

                                                        }
                                                        else
                                                        {
                                                            if (lsbBrushes.SelectedItem != null && interfaced == 12)
                                                            {
                                                                var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                                                par.ProthèseNonCommandé = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                                                proxy.UpdateParamétreAsync(par);

                                                            }
                                                            else
                                                            {
                                                                if (lsbBrushes.SelectedItem != null && interfaced == 13)
                                                                {
                                                                    var vff = (((WPFBrush)lsbBrushes.SelectedItem).Hex);
                                                                    par.Etatbuccal = ((WPFBrush)lsbBrushes.SelectedItem).Hex;
                                                                    proxy.UpdateParamétreAsync(par);

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
           //     this.WindowTitleBrush = Brushes.Red;

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerColorPalette(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerColorPalette(HandleProxy));
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
