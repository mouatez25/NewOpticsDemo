
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
using NewOptics.Article;


namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for MonStock.xaml
    /// </summary>
    public partial class MonStock :Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerStock();
     
        bool filtre = false;

        public MonStock(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {
            InitializeComponent();
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllProdf();//.Where(x =>x.quantite>0);
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
                FamilleCombo.ItemsSource=proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
                AMSDataGrid.ItemsSource = proxy.GetAllam();
              
                
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf()).AsEnumerable().Sum(o => o.quantite * o.previent)));
                txtAMS.Text = Convert.ToString(((proxy.GetAllam()).AsEnumerable().Sum(o => o.quantite * o.previent)));
                callbackrecu.InsertProdfCallbackEvent += new ICallback.CallbackEventHandler21(callbackrecu_Refresh);
                callbackrecu.InsertamCallbackEvent += new ICallback.CallbackEventHandler26(callbackrecuF_Refresh);
                callbackrecu.InsertReceptProdfCallbackevent += new ICallback.CallbackEventHandler50(callbackrecuProdfRecept_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertProdf e)
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

        void callbackrecuF_Refresh(object source, CallbackEventInsertam e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresham(e.clientleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void callbackrecuProdfRecept_Refresh(object source, CallbackEventInsertProdfRecept e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshReceptProdf(e.clientleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshReceptProdf(List<SVC.Prodf> listMembershipOptic)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Prodf>;
                List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();
                List<SVC.Prodf> ancienliste = LISTITEM.Where(n => n.nfact == listMembershipOptic.First().nfact).ToList();


                foreach(SVC.Prodf item in listMembershipOptic)
                {
                    var objectmodifed = LISTITEM.Find(n => n.Id == item.Id);
                    
                    var index = LISTITEM.IndexOf(objectmodifed);
                     if (index != -1)
                        LISTITEM[index] = item;
                     if(LISTITEM.Contains(item)!=true)
                    {
                        LISTITEM.Add(item);
                    }

                }
                foreach(SVC.Prodf item in ancienliste)
                {
                    if (listMembershipOptic.Contains(item) != true)
                    {
                        LISTITEM.Remove(item);
                    }
                }

             




                  

                PatientDataGrid.ItemsSource = LISTITEM;
                txtFournisseur.Text = Convert.ToString(((LISTITEM).AsEnumerable().Sum(o => o.quantite * o.previent)));

















             //   PatientDataGrid.ItemsSource = listMembershipOptic;
              //  txtFournisseur.Text = Convert.ToString(((listMembershipOptic).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch(Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresham(List<SVC.am> listMembershipOptic)
        {
            try
            {
                AMSDataGrid.ItemsSource = listMembershipOptic;
                txtAMS.Text = Convert.ToString(((listMembershipOptic).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }catch(Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(SVC.Prodf listMembershipOptic,int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Prodf>;
                List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {
                        //  var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;


                        var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listMembershipOptic;
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
                }

                PatientDataGrid.ItemsSource = LISTITEM;
                txtFournisseur.Text = Convert.ToString(((LISTITEM).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
       
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerStock(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerStock(HandleProxy));
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
            try { 
            if (MemberUser.CreationAms == true && PatientDataGrid.SelectedItem != null)
            {
                SVC.Prodf SelectReceptFourn = PatientDataGrid.SelectedItem as SVC.Prodf;
                   
                        AjouterAms CLSession = new AjouterAms(proxy, MemberUser, callback, SelectReceptFourn);
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

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool AMResult = false;
                bool ProdfResult = false;
                if (MemberUser.SuppressionAms== true && AMSDataGrid.SelectedItem != null)
                {

                    if (proxy != null)
                    {
                        if (proxy.State == CommunicationState.Faulted)
                        {
                            HandleProxy();
                        }
                        else
                        {

                            SVC.am SelectReceptFourn = AMSDataGrid.SelectedItem as SVC.am;
                            SVC.Prodf selectprodf = (proxy.GetAllProdf()).Find(n => n.cp == SelectReceptFourn.codeprod && n.design == SelectReceptFourn.design && n.cf == SelectReceptFourn.cf && n.nfact == SelectReceptFourn.nfact);
                            try
                            {
                                using (var ts = new TransactionScope())
                                {
                                    if (SelectReceptFourn.quantite > 0)
                                    {
                                        if (SelectReceptFourn.stockage == true)
                                        {
                                            /*  100-100        *//*ams valeur supp a prodf*/
                                            var value = SelectReceptFourn.quantite - selectprodf.quantite;
                                            if (value > 0)
                                            {

                                                AMResult = false;
                                                ProdfResult = false;
                                            }
                                            else
                                            {
                                                if (value <= 0)
                                                {

                                                    proxy.Deleteam(SelectReceptFourn);
                                                    AMResult = true;
                                                    selectprodf.quantite = selectprodf.quantite - SelectReceptFourn.quantite;
                                                    proxy.UpdateProdf(selectprodf);
                                                    ProdfResult = true;
                                                }


                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (SelectReceptFourn.quantite < 0)
                                        {
                                            if (SelectReceptFourn.déstockage == true)
                                            {
                                                proxy.Deleteam(SelectReceptFourn);
                                                AMResult = true;
                                                selectprodf.quantite = selectprodf.quantite - SelectReceptFourn.quantite;
                                                proxy.UpdateProdf(selectprodf);
                                                ProdfResult = true;
                                            }
                                        }
                                    }



                                    if (AMResult && ProdfResult)
                                    {
                                        ts.Complete();
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                proxy.AjouterAmsRefresh();
                                proxy.AjouterProdfRefresh();

                            }
                            catch (TransactionAbortedException ex)
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

                if (MemberUser.ImpressionAms== true)
                {
                    if (AMSDataGrid.SelectedItem != null)
                    {
                        SVC.am SelectMedecin = AMSDataGrid.SelectedItem as SVC.am;
                        List<SVC.am> listdepaief = new List<SVC.am>();
                        listdepaief.Add(SelectMedecin);
                        ImpressionAMS clsho = new ImpressionAMS(proxy, listdepaief);
                        clsho.Show();
                    }
                    else
                    {
                        List<SVC.am> listdepaief = AMSDataGrid.Items.OfType<SVC.am>().ToList();
                        ImpressionAMS clsho = new ImpressionAMS(proxy, listdepaief);
                        clsho.Show();
                        /*  if (txtRechercheMotif.Text != "")
                          {
                              var test = AMSDataGrid.ItemsSource as IEnumerable<SVC.am>;

                              var t = (from e1 in test

                                       where e1.observ.ToUpper().Contains(txtRechercheMotif.Text.ToUpper())

                                       select e1);



                              ImpressionAMS clsho = new ImpressionAMS(proxy, t.ToList());
                              clsho.Show();
                          }
                          else
                          {
                             // List<SVC.am> test = AMSDataGrid.ItemsSource as List<SVC.am>;

                              var test = AMSDataGrid.ItemsSource as IEnumerable<SVC.am>;
                              ImpressionAMS clsho = new ImpressionAMS(proxy, test.ToList());
                              clsho.Show();

                          }*/


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
            if (txtRechercheMotif.Text.Trim() != "")
            {


                var t = (from e1 in proxy.GetAllam()

                         where e1.dates >= DateSaisieDébut.SelectedDate && e1.dates <= DateSaisieFin.SelectedDate && e1.observ.ToUpper().Contains(txtRechercheMotif.Text.ToUpper())

                         select e1);
                AMSDataGrid.ItemsSource = t;

                txtAMS.Text = Convert.ToString((t).AsEnumerable().Sum(o => o.quantite * o.previent));
            }
            else
            {


                var t = (from e1 in proxy.GetAllam()

                         where e1.dates >= DateSaisieDébut.SelectedDate && e1.dates <= DateSaisieFin.SelectedDate

                         select e1);
                AMSDataGrid.ItemsSource = t;
                txtAMS.Text = Convert.ToString((t).AsEnumerable().Sum(o => o.quantite * o.previent));
                    filtre = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btndeFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            DateSaisieFin.SelectedDate = DateTime.Now;
            DateSaisieDébut.SelectedDate = new DateTime(2018, 01, 01);

            AMSDataGrid.ItemsSource = proxy.GetAllam();
            txtAMS.Text = Convert.ToString(((proxy.GetAllam()).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
         

        private void chstockdisponible_Checked(object sender, RoutedEventArgs e)
        {
          /*  try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (chstockdisponible.IsChecked == true)
                {
                    //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Prodf temp = item as SVC.Prodf;
                        return temp.quantite > 0;


                    };
                    txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf().Where(n=>n.quantite>0)).AsEnumerable().Sum(o => o.quantite * o.previent)));

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }*/


        }

         

        private void FournisseurCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try { 
            DateSaisieFin.SelectedDate = DateTime.Now;
            DateSaisieDébut.SelectedDate = new DateTime(2018, 01, 01);
            PatientDataGrid.ItemsSource = proxy.GetAllProdf();
            FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
                FamilleCombo.ItemsSource=proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf()).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimerStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (MemberUser.ImpressionAms == true)
                {
                    if (PatientDataGrid.SelectedItem != null)
                    {
                        SVC.Prodf SelectMedecin = PatientDataGrid.SelectedItem as SVC.Prodf;
                        List<SVC.Prodf> listdepaief = new List<SVC.Prodf>();

                        listdepaief.Add(SelectMedecin);

                        ImpressionStock clsho = new ImpressionStock(proxy, listdepaief);
                        clsho.Show();
                    }
                    else
                    {
                        List<SVC.Prodf> listdepaief = PatientDataGrid.Items.OfType<SVC.Prodf>().ToList();
                        //List<SVC.Prodf> listdepaief = new List<SVC.Prodf>();

                      //  listdepaief.Add(SelectMedecin);

                        ImpressionStock clsho = new ImpressionStock(proxy, listdepaief);
                        clsho.Show();
                    }
                 /*   else
                    {
                        if (txtRecherche.Text != "" && FournisseurCombo.SelectedItem != null)
                        {
                            List<SVC.Prodf> test = PatientDataGrid.ItemsSource as List<SVC.Prodf>;
                            SVC.Fourn tfourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                            IEnumerable<SVC.Prodf> t;
                            if (chstockdisponible.IsChecked == true)
                            {
                                t = (from e1 in test

                                     where e1.design.ToUpper().Contains(txtRecherche.Text.ToUpper()) && e1.cf == tfourn.Id && e1.quantite > 0

                                     select e1);
                            }
                            else
                            {
                                t = (from e1 in test

                                     where e1.design.ToUpper().Contains(txtRecherche.Text.ToUpper()) && e1.cf == tfourn.Id

                                     select e1);
                            }


                            ImpressionStock clsho = new ImpressionStock(proxy, t.ToList());
                            clsho.Show();
                        }
                        else
                        {
                            if (txtRecherche.Text != "" && FournisseurCombo.SelectedItem == null)
                            {
                                IEnumerable<SVC.Prodf> t;
                                List<SVC.Prodf> test = PatientDataGrid.ItemsSource as List<SVC.Prodf>;
                                if (chstockdisponible.IsChecked == true)
                                {
                                    t = (from e1 in test

                                         where e1.design.ToUpper().Contains(txtRecherche.Text.ToUpper()) && e1.quantite > 0

                                         select e1);
                                }
                                else
                                {
                                    t = (from e1 in test

                                         where e1.design.ToUpper().Contains(txtRecherche.Text.ToUpper())

                                         select e1);
                                }


                                ImpressionStock clsho = new ImpressionStock(proxy, t.ToList());
                                clsho.Show();
                            }
                            else
                            {
                                if (txtRecherche.Text == "" && FournisseurCombo.SelectedItem == null)
                                {
                                    List<SVC.Prodf> test = new List<SVC.Prodf>();
                                    if (chstockdisponible.IsChecked == true)
                                    {
                                        List<SVC.Prodf> t1 = PatientDataGrid.ItemsSource as List<SVC.Prodf>;
                                        test = t1.Where(n => n.quantite > 0).ToList();
                                    }
                                    else
                                    {
                                        test = PatientDataGrid.ItemsSource as List<SVC.Prodf>;

                                    }

                                    ImpressionStock clsho = new ImpressionStock(proxy, test.ToList());
                                    clsho.Show();
                                }
                                else
                                {
                                    if (txtRecherche.Text == "" && FournisseurCombo.SelectedItem != null)
                                    {
                                        List<SVC.Prodf> test = PatientDataGrid.ItemsSource as List<SVC.Prodf>;
                                        SVC.Fourn tfourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                                        IEnumerable<SVC.Prodf> t;
                                        if (chstockdisponible.IsChecked == true)
                                        {
                                            t = (from e1 in test

                                                 where  e1.cf == tfourn.Id && e1.quantite > 0

                                                 select e1);
                                        }
                                        else
                                        {
                                            t = (from e1 in test

                                                 where  e1.cf == tfourn.Id

                                                 select e1);
                                        }


                                        ImpressionStock clsho = new ImpressionStock(proxy, t.ToList());
                                        clsho.Show();
                                    }
                                }

                            }


                        }
                    }*/
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void AMSDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { 
            AMSDataGrid.ItemsSource = proxy.GetAllam();
            DateSaisieDébut.SelectedDate = DateTime.Now.Date;
            DateSaisieFin.SelectedDate = DateTime.Now.Date;
            txtRechercheMotif.Text = "";
            txtRechercheDesign.Text = "";
            txtAMS.Text = Convert.ToString((proxy.GetAllam()).AsEnumerable().Sum(o => o.quantite * o.previent));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            { 
            PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(x => x.quantite > 0);
            FamilleCombo.SelectedItem = null;
            FournisseurCombo.SelectedItem = null;
            chstockdisponible.IsChecked = false;
            txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf()).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chstockdisponible_Unchecked(object sender, RoutedEventArgs e)
        {
            /*
           
            try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (chstockdisponible.IsChecked == false)
                {
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Prodf temp = item as SVC.Prodf;
                        return temp.quantite >= 0 || temp.quantite <= 0;


                    };
                    txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf())).AsEnumerable().Sum(o => o.quantite * o.previent));

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }*/
        }

       

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null && MemberUser.ModificationFichier==true)
                {
                    var selectedprodf = PatientDataGrid.SelectedItem as SVC.Prodf;
                    ModiTarif cl = new ModiTarif(proxy, MemberUser, callback, selectedprodf);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

         


        

        

        private void btnFilterStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool fournisseur, famille= false;
                int quantite = 0;
                if (FournisseurCombo.SelectedItem!=null)
                {
                    fournisseur = true;
                }
                else
                {
                    fournisseur = false;
                }
                if (FamilleCombo.SelectedItem != null)
                {
                    famille = true;
                }
                else
                {
                    famille = false;
                }
                if (chstockdisponible.IsChecked==true)
                {
                    quantite=1;
                }
                else
                {
                    quantite = 0;
                }

               if (FournisseurCombo.SelectedItem!=null)
                {
                    SVC.Fourn selectedfourn = FournisseurCombo.SelectedItem as SVC.Fourn;

                    if (famille==true)
                    {
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(n => n.quantite >= quantite && n.IdFamille == selectedfamille.Id && n.cf == selectedfourn.Id); 
                    }
                    else
                    {
                        PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(n => n.quantite >= quantite && n.cf == selectedfourn.Id);
                    }
                }
                else
                {

                    if (famille == true)
                    {
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(n => n.quantite >= quantite && n.IdFamille == selectedfamille.Id );
                    }
                    else
                    {
                        PatientDataGrid.ItemsSource = proxy.GetAllProdf().Where(n => n.quantite >= quantite );
                    }
                }
                var test = PatientDataGrid.Items.OfType<SVC.Prodf>();
                txtFournisseur.Text = Convert.ToString(test.AsEnumerable().Sum(o => o.quantite * o.previent));

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

      

        private void FournisseurCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FournisseurCombo.SelectedItem != null)
                {
                    SVC.Fourn t = FournisseurCombo.SelectedItem as SVC.Fourn;

                    var filter = (t.raison).ToString();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    if (filter == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            SVC.Prodf p = o as SVC.Prodf;
                            if (t.raison == "txtId")
                                return (p.fourn == filter);
                            return (p.fourn.ToUpper().Contains(filter.ToUpper()));
                        };
                        txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf().Where(n => n.fourn.Contains(filter.ToUpper()))).AsEnumerable().Sum(o => o.quantite * o.previent)));

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void FamilleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                //  ;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Prodf p = o as SVC.Prodf;
                        if (t.Name == "txtId")
                            return (p.design == filter);
                        return (p.design.ToUpper().Contains(filter.ToUpper()));


                    };
                    var test = PatientDataGrid.Items.OfType<SVC.Prodf>();
                    txtFournisseur.Text = Convert.ToString(test.AsEnumerable().Sum(o => o.quantite * o.previent));

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRechercheMotif_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtRechercheMotif.Text != null)
                {
                    TextBox t = (TextBox)sender;
                    string filter = t.Text;
                    ICollectionView cv = CollectionViewSource.GetDefaultView(AMSDataGrid.ItemsSource);
                    if (filter == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            SVC.am p = o as SVC.am;
                            if (t.Name == "txtId")
                                return (p.observ == filter);
                            return (p.observ.ToUpper().Contains(filter.ToUpper()));


                        };
                        var test = AMSDataGrid.Items.OfType<SVC.am>();
                        txtAMS.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.quantite * o.previent));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRechercheDesign_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtRechercheDesign.Text != null)
                {
                    TextBox t = (TextBox)sender;
                    string filter = t.Text;
                    ICollectionView cv = CollectionViewSource.GetDefaultView(AMSDataGrid.ItemsSource);
                    if (filter == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            SVC.am p = o as SVC.am;
                            if (t.Name == "txtId")
                                return (p.observ == filter);
                            return (p.design.ToUpper().Contains(filter.ToUpper()));


                        };
                        var test = AMSDataGrid.Items.OfType<SVC.am>();
                        txtAMS.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.quantite * o.previent));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
