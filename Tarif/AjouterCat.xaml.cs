
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

namespace NewOptics.Tarif
{
    /// <summary>
    /// Interaction logic for AjouterCat.xaml
    /// </summary>
    public partial class AjouterCat : Window
    {
        SVC.CatSupp special;
        SVC.ServiceCliniqueClient proxy;

        private delegate void FaultedInvokerAjouterCat();
        SVC.MembershipOptic MembershipOptic;
        string title;
        //  Brush titlebrush;
        public AjouterCat(SVC.ServiceCliniqueClient proxyrecu, SVC.CatSupp spécialtiérecu, SVC.MembershipOptic membershirecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                special = spécialtiérecu;
                MembershipOptic = membershirecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (special != null)
                {
                    FournVousGrid.DataContext = special;
                }
                else
                {
                    btnCreer.IsEnabled = false;
                }
                title = this.Title;
                //  titlebrush = this.WindowTitleBrush;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAjouterCat(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAjouterCat(HandleProxy));
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
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.CreationFichier == true && special == null && txtRaison.Text != "")
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        SVC.CatSupp pa = new SVC.CatSupp
                        {
                            Cat = txtRaison.Text.Trim(),

                        };
                        proxy.InsertCatSupp(pa);
                        ts.Complete();

                    }
                    proxy.AjouterCatSuppRefresh();

                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    btnCreer.IsEnabled = false;
                }
                else
                {
                    if (MembershipOptic.ModificationAchat == true && special != null)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateCatSupp(special);
                            ts.Complete();
                        }
                        proxy.AjouterCatSuppRefresh();
                    }
                    else
                    {

                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        

        private void txtRaison_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtRaison.Text.Trim() != "" && special == null)
                {

                    var query = from c in proxy.GetAllCatSupp()
                                select new { c.Cat };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.Cat.Trim().ToUpper() == txtRaison.Text.Trim().ToUpper()).FirstOrDefault();

                    if (disponible != null)
                    {
                        this.Title = "Cette Catégorie Existe";
                        // this.WindowTitleBrush = Brushes.Red;

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;


                    }
                    else
                    {
                        if (txtRaison.Text.Trim() != "")
                        {
                            this.Title = title;
                            // this.WindowTitleBrush = titlebrush;
                            btnCreer.IsEnabled = true;
                            btnCreer.Opacity = 1;

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



