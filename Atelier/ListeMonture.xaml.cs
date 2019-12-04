
using Microsoft.Reporting.WinForms;
using NewOptics.Administrateur;
using NewOptics.Caisse;
using NewOptics.ClientA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
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

namespace NewOptics.Atelier
{
    /// <summary>
    /// Interaction logic for ListeMonture.xaml
    /// </summary>
    public partial class ListeMonture : Page
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerListeMonture();
       
      
        public ListeMonture(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                 
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllMontureDate(DateSaisieFin.SelectedDate.Value, DateSaisieDébut.SelectedDate.Value);

                callbackrecu.InsertMontureCallbackevent += new ICallback.CallbackEventHandler34(callbackrecumonture_Refresh);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecumonture_Refresh(object source, CallbackEventInsertMonture e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshMonture(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshMonture(SVC.Monture listmembership, int oper)
        {
            try
            {

                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Monture>;
                List<SVC.Monture> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                        //   var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //  objectmodifed = listmembership;

                        var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //objectmodifed = listmembership;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listmembership;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                PatientDataGrid.ItemsSource = LISTITEM;


            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeMonture(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeMonture(HandleProxy));
                return;
            }
            HandleProxy();
        }
         

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModuleDossierClient == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.Monture selecedmonture = PatientDataGrid.SelectedItem as SVC.Monture;
                    List<SVC.ClientV> client = proxy.GetAllClientVBYID(Convert.ToInt16(selecedmonture.IdClient));
                    //List<SVC.Monture> selectedmonture1 = proxy.GetAllMonturebycodebar(selecedmonture.NCommande);
                    MontureClient cl = new MontureClient(proxy, callback, memberuser, client.First(), selecedmonture);
                    cl.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

       

      

        private void checkEnMontage_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
              //  bool filterValue = Convert.ToBoolean(checkEnMontage.IsChecked);

                CheckBox t = (CheckBox)sender;
                bool filterValue =Convert.ToBoolean(t.IsChecked);
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == false)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.EnMontage == filterValue);
                        return (p.EnMontage.Value.Equals(filterValue));
                    };
                }


                PatientDataGrid.SelectedItem = null;

                
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkEnMontage_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //  bool filterValue = Convert.ToBoolean(checkEnMontage.IsChecked);

                CheckBox t = (CheckBox)sender;
                bool filterValue = Convert.ToBoolean(t.IsChecked);
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == false)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.EnMontage == filterValue);
                        return (p.EnMontage.Value.Equals(filterValue));
                    };
                }


                PatientDataGrid.SelectedItem = null;


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkRéceptionée_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //  bool filterValue = Convert.ToBoolean(checkEnMontage.IsChecked);

                CheckBox t = (CheckBox)sender;
                bool filterValue = Convert.ToBoolean(t.IsChecked);
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == false)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.Délivre == filterValue);
                        return (p.Délivre.Value.Equals(filterValue));
                    };
                }


                PatientDataGrid.SelectedItem = null;


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkRéceptionée_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                //  bool filterValue = Convert.ToBoolean(checkEnMontage.IsChecked);

                CheckBox t = (CheckBox)sender;
                bool filterValue = Convert.ToBoolean(t.IsChecked);
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == false)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.Délivre == filterValue);
                        return (p.Délivre.Value.Equals(filterValue));
                    };
                }


                PatientDataGrid.SelectedItem = null;


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkeffe_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                //  bool filterValue = Convert.ToBoolean(checkEnMontage.IsChecked);

                CheckBox t = (CheckBox)sender;
                bool filterValue = Convert.ToBoolean(t.IsChecked);
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == false)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.Monte == filterValue);
                        return (p.Monte.Value.Equals(filterValue));
                    };
                }


                PatientDataGrid.SelectedItem = null;


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkeffe_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //  bool filterValue = Convert.ToBoolean(checkEnMontage.IsChecked);

                CheckBox t = (CheckBox)sender;
                bool filterValue = Convert.ToBoolean(t.IsChecked);
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == false)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.Monte == filterValue);
                        return (p.Monte.Value.Equals(filterValue));
                    };
                }


                PatientDataGrid.SelectedItem = null;


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (ParDateCommande.IsChecked == true)
            {
                PatientDataGrid.ItemsSource = proxy.GetAllMontureDate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date);
            }
            else
            {
                if (ParDateLivraison.IsChecked == true)
                {
                    PatientDataGrid.ItemsSource = proxy.GetAllMontureDateLivraison(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date);
                }
            }
        }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
               
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllMontureDate(DateTime.Now.Date, DateTime.Now.Date);
                 ParDateCommande.IsChecked = true; 
             }
            catch (Exception ex)

            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PatientDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (memberuser.ModuleDossierClient == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.Monture selecedmonture = PatientDataGrid.SelectedItem as SVC.Monture;
                    List<SVC.ClientV> client = proxy.GetAllClientVBYID(Convert.ToInt16(selecedmonture.IdClient));
                    //List<SVC.Monture> selectedmonture1 = proxy.GetAllMonturebycodebar(selecedmonture.NCommande);
                    MontureClient cl = new MontureClient(proxy, callback, memberuser, client.First(), selecedmonture);
                    cl.Show();
                }

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
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Monture p = o as SVC.Monture;
                        if (t.Name == "txtId")
                            return (p.RaisonClient == filter);
                        return (p.RaisonClient.ToUpper().Contains(filter.ToUpper()));
                    };
                }


                PatientDataGrid.SelectedItem = null;

              
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewMonture vl = new NewMonture(proxy,callback,memberuser,1);
                vl.Show();
                    
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
