using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for InsertFamille.xaml
    /// </summary>
    public partial class InsertFamille : DXWindow
    {
        SVC.FamilleAliment special;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerFamille();
        SVC.Membership membership;
        string title;
        Brush titlebrush;
        public InsertFamille(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.FamilleAliment spécialtiérecu, SVC.Membership membershirecu)
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
                    f.Content = "Modifier Famille";
                }
                else
                {
                    btnCreer.IsEnabled = false;
                    f.Content = "Créer une Famille";
                }
                title = this.Title;
                //      titlebrush = this.WindowTitleBrush;
            }
            catch
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerFamille(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerFamille(HandleProxy));
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
                        SVC.FamilleAliment pa = new SVC.FamilleAliment
                        {
                            FamilleAlimentDesign = txSpecial.Text.Trim(),
                        };
                        proxy.InsertFamilleAliment(pa);
                        ts.Complete();
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        btnCreer.IsEnabled = false;
                    }
                    proxy.AjouterAlimentFamilleRefresh();
                }
                else
                {
                    if (membership.CréationAdministrateur == true && special != null)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateFamilleAliment(special);
                            ts.Complete();
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        proxy.AjouterAlimentFamilleRefresh();
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        //    this.WindowTitleBrush = Brushes.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void txSpecial_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txSpecial.Text.Trim() != ""/* && special  == null*/)
            {

                var query = from c in proxy.GetAllFamilleAliment()
                            select new { c.FamilleAlimentDesign };

                var results = query.ToList();
                var disponible = results.Where(list1 => list1.FamilleAlimentDesign.Trim().ToUpper() == txSpecial.Text.Trim().ToUpper()).FirstOrDefault();

                if (disponible != null)
                {
                    this.Title = "Cette famille Existe";
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
    }
}
