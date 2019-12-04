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
    /// Interaction logic for LentilleClient.xaml
    /// </summary>
    public partial class LentilleClient : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic memberuser;
        SVC.ClientV Clientvv;
        private delegate void FaultedInvokerLentilleClient();
        bool nouvellelentille = false;
        bool anciennelentille = false;
        SVC.LentilleClient LentilleClass;
        bool Lentilleversementzero = false;
        bool visualiserLentille = false;
        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        int interfaceimpressionlentille = 0;
        DXDialog dialog1;
        SVC.Param selectedparam;
        public LentilleClient(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu, SVC.ClientV clientrecu, SVC.LentilleClient LENTILLERECU)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                callback = callbackrecu;
                memberuser = memberrecu;
                Clientvv = clientrecu;
                selectedparam=proxy.GetAllParamétre();
                /********************************Lentille**************************************/
                LentilleDatagrid.ItemsSource = proxy.GetAllLentilleClientbycode(Clientvv.Id).OrderBy(n => n.Date);
                LentilleClass = LENTILLERECU;
                LentilleDatagrid.SelectedItem= LENTILLERECU;
                existelentille(LentilleClass);
                callbackrecu.InsertLentilleClientCallbackevent += new ICallback.CallbackEventHandler35(callbackreculentille_Refresh);
                /****************************************************************************************/
            }
            catch (Exception ex)
            {
                HandleProxy();
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
                         txtMontantTotalENCLentille.IsEnabled = true;
                    }
                    else
                    {
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

                    };
                    GridLentille.DataContext = LentilleClass;
                    GridLentille.IsEnabled = true;
                    TxtStatutGlobalLentille.Content = "Devis";
                    TxtStatutGlobalLentille.Background = System.Windows.Media.Brushes.PaleVioletRed;
                    nouvellelentille = true;
                    anciennelentille = false;

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
                         txtMontantTotalENCLentille.IsEnabled = true;
                    }
                    else
                    {
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
                                        var VisiteApayerexiste = proxy.GetAllF1ByCleDossier(selecedmonture.Cle).Any();
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

        private void btnImprimerLentille_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                dialog1 = new DXDialog("Impression", DialogButtons.YesNo, true);
                dialog1.Template = Resources["template6"] as ControlTemplate;

                // dialog1.Content = Resources["content"];
                dialog1.ResizeMode = ResizeMode.NoResize;
                dialog1.Width = 350;
                dialog1.Height = 200;
                dialog1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog1.ShowDialog();
                interfaceimpressionlentille = 0;
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
                    SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 1, LentilleClass, 0,Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 1, LentilleClass, 1,Clientvv);
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
                    SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 2, LentilleClass, 0,Clientvv);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        SelectionLentille cl = new SelectionLentille(proxy, memberuser, callback, 2, LentilleClass, 1,Clientvv);
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
                    SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 7, LentilleClass, 0);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 7, LentilleClass, 0);
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
                    SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 8, LentilleClass, 0);
                    cl.Show();
                }
                else
                {
                    if (nouvellelentille == false && anciennelentille == true && LentilleClass.StatutVente == false)
                    {
                        SelectionProduitLentille cl = new SelectionProduitLentille(proxy, memberuser, callback, 8, LentilleClass, 0);
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
                         txtMontantTotalENCLentille.IsEnabled = true;
                    }
                    else
                    {
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

                ExportA4(reportViewer1);
                Print(false);
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
    }
}
