
using NewOptics.Administrateur;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ListeCategorieSupplement.xaml
    /// </summary>
    public partial class ListeCategorieSupplement : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerListeCategorieSupplement();
        public ListeCategorieSupplement(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                FamilleAlimentDataGrid.ItemsSource = proxy.GetAllCatSupp();
                callbackrecu.InsertCatSuppCallbackevent += new ICallback.CallbackEventHandler56(callbackDci_Refresh);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackDci_Refresh(object source, CallbackEventInsertCatSupp e)
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
        public void AddRefresh(List<SVC.CatSupp> listMembershipOptic)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {

                        FamilleAlimentDataGrid.ItemsSource = listMembershipOptic;


                    }
                }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeCategorieSupplement(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeCategorieSupplement(HandleProxy));
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
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterCat CLSession = new AjouterCat(proxy, null, memberuser);
                    CLSession.Show();

                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.SuppressionFichier == true && FamilleAlimentDataGrid.SelectedItem != null)
                {
                    SVC.CatSupp selectssalleattente = FamilleAlimentDataGrid.SelectedItem as SVC.CatSupp;

                    if (selectssalleattente.Id != 19 && selectssalleattente.Id != 10 && selectssalleattente.Id != 13 && selectssalleattente.Id != 1)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.DeleteCatSupp(selectssalleattente);
                            ts.Complete();
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        proxy.AjouterCatSuppRefresh();
                    }
                    else
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous ne pouvez pas supprimer cette catégorie", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModificationFichier == true)
                {
                    SVC.CatSupp SelectMedecin = FamilleAlimentDataGrid.SelectedItem as SVC.CatSupp;
                   
                        AjouterCat CLSession = new AjouterCat(proxy, SelectMedecin, memberuser);
                        CLSession.Show();
                   
                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {

        }



        private void FamilleAlimentDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (memberuser.ModificationFichier == true)
                {
                    SVC.CatSupp SelectMedecin = FamilleAlimentDataGrid.SelectedItem as SVC.CatSupp;

                    AjouterCat CLSession = new AjouterCat(proxy, SelectMedecin, memberuser);
                        CLSession.Show();
                 

                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


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
        
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(FamilleAlimentDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.CatSupp p = o as SVC.CatSupp;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Cat.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
