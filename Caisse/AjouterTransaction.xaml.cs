
using NewOptics.Administrateur;
using MahApps.Metro.Controls;
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

namespace NewOptics.Caisse
{
    /// <summary>
    /// Interaction logic for AjouterTransaction.xaml
    /// </summary>
    public partial class AjouterTransaction :Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerMotifDépense();
        SVC.Depense depenseP;
        int interfaceAppel;
        SVC.Recouf RecoufFourn;
        SVC.F1 VisiteApayer;
      
        List<SVC.F1> visiteversementmultiple;
        List<SVC.Recouf> visiteversementmultipleRecouf;
        public bool DepaiemtMultipleSucces , DepaiemSucces , UpdateVisitesucess  , InsertDepensesucces ;
        bool manuellecreation = false;
        int MANUELL = 0;
        public AjouterTransaction(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu,SVC.Depense depenserecu ,int interfaceRecu,SVC.Recouf recoufrecu,SVC.F1 visiterecu,List<SVC.F1> listrecuvisitte, List<SVC.Recouf> listrecuvisitteRecouf)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                MotifDepense.ItemsSource = proxy.GetAllMotifDepense();
                interfaceAppel = interfaceRecu;

                if (interfaceAppel == 1)
                {/* première interface écriture manuellle*/
                    /*************modification***************/
                    if (depenserecu != null)
                    {
                        manuellecreation = false;
                        MANUELL = 2;
                        depenseP = depenserecu;
                        if (depenseP.Débit == true)
                        {
                            txtRubriqueMontant.Visibility = Visibility.Visible;
                            txtRubriqueMontantD.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            if (depenseP.Crédit == true)
                            {
                                txtRubriqueMontantD.Visibility = Visibility.Visible;
                                txtRubriqueMontant.Visibility = Visibility.Collapsed;
                            }
                        }
                        ModeRéglement.SelectedIndex = 0;
                        GridTransaction.DataContext = depenseP;
                      //  MessageBoxResult resultcd1 = Xceed.Wpf.Toolkit.MessageBox.Show("Init comp i'm here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        /*****************creation********************/
                        manuellecreation = true;
                        MANUELL = 1;
                        depenseP = new SVC.Depense
                        {
                            DateDebit=DateTime.Now,
                            DateSaisie=DateTime.Now,
                          //  CompteDébité=""
                          Crédit=false,
                          Débit=true,
                          Auto=false,
                          Username=memberuser.Username,
                          ModePaiement= "Espèces",
                         
                        };
                        txtRubriqueMontant.Visibility = Visibility.Visible;
                        txtRubriqueMontantD.Visibility = Visibility.Collapsed;
                      
                        GridTransaction.DataContext = depenseP;
                     //   MessageBoxResult resultcd1 = Xceed.Wpf.Toolkit.MessageBox.Show("Init création", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                    }
                }
                else
                {
                    if (interfaceAppel == 2)
                    {/*deuxiéme interface paiement fournisseur*/
                        if (depenserecu == null)
                        {
                            RecoufFourn = recoufrecu;
                            depenseP = new SVC.Depense();
                            depenseP.DateSaisie = DateTime.Now;
                            depenseP.DateDebit = DateTime.Now;
                            depenseP.Débit = true;
                            depenseP.Crédit = false;
                            f.Content = "Facture :" + RecoufFourn.nfact + " " + recoufrecu.Fournisseur;

                            txtRubriqueMontant.Visibility = Visibility.Visible;
                            txtRubriqueMontantD.Visibility = Visibility.Collapsed;
                            txtDate.SelectedDate = DateTime.Now.Date;
                            chCredit.IsEnabled = false;
                            chDebit.IsChecked = true;
                            depenseP.RubriqueComptable = "Paiement Facture :" + RecoufFourn.nfact;
                            depenseP.Num_Facture = RecoufFourn.nfact;
                            depenseP.Montant = RecoufFourn.reste;
                            txtNumFacture.IsEnabled = false;
                          

                            GridTransaction.DataContext = depenseP;
                        }

                    }
                    else
                    {
                        if (interfaceAppel == 3)
                        {/*troisiéme interface réglement visite patient*/
                            if (visiterecu != null)
                            {
                             VisiteApayer = visiterecu;
                               depenseP = new SVC.Depense();
                               depenseP.DateSaisie = DateTime.Now;
                                depenseP.DateDebit = DateTime.Now;
                                depenseP.Débit = false;
                                depenseP.Crédit =true;
                                depenseP.ModePaiement = "Espèces";
                                f.Content = VisiteApayer.nfact + " " + VisiteApayer.raison;
                                txtNumFacture.IsEnabled = false;
                                txtRubriqueMontant.Visibility = Visibility.Collapsed;
                                txtRubriqueMontantD.Visibility = Visibility.Visible;
                                    txtDate.SelectedDate = DateTime.Now.Date;
                                    chCredit.IsEnabled = true;
                                    chDebit.IsChecked = false;
                                   chDebit.IsEnabled = false;
                                    depenseP.RubriqueComptable = "Paiement Facture :" + VisiteApayer.raison;
                                  depenseP.Num_Facture =Convert.ToString(VisiteApayer.Id);
                                   depenseP.MontantCrédit= VisiteApayer.reste;
                                    txtNumFacture.IsEnabled = false;
                                  
                                chImpressionRecuPatient.Visibility = Visibility.Visible;
                                    GridTransaction.DataContext = depenseP;
                            }
                        }else
                        {/************multiple versement visite****************/
                            if (interfaceAppel == 4)
                            {
                                if (visiterecu == null)
                                {
                                    visiteversementmultiple = listrecuvisitte;

                                    depenseP = new SVC.Depense();
                                    depenseP.DateSaisie = DateTime.Now;
                                    depenseP.DateDebit = DateTime.Now;
                                    depenseP.Débit = false;
                                    depenseP.Crédit = true;
                                    chCredit.IsEnabled = true;
                                    chDebit.IsChecked = false;
                                    chDebit.IsEnabled = false;
                                    /************************/
                                    f.Content = "Versement multiple" + " " + visiteversementmultiple.AsEnumerable().First().raison;
                                    /*****************************************/
                                    txtNumFacture.IsEnabled = false;
                                    txtRubriqueMontant.Visibility = Visibility.Collapsed;
                                    txtRubriqueMontantD.Visibility = Visibility.Visible;
                                    txtDate.SelectedDate = DateTime.Now.Date;
                                    /******************************************************/
                                    depenseP.RubriqueComptable = "Paiement Facture multiple :" + visiteversementmultiple.AsEnumerable().First().raison ;
                                    depenseP.Num_Facture = "Paiement Facture multiple ";
                                    /***********************************************************/
                                    depenseP.MontantCrédit = visiteversementmultiple.AsEnumerable().Sum(o => o.reste);

                                    txtNumFacture.IsEnabled = false;
                                   
                                    chImpressionRecuPatient.Visibility = Visibility.Visible;
                                    GridTransaction.DataContext = depenseP;

                                }
                            }else
                            {
                                if (interfaceAppel == 5)
                                {
                                    if (visiterecu == null)
                                    {
                                        visiteversementmultipleRecouf = listrecuvisitteRecouf;

                                        depenseP = new SVC.Depense();
                                        depenseP.DateSaisie = DateTime.Now;
                                        depenseP.DateDebit = DateTime.Now;
                                        depenseP.Débit = true;
                                        depenseP.Crédit = false;
                                        chCredit.IsEnabled = false;
                                        chDebit.IsChecked = true;
                                      
                                        /************************/
                                        f.Content = "Versement multiple" + " " + listrecuvisitteRecouf.AsEnumerable().First().Fournisseur;
                                        /*****************************************/
                                        txtNumFacture.IsEnabled = false;

                                        txtRubriqueMontant.Visibility = Visibility.Visible;
                                        txtRubriqueMontantD.Visibility = Visibility.Collapsed;
                                        txtDate.SelectedDate = DateTime.Now.Date;
                                        /******************************************************/
                                        depenseP.RubriqueComptable = "Paiement Facture multiple :" + listrecuvisitteRecouf.AsEnumerable().First().Fournisseur;
                                        depenseP.Num_Facture = "Paiement Facture multiple Fournisseur";
                                        /***********************************************************/
                                        depenseP.Montant = listrecuvisitteRecouf.AsEnumerable().Sum(o => o.reste);
                                        depenseP.MontantCrédit= listrecuvisitteRecouf.AsEnumerable().Sum(o => o.reste);

                                        txtNumFacture.IsEnabled = false;

                                        chImpressionRecuPatient.Visibility = Visibility.Collapsed;
                                        GridTransaction.DataContext = depenseP;
                                     //   MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(listrecuvisitteRecouf.AsEnumerable().Sum(o => o.reste).ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                    }
                                }

                            }
                        
                        }
                    }

                    callbackrecu.InsertMotifDepenseCallbackEvent += new ICallback.CallbackEventHandler19(callbackrecu_Refresh);
                 
                }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMotifDépense(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMotifDépense(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertMotifDepense e)
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(List<SVC.MotifDepense> listMembershipOptic)
        {
            try
            {

          
            MotifDepense.ItemsSource = listMembershipOptic;
            }catch (Exception ex)

            {
                this.Title = ex.Message;
            //    this.WindowTitleBrush = Brushes.Red;
            }
        }

        private void MotifDepense_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            { 
            if (MotifDepense.SelectedItem != null)
            {
                SVC.MotifDepense SelectMedecin = MotifDepense.SelectedItem as SVC.MotifDepense;
                txtRubriqueComptable.Text = SelectMedecin.MotifD.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
          
            BACK.Visibility = Visibility.Visible;
            bool depaiefResult = false;
            bool RecoufResult = false;
            bool depenseResult = false;
            try
            {
                    /*************************manuelle creation***********/
                if (manuellecreation=true && interfaceAppel == 1 && MANUELL==1)
                {
                    if ((txtRubriqueMontant.Text != "0" || txtRubriqueMontantD.Text != "0") && txtDate.SelectedDate != null && txtDateOper.SelectedDate != null && ModeRéglement.SelectedValue!= null && txtRubriqueComptable.Text != "" && (chDebit.IsChecked == true || chCredit.IsChecked == true))
                    {
                        using (var ts = new TransactionScope())
                        {

                                depenseP.DateDebit = txtDateOper.SelectedDate;
                            depenseP.DateSaisie = txtDate.SelectedDate;

                                depenseP.ModePaiement = ModeRéglement.SelectedValue.ToString();
                            //CompteDébité=
                            depenseP.RubriqueComptable = txtRubriqueComptable.Text.Trim();
                            // depenseP.Montant = 0;
                            //depenseP.MontantCrédit = 0;
                          //  depenseP.Commentaires = txtComentaire.Text.Trim();
                            //depenseP.Num_Facture = txtNumFacture.Text.Trim();
                            //depenseP.NumCheque = txtNumCheque.Text.Trim();
                            depenseP.Username = memberuser.Username.ToString();
                            depenseP.Auto = false;


                            if (radioCompteCaisse.IsChecked == true && radioCompteBanque.IsChecked == false)
                            {
                                depenseP.CompteDébité = "Caisse";
                            }
                            else
                            {
                                if (radioCompteBanque.IsChecked == true && radioCompteCaisse.IsChecked == false)
                                {
                                    depenseP.CompteDébité = "Banque";
                                }
                            }
                            if (chDebit.IsChecked == true && txtRubriqueMontant.Text != "0")
                            {
                                depenseP.Débit = true;
                                depenseP.Crédit = false;
                                depenseP.Montant = Convert.ToDecimal(txtRubriqueMontant.Text);
                                depenseP.MontantCrédit = 0;
                                //   MessageBoxResult resultc1k = Xceed.Wpf.Toolkit.MessageBox.Show("debit", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                if (chCredit.IsChecked == true && txtRubriqueMontantD.Text != "0")
                                {
                                    depenseP.Crédit = true;
                                    depenseP.Débit = false;
                                    depenseP.MontantCrédit = Convert.ToDecimal(txtRubriqueMontantD.Text);
                                    depenseP.Montant = 0;
                                    //      MessageBoxResult resultc1k = Xceed.Wpf.Toolkit.MessageBox.Show("credit", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }

                            proxy.InsertDepense(depenseP);
                            ts.Complete();

                            //    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        //    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("création touours", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            MessageBoxResult resultcG1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            //   this.WindowTitleBrush = Brushes.Green;


                        }
                        proxy.AjouterDepenseRefresh();
                    }

                }
                else
                {/****************modification ecriture manuelle**********/
                    if (/*manuellecreation=false*/interfaceAppel == 1 && MANUELL==2)
                    {
                        if ((txtRubriqueMontant.Text != "0" || txtRubriqueMontantD.Text != "0") && txtDate.SelectedDate != null && txtDateOper.SelectedDate != null && ModeRéglement.SelectedValue != null && txtRubriqueComptable.Text != "" )
                        {
                            using (var ts = new TransactionScope())
                            {
                                if (radioCompteCaisse.IsChecked == true)
                                {
                                    depenseP.CompteDébité = "Caisse";
                                }
                                else
                                {
                                    if (radioCompteBanque.IsChecked == true)
                                    {
                                        depenseP.CompteDébité = "Banque";
                                    }
                                }
                                if (chDebit.IsChecked == true && txtRubriqueMontant.Text != "0")
                                {
                                    depenseP.Débit = true;
                                    depenseP.Crédit = false;
                                    depenseP.Montant = Convert.ToDecimal(txtRubriqueMontant.Text);
                                    depenseP.MontantCrédit = 0;
                                }
                                else
                                {
                                    if (chCredit.IsChecked == true && txtRubriqueMontantD.Text != "0")
                                    {
                                        depenseP.Crédit = true;
                                        depenseP.Débit = false;
                                        depenseP.MontantCrédit = Convert.ToDecimal(txtRubriqueMontantD.Text);
                                        depenseP.Montant = 0;
                                    }
                                }

                                proxy.UpdateDepense(depenseP);
                                ts.Complete();
                            //    MessageBoxResult resultcd1 = Xceed.Wpf.Toolkit.MessageBox.Show("Modification manuelle", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            proxy.AjouterDepenseRefresh();
                        }
                    }
                    else
                    {
                        /******************paiement fournisseur**********************/
                        if (depenseP != null && txtDate.SelectedDate != null && txtDateOper.SelectedDate != null && ModeRéglement.SelectedValue != null && txtRubriqueComptable.Text != "" && txtRubriqueMontant.Text != "0" /*&&  txtComentaire.Text != "" && txtNumCheque.Text != "" */&& interfaceAppel == 2)
                        {
                            try
                            {
                                using (var ts = new TransactionScope())
                                {

                                    depenseP.DateDebit = txtDateOper.SelectedDate;
                                    depenseP.DateSaisie = txtDate.SelectedDate;
                                    depenseP.ModePaiement = ModeRéglement.SelectedValue.ToString();

                                    depenseP.RubriqueComptable = txtRubriqueComptable.Text.Trim();
                                 depenseP.Montant = Convert.ToDecimal(txtRubriqueMontant.Text);
                                      depenseP.MontantCrédit = 0;

                                 //   depenseP.Commentaires = txtComentaire.Text.Trim();
                                   // depenseP.Num_Facture = txtNumFacture.Text.Trim();
                                  //  depenseP.NumCheque = txtNumCheque.Text.Trim();
                                    depenseP.Username = memberuser.Username.ToString();
                                    depenseP.Auto = true;


                                    if (radioCompteCaisse.IsChecked == true && radioCompteBanque.IsChecked == false)
                                    {
                                        depenseP.CompteDébité = "Caisse";
                                    }
                                    else
                                    {
                                        if (radioCompteBanque.IsChecked == true && radioCompteCaisse.IsChecked == false)
                                        {
                                            depenseP.CompteDébité = "Banque";
                                        }
                                    }
                                    proxy.InsertDepenseAsync(depenseP);
                                    //   MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Insert Depense succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                    depenseResult = true;
                                        SVC.depaief dpf = new SVC.depaief
                                        {
                                            date = depenseP.DateDebit,
                                            montant = depenseP.Montant,
                                            paiem = depenseP.ModePaiement + " facture :" + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                            oper = depenseP.Username,
                                            dates = depenseP.DateSaisie,
                                            banque = depenseP.CompteDébité,
                                            nfact = depenseP.Num_Facture,
                                            amontant = RecoufFourn.net,
                                            //     cle = RecoufFourn.codef + RecoufFourn.nfact + RecoufFourn.date,
                                            cle = RecoufFourn.cle,
                                            cf = RecoufFourn.codef,
                                            Multiple = false,
                                            CodeFourn= RecoufFourn.codef,
                                            RaisonFourn= RecoufFourn.Fournisseur,
                                        };
                                    proxy.Insertdepaief(dpf);
                                    //  MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show("Insert Depeif succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                    depaiefResult = true;
                                    RecoufFourn.versement = RecoufFourn.versement + depenseP.Montant;
                                    RecoufFourn.reste = RecoufFourn.net - RecoufFourn.versement;
                                        if (RecoufFourn.reste == 0)
                                        {
                                            RecoufFourn.soldé = true;
                                        }
                                        else
                                        {
                                            RecoufFourn.soldé = false;
                                        }



                                        proxy.UpdateRecouf(RecoufFourn);
                                    // MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("Update Recouf succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                    RecoufResult = true; 
                                    if (depaiefResult && RecoufResult && depenseResult)
                                    {
                                        ts.Complete();
                                        //   ts.Dispose();
                                        btnCreer.IsEnabled = false;
                                    }
                                    else
                                    {

                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                    if (depaiefResult && RecoufResult && depenseResult)
                                    {
                                        proxy.AjouterTransactionACHATRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        proxy.AjouterRecoufRefresh();
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }




                                }
                            catch (FaultException ex)
                            {
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


                            }
                            catch (Exception ex)
                            {
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                            }

                        }
                        else
                        {
                                /**************réglement client**********************/
                            if (depenseP != null && txtDate.SelectedDate != null && txtDateOper.SelectedDate != null && ModeRéglement.SelectedValue != null && txtRubriqueComptable.Text != "" && txtRubriqueMontantD.Text != "0" /*&& txtComentaire.Text != "" && txtNumCheque.Text != "" */&& interfaceAppel == 3)
                            {
                                try
                                {
                                   // bool varcastraitement = false;
                                    //var castraitement = (proxy.GetAllTraitement()).Find(n => n.Id == VisiteApayer.IdCas);
                                    SVC.Depeiment dpf;
                                        bool lentilletrouve = false;
                                        bool monturetrouve = false;
                                        int interfacerefresh = 0;
                                        var existelentille = proxy.GetAllLentilleClientbyDossier(VisiteApayer.cleDossier).Any();
                                        var existemonture = proxy.GetAllMonturebyDossier(VisiteApayer.cleDossier).Any();
                                        using (var ts = new TransactionScope())
                                    {

                                        depenseP.DateDebit = txtDateOper.SelectedDate;
                                        depenseP.DateSaisie = txtDate.SelectedDate;
                                        depenseP.ModePaiement = ModeRéglement.SelectedValue.ToString();

                                        depenseP.RubriqueComptable = txtRubriqueComptable.Text.Trim();
                                        depenseP.Montant = 0;
                                        depenseP.MontantCrédit = Convert.ToDecimal(txtRubriqueMontantD.Text);

                                       // depenseP.Commentaires = txtComentaire.Text.Trim();
                                     //   depenseP.Num_Facture = txtNumFacture.Text.Trim();
                                   //     depenseP.NumCheque = txtNumCheque.Text.Trim();
                                        depenseP.Username = memberuser.Username.ToString();
                                        depenseP.Auto = true;
                                        depenseP.cle = VisiteApayer.cle;

                                        if (radioCompteCaisse.IsChecked == true && radioCompteBanque.IsChecked == false)
                                        {
                                            depenseP.CompteDébité = "Caisse";
                                        }
                                        else
                                        {
                                            if (radioCompteBanque.IsChecked == true && radioCompteCaisse.IsChecked == false)
                                            {
                                                depenseP.CompteDébité = "Banque";
                                            }
                                        }
                                        proxy.InsertDepense(depenseP);
                                        depenseResult = true;
                                        //    MessageBoxResult resultcx = Xceed.Wpf.Toolkit.MessageBox.Show("depenseResult = true", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);

                                        dpf = new SVC.Depeiment
                                        {
                                            date = depenseP.DateDebit,
                                            montant = Convert.ToDecimal(depenseP.MontantCrédit),
                                            paiem = depenseP.ModePaiement + " Facture :" + VisiteApayer.nfact + " " + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                            oper = depenseP.Username,
                                            dates = depenseP.DateSaisie,
                                            banque = depenseP.CompteDébité,
                                            nfact = depenseP.Num_Facture,
                                            amontant = Convert.ToDecimal(VisiteApayer.net),
                                            cle = depenseP.cle,
                                            cp = VisiteApayer.Id,
                                            Multiple=false,
                                           CodeClient=VisiteApayer.codeclient,
                                           RaisonClient=VisiteApayer.raison,
                                           
                                        };

                                        proxy.InsertDepeiment(dpf);
                                        depaiefResult = true;
                                        //    MessageBoxResult resuldtcx = Xceed.Wpf.Toolkit.MessageBox.Show("depaiefResult", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                        VisiteApayer.versement = Convert.ToDecimal(VisiteApayer.versement) + Convert.ToDecimal(depenseP.MontantCrédit);
                                        VisiteApayer.reste = Convert.ToDecimal(VisiteApayer.net) - Convert.ToDecimal(VisiteApayer.versement);
                                        if (VisiteApayer.reste == 0)
                                        {
                                            VisiteApayer.soldé = true;
                                        }
                                        else
                                        {
                                            VisiteApayer.soldé = false;
                                        }



                                        proxy.UpdateF1(VisiteApayer);
                                        RecoufResult = true;
                                            //    MessageBoxResult resuldttcx = Xceed.Wpf.Toolkit.MessageBox.Show("  RecoufResult = true;", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information); 

                                            /* if (VisiteApayer.ModeFacturation == 2)
                                             {
                                                 castraitement.Versement = castraitement.Versement + depenseP.MontantCrédit;
                                                 castraitement.Reste = castraitement.Reste - depenseP.MontantCrédit;
                                                 proxy.UpdateTypeTraitement(castraitement);
                                                 varcastraitement = true;
                                              //   MessageBoxResult resuldsttcx = Xceed.Wpf.Toolkit.MessageBox.Show(" varcastraitement = true;", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);

                                             }*/

                                          
                                            if (existelentille==true)
                                            {
                                                var lentille = proxy.GetAllLentilleClientbyDossier(VisiteApayer.cleDossier).First();

                                                lentille.Encaissé = lentille.Encaissé + dpf.montant;
                                                lentille.Reste = lentille.Reste - dpf.montant;
                                                proxy.UpdateLentilleClient(lentille);
                                                lentilletrouve = true;
                                                interfacerefresh = 2;
                                            }
                                            else
                                            {
                                                lentilletrouve = true;
                                            }
                                            if (existemonture == true)
                                            {
                                                var lentille = proxy.GetAllMonturebyDossier(VisiteApayer.cleDossier).First();

                                                lentille.Encaissé = lentille.Encaissé + dpf.montant;
                                                lentille.Reste = lentille.Reste - dpf.montant;
                                                proxy.UpdateMonture(lentille);
                                                monturetrouve = true;
                                                interfacerefresh = 1;
                                            }
                                            else
                                            {
                                                monturetrouve = true;
                                            }


                                                if (depaiefResult && RecoufResult && depenseResult && lentilletrouve && monturetrouve)//&& VisiteApayer.ModeFacturation!=2
                                        {
                                            ts.Complete();

                                        //    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information); btnCreer.IsEnabled = false;
                                        }
                                       else
                                        {
                                          /*  if (depaiefResult && RecoufResult && depenseResult && varcastraitement && VisiteApayer.ModeFacturation == 2)
                                            {
                                                ts.Complete();

                                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information); btnCreer.IsEnabled = false;

                                            }*/
                                           // else
                                          //  {
                                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                           // }
                                        }
                                    }
                                        if (depaiefResult && RecoufResult && depenseResult )//&& VisiteApayer.ModeFacturation!=2
                                        {
                                            proxy.AjouterTransactionPaiementRefresh();
                                            proxy.AjouterDepenseRefresh();
                                            proxy.AjouterSoldeF1Refresh();
                                            if (interfacerefresh==1)
                                            {
                                                proxy.AjouterMontureRefresh(Convert.ToInt16(VisiteApayer.codeclient));
                                            }
                                            else
                                            {
                                                if (interfacerefresh == 2)
                                                {
                                                    proxy.AjouterLentilleClientRefresh(Convert.ToInt16(VisiteApayer.codeclient));
                                                }
                                            }
                                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information); btnCreer.IsEnabled = false;
                                        }
                                  

                                    if (chImpressionRecuPatient.IsChecked == true)
                                    {
                                        List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                        listedepaiem.Add(dpf);
                                        List<SVC.F1> listevisite = new List<SVC.F1>();
                                        listevisite.Add(VisiteApayer);
                                        ImpressionRecu cl = new ImpressionRecu(proxy, listedepaiem, listevisite);
                                        cl.Show();
                                    }



                                }
                                catch (FaultException ex)
                                {
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                                catch (Exception ex)
                                {
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                if (depenseP != null && txtDate.SelectedDate != null && txtDateOper.SelectedDate != null && ModeRéglement.SelectedValue != null && txtRubriqueComptable.Text != "" && txtRubriqueMontantD.Text != "0"/* && txtComentaire.Text != "" && txtNumCheque.Text != "" */&& interfaceAppel == 4)
                                {

                                    if (Convert.ToDecimal(txtRubriqueMontantD.Text)<= visiteversementmultiple.AsEnumerable().Sum(o => o.reste))
                                    {
                                        SVC.Depeiment dpf;
                                            List<SVC.Monture> listmonture = new List<SVC.Monture>();
                                            List<SVC.LentilleClient> listlentille = new List<SVC.LentilleClient>();
                                            foreach (var item in visiteversementmultiple)
                                            {
                                                var existemonture = proxy.GetAllMonturebyDossier(item.cleDossier).Any();
                                                if (existemonture==true)
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
                                            using (var ts = new TransactionScope())
                                        {

                                            /******************insert depense*******************/

                                            depenseP.DateDebit = txtDateOper.SelectedDate;
                                            depenseP.DateSaisie = txtDate.SelectedDate;
                                            depenseP.ModePaiement = ModeRéglement.SelectedValue.ToString();

                                            depenseP.RubriqueComptable = txtRubriqueComptable.Text.Trim();
                                            depenseP.Montant = 0;
                                            depenseP.MontantCrédit = Convert.ToDecimal(txtRubriqueMontantD.Text);

                                          //  depenseP.Commentaires = txtComentaire.Text.Trim();
                                        //    depenseP.Num_Facture = txtNumFacture.Text.Trim();
                                            //depenseP.NumCheque = txtNumCheque.Text.Trim();
                                            depenseP.Username = memberuser.Username.ToString();
                                            depenseP.Auto = true;

                                            if (radioCompteCaisse.IsChecked == true && radioCompteBanque.IsChecked == false)
                                            {
                                                depenseP.CompteDébité = "Caisse";
                                            }
                                            else
                                            {
                                                if (radioCompteBanque.IsChecked == true && radioCompteCaisse.IsChecked == false)
                                                {
                                                    depenseP.CompteDébité = "Banque";
                                                }
                                            }
                                            /****************************depaiement*//////////////////////
                                            dpf = new SVC.Depeiment
                                            {
                                                date = depenseP.DateDebit,
                                                montant = Convert.ToDecimal(depenseP.MontantCrédit),
                                                paiem = depenseP.ModePaiement + " Facture Multiple" + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                                oper = depenseP.Username,
                                                dates = depenseP.DateSaisie,
                                                banque = depenseP.CompteDébité,
                                                nfact = depenseP.Num_Facture,
                                                amontant = Convert.ToDecimal(visiteversementmultiple.AsEnumerable().Sum(o => o.reste)),
                                                //      cle = depenseP.cle,
                                                cp = visiteversementmultiple.AsEnumerable().First().Id,
                                                Multiple = true,
                                                CleMultiple = visiteversementmultiple.Count() + visiteversementmultiple.AsEnumerable().Sum(o => o.reste).ToString() + DateTime.Now,
                                               
                                                CodeClient= visiteversementmultiple.First().codeclient,
                                                RaisonClient= visiteversementmultiple.First().raison,
                                              
                                                //    enreg=ite
                                            };


                                            /**************************************************************/

                                           Decimal montantAinserer = Convert.ToDecimal(txtRubriqueMontantD.Text);
                                                bool monturetrouve = false;
                                                bool lentilletrouve = false;
                                                int interfacerefresh = 0;
                                                //   foreach (SVC.Visite itemvisite in visiteversementmultiple)
                                                // {
                                                for (int i = visiteversementmultiple.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/

                                            {
                                                    //   MessageBoxResult resultcd1 = Xceed.Wpf.Toolkit.MessageBox.Show("1st boucle "+ montantAinserer, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                              
                                                    var itemvisite = visiteversementmultiple.ElementAt(i);
                                                if (montantAinserer >= itemvisite.reste && montantAinserer != 0)
                                                    {
                                                       // MessageBoxResult resultcf1 = Xceed.Wpf.Toolkit.MessageBox.Show("montant 1 "+montantAinserer.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                        //MessageBoxResult resultcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show(itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                        /******************Insert MultipleDepaiement*************************/
                                                        DepaiemtMultipleSucces = false;
                                                  
                                                        SVC.DepeiementMultiple DepeiementMultipleObject = new SVC.DepeiementMultiple
                                                    {
                                                        date = depenseP.DateDebit,
                                                        montant = Convert.ToDecimal(itemvisite.reste),
                                                        paiem = depenseP.ModePaiement + " Facture :" + itemvisite.nfact+ " " + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                                        oper = depenseP.Username,
                                                        dates = depenseP.DateSaisie,
                                                        banque = depenseP.CompteDébité,
                                                        nfact = "Versement multiple",
                                                        amontant = Convert.ToDecimal(itemvisite.net),
                                                        cleVisite = itemvisite.cle,
                                                        cp = itemvisite.Id,
                                                        cleMultiple = dpf.CleMultiple,
                                                    
                                                        CodeClient=itemvisite.codeclient,
                                                        RaisonClient=itemvisite.raison,
                                                        
                                                    };
                                                    proxy.InsertDepeiementMultiple(DepeiementMultipleObject);
                                                    DepaiemtMultipleSucces = true;
                                                       // MessageBoxResult resuldstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("DepeiementMultiple "+itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                                        /********************Update Visite***************************/
                                                        UpdateVisitesucess = false;
                                                          itemvisite.versement = Convert.ToDecimal(itemvisite.versement) + Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                          itemvisite.reste = Convert.ToDecimal(itemvisite.net) - Convert.ToDecimal(itemvisite.versement);
                                                          if (itemvisite.reste == 0)
                                                          {
                                                              itemvisite.soldé = true;
                                                          }
                                                          else
                                                          {
                                                              itemvisite.soldé = false;
                                                          }
                                                          proxy.UpdateF1(itemvisite);
                                                          UpdateVisitesucess = true;
                                                     //   MessageBoxResult resuldsfsdfdstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("itemvisite " + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);



                                                        monturetrouve = false;
                                                              lentilletrouve = false;
                                                     //   MessageBoxResult resuldsfsdfddsfdsfstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("avant lentille " + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                        bool existelentille = false;
                                                        existelentille =listlentille.Any(n => n.Cle == itemvisite.cleDossier);//proxy.GetAllLentilleClientbyDossier(itemvisite.cleDossier).Any();
                                                        // MessageBoxResult resuldsqdqssfsdfdstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("existelentille" + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                    //    MessageBoxResult resuldsfsdfddsfdsfstcddf1 = Xceed.Wpf.Toolkit.MessageBox.Show("avant monture " + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                        bool existemonture = false;
                                                        //  existemonture = proxy.GetAllMonturebyDossier(itemvisite.cleDossier).Any();
                                                        existemonture = listmonture.Any(n => n.Cle == itemvisite.cleDossier); // proxy.GetAllMonture().Any(n=>n.Cle.Trim()== itemvisite.cleDossier.Trim());

                                                //        MessageBoxResult resuldsfsdfddsdfdsfstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("itemvisite " + existemonture.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                        if (existelentille == true)
                                                              {
                                                               //   MessageBoxResult resultcd31 = Xceed.Wpf.Toolkit.MessageBox.Show(existelentille.ToString()+" ani hna1", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                                  var lentille = listlentille.Where(n => n.Cle == itemvisite.cleDossier).First();

                                                                  lentille.Encaissé = lentille.Encaissé + DepeiementMultipleObject.montant;
                                                                  lentille.Reste = lentille.Reste - DepeiementMultipleObject.montant;
                                                                  proxy.UpdateLentilleClient(lentille);
                                                                  lentilletrouve = true;
                                                                  interfacerefresh = 2;
                                                                //  MessageBoxResult resultdcf1 = Xceed.Wpf.Toolkit.MessageBox.Show("lentille 1", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                              }
                                                              else
                                                              {
                                                                  lentilletrouve = true;
                                                              }
                                                              if (existemonture == true)
                                                              {
                                                                  var lentille = listmonture.Where(n => n.Cle == itemvisite.cleDossier).First();

                                                                  lentille.Encaissé = lentille.Encaissé + DepeiementMultipleObject.montant;
                                                                  lentille.Reste = lentille.Reste - DepeiementMultipleObject.montant;
                                                                  proxy.UpdateMonture(lentille);
                                                                  monturetrouve = true;
                                                                  interfacerefresh = 1;
                                                                //  MessageBoxResult resultcsf1 = Xceed.Wpf.Toolkit.MessageBox.Show("monture 1", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                              }
                                                              else
                                                              {
                                                                  monturetrouve = true;  
                                                              }

                                                              /*****************mettre à jours le montant*////////////////////////
                                                        montantAinserer = montantAinserer - Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                    depenseP.cle = depenseP.cle + itemvisite.Id + "/";
                                                      //  MessageBoxResult ressultcf1 = Xceed.Wpf.Toolkit.MessageBox.Show("montant 1 fin " + montantAinserer.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                    }
                                                else
                                                {
                                                    if (montantAinserer < itemvisite.reste && montantAinserer != 0)
                                                    {
                                                        DepaiemtMultipleSucces = false;
                                                          
                                                            SVC.DepeiementMultiple DepeiementMultipleObject = new SVC.DepeiementMultiple
                                                        {
                                                            date = depenseP.DateDebit,
                                                            montant = Convert.ToDecimal(montantAinserer),
                                                            paiem = depenseP.ModePaiement + " Facture :" + itemvisite.nfact + " " + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                                            oper = depenseP.Username,
                                                            dates = depenseP.DateSaisie,
                                                            banque = depenseP.CompteDébité,
                                                            nfact = "Versement multiple",
                                                            amontant = Convert.ToDecimal(itemvisite.net),
                                                            cleVisite = itemvisite.cle,
                                                            cp = itemvisite.Id,
                                                            cleMultiple = dpf.CleMultiple,
                                                          
                                                            CodeClient=itemvisite.codeclient,
                                                            RaisonClient=itemvisite.raison,
                                                            
                                                        };
                                                        proxy.InsertDepeiementMultiple(DepeiementMultipleObject);
                                                        DepaiemtMultipleSucces = true;
                                                  
                                                        UpdateVisitesucess = false;
                                                        itemvisite.versement = Convert.ToDecimal(itemvisite.versement) + Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                        itemvisite.reste = Convert.ToDecimal(itemvisite.net) - Convert.ToDecimal(itemvisite.versement);
                                                        if (itemvisite.reste == 0)
                                                        {
                                                            itemvisite.soldé = true;
                                                        }
                                                        else
                                                        {
                                                            itemvisite.soldé = false;
                                                        }



                                                        proxy.UpdateF1(itemvisite);
                                                        UpdateVisitesucess = true;
                                                 
                                                     

                                                            monturetrouve = false;
                                                            lentilletrouve = false;
                                                            monturetrouve = false;
                                                            lentilletrouve = false;
                                                           // MessageBoxResult resuldsfsdfddsfdsfstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("avant lentille " + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                            bool existelentille = false;
                                                            existelentille = listlentille.Any(n => n.Cle == itemvisite.cleDossier);//proxy.GetAllLentilleClientbyDossier(itemvisite.cleDossier).Any();
                                                                                                                                   // MessageBoxResult resuldsqdqssfsdfdstcdf1 = Xceed.Wpf.Toolkit.MessageBox.Show("existelentille" + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                         //   MessageBoxResult resuldsfsdfddsfdsfstcddf1 = Xceed.Wpf.Toolkit.MessageBox.Show("avant monture " + itemvisite.nfact.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                            bool existemonture = false;
                                                            //  existemonture = proxy.GetAllMonturebyDossier(itemvisite.cleDossier).Any();
                                                            existemonture = listmonture.Any(n => n.Cle == itemvisite.cleDossier); // proxy.GetAllMonture().Any(n=>n.Cle.Trim()== itemvisite.cleDossier.Trim());
                                                            if (existelentille == true)
                                                            {
                        //                                        MessageBoxResult resultcd31 = Xceed.Wpf.Toolkit.MessageBox.Show(existelentille.ToString() + " ani hna2", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                                var lentille = listlentille.Where(n => n.Cle == itemvisite.cleDossier).First();

                                                                lentille.Encaissé = lentille.Encaissé + DepeiementMultipleObject.montant;
                                                                lentille.Reste = lentille.Reste - DepeiementMultipleObject.montant;
                                                                proxy.UpdateLentilleClient(lentille);
                                                                lentilletrouve = true;
                                                                interfacerefresh = 2;
                                                            //    MessageBoxResult resultcf1 = Xceed.Wpf.Toolkit.MessageBox.Show("lentille 2", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                            }
                                                            else
                                                            {
                                                                lentilletrouve = true;
                                                            }
                                                            if (existemonture == true)
                                                            {
                                                                var lentille = listmonture.Where(n => n.Cle == itemvisite.cleDossier).First();

                                                                lentille.Encaissé = lentille.Encaissé + DepeiementMultipleObject.montant;
                                                                lentille.Reste = lentille.Reste - DepeiementMultipleObject.montant;
                                                                proxy.UpdateMonture(lentille);
                                                                monturetrouve = true;
                                                                interfacerefresh = 1;
                                                              //  MessageBoxResult resultcf1 = Xceed.Wpf.Toolkit.MessageBox.Show("monture 2", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                            }
                                                            else
                                                            {
                                                                monturetrouve = true;
                                                            }
                                                            montantAinserer = montantAinserer - Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                            depenseP.cle = depenseP.cle + itemvisite.Id + "/";
                                                        }
                                                        else
                                                    {
                                                     MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("NOn pour les deux conditions", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                    }

                                                }
                                            }
                                            /***********************insert depense*******************************/


                                            proxy.InsertDepense(depenseP);
                                            InsertDepensesucces = true;
                                      //     MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Depense ok", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            /*****************************insert depaiement global*****************************/



                                            dpf.cle = depenseP.cle;
                                            proxy.InsertDepeiment(dpf);
                                            DepaiemSucces = true;
                                       //  MessageBoxResult resultc1d = Xceed.Wpf.Toolkit.MessageBox.Show("Depaiement ok", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                            if (DepaiemtMultipleSucces==true && DepaiemSucces==true && UpdateVisitesucess==true && InsertDepensesucces == true && monturetrouve==true && lentilletrouve==true)
                                                {
                                                ts.Complete();
                                             //   MessageBoxResult resultc1s = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées , NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            }
                                            else
                                            {
                                                MessageBoxResult resultc14 = Xceed.Wpf.Toolkit.MessageBox.Show(InsertDepensesucces.ToString()+UpdateVisitesucess.ToString()+DepaiemSucces.ToString()+ DepaiemtMultipleSucces.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                            }
                                        }
                                            if (DepaiemtMultipleSucces == true && DepaiemSucces == true && UpdateVisitesucess == true && InsertDepensesucces == true)
                                            {
                                                proxy.AjouterTransactionPaiementRefresh();
                                                proxy.AjouterDepenseRefresh();
                                                proxy.AjouterSoldeF1Refresh();
                                                 MessageBoxResult resultc1s = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                              //  MessageBoxResult resultc1s = Xceed.Wpf.Toolkit.MessageBox.Show("Refresh", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            }

                                            if (chImpressionRecuPatient.IsChecked == true)
                                        {
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            listedepaiem.Add(dpf);
                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            listevisite.Add(VisiteApayer);
                                            ImpressionRecu cl = new ImpressionRecu(proxy, listedepaiem, listevisite);
                                            cl.Show();
                                        }
                                    } else
                                    {
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée+"  "+"Le montant saisie doit étre inferieur ou égale au total des restes", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                    }
                                    }
                                    else
                                    {
                                        if (depenseP != null && txtDate.SelectedDate != null && txtDateOper.SelectedDate != null && ModeRéglement.SelectedValue != null && txtRubriqueComptable.Text != "" && txtRubriqueMontantD.Text != "0"/* && txtComentaire.Text != "" && txtNumCheque.Text != "" */&& interfaceAppel == 5)
                                        {

                                            if (Convert.ToDecimal(txtRubriqueMontant.Text) <= visiteversementmultipleRecouf.AsEnumerable().Sum(o => o.reste))
                                            {
                                                SVC.depaief dpf;

                                                using (var ts = new TransactionScope())
                                                {

                                                    /******************insert depense*******************/

                                                    depenseP.DateDebit = txtDateOper.SelectedDate;
                                                    depenseP.DateSaisie = txtDate.SelectedDate;
                                                    depenseP.ModePaiement = ModeRéglement.SelectedValue.ToString();

                                                    depenseP.RubriqueComptable = txtRubriqueComptable.Text.Trim();
                                                    depenseP.Montant = Convert.ToDecimal(txtRubriqueMontant.Text);
                                                    depenseP.MontantCrédit = 0;

                                                    //  depenseP.Commentaires = txtComentaire.Text.Trim();
                                                    //    depenseP.Num_Facture = txtNumFacture.Text.Trim();
                                                    //depenseP.NumCheque = txtNumCheque.Text.Trim();
                                                    depenseP.Username = memberuser.Username.ToString();
                                                    depenseP.Auto = true;

                                                    if (radioCompteCaisse.IsChecked == true && radioCompteBanque.IsChecked == false)
                                                    {
                                                        depenseP.CompteDébité = "Caisse";
                                                    }
                                                    else
                                                    {
                                                        if (radioCompteBanque.IsChecked == true && radioCompteCaisse.IsChecked == false)
                                                        {
                                                            depenseP.CompteDébité = "Banque";
                                                        }
                                                    }
                                                    /****************************depaiement*//////////////////////
                                                    dpf = new SVC.depaief
                                                    {
                                                        date = depenseP.DateDebit,
                                                        montant = Convert.ToDecimal(depenseP.Montant),
                                                        paiem = depenseP.ModePaiement + " Facture Multiple" + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                                        oper = depenseP.Username,
                                                        dates = depenseP.DateSaisie,
                                                        banque = depenseP.CompteDébité,
                                                        nfact = depenseP.Num_Facture,
                                                        amontant = Convert.ToDecimal(visiteversementmultipleRecouf.AsEnumerable().Sum(o => o.reste)),
                                                        //      cle = depenseP.cle,
                                                        cf = visiteversementmultipleRecouf.AsEnumerable().First().codef,
                                                        Multiple = true,
                                                        CleMultiple = visiteversementmultipleRecouf.Count() + visiteversementmultipleRecouf.AsEnumerable().Sum(o => o.reste).ToString() + DateTime.Now,

                                                        CodeFourn = visiteversementmultipleRecouf.First().codef,
                                                        RaisonFourn = visiteversementmultipleRecouf.First().Fournisseur,

                                                        //    enreg=ite
                                                    };


                                                    /**************************************************************/

                                                    Decimal montantAinserer = Convert.ToDecimal(txtRubriqueMontant.Text);

                                                    //   foreach (SVC.Visite itemvisite in visiteversementmultiple)
                                                    // {
                                                    for (int i = visiteversementmultipleRecouf.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/

                                                    {
                                                        //   MessageBoxResult resultcd1 = Xceed.Wpf.Toolkit.MessageBox.Show("1st boucle "+ montantAinserer, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                        var itemvisite = visiteversementmultipleRecouf.ElementAt(i);
                                                        if (montantAinserer >= itemvisite.reste && montantAinserer != 0)
                                                        {
                                                            /******************Insert MultipleDepaiement*************************/
                                                            DepaiemtMultipleSucces = false;
                                                            SVC.DepeiementMultipleFournisseur DepeiementMultipleObject = new SVC.DepeiementMultipleFournisseur
                                                            {
                                                                date = depenseP.DateDebit,
                                                                montant = Convert.ToDecimal(itemvisite.reste),
                                                                paiem = depenseP.ModePaiement + " Facture :" + itemvisite.nfact + " " + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                                                oper = depenseP.Username,
                                                                dates = depenseP.DateSaisie,
                                                                banque = depenseP.CompteDébité,
                                                                nfact = "Versement multiple",
                                                                amontant = Convert.ToDecimal(itemvisite.net),
                                                                cleVisite = itemvisite.cle,
                                                                cp = itemvisite.Id,
                                                                cleMultiple = dpf.CleMultiple,

                                                                CodeFourn =itemvisite.codef,
                                                                RaisonFourn = itemvisite.Fournisseur,

                                                            };
                                                            proxy.InsertDepeiementMultipleFournisseur(DepeiementMultipleObject);
                                                            DepaiemtMultipleSucces = true;


                                                            /********************Update Visite***************************/
                                                            UpdateVisitesucess = false;
                                                            itemvisite.versement = Convert.ToDecimal(itemvisite.versement) + Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                            itemvisite.reste = Convert.ToDecimal(itemvisite.net) - Convert.ToDecimal(itemvisite.versement);
                                                            if (itemvisite.reste == 0)
                                                            {
                                                                itemvisite.soldé = true;
                                                            }
                                                            else
                                                            {
                                                                itemvisite.soldé = false;
                                                            }



                                                            proxy.UpdateRecouf(itemvisite);
                                                            UpdateVisitesucess = true;

                                                            /*****************mettre à jours le montant*////////////////////////
                                                            montantAinserer = montantAinserer - Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                            depenseP.cle = depenseP.cle + itemvisite.Id + "/";
                                                        }
                                                        else
                                                        {
                                                            if (montantAinserer < itemvisite.reste && montantAinserer != 0)
                                                            {
                                                                DepaiemtMultipleSucces = false;
                                                                SVC.DepeiementMultipleFournisseur DepeiementMultipleObject = new SVC.DepeiementMultipleFournisseur
                                                                {
                                                                    date = depenseP.DateDebit,
                                                                    montant = Convert.ToDecimal(montantAinserer),
                                                                    paiem = depenseP.ModePaiement + " Facture :" + itemvisite.nfact + " " + depenseP.Num_Facture + " date :" + depenseP.DateDebit + "Libellé :" + depenseP.NumCheque,
                                                                    oper = depenseP.Username,
                                                                    dates = depenseP.DateSaisie,
                                                                    banque = depenseP.CompteDébité,
                                                                    nfact = "Versement multiple",
                                                                    amontant = Convert.ToDecimal(itemvisite.net),
                                                                    cleVisite = itemvisite.cle,
                                                                    cp = itemvisite.Id,
                                                                    cleMultiple = dpf.CleMultiple,

                                                                    CodeFourn = itemvisite.codef,
                                                                    RaisonFourn= itemvisite.Fournisseur,

                                                                };
                                                                proxy.InsertDepeiementMultipleFournisseur(DepeiementMultipleObject);
                                                                DepaiemtMultipleSucces = true;
                                                                //    MessageBoxResult resultcd31 = Xceed.Wpf.Toolkit.MessageBox.Show("depeiement multi^ple ok", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);



                                                                /********************Update Visite***************************/
                                                                UpdateVisitesucess = false;
                                                                itemvisite.versement = Convert.ToDecimal(itemvisite.versement) + Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                                itemvisite.reste = Convert.ToDecimal(itemvisite.net) - Convert.ToDecimal(itemvisite.versement);
                                                                if (itemvisite.reste == 0)
                                                                {
                                                                    itemvisite.soldé = true;
                                                                }
                                                                else
                                                                {
                                                                    itemvisite.soldé = false;
                                                                }



                                                                proxy.UpdateRecouf(itemvisite);
                                                                UpdateVisitesucess = true;
                                                                //     MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Update visite ok", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                                /*****************mettre à jours le montant*////////////////////////

                                                                montantAinserer = montantAinserer - Convert.ToDecimal(DepeiementMultipleObject.montant);
                                                                depenseP.cle = depenseP.cle + itemvisite.Id + "/";
                                                                
                                                            }
                                                            else
                                                            {
                                                                //  MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("NOn pour les deux conditions", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                            }

                                                        }
                                                    }
                                                    /***********************insert depense*******************************/


                                                    proxy.InsertDepense(depenseP);
                                                    InsertDepensesucces = true;
                                                    //   MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Depense ok", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                    /*****************************insert depaiement global*****************************/



                                                    dpf.cle = depenseP.cle;
                                                    proxy.Insertdepaief(dpf);
                                                    DepaiemSucces = true;
                                                    //   MessageBoxResult resultc1d = Xceed.Wpf.Toolkit.MessageBox.Show("Depaiement ok", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                                    if (DepaiemtMultipleSucces == true && DepaiemSucces == true && UpdateVisitesucess == true && InsertDepensesucces == true)
                                                    {
                                                        ts.Complete();
                                                        //   MessageBoxResult resultc1s = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées , NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                    }
                                                    else
                                                    {
                                                        MessageBoxResult resultc14 = Xceed.Wpf.Toolkit.MessageBox.Show(InsertDepensesucces.ToString() + UpdateVisitesucess.ToString() + DepaiemSucces.ToString() + DepaiemtMultipleSucces.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                    }
                                                }
                                                if (DepaiemtMultipleSucces == true && DepaiemSucces == true && UpdateVisitesucess == true && InsertDepensesucces == true)
                                                {
                                                    proxy.AjouterTransactionPaiementRefresh();
                                                    proxy.AjouterDepenseRefresh();
                                                    proxy.AjouterRecoufRefresh();
                                                    MessageBoxResult resultc1s = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                }

                                              /*  if (chImpressionRecuPatient.IsChecked == true)
                                                {
                                                    List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                                    listedepaiem.Add(dpf);
                                                    List<SVC.F1> listevisite = new List<SVC.F1>();
                                                    listevisite.Add(VisiteApayer);
                                                    ImpressionRecu cl = new ImpressionRecu(proxy, listedepaiem, listevisite);
                                                    cl.Show();
                                                }*/
                                            }
                                            else
                                            {
                                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée + "  " + "Le montant saisie doit étre inferieur ou égale au total des restes", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                            }
                                        }
                                        else
                                        {
                                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                               
                               
                                   
                               
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
               // var result = MessageBox.Show(ex.InnerException.Message);
            }finally
            {
              
                BACK.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chDebit_Checked(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chDebit.IsChecked == true)
            {
                txtRubriqueMontantD.Visibility = Visibility.Collapsed;
                txtRubriqueMontant.Visibility = Visibility.Visible;
                txtRubriqueMontant.Text = txtRubriqueMontantD.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chCredit_Checked(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (chCredit.IsChecked == true)
            {
                txtRubriqueMontantD.Visibility = Visibility.Visible;
                txtRubriqueMontant.Visibility = Visibility.Collapsed;
                txtRubriqueMontantD.Text = txtRubriqueMontant.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void txtRubriqueMontantD_KeyDown(object sender, KeyEventArgs e)
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

        private void BTNCLOSTE_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
