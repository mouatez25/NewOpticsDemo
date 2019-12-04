

using NewOptics.Administrateur;
using NewOptics.Vente;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Statistique
{
    /// <summary>
    /// Interaction logic for JournalDesVentes.xaml
    /// </summary>
    public partial class JournalDesVentes : Window
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic MemberUser;
        private delegate void FaultedInvokerJournalDesVentes();
        int interfaceimpression = 0;
        public JournalDesVentes(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = memberrecu;
                DateVisiteDébut.SelectedDate= DateTime.Now;
                DateVisiteFin.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")); ;
                interfaceimpression = 0;

                PatientDataGrid.SelectedItem = null;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerJournalDesVentes(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            try
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerJournalDesVentes(HandleProxy));
                    return;
                }
                HandleProxy();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
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
                        /*   proxy.Abort();
                           proxy = null;
                           this.Close();*/
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

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {

               
                int radiobutton = 0;
                if (DateVisiteDébut.SelectedDate != null && DateVisiteFin.SelectedDate != null)
                {
                    if (chFacture.IsChecked == true)
                    {
                      
                        radiobutton = 1;
                    }
                    else
                    {
                        if (chBonDeLivraison.IsChecked == true)
                        {
                           
                            radiobutton = 2;
                        }
                        else
                        {
                            if (chFactureProvisoire.IsChecked == true)
                            {
                                radiobutton = 3;
                            }
                            else
                            {
                                if (chComptoir.IsChecked == true)
                                {
                                    radiobutton = 4;
                                }
                                else
                                {
                                    if(chTout.IsChecked==true)
                                    {
                                        radiobutton = 5;
                                    }
                                }
                            }
                        }
                         
                    }
                   

                    if (radiobutton == 1)
                    {
                        PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) == "F" || n.nfact.Substring(0, 1) == "A"));
                   
                    }
                    else
                    {   if (radiobutton == 2)
                        {
                            PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) == "B" || (n.nfact.Substring(0, 1) == "C" && n.nfact.Substring(0,2)!="Co") ));
                           
                        }
                        else
                        {
                            if(radiobutton == 3)
                            {
                                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) == "R"));
                               
                            }
                            else
                            {
                                if (radiobutton == 4)
                                {
                                    PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0,2)=="Co"));
                                   
                                }
                                else
                                {
                                                
                                        if (radiobutton == 5)
                                        {
                                        PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" /*&& n.nfact.Substring(0, 1) != "R" && n.nfact.Substring(0, 2) != "Co"*/));
                                      
                                        }
                                 
                                    }
                            }
                        }
                    }
                    interfaceimpression = 0;
                    PatientDataGrid.SelectedItem = null;
                    var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                    txtHT.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.ht));
                    TxtTva.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.tva));
                    txtttc.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.ht + o.tva));
                    txtRemise.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.remise));
                    txtTimbres.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.timbre));
                    txtNet.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.net));
                }



            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SVC.F1> itemsSource1 = PatientDataGrid.Items.OfType<SVC.F1>().ToList();
                List<SVC.F1> itemsSource0 = new List<SVC.F1>();
                foreach (var p in itemsSource1)
                {
                   

                    if ((p.nfact.Substring(0, 1) != "P" && p.nfact.Substring(0, 1) != "R"))
                    {
                        itemsSource0.Add(p);
                    }
                }
                ImpressionVente cl = new ImpressionVente(proxy, itemsSource0, DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date, 1);
                cl.Show();
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

      

       

        private void btndeFilter_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DateVisiteDébut.SelectedDate = DateTime.Now;
                DateVisiteFin.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")); ;
                chTout.IsChecked = true;
               
                interfaceimpression = 0;
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtRecherchenOM_TextChanged(object sender, TextChangedEventArgs e)
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
                        SVC.F1 p = o as SVC.F1;
                        if (t.Name == "txtId")
                            return (p.raison == filter);
                        return (p.raison.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
