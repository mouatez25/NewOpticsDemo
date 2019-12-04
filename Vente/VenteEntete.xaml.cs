
using NewOptics.Administrateur;
using NewOptics.Caisse;
using NewOptics.Comptoir;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Vente
{
    /// <summary>
    /// Interaction logic for Vente.xaml
    /// </summary>
    public partial class VenteEntete : Page
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerVentes();
        public VenteEntete(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;

                DateSaisieFin.SelectedDate= DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateTime.Now, DateTime.Now).Where(n=> (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"));
                txtDebit.Text = Convert.ToString((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.net));
                TxtCreebit.Text = Convert.ToString((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.versement));
                txtSolde.Text = Convert.ToString(((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.reste)));
                callbackrecu.InsertF1CallbackEvent += new ICallback.CallbackEventHandler6(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                PatientDataGrid.SelectedItem = null;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVentes(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVentes(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertF1 e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav, e.operleav);
            }));
        }
        public void AddRefresh(SVC.F1 listmembership, int oper)
        {
            try
            {
                if ((listmembership.nfact.Substring(0, 1) != "P" && listmembership.nfact.Substring(0, 1) != "R"))
                {
                    if (listmembership.date >= DateSaisieDébut.SelectedDate && listmembership.date <= DateSaisieFin.SelectedDate)
                    {
                        var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                        List<SVC.F1> LISTITEM = LISTITEM1.ToList();

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

                        var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                        txtDebit.Text = Convert.ToString((LISTITEM.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))).AsEnumerable().Sum(o => o.net));
                        TxtCreebit.Text = Convert.ToString((LISTITEM.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))).AsEnumerable().Sum(o => o.versement));
                        txtSolde.Text = Convert.ToString((LISTITEM.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))).AsEnumerable().Sum(o => o.reste));
                    } } }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Value = "";
                 
                if (CompteComboBox.SelectedIndex >= 0 && CompteComboBox.SelectedIndex >= 0 && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {
                    Value = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                    switch (Value)
                    {
                        case "Tous Mes Ventes":
                             
                                PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date));


                            



                            break;
                        case "Facture":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n=>n.nfact.Substring(0,1)=="F"));

                            break;
                        case "Facture d'avoir":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(0, 1) == "A"));
                            break;
                        case "Bon Livraison":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(0, 1) == "B"));
                            break;
                        case "Bon d'avoir":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(0, 1) == "C"));
                            break;
                        case "Proforma":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(0, 1) == "P"));
                            break;
                        case "Facture provisoir":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(0, 1) == "R"));
                            break;
                        case "Vente comptoir":
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(0,2) == "Co"));
                            break;
                        default:
                            PatientDataGrid.ItemsSource = (proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date));


                            break;
                    }
                    var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                    txtDebit.Text = Convert.ToString((test.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))).AsEnumerable().Sum(o => o.net));
                    TxtCreebit.Text = Convert.ToString((test.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))).AsEnumerable().Sum(o => o.versement));
                    txtSolde.Text = Convert.ToString((test.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))).AsEnumerable().Sum(o => o.reste));
                    PatientDataGrid.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {

                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModuleStatistiqueAcces==true && DateSaisieDébut.SelectedDate.Value.Date != null && DateSaisieFin.SelectedDate.Value.Date != null)
                {
                    if (PatientDataGrid.SelectedItem!=null)
                    {
                        SVC.F1 selected = PatientDataGrid.SelectedItem as SVC.F1;
                        if (selected.nfact.Substring(0, 2) != "Co")
                        {

                            List<SVC.F1> list = new List<SVC.F1>();
                            list.Add(selected);
                            SVC.ClientV client = proxy.GetAllClientVBYID(Convert.ToInt16(selected.codeclient)).First();
                            ClientA.ImpressionFacture cl = new ClientA.ImpressionFacture(proxy, list, client,1);
                            cl.Show();
                        }
                        else
                        {
                            if (selected.nfact.Substring(0, 2) == "Co")
                            {
                                List<SVC.F1> list = new List<SVC.F1>();
                                list.Add(selected);
                                SVC.ClientV client = proxy.GetAllClientVBYID(Convert.ToInt16(selected.codeclient)).First();
                                ImpressionTickets cl = new ImpressionTickets(proxy,list,client);
                                cl.Show();
                            }
                        }
                        


                    }
                    else
                    {
                        List<SVC.F1> itemsSource0 = new List<SVC.F1>();
                        List<SVC.F1> itemsSource1 = PatientDataGrid.Items.OfType<SVC.F1>().ToList();
                       foreach(var p in itemsSource1)
                        {
                            

                            if ((p.nfact.Substring(0, 1) != "P" && p.nfact.Substring(0, 1) != "R"))
                            {
                                itemsSource0.Add(p);
                            }
                        }
                        ImpressionVente cl = new ImpressionVente(proxy, itemsSource0, DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date, 0);
                        cl.Show();
                    }
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

       

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void PatientDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnNewRéglement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationRecouv == true && PatientDataGrid.SelectedItem != null && PatientDataGrid.SelectedItems.Count == 1)
                {

                    SVC.F1 SelectMedecin = PatientDataGrid.SelectedItem as SVC.F1;
                    if (SelectMedecin.reste != 0)
                    {
                        if (SelectMedecin.nfact.Substring(0, 1) != "P" && SelectMedecin.nfact.Substring(0, 1) != "R")
                        {
                            AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, null, 3, null, SelectMedecin, null, null);
                            bn.Show();
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Facture déja soldé", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                    }

                }
                else
                {
                    if (memberuser.CreationRecouv == true && PatientDataGrid.SelectedItem != null && PatientDataGrid.SelectedItems.Count > 1)
                    {
                        List<SVC.F1> selectedreste = PatientDataGrid.SelectedItems.OfType<SVC.F1>().ToList();
                        if (DetectPatientInLIST(selectedreste))
                        {
                            List<SVC.F1> Realselectedvisitewithreste = new List<SVC.F1>();
                            foreach (SVC.F1 item in selectedreste)
                            {
                                if (item.reste != 0)
                                {
                                    Realselectedvisitewithreste.Add(item);
                                }

                            }
                            if (Realselectedvisitewithreste.Count == 1)
                            {
                                AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, null, 3, null, Realselectedvisitewithreste.First(), null, null);
                                bn.Show();
                            }
                            else
                            {
                                if (Realselectedvisitewithreste.Count > 1)
                                {
                                    AjouterTransaction bn = new AjouterTransaction(proxy, memberuser, callback, null, 4, null, null, Realselectedvisitewithreste, null);
                                    bn.Show();
                                }
                                else
                                {
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }
                        }
                        else
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionnez les visites d'un même patient", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public bool DetectPatientInLIST(List<SVC.F1> myList)
        {
            if (myList.Any())
            {
                var value = myList.First().codeclient;
                return myList.All(item => item.codeclient == value);
            }
            else
            {
                return true;
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
                        SVC.F1 p = o as SVC.F1;
                        if (t.Name == "txtId")
                            return (p.nfact == filter);
                        return (p.nfact.ToUpper().Contains(filter.ToUpper()));
                    };
                }
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
