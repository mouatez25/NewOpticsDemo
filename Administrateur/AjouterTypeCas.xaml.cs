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
    /// Interaction logic for AjouterTypeCas.xaml
    /// </summary>
    public partial class AjouterTypeCas : DXWindow
    {
        SVC.TypeCa special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerTypeCas();
        SVC.Membership membership;
        string title;
        Brush titlebrush;
        public AjouterTypeCas(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.TypeCa motifrecu, SVC.Membership memberrecu)
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
                }
                else
                {
                    btnCreer.IsEnabled = false;
                }
                title = this.Title;
           //     titlebrush = this.WindowTitleBrush;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerTypeCas(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerTypeCas(HandleProxy));
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
                SVC.TypeCa pa = new SVC.TypeCa
                {
                    Libelle = txtLibelle.Text.Trim(),
                    Examen=txtExamen.Text.Trim(),
                };
                proxy.InsertTypeCasAsync(pa);
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                btnCreer.IsEnabled = false;
            }
            else
            {
                if (membership.CréationAdministrateur == true && special != null)
                {
                    proxy.UpdateTypeCasAsync(special);
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

        private void txtLibelle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (txtLibelle.Text.Trim() != "")
            {

                var query = from c in proxy.GetAllTypeCas()
                            select new { c.Libelle };

                var results = query.ToList();
                var disponible = results.Where(list1 => list1.Libelle.Trim().ToUpper() == txtLibelle.Text.Trim().ToUpper()).FirstOrDefault();

                if (disponible != null)
                {
                    this.Title = "Ce Type Cas Existe";
          //          this.WindowTitleBrush = Brushes.Red;

                    btnCreer.IsEnabled = false;
                    btnCreer.Opacity = 0.2;


                }
                else
                {
                    if (txtLibelle.Text.Trim() != "")
                    {
                        this.Title = title;
                      //  this.WindowTitleBrush = titlebrush;
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
