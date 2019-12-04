
using NewOptics.Administrateur;
using NewOptics.Caisse;
using NewOptics.ClientA;
using NewOptics.Vente;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.CreditCreance
{
    /// <summary>
    /// Interaction logic for CreditClient.xaml
    /// </summary>
    public partial class CreditClient : Window
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic MemberUser;
        private delegate void FaultedInvokerReglementPatient();
        //  SVC.Patient selectedpatient;
        int interfaceimpression = 0;
        List<SVC.F1> visitecredit;
        
        public CreditClient(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = memberrecu;
                DateVisiteDébut.SelectedDate = DateTime.Now;
                DateVisiteFin.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")); ;
                interfaceimpression = 0;
                txtAchat.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.net)));
                TxtVersement.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.versement)));
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.reste))));
                callbackrecu.InsertF1CallbackEvent += new ICallback.CallbackEventHandler6(callbackrecu_Refresh);
                callbackrecu.InsertF1ListCallbackEvent += new ICallback.CallbackEventHandler27(callbackrecuF1List_Refresh);
                PatientDataGrid.SelectedItem = null;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecuF1List_Refresh(object source, CallbackEventInsertF1List e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshF1ListVente(e.clientleav);
            }));
        }
        public void AddRefreshF1ListVente(List<SVC.F1> memberfacture)
        {

            try
            {


                try
                {
                    foreach (SVC.F1 listmembership in memberfacture)
                    {
                        //  MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("i'm here3", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        /*******************************************************************************************/
                         var LISTITEM11 = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                            List<SVC.F1> LISTITEM0 = LISTITEM11.ToList();
                            



                                var objectmodifed = LISTITEM0.Find(n => n.Id == listmembership.Id);
                                //objectmodifed = listmembership;
                                var index = LISTITEM0.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM0[index] = listmembership;



                              PatientDataGrid.ItemsSource = LISTITEM0.OrderBy(r => r.date);
                           
                        txtAchat.Text = Convert.ToString((LISTITEM0.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.net)));
                        TxtVersement.Text = Convert.ToString((LISTITEM0.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.versement)));
                        txtFournisseur.Text = Convert.ToString(((LISTITEM0.Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.reste))));

                    }
                }
                catch (Exception ex)
                {
                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }



        }

        void callbackrecu_Refresh(object source, CallbackEventInsertF1 e)
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
        public void AddRefresh(SVC.F1 listmembership, int oper)
        {
            try
            {
                if ((listmembership.nfact.Substring(0, 1) != "P" && listmembership.nfact.Substring(0, 1) != "R"))
                {
                    var LISTITEM11 = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                    List<SVC.F1> LISTITEM0 = LISTITEM11.ToList();

                    if (oper == 1)
                    {
                        LISTITEM0.Add(listmembership);
                    }
                    else
                    {
                        if (oper == 2)
                        {


                            var objectmodifed = LISTITEM0.Find(n => n.Id == listmembership.Id);
                            //objectmodifed = listmembership;
                            var index = LISTITEM0.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEM0[index] = listmembership;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                var deleterendez = LISTITEM0.Where(n => n.Id == listmembership.Id).First();
                                LISTITEM0.Remove(deleterendez);
                            }
                        }
                    }







                    PatientDataGrid.ItemsSource = LISTITEM0;
                    txtAchat.Text = Convert.ToString((LISTITEM0).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.net));
                    TxtVersement.Text = Convert.ToString((LISTITEM0).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.versement));
                    txtFournisseur.Text = Convert.ToString((LISTITEM0).AsEnumerable().Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).Sum(o => o.reste));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerReglementPatient(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            try
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerReglementPatient(HandleProxy));
                    return;
                }
                HandleProxy();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
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
                        /*   proxy.Abort();
                           proxy = null;
                           this.Close();*/
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

        private void btnSolde_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationRecouv == true && PatientDataGrid.SelectedItem != null && PatientDataGrid.SelectedItems.Count == 1)
                {

                    SVC.F1 SelectMedecin = PatientDataGrid.SelectedItem as SVC.F1;
                    if (SelectMedecin.reste != 0)
                    {
                        if (SelectMedecin.nfact.Substring(0, 1) != "P" && SelectMedecin.nfact.Substring(0, 1) != "R")
                        {
                            AjouterTransaction bn = new AjouterTransaction(proxy, MemberUser, callback, null, 3, null, SelectMedecin, null,null);
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
                    if (MemberUser.CreationRecouv == true && PatientDataGrid.SelectedItem != null && PatientDataGrid.SelectedItems.Count > 1)
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
                                AjouterTransaction bn = new AjouterTransaction(proxy,MemberUser, callback, null, 3, null, Realselectedvisitewithreste.First(), null,null);
                                bn.Show();
                            }
                            else
                            {
                                if (Realselectedvisitewithreste.Count > 1)
                                {
                                    AjouterTransaction bn = new AjouterTransaction(proxy, MemberUser, callback, null, 4, null, null, Realselectedvisitewithreste,null);
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
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionnez les factures d'un même client", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void btnListeVersement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.ModuleRecouv == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.F1 SelectMedecin = PatientDataGrid.SelectedItem as SVC.F1;
                    if (SelectMedecin.codeclient == 0)
                    {
                        var patient = (proxy.GetAllClientV()).Find(n => n.cle == SelectMedecin.cle);

                        VersementClient cl = new VersementClient(proxy, MemberUser, callback, patient, SelectMedecin, true);
                        cl.Show();
                    }
                    else
                    {
                        var patient = (proxy.GetAllClientV()).Find(n => n.Id == SelectMedecin.codeclient);
                        VersementClient cl = new VersementClient(proxy, MemberUser, callback, patient, SelectMedecin, false);
                        cl.Show();
                    }
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.ModuleStatistiqueAcces == true && DateVisiteDébut.SelectedDate.Value.Date != null && DateVisiteFin.SelectedDate.Value.Date != null)
                {
                    if (interfaceimpression == 0)
                    {


                        if (PatientDataGrid.SelectedItem != null)
                        {
                            SVC.F1 selected = PatientDataGrid.SelectedItem as SVC.F1;
                            List<SVC.F1> list = new List<SVC.F1>();
                            list.Add(selected);
                            SVC.ClientV client = proxy.GetAllClientVBYID(Convert.ToInt16(selected.codeclient)).First();
                            ClientA.ImpressionFacture cl = new ClientA.ImpressionFacture(proxy, list, client, 1);
                            cl.Show();


                        }
                        else
                        {

                            List<SVC.F1> itemsSourceL = PatientDataGrid.Items.OfType<SVC.F1>().ToList();
                            List<SVC.F1> itemsSource0 = new List<SVC.F1>();
                            foreach (var p in itemsSourceL)
                            {
                              

                                if ((p.nfact.Substring(0, 1) != "P" && p.nfact.Substring(0, 1) != "R"))
                                {
                                    itemsSource0.Add(p);
                                }
                            }
                            ImpressionEtat cl = new ImpressionEtat(proxy, itemsSource0, DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date);
                            cl.Show();

                        }
                    }
                    else
                    {
                      
                      
                        List<SVC.F1> itemsSource0 = PatientDataGrid.Items.OfType<SVC.F1>().ToList();
                       
                        ImpressionVente cl = new ImpressionVente(proxy, itemsSource0, DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date, 0);
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

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    Boolean Value = false;
                    int radiobutton = 0;
                if (DateVisiteDébut.SelectedDate != null && DateVisiteFin.SelectedDate != null)
                    {
                        if (chFactureRegler.IsChecked == true)
                        {
                            Value = true;
                            radiobutton = 1;
                        }
                        else
                        {
                            if (chFactureNORegler.IsChecked == true)
                            {
                                Value = false;
                                radiobutton = 1;
                            }
                            else
                            {
                                radiobutton = 0;
                            }

                        }
                        if (chFacture.IsChecked == true)
                        {
                            radiobutton = 0;
                        }

                    if (radiobutton==1)
                    {
                        PatientDataGrid.ItemsSource=proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R" && n.soldé==Value));
                        txtAchat.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R" && n.soldé == Value)).AsEnumerable().Sum(o => o.net)));
                        TxtVersement.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R" && n.soldé == Value)).AsEnumerable().Sum(o => o.versement)));
                        txtFournisseur.Text = Convert.ToString(((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R" && n.soldé == Value)).AsEnumerable().Sum(o => o.reste))));
                    }
                    else
                    {
                        PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"));
                        txtAchat.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.net)));
                        TxtVersement.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.versement)));
                        txtFournisseur.Text = Convert.ToString(((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.reste))));
                    }
                    interfaceimpression = 0;
                    PatientDataGrid.SelectedItem = null;
                }

                

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btndeFilter_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DateVisiteDébut.SelectedDate= DateTime.Now;
                DateVisiteFin.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")); ;
                chFacture.IsChecked = true;
                btnSolde.IsEnabled = true;
                txtAchat.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.net)));
                TxtVersement.Text = Convert.ToString((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.versement)));
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllF1(DateVisiteDébut.SelectedDate.Value.Date, DateVisiteFin.SelectedDate.Value.Date).Where(n => (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R")).AsEnumerable().Sum(o => o.reste))));
                interfaceimpression = 0;
                PatientDataGrid.SelectedItem = null;
            } catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        

        
        private void BTNCALCUL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                visitecredit = new List<SVC.F1>();
                var listevisitecode = (from ta in proxy.GetAllF1All()
                                      
                                       where ta.codeclient != 0 && ta.nfact.Substring(0,1)!="P" && ta.nfact.Substring(0, 1) != "R"
                                       select ta.codeclient).Distinct();

                foreach (int itemcode in listevisitecode)
                {
                    SVC.F1 newvisite = new SVC.F1();
                    newvisite.net = 0;
                    newvisite.versement = 0;
                    newvisite.reste = 0;
                    //   newvisite.cle = "";
                    foreach (SVC.F1 itemvisite in proxy.GetAllF1All().Where(n=>n.nfact.Substring(0, 2) != "Co" && n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R"))
                    {

                        if (itemcode == itemvisite.codeclient && itemcode != 0)
                        {

                            newvisite.codeclient = itemcode;
                            // if(itemcode==0)
                            /*{
                                newvisite.NomPatient = " Total Ancien solde";
                                newvisite.PrénomPatient = "Du cabinet";
                            }
                            else
                            {*/
                            newvisite.raison = itemvisite.raison;
                            
                            //  }
                            newvisite.cle = itemvisite.cle;
                            newvisite.net = newvisite.net + itemvisite.net;
                            newvisite.versement = newvisite.versement + itemvisite.versement;
                            newvisite.reste = newvisite.reste + itemvisite.reste;

                        }
                    }
                    visitecredit.Add(newvisite);
                }
                List<SVC.F1> listcle = proxy.GetAllF1All().Where(n => n.codeclient == 0 && n.cle != "" && n.raison.Trim()!= "Vente comptoir").ToList();

                foreach (SVC.F1 itemcle in listcle)
                {
                    SVC.F1 newvisite = new SVC.F1();
                    newvisite.net = 0;
                    newvisite.versement = 0;
                    newvisite.reste = 0;



                    newvisite.codeclient = itemcle.codeclient;


                    newvisite.raison = "Ancien Solde " + itemcle.raison;
                    newvisite.codeclient = itemcle.codeclient;

                    newvisite.net = newvisite.net + itemcle.net;
                    newvisite.versement = newvisite.versement + itemcle.versement;
                    newvisite.reste = newvisite.reste + itemcle.reste;
                    visitecredit.Add(newvisite);





                }


                PatientDataGrid.ItemsSource = visitecredit;
                var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                txtAchat.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.net));
                TxtVersement.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.versement));
                txtFournisseur.Text = Convert.ToString(((test).AsEnumerable().Sum(o => o.reste)));

                btnSolde.IsEnabled = false;
                interfaceimpression = 1;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
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
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtRecherchenOM_TextChanged(object sender, TextChangedEventArgs e)
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
                            return (p.raison == filter);
                        return (p.raison.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
