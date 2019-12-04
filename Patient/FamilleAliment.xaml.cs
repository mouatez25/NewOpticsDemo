using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
using MahApps.Metro.Controls;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for FamilleAliment.xaml
    /// </summary>
    public partial class FamilleAliment : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerListeFamilleAliment();
        public FamilleAliment(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;

             FamilleAlimentDataGrid.ItemsSource= proxy.GetAllFamilleAliment();
                callbackrecu.InsertFamilleAlimentCallbackevent += new ICallback.CallbackEventHandler29(callbackDci_Refresh);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

            proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            //    this.WindowTitleBrush = Brushes.Red;
            }
        }
        void callbackDci_Refresh(object source, CallbackEventInsertFamilleAliment e)
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
                this.Title = ex.Message;
             //   this.WindowTitleBrush = Brushes.Red;
            }
        }
        public void AddRefresh(List<SVC.FamilleAliment> listmembership)
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

                        FamilleAlimentDataGrid.ItemsSource = listmembership;


                    }
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
             //   this.WindowTitleBrush = Brushes.Red;
            }

        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeFamilleAliment(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeFamilleAliment(HandleProxy));
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

        private void FamilleAlimentDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (memberuser.CréationAdministrateur == true && FamilleAlimentDataGrid.SelectedItem != null)
            {
                SVC.FamilleAliment selectedfamille = FamilleAlimentDataGrid.SelectedItem as SVC.FamilleAliment;
                InsertFamille cl = new InsertFamille(proxy, callback, selectedfamille, memberuser);
                cl.Show();
            }
            else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if(memberuser.CréationAdministrateur==true)
             {
                InsertFamille cl = new InsertFamille(proxy, callback, null, memberuser);
                cl.Show();
            }
            else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.SuppressionAdministrateur == true && FamilleAlimentDataGrid.SelectedItem != null)
            {
                SVC.FamilleAliment selectedfamille = FamilleAlimentDataGrid.SelectedItem as SVC.FamilleAliment;
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    proxy.DeletFamilleAliment(selectedfamille);
                    ts.Complete();
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                proxy.AjouterAlimentFamilleRefresh();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.CréationAdministrateur == true && FamilleAlimentDataGrid.SelectedItem!=null)
            {
                SVC.FamilleAliment selectedfamille = FamilleAlimentDataGrid.SelectedItem as SVC.FamilleAliment;
                InsertFamille cl = new InsertFamille(proxy, callback, selectedfamille, memberuser);
                cl.Show();
            }
            else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
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
                    SVC.FamilleAliment p = o as SVC.FamilleAliment;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.FamilleAlimentDesign.ToUpper().StartsWith(filter.ToUpper()));
                };
            }
        }
    }
}
