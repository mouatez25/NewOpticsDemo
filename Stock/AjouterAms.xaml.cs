
using NewOptics.Administrateur;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for AjouterAms.xaml
    /// </summary>
    public partial class AjouterAms : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MemberUser;
        ICallback callback;
     
        private delegate void FaultedInvokerAms();
        SVC.Prodf produit;
        SVC.am autresort;
        int medecinid = 0;
        public AjouterAms(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Prodf amsselec)
        {
            try
            {


                InitializeComponent();
            //    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-Us");

                proxy = proxyrecu;
                MemberUser = memberrecu;
                callback = callbackrecu;
               
              
                produit = amsselec;
                autresort = new SVC.am
                {
                    previent = produit.previent,
                   
                    codeprod = produit.cp,
                    design = produit.design,
                    cf = produit.cf,
                    nfact = produit.nfact,
                    datef = produit.datef,
                    dates = DateTime.Now,
                    déstockage = true,
                    stockage = false,
                };
                
                GridTransaction.DataContext = autresort;
                txtcodeprod.Text = produit.cp.ToString();
                txtdesign.Text = produit.design.ToString();
                txtnfact.Text = produit.nfact.ToString();
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
         
 catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAms(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAms(HandleProxy));
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
                bool AMResult = false;
                bool ProdfResult = false;

                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {


                        if (InformationComplet() && MemberUser.CreationAms==true)
                        {
                            try
                            {
                                using (var ts = new TransactionScope())
                            {

                                    if (autresort.quantite > 0)
                                    {
                                        if (chdéstockage.IsChecked == true)
                                        {
                                            if (produit.quantite >= autresort.quantite)
                                            {
                                                autresort.quantite = autresort.quantite * -1;
                                                autresort.observ = txtObserv.Text;
                                                autresort.oper = MemberUser.Username;
                                               
                                                proxy.Insertam(autresort);
  
                                                AMResult = true;
                                                produit.quantite = produit.quantite + autresort.quantite;
                                                proxy.UpdateProdf(produit);
                                                ProdfResult = true;
                                            }
                                            else
                                            {
                                                AMResult = false;

                                            }

                                        }
                                        else
                                        {
                                            if (chstockage.IsChecked == true)
                                            {
                                                autresort.quantite = autresort.quantite;
                                                autresort.observ = txtObserv.Text;
                                                autresort.oper = MemberUser.Username;
                                                proxy.Insertam(autresort);
                                                AMResult = true;
                                                produit.quantite = produit.quantite + autresort.quantite;
                                                proxy.UpdateProdf(produit);
                                                ProdfResult = true;
                                            }
                                        }
                                        
                                  

                                        if (AMResult && ProdfResult)
                                        {
                                            ts.Complete();
                                        }
                                        else
                                        {
                                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                    }
                                }
                                if (AMResult && ProdfResult)
                                {
                                  
                                    proxy.AjouterProdfRefresh();
                                    proxy.AjouterAmsRefresh();
                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    this.Close();
                                }

                            }
                            catch (TransactionAbortedException ex)
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                              

                            }
                            catch (Exception ex)
                            {

                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        bool InformationComplet()
        {
            if (produit!= null && txtDate.SelectedDate != null && txtnfact.Text != ""&& !string.IsNullOrEmpty(txtQuantité.Text) && !string.IsNullOrEmpty(txtObserv.Text) && (chstockage.IsChecked == true || chdéstockage.IsChecked == true))
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        private void txtQuantité_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }


       

        private void txtObserv_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                case Key.B:
                case Key.C:
                case Key.D:
                case Key.E:
                case Key.F:
                case Key.G:
                case Key.H:
                case Key.I:
                case Key.J:
                case Key.K:
                case Key.L:
                case Key.M:
                case Key.N:
                case Key.O:
                case Key.P:
                case Key.Q:
                case Key.R:
                case Key.S:
                case Key.T:
                case Key.U:
                case Key.V:
                case Key.W:
                case Key.X:
                case Key.Y:
                case Key.Z:
                case Key.Space:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
    }
}
