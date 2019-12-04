using DevExpress.Xpf.Core;
using Microsoft.Reporting.WinForms;
using NewOptics.Administrateur;
using NewOptics.Caisse;
using NewOptics.Stock;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MontureClient.xaml
    /// </summary>
    public partial class MontureClient : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;
        SVC.ClientV Clientvv;
        private delegate void FaultedInvokerMonture();
        SVC.Monture MontureClass;
        bool nouvellemonture = false;
        bool anciennemonture = false;
        bool montureversementzero = false;
        List<SVC.MontureSupplement> listsupp1;
        List<SVC.MontureSupplement> listsupp2;
        List<SVC.MontureSupplement> listsupp3;
        List<SVC.MontureSupplement> listsupp4;
        List<SVC.MontureSupplement> anciennelistsupp1;
        List<SVC.MontureSupplement> anciennelistsupp2;
        List<SVC.MontureSupplement> anciennelistsupp3;
        List<SVC.MontureSupplement> anciennelistsupp4;
        int interfaceimpressionmonture = 0;
        bool visualiser = false;
        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        DXDialog dialog1;
        SVC.Param selectedparam;
        public MontureClient(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu, SVC.ClientV clientrecu,SVC.Monture MONTURERECU)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;

                callback = callbackrecu;
                memberuser = memberrecu;
                Clientvv = clientrecu;
                selectedparam=proxy.GetAllParamétre();
                /******************************Monture***************************************/
                this.Title = "Dossier Monture : " + Clientvv.Raison;
                MontureDatagrid.ItemsSource = proxy.GetAllMonturebycode(Clientvv.Id).OrderBy(n => n.Date);
                MontureClass = MONTURERECU;
                montureexiste(MontureClass);
                MontureDatagrid.SelectedItem = MONTURERECU;
                /**/
                callbackrecu.InsertMontureCallbackevent += new ICallback.CallbackEventHandler34(callbackrecumonture_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void callbackrecumonture_Refresh(object source, CallbackEventInsertMonture e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshMonture(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshMonture(SVC.Monture listmembership, int oper)
        {
            try
            {

                var LISTITEM1 = MontureDatagrid.ItemsSource as IEnumerable<SVC.Monture>;
                List<SVC.Monture> LISTITEM = LISTITEM1.ToList();

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

                MontureDatagrid.ItemsSource = LISTITEM;


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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMonture(HandleProxy));
                return;
            }
            HandleProxy();
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

        private void RunAtelierDouble(List<SVC.Monture> monture, List<SVC.ClientV> CLIENRECU)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.FicheAtelierMontureDouble), false);
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

        private void RunAtelier(List<SVC.Monture> monture, List<SVC.ClientV> CLIENRECU)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.FicheAtelierMonture), false);
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

        private void RunMonturePaiement(List<SVC.Monture> monture, List<SVC.ClientV> CLIENRECU, List<SVC.F1> listevisite, List<SVC.Depeiment> listerecu)
        {
            try
            {

                LocalReport reportViewer1 = new LocalReport();


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.ImpressionFicheAtelierMontureRecu), false);
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

        private void chimpression_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                visualiser = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void chimpression_Unchecked(object sender, RoutedEventArgs e)
        {

            try
            {
                visualiser = false;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMonture(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationDossierClient == true)
                {
                    MontureClass = new SVC.Monture
                    {
                        Date = DateTime.Now,
                        UserName = memberuser.Username,
                        Délivre = false,
                        Monte = false,
                        RaisonClient = Clientvv.Raison,
                        StatutDevis = true,
                        StatutVente = false,
                        IdClient = Clientvv.Id,
                        EnMontage = false,
                        DroiteCylindrePlus = true,
                        DroiteCylindreMoin = false,
                        GaucheCylindreMoin = false,
                        GaucheCylindrePlus = true,
                        AccessoiresQuantite1 = 0,
                        AccessoiresQuantite2 = 0,
                        AccessoiresPrix1 = 0,
                        AccessoiresPrix2 = 0,
                        GaucheFleshBas = false,
                        GaucheFleshHaut = true,
                        GaucheFleshDroite = true,
                        GaucheFleshGauche = false,
                        DroiteFleshBas = false,
                        DroiteFleshHaut = true,
                        DroiteFleshDroite = true,
                        DroiteFleshGauche = false,
                        DroitPrixVerreLoin = 0,
                        Encaissé = 0,
                        Reste = 0,
                        Interm = true,
                        Loin = true,
                        Pres = true,
                        Dates = DateTime.Now,
                        DroitPrixVerrePres = 0,

                        GauchePrixVerreLoin = 0,
                        GauchePrixVerrePres = 0,
                        PrixMontureLoin = 0,
                        PrixMonturePres = 0,
                        Remise = 0,
                    };
                    GridMonture.DataContext = MontureClass;
                    GridMonture.IsEnabled = true;
                    TxtStatutGlobal.Content = "Devis";
                    TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;

                    nouvellemonture = true;
                    anciennemonture = false;
                    txtDroiteFleshHaut.Visibility = Visibility.Visible;
                    DroiteFleshBas.Visibility = Visibility.Collapsed;
                    txtDroiteFleshDroite.Visibility = Visibility.Visible;
                    txtDroiteFleshGauche.Visibility = Visibility.Collapsed;

                    txtGaucheFleshHaut.Visibility = Visibility.Visible;
                    txtGaucheFleshBas.Visibility = Visibility.Collapsed;
                    txtGaucheFleshDroite.Visibility = Visibility.Visible;
                    txtGaucheFleshGauche.Visibility = Visibility.Collapsed;

                    listsupp1 = new List<SVC.MontureSupplement>();
                    listsupp2 = new List<SVC.MontureSupplement>();
                    listsupp3 = new List<SVC.MontureSupplement>();
                    listsupp4 = new List<SVC.MontureSupplement>();
                    txtMontantTotalENC.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }


        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.SupressionDossierClient == true && MontureDatagrid.SelectedItem != null)
            {
                var selectedmonture = MontureDatagrid.SelectedItem as SVC.Monture;
                if (selectedmonture.StatutVente == false)
                {
                    bool sucees = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {

                        proxy.DeleteMonture(selectedmonture);
                        ts.Complete();
                        sucees = true;
                    }
                    if (sucees == true)
                    {
                        proxy.AjouterMontureRefresh(Clientvv.Id);
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                }
                else
                {
                    if (selectedmonture.StatutVente == true)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez tout d'abord supprimer le document de vente associé", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MontureDatagrid.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {
                    MontureClass = MontureDatagrid.SelectedItem as SVC.Monture;
                    GridMonture.DataContext = MontureClass;
                    GridMonture.IsEnabled = true;
                    nouvellemonture = false;
                    anciennemonture = true;
                    if (MontureClass.StatutVente == false)
                    {
                         txtMontantTotalENC.IsEnabled = true;
                    }
                    else
                    {
                         txtMontantTotalENC.IsEnabled = false;
                    }

                    if (MontureClass.StatutDevis == true && MontureClass.StatutVente == false)
                    {
                        TxtStatutGlobal.Content = "Devis";

                        TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (MontureClass.StatutDevis == true && MontureClass.StatutVente == true)
                        {
                            TxtStatutGlobal.Content = "Vente validée";
                            TxtStatutGlobal.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }


                    if (MontureClass.DroiteFleshHaut == true)
                    {
                        txtDroiteFleshHaut.Visibility = Visibility.Visible;
                        DroiteFleshBas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtDroiteFleshHaut.Visibility = Visibility.Collapsed;
                        DroiteFleshBas.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.DroiteFleshDroite == true)
                    {
                        txtDroiteFleshDroite.Visibility = Visibility.Visible;
                        txtDroiteFleshGauche.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtDroiteFleshDroite.Visibility = Visibility.Collapsed;
                        txtDroiteFleshGauche.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.GaucheFleshHaut == true)
                    {
                        txtGaucheFleshHaut.Visibility = Visibility.Visible;
                        txtGaucheFleshBas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtGaucheFleshHaut.Visibility = Visibility.Collapsed;
                        txtGaucheFleshBas.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.GaucheFleshDroite == true)
                    {
                        txtGaucheFleshDroite.Visibility = Visibility.Visible;
                        txtGaucheFleshGauche.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtGaucheFleshDroite.Visibility = Visibility.Collapsed;
                        txtGaucheFleshGauche.Visibility = Visibility.Visible;
                    }
                    /**************************************************************/
                    if (MontureClass.Loin == true && MontureClass.Pres == true)
                    {

                        ODLOIN.Text = "Loin";
                        ODITERM.Text = "Interm.";
                        ODPRES.Text = "Près";
                        OGLOIN.Text = "Loin";
                        OGINTERM.Text = "Interm.";
                        OGPRES.Text = "Près";
                        MonturePresLabel.Text = "PRES";
                        MontureLoinLabel.Text = "LOIN";


                    }
                    else
                    {
                        if (MontureClass.Loin == true && MontureClass.Pres == false)
                        {


                            ODLOIN.Text = "Loin";
                            ODITERM.Text = "Loin";
                            ODPRES.Text = "Près";
                            OGLOIN.Text = "Loin";
                            OGINTERM.Text = "Loin";
                            OGPRES.Text = "Près";
                            MonturePresLabel.Text = "LOIN";
                            MontureLoinLabel.Text = "LOIN";


                        }
                        else
                        {
                            if (MontureClass.Loin == false && MontureClass.Pres == true)
                            {
                                ODLOIN.Text = "Loin";
                                ODITERM.Text = "Près";
                                ODPRES.Text = "Près";
                                OGLOIN.Text = "Loin";
                                OGINTERM.Text = "Près";
                                OGPRES.Text = "Près";
                                MonturePresLabel.Text = "PRES";
                                MontureLoinLabel.Text = "PRES";
                            }
                        }
                    }
                    if (MontureClass.Encaissé != 0)
                    {
                        montureversementzero = true;
                    }
                    else
                    {
                        montureversementzero = false;
                    }
                    if (MontureClass.StatutDevis == true && MontureClass.StatutVente == false)
                    {
                        TxtStatutGlobal.Content = "Devis";

                        TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (MontureClass.StatutDevis == true && MontureClass.StatutVente == true)
                        {
                            TxtStatutGlobal.Content = "Vente validée";
                            TxtStatutGlobal.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }

                }
                listsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                listsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList();
                listsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList();
                listsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();
                anciennelistsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                anciennelistsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList();
                anciennelistsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList();
                anciennelistsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();
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
                try
                {
                    dialog1 = new DXDialog("Impression", DialogButtons.YesNo, true);
                    dialog1.Template = Resources["template5"] as ControlTemplate;

                    // dialog1.Content = Resources["content"];
                    dialog1.ResizeMode = ResizeMode.NoResize;
                    dialog1.Width = 350;
                    dialog1.Height = 200;
                    dialog1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog1.ShowDialog();
                    interfaceimpressionmonture = 0;
                }
                catch (Exception ex)
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionmonture = 1;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionmonture = 2;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionmonture = 3;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            try
            {
                interfaceimpressionmonture = 4;

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                <PageWidth>11.75in</PageWidth>
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

        private void btnCreerImpression_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (visualiser == true)
                {
                    switch (interfaceimpressionmonture)
                    {
                        case 1:
                            if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.Monture selectedmont = MontureDatagrid.SelectedItem as SVC.Monture;
                                List<SVC.Monture> mm = new List<SVC.Monture>();
                                mm.Add(selectedmont);
                                List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                clientvvv.Add(Clientvv);
                                ImpressionFicheAtelier cl = new ImpressionFicheAtelier(proxy, mm, clientvvv, interfaceimpressionmonture);
                                cl.Show();
                                dialog1.Close();
                                interfaceimpressionmonture = 0;
                                visualiser = false;
                            }
                            break;
                        case 4:
                            if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.Monture selectedmont = MontureDatagrid.SelectedItem as SVC.Monture;
                                List<SVC.Monture> mm = new List<SVC.Monture>();
                                mm.Add(selectedmont);
                                List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                clientvvv.Add(Clientvv);
                                ImpressionFicheAtelier cl = new ImpressionFicheAtelier(proxy, mm, clientvvv, interfaceimpressionmonture);
                                cl.Show();

                                dialog1.Close();
                                interfaceimpressionmonture = 0;
                                visualiser = false;
                            }
                            break;
                        case 3:
                            if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.Monture selecedmonture = MontureDatagrid.SelectedItem as SVC.Monture;

                                if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == true)
                                {

                                    List<SVC.Monture> listmonture = new List<SVC.Monture>();
                                    listmonture.Add(selecedmonture);
                                    var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
                                    if (VisiteApayerexiste == true)
                                    {
                                        SVC.F1 VisiteApayer = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).First();
                                        var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            /* List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                             List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                             listedepaiem.Add(dpf.First());*/

                                            List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                            List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                            dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                            listedepaiem.Add(dpf.Last());

                                            List<SVC.F1> listevisite = new List<SVC.F1>();
                                            listevisite.Add(VisiteApayer);
                                            List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                                            listclient.Add(Clientvv);
                                            ImpressionMonturePaiement cl = new ImpressionMonturePaiement(proxy, listmonture, listclient, interfaceimpressionmonture, listevisite, listedepaiem);
                                            cl.Show();
                                            dialog1.Close();
                                            interfaceimpressionmonture = 0;
                                            visualiser = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult dresult = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }
                                    }
                                }
                                else
                                {
                                    if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == false)
                                    {
                                        List<SVC.Monture> listmonture = new List<SVC.Monture>();
                                        listmonture.Add(selecedmonture);

                                        var dpfexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                        if (dpfexiste == true)
                                        {
                                            /* List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                             List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                             listedepaiem.Add(dpf.First());*/

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
                                            ImpressionMonturePaiement cl = new ImpressionMonturePaiement(proxy, listmonture, listclient, interfaceimpressionmonture, listevisite, listedepaiem);
                                            cl.Show();
                                            dialog1.Close();
                                            interfaceimpressionmonture = 0;
                                            visualiser = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult dresult = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                        }
                                    }
                                    else
                                    {
                                        MessageBoxResult dresult = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                                    }

                                }
                            }
                            break;
                        case 2:
                            if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                            {
                                SVC.Monture selecedmonture = MontureDatagrid.SelectedItem as SVC.Monture;
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
                                            interfaceimpressionmonture = 0;
                                            visualiser = false;
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
                                            interfaceimpressionmonture = 0;
                                            visualiser = false;
                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                        }

                                    }
                                }
                            }
                            break;

                    }
                }
                else
                {
                    if (visualiser == false)
                    {
                        switch (interfaceimpressionmonture)
                        {
                            case 1:
                                if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                                {
                                    SVC.Monture selectedmont = MontureDatagrid.SelectedItem as SVC.Monture;
                                    List<SVC.Monture> mm = new List<SVC.Monture>();
                                    mm.Add(selectedmont);
                                    List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                    clientvvv.Add(Clientvv);
                                    RunAtelier(mm, clientvvv);
                                    visualiser = false;
                                    dialog1.Close();
                                    interfaceimpressionmonture = 0;
                                    visualiser = false;

                                }
                                break;
                            case 4:
                                if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                                {
                                    SVC.Monture selectedmont = MontureDatagrid.SelectedItem as SVC.Monture;
                                    List<SVC.Monture> mm = new List<SVC.Monture>();
                                    mm.Add(selectedmont);
                                    List<SVC.ClientV> clientvvv = new List<SVC.ClientV>();
                                    clientvvv.Add(Clientvv);
                                    RunAtelierDouble(mm, clientvvv);
                                    visualiser = false;
                                    dialog1.Close();
                                    interfaceimpressionmonture = 0;
                                    visualiser = false;
                                }
                                break;
                            case 3:
                                if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                                {
                                    SVC.Monture selecedmonture = MontureDatagrid.SelectedItem as SVC.Monture;
                                    if (selecedmonture.StatutDevis == true && selecedmonture.StatutVente == true)
                                    {
                                        List<SVC.Monture> listmonture = new List<SVC.Monture>();
                                        listmonture.Add(selecedmonture);
                                        var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
                                        if (VisiteApayerexiste == true)
                                        {
                                            SVC.F1 VisiteApayer = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).First();
                                            var dpfexiste = proxy.GetAllDepeimentByF1(VisiteApayer.cle).Any();
                                            if (dpfexiste == true)
                                            {
                                                /* List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                                 List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                                 listedepaiem.Add(dpf.First());*/

                                                List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                                List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                                dpf.Last().montant = dpf.AsEnumerable().Sum(n => n.montant);
                                                listedepaiem.Add(dpf.Last());

                                                List<SVC.F1> listevisite = new List<SVC.F1>();
                                                listevisite.Add(VisiteApayer);
                                                List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                                                listclient.Add(Clientvv);
                                                RunMonturePaiement(listmonture, listclient, listevisite, listedepaiem);
                                                visualiser = false;
                                                dialog1.Close();
                                                interfaceimpressionmonture = 0;
                                                visualiser = false;
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
                                            List<SVC.Monture> listmonture = new List<SVC.Monture>();
                                            listmonture.Add(selecedmonture);

                                            var dpfexiste = proxy.GetAllDepeimentByF1(selecedmonture.Cle).Any();
                                            if (dpfexiste == true)
                                            {
                                                /* List<SVC.Depeiment> dpf = proxy.GetAllDepeimentByF1(VisiteApayer.cle);
                                                 List<SVC.Depeiment> listedepaiem = new List<SVC.Depeiment>();
                                                 listedepaiem.Add(dpf.First());*/

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
                                                RunMonturePaiement(listmonture, listclient, listevisite, listedepaiem);
                                                visualiser = false;
                                                dialog1.Close();
                                                interfaceimpressionmonture = 0;
                                                visualiser = false;
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
                                if (MontureDatagrid.SelectedItem != null && memberuser.ImpressionDossierClient == true)
                                {
                                    SVC.Monture selecedmonture = MontureDatagrid.SelectedItem as SVC.Monture;
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
                                                interfaceimpressionmonture = 0;
                                                visualiser = false;
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
                                                RunRecu(listedepaiem, listevisite);

                                                dialog1.Close();
                                                interfaceimpressionmonture = 0;
                                                visualiser = false;
                                            }
                                            else
                                            {
                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Paiement inéxistant", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                            }

                                        }
                                    }
                                }
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

    

        private void MontureDatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MontureDatagrid.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {
                    MontureClass = MontureDatagrid.SelectedItem as SVC.Monture;
                    GridMonture.DataContext = MontureClass;
                    GridMonture.IsEnabled = true;
                    nouvellemonture = false;
                    anciennemonture = true;
                    if (MontureClass.StatutVente == false)
                    {
                         txtMontantTotalENC.IsEnabled = true;
                    }
                    else
                    {
                         txtMontantTotalENC.IsEnabled = false;
                    }

                    if (MontureClass.StatutDevis == true && MontureClass.StatutVente == false)
                    {
                        TxtStatutGlobal.Content = "Devis";

                        TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (MontureClass.StatutDevis == true && MontureClass.StatutVente == true)
                        {
                            TxtStatutGlobal.Content = "Vente validée";
                            TxtStatutGlobal.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }


                    if (MontureClass.DroiteFleshHaut == true)
                    {
                        txtDroiteFleshHaut.Visibility = Visibility.Visible;
                        DroiteFleshBas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtDroiteFleshHaut.Visibility = Visibility.Collapsed;
                        DroiteFleshBas.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.DroiteFleshDroite == true)
                    {
                        txtDroiteFleshDroite.Visibility = Visibility.Visible;
                        txtDroiteFleshGauche.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtDroiteFleshDroite.Visibility = Visibility.Collapsed;
                        txtDroiteFleshGauche.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.GaucheFleshHaut == true)
                    {
                        txtGaucheFleshHaut.Visibility = Visibility.Visible;
                        txtGaucheFleshBas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtGaucheFleshHaut.Visibility = Visibility.Collapsed;
                        txtGaucheFleshBas.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.GaucheFleshDroite == true)
                    {
                        txtGaucheFleshDroite.Visibility = Visibility.Visible;
                        txtGaucheFleshGauche.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtGaucheFleshDroite.Visibility = Visibility.Collapsed;
                        txtGaucheFleshGauche.Visibility = Visibility.Visible;
                    }
                    /**************************************************************/
                    if (MontureClass.Loin == true && MontureClass.Pres == true)
                    {

                        ODLOIN.Text = "Loin";
                        ODITERM.Text = "Interm.";
                        ODPRES.Text = "Près";
                        OGLOIN.Text = "Loin";
                        OGINTERM.Text = "Interm.";
                        OGPRES.Text = "Près";
                        MonturePresLabel.Text = "PRES";
                        MontureLoinLabel.Text = "LOIN";


                    }
                    else
                    {
                        if (MontureClass.Loin == true && MontureClass.Pres == false)
                        {


                            ODLOIN.Text = "Loin";
                            ODITERM.Text = "Loin";
                            ODPRES.Text = "Près";
                            OGLOIN.Text = "Loin";
                            OGINTERM.Text = "Loin";
                            OGPRES.Text = "Près";
                            MonturePresLabel.Text = "LOIN";
                            MontureLoinLabel.Text = "LOIN";


                        }
                        else
                        {
                            if (MontureClass.Loin == false && MontureClass.Pres == true)
                            {
                                ODLOIN.Text = "Loin";
                                ODITERM.Text = "Près";
                                ODPRES.Text = "Près";
                                OGLOIN.Text = "Loin";
                                OGINTERM.Text = "Près";
                                OGPRES.Text = "Près";
                                MonturePresLabel.Text = "PRES";
                                MontureLoinLabel.Text = "PRES";
                            }
                        }
                    }
                    if (MontureClass.Encaissé != 0)
                    {
                        montureversementzero = true;
                    }
                    else
                    {
                        montureversementzero = false;
                    }
                    if (MontureClass.StatutDevis == true && MontureClass.StatutVente == false)
                    {
                        TxtStatutGlobal.Content = "Devis";

                        TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (MontureClass.StatutDevis == true && MontureClass.StatutVente == true)
                        {
                            TxtStatutGlobal.Content = "Vente validée";
                            TxtStatutGlobal.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }

                }
                listsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                listsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList();
                listsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList();
                listsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();
                anciennelistsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                anciennelistsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList();
                anciennelistsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList();
                anciennelistsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnformule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /********************GroiteLoin*************************************/
                if (txtLoinSphereDroite.Text != "" && txtLoinCylindreDroite.Text != "" && txtLoinAxeDroite.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtLoinSphereDroite.Text != "")
                    {
                        if (decimal.TryParse(txtLoinSphereDroite.Text, out sphere))
                            sphere = Convert.ToDecimal(txtLoinSphereDroite.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtLoinSphereDroite.Text = "";
                        sphere = 0;
                    }
                    if (txtLoinCylindreDroite.Text != "")
                    {
                        if (decimal.TryParse(txtLoinCylindreDroite.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtLoinCylindreDroite.Text);
                            if (cylindre > 0)
                            {
                                MontureClass.DroiteCylindrePlus = true;
                                MontureClass.DroiteCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    MontureClass.DroiteCylindrePlus = false;
                                    MontureClass.DroiteCylindreMoin = true;
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
                        txtLoinCylindreDroite.Text = "";
                        cylindre = 0;
                    }

                    if (txtLoinAxeDroite.Text != "")
                    {
                        if (int.TryParse(txtLoinAxeDroite.Text, out axe))
                            axe = Convert.ToInt16(txtLoinAxeDroite.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtLoinAxeDroite.Text = "";
                        axe = 0;
                    }
                    txtLoinCylindreDroite.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtLoinSphereDroite.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (MontureClass.DroiteCylindrePlus == true && MontureClass.DroiteCylindreMoin == false)
                    {
                        txtLoinAxeDroite.Text = (axe + 90).ToString();
                        MontureClass.DroiteCylindrePlus = false;
                        MontureClass.DroiteCylindreMoin = true;

                    }
                    else
                    {
                        if (MontureClass.DroiteCylindrePlus == false && MontureClass.DroiteCylindreMoin == true)
                        {
                            txtLoinAxeDroite.Text = (axe - 90).ToString();
                            MontureClass.DroiteCylindrePlus = true;
                            MontureClass.DroiteCylindreMoin = false;
                        }
                    }
                }
                /********************GroiteIntermediaire*************************************/
                if (txtIntermSphereDroite.Text != "" && txtIntermCylindreDroite.Text != "" && txtIntermAxeDroite.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtIntermSphereDroite.Text != "")
                    {
                        if (decimal.TryParse(txtIntermSphereDroite.Text, out sphere))
                            sphere = Convert.ToDecimal(txtIntermSphereDroite.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtIntermSphereDroite.Text = "";
                        sphere = 0;
                    }
                    if (txtIntermCylindreDroite.Text != "")
                    {
                        if (decimal.TryParse(txtIntermCylindreDroite.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtIntermCylindreDroite.Text);

                            if (cylindre > 0)
                            {
                                MontureClass.DroiteCylindrePlus = true;
                                MontureClass.DroiteCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    MontureClass.DroiteCylindrePlus = false;
                                    MontureClass.DroiteCylindreMoin = true;
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
                        txtIntermCylindreDroite.Text = "";
                        cylindre = 0;
                    }

                    if (txtIntermAxeDroite.Text != "")
                    {
                        if (int.TryParse(txtIntermAxeDroite.Text, out axe))
                            axe = Convert.ToInt16(txtIntermAxeDroite.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtIntermAxeDroite.Text = "";
                        axe = 0;
                    }
                    txtIntermCylindreDroite.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtIntermSphereDroite.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (MontureClass.DroiteCylindrePlus == true && MontureClass.DroiteCylindreMoin == false)
                    {
                        txtIntermAxeDroite.Text = (axe + 90).ToString();
                        MontureClass.DroiteCylindrePlus = false;
                        MontureClass.DroiteCylindreMoin = true;

                    }
                    else
                    {
                        if (MontureClass.DroiteCylindrePlus == false && MontureClass.DroiteCylindreMoin == true)
                        {
                            txtIntermAxeDroite.Text = (axe - 90).ToString();
                            MontureClass.DroiteCylindrePlus = true;
                            MontureClass.DroiteCylindreMoin = false;
                        }
                    }
                }

                /********************GroitePres*************************************/
                if (txtPresSphereDroite.Text != "" && txtPresCylindreDroite.Text != "" && txtPresAxeDroite.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtPresSphereDroite.Text != "")
                    {
                        if (decimal.TryParse(txtPresSphereDroite.Text, out sphere))
                            sphere = Convert.ToDecimal(txtPresSphereDroite.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtPresSphereDroite.Text = "";
                        sphere = 0;
                    }
                    if (txtPresCylindreDroite.Text != "")
                    {
                        if (decimal.TryParse(txtPresCylindreDroite.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtPresCylindreDroite.Text);
                            if (cylindre > 0)
                            {
                                MontureClass.DroiteCylindrePlus = true;
                                MontureClass.DroiteCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    MontureClass.DroiteCylindrePlus = false;
                                    MontureClass.DroiteCylindreMoin = true;
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
                        txtPresCylindreDroite.Text = "";
                        cylindre = 0;
                    }

                    if (txtPresAxeDroite.Text != "")
                    {
                        if (int.TryParse(txtPresAxeDroite.Text, out axe))
                            axe = Convert.ToInt16(txtPresAxeDroite.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtPresAxeDroite.Text = "";
                        axe = 0;
                    }
                    txtPresCylindreDroite.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtPresSphereDroite.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (MontureClass.DroiteCylindrePlus == true && MontureClass.DroiteCylindreMoin == false)
                    {
                        txtPresAxeDroite.Text = (axe + 90).ToString();
                        MontureClass.DroiteCylindrePlus = false;
                        MontureClass.DroiteCylindreMoin = true;

                    }
                    else
                    {
                        if (MontureClass.DroiteCylindrePlus == false && MontureClass.DroiteCylindreMoin == true)
                        {
                            txtPresAxeDroite.Text = (axe - 90).ToString();
                            MontureClass.DroiteCylindrePlus = true;
                            MontureClass.DroiteCylindreMoin = false;
                        }
                    }
                }
                /*****************************GAUUUUUUUUUCHHHHHHHHHHHHHHEEEEE*****************************/
                /********************GaucheLoin*************************************/
                if (txtLoinSphereGauche.Text != "" && txtLoinCylindreGauche.Text != "" && txtLoinAxeGauche.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtLoinSphereGauche.Text != "")
                    {
                        if (decimal.TryParse(txtLoinSphereGauche.Text, out sphere))
                            sphere = Convert.ToDecimal(txtLoinSphereGauche.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtLoinSphereGauche.Text = "";
                        sphere = 0;
                    }
                    if (txtLoinCylindreGauche.Text != "")
                    {
                        if (decimal.TryParse(txtLoinCylindreGauche.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtLoinCylindreGauche.Text);
                            if (cylindre > 0)
                            {
                                MontureClass.GaucheCylindrePlus = true;
                                MontureClass.GaucheCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    MontureClass.GaucheCylindrePlus = false;
                                    MontureClass.GaucheCylindreMoin = true;
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
                        txtLoinCylindreGauche.Text = "";
                        cylindre = 0;
                    }

                    if (txtLoinAxeGauche.Text != "")
                    {
                        if (int.TryParse(txtLoinAxeGauche.Text, out axe))
                            axe = Convert.ToInt16(txtLoinAxeGauche.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtLoinAxeGauche.Text = "";
                        axe = 0;
                    }
                    txtLoinCylindreGauche.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtLoinSphereGauche.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (MontureClass.GaucheCylindrePlus == true && MontureClass.GaucheCylindreMoin == false)
                    {
                        txtLoinAxeGauche.Text = (axe + 90).ToString();
                        MontureClass.GaucheCylindrePlus = false;
                        MontureClass.GaucheCylindreMoin = true;

                    }
                    else
                    {
                        if (MontureClass.GaucheCylindrePlus == false && MontureClass.GaucheCylindreMoin == true)
                        {
                            txtLoinAxeGauche.Text = (axe - 90).ToString();
                            MontureClass.GaucheCylindrePlus = true;
                            MontureClass.GaucheCylindreMoin = false;
                        }
                    }
                }
                /********************GaucheIntermediarire*************************************/
                if (txtIntermSphereGauche.Text != "" && txtIntermCylindreGauche.Text != "" && txtIntermAxeGauche.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtIntermSphereGauche.Text != "")
                    {
                        if (decimal.TryParse(txtIntermSphereGauche.Text, out sphere))
                            sphere = Convert.ToDecimal(txtIntermSphereGauche.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtIntermSphereGauche.Text = "";
                        sphere = 0;
                    }
                    if (txtIntermCylindreGauche.Text != "")
                    {
                        if (decimal.TryParse(txtIntermCylindreGauche.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtIntermCylindreGauche.Text);
                            if (cylindre > 0)
                            {
                                MontureClass.GaucheCylindrePlus = true;
                                MontureClass.GaucheCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    MontureClass.GaucheCylindrePlus = false;
                                    MontureClass.GaucheCylindreMoin = true;
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
                        txtIntermCylindreGauche.Text = "";
                        cylindre = 0;
                    }

                    if (txtIntermAxeGauche.Text != "")
                    {
                        if (int.TryParse(txtIntermAxeGauche.Text, out axe))
                            axe = Convert.ToInt16(txtIntermAxeGauche.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtIntermAxeGauche.Text = "";
                        axe = 0;
                    }
                    txtIntermCylindreGauche.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtIntermSphereGauche.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (MontureClass.GaucheCylindrePlus == true && MontureClass.GaucheCylindreMoin == false)
                    {
                        txtIntermAxeGauche.Text = (axe + 90).ToString();
                        MontureClass.GaucheCylindrePlus = false;
                        MontureClass.GaucheCylindreMoin = true;

                    }
                    else
                    {
                        if (MontureClass.GaucheCylindrePlus == false && MontureClass.GaucheCylindreMoin == true)
                        {
                            txtIntermAxeGauche.Text = (axe - 90).ToString();
                            MontureClass.GaucheCylindrePlus = true;
                            MontureClass.GaucheCylindreMoin = false;
                        }
                    }
                }
                /********************GauchePres*************************************/
                if (txtPresSphereGauche.Text != "" && txtPresCylindreGauche.Text != "" && txtPresAxeGauche.Text != "")
                {

                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtPresSphereGauche.Text != "")
                    {
                        if (decimal.TryParse(txtPresSphereGauche.Text, out sphere))
                            sphere = Convert.ToDecimal(txtPresSphereGauche.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtPresSphereGauche.Text = "";
                        sphere = 0;
                    }
                    if (txtPresCylindreGauche.Text != "")
                    {
                        if (decimal.TryParse(txtPresCylindreGauche.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtPresCylindreGauche.Text);
                            if (cylindre > 0)
                            {
                                MontureClass.GaucheCylindrePlus = true;
                                MontureClass.GaucheCylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    MontureClass.GaucheCylindrePlus = false;
                                    MontureClass.GaucheCylindreMoin = true;
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
                        txtPresCylindreGauche.Text = "";
                        cylindre = 0;
                    }

                    if (txtPresAxeGauche.Text != "")
                    {
                        if (int.TryParse(txtPresAxeGauche.Text, out axe))
                            axe = Convert.ToInt16(txtPresAxeGauche.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtPresAxeGauche.Text = "";
                        axe = 0;
                    }
                    txtPresCylindreGauche.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtPresSphereGauche.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (MontureClass.GaucheCylindrePlus == true && MontureClass.GaucheCylindreMoin == false)
                    {
                        txtPresAxeGauche.Text = (axe + 90).ToString();
                        MontureClass.GaucheCylindrePlus = false;
                        MontureClass.GaucheCylindreMoin = true;

                    }
                    else
                    {
                        if (MontureClass.GaucheCylindrePlus == false && MontureClass.GaucheCylindreMoin == true)
                        {
                            txtPresAxeGauche.Text = (axe - 90).ToString();
                            MontureClass.GaucheCylindrePlus = true;
                            MontureClass.GaucheCylindreMoin = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
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
        /*  private void ValiderMonture_Click(object sender, RoutedEventArgs e)
          {
              try
              {

                  if (nouvellemonture == true && anciennemonture == false && memberuser.CreationDossierClient == true)
                  {
                      //                    string _numbers = MontureClass.IdClient.ToString() + MontureClass.MontantTotal.ToString() + DateTime.Now.Day.ToString()+MontureClass.DroitPrixVerreLoin.ToString()+MontureClass.DroitPrixVerrePres.ToString();

                      int interfacecreationmonture = 0;
                      bool creationmonture, creationcaisse, creationdepaiement = false;
                      MontureClass.StatutDevis = true;
                      MontureClass.StatutVente = false;
                      string _numbers = Convert.ToString(MontureClass.IdClient) + Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute) + Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond);

                      MessageBoxResult resul0333 = Xceed.Wpf.Toolkit.MessageBox.Show(Convert.ToString(DoWork(_numbers)), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                      MontureClass.NCommande = Convert.ToString(DoWork(_numbers));

                      SVC.Depense CAISSEMONTURE;
                      SVC.Depeiment PAIEMENTMONTURE;
                      if (txtMontantTotalENC.Text != "")
                      {
                          if (Convert.ToDecimal(txtMontantTotalENC.Text) != 0)
                          {
                              MontureClass.Encaissé = Convert.ToDecimal(txtMontantTotalENC.Text);
                              MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                          }
                          else
                          {
                              MontureClass.Encaissé = 0;
                              MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                          }
                      }
                      else
                      {
                          MontureClass.Encaissé = 0;
                          MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                      }
                      MontureClass.Cle = Clientvv.Id + Clientvv.Raison + MontureClass.MontantTotal + DateTime.Now.TimeOfDay;
                      if (txtAccessoiresQuantite1.Text != "")
                      {
                          if (Convert.ToDecimal(txtAccessoiresQuantite1.Text) == 0)
                          {
                              MontureClass.Accessoires1 = "";
                              MontureClass.AccessoiresPrix1 = null;
                              txtAccessoires1.Text = "";
                              txtAccessoiresPrix1.Text = "0";
                          }
                      }
                      if (txtAccessoiresQuantite2.Text != "")
                      {
                          if (Convert.ToDecimal(txtAccessoiresQuantite2.Text) == 0)
                          {
                              MontureClass.Accessoires2 = "";
                              MontureClass.AccessoiresPrix2 = null;
                              txtAccessoires2.Text = "";
                              txtAccessoiresPrix2.Text = "0";
                          }
                      }
                      if (MontureClass.Encaissé != 0)
                      {


                          PAIEMENTMONTURE = new SVC.Depeiment
                          {
                              date = MontureClass.Date,
                              montant = Convert.ToDecimal(MontureClass.Encaissé),
                              paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " date :" + MontureClass.Date,
                              oper = memberuser.Username,
                              dates = MontureClass.Dates,
                              banque = "Caisse",
                              nfact = MontureClass.Date + " " + MontureClass.RaisonClient,
                              amontant = Convert.ToDecimal(MontureClass.MontantTotal),
                              cle = MontureClass.Cle,
                              cp = MontureClass.Id,
                              Multiple = false,
                              CodeClient = MontureClass.IdClient,
                              RaisonClient = MontureClass.RaisonClient,

                          };
                          CAISSEMONTURE = new SVC.Depense
                          {
                              cle = MontureClass.Cle,
                              Auto = true,
                              Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                              CompteDébité = "Caisse",
                              Crédit = true,
                              DateDebit = MontureClass.Date,
                              DateSaisie = MontureClass.Dates,
                              Débit = false,
                              ModePaiement = "ESPECES",
                              Montant = 0,
                              MontantCrédit = MontureClass.Encaissé,
                              NumCheque = Convert.ToString(MontureClass.Id),
                              Num_Facture = MontureClass.Date + " " + MontureClass.RaisonClient,
                              RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                              Username = memberuser.Username,

                          };
                          interfacecreationmonture = 1;



                          using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                          {

                              proxy.InsertMonture(MontureClass);
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
                              proxy.AjouterMontureRefresh(Clientvv.Id);
                              GridMonture.IsEnabled = false;
                              GridMonture.DataContext = null;
                              montureversementzero = false;
                              MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                          }
                      }
                      else
                      {
                          using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                          {


                              proxy.InsertMonture(MontureClass);
                              ts.Complete();
                              creationmonture = true;







                          }
                          if (creationmonture == true)
                          {
                              proxy.AjouterTransactionPaiementRefresh();
                              proxy.AjouterDepenseRefresh();
                              proxy.AjouterMontureRefresh(Clientvv.Id);
                              GridMonture.IsEnabled = false;
                              GridMonture.DataContext = null;
                              montureversementzero = false;
                              MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                          }
                      }


                  }
                  else
                  {
                      if (nouvellemonture == false && anciennemonture == true && memberuser.ModificationDossierClient == true)
                      {
                          int interfacecreationmonture = 0;
                          bool creationmonture, creationcaisse, creationdepaiement = false;

                          // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                          SVC.Depense CAISSEMONTURE;
                          SVC.Depeiment PAIEMENTMONTURE;


                          if (txtMontantTotalENC.Text != "")
                          {
                              if (Convert.ToDecimal(txtMontantTotalENC.Text) != 0)
                              {
                                  MontureClass.Encaissé = Convert.ToDecimal(txtMontantTotalENC.Text);
                                  MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;

                              }
                              else
                              {
                                  MontureClass.Encaissé = 0;
                                  MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;

                              }

                          }
                          else
                          {
                              MontureClass.Encaissé = 0;
                              MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;

                          }
                          if (txtAccessoiresQuantite1.Text != "")
                          {
                              if (Convert.ToDecimal(txtAccessoiresQuantite1.Text) == 0)
                              {

                                  MontureClass.Accessoires1 = "";
                                  MontureClass.AccessoiresPrix1 = null;
                                  txtAccessoires1.Text = "";
                                  txtAccessoiresPrix1.Text = "0";

                              }
                          }
                          if (txtAccessoiresQuantite2.Text != "")
                          {
                              if (Convert.ToDecimal(txtAccessoiresQuantite2.Text) == 0)
                              {
                                  MontureClass.Accessoires2 = "";
                                  MontureClass.AccessoiresPrix2 = null;
                                  txtAccessoires2.Text = "";
                                  txtAccessoiresPrix2.Text = "0";
                              }
                          }
                          if (montureversementzero == false)
                          {
                              if (MontureClass.Encaissé != 0)
                              {


                                  PAIEMENTMONTURE = new SVC.Depeiment
                                  {
                                      date = MontureClass.Date,
                                      montant = Convert.ToDecimal(MontureClass.Encaissé),
                                      paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " date :" + MontureClass.Date,
                                      oper = memberuser.Username,
                                      dates = MontureClass.Dates,
                                      banque = "Caisse",
                                      nfact = MontureClass.Date + " " + MontureClass.RaisonClient,
                                      amontant = Convert.ToDecimal(MontureClass.MontantTotal),
                                      cle = MontureClass.Cle,
                                      cp = MontureClass.Id,
                                      Multiple = false,
                                      CodeClient = MontureClass.IdClient,
                                      RaisonClient = MontureClass.RaisonClient,

                                  };
                                  CAISSEMONTURE = new SVC.Depense
                                  {
                                      cle = MontureClass.Cle,
                                      Auto = true,
                                      Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                                      CompteDébité = "Caisse",
                                      Crédit = true,
                                      DateDebit = MontureClass.Date,
                                      DateSaisie = MontureClass.Dates,
                                      Débit = false,
                                      ModePaiement = "ESPECES",
                                      Montant = 0,
                                      MontantCrédit = MontureClass.Encaissé,
                                      NumCheque = Convert.ToString(MontureClass.Id),
                                      Num_Facture = MontureClass.Date + " " + MontureClass.RaisonClient,
                                      RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                                      Username = memberuser.Username,

                                  };
                                  interfacecreationmonture = 1;
                                  using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                  {

                                      proxy.UpdateMonture(MontureClass);
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
                                      proxy.AjouterMontureRefresh(Clientvv.Id);
                                      GridMonture.IsEnabled = false;
                                      GridMonture.DataContext = null;
                                      montureversementzero = false;

                                      MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                  }
                              }
                              else
                              {
                                  using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                  {


                                      proxy.UpdateMonture(MontureClass);
                                      ts.Complete();
                                      creationmonture = true;







                                  }
                                  if (creationmonture == true)
                                  {
                                      proxy.AjouterTransactionPaiementRefresh();
                                      proxy.AjouterDepenseRefresh();
                                      proxy.AjouterMontureRefresh(Clientvv.Id);
                                      GridMonture.IsEnabled = false;
                                      GridMonture.DataContext = null;
                                      montureversementzero = false;

                                      MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                  }
                              }
                          }
                          else
                          {
                              if (montureversementzero == true)
                              {
                                  if (MontureClass.Encaissé == 0)
                                  {
                                      CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == MontureClass.Cle).First();
                                      PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == MontureClass.Cle).First();
                                      using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                      {

                                          proxy.UpdateMonture(MontureClass);
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
                                          proxy.AjouterMontureRefresh(Clientvv.Id);
                                          GridMonture.IsEnabled = false;
                                          GridMonture.DataContext = null;
                                          montureversementzero = false;
                                          MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                      }
                                  }
                                  else
                                  {
                                      if (MontureClass.Encaissé != 0)
                                      {
                                          CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == MontureClass.Cle).First();
                                          CAISSEMONTURE.MontantCrédit = MontureClass.Encaissé;
                                          PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == MontureClass.Cle).First();
                                          PAIEMENTMONTURE.montant = Convert.ToDecimal(MontureClass.Encaissé);
                                          using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                          {

                                              proxy.UpdateMonture(MontureClass);
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
                                              proxy.AjouterMontureRefresh(Clientvv.Id);
                                              GridMonture.IsEnabled = false;
                                              GridMonture.DataContext = null;
                                              montureversementzero = false;
                                              MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

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

        /*  private void ValiderMonture_Click(object sender, RoutedEventArgs e)
          {
              try
              {
                  /****************création monture**************************/
        /*   if (nouvellemonture == true && anciennemonture == false && memberuser.CreationDossierClient == true)
           {
               //                    string _numbers = MontureClass.IdClient.ToString() + MontureClass.MontantTotal.ToString() + DateTime.Now.Day.ToString()+MontureClass.DroitPrixVerreLoin.ToString()+MontureClass.DroitPrixVerrePres.ToString();

               int interfacecreationmonture = 0;
               bool creationmonture, creationcaisse, creationdepaiement = false;
               bool creationsuppmonture1 = false;
               bool creationsuppmonture2 = false;
               bool creationsuppmonture3 = false;
               bool creationsuppmonture4 = false;
               MontureClass.StatutDevis = true;
               MontureClass.StatutVente = false;
               string _numbers = Convert.ToString(MontureClass.IdClient) + Convert.ToString(MontureClass.MontantTotal)+ Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute) + Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond);

               //    MessageBoxResult resul0333 = Xceed.Wpf.Toolkit.MessageBox.Show(Convert.ToString(DoWork(_numbers)), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
               MontureClass.NCommande = Convert.ToString(DoWork(_numbers));

               SVC.Depense CAISSEMONTURE;
               SVC.Depeiment PAIEMENTMONTURE;
               if (txtMontantTotalENC.Text != "")
               {
                   if (Convert.ToDecimal(txtMontantTotalENC.Text) != 0)
                   {
                       MontureClass.Encaissé = Convert.ToDecimal(txtMontantTotalENC.Text);
                       MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                   }
                   else
                   {
                       MontureClass.Encaissé = 0;
                       MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                   }
               }
               else
               {
                   MontureClass.Encaissé = 0;
                   MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
               }
               MontureClass.Cle = Clientvv.Id + Clientvv.Raison + MontureClass.MontantTotal + DateTime.Now.TimeOfDay;
               if (txtAccessoiresQuantite1.Text != "")
               {
                   if (Convert.ToDecimal(txtAccessoiresQuantite1.Text) == 0)
                   {
                       MontureClass.Accessoires1 = "";
                       MontureClass.AccessoiresPrix1 = null;
                       txtAccessoires1.Text = "";
                       txtAccessoiresPrix1.Text = "0";
                   }
               }
               if (txtAccessoiresQuantite2.Text != "")
               {
                   if (Convert.ToDecimal(txtAccessoiresQuantite2.Text) == 0)
                   {
                       MontureClass.Accessoires2 = "";
                       MontureClass.AccessoiresPrix2 = null;
                       txtAccessoires2.Text = "";
                       txtAccessoiresPrix2.Text = "0";
                   }
               }
               if (MontureClass.Encaissé != 0)
               {

                   PAIEMENTMONTURE = new SVC.Depeiment
                   {
                       date = MontureClass.Date,
                       montant = Convert.ToDecimal(MontureClass.Encaissé),
                       paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " date :" + MontureClass.Date,
                       oper = memberuser.Username,
                       dates = MontureClass.Dates,
                       banque = "Caisse",
                       nfact = MontureClass.Date + " " + MontureClass.RaisonClient,
                       amontant = Convert.ToDecimal(MontureClass.MontantTotal),
                       cle = MontureClass.Cle,
                       cp = MontureClass.Id,
                       Multiple = false,
                       CodeClient = MontureClass.IdClient,
                       RaisonClient = MontureClass.RaisonClient,

                   };
                   CAISSEMONTURE = new SVC.Depense
                   {
                       cle = MontureClass.Cle,
                       Auto = true,
                       Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                       CompteDébité = "Caisse",
                       Crédit = true,
                       DateDebit = MontureClass.Date,
                       DateSaisie = MontureClass.Dates,
                       Débit = false,
                       ModePaiement = "ESPECES",
                       Montant = 0,
                       MontantCrédit = MontureClass.Encaissé,
                       NumCheque = Convert.ToString(MontureClass.Id),
                       Num_Facture = MontureClass.Date + " " + MontureClass.RaisonClient,
                       RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                       Username = memberuser.Username,

                   };
                   interfacecreationmonture = 1;



                   using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                   {
                       /**********sup1********//////
                                               /*        if (listsupp1 != null)
                                                       {
                                                           if (listsupp1.Count > 0)
                                                           {
                                                               foreach (var itemsupp in listsupp1)
                                                               {
                                                                   creationsuppmonture1 = false;
                                                                   itemsupp.cle = MontureClass.Cle;
                                                                   itemsupp.interfaceMonture = 1;
                                                                   proxy.InsertMontureSupplement(itemsupp);
                                                                   creationsuppmonture1 = true;
                                                               }
                                                           }
                                                           else
                                                           {
                                                               creationsuppmonture1 = true;
                                                           }
                                                       }
                                                       else
                                                       {
                                                           creationsuppmonture1 = true;
                                                       }
                                                       /*************sup2*//////
                                                                           /*   if (listsupp2 != null)
                                                                              {
                                                                                  if (listsupp2.Count > 0)
                                                                                  {
                                                                                      foreach (var itemsupp in listsupp2)
                                                                                      {
                                                                                          creationsuppmonture1 = false;
                                                                                          itemsupp.cle = MontureClass.Cle;
                                                                                          itemsupp.interfaceMonture = 2;
                                                                                          proxy.InsertMontureSupplement(itemsupp);
                                                                                          creationsuppmonture1 = true;
                                                                                      }
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      creationsuppmonture2 = true;
                                                                                  }
                                                                              }
                                                                              else
                                                                              {
                                                                                  creationsuppmonture2 = true;
                                                                              }
                                                                              /***********supp3***************/
                                                                           /*   if (listsupp3 != null)
                                                                              {
                                                                                  if (listsupp3.Count > 0)
                                                                                  {
                                                                                      foreach (var itemsupp in listsupp3)
                                                                                      {
                                                                                          creationsuppmonture3 = false;
                                                                                          itemsupp.cle = MontureClass.Cle;
                                                                                          itemsupp.interfaceMonture = 3;
                                                                                          proxy.InsertMontureSupplement(itemsupp);
                                                                                          creationsuppmonture3 = true;
                                                                                      }
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      creationsuppmonture3 = true;
                                                                                  }
                                                                              }
                                                                              else
                                                                              {
                                                                                  creationsuppmonture3 = true;
                                                                              }
                                                                              /*******************sup4************/
                                                                           /*   if (listsupp4 != null)
                                                                              {
                                                                                  if (listsupp4.Count > 0)
                                                                                  {
                                                                                      foreach (var itemsupp in listsupp4)
                                                                                      {
                                                                                          creationsuppmonture4 = false;
                                                                                          itemsupp.cle = MontureClass.Cle;
                                                                                          itemsupp.interfaceMonture = 4;
                                                                                          proxy.InsertMontureSupplement(itemsupp);
                                                                                          creationsuppmonture4 = true;
                                                                                      }
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      creationsuppmonture4 = true;
                                                                                  }
                                                                              }
                                                                              else
                                                                              {
                                                                                  creationsuppmonture4 = true;
                                                                              }
                                                                              /***********************************/
                                                                           /*  proxy.InsertMonture(MontureClass);
                                                                             creationmonture = true;


                                                                             proxy.InsertDepense(CAISSEMONTURE);
                                                                             creationcaisse = true;
                                                                             proxy.InsertDepeiment(PAIEMENTMONTURE);
                                                                             creationdepaiement = true;

                                                                             if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                             {
                                                                                 ts.Complete();
                                                                             }
                                                                         }
                                                                         if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                         {
                                                                             proxy.AjouterTransactionPaiementRefresh();
                                                                             proxy.AjouterDepenseRefresh();
                                                                             proxy.AjouterMontureRefresh(Clientvv.Id);
                                                                             proxy.AjouterMontureSupplementRefresh(MontureClass.Cle);
                                                                             GridMonture.IsEnabled = false;
                                                                             GridMonture.DataContext = null;
                                                                             montureversementzero = false;
                                                                             MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                                         }
                                                                     }
                                                                     else
                                                                     {
                                                                         // MessageBoxResult resul03s = Xceed.Wpf.Toolkit.MessageBox.Show("Im here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                                         using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                                         {

                                                                             /**********sup1********//////
                                                                                                     /*  if (listsupp1 != null)
                                                                                                       {
                                                                                                           if (listsupp1.Count > 0)
                                                                                                           {
                                                                                                               foreach (var itemsupp in listsupp1)
                                                                                                               {
                                                                                                                   creationsuppmonture1 = false;
                                                                                                                   itemsupp.cle = MontureClass.Cle;
                                                                                                                   itemsupp.interfaceMonture = 1;
                                                                                                                   proxy.InsertMontureSupplement(itemsupp);
                                                                                                                   creationsuppmonture1 = true;
                                                                                                               }
                                                                                                           }
                                                                                                           else
                                                                                                           {
                                                                                                               creationsuppmonture1 = true;
                                                                                                           }
                                                                                                       }
                                                                                                       else
                                                                                                       {
                                                                                                           creationsuppmonture1 = true;
                                                                                                       }
                                                                                                       /*************sup2*//////
                                                                                                                           /*    if (listsupp2 != null)
                                                                                                                               {
                                                                                                                                   if (listsupp2.Count > 0)
                                                                                                                                   {
                                                                                                                                       foreach (var itemsupp in listsupp2)
                                                                                                                                       {
                                                                                                                                           creationsuppmonture1 = false;
                                                                                                                                           itemsupp.cle = MontureClass.Cle;
                                                                                                                                           itemsupp.interfaceMonture = 2;
                                                                                                                                           proxy.InsertMontureSupplement(itemsupp);
                                                                                                                                           creationsuppmonture1 = true;
                                                                                                                                       }
                                                                                                                                   }
                                                                                                                                   else
                                                                                                                                   {
                                                                                                                                       creationsuppmonture2 = true;
                                                                                                                                   }
                                                                                                                               }
                                                                                                                               else
                                                                                                                               {
                                                                                                                                   creationsuppmonture2 = true;
                                                                                                                               }
                                                                                                                               /***********supp3***************/
                                                                                                                           /*  if (listsupp3 != null)
                                                                                                                             {
                                                                                                                                 if (listsupp3.Count > 0)
                                                                                                                                 {
                                                                                                                                     foreach (var itemsupp in listsupp3)
                                                                                                                                     {
                                                                                                                                         creationsuppmonture3 = false;
                                                                                                                                         itemsupp.cle = MontureClass.Cle;
                                                                                                                                         itemsupp.interfaceMonture = 3;
                                                                                                                                         proxy.InsertMontureSupplement(itemsupp);
                                                                                                                                         creationsuppmonture3 = true;
                                                                                                                                     }
                                                                                                                                 }
                                                                                                                                 else
                                                                                                                                 {
                                                                                                                                     creationsuppmonture3 = true;
                                                                                                                                 }
                                                                                                                             }
                                                                                                                             else
                                                                                                                             {
                                                                                                                                 creationsuppmonture3 = true;
                                                                                                                             }
                                                                                                                             /*******************sup4************/
                                                                                                                           /*    if (listsupp4 != null)
                                                                                                                               {
                                                                                                                                   if (listsupp4.Count > 0)
                                                                                                                                   {
                                                                                                                                       foreach (var itemsupp in listsupp4)
                                                                                                                                       {
                                                                                                                                           creationsuppmonture4 = false;
                                                                                                                                           itemsupp.cle = MontureClass.Cle;
                                                                                                                                           itemsupp.interfaceMonture = 4;
                                                                                                                                           proxy.InsertMontureSupplement(itemsupp);
                                                                                                                                           creationsuppmonture4 = true;
                                                                                                                                       }
                                                                                                                                   }
                                                                                                                                   else
                                                                                                                                   {
                                                                                                                                       creationsuppmonture4 = true;
                                                                                                                                   }
                                                                                                                               }
                                                                                                                               else
                                                                                                                               {
                                                                                                                                   creationsuppmonture4 = true;
                                                                                                                               }
                                                                                                                               proxy.InsertMonture(MontureClass);
                                                                                                                               creationmonture = true;

                                                                                                                               if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                               {
                                                                                                                                   ts.Complete();
                                                                                                                               }

                                                                                                                           }
                                                                                                                           if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                           {
                                                                                                                               proxy.AjouterTransactionPaiementRefresh();
                                                                                                                               proxy.AjouterDepenseRefresh();
                                                                                                                               proxy.AjouterMontureRefresh(Clientvv.Id);
                                                                                                                               proxy.AjouterMontureSupplementRefresh(MontureClass.Cle);
                                                                                                                               GridMonture.IsEnabled = false;
                                                                                                                               GridMonture.DataContext = null;
                                                                                                                               montureversementzero = false;
                                                                                                                               MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                                                                                           }
                                                                                                                       }


                                                                                                                   }
                                                                                                                   /***************************modification monture*****************/
                                                                                                                           /* else
                                                                                                                            {
                                                                                                                                if (nouvellemonture == false && anciennemonture == true && memberuser.ModificationDossierClient == true)
                                                                                                                                {
                                                                                                                                    int interfacecreationmonture = 0;
                                                                                                                                    bool creationmonture, creationcaisse, creationdepaiement = false;
                                                                                                                                    bool creationsuppmonture1 = false;
                                                                                                                                    bool creationsuppmonture2 = false;
                                                                                                                                    bool creationsuppmonture3 = false;
                                                                                                                                    bool creationsuppmonture4 = false;
                                                                                                                                    // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                                                                                                                                    SVC.Depense CAISSEMONTURE;
                                                                                                                                    SVC.Depeiment PAIEMENTMONTURE;


                                                                                                                                    if (txtMontantTotalENC.Text != "")
                                                                                                                                    {
                                                                                                                                        if (Convert.ToDecimal(txtMontantTotalENC.Text) != 0)
                                                                                                                                        {
                                                                                                                                            MontureClass.Encaissé = Convert.ToDecimal(txtMontantTotalENC.Text);
                                                                                                                                            MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            MontureClass.Encaissé = 0;
                                                                                                                                            MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                                                                                                                                        }

                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        MontureClass.Encaissé = 0;
                                                                                                                                        MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                                                                                                                                    }
                                                                                                                                    if (txtAccessoiresQuantite1.Text != "")
                                                                                                                                    {
                                                                                                                                        if (Convert.ToDecimal(txtAccessoiresQuantite1.Text) == 0)
                                                                                                                                        {
                                                                                                                                            MontureClass.Accessoires1 = "";
                                                                                                                                            MontureClass.AccessoiresPrix1 = null;
                                                                                                                                            txtAccessoires1.Text = "";
                                                                                                                                            txtAccessoiresPrix1.Text = "0";
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    if (txtAccessoiresQuantite2.Text != "")
                                                                                                                                    {
                                                                                                                                        if (Convert.ToDecimal(txtAccessoiresQuantite2.Text) == 0)
                                                                                                                                        {
                                                                                                                                            MontureClass.Accessoires2 = "";
                                                                                                                                            MontureClass.AccessoiresPrix2 = null;
                                                                                                                                            txtAccessoires2.Text = "";
                                                                                                                                            txtAccessoiresPrix2.Text = "0";
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    ///MontureCLass ==0 lors du chargement
                                                                                                                                    if (montureversementzero == false)
                                                                                                                                    {
                                                                                                                                        //nouveau encaissement alors insertion
                                                                                                                                        if (MontureClass.Encaissé != 0)
                                                                                                                                        {
                                                                                                                                            PAIEMENTMONTURE = new SVC.Depeiment
                                                                                                                                            {
                                                                                                                                                date = MontureClass.Date,
                                                                                                                                                montant = Convert.ToDecimal(MontureClass.Encaissé),
                                                                                                                                                paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " date :" + MontureClass.Date,
                                                                                                                                                oper = memberuser.Username,
                                                                                                                                                dates = MontureClass.Dates,
                                                                                                                                                banque = "Caisse",
                                                                                                                                                nfact = MontureClass.Date + " " + MontureClass.RaisonClient,
                                                                                                                                                amontant = Convert.ToDecimal(MontureClass.MontantTotal),
                                                                                                                                                cle = MontureClass.Cle,
                                                                                                                                                cp = MontureClass.Id,
                                                                                                                                                Multiple = false,
                                                                                                                                                CodeClient = MontureClass.IdClient,
                                                                                                                                                RaisonClient = MontureClass.RaisonClient,

                                                                                                                                            };
                                                                                                                                            CAISSEMONTURE = new SVC.Depense
                                                                                                                                            {
                                                                                                                                                cle = MontureClass.Cle,
                                                                                                                                                Auto = true,
                                                                                                                                                Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                                                                                                                                                CompteDébité = "Caisse",
                                                                                                                                                Crédit = true,
                                                                                                                                                DateDebit = MontureClass.Date,
                                                                                                                                                DateSaisie = MontureClass.Dates,
                                                                                                                                                Débit = false,
                                                                                                                                                ModePaiement = "ESPECES",
                                                                                                                                                Montant = 0,
                                                                                                                                                MontantCrédit = MontureClass.Encaissé,
                                                                                                                                                NumCheque = Convert.ToString(MontureClass.Id),
                                                                                                                                                Num_Facture = MontureClass.Date + " " + MontureClass.RaisonClient,
                                                                                                                                                RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                                                                                                                                                Username = memberuser.Username,

                                                                                                                                            };
                                                                                                                                            interfacecreationmonture = 1;
                                                                                                                                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                                                                                                            {
                                                                                                                                                #region monturesupp
                                                                                                                                                if (listsupp1 != null)
                                                                                                                                                {
                                                                                                                                                    if (listsupp1.Count() > 0)
                                                                                                                                                    {
                                                                                                                                                        foreach (var item in listsupp1)
                                                                                                                                                        {
                                                                                                                                                            var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                                                                                                                            if (existe == false)
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture1 = false;
                                                                                                                                                                item.cle = MontureClass.Cle;
                                                                                                                                                                item.interfaceMonture = 1;
                                                                                                                                                                proxy.InsertMontureSupplement(item);
                                                                                                                                                                creationsuppmonture1 = true;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture1 = false;
                                                                                                                                                                //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                                proxy.UpdateMontureSupplement(item);
                                                                                                                                                                creationsuppmonture1 = true;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        creationsuppmonture1 = true;
                                                                                                                                                    }
                                                                                                                                                }

                                                                                                                                                if (listsupp2 != null)
                                                                                                                                                {
                                                                                                                                                    if (listsupp2.Count() > 0)
                                                                                                                                                    {
                                                                                                                                                        foreach (var item in listsupp2)
                                                                                                                                                        {
                                                                                                                                                            var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                                                                                                                            if (existe == false)
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture2 = false;
                                                                                                                                                                item.cle = MontureClass.Cle;
                                                                                                                                                                item.interfaceMonture = 2;
                                                                                                                                                                proxy.InsertMontureSupplement(item);
                                                                                                                                                                creationsuppmonture2 = true;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture2 = false;
                                                                                                                                                                //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                                proxy.UpdateMontureSupplement(item);
                                                                                                                                                                creationsuppmonture2 = true;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        creationsuppmonture2 = true;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                if (listsupp3 != null)
                                                                                                                                                {
                                                                                                                                                    if (listsupp3.Count() > 0)
                                                                                                                                                    {
                                                                                                                                                        foreach (var item in listsupp3)
                                                                                                                                                        {
                                                                                                                                                            var existe = anciennelistsupp3.Any(n => n.Id == item.Id);
                                                                                                                                                            if (existe == false)
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture3 = false;
                                                                                                                                                                item.cle = MontureClass.Cle;
                                                                                                                                                                item.interfaceMonture = 3;
                                                                                                                                                                proxy.InsertMontureSupplement(item);
                                                                                                                                                                creationsuppmonture3 = true;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture3 = false;
                                                                                                                                                                //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                                proxy.UpdateMontureSupplement(item);
                                                                                                                                                                creationsuppmonture3 = true;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        creationsuppmonture3 = true;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                if (listsupp4 != null)
                                                                                                                                                {
                                                                                                                                                    if (listsupp4.Count() > 0)
                                                                                                                                                    {
                                                                                                                                                        foreach (var item in listsupp4)
                                                                                                                                                        {
                                                                                                                                                            var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                                                                                                                            if (existe == false)
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture4 = false;
                                                                                                                                                                item.cle = MontureClass.Cle;
                                                                                                                                                                item.interfaceMonture = 4;
                                                                                                                                                                proxy.InsertMontureSupplement(item);
                                                                                                                                                                creationsuppmonture4 = true;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                creationsuppmonture4 = false;
                                                                                                                                                                //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                                proxy.UpdateMontureSupplement(item);
                                                                                                                                                                creationsuppmonture4 = true;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        creationsuppmonture4 = true;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                #endregion
                                                                                                                                                proxy.UpdateMonture(MontureClass);
                                                                                                                                                creationmonture = true;


                                                                                                                                                proxy.InsertDepense(CAISSEMONTURE);
                                                                                                                                                creationcaisse = true;
                                                                                                                                                proxy.InsertDepeiment(PAIEMENTMONTURE);
                                                                                                                                                creationdepaiement = true;

                                                                                                                                                if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                                {
                                                                                                                                                    ts.Complete();
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                            {
                                                                                                                                                proxy.AjouterTransactionPaiementRefresh();
                                                                                                                                                proxy.AjouterDepenseRefresh();
                                                                                                                                                proxy.AjouterMontureRefresh(Clientvv.Id);
                                                                                                                                                GridMonture.IsEnabled = false;
                                                                                                                                                GridMonture.DataContext = null;
                                                                                                                                                montureversementzero = false;

                                                                                                                                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        { /* MontureClass */
                                                                                                                           /*   if (MontureClass.Encaissé == 0)
                                                                                                                              {
                                                                                                                                  using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                                                                                                  {
                                                                                                                                      #region monturesupp

                                                                                                                                      if (listsupp1 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp1.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp1)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture1 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 1;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture1 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture1 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture1 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture1 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      else
                                                                                                                                      {
                                                                                                                                          creationsuppmonture1 = true;

                                                                                                                                      }

                                                                                                                                      if (listsupp2 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp2.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp2)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture2 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 2;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture2 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture2 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture2 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture2 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      else
                                                                                                                                      {
                                                                                                                                          creationsuppmonture2 = true;

                                                                                                                                      }
                                                                                                                                      if (listsupp3 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp3.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp3)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp3.Any(n => n.Id == item.Id);

                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture3 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 3;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture3 = true;

                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture3 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture3 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture3 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      else
                                                                                                                                      {
                                                                                                                                          creationsuppmonture3 = true;

                                                                                                                                      }
                                                                                                                                      if (listsupp4 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp4.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp4)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture4 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 4;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture4 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture4 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture4 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture4 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      else
                                                                                                                                      {
                                                                                                                                          creationsuppmonture4 = true;

                                                                                                                                      }
                                                                                                                                      #endregion

                                                                                                                                      proxy.UpdateMonture(MontureClass);
                                                                                                                                      creationmonture = true;
                                                                                                                                      if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                      {
                                                                                                                                          ts.Complete();
                                                                                                                                      }
                                                                                                                                  }
                                                                                                                                  if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                  {
                                                                                                                                      proxy.AjouterTransactionPaiementRefresh();
                                                                                                                                      proxy.AjouterDepenseRefresh();
                                                                                                                                      proxy.AjouterMontureRefresh(Clientvv.Id);
                                                                                                                                      GridMonture.IsEnabled = false;
                                                                                                                                      GridMonture.DataContext = null;
                                                                                                                                      montureversementzero = false;
                                                                                                                                      MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                                                                                                                  }
                                                                                                                              }

                                                                                                                          }
                                                                                                                      }
                                                                                                                      else
                                                                                                                      {
                                                                                                                          if (montureversementzero == true)
                                                                                                                          {
                                                                                                                              //lors du chargement montureclass!=0 apres montureclass==0 
                                                                                                                              //suppression des ecritures dans depense et depaiement
                                                                                                                              if (MontureClass.Encaissé == 0)
                                                                                                                              {
                                                                                                                                  CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == MontureClass.Cle).First();
                                                                                                                                  PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == MontureClass.Cle).First();
                                                                                                                                  using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                                                                                                  {

                                                                                                                                      proxy.UpdateMonture(MontureClass);
                                                                                                                                      creationmonture = true;
                                                                                                                                      proxy.DeleteDepense(CAISSEMONTURE);
                                                                                                                                      creationcaisse = true;
                                                                                                                                      proxy.DeleteDepeiment(PAIEMENTMONTURE);
                                                                                                                                      creationdepaiement = true;
                                                                                                                                      #region monturesupplement
                                                                                                                                      if (listsupp1 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp1.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp1)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture1 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 1;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture1 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture1 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture1 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture1 = true;
                                                                                                                                          }
                                                                                                                                      }

                                                                                                                                      if (listsupp2 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp2.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp2)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture2 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 2;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture2 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture2 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture2 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture2 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      if (listsupp3 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp3.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp3)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp3.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture3 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 3;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture3 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture3 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture3 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture3 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      if (listsupp4 != null)
                                                                                                                                      {
                                                                                                                                          if (listsupp4.Count() > 0)
                                                                                                                                          {
                                                                                                                                              foreach (var item in listsupp4)
                                                                                                                                              {
                                                                                                                                                  var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                                                                                                                  if (existe == false)
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture4 = false;
                                                                                                                                                      item.cle = MontureClass.Cle;
                                                                                                                                                      item.interfaceMonture = 4;
                                                                                                                                                      proxy.InsertMontureSupplement(item);
                                                                                                                                                      creationsuppmonture4 = true;
                                                                                                                                                  }
                                                                                                                                                  else
                                                                                                                                                  {
                                                                                                                                                      creationsuppmonture4 = false;
                                                                                                                                                      //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                      proxy.UpdateMontureSupplement(item);
                                                                                                                                                      creationsuppmonture4 = true;
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          else
                                                                                                                                          {
                                                                                                                                              creationsuppmonture4 = true;
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      #endregion
                                                                                                                                      if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                      {
                                                                                                                                          ts.Complete();
                                                                                                                                      }
                                                                                                                                  }
                                                                                                                                  if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                  {
                                                                                                                                      proxy.AjouterTransactionPaiementRefresh();
                                                                                                                                      proxy.AjouterDepenseRefresh();
                                                                                                                                      proxy.AjouterMontureRefresh(Clientvv.Id);
                                                                                                                                      GridMonture.IsEnabled = false;
                                                                                                                                      GridMonture.DataContext = null;
                                                                                                                                      montureversementzero = false;
                                                                                                                                      MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                                                                                                                  }
                                                                                                                              }
                                                                                                                              else
                                                                                                                              {
                                                                                                                                  //lors du chargement montureclass!=0 apres montureclass!==0 
                                                                                                                                  //alors mise à jours des écritures dans depense et depaiement
                                                                                                                                  if (MontureClass.Encaissé != 0)
                                                                                                                                  {
                                                                                                                                      CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == MontureClass.Cle).First();
                                                                                                                                      CAISSEMONTURE.MontantCrédit = MontureClass.Encaissé;
                                                                                                                                      PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == MontureClass.Cle).First();
                                                                                                                                      PAIEMENTMONTURE.montant = Convert.ToDecimal(MontureClass.Encaissé);
                                                                                                                                      using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                                                                                                      {
                                                                                                                                          #region monturesupplement
                                                                                                                                          if (listsupp1 != null)
                                                                                                                                          {
                                                                                                                                              if (listsupp1.Count() > 0)
                                                                                                                                              {
                                                                                                                                                  foreach (var item in listsupp1)
                                                                                                                                                  {
                                                                                                                                                      var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                                                                                                                      if (existe == false)
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture1 = false;
                                                                                                                                                          item.cle = MontureClass.Cle;
                                                                                                                                                          item.interfaceMonture = 1;
                                                                                                                                                          proxy.InsertMontureSupplement(item);
                                                                                                                                                          creationsuppmonture1 = true;
                                                                                                                                                      }
                                                                                                                                                      else
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture1 = false;
                                                                                                                                                          //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                          proxy.UpdateMontureSupplement(item);
                                                                                                                                                          creationsuppmonture1 = true;
                                                                                                                                                      }
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                              else
                                                                                                                                              {
                                                                                                                                                  creationsuppmonture1 = true;
                                                                                                                                              }
                                                                                                                                          }

                                                                                                                                          if (listsupp2 != null)
                                                                                                                                          {
                                                                                                                                              if (listsupp2.Count() > 0)
                                                                                                                                              {
                                                                                                                                                  foreach (var item in listsupp2)
                                                                                                                                                  {
                                                                                                                                                      var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                                                                                                                      if (existe == false)
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture2 = false;
                                                                                                                                                          item.cle = MontureClass.Cle;
                                                                                                                                                          item.interfaceMonture = 2;
                                                                                                                                                          proxy.InsertMontureSupplement(item);
                                                                                                                                                          creationsuppmonture2 = true;
                                                                                                                                                      }
                                                                                                                                                      else
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture2 = false;
                                                                                                                                                          //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                          proxy.UpdateMontureSupplement(item);
                                                                                                                                                          creationsuppmonture2 = true;
                                                                                                                                                      }
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                              else
                                                                                                                                              {
                                                                                                                                                  creationsuppmonture2 = true;
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          if (listsupp3 != null)
                                                                                                                                          {
                                                                                                                                              if (listsupp3.Count() > 0)
                                                                                                                                              {
                                                                                                                                                  foreach (var item in listsupp3)
                                                                                                                                                  {
                                                                                                                                                      var existe = anciennelistsupp3.Any(n => n.Id == item.Id);
                                                                                                                                                      if (existe == false)
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture3 = false;
                                                                                                                                                          item.cle = MontureClass.Cle;
                                                                                                                                                          item.interfaceMonture = 3;
                                                                                                                                                          proxy.InsertMontureSupplement(item);
                                                                                                                                                          creationsuppmonture3 = true;
                                                                                                                                                      }
                                                                                                                                                      else
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture3 = false;
                                                                                                                                                          //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                          proxy.UpdateMontureSupplement(item);
                                                                                                                                                          creationsuppmonture3 = true;
                                                                                                                                                      }
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                              else
                                                                                                                                              {
                                                                                                                                                  creationsuppmonture3 = true;
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          if (listsupp4 != null)
                                                                                                                                          {
                                                                                                                                              if (listsupp4.Count() > 0)
                                                                                                                                              {
                                                                                                                                                  foreach (var item in listsupp4)
                                                                                                                                                  {
                                                                                                                                                      var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                                                                                                                      if (existe == false)
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture4 = false;
                                                                                                                                                          item.cle = MontureClass.Cle;
                                                                                                                                                          item.interfaceMonture = 4;
                                                                                                                                                          proxy.InsertMontureSupplement(item);
                                                                                                                                                          creationsuppmonture4 = true;
                                                                                                                                                      }
                                                                                                                                                      else
                                                                                                                                                      {
                                                                                                                                                          creationsuppmonture4 = false;
                                                                                                                                                          //item.CleSupplement = Supplement.VerresAssociés;
                                                                                                                                                          proxy.UpdateMontureSupplement(item);
                                                                                                                                                          creationsuppmonture4 = true;
                                                                                                                                                      }
                                                                                                                                                  }
                                                                                                                                              }
                                                                                                                                              else
                                                                                                                                              {
                                                                                                                                                  creationsuppmonture4 = true;
                                                                                                                                              }
                                                                                                                                          }
                                                                                                                                          #endregion
                                                                                                                                          proxy.UpdateMonture(MontureClass);
                                                                                                                                          creationmonture = true;


                                                                                                                                          proxy.UpdateDepense(CAISSEMONTURE);
                                                                                                                                          creationcaisse = true;
                                                                                                                                          proxy.UpdateDepeiment(PAIEMENTMONTURE);
                                                                                                                                          creationdepaiement = true;

                                                                                                                                          if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                          {
                                                                                                                                              ts.Complete();
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                      if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                                                                                                                      {
                                                                                                                                          proxy.AjouterTransactionPaiementRefresh();
                                                                                                                                          proxy.AjouterDepenseRefresh();
                                                                                                                                          proxy.AjouterMontureRefresh(Clientvv.Id);
                                                                                                                                          GridMonture.IsEnabled = false;
                                                                                                                                          GridMonture.DataContext = null;
                                                                                                                                          montureversementzero = false;
                                                                                                                                          MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

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
                                                                                                      */


        private void ValiderMonture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false && memberuser.CreationDossierClient == true)
                {

                    int interfacecreationmonture = 0;
                    bool creationmonture, creationcaisse, creationdepaiement = false;
                    bool creationsuppmonture1 = false;
                    bool creationsuppmonture2 = false;
                    bool creationsuppmonture3 = false;
                    bool creationsuppmonture4 = false;
                    MontureClass.StatutDevis = true;
                    MontureClass.StatutVente = false;
                    string _numbers = Convert.ToString(MontureClass.IdClient) + Convert.ToString(MontureClass.MontantTotal) + Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month) + Convert.ToString(DateTime.Now.Day) + Convert.ToString(DateTime.Now.Hour) + Convert.ToString(DateTime.Now.Minute) + Convert.ToString(DateTime.Now.Second) + Convert.ToString(DateTime.Now.Millisecond);

                    //    MessageBoxResult resul0333 = Xceed.Wpf.Toolkit.MessageBox.Show(Convert.ToString(DoWork(_numbers)), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    MontureClass.NCommande = Convert.ToString(DoWork(_numbers).ToString());

                    SVC.Depense CAISSEMONTURE;
                    SVC.Depeiment PAIEMENTMONTURE;
                    if (txtMontantTotalENC.Text != "")
                    {
                        if (Convert.ToDecimal(txtMontantTotalENC.Text) != 0)
                        {
                            MontureClass.Encaissé = Convert.ToDecimal(txtMontantTotalENC.Text);
                            MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                        }
                        else
                        {
                            MontureClass.Encaissé = 0;
                            MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                        }
                    }
                    else
                    {
                        MontureClass.Encaissé = 0;
                        MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                    }
                    MontureClass.Cle = Clientvv.Id + Clientvv.Raison + MontureClass.MontantTotal + DateTime.Now.TimeOfDay;
                    if (txtAccessoiresQuantite1.Text != "")
                    {
                        if (Convert.ToDecimal(txtAccessoiresQuantite1.Text) == 0)
                        {
                            MontureClass.Accessoires1 = "";
                            MontureClass.AccessoiresPrix1 = null;
                            txtAccessoires1.Text = "";
                            txtAccessoiresPrix1.Text = "0";
                        }
                    }
                    if (txtAccessoiresQuantite2.Text != "")
                    {
                        if (Convert.ToDecimal(txtAccessoiresQuantite2.Text) == 0)
                        {
                            MontureClass.Accessoires2 = "";
                            MontureClass.AccessoiresPrix2 = null;
                            txtAccessoires2.Text = "";
                            txtAccessoiresPrix2.Text = "0";
                        }
                    }
                    if (MontureClass.Encaissé != 0)
                    {

                        PAIEMENTMONTURE = new SVC.Depeiment
                        {
                            date = MontureClass.Date,
                            montant = Convert.ToDecimal(MontureClass.Encaissé),
                            paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " date :" + MontureClass.Date,
                            oper = memberuser.Username,
                            dates = MontureClass.Dates,
                            banque = "Caisse",
                            nfact = MontureClass.Date + " " + MontureClass.RaisonClient,
                            amontant = Convert.ToDecimal(MontureClass.MontantTotal),
                            cle = MontureClass.Cle,
                            cp = MontureClass.Id,
                            Multiple = false,
                            CodeClient = MontureClass.IdClient,
                            RaisonClient = MontureClass.RaisonClient,

                        };
                        CAISSEMONTURE = new SVC.Depense
                        {
                            cle = MontureClass.Cle,
                            Auto = true,
                            Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                            CompteDébité = "Caisse",
                            Crédit = true,
                            DateDebit = MontureClass.Date,
                            DateSaisie = MontureClass.Dates,
                            Débit = false,
                            ModePaiement = "ESPECES",
                            Montant = 0,
                            MontantCrédit = MontureClass.Encaissé,
                            NumCheque = Convert.ToString(MontureClass.Id),
                            Num_Facture = MontureClass.Date + " " + MontureClass.RaisonClient,
                            RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                            Username = memberuser.Username,

                        };
                        interfacecreationmonture = 1;



                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (listsupp1 != null)
                            {
                                if (listsupp1.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp1)
                                    {
                                        creationsuppmonture1 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 1;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture1 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture1 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture1 = true;
                            }
                            if (listsupp2 != null)
                            {
                                if (listsupp2.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp2)
                                    {
                                        creationsuppmonture1 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 2;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture1 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture2 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture2 = true;
                            }
                            if (listsupp3 != null)
                            {
                                if (listsupp3.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp3)
                                    {
                                        creationsuppmonture3 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 3;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture3 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture3 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture3 = true;
                            }
                            if (listsupp4 != null)
                            {
                                if (listsupp4.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp4)
                                    {
                                        creationsuppmonture4 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 4;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture4 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture4 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture4 = true;
                            }
                            proxy.InsertMonture(MontureClass);
                            creationmonture = true;


                            proxy.InsertDepense(CAISSEMONTURE);
                            creationcaisse = true;
                            proxy.InsertDepeiment(PAIEMENTMONTURE);
                            creationdepaiement = true;

                            if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                            {
                                ts.Complete();
                            }
                        }
                        if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                        {
                            proxy.AjouterTransactionPaiementRefresh();
                            proxy.AjouterDepenseRefresh();
                            proxy.AjouterMontureRefresh(Clientvv.Id);
                            proxy.AjouterMontureSupplementRefresh(MontureClass.Cle);
                            GridMonture.IsEnabled = false;
                            GridMonture.DataContext = null;
                            montureversementzero = false;
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                    else
                    {
                        // MessageBoxResult resul03s = Xceed.Wpf.Toolkit.MessageBox.Show("Im here", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            if (listsupp1 != null)
                            {
                                if (listsupp1.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp1)
                                    {
                                        creationsuppmonture1 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 1;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture1 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture1 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture1 = true;
                            }
                            if (listsupp2 != null)
                            {
                                if (listsupp2.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp2)
                                    {
                                        creationsuppmonture1 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 2;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture1 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture2 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture2 = true;
                            }
                            if (listsupp3 != null)
                            {
                                if (listsupp3.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp3)
                                    {
                                        creationsuppmonture3 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 3;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture3 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture3 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture3 = true;
                            }
                            if (listsupp4 != null)
                            {
                                if (listsupp4.Count > 0)
                                {
                                    foreach (var itemsupp in listsupp4)
                                    {
                                        creationsuppmonture4 = false;
                                        itemsupp.cle = MontureClass.Cle;
                                        itemsupp.interfaceMonture = 4;
                                        proxy.InsertMontureSupplement(itemsupp);
                                        creationsuppmonture4 = true;
                                    }
                                }
                                else
                                {
                                    creationsuppmonture4 = true;
                                }
                            }
                            else
                            {
                                creationsuppmonture4 = true;
                            }
                            proxy.InsertMonture(MontureClass);
                            creationmonture = true;

                            if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                            {
                                ts.Complete();
                            }

                        }
                        if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                        {
                            proxy.AjouterTransactionPaiementRefresh();
                            proxy.AjouterDepenseRefresh();
                            proxy.AjouterMontureRefresh(Clientvv.Id);
                            proxy.AjouterMontureSupplementRefresh(MontureClass.Cle);
                            GridMonture.IsEnabled = false;
                            GridMonture.DataContext = null;
                            montureversementzero = false;
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }


                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && memberuser.ModificationDossierClient == true)
                    {
                        int interfacecreationmonture = 0;
                        bool creationmonture, creationcaisse, creationdepaiement = false;
                        bool creationsuppmonture1 = false;
                        bool creationsuppmonture2 = false;
                        bool creationsuppmonture3 = false;
                        bool creationsuppmonture4 = false;
                        // MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotalENC.Text);
                        SVC.Depense CAISSEMONTURE;
                        SVC.Depeiment PAIEMENTMONTURE;


                        if (txtMontantTotalENC.Text != "")
                        {
                            if (Convert.ToDecimal(txtMontantTotalENC.Text) != 0)
                            {
                                MontureClass.Encaissé = Convert.ToDecimal(txtMontantTotalENC.Text);
                                MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                            }
                            else
                            {
                                MontureClass.Encaissé = 0;
                                MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                            }

                        }
                        else
                        {
                            MontureClass.Encaissé = 0;
                            MontureClass.Reste = MontureClass.MontantTotal - MontureClass.Encaissé;
                        }
                        if (txtAccessoiresQuantite1.Text != "")
                        {
                            if (Convert.ToDecimal(txtAccessoiresQuantite1.Text) == 0)
                            {
                                MontureClass.Accessoires1 = "";
                                MontureClass.AccessoiresPrix1 = null;
                                txtAccessoires1.Text = "";
                                txtAccessoiresPrix1.Text = "0";
                            }
                        }
                        if (txtAccessoiresQuantite2.Text != "")
                        {
                            if (Convert.ToDecimal(txtAccessoiresQuantite2.Text) == 0)
                            {
                                MontureClass.Accessoires2 = "";
                                MontureClass.AccessoiresPrix2 = null;
                                txtAccessoires2.Text = "";
                                txtAccessoiresPrix2.Text = "0";
                            }
                        }
                        ///MontureCLass ==0 lors du chargement
                        if (montureversementzero == false)
                        {
                            //nouveau encaissement alors insertion
                            if (MontureClass.Encaissé != 0)
                            {
                                PAIEMENTMONTURE = new SVC.Depeiment
                                {
                                    date = MontureClass.Date,
                                    montant = Convert.ToDecimal(MontureClass.Encaissé),
                                    paiem = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " date :" + MontureClass.Date,
                                    oper = memberuser.Username,
                                    dates = MontureClass.Dates,
                                    banque = "Caisse",
                                    nfact = MontureClass.Date + " " + MontureClass.RaisonClient,
                                    amontant = Convert.ToDecimal(MontureClass.MontantTotal),
                                    cle = MontureClass.Cle,
                                    cp = MontureClass.Id,
                                    Multiple = false,
                                    CodeClient = MontureClass.IdClient,
                                    RaisonClient = MontureClass.RaisonClient,

                                };
                                CAISSEMONTURE = new SVC.Depense
                                {
                                    cle = MontureClass.Cle,
                                    Auto = true,
                                    Commentaires = "ESPECES" + " VERSEMENT SUR :" + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                                    CompteDébité = "Caisse",
                                    Crédit = true,
                                    DateDebit = MontureClass.Date,
                                    DateSaisie = MontureClass.Dates,
                                    Débit = false,
                                    ModePaiement = "ESPECES",
                                    Montant = 0,
                                    MontantCrédit = MontureClass.Encaissé,
                                    NumCheque = Convert.ToString(MontureClass.Id),
                                    Num_Facture = MontureClass.Date + " " + MontureClass.RaisonClient,
                                    RubriqueComptable = "ESPECES VERSEMENT SUR: " + "Ordonnance optique" + " " + MontureClass.RaisonClient + " " + " date :" + MontureClass.Date,
                                    Username = memberuser.Username,

                                };
                                interfacecreationmonture = 1;
                                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    #region monturesupp
                                    if (listsupp1 != null)
                                    {
                                        if (listsupp1.Count() > 0)
                                        {
                                            foreach (var item in listsupp1)
                                            {
                                                var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                if (existe == false)
                                                {
                                                    creationsuppmonture1 = false;
                                                    item.cle = MontureClass.Cle;
                                                    item.interfaceMonture = 1;
                                                    proxy.InsertMontureSupplement(item);
                                                    creationsuppmonture1 = true;
                                                }
                                                else
                                                {
                                                    creationsuppmonture1 = false;
                                                    //item.CleSupplement = Supplement.VerresAssociés;
                                                    proxy.UpdateMontureSupplement(item);
                                                    creationsuppmonture1 = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture1 = true;
                                        }
                                    }

                                    if (listsupp2 != null)
                                    {
                                        if (listsupp2.Count() > 0)
                                        {
                                            foreach (var item in listsupp2)
                                            {
                                                var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                if (existe == false)
                                                {
                                                    creationsuppmonture2 = false;
                                                    item.cle = MontureClass.Cle;
                                                    item.interfaceMonture = 2;
                                                    proxy.InsertMontureSupplement(item);
                                                    creationsuppmonture2 = true;
                                                }
                                                else
                                                {
                                                    creationsuppmonture2 = false;
                                                    //item.CleSupplement = Supplement.VerresAssociés;
                                                    proxy.UpdateMontureSupplement(item);
                                                    creationsuppmonture2 = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture2 = true;
                                        }
                                    }
                                    if (listsupp3 != null)
                                    {
                                        if (listsupp3.Count() > 0)
                                        {
                                            foreach (var item in listsupp3)
                                            {
                                                var existe = anciennelistsupp3.Any(n => n.Id == item.Id);
                                                if (existe == false)
                                                {
                                                    creationsuppmonture3 = false;
                                                    item.cle = MontureClass.Cle;
                                                    item.interfaceMonture = 3;
                                                    proxy.InsertMontureSupplement(item);
                                                    creationsuppmonture3 = true;
                                                }
                                                else
                                                {
                                                    creationsuppmonture3 = false;
                                                    //item.CleSupplement = Supplement.VerresAssociés;
                                                    proxy.UpdateMontureSupplement(item);
                                                    creationsuppmonture3 = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture3 = true;
                                        }
                                    }
                                    if (listsupp4 != null)
                                    {
                                        if (listsupp4.Count() > 0)
                                        {
                                            foreach (var item in listsupp4)
                                            {
                                                var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                if (existe == false)
                                                {
                                                    creationsuppmonture4 = false;
                                                    item.cle = MontureClass.Cle;
                                                    item.interfaceMonture = 4;
                                                    proxy.InsertMontureSupplement(item);
                                                    creationsuppmonture4 = true;
                                                }
                                                else
                                                {
                                                    creationsuppmonture4 = false;
                                                    //item.CleSupplement = Supplement.VerresAssociés;
                                                    proxy.UpdateMontureSupplement(item);
                                                    creationsuppmonture4 = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture4 = true;
                                        }
                                    }
                                    #endregion
                                    proxy.UpdateMonture(MontureClass);
                                    creationmonture = true;


                                    proxy.InsertDepense(CAISSEMONTURE);
                                    creationcaisse = true;
                                    proxy.InsertDepeiment(PAIEMENTMONTURE);
                                    creationdepaiement = true;

                                    if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                    {
                                        ts.Complete();
                                    }
                                }
                                if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                {
                                    proxy.AjouterTransactionPaiementRefresh();
                                    proxy.AjouterDepenseRefresh();
                                    proxy.AjouterMontureRefresh(Clientvv.Id);
                                    GridMonture.IsEnabled = false;
                                    GridMonture.DataContext = null;
                                    montureversementzero = false;

                                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }
                            else
                            {
                                if (MontureClass.Encaissé == 0)
                                {
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        #region monturesupp

                                        if (listsupp1 != null)
                                        {
                                            if (listsupp1.Count() > 0)
                                            {
                                                foreach (var item in listsupp1)
                                                {
                                                    var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture1 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 1;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture1 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture1 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture1 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture1 = true;
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture1 = true;

                                        }

                                        if (listsupp2 != null)
                                        {
                                            if (listsupp2.Count() > 0)
                                            {
                                                foreach (var item in listsupp2)
                                                {
                                                    var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture2 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 2;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture2 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture2 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture2 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture2 = true;
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture2 = true;

                                        }
                                        if (listsupp3 != null)
                                        {
                                            if (listsupp3.Count() > 0)
                                            {
                                                foreach (var item in listsupp3)
                                                {
                                                    var existe = anciennelistsupp3.Any(n => n.Id == item.Id);

                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture3 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 3;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture3 = true;

                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture3 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture3 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture3 = true;
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture3 = true;

                                        }
                                        if (listsupp4 != null)
                                        {
                                            if (listsupp4.Count() > 0)
                                            {
                                                foreach (var item in listsupp4)
                                                {
                                                    var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture4 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 4;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture4 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture4 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture4 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture4 = true;
                                            }
                                        }
                                        else
                                        {
                                            creationsuppmonture4 = true;

                                        }
                                        #endregion

                                        proxy.UpdateMonture(MontureClass);
                                        creationmonture = true;
                                        if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                        {
                                            ts.Complete();
                                        }
                                    }
                                    if (creationmonture == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                    {
                                        proxy.AjouterTransactionPaiementRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        proxy.AjouterMontureRefresh(Clientvv.Id);
                                        GridMonture.IsEnabled = false;
                                        GridMonture.DataContext = null;
                                        montureversementzero = false;
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (montureversementzero == true)
                            {
                                //lors du chargement montureclass!=0 apres montureclass==0 
                                //suppression des ecritures dans depense et depaiement
                                if (MontureClass.Encaissé == 0)
                                {
                                    CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == MontureClass.Cle).First();
                                    PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == MontureClass.Cle).First();
                                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {

                                        proxy.UpdateMonture(MontureClass);
                                        creationmonture = true;
                                        proxy.DeleteDepense(CAISSEMONTURE);
                                        creationcaisse = true;
                                        proxy.DeleteDepeiment(PAIEMENTMONTURE);
                                        creationdepaiement = true;
                                        #region monturesupplement
                                        if (listsupp1 != null)
                                        {
                                            if (listsupp1.Count() > 0)
                                            {
                                                foreach (var item in listsupp1)
                                                {
                                                    var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture1 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 1;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture1 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture1 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture1 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture1 = true;
                                            }
                                        }

                                        if (listsupp2 != null)
                                        {
                                            if (listsupp2.Count() > 0)
                                            {
                                                foreach (var item in listsupp2)
                                                {
                                                    var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture2 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 2;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture2 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture2 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture2 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture2 = true;
                                            }
                                        }
                                        if (listsupp3 != null)
                                        {
                                            if (listsupp3.Count() > 0)
                                            {
                                                foreach (var item in listsupp3)
                                                {
                                                    var existe = anciennelistsupp3.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture3 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 3;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture3 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture3 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture3 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture3 = true;
                                            }
                                        }
                                        if (listsupp4 != null)
                                        {
                                            if (listsupp4.Count() > 0)
                                            {
                                                foreach (var item in listsupp4)
                                                {
                                                    var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                    if (existe == false)
                                                    {
                                                        creationsuppmonture4 = false;
                                                        item.cle = MontureClass.Cle;
                                                        item.interfaceMonture = 4;
                                                        proxy.InsertMontureSupplement(item);
                                                        creationsuppmonture4 = true;
                                                    }
                                                    else
                                                    {
                                                        creationsuppmonture4 = false;
                                                        //item.CleSupplement = Supplement.VerresAssociés;
                                                        proxy.UpdateMontureSupplement(item);
                                                        creationsuppmonture4 = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                creationsuppmonture4 = true;
                                            }
                                        }
                                        #endregion
                                        if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                        {
                                            ts.Complete();
                                        }
                                    }
                                    if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                    {
                                        proxy.AjouterTransactionPaiementRefresh();
                                        proxy.AjouterDepenseRefresh();
                                        proxy.AjouterMontureRefresh(Clientvv.Id);
                                        GridMonture.IsEnabled = false;
                                        GridMonture.DataContext = null;
                                        montureversementzero = false;
                                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                }
                                else
                                {
                                    //lors du chargement montureclass!=0 apres montureclass!==0 
                                    //alors mise à jours des écritures dans depense et depaiement
                                    if (MontureClass.Encaissé != 0)
                                    {
                                        CAISSEMONTURE = proxy.GetAllDepense().Where(n => n.cle == MontureClass.Cle).First();
                                        CAISSEMONTURE.MontantCrédit = MontureClass.Encaissé;
                                        PAIEMENTMONTURE = proxy.GetAllDepeiment().Where(n => n.cle == MontureClass.Cle).First();
                                        PAIEMENTMONTURE.montant = Convert.ToDecimal(MontureClass.Encaissé);
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            #region monturesupplement
                                            if (listsupp1 != null)
                                            {
                                                if (listsupp1.Count() > 0)
                                                {
                                                    foreach (var item in listsupp1)
                                                    {
                                                        var existe = anciennelistsupp1.Any(n => n.Id == item.Id);
                                                        if (existe == false)
                                                        {
                                                            creationsuppmonture1 = false;
                                                            item.cle = MontureClass.Cle;
                                                            item.interfaceMonture = 1;
                                                            proxy.InsertMontureSupplement(item);
                                                            creationsuppmonture1 = true;
                                                        }
                                                        else
                                                        {
                                                            creationsuppmonture1 = false;
                                                            //item.CleSupplement = Supplement.VerresAssociés;
                                                            proxy.UpdateMontureSupplement(item);
                                                            creationsuppmonture1 = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    creationsuppmonture1 = true;
                                                }
                                            }

                                            if (listsupp2 != null)
                                            {
                                                if (listsupp2.Count() > 0)
                                                {
                                                    foreach (var item in listsupp2)
                                                    {
                                                        var existe = anciennelistsupp2.Any(n => n.Id == item.Id);
                                                        if (existe == false)
                                                        {
                                                            creationsuppmonture2 = false;
                                                            item.cle = MontureClass.Cle;
                                                            item.interfaceMonture = 2;
                                                            proxy.InsertMontureSupplement(item);
                                                            creationsuppmonture2 = true;
                                                        }
                                                        else
                                                        {
                                                            creationsuppmonture2 = false;
                                                            //item.CleSupplement = Supplement.VerresAssociés;
                                                            proxy.UpdateMontureSupplement(item);
                                                            creationsuppmonture2 = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    creationsuppmonture2 = true;
                                                }
                                            }
                                            if (listsupp3 != null)
                                            {
                                                if (listsupp3.Count() > 0)
                                                {
                                                    foreach (var item in listsupp3)
                                                    {
                                                        var existe = anciennelistsupp3.Any(n => n.Id == item.Id);
                                                        if (existe == false)
                                                        {
                                                            creationsuppmonture3 = false;
                                                            item.cle = MontureClass.Cle;
                                                            item.interfaceMonture = 3;
                                                            proxy.InsertMontureSupplement(item);
                                                            creationsuppmonture3 = true;
                                                        }
                                                        else
                                                        {
                                                            creationsuppmonture3 = false;
                                                            //item.CleSupplement = Supplement.VerresAssociés;
                                                            proxy.UpdateMontureSupplement(item);
                                                            creationsuppmonture3 = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    creationsuppmonture3 = true;
                                                }
                                            }
                                            if (listsupp4 != null)
                                            {
                                                if (listsupp4.Count() > 0)
                                                {
                                                    foreach (var item in listsupp4)
                                                    {
                                                        var existe = anciennelistsupp4.Any(n => n.Id == item.Id);
                                                        if (existe == false)
                                                        {
                                                            creationsuppmonture4 = false;
                                                            item.cle = MontureClass.Cle;
                                                            item.interfaceMonture = 4;
                                                            proxy.InsertMontureSupplement(item);
                                                            creationsuppmonture4 = true;
                                                        }
                                                        else
                                                        {
                                                            creationsuppmonture4 = false;
                                                            //item.CleSupplement = Supplement.VerresAssociés;
                                                            proxy.UpdateMontureSupplement(item);
                                                            creationsuppmonture4 = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    creationsuppmonture4 = true;
                                                }
                                            }
                                            #endregion
                                            proxy.UpdateMonture(MontureClass);
                                            creationmonture = true;


                                            proxy.UpdateDepense(CAISSEMONTURE);
                                            creationcaisse = true;
                                            proxy.UpdateDepeiment(PAIEMENTMONTURE);
                                            creationdepaiement = true;

                                            if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                            {
                                                ts.Complete();
                                            }
                                        }
                                        if (creationcaisse == true && creationmonture == true && creationdepaiement == true && creationsuppmonture1 == true && creationsuppmonture2 == true && creationsuppmonture3 == true && creationsuppmonture4 == true)
                                        {
                                            proxy.AjouterTransactionPaiementRefresh();
                                            proxy.AjouterDepenseRefresh();
                                            proxy.AjouterMontureRefresh(Clientvv.Id);
                                            GridMonture.IsEnabled = false;
                                            GridMonture.DataContext = null;
                                            montureversementzero = false;
                                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

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

        void montureexiste(SVC.Monture MontureClass)
        {
            try
            {
                if (MontureDatagrid.SelectedItem != null && memberuser.ModificationDossierClient == true)
                {
                    MontureClass = MontureDatagrid.SelectedItem as SVC.Monture;
                    GridMonture.DataContext = MontureClass;
                    GridMonture.IsEnabled = true;
                    nouvellemonture = false;
                    anciennemonture = true;
                    if (MontureClass.StatutVente == false)
                    {
                         txtMontantTotalENC.IsEnabled = true;
                    }
                    else
                    {
                         txtMontantTotalENC.IsEnabled = false;
                    }

                    if (MontureClass.StatutDevis == true && MontureClass.StatutVente == false)
                    {
                        TxtStatutGlobal.Content = "Devis";

                        TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (MontureClass.StatutDevis == true && MontureClass.StatutVente == true)
                        {
                            TxtStatutGlobal.Content = "Vente validée";
                            TxtStatutGlobal.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }


                    if (MontureClass.DroiteFleshHaut == true)
                    {
                        txtDroiteFleshHaut.Visibility = Visibility.Visible;
                        DroiteFleshBas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtDroiteFleshHaut.Visibility = Visibility.Collapsed;
                        DroiteFleshBas.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.DroiteFleshDroite == true)
                    {
                        txtDroiteFleshDroite.Visibility = Visibility.Visible;
                        txtDroiteFleshGauche.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtDroiteFleshDroite.Visibility = Visibility.Collapsed;
                        txtDroiteFleshGauche.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.GaucheFleshHaut == true)
                    {
                        txtGaucheFleshHaut.Visibility = Visibility.Visible;
                        txtGaucheFleshBas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtGaucheFleshHaut.Visibility = Visibility.Collapsed;
                        txtGaucheFleshBas.Visibility = Visibility.Visible;
                    }

                    if (MontureClass.GaucheFleshDroite == true)
                    {
                        txtGaucheFleshDroite.Visibility = Visibility.Visible;
                        txtGaucheFleshGauche.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtGaucheFleshDroite.Visibility = Visibility.Collapsed;
                        txtGaucheFleshGauche.Visibility = Visibility.Visible;
                    }
                    /**************************************************************/
                    if (MontureClass.Loin == true && MontureClass.Pres == true)
                    {

                        ODLOIN.Text = "Loin";
                        ODITERM.Text = "Interm.";
                        ODPRES.Text = "Près";
                        OGLOIN.Text = "Loin";
                        OGINTERM.Text = "Interm.";
                        OGPRES.Text = "Près";
                        MonturePresLabel.Text = "PRES";
                        MontureLoinLabel.Text = "LOIN";


                    }
                    else
                    {
                        if (MontureClass.Loin == true && MontureClass.Pres == false)
                        {


                            ODLOIN.Text = "Loin";
                            ODITERM.Text = "Loin";
                            ODPRES.Text = "Près";
                            OGLOIN.Text = "Loin";
                            OGINTERM.Text = "Loin";
                            OGPRES.Text = "Près";
                            MonturePresLabel.Text = "LOIN";
                            MontureLoinLabel.Text = "LOIN";


                        }
                        else
                        {
                            if (MontureClass.Loin == false && MontureClass.Pres == true)
                            {
                                ODLOIN.Text = "Loin";
                                ODITERM.Text = "Près";
                                ODPRES.Text = "Près";
                                OGLOIN.Text = "Loin";
                                OGINTERM.Text = "Près";
                                OGPRES.Text = "Près";
                                MonturePresLabel.Text = "PRES";
                                MontureLoinLabel.Text = "PRES";
                            }
                        }
                    }
                    if (MontureClass.Encaissé != 0)
                    {
                        montureversementzero = true;
                    }
                    else
                    {
                        montureversementzero = false;
                    }
                    if (MontureClass.StatutDevis == true && MontureClass.StatutVente == false)
                    {
                        TxtStatutGlobal.Content = "Devis";

                        TxtStatutGlobal.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    }
                    else
                    {
                        if (MontureClass.StatutDevis == true && MontureClass.StatutVente == true)
                        {
                            TxtStatutGlobal.Content = "Vente validée";
                            TxtStatutGlobal.Background = System.Windows.Media.Brushes.LightGreen;
                        }
                    }

                }
                listsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                listsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList();
                listsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList();
                listsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();
                anciennelistsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                anciennelistsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList();
                anciennelistsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList();
                anciennelistsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void AnnulerMonture_Click(object sender, RoutedEventArgs e)
        {
            GridMonture.DataContext = null;
            GridMonture.IsEnabled = false;
            nouvellemonture = false;
            anciennemonture = false;
            txtDroiteFleshHaut.Visibility = Visibility.Visible;
            DroiteFleshBas.Visibility = Visibility.Collapsed;
            txtDroiteFleshDroite.Visibility = Visibility.Visible;
            txtDroiteFleshGauche.Visibility = Visibility.Collapsed;
            txtGaucheFleshHaut.Visibility = Visibility.Visible;
            txtGaucheFleshBas.Visibility = Visibility.Collapsed;
            txtGaucheFleshDroite.Visibility = Visibility.Visible;
            txtGaucheFleshGauche.Visibility = Visibility.Collapsed;
            ODLOIN.Text = "Loin";
            ODITERM.Text = "Interm.";
            ODPRES.Text = "Près";
            OGLOIN.Text = "Loin";
            OGINTERM.Text = "Interm.";
            OGPRES.Text = "Près";
            MonturePresLabel.Text = "PRES";
            MontureLoinLabel.Text = "LOIN";
        }

        /*  private void txtDroiteVerreLoinDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
          {
              try
              {
                  if (nouvellemonture == true && anciennemonture == false)
                  {
                      SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 1, MontureClass, 0);
                      cl.Show();
                  }
                  else
                  {
                      if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                      {
                          SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 1, MontureClass, 0);
                          cl.Show();
                      }
                  }
              }
              catch (Exception ex)
              {
                  MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

              }
          }*/

        private void txtDroiteVerreLoinDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    //  listsupp1 = new List<SVC.MontureSupplement>();
                    SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 1, MontureClass, 0, listsupp1,Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        //    listsupp1 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 1).ToList();
                        SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 1, MontureClass, 1, listsupp1,Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /* private void txtGaucheVerreLoinDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
         {
             try
             {
                 if (nouvellemonture == true && anciennemonture == false)
                 {
                     SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 2, MontureClass, 0);
                     cl.Show();
                 }

                 else
                 {
                     if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                     {
                         SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 2, MontureClass, 0);
                         cl.Show();
                     }
                 }

             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
        private void txtGaucheVerreLoinDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    // listsupp2 = new List<SVC.MontureSupplement>();
                    SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 2, MontureClass, 0, listsupp2,Clientvv);
                    cl.Show();
                }

                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        //  listsupp2 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 2).ToList(); ;

                        SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 2, MontureClass, 1, listsupp2,Clientvv);
                        cl.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtMontureDesignationLoin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    //  SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 5, MontureClass, 0);
                    selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 5, MontureClass, 0, Clientvv);
                    cl.Show();
                }

                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        //   SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 5, MontureClass, 0);
                        selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 5, MontureClass, 0, Clientvv);
                        cl.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void MontureDatagrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MontureDatagrid.ItemsSource = proxy.GetAllMonturebycode(Clientvv.Id).OrderBy(n => n.Date);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        /* private void txtDroiteVerrePresDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
         {
             try
             {
                 if (nouvellemonture == true && anciennemonture == false)
                 {
                     SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 3, MontureClass, 0);
                     cl.Show();
                 }
                 else
                 {
                     if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                     {
                         SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 3, MontureClass, 0);
                         cl.Show();
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
        private void txtDroiteVerrePresDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {

                    // listsupp3 = new List<SVC.MontureSupplement>();
                    SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 3, MontureClass, 0, listsupp3,Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {

                        //     listsupp3 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 3).ToList(); ;
                        SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 3, MontureClass, 1, listsupp3,Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /* private void txtGaucheVerrePresDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
         {
             try
             {
                 if (nouvellemonture == true && anciennemonture == false)
                 {
                     SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 4, MontureClass, 0);
                     cl.Show();
                 }
                 else
                 {
                     if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                     {
                         SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 4, MontureClass, 0);
                         cl.Show();
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

             }
         }*/
        private void txtGaucheVerrePresDesignation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    // listsupp4 = new List<SVC.MontureSupplement>();
                    SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 4, MontureClass, 0, listsupp4,Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        //  listsupp4 = proxy.GetAllMontureSupplementbycle(MontureClass.Cle).Where(n => n.interfaceMonture == 4).ToList();

                        SelectionVerre cl = new SelectionVerre(proxy, memberuser, callback, 4, MontureClass, 1, listsupp4,Clientvv);
                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtMontureDesignationPres_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    // SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 6, MontureClass, 0);
                    selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 6, MontureClass, 0, Clientvv);

                    cl.Show();
                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        // SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 6, MontureClass, 0);
                        selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 6, MontureClass, 0, Clientvv);

                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoires1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    // SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 7, MontureClass, 0);
                    selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 7, MontureClass, 0, Clientvv);

                    cl.Show();
                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        //  SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 7, MontureClass, 0);
                        selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 7, MontureClass, 0, Clientvv);

                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoires2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (nouvellemonture == true && anciennemonture == false)
                {
                    //   SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 8, MontureClass, 0);
                    selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 8, MontureClass, 0, Clientvv);

                    cl.Show();
                }
                else
                {
                    if (nouvellemonture == false && anciennemonture == true && MontureClass.StatutVente == false)
                    {
                        //   SelectionProduit cl = new SelectionProduit(proxy, memberuser, callback, 8, MontureClass, 0);
                        selectProduitCat cl = new selectProduitCat(proxy, memberuser, callback, 8, MontureClass, 0, Clientvv);

                        cl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void txtGaucheFleshDroite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtGaucheFleshDroite.Visibility = Visibility.Collapsed;
                txtGaucheFleshGauche.Visibility = Visibility.Visible;
                MontureClass.GaucheFleshDroite = false;
                MontureClass.GaucheFleshGauche = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtGaucheFleshGauche_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtGaucheFleshDroite.Visibility = Visibility.Visible;
                txtGaucheFleshGauche.Visibility = Visibility.Collapsed;
                MontureClass.GaucheFleshDroite = true;
                MontureClass.GaucheFleshGauche = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void txtDroiteFleshHaut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDroiteFleshHaut.Visibility = Visibility.Collapsed;
                DroiteFleshBas.Visibility = Visibility.Visible;
                MontureClass.DroiteFleshBas = true;
                MontureClass.DroiteFleshHaut = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DroiteFleshBas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDroiteFleshHaut.Visibility = Visibility.Visible;
                DroiteFleshBas.Visibility = Visibility.Collapsed;
                MontureClass.DroiteFleshBas = false;
                MontureClass.DroiteFleshHaut = true;
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

        private void txtPrixTotalLoin_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixMontureLoin_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixTotalPres_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixMonturePres_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixTotalAcc1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtPrixTotalAcc2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresPrix1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite1.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite1.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix1.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix1.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc1.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresPrix2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite2.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite2.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix2.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix2.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc2.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtDroitPrixVerreLoin_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal DroitPrixLoin, GauchePrixLoin = 0;
                if (txtDroitPrixVerreLoin.Text != "")
                {
                    DroitPrixLoin = Convert.ToDecimal(txtDroitPrixVerreLoin.Text);
                }
                else
                {
                    DroitPrixLoin = 0;
                }
                if (txtGauchePrixVerreLoin.Text != "")
                {
                    GauchePrixLoin = Convert.ToDecimal(txtGauchePrixVerreLoin.Text);
                }
                else
                {
                    GauchePrixLoin = 0;
                }


                txtPrixTotalLoin.Text = (DroitPrixLoin + GauchePrixLoin).ToString();



            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtGauchePrixVerreLoin_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal DroitPrixLoin, GauchePrixLoin = 0;
                if (txtDroitPrixVerreLoin.Text != "")
                {
                    DroitPrixLoin = Convert.ToDecimal(txtDroitPrixVerreLoin.Text);
                }
                else
                {
                    DroitPrixLoin = 0;
                }
                if (txtGauchePrixVerreLoin.Text != "")
                {
                    GauchePrixLoin = Convert.ToDecimal(txtGauchePrixVerreLoin.Text);
                }
                else
                {
                    GauchePrixLoin = 0;
                }


                txtPrixTotalLoin.Text = (DroitPrixLoin + GauchePrixLoin).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtDroitPrixVerrePres_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal DroitPrixPres, GauchePrixPres = 0;
                if (txtDroitPrixVerrePres.Text != "")
                {
                    DroitPrixPres = Convert.ToDecimal(txtDroitPrixVerrePres.Text);
                }
                else
                {
                    DroitPrixPres = 0;
                }
                if (txtGauchePrixVerrePres.Text != "")
                {
                    GauchePrixPres = Convert.ToDecimal(txtGauchePrixVerrePres.Text);
                }
                else
                {
                    GauchePrixPres = 0;
                }


                txtPrixTotalPres.Text = (DroitPrixPres + GauchePrixPres).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtGauchePrixVerrePres_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal DroitPrixPres, GauchePrixPres = 0;
                if (txtDroitPrixVerrePres.Text != "")
                {
                    DroitPrixPres = Convert.ToDecimal(txtDroitPrixVerrePres.Text);
                }
                else
                {
                    DroitPrixPres = 0;
                }
                if (txtGauchePrixVerrePres.Text != "")
                {
                    GauchePrixPres = Convert.ToDecimal(txtGauchePrixVerrePres.Text);
                }
                else
                {
                    GauchePrixPres = 0;
                }


                txtPrixTotalPres.Text = (DroitPrixPres + GauchePrixPres).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresQuantite1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite1.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite1.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix1.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix1.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc1.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtAccessoiresQuantite2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Decimal QuantiteAccessoire, Prix = 0;
                if (txtAccessoiresQuantite2.Text != "")
                {
                    QuantiteAccessoire = Convert.ToDecimal(txtAccessoiresQuantite2.Text);
                }
                else
                {
                    QuantiteAccessoire = 0;
                }
                if (txtAccessoiresPrix2.Text != "")
                {
                    Prix = Convert.ToDecimal(txtAccessoiresPrix2.Text);
                }
                else
                {
                    Prix = 0;
                }


                txtPrixTotalAcc2.Text = (QuantiteAccessoire * Prix).ToString();


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void GaucheFleshBas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtGaucheFleshHaut.Visibility = Visibility.Visible;
                txtGaucheFleshBas.Visibility = Visibility.Collapsed;
                MontureClass.GaucheFleshHaut = true;
                MontureClass.GaucheFleshBas = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnVision_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MontureClass.Loin == true && MontureClass.Pres == true)
                {
                    MontureClass.Loin = true;
                    MontureClass.Pres = false;
                    ODLOIN.Text = "Loin";
                    ODITERM.Text = "Loin";
                    ODPRES.Text = "Près";
                    OGLOIN.Text = "Loin";
                    OGINTERM.Text = "Loin";
                    OGPRES.Text = "Près";
                    MonturePresLabel.Text = "LOIN";
                    MontureLoinLabel.Text = "LOIN";
                    txtIntermSphereDroite.Text = txtLoinSphereDroite.Text;
                    txtIntermCylindreDroite.Text = txtLoinCylindreDroite.Text;
                    txtIntermAxeDroite.Text = txtLoinAxeDroite.Text;
                    txtIntermAddDroite.Text = txtLoinAddDroite.Text;
                    txtIntermPrismeDroite.Text = txtLoinPrismeDroite.Text;
                    txtIntermBaseDroite.Text = txtLoinBaseDroite.Text;
                    /*************************************************/
                    txtIntermSphereGauche.Text = txtLoinSphereGauche.Text;
                    txtIntermCylindreGauche.Text = txtLoinCylindreGauche.Text;
                    txtIntermAxeGauche.Text = txtLoinAxeGauche.Text;
                    txtIntermAddGauche.Text = txtLoinAddGauche.Text;
                    txtIntermPrismeGauche.Text = txtLoinPrismeGauche.Text;
                    txtIntermBaseGauche.Text = txtLoinBaseGauche.Text;
                }
                else
                {
                    if (MontureClass.Loin == true && MontureClass.Pres == false)
                    {
                        MontureClass.Loin = false;
                        MontureClass.Pres = true;
                        ODLOIN.Text = "Loin";
                        ODITERM.Text = "Près";
                        ODPRES.Text = "Près";
                        OGLOIN.Text = "Loin";
                        OGINTERM.Text = "Près";
                        OGPRES.Text = "Près";
                        MonturePresLabel.Text = "PRES";
                        MontureLoinLabel.Text = "PRES";
                        txtIntermSphereDroite.Text = txtPresSphereDroite.Text;
                        txtIntermCylindreDroite.Text = txtPresCylindreDroite.Text;
                        txtIntermAxeDroite.Text = txtPresAxeDroite.Text;
                        txtIntermAddDroite.Text = "";
                        txtIntermPrismeDroite.Text = txtPresPrismeDroite.Text;
                        txtIntermBaseDroite.Text = txtPresBaseDroite.Text;
                        /****************************************/
                        txtIntermSphereGauche.Text = txtPresSphereGauche.Text;
                        txtIntermCylindreGauche.Text = txtPresCylindreGauche.Text;
                        txtIntermAxeGauche.Text = txtPresAxeGauche.Text;
                        txtIntermAddGauche.Text = "";
                        txtIntermPrismeGauche.Text = txtPresPrismeGauche.Text;
                        txtIntermBaseGauche.Text = txtPresBaseGauche.Text;
                    }
                    else
                    {
                        if (MontureClass.Loin == false && MontureClass.Pres == true)
                        {
                            MontureClass.Loin = true;
                            MontureClass.Pres = true;
                            ODLOIN.Text = "Loin";
                            ODITERM.Text = "Interm.";
                            ODPRES.Text = "Près";
                            OGLOIN.Text = "Loin";
                            OGINTERM.Text = "Interm.";
                            OGPRES.Text = "Près";
                            MonturePresLabel.Text = "PRES";
                            MontureLoinLabel.Text = "LOIN";
                            txtIntermSphereDroite.Text = "";
                            txtIntermCylindreDroite.Text = "";
                            txtIntermAxeDroite.Text = "";
                            txtIntermAddDroite.Text = "";
                            txtIntermPrismeDroite.Text = "";
                            txtIntermBaseDroite.Text = "";
                            /********************************/
                            txtIntermSphereGauche.Text = "";
                            txtIntermCylindreGauche.Text = "";
                            txtIntermAxeGauche.Text = "";
                            txtIntermAddGauche.Text = "";
                            txtIntermPrismeGauche.Text = "";
                            txtIntermBaseGauche.Text = "";
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtGaucheFleshHaut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtGaucheFleshHaut.Visibility = Visibility.Collapsed;
                txtGaucheFleshBas.Visibility = Visibility.Visible;
                MontureClass.GaucheFleshHaut = false;
                MontureClass.GaucheFleshBas = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void txtDroiteFleshGauche_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDroiteFleshGauche.Visibility = Visibility.Collapsed;
                txtDroiteFleshDroite.Visibility = Visibility.Visible;
                MontureClass.DroiteFleshDroite = true;
                MontureClass.DroiteFleshGauche = false;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void txtDroiteFleshDroite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDroiteFleshGauche.Visibility = Visibility.Visible;
                txtDroiteFleshDroite.Visibility = Visibility.Collapsed;
                MontureClass.DroiteFleshDroite = false;
                MontureClass.DroiteFleshGauche = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtRemiseMonture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                Decimal PrixTotalLoin, PrixMontureLoin, PrixTotalPres, PrixMonturePres, PrixTotalAcc1, PrixTotalAcc2, remisemonture, montanttotal = 0;

                if (txtPrixTotalLoin.Text != "")
                {
                    PrixTotalLoin = Convert.ToDecimal(txtPrixTotalLoin.Text);
                }
                else
                {
                    PrixTotalLoin = 0;
                }
                if (txtPrixMontureLoin.Text != "")
                {
                    PrixMontureLoin = Convert.ToDecimal(txtPrixMontureLoin.Text);
                }
                else
                {
                    PrixMontureLoin = 0;
                }
                if (txtPrixTotalPres.Text != "")
                {
                    PrixTotalPres = Convert.ToDecimal(txtPrixTotalPres.Text);
                }
                else
                {
                    PrixTotalPres = 0;
                }

                if (txtPrixMonturePres.Text != "")
                {
                    PrixMonturePres = Convert.ToDecimal(txtPrixMonturePres.Text);
                }
                else
                {
                    PrixMonturePres = 0;
                }
                if (txtPrixTotalAcc1.Text != "")
                {
                    PrixTotalAcc1 = Convert.ToDecimal(txtPrixTotalAcc1.Text);
                }
                else
                {
                    PrixTotalAcc1 = 0;
                }

                if (txtPrixTotalAcc2.Text != "")
                {
                    PrixTotalAcc2 = Convert.ToDecimal(txtPrixTotalAcc2.Text);
                }
                else
                {
                    PrixTotalAcc2 = 0;
                }

                montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                if (!String.IsNullOrEmpty(txtRemiseMonture.Text))
                {
                    remisemonture = Convert.ToDecimal(txtRemiseMonture.Text);
                    if (remisemonture != 0 && montanttotal != 0)
                    {
                        if (((selectedparam.maxremisevente * montanttotal) / 100 >= Convert.ToDecimal(remisemonture)))
                        {
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture);
                        }
                        else
                        {
                            txtRemiseMonture.Text = "";
                            montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                        }
                    }
                    else
                    {
                        txtRemiseMonture.Text = "";
                        montanttotal = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin);
                    }
                }
                else
                {
                    remisemonture = 0;
                    txtRemiseMonture.Text = "";
                }
                /**********************************/

                /***********************************/

                // txtMontantTotal.Text = (PrixTotalAcc2 + PrixTotalAcc1 + PrixMonturePres + PrixTotalPres + PrixMontureLoin + PrixTotalLoin - remisemonture).ToString();

                txtMontantTotal.Text = (montanttotal).ToString();
                MontureClass.MontantTotal = Convert.ToDecimal(txtMontantTotal.Text);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtLoinAddDroite_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                decimal LoinAddDroite, LoinSphereDroite = 0;
                if (txtLoinAddDroite.Text != "")
                {
                    if (decimal.TryParse(txtLoinAddDroite.Text, out LoinAddDroite))
                        LoinAddDroite = Convert.ToDecimal(txtLoinAddDroite.Text);
                    else
                        LoinAddDroite = 0;
                }
                else
                {
                    txtLoinAddDroite.Text = "";
                    LoinAddDroite = 0;
                }
                if (txtLoinSphereDroite.Text != "")
                {
                    if (decimal.TryParse(txtLoinSphereDroite.Text, out LoinSphereDroite))
                        LoinSphereDroite = Convert.ToDecimal(txtLoinSphereDroite.Text);
                    else
                        LoinSphereDroite = 0;
                }
                else
                {
                    txtLoinSphereDroite.Text = "";
                    LoinSphereDroite = 0;
                }
                if (LoinAddDroite != 0 && LoinSphereDroite != 0)
                {

                    txtPresCylindreDroite.Text = txtLoinCylindreDroite.Text;
                    txtPresAxeDroite.Text = txtLoinAxeDroite.Text;
                    txtPresSphereDroite.Text = (LoinAddDroite + LoinSphereDroite).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void txtLoinAddGauche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                decimal LoinAddDroite, LoinSphereDroite = 0;
                if (txtLoinAddGauche.Text != "")
                {
                    if (decimal.TryParse(txtLoinAddGauche.Text, out LoinAddDroite))
                        LoinAddDroite = Convert.ToDecimal(txtLoinAddGauche.Text);
                    else
                        LoinAddDroite = 0;
                }
                else
                {
                    txtLoinAddGauche.Text = "";
                    LoinAddDroite = 0;
                }
                if (txtLoinSphereGauche.Text != "")
                {
                    if (decimal.TryParse(txtLoinSphereGauche.Text, out LoinSphereDroite))
                        LoinSphereDroite = Convert.ToDecimal(txtLoinSphereGauche.Text);
                    else
                        LoinSphereDroite = 0;
                }
                else
                {
                    txtLoinSphereGauche.Text = "";
                    LoinSphereDroite = 0;
                }
                if (LoinAddDroite != 0 && LoinSphereDroite != 0)
                {

                    txtPresCylindreGauche.Text = txtLoinCylindreGauche.Text;
                    txtPresAxeGauche.Text = txtLoinAxeGauche.Text;
                    txtPresSphereGauche.Text = (LoinAddDroite + LoinSphereDroite).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
