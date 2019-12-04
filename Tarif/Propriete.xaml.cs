
using NewOptics.Administrateur;
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

namespace NewOptics.Tarif
{
    /// <summary>
    /// Interaction logic for Propriete.xaml
    /// </summary>
    public partial class Propriete : Window
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic MEMBER;
        private delegate void FaultedInvokerPropriete();
        public Propriete(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrecu,SVC.MembershipOptic memberrecu,SVC.VerreAssocie verrerecu,string textrecu)
        {
            try
            {
                InitializeComponent();
                MEMBER = memberrecu;
                proxy = proxyrecu;
                callback = callbackrecu;
                ExSupp.Text = textrecu;
                GridProp.DataContext = verrerecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch(Exception ex)
            {
                HandleProxy();
            }
           
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerPropriete(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerPropriete(HandleProxy));
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
