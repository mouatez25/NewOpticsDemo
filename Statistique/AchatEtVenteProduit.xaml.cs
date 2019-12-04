
using NewOptics.Administrateur;
using MahApps.Metro.Controls;
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
    /// Interaction logic for AchatEtVenteProduit.xaml
    /// </summary>
    public partial class AchatEtVenteProduit : Window
    {
        ICallback callback;
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MembershipOptic;
        SVC.Param selectedparam;
        private delegate void FaultedInvokerAchatVenteProduit();

        public AchatEtVenteProduit(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MembershipOptic = memberrecu;
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                NomenclatureProduit.ItemsSource = proxy.GetAllProduit().OrderBy(n => n.design); ;
                selectedparam = proxy.GetAllParamétre();
                if (selectedparam.AffichPrixAchatVente == true)
                {
                    ProduitHistoriquevente.Columns[3].Visibility = Visibility.Visible;
                }
                else
                {
                    ProduitHistoriquevente.Columns[3].Visibility = Visibility.Collapsed;
                }
                callbackrecu.InsertProduitCallbackEvent += new ICallback.CallbackEventHandler22(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
    

        }
        void callbackrecu_Refresh(object source, CallbackEventInsertProduit  e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav,e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAchatVenteProduit(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAchatVenteProduit(HandleProxy));
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
        public void AddRefresh(SVC.Produit listMembershipOptic,int oper)
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
                        var LISTITEM11 = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Produit>;
                        List<SVC.Produit> LISTITEM0 = LISTITEM11.ToList();

                        if (oper == 1)
                        {
                            LISTITEM0.Add(listMembershipOptic);
                        }
                        else
                        {
                            if (oper == 2)
                            {


                                var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                //objectmodifed = listMembershipOptic;
                                var index = LISTITEM0.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM0[index] = listMembershipOptic;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    var deleterendez = LISTITEM0.Where(n => n.Id == listMembershipOptic.Id).First();
                                    LISTITEM0.Remove(deleterendez);
                                }
                            }
                        }










                        NomenclatureProduit.ItemsSource = LISTITEM0.OrderBy(n => n.design);

                       
                    }
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
                ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                //  ;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Produit p = o as SVC.Produit;
                        if (t.Name == "txtId")
                            return (p.design == filter);
                        return (p.design.ToUpper().Contains(filter.ToUpper()));


                    };
                  //  txtAchat.Text = Convert.ToString((proxy.GetAllProdf().Where(n => n.design.Contains(filter.ToUpper()))).AsEnumerable().Sum(o => o.quantite * o.previent));

                }
            }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NomenclatureProduit.SelectedItem != null && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null && MembershipOptic.ModuleStatistiqueAcces==true)
                {

                    SVC.Produit  selectedprodf = NomenclatureProduit.SelectedItem as SVC.Produit;
                    List<SVC.Recept> listrecept= proxy.GetAllRecept().Where(n => n.codeprod == selectedprodf.Id && n.date >= DateSaisieDébut.SelectedDate.Value && n.date <= DateSaisieFin.SelectedDate.Value ).ToList();
                    foreach(var item in listrecept)
                    {
                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                        {
                            item.quantite = -1 * item.quantite;
                           // item.previent = -1 * item.previent;
                           // item.prixa = -1 * item.prixa;
                            //item.prixb = -1 * item.prixb;
                           // item.prixc = -1 * item.prixc;
                        }
                    }
                    ReceptDatagrid.ItemsSource = listrecept;
                    txtAchat.Text = Convert.ToString((listrecept.AsEnumerable().Sum(o => o.quantite * o.previent)));

                    List<SVC.Facture> listfacture = proxy.GetAllFactureBycodeproduit(selectedprodf.Id).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate).ToList();
                  
                    foreach(var item in listfacture)
                    {
                        if(item.nfact.Substring(0, 2) != "Co" && item.nfact.Substring(0, 1) == "C")
                        {
                            item.quantite = -1 * item.quantite;
                        }else
                        {
                            if (item.nfact.Substring(0, 1) == "A")
                            {
                                item.quantite = -1 * item.quantite;
                            }
                        }
                    }
                    ProduitHistoriquevente.ItemsSource = listfacture;
                    txtVente.Text = Convert.ToString((listfacture).AsEnumerable().Sum(o => o.quantite * o.privente));


                    NomProduitVente.Content =selectedprodf.Id+" "+selectedprodf.design.ToString();
                    NomProduitDestockage.Content = selectedprodf.Id+" "+ selectedprodf.design.ToString();

                }else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionner un produit", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        private void btnImprimerStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ImpressionAchat == true && ReceptDatagrid.Items.Count>0 && DateSaisieDébut.SelectedDate!=null && DateSaisieFin.SelectedDate!=null)
                {
                //   var listimpr = ReceptDatagrid.SelectedItems as IEnumerable<SVC.Recept>;
                   // List<SVC.Recept> test = ReceptDatagrid.ItemsSource as List<SVC.Recept>;
                    var itemsSource0 = ReceptDatagrid.ItemsSource as IEnumerable;// List<SVC.SalleDattente>;
                    List<SVC.Recept> itemsSource1 = new List<SVC.Recept>();
                    foreach (SVC.Recept item in itemsSource0)
                    {
                        itemsSource1.Add(item);
                    }
                    ImpressionAchat cl = new ImpressionAchat(proxy, itemsSource1, DateSaisieDébut.SelectedDate.Value,DateSaisieFin.SelectedDate.Value);

                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimerDestoStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ImpressionAchat == true && ProduitHistoriquevente.Items.Count > 0 && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {
                    //   var listimpr = ReceptDatagrid.SelectedItems as IEnumerable<SVC.Recept>;
                    // List<SVC.Recept> test = ReceptDatagrid.ItemsSource as List<SVC.Recept>;
                    var itemsSource0 = ProduitHistoriquevente.ItemsSource as IEnumerable;// List<SVC.SalleDattente>;
                    List<SVC.Facture> itemsSource1 = new List<SVC.Facture>();
                    foreach (SVC.Facture item in itemsSource0)
                    {
                        itemsSource1.Add(item);
                    }
                    ImpressionDestockage cl = new ImpressionDestockage(proxy, itemsSource1, DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value);

                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
