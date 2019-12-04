using GestionClinique.Administrateur;
using GestionClinique.SVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.RendezVous
{
    /// <summary>
    /// Interaction logic for ListeRendezVous.xaml
    /// </summary>
    public partial class ListeRendezVous : UserControl
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerListeRendezVous();
        public ListeRendezVous(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                RendezVousDataGrid.ItemsSource = proxy.GetAllRendezVous(DateTime.Now.Date, DateTime.Now.Date);
                NBSALLEATTENTE.Text = ((proxy.GetAllRendezVous(DateTime.Now.Date, DateTime.Now.Date)).AsEnumerable().Count()).ToString();

                DateSaisieDébut.SelectedDate = DateTime.Now;
                DateSaisieFin.SelectedDate = DateTime.Now;
                callbackrecu.InsertRendezVousCallbackEvent += new ICallback.CallbackEventHandler8(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertRendezVous e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
               AddRefresh(e.clientleav,e.operleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.RendezVou listmembership,int oper)
        {
            try
            {
                var LISTITEM1 = RendezVousDataGrid.ItemsSource as IEnumerable<SVC.RendezVou>;
                List<SVC.RendezVou> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                        var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        objectmodifed = listmembership;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }

                    RendezVousDataGrid.ItemsSource = LISTITEM;
                    NBSALLEATTENTE.Text = ((LISTITEM).Where(n => n.Date == DateTime.Now.Date).AsEnumerable().Count()).ToString();


                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


            }
        }
        private void RendezVousDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try { 
            if (RendezVousDataGrid.SelectedItem != null && memberuser.ModificationRendezVous == true)
            {
                SVC.RendezVou SelectRendezVous = RendezVousDataGrid.SelectedItem as SVC.RendezVou;


                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, memberuser, callback, 2, null);
                CLMedecin.Show();

                    /*probléme selected item*/

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeRendezVous(HandleProxy));
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
                        var wndlistsession = Window.GetWindow(this);

                        Grid test = (Grid)wndlistsession.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;
                        Button confirmer = (Button)wndlistsession.FindName("Confirmer");
                        confirmer.IsEnabled = false;
                        Grid tests = (Grid)wndlistsession.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeRendezVous(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

            try { 
            if (memberuser.CréationRendezVous == true)
            {
                SVC.RendezVou SelectRendezVous = new RendezVou
                {
                    PrisPar = memberuser.UserName,

                };


                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, memberuser, callback, 3, null);
                CLMedecin.Show();



                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (memberuser.ImpressionRendezVous == true && RendezVousDataGrid.SelectedItem != null)
            {
                SVC.RendezVou SelectMedecin = RendezVousDataGrid.SelectedItem as SVC.RendezVou;
                ImpressionTicket clsho = new ImpressionTicket(proxy, SelectMedecin);
                clsho.Show();
            }
            else
            {
                if (memberuser.ImpressionRendezVous == true && RendezVousDataGrid.SelectedItem == null)
                {
                    List<SVC.RendezVou> test1= proxy.GetAllRendezVous(DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value).ToList();
                    ImpressionListeRendezVous clsho = new ImpressionListeRendezVous(proxy, test1);
                    clsho.Show();
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (RendezVousDataGrid.SelectedItem != null && memberuser.ModificationRendezVous == true)
            {
                SVC.RendezVou SelectRendezVous = RendezVousDataGrid.SelectedItem as SVC.RendezVou;


                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, memberuser, callback, 2, null);
                CLMedecin.Show();

                    /*probléme selected item*/

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RendezVousDataGrid.SelectedItem != null && memberuser.SuppressionPatient == true)
                {
                    SVC.RendezVou SelectRendezVous = RendezVousDataGrid.SelectedItem as SVC.RendezVou;
                    proxy.DeleteRendezVous(SelectRendezVous);
                    proxy.AjouterRendezVOUSfRefresh();
                    proxy.AjouterSalleAtentefRefresh();
                }
                else
                {
                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            TextBox t = (TextBox)sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(RendezVousDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    SVC.RendezVou p = o as SVC.RendezVou;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.Nom.ToUpper().StartsWith(filter.ToUpper()));
                };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {
                    RendezVousDataGrid.ItemsSource = proxy.GetAllRendezVous(DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value);
                    NBSALLEATTENTE.Text = ((proxy.GetAllRendezVous(DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value)).AsEnumerable().Count()).ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void RendezVousDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                RendezVousDataGrid.ItemsSource = proxy.GetAllRendezVous(DateTime.Now, DateTime.Now);
                DateSaisieDébut.SelectedDate = DateTime.Now;
                DateSaisieFin.SelectedDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
