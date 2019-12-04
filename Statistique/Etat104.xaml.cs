
using NewOptics.Administrateur;
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
    /// Interaction logic for Etat104.xaml
    /// </summary>
    public partial class Etat104 : Window
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic MemberUser;
        private delegate void FaultedInvokerEtat104();

        public Etat104(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = memberrecu;
                DateVisiteDébut.SelectedDate = DateTime.Now;
                DateVisiteFin.SelectedDate = DateTime.Now;


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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerEtat104(HandleProxy));
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
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerEtat104(HandleProxy));
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
                if (DateVisiteDébut.SelectedDate != null && DateVisiteFin.SelectedDate != null)
                {
                    List<Etat104Class> Etat104Classliste = new List<Etat104Class>();
                    List<SVC.F1> listf1 = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(ta => ta.codeclient != 0 && (ta.nfact.Substring(0, 1) == "F" || ta.nfact.Substring(0, 1) == "A")).ToList();
                    var listevisitecode = (from ta in listf1

                                           where (ta.nfact.Substring(0, 1) == "F" || ta.nfact.Substring(0, 1) != "A")
                                           select ta.codeclient).Distinct();
                    foreach (int itemcode in listevisitecode)
                    {
                        Etat104Class nn = new Etat104Class();
                        nn.totalht = 0;
                        nn.totaltva = 0;
                        foreach (SVC.F1 itemvisite in listf1)
                        {
                            if (itemcode == itemvisite.codeclient && itemcode != 0)
                            {
                                nn.totalht = Convert.ToDecimal(nn.totalht + itemvisite.ht);
                                nn.totaltva = Convert.ToDecimal(nn.totaltva + itemvisite.tva);
                                nn.Id = itemcode;
                            }
                        }
                        Etat104Classliste.Add(nn);
                    }
                    foreach (var item in Etat104Classliste)
                    {
                        SVC.ClientV clientv = proxy.GetAllClientVBYID(item.Id).First();
                        item.adresse = clientv.adresse;
                        item.ai = clientv.ai;
                        item.mf = clientv.mf;
                        item.Raison = clientv.Raison;
                        item.rc = clientv.rc;

                    }

                    PatientDataGrid.ItemsSource = Etat104Classliste;
                    var test = PatientDataGrid.ItemsSource as IEnumerable<Etat104Class>;
                    TxtTva.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.totaltva));
                    txtHT.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.totalht));
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
                List<Etat104Class> itemsSource0 = PatientDataGrid.Items.OfType<Etat104Class>().ToList();
              
                Impression104 cl = new Impression104(proxy, itemsSource0, DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date);
                cl.Show();
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
                        Etat104Class p = o as Etat104Class;
                        if (t.Name == "txtId")
                            return (p.Raison == filter);
                        return (p.Raison.ToUpper().Contains(filter.ToUpper()));
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
