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
    /// Interaction logic for AjouterSpécialité.xaml
    /// </summary>
    public partial class AjouterSpécialité : DXWindow
    {
        SVC.Spécialité special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerSpécialité();
        SVC.Membership membership;
        string title;
        Brush titlebrush;
        public AjouterSpécialité(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrecu, SVC.Spécialité spécialtiérecu,SVC.Membership membershirecu)
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
                    SpécialitéGrid.DataContext = special;
                }
                else
                {
                    btnCreer.IsEnabled = false;
                }
                title = this.Title;
               // titlebrush = this.WindowTitleBrush;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSpécialité(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSpécialité(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (membership.CréationAdministrateur== true && special==null)
            {
                SVC.Spécialité pa = new SVC.Spécialité
                {
                    Spécialité1 = txSpecial.Text.Trim(),
                };
                proxy.InsertSpécialitéAsync(pa);
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                btnCreer.IsEnabled = false;
            }
            else
            {
                if (membership.CréationAdministrateur == true && special != null)
                {
                    proxy.UpdateSpécialité(special);
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

        private void txSpecial_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (txSpecial.Text.Trim() != ""/* && special  == null*/)
            {

                var query = from c in proxy.GetAllSpécialité()
                            select new { c.Spécialité1};

                var results = query.ToList();
                var disponible = results.Where(list1 => list1.Spécialité1.Trim().ToUpper() == txSpecial.Text.Trim().ToUpper()).FirstOrDefault();

                if (disponible != null)
                {
                    this.Title = "Cette Spécialité Existe";
                  //  this.WindowTitleBrush = Brushes.Red;

                    btnCreer.IsEnabled = false;
                    btnCreer.Opacity = 0.2;

                   
                }
                else
                {
                    if (txSpecial.Text.Trim() != "")
                    {
                        this.Title = title;
                    //    this.WindowTitleBrush = titlebrush;
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
