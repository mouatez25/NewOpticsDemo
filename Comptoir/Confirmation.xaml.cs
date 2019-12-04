
using Microsoft.Reporting.WinForms;
using NewOptics.Administrateur;
using NewOptics.ClientA;
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
using Nut;

namespace NewOptics.Comptoir
{
    /// <summary>
    /// Interaction logic for Confirmation.xaml
    /// </summary>
    public partial class Confirmation : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerComptoirConfiramtion();
        SVC.Param paramtetre;
        Comptoir comptoircalcu;
        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        public Confirmation(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic membershiprecu, Comptoir comptoirrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = membershiprecu;
                comptoircalcu = comptoirrecu;
                comptoirrecu.IsEnabled = false;
                txttotalcaluc.Text = String.Format("{0:0.##}", ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)));
                txtversement.Text = String.Format("{0:0.##}", ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)));
                List<SVC.ClientV> listclient = new List<SVC.ClientV>();
                SVC.ClientV l = new SVC.ClientV
                {
                    Id = 0,
                    Raison = "Vente comptoir",
                    adresse = "Vente comptoir",
                    dates = new DateTime(1987, 07, 31),
                    oper = "Administrateur",
                    solde = 0,
                };
                listclient.Add(l);
                comboclient.ItemsSource = listclient;// proxy.GetAllClientVBYID(0).OrderBy(n => n.Raison);
                comboclient.SelectedIndex = 0;
                txtremise.Text = "0";
                datevente.SelectedDate = DateTime.Now;


                paramtetre = proxy.GetAllParamétre();

                if (paramtetre.affiben == true)
                {
                    Bénéfice.Visibility = Visibility.Visible;
                    Bénéficemont.Visibility = Visibility.Visible;
                    Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite))));

                }
                if (paramtetre.modidate == true)
                {
                    datevente.IsEnabled = true;
                }
                else
                {
                    datevente.IsEnabled = false;
                }
                callbackrecu.InsertClientVCallbackEvent += new ICallback.CallbackEventHandler7(callbackrecu_Refresh);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertClientV e)
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
        public void AddRefresh(SVC.ClientV listmembership, int oper)
        {
            try
            {
                if (chcreit.IsChecked == true)
                {
                    var LISTITEM1 = comboclient.ItemsSource as IEnumerable<SVC.ClientV>;
                    List<SVC.ClientV> LISTITEM = LISTITEM1.ToList();

                    if (oper == 1)
                    {
                        LISTITEM.Add(listmembership);
                    }
                    else
                    {
                        if (oper == 2)
                        {

                            var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                            var index = LISTITEM.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEM[index] = listmembership;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                                LISTITEM.Remove(deleterendez);
                            }
                        }
                    }

                    comboclient.ItemsSource = LISTITEM;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerComptoirConfiramtion(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerComptoirConfiramtion(HandleProxy));
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
        private void Print()
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
                printDoc.DocumentName = "Ticket Comptoir";
                printDoc.Print();
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
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

        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>72mm</PageWidth>
                <PageHeight>210mm</PageHeight>
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
        private Stream CreateStream(string name,
 string fileNameExtension, Encoding encoding,
 string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        private void CONFIRMERVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcreit.IsChecked == true && comboclient.SelectedItem != null && MemberUser.ModuleVenteCompoirAcces == true)
                {
                    SVC.ClientV selectedclient = comboclient.SelectedItem as SVC.ClientV;
                    if (selectedclient.Id != 0)
                    {
                        SVC.F1 selectF1 = new SVC.F1
                        {
                            codeclient = selectedclient.Id,
                            dates = DateTime.Now,
                            date = datevente.SelectedDate,
                            oper = MemberUser.Username,
                            timbre = 0,
                            bcom = "",
                            echeance = 0,
                            raison = selectedclient.Raison,

                            heure = DateTime.Now.TimeOfDay,

                            tva = 0,
                            modep = "ESPECES",
                            net = Convert.ToDecimal(txtnouveau.Text),
                            ht = (comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),

                            cle = selectedclient.Id + selectedclient.Raison + txtnouveau.Text + DateTime.Now.TimeOfDay,
                        };
                        if (txtremise.Text != "")
                        {
                            if (Convert.ToDecimal(txtremise.Text) != 0)
                            {
                                selectF1.remise = Convert.ToDecimal(txtremise.Text);
                            }
                            else
                            {
                                selectF1.remise = 0;
                            }

                        }
                        else
                        {
                            selectF1.remise = 0;
                        }
                        if (txtversement.Text != "")
                        {
                            if (Convert.ToDecimal(txtversement.Text) != 0)
                            {
                                selectF1.versement = Convert.ToDecimal(txtversement.Text);
                                selectF1.reste = selectF1.net - selectF1.versement;

                            }
                            else
                            {
                                selectF1.versement = 0;
                                selectF1.reste = selectF1.net - selectF1.versement;

                            }

                        }
                        else
                        {
                            selectF1.versement = 0;
                            selectF1.reste = selectF1.net - selectF1.versement;

                        }

                        if (selectF1.reste != 0)
                        {
                            selectF1.soldé = false;
                        }
                        else
                        {
                            selectF1.soldé = true;

                        }


                        var remisepourfacture = selectF1.remise;
                        bool Operfacture = false;

                        List<int> listrefresh = new List<int>();

                        foreach (SVC.Facture newfacture in comptoircalcu.listcomptoir)
                        {
                            newfacture.cle = selectF1.cle;
                            newfacture.codeclient = selectF1.codeclient;
                            newfacture.Client = selectF1.raison;
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
                                listrefresh.Add(Convert.ToInt16(newfacture.ficheproduit));
                            }
                        }

                        if (selectF1.versement != 0)
                        {


                            SVC.Depeiment PAIEMENT = new SVC.Depeiment
                            {
                                date = selectF1.date,
                                montant = Convert.ToDecimal(selectF1.versement),
                                paiem = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                oper = MemberUser.Username,
                                dates = selectF1.dates,
                                banque = "Caisse",
                                nfact = selectF1.nfact,
                                amontant = Convert.ToDecimal(selectF1.net),
                                cle = selectF1.cle,
                                cp = selectF1.Id,
                                Multiple = false,
                                CodeClient = selectF1.codeclient,
                                RaisonClient = selectF1.raison,

                            };
                            SVC.Depense CAISSE = new SVC.Depense
                            {
                                cle = selectF1.cle,
                                Auto = true,
                                Commentaires = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                CompteDébité = "Caisse",
                                Crédit = true,
                                DateDebit = selectF1.date,
                                DateSaisie = selectF1.dates,
                                Débit = false,
                                ModePaiement = "ESPECES",
                                Montant = 0,
                                MontantCrédit = selectF1.versement,
                                NumCheque = Convert.ToString(selectF1.Id),
                                Num_Facture = selectF1.nfact,
                                RubriqueComptable = "ESPECES COMPTOIR",
                                Username = MemberUser.Username,

                            };
                            bool ii = false;
                            bool depense = false;
                            bool depaiement = false;
                            List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                            foreach (var item in comptoircalcu.listcomptoir)
                            {
                                if (item.quantite != 0)
                                {
                                    listsanszero.Add(item);
                                }
                            }
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                ii = proxy.InsertFacture(selectF1, listsanszero, "B");

                                Operfacture = true;
                                proxy.InsertDepeiment(PAIEMENT);
                                depaiement = true;
                                proxy.InsertDepense(CAISSE);
                                depense = true;

                                if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                {
                                    ts.Complete();
                                }
                            }
                            if (Operfacture == true && ii == true && depaiement == true && depense == true)
                            {
                                proxy.AjouterProdflistRefresh(listrefresh);
                                proxy.AjouterSoldeF1Refresh();
                                this.comboclient.IsEnabled = true;
                                comptoircalcu.listcomptoir = new List<SVC.Facture>();
                                comptoircalcu.ReceptDatagrid.ItemsSource = comptoircalcu.listcomptoir;
                                comptoircalcu.digitalGaugeControl1.Content = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                                comptoircalcu.nbreproduit.Text = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Count());
                                if (comptoircalcu.chimprimer.IsChecked == true)
                                {
                                    var Nfact = proxy.GetAllF1ByCle(selectF1.cle);
                                    LocalReport reportViewer1 = new LocalReport();
                                    Nfact.Add(selectF1);
                                    string nfact = Nfact.First().nfact.Substring(0, 1);
                                    string arrété = "";
                                    string document = "";
                                    switch (nfact)
                                    {
                                        case "F":
                                            arrété = "Arrétée la présente facture à la somme de";
                                            document = "Facture";
                                            break;
                                        case "A":
                                            arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                            document = "Facture d'Avoir";
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
                                            break;
                                        case "R":
                                            arrété = "Arrétée la présente facture à la somme de";
                                            document = "Facture";
                                            break;
                                    }

                                    /******************Dataset1 parametre****************************/
                                    var selpara = new List<SVC.Param>();
                                    selpara.Add((proxy.GetAllParamétre()));
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
                                    MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                                    reportViewer1.LoadReportDefinition(MyRptStream);
                                    /*************************************************/
                                    ReportDataSource rds = new ReportDataSource();
                                    rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                                          //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                                    rds.Value = Nfact;
                                    reportViewer1.DataSources.Add(rds);
                                    /**************************************************/
                                    var ClientList = new List<SVC.ClientV>();
                                    ClientList.Add(selectedclient);
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
                                    TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                                    ReportParameter paramTimeTo = new ReportParameter();
                                    paramTimeTo.Name = "TimeTo";
                                    paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                                    reportViewer1.SetParameters(paramTimeTo);
                                    reportViewer1.Refresh();

                                    Export(reportViewer1);
                                    Print();
                                }
                                comptoircalcu.IsEnabled = true;
                                comptoircalcu.Activate();

                                this.Close();
                            }
                        }
                        else
                        {
                            bool ii = false;
                            List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                            foreach (var item in comptoircalcu.listcomptoir)
                            {
                                if (item.quantite != 0)
                                {
                                    listsanszero.Add(item);
                                }
                            }
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                ii = proxy.InsertFacture(selectF1, listsanszero, "B");

                                Operfacture = true;


                                if (Operfacture == true && ii == true)
                                {
                                    ts.Complete();
                                }
                            }
                            if (Operfacture == true && ii == true)
                            {
                                proxy.AjouterProdflistRefresh(listrefresh);
                                proxy.AjouterSoldeF1Refresh();
                                this.comboclient.IsEnabled = true;
                                comptoircalcu.listcomptoir = new List<SVC.Facture>();
                                comptoircalcu.ReceptDatagrid.ItemsSource = comptoircalcu.listcomptoir;
                                comptoircalcu.digitalGaugeControl1.Content= Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                                comptoircalcu.nbreproduit.Text = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Count());
                                if (comptoircalcu.chimprimer.IsChecked == true)
                                {
                                    var Nfact = proxy.GetAllF1ByCle(selectF1.cle);
                                    LocalReport reportViewer1 = new LocalReport();
                                    Nfact.Add(selectF1);
                                    string nfact = Nfact.First().nfact.Substring(0, 1);
                                    string arrété = "";
                                    string document = "";
                                    switch (nfact)
                                    {
                                        case "F":
                                            arrété = "Arrétée la présente facture à la somme de";
                                            document = "Facture";
                                            break;
                                        case "A":
                                            arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                            document = "Facture d'Avoir";
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
                                            break;
                                        case "R":
                                            arrété = "Arrétée la présente facture à la somme de";
                                            document = "Facture";
                                            break;
                                    }

                                    /******************Dataset1 parametre****************************/
                                    var selpara = new List<SVC.Param>();
                                    selpara.Add((proxy.GetAllParamétre()));
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
                                    MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                                    reportViewer1.LoadReportDefinition(MyRptStream);
                                    /*************************************************/
                                    ReportDataSource rds = new ReportDataSource();
                                    rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                                          //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                                    rds.Value = Nfact;
                                    reportViewer1.DataSources.Add(rds);
                                    /**************************************************/
                                    var ClientList = new List<SVC.ClientV>();
                                    ClientList.Add(selectedclient);
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
                                    TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                                    ReportParameter paramTimeTo = new ReportParameter();
                                    paramTimeTo.Name = "TimeTo";
                                    paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                                    reportViewer1.SetParameters(paramTimeTo);
                                    reportViewer1.Refresh();

                                    Export(reportViewer1);
                                    Print();
                                }
                                comptoircalcu.IsEnabled = true;
                                comptoircalcu.Activate();
                                this.Close();
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
                    if (chcreit.IsChecked == false && comboclient.SelectedItem != null)
                    {
                        SVC.ClientV selectedclient = comboclient.SelectedItem as SVC.ClientV;
                        if (selectedclient.Id == 0)
                        {
                            SVC.F1 selectF1 = new SVC.F1
                            {
                                codeclient = selectedclient.Id = 0,
                                dates = DateTime.Now,
                                date = datevente.SelectedDate,
                                oper = MemberUser.Username,
                                timbre = 0,
                                bcom = "",
                                echeance = 0,
                                raison = selectedclient.Raison,
                                soldé = true,
                                heure = DateTime.Now.TimeOfDay,
                                reste = 0,
                                tva = 0,
                                modep = "ESPECES",
                                net = Convert.ToDecimal(txtnouveau.Text),
                                ht = (comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),
                                versement = Convert.ToDecimal(txtnouveau.Text),
                                cle = selectedclient.Id + selectedclient.Raison + txtnouveau.Text + DateTime.Now.TimeOfDay,
                            };
                            if (txtremise.Text != "")
                            {
                                if (Convert.ToDecimal(txtremise.Text) != 0)
                                {
                                    selectF1.remise = Convert.ToDecimal(txtremise.Text);
                                }
                                else
                                {
                                    selectF1.remise = 0;
                                }

                            }
                            else
                            {
                                selectF1.remise = 0;
                            }
                            var remisepourfacture = selectF1.remise;
                            bool Operfacture = false;

                            List<int> listrefresh = new List<int>();

                            foreach (SVC.Facture newfacture in comptoircalcu.listcomptoir)
                            {
                                newfacture.cle = selectF1.cle;
                                newfacture.codeclient = selectF1.codeclient;
                                newfacture.Client = selectF1.raison;
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
                                    listrefresh.Add(Convert.ToInt16(newfacture.ficheproduit));
                                }
                            }
                            SVC.Depeiment PAIEMENT = new SVC.Depeiment
                            {
                                date = selectF1.date,
                                montant = Convert.ToDecimal(selectF1.net),
                                paiem = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                oper = MemberUser.Username,
                                dates = selectF1.dates,
                                banque = "Caisse",
                                nfact = selectF1.nfact,
                                amontant = Convert.ToDecimal(selectF1.net),
                                cle = selectF1.cle,
                                cp = selectF1.Id,
                                Multiple = false,
                                CodeClient = selectF1.codeclient,
                                RaisonClient = selectF1.raison,

                            };
                            SVC.Depense CAISSE = new SVC.Depense
                            {
                                cle = selectF1.cle,
                                Auto = true,
                                Commentaires = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                CompteDébité = "Caisse",
                                Crédit = true,
                                DateDebit = selectF1.date,
                                DateSaisie = selectF1.dates,
                                Débit = false,
                                ModePaiement = "ESPECES",
                                Montant = 0,
                                MontantCrédit = selectF1.net,
                                NumCheque = Convert.ToString(selectF1.Id),
                                Num_Facture = selectF1.nfact,
                                RubriqueComptable = "ESPECES COMPTOIR",
                                Username = MemberUser.Username,

                            };

                            bool ii = false;
                            bool depense = false;
                            bool depaiement = false;
                            List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                            foreach (var item in comptoircalcu.listcomptoir)
                            {
                                if (item.quantite != 0)
                                {

                                    listsanszero.Add(item);
                                }
                            }
                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                ii = proxy.InsertFacture(selectF1, listsanszero, "Comptoir");

                                Operfacture = true;
                                proxy.InsertDepeiment(PAIEMENT);
                                depaiement = true;
                                proxy.InsertDepense(CAISSE);
                                depense = true;

                                if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                {
                                    ts.Complete();
                                }
                            }
                            if (Operfacture == true && ii == true && depaiement == true && depense == true)
                            {
                                proxy.AjouterProdflistRefresh(listrefresh);
                                proxy.AjouterSoldeF1Refresh();

                                this.comboclient.IsEnabled = true;
                                comptoircalcu.listcomptoir = new List<SVC.Facture>();
                                comptoircalcu.ReceptDatagrid.ItemsSource = comptoircalcu.listcomptoir;
                                comptoircalcu.digitalGaugeControl1.Content = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                                comptoircalcu.nbreproduit.Text = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Count());
                                if (comptoircalcu.chimprimer.IsChecked == true)
                                {

                                    var Nfact = proxy.GetAllF1ByCle(selectF1.cle);


                                    LocalReport reportViewer1 = new LocalReport();

                                    string nfact = Nfact.First().nfact.Substring(0, 1);
                                    string arrété = "";
                                    string document = "";
                                    switch (nfact)
                                    {
                                        case "F":
                                            arrété = "Arrétée la présente facture à la somme de";
                                            document = "Facture";
                                            break;
                                        case "A":
                                            arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                            document = "Facture d'Avoir";
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
                                            break;
                                        case "R":
                                            arrété = "Arrétée la présente facture à la somme de";
                                            document = "Facture";
                                            break;
                                    }

                                    /******************Dataset1 parametre****************************/
                                    var selpara = new List<SVC.Param>();
                                    selpara.Add((proxy.GetAllParamétre()));
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
                                    MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                                    reportViewer1.LoadReportDefinition(MyRptStream);
                                    /*************************************************/
                                    ReportDataSource rds = new ReportDataSource();
                                    rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                                          //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                                    rds.Value = Nfact;
                                    reportViewer1.DataSources.Add(rds);
                                    /**************************************************/
                                    var ClientList = new List<SVC.ClientV>();
                                    ClientList.Add(selectedclient);
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
                                    TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                                    ReportParameter paramTimeTo = new ReportParameter();
                                    paramTimeTo.Name = "TimeTo";
                                    paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                                    reportViewer1.SetParameters(paramTimeTo);
                                    reportViewer1.Refresh();

                                    Export(reportViewer1);
                                    Print();
                                }
                                comptoircalcu.IsEnabled = true;
                                comptoircalcu.Activate();
                                this.Close();
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

        private void annulerVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                comptoircalcu.IsEnabled = true;
                comptoircalcu.Activate();

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void DXWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.F12:
                        try
                        {
                            if (chcreit.IsChecked == true && comboclient.SelectedItem != null && MemberUser.ModuleVenteCompoirAcces == true)
                            {
                                SVC.ClientV selectedclient = comboclient.SelectedItem as SVC.ClientV;
                                if (selectedclient.Id != 0)
                                {
                                    SVC.F1 selectF1 = new SVC.F1
                                    {
                                        codeclient = selectedclient.Id,
                                        dates = DateTime.Now,
                                        date = datevente.SelectedDate,
                                        oper = MemberUser.Username,
                                        timbre = 0,
                                        bcom = "",
                                        echeance = 0,
                                        raison = selectedclient.Raison,

                                        heure = DateTime.Now.TimeOfDay,

                                        tva = 0,
                                        modep = "ESPECES",
                                        net = Convert.ToDecimal(txtnouveau.Text),
                                        ht = (comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),

                                        cle = selectedclient.Id + selectedclient.Raison + txtnouveau.Text + DateTime.Now.TimeOfDay,
                                    };
                                    if (txtremise.Text != "")
                                    {
                                        if (Convert.ToDecimal(txtremise.Text) != 0)
                                        {
                                            selectF1.remise = Convert.ToDecimal(txtremise.Text);
                                        }
                                        else
                                        {
                                            selectF1.remise = 0;
                                        }

                                    }
                                    else
                                    {
                                        selectF1.remise = 0;
                                    }
                                    if (txtversement.Text != "")
                                    {
                                        if (Convert.ToDecimal(txtversement.Text) != 0)
                                        {
                                            selectF1.versement = Convert.ToDecimal(txtversement.Text);
                                            selectF1.reste = selectF1.net - selectF1.versement;

                                        }
                                        else
                                        {
                                            selectF1.versement = 0;
                                            selectF1.reste = selectF1.net - selectF1.versement;

                                        }

                                    }
                                    else
                                    {
                                        selectF1.versement = 0;
                                        selectF1.reste = selectF1.net - selectF1.versement;

                                    }

                                    if (selectF1.reste != 0)
                                    {
                                        selectF1.soldé = false;
                                    }
                                    else
                                    {
                                        selectF1.soldé = true;

                                    }


                                    var remisepourfacture = selectF1.remise;
                                    bool Operfacture = false;

                                    List<int> listrefresh = new List<int>();

                                    foreach (SVC.Facture newfacture in comptoircalcu.listcomptoir)
                                    {
                                        newfacture.cle = selectF1.cle;
                                        newfacture.codeclient = selectF1.codeclient;
                                        newfacture.Client = selectF1.raison;
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
                                            listrefresh.Add(Convert.ToInt16(newfacture.ficheproduit));
                                        }
                                    }

                                    if (selectF1.versement != 0)
                                    {


                                        SVC.Depeiment PAIEMENT = new SVC.Depeiment
                                        {
                                            date = selectF1.date,
                                            montant = Convert.ToDecimal(selectF1.versement),
                                            paiem = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                            oper = MemberUser.Username,
                                            dates = selectF1.dates,
                                            banque = "Caisse",
                                            nfact = selectF1.nfact,
                                            amontant = Convert.ToDecimal(selectF1.net),
                                            cle = selectF1.cle,
                                            cp = selectF1.Id,
                                            Multiple = false,
                                            CodeClient = selectF1.codeclient,
                                            RaisonClient = selectF1.raison,

                                        };
                                        SVC.Depense CAISSE = new SVC.Depense
                                        {
                                            cle = selectF1.cle,
                                            Auto = true,
                                            Commentaires = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                            CompteDébité = "Caisse",
                                            Crédit = true,
                                            DateDebit = selectF1.date,
                                            DateSaisie = selectF1.dates,
                                            Débit = false,
                                            ModePaiement = "ESPECES",
                                            Montant = 0,
                                            MontantCrédit = selectF1.versement,
                                            NumCheque = Convert.ToString(selectF1.Id),
                                            Num_Facture = selectF1.nfact,
                                            RubriqueComptable = "ESPECES COMPTOIR",
                                            Username = MemberUser.Username,

                                        };
                                        bool ii = false;
                                        bool depense = false;
                                        bool depaiement = false;
                                        List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                        foreach (var item in comptoircalcu.listcomptoir)
                                        {
                                            if (item.quantite != 0)
                                            {
                                                listsanszero.Add(item);
                                            }
                                        }
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            ii = proxy.InsertFacture(selectF1, listsanszero, "B");

                                            Operfacture = true;
                                            proxy.InsertDepeiment(PAIEMENT);
                                            depaiement = true;
                                            proxy.InsertDepense(CAISSE);
                                            depense = true;

                                            if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                            {
                                                ts.Complete();
                                            }
                                        }
                                        if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                        {
                                            proxy.AjouterProdflistRefresh(listrefresh);
                                            proxy.AjouterSoldeF1Refresh();
                                            this.comboclient.IsEnabled = true;
                                            comptoircalcu.listcomptoir = new List<SVC.Facture>();
                                            comptoircalcu.ReceptDatagrid.ItemsSource = comptoircalcu.listcomptoir;
                                            comptoircalcu.digitalGaugeControl1.Content = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                                            comptoircalcu.nbreproduit.Text = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Count());
                                            if (comptoircalcu.chimprimer.IsChecked == true)
                                            {
                                                var Nfact = proxy.GetAllF1ByCle(selectF1.cle);
                                                LocalReport reportViewer1 = new LocalReport();
                                                Nfact.Add(selectF1);
                                                string nfact = Nfact.First().nfact.Substring(0, 1);
                                                string arrété = "";
                                                string document = "";
                                                switch (nfact)
                                                {
                                                    case "F":
                                                        arrété = "Arrétée la présente facture à la somme de";
                                                        document = "Facture";
                                                        break;
                                                    case "A":
                                                        arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                                        document = "Facture d'Avoir";
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
                                                        break;
                                                    case "R":
                                                        arrété = "Arrétée la présente facture à la somme de";
                                                        document = "Facture";
                                                        break;
                                                }

                                                /******************Dataset1 parametre****************************/
                                                var selpara = new List<SVC.Param>();
                                                selpara.Add((proxy.GetAllParamétre()));
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
                                                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                                                reportViewer1.LoadReportDefinition(MyRptStream);
                                                /*************************************************/
                                                ReportDataSource rds = new ReportDataSource();
                                                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                                                rds.Value = Nfact;
                                                reportViewer1.DataSources.Add(rds);
                                                /**************************************************/
                                                var ClientList = new List<SVC.ClientV>();
                                                ClientList.Add(selectedclient);
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
                                                TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                                                ReportParameter paramTimeTo = new ReportParameter();
                                                paramTimeTo.Name = "TimeTo";
                                                paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                                                reportViewer1.SetParameters(paramTimeTo);
                                                reportViewer1.Refresh();

                                                Export(reportViewer1);
                                                Print();
                                            }
                                            comptoircalcu.IsEnabled = true;
                                            comptoircalcu.Activate();
                                            this.Close();
                                        }
                                    }
                                    else
                                    {
                                        bool ii = false;
                                        List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                        foreach (var item in comptoircalcu.listcomptoir)
                                        {
                                            if (item.quantite != 0)
                                            {
                                                listsanszero.Add(item);
                                            }
                                        }
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            ii = proxy.InsertFacture(selectF1, listsanszero, "B");

                                            Operfacture = true;


                                            if (Operfacture == true && ii == true)
                                            {
                                                ts.Complete();
                                            }
                                        }
                                        if (Operfacture == true && ii == true)
                                        {
                                            proxy.AjouterProdflistRefresh(listrefresh);
                                            proxy.AjouterSoldeF1Refresh();
                                            this.comboclient.IsEnabled = true;
                                            comptoircalcu.listcomptoir = new List<SVC.Facture>();
                                            comptoircalcu.ReceptDatagrid.ItemsSource = comptoircalcu.listcomptoir;
                                            comptoircalcu.digitalGaugeControl1.Content= Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                                            comptoircalcu.nbreproduit.Text = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Count());
                                            if (comptoircalcu.chimprimer.IsChecked == true)
                                            {
                                                var Nfact = proxy.GetAllF1ByCle(selectF1.cle);
                                                LocalReport reportViewer1 = new LocalReport();
                                                Nfact.Add(selectF1);
                                                string nfact = Nfact.First().nfact.Substring(0, 1);
                                                string arrété = "";
                                                string document = "";
                                                switch (nfact)
                                                {
                                                    case "F":
                                                        arrété = "Arrétée la présente facture à la somme de";
                                                        document = "Facture";
                                                        break;
                                                    case "A":
                                                        arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                                        document = "Facture d'Avoir";
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
                                                        break;
                                                    case "R":
                                                        arrété = "Arrétée la présente facture à la somme de";
                                                        document = "Facture";
                                                        break;
                                                }

                                                /******************Dataset1 parametre****************************/
                                                var selpara = new List<SVC.Param>();
                                                selpara.Add((proxy.GetAllParamétre()));
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
                                                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                                                reportViewer1.LoadReportDefinition(MyRptStream);
                                                /*************************************************/
                                                ReportDataSource rds = new ReportDataSource();
                                                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                                                rds.Value = Nfact;
                                                reportViewer1.DataSources.Add(rds);
                                                /**************************************************/
                                                var ClientList = new List<SVC.ClientV>();
                                                ClientList.Add(selectedclient);
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
                                                TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                                                ReportParameter paramTimeTo = new ReportParameter();
                                                paramTimeTo.Name = "TimeTo";
                                                paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                                                reportViewer1.SetParameters(paramTimeTo);
                                                reportViewer1.Refresh();

                                                Export(reportViewer1);
                                                Print();
                                            }
                                            comptoircalcu.IsEnabled = true;
                                            comptoircalcu.Activate();
                                            this.Close();
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
                                if (chcreit.IsChecked == false && comboclient.SelectedItem != null)
                                {
                                    SVC.ClientV selectedclient = comboclient.SelectedItem as SVC.ClientV;
                                    if (selectedclient.Id == 0)
                                    {
                                        SVC.F1 selectF1 = new SVC.F1
                                        {
                                            codeclient = selectedclient.Id = 0,
                                            dates = DateTime.Now,
                                            date = datevente.SelectedDate,
                                            oper = MemberUser.Username,
                                            timbre = 0,
                                            bcom = "",
                                            echeance = 0,
                                            raison = selectedclient.Raison,
                                            soldé = true,
                                            heure = DateTime.Now.TimeOfDay,
                                            reste = 0,
                                            tva = 0,
                                            modep = "ESPECES",
                                            net = Convert.ToDecimal(txtnouveau.Text),
                                            ht = (comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),
                                            versement = Convert.ToDecimal(txtnouveau.Text),
                                            cle = selectedclient.Id + selectedclient.Raison + txtnouveau.Text + DateTime.Now.TimeOfDay,
                                        };
                                        if (txtremise.Text != "")
                                        {
                                            if (Convert.ToDecimal(txtremise.Text) != 0)
                                            {
                                                selectF1.remise = Convert.ToDecimal(txtremise.Text);
                                            }
                                            else
                                            {
                                                selectF1.remise = 0;
                                            }

                                        }
                                        else
                                        {
                                            selectF1.remise = 0;
                                        }
                                        var remisepourfacture = selectF1.remise;
                                        bool Operfacture = false;

                                        List<int> listrefresh = new List<int>();

                                        foreach (SVC.Facture newfacture in comptoircalcu.listcomptoir)
                                        {
                                            newfacture.cle = selectF1.cle;
                                            newfacture.codeclient = selectF1.codeclient;
                                            newfacture.Client = selectF1.raison;
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
                                                listrefresh.Add(Convert.ToInt16(newfacture.ficheproduit));
                                            }
                                        }
                                        SVC.Depeiment PAIEMENT = new SVC.Depeiment
                                        {
                                            date = selectF1.date,
                                            montant = Convert.ToDecimal(selectF1.net),
                                            paiem = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                            oper = MemberUser.Username,
                                            dates = selectF1.dates,
                                            banque = "Caisse",
                                            nfact = selectF1.nfact,
                                            amontant = Convert.ToDecimal(selectF1.net),
                                            cle = selectF1.cle,
                                            cp = selectF1.Id,
                                            Multiple = false,
                                            CodeClient = selectF1.codeclient,
                                            RaisonClient = selectF1.raison,

                                        };
                                        SVC.Depense CAISSE = new SVC.Depense
                                        {
                                            cle = selectF1.cle,
                                            Auto = true,
                                            Commentaires = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                                            CompteDébité = "Caisse",
                                            Crédit = true,
                                            DateDebit = selectF1.date,
                                            DateSaisie = selectF1.dates,
                                            Débit = false,
                                            ModePaiement = "ESPECES",
                                            Montant = 0,
                                            MontantCrédit = selectF1.net,
                                            NumCheque = Convert.ToString(selectF1.Id),
                                            Num_Facture = selectF1.nfact,
                                            RubriqueComptable = "ESPECES COMPTOIR",
                                            Username = MemberUser.Username,

                                        };

                                        bool ii = false;
                                        bool depense = false;
                                        bool depaiement = false;
                                        List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                                        foreach (var item in comptoircalcu.listcomptoir)
                                        {
                                            if (item.quantite != 0)
                                            {

                                                listsanszero.Add(item);
                                            }
                                        }
                                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            ii = proxy.InsertFacture(selectF1, listsanszero, "Comptoir");

                                            Operfacture = true;
                                            proxy.InsertDepeiment(PAIEMENT);
                                            depaiement = true;
                                            proxy.InsertDepense(CAISSE);
                                            depense = true;

                                            if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                            {
                                                ts.Complete();
                                            }
                                        }
                                        if (Operfacture == true && ii == true && depaiement == true && depense == true)
                                        {
                                            proxy.AjouterProdflistRefresh(listrefresh);
                                            proxy.AjouterSoldeF1Refresh();

                                            this.comboclient.IsEnabled = true;
                                            comptoircalcu.listcomptoir = new List<SVC.Facture>();
                                            comptoircalcu.ReceptDatagrid.ItemsSource = comptoircalcu.listcomptoir;
                                            comptoircalcu.digitalGaugeControl1.Content = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                                            comptoircalcu.nbreproduit.Text = Convert.ToString((comptoircalcu.listcomptoir).AsEnumerable().Count());
                                            if (comptoircalcu.chimprimer.IsChecked == true)
                                            {

                                                var Nfact = proxy.GetAllF1ByCle(selectF1.cle);


                                                LocalReport reportViewer1 = new LocalReport();

                                                string nfact = Nfact.First().nfact.Substring(0, 1);
                                                string arrété = "";
                                                string document = "";
                                                switch (nfact)
                                                {
                                                    case "F":
                                                        arrété = "Arrétée la présente facture à la somme de";
                                                        document = "Facture";
                                                        break;
                                                    case "A":
                                                        arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                                        document = "Facture d'Avoir";
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
                                                        break;
                                                    case "R":
                                                        arrété = "Arrétée la présente facture à la somme de";
                                                        document = "Facture";
                                                        break;
                                                }

                                                /******************Dataset1 parametre****************************/
                                                var selpara = new List<SVC.Param>();
                                                selpara.Add((proxy.GetAllParamétre()));
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
                                                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                                                reportViewer1.LoadReportDefinition(MyRptStream);
                                                /*************************************************/
                                                ReportDataSource rds = new ReportDataSource();
                                                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                                                rds.Value = Nfact;
                                                reportViewer1.DataSources.Add(rds);
                                                /**************************************************/
                                                var ClientList = new List<SVC.ClientV>();
                                                ClientList.Add(selectedclient);
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
                                                TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                                                ReportParameter paramTimeTo = new ReportParameter();
                                                paramTimeTo.Name = "TimeTo";
                                                paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                                                reportViewer1.SetParameters(paramTimeTo);
                                                reportViewer1.Refresh();

                                                Export(reportViewer1);
                                                Print();
                                            }
                                            comptoircalcu.IsEnabled = true;
                                            comptoircalcu.Activate();
                                            this.Close();
                                        }



                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                        }

                        break;
                    case Key.Escape:
                        comptoircalcu.IsEnabled = true;
                        comptoircalcu.Activate();
                        this.Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtremise_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DXWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                comptoircalcu.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtremise_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Subtract:

                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void chcreit_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                comboclient.IsEnabled = true;
                txtversement.IsEnabled = true;
                dtD.IsEnabled = true;
                btnPatient.IsEnabled = true;
                comboclient.ItemsSource = proxy.GetAllClientV().OrderBy(n => n.Raison);

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void chcreit_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                comboclient.IsEnabled = false;
                txtversement.IsEnabled = false;
                dtD.IsEnabled = false;
                btnPatient.IsEnabled = false;
                comboclient.ItemsSource = proxy.GetAllClientVBYID(0).OrderBy(n => n.Raison);
                comboclient.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true)
                {
                    NewClient CLMedecin = new NewClient(proxy, MemberUser, null);
                    CLMedecin.Show();

                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtversement_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Subtract:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

         

        private void txtremise_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtremise.Text != "")
                {

                    var net = Convert.ToDecimal((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                    if ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite) > 0)
                    {
                        if (Convert.ToDecimal(txtremise.Text) != 0)
                        {
                            if (((paramtetre.maxremisevente * net) / 100 >= Convert.ToDecimal(txtremise.Text)))
                            {
                                txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite) - Convert.ToDecimal(txtremise.Text))));
                                Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtremise.Text)));
                            }
                            else
                            {
                                txtremise.Text = "";
                                txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite))));
                                Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite))));
                            }
                        }
                        else
                        {
                            txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite))));
                            Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite))));
                        }
                    }
                    else
                    {
                        if ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite) < 0)
                        {
                            if (txtremise.Text.Count() > 1)
                            {
                                if (Convert.ToDecimal(txtremise.Text) != 0)
                                {
                                    //  if (((paramtetre.maxremisevente * net) / 100 >=  Convert.ToDecimal(txtremise.Text)))
                                    //{
                                    txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite) - Convert.ToDecimal(txtremise.Text))));
                                    Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)) - Convert.ToDecimal(txtremise.Text)));
                                    //}
                                    /*else
                                    {
                                        txtremise.Text = "";
                                        txtnouveau.Text = Convert.ToString(((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                        Bénéficemont.Text = Convert.ToString(((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite)));
                                    }*/
                                }
                                else
                                {
                                    //  txtremise.Text = "";
                                    txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite))));
                                    Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite))));
                                }
                            }
                            else
                            {
                                if (txtremise.Text.Count() == 1)
                                {
                                    var letter = txtremise.Text.Substring(0, 1);
                                    if (letter != "-")
                                    {
                                        txtremise.Text = "";
                                        txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite))));
                                        Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite))));
                                    }

                                }
                            }
                        }
                    }



                }
                else
                {
                    Bénéficemont.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)) - ((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.previent * o.quantite))));
                    txtnouveau.Text = String.Format("{0:0.##}", (((comptoircalcu.listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite))));

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
