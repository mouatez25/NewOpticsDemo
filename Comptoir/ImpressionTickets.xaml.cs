
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Reporting.WinForms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.IO;

using System.Windows.Threading;
using Nut;

namespace NewOptics.Comptoir
{
    /// <summary>
    /// Interaction logic for ImpressionTickets.xaml
    /// </summary>
    public partial class ImpressionTickets : Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionTicket();
        public ImpressionTickets(SVC.ServiceCliniqueClient proxyrecu, List<SVC.F1> Nfact, SVC.ClientV CLIENRECU)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                /******************************/
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

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                /*************************************************/
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                rds.Value = Nfact;
                this.reportViewer1.LocalReport.DataSources.Add(rds);
                /**************************************************/
                var ClientList = new List<SVC.ClientV>();
                ClientList.Add(CLIENRECU);
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", ClientList));
                /********************************************/
                var FactureList = proxy.GetAllFactureBycompteur(Nfact.First().nfact);

                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", FactureList.ToList()));
                /*********************************************/

                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                /********ImagePath************************************/
                reportViewer1.LocalReport.EnableExternalImages = true;
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);
                /**************************************************************/
                ReportParameter paramDocument = new ReportParameter();
                paramDocument.Name = "Document";
                paramDocument.Values.Add(document);
                reportViewer1.LocalReport.SetParameters(paramDocument);
                /*************************************************************/
                ReportParameter paramArrettee = new ReportParameter();
                paramArrettee.Name = "Arrettee";
                paramArrettee.Values.Add(arrété);
                reportViewer1.LocalReport.SetParameters(paramArrettee);
                /*************************************************/
                ReportParameter paramMontantString = new ReportParameter();
                paramMontantString.Name = "MontantString";
                paramMontantString.Values.Add(Montant);
                reportViewer1.LocalReport.SetParameters(paramMontantString);
                /*********************************************************/
                TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);
                
                
                ReportParameter paramTimeTo = new ReportParameter();
                paramTimeTo.Name = "TimeTo";
                paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                reportViewer1.LocalReport.SetParameters(paramTimeTo);
                /************************************/
                reportViewer1.RefreshReport();
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionTicket(HandleProxy));
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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionTicket(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
