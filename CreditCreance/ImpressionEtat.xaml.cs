
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Microsoft.Reporting.WinForms;
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
    /// Interaction logic for ImpressionEtat.xaml
    /// </summary>
    public partial class ImpressionEtat : Window
    {
        NewOptics.SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionEtat();
        public ImpressionEtat(SVC.ServiceCliniqueClient proxyrecu, List<SVC.F1> listerecu, DateTime datedebut, DateTime datefin)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                List<SVC.F1> listf1modi = new List<SVC.F1>();
                foreach (SVC.F1 itemf1 in listerecu)
                {
                    listf1modi.Add(itemf1);
                    var okmultiple = (proxy.GetAllDepeiementMultiple()).FindAll(n => n.cleVisite == itemf1.cle);
                    List<SVC.Depeiment> depaiemlist;
                    SVC.Depeiment DepeimentMultiple;
                    depaiemlist = (proxy.GetAllDepeiment()).Where(n => n.cle == itemf1.cle).ToList();
                    foreach (SVC.Depeiment itemdep in depaiemlist)
                    {
                        SVC.F1 ff = new SVC.F1
                        {
                            nfact = "Versement",
                            date = itemdep.date,
                            versement = itemdep.montant,
                            raison = itemdep.paiem,

                        };
                        listf1modi.Add(ff);
                    }
                    if (okmultiple.Count > 0)
                    {
                        foreach (SVC.DepeiementMultiple itemmultiple in okmultiple)
                        {
                            DepeimentMultiple = proxy.GetAllDepeiment().Find(n => n.CleMultiple == itemmultiple.cleMultiple);
                            if (!depaiemlist.Contains(DepeimentMultiple) && DepeimentMultiple.Multiple == true)
                            {
                                depaiemlist.Add(DepeimentMultiple);
                                SVC.F1 ff = new SVC.F1
                                {
                                    nfact = "Versement",
                                    date = itemmultiple.date,
                                    versement = itemmultiple.montant,
                                    raison = itemmultiple.paiem,

                                };
                                listf1modi.Add(ff);
                            }
                        }
                    }
                }


                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.EtatClientVersement), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();

                rds.Value = listf1modi;
                this.reportViewer1.LocalReport.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));

                reportViewer1.LocalReport.EnableExternalImages = true;

                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);
                /************************/
                ReportParameter paramDateDebut = new ReportParameter();
                paramDateDebut.Name = "DateDebut";
                paramDateDebut.Values.Add(datedebut.Date.ToString());
                reportViewer1.LocalReport.SetParameters(paramDateDebut);
                /****************************/
                /************************/
                ReportParameter paramDateFin = new ReportParameter();
                paramDateFin.Name = "DateFin";
                paramDateFin.Values.Add(datefin.Date.ToString());
                reportViewer1.LocalReport.SetParameters(paramDateFin);
                /****************************/
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionEtat(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionEtat(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
