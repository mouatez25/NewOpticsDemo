using DevExpress.Xpf.Core;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Appointement
{
    /// <summary>
    /// Interaction logic for InsertStatus.xaml
    /// </summary>
    public partial class InsertStatus : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        private delegate void FaultedInvokerAjouterStatus();
        int interfacecreation = 0;
        SVC.Statu motifclasse;
        public InsertStatus(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, SVC.Statu motifrecu)
        {
            try
            {
                InitializeComponent();
                memberuser = memberrecu;
                proxy = proxyrecu;

                if (motifrecu == null)
                {
                    interfacecreation = 1;
                    motifclasse = new SVC.Statu
                    {
                        Color ="#000000",
                    };
                    MotifGrid.DataContext = motifclasse;

                }
                else
                {
                    interfacecreation = 2;
                    motifclasse = motifrecu;
                    MotifGrid.DataContext = motifclasse;

                    var converter = new System.Windows.Media.BrushConverter();
                    BtnColor.Background = (Brush)converter.ConvertFromString((motifclasse).Color);
                }



                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAjouterStatus(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAjouterStatus(HandleProxy));
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
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationPlanning == true && interfacecreation == 1 && txtRaison.Text != "")
                {
                    if (motifclasse.Color != "")
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {

                        proxy.InsertStatu(motifclasse);
                        ts.Complete();

                       
                           
                     }
                        proxy.AjouterStatuRefresh();
                    }
                  
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    if (memberuser.ModificationAchat == true && interfacecreation == 2)
                    {
                        if (motifclasse.Color != "")
                        {
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateStatu(motifclasse);
                            ts.Complete();
                        }

                            
                                proxy.AjouterStatuRefresh();
                        }
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void BtnColor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPaleteStatus CLSession = new ColorPaleteStatus(proxy, memberuser, motifclasse, BtnColor);

                CLSession.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtRaison_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try
            {
                if (txtRaison.EditValue != null)
                {
                    if (txtRaison.Text.Trim() != "" && interfacecreation == 1)
                    {

                        var query = from c in proxy.GetAllStatu()
                                    select new { c.Status };

                        var results = query.ToList();
                        var disponible = results.Where(list1 => list1.Status.Trim().ToUpper() == txtRaison.Text.Trim().ToUpper()).FirstOrDefault();

                        if (disponible != null)
                        {


                            btnCreer.IsEnabled = false;
                            btnCreer.Opacity = 0.2;


                        }
                        else
                        {
                            if (txtRaison.Text.Trim() != "")
                            {


                                btnCreer.IsEnabled = true;
                                btnCreer.Opacity = 1;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

    }
}
