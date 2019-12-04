
using NewOptics.Administrateur;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NewOptics.Achat
{
    /// <summary>
    /// Interaction logic for AvoirFournisseur.xaml
    /// </summary>
    public partial class AvoirFournisseur : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        List<SVC.Recept> tte;
        ICallback callback;
        bool creation = false;
        int ajout;
        List<SVC.Recept> itemsSource;
        SVC.Recouf depenseP;
        private delegate void FaultedInvokerAVOIRACHAT();
        bool ReceptResult = false;
        bool RecoufResult = false;
        bool ProdfResult = false;
        SVC.Param parametre;
        bool tunisie = false;
        public AvoirFournisseur(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu,ICallback callbackrecu, SVC.Recouf depenserecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                memberuser = memberrecu;
                cbDci.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);
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
                    creation = false;
                    ReceptDatagrid.Visibility = Visibility.Visible;

                    ajout = 2;
                    depenseP = depenserecu;
                    GridRecouf.DataContext = depenseP;
                    tte = proxy.GetAllReceptBYNFACT(depenseP.nfact.Trim()).ToList();
                    ReceptDatagrid.ItemsSource = tte;
                    ReceptDatagrid.DataContext = tte;
                    List<SVC.Fourn> testmedecin1 = proxy.GetAllFourn();
                    ComboFournisseur.ItemsSource = testmedecin1;
                    List<SVC.Fourn> tte1 = testmedecin1.Where(n => n.raison == depenseP.Fournisseur && n.Id == depenseP.codef).ToList();
                    ComboFournisseur.SelectedItem = tte1.First();
                    ComboFournisseur.IsEnabled = false;
                    txtnfact.IsEnabled = false;
                    txtTTC.Text = string.Format("{0:0.00}", (tte).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                    NomenclatureProduit.ItemsSource = proxy.GetAllProdf().Where(n => n.cf == tte1.First().Id).OrderBy(n => n.design);
                    btnCreer.IsEnabled = true;
                    txtnfact.IsEnabled = false;
                }
               else
                {
                    NomenclatureProduit.IsEnabled = false;
                    creation = true;
                    btnCreer.IsEnabled = false;
                    ajout = 1;
                    depenseP = new SVC.Recouf
                    {
                        username = memberuser.Username,
                        dates = DateTime.Now,
                        avoir = true,
                        fiscal=false,
                        Nonfiscal=true,
                       date = DateTime.Now,
                    };
                    GridRecouf.DataContext = depenseP;
                    ComboFournisseur.ItemsSource = proxy.GetAllFourn();
                    ComboFournisseur.SelectedIndex = -1;
                    itemsSource = new List<SVC.Recept>();
                    ReceptDatagrid.DataContext = itemsSource;
                    ReceptDatagrid.CanUserDeleteRows = true;
                    txtRemise.Text = "";
                    txtTTC.Text = "";
                    txttva.Text = "";
                    txtht.Text = "";
                } 

     
                callbackrecu.InsertReceptCallbackEvent += new ICallback.CallbackEventHandler24(callbackrecurecept_Refresh);
                callbackrecu.InsertFournCallbackEvent += new ICallback.CallbackEventHandler20(callbackrecufourn_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
        public void AddRefreshRecept(List<SVC.Recept> listMembershipOptic)
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
                        if (listMembershipOptic.Count() > 0)
                        {
                            tte = listMembershipOptic.Where(n => n.nfact.Trim() == txtnfact.Text.Trim()).ToList();


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
        public void AddRefreshFourn(List<SVC.Fourn> listMembershipOptic)
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
                        //   if (listMembershipOptic.Count() > 0)
                        //   {
                        ComboFournisseur.ItemsSource = listMembershipOptic;
                        ComboFournisseur.DataContext = listMembershipOptic;
                        //   }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAVOIRACHAT(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAVOIRACHAT(HandleProxy));
                return;
            }
            HandleProxy();
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

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ajout == 1 && InformationComplet() && memberuser.CreationAchat==true)
                {
                    try
                    {
                        var itemsSourceDatagrid = ReceptDatagrid.ItemsSource as IEnumerable;
                        SVC.Fourn selectedfournisseur = ComboFournisseur.SelectedItem as SVC.Fourn;
                        var itemsSourceProdf = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            
                            foreach (SVC.Recept item in itemsSourceDatagrid)
                            {
                                var foundexiste = (itemsSourceProdf).Any(itemf => itemf.Id == item.FicheProdfAvoir);
                                if (foundexiste == true)
                                {
                                    var found = (itemsSourceProdf).First(itemf => itemf.Id == item.FicheProdfAvoir);

                                    if (item.quantite > 0 && item.previent > 0)/*Si la quantite et le prix de revient*/
                                    {

                                        var pa = (itemsSourceProdf).First(itemf => itemf.Id == item.FicheProdfAvoir);

                                        //   var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);
                                        var Value = (found.quantite) - (item.quantite * item.collisage);

                                        ReceptResult = false;

                                        /*****************nOUVELLE QUANTITE INF A NOUVELLE AJOUTER LE PRODUIT****************/
                                        if (Value >= 0)
                                        {

                                            item.cf = selectedfournisseur.Id;

                                            item.Fournisseur = selectedfournisseur.raison;


                                          
                                            ProdfResult = false;
                                            if (pa.quantite >= Value)
                                            {
                                                proxy.InsertRecept(item);
                                                ReceptResult = true;
                                             //   pa.cp = item.codeprod;
                                               // pa.design = item.design;
                                                pa.quantite = (Value);



                                                proxy.UpdateProdf(pa);
                                                ProdfResult = true;
                                            }
                                            else
                                            {
                                                if (pa.quantite < Value)
                                                {
                                                    MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("La quantité  " + pa.quantite + " seulement disponible en stock pour " + pa.design + " Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                    ProdfResult = false;
                                                    ReceptResult = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ProdfResult = false;
                                            ReceptResult = false;
                                            MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("La quantité  " + pa.quantite + " seulement disponible en stock pour " + pa.design + " Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                            break;


                                        }

                                    }

                                }
                                else
                                {
                                    MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit inexistant  Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }




                            }




                            RecoufResult = false;
                            depenseP.codef = selectedfournisseur.Id;
                            depenseP.Fournisseur = selectedfournisseur.raison;

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
                                depenseP.echeance = Convert.ToInt16(txteche.Text);
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
                            depenseP.versement = 0;
                            depenseP.reste =  Convert.ToDecimal(txtNet.Text);
                           // depenseP.reste = (-1) * Convert.ToDecimal(txtNet.Text);

                            proxy.InsertRecouf(depenseP);
                            RecoufResult = true;



                            if (ReceptResult && RecoufResult && ProdfResult )
                            {
                                ts.Complete();


                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                btnCreer.IsEnabled = false;

                            }
                            else
                            {

                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        if (ReceptResult==true && RecoufResult==true && ProdfResult==true )
                       {


                                proxy.AjouterFactureAchatSansProdfRefresh(depenseP.nfact.Trim());
                                proxy.AjouterProdfReceptRefresh(depenseP.Fournisseur, depenseP.nfact, depenseP.codef);
                                ReceptDatagrid.CanUserDeleteRows = false;
                        }
                        else
                        {
                            itemsSource = new List<SVC.Recept>();
                            ReceptDatagrid.ItemsSource = itemsSource;
                            ReceptDatagrid.DataContext = itemsSource;
                            NomenclatureProduit.ItemsSource = proxy.GetAllProdf().Where(n => n.cf == selectedfournisseur.Id).OrderBy(n => n.design);
                        }


                     
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
                else
                {
                    if (ajout == 2 && InformationComplet() && memberuser.ModificationAchat==true)
                    {
                    
                        try
                        {
                            var itemsSourceDatagrid = ReceptDatagrid.ItemsSource as IEnumerable;
                            SVC.Fourn selectedfournisseur = ComboFournisseur.SelectedItem as SVC.Fourn;
                            List<SVC.Recept> itemsSourcerecept = (proxy.GetAllRecept()).Where(n => n.Fournisseur == depenseP.Fournisseur && n.nfact == depenseP.nfact && n.cf == depenseP.codef).ToList();

                            var itemsSourceProdf = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
                            using (var ts = new TransactionScope())
                            {


                                

                               // fdssdf List<SVC.Prodf> itemsSourceProdf = (proxy.GetAllProdf()).Where(n => n.fourn == depenseP.Fournisseur && n.nfact == depenseP.nfact && n.cf == depenseP.codef).ToList();
                                foreach (SVC.Recept item in itemsSourceDatagrid)
                                {

                                    var found = (itemsSourcerecept).Find(itemf => itemf.Id == item.Id);

                                    if (found == null)
                                    {
                                        var foundexiste = (itemsSourceProdf).Any(itemf => itemf.Id == item.FicheProdfAvoir);
                                        if (foundexiste == true)
                                        {
                                            var pa = (itemsSourceProdf).First(itemf => itemf.Id == item.FicheProdfAvoir);

                                            if (item.quantite > 0 && item.previent > 0)/*Si la quantite et le prix de revient*/
                                            {

                                             

                                                //   var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);
                                                var Value = (pa.quantite) - (item.quantite * item.collisage);

                                              

                                                /*****************nOUVELLE QUANTITE INF A NOUVELLE AJOUTER LE PRODUIT****************/
                                                if (Value >= 0)
                                                {
                                                   
                                                    



                                                    ProdfResult = false;
                                                    ReceptResult = false;
                                                    if (pa.quantite >= Value)
                                                    {
                                                        proxy.InsertRecept(item);
                                                        ReceptResult = true;
                                                        //   pa.cp = item.codeprod;
                                                        // pa.design = item.design;
                                                        pa.quantite = (Value);



                                                        proxy.UpdateProdf(pa);
                                                        ProdfResult = true;
                                                    }
                                                    else
                                                    {
                                                        if (pa.quantite < Value)
                                                        {
                                                            MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("La quantité  " + pa.quantite + " seulement disponible en stock pour " + pa.design + " Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                            ProdfResult = false;
                                                            ReceptResult = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ProdfResult = false;
                                                    ReceptResult = false;
                                                    
                                                    MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("La quantité  " + pa.quantite + " seulement disponible en stock pour " + pa.design + " Modification Imposible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                    break;

                                                }

                                            }

                                        }
                                    }
                                    else
                                    {/*Le cas ou le produit existe déja*/
                                        if (found != null)
                                        {
                                            var pa = (itemsSourceProdf).First(n => n.Id == item.FicheProdfAvoir);

                                            if (item.quantite > 0 && item.previent > 0)/*Si la quantite et le prix de revient*/
                                            {


                                                //   var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);
                                                var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);

                                              

                                                /*****************nOUVELLE QUANTITE INF A NOUVELLE AJOUTER LE PRODUIT****************/
                                                if (Value > 0)
                                                {

                                                    ReceptResult = false;
                                                    proxy.UpdateRecept(item);
                                                    //  MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("UpdateRecept positive avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    ReceptResult = true;
                                                    ProdfResult = false;
                                                    pa.quantite = (pa.quantite) + (Value);
                                                    proxy.UpdateProdf(pa);
                                                        //    MessageBoxResult result111 = Xceed.Wpf.Toolkit.MessageBox.Show("UpdateProdf positive avec succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        ProdfResult = true;
                                                   
                                                }
                                                else
                                                {
                                                    
                                                 
                                                    if (Value < 0)
                                                    {
                                                        if (pa.quantite>=-Value)
                                                        {
                                                            ReceptResult = false;
                                                            proxy.UpdateRecept(item);
                                                            ReceptResult = true;

                                                            ProdfResult = false;

                                                            pa.quantite = (pa.quantite)+(Value);
                                                            
                                                            proxy.UpdateProdf(pa);

                                                            ProdfResult = true;
                                                        }
                                                        else
                                                        {
                                                            ReceptResult = false;
                                                            ProdfResult = false;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Value == 0)
                                                        {
                                                           // ReceptResult =false;

                                                           // proxy.UpdateRecept(item);
                                                            ReceptResult = true;
                                                           // ProdfResult = false;
                                                           // proxy.UpdateProdf(pa);
                                                            ProdfResult = true;
                                                        }

                                                    }

                                                }

                                            }///fin de quantite et prix achat supp 0 c-a-d modification autorisée
                                            else
                                            {
                                                if (item.quantite == 0)/*Supprimer Un Produit dans la facture*/
                                                {

                                                  //  var pa = (itemsSourceProdf).First(n => n.Id==item.FicheProdfAvoir);

                                                    var Value = (found.quantite * found.collisage) - (item.quantite * item.collisage);
                                                   
                                                       
                                                           
                                                                ProdfResult = false;
                                                                ReceptResult = false;
                                                                proxy.DeleteRecept(item);
                                                                ReceptResult = true;

                                                                pa.quantite = (pa.quantite) + (Value);

                                                               
                                                                proxy.UpdateProdf(pa);

                                                                ProdfResult = true;
                                                           
                                                       
                                                   
                                                }
                                                else
                                                {
                                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                                    ReceptResult = false;
                                                    ProdfResult = true;
                                                    break;
                                                }
                                            }//fin de cas nouvelle quantite=0 dans recept

                                        }
                                    }//existe fin

                                }///fin de parcour forech in new itemsource

                                // SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                                RecoufResult = false;
                              
                                depenseP.avoir = true;
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
                                    depenseP.echeance = Convert.ToInt16(txteche.Text);
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
                             //   depenseP.reste = Convert.ToDecimal(txtNet.Text) - depenseP.versement;

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


                                if (ReceptResult && ProdfResult && RecoufResult )
                                {
                                    ts.Complete();
                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }





                            }
                            if (ReceptResult && ProdfResult && RecoufResult )
                            {
                                proxy.AjouterFactureAchatSansProdfRefresh(depenseP.nfact.Trim());

                                proxy.AjouterProdfReceptRefresh(depenseP.Fournisseur, depenseP.nfact, depenseP.codef);
                            }
                            else
                            {
                                proxy.AjouterFactureAchatSansProdfRefresh(depenseP.nfact.Trim());
                                NomenclatureProduit.ItemsSource = proxy.GetAllProdf().Where(n => n.cf == selectedfournisseur.Id).OrderBy(n => n.design);

                                proxy.AjouterProdfReceptRefresh(depenseP.Fournisseur, depenseP.nfact, depenseP.codef);
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

            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

       

        private void cbDci_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ComboFournisseur.SelectedItem!=null)
                {
                    SVC.Fourn selectedfourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                    NomenclatureProduit.ItemsSource = proxy.GetAllProdf().Where(n => n.cf == selectedfourn.Id).OrderBy(n => n.design);
                    cbDci.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(r => r.FamilleProduit1);
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
               
                    SVC.Prodf SelectReceptNome = NomenclatureProduit.SelectedItem as SVC.Prodf;

                    SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;

                    SVC.Recept gridnom = new SVC.Recept
                    {
                        design = SelectReceptNome.design,
                        codeprod = SelectReceptNome.cp,
                        FicheProdfAvoir=SelectReceptNome.Id,
                        cf = SelectReceptFourn.Id,
                        Fournisseur = SelectReceptFourn.raison,
                        quantite = 0,
                        previent = SelectReceptNome.previent,
                        prixa = SelectReceptNome.prixa,
                        prixb = SelectReceptNome.prixb,
                        prixc = SelectReceptNome.prixc,
                        dates = DateTime.Now,
                        date = txtDateOper.SelectedDate,
                        nfact = txtnfact.Text,
                        collisage = 1,
                        lot = SelectReceptNome.lot,
                        perempt = SelectReceptNome.perempt,
                        tva = SelectReceptNome.tva,
                        IdFamille = SelectReceptNome.IdFamille,
                        cab = SelectReceptNome.cab,
                       
                    };
                    var found = itemsSource.Find(item => item.FicheProdfAvoir == SelectReceptNome.Id && item.design == SelectReceptNome.design);
                    if (found == null)
                    {
                      
                        itemsSource.Add(gridnom);
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit déja dans la facture d'avoir", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
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
                        SVC.Prodf SelectReceptNome = NomenclatureProduit.SelectedItem as SVC.Prodf;

                        SVC.Fourn SelectReceptFourn = ComboFournisseur.SelectedItem as SVC.Fourn;

                        SVC.Recept gridnom = new SVC.Recept
                        {
                            design = SelectReceptNome.design,
                            codeprod = SelectReceptNome.cp,
                            FicheProdfAvoir = SelectReceptNome.Id,
                            cf = SelectReceptFourn.Id,
                            Fournisseur = SelectReceptFourn.raison,
                            quantite = 0,
                            previent = SelectReceptNome.previent,
                            prixa=SelectReceptNome.prixa,
                            prixb= SelectReceptNome.prixb,
                            prixc= SelectReceptNome.prixc,
                            cab= SelectReceptNome.cab,
                            perempt = SelectReceptNome.perempt,
                            dates = DateTime.Now,
                            date = txtDateOper.SelectedDate,
                            nfact = txtnfact.Text,
                            collisage = 1,
                            lot = SelectReceptNome.lot,
                            tva = SelectReceptNome.tva,
                            IdFamille = SelectReceptNome.IdFamille,
                        };
                        var found = tte.Find(item => item.FicheProdfAvoir== SelectReceptNome.Id && item.design == SelectReceptNome.design);
                        if (found == null)
                        {
                            tte.Add(gridnom);
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit déja dans la facture d'avoir", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        // ReceptDatagrid.ItemsSource = tte;
                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                    }
                    else
                    {
                        labelEtat.Content = "Vous devez remplir tous les champs";
                        labelEtat.Foreground = Brushes.Red;

                    }

                }
            }
            catch (Exception EX)
            {
                labelEtat.Content = EX.Message.ToString();
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
                            txtNet.Text = string.Format("{0:0.00}", valueTTC + value);
                        }
                        else
                        {
                            txtNet.Text = string.Format("{0:0.00}", valueTTC + value-Convert.ToDecimal(0.6));

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
                                txtNet.Text = string.Format("{0:0.00}", valueTTC - Convert.ToDecimal(0.6));

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
                            NomenclatureProduit.IsEnabled = false;

                        }
                        else
                        {
                            if (txtnfact.Text.Trim() != "")
                            {
                                labelEtat.Content = "";
                                btnCreer.IsEnabled = true;
                                btnCreer.Opacity = 1;
                                labelEtat.BorderBrush = Brushes.Red;
                                NomenclatureProduit.IsEnabled = true;

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

         

        private void ReceptDatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {

                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Recept>;
                txtht.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => o.previent * o.quantite));
                txttva.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => ((o.previent * o.tva) / 100) * o.quantite));
                txtTTC.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                if (txtRemise.Text != "")
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1 * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))) + Convert.ToDecimal(txtRemise.Text));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1 * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))) + Convert.ToDecimal(txtRemise.Text)- Convert.ToDecimal(0.6));

                    }
                }
                else
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1) * ((test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1) * ((test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)))- Convert.ToDecimal(0.6));

                    }
                }


                //                }
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
                    txtht.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => o.previent * o.quantite));
                    txttva.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => ((o.previent * o.tva) / 100) * o.quantite));
                    txtTTC.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                    if (txtRemise.Text != "")
                    {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1 * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))) + Convert.ToDecimal(txtRemise.Text));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1 * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))) + Convert.ToDecimal(txtRemise.Text)- Convert.ToDecimal(0.6));

                    }
                }
                    else
                    {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1) * ((test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1) * ((test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)))- Convert.ToDecimal(0.6));

                    }
                }


                    //                }
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
                txtht.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => o.previent * o.quantite));
                txttva.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => ((o.previent * o.tva) / 100) * o.quantite));
                txtTTC.Text = string.Format("{0:0.00}", (-1) * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)));
                if (txtRemise.Text != "")
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1 * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))) + Convert.ToDecimal(txtRemise.Text));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1 * (test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))) + Convert.ToDecimal(txtRemise.Text)- Convert.ToDecimal(0.6));

                    }
                }
                else
                {
                    if (tunisie != true)
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1) * ((test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite))));
                    }
                    else
                    {
                        txtNet.Text = string.Format("{0:0.00}", (-1) * ((test).AsEnumerable().Sum(o => (((o.previent * o.tva) / 100) * o.quantite) + (o.previent * o.quantite)))- Convert.ToDecimal(0.6));

                    }
                }


                //                }
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
                    // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(selectaliment.FamilleProduit1, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    int filter = selectaliment.Id;


                    ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);

                    cv.Filter = o =>
                    {
                        SVC.Prodf p = o as SVC.Prodf;
                        return (p.IdFamille.ToString().ToUpper().Contains(filter.ToString().ToUpper()));
                    };

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
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Prodf p = o as SVC.Prodf;
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
                    txtDateOper.BorderBrush = Brushes.White;
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

        private void ComboFournisseur_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (ComboFournisseur.SelectedIndex > -1 && ComboFournisseur.SelectedItem != null && creation == true)
                {
                    SVC.Fourn selectedfourn = ComboFournisseur.SelectedItem as SVC.Fourn;
                    NomenclatureProduit.ItemsSource = proxy.GetAllProdf().Where(n => n.cf == selectedfourn.Id).OrderBy(n => n.design);
                    ReceptDatagrid.Visibility = Visibility.Visible;

                }
                else
                {

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
