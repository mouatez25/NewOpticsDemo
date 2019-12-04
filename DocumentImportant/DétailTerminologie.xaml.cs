using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
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

namespace GestionClinique.EtatEtRapport
{
    /// <summary>
    /// Interaction logic for DétailTerminologie.xaml
    /// </summary>
    public partial class DétailTerminologie : DXWindow
    {
        SVC.Dictionnaire special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerDétailTerminologie();
        SVC.Membership membership;
       
        public DétailTerminologie(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.Dictionnaire spécialtiérecu, SVC.Membership membershirecu)
        {
            try
            {
                InitializeComponent();
                callback = callbackrecu;
                proxy = proxyrecu;
                special = spécialtiérecu;
                membership = membershirecu;
                ActeGrid.DataContext = spécialtiérecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
              
              
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDétailTerminologie(HandleProxy));
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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDétailTerminologie(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
