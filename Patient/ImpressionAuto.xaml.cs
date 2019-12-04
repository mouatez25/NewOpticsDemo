using DevExpress.Xpf.Core;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for ImpressionAuto.xaml
    /// </summary>
    public partial class ImpressionAuto : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionAuto();
        public ImpressionAuto(int interfacerec, SVC.ServiceCliniqueClient proxyrecu, SVC.Patient patientrecu, SVC.autosurveillance autorecu, List<SVC.autosurveillanceDetail> autodetailrecu, int nbpage)
        {
            try
            {
                InitializeComponent();

                var listpatient = new List<SVC.Patient>();
                listpatient.Add(patientrecu);

                var listautosurveillance = new List<SVC.autosurveillance>();
                listautosurveillance.Add(autorecu);

                proxy = proxyrecu;
                // datable = datatablerecu;

               

                    if (nbpage == 4)
                    {
                        MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.quatreSemaineParPage), false);

                        reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                    } else
                    {
                        if (nbpage == 1)
                        {
                        MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.UneSemaineParPage), false);

                        reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                    } else
                        {
                            if (nbpage == 2)
                            {
                                MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.DeuxSemaineParpage), false);

                                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                            }
                        }
                    }

              

                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet2", listpatient));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", listautosurveillance));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", autodetailrecu));



                reportViewer1.LocalReport.EnableExternalImages = true;


                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);
                if (interfacerec == 1)
              {
                ReportParameter paramType = new ReportParameter();
                    paramType.Name = "TypeD";

                    paramType.Values.Add("Type 1, traitement par insuline");
                reportViewer1.LocalReport.SetParameters(paramType);
                    ReportParameter paramTypetr = new ReportParameter();
                    paramTypetr.Name = "TraitementGrid";

                    paramTypetr.Values.Add("Insuline");
                    reportViewer1.LocalReport.SetParameters(paramTypetr);
                }else
                {
                    if (interfacerec == 2)
                    {
                        ReportParameter paramType = new ReportParameter();
                        paramType.Name = "TypeD";

                        paramType.Values.Add("Type 2, traitement par antidiabétiques oraux");
                        reportViewer1.LocalReport.SetParameters(paramType);
                        ReportParameter paramTypetr = new ReportParameter();
                        paramTypetr.Name = "TraitementGrid";

                        paramTypetr.Values.Add("Traitement");
                        reportViewer1.LocalReport.SetParameters(paramTypetr);
                    }
                }

                reportViewer1.RefreshReport();

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionAuto(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionAuto(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
