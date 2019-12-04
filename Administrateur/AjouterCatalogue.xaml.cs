using DevExpress.Xpf.Core;
using MahApps.Metro.Controls;
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
    /// Interaction logic for AjouterCatalogue.xaml
    /// </summary>
    public partial class AjouterCatalogue : DXWindow
    {
        SVC.Catalogue special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerCatalogue();
        SVC.Membership membership;
        string title;
      //  Brush titlebrush;
        public AjouterCatalogue(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.Catalogue spécialtiérecu, SVC.Membership membershirecu)
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
                    f.Content = "Modifier Catalogue";
                }
                else
                {
                    btnCreer.IsEnabled = false;
                    f.Content = "Créer un Catalogue";
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCatalogue(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCatalogue(HandleProxy));
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
                        SVC.Catalogue pa = new SVC.Catalogue
                        {
                            Catalogue1 = txSpecial.Text.Trim(),
                        };
                        proxy.InsertCatalogue(pa);
                        ts.Complete();
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        //     this.WindowTitleBrush = Brushes.Green;
                        btnCreer.IsEnabled = false;
                    }
                    proxy.AjouterActeCatalogueRefresh();
                }
                else
                {
                    if (membership.CréationAdministrateur == true && special != null)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateCatalogue(special);
                            ts.Complete();
                        }
                        proxy.AjouterActeCatalogueRefresh();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        //    this.WindowTitleBrush = Brushes.Red;
                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txSpecial_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (txSpecial.Text.Trim() != ""/* && special  == null*/)
            {

                var query = from c in proxy.GetAllCatalogue()
                            select new { c.Catalogue1};

                var results = query.ToList();
                var disponible = results.Where(list1 => list1.Catalogue1.Trim().ToUpper() == txSpecial.Text.Trim().ToUpper()).FirstOrDefault();

                if (disponible != null)
                {
                    this.Title = "Ce Catalogue Existe";
                 //   this.WindowTitleBrush = Brushes.Red;

                    btnCreer.IsEnabled = false;
                    btnCreer.Opacity = 0.2;


                }
                else
                {
                    if (txSpecial.Text.Trim() != "")
                    {
                        this.Title = title;
                   //     this.WindowTitleBrush = titlebrush;
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

        private void BTNCLOSTE_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
