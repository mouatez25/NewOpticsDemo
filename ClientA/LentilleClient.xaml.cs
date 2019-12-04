
using Microsoft.Reporting.WinForms;
using NewOptics.Administrateur;
using NewOptics.Caisse;
using NewOptics.Commande;
using NewOptics.Stock;
using Nut;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
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

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for LentilleClient.xaml
    /// </summary>
    public partial class LentilleClient :Window
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;
        SVC.ClientV Clientvv;
        bool factureopern = false;
        SVC.F1 ancienneF1;
        SVC.F1 selectedF1;
        SVC.Param selectedparam;
        SVC.F1 nouveauF1;
        bool facturenew = false;
        bool facturemodif = false;
        private delegate void FaultedInvokerLentilleClient();
        List<SVC.Facture> factureselectedl;
        List<SVC.Facture> anciennefactureselectedl;
        List<SVC.Prodf> produitavendre = new List<SVC.Prodf>();
        Window dialog1;
        int documenttype = 0;
       
        bool fermer = false;
      
        SVC.LentilleClient LentilleClass;
   
        bool nouvellelentille = false;
        bool anciennelentille = false;
       
       
    
        bool Lentilleversementzero = false;
        int interfacefacturation = 0;
        Microsoft.Win32.OpenFileDialog op = null;
       
       
        SVC.Prodf selectedtropuvé;
        SVC.Produit selectedtropuvéNONSTOCK;
       
     
        int interfaceimpressionlentille = 0;
       bool visualiserLentille = false;
        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        bool visualiserFacture = false;
        int interfaceimpressionfacture = 0;
        bool tunisie = false;
        bool facturenonstock = false;
        public LentilleClient(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu, SVC.ClientV clientrecu, SVC.LentilleClient LENTILLERECU)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                callback = callbackrecu;
                memberuser = memberrecu;
                Clientvv = clientrecu;
                selectedparam = proxy.GetAllParamétre();
                /********************************Lentille**************************************/
                LentilleDatagrid.ItemsSource = proxy.GetAllLentilleClientbycode(Clientvv.Id).OrderBy(n => n.Date);

                if (LENTILLERECU != null)
                {
                    LentilleClass = LENTILLERECU;
                LentilleDatagrid.SelectedItem = LENTILLERECU;
                existelentille(LentilleClass);
                }
                tsdfsdfgendsft.IsSelected = true;
                if (selectedparam.AffichPrixAchatVente == true)
                {
                    NomenclatureProduit.Columns[3].Visibility = Visibility.Visible;
                    ReceptDatagrid.Columns[9].Visibility = Visibility.Visible;
                }
                else
                {
                    NomenclatureProduit.Columns[3].Visibility = Visibility.Collapsed;
                    ReceptDatagrid.Columns[9].Visibility = Visibility.Collapsed;

                }
                if (selectedparam.ModiPrix == true)
                {
                    ReceptDatagrid.Columns[2].IsReadOnly = false;
                }
                else
                {
                    ReceptDatagrid.Columns[2].IsReadOnly = true;
                }
                if (selectedparam.modidate == true)
                {
                    txtDateOper.IsEnabled = true;
                }
                if (selectedparam.affiben == true)
                {
                    Bénéfice.Visibility = Visibility.Visible;
                    Bénéficemont.Visibility = Visibility.Visible;

                }
                /********************************Facturation*****************///////
                ListeDesDocuments.ItemsSource = proxy.GetAllF1Bycode(Clientvv.Id).Where(n => n.nfact.Substring(0, 2) != "Co").OrderBy(n => n.date);
                callbackrecu.InsertProdfListCallbackevent += new ICallback.CallbackEventHandler10(callbackrecuprodf_Refresh);
                callbackrecu.InsertProdfCallbackEvent += new ICallback.CallbackEventHandler21(callbackrecuprodf_Refresh);
                /****************************Historique************************************************/
                callbackrecu.InsertF1CallbackEvent += new ICallback.CallbackEventHandler6(callbackrecu_Refresh);

                callbackrecu.InsertFactureVentefListCallbackevent += new ICallback.CallbackEventHandler11(callbackrecuFactureList_Refresh);
                callbackrecu.InsertF1ListCallbackEvent += new ICallback.CallbackEventHandler27(callbackrecuF1List_Refresh);
                /*****************************************************************/
                /********************************************************/
                callbackrecu.InsertLentilleClientCallbackevent += new ICallback.CallbackEventHandler35(callbackreculentille_Refresh);
                /****************************************************************************************/
            }
            catch (Exception ex)
            {
                HandleProxy();
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
                        if (listmembership.codeclient == Clientvv.Id)
                        {

                            var LISTITEM22 = ListeDesDocuments.ItemsSource as IEnumerable<SVC.F1>;
                            List<SVC.F1> LISTITEM2 = LISTITEM22.ToList();
                            if (listmembership.cle == Clientvv.cle || listmembership.codeclient == Clientvv.Id)
                            {





                                var objectmodifed = LISTITEM2.Find(n => n.Id == listmembership.Id);
                                //objectmodifed = listmembership;
                                var index = LISTITEM2.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM2[index] = listmembership;




                            }

                            ListeDesDocuments.ItemsSource = LISTITEM2.OrderBy(r => r.date);


                        }
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
        void callbackrecuFactureList_Refresh(object source, CallbackEventInsertListfacturevente e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshFactureListVente(e.clientleav);
            }));
        }
        public void AddRefreshFactureListVente(List<SVC.Facture> memberfacture)
        {

            try
            {
                if (memberfacture.First().codeclient == Clientvv.Id && nouveauF1 != null)
                {
                    /*******************************************************************************************/
                    if (facturemodif == true && facturenew == false && nouveauF1.cle == memberfacture.First().cle)
                    {

                        ReceptDatagrid.ItemsSource = memberfacture;

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }



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
                //   MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("i'm here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                /*******************************************************************************************/
                if (listmembership.codeclient == Clientvv.Id || listmembership.cle == Clientvv.cle)
                {
                    /*****************facturation grid************/
                    var LISTITEM22 = ListeDesDocuments.ItemsSource as IEnumerable<SVC.F1>;
                    List<SVC.F1> LISTITEM2 = LISTITEM22.ToList();
                    if (listmembership.cle == Clientvv.cle || listmembership.codeclient == Clientvv.Id)
                    {
                        if (oper == 1)
                        {
                            LISTITEM2.Add(listmembership);

                        }
                        else
                        {
                            if (oper == 2)
                            {




                                var objectmodifed = LISTITEM2.Find(n => n.Id == listmembership.Id);
                                //objectmodifed = listmembership;
                                var index = LISTITEM2.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM2[index] = listmembership;

                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    var deleterendez = LISTITEM2.Where(n => n.Id == listmembership.Id).First();
                                    LISTITEM2.Remove(deleterendez);
                                }
                            }
                        }

                    }

                    ListeDesDocuments.ItemsSource = LISTITEM2.OrderBy(r => r.date);
                    if (oper == 1)
                    {
                        ListeDesDocuments.SelectedItem = listmembership;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }



        }
        void callbackrecuprodf_Refresh(object source, CallbackEventInsertListProdf e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshProdfListe(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshProdfListe(List<SVC.Prodf> listMembershipOpticlist, int oper)
        {
            try
            {
                if (factureopern == true)
                {
                    var LISTITEM1 = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;


                    List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();

                    foreach (var listMembershipOptic in listMembershipOpticlist)
                    {

                        if (oper == 1)
                        {
                            LISTITEM.Add(listMembershipOptic);
                        }
                        else
                        {
                            if (oper == 2 || oper == 4)
                            {



                                var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                                var index = LISTITEM.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM[index] = listMembershipOptic;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                                    LISTITEM.Remove(deleterendez);
                                }
                            }
                        }

                        NomenclatureProduit.ItemsSource = LISTITEM;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        void callbackrecuprodf_Refresh(object source, CallbackEventInsertProdf e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshProdf(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshProdf(SVC.Prodf listMembershipOptic, int oper)
        {
            try
            {
                if (factureopern == true)
                {
                    var LISTITEM1 = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
                    List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();

                    if (oper == 1)
                    {
                        LISTITEM.Add(listMembershipOptic);
                    }
                    else
                    {
                        if (oper == 2)
                        {

                            var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                            var index = LISTITEM.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEM[index] = listMembershipOptic;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                                LISTITEM.Remove(deleterendez);
                            }
                        }
                    }

                    NomenclatureProduit.ItemsSource = LISTITEM;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        void existelentille(SVC.LentilleClient LentilleClass)
        {
            try
            {

                GridLentille.DataContext = LentilleClass;
                GridLentille.IsEnabled = true;

                nouvellelentille = false;
                anciennelentille = true;

                if (LentilleClass.Encaissé != 0)
                {
                    Lentilleversementzero = true;
                }
                else
                {
                    Lentilleversementzero = false;
                }
                /************************************************/
                if (LentilleClass.StatutVente == false)
                {
                    btnventeLentille.IsEnabled = true;
                    txtMontantTotalENCLentille.IsEnabled = true;
               
                }
                else
                {
                    txtMontantTotalENCLentille.IsEnabled = false;
                    btnventeLentille.IsEnabled = false;
                }

                if (LentilleClass.StatutDevis == true && LentilleClass.StatutVente == false)
                {
                    TxtStatutGlobalLentille.Content = "Devis";

                    TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.PaleVioletRed;
                }
                else
                {
                    if (LentilleClass.StatutDevis == true && LentilleClass.StatutVente == true)
                    {
                        TxtStatutGlobalLentille.Content = "Vente validée";
                        TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.LightGreen;
                    }
                }






                /**************************************************************/




            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void callbackreculentille_Refresh(object source, CallbackEventInsertLentilleClient e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshLentilleClient(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshLentilleClient(SVC.LentilleClient listmembership, int oper)
        {
            try
            {

                var LISTITEM1 = LentilleDatagrid.ItemsSource as IEnumerable<SVC.LentilleClient>;
                List<SVC.LentilleClient> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                        //   var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //  objectmodifed = listmembership;

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

                LentilleDatagrid.ItemsSource = LISTITEM;


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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerLentilleClient(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerLentilleClient(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void btnventeLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dialog1 = new Window();
                dialog1.Title = "Veuillez choisir un document";
                dialog1.Template = Resources["template4"] as ControlTemplate;

                interfacefacturation = 2;
                dialog1.ResizeMode = ResizeMode.NoResize;
                dialog1.Width = 350;
                dialog1.Height = 300;
                dialog1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog1.ShowDialog();


                facturenew = true;
                facturemodif = false;


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void btnNewLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationDossierClient == true)
                {
                    LentilleClass = new SVC.LentilleClient
                    {
                        Date = DateTime.Now,
                        UserName = memberuser.Username,
                        Délivre = false,
                        RaisonClient = Clientvv.Raison,
                        StatutDevis = true,
                        StatutVente = false,
                        IdClient = Clientvv.Id,
                        DroiteCylindrePlus = true,
                        DroiteCylindreMoin = false,
                        GaucheCylindreMoin = false,
                        GaucheCylindrePlus = true,
                        AccessoiresQuantite1 = 0,
                        AccessoiresQuantite2 = 0,
                        AccessoiresPrix1 = 0,
                        AccessoiresPrix2 = 0,
                        Encaissé = 0,
                        Reste = 0,
                        Dates = DateTime.Now,
                        DroitQuantiteLentille = 0,
                        DroitPrixLentille = 0,
                        GauchePrixLentille = 0,
                        GaucheQuantiteLentille = 0,
                        MontantTotal = 0,
                        TypeDevisite = "Inconnu",
                        Dioptrie = true,
                        mm = false,
                        Remise = 0,
                        TypeOD = "",
                        TypeOG = "",
                        DiametreOD = "",
                        DiametreOG = "",
                        RayonOD = "",
                        RayonOG = "",

                    };
                    btnventeLentille.IsEnabled = false;
                    GridLentille.DataContext = LentilleClass;
                    GridLentille.IsEnabled = true;
                    TxtStatutGlobalLentille.Content = "Devis";
                    TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    nouvellelentille = true;
                    anciennelentille = false;
                    txtMontantTotalENCLentille.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void btnSuppLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LentilleDatagrid.SelectedItem != null && memberuser.SupressionDossierClient == true)
                {
                    var selectedlentille = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                    if (selectedlentille.StatutVente == true)
                    {
                        bool sucees = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            proxy.DeleteLentilleClient(selectedlentille);
                            ts.Complete();
                            sucees = true;
                        }
                        if (sucees == true)
                        {
                            proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                    else
                    {
                        if (selectedlentille.StatutVente == false)
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez tout d'abord supprimer le document de vente associé", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnEditLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LentilleDatagrid.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {
                    LentilleClass = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                    GridLentille.DataContext = LentilleClass;
                    GridLentille.IsEnabled = true;

                    nouvellelentille = false;
                    anciennelentille = true;

                    if (LentilleClass.Encaissé != 0)
                    {
                        Lentilleversementzero = true;
                    }
                    else
                    {
                        Lentilleversementzero = false;
                    }
                    /************************************************/
                    if (LentilleClass.StatutVente == false)
                    {
                        btnventeLentille.IsEnabled = true;
                        txtMontantTotalENCLentille.IsEnabled = true;
                    }
                    else
                    {
                        btnventeLentille.IsEnabled = false;
                        txtMontantTotalENCLentille.IsEnabled = false;
                    }

                    if (LentilleClass.StatutDevis == true && LentilleClass.StatutVente == false)
                    {
                        TxtStatutGlobalLentille.Content = "Devis";

                        TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (LentilleClass.StatutDevis == true && LentilleClass.StatutVente == true)
                        {
                            TxtStatutGlobalLentille.Content = "Vente validée";
                            TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }






                    /**************************************************************/



                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
     



        private void Ficheatelier_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionlentille = 1;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void Reçu_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionlentille = 2;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void atelierreçu_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionlentille = 3;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void _double_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionlentille = 4;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chimpression_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                visualiserLentille = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chimpression_Unchecked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                visualiserLentille = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void RunRecu(List<SVC.Depeiment> listerecu, List<SVC.F1> listevisite)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.RecuDePaiement), false);
                // LocalReport reportViewer1 = new LocalReport();
                reportViewer1.LoadReportDefinition(MyRptStream);


                //reportViewer1.LoadReportDefinition(MyRptStream);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                rds.Value = listerecu;
                reportViewer1.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                SVC.Param paramlocal = (proxy.GetAllParamétre());
                paramlocal.logo = "D/Logo.jpg";
                selpara.Add((paramlocal));


                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", listevisite));

                if (proxy.GetAllParamétre().logo != "")
                {

                    if (proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().logo.ToString()) == true)
                    {
                        reportViewer1.EnableExternalImages = true;

                        String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                        ReportParameter paramLogo = new ReportParameter();
                        paramLogo.Name = "ImagePath";
                        //  paramLogo.Values.Add(@"file:///C:\Logo.png");
                        paramLogo.Values.Add(@"file:///" + photolocation);
                        reportViewer1.SetParameters(paramLogo);
                        // reportViewer1.LocalReport.SetParameters(parameter);
                    }

                }
                reportViewer1.Refresh();

                Export(reportViewer1);
                Print(true);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private Stream CreateStream(string name,
string fileNameExtension, Encoding encoding,
string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        /* private void Print()
         {
             try
             {
                 if (m_streams == null || m_streams.Count == 0)
                     throw new Exception("Error: no stream to print.");
                 PrintDocument printDoc = new PrintDocument();
                 if (!printDoc.PrinterSettings.IsValid)
                 {
                     throw new Exception("Error: cannot find the default printer.");
                 }
                 else
                 {
                     printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                     m_currentPageIndex = 0;
                     printDoc.DocumentName = Clientvv.Raison;
                     printDoc.Print();
                 }
             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
        private void Print(bool landscape)
        {
            try
            {
                if (m_streams == null || m_streams.Count == 0)
                    throw new Exception("Error: no stream to print.");
                PrintDocument printdoc = new PrintDocument();
                if (!printdoc.PrinterSettings.IsValid)
                {
                    throw new Exception("Error: cannot find the default printer.");
                }
                else
                {
                    printdoc.DefaultPageSettings.Landscape = landscape;
                    printdoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    m_currentPageIndex = 0;
                    printdoc.DocumentName = Clientvv.Raison;
                    printdoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Metafile pageImage = new
                   Metafile(m_streams[m_currentPageIndex]);

                // Adjust rectangular area with printer margins.
                System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                    ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                    ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                    ev.PageBounds.Width,
                    ev.PageBounds.Height);

                // Draw a white background for the report
                ev.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), adjustedRect);

                // Draw the report content
                ev.Graphics.DrawImage(pageImage, adjustedRect);

                // Prepare for the next page. Make sure we haven't hit the end.
                m_currentPageIndex++;
                ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void Export(LocalReport report)
        {
            try
            {

                /* string deviceInfo =
                   @"<DeviceInfo>
                 <OutputFormat>EMF</OutputFormat>
                  <PageWidth>& w / 100 & "in</PageWidth>
                 <PageHeight>& h / 100 & </PageHeight>
                 <MarginTop>0.cm</MarginTop>
                 <MarginLeft>0in</MarginLeft>
                 <MarginRight>0in</MarginRight>
                 <MarginBottom>0in</MarginBottom>
             </DeviceInfo>";*/

                string deviceInfo =
                     @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                 <PageWidth>5.87in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.5cm</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0.39in</MarginBottom>
            </DeviceInfo>";


                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream,
                   out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void ExportLand(LocalReport report)
        {
            try
            {

                /* string deviceInfo =
                   @"<DeviceInfo>
                 <OutputFormat>EMF</OutputFormat>
                  <PageWidth>& w / 100 & "in</PageWidth>
                 <PageHeight>& h / 100 & </PageHeight>
                 <MarginTop>0.cm</MarginTop>
                 <MarginLeft>0in</MarginLeft>
                 <MarginRight>0in</MarginRight>
                 <MarginBottom>0in</MarginBottom>
             </DeviceInfo>";*/

                string deviceInfo =
                     @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                 <PageWidth>8.5inin</PageWidth>
                <PageHeight>5.87in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";


                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream,
                   out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ExportA4(LocalReport report)
        {
            try
            {
                string deviceInfo =
                  @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>5,708661in</PageWidth>
                <PageHeight>8,26772in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream,
                   out warnings);
                foreach (Stream stream in m_streams)
                    stream.Position = 0;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnCreerImpression_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (visualiserLentille == true)
                {
                    switch (interfaceimpressionlentille)
                    {
                        case 1:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selectedmont = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                List<SVC.LentilleClient> mm = new List<SVC.LentilleClient>();
                                mm.Add(selectedmont);
                                List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                clientvvv.Add(Clientvv);
                                ImpressionLentille cl = new ImpressionLentille(proxy, mm, clientvvv, interfaceimpressionlentille);
                                cl.Show();
                                dialog1.Close();
                                interfaceimpressionlentille = 0;
                                visualiserLentille = false;
                            }
                            break;
                        case 4:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selectedmont = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                List<SVC.LentilleClient> mm = new List<SVC.LentilleClient>();
                                mm.Add(selectedmont);
                                List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                clientvvv.Add(Clientvv);
                                ImpressionLentille cl = new ImpressionLentille(proxy, mm, clientvvv, interfaceimpressionlentille);
                                cl.Show();

                                dialog1.Close();
                                interfaceimpressionlentille = 0;
                                visualiserLentille = false;
                            }
                            break;
                        case 3:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selecedmonture = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == true)
                                {
                                    List<SVC.LentilleClient> listmonture = new List<SVC.LentilleClient>();
                                    listmonture.Add(selecedmonture);
                                    var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
                                    if (VisiteApayerexiste == true)
                                    {
                                        SVC.F1 VisiteApayer = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).First();
                                        var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            /*  List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                              List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                              listedepaiem.Add(dpf.First()); */

                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());

                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            listevisite.Add(VisiteApayer);
                                            List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                                            listclient.Add(Clientvv);
                                            ImpressionLentilleRecu cl = new ImpressionLentilleRecu(proxy, listmonture, listclient, listedepaiem, listevisite);
                                            cl.Show();
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                    }
                                }
                                else
                                {
                                    if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == false)
                                    {
                                        List<SVC.LentilleClient> listmonture = new List<SVC.LentilleClient>();
                                        listmonture.Add(selecedmonture);

                                        var dpfexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            /*  List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                              List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                              listedepaiem.Add(dpf.First()); */

                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(selecedmonture.Cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());

                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            SVC.F1 VisiteApayer = new SVC.F1
                                            {
                                                codeclient = selecedmonture.IdClient,
                                                raison = selecedmonture.RaisonClient,
                                            };
                                            listevisite.Add(VisiteApayer);
                                            List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                                            listclient.Add(Clientvv);
                                            ImpressionLentilleRecu cl = new ImpressionLentilleRecu(proxy, listmonture, listclient, listedepaiem, listevisite);
                                            cl.Show();
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }

                                    }
                                }

                            }
                            break;
                        case 2:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selecedmonture = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == true)
                                {
                                    var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
                                    if (VisiteApayerexiste == true)
                                    {
                                        SVC.F1 VisiteApayer = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).First();
                                        var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            //  var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());
                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            listevisite.Add(VisiteApayer);
                                            ImpressionRecu cl = new ImpressionRecu(proxy, listedepaiem, listevisite);
                                            cl.Show();
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                    }

                                }
                                else
                                {
                                    if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == false)
                                    {

                                        var dpfexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            //  var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(selecedmonture.Cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());
                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            SVC.F1 VisiteApayer = new SVC.F1
                                            {
                                                codeclient = selecedmonture.IdClient,
                                                raison = selecedmonture.RaisonClient,
                                            };
                                            listevisite.Add(VisiteApayer);
                                            ImpressionRecu cl = new ImpressionRecu(proxy, listedepaiem, listevisite);
                                            cl.Show();
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                    }
                                }
                            }

                            break;

                    }
                }
                else
                {
                    switch (interfaceimpressionlentille)
                    {
                        case 1:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selectedmont = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                List<SVC.LentilleClient> mm = new List<SVC.LentilleClient>();
                                mm.Add(selectedmont);
                                List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                clientvvv.Add(Clientvv);
                                RunAtelierLentille(mm, clientvvv);
                                dialog1.Close();
                                interfaceimpressionlentille = 0;
                                visualiserLentille = false;
                            }
                            break;
                        case 2:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selecedmonture = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == true)
                                {
                                    var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
                                    if (VisiteApayerexiste == true)
                                    {
                                        SVC.F1 VisiteApayer = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).First();
                                        var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            //  var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());
                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            listevisite.Add(VisiteApayer);
                                            RunRecu(listedepaiem, listevisite);
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                    }
                                }
                                else
                                {
                                    if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == false)
                                    {
                                        var VisiteApayerexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                        if (VisiteApayerexiste == true)
                                        {

                                            var dpfexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                            if (dpfexiste == true)
                                            {
                                                //  var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                                List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(selecedmonture.Cle);
                                                List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                                dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                                listedepaiem.Add(dpf.Last());
                                                List<SVC.F1> listevisite = new List<SVC.F1>();
                                                SVC.F1 VisiteApayer = new SVC.F1
                                                {
                                                    codeclient = selecedmonture.IdClient,
                                                    raison = selecedmonture.RaisonClient,
                                                };
                                                listevisite.Add(VisiteApayer);
                                                RunRecu(listedepaiem, listevisite);
                                                dialog1.Close();
                                                interfaceimpressionlentille = 0;
                                                visualiserLentille = false;
                                            }
                                            else
                                            {
                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                            }
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }
                                    }
                                }
                            }
                            break;
                        case 3:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selecedmonture = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == true)
                                {
                                    List<SVC.LentilleClient> listmonture = new List<SVC.LentilleClient>();
                                    listmonture.Add(selecedmonture);
                                    var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
                                    if (VisiteApayerexiste == true)
                                    {
                                        SVC.F1 VisiteApayer = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).First();
                                        var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            /*  List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                              List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                              listedepaiem.Add(dpf.First()); */

                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());

                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            listevisite.Add(VisiteApayer);
                                            List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                                            listclient.Add(Clientvv);
                                            RunLentillePaiement(listmonture, listclient, listedepaiem, listevisite);
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                    }

                                }
                                else
                                {
                                    if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == false)
                                    {
                                        List<SVC.LentilleClient> listmonture = new List<SVC.LentilleClient>();
                                        listmonture.Add(selecedmonture);

                                        var dpfexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            /*  List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                              List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                              listedepaiem.Add(dpf.First()); */

                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(selecedmonture.Cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());

                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            SVC.F1 VisiteApayer = new SVC.F1
                                            {
                                                codeclient = selecedmonture.IdClient,
                                                raison = selecedmonture.RaisonClient,
                                            };
                                            listevisite.Add(VisiteApayer);
                                            List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                                            listclient.Add(Clientvv);
                                            RunLentillePaiement(listmonture, listclient, listedepaiem, listevisite);
                                            dialog1.Close();
                                            interfaceimpressionlentille = 0;
                                            visualiserLentille = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }

                                    }
                                }
                            }
                            break;
                        case 4:
                            if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.LentilleClient selectedmont = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                                List<SVC.LentilleClient> mm = new List<SVC.LentilleClient>();
                                mm.Add(selectedmont);
                                List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                clientvvv.Add(Clientvv);
                                RunAtelierLentilleDouble(mm, clientvvv);
                                dialog1.Close();
                                interfaceimpressionlentille = 0;
                                visualiserLentille = false;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void RunAtelierLentilleDouble(List<SVC.LentilleClient> monture, List<SVC.ClientV> CLIENRECU)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.LentilleDouble), false);
                // LocalReport reportViewer1 = new LocalReport();
                reportViewer1.LoadReportDefinition(MyRptStream);


                //reportViewer1.LoadReportDefinition(MyRptStream);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                rds.Value = monture;
                reportViewer1.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                SVC.Param paramlocal = (proxy.GetAllParamétre());
                paramlocal.logo = "D/Logo.jpg";
                selpara.Add((paramlocal));


                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", selpara));
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", CLIENRECU));

                if (proxy.GetAllParamétre().logo != "")
                {

                    if (proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().logo.ToString()) == true)
                    {
                        reportViewer1.EnableExternalImages = true;

                        String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                        ReportParameter paramLogo = new ReportParameter();
                        paramLogo.Name = "ImagePath";
                        //  paramLogo.Values.Add(@"file:///C:\Logo.png");
                        paramLogo.Values.Add(@"file:///" + photolocation);
                        reportViewer1.SetParameters(paramLogo);
                        // reportViewer1.LocalReport.SetParameters(parameter);
                    }

                }
                reportViewer1.Refresh();

                ExportA4(reportViewer1);
                Print(false);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void RunAtelierLentille(List<SVC.LentilleClient> monture, List<SVC.ClientV> CLIENRECU)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.FicheAtelierLentille), false);
                // LocalReport reportViewer1 = new LocalReport();
                reportViewer1.LoadReportDefinition(MyRptStream);


                //reportViewer1.LoadReportDefinition(MyRptStream);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                rds.Value = monture;
                reportViewer1.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                SVC.Param paramlocal = (proxy.GetAllParamétre());
                paramlocal.logo = "D/Logo.jpg";
                selpara.Add((paramlocal));


                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", selpara));
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", CLIENRECU));

                if (proxy.GetAllParamétre().logo != "")
                {

                    if (proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().logo.ToString()) == true)
                    {
                        reportViewer1.EnableExternalImages = true;

                        String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                        ReportParameter paramLogo = new ReportParameter();
                        paramLogo.Name = "ImagePath";
                        //  paramLogo.Values.Add(@"file:///C:\Logo.png");
                        paramLogo.Values.Add(@"file:///" + photolocation);
                        reportViewer1.SetParameters(paramLogo);
                        // reportViewer1.LocalReport.SetParameters(parameter);
                    }

                }
                reportViewer1.Refresh();

                ExportLand(reportViewer1);
                Print(true);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void RunLentillePaiement(List<SVC.LentilleClient> monture, List<SVC.ClientV> CLIENRECU, List<SVC.Depeiment> listerecu, List<SVC.F1> listevisite)
        {

            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.LentilleRecu), false);
                // LocalReport reportViewer1 = new LocalReport();
                reportViewer1.LoadReportDefinition(MyRptStream);


                //reportViewer1.LoadReportDefinition(MyRptStream);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                rds.Value = monture;
                reportViewer1.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                SVC.Param paramlocal = (proxy.GetAllParamétre());
                paramlocal.logo = "D/Logo.jpg";
                selpara.Add((paramlocal));


                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", selpara));
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", CLIENRECU));
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", listerecu));
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", listevisite));
                if (proxy.GetAllParamétre().logo != "")
                {

                    if (proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().logo.ToString()) == true)
                    {
                        reportViewer1.EnableExternalImages = true;

                        String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                        ReportParameter paramLogo = new ReportParameter();
                        paramLogo.Name = "ImagePath";
                        //  paramLogo.Values.Add(@"file:///C:\Logo.png");
                        paramLogo.Values.Add(@"file:///" + photolocation);
                        reportViewer1.SetParameters(paramLogo);
                        // reportViewer1.LocalReport.SetParameters(parameter);
                    }

                }
                reportViewer1.Refresh();

                ExportA4(reportViewer1);
                Print(false);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void RunFactureTunisie(SVC.ServiceCliniqueClient proxyrecu, List<SVC.F1> Nfact, SVC.ClientV CLIENRECU, int interfaceimm)
        {
            try
            {
                LocalReport reportViewer1 = new LocalReport();
                var selpara = new List<SVC.Param>();
                SVC.Param selectpara = proxy.GetAllParamétre();
                /******************************/
                string nfact = Nfact.First().nfact.Substring(0, 1);
                string arrété = "";
                string document = "";
                switch (nfact)
                {
                    case "F":

                        document = "Facture";
                        arrété = "Arrétée la présente facture à la somme de";

                        selectpara.ImpressionCollisage = false;
                        break;
                    case "A":

                        arrété = "Arrétée la présente facture d'Avoir à la somme de";
                        document = "Facture d'Avoir";

                        selectpara.ImpressionCollisage = false;
                        break;
                    case "B":

                        arrété = "Arrétée le présent Bon de Livraison à la somme de";
                        document = "Bon de Livraison";

                        // selectpara.ImpressionCollisage = false;
                        break;
                    case "C":

                        arrété = "Arrétée le présent Avoir de Bon de Livraison à la somme de";
                        document = "Bon d'Avoir";


                        break;
                    case "P":

                        arrété = "Arrétée la présente facture Proforma à la somme de";
                        document = "Facture Proforma";

                        selectpara.ImpressionCollisage = false;
                        break;
                    case "R":

                        arrété = "Arrétée la présente facture à la somme de";
                        document = "Facture";

                        selectpara.ImpressionCollisage = false;
                        break;
                }

                /******************Dataset1 parametre****************************/

                selpara.Add(selectpara);

                /****************Convertion*****************/
                string Montant = "";



                /************************************/
                if (interfaceimm == 2)
                {

                    if (Nfact.First().net > 0)
                    {
                        Montant = (Convert.ToDecimal(Nfact.First().net)).ToText(Nut.Currency.EUR, Nut.Language.French);

                    }
                    else
                    {
                        if (Nfact.First().net < 0)
                        {
                            var mm = -Nfact.First().net;
                            Montant = "Moin " + (Convert.ToDecimal(mm)).ToText(Nut.Currency.EUR, Nut.Language.French);
                        }
                    }
                    Montant = Montant.Replace("euro", selpara.First().Limon.ToString());
                    Montant = Montant.Replace("centime", "millime");


                    MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.BonDeLivraisonDemiTunisie), false);

                    reportViewer1.LoadReportDefinition(MyRptStream);


                }
                else
                {
                    if (interfaceimm == 1)
                    {
                        if (selectpara.FactureSansTva == false)
                        {
                            if (Nfact.First().net > 0)
                            {
                                Montant = (Convert.ToDecimal(Nfact.First().net)).ToText(Nut.Currency.EUR, Nut.Language.French);

                            }
                            else
                            {
                                if (Nfact.First().net < 0)
                                {
                                    var mm = -Nfact.First().net;
                                    Montant = "Moin " + (Convert.ToDecimal(mm)).ToText(Nut.Currency.EUR, Nut.Language.French);
                                }
                            }
                            Montant = Montant.Replace("euro", selpara.First().Limon.ToString());
                            Montant = Montant.Replace("centime", "millime");
                            MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.FactureTunisienne), false);

                            reportViewer1.LoadReportDefinition(MyRptStream);


                        }
                        else
                        {
                            if (selectpara.FactureSansTva == true)
                            {
                                if (Nfact.First().net > 0)
                                {
                                    Montant = (Convert.ToDecimal(Nfact.First().net)).ToText(Nut.Currency.EUR, Nut.Language.French);

                                }
                                else
                                {
                                    if (Nfact.First().net < 0)
                                    {
                                        var mm = -Nfact.First().net;
                                        Montant = "Moin " + (Convert.ToDecimal(mm)).ToText(Nut.Currency.EUR, Nut.Language.French);
                                    }
                                }
                                Montant = Montant.Replace("euro", selpara.First().Limon.ToString());
                                Montant = Montant.Replace("centime", "millime");
                                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.FactureSansTva), false);

                                reportViewer1.LoadReportDefinition(MyRptStream);
                            }

                        }
                    }
                }

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                rds.Value = Nfact;
                reportViewer1.DataSources.Add(rds);
                /**************************************************/
                var ClientList = new List<SVC.ClientV>();
                ClientList.Add(CLIENRECU);
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", ClientList));
                /********************************************/
                var FactureList = proxy.GetAllFactureBycompteur(Nfact.First().nfact);

                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", FactureList.ToList()));
                /*********************************************/

                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                /*****************dataset 5***************/
                List<TVACLASS> listrva = new List<TVACLASS>();
                foreach (var facture in FactureList)
                {
                    if (listrva.Any(n => n.TVA == facture.tva) == false)
                    {
                        TVACLASS tt = new TVACLASS
                        {
                            TVA = Convert.ToDecimal(facture.tva),
                            BASE = Convert.ToDecimal(facture.privente * facture.quantite),
                            MONTANT = Convert.ToDecimal(((facture.privente * facture.quantite) * facture.tva) / 100),
                        };
                        if (tt.TVA != 0)
                        {
                            listrva.Add(tt);
                        }
                    }
                    else
                    {

                        var found = listrva.Find(n => n.TVA == facture.tva);
                        if (found.TVA != 0)
                        {
                            found.BASE = found.BASE + Convert.ToDecimal(facture.privente * facture.quantite);
                            found.MONTANT = found.MONTANT + (Convert.ToDecimal(((facture.privente * facture.quantite) * facture.tva) / 100));
                        }
                    }

                }
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", listrva));

                /********************************/
                /********ImagePath************************************/
                reportViewer1.EnableExternalImages = true;
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.SetParameters(paramLogo);
                /**************************************************************/
                ReportParameter paramDocument = new ReportParameter();
                paramDocument.Name = "Document";
                paramDocument.Values.Add(document);
                reportViewer1.SetParameters(paramDocument);
                /*************************************************************/
                ReportParameter paramArrettee = new ReportParameter();
                paramArrettee.Name = "Arrettee";
                paramArrettee.Values.Add(arrété);
                reportViewer1.SetParameters(paramArrettee);
                /*************************************************/
                ReportParameter paramMontantString = new ReportParameter();
                paramMontantString.Name = "MontantString";
                paramMontantString.Values.Add(Montant);
                reportViewer1.SetParameters(paramMontantString);
                /*********************************************************/
                /*************************************************/
                //List<SVC.F1> ListF1 = new List<SVC.F1>();
                var soldeexiste = proxy.GetAllF1All().Any(n => (n.codeclient == CLIENRECU.Id || n.cle == CLIENRECU.cle) && (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R") && n.reste != 0);// proxy.GetAllF1Bycode(CLIENRECU.Id).Any(n => n.reste != 0);
                decimal existe = 0;
                if (soldeexiste == true)
                {
                    existe = Convert.ToDecimal(proxy.GetAllF1All().Where(n => (n.codeclient == CLIENRECU.Id || n.cle == CLIENRECU.cle) && (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R") && n.reste != 0).AsEnumerable().Sum(n => n.reste));

                }
                else
                {
                    existe = 0;
                }


                ReportParameter paramSoldeClient = new ReportParameter();
                paramSoldeClient.Name = "SoldeClient";
                paramSoldeClient.Values.Add(existe.ToString());
                reportViewer1.SetParameters(paramSoldeClient);
                /*********************************************************/
                reportViewer1.Refresh();
                if (interfaceimm == 2)
                {
                    Export(reportViewer1);
                    Print(false);
                }
                else
                {
                    if (interfaceimm == 1)
                    {
                        ExportA4(reportViewer1);
                        Print(false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void RunFacture(SVC.ServiceCliniqueClient proxyrecu, List<SVC.F1> Nfact, SVC.ClientV CLIENRECU, int interfaceimm)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();
                var selpara = new List<SVC.Param>();
                SVC.Param selectpara = proxy.GetAllParamétre();
                selpara.Add((selectpara));

                string nfact = Nfact.First().nfact.Substring(0, 1);
                string arrété = "";
                string document = "";
                switch (nfact)
                {
                    case "F":
                        arrété = "Arrétée la présente facture à la somme de";
                        document = "Facture";
                        selectpara.ImpressionCollisage = false;
                        break;
                    case "A":
                        arrété = "Arrétée la présente facture d'Avoir à la somme de";
                        document = "Facture d'Avoir";
                        selectpara.ImpressionCollisage = false;
                        break;
                    case "B":
                        arrété = "Arrétée le présent Bon de Livraison à la somme de";
                        document = "Bon de Livraison";
                        break;
                    case "C":
                        arrété = "Arrétée le présent Avoir de Bon de Livraison à la somme de";
                        document = "Bon d'Avoir";

                        break;
                    case "P":
                        arrété = "Arrétée la présente facture Proforma à la somme de";
                        document = "Facture Proforma";
                        selectpara.ImpressionCollisage = false;
                        break;
                    case "R":
                        arrété = "Arrétée la présente facture à la somme de";
                        document = "Facture";
                        selectpara.ImpressionCollisage = false;
                        break;
                }

                /******************Dataset1 parametre****************************/

                /****************Convertion*****************/
                string Montant = "";

                if (Nfact.First().net > 0)
                {
                    Montant = (Convert.ToDecimal(Nfact.First().net)).ToText(Nut.Currency.EUR, Nut.Language.French);

                }
                else
                {
                    if (Nfact.First().net < 0)
                    {
                        var mm = -Nfact.First().net;
                        Montant = "Moin " + (Convert.ToDecimal(mm)).ToText(Nut.Currency.EUR, Nut.Language.French);
                    }
                }
                Montant = Montant.Replace("euro", selpara.First().Limon.ToString());

                /************************************/
                if (interfaceimm == 2)
                {
                    MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.BonDeLivraisonDemi), false);

                    reportViewer1.LoadReportDefinition(MyRptStream);
                }
                else
                {
                    if (interfaceimm == 1)
                    {
                        if (selectpara.FactureSansTva == false)
                        {
                            MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Facture), false);

                            reportViewer1.LoadReportDefinition(MyRptStream);
                        }
                        else
                        {
                            if (selectpara.FactureSansTva == true)
                            {
                                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.FactureSansTva), false);

                                reportViewer1.LoadReportDefinition(MyRptStream);
                            }
                        }
                    }
                }

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                rds.Value = Nfact;
                reportViewer1.DataSources.Add(rds);
                /**************************************************/
                var ClientList = new List<SVC.ClientV>();
                ClientList.Add(CLIENRECU);
                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", ClientList));
                /********************************************/
                var FactureList = proxy.GetAllFactureBycompteur(Nfact.First().nfact);

                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", FactureList.ToList()));
                /*********************************************/

                reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                /********ImagePath************************************/
                reportViewer1.EnableExternalImages = true;
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.SetParameters(paramLogo);
                /**************************************************************/
                ReportParameter paramDocument = new ReportParameter();
                paramDocument.Name = "Document";
                paramDocument.Values.Add(document);
                reportViewer1.SetParameters(paramDocument);
                /*************************************************************/
                ReportParameter paramArrettee = new ReportParameter();
                paramArrettee.Name = "Arrettee";
                paramArrettee.Values.Add(arrété);
                reportViewer1.SetParameters(paramArrettee);
                /*************************************************/
                ReportParameter paramMontantString = new ReportParameter();
                paramMontantString.Name = "MontantString";
                paramMontantString.Values.Add(Montant);
                reportViewer1.SetParameters(paramMontantString);
                /*********************************************************/
                /*************************************************/
                var soldeexiste = proxy.GetAllF1All().Any(n => (n.codeclient == CLIENRECU.Id || n.cle == CLIENRECU.cle) && (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R") && n.reste != 0);// proxy.GetAllF1Bycode(CLIENRECU.Id).Any(n => n.reste != 0);
                decimal existe = 0;
                if (soldeexiste == true)
                {
                    existe = Convert.ToDecimal(proxy.GetAllF1All().Where(n => (n.codeclient == CLIENRECU.Id || n.cle == CLIENRECU.cle) && (n.nfact.Substring(0, 1) != "P" && n.nfact.Substring(0, 1) != "R") && n.reste != 0).AsEnumerable().Sum(n => n.reste));

                }
                else
                {
                    existe = 0;
                }

                ReportParameter paramSoldeClient = new ReportParameter();
                paramSoldeClient.Name = "SoldeClient";
                paramSoldeClient.Values.Add(existe.ToString());
                reportViewer1.SetParameters(paramSoldeClient);


                if (interfaceimm == 2)
                {
                    Export(reportViewer1);
                    Print(false);
                }
                else
                {
                    if (interfaceimm == 1)
                    {
                        ExportA4(reportViewer1);
                        Print(false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private void btnImprimerLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*   if (LentilleDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                   {
                       SVC.LentilleClient selectedmont = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                       List<SVC.LentilleClient> mm = new List<SVC.LentilleClient>();
                       mm.Add(selectedmont);
                       List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                       clientvvv.Add(Clientvv);
                       ImpressionLentille cl = new ImpressionLentille(proxy, mm, clientvvv);
                       cl.Show();
                   }*/
                if (LentilleDatagrid.SelectedItem != null)
                {
                    dialog1 = new Window();
                    dialog1.Title = "Impression";
                    dialog1.Template = Resources["template6"] as ControlTemplate;

                    // dialog1.Content = Resources["content"];
                    dialog1.ResizeMode = ResizeMode.NoResize;
                    dialog1.Width = 350;
                    dialog1.Height = 280;
                    dialog1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog1.ShowDialog();
                    interfaceimpressionlentille = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        System.Int64 DoWork(string _numbers)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(13);
            string numberAsString = "";
            System.Int64 numberAsNumber = 0;

            for (var i = 0; i < 13; i++)
            {
                builder.Append(_numbers[random.Next(0, _numbers.Length)]);
            }

            numberAsString = builder.ToString();
            numberAsNumber = System.Int64.Parse(numberAsString);
            return numberAsNumber;
        }
        /* private void ValiderLentille_Click(object sender, RoutedEventArgs e)
         {
             try
             {

                 if (nouvellelentille == true && anciennelentille == false && memberuser.CreationDossierClient == true)
                 {
                     int interfacecreationmonture = 0;
                     bool creationmonture, creationcaisse, creationdepaiement = false;
                     LentilleClass.StatutDevis = true;
                     LentilleClass.StatutVente = false;
                     SVC.Depeiment PAIEMENTLentille;
                     // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                     SVC.Depense CAISSEMONTURE;
                     SVC.Depeiment PAIEMENTMONTURE;
                     SVC.Depense CAISSELentille;
                     string _numbers = Convert.ToString(LentilleClass.IdClient) + Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute) + Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond);

                   //  MessageBoxResult resul0333 = Xceed.Wpf.Toolkit.MessageBox.Show(Convert.ToString(DoWork(_numbers)), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                     LentilleClass.Ncommande = Convert.ToString(DoWork(_numbers));
                     if (txtMontantTotalENCLentille.Text != "")
                     {
                         if (Convert.ToDecimal(txtMontantTotalENCLentille.Text) != 0)
                         {
                             LentilleClass.Encaissé = Convert.ToDecimal(txtMontantTotalENCLentille.Text);
                             LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                         }
                         else
                         {
                             LentilleClass.Encaissé = 0;
                             LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                         }

                     }
                     else
                     {
                         LentilleClass.Encaissé = 0;
                         LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                     }
                     LentilleClass.Cle = Clientvv.Id + Clientvv.Raison + LentilleClass.MontantTotal + DateTime.Now.TimeOfDay;
                     if (LentilleClass.AccessoiresQuantite1 == 0)
                     {
                         LentilleClass.Accessoires1 = "";
                         LentilleClass.AccessoiresQuantite1 = null;
                     }
                     if (LentilleClass.AccessoiresQuantite2 == 0)
                     {
                         LentilleClass.Accessoires2 = "";
                         LentilleClass.AccessoiresQuantite2 = null;
                     }
                     if (LentilleClass.DroitPrixLentille == 0)
                     {
                         LentilleClass.DroiteLentilleDesignation = "";
                         LentilleClass.DroitPrixLentille = null;
                     }
                     if (LentilleClass.GauchePrixLentille == 0)
                     {
                         LentilleClass.GaucheLentilleDesignation = "";
                         LentilleClass.GauchePrixLentille = null;
                     }
                     if (LentilleClass.Encaissé != 0)
                     {


                         PAIEMENTLentille = new SVC.Depeiment
                         {
                             date = LentilleClass.Date,
                             montant = Convert.ToDecimal(LentilleClass.Encaissé),
                             paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " date :" + LentilleClass.Date,
                             oper = memberuser.Username,
                             dates = LentilleClass.Dates,
                             banque = "Caisse",
                             nfact = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                             amontant = Convert.ToDecimal(LentilleClass.MontantTotal),
                             cle = LentilleClass.Cle,
                             cp = LentilleClass.Id,
                             Multiple = false,
                             CodeClient = LentilleClass.IdClient,
                             RaisonClient = LentilleClass.RaisonClient,

                         };
                         CAISSELentille = new SVC.Depense
                         {
                             cle = LentilleClass.Cle,
                             Auto = true,
                             Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                             CompteDébité = "Caisse",
                             Crédit = true,
                             DateDebit = LentilleClass.Date,
                             DateSaisie = LentilleClass.Dates,
                             Débit = false,
                             ModePaiement = "ESPECES",
                             Montant = 0,
                             MontantCrédit = LentilleClass.Encaissé,
                             NumCheque = Convert.ToString(LentilleClass.Id),
                             Num_Facture = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                             RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                             Username = memberuser.Username,

                         };
                         interfacecreationmonture = 1;


                         using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                         {

                             proxy.InsertLentilleClient(LentilleClass);
                             creationmonture = true;


                             proxy.InsertDepense(CAISSELentille);
                             creationcaisse = true;
                             proxy.InsertDepeiment(PAIEMENTLentille);
                             creationdepaiement = true;

                             if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                             {
                                 ts.Complete();
                             }
                         }
                         if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                         {
                             proxy.AjouterTransactionPaiementRefresh();
                             proxy.AjouterDepenseRefresh();
                             proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                             GridLentille.IsEnabled = false;
                             GridLentille.DataContext = null;
                             LentilleClass = null;
                             Lentilleversementzero = false;
                             MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                         }
                     }
                     else
                     {
                         using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                         {


                             proxy.InsertLentilleClient(LentilleClass);
                             ts.Complete();
                             creationmonture = true;







                         }
                         if (creationmonture == true)
                         {
                             proxy.AjouterTransactionPaiementRefresh();
                             proxy.AjouterDepenseRefresh();
                             proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                             GridLentille.IsEnabled = false;
                             GridLentille.DataContext = null;
                             LentilleClass = null;
                             MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                         }
                     }


                 }
                 else
                 {
                     if (nouvellelentille == false && anciennelentille == true && memberuser.ModificationDossierClient == true)
                     {
                         //MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("I was here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                         int interfacecreationmonture = 0;
                         bool creationmonture, creationcaisse, creationdepaiement = false;

                         // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                         SVC.Depense CAISSEMONTURE;
                         SVC.Depeiment PAIEMENTMONTURE;

                         if (txtMontantTotalENCLentille.Text != "")
                         {
                             if (Convert.ToDecimal(txtMontantTotalENCLentille.Text) != 0)
                             {
                                 LentilleClass.Encaissé = Convert.ToDecimal(txtMontantTotalENCLentille.Text);
                                 LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                             }
                             else
                             {
                                 LentilleClass.Encaissé = 0;
                                 LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                             }

                         }
                         else
                         {
                             LentilleClass.Encaissé = 0;
                             LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                         }



                         if (LentilleClass.AccessoiresQuantite1 == 0)
                         {
                             LentilleClass.Accessoires1 = "";
                             LentilleClass.AccessoiresQuantite1 = null;
                         }
                         if (LentilleClass.AccessoiresQuantite2 == 0)
                         {
                             LentilleClass.Accessoires2 = "";
                             LentilleClass.AccessoiresQuantite2 = null;
                         }
                         if (LentilleClass.DroitPrixLentille == 0)
                         {
                             LentilleClass.DroiteLentilleDesignation = "";
                             LentilleClass.DroitPrixLentille = null;
                         }
                         if (LentilleClass.GauchePrixLentille == 0)
                         {
                             LentilleClass.GaucheLentilleDesignation = "";
                             LentilleClass.GauchePrixLentille = null;
                         }


                         if (Lentilleversementzero == false)
                         {
                             if (LentilleClass.Encaissé != 0)
                             {


                                 PAIEMENTMONTURE = new SVC.Depeiment
                                 {
                                     date = LentilleClass.Date,
                                     montant = Convert.ToDecimal(LentilleClass.Encaissé),
                                     paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " date :" + LentilleClass.Date,
                                     oper = memberuser.Username,
                                     dates = LentilleClass.Dates,
                                     banque = "Caisse",
                                     nfact = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                                     amontant = Convert.ToDecimal(LentilleClass.MontantTotal),
                                     cle = LentilleClass.Cle,
                                     cp = LentilleClass.Id,
                                     Multiple = false,
                                     CodeClient = LentilleClass.IdClient,
                                     RaisonClient = LentilleClass.RaisonClient,

                                 };
                                 CAISSEMONTURE = new SVC.Depense
                                 {
                                     cle = LentilleClass.Cle,
                                     Auto = true,
                                     Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                                     CompteDébité = "Caisse",
                                     Crédit = true,
                                     DateDebit = LentilleClass.Date,
                                     DateSaisie = LentilleClass.Dates,
                                     Débit = false,
                                     ModePaiement = "ESPECES",
                                     Montant = 0,
                                     MontantCrédit = LentilleClass.Encaissé,
                                     NumCheque = Convert.ToString(LentilleClass.Id),
                                     Num_Facture = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                                     RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                                     Username = memberuser.Username,

                                 };
                                 interfacecreationmonture = 1;
                                 using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                 {

                                     proxy.UpdateLentilleClient(LentilleClass);
                                     creationmonture = true;


                                     proxy.InsertDepense(CAISSEMONTURE);
                                     creationcaisse = true;
                                     proxy.InsertDepeiment(PAIEMENTMONTURE);
                                     creationdepaiement = true;

                                     if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                     {
                                         ts.Complete();
                                     }
                                 }
                                 if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                 {
                                     proxy.AjouterTransactionPaiementRefresh();
                                     proxy.AjouterDepenseRefresh();
                                     proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                     GridLentille.IsEnabled = false;
                                     GridLentille.DataContext = null;
                                     LentilleClass = null;
                                     Lentilleversementzero = false;
                                     MessageBoxResult resul56e03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                 }
                             }
                             else
                             {
                                 using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                 {


                                     proxy.UpdateLentilleClient(LentilleClass);
                                     ts.Complete();
                                     creationmonture = true;







                                 }
                                 if (creationmonture == true)
                                 {
                                     proxy.AjouterTransactionPaiementRefresh();
                                     proxy.AjouterDepenseRefresh();
                                     proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                     GridLentille.IsEnabled = false;
                                     GridLentille.DataContext = null;
                                     LentilleClass = null;
                                     MessageBoxResult resul0d3 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                 }
                             }
                         }
                         else
                         {
                             if (Lentilleversementzero == true)
                             {
                                 if (LentilleClass.Encaissé == 0)
                                 {
                                     CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == LentilleClass.Cle).First();
                                     PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == LentilleClass.Cle).First();
                                     using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                     {

                                         proxy.UpdateLentilleClient(LentilleClass);
                                         creationmonture = true;


                                         proxy.DeleteDepense(CAISSEMONTURE);
                                         creationcaisse = true;
                                         proxy.DeleteDepeiment(PAIEMENTMONTURE);
                                         creationdepaiement = true;

                                         if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                         {
                                             ts.Complete();
                                         }
                                     }
                                     if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                     {
                                         proxy.AjouterTransactionPaiementRefresh();
                                         proxy.AjouterDepenseRefresh();
                                         proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                         GridLentille.IsEnabled = false;
                                         GridLentille.DataContext = null;
                                         LentilleClass = null;
                                         Lentilleversementzero = false;
                                         MessageBoxResult resul0q3 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                     }
                                 }
                                 else
                                 {
                                     if (LentilleClass.Encaissé != 0)
                                     {
                                         CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == LentilleClass.Cle).First();
                                         CAISSEMONTURE.MontantCrédit = LentilleClass.Encaissé;
                                         PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == LentilleClass.Cle).First();
                                         PAIEMENTMONTURE.montant = Convert.ToDecimal(LentilleClass.Encaissé);
                                         using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                         {

                                             proxy.UpdateLentilleClient(LentilleClass);
                                             creationmonture = true;


                                             proxy.UpdateDepense(CAISSEMONTURE);
                                             creationcaisse = true;
                                             proxy.UpdateDepeiment(PAIEMENTMONTURE);
                                             creationdepaiement = true;

                                             if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                             {
                                                 ts.Complete();
                                             }
                                         }
                                         if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                         {
                                             proxy.AjouterTransactionPaiementRefresh();
                                             proxy.AjouterDepenseRefresh();
                                             proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                             GridLentille.IsEnabled = false;
                                             GridLentille.DataContext = null;
                                             LentilleClass = null;
                                             Lentilleversementzero = false;
                                             MessageBoxResult resul203 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                         }
                                     }
                                 }
                             }
                         }

                     }
                 }


             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
             }

         }*/
        private void ValiderLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (nouvellelentille == true && anciennelentille == false && memberuser.CreationDossierClient == true)
                {
                    int interfacecreationmonture = 0;
                    bool creationmonture, creationcaisse, creationdepaiement = false;
                    LentilleClass.StatutDevis = true;
                    LentilleClass.StatutVente = false;
                    SVC.Depeiment PAIEMENTLentille;
                    // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                    SVC.Depense CAISSEMONTURE;
                    SVC.Depeiment PAIEMENTMONTURE;
                    SVC.Depense CAISSELentille;
                    string _numbers = Convert.ToString(LentilleClass.IdClient) + Convert.ToString(LentilleClass.MontantTotal) + Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute) + Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond);

                    //    MessageBoxResult resul0333 = Xceed.Wpf.Toolkit.MessageBox.Show(Convert.ToString(DoWork(_numbers)), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    LentilleClass.Ncommande = Convert.ToString(DoWork(_numbers));
                    if (txtMontantTotalENCLentille.Text != "")
                    {
                        if (Convert.ToDecimal(txtMontantTotalENCLentille.Text) != 0)
                        {
                            LentilleClass.Encaissé = Convert.ToDecimal(txtMontantTotalENCLentille.Text);
                            LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                        }
                        else
                        {
                            LentilleClass.Encaissé = 0;
                            LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                        }

                    }
                    else
                    {
                        LentilleClass.Encaissé = 0;
                        LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                    }
                    LentilleClass.Cle = Clientvv.Id + Clientvv.Raison + LentilleClass.MontantTotal + DateTime.Now.TimeOfDay;
                    if (LentilleClass.AccessoiresQuantite1 == 0)
                    {
                        LentilleClass.Accessoires1 = "";
                        LentilleClass.AccessoiresQuantite1 = null;
                    }
                    if (LentilleClass.AccessoiresQuantite2 == 0)
                    {
                        LentilleClass.Accessoires2 = "";
                        LentilleClass.AccessoiresQuantite2 = null;
                    }
                    if (LentilleClass.DroitPrixLentille == 0)
                    {
                        LentilleClass.DroiteLentilleDesignation = "";
                        LentilleClass.DroitPrixLentille = null;
                    }
                    if (LentilleClass.GauchePrixLentille == 0)
                    {
                        LentilleClass.GaucheLentilleDesignation = "";
                        LentilleClass.GauchePrixLentille = null;
                    }
                    if (LentilleClass.Encaissé != 0)
                    {


                        PAIEMENTLentille = new SVC.Depeiment
                        {
                            date = LentilleClass.Date,
                            montant = Convert.ToDecimal(LentilleClass.Encaissé),
                            paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " date :" + LentilleClass.Date,
                            oper = memberuser.Username,
                            dates = LentilleClass.Dates,
                            banque = "Caisse",
                            nfact = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                            amontant = Convert.ToDecimal(LentilleClass.MontantTotal),
                            cle = LentilleClass.Cle,
                            cp = LentilleClass.Id,
                            Multiple = false,
                            CodeClient = LentilleClass.IdClient,
                            RaisonClient = LentilleClass.RaisonClient,

                        };
                        CAISSELentille = new SVC.Depense
                        {
                            cle = LentilleClass.Cle,
                            Auto = true,
                            Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                            CompteDébité = "Caisse",
                            Crédit = true,
                            DateDebit = LentilleClass.Date,
                            DateSaisie = LentilleClass.Dates,
                            Débit = false,
                            ModePaiement = "ESPECES",
                            Montant = 0,
                            MontantCrédit = LentilleClass.Encaissé,
                            NumCheque = Convert.ToString(LentilleClass.Id),
                            Num_Facture = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                            RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                            Username = memberuser.Username,

                        };
                        interfacecreationmonture = 1;


                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            proxy.InsertLentilleClient(LentilleClass);
                            creationmonture = true;


                            proxy.InsertDepense(CAISSELentille);
                            creationcaisse = true;
                            proxy.InsertDepeiment(PAIEMENTLentille);
                            creationdepaiement = true;

                            if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                            {
                                ts.Complete();
                            }
                        }
                        if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                        {

                            proxy.AjouterTransactionPaiementRefresh();
                            proxy.AjouterDepenseRefresh();
                            proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                            GridLentille.IsEnabled = false;
                            GridLentille.DataContext = null;
                            LentilleClass = null;
                            Lentilleversementzero = false;
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                    else
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {


                            proxy.InsertLentilleClient(LentilleClass);
                            ts.Complete();
                            creationmonture = true;







                        }
                        if (creationmonture == true)
                        {
                            proxy.AjouterTransactionPaiementRefresh();
                            proxy.AjouterDepenseRefresh();
                            proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                            GridLentille.IsEnabled = false;
                            GridLentille.DataContext = null;
                            LentilleClass = null;
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }


                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && memberuser.ModificationDossierClient == true)
                    {
                        //MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show("I was here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        int interfacecreationmonture = 0;
                        bool creationmonture, creationcaisse, creationdepaiement = false;

                        // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                        SVC.Depense CAISSEMONTURE;
                        SVC.Depeiment PAIEMENTMONTURE;

                        if (txtMontantTotalENCLentille.Text != "")
                        {
                            if (Convert.ToDecimal(txtMontantTotalENCLentille.Text) != 0)
                            {
                                LentilleClass.Encaissé = Convert.ToDecimal(txtMontantTotalENCLentille.Text);
                                LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                            }
                            else
                            {
                                LentilleClass.Encaissé = 0;
                                LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                            }

                        }
                        else
                        {
                            LentilleClass.Encaissé = 0;
                            LentilleClass.Reste = LentilleClass.MontantTotal - LentilleClass.Encaissé;

                        }



                        if (LentilleClass.AccessoiresQuantite1 == 0)
                        {
                            LentilleClass.Accessoires1 = "";
                            LentilleClass.AccessoiresQuantite1 = null;
                        }
                        if (LentilleClass.AccessoiresQuantite2 == 0)
                        {
                            LentilleClass.Accessoires2 = "";
                            LentilleClass.AccessoiresQuantite2 = null;
                        }
                        if (LentilleClass.DroitPrixLentille == 0)
                        {
                            LentilleClass.DroiteLentilleDesignation = "";
                            LentilleClass.DroitPrixLentille = null;
                        }
                        if (LentilleClass.GauchePrixLentille == 0)
                        {
                            LentilleClass.GaucheLentilleDesignation = "";
                            LentilleClass.GauchePrixLentille = null;
                        }


                        if (Lentilleversementzero == false)
                        {
                            if (LentilleClass.Encaissé != 0)
                            {


                                PAIEMENTMONTURE = new SVC.Depeiment
                                {
                                    date = LentilleClass.Date,
                                    montant = Convert.ToDecimal(LentilleClass.Encaissé),
                                    paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " date :" + LentilleClass.Date,
                                    oper = memberuser.Username,
                                    dates = LentilleClass.Dates,
                                    banque = "Caisse",
                                    nfact = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                                    amontant = Convert.ToDecimal(LentilleClass.MontantTotal),
                                    cle = LentilleClass.Cle,
                                    cp = LentilleClass.Id,
                                    Multiple = false,
                                    CodeClient = LentilleClass.IdClient,
                                    RaisonClient = LentilleClass.RaisonClient,

                                };
                                CAISSEMONTURE = new SVC.Depense
                                {
                                    cle = LentilleClass.Cle,
                                    Auto = true,
                                    Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                                    CompteDébité = "Caisse",
                                    Crédit = true,
                                    DateDebit = LentilleClass.Date,
                                    DateSaisie = LentilleClass.Dates,
                                    Débit = false,
                                    ModePaiement = "ESPECES",
                                    Montant = 0,
                                    MontantCrédit = LentilleClass.Encaissé,
                                    NumCheque = Convert.ToString(LentilleClass.Id),
                                    Num_Facture = LentilleClass.Date + " " + LentilleClass.RaisonClient,
                                    RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + LentilleClass.RaisonClient + " " + " date :" + LentilleClass.Date,
                                    Username = memberuser.Username,

                                };
                                interfacecreationmonture = 1;
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {

                                    proxy.UpdateLentilleClient(LentilleClass);
                                    creationmonture = true;


                                    proxy.InsertDepense(CAISSEMONTURE);
                                    creationcaisse = true;
                                    proxy.InsertDepeiment(PAIEMENTMONTURE);
                                    creationdepaiement = true;

                                    if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                    {
                                        ts.Complete();
                                    }
                                }
                                if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                {
                                    proxy.AjouterTransactionPaiementRefresh();
                                    proxy.AjouterDepenseRefresh();
                                    proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                    GridLentille.IsEnabled = false;
                                    GridLentille.DataContext = null;
                                    LentilleClass = null;
                                    Lentilleversementzero = false;
                                    MessageBoxResult resul56e03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }
                            else
                            {
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {


                                    proxy.UpdateLentilleClient(LentilleClass);
                                    ts.Complete();
                                    creationmonture = true;







                                }
                                if (creationmonture == true)
                                {
                                    proxy.AjouterTransactionPaiementRefresh();
                                    proxy.AjouterDepenseRefresh();
                                    proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                    GridLentille.IsEnabled = false;
                                    GridLentille.DataContext = null;
                                    LentilleClass = null;
                                    MessageBoxResult resul0d3 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        else
                        {
                            if (Lentilleversementzero == true)
                            {
                                if (LentilleClass.Encaissé == 0)
                                {
                                    CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == LentilleClass.Cle).First();
                                    PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == LentilleClass.Cle).First();
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {

                                        proxy.UpdateLentilleClient(LentilleClass);
                                        creationmonture = true;


                                        proxy.DeleteDepense(CAISSEMONTURE);
                                        creationcaisse = true;
                                        proxy.DeleteDepeiment(PAIEMENTMONTURE);
                                        creationdepaiement = true;

                                        if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                        {
                                            ts.Complete();
                                        }
                                    }
                                    if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                    {
                                        proxy.AjouterTransactionPaiementRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                        GridLentille.IsEnabled = false;
                                        GridLentille.DataContext = null;
                                        LentilleClass = null;
                                        Lentilleversementzero = false;
                                        MessageBoxResult resul0q3 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                }
                                else
                                {
                                    if (LentilleClass.Encaissé != 0)
                                    {
                                        CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == LentilleClass.Cle).First();
                                        CAISSEMONTURE.MontantCrédit = LentilleClass.Encaissé;
                                        PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == LentilleClass.Cle).First();
                                        PAIEMENTMONTURE.montant = Convert.ToDecimal(LentilleClass.Encaissé);
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {

                                            proxy.UpdateLentilleClient(LentilleClass);
                                            creationmonture = true;


                                            proxy.UpdateDepense(CAISSEMONTURE);
                                            creationcaisse = true;
                                            proxy.UpdateDepeiment(PAIEMENTMONTURE);
                                            creationdepaiement = true;

                                            if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                            {
                                                ts.Complete();
                                            }
                                        }
                                        if (creationcaisse == true && creationmonture == true && creationdepaiement == true)
                                        {
                                            proxy.AjouterTransactionPaiementRefresh();
                                            proxy.AjouterDepenseRefresh();
                                            proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                            GridLentille.IsEnabled = false;
                                            GridLentille.DataContext = null;
                                            LentilleClass = null;
                                            Lentilleversementzero = false;
                                            MessageBoxResult resul203 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }
                                    }
                                }
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void AnnulerLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridLentille.IsEnabled = false;
                GridLentille.DataContext = null;
                GridLentille.IsEnabled = false;
                nouvellelentille = false;
                anciennelentille = false;



            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnTransposerLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /********************GroiteLoin*************************************/
                if (txtAncienSphereDroite.Text != "" && txtAncienCylindreDroite.Text != "" && txtAncienAxeDroite.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtAncienSphereDroite.Text != "")
                    {
                        if (decimal.TryParse(txtAncienSphereDroite.Text, out sphere))
                            sphere = Convert.ToDecimal(txtAncienSphereDroite.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtAncienSphereDroite.Text = "";
                        sphere = 0;
                    }
                    if (txtAncienCylindreDroite.Text != "")
                    {
                        if (decimal.TryParse(txtAncienCylindreDroite.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtAncienCylindreDroite.Text);
                            if (cylindre > 0)
                            {
                                LentilleClass.DroiteCylindrePlus = true;
                                LentilleClass.DroiteCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    LentilleClass.DroiteCylindrePlus = false;
                                    LentilleClass.DroiteCylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtAncienCylindreDroite.Text = "";
                        cylindre = 0;
                    }

                    if (txtAncienAxeDroite.Text != "")
                    {
                        if (int.TryParse(txtAncienAxeDroite.Text, out axe))
                            axe = Convert.ToInt16(txtAncienAxeDroite.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtAncienAxeDroite.Text = "";
                        axe = 0;
                    }
                    txtAncienCylindreDroite.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtAncienSphereDroite.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (LentilleClass.DroiteCylindrePlus == true && LentilleClass.DroiteCylindreMoin == false)
                    {
                        txtAncienAxeDroite.Text = (axe + 90).ToString();
                        LentilleClass.DroiteCylindrePlus = false;
                        LentilleClass.DroiteCylindreMoin = true;

                    }
                    else
                    {
                        if (LentilleClass.DroiteCylindrePlus == false && LentilleClass.DroiteCylindreMoin == true)
                        {
                            txtAncienAxeDroite.Text = (axe - 90).ToString();
                            LentilleClass.DroiteCylindrePlus = true;
                            LentilleClass.DroiteCylindreMoin = false;
                        }
                    }
                } /********************GroiteIntermediaire*************************************/
                if (txtNouvSphereDroite.Text != "" && txtNouvCylindreDroite.Text != "" && txtNouvAxeDroite.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtNouvSphereDroite.Text != "")
                    {
                        if (decimal.TryParse(txtNouvSphereDroite.Text, out sphere))
                            sphere = Convert.ToDecimal(txtNouvSphereDroite.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtNouvSphereDroite.Text = "";
                        sphere = 0;
                    }
                    if (txtNouvCylindreDroite.Text != "")
                    {
                        if (decimal.TryParse(txtNouvCylindreDroite.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtNouvCylindreDroite.Text);
                            if (cylindre > 0)
                            {
                                LentilleClass.DroiteCylindrePlus = true;
                                LentilleClass.DroiteCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    LentilleClass.DroiteCylindrePlus = false;
                                    LentilleClass.DroiteCylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtNouvCylindreDroite.Text = "";
                        cylindre = 0;
                    }

                    if (txtNouvAxeDroite.Text != "")
                    {
                        if (int.TryParse(txtNouvAxeDroite.Text, out axe))
                            axe = Convert.ToInt16(txtNouvAxeDroite.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtNouvAxeDroite.Text = "";
                        axe = 0;
                    }
                    txtNouvCylindreDroite.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtNouvSphereDroite.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (LentilleClass.DroiteCylindrePlus == true && LentilleClass.DroiteCylindreMoin == false)
                    {
                        txtNouvAxeDroite.Text = (axe + 90).ToString();
                        LentilleClass.DroiteCylindrePlus = false;
                        LentilleClass.DroiteCylindreMoin = true;

                    }
                    else
                    {
                        if (LentilleClass.DroiteCylindrePlus == false && LentilleClass.DroiteCylindreMoin == true)
                        {
                            txtNouvAxeDroite.Text = (axe - 90).ToString();
                            LentilleClass.DroiteCylindrePlus = true;
                            LentilleClass.DroiteCylindreMoin = false;
                        }
                    }
                }
                /********************GroitePres*************************************/
                if (txtLentilleSphereDroite.Text != "" && txtLentilleCylindreDroite.Text != "" && txtLentilleAxeDroite.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtLentilleSphereDroite.Text != "")
                    {
                        if (decimal.TryParse(txtLentilleSphereDroite.Text, out sphere))
                            sphere = Convert.ToDecimal(txtLentilleSphereDroite.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtLentilleSphereDroite.Text = "";
                        sphere = 0;
                    }
                    if (txtLentilleCylindreDroite.Text != "")
                    {
                        if (decimal.TryParse(txtLentilleCylindreDroite.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtLentilleCylindreDroite.Text);
                            if (cylindre > 0)
                            {
                                LentilleClass.DroiteCylindrePlus = true;
                                LentilleClass.DroiteCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    LentilleClass.DroiteCylindrePlus = false;
                                    LentilleClass.DroiteCylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtLentilleCylindreDroite.Text = "";
                        cylindre = 0;
                    }

                    if (txtLentilleAxeDroite.Text != "")
                    {
                        if (int.TryParse(txtLentilleAxeDroite.Text, out axe))
                            axe = Convert.ToInt16(txtLentilleAxeDroite.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtLentilleAxeDroite.Text = "";
                        axe = 0;
                    }
                    txtLentilleCylindreDroite.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtLentilleSphereDroite.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (LentilleClass.DroiteCylindrePlus == true && LentilleClass.DroiteCylindreMoin == false)
                    {
                        txtLentilleAxeDroite.Text = (axe + 90).ToString();
                        LentilleClass.DroiteCylindrePlus = false;
                        LentilleClass.DroiteCylindreMoin = true;

                    }
                    else
                    {
                        if (LentilleClass.DroiteCylindrePlus == false && LentilleClass.DroiteCylindreMoin == true)
                        {
                            txtLentilleAxeDroite.Text = (axe - 90).ToString();
                            LentilleClass.DroiteCylindrePlus = true;
                            LentilleClass.DroiteCylindreMoin = false;
                        }
                    }
                }
                /********************GaucheLoin*************************************/
                if (txtAncienSphereGauche.Text != "" && txtAncienCylindreGauche.Text != "" && txtAncienAxeGauche.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtAncienSphereGauche.Text != "")
                    {
                        if (decimal.TryParse(txtAncienSphereGauche.Text, out sphere))
                            sphere = Convert.ToDecimal(txtAncienSphereGauche.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtAncienSphereGauche.Text = "";
                        sphere = 0;
                    }
                    if (txtAncienCylindreGauche.Text != "")
                    {
                        if (decimal.TryParse(txtAncienCylindreGauche.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtAncienCylindreGauche.Text);
                            if (cylindre > 0)
                            {
                                LentilleClass.GaucheCylindrePlus = true;
                                LentilleClass.GaucheCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    LentilleClass.GaucheCylindrePlus = false;
                                    LentilleClass.GaucheCylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtAncienCylindreGauche.Text = "";
                        cylindre = 0;
                    }

                    if (txtAncienAxeGauche.Text != "")
                    {
                        if (int.TryParse(txtAncienAxeGauche.Text, out axe))
                            axe = Convert.ToInt16(txtAncienAxeGauche.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtAncienAxeGauche.Text = "";
                        axe = 0;
                    }
                    txtAncienCylindreGauche.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtAncienSphereGauche.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (LentilleClass.GaucheCylindrePlus == true && LentilleClass.GaucheCylindreMoin == false)
                    {
                        txtAncienAxeGauche.Text = (axe + 90).ToString();
                        LentilleClass.GaucheCylindrePlus = false;
                        LentilleClass.GaucheCylindreMoin = true;

                    }
                    else
                    {
                        if (LentilleClass.GaucheCylindrePlus == false && LentilleClass.GaucheCylindreMoin == true)
                        {
                            txtAncienAxeGauche.Text = (axe - 90).ToString();
                            LentilleClass.GaucheCylindrePlus = true;
                            LentilleClass.GaucheCylindreMoin = false;
                        }
                    }
                }
                /***********************Nouv gauche***********************/
                if (txtNouvSphereGauche.Text != "" && txtNouvCylindreGauche.Text != "" && txtNouvAxeGauche.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtNouvSphereGauche.Text != "")
                    {
                        if (decimal.TryParse(txtNouvSphereGauche.Text, out sphere))
                            sphere = Convert.ToDecimal(txtNouvSphereGauche.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtNouvSphereGauche.Text = "";
                        sphere = 0;
                    }
                    if (txtNouvCylindreGauche.Text != "")
                    {
                        if (decimal.TryParse(txtNouvCylindreGauche.Text, out cylindre))
                        {

                            cylindre = Convert.ToDecimal(txtNouvCylindreGauche.Text);
                            if (cylindre > 0)
                            {
                                LentilleClass.GaucheCylindrePlus = true;
                                LentilleClass.GaucheCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    LentilleClass.GaucheCylindrePlus = false;
                                    LentilleClass.GaucheCylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtNouvCylindreGauche.Text = "";
                        cylindre = 0;
                    }

                    if (txtNouvAxeGauche.Text != "")
                    {
                        if (int.TryParse(txtNouvAxeGauche.Text, out axe))
                            axe = Convert.ToInt16(txtNouvAxeGauche.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtNouvAxeGauche.Text = "";
                        axe = 0;
                    }
                    txtNouvCylindreGauche.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtNouvSphereGauche.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (LentilleClass.GaucheCylindrePlus == true && LentilleClass.GaucheCylindreMoin == false)
                    {
                        txtNouvAxeGauche.Text = (axe + 90).ToString();
                        LentilleClass.GaucheCylindrePlus = false;
                        LentilleClass.GaucheCylindreMoin = true;

                    }
                    else
                    {
                        if (LentilleClass.GaucheCylindrePlus == false && LentilleClass.GaucheCylindreMoin == true)
                        {
                            txtNouvAxeGauche.Text = (axe - 90).ToString();
                            LentilleClass.GaucheCylindrePlus = true;
                            LentilleClass.GaucheCylindreMoin = false;
                        }
                    }
                }
                /********************GauchePres*************************************/
                if (txtLentilleSphereGauche.Text != "" && txtLentilleCylindreGauche.Text != "" && txtLentilleAxeGauche.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtLentilleSphereGauche.Text != "")
                    {
                        if (decimal.TryParse(txtLentilleSphereGauche.Text, out sphere))
                            sphere = Convert.ToDecimal(txtLentilleSphereGauche.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtLentilleSphereGauche.Text = "";
                        sphere = 0;
                    }
                    if (txtLentilleCylindreGauche.Text != "")
                    {
                        if (decimal.TryParse(txtLentilleCylindreGauche.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtLentilleCylindreGauche.Text);
                            if (cylindre > 0)
                            {
                                LentilleClass.GaucheCylindrePlus = true;
                                LentilleClass.GaucheCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    LentilleClass.GaucheCylindrePlus = false;
                                    LentilleClass.GaucheCylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtLentilleCylindreGauche.Text = "";
                        cylindre = 0;
                    }

                    if (txtLentilleAxeGauche.Text != "")
                    {
                        if (int.TryParse(txtLentilleAxeGauche.Text, out axe))
                            axe = Convert.ToInt16(txtLentilleAxeGauche.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtLentilleAxeGauche.Text = "";
                        axe = 0;
                    }
                    txtLentilleCylindreGauche.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtLentilleSphereGauche.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (LentilleClass.GaucheCylindrePlus == true && LentilleClass.GaucheCylindreMoin == false)
                    {
                        txtLentilleAxeGauche.Text = (axe + 90).ToString();
                        LentilleClass.GaucheCylindrePlus = false;
                        LentilleClass.GaucheCylindreMoin = true;

                    }
                    else
                    {
                        if (LentilleClass.GaucheCylindrePlus == false && LentilleClass.GaucheCylindreMoin == true)
                        {
                            txtLentilleAxeGauche.Text = (axe - 90).ToString();
                            LentilleClass.GaucheCylindrePlus = true;
                            LentilleClass.GaucheCylindreMoin = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtKeratometrieDroiteH_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Double KeratometrieDroiteH = 0;
                Double moyDroite = 0;
                if (txtKeratometrieDroiteH.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieDroiteH.Text, out KeratometrieDroiteH))
                    {
                        KeratometrieDroiteH = Convert.ToDouble(txtKeratometrieDroiteH.Text);
                    }
                    else
                    {
                        txtKeratometrieDroiteH.Text = "";
                    }
                }
                Double KeratometrieDroiteV = 0;

                if (txtKeratometrieDroiteV.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieDroiteV.Text, out KeratometrieDroiteV))
                    {
                        KeratometrieDroiteV = Convert.ToDouble(txtKeratometrieDroiteV.Text);
                    }
                    else
                    {
                        txtKeratometrieDroiteV.Text = "";
                    }
                }
                moyDroite = (KeratometrieDroiteV + KeratometrieDroiteH) / 2;
                if (moyDroite != 0)
                {
                    txtKeratometrieDroitemoy.Text = moyDroite.ToString();
                    LentilleClass.KeratometrieDroitemoy = moyDroite.ToString();
                }
                else
                {
                    txtKeratometrieDroitemoy.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        /* private void txtDroiteLentilleDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
         {
             try
             {
                 if (nouvellelentille == true && anciennelentille == false)
                 {
                     SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 1, LentilleClass, 0);
                     cl.Show();
                 }
                 else
                 {
                     if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                     {
                         SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 1, LentilleClass, 0);
                         cl.Show();
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
        private void txtDroiteLentilleDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellelentille == true && anciennelentille == false)
                {
                    SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 1, LentilleClass, 0, Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 1, LentilleClass, 1, Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtDroiteVerreLoinDesignation_KeyDown(object sender, KeyEventArgs e)
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

        private void txtDroitQuantiteLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteLentilleDroite, QuantiteLentilleGauche, PrixDroite, PrixGauche, PrixTotal = 0;
                if (txtDroitQuantiteLentille.Text != "")
                {
                    QuantiteLentilleDroite = Convert.ToDecimal(txtDroitQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleDroite = 0;
                }
                if (txtDroitPrixLentille.Text != "")
                {
                    PrixDroite = Convert.ToDecimal(txtDroitPrixLentille.Text);
                }
                else
                {
                    PrixDroite = 0;
                }
                /********************gauche************/
                if (txtGaucheQuantiteLentille.Text != "")
                {
                    QuantiteLentilleGauche = Convert.ToDecimal(txtGaucheQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleGauche = 0;
                }
                if (txtGauchePrixLentille.Text != "")
                {
                    PrixGauche = Convert.ToDecimal(txtGauchePrixLentille.Text);
                }
                else
                {
                    PrixGauche = 0;
                }


                PrixTotal = (PrixDroite * QuantiteLentilleDroite) + (PrixGauche * QuantiteLentilleGauche);


                txtPrixTotalLentille.Text = (PrixTotal).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /* private void txtGaucheLentilleDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
         {
             try
             {
                 if (nouvellelentille == true && anciennelentille == false)
                 {
                     SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 2, LentilleClass, 0);
                     cl.Show();
                 }
                 else
                 {
                     if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                     {
                         SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 2, LentilleClass, 0);
                         cl.Show();
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
        private void txtGaucheLentilleDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellelentille == true && anciennelentille == false)
                {
                    SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 2, LentilleClass, 0, Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 2, LentilleClass, 1, Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtGauchePrixLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteLentilleDroite, QuantiteLentilleGauche, PrixDroite, PrixGauche, PrixTotal = 0;
                if (txtDroitQuantiteLentille.Text != "")
                {
                    QuantiteLentilleDroite = Convert.ToDecimal(txtDroitQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleDroite = 0;
                }
                if (txtDroitPrixLentille.Text != "")
                {
                    PrixDroite = Convert.ToDecimal(txtDroitPrixLentille.Text);
                }
                else
                {
                    PrixDroite = 0;
                }
                /********************gauche************/
                if (txtGaucheQuantiteLentille.Text != "")
                {
                    QuantiteLentilleGauche = Convert.ToDecimal(txtGaucheQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleGauche = 0;
                }
                if (txtGauchePrixLentille.Text != "")
                {
                    PrixGauche = Convert.ToDecimal(txtGauchePrixLentille.Text);
                }
                else
                {
                    PrixGauche = 0;
                }


                PrixTotal = (PrixDroite * QuantiteLentilleDroite) + (PrixGauche * QuantiteLentilleGauche);


                txtPrixTotalLentille.Text = (PrixTotal).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtGaucheQuantiteLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteLentilleDroite, QuantiteLentilleGauche, PrixDroite, PrixGauche, PrixTotal = 0;
                if (txtDroitQuantiteLentille.Text != "")
                {
                    QuantiteLentilleDroite = Convert.ToDecimal(txtDroitQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleDroite = 0;
                }
                if (txtDroitPrixLentille.Text != "")
                {
                    PrixDroite = Convert.ToDecimal(txtDroitPrixLentille.Text);
                }
                else
                {
                    PrixDroite = 0;
                }
                /********************gauche************/
                if (txtGaucheQuantiteLentille.Text != "")
                {
                    QuantiteLentilleGauche = Convert.ToDecimal(txtGaucheQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleGauche = 0;
                }
                if (txtGauchePrixLentille.Text != "")
                {
                    PrixGauche = Convert.ToDecimal(txtGauchePrixLentille.Text);
                }
                else
                {
                    PrixGauche = 0;
                }


                PrixTotal = (PrixDroite * QuantiteLentilleDroite) + (PrixGauche * QuantiteLentilleGauche);


                txtPrixTotalLentille.Text = (PrixTotal).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixTotalLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLentille, PrixAccessoires1, PrixAccessoires2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLentille.Text != "")
                {
                    PrixTotalLentille = Convert.ToDecimal(txtPrixTotalLentille.Text);
                }
                else
                {
                    PrixTotalLentille = 0;
                }
                if (txtPrixTotalAcc1Lentille.Text != "")
                {
                    PrixAccessoires1 = Convert.ToDecimal(txtPrixTotalAcc1Lentille.Text);
                }
                else
                {
                    PrixAccessoires1 = 0;
                }
                if (txtPrixTotalAcc2Lentille.Text != "")
                {
                    PrixAccessoires2 = Convert.ToDecimal(txtPrixTotalAcc2Lentille.Text);
                }
                else
                {
                    PrixAccessoires2 = 0;
                }

                montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                if (!String.IsNullOrEmpty(txtRemiseLentille.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseLentille.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2 - remisemonture);
                        }
                        else
                        {
                            txtRemiseLentille.Text = "";
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                        }
                    }
                    else
                    {
                        txtRemiseLentille.Text = "";
                        montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseLentille.Text = "";
                }

                txtMontantTotalLentille.Text = (montanttotal).ToString();
                LentilleClass.MontantTotal = Convert.ToDecimal(txtMontantTotalLentille.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoires1Lentille_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellelentille == true && anciennelentille == false)
                {
                    //   SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 7, LentilleClass, 0);
                    SelectionProduitLentilleCat cl = new SelectionProduitLentilleCat(proxy, memberuser, callback, 7, LentilleClass, 0, Clientvv);

                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        // SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 7, LentilleClass, 0);
                        SelectionProduitLentilleCat cl = new SelectionProduitLentilleCat(proxy, memberuser, callback, 7, LentilleClass, 0, Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresQuantite1Lentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite1Lentille.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite1Lentille.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix1Lentille.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix1Lentille.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc1Lentille.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresPrix1Lentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite1Lentille.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite1Lentille.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix1Lentille.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix1Lentille.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc1Lentille.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixTotalAcc1Lentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLentille, PrixAccessoires1, PrixAccessoires2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLentille.Text != "")
                {
                    PrixTotalLentille = Convert.ToDecimal(txtPrixTotalLentille.Text);
                }
                else
                {
                    PrixTotalLentille = 0;
                }
                if (txtPrixTotalAcc1Lentille.Text != "")
                {
                    PrixAccessoires1 = Convert.ToDecimal(txtPrixTotalAcc1Lentille.Text);
                }
                else
                {
                    PrixAccessoires1 = 0;
                }
                if (txtPrixTotalAcc2Lentille.Text != "")
                {
                    PrixAccessoires2 = Convert.ToDecimal(txtPrixTotalAcc2Lentille.Text);
                }
                else
                {
                    PrixAccessoires2 = 0;
                }

                montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                if (!String.IsNullOrEmpty(txtRemiseLentille.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseLentille.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2 - remisemonture);
                        }
                        else
                        {
                            txtRemiseLentille.Text = "";
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                        }
                    }
                    else
                    {
                        txtRemiseLentille.Text = "";
                        montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseLentille.Text = "";
                }

                txtMontantTotalLentille.Text = (montanttotal).ToString();
                LentilleClass.MontantTotal = Convert.ToDecimal(txtMontantTotalLentille.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoires2Lentille_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellelentille == true && anciennelentille == false)
                {
                    //  SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 8, LentilleClass, 0);
                    SelectionProduitLentilleCat cl = new SelectionProduitLentilleCat(proxy, memberuser, callback, 8, LentilleClass, 0, Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        //SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 8, LentilleClass, 0);
                        SelectionProduitLentilleCat cl = new SelectionProduitLentilleCat(proxy, memberuser, callback, 8, LentilleClass, 0, Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresQuantite2Lentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite2Lentille.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite2Lentille.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix2Lentille.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix2Lentille.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc2Lentille.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresPrix2Lentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite2Lentille.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite2Lentille.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix2Lentille.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix2Lentille.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc2Lentille.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixTotalAcc2Lentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLentille, PrixAccessoires1, PrixAccessoires2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLentille.Text != "")
                {
                    PrixTotalLentille = Convert.ToDecimal(txtPrixTotalLentille.Text);
                }
                else
                {
                    PrixTotalLentille = 0;
                }
                if (txtPrixTotalAcc1Lentille.Text != "")
                {
                    PrixAccessoires1 = Convert.ToDecimal(txtPrixTotalAcc1Lentille.Text);
                }
                else
                {
                    PrixAccessoires1 = 0;
                }
                if (txtPrixTotalAcc2Lentille.Text != "")
                {
                    PrixAccessoires2 = Convert.ToDecimal(txtPrixTotalAcc2Lentille.Text);
                }
                else
                {
                    PrixAccessoires2 = 0;
                }

                montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                if (!String.IsNullOrEmpty(txtRemiseLentille.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseLentille.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2 - remisemonture);
                        }
                        else
                        {
                            txtRemiseLentille.Text = "";
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                        }
                    }
                    else
                    {
                        txtRemiseLentille.Text = "";
                        montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseLentille.Text = "";
                }

                txtMontantTotalLentille.Text = (montanttotal).ToString();
                LentilleClass.MontantTotal = Convert.ToDecimal(txtMontantTotalLentille.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnDiopmm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Double KeratometrieDroiteH = 0;
                Double moyDroite = 0;
                if (txtKeratometrieDroiteH.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieDroiteH.Text, out KeratometrieDroiteH))
                    {
                        KeratometrieDroiteH = Convert.ToDouble(txtKeratometrieDroiteH.Text);
                        txtKeratometrieDroiteH.Text = (337.5 / KeratometrieDroiteH).ToString();
                    }
                    else
                    {
                        txtKeratometrieDroiteH.Text = "";
                    }
                }
                Double KeratometrieDroiteV = 0;

                if (txtKeratometrieDroiteV.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieDroiteV.Text, out KeratometrieDroiteV))
                    {
                        KeratometrieDroiteV = Convert.ToDouble(txtKeratometrieDroiteV.Text);
                        txtKeratometrieDroiteV.Text = (337.5 / KeratometrieDroiteV).ToString();
                    }
                    else
                    {
                        txtKeratometrieDroiteV.Text = "";
                    }
                }
                moyDroite = (KeratometrieDroiteV + KeratometrieDroiteH) / 2;
                if (moyDroite != 0)
                {
                    txtKeratometrieDroitemoy.Text = moyDroite.ToString();
                }
                else
                {
                    txtKeratometrieDroitemoy.Text = "";

                }

                Double KeratometrieGaucheH = 0;
                Double moyGauche = 0;
                if (txtKeratometrieGaucheH.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieGaucheH.Text, out KeratometrieGaucheH))
                    {
                        KeratometrieGaucheH = Convert.ToDouble(txtKeratometrieGaucheH.Text);
                        txtKeratometrieGaucheH.Text = (337.5 / KeratometrieGaucheH).ToString();
                    }
                    else
                    {
                        txtKeratometrieGaucheH.Text = "";
                    }
                }

                Double KeratometrieGaucheV = 0;

                if (txtKeratometrieGaucheV.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieGaucheV.Text, out KeratometrieGaucheV))
                    {
                        KeratometrieGaucheV = Convert.ToDouble(txtKeratometrieGaucheV.Text);
                        txtKeratometrieGaucheV.Text = (337.5 / KeratometrieGaucheV).ToString();
                    }
                    else
                    {
                        txtKeratometrieGaucheV.Text = "";
                    }
                }
                moyGauche = (KeratometrieGaucheV + KeratometrieGaucheH) / 2;
                if (moyGauche != 0)
                {
                    txtKeratometrieGauchemoy.Text = moyGauche.ToString();
                }
                else
                {
                    txtKeratometrieGauchemoy.Text = "";

                }
                if (LentilleClass.Dioptrie == true && LentilleClass.mm == false)
                {
                    LentilleClass.Dioptrie = false;
                    LentilleClass.mm = true;
                }
                else
                {
                    LentilleClass.Dioptrie = true;
                    LentilleClass.mm = false;
                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtKeratometrieDroiteV_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Double KeratometrieDroiteH = 0;
                Double moyDroite = 0;
                if (txtKeratometrieDroiteH.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieDroiteH.Text, out KeratometrieDroiteH))
                    {
                        KeratometrieDroiteH = Convert.ToDouble(txtKeratometrieDroiteH.Text);
                    }
                    else
                    {
                        txtKeratometrieDroiteH.Text = "";
                    }
                }
                Double KeratometrieDroiteV = 0;

                if (txtKeratometrieDroiteV.Text != "")
                {
                    if (Double.TryParse(txtKeratometrieDroiteV.Text, out KeratometrieDroiteV))
                    {
                        KeratometrieDroiteV = Convert.ToDouble(txtKeratometrieDroiteV.Text);
                    }
                    else
                    {
                        txtKeratometrieDroiteV.Text = "";
                    }
                }
                moyDroite = (KeratometrieDroiteV + KeratometrieDroiteH) / 2;
                if (moyDroite != 0)
                {
                    txtKeratometrieDroitemoy.Text = moyDroite.ToString();
                    LentilleClass.KeratometrieDroitemoy = moyDroite.ToString();
                }
                else
                {
                    txtKeratometrieDroitemoy.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtKeratometrieGaucheH_TextChanged(object sender, TextChangedEventArgs e)
        {
            Double KeratometrieGaucheH = 0;
            Double moyGauche = 0;
            if (txtKeratometrieGaucheH.Text != "")
            {
                if (Double.TryParse(txtKeratometrieGaucheH.Text, out KeratometrieGaucheH))
                {
                    KeratometrieGaucheH = Convert.ToDouble(txtKeratometrieGaucheH.Text);
                }
                else
                {
                    txtKeratometrieGaucheH.Text = "";
                }
            }

            Double KeratometrieGaucheV = 0;

            if (txtKeratometrieGaucheV.Text != "")
            {
                if (Double.TryParse(txtKeratometrieGaucheV.Text, out KeratometrieGaucheV))
                {
                    KeratometrieGaucheV = Convert.ToDouble(txtKeratometrieGaucheV.Text);
                }
                else
                {
                    txtKeratometrieGaucheV.Text = "";
                }
            }
            moyGauche = (KeratometrieGaucheV + KeratometrieGaucheH) / 2;
            if (moyGauche != 0)
            {
                txtKeratometrieGauchemoy.Text = moyGauche.ToString();
                LentilleClass.KeratometrieGauchemoy = moyGauche.ToString();

            }
            else
            {
                txtKeratometrieGauchemoy.Text = "";

            }
        }
        private void txtDroitPrixLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteLentilleDroite, QuantiteLentilleGauche, PrixDroite, PrixGauche, PrixTotal = 0;
                if (txtDroitQuantiteLentille.Text != "")
                {
                    QuantiteLentilleDroite = Convert.ToDecimal(txtDroitQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleDroite = 0;
                }
                if (txtDroitPrixLentille.Text != "")
                {
                    PrixDroite = Convert.ToDecimal(txtDroitPrixLentille.Text);
                }
                else
                {
                    PrixDroite = 0;
                }
                /********************gauche************/
                if (txtGaucheQuantiteLentille.Text != "")
                {
                    QuantiteLentilleGauche = Convert.ToDecimal(txtGaucheQuantiteLentille.Text);
                }
                else
                {
                    QuantiteLentilleGauche = 0;
                }
                if (txtGauchePrixLentille.Text != "")
                {
                    PrixGauche = Convert.ToDecimal(txtGauchePrixLentille.Text);
                }
                else
                {
                    PrixGauche = 0;
                }


                PrixTotal = (PrixDroite * QuantiteLentilleDroite) + (PrixGauche * QuantiteLentilleGauche);


                txtPrixTotalLentille.Text = (PrixTotal).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtKeratometrieGaucheV_TextChanged(object sender, TextChangedEventArgs e)
        {
            Double KeratometrieGaucheH = 0;
            Double moyGauche = 0;
            if (txtKeratometrieGaucheH.Text != "")
            {
                if (Double.TryParse(txtKeratometrieGaucheH.Text, out KeratometrieGaucheH))
                {
                    KeratometrieGaucheH = Convert.ToDouble(txtKeratometrieGaucheH.Text);
                }
                else
                {
                    txtKeratometrieGaucheH.Text = "";
                }
            }

            Double KeratometrieGaucheV = 0;

            if (txtKeratometrieGaucheV.Text != "")
            {
                if (Double.TryParse(txtKeratometrieGaucheV.Text, out KeratometrieGaucheV))
                {
                    KeratometrieGaucheV = Convert.ToDouble(txtKeratometrieGaucheV.Text);
                }
                else
                {
                    txtKeratometrieGaucheV.Text = "";
                }
            }
            moyGauche = (KeratometrieGaucheV + KeratometrieGaucheH) / 2;
            if (moyGauche != 0)
            {
                txtKeratometrieGauchemoy.Text = moyGauche.ToString();
                LentilleClass.KeratometrieGauchemoy = moyGauche.ToString();
            }
            else
            {
                txtKeratometrieGauchemoy.Text = "";

            }
        }

        private void LentilleDatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (LentilleDatagrid.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {
                    LentilleClass = LentilleDatagrid.SelectedItem as SVC.LentilleClient;
                    GridLentille.DataContext = LentilleClass;
                    GridLentille.IsEnabled = true;

                    nouvellelentille = false;
                    anciennelentille = true;

                    if (LentilleClass.Encaissé != 0)
                    {
                        Lentilleversementzero = true;
                    }
                    else
                    {
                        Lentilleversementzero = false;
                    }
                    /************************************************/
                    if (LentilleClass.StatutVente == false)
                    {
                        btnventeLentille.IsEnabled = true;
                        txtMontantTotalENCLentille.IsEnabled = true;
                    }
                    else
                    {
                        btnventeLentille.IsEnabled = false;
                        txtMontantTotalENCLentille.IsEnabled = false;
                    }

                    if (LentilleClass.StatutDevis == true && LentilleClass.StatutVente == false)
                    {
                        TxtStatutGlobalLentille.Content = "Devis";

                        TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (LentilleClass.StatutDevis == true && LentilleClass.StatutVente == true)
                        {
                            TxtStatutGlobalLentille.Content = "Vente validée";
                            TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }






                    /**************************************************************/



                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }


        private void txtRemiseLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLentille, PrixAccessoires1, PrixAccessoires2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLentille.Text != "")
                {
                    PrixTotalLentille = Convert.ToDecimal(txtPrixTotalLentille.Text);
                }
                else
                {
                    PrixTotalLentille = 0;
                }
                if (txtPrixTotalAcc1Lentille.Text != "")
                {
                    PrixAccessoires1 = Convert.ToDecimal(txtPrixTotalAcc1Lentille.Text);
                }
                else
                {
                    PrixAccessoires1 = 0;
                }
                if (txtPrixTotalAcc2Lentille.Text != "")
                {
                    PrixAccessoires2 = Convert.ToDecimal(txtPrixTotalAcc2Lentille.Text);
                }
                else
                {
                    PrixAccessoires2 = 0;
                }

                montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                if (!String.IsNullOrEmpty(txtRemiseLentille.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseLentille.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2 - remisemonture);
                        }
                        else
                        {
                            txtRemiseLentille.Text = "";
                            montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                        }
                    }
                    else
                    {
                        txtRemiseLentille.Text = "";
                        montanttotal = (PrixTotalLentille + PrixAccessoires1 + PrixAccessoires2);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseLentille.Text = "";
                }

                txtMontantTotalLentille.Text = (montanttotal).ToString();
                LentilleClass.MontantTotal = Convert.ToDecimal(txtMontantTotalLentille.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void creationstocklentille()
        {
            try
            {
                factureselectedl = new List<SVC.Facture>();
                #region accessoires1
                if (LentilleClass.IdAccessoires1 != 0 && LentilleClass.AccessoiresQuantite1 > 0)
                {
                    if (LentilleClass.DroiteStatutAccessoires1 == 0)
                    {
                        SVC.Prodf selectedtropuvé = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdAccessoires1)).First();
                        if (selectedtropuvé.quantite > 0 && selectedtropuvé.quantite >= LentilleClass.AccessoiresQuantite1)
                        {
                            if (selectedtropuvé.perempt != null)
                            {
                                if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        dates = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = LentilleClass.AccessoiresPrix1,
                                        quantite = LentilleClass.AccessoiresQuantite1,
                                        Total = LentilleClass.AccessoiresPrix1 * LentilleClass.AccessoiresQuantite1,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        codeclient = Clientvv.Id,
                                        Client = Clientvv.Raison,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {


                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {
                                        found.quantite = found.quantite + LentilleClass.AccessoiresQuantite1;
                                        found.Total = found.Total + LentilleClass.AccessoiresQuantite1 * LentilleClass.AccessoiresPrix1;



                                    }


                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.cp,
                                    dates = DateTime.Now,
                                    datef = selectedtropuvé.datef,
                                    design = selectedtropuvé.design,
                                    lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    perempt = selectedtropuvé.perempt,
                                    tva = selectedtropuvé.tva,
                                    previent = selectedtropuvé.previent,
                                    privente = LentilleClass.AccessoiresPrix1,
                                    quantite = LentilleClass.AccessoiresQuantite1,
                                    Total = LentilleClass.AccessoiresPrix1 * LentilleClass.AccessoiresQuantite1,
                                    collisage = selectedtropuvé.collisage,
                                    ficheproduit = selectedtropuvé.Id,
                                    ff = selectedtropuvé.nfact,
                                    Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,


                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {


                                    found.quantite = found.quantite + LentilleClass.AccessoiresQuantite1;
                                    found.Total = found.Total + LentilleClass.AccessoiresQuantite1 * LentilleClass.AccessoiresPrix1;

                                }


                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        if (LentilleClass.DroiteStatutAccessoires1 == 1)
                        {
                            var existeaccessoire1 = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdAccessoires1)).Where(n => n.quantite != 0).Any();
                            if (existeaccessoire1 == true)
                            {
                                var selectedtropuvé = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdAccessoires1)).Where(n => n.quantite != 0).First();
                                if (selectedtropuvé.quantite > 0 && selectedtropuvé.quantite >= LentilleClass.AccessoiresQuantite1)
                                {
                                    if (selectedtropuvé.perempt != null)
                                    {
                                        if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                        {
                                            SVC.Facture facturevente = new SVC.Facture
                                            {
                                                cab = selectedtropuvé.cab,
                                                cf = selectedtropuvé.cf,
                                                codeprod = selectedtropuvé.cp,
                                                dates = DateTime.Now,
                                                datef = selectedtropuvé.datef,
                                                design = selectedtropuvé.design,
                                                lot = selectedtropuvé.lot,
                                                oper = memberuser.Username,
                                                perempt = selectedtropuvé.perempt,
                                                tva = selectedtropuvé.tva,
                                                previent = selectedtropuvé.previent,
                                                privente = LentilleClass.AccessoiresPrix1,
                                                quantite = LentilleClass.AccessoiresQuantite1,
                                                Total = LentilleClass.AccessoiresPrix1 * LentilleClass.AccessoiresQuantite1,
                                                ficheproduit = selectedtropuvé.Id,
                                                ff = selectedtropuvé.nfact,
                                                Fournisseur = selectedtropuvé.fourn,
                                                codeclient = Clientvv.Id,
                                                Client = Clientvv.Raison,
                                                collisage = selectedtropuvé.collisage,
                                            };
                                            var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                            if (found == null)
                                            {


                                                factureselectedl.Add(facturevente);

                                            }
                                            else
                                            {
                                                found.quantite = found.quantite + LentilleClass.AccessoiresQuantite1;
                                                found.Total = found.Total + LentilleClass.AccessoiresQuantite1 * LentilleClass.AccessoiresPrix1;



                                            }


                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                    else
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé.previent,
                                            privente = LentilleClass.AccessoiresPrix1,
                                            quantite = LentilleClass.AccessoiresQuantite1,
                                            Total = LentilleClass.AccessoiresPrix1 * LentilleClass.AccessoiresQuantite1,
                                            collisage = selectedtropuvé.collisage,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,


                                        };
                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {

                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {


                                            found.quantite = found.quantite + LentilleClass.AccessoiresQuantite1;
                                            found.Total = found.Total + LentilleClass.AccessoiresQuantite1 * LentilleClass.AccessoiresPrix1;

                                        }


                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }


                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.Accessoires1.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);

                            }

                        }
                    }
                }
                #endregion
                #region accessoires2
                if (LentilleClass.IdAccessoires2 != 0 && LentilleClass.AccessoiresQuantite2 > 0)
                {
                    if (LentilleClass.DroiteStatutAccessoires2 == 0)
                    {
                        SVC.Prodf selectedtropuvé = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdAccessoires2)).First();
                        if (selectedtropuvé.quantite > 0 && selectedtropuvé.quantite >= LentilleClass.AccessoiresQuantite2)
                        {
                            if (selectedtropuvé.perempt != null)
                            {
                                if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        dates = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = LentilleClass.AccessoiresPrix2,
                                        quantite = LentilleClass.AccessoiresQuantite2,
                                        Total = LentilleClass.AccessoiresPrix2 * LentilleClass.AccessoiresQuantite2,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        codeclient = Clientvv.Id,
                                        Client = Clientvv.Raison,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {


                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {


                                        found.quantite = found.quantite + LentilleClass.AccessoiresQuantite2;
                                        found.Total = found.Total + LentilleClass.AccessoiresQuantite2 * LentilleClass.AccessoiresPrix2;






                                    }


                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.cp,
                                    dates = DateTime.Now,
                                    datef = selectedtropuvé.datef,
                                    design = selectedtropuvé.design,
                                    lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    perempt = selectedtropuvé.perempt,
                                    tva = selectedtropuvé.tva,
                                    previent = selectedtropuvé.previent,
                                    privente = LentilleClass.AccessoiresPrix2,
                                    quantite = LentilleClass.AccessoiresQuantite2,
                                    Total = LentilleClass.AccessoiresPrix2 * LentilleClass.AccessoiresQuantite2,
                                    ficheproduit = selectedtropuvé.Id,
                                    ff = selectedtropuvé.nfact,
                                    Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,

                                    collisage = selectedtropuvé.collisage,
                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {

                                    found.quantite = found.quantite + LentilleClass.AccessoiresQuantite2;
                                    found.Total = found.Total + LentilleClass.AccessoiresQuantite2 * LentilleClass.AccessoiresPrix2;


                                }


                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        if (LentilleClass.DroiteStatutAccessoires2 == 1)
                        {
                            var existeaccesoire1 = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdAccessoires2)).Where(n => n.quantite != 0).Any();
                            if (existeaccesoire1 == true)
                            {
                                var selectedtropuvé = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdAccessoires2)).Where(n => n.quantite != 0).First();
                                if (selectedtropuvé.quantite > 0 && selectedtropuvé.quantite >= LentilleClass.AccessoiresQuantite2)
                                {
                                    if (selectedtropuvé.perempt != null)
                                    {
                                        if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                        {
                                            SVC.Facture facturevente = new SVC.Facture
                                            {
                                                cab = selectedtropuvé.cab,
                                                cf = selectedtropuvé.cf,
                                                codeprod = selectedtropuvé.cp,
                                                dates = DateTime.Now,
                                                datef = selectedtropuvé.datef,
                                                design = selectedtropuvé.design,
                                                lot = selectedtropuvé.lot,
                                                oper = memberuser.Username,
                                                perempt = selectedtropuvé.perempt,
                                                tva = selectedtropuvé.tva,
                                                previent = selectedtropuvé.previent,
                                                privente = LentilleClass.AccessoiresPrix2,
                                                quantite = LentilleClass.AccessoiresQuantite2,
                                                Total = LentilleClass.AccessoiresPrix2 * LentilleClass.AccessoiresQuantite2,
                                                ficheproduit = selectedtropuvé.Id,
                                                ff = selectedtropuvé.nfact,
                                                Fournisseur = selectedtropuvé.fourn,
                                                codeclient = Clientvv.Id,
                                                Client = Clientvv.Raison,
                                                collisage = selectedtropuvé.collisage,
                                            };
                                            var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                            if (found == null)
                                            {


                                                factureselectedl.Add(facturevente);

                                            }
                                            else
                                            {


                                                found.quantite = found.quantite + LentilleClass.AccessoiresQuantite2;
                                                found.Total = found.Total + LentilleClass.AccessoiresQuantite2 * LentilleClass.AccessoiresPrix2;






                                            }


                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                    else
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé.previent,
                                            privente = LentilleClass.AccessoiresPrix2,
                                            quantite = LentilleClass.AccessoiresQuantite2,
                                            Total = LentilleClass.AccessoiresPrix2 * LentilleClass.AccessoiresQuantite2,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,
                                            collisage = selectedtropuvé.collisage,

                                        };
                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {

                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {

                                            found.quantite = found.quantite + LentilleClass.AccessoiresQuantite2;
                                            found.Total = found.Total + LentilleClass.AccessoiresQuantite2 * LentilleClass.AccessoiresPrix2;


                                        }


                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.Accessoires1.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.Accessoires2.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);

                        }

                    }
                }
                #endregion
                /*******************************************************************/
                #region Lentille Droite
                if (LentilleClass.IdDroiteLentille != 0 && LentilleClass.DroitQuantiteLentille != 0)
                {
                    // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.DroiteStatutLentille.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    if (LentilleClass.DroiteStatutLentille == 0)
                    {

                        SVC.Prodf selectedtropuvé = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdDroiteLentille)).First();
                        if (selectedtropuvé.quantite > 0)
                        {
                            if (selectedtropuvé.perempt != null)
                            {
                                if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        dates = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = LentilleClass.DroitPrixLentille,
                                        quantite = LentilleClass.DroitQuantiteLentille,
                                        Total = LentilleClass.DroitPrixLentille * LentilleClass.DroitQuantiteLentille,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        codeclient = Clientvv.Id,
                                        Client = Clientvv.Raison,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {


                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {


                                        found.quantite = found.quantite + LentilleClass.DroitQuantiteLentille;
                                        found.Total = found.Total + LentilleClass.DroitQuantiteLentille * LentilleClass.DroitPrixLentille;






                                    }


                                }
                                else
                                {
                                    MessageBoxResult resdult = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.cp,
                                    dates = DateTime.Now,
                                    datef = selectedtropuvé.datef,
                                    design = selectedtropuvé.design,
                                    lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    perempt = selectedtropuvé.perempt,
                                    tva = selectedtropuvé.tva,
                                    previent = selectedtropuvé.previent,
                                    privente = LentilleClass.DroitPrixLentille,
                                    quantite = LentilleClass.DroitQuantiteLentille,
                                    Total = LentilleClass.DroitPrixLentille * LentilleClass.DroitQuantiteLentille,
                                    ficheproduit = selectedtropuvé.Id,
                                    ff = selectedtropuvé.nfact,
                                    Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,
                                    collisage = selectedtropuvé.collisage,

                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);
                                    //     MessageBoxResult resudlt = Xceed.Wpf.Toolkit.MessageBox.Show(facturevente.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {

                                    found.quantite = found.quantite + LentilleClass.DroitQuantiteLentille;
                                    found.Total = found.Total + LentilleClass.DroitQuantiteLentille * LentilleClass.DroitPrixLentille;


                                }


                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        if (LentilleClass.DroiteStatutLentille == 1)
                        {
                            var existedroitelentille = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdDroiteLentille)).Where(n => n.quantite != 0).Any();
                            if (existedroitelentille == true)
                            {
                                var selectedtropuvé = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdDroiteLentille)).Where(n => n.quantite != 0).First();
                                if (selectedtropuvé.quantite > 0)
                                {
                                    if (selectedtropuvé.perempt != null)
                                    {
                                        if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                        {
                                            SVC.Facture facturevente = new SVC.Facture
                                            {
                                                cab = selectedtropuvé.cab,
                                                cf = selectedtropuvé.cf,
                                                codeprod = selectedtropuvé.cp,
                                                dates = DateTime.Now,
                                                datef = selectedtropuvé.datef,
                                                design = selectedtropuvé.design,
                                                lot = selectedtropuvé.lot,
                                                oper = memberuser.Username,
                                                perempt = selectedtropuvé.perempt,
                                                tva = selectedtropuvé.tva,
                                                previent = selectedtropuvé.previent,
                                                privente = LentilleClass.DroitPrixLentille,
                                                quantite = LentilleClass.DroitQuantiteLentille,
                                                Total = LentilleClass.DroitPrixLentille * LentilleClass.DroitQuantiteLentille,
                                                ficheproduit = selectedtropuvé.Id,
                                                ff = selectedtropuvé.nfact,
                                                Fournisseur = selectedtropuvé.fourn,
                                                codeclient = Clientvv.Id,
                                                Client = Clientvv.Raison,
                                                collisage = selectedtropuvé.collisage,
                                            };
                                            var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                            if (found == null)
                                            {


                                                factureselectedl.Add(facturevente);

                                            }
                                            else
                                            {


                                                found.quantite = found.quantite + LentilleClass.DroitQuantiteLentille;
                                                found.Total = found.Total + LentilleClass.DroitQuantiteLentille * LentilleClass.DroitPrixLentille;






                                            }


                                        }
                                        else
                                        {
                                            MessageBoxResult redsult = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                    else
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé.previent,
                                            privente = LentilleClass.DroitPrixLentille,
                                            quantite = LentilleClass.DroitQuantiteLentille,
                                            Total = LentilleClass.DroitPrixLentille * LentilleClass.DroitQuantiteLentille,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,
                                            collisage = selectedtropuvé.collisage,

                                        };
                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {

                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {

                                            found.quantite = found.quantite + LentilleClass.DroitQuantiteLentille;
                                            found.Total = found.Total + LentilleClass.DroitQuantiteLentille * LentilleClass.DroitPrixLentille;


                                        }


                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                MessageBoxResult resudflt = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.DroiteLentilleDesignation.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);

                            }
                        }
                    }
                }
                #endregion
                /********************************************************************/
                #region lentille gauche
                if (LentilleClass.IdGaucheLentille != 0 && LentilleClass.GaucheQuantiteLentille != 0)
                {
                    if (LentilleClass.GaucheStatutLentille == 0)
                    {
                        SVC.Prodf selectedtropuvé = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdGaucheLentille)).First();

                        if (selectedtropuvé.quantite > 0)
                        {
                            if (selectedtropuvé.perempt != null)
                            {
                                if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        dates = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = LentilleClass.GauchePrixLentille,
                                        quantite = LentilleClass.GaucheQuantiteLentille,
                                        Total = LentilleClass.GauchePrixLentille * LentilleClass.GaucheQuantiteLentille,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        codeclient = Clientvv.Id,
                                        Client = Clientvv.Raison,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {


                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {


                                        found.quantite = found.quantite + LentilleClass.GaucheQuantiteLentille;
                                        found.Total = found.Total + LentilleClass.GaucheQuantiteLentille * LentilleClass.GauchePrixLentille;






                                    }


                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.cp,
                                    dates = DateTime.Now,
                                    datef = selectedtropuvé.datef,
                                    design = selectedtropuvé.design,
                                    lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    perempt = selectedtropuvé.perempt,
                                    tva = selectedtropuvé.tva,
                                    previent = selectedtropuvé.previent,
                                    privente = LentilleClass.GauchePrixLentille,
                                    quantite = LentilleClass.GaucheQuantiteLentille,
                                    Total = LentilleClass.GauchePrixLentille * LentilleClass.GaucheQuantiteLentille,
                                    ficheproduit = selectedtropuvé.Id,
                                    ff = selectedtropuvé.nfact,
                                    Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,
                                    collisage = selectedtropuvé.collisage,

                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {

                                    found.quantite = found.quantite + LentilleClass.GaucheQuantiteLentille;
                                    found.Total = found.Total + LentilleClass.GaucheQuantiteLentille * LentilleClass.GauchePrixLentille;


                                }


                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        if (LentilleClass.GaucheStatutLentille == 1)
                        {
                            var existegachelentille = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdGaucheLentille)).Where(n => n.quantite != 0).Any();
                            if (existegachelentille == true)
                            {
                                var selectedtropuvé = proxy.GetAllProdfbycode(Convert.ToInt32(LentilleClass.IdGaucheLentille)).Where(n => n.quantite != 0).First();

                                if (selectedtropuvé.quantite > 0)
                                {
                                    if (selectedtropuvé.perempt != null)
                                    {
                                        if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                        {
                                            SVC.Facture facturevente = new SVC.Facture
                                            {
                                                cab = selectedtropuvé.cab,
                                                cf = selectedtropuvé.cf,
                                                codeprod = selectedtropuvé.cp,
                                                dates = DateTime.Now,
                                                datef = selectedtropuvé.datef,
                                                design = selectedtropuvé.design,
                                                lot = selectedtropuvé.lot,
                                                oper = memberuser.Username,
                                                perempt = selectedtropuvé.perempt,
                                                tva = selectedtropuvé.tva,
                                                previent = selectedtropuvé.previent,
                                                privente = LentilleClass.GauchePrixLentille,
                                                quantite = LentilleClass.GaucheQuantiteLentille,
                                                Total = LentilleClass.GauchePrixLentille * LentilleClass.GaucheQuantiteLentille,
                                                ficheproduit = selectedtropuvé.Id,
                                                ff = selectedtropuvé.nfact,
                                                Fournisseur = selectedtropuvé.fourn,
                                                codeclient = Clientvv.Id,
                                                Client = Clientvv.Raison,
                                                collisage = selectedtropuvé.collisage,
                                            };
                                            var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                            if (found == null)
                                            {


                                                factureselectedl.Add(facturevente);

                                            }
                                            else
                                            {


                                                found.quantite = found.quantite + LentilleClass.GaucheQuantiteLentille;
                                                found.Total = found.Total + LentilleClass.GaucheQuantiteLentille * LentilleClass.GauchePrixLentille;






                                            }


                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                    else
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé.previent,
                                            privente = LentilleClass.GauchePrixLentille,
                                            quantite = LentilleClass.GaucheQuantiteLentille,
                                            Total = LentilleClass.GauchePrixLentille * LentilleClass.GaucheQuantiteLentille,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,
                                            collisage = selectedtropuvé.collisage,

                                        };
                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {

                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {

                                            found.quantite = found.quantite + LentilleClass.GaucheQuantiteLentille;
                                            found.Total = found.Total + LentilleClass.GaucheQuantiteLentille * LentilleClass.GauchePrixLentille;


                                        }


                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Le produit " + selectedtropuvé.design + " avec quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }

                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.GaucheLentilleDesignation.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                #endregion
                /********************************************************************/

                ReceptDatagrid.ItemsSource = factureselectedl;
                ReceptDatagrid.DataContext = factureselectedl;

                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void FacturationDesign()
        {
            Grid.SetColumnSpan(RéglemdfentGfsdfsrid, 1);
            Grid.SetRowSpan(ListeDesDocuments, 3);
            WindowBorderFacture.Visibility = Visibility.Visible;
            gridspl.Visibility = Visibility.Visible;
            NomenclatureDatagrid.Visibility = Visibility.Visible;
            girdspi1.Visibility = Visibility.Visible;
            WindowBorderFacture.IsEnabled = true;
            NomenclatureProduit.ItemsSource = (proxy.GetAllProdf().OrderBy(n => n.design));
            factureopern = true;
            BtnCreerProduit.Visibility = Visibility.Collapsed;
            FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
        }
        void creationSansStockLentille()
        {
            try
            {
                factureselectedl = new List<SVC.Facture>();
                #region accessoires1
                if (LentilleClass.IdAccessoires1 != 0 && LentilleClass.AccessoiresQuantite1 > 0)
                {
                    if (LentilleClass.DroiteStatutAccessoires1 == 0)
                    {

                        SVC.Prodf selectedtropuvé1 = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdAccessoires1)).First();
                        SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé1.cp)).First();

                        SVC.Facture facturevente = new SVC.Facture
                        {
                            cab = selectedtropuvé.cab,
                            // cf = selectedtropuvé.cf,
                            codeprod = selectedtropuvé.Id,
                            dates = DateTime.Now,
                            datef = DateTime.Now,
                            design = selectedtropuvé.design,
                            //   lot = selectedtropuvé.lot,
                            oper = memberuser.Username,
                            // perempt = selectedtropuvé.perempt,
                            tva = 0,
                            previent = selectedtropuvé.PrixRevient,
                            privente = LentilleClass.AccessoiresPrix1,
                            quantite = LentilleClass.AccessoiresQuantite1,
                            Total = LentilleClass.AccessoiresPrix1 * LentilleClass.AccessoiresQuantite1,
                            collisage = 1,
                            ficheproduit = selectedtropuvé.Id,
                            //ff = selectedtropuvé.nfact,
                            // Fournisseur = selectedtropuvé.fourn,
                            codeclient = Clientvv.Id,
                            Client = Clientvv.Raison,
                            Auto = true,

                        };
                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                        if (found == null)
                        {

                            factureselectedl.Add(facturevente);

                        }
                        else
                        {


                            found.quantite = found.quantite + LentilleClass.AccessoiresQuantite1;
                            found.Total = found.Total + LentilleClass.AccessoiresQuantite1 * LentilleClass.AccessoiresPrix1;

                        }



                    }
                    else
                    {
                        if (LentilleClass.DroiteStatutAccessoires1 == 1)
                        {
                            var existeaccessoire1 = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdAccessoires1)).Any();
                            if (existeaccessoire1 == true)
                            {
                                SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdAccessoires1)).First();

                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    //     cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.Id,
                                    dates = DateTime.Now,
                                    datef = DateTime.Now,
                                    design = selectedtropuvé.design,
                                    // lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    //  perempt = selectedtropuvé.perempt,
                                    tva = 0,
                                    previent = selectedtropuvé.PrixRevient,
                                    privente = LentilleClass.AccessoiresPrix1,
                                    quantite = LentilleClass.AccessoiresQuantite1,
                                    Total = LentilleClass.AccessoiresPrix1 * LentilleClass.AccessoiresQuantite1,
                                    collisage = 1,
                                    ficheproduit = selectedtropuvé.Id,
                                    //  ff = selectedtropuvé.nfact,
                                    //  Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,

                                    Auto = true,
                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {


                                    found.quantite = found.quantite + LentilleClass.AccessoiresQuantite1;
                                    found.Total = found.Total + LentilleClass.AccessoiresQuantite1 * LentilleClass.AccessoiresPrix1;

                                }




                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.Accessoires1.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);

                            }

                        }
                    }
                }
                #endregion
                #region accessoires2
                if (LentilleClass.IdAccessoires2 != 0 && LentilleClass.AccessoiresQuantite2 > 0)
                {
                    if (LentilleClass.DroiteStatutAccessoires2 == 0)
                    {

                        SVC.Prodf selectedtropuvé1 = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdAccessoires2)).First();
                        SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé1.cp)).First();

                        SVC.Facture facturevente = new SVC.Facture
                        {
                            cab = selectedtropuvé.cab,
                            //cf = selectedtropuvé.cf,
                            codeprod = selectedtropuvé.Id,
                            dates = DateTime.Now,
                            datef = DateTime.Now,
                            design = selectedtropuvé.design,
                            //   lot = selectedtropuvé.lot,
                            oper = memberuser.Username,
                            //  perempt = selectedtropuvé.perempt,
                            tva = 0,
                            previent = selectedtropuvé.PrixRevient,
                            privente = LentilleClass.AccessoiresPrix2,
                            quantite = LentilleClass.AccessoiresQuantite2,
                            Total = LentilleClass.AccessoiresPrix2 * LentilleClass.AccessoiresQuantite2,
                            ficheproduit = selectedtropuvé.Id,
                            //  ff = selectedtropuvé.nfact,
                            //  Fournisseur = selectedtropuvé.fourn,
                            codeclient = Clientvv.Id,
                            Client = Clientvv.Raison,
                            Auto = true,
                            collisage = 1,
                        };
                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                        if (found == null)
                        {

                            factureselectedl.Add(facturevente);

                        }
                        else
                        {

                            found.quantite = found.quantite + LentilleClass.AccessoiresQuantite2;
                            found.Total = found.Total + LentilleClass.AccessoiresQuantite2 * LentilleClass.AccessoiresPrix2;


                        }



                    }
                    else
                    {
                        if (LentilleClass.DroiteStatutAccessoires2 == 1)
                        {
                            var existeaccesoire1 = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdAccessoires2)).Any();
                            if (existeaccesoire1 == true)
                            {
                                SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdAccessoires2)).First();

                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    //cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.Id,
                                    dates = DateTime.Now,
                                    datef = DateTime.Now,
                                    design = selectedtropuvé.design,
                                    //   lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    //  perempt = selectedtropuvé.perempt,
                                    tva = 0,
                                    previent = selectedtropuvé.PrixRevient,
                                    privente = LentilleClass.AccessoiresPrix2,
                                    quantite = LentilleClass.AccessoiresQuantite2,
                                    Total = LentilleClass.AccessoiresPrix2 * LentilleClass.AccessoiresQuantite2,
                                    ficheproduit = selectedtropuvé.Id,
                                    //   ff = selectedtropuvé.nfact,
                                    //  Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,
                                    collisage = 1,

                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {

                                    found.quantite = found.quantite + LentilleClass.AccessoiresQuantite2;
                                    found.Total = found.Total + LentilleClass.AccessoiresQuantite2 * LentilleClass.AccessoiresPrix2;


                                }



                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.Accessoires1.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.Accessoires2.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);

                        }

                    }
                }
                #endregion
                /*******************************************************************/
                #region Lentille Droite
                if (LentilleClass.IdDroiteLentille != 0 && LentilleClass.DroitQuantiteLentille != 0)
                {
                    // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.DroiteStatutLentille.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    if (LentilleClass.DroiteStatutLentille == 0)
                    {


                        SVC.Prodf selectedtropuvé1 = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdDroiteLentille)).First();
                        SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé1.cp)).First();

                        SVC.Facture facturevente = new SVC.Facture
                        {
                            cab = selectedtropuvé.cab,
                            //  cf = selectedtropuvé.cf,
                            codeprod = selectedtropuvé.Id,
                            dates = DateTime.Now,
                            datef = DateTime.Now,
                            design = selectedtropuvé.design,
                            // lot = selectedtropuvé.lot,
                            oper = memberuser.Username,
                            //  perempt = selectedtropuvé.perempt,
                            tva = 0,
                            previent = selectedtropuvé.PrixRevient,
                            privente = LentilleClass.DroitPrixLentille,
                            quantite = LentilleClass.DroitQuantiteLentille,
                            Total = LentilleClass.DroitPrixLentille * LentilleClass.DroitQuantiteLentille,
                            ficheproduit = selectedtropuvé.Id,
                            //   ff = selectedtropuvé.nfact,
                            //  Fournisseur = selectedtropuvé.fourn,
                            codeclient = Clientvv.Id,
                            Client = Clientvv.Raison,
                            collisage = 1,

                        };
                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                        if (found == null)
                        {

                            factureselectedl.Add(facturevente);
                            //     MessageBoxResult resudlt = Xceed.Wpf.Toolkit.MessageBox.Show(facturevente.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {

                            found.quantite = found.quantite + LentilleClass.DroitQuantiteLentille;
                            found.Total = found.Total + LentilleClass.DroitQuantiteLentille * LentilleClass.DroitPrixLentille;


                        }



                    }
                    else
                    {
                        if (LentilleClass.DroiteStatutLentille == 1)
                        {
                            var existedroitelentille = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdDroiteLentille)).Any();
                            if (existedroitelentille == true)
                            {
                                SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdDroiteLentille)).First();

                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    //cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.Id,
                                    dates = DateTime.Now,
                                    datef = DateTime.Now,
                                    design = selectedtropuvé.design,
                                    // lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    //   perempt = selectedtropuvé.perempt,
                                    tva = 0,
                                    previent = selectedtropuvé.PrixRevient,
                                    privente = LentilleClass.DroitPrixLentille,
                                    quantite = LentilleClass.DroitQuantiteLentille,
                                    Total = LentilleClass.DroitPrixLentille * LentilleClass.DroitQuantiteLentille,
                                    ficheproduit = selectedtropuvé.Id,
                                    //   ff = selectedtropuvé.nfact,
                                    //  Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,
                                    collisage = 1,

                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {

                                    found.quantite = found.quantite + LentilleClass.DroitQuantiteLentille;
                                    found.Total = found.Total + LentilleClass.DroitQuantiteLentille * LentilleClass.DroitPrixLentille;


                                }



                            }
                            else
                            {
                                MessageBoxResult resudflt = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.DroiteLentilleDesignation.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);

                            }
                        }
                    }
                }
                #endregion
                /********************************************************************/
                #region lentille gauche
                if (LentilleClass.IdGaucheLentille != 0 && LentilleClass.GaucheQuantiteLentille != 0)
                {
                    if (LentilleClass.GaucheStatutLentille == 0)
                    {

                        SVC.Prodf selectedtropuvé1 = proxy.GetAllProdfbyfiche(Convert.ToInt32(LentilleClass.IdGaucheLentille)).First();
                        SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé1.cp)).First();


                        SVC.Facture facturevente = new SVC.Facture
                        {
                            cab = selectedtropuvé.cab,
                            //   cf = selectedtropuvé.cf,
                            codeprod = selectedtropuvé.Id,
                            dates = DateTime.Now,
                            datef = DateTime.Now,
                            design = selectedtropuvé.design,
                            //  lot = selectedtropuvé.lot,
                            oper = memberuser.Username,
                            //  perempt = selectedtropuvé.perempt,
                            tva = 0,
                            previent = selectedtropuvé.PrixRevient,
                            privente = LentilleClass.GauchePrixLentille,
                            quantite = LentilleClass.GaucheQuantiteLentille,
                            Total = LentilleClass.GauchePrixLentille * LentilleClass.GaucheQuantiteLentille,
                            ficheproduit = selectedtropuvé.Id,
                            //  ff = selectedtropuvé.nfact,
                            //    Fournisseur = selectedtropuvé.fourn,
                            codeclient = Clientvv.Id,
                            Client = Clientvv.Raison,
                            collisage = 1,

                        };
                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                        if (found == null)
                        {

                            factureselectedl.Add(facturevente);

                        }
                        else
                        {

                            found.quantite = found.quantite + LentilleClass.GaucheQuantiteLentille;
                            found.Total = found.Total + LentilleClass.GaucheQuantiteLentille * LentilleClass.GauchePrixLentille;


                        }



                    }
                    else
                    {
                        if (LentilleClass.GaucheStatutLentille == 1)
                        {
                            var existegachelentille = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdGaucheLentille)).Any();
                            if (existegachelentille == true)
                            {
                                SVC.Produit selectedtropuvé = proxy.GetAllProduitbyid(Convert.ToInt32(LentilleClass.IdGaucheLentille)).First();

                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    // cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.Id,
                                    dates = DateTime.Now,
                                    datef = DateTime.Now,
                                    design = selectedtropuvé.design,
                                    //   lot = selectedtropuvé.lot,
                                    oper = memberuser.Username,
                                    // perempt = selectedtropuvé.perempt,
                                    tva = 0,
                                    previent = selectedtropuvé.PrixRevient,
                                    privente = LentilleClass.GauchePrixLentille,
                                    quantite = LentilleClass.GaucheQuantiteLentille,
                                    Total = LentilleClass.GauchePrixLentille * LentilleClass.GaucheQuantiteLentille,
                                    ficheproduit = selectedtropuvé.Id,
                                    // ff = selectedtropuvé.nfact,
                                    //  Fournisseur = selectedtropuvé.fourn,
                                    codeclient = Clientvv.Id,
                                    Client = Clientvv.Raison,
                                    collisage = 1,

                                };
                                var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {

                                    factureselectedl.Add(facturevente);

                                }
                                else
                                {

                                    found.quantite = found.quantite + LentilleClass.GaucheQuantiteLentille;
                                    found.Total = found.Total + LentilleClass.GaucheQuantiteLentille * LentilleClass.GauchePrixLentille;


                                }



                            }
                            else
                            {
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(LentilleClass.GaucheLentilleDesignation.ToString() + " n'existe pas dans le stock vente impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                #endregion
                /********************************************************************/

                ReceptDatagrid.ItemsSource = factureselectedl;
                ReceptDatagrid.DataContext = factureselectedl;

                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void FacturationDesignNonStock()
        {

            var dgtc = NomenclatureProduit.Columns[1] as DataGridTextColumn;
            dgtc.Binding = new System.Windows.Data.Binding("PrixVente");
            var dgtc3 = NomenclatureProduit.Columns[3] as DataGridTextColumn;
            dgtc3.Binding = new System.Windows.Data.Binding("PrixRevient");
            Grid.SetColumnSpan(RéglemdfentGfsdfsrid, 1);
            Grid.SetRowSpan(ListeDesDocuments, 3);
            WindowBorderFacture.Visibility = Visibility.Visible;
            gridspl.Visibility = Visibility.Visible;
            NomenclatureDatagrid.Visibility = Visibility.Visible;
            girdspi1.Visibility = Visibility.Visible;
            WindowBorderFacture.IsEnabled = true;
            NomenclatureProduit.ItemsSource = (proxy.GetAllProduit().OrderBy(n => n.design));
            factureopern = true;
            BtnCreerProduit.Visibility = Visibility.Visible;
            FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
        }

        private void ConfirmerDocument1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationDossierClient == true)
                {
                    if (facturenonstock != true)
                    {




                        string cle = "";
                        bool monture = false;
                        bool lentille = false;
                        decimal versementdossier = 0; decimal remise = 0;

                         
                            if (interfacefacturation == 2)
                            {
                                creationstocklentille();
                                cle = LentilleClass.Cle;
                                monture = false;
                                lentille = true;
                                versementdossier = Convert.ToDecimal(LentilleClass.Encaissé);
                                remise = Convert.ToDecimal(LentilleClass.Remise);
                            }
                            else
                            {
                                remise = 0;
                            }
                       


                        nouveauF1 = new SVC.F1
                        {
                            codeclient = Clientvv.Id,
                            date = DateTime.Now,
                            dates = DateTime.Now,
                            raison = Clientvv.Raison,
                            modep = "A TERME",
                            oper = memberuser.Username,
                            cleDossier = cle,
                            Monture = monture,
                            Lentille = lentille,
                            versement = versementdossier,
                            // ht= 0,
                            // net= 0,
                            /*  timbre = 0,
                              echeance = 0,
                              ht = 0,
                              net = 0,

                              tva = 0,
                              versement = 0,
                              reste = 0,
                              */
                            remise = remise,
                            Auto = false,
                        };
                        tsdffgent.IsSelected = true;

                        //    selectchanged();
                        WindowBorderFacture.DataContext = nouveauF1;
                        if (fermer == true)
                        {
                            txtnVersement.IsEnabled = true;
                            FacturationDesign();
                            ReceptDatagrid.DataContext = factureselectedl;
                            ReceptDatagrid.ItemsSource = factureselectedl;
                            ReceptDatagrid.IsEnabled = true;
                            txtTTC.Text = "0";
                            txtnfact.IsEnabled = false;


                            if (documenttype == 3)
                            {
                                NomDocumentLabel.Content = "bon de livraison";
                                chBonLivraison.IsChecked = true;
                                txtnVersement.IsEnabled = true;
                                selectedparam = proxy.GetAllParamétre();

                                if (selectedparam.AffichPrixAchatVente == true)
                                {
                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                }
                                if (selectedparam.ModiPrix == true)
                                {
                                    ReceptDatagrid.Columns[2].IsReadOnly = false;
                                }
                                else
                                {
                                    ReceptDatagrid.Columns[2].IsReadOnly = true;
                                }
                                if (selectedparam.modidate == true)
                                {
                                    txtDateOper.IsEnabled = true;
                                }
                                if (selectedparam.affiben == true)
                                {
                                    Bénéfice.Visibility = Visibility.Visible;
                                    Bénéficemont.Visibility = Visibility.Visible;

                                }
                            }
                            else
                            {


                                if (documenttype == 1)
                                {
                                    NomDocumentLabel.Content = "Nouvelle Facture";
                                    chFacture.IsChecked = true;
                                    txtnVersement.IsEnabled = true;
                                    selectedparam = proxy.GetAllParamétre();

                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;
                                        //   Bénéficemont.Text = Convert.ToString(((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                    }
                                }
                                else
                                {
                                    if (documenttype == 5)
                                    {
                                        NomDocumentLabel.Content = "Nouvelle Proforma";
                                        chProforma.IsChecked = true;
                                        txtnVersement.IsEnabled = false;
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                        txtDateOper.IsEnabled = true;


                                    }
                                }
                            }



                            //    txtRemise.Text=remise.ToString();
                            ///    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(interfacefacturation.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            //    calculef1();
                            dialog1.Close();

                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un document de vente", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                    else
                    {



                        string cle = "";
                        bool monture = false;
                        bool lentille = false;
                        decimal versementdossier = 0; decimal remise = 0;

                         
                            if (interfacefacturation == 2)
                            {
                                creationSansStockLentille();
                                cle = LentilleClass.Cle;
                                monture = false;
                                lentille = true;
                                versementdossier = Convert.ToDecimal(LentilleClass.Encaissé);
                                remise = Convert.ToDecimal(LentilleClass.Remise);
                            }
                            else
                            {
                                remise = 0;
                            }
                     


                        nouveauF1 = new SVC.F1
                        {
                            codeclient = Clientvv.Id,
                            date = DateTime.Now,
                            dates = DateTime.Now,
                            raison = Clientvv.Raison,
                            modep = "A TERME",
                            oper = memberuser.Username,
                            cleDossier = cle,
                            Monture = monture,
                            Lentille = lentille,
                            versement = versementdossier,
                            // ht= 0,
                            // net= 0,
                            /*  timbre = 0,
                              echeance = 0,
                              ht = 0,
                              net = 0,

                              tva = 0,
                              versement = 0,
                              reste = 0,
                              */
                            remise = remise,
                            Auto = true,
                        };
                        tsdffgent.IsSelected = true;

                        //    selectchanged();
                        WindowBorderFacture.DataContext = nouveauF1;
                        if (fermer == true)
                        {
                            txtnVersement.IsEnabled = true;
                            FacturationDesignNonStock();
                            ReceptDatagrid.DataContext = factureselectedl;
                            ReceptDatagrid.ItemsSource = factureselectedl;
                            ReceptDatagrid.IsEnabled = true;
                            txtTTC.Text = "0";
                            txtnfact.IsEnabled = false;


                            if (documenttype == 3)
                            {
                                NomDocumentLabel.Content = "bon de livraison";
                                chBonLivraison.IsChecked = true;
                                txtnVersement.IsEnabled = true;
                                selectedparam = proxy.GetAllParamétre();

                                if (selectedparam.AffichPrixAchatVente == true)
                                {
                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                }
                                if (selectedparam.ModiPrix == true)
                                {
                                    ReceptDatagrid.Columns[2].IsReadOnly = false;
                                }
                                else
                                {
                                    ReceptDatagrid.Columns[2].IsReadOnly = true;
                                }
                                if (selectedparam.modidate == true)
                                {
                                    txtDateOper.IsEnabled = true;
                                }
                                if (selectedparam.affiben == true)
                                {
                                    Bénéfice.Visibility = Visibility.Visible;
                                    Bénéficemont.Visibility = Visibility.Visible;

                                }
                            }
                            else
                            {


                                if (documenttype == 1)
                                {
                                    NomDocumentLabel.Content = "Nouvelle Facture";
                                    chFacture.IsChecked = true;
                                    txtnVersement.IsEnabled = true;
                                    selectedparam = proxy.GetAllParamétre();

                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;
                                        //   Bénéficemont.Text = Convert.ToString(((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                    }
                                }
                                else
                                {
                                    if (documenttype == 5)
                                    {
                                        NomDocumentLabel.Content = "Nouvelle Proforma";
                                        chProforma.IsChecked = true;
                                        txtnVersement.IsEnabled = false;
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                        txtDateOper.IsEnabled = true;


                                    }
                                }
                            }



                            //    txtRemise.Text=remise.ToString();
                            ///    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(interfacefacturation.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            //    calculef1();
                            dialog1.Close();

                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un document de vente", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnCreerImpressionDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (visualiserFacture == true)
                {

                    if (tunisie != true)
                    {
                        if (ListeDesDocuments.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                        {
                            SVC.F1 SelectedClient = ListeDesDocuments.SelectedItem as SVC.F1;
                            List<SVC.F1> facturelist = new List<SVC.F1>();
                            facturelist.Add(SelectedClient);
                            ImpressionFacture cl = new ImpressionFacture(proxy, facturelist, Clientvv, interfaceimpressionfacture);
                            cl.Show();
                            dialog1.Close();
                        }
                    }
                    else
                    {
                        if (ListeDesDocuments.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                        {
                            SVC.F1 SelectedClient = ListeDesDocuments.SelectedItem as SVC.F1;
                            List<SVC.F1> facturelist = new List<SVC.F1>();
                            facturelist.Add(SelectedClient);
                            ImpressionFactureTunisie cl = new ImpressionFactureTunisie(proxy, facturelist, Clientvv, interfaceimpressionfacture);
                            cl.Show();
                            dialog1.Close();
                        }
                    }

                }
                else
                {
                    if (visualiserFacture == false)
                    {
                        if (tunisie != true)
                        {
                            SVC.F1 SelectedClient = ListeDesDocuments.SelectedItem as SVC.F1;
                            List<SVC.F1> facturelist = new List<SVC.F1>();
                            facturelist.Add(SelectedClient);
                            RunFacture(proxy, facturelist, Clientvv, interfaceimpressionfacture);
                            dialog1.Close();
                        }
                        else
                        {
                            SVC.F1 SelectedClient = ListeDesDocuments.SelectedItem as SVC.F1;
                            List<SVC.F1> facturelist = new List<SVC.F1>();
                            facturelist.Add(SelectedClient);
                            RunFactureTunisie(proxy, facturelist, Clientvv, interfaceimpressionfacture);
                            dialog1.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
 
         private string getCurrentCellValue(TextBox txtCurCell)
        {
            return txtCurCell.Text;
        }
        private void ReceptDatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (facturenonstock == false)
                {
                    int col1 = e.Column.DisplayIndex;
                    //int row1 = Convert.ToInt32(e.Row.Header);
                    if (getCurrentCellValue((TextBox)e.EditingElement) == "" && (col1 == 2 || col1 == 3 || col1 == 4))
                    {
                        var el = e.EditingElement as TextBox;
                        el.Text = "0.00";
                        e.Cancel = true;
                        //   CONFIRMERVENTE.IsEnabled = false;
                        // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous ne pouvez pas laisser les champs vide,", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                    else
                    {
                        CONFIRMERVENTE.IsEnabled = true;
                    }

                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                    var testprodf = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
                    //var testprodf = selectedtropuvé;
                    if (ReceptDatagrid.SelectedItem != null && testprodf.Count() > 0)
                    {
                        SVC.Facture selectedrecept = ReceptDatagrid.SelectedItem as SVC.Facture;
                        //  SVC.Prodf objectmodifed = (testprodf.ToList()).Find(n => n.Id == selectedrecept.ficheproduit);
                        SVC.Prodf objectmodifed = selectedtropuvé;



                        NomenclatureProduit.SelectedItem = objectmodifed;
                        if (getCurrentCellValue((TextBox)e.EditingElement) != "" && (col1 == 3) && facturenew == true && facturemodif == false)
                        {
                            if (documenttype == 1 || documenttype == 3)
                            {
                                var el = e.EditingElement as TextBox;
                                if (Convert.ToDecimal(el.Text) > objectmodifed.quantite)
                                {
                                    el.Text = "0.00";
                                    // e.Cancel = true;
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantite insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                        }
                        else
                        {
                            if (getCurrentCellValue((TextBox)e.EditingElement) != "" && (col1 == 3) && facturenew == false && facturemodif == true)
                            {
                                if (documenttype == 1 || documenttype == 3)
                                {
                                    var el = e.EditingElement as TextBox;

                                    bool found = (anciennefactureselectedl.ToList()).Any(n => n.Id == selectedrecept.Id);

                                    if (found == true)
                                    {
                                        SVC.Facture ancienfacturemodifed = (anciennefactureselectedl.ToList()).Find(n => n.Id == selectedrecept.Id);

                                        if (objectmodifed.quantite < Convert.ToDecimal(el.Text) - ancienfacturemodifed.quantite)
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantite insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                            el.Text = Convert.ToString(ancienfacturemodifed.quantite);
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(el.Text) > objectmodifed.quantite)
                                        {
                                            el.Text = "0.00";
                                            // e.Cancel = true;
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantite insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                }
                                else
                                {
                                    if (documenttype == 2 || documenttype == 4)
                                    {
                                        var el = e.EditingElement as TextBox;
                                        bool found = (anciennefactureselectedl.ToList()).Any(n => n.Id == selectedrecept.Id);
                                        if (found)
                                        {
                                            SVC.Facture ancienfacturemodifed = (anciennefactureselectedl.ToList()).Find(n => n.Id == selectedrecept.Id);
                                            if (objectmodifed.quantite < ancienfacturemodifed.quantite - Convert.ToDecimal(el.Text))
                                            {
                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantite insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                                el.Text = Convert.ToString(ancienfacturemodifed.quantite);
                                            }
                                        }

                                    }
                                }

                            }
                        }
                    }

                    try
                    {



                        foreach (var item in test)
                        {
                            item.Total = (item.privente * item.quantite) + (((item.privente * item.quantite) * item.tva) / 100);
                        }






                        if (documenttype == 2 || documenttype == 4)
                        {
                            var ht = ((-test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                            string strht = string.Format("{0:0.00}", ht);
                            txtht.Text = strht;
                            //////////////////////////////////
                            var tva = (-(test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                            string strtva = string.Format("{0:0.00}", tva);
                            txttva.Text = strtva;
                            /////////////////////////////////
                            var ttc = (-(test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                            string strttc = string.Format("{0:0.00}", ttc);
                            txtTTC.Text = strttc;

                            decimal timbre = 0;
                            decimal remise = 0;
                            decimal avecremise = 0;
                            decimal Benf = 0;
                            if (txtRemise.Text != "")
                            {
                                if (Convert.ToDecimal(txtRemise.Text) != 0)
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) > 0)
                                    {
                                        avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(txtRemise.Text) < 0)
                                        {
                                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) + Convert.ToDecimal(txtRemise.Text);
                                            remise = -Convert.ToDecimal(txtRemise.Text);
                                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                        }
                                    }
                                }
                                else
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    remise = 0;
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }

                            if (tunisie != true)
                            {
                                if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                                {
                                    //   var timbres = (avecremise * 1 / 100);
                                    timbre = (avecremise * 1 / 100);

                                    if (timbre > 2500)
                                    {
                                        timbre = 2500;
                                    }
                                    string strtimbre = string.Format("{0:0.00}", -timbre);

                                    txtTimbre.Text = strtimbre;
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);

                                }
                            }
                            else
                            {
                                if (chFactureAvoir.IsChecked == true)
                                {
                                    timbre = -Convert.ToDecimal(0.6);
                                    txtTimbre.Text = Convert.ToString(-timbre);
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);
                                }
                            }
                            var Net = (-(avecremise + timbre));
                            string strNet = string.Format("{0:0.00}", Net);
                            txtNet.Text = strNet;
                            Bénéficemont.Text = String.Format("{0:0.##}", (-Benf));
                        }
                        else
                        {
                            var ht = ((test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                            string strht = string.Format("{0:0.00}", ht);
                            txtht.Text = strht;
                            //////////////////////////////////
                            var tva = ((test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                            string strtva = string.Format("{0:0.00}", tva);
                            txttva.Text = strtva;
                            /////////////////////////////////
                            var ttc = ((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                            string strttc = string.Format("{0:0.00}", ttc);
                            txtTTC.Text = strttc;

                            decimal timbre = 0;
                            decimal remise = 0;
                            decimal avecremise = 0;
                            decimal Benf = 0;
                            if (txtRemise.Text != "")
                            {
                                if (Convert.ToDecimal(txtRemise.Text) != 0)
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                                }
                                else
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    remise = 0;
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                            if (tunisie != true)
                            {
                                if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                                {

                                    // var timbres = (avecremise * 1 / 100);

                                    timbre = (avecremise * 1 / 100);
                                    if (timbre > 2500)
                                    {
                                        timbre = 2500;
                                    }
                                    string strtimbre = string.Format("{0:0.00}", timbre);
                                    txtTimbre.Text = strtimbre;
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);

                                }
                            }
                            else
                            {
                                if (chFacture.IsChecked == true)
                                {
                                    timbre = Convert.ToDecimal(0.6);
                                    txtTimbre.Text = Convert.ToString(timbre);
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);
                                }
                            }
                            var Net = ((avecremise + timbre));
                            string strNet = string.Format("{0:0.00}", Net);
                            txtNet.Text = strNet;

                            Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                        }





                        ReceptDatagrid.ItemsSource = test;


                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }
                else
                {
                    int col1 = e.Column.DisplayIndex;
                    //int row1 = Convert.ToInt32(e.Row.Header);
                    if (getCurrentCellValue((TextBox)e.EditingElement) == "" && (col1 == 2 || col1 == 3 || col1 == 4))
                    {
                        var el = e.EditingElement as TextBox;
                        el.Text = "0.00";
                        e.Cancel = true;
                        //   CONFIRMERVENTE.IsEnabled = false;
                        // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous ne pouvez pas laisser les champs vide,", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                    else
                    {
                        CONFIRMERVENTE.IsEnabled = true;
                    }

                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;


                    try
                    {



                        foreach (var item in test)
                        {
                            item.Total = (item.privente * item.quantite) + (((item.privente * item.quantite) * item.tva) / 100);
                        }






                        if (documenttype == 2 || documenttype == 4)
                        {
                            var ht = ((-test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                            string strht = string.Format("{0:0.00}", ht);
                            txtht.Text = strht;
                            //////////////////////////////////
                            var tva = (-(test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                            string strtva = string.Format("{0:0.00}", tva);
                            txttva.Text = strtva;
                            /////////////////////////////////
                            var ttc = (-(test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                            string strttc = string.Format("{0:0.00}", ttc);
                            txtTTC.Text = strttc;

                            decimal timbre = 0;
                            decimal remise = 0;
                            decimal avecremise = 0;
                            decimal Benf = 0;
                            if (txtRemise.Text != "")
                            {
                                if (Convert.ToDecimal(txtRemise.Text) != 0)
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) > 0)
                                    {
                                        avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(txtRemise.Text) < 0)
                                        {
                                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) + Convert.ToDecimal(txtRemise.Text);
                                            remise = -Convert.ToDecimal(txtRemise.Text);
                                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                        }
                                    }
                                }
                                else
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    remise = 0;
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }

                            if (tunisie != true)
                            {
                                if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                                {
                                    // var timbres = (avecremise * 1 / 100);
                                    timbre = (avecremise * 1 / 100);

                                    if (timbre > 2500)
                                    {
                                        timbre = 2500;
                                    }
                                    string strtimbre = string.Format("{0:0.00}", -timbre);
                                    txtTimbre.Text = strtimbre;
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);

                                }
                            }
                            else
                            {
                                if (chFactureAvoir.IsChecked == true)
                                {
                                    timbre = -Convert.ToDecimal(0.6);
                                    txtTimbre.Text = Convert.ToString(-timbre);
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);
                                }
                            }
                            var Net = (-(avecremise + timbre));
                            string strNet = string.Format("{0:0.00}", Net);
                            txtNet.Text = strNet;
                            Bénéficemont.Text = String.Format("{0:0.##}", (-Benf));
                        }
                        else
                        {
                            var ht = ((test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                            string strht = string.Format("{0:0.00}", ht);
                            txtht.Text = strht;
                            //////////////////////////////////
                            var tva = ((test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                            string strtva = string.Format("{0:0.00}", tva);
                            txttva.Text = strtva;
                            /////////////////////////////////
                            var ttc = ((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                            string strttc = string.Format("{0:0.00}", ttc);
                            txtTTC.Text = strttc;

                            decimal timbre = 0;
                            decimal remise = 0;
                            decimal avecremise = 0;
                            decimal Benf = 0;
                            if (txtRemise.Text != "")
                            {
                                if (Convert.ToDecimal(txtRemise.Text) != 0)
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                                }
                                else
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    remise = 0;
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                            if (tunisie != true)
                            {
                                if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                                {

                                    //  var timbres = (avecremise * 1 / 100);
                                    timbre = (avecremise * 1 / 100);
                                    if (timbre > 2500)
                                    {
                                        timbre = 2500;
                                    }
                                    string strtimbre = string.Format("{0:0.00}", timbre);
                                    txtTimbre.Text = strtimbre;

                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);

                                }
                            }
                            else
                            {
                                if (chFacture.IsChecked == true)
                                {
                                    timbre = Convert.ToDecimal(0.6);
                                    txtTimbre.Text = Convert.ToString(timbre);
                                }
                                else
                                {
                                    timbre = 0;
                                    txtTimbre.Text = Convert.ToString(timbre);
                                }
                            }
                            var Net = ((avecremise + timbre));
                            string strNet = string.Format("{0:0.00}", Net);
                            txtNet.Text = strNet;

                            Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                        }





                        ReceptDatagrid.ItemsSource = test;


                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ReceptDatagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (facturenonstock == false)
                {
                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                    var testprodf = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;


                    if (ReceptDatagrid.SelectedItem != null && testprodf.Count() > 0)
                    {
                        SVC.Facture selectedrecept = ReceptDatagrid.SelectedItem as SVC.Facture;
                        //  SVC.Prodf objectmodifed = (testprodf.ToList()).Find(n => n.Id == selectedrecept.ficheproduit);

                        SVC.Prodf objectmodifed = selectedtropuvé;



                        NomenclatureProduit.SelectedItem = objectmodifed;

                    }

                    foreach (var item in test)
                    {
                        item.Total = (item.privente * item.quantite) + (((item.privente * item.quantite) * item.tva) / 100);
                    }
                    if (documenttype == 2 || documenttype == 4)
                    {
                        var ht = ((-test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = (-(test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = (-(test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                if (Convert.ToDecimal(txtRemise.Text) > 0)
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                }
                                else
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) < 0)
                                    {
                                        avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) + Convert.ToDecimal(txtRemise.Text);
                                        remise = -Convert.ToDecimal(txtRemise.Text);
                                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }
                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                            {
                                // var timbres = (avecremise * 1 / 100);
                                timbre = (avecremise * 1 / 100);

                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", -timbre);
                                txtTimbre.Text = strtimbre;
                                /* timbre = (avecremise * 1 / 100);
                                 txtTimbre.Text = Convert.ToString(-timbre);*/
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFactureAvoir.IsChecked == true)
                            {
                                timbre = -Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(-timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = (-(avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;
                        Bénéficemont.Text = String.Format("{0:0.##}", (-Benf));
                    }
                    else
                    {
                        var ht = ((test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = ((test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = ((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                remise = Convert.ToDecimal(txtRemise.Text);
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                            {
                                // timbre = (avecremise * 1 / 100);
                                //txtTimbre.Text = Convert.ToString(timbre);
                                //  var timbres = (avecremise * 1 / 100);

                                timbre = (avecremise * 1 / 100);
                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", timbre);
                                txtTimbre.Text = strtimbre;
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFacture.IsChecked == true)
                            {
                                timbre = Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = ((avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;

                        Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                    }
                    ReceptDatagrid.ItemsSource = test;
                }
                else
                {
                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;




                    foreach (var item in test)
                    {
                        item.Total = (item.privente * item.quantite) + (((item.privente * item.quantite) * item.tva) / 100);
                    }
                    if (documenttype == 2 || documenttype == 4)
                    {
                        var ht = ((-test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = (-(test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = (-(test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                if (Convert.ToDecimal(txtRemise.Text) > 0)
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                }
                                else
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) < 0)
                                    {
                                        avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) + Convert.ToDecimal(txtRemise.Text);
                                        remise = -Convert.ToDecimal(txtRemise.Text);
                                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }
                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                            {
                                //  var timbres = (avecremise * 1 / 100);
                                timbre = (avecremise * 1 / 100);

                                /* timbre = (avecremise * 1 / 100);
                                 txtTimbre.Text = Convert.ToString(-timbre);*/
                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", -timbre);
                                txtTimbre.Text = strtimbre;
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFactureAvoir.IsChecked == true)
                            {
                                timbre = -Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(-timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = (-(avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;
                        Bénéficemont.Text = String.Format("{0:0.##}", (-Benf));
                    }
                    else
                    {
                        var ht = ((test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = ((test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = ((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                remise = Convert.ToDecimal(txtRemise.Text);
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                            {
                                // timbre = (avecremise * 1 / 100);
                                //txtTimbre.Text = Convert.ToString(timbre);
                                //   var timbres = (avecremise * 1 / 100);

                                timbre = (avecremise * 1 / 100);
                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", timbre);
                                txtTimbre.Text = strtimbre;
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFacture.IsChecked == true)
                            {
                                timbre = Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = ((avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;

                        Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                    }
                    ReceptDatagrid.ItemsSource = test;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void ReceptDatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (facturenonstock == false)
                {
                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                    var testprodf = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;

                    if (ReceptDatagrid.SelectedItem != null && testprodf.Count() > 0)
                    {
                        SVC.Facture selectedrecept = ReceptDatagrid.SelectedItem as SVC.Facture;
                        //   SVC.Prodf objectmodifed = (testprodf.ToList()).Find(n => n.Id == selectedrecept.ficheproduit);
                        SVC.Prodf objectmodifed = selectedtropuvé;

                        NomenclatureProduit.SelectedItem = objectmodifed;

                    }

                    foreach (var item in test)
                    {
                        item.Total = (item.privente * item.quantite) + (((item.privente * item.quantite) * item.tva) / 100);
                    }

                    if (documenttype == 2 || documenttype == 4)
                    {
                        var ht = ((-test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = (-(test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = (-(test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                if (Convert.ToDecimal(txtRemise.Text) > 0)
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                }
                                else
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) < 0)
                                    {
                                        avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) + Convert.ToDecimal(txtRemise.Text);
                                        remise = -Convert.ToDecimal(txtRemise.Text);
                                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }
                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                            {
                                //  var timbres = (avecremise * 1 / 100);
                                timbre = (avecremise * 1 / 100);

                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", -timbre);
                                txtTimbre.Text = strtimbre;
                                /* timbre = (avecremise * 1 / 100);
                                 txtTimbre.Text = Convert.ToString(-timbre);*/
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFactureAvoir.IsChecked == true)
                            {
                                timbre = -Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(-timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = (-(avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;
                        Bénéficemont.Text = String.Format("{0:0.##}", (-Benf));
                    }
                    else
                    {
                        var ht = ((test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = ((test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = ((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                remise = Convert.ToDecimal(txtRemise.Text);
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                            {
                                // timbre = (avecremise * 1 / 100);
                                //txtTimbre.Text = Convert.ToString(timbre);
                                //    var timbres = (avecremise * 1 / 100);

                                timbre = (avecremise * 1 / 100);
                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", timbre);
                                txtTimbre.Text = strtimbre;
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFacture.IsChecked == true)
                            {
                                timbre = Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = ((avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;

                        Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                    }



                    ReceptDatagrid.ItemsSource = test;

                }
                else
                {
                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;



                    foreach (var item in test)
                    {
                        item.Total = (item.privente * item.quantite) + (((item.privente * item.quantite) * item.tva) / 100);
                    }

                    if (documenttype == 2 || documenttype == 4)
                    {
                        var ht = ((-test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = (-(test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = (-(test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                if (Convert.ToDecimal(txtRemise.Text) > 0)
                                {
                                    avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                }
                                else
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) < 0)
                                    {
                                        avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) + Convert.ToDecimal(txtRemise.Text);
                                        remise = -Convert.ToDecimal(txtRemise.Text);
                                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }
                                }
                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                            {
                                //  var timbres = (avecremise * 1 / 100);
                                timbre = (avecremise * 1 / 100);

                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", -timbre);
                                txtTimbre.Text = strtimbre;
                                /* timbre = (avecremise * 1 / 100);
                                 txtTimbre.Text = Convert.ToString(-timbre);*/
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFactureAvoir.IsChecked == true)
                            {
                                timbre = -Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(-timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = (-(avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;
                        Bénéficemont.Text = String.Format("{0:0.##}", (-Benf));
                    }
                    else
                    {
                        var ht = ((test.AsEnumerable().Sum(o => o.privente * o.quantite)));
                        string strht = string.Format("{0:0.00}", ht);
                        txtht.Text = strht;
                        //////////////////////////////////
                        var tva = ((test).AsEnumerable().Sum(o => ((o.privente * o.tva) / 100) * o.quantite));
                        string strtva = string.Format("{0:0.00}", tva);
                        txttva.Text = strtva;
                        /////////////////////////////////
                        var ttc = ((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                        string strttc = string.Format("{0:0.00}", ttc);
                        txtTTC.Text = strttc;

                        decimal timbre = 0;
                        decimal remise = 0;
                        decimal avecremise = 0;
                        decimal Benf = 0;
                        if (txtRemise.Text != "")
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                remise = Convert.ToDecimal(txtRemise.Text);
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                            }
                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((test).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                        }
                        if (tunisie != true)
                        {
                            if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                            {
                                // timbre = (avecremise * 1 / 100);
                                //txtTimbre.Text = Convert.ToString(timbre);
                                //var timbres = (avecremise * 1 / 100);

                                timbre = (avecremise * 1 / 100);
                                if (timbre > 2500)
                                {
                                    timbre = 2500;
                                }
                                string strtimbre = string.Format("{0:0.00}", timbre);
                                txtTimbre.Text = strtimbre;
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);

                            }
                        }
                        else
                        {
                            if (chFacture.IsChecked == true)
                            {
                                timbre = Convert.ToDecimal(0.6);
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                            else
                            {
                                timbre = 0;
                                txtTimbre.Text = Convert.ToString(timbre);
                            }
                        }
                        var Net = ((avecremise + timbre));
                        string strNet = string.Format("{0:0.00}", Net);
                        txtNet.Text = strNet;

                        Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                    }



                    ReceptDatagrid.ItemsSource = test;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void NonFacturationDesign()
        {
            var dgtc = NomenclatureProduit.Columns[1] as DataGridTextColumn;
            dgtc.Binding = new System.Windows.Data.Binding("privente");
            var dgtc3 = NomenclatureProduit.Columns[3] as DataGridTextColumn;
            dgtc3.Binding = new System.Windows.Data.Binding("previent");
            Grid.SetColumnSpan(RéglemdfentGfsdfsrid, 2);
            Grid.SetRowSpan(ListeDesDocuments, 8);
            WindowBorderFacture.Visibility = Visibility.Collapsed;
            gridspl.Visibility = Visibility.Collapsed;
            NomenclatureDatagrid.Visibility = Visibility.Collapsed;
            girdspi1.Visibility = Visibility.Collapsed;
            factureselectedl = new List<SVC.Facture>();
            //  ReceptDatagrid.DataContext = factureselectedl;
            //  ReceptDatagrid.ItemsSource = factureselectedl;
            nouveauF1 = new SVC.F1();
            BtnCreerProduit.Visibility = Visibility.Collapsed;
            //RéglementGfsdfsrid.DataContext = nouveauF1;

            fermer = false;
            documenttype = 0;
            facturenonstock = false;
        }

        private void CONFIRMERVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (facturenonstock == false)
                {

                    /*************************************ajout d'un nouveau document de vente***************/
                    if (facturenew == true && facturemodif == false && memberuser.CreationDossierClient == true)
                    {
                        string document = "";
                        if (documenttype == 1)
                        {
                            document = "F";
                        }
                        else
                        {
                            if (documenttype == 2)
                            {
                                document = "A";
                            }
                            else
                            {
                                if (documenttype == 3)
                                {
                                    document = "B";
                                }
                                else
                                {
                                    if (documenttype == 4)
                                    {
                                        document = "C";
                                    }
                                    else
                                    {

                                        if (documenttype == 5)
                                        {
                                            document = "P";
                                        }
                                        else
                                        {
                                            if (documenttype == 6)
                                            {
                                                document = "R";

                                            }
                                        }

                                    }
                                }
                            }
                        }

                        if (Clientvv.Id != 0)
                        {

                            if (txtRemise.Text != "")
                            {
                                if (Convert.ToDecimal(txtRemise.Text) != 0)
                                {
                                    nouveauF1.remise = Convert.ToDecimal(txtRemise.Text);
                                }
                                else
                                {
                                    nouveauF1.remise = 0;
                                }

                            }
                            else
                            {
                                nouveauF1.remise = 0;
                            }



                            /************************************************/

                            if (documenttype == 2 || documenttype == 4)
                            {
                                decimal timbre = 0;
                                decimal remise = 0;
                                decimal avecremise = 0;
                                if (txtRemise.Text != "")
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) != 0)
                                    {
                                        avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text));
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                    }
                                    else
                                    {
                                        avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                        remise = 0;
                                    }
                                }
                                else
                                {
                                    avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                }
                                if (tunisie != true)
                                {
                                    if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                                    {
                                        nouveauF1.timbre = (avecremise * 1 / 100);
                                        timbre = (avecremise * 1 / 100);
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;

                                    }
                                }
                                else
                                {
                                    if (chFactureAvoir.IsChecked == true)
                                    {
                                        nouveauF1.timbre = -Convert.ToDecimal(0.6);
                                        timbre = -timbre;
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;
                                    }
                                }
                                nouveauF1.net = (avecremise + timbre);
                                if (txtnVersement.Text != "")
                                {
                                    if (Convert.ToDecimal(txtnVersement.Text) != 0)
                                    {
                                        nouveauF1.versement = Convert.ToDecimal(txtnVersement.Text);
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }
                                    else
                                    {
                                        nouveauF1.versement = 0;
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }

                                }
                                else
                                {
                                    nouveauF1.versement = 0;
                                    nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                }

                                nouveauF1.tva = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                nouveauF1.ht = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                if (nouveauF1.reste != 0)
                                {
                                    nouveauF1.soldé = false;
                                }
                                else
                                {
                                    nouveauF1.soldé = true;

                                }
                            }
                            else
                            {
                                decimal timbre = 0;
                                decimal remise = 0;
                                decimal avecremise = 0;
                                if (txtRemise.Text != "")
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) != 0)
                                    {
                                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                    }
                                    else
                                    {
                                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                        remise = 0;
                                    }
                                }
                                else
                                {
                                    avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                }
                                if (tunisie != true)
                                {
                                    if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true))
                                    {
                                        nouveauF1.timbre = (avecremise * 1 / 100);
                                        timbre = (avecremise * 1 / 100);
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;

                                    }
                                }
                                else
                                {
                                    if (chFacture.IsChecked == true)
                                    {
                                        nouveauF1.timbre = Convert.ToDecimal(0.6); ;
                                        timbre = Convert.ToDecimal(0.6);
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;
                                    }
                                }
                                nouveauF1.net = avecremise + timbre;
                                if (txtnVersement.Text != "")
                                {
                                    if (Convert.ToDecimal(txtnVersement.Text) != 0)
                                    {
                                        nouveauF1.versement = Convert.ToDecimal(txtnVersement.Text);
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }
                                    else
                                    {
                                        nouveauF1.versement = 0;
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }

                                }
                                else
                                {
                                    nouveauF1.versement = 0;
                                    nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                }

                                nouveauF1.tva = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                nouveauF1.ht = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                if (nouveauF1.reste != 0)
                                {
                                    nouveauF1.soldé = false;
                                }
                                else
                                {
                                    nouveauF1.soldé = true;

                                }
                            }
                            if (nouveauF1.echeance != 0)
                            {
                                nouveauF1.datecheance = nouveauF1.date.Value.AddDays(Convert.ToDouble(nouveauF1.echeance));
                            }
                            nouveauF1.cle = Clientvv.Id + Clientvv.Raison + nouveauF1.net + DateTime.Now.TimeOfDay;
                            nouveauF1.heure = DateTime.Now.TimeOfDay;
                            /*****************************************************/

                            var remisepourfacture = nouveauF1.remise;
                            bool Operfacture = false;

                            List<int> listrefresh = new List<int>();

                            foreach (SVC.Facture newfacture in factureselectedl)
                            {
                                newfacture.cle = nouveauF1.cle;
                                newfacture.codeclient = nouveauF1.codeclient;
                                if (remisepourfacture != 0)
                                {
                                    if (remisepourfacture >= ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite)))
                                    {
                                        newfacture.remise = ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                        remisepourfacture = remisepourfacture - ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                    }
                                    else
                                    {
                                        newfacture.remise = remisepourfacture;
                                    }

                                }
                                else
                                {
                                    newfacture.remise = 0;
                                }
                                if (newfacture.codeprod != 0)
                                {
                                    listrefresh.Add(Convert.ToInt32(newfacture.ficheproduit));
                                }
                            }


                            if (nouveauF1.versement != 0)
                            {


                                SVC.Depeiment PAIEMENT = new SVC.Depeiment
                                {
                                    date = nouveauF1.date,
                                    montant = Convert.ToDecimal(nouveauF1.versement),
                                    paiem = "ESPECES" + " Vente :" + nouveauF1.nfact + " " + " date :" + nouveauF1.date,
                                    oper = memberuser.Username,
                                    dates = nouveauF1.dates,
                                    banque = "Caisse",
                                    nfact = nouveauF1.nfact,
                                    amontant = Convert.ToDecimal(nouveauF1.net),
                                    cle = nouveauF1.cle,
                                    cp = nouveauF1.Id,
                                    Multiple = false,
                                    CodeClient = nouveauF1.codeclient,
                                    RaisonClient = nouveauF1.raison,

                                };
                                SVC.Depense CAISSE = new SVC.Depense
                                {
                                    cle = nouveauF1.cle,
                                    Auto = true,
                                    Commentaires = "ESPECES" + " Vente :" + nouveauF1.nfact + " " + " date :" + nouveauF1.date,
                                    CompteDébité = "Caisse",
                                    Crédit = true,
                                    DateDebit = nouveauF1.date,
                                    DateSaisie = nouveauF1.dates,
                                    Débit = false,
                                    ModePaiement = "ESPECES",
                                    Montant = 0,
                                    MontantCrédit = nouveauF1.versement,
                                    NumCheque = Convert.ToString(nouveauF1.Id),
                                    Num_Facture = nouveauF1.nfact,
                                    RubriqueComptable = "ESPECES document de vente: " + nouveauF1.raison + " " + nouveauF1.nfact,
                                    Username = memberuser.Username,

                                };
                                bool ii = false;
                                bool depense = false;
                                bool depaiement = false;
                                List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                foreach (var item in factureselectedl)
                                {
                                    if (item.quantite != 0)
                                    {
                                        item.cle = nouveauF1.cle;
                                        listsanszero.Add(item);
                                    }
                                }
                                if (interfacefacturation == 0)
                                {
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        ii = proxy.InsertFacture(nouveauF1, listsanszero, document);
                                        Operfacture = true;
                                        if (documenttype != 6 && documenttype != 5)
                                        {
                                            proxy.InsertDepeiment(PAIEMENT);
                                            depaiement = true;
                                            proxy.InsertDepense(CAISSE);
                                            depense = true;
                                        }
                                        else
                                        {

                                            if (documenttype == 5 || documenttype == 6)
                                            {
                                                depense = true;
                                                depaiement = true;
                                            }

                                        }

                                        if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                        {
                                            ts.Complete();
                                            facturemodif = true;
                                            facturenew = false;

                                        }
                                    }
                                    if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                    {

                                        proxy.AjouterProdflistRefresh(listrefresh);
                                        proxy.AjouterSoldeF1Refresh();
                                        // proxy.AjouterDepenseRefresh();
                                        NonFacturationDesign();
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                }
                                else
                                {
                                    
                                        /****************lentille****************************/
                                        if (interfacefacturation == 2)
                                        {
                                            bool modiflentille = false;
                                            bool bonfacture = false;
                                            var existeversement = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).Any();
                                            SVC.Depeiment depaiementclass = new SVC.Depeiment(); ;
                                            SVC.Depense depenseclass = new SVC.Depense();

                                            if (existeversement == true)
                                            {
                                                depaiementclass = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).First();
                                                depenseclass = proxy.GetAllDepenseByF1(nouveauF1.cleDossier).First();
                                            }

                                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                            {
                                                ii = proxy.InsertFacture(nouveauF1, listsanszero, document);

                                                Operfacture = true;
                                                if (existeversement == false)
                                                {
                                                    if (documenttype == 1 || documenttype == 3)
                                                    {
                                                        depaiement = false;
                                                        proxy.InsertDepeiment(PAIEMENT);
                                                        depaiement = true;
                                                        depense = false;
                                                        proxy.InsertDepense(CAISSE);
                                                        depense = true;

                                                        modiflentille = false;
                                                        LentilleClass.Encaissé = nouveauF1.versement;
                                                        LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                        LentilleClass.StatutVente = true;

                                                        proxy.UpdateLentilleClient(LentilleClass);
                                                        modiflentille = true;
                                                        bonfacture = true;
                                                    }
                                                    else
                                                    {
                                                        modiflentille = true;
                                                        depense = true;
                                                        depaiement = true;
                                                    }
                                                }
                                                else
                                                {
                                                    /* depaiement = true;
                                                     depense = true;
                                                     LentilleClass.StatutVente = true;
                                                     proxy.UpdateLentilleClient(LentilleClass);
                                                     modiflentille = true;*/

                                                    if (nouveauF1.versement != 0)
                                                    {
                                                        if (documenttype == 1 || documenttype == 3)
                                                        {
                                                            depaiement = false;
                                                            depaiementclass.montant = nouveauF1.versement;
                                                            depaiementclass.cle = nouveauF1.cle;
                                                            proxy.UpdateDepeiment(depaiementclass);
                                                            depaiement = true;
                                                            depense = false;
                                                            depenseclass.MontantCrédit = nouveauF1.versement;
                                                            depenseclass.cle = nouveauF1.cle;
                                                            proxy.UpdateDepense(depenseclass);
                                                            depense = true;
                                                            modiflentille = false;
                                                            LentilleClass.Encaissé = nouveauF1.versement;
                                                            LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                            LentilleClass.StatutVente = true;

                                                            proxy.UpdateLentilleClient(LentilleClass);
                                                            modiflentille = true;
                                                            bonfacture = true;
                                                        }
                                                        else
                                                        {
                                                            modiflentille = true; depense = true; depaiement = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (documenttype == 1 || documenttype == 3)
                                                        {
                                                            depaiement = false;
                                                            proxy.DeleteDepeiment(depaiementclass);

                                                            depaiement = true;
                                                            depense = false;
                                                            proxy.DeleteDepense(depenseclass);
                                                            depense = true;
                                                            modiflentille = false;
                                                            LentilleClass.Encaissé = nouveauF1.versement;
                                                            LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                            LentilleClass.StatutVente = true;

                                                            proxy.UpdateLentilleClient(LentilleClass);
                                                            modiflentille = true;
                                                            bonfacture = true;
                                                        }
                                                        else
                                                        {
                                                            modiflentille = true; depense = true; depaiement = true;

                                                        }
                                                    }
                                                }
                                                if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                                {
                                                    ts.Complete();
                                                    facturemodif = false;
                                                    facturenew = false;

                                                }
                                            }
                                            if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                            {

                                                proxy.AjouterProdflistRefresh(listrefresh);
                                                proxy.AjouterSoldeF1Refresh();
                                                if (bonfacture == true)
                                                {
                                                    proxy.AjouterLentilleClientRefresh(Clientvv.Id);

                                                }
                                                // proxy.AjouterDepenseRefresh();
                                                GridLentille.IsEnabled = false;
                                                GridLentille.DataContext = null;
                                                LentilleClass = null;
                                                Lentilleversementzero = false;

                                                NonFacturationDesign();
                                                MessageBoxResult resFult = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            }
                                        }
                            
                                }
                            }
                            /********************versment ==0)********/
                            else
                            {

                                bool ii = false;
                                List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                foreach (var item in factureselectedl)
                                {
                                    if (item.quantite != 0)
                                    {
                                        item.cle = nouveauF1.cle;
                                        listsanszero.Add(item);
                                    }
                                }
                                /***********Monture dossier*********************/
                         
                                    if (interfacefacturation == 0)
                                    {
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            ii = proxy.InsertFacture(nouveauF1, listsanszero, document);

                                            Operfacture = true;


                                            if (Operfacture == true && ii == true)
                                            {
                                                ts.Complete();
                                                facturemodif = true;
                                                facturenew = false;

                                            }
                                        }
                                        if (Operfacture == true && ii == true)
                                        {

                                            proxy.AjouterProdflistRefresh(listrefresh);
                                            proxy.AjouterSoldeF1Refresh();

                                            NonFacturationDesign();
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                        }
                                    }
                                    else
                                    {
                                        if (interfacefacturation == 2)
                                        {
                                            bool modiflentille = false;
                                            bool depaiement = false;
                                            bool depense = false;
                                            bool facturebon = true;
                                            var existeversement = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).Any();
                                            SVC.Depeiment depaiementclass = new SVC.Depeiment(); ;
                                            SVC.Depense depenseclass = new SVC.Depense();
                                            //  MessageBoxResult resFult = Xceed.Wpf.Toolkit.MessageBox.Show(existeversement.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            if (existeversement == true)
                                            {

                                                depaiementclass = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).First();
                                                depenseclass = proxy.GetAllDepenseByF1(nouveauF1.cleDossier).First();
                                            }


                                            if (existeversement == true)
                                            {
                                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                {
                                                    ii = proxy.InsertFacture(nouveauF1, listsanszero, document);

                                                    Operfacture = true;
                                                    if (documenttype == 1 || documenttype == 3)
                                                    {
                                                        depaiement = false;
                                                        proxy.DeleteDepeiment(depaiementclass);

                                                        depaiement = true;
                                                        depense = false;
                                                        proxy.DeleteDepense(depenseclass);
                                                        depense = true;
                                                        modiflentille = false;
                                                        LentilleClass.Encaissé = nouveauF1.versement;
                                                        LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                        LentilleClass.StatutVente = true;

                                                        proxy.UpdateLentilleClient(LentilleClass);
                                                        modiflentille = true;
                                                        facturebon = true;
                                                    }
                                                    else
                                                    {
                                                        modiflentille = true; depense = true; depaiement = true;
                                                    }
                                                    if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                                    {
                                                        ts.Complete();
                                                        facturemodif = false;
                                                        facturenew = false;

                                                    }

                                                }
                                                if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                                {

                                                    proxy.AjouterProdflistRefresh(listrefresh);
                                                    proxy.AjouterSoldeF1Refresh();
                                                    if (facturebon == true)
                                                    {
                                                        proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                                    }
                                                    //  proxy.AjouterDepenseRefresh();
                                                    GridLentille.IsEnabled = false;
                                                    GridLentille.DataContext = null;
                                                    LentilleClass = null;
                                                    Lentilleversementzero = false;

                                                    NonFacturationDesign();
                                                    MessageBoxResult sresFult = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                }

                                            }
                                            else
                                            {

                                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                {
                                                    ii = proxy.InsertFacture(nouveauF1, listsanszero, document);

                                                    Operfacture = true;
                                                    if (documenttype == 1 || documenttype == 3)
                                                    {
                                                        LentilleClass.Encaissé = nouveauF1.versement;
                                                        LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                        LentilleClass.StatutVente = true;
                                                        proxy.UpdateLentilleClient(LentilleClass);
                                                        modiflentille = true;
                                                        facturebon = true;
                                                    }
                                                    else
                                                    {
                                                        modiflentille = true;
                                                    }

                                                    if (Operfacture == true && ii == true && modiflentille == true)
                                                    {
                                                        ts.Complete();
                                                        facturemodif = false;
                                                        facturenew = false;

                                                    }
                                                }
                                                if (Operfacture == true && ii == true && modiflentille == true)
                                                {

                                                    proxy.AjouterProdflistRefresh(listrefresh);
                                                    proxy.AjouterSoldeF1Refresh();
                                                    if (facturebon == true)
                                                    {
                                                        proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                                    }
                                                    GridLentille.IsEnabled = false;
                                                    GridLentille.DataContext = null;
                                                    LentilleClass = null;
                                                    Lentilleversementzero = false;

                                                    NonFacturationDesign();
                                                    MessageBoxResult resDFult = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                                }
                                            }

                                        }

                                    }

                            
                            }






                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un autre client", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                    }
                    else
                    {
                        /******************************************** modification facture*********************************/
                        if (facturenew == false && facturemodif == true && memberuser.ModificationDossierClient == true)
                        {

                            /************************La partie F1***********************/
                            String document = "";
                            if (documenttype == 1)
                            {
                                document = "F";
                            }
                            else
                            {
                                if (documenttype == 2)
                                {
                                    document = "A";
                                }
                                else
                                {
                                    if (documenttype == 3)
                                    {
                                        document = "B";
                                    }
                                    else
                                    {
                                        if (documenttype == 4)
                                        {
                                            document = "C";
                                        }
                                        else
                                        {
                                            if (documenttype == 5)
                                            {
                                                document = "P";
                                            }
                                            else
                                            {
                                                if (documenttype == 6)
                                                {
                                                    document = "R";

                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            if (Clientvv.Id != 0)
                            {



                                if (txtRemise.Text != "")
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) != 0)
                                    {
                                        if (ancienneF1.remise != Convert.ToDecimal(txtRemise.Text))
                                        {
                                            selectedF1.remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                    }
                                    else
                                    {
                                        selectedF1.remise = 0;
                                    }

                                }
                                else
                                {
                                    selectedF1.remise = 0;
                                }



                                /************************************************/

                                if (documenttype == 2 || documenttype == 4)
                                {
                                    decimal timbre = 0;
                                    decimal remise = 0;
                                    decimal avecremise = 0;
                                    if (txtRemise.Text != "")
                                    {
                                        if (Convert.ToDecimal(txtRemise.Text) != 0)
                                        {
                                            avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text));
                                            remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                        else
                                        {
                                            avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                            remise = 0;
                                        }
                                    }
                                    else
                                    {
                                        avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                    }
                                    if (tunisie != true)
                                    {
                                        if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                                        {
                                            selectedF1.timbre = (avecremise * 1 / 100);
                                            timbre = (avecremise * 1 / 100);
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;

                                        }
                                    }
                                    else
                                    {
                                        if (chFactureAvoir.IsChecked == true)
                                        {
                                            selectedF1.timbre = -Convert.ToDecimal(0.6); ;
                                            timbre = -timbre;
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;
                                        }
                                    }
                                    selectedF1.net = (avecremise + timbre);
                                    selectedF1.reste = selectedF1.net - selectedF1.versement;



                                    selectedF1.tva = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                    selectedF1.ht = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                    if (selectedF1.reste != 0)
                                    {
                                        selectedF1.soldé = false;
                                    }
                                    else
                                    {
                                        selectedF1.soldé = true;

                                    }
                                }
                                else
                                {
                                    decimal timbre = 0;
                                    decimal remise = 0;
                                    decimal avecremise = 0;
                                    if (txtRemise.Text != "")
                                    {
                                        if (Convert.ToDecimal(txtRemise.Text) != 0)
                                        {
                                            avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                            remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                        else
                                        {
                                            avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                            remise = 0;
                                        }
                                    }
                                    else
                                    {
                                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    }

                                    if (tunisie != true)
                                    {
                                        if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true))
                                        {
                                            selectedF1.timbre = (avecremise * 1 / 100);
                                            timbre = (avecremise * 1 / 100);
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;

                                        }
                                    }
                                    else
                                    {
                                        if (chFacture.IsChecked == true)
                                        {
                                            selectedF1.timbre = Convert.ToDecimal(0.6);
                                            timbre = Convert.ToDecimal(0.6);
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;
                                        }
                                    }
                                    selectedF1.net = avecremise + timbre;
                                    selectedF1.reste = selectedF1.net - selectedF1.versement;



                                    selectedF1.tva = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                    selectedF1.ht = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                    if (selectedF1.reste != 0)
                                    {
                                        selectedF1.soldé = false;
                                    }
                                    else
                                    {
                                        selectedF1.soldé = true;

                                    }
                                }
                                if (selectedF1.echeance != 0)
                                {
                                    if (selectedF1.echeance != ancienneF1.echeance)
                                    {
                                        selectedF1.datecheance = selectedF1.date.Value.AddDays(Convert.ToDouble(selectedF1.echeance));
                                    }
                                }

                                /***************************************************/

                                List<int> listrefresh = new List<int>();
                                List<SVC.Facture> NouvelleFacture = new List<SVC.Facture>();
                                List<SVC.Facture> AncienneFacture = new List<SVC.Facture>();
                                var remisepourfacture = selectedF1.remise;

                                bool Operfacture = false;


                                foreach (SVC.Facture newfacture in factureselectedl)
                                {

                                    var found = (anciennefactureselectedl).Any(itemf => itemf.ficheproduit == newfacture.ficheproduit);
                                    if (found == false)
                                    {
                                        if (newfacture.quantite != 0)
                                        {
                                            newfacture.cle = selectedF1.cle;
                                            newfacture.nfact = selectedF1.nfact;


                                            NouvelleFacture.Add(newfacture);
                                        }
                                        if (newfacture.codeprod != 0)
                                        {
                                            listrefresh.Add(Convert.ToInt32(newfacture.ficheproduit));
                                        }

                                    }
                                    else
                                    {
                                        if (remisepourfacture != 0)
                                        {
                                            if (remisepourfacture >= ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite)))
                                            {
                                                newfacture.remise = ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                                remisepourfacture = remisepourfacture - ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                            }
                                            else
                                            {
                                                newfacture.remise = remisepourfacture;
                                            }

                                        }
                                        else
                                        {
                                            newfacture.remise = 0;
                                        }
                                        AncienneFacture.Add(newfacture);
                                        if (newfacture.codeprod != 0)
                                        {
                                            listrefresh.Add(Convert.ToInt32(newfacture.ficheproduit));
                                        }
                                    }
                                }
                                bool ExisteMonture = false;
                                ExisteMonture = proxy.GetAllMonturebyDossier(Convert.ToString(selectedF1.cleDossier)).Any();
                                bool existelentille = false;
                                existelentille = proxy.GetAllLentilleClientbyDossier(Convert.ToString(selectedF1.cleDossier)).Any();

                                if (!(selectedF1.ht == 0 && selectedF1.versement != 0) && ExisteMonture == false && existelentille == false)
                                {


                                    bool ii = false;
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        ii = proxy.UpdateFacture(selectedF1, NouvelleFacture, AncienneFacture, document);

                                        Operfacture = true;



                                        if (Operfacture == true && ii == true)
                                        {
                                            ts.Complete();
                                            facturemodif = true;
                                            facturenew = false;

                                        }
                                    }
                                    if (Operfacture == true && ii == true)
                                    {

                                        proxy.AjouterProdflistRefresh(listrefresh);
                                        proxy.AjouterFactureVenteRefresh(selectedF1.cle);
                                        proxy.AjouterSoldeF1Refresh();

                                        NonFacturationDesign();
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        /*******************************************************/
                                        //  selectedF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                                        /*     ancienneF1 = proxy.GetAllF1ByVisiteOper(selectedF1.nfact).First();
                                             factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                             anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact); 
                                             /************************************************************************/
                                        /*    ReceptDatagrid.ItemsSource = factureselectedl;
                                          ReceptDatagrid.DataContext = factureselectedl;
                                          CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                          RéglementGfsdfsrid.DataContext = selectedF1;
                                          var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                          string strTTC = string.Format("{0:0.00}", TTC);
                                          txtTTC.Text = strTTC;*/
                                    }
                                    else
                                    {

                                        if (anciennefactureselectedl != null)
                                        {

                                            /***************************************************/
                                            ancienneF1 = proxy.GetAllF1ByVisiteOper(selectedF1.nfact).First();
                                            factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                            anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                            /************************************************************************/
                                            ReceptDatagrid.ItemsSource = factureselectedl;
                                            ReceptDatagrid.DataContext = factureselectedl;
                                            CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                            WindowBorderFacture.DataContext = selectedF1;
                                            var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                            string strTTC = string.Format("{0:0.00}", TTC);
                                            txtTTC.Text = strTTC;
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }
                                    }
                                }
                                else
                                {
                                 
                                        if ((selectedF1.ht == 0 && selectedF1.versement == 0) && ExisteMonture == false && existelentille == true)
                                        {
                                            bool ii = false;
                                            bool updatemonture = false;
                                            SVC.LentilleClient monture1 = proxy.GetAllLentilleClientbyDossier(Convert.ToString(selectedF1.cleDossier)).Last();

                                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                            {
                                                ii = proxy.UpdateFacture(selectedF1, NouvelleFacture, AncienneFacture, document);

                                                Operfacture = true;

                                                monture1.StatutDevis = true;
                                                monture1.StatutVente = false;
                                                proxy.UpdateLentilleClient(monture1);
                                                updatemonture = true;

                                                if (Operfacture == true && ii == true && updatemonture == true)
                                                {
                                                    ts.Complete();
                                                    facturemodif = true;
                                                    facturenew = false;

                                                }
                                            }
                                            if (Operfacture == true && ii == true && updatemonture == true)
                                            {


                                                proxy.AjouterProdflistRefresh(listrefresh);
                                                proxy.AjouterFactureVenteRefresh(selectedF1.cle);
                                                proxy.AjouterSoldeF1Refresh();
                                                NonFacturationDesign();
                                                proxy.AjouterTransactionPaiementRefresh();
                                                proxy.AjouterDepenseRefresh();
                                                proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                                GridLentille.IsEnabled = false;
                                                GridLentille.DataContext = null;
                                                LentilleClass = null;
                                                Lentilleversementzero = false;
                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            }
                                            else
                                            {

                                                if (anciennefactureselectedl != null)
                                                {

                                                    /***************************************************/
                                                    ancienneF1 = proxy.GetAllF1ByVisiteOper(selectedF1.nfact).First();
                                                    factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                                    anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                                    /************************************************************************/
                                                    ReceptDatagrid.ItemsSource = factureselectedl;
                                                    ReceptDatagrid.DataContext = factureselectedl;
                                                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                                    WindowBorderFacture.DataContext = selectedF1;
                                                    var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                                    string strTTC = string.Format("{0:0.00}", TTC);
                                                    txtTTC.Text = strTTC;
                                                    NonFacturationDesign();
                                                    proxy.AjouterTransactionPaiementRefresh();
                                                    proxy.AjouterDepenseRefresh();
                                                    proxy.AjouterMontureRefresh(Clientvv.Id);
                                                   
                                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                 

                                }
                            }
                        }



                    }
                }
                else
                {
                    if (facturenew == true && facturemodif == false && memberuser.CreationDossierClient == true)
                    {
                        string document = "";
                        if (documenttype == 1)
                        {
                            document = "F";
                        }
                        else
                        {
                            if (documenttype == 2)
                            {
                                document = "A";
                            }
                            else
                            {
                                if (documenttype == 3)
                                {
                                    document = "B";
                                }
                                else
                                {
                                    if (documenttype == 4)
                                    {
                                        document = "C";
                                    }
                                    else
                                    {

                                        if (documenttype == 5)
                                        {
                                            document = "P";
                                        }
                                        else
                                        {
                                            if (documenttype == 6)
                                            {
                                                document = "R";

                                            }
                                        }

                                    }
                                }
                            }
                        }

                        if (Clientvv.Id != 0)
                        {

                            if (txtRemise.Text != "")
                            {
                                if (Convert.ToDecimal(txtRemise.Text) != 0)
                                {
                                    nouveauF1.remise = Convert.ToDecimal(txtRemise.Text);
                                }
                                else
                                {
                                    nouveauF1.remise = 0;
                                }

                            }
                            else
                            {
                                nouveauF1.remise = 0;
                            }



                            /************************************************/

                            if (documenttype == 2 || documenttype == 4)
                            {
                                decimal timbre = 0;
                                decimal remise = 0;
                                decimal avecremise = 0;
                                if (txtRemise.Text != "")
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) != 0)
                                    {
                                        avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text));
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                    }
                                    else
                                    {
                                        avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                        remise = 0;
                                    }
                                }
                                else
                                {
                                    avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                }
                                if (tunisie != true)
                                {
                                    if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                                    {
                                        nouveauF1.timbre = (avecremise * 1 / 100);
                                        timbre = (avecremise * 1 / 100);
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;

                                    }
                                }
                                else
                                {
                                    if (chFactureAvoir.IsChecked == true)
                                    {
                                        nouveauF1.timbre = -Convert.ToDecimal(0.6);
                                        timbre = -timbre;
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;
                                    }
                                }
                                nouveauF1.net = (avecremise + timbre);
                                if (txtnVersement.Text != "")
                                {
                                    if (Convert.ToDecimal(txtnVersement.Text) != 0)
                                    {
                                        nouveauF1.versement = Convert.ToDecimal(txtnVersement.Text);
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }
                                    else
                                    {
                                        nouveauF1.versement = 0;
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }

                                }
                                else
                                {
                                    nouveauF1.versement = 0;
                                    nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                }

                                nouveauF1.tva = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                nouveauF1.ht = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                if (nouveauF1.reste != 0)
                                {
                                    nouveauF1.soldé = false;
                                }
                                else
                                {
                                    nouveauF1.soldé = true;

                                }
                            }
                            else
                            {
                                decimal timbre = 0;
                                decimal remise = 0;
                                decimal avecremise = 0;
                                if (txtRemise.Text != "")
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) != 0)
                                    {
                                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                    }
                                    else
                                    {
                                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                        remise = 0;
                                    }
                                }
                                else
                                {
                                    avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                }
                                if (tunisie != true)
                                {
                                    if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true))
                                    {
                                        nouveauF1.timbre = (avecremise * 1 / 100);
                                        timbre = (avecremise * 1 / 100);
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;

                                    }
                                }
                                else
                                {
                                    if (chFacture.IsChecked == true)
                                    {
                                        nouveauF1.timbre = Convert.ToDecimal(0.6); ;
                                        timbre = Convert.ToDecimal(0.6);
                                    }
                                    else
                                    {
                                        nouveauF1.timbre = 0;
                                        timbre = 0;
                                    }
                                }
                                nouveauF1.net = avecremise + timbre;
                                if (txtnVersement.Text != "")
                                {
                                    if (Convert.ToDecimal(txtnVersement.Text) != 0)
                                    {
                                        nouveauF1.versement = Convert.ToDecimal(txtnVersement.Text);
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }
                                    else
                                    {
                                        nouveauF1.versement = 0;
                                        nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                    }

                                }
                                else
                                {
                                    nouveauF1.versement = 0;
                                    nouveauF1.reste = nouveauF1.net - nouveauF1.versement;

                                }

                                nouveauF1.tva = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                nouveauF1.ht = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                if (nouveauF1.reste != 0)
                                {
                                    nouveauF1.soldé = false;
                                }
                                else
                                {
                                    nouveauF1.soldé = true;

                                }
                            }
                            if (nouveauF1.echeance != 0)
                            {
                                nouveauF1.datecheance = nouveauF1.date.Value.AddDays(Convert.ToDouble(nouveauF1.echeance));
                            }
                            nouveauF1.cle = Clientvv.Id + Clientvv.Raison + nouveauF1.net + DateTime.Now.TimeOfDay;
                            nouveauF1.heure = DateTime.Now.TimeOfDay;
                            /*****************************************************/

                            var remisepourfacture = nouveauF1.remise;
                            bool Operfacture = false;

                            //    List<int> listrefresh = new List<int>();

                            foreach (SVC.Facture newfacture in factureselectedl)
                            {
                                newfacture.cle = nouveauF1.cle;
                                newfacture.codeclient = nouveauF1.codeclient;
                                if (remisepourfacture != 0)
                                {
                                    if (remisepourfacture >= ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite)))
                                    {
                                        newfacture.remise = ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                        remisepourfacture = remisepourfacture - ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                    }
                                    else
                                    {
                                        newfacture.remise = remisepourfacture;
                                    }

                                }
                                else
                                {
                                    newfacture.remise = 0;
                                }
                                /*  if (newfacture.codeprod != 0)
                                  {
                                      listrefresh.Add(Convert.ToInt32(newfacture.ficheproduit));
                                  }*/
                            }


                            if (nouveauF1.versement != 0)
                            {


                                SVC.Depeiment PAIEMENT = new SVC.Depeiment
                                {
                                    date = nouveauF1.date,
                                    montant = Convert.ToDecimal(nouveauF1.versement),
                                    paiem = "ESPECES" + " Vente :" + nouveauF1.nfact + " " + " date :" + nouveauF1.date,
                                    oper = memberuser.Username,
                                    dates = nouveauF1.dates,
                                    banque = "Caisse",
                                    nfact = nouveauF1.nfact,
                                    amontant = Convert.ToDecimal(nouveauF1.net),
                                    cle = nouveauF1.cle,
                                    cp = nouveauF1.Id,
                                    Multiple = false,
                                    CodeClient = nouveauF1.codeclient,
                                    RaisonClient = nouveauF1.raison,

                                };
                                SVC.Depense CAISSE = new SVC.Depense
                                {
                                    cle = nouveauF1.cle,
                                    Auto = true,
                                    Commentaires = "ESPECES" + " Vente :" + nouveauF1.nfact + " " + " date :" + nouveauF1.date,
                                    CompteDébité = "Caisse",
                                    Crédit = true,
                                    DateDebit = nouveauF1.date,
                                    DateSaisie = nouveauF1.dates,
                                    Débit = false,
                                    ModePaiement = "ESPECES",
                                    Montant = 0,
                                    MontantCrédit = nouveauF1.versement,
                                    NumCheque = Convert.ToString(nouveauF1.Id),
                                    Num_Facture = nouveauF1.nfact,
                                    RubriqueComptable = "ESPECES document de vente: " + nouveauF1.raison + " " + nouveauF1.nfact,
                                    Username = memberuser.Username,

                                };
                                bool ii = false;
                                bool depense = false;
                                bool depaiement = false;
                                List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                foreach (var item in factureselectedl)
                                {
                                    if (item.quantite != 0)
                                    {
                                        item.cle = nouveauF1.cle;
                                        listsanszero.Add(item);
                                    }
                                }
                                if (interfacefacturation == 0)
                                {
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        ii = proxy.InsertFactureSansStock(nouveauF1, listsanszero, document);
                                        Operfacture = true;
                                        if (documenttype != 6 && documenttype != 5)
                                        {
                                            proxy.InsertDepeiment(PAIEMENT);
                                            depaiement = true;
                                            proxy.InsertDepense(CAISSE);
                                            depense = true;
                                        }
                                        else
                                        {

                                            if (documenttype == 5 || documenttype == 6)
                                            {
                                                depense = true;
                                                depaiement = true;
                                            }

                                        }

                                        if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                        {
                                            ts.Complete();
                                            facturemodif = true;
                                            facturenew = false;

                                        }
                                    }
                                    if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                    {

                                        //   proxy.AjouterProdflistRefresh(listrefresh);
                                        proxy.AjouterSoldeF1Refresh();
                                        // proxy.AjouterDepenseRefresh();
                                        NonFacturationDesignNonStock();
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                }
                                else
                                {
                                    
                                        /****************lentille****************************/
                                        if (interfacefacturation == 2)
                                        {
                                            bool modiflentille = false;
                                            bool bonfacture = false;
                                            var existeversement = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).Any();
                                            SVC.Depeiment depaiementclass = new SVC.Depeiment(); ;
                                            SVC.Depense depenseclass = new SVC.Depense();

                                            if (existeversement == true)
                                            {
                                                depaiementclass = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).First();
                                                depenseclass = proxy.GetAllDepenseByF1(nouveauF1.cleDossier).First();
                                            }

                                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                            {
                                                ii = proxy.InsertFactureSansStock(nouveauF1, listsanszero, document);

                                                Operfacture = true;
                                                if (existeversement == false)
                                                {
                                                    if (documenttype == 1 || documenttype == 3)
                                                    {
                                                        depaiement = false;
                                                        proxy.InsertDepeiment(PAIEMENT);
                                                        depaiement = true;
                                                        depense = false;
                                                        proxy.InsertDepense(CAISSE);
                                                        depense = true;

                                                        modiflentille = false;
                                                        LentilleClass.Encaissé = nouveauF1.versement;
                                                        LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                        LentilleClass.StatutVente = true;

                                                        proxy.UpdateLentilleClient(LentilleClass);
                                                        modiflentille = true;
                                                        bonfacture = true;
                                                    }
                                                    else
                                                    {
                                                        modiflentille = true;
                                                        depense = true;
                                                        depaiement = true;
                                                    }
                                                }
                                                else
                                                {
                                                    /* depaiement = true;
                                                     depense = true;
                                                     LentilleClass.StatutVente = true;
                                                     proxy.UpdateLentilleClient(LentilleClass);
                                                     modiflentille = true;*/

                                                    if (nouveauF1.versement != 0)
                                                    {
                                                        if (documenttype == 1 || documenttype == 3)
                                                        {
                                                            depaiement = false;
                                                            depaiementclass.montant = nouveauF1.versement;
                                                            depaiementclass.cle = nouveauF1.cle;
                                                            proxy.UpdateDepeiment(depaiementclass);
                                                            depaiement = true;
                                                            depense = false;
                                                            depenseclass.MontantCrédit = nouveauF1.versement;
                                                            depenseclass.cle = nouveauF1.cle;
                                                            proxy.UpdateDepense(depenseclass);
                                                            depense = true;
                                                            modiflentille = false;
                                                            LentilleClass.Encaissé = nouveauF1.versement;
                                                            LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                            LentilleClass.StatutVente = true;

                                                            proxy.UpdateLentilleClient(LentilleClass);
                                                            modiflentille = true;
                                                            bonfacture = true;
                                                        }
                                                        else
                                                        {
                                                            modiflentille = true; depense = true; depaiement = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (documenttype == 1 || documenttype == 3)
                                                        {
                                                            depaiement = false;
                                                            proxy.DeleteDepeiment(depaiementclass);

                                                            depaiement = true;
                                                            depense = false;
                                                            proxy.DeleteDepense(depenseclass);
                                                            depense = true;
                                                            modiflentille = false;
                                                            LentilleClass.Encaissé = nouveauF1.versement;
                                                            LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                            LentilleClass.StatutVente = true;

                                                            proxy.UpdateLentilleClient(LentilleClass);
                                                            modiflentille = true;
                                                            bonfacture = true;
                                                        }
                                                        else
                                                        {
                                                            modiflentille = true; depense = true; depaiement = true;

                                                        }
                                                    }
                                                }
                                                if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                                {
                                                    ts.Complete();
                                                    facturemodif = false;
                                                    facturenew = false;

                                                }
                                            }
                                            if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                            {

                                                //  proxy.AjouterProdflistRefresh(listrefresh);
                                                proxy.AjouterSoldeF1Refresh();
                                                if (bonfacture == true)
                                                {
                                                    proxy.AjouterLentilleClientRefresh(Clientvv.Id);

                                                }
                                                // proxy.AjouterDepenseRefresh();
                                                GridLentille.IsEnabled = false;
                                                GridLentille.DataContext = null;
                                                LentilleClass = null;
                                                Lentilleversementzero = false;

                                                NonFacturationDesignNonStock();
                                                MessageBoxResult resFult = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            }
                                        }
                                    
                                }
                            }
                            /********************versment ==0)********/
                            else
                            {

                                bool ii = false;
                                List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                foreach (var item in factureselectedl)
                                {
                                    if (item.quantite != 0)
                                    {
                                        item.cle = nouveauF1.cle;
                                        listsanszero.Add(item);
                                    }
                                }
                       
                               
                                    if (interfacefacturation == 0)
                                    {
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            ii = proxy.InsertFactureSansStock(nouveauF1, listsanszero, document);

                                            Operfacture = true;


                                            if (Operfacture == true && ii == true)
                                            {
                                                ts.Complete();
                                                facturemodif = true;
                                                facturenew = false;

                                            }
                                        }
                                        if (Operfacture == true && ii == true)
                                        {

                                            //   proxy.AjouterProdflistRefresh(listrefresh);
                                            proxy.AjouterSoldeF1Refresh();

                                            NonFacturationDesignNonStock();
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                        }
                                    }
                                    else
                                    {
                                        if (interfacefacturation == 2)
                                        {
                                            bool modiflentille = false;
                                            bool depaiement = false;
                                            bool depense = false;
                                            bool facturebon = true;
                                            var existeversement = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).Any();
                                            SVC.Depeiment depaiementclass = new SVC.Depeiment(); ;
                                            SVC.Depense depenseclass = new SVC.Depense();
                                            //  MessageBoxResult resFult = Xceed.Wpf.Toolkit.MessageBox.Show(existeversement.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            if (existeversement == true)
                                            {

                                                depaiementclass = proxy.GetAllDepeimentByF1(nouveauF1.cleDossier).First();
                                                depenseclass = proxy.GetAllDepenseByF1(nouveauF1.cleDossier).First();
                                            }


                                            if (existeversement == true)
                                            {
                                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                {
                                                    ii = proxy.InsertFactureSansStock(nouveauF1, listsanszero, document);

                                                    Operfacture = true;
                                                    if (documenttype == 1 || documenttype == 3)
                                                    {
                                                        depaiement = false;
                                                        proxy.DeleteDepeiment(depaiementclass);

                                                        depaiement = true;
                                                        depense = false;
                                                        proxy.DeleteDepense(depenseclass);
                                                        depense = true;
                                                        modiflentille = false;
                                                        LentilleClass.Encaissé = nouveauF1.versement;
                                                        LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                        LentilleClass.StatutVente = true;

                                                        proxy.UpdateLentilleClient(LentilleClass);
                                                        modiflentille = true;
                                                        facturebon = true;
                                                    }
                                                    else
                                                    {
                                                        modiflentille = true; depense = true; depaiement = true;
                                                    }
                                                    if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                                    {
                                                        ts.Complete();
                                                        facturemodif = false;
                                                        facturenew = false;

                                                    }

                                                }
                                                if (Operfacture == true && ii == true && depaiement == true && depense == true && modiflentille == true)
                                                {

                                                    //proxy.AjouterProdflistRefresh(listrefresh);
                                                    proxy.AjouterSoldeF1Refresh();
                                                    if (facturebon == true)
                                                    {
                                                        proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                                    }
                                                    //  proxy.AjouterDepenseRefresh();
                                                    GridLentille.IsEnabled = false;
                                                    GridLentille.DataContext = null;
                                                    LentilleClass = null;
                                                    Lentilleversementzero = false;

                                                    NonFacturationDesignNonStock();
                                                    MessageBoxResult sresFult = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                }

                                            }
                                            else
                                            {

                                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                {
                                                    ii = proxy.InsertFactureSansStock(nouveauF1, listsanszero, document);

                                                    Operfacture = true;
                                                    if (documenttype == 1 || documenttype == 3)
                                                    {
                                                        LentilleClass.Encaissé = nouveauF1.versement;
                                                        LentilleClass.Reste = LentilleClass.MontantTotal - nouveauF1.versement;
                                                        LentilleClass.StatutVente = true;
                                                        proxy.UpdateLentilleClient(LentilleClass);
                                                        modiflentille = true;
                                                        facturebon = true;
                                                    }
                                                    else
                                                    {
                                                        modiflentille = true;
                                                    }

                                                    if (Operfacture == true && ii == true && modiflentille == true)
                                                    {
                                                        ts.Complete();
                                                        facturemodif = false;
                                                        facturenew = false;

                                                    }
                                                }
                                                if (Operfacture == true && ii == true && modiflentille == true)
                                                {

                                                    // proxy.AjouterProdflistRefresh(listrefresh);
                                                    proxy.AjouterSoldeF1Refresh();
                                                    if (facturebon == true)
                                                    {
                                                        proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                                    }
                                                    GridLentille.IsEnabled = false;
                                                    GridLentille.DataContext = null;
                                                    LentilleClass = null;
                                                    Lentilleversementzero = false;

                                                    NonFacturationDesignNonStock();
                                                    MessageBoxResult resDFult = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);


                                                }
                                            }

                                        }

                                    }

                         
                            }






                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un autre client", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                    }
                    else
                    {
                        if (facturenew == false && facturemodif == true && memberuser.ModificationDossierClient == true)
                        {

                            /************************La partie F1***********************/
                            String document = "";
                            if (documenttype == 1)
                            {
                                document = "F";
                            }
                            else
                            {
                                if (documenttype == 2)
                                {
                                    document = "A";
                                }
                                else
                                {
                                    if (documenttype == 3)
                                    {
                                        document = "B";
                                    }
                                    else
                                    {
                                        if (documenttype == 4)
                                        {
                                            document = "C";
                                        }
                                        else
                                        {
                                            if (documenttype == 5)
                                            {
                                                document = "P";
                                            }
                                            else
                                            {
                                                if (documenttype == 6)
                                                {
                                                    document = "R";

                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            if (Clientvv.Id != 0)
                            {



                                if (txtRemise.Text != "")
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) != 0)
                                    {
                                        if (ancienneF1.remise != Convert.ToDecimal(txtRemise.Text))
                                        {
                                            selectedF1.remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                    }
                                    else
                                    {
                                        selectedF1.remise = 0;
                                    }

                                }
                                else
                                {
                                    selectedF1.remise = 0;
                                }



                                /************************************************/

                                if (documenttype == 2 || documenttype == 4)
                                {
                                    decimal timbre = 0;
                                    decimal remise = 0;
                                    decimal avecremise = 0;
                                    if (txtRemise.Text != "")
                                    {
                                        if (Convert.ToDecimal(txtRemise.Text) != 0)
                                        {
                                            avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text));
                                            remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                        else
                                        {
                                            avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                            remise = 0;
                                        }
                                    }
                                    else
                                    {
                                        avecremise = (-Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))));
                                    }
                                    if (tunisie != true)
                                    {
                                        if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                                        {
                                            selectedF1.timbre = (avecremise * 1 / 100);
                                            timbre = (avecremise * 1 / 100);
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;

                                        }
                                    }
                                    else
                                    {
                                        if (chFactureAvoir.IsChecked == true)
                                        {
                                            selectedF1.timbre = -Convert.ToDecimal(0.6); ;
                                            timbre = -timbre;
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;
                                        }
                                    }
                                    selectedF1.net = (avecremise + timbre);
                                    selectedF1.reste = selectedF1.net - selectedF1.versement;



                                    selectedF1.tva = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                    selectedF1.ht = -(factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                    if (selectedF1.reste != 0)
                                    {
                                        selectedF1.soldé = false;
                                    }
                                    else
                                    {
                                        selectedF1.soldé = true;

                                    }
                                }
                                else
                                {
                                    decimal timbre = 0;
                                    decimal remise = 0;
                                    decimal avecremise = 0;
                                    if (txtRemise.Text != "")
                                    {
                                        if (Convert.ToDecimal(txtRemise.Text) != 0)
                                        {
                                            avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                            remise = Convert.ToDecimal(txtRemise.Text);
                                        }
                                        else
                                        {
                                            avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                            remise = 0;
                                        }
                                    }
                                    else
                                    {
                                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    }

                                    if (tunisie != true)
                                    {
                                        if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true))
                                        {
                                            selectedF1.timbre = (avecremise * 1 / 100);
                                            timbre = (avecremise * 1 / 100);
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;

                                        }
                                    }
                                    else
                                    {
                                        if (chFacture.IsChecked == true)
                                        {
                                            selectedF1.timbre = Convert.ToDecimal(0.6);
                                            timbre = Convert.ToDecimal(0.6);
                                        }
                                        else
                                        {
                                            selectedF1.timbre = 0;
                                            timbre = 0;
                                        }
                                    }
                                    selectedF1.net = avecremise + timbre;
                                    selectedF1.reste = selectedF1.net - selectedF1.versement;



                                    selectedF1.tva = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite));
                                    selectedF1.ht = (factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.quantite))));
                                    if (selectedF1.reste != 0)
                                    {
                                        selectedF1.soldé = false;
                                    }
                                    else
                                    {
                                        selectedF1.soldé = true;

                                    }
                                }
                                if (selectedF1.echeance != 0)
                                {
                                    if (selectedF1.echeance != ancienneF1.echeance)
                                    {
                                        selectedF1.datecheance = selectedF1.date.Value.AddDays(Convert.ToDouble(selectedF1.echeance));
                                    }
                                }

                                /***************************************************/

                                List<SVC.Facture> NouvelleFacture = new List<SVC.Facture>();
                                List<SVC.Facture> AncienneFacture = new List<SVC.Facture>();
                                var remisepourfacture = selectedF1.remise;

                                bool Operfacture = false;


                                foreach (SVC.Facture newfacture in factureselectedl)
                                {

                                    var found = (anciennefactureselectedl).Any(itemf => itemf.ficheproduit == newfacture.ficheproduit);
                                    if (found == false)
                                    {
                                        if (newfacture.quantite != 0)
                                        {
                                            newfacture.cle = selectedF1.cle;
                                            newfacture.nfact = selectedF1.nfact;


                                            NouvelleFacture.Add(newfacture);
                                        }


                                    }
                                    else
                                    {
                                        if (remisepourfacture != 0)
                                        {
                                            if (remisepourfacture >= ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite)))
                                            {
                                                newfacture.remise = ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                                remisepourfacture = remisepourfacture - ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                                            }
                                            else
                                            {
                                                newfacture.remise = remisepourfacture;
                                            }

                                        }
                                        else
                                        {
                                            newfacture.remise = 0;
                                        }
                                        AncienneFacture.Add(newfacture);

                                    }
                                }
                                bool ExisteMonture = false;
                                ExisteMonture = proxy.GetAllMonturebyDossier(Convert.ToString(selectedF1.cleDossier)).Any();
                                bool existelentille = false;
                                existelentille = proxy.GetAllLentilleClientbyDossier(Convert.ToString(selectedF1.cleDossier)).Any();

                                if (!(selectedF1.ht == 0 && selectedF1.versement != 0) && ExisteMonture == false && existelentille == false)
                                {


                                    bool ii = false;
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        ii = proxy.UpdateFactureSansStock(selectedF1, NouvelleFacture, AncienneFacture, document);

                                        Operfacture = true;



                                        if (Operfacture == true && ii == true)
                                        {
                                            ts.Complete();
                                            facturemodif = true;
                                            facturenew = false;

                                        }
                                    }
                                    if (Operfacture == true && ii == true)
                                    {

                                        // proxy.AjouterProdflistRefresh(listrefresh);
                                        proxy.AjouterFactureVenteRefresh(selectedF1.cle);
                                        proxy.AjouterSoldeF1Refresh();

                                        NonFacturationDesignNonStock();
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        /*******************************************************/
                                        //  selectedF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                                        /*     ancienneF1 = proxy.GetAllF1ByVisiteOper(selectedF1.nfact).First();
                                             factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                             anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact); 
                                             /************************************************************************/
                                        /*    ReceptDatagrid.ItemsSource = factureselectedl;
                                          ReceptDatagrid.DataContext = factureselectedl;
                                          CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                          RéglementGfsdfsrid.DataContext = selectedF1;
                                          var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                          string strTTC = string.Format("{0:0.00}", TTC);
                                          txtTTC.Text = strTTC;*/
                                    }
                                    else
                                    {

                                        if (anciennefactureselectedl != null)
                                        {

                                            /***************************************************/
                                            ancienneF1 = proxy.GetAllF1ByVisiteOper(selectedF1.nfact).First();
                                            factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                            anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                            /************************************************************************/
                                            ReceptDatagrid.ItemsSource = factureselectedl;
                                            ReceptDatagrid.DataContext = factureselectedl;
                                            CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                            WindowBorderFacture.DataContext = selectedF1;
                                            var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                            string strTTC = string.Format("{0:0.00}", TTC);
                                            txtTTC.Text = strTTC;
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }
                                    }
                                }
                                else
                                {
                                  
                                   
                                        if ((selectedF1.ht == 0 && selectedF1.versement == 0) && ExisteMonture == false && existelentille == true)
                                        {
                                            bool ii = false;
                                            bool updatemonture = false;
                                            SVC.LentilleClient monture1 = proxy.GetAllLentilleClientbyDossier(Convert.ToString(selectedF1.cleDossier)).Last();

                                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                            {
                                                ii = proxy.UpdateFactureSansStock(selectedF1, NouvelleFacture, AncienneFacture, document);

                                                Operfacture = true;

                                                monture1.StatutDevis = true;
                                                monture1.StatutVente = false;
                                                proxy.UpdateLentilleClient(monture1);
                                                updatemonture = true;

                                                if (Operfacture == true && ii == true && updatemonture == true)
                                                {
                                                    ts.Complete();
                                                    facturemodif = true;
                                                    facturenew = false;

                                                }
                                            }
                                            if (Operfacture == true && ii == true && updatemonture == true)
                                            {


                                                //   proxy.AjouterProdflistRefresh(listrefresh);
                                                proxy.AjouterFactureVenteRefresh(selectedF1.cle);
                                                proxy.AjouterSoldeF1Refresh();
                                                NonFacturationDesignNonStock();
                                                proxy.AjouterTransactionPaiementRefresh();
                                                proxy.AjouterDepenseRefresh();
                                                proxy.AjouterLentilleClientRefresh(Clientvv.Id);
                                                GridLentille.IsEnabled = false;
                                                GridLentille.DataContext = null;
                                                LentilleClass = null;
                                                Lentilleversementzero = false;
                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                            }
                                            else
                                            {

                                                if (anciennefactureselectedl != null)
                                                {

                                                    /***************************************************/
                                                    ancienneF1 = proxy.GetAllF1ByVisiteOper(selectedF1.nfact).First();
                                                    factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                                    anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                                                    /************************************************************************/
                                                    ReceptDatagrid.ItemsSource = factureselectedl;
                                                    ReceptDatagrid.DataContext = factureselectedl;
                                                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                                    WindowBorderFacture.DataContext = selectedF1;
                                                    var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                                    string strTTC = string.Format("{0:0.00}", TTC);
                                                    txtTTC.Text = strTTC;
                                                    NonFacturationDesign();
                                                    proxy.AjouterTransactionPaiementRefresh();
                                                    proxy.AjouterDepenseRefresh();
                                                    proxy.AjouterMontureRefresh(Clientvv.Id);
                                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                   

                                }
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
        void NonFacturationDesignNonStock()
        {

            Grid.SetColumnSpan(RéglemdfentGfsdfsrid, 2);
            Grid.SetRowSpan(ListeDesDocuments, 8);
            WindowBorderFacture.Visibility = Visibility.Collapsed;
            gridspl.Visibility = Visibility.Collapsed;
            NomenclatureDatagrid.Visibility = Visibility.Collapsed;
            girdspi1.Visibility = Visibility.Collapsed;
            factureselectedl = new List<SVC.Facture>();
            BtnCreerProduit.Visibility = Visibility.Collapsed;
            nouveauF1 = new SVC.F1();
            //RéglementGfsdfsrid.DataContext = nouveauF1;

            fermer = false;
            documenttype = 0;
            facturenonstock = false;
        }

        private void annulerVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NonFacturationDesign();
                ListeDesDocuments.ItemsSource = proxy.GetAllF1Bycode(Clientvv.Id).Where(n => n.nfact.Substring(0, 2) != "Co").OrderBy(n => n.date);
                interfacefacturation = 0;
                facturenew = false;
                facturemodif = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtRemise_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Decimal:
                case Key.Tab:
                case Key.Subtract:
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void txtRemise_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {


                if (documenttype == 2 || documenttype == 4)
                {
                    decimal timbre = 0;
                    decimal remise = 0;
                    decimal avecremise = 0;
                    decimal Benf = 0;
                    if (txtRemise.Text != "")
                    {

                        if (txtRemise.Text.Count() > 1)
                        {
                            if (Convert.ToDecimal(txtRemise.Text) != 0)
                            {
                                // var net = Convert.ToDecimal((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite));
                                // if (((selectedparam.maxremisevente * net) / 100 >= Convert.ToDecimal(txtRemise.Text)))
                                //{
                                if (Convert.ToDecimal(txtRemise.Text) < 0)
                                {
                                    avecremise = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                }
                                else
                                {
                                    avecremise = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                    remise = Convert.ToDecimal(txtRemise.Text);
                                    Benf = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                }
                                // }
                                /*  else
                                  {
                                      txtRemise.Text = "";
                                      avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                      remise = 0;
                                      Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));
                                  }*/


                            }
                            else
                            {
                                avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                remise = Convert.ToDecimal(txtRemise.Text);
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                            }

                        }
                        else
                        {
                            if (txtRemise.Text.Count() == 1)
                            {
                                var letter = txtRemise.Text.Substring(0, 1);

                                if (letter == "-")
                                {

                                    avecremise = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                    remise = 0;
                                    Benf = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));
                                }
                                else
                                {
                                    if (Convert.ToDecimal(txtRemise.Text) > 0)
                                    {
                                        avecremise = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                        remise = Convert.ToDecimal(txtRemise.Text);
                                        Benf = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));
                                    }

                                }

                            }
                        }
                    }
                    else
                    {
                        txtRemise.Text = "";
                        avecremise = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                        remise = 0;
                        Benf = -Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                    }
                    //      Bénéficemont.Text = Benf.ToString();

                    if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && (chFacture.IsChecked == true || chFactureAvoir.IsChecked == true))
                    {
                        timbre = (avecremise * 1 / 100);
                        if (timbre > 2500)
                        {
                            timbre = 2500;
                        }
                        txtTimbre.Text = Convert.ToString(timbre);
                    }
                    else
                    {
                        timbre = 0;
                        txtTimbre.Text = Convert.ToString(timbre);

                    }
                    txtNet.Text = Convert.ToString((avecremise + timbre));
                    Bénéficemont.Text = String.Format("{0:0.##}", (Benf));
                }
                else
                {
                    decimal timbre = 0;
                    decimal remise = 0;
                    decimal avecremise = 0;
                    decimal Benf = 0;
                    if (txtRemise.Text != "")
                    {
                        if (Convert.ToDecimal(txtRemise.Text) != 0)
                        {
                            var net = Convert.ToDecimal((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite));
                            if (((selectedparam.maxremisevente * net) / 100 >= Convert.ToDecimal(txtRemise.Text)))
                            {
                                avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                                remise = Convert.ToDecimal(txtRemise.Text);
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                            }
                            else
                            {
                                txtRemise.Text = "";
                                avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                                remise = 0;
                                Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));
                            }


                        }
                        else
                        {
                            avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)))) - Convert.ToDecimal(txtRemise.Text);
                            remise = Convert.ToDecimal(txtRemise.Text);
                            Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtRemise.Text));

                        }
                    }
                    else
                    {
                        txtRemise.Text = "";
                        avecremise = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite))));
                        remise = 0;
                        Benf = Convert.ToDecimal(((factureselectedl).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((factureselectedl).AsEnumerable().Sum(o => o.previent * o.quantite)));

                    }
                    Bénéficemont.Text = String.Format("{0:0.##}", Benf);
                    if (((ComboBoxItem)Modep.SelectedItem).Content.ToString() == "ESPECES" && chFacture.IsChecked == true)
                    {
                        timbre = (avecremise * 1 / 100);
                        if (timbre > 2500)
                        {
                            timbre = 2500;
                        }
                        txtTimbre.Text = Convert.ToString(timbre);
                    }
                    else
                    {
                        timbre = 0;
                        txtTimbre.Text = Convert.ToString(timbre);

                    }
                    txtNet.Text = Convert.ToString(avecremise + timbre);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnNouvelleFacture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dialog1 = new Window();
                dialog1.Title = "Veuillez choisir un document";
                dialog1.Template = Resources["template3"] as ControlTemplate;

                // dialog1.Content = Resources["content"];
                dialog1.ResizeMode = ResizeMode.NoResize;
                dialog1.Width = 350;
                dialog1.Height = 250;
                dialog1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog1.ShowDialog();
                interfacefacturation = 0;
                facturenew = true;
                facturemodif = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }


        }

        private void btnModifierFacture_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (ListeDesDocuments.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {

                    selectedF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                    if (selectedF1.Auto != true)
                    {
                        if (selectedF1.nfact != "Ancien solde")
                        {
                            ancienneF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                            factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            FacturationDesign();
                            bool existemonture = proxy.GetAllMonturebyDossier(selectedF1.cleDossier).Any();
                            bool existelentille = proxy.GetAllLentilleClientbyDossier(selectedF1.cleDossier).Any();
                            if (existemonture == true && existelentille == false)
                            {
                                if (selectedF1.versement != 0)
                                {
                                    ReceptDatagrid.IsEnabled = false;
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }

                            }
                            else
                            {
                                if (existemonture == false && existelentille == true)
                                {
                                    if (selectedF1.versement != 0)
                                    {
                                        ReceptDatagrid.IsEnabled = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.IsEnabled = true;
                                    }
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }
                            }
                            if (factureselectedl != null)
                            {
                                ReceptDatagrid.ItemsSource = factureselectedl;
                                ReceptDatagrid.DataContext = factureselectedl;
                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                WindowBorderFacture.DataContext = selectedF1;
                                var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                string strTTC = string.Format("{0:0.00}", TTC);
                                txtTTC.Text = strTTC;

                            }
                            facturenew = false;
                            facturemodif = true;
                            string nfact = selectedF1.nfact.Substring(0, 1);

                            switch (nfact)
                            {
                                case "F":
                                    chFacture.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Facture";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 1;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "A":
                                    chFactureAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture d'avoir";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 2;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "B":
                                    chBonLivraison.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 3;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "C":
                                    chBonLivraisonAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "avoir bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 4;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }

                                    break;
                                case "P":
                                    chProforma.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Proforma";
                                    ReceptDatagrid.CanUserDeleteRows = true;
                                    documenttype = 5;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;



                                    break;
                                case "R":
                                    chFactureProvisoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture provisoir";
                                    ReceptDatagrid.CanUserDeleteRows = true;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;
                                    documenttype = 6;
                                    break;
                            }
                        }

                    }
                    else
                    {
                        if (selectedF1.nfact != "Ancien solde")
                        {
                            facturenonstock = true;
                            ancienneF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                            factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            FacturationDesignNonStock();
                            bool existemonture = proxy.GetAllMonturebyDossier(selectedF1.cleDossier).Any();
                            bool existelentille = proxy.GetAllLentilleClientbyDossier(selectedF1.cleDossier).Any();
                            if (existemonture == true && existelentille == false)
                            {
                                if (selectedF1.versement != 0)
                                {
                                    ReceptDatagrid.IsEnabled = false;
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }

                            }
                            else
                            {
                                if (existemonture == false && existelentille == true)
                                {
                                    if (selectedF1.versement != 0)
                                    {
                                        ReceptDatagrid.IsEnabled = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.IsEnabled = true;
                                    }
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }
                            }
                            if (factureselectedl != null)
                            {
                                ReceptDatagrid.ItemsSource = factureselectedl;
                                ReceptDatagrid.DataContext = factureselectedl;
                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                WindowBorderFacture.DataContext = selectedF1;
                                var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                string strTTC = string.Format("{0:0.00}", TTC);
                                txtTTC.Text = strTTC;

                            }
                            facturenew = false;
                            facturemodif = true;
                            string nfact = selectedF1.nfact.Substring(0, 1);

                            switch (nfact)
                            {
                                case "F":
                                    chFacture.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Facture";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 1;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "A":
                                    chFactureAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture d'avoir";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 2;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "B":
                                    chBonLivraison.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 3;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "C":
                                    chBonLivraisonAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "avoir bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 4;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }

                                    break;
                                case "P":
                                    chProforma.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Proforma";
                                    ReceptDatagrid.CanUserDeleteRows = true;
                                    documenttype = 5;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;



                                    break;
                                case "R":
                                    chFactureProvisoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture provisoir";
                                    ReceptDatagrid.CanUserDeleteRows = true;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;
                                    documenttype = 6;
                                    break;
                            }
                        }
                    }

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnImprimmerFacture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListeDesDocuments.SelectedItem != null)
                {
                    dialog1 = new Window();
                    dialog1.Title = "Impression";
                    dialog1.Template = Resources["template7"] as ControlTemplate;

                    // dialog1.Content = Resources["content"];
                    dialog1.ResizeMode = ResizeMode.NoResize;
                    dialog1.Width = 350;
                    dialog1.Height = 200;
                    dialog1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog1.ShowDialog();
                    interfaceimpressionfacture = 0;
                    visualiserFacture = false;
                }
                /* if (ListeDesDocuments.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                 {
                     SVC.F1 SelectedClient = ListeDesDocuments.SelectedItem as SVC.F1;
                     List<SVC.F1> facturelist = new List<SVC.F1>();
                     facturelist.Add(SelectedClient);
                     ImpressionFacture cl = new ImpressionFacture(proxy, facturelist, Clientvv);
                     cl.Show();
                 }
                 else
                 {
                     MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                 }*/
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ListeDesDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (ListeDesDocuments.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {

                    selectedF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                    if (selectedF1.Auto != true)
                    {
                        if (selectedF1.nfact != "Ancien solde")
                        {
                            ancienneF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                            factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            FacturationDesign();
                            bool existemonture = proxy.GetAllMonturebyDossier(selectedF1.cleDossier).Any();
                            bool existelentille = proxy.GetAllLentilleClientbyDossier(selectedF1.cleDossier).Any();
                            if (existemonture == true && existelentille == false)
                            {
                                if (selectedF1.versement != 0)
                                {
                                    ReceptDatagrid.IsEnabled = false;
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }

                            }
                            else
                            {
                                if (existemonture == false && existelentille == true)
                                {
                                    if (selectedF1.versement != 0)
                                    {
                                        ReceptDatagrid.IsEnabled = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.IsEnabled = true;
                                    }
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }
                            }
                            if (factureselectedl != null)
                            {
                                ReceptDatagrid.ItemsSource = factureselectedl;
                                ReceptDatagrid.DataContext = factureselectedl;
                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                WindowBorderFacture.DataContext = selectedF1;
                                var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                string strTTC = string.Format("{0:0.00}", TTC);
                                txtTTC.Text = strTTC;

                            }
                            facturenew = false;
                            facturemodif = true;
                            string nfact = selectedF1.nfact.Substring(0, 1);

                            switch (nfact)
                            {
                                case "F":
                                    chFacture.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Facture";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 1;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "A":
                                    chFactureAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture d'avoir";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 2;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "B":
                                    chBonLivraison.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 3;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "C":
                                    chBonLivraisonAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "avoir bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 4;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }

                                    break;
                                case "P":
                                    chProforma.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Proforma";
                                    ReceptDatagrid.CanUserDeleteRows = true;
                                    documenttype = 5;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;



                                    break;
                                case "R":
                                    chFactureProvisoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture provisoir";
                                    ReceptDatagrid.CanUserDeleteRows = true;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;
                                    documenttype = 6;
                                    break;
                            }
                        }

                    }
                    else
                    {
                        if (selectedF1.nfact != "Ancien solde")
                        {
                            facturenonstock = true;
                            ancienneF1 = ListeDesDocuments.SelectedItem as SVC.F1;
                            factureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            anciennefactureselectedl = proxy.GetAllFactureBycompteur(selectedF1.nfact);
                            FacturationDesignNonStock();
                            bool existemonture = proxy.GetAllMonturebyDossier(selectedF1.cleDossier).Any();
                            bool existelentille = proxy.GetAllLentilleClientbyDossier(selectedF1.cleDossier).Any();
                            if (existemonture == true && existelentille == false)
                            {
                                if (selectedF1.versement != 0)
                                {
                                    ReceptDatagrid.IsEnabled = false;
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }

                            }
                            else
                            {
                                if (existemonture == false && existelentille == true)
                                {
                                    if (selectedF1.versement != 0)
                                    {
                                        ReceptDatagrid.IsEnabled = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.IsEnabled = true;
                                    }
                                }
                                else
                                {
                                    ReceptDatagrid.IsEnabled = true;
                                }
                            }
                            if (factureselectedl != null)
                            {
                                ReceptDatagrid.ItemsSource = factureselectedl;
                                ReceptDatagrid.DataContext = factureselectedl;
                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                WindowBorderFacture.DataContext = selectedF1;
                                var TTC = ((factureselectedl).AsEnumerable().Sum(o => (((o.privente * o.tva) / 100) * o.quantite) + (o.privente * o.quantite)));
                                string strTTC = string.Format("{0:0.00}", TTC);
                                txtTTC.Text = strTTC;

                            }
                            facturenew = false;
                            facturemodif = true;
                            string nfact = selectedF1.nfact.Substring(0, 1);

                            switch (nfact)
                            {
                                case "F":
                                    chFacture.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Facture";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 1;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "A":
                                    chFactureAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture d'avoir";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 2;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "B":
                                    chBonLivraison.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 3;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                    break;
                                case "C":
                                    chBonLivraisonAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "avoir bon de livraison";
                                    ReceptDatagrid.CanUserDeleteRows = false;
                                    selectedparam = proxy.GetAllParamétre();
                                    documenttype = 4;
                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }

                                    break;
                                case "P":
                                    chProforma.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "modifier Proforma";
                                    ReceptDatagrid.CanUserDeleteRows = true;
                                    documenttype = 5;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;



                                    break;
                                case "R":
                                    chFactureProvisoir.IsChecked = true;
                                    txtnVersement.IsEnabled = false;
                                    NomDocumentLabel.Content = "Facture provisoir";
                                    ReceptDatagrid.CanUserDeleteRows = true;

                                    ReceptDatagrid.Columns[2].IsReadOnly = false;

                                    txtDateOper.IsEnabled = true;
                                    documenttype = 6;
                                    break;
                            }
                        }
                    }

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ListeDesDocuments_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListeDesDocuments.ItemsSource = proxy.GetAllF1Bycode(Clientvv.Id).Where(n => n.nfact.Substring(0, 2) != "Co").OrderBy(n => n.date);

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        
         
        private void chstockdisponible_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (facturenonstock != true)
                {
                    ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                    if (chstockdisponible.IsChecked == true && chcout.IsChecked == false)
                    {
                        //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                        cv00.Filter = delegate (object item)
                        {
                            SVC.Prodf temp = item as SVC.Prodf;
                            return temp.quantite > 0;


                        };
                        string ValueCompte = "";
                        if (CompteComboBox.SelectedIndex >= 0)
                        {

                            var test = NomenclatureProduit.ItemsSource as IEnumerable;



                            ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                            switch (ValueCompte)
                            {
                                case "Prixa":
                                    foreach (SVC.Prodf item in test)
                                    {
                                        item.privente = item.prixa;
                                    }

                                    break;
                                case "Prixb":
                                    foreach (SVC.Prodf item in test)
                                    {
                                        item.privente = item.prixb;
                                    }
                                    break;

                                case "Prixc":
                                    foreach (SVC.Prodf item in test)
                                    {
                                        item.privente = item.prixc;
                                    }
                                    break;

                                default:


                                    break;
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

        private void chstockdisponible_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (facturenonstock != true)
                {
                    ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                    if (chstockdisponible.IsChecked == false && chcout.IsChecked == false)
                    {
                        cv00.Filter = delegate (object item)
                        {
                            SVC.Prodf temp = item as SVC.Prodf;
                            return temp.quantite >= 0 || temp.quantite <= 0;


                        };
                        string ValueCompte = "";
                        if (CompteComboBox.SelectedIndex >= 0)
                        {

                            var test = NomenclatureProduit.ItemsSource as IEnumerable;



                            ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                            switch (ValueCompte)
                            {
                                case "Prixa":
                                    foreach (SVC.Prodf item in test)
                                    {
                                        item.privente = item.prixa;
                                    }

                                    break;
                                case "Prixb":
                                    foreach (SVC.Prodf item in test)
                                    {
                                        item.privente = item.prixb;
                                    }
                                    break;

                                case "Prixc":
                                    foreach (SVC.Prodf item in test)
                                    {
                                        item.privente = item.prixc;
                                    }
                                    break;

                                default:


                                    break;
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
        private void chcout_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcout.IsChecked == true)
                {

                    var existe = (proxy.GetAllProdf().Any(n => n.quantite != 0));
                    if (existe == true)
                    {
                        List<SVC.Prodf> listprodfcoutmoyen = new List<SVC.Prodf>();

                        var listeCPcode = (from ta in proxy.GetAllProdf()

                                           where ta.quantite != 0
                                           select ta.cp).Distinct();
                        foreach (int cp in listeCPcode)
                        {
                            SVC.Prodf pp = new SVC.Prodf
                            {
                                cp = cp,
                                quantite = 0,
                                previent = 0,
                                prixa = 0,
                                prixb = 0,
                                prixc = 0,

                            };
                            listprodfcoutmoyen.Add(pp);
                        }
                        List<SVC.Prodf> listprodf = (proxy.GetAllProdf().Where(n => n.quantite != 0).ToList());

                        foreach (var existeinfirst in listprodfcoutmoyen)
                        {

                            string ValueCompte = "";
                            if (CompteComboBox.SelectedIndex >= 0)
                            {

                                ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                                foreach (SVC.Prodf itemprodf in listprodf.Reverse<SVC.Prodf>())
                                {


                                    if (existeinfirst.cp == itemprodf.cp)
                                    {
                                        /*    existeinfirst.quantite = listprodf.AsEnumerable().Sum(n=>n.quantite);
                                            existeinfirst.previent = (listprodf.AsEnumerable().Sum(n => n.previent)) / existeinfirst.quantite;
                                            existeinfirst.privente = (listprodf.AsEnumerable().Sum(n => n.privente)) / existeinfirst.quantite;
                                        existeinfirst.prixa = (listprodf.AsEnumerable().Sum(n => n.prixa)) / existeinfirst.quantite;
                                        existeinfirst.prixb = (listprodf.AsEnumerable().Sum(n => n.prixb)) / existeinfirst.quantite;
                                        existeinfirst.prixc = (listprodf.AsEnumerable().Sum(n => n.prixc)) / existeinfirst.quantite;
                                        */

                                        existeinfirst.design = itemprodf.design;


                                        existeinfirst.design = itemprodf.design;
                                        existeinfirst.previent = ((existeinfirst.previent * existeinfirst.quantite) + (itemprodf.previent * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);

                                        existeinfirst.prixa = ((existeinfirst.prixa * existeinfirst.quantite) + (itemprodf.prixa * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);
                                        existeinfirst.prixb = ((existeinfirst.prixb * existeinfirst.quantite) + (itemprodf.prixb * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);
                                        existeinfirst.prixc = ((existeinfirst.prixc * existeinfirst.quantite) + (itemprodf.prixc * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);
                                        existeinfirst.quantite = existeinfirst.quantite + itemprodf.quantite;





                                        switch (ValueCompte)
                                        {
                                            case "Prixa":

                                                existeinfirst.privente = ((existeinfirst.prixa * existeinfirst.quantite) + (itemprodf.prixa * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);


                                                break;
                                            case "Prixb":

                                                existeinfirst.privente = ((existeinfirst.prixb * existeinfirst.quantite) + (itemprodf.prixb * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);

                                                break;

                                            case "Prixc":

                                                existeinfirst.privente = ((existeinfirst.prixc * existeinfirst.quantite) + (itemprodf.prixc * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);

                                                break;

                                            default:


                                                break;
                                        }


                                    }
                                }
                            }




                            NomenclatureProduit.ItemsSource = listprodfcoutmoyen;
                        }



                    }
                }
            }

            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chcout_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                NomenclatureProduit.ItemsSource = (proxy.GetAllProdf().OrderBy(n => n.design));
                string ValueCompte = "";
                if (CompteComboBox.SelectedIndex >= 0)
                {

                    var test = NomenclatureProduit.ItemsSource as IEnumerable;



                    ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                    switch (ValueCompte)
                    {
                        case "Prixa":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixa;
                            }

                            break;
                        case "Prixb":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixb;
                            }
                            break;

                        case "Prixc":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixc;
                            }
                            break;

                        default:


                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void BtnReception_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (memberuser.CreationCommande == true)
                {

                    AjouterCommande cl = new AjouterCommande(proxy, memberuser, callback, null, null, null);
                    cl.Show();


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void BtnService_Click(object sender, RoutedEventArgs e)
        {
            #region chked



            SVC.Facture facturevente = new SVC.Facture
            {
                cab = "",
                cf = 0,
                codeprod = 0,
                date = DateTime.Now,
                datef = DateTime.Now,
                design = "Service et maintenance",
                lot = "",
                oper = memberuser.Username,
                perempt = null,
                tva = 0,
                previent = 0,
                privente = 1,
                quantite = 1,
                Total = 1 * 1,
                ficheproduit = 0,
                ff = "Service et maintenance",
                Fournisseur = "Service et maintenance",
                collisage = 1,
            };
            var found = factureselectedl.Find(item => item.ficheproduit == 0);
            if (found == null)
            {

                factureselectedl.Add(facturevente);

            }
            else
            {
                //  foreach (SVC.Prodf selectedprodf in produitavendre)
                // {

                found.quantite = found.quantite + 1;
                found.Total = found.Total + (facturevente.Total);

                // }



            }
            ReceptDatagrid.ItemsSource = factureselectedl;
            CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

        }


        #endregion




        private void BtnCreerProduit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterProduit cl = new AjouterProduit(proxy, null, memberuser, callback);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chcodebare_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcodebare.IsChecked == true)
                {
                    txtbarrecode.Focus();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chcodebare_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcodebare.IsChecked == false)
                {
                    txtRecherche.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtbarrecode_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                txtbarrecode.Text = "";
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtbarrecode_LostFocus_1(object sender, RoutedEventArgs e)
        {
            try
            {

                ((TextBox)sender).Text = "123456789";

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        SVC.Prodf retournercodebarreprodffirst(string codeabarre)
        {

            var ll = proxy.GetAllTcabbycode(codeabarre).ToList();
            var test = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
            SVC.Prodf produit = new SVC.Prodf();
            if (ll.Count > 0)
            {
                foreach (var itemcode in ll)
                {

                    var tte = test.Any(n => n.cab == itemcode.cleproduit && n.quantite > 0);
                    if (tte == true)
                    {

                        var produitexiste = test.Where(n => n.cab == itemcode.cleproduit && n.quantite > 0);

                        foreach (var prodff in produitexiste)
                        {

                            bool selectedprodfexiste = produitavendre.Any(n => n.Id == prodff.Id);

                            if (prodff.perempt != null)
                            {

                                if (selectedprodfexiste)
                                {
                                    var selectedprodf = produitavendre.Find(n => n.Id == prodff.Id);

                                    if (prodff.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)) && selectedprodf.prixa > selectedprodf.quantite)
                                    {

                                        produit = prodff;
                                        break;
                                    }
                                }
                                else
                                {

                                    produit = prodff;
                                    break;

                                }
                            }
                            else
                            {
                                if (selectedprodfexiste)
                                {
                                    var selectedprodf = produitavendre.Find(n => n.Id == prodff.Id);
                                    if (selectedprodf.prixa > selectedprodf.quantite)
                                    {
                                        // if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))

                                        produit = prodff;
                                        break;
                                    }

                                }
                                else
                                {
                                    produit = prodff;
                                    break;
                                }
                            }



                        }
                    }
                }
            }


            return produit;
        }
        SVC.Produit retournercodebarreproduitfirst(string codeabarre)
        {

            var ll = proxy.GetAllTcabbycode(codeabarre).ToList();
            var test = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Produit>;
            SVC.Produit produit = new SVC.Produit();
            if (ll.Count > 0)
            {
                foreach (var itemcode in ll)
                {

                    var tte = test.Any(n => n.cab == itemcode.cleproduit);
                    if (tte == true)
                    {

                        var produitexiste = test.Where(n => n.cab == itemcode.cleproduit);

                        foreach (var prodff in produitexiste)
                        {



                            produit = prodff;
                            break;

                        }
                    }
                }
            }


            return produit;
        }

        private void txtbarrecode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        if (/*txtbarrecode.Text.Count() == txtbarrecode.MaxLength && */CompteComboBox.SelectedIndex >= 0)
                        {
                            if (facturenonstock != true)
                            {
                                SVC.Prodf selectedtropuvé = retournercodebarreprodffirst(txtbarrecode.Text);
                                var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                                SVC.Produit selectedproduit = new SVC.Produit();
                                if (existeproduit)
                                {
                                    selectedproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                }
                                if (selectedtropuvé != null && selectedtropuvé.Id != 0 && selectedtropuvé.quantite > 0)
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        date = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = selectedtropuvé.privente,
                                        quantite = 1,
                                        Total = selectedtropuvé.privente * 1,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    if (existeproduit)
                                    {
                                        facturevente.serialnumber = selectedproduit.marque;
                                    }
                                    else
                                    {
                                        facturevente.serialnumber = "";
                                    }
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {

                                        SVC.Prodf produitadd = new SVC.Prodf
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            cp = selectedtropuvé.cp,
                                            datef = selectedtropuvé.datef,
                                            dates = selectedtropuvé.dates,
                                            design = selectedtropuvé.design,
                                            famille = selectedtropuvé.famille,
                                            fourn = selectedtropuvé.fourn,
                                            IdFamille = selectedtropuvé.IdFamille,
                                            Id = selectedtropuvé.Id,
                                            lot = selectedtropuvé.lot,
                                            nfact = selectedtropuvé.nfact,
                                            perempt = selectedtropuvé.perempt,
                                            previent = selectedtropuvé.previent,
                                            privente = selectedtropuvé.privente,
                                            prixa = selectedtropuvé.quantite,
                                            prixb = selectedtropuvé.prixb,
                                            prixc = selectedtropuvé.prixc,
                                            quantite = 1,
                                            tva = selectedtropuvé.tva,
                                            collisage = selectedtropuvé.collisage,
                                        };
                                        produitadd.quantite = 1;
                                        produitavendre.Add(produitadd);
                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {
                                        SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedtropuvé.Id);

                                        if (selectedprodf.Id == found.ficheproduit && found.quantite < selectedtropuvé.quantite)
                                        {
                                            found.quantite = found.quantite + 1;
                                            found.Total = found.Total + (facturevente.Total);
                                            selectedprodf.quantite = selectedprodf.quantite + 1;

                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }




                                    }
                                    if (factureselectedl.Count > 0)
                                    {
                                        ReceptDatagrid.ItemsSource = factureselectedl;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();





                                        txtbarrecode.Text = "";
                                        txtbarrecode.Focus();
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        txtbarrecode.Text = "";
                                        txtbarrecode.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit n'existe pas quantité quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    txtbarrecode.Text = "";
                                    txtbarrecode.Focus();

                                }
                            }
                            else
                            {
                                SVC.Produit selectedtropuvé = retournercodebarreproduitfirst(txtbarrecode.Text);
                                if (selectedtropuvé != null && selectedtropuvé.Id != 0)
                                {
                                    if (selectedtropuvé.PrixRevient == null)
                                    {
                                        selectedtropuvé.PrixRevient = 0;
                                    }
                                    if (selectedtropuvé.PrixVente == null)
                                    {
                                        selectedtropuvé.PrixVente = 0;
                                    }
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        //   cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.Id,
                                        date = DateTime.Now,
                                        datef = DateTime.Now,
                                        design = selectedtropuvé.design,
                                        // lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        //   perempt = selectedtropuvé.perempt,
                                        tva = 0,
                                        previent = selectedtropuvé.PrixRevient,
                                        privente = selectedtropuvé.PrixVente,
                                        quantite = 1,
                                        Total = selectedtropuvé.PrixVente * 1,
                                        ficheproduit = selectedtropuvé.Id,
                                        //   ff = selectedtropuvé.nfact,
                                        //   Fournisseur = selectedtropuvé.fourn,
                                        collisage = 1,
                                        serialnumber = selectedtropuvé.marque,
                                    };

                                    //   factureselectedl.Add(facturevente);
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {

                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {
                                        found.quantite = found.quantite + 1;
                                        found.Total = found.Total + (facturevente.Total);
                                    }

                                    if (factureselectedl.Count > 0)
                                    {
                                        ReceptDatagrid.ItemsSource = factureselectedl;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();





                                        txtbarrecode.Text = "";
                                        txtbarrecode.Focus();
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        txtbarrecode.Text = "";
                                        txtbarrecode.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit n'existe pas quantité quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    txtbarrecode.Text = "";
                                    txtbarrecode.Focus();

                                }
                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NomenclatureProduit.SelectedItem != null)
                {
                    SVC.Prodf selectedprodf = NomenclatureProduit.SelectedItem as SVC.Prodf;
                    var produit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedprodf.cp)).First();
                    ImageProduit cl = new ImageProduit(proxy, produit, memberuser, callback);
                    cl.Show();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void txtnVersement_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Subtract:
                case Key.Decimal:
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
        private void txtDateOper_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void txteche_KeyDown(object sender, KeyEventArgs e)
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
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
        bool remplir()
        {
            if (txtDateOper.SelectedDate != null)
            {





                return true;
            }
            else
            {
                return false;
            }
        }

        private void NomenclatureProduit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (facturenonstock == false)
                {

                    if (chcout.IsChecked == false)
                    {
                        if (NomenclatureProduit.SelectedItem != null && CompteComboBox.SelectedIndex >= 0 && remplir() && factureopern == true)
                        {
                            //    this.Title = title;
                            //  this.WindowTitleBrush = brushajouterfacture;

                            selectedtropuvé = NomenclatureProduit.SelectedItem as SVC.Prodf;
                            var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                            SVC.Produit selectedproduit = new SVC.Produit();
                            if (existeproduit)
                            {
                                selectedproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                // facturevente.serialnumber = selectedproduit.marque;
                            }
                            if (selectedtropuvé.quantite > 0)
                            {
                                if (selectedtropuvé.perempt != null)
                                {
                                    if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                    {

                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé.previent,
                                            privente = selectedtropuvé.privente,
                                            quantite = 0,
                                            Total = selectedtropuvé.privente * 0,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,
                                            collisage = selectedtropuvé.collisage,

                                        };
                                        //   var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                                        if (existeproduit)
                                        {
                                            //    SVC.Produit selectedproduit= proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                            facturevente.serialnumber = selectedproduit.marque;
                                        }
                                        else
                                        {
                                            facturevente.serialnumber = "";
                                        }

                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {


                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {


                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);






                                        }
                                        ReceptDatagrid.ItemsSource = factureselectedl;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    }
                                }
                                else
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        dates = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = selectedtropuvé.privente,
                                        quantite = 0,
                                        Total = selectedtropuvé.privente * 0,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        codeclient = Clientvv.Id,
                                        Client = Clientvv.Raison,
                                        collisage = selectedtropuvé.collisage,

                                        /*****************************************/

                                    };
                                    if (existeproduit)
                                    {
                                        //    SVC.Produit selectedproduit= proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                        facturevente.serialnumber = selectedproduit.marque;
                                    }
                                    else
                                    {
                                        facturevente.serialnumber = "";
                                    }
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {

                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {

                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                    }
                                    ReceptDatagrid.ItemsSource = factureselectedl;
                                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                                }
                            }



                            else
                            {
                                if (selectedtropuvé.quantite == 0 && chBonLivraisonAvoir.IsChecked == true)
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        dates = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = memberuser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = selectedtropuvé.privente,
                                        quantite = 0,
                                        Total = selectedtropuvé.privente * -1,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        codeclient = Clientvv.Id,
                                        Client = Clientvv.Raison,
                                        collisage = selectedtropuvé.collisage,

                                    };
                                    if (existeproduit)
                                    {
                                        //    SVC.Produit selectedproduit= proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                        facturevente.serialnumber = selectedproduit.marque;
                                    }
                                    else
                                    {
                                        facturevente.serialnumber = "";
                                    }
                                    var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {
                                        factureselectedl.Add(facturevente);

                                    }
                                    else
                                    {

                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                    }
                                    ReceptDatagrid.ItemsSource = factureselectedl;
                                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();


                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                }
                            }

                        }
                    }
                    else
                    {
                        if (chcout.IsChecked == true)
                        {

                            if (NomenclatureProduit.SelectedItem != null && CompteComboBox.SelectedIndex >= 0)
                            {
                                var selectedtropuvé1 = NomenclatureProduit.SelectedItem as SVC.Prodf;
                                selectedtropuvé = proxy.GetAllProdfbycode(Convert.ToInt32(selectedtropuvé1.cp)).Where(n => n.quantite != 0).First();
                                var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                                SVC.Produit selectedproduit = new SVC.Produit();
                                if (existeproduit)
                                {
                                    selectedproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                    // facturevente.serialnumber = selectedproduit.marque;
                                }
                                if (selectedtropuvé.quantite > 0)
                                {
                                    if (selectedtropuvé.perempt != null)
                                    {
                                        if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                        {
                                            SVC.Facture facturevente = new SVC.Facture
                                            {
                                                cab = selectedtropuvé.cab,
                                                cf = selectedtropuvé.cf,
                                                codeprod = selectedtropuvé.cp,
                                                dates = DateTime.Now,
                                                datef = selectedtropuvé.datef,
                                                design = selectedtropuvé.design,
                                                lot = selectedtropuvé.lot,
                                                oper = memberuser.Username,
                                                perempt = selectedtropuvé.perempt,
                                                tva = selectedtropuvé.tva,
                                                previent = selectedtropuvé1.previent,
                                                privente = selectedtropuvé1.privente,
                                                quantite = 0,
                                                Total = selectedtropuvé1.privente * 0,
                                                ficheproduit = selectedtropuvé.Id,
                                                ff = selectedtropuvé.nfact,
                                                Fournisseur = selectedtropuvé.fourn,
                                                codeclient = Clientvv.Id,
                                                Client = Clientvv.Raison,
                                                collisage = selectedtropuvé.collisage,

                                            };
                                            if (existeproduit)
                                            {
                                                //    SVC.Produit selectedproduit= proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                                facturevente.serialnumber = selectedproduit.marque;
                                            }
                                            else
                                            {
                                                facturevente.serialnumber = "";
                                            }
                                            var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                            if (found == null)
                                            {


                                                factureselectedl.Add(facturevente);

                                            }
                                            else
                                            {


                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);






                                            }
                                            ReceptDatagrid.ItemsSource = factureselectedl;
                                            CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                    else
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé1.previent,
                                            privente = selectedtropuvé1.privente,
                                            quantite = 0,
                                            Total = selectedtropuvé1.privente * 0,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,
                                            collisage = selectedtropuvé.collisage,


                                        };
                                        if (existeproduit)
                                        {
                                            //    SVC.Produit selectedproduit= proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                            facturevente.serialnumber = selectedproduit.marque;
                                        }
                                        else
                                        {
                                            facturevente.serialnumber = "";
                                        }
                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {

                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {

                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }
                                        ReceptDatagrid.ItemsSource = factureselectedl;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();

                                    }
                                }
                                else
                                {
                                    if (selectedtropuvé.quantite == 0 && chBonLivraisonAvoir.IsChecked == true)
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            dates = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = memberuser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé1.previent,
                                            privente = selectedtropuvé1.privente,
                                            quantite = 0,
                                            Total = selectedtropuvé1.privente * -1,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            codeclient = Clientvv.Id,
                                            Client = Clientvv.Raison,
                                            collisage = selectedtropuvé.collisage,

                                        };
                                        if (existeproduit)
                                        {
                                            //    SVC.Produit selectedproduit= proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                            facturevente.serialnumber = selectedproduit.marque;
                                        }
                                        else
                                        {
                                            facturevente.serialnumber = "";
                                        }
                                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {
                                            factureselectedl.Add(facturevente);

                                        }
                                        else
                                        {

                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }
                                        ReceptDatagrid.ItemsSource = factureselectedl;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();


                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                    }
                                }

                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un tarif", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                }
                else
                {
                    if (NomenclatureProduit.SelectedItem != null && CompteComboBox.SelectedIndex >= 0 && remplir() && factureopern == true)
                    {
                        //    this.Title = title;
                        //  this.WindowTitleBrush = brushajouterfacture;

                        selectedtropuvéNONSTOCK = NomenclatureProduit.SelectedItem as SVC.Produit;
                        // if (selectedtropuvé.quantite > 0)
                        //{
                        if (selectedtropuvéNONSTOCK.PrixVente == null)
                        {
                            selectedtropuvéNONSTOCK.PrixVente = 0;
                        }
                        if (selectedtropuvéNONSTOCK.PrixRevient == null)
                        {
                            selectedtropuvéNONSTOCK.PrixRevient = 0;
                        }

                        SVC.Facture facturevente = new SVC.Facture
                        {
                            cab = selectedtropuvéNONSTOCK.cab,
                            //  cf = 0,
                            codeprod = selectedtropuvéNONSTOCK.Id,
                            dates = DateTime.Now,
                            datef = DateTime.Now,
                            design = selectedtropuvéNONSTOCK.design,
                            // lot = 0,
                            oper = memberuser.Username,
                            // perempt = selectedtropuvéNONSTOCK.perempt,
                            tva = 0,
                            previent = selectedtropuvéNONSTOCK.PrixRevient,
                            privente = selectedtropuvéNONSTOCK.PrixVente,
                            quantite = 0,
                            Total = selectedtropuvéNONSTOCK.PrixVente * 0,
                            ficheproduit = selectedtropuvéNONSTOCK.Id,
                            // ff = selectedtropuvéNONSTOCK.nfact,
                            //  Fournisseur = selectedtropuvéNONSTOCK.fourn,
                            codeclient = Clientvv.Id,
                            Client = Clientvv.Raison,
                            collisage = 1,
                            Auto = true,
                            serialnumber = selectedtropuvéNONSTOCK.marque,
                            /*****************************************/

                        };
                        var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvéNONSTOCK.Id);
                        if (found == null)
                        {

                            factureselectedl.Add(facturevente);

                        }
                        else
                        {

                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                        }
                        ReceptDatagrid.ItemsSource = factureselectedl;
                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();


                        // }



                        /* else
                          {
                              if (selectedtropuvé.quantite == 0 && chBonLivraisonAvoir.IsChecked == true)
                              {
                                  SVC.Facture facturevente = new SVC.Facture
                                  {
                                      cab = selectedtropuvéNONSTOCK.cab,
                                    //  cf = selectedtropuvéNONSTOCK.cf,
                                      codeprod = selectedtropuvéNONSTOCK.Id,
                                      dates = DateTime.Now,
                                      datef = DateTime.Now,
                                      design = selectedtropuvéNONSTOCK.design,
                                    //  lot = selectedtropuvéNONSTOCK.lot,
                                      oper = memberuser.Username,
                                   //   perempt = selectedtropuvéNONSTOCK.perempt,
                                    //  tva = selectedtropuvéNONSTOCK.tva,
                                      previent = selectedtropuvéNONSTOCK.PrixRevient,
                                      privente = selectedtropuvéNONSTOCK.PrixVente,
                                      quantite = 0,
                                      Total = selectedtropuvéNONSTOCK.PrixVente * -1,
                                      ficheproduit = selectedtropuvéNONSTOCK.Id,
                                  //    ff = selectedtropuvéNONSTOCK.nfact,
                                   //   Fournisseur = selectedtropuvéNONSTOCK.fourn,
                                      codeclient = Clientvv.Id,
                                      Client = Clientvv.Raison,
                                     // collisage = selectedtropuvéNONSTOCK.collisage,
                                     Auto=true,
                                  };
                                  var found = factureselectedl.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                  if (found == null)
                                  {
                                      factureselectedl.Add(facturevente);

                                  }
                                  else
                                  {

                                      MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est déja dans la facture", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                  }
                                  ReceptDatagrid.ItemsSource = factureselectedl;
                                  CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();


                              }
                              else
                              {
                                  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                              }
                          }*/

                    }
                }


            }
            catch (Exception EX)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(EX.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void NomenclatureProduit_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (facturenonstock != true)
            {
                NomenclatureProduit.ItemsSource = proxy.GetAllProdf().OrderBy(n => n.design);
            }
            else
            {
                NomenclatureProduit.ItemsSource = proxy.GetAllProduit().OrderBy(n => n.design);
            }
        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (facturenonstock != true)
                {
                    TextBox t = (TextBox)sender;
                    string filter = t.Text;
                    ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                    if (filter == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            SVC.Prodf p = o as SVC.Prodf;
                            if (t.Name == "txtId")
                                return (p.Id == Convert.ToInt32(filter));
                            return (p.design.ToUpper().Contains(filter.ToUpper()));
                        };
                    }
                    /*  string filterValue = txtRecherche.Text;
                      if (!String.IsNullOrEmpty(filterValue))
                          NomenclatureProduit.Columns[1].AutoFilterValue = filterValue;
                      else NomenclatureProduit.FilterString = "([Id]  >= 0)";*/
                }
                else
                {
                    TextBox t = (TextBox)sender;
                    string filter = t.Text;
                    ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
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
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void FournisseurCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (facturenonstock != true)
                {
                    ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);

                    cv00.Filter = delegate (object item)
                    {
                        SVC.Prodf temp = item as SVC.Prodf;
                        return temp.IdFamille != -1;


                    };

                    FamilleCombo.SelectedIndex = -1;
                }
                else
                {
                    ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);

                    cv00.Filter = delegate (object item)
                    {
                        SVC.Produit temp = item as SVC.Produit;
                        return temp.IdFamille != -1;


                    };

                    FamilleCombo.SelectedIndex = -1;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void FamilleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FamilleCombo.SelectedItem != null)
                {
                    if (facturenonstock != true)
                    {
                        SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                        var filter = (t.Id).ToString();

                        ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                        if (filter == "")
                            cv.Filter = null;
                        else
                        {
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Id.ToString() == "txtId")
                                    return ((p.IdFamille).ToString() == filter);
                                return (p.IdFamille.ToString().ToUpper().Contains(filter.ToUpper()));
                            };

                        }
                    }
                    else
                    {
                        SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                        var filter = (t.Id).ToString();

                        ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                        if (filter == "")
                            cv.Filter = null;
                        else
                        {
                            cv.Filter = o =>
                            {
                                SVC.Produit p = o as SVC.Produit;
                                if (t.Id.ToString() == "txtId")
                                    return ((p.IdFamille).ToString() == filter);
                                return (p.IdFamille.ToString().ToUpper().Contains(filter.ToUpper()));
                            };

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void chFactureNew_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                documenttype = 1;
                fermer = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void chFactureAvoirNew_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                documenttype = 2;
                fermer = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void chBonLivraisonNew_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                documenttype = 3;
                fermer = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void chBonLivraisonAvoirNew_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                documenttype = 4;
                fermer = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void chProformaNew_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                documenttype = 5;
                fermer = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void chFactureProvisoirNew_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                documenttype = 6;
                fermer = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void chfacturation_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                facturenonstock = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void chfacturation_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                facturenonstock = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chfacturationmonture_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                facturenonstock = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chfacturationmonture_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                facturenonstock = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void facturea4_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionfacture = 1;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void facturea5_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionfacture = 2;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chimpression_Checked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                visualiserFacture = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }
        private void ConfirmerDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationDossierClient == true)
                {
                    if (facturenonstock == false)
                    {
                        if (fermer == true)
                        {
                            txtnVersement.IsEnabled = true;
                            FacturationDesign();
                            factureselectedl = new List<SVC.Facture>();
                            ReceptDatagrid.DataContext = factureselectedl;
                            ReceptDatagrid.ItemsSource = factureselectedl;
                            ReceptDatagrid.IsEnabled = true;
                            txtTTC.Text = "0";
                            txtnfact.IsEnabled = false;


                            if (documenttype == 3)
                            {
                                NomDocumentLabel.Content = "bon de livraison";
                                chBonLivraison.IsChecked = true;
                                txtnVersement.IsEnabled = true;
                                selectedparam = proxy.GetAllParamétre();

                                if (selectedparam.AffichPrixAchatVente == true)
                                {
                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                }
                                if (selectedparam.ModiPrix == true)
                                {
                                    ReceptDatagrid.Columns[2].IsReadOnly = false;
                                }
                                else
                                {
                                    ReceptDatagrid.Columns[2].IsReadOnly = true;
                                }
                                if (selectedparam.modidate == true)
                                {
                                    txtDateOper.IsEnabled = true;
                                }
                                if (selectedparam.affiben == true)
                                {
                                    Bénéfice.Visibility = Visibility.Visible;
                                    Bénéficemont.Visibility = Visibility.Visible;

                                }
                            }
                            else
                            {
                                if (documenttype == 4)
                                {
                                    NomDocumentLabel.Content = "avoir bon de livraison";
                                    chBonLivraisonAvoir.IsChecked = true;
                                    txtnVersement.IsEnabled = true;
                                    selectedparam = proxy.GetAllParamétre();

                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                }
                                else
                                {
                                    if (documenttype == 1)
                                    {
                                        NomDocumentLabel.Content = "Nouvelle Facture";
                                        chFacture.IsChecked = true;
                                        txtnVersement.IsEnabled = true;
                                        selectedparam = proxy.GetAllParamétre();

                                        if (selectedparam.AffichPrixAchatVente == true)
                                        {
                                            NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                        }
                                        else
                                        {
                                            NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                        }
                                        if (selectedparam.ModiPrix == true)
                                        {
                                            ReceptDatagrid.Columns[2].IsReadOnly = false;
                                        }
                                        else
                                        {
                                            ReceptDatagrid.Columns[2].IsReadOnly = true;
                                        }
                                        if (selectedparam.modidate == true)
                                        {
                                            txtDateOper.IsEnabled = true;
                                        }
                                        if (selectedparam.affiben == true)
                                        {
                                            Bénéfice.Visibility = Visibility.Visible;
                                            Bénéficemont.Visibility = Visibility.Visible;
                                            //   Bénéficemont.Text = Convert.ToString(((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                        }
                                    }
                                    else
                                    {
                                        if (documenttype == 2)
                                        {
                                            NomDocumentLabel.Content = "Facture d'avoir";
                                            chFactureAvoir.IsChecked = true;
                                            txtnVersement.IsEnabled = true;
                                            selectedparam = proxy.GetAllParamétre();

                                            if (selectedparam.AffichPrixAchatVente == true)
                                            {
                                                NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                            }
                                            else
                                            {
                                                NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                            }
                                            if (selectedparam.ModiPrix == true)
                                            {
                                                ReceptDatagrid.Columns[2].IsReadOnly = false;
                                            }
                                            else
                                            {
                                                ReceptDatagrid.Columns[2].IsReadOnly = true;
                                            }
                                            if (selectedparam.modidate == true)
                                            {
                                                txtDateOper.IsEnabled = true;
                                            }
                                            if (selectedparam.affiben == true)
                                            {
                                                Bénéfice.Visibility = Visibility.Visible;
                                                Bénéficemont.Visibility = Visibility.Visible;

                                            }
                                        }
                                        else
                                        {
                                            if (documenttype == 6)
                                            {
                                                NomDocumentLabel.Content = "Facture provisoir";
                                                chFactureProvisoir.IsChecked = true;
                                                txtnVersement.IsEnabled = true;



                                                ReceptDatagrid.Columns[2].IsReadOnly = false;


                                                txtDateOper.IsEnabled = true;


                                            }
                                            else
                                            {
                                                if (documenttype == 5)
                                                {
                                                    NomDocumentLabel.Content = "Nouvelle Proforma";
                                                    chProforma.IsChecked = true;
                                                    txtnVersement.IsEnabled = false;
                                                    ReceptDatagrid.Columns[2].IsReadOnly = false;


                                                    txtDateOper.IsEnabled = true;
                                                }
                                            }

                                        }
                                    }
                                }
                            }



                            CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                            nouveauF1 = new SVC.F1
                            {
                                codeclient = Clientvv.Id,
                                date = DateTime.Now,
                                dates = DateTime.Now,
                                raison = Clientvv.Raison,
                                timbre = 0,
                                echeance = 0,
                                ht = 0,
                                net = 0,
                                oper = memberuser.Username,
                                tva = 0,
                                versement = 0,
                                reste = 0,
                                modep = "A TERME",
                                remise = 0,

                            };
                            WindowBorderFacture.DataContext = nouveauF1;
                            dialog1.Close();
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un document de vente", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                    else
                    {
                        if (facturenonstock == true)
                        {
                            if (fermer == true)
                            {
                                txtnVersement.IsEnabled = true;
                                FacturationDesignNonStock();
                                factureselectedl = new List<SVC.Facture>();
                                ReceptDatagrid.DataContext = factureselectedl;
                                ReceptDatagrid.ItemsSource = factureselectedl;
                                ReceptDatagrid.IsEnabled = true;
                                txtTTC.Text = "0";
                                txtnfact.IsEnabled = false;


                                if (documenttype == 3)
                                {
                                    NomDocumentLabel.Content = "bon de livraison";
                                    chBonLivraison.IsChecked = true;
                                    txtnVersement.IsEnabled = true;
                                    selectedparam = proxy.GetAllParamétre();

                                    if (selectedparam.AffichPrixAchatVente == true)
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                    }
                                    if (selectedparam.ModiPrix == true)
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ReceptDatagrid.Columns[2].IsReadOnly = true;
                                    }
                                    if (selectedparam.modidate == true)
                                    {
                                        txtDateOper.IsEnabled = true;
                                    }
                                    if (selectedparam.affiben == true)
                                    {
                                        Bénéfice.Visibility = Visibility.Visible;
                                        Bénéficemont.Visibility = Visibility.Visible;

                                    }
                                }
                                else
                                {
                                    if (documenttype == 4)
                                    {
                                        NomDocumentLabel.Content = "avoir bon de livraison";
                                        chBonLivraisonAvoir.IsChecked = true;
                                        txtnVersement.IsEnabled = true;
                                        selectedparam = proxy.GetAllParamétre();

                                        if (selectedparam.AffichPrixAchatVente == true)
                                        {
                                            NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                        }
                                        else
                                        {
                                            NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                        }
                                        if (selectedparam.ModiPrix == true)
                                        {
                                            ReceptDatagrid.Columns[2].IsReadOnly = false;
                                        }
                                        else
                                        {
                                            ReceptDatagrid.Columns[2].IsReadOnly = true;
                                        }
                                        if (selectedparam.modidate == true)
                                        {
                                            txtDateOper.IsEnabled = true;
                                        }
                                        if (selectedparam.affiben == true)
                                        {
                                            Bénéfice.Visibility = Visibility.Visible;
                                            Bénéficemont.Visibility = Visibility.Visible;

                                        }
                                    }
                                    else
                                    {
                                        if (documenttype == 1)
                                        {
                                            NomDocumentLabel.Content = "Nouvelle Facture";
                                            chFacture.IsChecked = true;
                                            txtnVersement.IsEnabled = true;
                                            selectedparam = proxy.GetAllParamétre();

                                            if (selectedparam.AffichPrixAchatVente == true)
                                            {
                                                NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                            }
                                            else
                                            {
                                                NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                            }
                                            if (selectedparam.ModiPrix == true)
                                            {
                                                ReceptDatagrid.Columns[2].IsReadOnly = false;
                                            }
                                            else
                                            {
                                                ReceptDatagrid.Columns[2].IsReadOnly = true;
                                            }
                                            if (selectedparam.modidate == true)
                                            {
                                                txtDateOper.IsEnabled = true;
                                            }
                                            if (selectedparam.affiben == true)
                                            {
                                                Bénéfice.Visibility = Visibility.Visible;
                                                Bénéficemont.Visibility = Visibility.Visible;
                                                //   Bénéficemont.Text = Convert.ToString(((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)));

                                            }
                                        }
                                        else
                                        {
                                            if (documenttype == 2)
                                            {
                                                NomDocumentLabel.Content = "Facture d'avoir";
                                                chFactureAvoir.IsChecked = true;
                                                txtnVersement.IsEnabled = true;
                                                selectedparam = proxy.GetAllParamétre();

                                                if (selectedparam.AffichPrixAchatVente == true)
                                                {
                                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Visible;
                                                }
                                                else
                                                {
                                                    NomenclatureProduit.Columns[4].Visibility = Visibility.Collapsed;
                                                }
                                                if (selectedparam.ModiPrix == true)
                                                {
                                                    ReceptDatagrid.Columns[2].IsReadOnly = false;
                                                }
                                                else
                                                {
                                                    ReceptDatagrid.Columns[2].IsReadOnly = true;
                                                }
                                                if (selectedparam.modidate == true)
                                                {
                                                    txtDateOper.IsEnabled = true;
                                                }
                                                if (selectedparam.affiben == true)
                                                {
                                                    Bénéfice.Visibility = Visibility.Visible;
                                                    Bénéficemont.Visibility = Visibility.Visible;

                                                }
                                            }
                                            else
                                            {
                                                if (documenttype == 6)
                                                {
                                                    NomDocumentLabel.Content = "Facture provisoir";
                                                    chFactureProvisoir.IsChecked = true;
                                                    txtnVersement.IsEnabled = true;



                                                    ReceptDatagrid.Columns[2].IsReadOnly = false;


                                                    txtDateOper.IsEnabled = true;


                                                }
                                                else
                                                {
                                                    if (documenttype == 5)
                                                    {
                                                        NomDocumentLabel.Content = "Nouvelle Proforma";
                                                        chProforma.IsChecked = true;
                                                        txtnVersement.IsEnabled = false;
                                                        ReceptDatagrid.Columns[2].IsReadOnly = false;


                                                        txtDateOper.IsEnabled = true;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }



                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                nouveauF1 = new SVC.F1
                                {
                                    codeclient = Clientvv.Id,
                                    date = DateTime.Now,
                                    dates = DateTime.Now,
                                    raison = Clientvv.Raison,
                                    timbre = 0,
                                    echeance = 0,
                                    ht = 0,
                                    net = 0,
                                    oper = memberuser.Username,
                                    tva = 0,
                                    versement = 0,
                                    reste = 0,
                                    modep = "A TERME",
                                    remise = 0,
                                    Auto = true,
                                };
                                WindowBorderFacture.DataContext = nouveauF1;
                                dialog1.Close();
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

        private void chimpression_Unchecked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                visualiserFacture = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }


        private void LentilleDatagrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                LentilleDatagrid.ItemsSource = proxy.GetAllLentilleClientbycode(Clientvv.Id).OrderBy(n => n.Date);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void CompteComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (facturenonstock != true)
                {
                    string ValueCompte = "";
                    if (CompteComboBox.SelectedIndex >= 0)
                    {

                        var test = NomenclatureProduit.ItemsSource as IEnumerable;



                        ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                        switch (ValueCompte)
                        {
                            case "Prixa":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixa;
                                }

                                break;
                            case "Prixb":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixb;
                                }
                                break;

                            case "Prixc":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixc;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void CompteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /* if (CompteComboBox.SelectedIndex >= 0)
                 {
                     lhdfhabel.Text = ((ComboBoxEditItem)CompteComboBox.SelectedItem).Content.ToString();
                     lhdfdfhabel.Foreground = System.Windows.Media.Brushes.Green;
                 }
                 else
                 {
                     lhdfhabel.Text = "Vous devez choisir un tarif";
                     lhdfdfhabel.Foreground = System.Windows.Media.Brushes.Red;
                 }*/

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
