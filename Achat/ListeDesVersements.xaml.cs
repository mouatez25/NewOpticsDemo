
using NewOptics.Administrateur;
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


namespace NewOptics.Achat
{
    /// <summary>
    /// Interaction logic for ListeDesVersements.xaml
    /// </summary>
    public partial class ListeDesVersements : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.Fourn Fournisseur;
        private delegate void FaultedInvokerListeVersement();
        List<SVC.depaief> depaiemlist;
        SVC.depaief DepeimentMultipleFournisseur;
        SVC.Recouf recoufregler;
        
        public ListeDesVersements(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu,SVC.Fourn fournrecu,SVC.Recouf recoufrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                Fournisseur = fournrecu;
                recoufregler = recoufrecu;
                depaiemlist = (proxy.GetAlldepaief().Where(n => n.cle== recoufregler.cle).ToList());
                labelFournisseur.Content = fournrecu.raison;
                var okmultiple = (proxy.GetAllDepeiementMultipleFournisseur()).FindAll(n => n.cleVisite == recoufregler.cle);
                if (okmultiple.Count > 0)
                {
                    foreach (SVC.DepeiementMultipleFournisseur itemmultiple in okmultiple)
                    {
                        DepeimentMultipleFournisseur = proxy.GetAlldepaief().Find(n => n.CleMultiple == itemmultiple.cleMultiple);
                        if (!depaiemlist.Contains(DepeimentMultipleFournisseur) && DepeimentMultipleFournisseur.Multiple == true)
                        {
                            depaiemlist.Add(DepeimentMultipleFournisseur);
                        }
                    }
                    FournDataGrid.ItemsSource = depaiemlist;
                }
                else
                {
                    FournDataGrid.ItemsSource = depaiemlist;
                }
                callbackrecu.InsertdepaiefCallbackEvent += new ICallback.CallbackEventHandler25(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
              }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertdepaief e)
        {
            try { 
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
        public void AddRefresh(SVC.depaief listMembershipOptic, int oper)
        {
            try
            {

                var LISTITEM1 = FournDataGrid.ItemsSource as IEnumerable<SVC.depaief>;
                List<SVC.depaief> LISTITEM = LISTITEM1.ToList();
                 
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
                   // if (LISTITEM.Count>0)
                  //  {
                        FournDataGrid.ItemsSource = LISTITEM;
                   // }
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeVersement(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeVersement(HandleProxy));
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
            try { 
            if (MemberUser.SuppressionRecouv == true && FournDataGrid.SelectedItem != null)
            {
                bool SupDepaif = false;
                bool SupDepanse = false;
                bool UpdateRecouf = false;
                try
                    {
                        SVC.depaief selectdepaief = FournDataGrid.SelectedItem as SVC.depaief;

                        if (selectdepaief.Multiple==false)
                        {
                            using (var ts = new TransactionScope())
                            {

                                /*suprimez l'écriture de depaief*/
                                proxy.DeletedepaiefAsync(selectdepaief);
                                SupDepaif = true;
                                // MessageBoxResult result11 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepaief succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                /*  suprimez l'écriture de depanse*/
                                var depense = (proxy.GetAllDepense()).Find(n => n.Num_Facture == selectdepaief.nfact && n.Débit == true && n.Montant == selectdepaief.montant);
                                proxy.DeleteDepenseAsync(depense);
                                SupDepanse = true;
                                //  MessageBoxResult result1g1 = Xceed.Wpf.Toolkit.MessageBox.Show("Deletedepense succées", "Medicus", MessageBoxButton.OK, MessageBoxImage.Information);
                                /*  update recouf c-a-d enlevez le montant */
                                var Recouf = (proxy.GetAllRecouf()).Find(n => n.nfact == selectdepaief.nfact && n.codef == selectdepaief.cf);
                                Recouf.reste = Recouf.reste + Recouf.versement;
                                Recouf.versement = Recouf.versement - selectdepaief.montant;
                                proxy.UpdateRecouf(Recouf);
                                UpdateRecouf = true;
                                if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                                {
                                    ts.Complete();
                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                }


                            }
                            if (SupDepaif == true && SupDepanse == true && UpdateRecouf == true)
                            {
                                proxy.AjouterTransactionACHATRefresh();
                                proxy.AjouterDepenseRefresh();
                                proxy.AjouterRecoufRefresh();
                                proxy.AjouterTransactionPaiementRefresh();
                             

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
                                List<int> identifiantfacture = new List<int>();
                                List<SVC.Recouf> listvisite = new List<SVC.Recouf>();
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Attention ! Vous allez supprimez un versement multiple qui va affecter d'autres enregistrements !veuillez confirmez votre choix", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    var depense = (proxy.GetAllDepense()).Find(n => n.cle == selectdepaief.cle && n.Débit == true && n.Montant == selectdepaief.montant);
                                    var depeiememultipl = (proxy.GetAllDepeiementMultipleFournisseur()).Where(n => n.cleMultiple == selectdepaief.CleMultiple).ToList();
                                    foreach (SVC.DepeiementMultipleFournisseur itemmultiple in depeiememultipl)
                                    {
                                        SVC.Recouf selectedvisite = proxy.GetAllRecoufBycode(itemmultiple.CodeFourn.Value).Find(n => n.cle == itemmultiple.cleVisite);

                                        if (!listvisite.Contains(selectedvisite))
                                        {
                                            listvisite.Add(selectedvisite);
                                        }
                                    }


                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        proxy.Deletedepaief(selectdepaief);
                                        SuppDepeiement = true;
                                        proxy.DeleteDepense(depense);
                                        SupDepense = true;
                                        for (int i = depeiememultipl.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/
                                        {
                                            var itemvisite = depeiememultipl.ElementAt(i);
                                            SVC.Recouf updatevisiteselected = listvisite.Find(n => n.cle == itemvisite.cleVisite);

                                            UpdateVisite = false;
                                            updatevisiteselected.reste = updatevisiteselected.reste + itemvisite.montant;
                                            updatevisiteselected.versement = updatevisiteselected.versement - itemvisite.montant;
                                            if (updatevisiteselected.reste != 0)
                                            {
                                                updatevisiteselected.soldé = false;
                                            }
                                            proxy.UpdateRecouf(updatevisiteselected);
                                            UpdateVisite = true;
                                            SupDepeiementMultiple = false;
                                            proxy.DeleteDepeiementMultipleFournisseur(itemvisite);
                                            SupDepeiementMultiple = true;
                                            identifiantfacture.Add(updatevisiteselected.Id);
                                        }
                                        if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true)
                                        {
                                            ts.Complete();

                                        }
                                        else
                                        {
                                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OpérationéchouéeTransaction, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                        }

                                    }

                                    if (SuppDepeiement == true && SupDepense == true && UpdateVisite == true && SupDepeiementMultiple == true)
                                    {
                                        proxy.AjouterTransactionPaiementRefresh();
                                        proxy.AjouterTransactionACHATRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        for (int i = identifiantfacture.Count - 1; i >= 0; i--)/*deux critére 1er connécté plus le parcour de la liste*/

                                        {
                                            var item = identifiantfacture.ElementAt(i);
                                            proxy.AjouterListeRecoufRefresh(item);
                                        }
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }

                                }
                            }
                        }
                    }
                catch (FaultException ex)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Medicus", MessageBoxButton.OK, MessageBoxImage.Stop);


                }
                catch (Exception ex)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
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

                if (MemberUser.ImpressionRecouv == true)
                {
                if (FournDataGrid.SelectedItem != null)
                   {
                        SVC.depaief SelectMedecin = FournDataGrid.SelectedItem as SVC.depaief;
                        List<SVC.depaief> listdepaief = new List<SVC.depaief>();
                        listdepaief.Add(SelectMedecin);
                        ImpressionListeDesVersements clsho = new ImpressionListeDesVersements(proxy, Fournisseur, listdepaief);
                        clsho.Show();
                  }
                    else
                    {
                        if (txtRecherche.Text != "")
                        {
                            List<SVC.depaief> test = FournDataGrid.ItemsSource as List<SVC.depaief>;

                            var t = (from e1 in test

                                     where e1.paiem.ToUpper().Contains(txtRecherche.Text.ToUpper())

                                     select e1);



                            ImpressionListeDesVersements clsho = new ImpressionListeDesVersements(proxy, Fournisseur, t.ToList());
                            clsho.Show();
                        }
                       else
                        {
                            List<SVC.depaief> test = FournDataGrid.ItemsSource as List<SVC.depaief>;


                            ImpressionListeDesVersements clsho = new ImpressionListeDesVersements(proxy, Fournisseur, test.ToList());
                            clsho.Show();

                        }


                   }
                }
            }catch (Exception ex)
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
                        SVC.depaief p = o as SVC.depaief;
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
