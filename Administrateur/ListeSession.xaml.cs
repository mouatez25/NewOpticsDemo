using GestionClinique.Administrateur;
using GestionClinique.SVC;
using MahApps.Metro.Controls;
using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    /// Interaction logic for ListeSession.xaml
    /// </summary>
    public partial class ListeSession :Page
    {
        SVC.ServiceCliniqueClient proxy;
        Membership SessionUser;
        private delegate void FaultedInvokerSession();
        public event EventHandler AddEventSession;
        bool SessionOpened = false;
        AjouterSession CLSession;
        public ListeSession(SVC.ServiceCliniqueClient proxy1, ICallback callbackrecu, SVC.Membership mem)
        {
            try
            { 
            InitializeComponent();
            proxy = proxy1;
            callbackrecu.InsertMmebershipCallbackEvent += new ICallback.CallbackEventHandler5(callbackrecu_Refresh);
            SessionUser = mem;
            proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
       
            proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        void callbackrecu_Refresh(object source, CallbackEventInsertMembership e)
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
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSession(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSession(HandleProxy));
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
                       
                       Grid test= (Grid)wndlistsession.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;
                        Button confirmer= (Button)wndlistsession.FindName("Confirmer");
                        confirmer.IsEnabled = false;
                        Grid tests = (Grid)wndlistsession.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;
                        if (SessionOpened == true)
                        {
                            AddEventSession(CLSession, new EventArgs());
                        }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSession(HandleProxy));
                return;
            }
            HandleProxy();
        }
           public void AddRefresh(List<Membership> listmembership)
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
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (SessionUser.CréationAdministrateur == true)
            {
                CLSession = new AjouterSession(null, proxy, this);
                SessionOpened = true;
                CLSession.Show();
            }
            else
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


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
            if (MedecinsDataGrid.SelectedItem != null && SessionUser.SuppressionAdministrateur==true)
            {
               
                SVC.Membership SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Membership;
                if (MedecinsDataGrid.SelectedItem != null && SelectMedecin.UserName!="Administrateur")
                {
                    var query = from c in proxy.GetAllMedecin()
                                select new { c.Nom, c.Prénom, c.UserName };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.UserName.Trim().ToUpper() == SelectMedecin.UserName.Trim().ToUpper()).FirstOrDefault();

                    if (disponible==null)
                    { 
                       
                        proxy.DeleteMembershipAsync(SelectMedecin);

                    }else
                    {

                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }else
                {



                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }else
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MedecinsDataGrid.SelectedItem != null && SessionUser.ModificationAdministrateur==true)
            {
                SVC.Membership SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Membership;
                 CLSession = new AjouterSession(SelectMedecin, proxy,this);
                SessionOpened = true;
                CLSession.Show();
                
            }
            else
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (MedecinsDataGrid.SelectedItem != null && SessionUser.ModificationAdministrateur==true)
            {
                SVC.Membership SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Membership;
                 CLSession = new AjouterSession(SelectMedecin, proxy,this);
                SessionOpened = true;
                CLSession.Show();
               
            }
            else
            {

                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

            var query = from c in proxy.GetAllMembership()
                        select c;


            MedecinsDataGrid.ItemsSource = query.ToList();
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
                    Membership p = o as Membership;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.UserName.ToUpper().StartsWith(filter.ToUpper()));
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
            if (SessionUser.ImpressionAdministrateur == true)
            {
                if (MedecinsDataGrid.SelectedItem != null)
                {
                    SVC.Membership SelectMedecin = MedecinsDataGrid.SelectedItem as SVC.Membership;

                   this.NavigationService.Navigate(new ImpressionOneSession(proxy, SelectMedecin));
                }
                else
                {
                    if (txtRecherche.Text != "")
                    {
                        List<Membership> test = MedecinsDataGrid.ItemsSource as List<Membership>;
                        var t = (from e1 in test

                                 where e1.UserName.ToUpper().StartsWith(txtRecherche.Text.ToUpper())

                                 select e1);

                        this.NavigationService.Navigate(new ImpressionSession(proxy, t.ToList()));
                    }
                    else
                    {
                        List<Membership> test = MedecinsDataGrid.ItemsSource as List<Membership>;


                        this.NavigationService.Navigate(new ImpressionSession(proxy, test));


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
