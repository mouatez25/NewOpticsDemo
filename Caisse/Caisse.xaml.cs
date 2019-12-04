
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Caisse
{
    /// <summary>
    /// Interaction logic for Caisse.xaml
    /// </summary>
    public partial class Caisse : Page
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerDépense();
        SVC.Client CLIENTCONNECT;
        bool soldedepartd = false;
        public Caisse(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu,SVC.Client clientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
              
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;

               List<SVC.Depense> itemsSource1 = proxy.GetAllDepense().Where(n => n.DateDebit >= DateSaisieDébut.SelectedDate.Value && n.DateDebit <= DateSaisieFin.SelectedDate.Value).ToList();
                var soldedepartmontant = proxy.GetAllDepense().Where(n => n.DateDebit < Convert.ToDateTime(DateSaisieDébut.SelectedDate.Value)).AsEnumerable().Sum(o => o.MontantCrédit - o.Montant);
                SVC.Depense soldedepart = new SVC.Depense
                {
                    CompteDébité = "Solde de départ",
                    DateSaisie = DateSaisieDébut.SelectedDate.Value,
                    DateDebit = DateSaisieDébut.SelectedDate.Value,
                    Username = "Solde de départ",
                    ModePaiement = "Solde de départ",
                    RubriqueComptable = "Solde de départ",
                };
                if (soldedepartmontant < 0)
                {
                    soldedepart.Montant = soldedepartmontant * -1;
                    soldedepart.MontantCrédit = 0;
                }
                else
                {
                    soldedepart.Montant = 0;
                    soldedepart.MontantCrédit = soldedepartmontant;
                }

                itemsSource1.Insert(0, soldedepart);
                PatientDataGrid.ItemsSource = itemsSource1;

                soldedepartd = true;

                txtDebit.Text=Convert.ToString((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.Montant));
                TxtCreebit.Text = Convert.ToString((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.MontantCrédit));
                txtSolde.Text = Convert.ToString(((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.MontantCrédit)) - ((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.Montant)));
                callbackrecu.InsertDepenseCallbackEvent += new ICallback.CallbackEventHandler18(callbackrecu_Refresh);
                CLIENTCONNECT = clientrecu;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDépense(HandleProxy));
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

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        var wndlistsession = Window.GetWindow(this);

                        Grid test = (Grid)wndlistsession.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;
                        Button confirmer = (Button)wndlistsession.FindName("Confirmer");
                        confirmer.IsEnabled = false;
                        Grid tests = (Grid)wndlistsession.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDépense(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertDepense e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav,e.operleav);
            }));
        }
        public void AddRefresh(SVC.Depense listmembership,int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Depense>;
                List<SVC.Depense> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                     //   var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                       // objectmodifed = listmembership;


                        var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //objectmodifed = listmembership;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listmembership;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }

                
                }
                PatientDataGrid.ItemsSource = LISTITEM;

                // PatientDataGrid.ItemsSource = proxy.GetAllDepense();
                var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Depense>;
                txtDebit.Text = Convert.ToString((LISTITEM).AsEnumerable().Sum(o => o.Montant));
                TxtCreebit.Text = Convert.ToString((LISTITEM).AsEnumerable().Sum(o => o.MontantCrédit));
                txtSolde.Text = Convert.ToString((LISTITEM).AsEnumerable().Sum(o => o.MontantCrédit) - ((test).AsEnumerable().Sum(o => o.Montant)));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationCaisse == true)
                {
                    AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, null, 1, null, null, null,null);
                    bn.Show();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.SuppressionCaisse== true && PatientDataGrid.SelectedItem!=null)
                {
                    SVC.Depense SelectMedecin = PatientDataGrid.SelectedItem as SVC.Depense;
                    if (SelectMedecin.Auto != true)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxQuestionOperation, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            using (var ts = new TransactionScope())
                            {
                                proxy.DeleteDepense(SelectMedecin);
                                ts.Complete();
                            }
                            proxy.AjouterDepenseRefresh();
                        }
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxEcriutreautomatique, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null && memberuser.ModificationCaisse == true)
                {

                    SVC.Depense SelectMedecin = PatientDataGrid.SelectedItem as SVC.Depense;
                    if (SelectMedecin.Auto == false)
                    {
                        AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, SelectMedecin, 1, null, null, null,null);
                        bn.Show();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxEcriutreautomatique, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (memberuser.ImpressionCaisse==true)
            { 
                var itemsSource0 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Depense>;
                List<SVC.Depense> itemsSource1 = new List<SVC.Depense>();
                DateTime dateinfreport= DateTime.Now;
                foreach (SVC.Depense item in itemsSource0)
                {
                    if (item.DateDebit<=dateinfreport)
                    {
                        dateinfreport = Convert.ToDateTime(item.DateDebit);
                    }
                    itemsSource1.Add(item);

                }
                if(soldedepartd==false)
                    {
                        var soldedepartmontant = proxy.GetAllDepense().Where(n => n.DateDebit < Convert.ToDateTime(dateinfreport)).AsEnumerable().Sum(o => o.MontantCrédit - o.Montant);
                        SVC.Depense soldedepart = new SVC.Depense
                        {
                            CompteDébité = "Solde de départ",
                            DateSaisie = dateinfreport,
                            DateDebit = dateinfreport,
                            Username = "Solde de départ",
                            ModePaiement = "Solde de départ",
                            RubriqueComptable = "Solde de départ",
                        };
                        if (soldedepartmontant < 0)
                        {
                            soldedepart.Montant = soldedepartmontant * -1;
                            soldedepart.MontantCrédit = 0;
                        }
                        else
                        {
                            soldedepart.Montant = 0;
                            soldedepart.MontantCrédit = soldedepartmontant;
                        }

                        itemsSource1.Insert(0, soldedepart);
                    }
          
                ImpressionMouvement cl = new ImpressionMouvement(proxy, itemsSource1);
                cl.Show();
            }else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

      /*  private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    SVC.Depense p = o as SVC.Depense;
                    if (t.Name == "txtId")
                        return (p.Id == Convert.ToInt32(filter));
                    return (p.RubriqueComptable.ToUpper().Contains(filter.ToUpper()));
                };
            }
        }*/

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null && memberuser.ModificationCaisse == true)
                {

                    SVC.Depense SelectMedecin = PatientDataGrid.SelectedItem as SVC.Depense;
                    if (SelectMedecin.Auto == false)
                    {
                        AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, SelectMedecin, 1, null, null, null,null);
                        bn.Show();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxEcriutreautomatique, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }

        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Value = "";
                string ValueCompte = "";
                if (ModeRéglement.SelectedIndex >= 0 && CompteComboBox.SelectedIndex >= 0 && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {

                    Value = ((ComboBoxItem)ModeRéglement.SelectedItem).Content.ToString();
                    ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                    switch (Value)
                    {
                        case "Tous les modes de règlement":
                            if (ValueCompte == "Tous Mes Comptes")
                            {
                                PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateDebit >= DateSaisieDébut.SelectedDate.Value && n.DateDebit <= DateSaisieFin.SelectedDate.Value);

                              //  MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(DateSaisieDébut.SelectedDate.Value.ToString()+" "+ DateSaisieFin.SelectedDate.Value.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateDebit >= DateSaisieDébut.SelectedDate.Value && n.DateDebit <= DateSaisieFin.SelectedDate.Value && n.CompteDébité == ValueCompte);
                            //    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(DateSaisieDébut.SelectedDate.Value.ToString() + " " + DateSaisieFin.SelectedDate.Value.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }



                            break;
                        default:
                            if (ValueCompte == "Tous Mes Comptes")
                            {
                                PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateDebit >= DateSaisieDébut.SelectedDate.Value && n.DateDebit <= DateSaisieFin.SelectedDate.Value && n.ModePaiement == Value);
                              //  MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(DateSaisieDébut.SelectedDate.Value.ToString() + " " + DateSaisieFin.SelectedDate.Value.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateDebit >= DateSaisieDébut.SelectedDate.Value && n.DateDebit <= DateSaisieFin.SelectedDate.Value && n.CompteDébité == ValueCompte && n.ModePaiement == Value);
                              //  MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(DateSaisieDébut.SelectedDate.Value.ToString() + " " + DateSaisieFin.SelectedDate.Value.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }

                            break;
                    }
                    var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Depense>;
                    txtDebit.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.Montant));
                    TxtCreebit.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.MontantCrédit));
                    txtSolde.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.MontantCrédit) - ((test).AsEnumerable().Sum(o => o.Montant)));
                    soldedepartd = false;
                }
            }catch(Exception ex)
            {
          
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }
  

        private void DateSaisieFin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
           /* string Value = "";
            string ValueCompte = "";
            if (ModeRéglement.SelectedIndex >= 0 && CompteComboBox.SelectedIndex >= 0)
            {
                Value = ((ComboBoxItem)ModeRéglement.SelectedItem).Content.ToString();
                ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                switch (Value)
                {
                    case "Tous les modes de règlement":
                        if (ValueCompte == "Tous Mes Comptes")
                        {
                            PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateSaisie >= DateSaisieDébut.SelectedDate && n.DateSaisie <= DateSaisieFin.SelectedDate);


                        }
                        else
                        {
                            PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateSaisie >= DateSaisieDébut.SelectedDate && n.DateSaisie <= DateSaisieFin.SelectedDate && n.CompteDébité == ValueCompte);
                        }



                        break;
                    default:
                        if (ValueCompte == "Tous Mes Comptes")
                        {
                            PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateSaisie >= DateSaisieDébut.SelectedDate && n.DateSaisie <= DateSaisieFin.SelectedDate && n.ModePaiement == Value);
                        }
                        else
                        {
                            PatientDataGrid.ItemsSource = (proxy.GetAllDepense()).Where(n => n.DateSaisie >= DateSaisieDébut.SelectedDate && n.DateSaisie <= DateSaisieFin.SelectedDate && n.CompteDébité == ValueCompte && n.ModePaiement == Value);
                        }

                        break;
                }
                var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Depense>;
                txtDebit.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.Montant));
                TxtCreebit.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.MontantCrédit));
                txtSolde.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.MontantCrédit) - ((test).AsEnumerable().Sum(o => o.Montant)));
            }*/
        }



        private void ModeRéglement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            
        }

        private void PatientDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            { 
 
            ModeRéglement.SelectedIndex = 0;
            CompteComboBox.SelectedIndex = 0;
                DateSaisieFin.SelectedDate = DateTime.Now.Date;
                DateSaisieDébut.SelectedDate = DateTime.Now.Date;
                List<SVC.Depense> itemsSource1 = proxy.GetAllDepense().Where(n => n.DateDebit >= DateTime.Now.Date && n.DateDebit <= DateTime.Now.Date).ToList();
                var soldedepartmontant = proxy.GetAllDepense().Where(n => n.DateDebit < Convert.ToDateTime(DateTime.Now.Date)).AsEnumerable().Sum(o => o.MontantCrédit - o.Montant);
                SVC.Depense soldedepart = new SVC.Depense
                {
                    CompteDébité = "Solde de départ",
                    DateSaisie = DateSaisieDébut.SelectedDate.Value,
                    DateDebit = DateSaisieDébut.SelectedDate.Value,
                    Username = "Solde de départ",
                    ModePaiement = "Solde de départ",
                    RubriqueComptable = "Solde de départ",
                };
                if (soldedepartmontant < 0)
                {
                    soldedepart.Montant = soldedepartmontant * -1;
                    soldedepart.MontantCrédit = 0;
                }
                else
                {
                    soldedepart.Montant = 0;
                    soldedepart.MontantCrédit = soldedepartmontant;
                }

                itemsSource1.Insert(0, soldedepart);
                PatientDataGrid.ItemsSource = itemsSource1;


                soldedepartd = true;
                txtDebit.Text = Convert.ToString((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.Montant));
                TxtCreebit.Text = Convert.ToString((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.MontantCrédit));
                txtSolde.Text = Convert.ToString(((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.MontantCrédit)) - ((proxy.GetAllDepense()).AsEnumerable().Sum(o => o.Montant)));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtRecherche_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {

                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Depense p = o as SVC.Depense;
                        if (t.Name == "txtId")
                            return (p.RubriqueComptable == filter);
                        return (p.RubriqueComptable.ToUpper().Contains(filter.ToUpper()));
                    };
                }


                PatientDataGrid.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
