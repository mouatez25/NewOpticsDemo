
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

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for ListeProduit.xaml
    /// </summary>
    public partial class ListeProduit :  Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerListeProduit();
        public ListeProduit(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                FournDataGrid.ItemsSource = proxy.GetAllProduit();
                callbackrecu.InsertProduitCallbackEvent += new ICallback.CallbackEventHandler22(callbackrecu_Refresh);
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
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.Produit listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM11 = FournDataGrid.ItemsSource as IEnumerable<SVC.Produit>;
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










                FournDataGrid.ItemsSource = LISTITEM0;


            }
            catch (Exception ex)
            {

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeProduit(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeProduit(HandleProxy));
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

        
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true)
                {
                    AjouterProduit cl = new AjouterProduit(proxy, null, MemberUser, callback);
                    cl.Show();
                }
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
                if (MemberUser.SuppressionFichier== true && FournDataGrid.SelectedItem != null)
                {
                    SVC.Produit SelectMedecin = FournDataGrid.SelectedItem as SVC.Produit;

                    bool succes = false;
                    bool lentilleverre = false;
                    bool existelentille = proxy.GetAllLentillebycode(SelectMedecin.cab).Any();
                    bool existeverre = proxy.GetAllVerrebycode(SelectMedecin.cab).Any();
                    int interfacecatalogue=0;
                    if (existelentille || existeverre)
                    {
                        if(existeverre==true && existelentille==false)
                        {
                            interfacecatalogue = 1;
                            var existeverrevaleur = proxy.GetAllVerrebycode(SelectMedecin.cab).First();

                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                proxy.DeleteVerre(existeverrevaleur);
                                lentilleverre = true;
                                proxy.DeleteProduit(SelectMedecin);
                                succes = true;
                                ts.Complete();
                              
                            }

                        }
                        else
                        {
                            if (existeverre == false && existelentille == true)
                            {
                                interfacecatalogue = 2;
                                var existelentillevaleur = proxy.GetAllLentillebycode(SelectMedecin.cab).First();
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    proxy.DeleteLentille(existelentillevaleur);
                                    lentilleverre = true;
                                    proxy.DeleteProduit(SelectMedecin);
                                    succes = true;
                                    ts.Complete();

                                }
                            }
                        }
                    }
                    else
                    {
                        interfacecatalogue = 0;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            proxy.DeleteProduit(SelectMedecin);
                            ts.Complete();
                            succes = true;
                        }
                    }
                    if (succes==true && interfacecatalogue==0)
                    {
                        proxy.AjouterProduitStockRefresh();
                        MessageBoxResult resultsucces = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        if (succes == true && interfacecatalogue == 1 && lentilleverre==true)
                        {
                            proxy.AjouterProduitStockRefresh();
                            proxy.AjouterVerreRefresh();
                            MessageBoxResult resultsucces = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            if (succes == true && interfacecatalogue == 2 && lentilleverre == true)
                            {
                                proxy.AjouterProduitStockRefresh();
                                proxy.AjouterLentilleRefresh();
                                MessageBoxResult resultsucces = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                        }
                    }


                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                if (MemberUser.ModificationAchat == true && FournDataGrid.SelectedItem != null)
                {
                    SVC.Produit selectproduit = FournDataGrid.SelectedItem as SVC.Produit;
                    AjouterProduit cl = new AjouterProduit(proxy, selectproduit, MemberUser, callback);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
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
                if(MemberUser.ImpressionFichier==true)
                {
                    var test = FournDataGrid.Items.OfType<SVC.Produit>();
                    ImpressionNomenclature cl = new ImpressionNomenclature(proxy, test.ToList());
                        cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }



     
        private void FournDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MemberUser.ModificationAchat == true && FournDataGrid.SelectedItem != null)
                {
                    SVC.Produit selectproduit = FournDataGrid.SelectedItem as SVC.Produit;
                    AjouterProduit cl = new AjouterProduit(proxy, selectproduit, MemberUser, callback);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous ne pouvez pas faire cette opération", "Medicus", MessageBoxButton.YesNo, MessageBoxImage.Question);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        

        private void FournDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FournDataGrid.ItemsSource = proxy.GetAllProduit();
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
                        SVC.Produit p = o as SVC.Produit;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
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
