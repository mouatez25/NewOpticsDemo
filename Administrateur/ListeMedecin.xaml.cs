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

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for ListeMedecin.xaml
    /// </summary>
    public partial class RendezVous : Page
    {
        SVC.ServiceCliniqueClient proxy;
        Membership sessionuser;
        private delegate void FaultedInvokerMedecin();
        ICallback callback;
        public RendezVous(SVC.ServiceCliniqueClient proxy1, ICallback callbackrecu, SVC.Membership mem)
        {
            try
            {
                InitializeComponent();
                proxy = proxy1;
                callbackrecu.InsertMedecinCallbackEvent += new ICallback.CallbackEventHandler6(callbackrecu_Refresh);
                sessionuser = mem;
                callback = callbackrecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMedecin(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMedecin(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertMedecin e)
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(List<Medecin> listmembership)
        {
            try
            { 
            MedecinsDataGrid.ItemsSource = listmembership;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
         
            try
            { 
            var query = from c in proxy.GetAllMedecin()
                        select c;
            MedecinsDataGrid.ItemsSource=query.ToList();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
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
                    if (sessionuser.CréationAdministrateur == true)
                    {
                        AjouterMedecin CLMedecin = new AjouterMedecin(null, proxy, sessionuser, callback);
                        CLMedecin.Show();
                    }
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
       
        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MedecinsDataGrid.SelectedItem != null && sessionuser.SuppressionAdministrateur == true)
                {

                    SVC.Medecin SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Medecin;
                    int nbrvisite = proxy.GetAllVisiteAll().Where(n => n.CodeMedecin == SelectMedecin.Id).Count();
                    if (nbrvisite==0)
                    {
                        proxy.DeleteMedecinAsync(SelectMedecin);
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }

                }
            }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MedecinsDataGrid.SelectedItem != null && sessionuser.ModificationAdministrateur==true)
            {
                SVC.Medecin SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Medecin;
               
                
                AjouterMedecin CLMedecin = new AjouterMedecin(SelectMedecin,proxy,sessionuser,callback);
                CLMedecin.Show();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            { 
            if (MedecinsDataGrid.SelectedItem != null && sessionuser.ModificationAdministrateur==true)
            {
                SVC.Medecin SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Medecin;
                
              
                AjouterMedecin CLMedecin = new AjouterMedecin(SelectMedecin,proxy,sessionuser,callback);
                CLMedecin.Show();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            TextBox t = (TextBox)sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(MedecinsDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Medecin p = o as Medecin;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.Nom.ToUpper().StartsWith(filter.ToUpper()));
                };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void txtRechercheSpeci_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            TextBox t = (TextBox)sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(MedecinsDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Medecin p = o as Medecin;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.SpécialitéMedecin.ToUpper().StartsWith(filter.ToUpper()));
                };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (sessionuser.ImpressionAdministrateur == true)
            {
                if (MedecinsDataGrid.SelectedItem != null && txtRechercheSpeci.Text == "" && txtRecherche.Text == "" )
                {
                   SVC.Medecin SelectMedecinDatagrid = MedecinsDataGrid.SelectedItem as SVC.Medecin;

                    List<ExclusionDay> tt = (from e1 in proxy.GetAllExclusionDay(SelectMedecinDatagrid.Nom, SelectMedecinDatagrid.Prénom)


                                             select e1).ToList();
                    if (SelectMedecinDatagrid.UserName!=null)
                    { 
                    var itemm = (from e1 in proxy.GetAllMembership()

                                where e1.UserName == SelectMedecinDatagrid.UserName

                                select e1).First();
             
                        if (tt != null)
                        {
                            this.NavigationService.Navigate(new ImpressionOneMedecin(proxy, SelectMedecinDatagrid, itemm, tt));
                        }else
                        { 
                        this.NavigationService.Navigate(new ImpressionOneMedecin(proxy, SelectMedecinDatagrid, itemm, null));
                        }
                    }
                    else
                    {
                        if (SelectMedecinDatagrid.UserName== null && tt!=null)
                        {
                            this.NavigationService.Navigate(new ImpressionOneMedecin(proxy, SelectMedecinDatagrid, null, tt));
                        }else
                        {
                            if (SelectMedecinDatagrid.UserName == null && tt == null)
                            {
                                this.NavigationService.Navigate(new ImpressionOneMedecin(proxy, SelectMedecinDatagrid, null, null));
                            }
                            }
                    }
                    
                
                }
                else
                {
                    if (txtRecherche.Text != "" && txtRechercheSpeci.Text == "")
                    {
                        List<Medecin> test = MedecinsDataGrid.ItemsSource as List<Medecin>;
                        var t = (from e1 in test

                                 where e1.Nom.ToUpper().StartsWith(txtRecherche.Text.ToUpper())

                                 select e1);

                        this.NavigationService.Navigate(new ImpresssionMedecin(proxy, t.ToList()));
                    }
                    else
                    {
                        if (txtRechercheSpeci.Text != "" && txtRecherche.Text == "")
                        {
                            List<Medecin> teste = MedecinsDataGrid.ItemsSource as List<Medecin>;
                            var t = (from e1 in teste

                                     where e1.SpécialitéMedecin.ToUpper().StartsWith(txtRechercheSpeci.Text.ToUpper())

                                     select e1);

                            this.NavigationService.Navigate(new ImpresssionMedecin(proxy, t.ToList()));
                        }else
                        {
                            if (txtRechercheSpeci.Text == "" && txtRecherche.Text == "" && MedecinsDataGrid.SelectedItem==null)
                            {
                                List<Medecin> test = MedecinsDataGrid.ItemsSource as List<Medecin>;


                                this.NavigationService.Navigate(new ImpresssionMedecin(proxy, test));
                            }
                        }

                    }
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

       
    }
}
