using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
using GestionClinique.Caisse;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for VersementPatient.xaml
    /// </summary>
    public partial class VersementPatient : DXWindow
    {
        SVC.Membership MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.Patient PatientR;
        private delegate void FaultedInvokerListeVersementDepeiment();
        List<SVC.Depeiment> depaiemlist;
        SVC.Visite VisiteAregler;
        string cléfiltre;
        SVC.Depeiment DepeimentMultiple;
        public VersementPatient(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership membershiprecu, ICallback callbackrecu, SVC.Patient fournrecu,SVC.Visite visiterecu,bool valeurbool)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = membershiprecu;
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
                labelFournisseur.Content =fournrecu.Id+" "+ fournrecu.Nom+ " "+fournrecu.Prénom;
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.Depeiment listmembership, int oper)
        {

            try
            {
                var LISTITEM1 = FournDataGrid.ItemsSource as IEnumerable<SVC.Depeiment>;
                List<SVC.Depeiment> LISTITEM = LISTITEM1.ToList();
                if (listmembership.cle == VisiteAregler.cle)
                {
                    if (oper == 1)
                    {
                        LISTITEM.Add(listmembership);
                    }
                    else
                    {
                        if (oper == 2)
                        {
                            var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                            objectmodifed = listmembership;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                                LISTITEM.Remove(deleterendez);
                            }
                        }

                        FournDataGrid.ItemsSource = LISTITEM;
                    }
                }
            }
                catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
            if (MemberUser.SuppréssionCaisse == true && FournDataGrid.SelectedItem != null)
            {
              
                try
                {
                    SVC.Depeiment selectdepaief = FournDataGrid.SelectedItem as SVC.Depeiment;

                    if (selectdepaief.Multiple == false)
                    {
                        bool SupDepaif = false;
                        bool SupDepanse = false;
                        bool UpdateRecouf = false;

                        var depense = (proxy.GetAllDepense()).Find(n => n.cle == selectdepaief.cle && n.Crédit == true && n.MontantCrédit == selectdepaief.montant);
                        var Recouf = (proxy.GetAllVisiteByVisite(selectdepaief.CodePatient.Value)).Find(n => n.cle == selectdepaief.cle);

                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {


                            /*suprimez l'écriture de depaief*/
                            proxy.DeleteDepeiment(selectdepaief);
                            SupDepaif = true;
                            // MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("paiement succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                            /*  suprimez l'écriture de depanse*/
                            proxy.DeleteDepense(depense);
                            SupDepanse = true;
                            // MessageBoxResult result1g1 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepense succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                            /*  update recouf c-a-d enlevez le montant */
                            Recouf.Reste = Recouf.Reste + selectdepaief.montant;
                            Recouf.Versement = Recouf.Versement - selectdepaief.montant;
                            if (Recouf.Reste!=0)
                            {
                                Recouf.Soldé = false;
                            }
                            proxy.UpdateVisite(Recouf);
                            UpdateRecouf = true;
                            //   MessageBoxResult result1g1d = Xceed.Wpf.Toolkit.MessageBox.Show("Visite yes", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);

                            if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                            {
                                ts.Complete();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }


                        }
                        proxy.AjouterDepenseRefresh();
                        proxy.AjouterSoldeVisiteRefresh();
                        proxy.AjouterTransactionPaiementRefresh();
                    }
                    else
                    {
                        if (selectdepaief.Multiple == true)
                        {
                            bool SuppDepeiement = false;
                            bool SupDepense = false;
                            bool SupDepeiementMultiple = false;
                            bool UpdateVisite = false;
                            List<SVC.Visite> listvisite=new List<SVC.Visite>();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Attention ! Vous allez supprimez un versement multiple qui va affecter d'autres enregistrements !veuillez confirmez votre choix",GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                var depense = (proxy.GetAllDepense()).Find(n => n.cle == selectdepaief.cle && n.Crédit == true && n.MontantCrédit == selectdepaief.montant);
                                var depeiememultipl = (proxy.GetAllDepeiementMultiple()).Where(n => n.cleMultiple == selectdepaief.CleMultiple).ToList();
                                foreach (SVC.DepeiementMultiple itemmultiple in depeiememultipl)
                                {
                                    SVC.Visite selectedvisite=proxy.GetAllVisiteByVisite(itemmultiple.CodePatient.Value).Find(n => n.cle == itemmultiple.cleVisite);

                                    if (!listvisite.Contains(selectedvisite) )
                                    {
                                        listvisite.Add(selectedvisite);
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
                                        SVC.Visite updatevisiteselected = listvisite.Find(n => n.cle == itemvisite.cleVisite);

                                        UpdateVisite = false;
                                        updatevisiteselected.Reste = updatevisiteselected.Reste + itemvisite.montant;
                                        updatevisiteselected.Versement = updatevisiteselected.Versement - itemvisite.montant;
                                        if (updatevisiteselected.Reste != 0)
                                        {
                                            updatevisiteselected.Soldé = false;
                                        }
                                        proxy.UpdateVisite(updatevisiteselected);
                                        UpdateVisite = true;
                                        SupDepeiementMultiple = false;
                                        proxy.DeleteDepeiementMultiple(itemvisite);
                                        SupDepeiementMultiple = true;
                                    }
                                   if (SuppDepeiement==true && SupDepense==true && UpdateVisite==true && SupDepeiementMultiple==true)
                                    {
                                        ts.Complete();
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                    else
                                    {
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OpérationéchouéeTransaction, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                    }

                                }
                                proxy.AjouterDepenseRefresh();
                                proxy.AjouterSoldeVisiteRefresh();
                                proxy.AjouterTransactionPaiementRefresh();

                            }
                        }
                    }
                }
                catch (FaultException ex)
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
                catch (Exception ex)
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (MemberUser.ImressionCaisse==true && FournDataGrid.SelectedItem!=null)
            {
                SVC.Depeiment selectdepeiemt = FournDataGrid.SelectedItem as SVC.Depeiment;
                List<SVC.Visite> vs = new List<SVC.Visite>();
                vs.Add(VisiteAregler);
                List<SVC.Depeiment> dp= new List<SVC.Depeiment>();
                dp.Add(selectdepeiemt);
                ImpressionRecu cl = new ImpressionRecu(proxy, dp, vs);
                cl.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez sélectionner un réglement", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
