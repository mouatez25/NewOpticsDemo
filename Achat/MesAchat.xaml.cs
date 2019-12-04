using DevExpress.Xpf.Core;
using NewOptics.Administrateur;

using MahApps.Metro.Controls;
using System;
using System.Collections;
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
using NewOptics.Caisse;

namespace NewOptics.Achat
{
    /// <summary>
    /// Interaction logic for MesAchat.xaml
    /// </summary>
    public partial class MesAchat : DXWindow
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerListeAchat();
        SVC.Client CLIENTCONNECT;
        List<SVC.Recouf> visitecredit;
        int interfaceimpression = 2;
        public MesAchat(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu,SVC.Client ClientRecu)
        {

            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = new DateTime(2016, 01, 01);
                PatientDataGrid.ItemsSource = proxy.GetAllRecouf();
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
                CLIENTCONNECT = ClientRecu;
                txtAchat.Text = Convert.ToString((proxy.GetAllRecouf()).AsEnumerable().Sum(o => o.net));
                TxtVersement.Text = Convert.ToString((proxy.GetAllRecouf()).AsEnumerable().Sum(o => o.versement));
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllRecouf()).AsEnumerable().Sum(o => o.reste)));

                callbackrecu.InsertRecoufCallbackEvent += new ICallback.CallbackEventHandler23(callbackrecu_Refresh);
                callbackrecu.InsertFournCallbackEvent+= new ICallback.CallbackEventHandler20(callbackrecufourn_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecufourn_Refresh(object source, CallbackEventInsertFourn e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshfourn(e.clientleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshfourn(List<SVC.Fourn> listMembershipOptic)
        {
            try
            {

                FournisseurCombo.ItemsSource = listMembershipOptic;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertRecouf e)
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
        public void AddRefresh(SVC.Recouf listmembership, int oper)
        {
            try
            {


                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Recouf>;
                List<SVC.Recouf> LISTITEM = LISTITEM1.ToList();

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


                        var objectmodifed = LISTITEM.Find(n => n.nfact == listmembership.nfact);
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
                            var deleterendez = LISTITEM.Where(n => n.nfact == listmembership.nfact).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }


                }
                PatientDataGrid.ItemsSource = LISTITEM;

                var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Recouf>;
                txtAchat.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.net));
                TxtVersement.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.versement));
                txtFournisseur.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.reste));










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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeAchat(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeAchat(HandleProxy));
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


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            { 

            if (MemberUser.CreationAchat == true)
            {
                AjouterFactureAchat CLSession = new AjouterFactureAchat(proxy, MemberUser, callback, null,CLIENTCONNECT);
                CLSession.Show();

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
            try { 
            if (MemberUser.ModificationAchat == true && PatientDataGrid.SelectedItem != null)
            {
                SVC.Recouf SelectReceptFourn = PatientDataGrid.SelectedItem as SVC.Recouf;
                if (SelectReceptFourn.codef!=0)
                {
                    AjouterFactureAchat CLSession = new AjouterFactureAchat(proxy, MemberUser, callback, SelectReceptFourn, CLIENTCONNECT);
                    CLSession.Show();
                } else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (interfaceimpression == 2)
                {
                    if (MemberUser.ImpressionAchat == true && PatientDataGrid.SelectedItem == null)
                    {
                        var itemsSource0 = PatientDataGrid.ItemsSource as IEnumerable;// List<SVC.SalleDattente>;
                        List<SVC.Recouf> itemsSource1 = new List<SVC.Recouf>();
                        foreach (SVC.Recouf item in itemsSource0)
                        {
                            itemsSource1.Add(item);
                        }
                        ImpressionMesAchats cl = new ImpressionMesAchats(proxy, itemsSource1,2);
                        cl.Show();
                    }
                    else
                    {
                        if (MemberUser.ImpressionAchat == true && PatientDataGrid.SelectedItem != null)
                        {
                            var tt = (PatientDataGrid.SelectedItem as SVC.Recouf);
                            ImpressionFactureAchat cl = new ImpressionFactureAchat(proxy, tt);
                            cl.Show();
                        }
                    }
                }
                else
                {
                    if (interfaceimpression == 1)
                    {
                        var itemsource = PatientDataGrid.ItemsSource as List<SVC.Recouf>;
                        ImpressionMesAchats cl = new ImpressionMesAchats(proxy, itemsource, 1);
                        cl.Show();
                    }
                }
            }
            
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            string Value = "";

            if (FiscalCombo.SelectedIndex >= 0)
            {
                Value = ((ComboBoxItem)FiscalCombo.SelectedItem).Content.ToString();

                switch (Value)
                {
                    case "Toutes les factures":
                        if (FournisseurCombo.SelectedItem == null)
                        {

                            PatientDataGrid.ItemsSource = (proxy.GetAllRecouf()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate);


                        }
                        else
                        {
                            SVC.Fourn ValueFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                            PatientDataGrid.ItemsSource = (proxy.GetAllRecouf()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && (n.codef == ValueFourn.Id || n.cle==ValueFourn.cle));
                        }



                        break;
                    case "Facture Fiscale":
                        if (FournisseurCombo.SelectedItem == null)
                        {

                            PatientDataGrid.ItemsSource = (proxy.GetAllRecouf()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.fiscal == true );


                        }
                        else
                        {
                            SVC.Fourn ValueFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                            PatientDataGrid.ItemsSource = (proxy.GetAllRecouf()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate &&  n.codef == ValueFourn.Id && n.fiscal == true &&(n.codef == ValueFourn.Id || n.cle == ValueFourn.cle));
                        }



                        break;
                    case "Facture sans T.V.A":
                        if (FournisseurCombo.SelectedItem == null)
                        {

                            PatientDataGrid.ItemsSource = (proxy.GetAllRecouf()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate &&  n.Nonfiscal == true);


                        }
                        else
                        {
                            SVC.Fourn ValueFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                            PatientDataGrid.ItemsSource = (proxy.GetAllRecouf()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate  && n.codef == ValueFourn.Id &&  n.Nonfiscal == true && (n.codef == ValueFourn.Id || n.cle == ValueFourn.cle));
                        }



                        break;
                }
                    interfaceimpression = 2;
                    var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Recouf>;
                txtAchat.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.net));
                TxtVersement.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.versement));
                txtFournisseur.Text = Convert.ToString(((test).AsEnumerable().Sum(o => o.reste)));

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

       

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try { 

            if (MemberUser.ModificationAchat == true && PatientDataGrid.SelectedItem != null)
            {
                SVC.Recouf SelectReceptFourn = PatientDataGrid.SelectedItem as SVC.Recouf;

                    if (SelectReceptFourn.avoir==false)
                    {
                        if (SelectReceptFourn.codef != 0)
                        {
                            AjouterFactureAchat CLSession = new AjouterFactureAchat(proxy, MemberUser, callback, SelectReceptFourn, CLIENTCONNECT);
                            CLSession.Show();
                        }
                        else
                        {
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        AvoirFournisseur CLSession = new AvoirFournisseur(proxy, MemberUser, callback, SelectReceptFourn);
                        CLSession.Show();
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

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            TextBox t = (TextBox)sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    SVC.Recouf p = o as SVC.Recouf;
                    if (t.Name == "txtId")
                        return (p.nfact == filter);
                    return (p.nfact.ToUpper().Contains(filter.ToUpper()));
                };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnSolde_Click(object sender, RoutedEventArgs e)
        {
           try {

                 if (MemberUser.CreationCaisse == true && PatientDataGrid.SelectedItem != null)
                {

                    SVC.Recouf SelectMedecin = PatientDataGrid.SelectedItem as SVC.Recouf;
                    if (SelectMedecin.reste != 0)
                    {
                        AjouterTransaction bn = new AjouterTransaction(proxy, MemberUser, callback, null, 2, SelectMedecin, null,null,null);
                        bn.Show();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Facture déja soldée",NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                    }

                    }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnListeVersement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true && FournisseurCombo.SelectedItem != null)
                {
                    SVC.Fourn SelectReceptFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                    ListeDesVersements CLSession = new ListeDesVersements(proxy, MemberUser, callback, SelectReceptFourn,null);

                    CLSession.Show();
                }
                else
                {
                    if (MemberUser.CreationFichier == true && PatientDataGrid.SelectedItem != null && FournisseurCombo.SelectedItem == null)
                    {
                        SVC.Recouf SelectReceptFourn = PatientDataGrid.SelectedItem as SVC.Recouf;
                        SVC.Fourn fournSelected = (proxy.GetAllFourn()).Where(n => n.Id == SelectReceptFourn.codef).First();
                        ListeDesVersements CLSession = new ListeDesVersements(proxy, MemberUser, callback, fournSelected,null);

                        CLSession.Show();
                    }
                    else
                    {
                        if (MemberUser.CreationFichier == true && FournisseurCombo.SelectedItem != null && PatientDataGrid.SelectedItem == null)
                        {
                            SVC.Fourn SelectReceptFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                            ListeDesVersements CLSession = new ListeDesVersements(proxy, MemberUser, callback, SelectReceptFourn,null);

                            CLSession.Show();
                        }else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez en moin choisir un fournisseur ou un enregistrement", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                    }
                }
            }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btndeFilter_Click(object sender, MouseButtonEventArgs e)
        {
            try { 
            DateSaisieFin.SelectedDate = DateTime.Now;
            DateSaisieDébut.SelectedDate = new DateTime(2016, 01, 01);
            FiscalCombo.SelectedIndex = 0;
                interfaceimpression = 2;
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
            PatientDataGrid.ItemsSource = proxy.GetAllRecouf();
            txtAchat.Text = Convert.ToString((proxy.GetAllRecouf()).AsEnumerable().Sum(o => o.net));
            TxtVersement.Text = Convert.ToString((proxy.GetAllRecouf()).AsEnumerable().Sum(o => o.versement));
            txtFournisseur.Text = Convert.ToString(((proxy.GetAllRecouf()).AsEnumerable().Sum(o => o.reste)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void BTNCALCUL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                visitecredit = new List<SVC.Recouf>();
                var listevisitecode = (from ta in proxy.GetAllRecouf()
                                       where ta.codef != 0
                                       select ta.codef).Distinct();
                foreach (int itemcode in listevisitecode)
                {
                    SVC.Recouf newvisite = new SVC.Recouf();
                    newvisite.net = 0;
                    newvisite.versement = 0;
                    newvisite.reste = 0;

                    foreach (SVC.Recouf itemvisite in proxy.GetAllRecouf())
                    {
                        if (itemcode == itemvisite.codef)
                        {

                            newvisite.codef = itemcode;
                            newvisite.Fournisseur = itemvisite.Fournisseur;
                            newvisite.net = newvisite.net + itemvisite.net;
                            newvisite.versement = newvisite.versement + itemvisite.versement;
                            newvisite.reste = newvisite.reste + itemvisite.reste;

                        }
                    }
                    visitecredit.Add(newvisite);
                }
                List<SVC.Recouf> listcle = proxy.GetAllRecouf().Where(n => n.codef== 0 && n.cle != "").ToList();

                foreach (SVC.Recouf itemcle in listcle)
                {
                    SVC.Recouf newvisite = new SVC.Recouf();
                    newvisite.net = 0;
                    newvisite.versement = 0;
                    newvisite.reste = 0;



                    newvisite.codef = itemcle.codef;


                    newvisite.Fournisseur = "Ancien Solde " + itemcle.Fournisseur;
                   // newvisite.PrénomPatient = itemcle.PrénomPatient;

                    newvisite.net = newvisite.net + itemcle.net;
                    newvisite.versement = newvisite.versement + itemcle.versement;
                    newvisite.reste = newvisite.reste + itemcle.reste;
                    visitecredit.Add(newvisite);





                }
                PatientDataGrid.ItemsSource = visitecredit;
                var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Recouf>;
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

        private void btnNewavoir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationAchat==true)
                {
                   AvoirFournisseur CLSession = new AvoirFournisseur(proxy, MemberUser, callback, null);
                    CLSession.Show();
                }
                
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
    }
    }

