
using NewOptics.Administrateur;
using NewOptics.Caisse;
using MahApps.Metro.Controls;
using System;
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

using NewOptics.CreditCreance;

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for VersementPatient.xaml
    /// </summary>
    public partial class VersementClient : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.ClientV PatientR;
        private delegate void FaultedInvokerListeVersementDepeiment();
        List<SVC.Depeiment> depaiemlist;
        SVC.F1 VisiteAregler;
        string cléfiltre;
        SVC.Depeiment DepeimentMultiple;
        public VersementClient(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu, SVC.ClientV fournrecu,SVC.F1 visiterecu,bool valeurbool)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                PatientR = fournrecu;
                VisiteAregler = visiterecu;//Convert.ToString(VisiteApayer.Id) + Convert.ToString(DateTime.Now.TimeOfDay)
                                           // cléfiltre =  Convert.ToString(VisiteAregler.Id) + Convert.ToString(DateTime.Now.TimeOfDay) ;
              //  if (valeurbool==false)
              //  {
                    depaiemlist = (proxy.GetAllDepeiment()).Where(n => n.cle == VisiteAregler.cle).ToList();
               // }else
               // {
              //      SVC.Depeiment soldededepart= (proxy.GetAllDepeiment()).Find(n => n.cle == PatientR.cle);
               //     depaiemlist.Add(soldededepart);
               // }
                labelFournisseur.Content =fournrecu.Id+" "+ fournrecu.Raison;
               // var okmultiple = (proxy.GetAllDepeiementMultiple()).Find(n => n.cleVisite == VisiteAregler.cle);
                var okmultiple = (proxy.GetAllDepeiementMultiple()).FindAll(n => n.cleVisite == VisiteAregler.cle);

                if (okmultiple.Count>0)
                {
                    foreach (SVC.DepeiementMultiple itemmultiple in okmultiple)
                    {
                        DepeimentMultiple = proxy.GetAllDepeiment().Find(n =>n.CleMultiple==itemmultiple.cleMultiple);
                        if(!depaiemlist.Contains(DepeimentMultiple) && DepeimentMultiple.Multiple==true)
                        {
                            depaiemlist.Add(DepeimentMultiple);
                        }
                    }


                    FournDataGrid.ItemsSource = depaiemlist;

                }
                else
                {
                    FournDataGrid.ItemsSource = depaiemlist;

                }

                callbackrecu.InsertDepaiemCallbackevent += new ICallback.CallbackEventHandler32(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertDepeiment e)
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
        public void AddRefresh(SVC.Depeiment listMembershipOptic, int oper)
        {

            try
            {
                var LISTITEM1 = FournDataGrid.ItemsSource as IEnumerable<SVC.Depeiment>;
                List<SVC.Depeiment> LISTITEM = LISTITEM1.ToList();
                if (listMembershipOptic.cle == VisiteAregler.cle)
                {
                    if (oper == 1)
                    {
                        LISTITEM.Add(listMembershipOptic);
                    }
                    else
                    {
                        if (oper == 2)
                        {
                            var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                            objectmodifed = listMembershipOptic;
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

                        FournDataGrid.ItemsSource = LISTITEM;
                    }
                }
            }
                catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }


        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeVersementDepeiment(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeVersementDepeiment(HandleProxy));
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

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.SuppressionRecouv == true && FournDataGrid.SelectedItem != null)
                {
                    bool SupDepaif = false;
                    bool SupDepanse = false;
                    bool UpdateRecouf = false;

                    SVC.Depeiment selectdepaief = FournDataGrid.SelectedItem as SVC.Depeiment;

                    if (selectdepaief.Multiple == false)
                    {

                        var depense = (proxy.GetAllDepense()).Find(n => n.cle == selectdepaief.cle && n.Crédit == true && n.MontantCrédit == selectdepaief.montant);
                        var Recouf = (proxy.GetAllF1ByCle(selectdepaief.cle)).Find(n => n.cle == selectdepaief.cle);
                        bool existelentille = proxy.GetAllLentilleClientbyDossier(Recouf.cleDossier).Any();
                        bool existemonture = proxy.GetAllMonturebyDossier(Recouf.cleDossier).Any();
                        if (existelentille == false && existemonture == false)
                        {
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {


                                /*suprimez l'écriture de depaief*/
                                proxy.DeleteDepeiment(selectdepaief);
                                SupDepaif = true;
                                //   MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("paiement succées", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                /*  suprimez l'écriture de depanse*/
                                proxy.DeleteDepense(depense);
                                SupDepanse = true;
                                //  MessageBoxResult result1g1 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepense succées", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                /*  update recouf c-a-d enlevez le montant */
                                Recouf.reste = Recouf.reste + selectdepaief.montant;
                                Recouf.versement = Recouf.versement - selectdepaief.montant;
                                Recouf.soldé = false;

                                proxy.UpdateF1(Recouf);
                                UpdateRecouf = true;
                                //   MessageBoxResult result1g1d = Xceed.Wpf.Toolkit.MessageBox.Show("Visite yes", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                                {
                                    ts.Complete();
                                }


                            }
                            if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                            {
                                proxy.AjouterTransactionPaiementRefresh();
                                proxy.AjouterDepenseRefresh();
                                proxy.AjouterSoldeF1Refresh();
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                        }
                        else
                        {
                            if (existelentille == true && existemonture == false)
                            {
                                bool updatelentille = false;
                                SVC.LentilleClient selectedlentille = proxy.GetAllLentilleClientbyDossier(Recouf.cleDossier).Last();
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {


                                    /*suprimez l'écriture de depaief*/
                                    proxy.DeleteDepeiment(selectdepaief);
                                    SupDepaif = true;
                                    //   MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("paiement succées", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    /*  suprimez l'écriture de depanse*/
                                    proxy.DeleteDepense(depense);
                                    SupDepanse = true;
                                    //  MessageBoxResult result1g1 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepense succées", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    /*  update recouf c-a-d enlevez le montant */
                                    Recouf.reste = Recouf.reste + selectdepaief.montant;
                                    Recouf.versement = Recouf.versement - selectdepaief.montant;
                                    Recouf.soldé = false;

                                    proxy.UpdateF1(Recouf);
                                    UpdateRecouf = true;
                                    //   MessageBoxResult result1g1d = Xceed.Wpf.Toolkit.MessageBox.Show("Visite yes", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    selectedlentille.Encaissé = selectedlentille.Encaissé - selectdepaief.montant;
                                    selectedlentille.Reste = selectedlentille.Reste + selectdepaief.montant;
                                    proxy.UpdateLentilleClient(selectedlentille);
                                    updatelentille = true;
                                    if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true && updatelentille == true)
                                    {
                                        ts.Complete();
                                    }


                                }
                                if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                                {
                                    proxy.AjouterTransactionPaiementRefresh();
                                    proxy.AjouterDepenseRefresh();
                                    proxy.AjouterSoldeF1Refresh();
                                    proxy.AjouterLentilleClientRefresh(Convert.ToInt16(selectdepaief.CodeClient)) ;
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }
                            else

                            {
                                if (existelentille == false && existemonture == true)
                                {
                                    bool updatemonture = false;
                                    SVC.Monture selectedlentille = proxy.GetAllMonturebyDossier(Recouf.cleDossier).Last();
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {


                                        /*suprimez l'écriture de depaief*/
                                        proxy.DeleteDepeiment(selectdepaief);
                                        SupDepaif = true;
                                        //   MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("paiement succées", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        /*  suprimez l'écriture de depanse*/
                                        proxy.DeleteDepense(depense);
                                        SupDepanse = true;
                                        //  MessageBoxResult result1g1 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepense succées", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        /*  update recouf c-a-d enlevez le montant */
                                        Recouf.reste = Recouf.reste + selectdepaief.montant;
                                        Recouf.versement = Recouf.versement - selectdepaief.montant;
                                        Recouf.soldé = false;

                                        proxy.UpdateF1(Recouf);
                                        UpdateRecouf = true;
                                        //   MessageBoxResult result1g1d = Xceed.Wpf.Toolkit.MessageBox.Show("Visite yes", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        selectedlentille.Encaissé = selectedlentille.Encaissé - selectdepaief.montant;
                                        selectedlentille.Reste = selectedlentille.Reste + selectdepaief.montant;
                                        proxy.UpdateMonture(selectedlentille);
                                        updatemonture = true;
                                        if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true && updatemonture == true)
                                        {
                                            ts.Complete();
                                        }


                                    }
                                    if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                                    {
                                        proxy.AjouterTransactionPaiementRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        proxy.AjouterSoldeF1Refresh();
                                        proxy.AjouterMontureRefresh(Convert.ToInt16(selectdepaief.CodeClient));
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (selectdepaief.Multiple == true)
                        {

                            bool SuppDepeiement = false;
                            bool SupDepense = false;
                            bool SupDepeiementMultiple = false;
                            bool UpdateVisite = false;
                            bool updatelentille = false;
                            bool updatemonture = false;
                            int interfacesupp = 0;
                            List<SVC.F1> listvisite = new List<SVC.F1>();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Attention ! Vous allez supprimez un versement multiple qui va affecter d'autres enregistrements !veuillez confirmez votre choix", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                var depense = (proxy.GetAllDepense()).Find(n => n.cle == selectdepaief.cle && n.Crédit == true && n.MontantCrédit == selectdepaief.montant);
                                var depeiememultipl = (proxy.GetAllDepeiementMultiple()).Where(n => n.cleMultiple == selectdepaief.CleMultiple).ToList();

                                foreach (SVC.DepeiementMultiple itemmultiple in depeiememultipl)
                                {
                                    SVC.F1 selectedvisite = proxy.GetAllF1Bycode(itemmultiple.CodeClient.Value).Find(n => n.cle == itemmultiple.cleVisite);

                                    if (!listvisite.Contains(selectedvisite))
                                    {
                                        listvisite.Add(selectedvisite);
                                    }
                                }
                                List<SVC.Monture> listmonture = new List<SVC.Monture>();
                                List<SVC.LentilleClient> listlentille = new List<SVC.LentilleClient>();

                                foreach (var item in listvisite)
                                {
                                    var existemonture = proxy.GetAllMonturebyDossier(item.cleDossier).Any();
                                    if (existemonture == true)
                                    {
                                        var monture = proxy.GetAllMonturebyDossier(item.cleDossier).First();
                                        listmonture.Add(monture);
                                    }
                                    var existelentille = proxy.GetAllLentilleClientbyDossier(item.cleDossier).Any();
                                    if (existelentille == true)
                                    {
                                        var lentille = proxy.GetAllLentilleClientbyDossier(item.cleDossier).First();
                                        listlentille.Add(lentille);
                                    }
                                }

                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    proxy.DeleteDepeiment(selectdepaief);
                                    SuppDepeiement = true;
                                    proxy.DeleteDepense(depense);
                                    SupDepense = true;
                                    for (int i = depeiememultipl.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                    {
                                        var itemvisite = depeiememultipl.ElementAt(i);
                                        SVC.F1 updatevisiteselected = listvisite.Find(n => n.cle == itemvisite.cleVisite);

                                        /*  UpdateVisite = false;
                                          updatevisiteselected.reste = updatevisiteselected.reste + itemvisite.montant;
                                          updatevisiteselected.versement = updatevisiteselected.versement - itemvisite.montant;
                                          if (updatevisiteselected.reste != 0)
                                          {
                                              updatevisiteselected.soldé = false;
                                          }
                                          proxy.UpdateF1(updatevisiteselected);
                                          UpdateVisite = true;
                                          SupDepeiementMultiple = false;
                                          proxy.DeleteDepeiementMultiple(itemvisite);
                                          SupDepeiementMultiple = true;*/
                                        bool existelentille = listlentille.Any(n => n.Cle == updatevisiteselected.cleDossier);
                                        bool existemonture = listmonture.Any(n => n.Cle == updatevisiteselected.cleDossier);
                                        /****************************************/


                                        /*   proxy.DeleteDepeiment(selectdepaief);
                                              SuppDepeiement = true;
                                              proxy.DeleteDepense(depense);
                                              SupDepense = true;*/

                                        UpdateVisite = false;
                                        updatevisiteselected.reste = updatevisiteselected.reste + itemvisite.montant;
                                        updatevisiteselected.versement = updatevisiteselected.versement - itemvisite.montant;
                                        if (updatevisiteselected.reste != 0)
                                        {
                                            updatevisiteselected.soldé = false;
                                        }
                                        proxy.UpdateF1(updatevisiteselected);
                                        UpdateVisite = true;
                                        SupDepeiementMultiple = false;
                                        proxy.DeleteDepeiementMultiple(itemvisite);
                                        SupDepeiementMultiple = true;

                                        if (existelentille == true && existemonture == false)
                                        {

                                            updatelentille = false;
                                            SVC.LentilleClient selectedlentille = listlentille.Where(n => n.Cle == updatevisiteselected.cleDossier).Last();
                                            selectedlentille.Encaissé = selectedlentille.Encaissé - selectdepaief.montant;
                                            selectedlentille.Reste = selectedlentille.Reste + selectdepaief.montant;
                                            proxy.UpdateLentilleClient(selectedlentille);
                                            updatelentille = true;
                                            interfacesupp = 2;
                                        }
                                        else
                                        {
                                            if (existelentille == false && existemonture == true)
                                            {
                                                updatemonture = false;
                                                SVC.Monture selectedlentille = listmonture.Where(n => n.Cle == updatevisiteselected.cleDossier).Last();
                                                selectedlentille.Encaissé = selectedlentille.Encaissé - selectdepaief.montant;
                                                selectedlentille.Reste = selectedlentille.Reste + selectdepaief.montant;
                                                proxy.UpdateMonture(selectedlentille);
                                                updatemonture = true;
                                                interfacesupp = 1;
                                            }
                                        }



                                        /*    if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                                            {
                                                proxy.AjouterTransactionPaiementRefresh();
                                                proxy.AjouterDepenseRefresh();
                                                proxy.AjouterSoldeF1Refresh();
                                                proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                            }*/

                                        /*********************************/






                                    }
                                    if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true && interfacesupp == 0)
                                    {
                                        ts.Complete();

                                    }
                                    else
                                    {
                                        if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true && interfacesupp == 1 && updatemonture == true)
                                        {
                                            ts.Complete();
                                        }
                                        else
                                        {
                                            if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true && interfacesupp == 2 && updatelentille == true)
                                            {
                                                ts.Complete();
                                            }
                                            else
                                            {
                                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OpérationéchouéeTransaction, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                            }
                                        }

                                    }

                                }
                                if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true && interfacesupp == 0)
                                {

                                    proxy.AjouterTransactionPaiementRefresh();
                                    proxy.AjouterDepenseRefresh();
                                    //proxy.AjouterSoldeF1Refresh();
                                    foreach (var item in listvisite)
                                    {
                                        proxy.AjouterSoldeF1MultipleRefresh(item.nfact, Convert.ToInt16(item.codeclient));
                                    }
                                    MessageBoxResult resultcdsf1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true && interfacesupp == 1 && updatemonture == true)
                                    {
                                        proxy.AjouterMontureRefreshClient(Convert.ToInt16(selectdepaief.CodeClient));
                                        proxy.AjouterTransactionPaiementRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        //proxy.AjouterSoldeF1Refresh();
                                        foreach (var item in listvisite)
                                        {
                                            proxy.AjouterSoldeF1MultipleRefresh(item.nfact, Convert.ToInt16(item.codeclient));
                                        }
                                        MessageBoxResult resultcdsf1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true && interfacesupp == 2 && updatelentille == true)
                                        {
                                            proxy.AjouterLentilleClientRefreshList(Convert.ToInt16(selectdepaief.CodeClient));
                                            proxy.AjouterTransactionPaiementRefresh();
                                            proxy.AjouterDepenseRefresh();
                                            // proxy.AjouterSoldeF1Refresh();
                                            foreach (var item in listvisite)
                                            {
                                                proxy.AjouterSoldeF1MultipleRefresh(item.nfact, Convert.ToInt16(item.codeclient));
                                            }
                                            MessageBoxResult resultcdsf1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                        }

                                    }
                                }

                            }


                        }
                    }
                }



                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (FaultException ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.ImpressionRecouv == true && FournDataGrid.SelectedItem != null)
                {
                    SVC.Depeiment selectdepeiemt = FournDataGrid.SelectedItem as SVC.Depeiment;
                    List<SVC.F1> vs = new List<SVC.F1>();
                    vs.Add(VisiteAregler);
                    List<SVC.Depeiment> dp = new List<SVC.Depeiment>();
                    dp.Add(selectdepeiemt);
                    ImpressionRecu cl = new ImpressionRecu(proxy, dp, vs);
                    cl.Show();
                }
                else
                {
                    if (MemberUser.ImpressionRecouv == true && FournDataGrid.SelectedItem == null)
                    {
                        var LISTITEM11 = FournDataGrid.ItemsSource as IEnumerable<SVC.Depeiment>;
                        ImpressionVersementClient cl = new ImpressionVersementClient(proxy, LISTITEM11.ToList());
                        cl.Show();
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
                ICollectionView cv = CollectionViewSource.GetDefaultView(FournDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Depeiment p = o as SVC.Depeiment;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.paiem.ToUpper().Contains(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
