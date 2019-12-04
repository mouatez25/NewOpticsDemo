using NewOptics.Administrateur;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Param
{
    /// <summary>
    /// Interaction logic for Compteurs.xaml
    /// </summary>
    public partial class Compteurs : Page
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;
        SVC.Param selectedparametre;
        SVC.Client localclient;
        private delegate void FaultedInvokerListeParamCompteurs();
        public Compteurs(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Client clientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;

                memberuser = memberrecu;

                callback = callbackrecu;
                localclient = clientrecu;
                selectedparametre = proxy.GetAllParamétre();
                InfGénéral.DataContext = selectedparametre;

                callbackrecu.InsertParamCallbackEvent += new ICallback.CallbackEventHandler16(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertParam e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.Param listmembership)
        {
            try
            {

                InfGénéral.DataContext = listmembership;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeParamCompteurs(HandleProxy));
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
                        proxy = null;

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;


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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeParamCompteurs(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            
                BACK.Visibility = Visibility.Visible;
                bool suuces = false;
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    proxy.UpdateParamétre(selectedparametre);
                    ts.Complete();
                    suuces = true;
                }
                if (suuces==true)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
              
                BACK.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
             
                BACK.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
           
            BACK.Visibility = Visibility.Visible;
          //      List<SVC.Prodf> listproduit = new List<SVC.Prodf>();
                var existe = proxy.GetAllProdf().Any(n => n.quantite == 0);
                if (existe == true)
                { MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Si toutes les fiches produits avec une quantité égal à zéro seront supprimées ", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        List<SVC.Prodf> LISTexiste = proxy.GetAllProdf().Where(n => n.quantite == 0).ToList();
                        for (int i = LISTexiste.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/

                        {
                            var item = LISTexiste.ElementAt(i);

                            using (var ts = new TransactionScope())
                            {
                                proxy.DeleteProdf(item);
                                ts.Complete();
                            }
                            proxy.AjouterProdfRefresh();
                        }
                      
                        BACK.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (result == MessageBoxResult.No)
                        {
                           
                            BACK.Visibility = Visibility.Collapsed;
                        }
                    }

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Stock avec quantité zero n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                  
                    BACK.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
              
                BACK.Visibility = Visibility.Collapsed;
            }
}
    }
}
