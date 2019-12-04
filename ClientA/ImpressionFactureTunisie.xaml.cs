
using NewOptics.Administrateur;
using Microsoft.Reporting.WinForms;
using Nut;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for ImpressionFactureTunisie.xaml
    /// </summary>
    public partial class ImpressionFactureTunisie : Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionDocumentTunisie();
        public ImpressionFactureTunisie(SVC.ServiceCliniqueClient proxyrecu, List<SVC.F1> Nfact, SVC.ClientV CLIENRECU, int interfaceimm)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
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

                        reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
               
                    
                }
                else
                {
                    if (interfaceimm == 1)
                    {
                        if (selectpara.FactureSansTva == false )
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

                            reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);


                        }
                        else
                        {
                            if (selectpara.FactureSansTva == true )
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

                                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                            }
                           
                        }
                    }
                }

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
                /*****************dataset 5***************/
                List<TVACLASS> listrva = new List<TVACLASS>();
                foreach (var facture in FactureList)
                {
                      if (listrva.Any(n=>n.TVA==facture.tva)==false)
                    {
                    TVACLASS tt = new TVACLASS
                    {
                        TVA = Convert.ToDecimal(facture.tva),
                        BASE = Convert.ToDecimal(facture.privente * facture.quantite),
                        MONTANT = Convert.ToDecimal(((facture.privente * facture.quantite) * facture.tva) / 100),
                    };
                        if (tt.TVA!=0)
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
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", listrva));

                /********************************/
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
                reportViewer1.LocalReport.SetParameters(paramSoldeClient);
                /*********************************************************/
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionDocumentTunisie(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionDocumentTunisie(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
