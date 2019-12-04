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

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for AjouterDictionnaire.xaml
    /// </summary>
    public partial class AjouterDictionnaire : DXWindow
    {
        SVC.Dictionnaire special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerDictionnaire();
        SVC.Membership membership;
        string title;
        public AjouterDictionnaire(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.Dictionnaire spécialtiérecu, SVC.Membership membershirecu)
        {
            try
            {
                InitializeComponent();
                callback = callbackrecu;
                proxy = proxyrecu;
                special = spécialtiérecu;
                membership = membershirecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (special != null)
                {
                    ActeGrid.DataContext = special;
                    f.Content = "Modifier terminologie dentaire";
                }
                else
                {
              
                    f.Content = "Créer une terminologie dentaire";
                }
                title = this.Title;
                //      titlebrush = this.WindowTitleBrush;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDictionnaire(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDictionnaire(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (membership.CréationAdministrateur == true && special == null)
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        SVC.Dictionnaire pa = new SVC.Dictionnaire
                        {
                          Mot=txtMots.Text.Trim(),
                          Explication=txtExplication.Text.Trim(),
                        };
                        proxy.InsertDictionnaire(pa);
                        ts.Complete();

                        //     this.WindowTitleBrush = Brushes.Green;
                   
                    }
                    proxy.AjouterDictionnaireRefresh();
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    if (membership.CréationAdministrateur == true && special != null)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateDictionnaire(special);
                            ts.Complete();
                        }

                        proxy.AjouterDictionnaireRefresh();
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        //    this.WindowTitleBrush = Brushes.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
