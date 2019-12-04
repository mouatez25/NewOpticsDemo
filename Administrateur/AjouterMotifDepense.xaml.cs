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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for AjouterMotifDepense.xaml
    /// </summary>
    public partial class AjouterMotifDepense : DXWindow
    {
        SVC.MotifDepense special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerMotifDepense();
        SVC.Membership membership;
        string title;
        Brush titlebrush;
        public AjouterMotifDepense(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MotifDepense motifrecu, SVC.Membership memberrecu)
        {
            try
            {
                InitializeComponent();
                callback = callbackrecu;
                proxy = proxyrecu;
                special = motifrecu;
                membership = memberrecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (special != null)
                {
                    MotifGrid.DataContext = special;
                    f.Content = "Modifier le motif de dépense";
                }
                else
                {
                    btnCreer.IsEnabled = false;
                    f.Content = "Créer un motif de dépense";
                }
                title = this.Title;
             //   titlebrush = this.WindowTitleBrush;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMotifDepense(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMotifDepense(HandleProxy));
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
                SVC.MotifDepense pa = new SVC.MotifDepense
                {
                   MotifD = txMotif.Text.Trim(),
                };
                proxy.InsertMotifDepenseAsync(pa);
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                btnCreer.IsEnabled = false;
            }
            else
            {
                if (membership.CréationAdministrateur == true && special != null)
                {
                    proxy.UpdateMotifDepenseAsync(special);
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txMotif_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (txMotif.Text.Trim() != ""/* && special  == null*/)
            {

                var query = from c in proxy.GetAllMotifDepense()
                            select new { c.MotifD };

                var results = query.ToList();
                var disponible = results.Where(list1 => list1.MotifD.Trim().ToUpper() == txMotif.Text.Trim().ToUpper()).FirstOrDefault();

                if (disponible != null)
                {
                    this.Title = "Ce Motif de Dépense Existe";
               //     this.WindowTitleBrush = Brushes.Red;

                    btnCreer.IsEnabled = false;
                    btnCreer.Opacity = 0.2;


                }
                else
                {
                    if (txMotif.Text.Trim() != "")
                    {
                        this.Title = title;
                  //      this.WindowTitleBrush = titlebrush;
                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;

                    }
                }
            }
            else
            {

                btnCreer.IsEnabled = false;
                btnCreer.Opacity = 0.2;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
