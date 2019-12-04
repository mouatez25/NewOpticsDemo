
using NewOptics.Administrateur;
using NewOptics.Fournisseur;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AjouterSupplement.xaml
    /// </summary>
    public partial class AjouterSupplement : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic membershi;
        ICallback callback;
        SVC.Supplement Supplement;
        private delegate void FaultedInvokerSupplement();
        int interfacecr = 0;
        List<SVC.VerreAssocie> anciennnelist;
        List<SVC.Incompatibilite> anciennnelistincompatibilite;
        public AjouterSupplement(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu, SVC.Supplement supplemenrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                membershi = memberrecu;
                if (supplemenrecu == null)
                {
                    interfacecr = 1;
                    ListeFournCombo.ItemsSource = proxy.GetAllFourn().OrderBy(n => n.raison);
                    ListeCatCombo.ItemsSource = proxy.GetAllCatSupp().OrderBy(n => n.Cat);
                    Supplement = new SVC.Supplement();
                    GridSupp.DataContext = Supplement;
                    List<SVC.VerreAssocie> ll = new List<SVC.VerreAssocie>();
                    VerreDataGrid.ItemsSource = ll;
                    List<SVC.Incompatibilite> lm = new List<SVC.Incompatibilite>();
                    IncoDataGrid.ItemsSource = lm;
                }
                else
                {
                    interfacecr = 2;
                    Supplement = supplemenrecu;
                    GridSupp.DataContext = Supplement;
                    VerreDataGrid.ItemsSource = proxy.GetAllVerreAssociebySupplement(Supplement.VerresAssociés);
                    anciennnelist = proxy.GetAllVerreAssociebySupplement(Supplement.VerresAssociés);
                    anciennnelistincompatibilite = proxy.GetAllIncompatibilitebySupplement(Supplement.Incompatibilités);
                    IncoDataGrid.ItemsSource = proxy.GetAllIncompatibilitebySupplement(Supplement.Incompatibilités);
                    List<SVC.Fourn> testmedecin = proxy.GetAllFourn().OrderBy(n => n.raison).ToList(); ;
                    ListeFournCombo.ItemsSource = testmedecin;
                    List<SVC.Fourn> tte = testmedecin.Where(n => n.Id == Supplement.IdFourn).ToList();
                    ListeFournCombo.SelectedItem = tte.First();

                    List<SVC.CatSupp> testmedecincat = proxy.GetAllCatSupp().OrderBy(n => n.Cat).ToList(); ;
                    ListeCatCombo.ItemsSource = testmedecincat;
                    List<SVC.CatSupp> ttecat = testmedecincat.Where(n => n.Id == Supplement.IdCat).ToList();
                    ListeCatCombo.SelectedItem = ttecat.First();
                }
                callbackrecu.InsertFournCallbackEvent += new ICallback.CallbackEventHandler20(callbackMarque_Refresh);
                callbackrecu.InsertIncompatibiliteCallbackevent += new ICallback.CallbackEventHandler14(callbackInco_Refresh);
                callbackrecu.InsertVerreAssocieCallbackevent += new ICallback.CallbackEventHandler12(callbackIVerre_Refresh);
                callbackrecu.InsertCatSuppCallbackevent += new ICallback.CallbackEventHandler56(callbackCat_Refresh);


            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void callbackIVerre_Refresh(object source, CallbackEventInsertVerreAssocie e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshVerre(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefreshVerre(SVC.VerreAssocie listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = VerreDataGrid.ItemsSource as IEnumerable<SVC.VerreAssocie>;
                List<SVC.VerreAssocie> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {
                        //  var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;


                        var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listMembershipOptic;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                VerreDataGrid.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        void callbackInco_Refresh(object source, CallbackEventInsertIncompatibilite e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshInco(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefreshInco(SVC.Incompatibilite listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = IncoDataGrid.ItemsSource as IEnumerable<SVC.Incompatibilite>;
                List<SVC.Incompatibilite> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {
                        //  var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;


                        var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listMembershipOptic;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                IncoDataGrid.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        void callbackCat_Refresh(object source, CallbackEventInsertCatSupp e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshCatSupp(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefreshCatSupp(List<SVC.CatSupp> listMembershipOptic)
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
                        if (interfacecr == 1)
                        {


                            ListeCatCombo.ItemsSource = listMembershipOptic;
                        }
                        else
                        {
                            if (interfacecr == 2)
                            {
                                List<SVC.CatSupp> testmedecin = listMembershipOptic;
                                ListeCatCombo.ItemsSource = testmedecin;
                                List<SVC.CatSupp> tte = testmedecin.Where(n => n.Id == Supplement.IdCat).ToList();
                                ListeCatCombo.SelectedItem = tte.First();
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        void callbackMarque_Refresh(object source, CallbackEventInsertFourn e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshMarque(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefreshMarque(List<SVC.Fourn> listMembershipOptic)
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
                        if (interfacecr == 1)
                        {


                            ListeFournCombo.ItemsSource = listMembershipOptic;
                        }
                        else
                        {
                            if (interfacecr == 2)
                            {
                                List<SVC.Fourn> testmedecin = listMembershipOptic;
                                ListeFournCombo.ItemsSource = testmedecin;
                                List<SVC.Fourn> tte = testmedecin.Where(n => n.Id == Supplement.IdFourn).ToList();
                                ListeFournCombo.SelectedItem = tte.First();
                            }

                        }

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSupplement(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSupplement(HandleProxy));
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

        private void btnFourn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (membershi.CreationFichier == true)
                {
                    AjouterFournisseur cl = new AjouterFournisseur(proxy, null, membershi);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtachat_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void txtvente_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (interfacecr == 1 && membershi.CreationFichier == true)
                {
                    bool opersucces = false;
                    bool operVerreAssocie = false;
                    bool operInco = false;
                    if (ListeFournCombo.SelectedItem != null)
                    {
                        SVC.Fourn selectedfourn = ListeFournCombo.SelectedItem as SVC.Fourn;
                        Supplement.Fournisseur = selectedfourn.raison;
                        Supplement.IdFourn = selectedfourn.Id;
                    }
                    if (ListeCatCombo.SelectedItem != null)
                    {
                        SVC.CatSupp selectedfourn = ListeCatCombo.SelectedItem as SVC.CatSupp;
                        Supplement.Categorie = selectedfourn.Cat;
                        Supplement.IdCat = selectedfourn.Id;
                    }
                    Supplement.VerresAssociés = DateTime.Now.Date.ToString() + DateTime.Now.TimeOfDay.ToString();
                    Supplement.Incompatibilités = DateTime.Now.Date.ToString() + DateTime.Now.TimeOfDay.ToString();

                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        var LISTITEM1 = VerreDataGrid.ItemsSource as IEnumerable<SVC.VerreAssocie>;
                        if (LISTITEM1.Count() > 0)
                        {
                            foreach (SVC.VerreAssocie item in LISTITEM1)
                            {
                                operVerreAssocie = false;
                                item.CleSupplement = Supplement.VerresAssociés;
                                proxy.InsertVerreAssocie(item);
                                operVerreAssocie = true;
                            }
                        }
                        else
                        {
                            operVerreAssocie = true;
                        }
                        var LISTITEM2 = IncoDataGrid.ItemsSource as IEnumerable<SVC.Incompatibilite>;
                        if (LISTITEM2.Count() > 0)
                        {
                            foreach (SVC.Incompatibilite item in LISTITEM2)
                            {
                                operInco = false;
                                item.CleSupplement = Supplement.Incompatibilités;
                                proxy.InsertIncompatibilite(item);
                                operInco = true;
                            }
                        }
                        else
                        {
                            operInco = true;
                        }

                        proxy.InsertSupplement(Supplement);
                        opersucces = true;
                        if (opersucces == true && operVerreAssocie == true && operInco == true)
                        {
                            ts.Complete();
                        }

                    }
                    if (opersucces == true && operVerreAssocie == true && operInco == true)
                    {
                        proxy.AjouterSupplementRefresh();
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {
                    if (interfacecr == 2 && membershi.ModificationFichier == true)
                    {
                        bool opersucces = false;
                        bool operVerreAssocie = false;
                        bool operInco = false;
                        if (ListeFournCombo.SelectedItem != null)
                        {
                            SVC.Fourn selectedfourn = ListeFournCombo.SelectedItem as SVC.Fourn;
                            Supplement.Fournisseur = selectedfourn.raison;
                            Supplement.IdFourn = selectedfourn.Id;
                        }
                        if (ListeCatCombo.SelectedItem != null)
                        {
                            SVC.CatSupp selectedfourn = ListeCatCombo.SelectedItem as SVC.CatSupp;
                            Supplement.Categorie = selectedfourn.Cat;
                            Supplement.IdCat = selectedfourn.Id;
                        }
                        var LISTITEM1 = VerreDataGrid.ItemsSource as IEnumerable<SVC.VerreAssocie>;

                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (LISTITEM1.Count() > 0)
                            {
                                foreach (SVC.VerreAssocie item in LISTITEM1)
                                {
                                    var existe = anciennnelist.Any(n => n.Id == item.Id);
                                    if (existe == false)
                                    {
                                        operVerreAssocie = false;
                                        item.CleSupplement = Supplement.VerresAssociés;
                                        proxy.InsertVerreAssocie(item);
                                        operVerreAssocie = true;
                                    }
                                    else
                                    {
                                        operVerreAssocie = false;
                                        //item.CleSupplement = Supplement.VerresAssociés;
                                        proxy.UpdateVerreAssocie(item);
                                        operVerreAssocie = true;
                                    }
                                }
                            }
                            else
                            {
                                operVerreAssocie = true;
                            }
                            var LISTITEM2 = IncoDataGrid.ItemsSource as IEnumerable<SVC.Incompatibilite>;
                            if (LISTITEM2.Count() > 0)
                            {
                                foreach (SVC.Incompatibilite item in LISTITEM2)
                                {
                                    var existe = anciennnelistincompatibilite.Any(n => n.Id == item.Id);

                                    if (existe == false)
                                    {
                                        operInco = false;
                                        item.CleSupplement = Supplement.Incompatibilités;
                                        proxy.InsertIncompatibilite(item);
                                        operInco = true;
                                    }
                                    else
                                    {
                                        operInco = false;
                                        // item.CleSupplement = Supplement.Incompatibilités;
                                        proxy.UpdateIncompatibilite(item);
                                        operInco = true;
                                    }
                                }
                            }
                            else
                            {
                                operInco = true;
                            }
                            proxy.UpdateSupplement(Supplement);
                            opersucces = true;
                            if (opersucces == true && operVerreAssocie == true && operInco == true)
                            {
                                ts.Complete();
                            }
                        }
                        if (opersucces == true && operVerreAssocie == true && operInco == true)
                        {
                            proxy.AjouterSupplementRefresh();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
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
                ListVerreSup cl = new ListVerreSup(proxy, membershi, callback, this);
                cl.Show();
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
                if (VerreDataGrid.SelectedItem != null && interfacecr == 2)
                {
                    SVC.VerreAssocie selecteditem = VerreDataGrid.SelectedItem as SVC.VerreAssocie;
                    bool succe = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.DeleteVerreAssocie(selecteditem);
                        succe = true;
                    }
                    if (succe == true)
                    {
                        //    proxy.AjouterVerreAssocieRefresh(selecteditem.Id);
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
                else
                {
                    if (VerreDataGrid.SelectedItem != null && interfacecr == 1)
                    {
                        SVC.VerreAssocie selecteditem = VerreDataGrid.SelectedItem as SVC.VerreAssocie;
                        var LISTITEM1 = VerreDataGrid.ItemsSource as IEnumerable<SVC.VerreAssocie>;
                        List<SVC.VerreAssocie> LISTITEM = LISTITEM1.ToList();
                        // var deleterendez = LISTITEM.Where(n => n.nfact == listmembership.nfact).First();
                        LISTITEM.Remove(selecteditem);
                        VerreDataGrid.ItemsSource = LISTITEM;

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

                if (VerreDataGrid.SelectedItem != null)
                {
                    SVC.VerreAssocie selectedverre = VerreDataGrid.SelectedItem as SVC.VerreAssocie;
                    Propriete cl = new Propriete(proxy, callback, membershi, selectedverre, txtDesign.Text);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnNewInc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListeSupplementSup cl = new ListeSupplementSup(proxy, membershi, callback, this);
                cl.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnSuppInc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IncoDataGrid.SelectedItem != null && interfacecr == 2)
                {
                    SVC.Incompatibilite selecteditem = IncoDataGrid.SelectedItem as SVC.Incompatibilite;
                    bool succe = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.DeleteIncompatibilite(selecteditem);
                        succe = true;
                    }
                    if (succe == true)
                    {
                        proxy.AjouterIncompatibiliteRefresh(selecteditem.Id);
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
                else
                {
                    if (IncoDataGrid.SelectedItem != null && interfacecr == 1)
                    {
                        SVC.Incompatibilite selecteditem = IncoDataGrid.SelectedItem as SVC.Incompatibilite;
                        var LISTITEM1 = IncoDataGrid.ItemsSource as IEnumerable<SVC.Incompatibilite>;
                        List<SVC.Incompatibilite> LISTITEM = LISTITEM1.ToList();
                        // var deleterendez = LISTITEM.Where(n => n.nfact == listmembership.nfact).First();
                        LISTITEM.Remove(selecteditem);
                        IncoDataGrid.ItemsSource = LISTITEM;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEditInc_Click(object sender, RoutedEventArgs e)
        {

        }



        private void txtDesign_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtDesign.Text != null && interfacecr == 1)
                {

                    var query = from c in proxy.GetAllSupplement()
                                select new { c.Libelle };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.Libelle.Trim().ToUpper() == txtDesign.Text.Trim().ToUpper()).FirstOrDefault();

                    if (disponible != null)
                    {


                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;


                    }
                    else
                    {
                        //   if (txtCodeCnas.Text.Trim() != "")
                        //  {

                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;

                        //}
                    }
                }
                else
                {

                    btnCreer.IsEnabled = true;
                    btnCreer.Opacity = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
    
