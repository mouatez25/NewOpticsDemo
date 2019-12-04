
using NewOptics.Administrateur;
using System;
using System.Collections;
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
    /// Interaction logic for PeremptionAlerte.xaml
    /// </summary>
    public partial class PeremptionAlerte : Window
    {
        ICallback callback;
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MembershipOptic;
        private delegate void FaultedInvokerPeremtion();
        int interfaceimpression = 0;
        public PeremptionAlerte(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MembershipOptic = memberrecu;
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
               
              
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
     
       
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerPeremtion((HandleProxy)));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerPeremtion((HandleProxy)));
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

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces==true)
                {
                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.perempt >= DateSaisieDébut.SelectedDate && n.perempt <= DateSaisieFin.SelectedDate && n.perempt != null && n.quantite != 0).ToList();
                    ReceptDatagrid.ItemsSource = listprof;
                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Prodf>;
                    txtstock.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.previent * o.quantite));

                    txtFiche.Text = Convert.ToString(((test).AsEnumerable().Count()));
                    interfaceimpression = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ImpressionAchat == true && ReceptDatagrid.Items.Count > 0 && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {

                    var itemsSource0 = ReceptDatagrid.Items.OfType<SVC.Prodf>();// List<SVC.SalleDattente>;
                    List<SVC.Prodf> itemsSource1 = new List<SVC.Prodf>();
                    foreach (SVC.Prodf item in itemsSource0)
                    {
                        itemsSource1.Add(item);
                    }

                    ImpressionStockalertePerme cl = new ImpressionStockalertePerme(proxy, itemsSource1, DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value, txtFiche.Text, txtstock.Text);

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
                ICollectionView cv = CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Prodf p = o as SVC.Prodf;
                        if (t.Name == "txtId")
                            return (p.design == filter);
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
                    };
                }
                var test = ReceptDatagrid.Items.OfType<SVC.Prodf>();
                txtstock.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.previent * o.quantite));

                txtFiche.Text = Convert.ToString(((test).AsEnumerable().Count()));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnFilterf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ImpressionAchat == true )
                {
                    var listevisitecode = (from ta in proxy.GetAllProdf()
                                           where ta.cp != 0
                                           select ta.cp).Distinct();
                  
                    // List<SVC.Prodf> resultprodf = proxy.GetAllProdf().Where(p =>listevisitecode.Any(p2 => p2 == p.cp)).ToList();
                    List<SVC.Prodf> resultprodf = proxy.GetAllProdf().Where(x => listevisitecode.Contains(x.cp)).ToList();
                    // List<SVC.Produit> result = proxy.GetAllProduit().Where(p => listevisitecode.Any(p2 => p2 == p.Id)).ToList();
                    List<SVC.Produit> result = proxy.GetAllProduit().Where(x => listevisitecode.Contains(x.Id)).ToList();
                    List<prodfcalcule> listprodfcal = new List<prodfcalcule>();
                    foreach (int itemint in listevisitecode)
                    {
                        foreach (SVC.Produit itemproduit in result)
                        {
                          //  foreach (SVC.Prodf itemprodf in resultprodf)
                            //{
                                if (itemproduit.Id ==itemint)
                                {
                                    prodfcalcule ta = new prodfcalcule
                                    {
                                        //cf = itemprodf.cf,
                                        cp = itemproduit.Id,
                                      //  datef = itemprodf.datef,
                                     //   dates = itemprodf.dates,
                                        design = itemproduit.design,
                                        famille = itemproduit.famille,
                                       // fourn = itemprodf.fourn,
                                        IdFamille = itemproduit.IdFamille,
                                       // lot = itemprodf.lot,
                                        //nfact = itemprodf.nfact,
                                        //perempt = itemprodf.perempt,
                                      //  previent = itemprodf.previent,
                                        quantite = 0,
                                        StockAlert = itemproduit.StockAlert,

                                    };
                                    listprodfcal.Add(ta);

                                
                            }

                        }
                    }

                    foreach (prodfcalcule item in listprodfcal)
                    {
                         foreach (SVC.Prodf itemprodf in resultprodf)
                        {
                        if (item.cp==itemprodf.cp)
                        {
                                item.cf = itemprodf.cf;
                                item.datef = itemprodf.datef;
                                item.dates = itemprodf.dates;
                                item.fourn = itemprodf.fourn;
                                item.lot = itemprodf.lot;
                                item.nfact = itemprodf.nfact;
                                item.perempt = itemprodf.perempt;
                                item.previent = itemprodf.previent;
                                item.quantite =item.quantite+ itemprodf.quantite;
                                
                        }
                    }
                    }
                    for (int i = listprodfcal.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/

                    {

                        var item = listprodfcal.ElementAt(i);

                        if (item.StockAlert<item.quantite || item.StockAlert==0 ||item.StockAlert==null )
                        {
                            listprodfcal.Remove(item);
                        }
                    }
                        StockalerteDatagrid.ItemsSource = listprodfcal;
                    txtFiches.Text = listprodfcal.Count().ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRecherchestockalerte_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(StockalerteDatagrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        prodfcalcule p = o as prodfcalcule;
                        if (t.Name == "txtId")
                            return (p.design == filter);
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
                    };
                }
                var lis = StockalerteDatagrid.Items.OfType<prodfcalcule>();
                txtFiches.Text = lis.Count().ToString();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimeur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ImpressionAchat == true && StockalerteDatagrid.Items.Count > 0 && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {

                    var itemsSource0 = StockalerteDatagrid.Items.OfType<prodfcalcule>();// List<SVC.SalleDattente>;
                    List<prodfcalcule> itemsSource1 = new List<prodfcalcule>();
                    foreach (prodfcalcule item in itemsSource0)
                    {
                        itemsSource1.Add(item);
                    }

                    ImpressionAlerte cl = new ImpressionAlerte(proxy, itemsSource1, DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value, txtFiches.Text);

                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
    public class prodfcalcule : SVC.Prodf
    {
        public Nullable<decimal> StockAlert { get; set; }
        public string famille { get; set; }
     
    }

}
