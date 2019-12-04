using NewOptics.Administrateur;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Transactions;
using System.Globalization;
using NewOptics.SVC;
using System.Threading;
 
using NewOptics.Fournisseur;
using NewOptics.Stock;
 

namespace NewOptics.Achat
{
    /// <summary>
    /// Interaction logic for AjouterFactureAchat.xaml
    /// </summary>
    public partial class AjouterFactureAchat :  Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        List<SVC.Recept> tte;
        ICallback callback;
        private delegate void FaultedInvokerFactureACHAT();
        SVC.Recouf depenseP;
        List<SVC.Recept> itemsSource;
        string title;
        Brush brushajouterfacture;
        int ajout;
        SVC.Client ClientConnécté;
        bool ReceptResult = false;
        bool RecoufResult = false;
        bool ProdfResult = false;
        decimal quantitebonus = 0;
        SVC.Param parametre;
        bool tunisie = false;
        public AjouterFactureAchat(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Recouf depenserecu, SVC.Client LOCALCLIENTRECU)
        {
            try
            {

                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                // title = this.Title;
                //  brushajouterfacture = this.WindowTitleBrush;
                NomenclatureProduit.ItemsSource = proxy.GetAllProduit().OrderBy(n => n.design); ;
                // Thread.CurrentThread.CurrentCulture = new CultureInfo("en-Us");
                cbDci.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
                ClientConnécté = LOCALCLIENTRECU;
                parametre = proxy.GetAllParamétre();
                if (parametre.Comon.Trim() == "TND")
                {
                    tunisie = true;
                    lblfiscal.Visibility = Visibility.Visible;
                    txtfiscla.Visibility = Visibility.Visible;
                }
                else
                {
                    tunisie = false;
                    lblfiscal.Visibility = Visibility.Collapsed;
                    txtfiscla.Visibility = Visibility.Collapsed;
                }
                if (depenserecu != null)
                {
                    ajout = 2;
                    depenseP = depenserecu;
                    GridRecouf.DataContext = depenseP;
                    //    tte = proxy.GetAllRecept().Where(n => n.Fournisseur == depenseP.Fournisseur && n.nfact.Trim() == depenseP.nfact.Trim() && n.cf == depenseP.codef).ToList();
                    tte = proxy.GetAllReceptBYNFACT(depenseP.nfact.Trim()).ToList();
                    ReceptDatagrid.ItemsSource = tte;
                    ReceptDatagrid.DataContext = tte;
                    List<SVC.Fourn> testmedecin1 = proxy.GetAllFourn();
                    ComboFournisseur.ItemsSource = testmedecin1;
                    List<SVC.Fourn> tte1 = testmedecin1.Where(n => n.raison == depenseP.Fournisseur && n.Id == depenseP.codef).ToList();
                    ComboFournisseur.SelectedItem = tte1.First();
                    txtTTC.Text = string.Format("{0:0.00}", (tte).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));

                    btnCreer.IsEnabled = true;
                    txtnfact.IsEnabled = false;
                    ComboFournisseur.IsEnabled = false;
                }
                else
                {
                    btnCreer.IsEnabled = false;
                    ajout = 1;
                    depenseP = new SVC.Recouf
                    {
                        username = memberuser.Username,
                        dates = DateTime.Now,
                        avoir = false,
                        Nonfiscal = true,
                        fiscal = false,
                        date = DateTime.Now,
                    };
                    GridRecouf.DataContext = depenseP;
                    ComboFournisseur.ItemsSource = proxy.GetAllFourn();
                    itemsSource = new List<SVC.Recept>();
                    ReceptDatagrid.DataContext = itemsSource;
                    ReceptDatagrid.CanUserDeleteRows = true;
                    txtRemise.Text = "";
                    txtTTC.Text = "";
                    txttva.Text = "";
                    txtht.Text = "";
                    //      ReceptDatagrid.ItemsSource = proxy.GetAllRecept();
                }


                callbackrecu.InsertProduitCallbackEvent += new ICallback.CallbackEventHandler22(callbackrecu_Refresh);
                callbackrecu.InsertReceptCallbackEvent += new ICallback.CallbackEventHandler24(callbackrecurecept_Refresh);
                callbackrecu.InsertFournCallbackEvent += new ICallback.CallbackEventHandler20(callbackrecufourn_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertProduit e)
        {
            try
            {
                Dispatcher.BeginInvoke
                    (DispatcherPriority.Normal, new Action(delegate
                    {
                        AddRefresh(e.clientleav, e.operleav);
                    }));
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }
        void callbackrecurecept_Refresh(object source, CallbackEventInsertRecept e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshRecept(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }
        void callbackrecufourn_Refresh(object source, CallbackEventInsertFourn e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshFourn(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }
        public void AddRefresh(SVC.Produit listMembership, int oper)
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
                            LISTITEM0.Add(listMembership);
                        }
                        else
                        {
                            if (oper == 2)
                            {


                                var objectmodifed = LISTITEM0.Find(n => n.Id == listMembership.Id);
                                //objectmodifed = listMembership;
                                var index = LISTITEM0.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM0[index] = listMembership;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembership.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    var deleterendez = LISTITEM0.Where(n => n.Id == listMembership.Id).First();
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
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }

        }
        public void AddRefreshRecept(List<SVC.Recept> listMembership)
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
                        if (listMembership.Count() > 0 && listMembership.Last().nfact == txtnfact.Text.Trim())
                        {
                            tte = listMembership.Where(n => n.nfact.Trim() == txtnfact.Text.Trim()).ToList();


                            ReceptDatagrid.ItemsSource = tte;
                            ReceptDatagrid.DataContext = tte;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }
        public void AddRefreshFourn(List<SVC.Fourn> listMembership)
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
                      
                        ComboFournisseur.ItemsSource = listMembership.OrderByDescending(n=>n.Id);
                        ComboFournisseur.DataContext = listMembership.OrderByDescending(n => n.Id);

                    }
                }
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerFactureACHAT(HandleProxy));
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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerFactureACHAT(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void AddProdfCompl(object sender, InsertProdfCompletedEventArgs e)
        {
            try
            {
                ProdfResult = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void AddReceptCompl(object sender, InsertReceptCompletedEventArgs e)
        {
            try
            {

                ReceptResult = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void AddRecoufCompl(object sender, InsertRecoufCompletedEventArgs e)
        {
            try
            {
                RecoufResult = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
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
                        try
                        {
                            #region creation
                            if (ajout == 1 && InformationComplet() && memberuser.CreationAchat == true)
                            {
                                bool quantitenull = false;
                                try
                                {

                                    //   CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                                    var itemsSource0 = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Recept>;
                                    List<SVC.Prodf> listproduit = new List<Prodf>();
                                    foreach (SVC.Recept item in itemsSource0)
                                    {
                                        if (item.CoutMoyen == true)
                                        {
                                            var existe = proxy.GetAllProdfbycode(Convert.ToInt32(item.codeprod)).Any(n => n.quantite != 0);
                                            if (existe)
                                            {
                                                var existelist = proxy.GetAllProdfbycode(Convert.ToInt32(item.codeprod)).Where(n => n.quantite != 0).First();
                                                listproduit.Add(existelist);

                                            }
                                            else
                                            {
                                                item.CoutMoyen = false;
                                            }
                                        }
                                    }
                                    /* foreach (SVC.Recept item in itemsSource0)
                                     {
                                         MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(item.CoutMoyen.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                         MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(item.perempt.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                     }*/
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {



                                        foreach (SVC.Recept item in itemsSource0)
                                        {
                                            if (item.CoutMoyen == false)
                                            {
                                                if (item.quantite > 0)/*&& item.previent > 0*/
                                                {
                                                    ReceptResult = false;
                                                    proxy.InsertRecept(item);
                                                    ReceptResult = true;

                                                    SVC.Prodf pa = new SVC.Prodf();
                                                    ProdfResult = false;
                                                    pa.cp = item.codeprod;
                                                    pa.design = item.design;
                                                    pa.quantite = item.quantite;
                                                    // pa.quantite = item.quantite * item.collisage;
                                                    // pa.previent = (item.previent) / item.collisage;+
                                                    // pa.prixa = (item.prixa) / item.collisage;
                                                    //  pa.prixb = (item.prixb) / item.collisage;
                                                    //  pa.prixc = (item.prixc) / item.collisage;
                                                    // pa.privente = (item.prixa) / item.collisage;
                                                    pa.previent = (item.previent);
                                                    pa.prixa = (item.prixa);
                                                    pa.prixb = (item.prixb);
                                                    pa.prixc = (item.prixc);
                                                    pa.tva = item.tva;
                                                    pa.cab = item.cab;
                                                    pa.privente = (item.prixa);
                                                    pa.collisage = item.quantite / item.collisage;

                                                    pa.nfact = item.nfact;
                                                    pa.fourn = item.Fournisseur;
                                                    pa.cf = item.cf;
                                                    pa.datef = item.date;
                                                    pa.dates = DateTime.Now;
                                                    pa.perempt = item.perempt;
                                                    pa.lot = item.lot;
                                                    pa.IdFamille = item.IdFamille;
                                                    pa.famille = item.Famille;
                                                    proxy.InsertProdf(pa);
                                                    ProdfResult = true;


                                                }
                                                else
                                                {
                                                    quantitenull = true;
                                                }
                                            }
                                            else
                                            {
                                                if (item.CoutMoyen == true)
                                                {
                                                    if (item.quantite > 0)/*&& item.previent > 0*/
                                                    {
                                                        ReceptResult = false;
                                                        proxy.InsertRecept(item);
                                                        ReceptResult = true;
                                                        var existein = listproduit.Any(n => n.cp == item.codeprod);
                                                        if (existein == false)
                                                        {
                                                            SVC.Prodf pa = new SVC.Prodf();
                                                            ProdfResult = false;
                                                            pa.cp = item.codeprod;
                                                            pa.design = item.design;
                                                            /*pa.quantite = item.quantite * item.collisage;
                                                            pa.previent = (item.previent) / item.collisage;
                                                            pa.prixa = (item.prixa) / item.collisage;
                                                            pa.prixb = (item.prixb) / item.collisage;
                                                            pa.prixc = (item.prixc) / item.collisage;
                                                            pa.privente = (item.prixa) / item.collisage;*/
                                                            pa.quantite = item.quantite;
                                                            pa.previent = (item.previent);
                                                            pa.prixa = (item.prixa);
                                                            pa.prixb = (item.prixb);
                                                            pa.prixc = (item.prixc);
                                                            pa.privente = (item.prixa);
                                                            pa.tva = item.tva;
                                                            pa.cab = item.cab;

                                                            pa.nfact = item.nfact;
                                                            pa.fourn = item.Fournisseur;
                                                            pa.cf = item.cf;
                                                            pa.datef = item.date;
                                                            pa.dates = DateTime.Now;
                                                            pa.perempt = item.perempt;
                                                            pa.lot = item.lot;
                                                            pa.IdFamille = item.IdFamille;
                                                            pa.famille = item.Famille;
                                                            pa.collisage = item.quantite / item.collisage;
                                                            proxy.InsertProdf(pa);
                                                            ProdfResult = true;
                                                        }
                                                        else
                                                        {
                                                            if (existein == true)
                                                            {
                                                                var pa = listproduit.Where(n => n.cp == item.codeprod).First();
                                                                // var newprevient = ((pa.quantite * pa.previent) + (item.quantite * item.collisage * (item.previent / item.collisage))) / (pa.quantite + (item.quantite * item.collisage));
                                                                /* pa.quantite = pa.quantite + (item.quantite * item.collisage);
                                                                 pa.prixa = (item.prixa) / item.collisage;
                                                                 pa.prixb = (item.prixb) / item.collisage;
                                                                 pa.prixc = (item.prixc) / item.collisage;
                                                                 pa.privente = (item.prixa) / item.collisage;*/
                                                                var newprevient = ((pa.quantite * pa.previent) + (item.quantite * item.previent)) / (pa.quantite + item.quantite);

                                                                ProdfResult = false;
                                                                pa.cp = item.codeprod;
                                                                pa.design = item.design;
                                                                pa.collisage = item.quantite / item.collisage;
                                                                pa.previent = newprevient;
                                                                pa.quantite = pa.quantite + (item.quantite);
                                                                pa.prixa = (item.prixa);
                                                                pa.prixb = (item.prixb);
                                                                pa.prixc = (item.prixc);
                                                                pa.privente = (item.prixa);
                                                                pa.tva = item.tva;
                                                                pa.cab = item.cab;

                                                                pa.nfact = item.nfact;
                                                                pa.fourn = item.Fournisseur;
                                                                pa.cf = item.cf;
                                                                pa.datef = item.date;
                                                                pa.dates = DateTime.Now;
                                                                pa.perempt = item.perempt;
                                                                pa.lot = item.lot;
                                                                pa.IdFamille = item.IdFamille;
                                                                pa.famille = item.Famille;
                                                                proxy.UpdateProdf(pa);
                                                                ProdfResult = true;
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        quantitenull = true;
                                                    }
                                                }
                                            }
                                        }

                                        SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                                        RecoufResult = false;
                                        depenseP.codef = SelectReceptFourn.Id;
                                        depenseP.Fournisseur = SelectReceptFourn.raison;

                                        depenseP.net = Convert.ToDecimal(txtNet.Text);
                                        depenseP.ht = Convert.ToDecimal(txtht.Text);
                                        depenseP.tva = Convert.ToDecimal(txttva.Text);
                                        if (txtRemise.Text != "")
                                        {
                                            depenseP.remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                        else
                                        {
                                            depenseP.remise = 0;
                                        }
                                        if (txteche.Text != "")
                                        {
                                            depenseP.echeance = Convert.ToInt32(txteche.Text);
                                        }
                                        else
                                        {
                                            depenseP.echeance = 0;
                                        }
                                        depenseP.username = memberuser.Username;
                                        depenseP.dates = DateTime.Now;
                                        depenseP.datecreat = DateTime.Now;
                                        depenseP.opercreat = memberuser.Username;
                                        depenseP.cle = Convert.ToString(SelectReceptFourn.Id) + Convert.ToString(txtnfact.Text) + Convert.ToString(txtDateOper.SelectedDate.Value.Date);
                                        depenseP.dateecheance = txtDateOper.SelectedDate.Value.AddDays(Convert.ToDouble(txteche.Text));
                                        depenseP.versement = 0;
                                        depenseP.reste = Convert.ToDecimal(txtNet.Text);
                                        proxy.InsertRecouf(depenseP);
                                        RecoufResult = true;



                                        if (ReceptResult && RecoufResult && ProdfResult && quantitenull == false)
                                        {
                                            ts.Complete();


                                            btnCreer.IsEnabled = false;

                                        }
                                        else
                                        {

                                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                    if (ReceptResult && RecoufResult && ProdfResult && quantitenull == false)
                                    {


                                        proxy.AjouterFactureAchatSansProdfRefresh(depenseP.nfact.Trim());
                                        proxy.AjouterProdfReceptRefresh(depenseP.Fournisseur, depenseP.nfact, depenseP.codef);
                                        proxy.AjouterRecoufRefresh();
                                        ReceptDatagrid.CanUserDeleteRows = false;
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        this.Close();
                                    }
                                    /* }*/
                                    /*  else
                                      {
                                          if (parametre.coutmoyen==true)
                                          {
                                              var itemsSource0 = ReceptDatagrid.ItemsSource as IEnumerable;
                                              List<SVC.Prodf> listproduit = new List<Prodf>();

                                              foreach (SVC.Recept item in itemsSource0)
                                              {
                                                  var existe=proxy.GetAllProdfbycode(Convert.ToInt32(item.codeprod)).Any(n=>n.quantite!=0);
                                                  if (existe)
                                                  {
                                                      var existelist = proxy.GetAllProdfbycode(Convert.ToInt32(item.codeprod)).Where(n => n.quantite != 0).First();
                                                      listproduit.Add(existelist);

                                                  }
                                              }
                                              using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                              {

                                                      foreach (SVC.Recept item in itemsSource0)
                                                  {
                                                      if (item.quantite > 0 && item.previent > 0)
                                                      {
                                                          ReceptResult = false;
                                                          proxy.InsertRecept(item);
                                                          ReceptResult = true;
                                                          var existein = listproduit.Any(n => n.cp == item.codeprod);
                                                          if (existein == false)
                                                          {
                                                              SVC.Prodf pa = new SVC.Prodf();
                                                              ProdfResult = false;
                                                              pa.cp = item.codeprod;
                                                              pa.design = item.design;
                                                              pa.quantite = item.quantite * item.collisage;
                                                              pa.previent = (item.previent) / item.collisage;
                                                              pa.prixa = (item.prixa) / item.collisage;
                                                              pa.prixb = (item.prixb) / item.collisage;
                                                              pa.prixc = (item.prixc) / item.collisage;
                                                              pa.tva = item.tva;
                                                              pa.cab = item.cab;
                                                              pa.privente = (item.prixa) / item.collisage;
                                                              pa.nfact = item.nfact;
                                                              pa.fourn = item.Fournisseur;
                                                              pa.cf = item.cf;
                                                              pa.datef = item.date;
                                                              pa.dates = DateTime.Now;
                                                              pa.perempt = item.perempt;
                                                              pa.lot = item.lot;
                                                              pa.IdFamille = item.IdFamille;
                                                              pa.famille = item.Famille;
                                                              proxy.InsertProdf(pa);
                                                              ProdfResult = true;
                                                          }
                                                          else
                                                          {
                                                              if (existein == true)
                                                              {
                                                                  var pa = listproduit.Where(n => n.cp == item.codeprod).First();
                                                                  var newprevient = ((pa.quantite * pa.previent) + (item.quantite * item.collisage * (item.previent/item.collisage))) / (pa.quantite + (item.quantite * item.collisage));
                                                                  ProdfResult = false;
                                                                  pa.cp = item.codeprod;
                                                                  pa.design = item.design;

                                                                  pa.previent = newprevient;
                                                                  pa.quantite = pa.quantite + (item.quantite * item.collisage);
                                                                  pa.prixa = (item.prixa) / item.collisage;
                                                                  pa.prixb = (item.prixb) / item.collisage;
                                                                  pa.prixc = (item.prixc) / item.collisage;
                                                                  pa.tva = item.tva;
                                                                  pa.cab = item.cab;
                                                                  pa.privente = (item.prixa) / item.collisage;
                                                                  pa.nfact = item.nfact;
                                                                  pa.fourn = item.Fournisseur;
                                                                  pa.cf = item.cf;
                                                                  pa.datef = item.date;
                                                                  pa.dates = DateTime.Now;
                                                                  pa.perempt = item.perempt;
                                                                  pa.lot = item.lot;
                                                                  pa.IdFamille = item.IdFamille;
                                                                  pa.famille = item.Famille;
                                                                  proxy.UpdateProdf(pa);
                                                                  ProdfResult = true;
                                                              }
                                                          }

                                                      }
                                                      else
                                                      {
                                                          quantitenull = true;
                                                      }

                                                  }

                                                  SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                                                  RecoufResult = false;
                                                  depenseP.codef = SelectReceptFourn.Id;
                                                  depenseP.Fournisseur = SelectReceptFourn.raison;

                                                  depenseP.net = Convert.ToDecimal(txtNet.Text);
                                                  depenseP.ht = Convert.ToDecimal(txtht.Text);
                                                  depenseP.tva = Convert.ToDecimal(txttva.Text);
                                                  if (txtRemise.Text != "")
                                                  {
                                                      depenseP.remise = Convert.ToDecimal(txtRemise.Text);
                                                  }
                                                  else
                                                  {
                                                      depenseP.remise = 0;
                                                  }
                                                  if (txteche.Text != "")
                                                  {
                                                      depenseP.echeance = Convert.ToInt32(txteche.Text);
                                                  }
                                                  else
                                                  {
                                                      depenseP.echeance = 0;
                                                  }
                                                  depenseP.username = memberuser.Username;
                                                  depenseP.dates = DateTime.Now;
                                                  depenseP.datecreat = DateTime.Now;
                                                  depenseP.opercreat = memberuser.Username;
                                                  depenseP.cle = Convert.ToString(SelectReceptFourn.Id) + Convert.ToString(txtnfact.Text) + Convert.ToString(txtDateOper.DateTime.Date);
                                                  depenseP.dateecheance = txtDateOper.DateTime.AddDays(Convert.ToDouble(txteche.Text));
                                                  depenseP.versement = 0;
                                                  depenseP.reste = Convert.ToDecimal(txtNet.Text);
                                                  proxy.InsertRecouf(depenseP);
                                                  RecoufResult = true;



                                                  if (ReceptResult && RecoufResult && ProdfResult && quantitenull == false)
                                                  {
                                                      ts.Complete();


                                                      btnCreer.IsEnabled = false;

                                                  }
                                                  else
                                                  {

                                                      MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                  }
                                              }
                                          }*/
                                    // }



                                }

                                catch (TransactionAbortedException ex)
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                }

                                catch (Exception ex)
                                {
                                    labelEtat.Content = ex.Message;
                                    labelEtat.Foreground = Brushes.Red;
                                }

                            }
                            #endregion
                            else
                            {
                                if (ajout == 2 && InformationComplet() && memberuser.ModificationAchat == true)
                                {
                                    bool quantitenull = true;
                                    try
                                    {
                                        var itemsSourceDatagrid = ReceptDatagrid.ItemsSource as IEnumerable;
                                        SVC.Fourn selectedfournisseur = ComboFournisseur.SelectedItem as SVC.Fourn;
                                        List<SVC.Recept> itemsSourcerecept = (proxy.GetAllRecept()).Where(n => n.Fournisseur == depenseP.Fournisseur && n.nfact == depenseP.nfact && n.cf == depenseP.codef).ToList();
                                        List<SVC.Prodf> itemsSourceProdf = (proxy.GetAllProdf()).Where(n => n.fourn == depenseP.Fournisseur && n.nfact == depenseP.nfact && n.cf == depenseP.codef).ToList();
                                        List<SVC.Prodf> listproduit = new List<Prodf>();

                                        foreach (SVC.Recept item in itemsSourceDatagrid)
                                        {
                                            var existe = proxy.GetAllProdfbycode(Convert.ToInt32(item.codeprod)).Any(n => n.quantite != 0);
                                            if (existe)
                                            {
                                                var existelist = proxy.GetAllProdfbycode(Convert.ToInt32(item.codeprod)).Where(n => n.quantite != 0).First();
                                                listproduit.Add(existelist);

                                            }
                                            else
                                            {
                                                item.CoutMoyen = false;
                                            }
                                        }
                                        using (var ts = new TransactionScope())
                                        {
                                            foreach (SVC.Recept item in itemsSourceDatagrid)
                                            {

                                                var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);

                                                if (found == null)
                                                {
                                                    if (item.CoutMoyen == false)
                                                    {
                                                        if (item.quantite > 0)/*&& item.previent > 0*/
                                                        {
                                                            ReceptResult = false;
                                                            proxy.InsertRecept(item);
                                                            ReceptResult = true;

                                                            SVC.Prodf pa = new SVC.Prodf();
                                                            ProdfResult = false;
                                                            pa.cp = item.codeprod;
                                                            pa.design = item.design;

                                                            pa.collisage = item.quantite / item.collisage;
                                                            /* pa.quantite = item.quantite * item.collisage;
                                                             pa.previent = (item.previent) / item.collisage;
                                                             pa.prixa = (item.prixa) / item.collisage;
                                                             pa.prixb = (item.prixb) / item.collisage;
                                                             pa.prixc = (item.prixc) / item.collisage;
                                                             pa.privente = (item.prixa) / item.collisage;*/
                                                            pa.quantite = item.quantite;
                                                            pa.previent = (item.previent);
                                                            pa.prixa = (item.prixa);
                                                            pa.prixb = (item.prixb);
                                                            pa.prixc = (item.prixc);
                                                            pa.privente = (item.prixa);
                                                            pa.tva = item.tva;
                                                            pa.cab = item.cab;



                                                            pa.nfact = item.nfact;
                                                            pa.fourn = item.Fournisseur;
                                                            pa.cf = item.cf;
                                                            pa.datef = item.date;
                                                            pa.dates = DateTime.Now;
                                                            pa.perempt = item.perempt;
                                                            pa.lot = item.lot;
                                                            pa.IdFamille = item.IdFamille;
                                                            pa.famille = item.Famille;
                                                            proxy.InsertProdf(pa);
                                                            ProdfResult = true;


                                                        }
                                                        else
                                                        {
                                                            quantitenull = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (item.CoutMoyen == true)
                                                        {
                                                            if (item.quantite > 0)/*&& item.previent > 0*/
                                                            {
                                                                ReceptResult = false;
                                                                proxy.InsertRecept(item);
                                                                ReceptResult = true;
                                                                var existein = listproduit.Any(n => n.cp == item.codeprod);
                                                                if (existein == false)
                                                                {
                                                                    SVC.Prodf pa = new SVC.Prodf();
                                                                    ProdfResult = false;
                                                                    pa.cp = item.codeprod;
                                                                    pa.design = item.design;
                                                                    pa.collisage = item.quantite / item.collisage;
                                                                    /*  pa.quantite = item.quantite * item.collisage;
                                                                      pa.previent = (item.previent) / item.collisage;
                                                                      pa.prixa = (item.prixa) / item.collisage;
                                                                      pa.prixb = (item.prixb) / item.collisage;
                                                                      pa.prixc = (item.prixc) / item.collisage;
                                                                      pa.privente = (item.prixa) / item.collisage;*/
                                                                    pa.quantite = item.quantite;
                                                                    pa.previent = (item.previent);
                                                                    pa.prixa = (item.prixa);
                                                                    pa.prixb = (item.prixb);
                                                                    pa.prixc = (item.prixc);
                                                                    pa.privente = (item.prixa);
                                                                    pa.tva = item.tva;
                                                                    pa.cab = item.cab;

                                                                    pa.nfact = item.nfact;
                                                                    pa.fourn = item.Fournisseur;
                                                                    pa.cf = item.cf;
                                                                    pa.datef = item.date;
                                                                    pa.dates = DateTime.Now;
                                                                    pa.perempt = item.perempt;
                                                                    pa.lot = item.lot;
                                                                    pa.IdFamille = item.IdFamille;
                                                                    pa.famille = item.Famille;
                                                                    proxy.InsertProdf(pa);
                                                                    ProdfResult = true;
                                                                }
                                                                else
                                                                {
                                                                    if (existein == true)
                                                                    {
                                                                        var pa = listproduit.Where(n => n.cp == item.codeprod).First();
                                                                        /* var newprevient = ((pa.quantite * pa.previent) + (item.quantite * item.collisage * (item.previent / item.collisage))) / (pa.quantite + (item.quantite * item.collisage));
                                                                         pa.quantite = pa.quantite + (item.quantite * item.collisage);
                                                                         pa.prixa = (item.prixa) / item.collisage;
                                                                         pa.prixb = (item.prixb) / item.collisage;
                                                                         pa.prixc = (item.prixc) / item.collisage;
                                                                         pa.privente = (item.prixa) / item.collisage;*/
                                                                        var newprevient = ((pa.quantite * pa.previent) + (item.quantite * (item.previent))) / (pa.quantite + (item.quantite));
                                                                        pa.quantite = pa.quantite + (item.quantite);
                                                                        pa.prixa = (item.prixa);
                                                                        pa.prixb = (item.prixb);
                                                                        pa.prixc = (item.prixc);
                                                                        pa.privente = (item.prixa);
                                                                        ProdfResult = false;
                                                                        pa.cp = item.codeprod;
                                                                        pa.design = item.design;
                                                                        pa.collisage = item.quantite / item.collisage;
                                                                        pa.previent = newprevient;

                                                                        pa.tva = item.tva;
                                                                        pa.cab = item.cab;

                                                                        pa.nfact = item.nfact;
                                                                        pa.fourn = item.Fournisseur;
                                                                        pa.cf = item.cf;
                                                                        pa.datef = item.date;
                                                                        pa.dates = DateTime.Now;
                                                                        pa.perempt = item.perempt;
                                                                        pa.lot = item.lot;
                                                                        pa.IdFamille = item.IdFamille;
                                                                        pa.famille = item.Famille;
                                                                        proxy.UpdateProdf(pa);
                                                                        ProdfResult = true;
                                                                    }
                                                                }

                                                            }
                                                            else
                                                            {
                                                                quantitenull = true;
                                                            }
                                                        }
                                                    }
                                                    /*  ReceptResult = false;
                                                      if (item.quantite > 0 && item.previent > 0)
                                                      {

                                                          proxy.InsertRecept(item);
                                                          ReceptResult = true;
                                                          var existein = listproduit.Any(n => n.cp == item.codeprod);
                                                          if (existein == false)
                                                          {
                                                              SVC.Prodf pa = new SVC.Prodf();
                                                              ProdfResult = false;
                                                              pa.cp = item.codeprod;
                                                              pa.design = item.design;
                                                              pa.quantite = item.quantite * item.collisage;
                                                              pa.previent = (item.previent) / item.collisage;
                                                              pa.prixa = (item.prixa) / item.collisage;
                                                              pa.prixb = (item.prixb) / item.collisage;
                                                              pa.prixc = (item.prixc) / item.collisage;
                                                              pa.tva = item.tva;
                                                              pa.cab = item.cab;
                                                              pa.privente = (item.prixa) / item.collisage;
                                                              pa.nfact = item.nfact;
                                                              pa.fourn = selectedfournisseur.raison;
                                                              pa.cf = selectedfournisseur.Id;
                                                              pa.datef = item.date;
                                                              pa.dates = DateTime.Now;
                                                              pa.perempt = item.perempt;
                                                              pa.lot = item.lot;
                                                              pa.IdFamille = item.IdFamille;
                                                              pa.famille = item.Famille;
                                                              proxy.InsertProdf(pa);
                                                              ProdfResult = true;
                                                          }
                                                          else
                                                          {
                                                              if (existein == true)
                                                              {
                                                                  var pa = listproduit.Where(n => n.cp == item.codeprod).First();
                                                                  var newprevient = ((pa.quantite * pa.previent) + (item.quantite * item.collisage * (item.previent / item.collisage))) / (pa.quantite + (item.quantite * item.collisage));
                                                                  ProdfResult = false;
                                                                  pa.cp = item.codeprod;
                                                                  pa.design = item.design;

                                                                  pa.previent = newprevient;
                                                                  pa.quantite = pa.quantite + (item.quantite * item.collisage);
                                                                  pa.prixa = (item.prixa) / item.collisage;
                                                                  pa.prixb = (item.prixb) / item.collisage;
                                                                  pa.prixc = (item.prixc) / item.collisage;
                                                                  pa.tva = item.tva;
                                                                  pa.cab = item.cab;
                                                                  pa.privente = (item.prixa) / item.collisage;
                                                                  pa.nfact = item.nfact;
                                                                  pa.fourn = item.Fournisseur;
                                                                  pa.cf = item.cf;
                                                                  pa.datef = item.date;
                                                                  pa.dates = DateTime.Now;
                                                                  pa.perempt = item.perempt;
                                                                  pa.lot = item.lot;
                                                                  pa.IdFamille = item.IdFamille;
                                                                  pa.famille = item.Famille;
                                                                  proxy.UpdateProdf(pa);
                                                                  ProdfResult = true;
                                                              }
                                                          }
                                                      }
                                                      else
                                                      {
                                                          quantitenull = false;
                                                      }*/

                                                }
                                                else
                                                {/*Le cas ou le produit existe déja*/
                                                    if (found != null)
                                                    {
                                                        if (item.quantite > 0)/*&& item.previent > 0*//*Si la quantite et le prix de revient*/
                                                        {

                                                            var pa = (itemsSourceProdf).Find(n => n.cp == item.codeprod && n.nfact == item.nfact);

                                                            // var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);
                                                            var Value = (found.quantite) - (item.quantite);

                                                            ReceptResult = false;

                                                            /*****************nOUVELLE QUANTITE INF A NOUVELLE AJOUTER LE PRODUIT****************/
                                                            if (Value > 0)
                                                            {

                                                                item.cf = selectedfournisseur.Id;

                                                                item.Fournisseur = selectedfournisseur.raison;


                                                                proxy.UpdateRecept(item);
                                                                //  MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("UpdateRecept positive avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                ReceptResult = true;
                                                                ProdfResult = false;
                                                                if (pa.quantite >= Value)
                                                                {

                                                                    pa.cp = item.codeprod;
                                                                    pa.design = item.design;
                                                                    pa.quantite = (pa.quantite) - (Value);
                                                                    //   pa.previent = (item.previent + ((item.previent * item.tva) / 100)) / item.collisage;

                                                                    /******************************/
                                                                    pa.previent = (item.previent);
                                                                    pa.prixa = (item.prixa);
                                                                    pa.prixb = (item.prixb);
                                                                    pa.prixc = (item.prixc);
                                                                    pa.tva = item.tva;
                                                                    pa.cab = item.cab;
                                                                    pa.privente = (item.prixa);
                                                                    pa.collisage = item.quantite / item.collisage;
                                                                    /*pa.previent = (item.previent) / item.collisage;
                                                                    pa.prixa = (item.prixa) / item.collisage;
                                                                    pa.prixb = (item.prixb) / item.collisage;
                                                                    pa.prixc = (item.prixc) / item.collisage;
                                                                    pa.tva = item.tva;
                                                                    pa.cab = item.cab;
                                                                    pa.privente = (item.prixa) / item.collisage;*/
                                                                    /*********************************************/


                                                                    pa.nfact = item.nfact;
                                                                    pa.fourn = selectedfournisseur.raison;
                                                                    pa.cf = selectedfournisseur.Id;
                                                                    pa.datef = item.date;
                                                                    // pa.dates = DateTime.Now;
                                                                    pa.perempt = item.perempt;
                                                                    pa.lot = item.lot;
                                                                    pa.IdFamille = item.IdFamille;
                                                                    pa.famille = item.Famille;


                                                                    proxy.UpdateProdf(pa);
                                                                    //    MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("UpdateProdf positive avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                    ProdfResult = true;
                                                                }
                                                                else
                                                                {
                                                                    if (pa.quantite < Value)
                                                                    {
                                                                        MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("La quantité  " + pa.quantite + " seulement disponible en stock pour " + pa.design + " Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                                        ProdfResult = false;
                                                                        quantitenull = false;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ReceptResult = false;
                                                                if (Value < 0)
                                                                {
                                                                    item.cf = selectedfournisseur.Id;

                                                                    item.Fournisseur = selectedfournisseur.raison;

                                                                    proxy.UpdateRecept(item);
                                                                    //  MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("UpdateRecept négative avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                    ReceptResult = true;

                                                                    ProdfResult = false;
                                                                    pa.cp = item.codeprod;
                                                                    pa.design = item.design;
                                                                    pa.quantite = (pa.quantite) - (Value);
                                                                    // pa.previent = (item.previent + ((item.previent * item.tva) / 100)) / item.collisage;
                                                                    /******************************/
                                                                    pa.previent = (item.previent);
                                                                    pa.prixa = (item.prixa);
                                                                    pa.prixb = (item.prixb);
                                                                    pa.prixc = (item.prixc);
                                                                    pa.tva = item.tva;
                                                                    pa.cab = item.cab;
                                                                    pa.privente = (item.prixa);

                                                                    /*   pa.previent = (item.previent) / item.collisage;
                                                                       pa.prixa = (item.prixa) / item.collisage;
                                                                       pa.prixb = (item.prixb) / item.collisage;
                                                                       pa.prixc = (item.prixc) / item.collisage;
                                                                       pa.tva = item.tva;
                                                                       pa.cab = item.cab;
                                                                       pa.privente = (item.prixa) / item.collisage;*/

                                                                    /*********************************************/
                                                                    pa.collisage = item.quantite / item.collisage;



                                                                    pa.nfact = item.nfact;
                                                                    pa.fourn = selectedfournisseur.raison;
                                                                    pa.cf = selectedfournisseur.Id;
                                                                    pa.datef = item.date;
                                                                    // pa.dates = DateTime.Now;
                                                                    pa.perempt = item.perempt;
                                                                    pa.lot = item.lot;
                                                                    pa.IdFamille = item.IdFamille;
                                                                    pa.famille = item.Famille;
                                                                    proxy.UpdateProdf(pa);

                                                                    ProdfResult = true;

                                                                }
                                                                else
                                                                {
                                                                    if (Value == 0)
                                                                    {
                                                                        item.cf = selectedfournisseur.Id;

                                                                        item.Fournisseur = selectedfournisseur.raison;

                                                                        proxy.UpdateRecept(item);
                                                                        //     MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("UpdateRecept rien de special avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                        ReceptResult = true;
                                                                        ProdfResult = false;
                                                                        pa.cp = item.codeprod;
                                                                        pa.design = item.design;
                                                                        //  pa.previent = (item.previent + ((item.previent * item.tva) / 100)) / item.collisage;
                                                                        /******************************/
                                                                        pa.previent = (item.previent);
                                                                        pa.prixa = (item.prixa);
                                                                        pa.prixb = (item.prixb);
                                                                        pa.prixc = (item.prixc);
                                                                        pa.tva = item.tva;
                                                                        pa.cab = item.cab;
                                                                        pa.privente = (item.prixa);

                                                                        /*  pa.previent = (item.previent) / item.collisage;
                                                                          pa.prixa = (item.prixa) / item.collisage;
                                                                          pa.prixb = (item.prixb) / item.collisage;
                                                                          pa.prixc = (item.prixc) / item.collisage;
                                                                          pa.tva = item.tva;
                                                                          pa.cab = item.cab;
                                                                          pa.privente = (item.prixa) / item.collisage;*/
                                                                        /*********************************************/
                                                                        pa.collisage = item.quantite / item.collisage;
                                                                        pa.nfact = item.nfact;
                                                                        pa.fourn = selectedfournisseur.raison;
                                                                        pa.cf = selectedfournisseur.Id;
                                                                        pa.datef = item.date;
                                                                        pa.perempt = item.perempt;
                                                                        pa.lot = item.lot;
                                                                        pa.IdFamille = item.IdFamille;
                                                                        pa.famille = item.Famille;
                                                                        proxy.UpdateProdf(pa);
                                                                        //  MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("update prodf rien de special avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                        ProdfResult = true;
                                                                    }

                                                                }

                                                            }

                                                        }///fin de quantite et prix achat supp 0 c-a-d modification autorisée
                                                        else
                                                        {
                                                            if (item.quantite == 0)/*Supprimer Un Produit dans la facture*/
                                                            {

                                                                var pa = (itemsSourceProdf).Find(n => n.cp == item.codeprod && n.nfact == item.nfact);
                                                                //   var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);

                                                                var Value = (found.quantite) - (item.quantite);
                                                                if (Value == (found.quantite))
                                                                {
                                                                    if (pa.quantite == Value)
                                                                    {
                                                                        ProdfResult = false;
                                                                        ReceptResult = false;
                                                                        proxy.DeleteRecept(item);
                                                                        proxy.DeleteProdf(pa);
                                                                        ProdfResult = true;
                                                                        ReceptResult = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (pa.quantite > Value)
                                                                        {
                                                                            ProdfResult = false;
                                                                            ReceptResult = false;
                                                                            proxy.DeleteRecept(item);
                                                                            ReceptResult = true;

                                                                            pa.quantite = (pa.quantite) - (Value);

                                                                            pa.nfact = "ce produit a été supprimer de sa facture original le " + DateTime.Now;
                                                                            pa.fourn = item.Fournisseur;
                                                                            pa.cf = item.cf;
                                                                            pa.IdFamille = item.IdFamille;
                                                                            pa.famille = item.Famille;
                                                                            proxy.UpdateProdf(pa);

                                                                            ProdfResult = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (pa.quantite > Value)
                                                                            {
                                                                                MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("La quantité  " + pa.quantite + " seulement disponible en stock pour " + pa.design + " Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                                ReceptResult = false;
                                                                quantitenull = false;
                                                            }
                                                        }//fin de cas nouvelle quantite=0 dans recept

                                                    }
                                                }//existe fin

                                            }///fin de parcour forech in new itemsource

                                            // SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                                            RecoufResult = false;
                                            depenseP.codef = selectedfournisseur.Id;
                                            depenseP.Fournisseur = selectedfournisseur.raison;
                                            depenseP.avoir = false; depenseP.net = Convert.ToDecimal(txtNet.Text);
                                            depenseP.ht = Convert.ToDecimal(txtht.Text);
                                            depenseP.tva = Convert.ToDecimal(txttva.Text);
                                            if (txtRemise.Text != "")
                                            {
                                                depenseP.remise = Convert.ToDecimal(txtRemise.Text);
                                            }
                                            else
                                            {
                                                depenseP.remise = 0;
                                            }
                                            if (txteche.Text != "")
                                            {
                                                depenseP.echeance = Convert.ToInt32(txteche.Text);
                                            }
                                            else
                                            {
                                                depenseP.echeance = 0;
                                            }
                                            depenseP.username = memberuser.Username;
                                            depenseP.dates = DateTime.Now;
                                            depenseP.datecreat = DateTime.Now;
                                            depenseP.opercreat = memberuser.Username;
                                            depenseP.cle = Convert.ToString(selectedfournisseur.Id) + Convert.ToString(txtnfact.Text) + Convert.ToString(txtDateOper.SelectedDate.Value.Date);
                                            depenseP.dateecheance = txtDateOper.SelectedDate.Value.AddDays(Convert.ToDouble(txteche.Text));
                                            depenseP.reste = Convert.ToDecimal(txtNet.Text) - depenseP.versement;
                                            //  depenseP.reste = Convert.ToDecimal(txtNet.Text);
                                            proxy.UpdateRecouf(depenseP);
                                            if (depenseP.versement != 0 && depenseP.net == 0)
                                            {
                                                //   MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                RecoufResult = true;
                                            }
                                            else
                                            {
                                                if (depenseP.versement == 0 && depenseP.net == 0)
                                                {
                                                    proxy.DeleteRecouf(depenseP);
                                                    RecoufResult = true;
                                                }
                                                else
                                                {
                                                    if (depenseP.versement == 0 && depenseP.net != 0)
                                                    {
                                                        RecoufResult = true;

                                                    }
                                                    else
                                                    {
                                                        if (depenseP.versement != 0 && depenseP.net != 0)
                                                        {
                                                            RecoufResult = true;
                                                        }
                                                    }
                                                }
                                            }


                                            if (ReceptResult && ProdfResult && RecoufResult && quantitenull)
                                            {
                                                ts.Complete();
                                            }
                                            else
                                            {
                                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                            }





                                        }
                                        if (ReceptResult && ProdfResult && RecoufResult && quantitenull)
                                        {
                                            proxy.AjouterFactureAchatSansProdfRefresh(depenseP.nfact.Trim());
                                            proxy.AjouterRecoufRefresh();

                                            proxy.AjouterProdfReceptRefresh(depenseP.Fournisseur, depenseP.nfact, depenseP.codef);
                                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                            this.Close();
                                        }
                                    }
                                    catch (FaultException ex)
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    }




                                }

                            }
                            // }
                            // }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void NomenclatureProduit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (ajout == 1 && NomenclatureProduit.SelectedItem != null && InformationComplet())
                {
                    //    this.Title = title;
                    //  this.WindowTitleBrush = brushajouterfacture;
                    SVC.Produit SelectReceptNome = NomenclatureProduit.SelectedItem as SVC.Produit;

                    SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;

                    if (SelectReceptNome.PrixVente == null)
                    {
                        SelectReceptNome.PrixVente = 0;
                    }
                    if (SelectReceptNome.PrixRevient == null)
                    {
                        SelectReceptNome.PrixRevient = 0;
                    }

                    SVC.Recept gridnom = new SVC.Recept
                    {
                        design = SelectReceptNome.design,
                        codeprod = SelectReceptNome.Id,
                        cf = SelectReceptFourn.Id,
                        Fournisseur = SelectReceptFourn.raison,
                        quantite = 0,
                        previent = SelectReceptNome.PrixRevient,
                        prixa = SelectReceptNome.PrixVente,
                        prixb = 0,
                        prixc = 0,
                        dates = DateTime.Now,
                        date = txtDateOper.SelectedDate,
                        nfact = txtnfact.Text,
                        collisage = 1,
                        lot = "",
                        tva = 0,
                        IdFamille = SelectReceptNome.IdFamille,
                        Famille = SelectReceptNome.famille,
                        cab = SelectReceptNome.clecab,
                        FicheProdfAvoir = 0,
                        CoutMoyen = parametre.coutmoyen,
                    };
                    var found = itemsSource.Find(item => item.codeprod == SelectReceptNome.Id && item.design == SelectReceptNome.design);
                    if (found == null)
                    {
                        itemsSource.Add(gridnom);
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit déja dans la facture d'achat", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    ReceptDatagrid.ItemsSource = itemsSource;
                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                }
                else
                {

                    if (ajout == 2 && NomenclatureProduit.SelectedItem != null && InformationComplet())
                    {


                        // / this.Title = title;
                        //this.WindowTitleBrush = brushajouterfacture;
                        SVC.Produit SelectReceptNome = NomenclatureProduit.SelectedItem as SVC.Produit;

                        SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                        if (SelectReceptNome.PrixVente == null)
                        {
                            SelectReceptNome.PrixVente = 0;
                        }
                        if (SelectReceptNome.PrixRevient == null)
                        {
                            SelectReceptNome.PrixRevient = 0;
                        }
                        SVC.Recept gridnom = new SVC.Recept
                        {
                            design = SelectReceptNome.design,
                            codeprod = SelectReceptNome.Id,
                            cf = SelectReceptFourn.Id,
                            Fournisseur = SelectReceptFourn.raison,
                            quantite = 0,
                            previent = SelectReceptNome.PrixRevient,
                            prixa = SelectReceptNome.PrixVente,
                            prixb = 0,
                            prixc = 0,
                            dates = DateTime.Now,
                            date = txtDateOper.SelectedDate,
                            nfact = txtnfact.Text,
                            collisage = 1,
                            lot = "",
                            tva = 0,
                            IdFamille = SelectReceptNome.IdFamille,
                            Famille = SelectReceptNome.famille,
                            FicheProdfAvoir = 0,
                            CoutMoyen = parametre.coutmoyen,
                            cab = SelectReceptNome.clecab,
                        };
                        var found = tte.Find(item => item.codeprod == SelectReceptNome.Id && item.design == SelectReceptNome.design);
                        if (found == null)
                        {
                            tte.Add(gridnom);
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit déja dans la facture d'achat", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        // ReceptDatagrid.ItemsSource = tte;
                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                    }
                    else
                    {
                        labelEtat.Content = "";
                        labelEtat.Foreground = Brushes.Red;
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez remplir tous les champs numéro de facture /fournisseur / date" , NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }

                }
            }
            catch (Exception EX)
            {
                labelEtat.Content = EX.Message.ToString();
                labelEtat.Foreground = Brushes.Red;
            }
        }

        private void txtnfact_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (ajout == 1)
                {
                    if (txtnfact.Text.Trim() != "")
                    {

                        var query = from c in proxy.GetAllRecouf()
                                    select new { c.nfact };

                        var results = query.ToList();
                        var disponible = results.Where(list1 => list1.nfact.Trim().ToUpper() == txtnfact.Text.Trim().ToUpper()).FirstOrDefault();

                        if (disponible != null)
                        {
                            labelEtat.Content = "Ce numéro de facture Existe déja";
                            labelEtat.Foreground = Brushes.Red;
                            labelEtat.BorderBrush = Brushes.Red;
                            btnCreer.IsEnabled = false;
                            btnCreer.Opacity = 0.2;


                        }
                        else
                        {
                            if (txtnfact.Text.Trim() != "")
                            {
                                labelEtat.Content = "";
                                btnCreer.IsEnabled = true;
                                btnCreer.Opacity = 1;
                                labelEtat.BorderBrush = Brushes.Red;

                            }
                        }
                    }
                    else
                    {

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;
                    }
                }
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }



        private void btncréerNomenclature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterProduit CLSession = new AjouterProduit(proxy, null, memberuser, callback);
                    CLSession.Show();

                }
                else
                {
                    labelEtat.Content = "Vous n'avez pas le droit de faire cette operation";
                    labelEtat.Foreground = Brushes.Red;

                }
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;


            }
        }
        bool InformationComplet()
        {
            try
            {
                if (ComboFournisseur.SelectedItem != null && txtDateOper.SelectedDate != null && txtnfact.Text != "" /*&& txteche.Text != ""*/ && (chNonFiscal.IsChecked == true || chFisclal.IsChecked == true))
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception EX)
            {
                labelEtat.Content = EX.Message.ToString();
                labelEtat.Foreground = Brushes.Red;
                return false;
            }
        }




        private void ReceptDatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {

                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Recept>;
                txtht.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => o.previent * o.quantite));
                txttva.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => ((o.previent * o.tva) / 100) * o.quantite));
                txtTTC.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                if (txtRemise.Text != "")
                {
                    if (tunisie != true)
                    {
                        //string.Format("{0:0.00}", ht)
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text) + Convert.ToDecimal(0.6));

                    }
                }
                else
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))+ Convert.ToDecimal(0.6));

                    }
                }

                // }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }




        private void ReceptDatagrid_CurrentCellChanged(object sender, EventArgs e)
        {

            try
            {

                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Recept>;
                txtht.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => o.previent * o.quantite));
                txttva.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => ((o.previent * o.tva) / 100) * o.quantite));
                txtTTC.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                if (txtRemise.Text != "")
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text)+ Convert.ToDecimal(0.6));

                    }
                }
                else
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))+ Convert.ToDecimal(0.6));

                    }
                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ReceptDatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            try
            {

                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Recept>;
                txtht.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => o.previent * o.quantite));
                txttva.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => ((o.previent * o.tva) / 100) * o.quantite));
                txtTTC.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                if (txtRemise.Text != "")
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text) +Convert.ToDecimal(0.6));

                    }
                }
                else
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))+ Convert.ToDecimal(0.6));

                    }
                }


                //                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txteche_KeyDown(object sender, KeyEventArgs e)
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



                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void txtRemise_KeyDown(object sender, KeyEventArgs e)
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



        private void txtRemise_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtRemise.Text != "")
                {
                    var value = Convert.ToDecimal(txtRemise.Text);

                    var valueTTC = Convert.ToDecimal(txtTTC.Text);
                    if (value != 0 && valueTTC != 0)
                    {
                        if (tunisie != true)
                        {
                            txtNet.Text = string.Format("{0:0.00}", valueTTC - value);
                        }
                        else
                        {
                            txtNet.Text = string.Format("{0:0.00}", valueTTC - value+ Convert.ToDecimal(0.6));

                        }
                    }
                }
                else
                {
                    if (txtRemise.Text == "")
                    {
                        var valueTTC = Convert.ToDecimal(txtTTC.Text);
                        if (valueTTC != 0)
                        {
                            if (tunisie != true)
                            {
                                txtNet.Text = string.Format("{0:0.00}", valueTTC);
                            }
                            else
                            {
                                txtNet.Text = string.Format("{0:0.00}", valueTTC + Convert.ToDecimal(0.6));
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

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterFournisseur cl = new AjouterFournisseur(proxy, null, memberuser);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }




        private void txtnfact_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtnfact.Text != "")
                {
                    txtnfact.BorderBrush = Brushes.Black;
                }
                else
                {
                    txtnfact.BorderBrush = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txteche_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txteche.Text != "")
                {
                    txteche.BorderBrush = Brushes.Black;
                }
                else
                {
                    txteche.BorderBrush = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void cbDci_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cbDci.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(r => r.FamilleProduit1);
                NomenclatureProduit.ItemsSource = proxy.GetAllProduit().OrderBy(n => n.design);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }



         

       
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ReceptDatagrid.SelectedItem != null && quantitebonus > 0)
                {
                    SVC.Recept selectedrecept = ReceptDatagrid.SelectedItem as SVC.Recept;
                    selectedrecept.previent = (selectedrecept.previent * selectedrecept.quantite) / (selectedrecept.quantite + quantitebonus);
                    selectedrecept.quantite = selectedrecept.quantite + quantitebonus;

                    /* SVC.Recept receptbonus = new SVC.Recept
                     {
                         cab= selectedrecept.cab,
                         cf = selectedrecept.cf,
                         Famille = selectedrecept.Famille,
                         date = selectedrecept.date,
                         dates = selectedrecept.dates,
                         design = selectedrecept.design,
                         codeprod = selectedrecept.codeprod,
                         collisage = selectedrecept.collisage,
                         CommandeId = selectedrecept.CommandeId,
                         CoutMoyen = selectedrecept.CoutMoyen,
                         FicheProdfAvoir = selectedrecept.FicheProdfAvoir,
                         Fournisseur = selectedrecept.Fournisseur,
                         IdFamille = selectedrecept.IdFamille,
                         lot = selectedrecept.lot,
                         nfact = selectedrecept.nfact,
                         perempt = selectedrecept.perempt,
                         previent = 0,
                         prixa = selectedrecept.prixa,
                         prixb = selectedrecept.prixb,
                         prixc = selectedrecept.prixc,
                         quantite =0,
                         tva = selectedrecept.tva,

                     };*/

                    // var itemsSource = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Recept>;
                    //  var itemsourcelist = itemsSource.ToList();
                    //  itemsourcelist.Add(receptbonus);
                    //  ReceptDatagrid.ItemsSource = itemsourcelist;
                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                    quantitebonus = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                quantitebonus = 0;
                if (t.Text != "")
                {
                    if (decimal.TryParse(t.Text, out quantitebonus))
                        quantitebonus = Convert.ToDecimal(t.Text);
                    else
                        quantitebonus = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void cbDci_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (cbDci.SelectedItem != null)

                {
                    SVC.FamilleProduit selectaliment = cbDci.SelectedItem as SVC.FamilleProduit;

                    string filter = selectaliment.FamilleProduit1;


                    ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);

                    cv.Filter = o =>
                    {
                        SVC.Produit p = o as SVC.Produit;
                        return (p.famille.ToUpper().Contains(filter.ToUpper()));
                    };

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRecherche_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Produit p = o as SVC.Produit;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                labelEtat.Content = ex.Message;
                labelEtat.Foreground = Brushes.Red;
            }
        }

        private void txtDateOper_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (txtDateOper.SelectedDate != null)
                {
                    txtDateOper.BorderBrush = Brushes.Black;
                }
                else
                {
                    txtDateOper.BorderBrush = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ComboFournisseur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboFournisseur.SelectedItem != null)
                {
                    ComboFournisseur.BorderBrush = Brushes.Black;
                }
                else
                {
                    ComboFournisseur.BorderBrush = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}