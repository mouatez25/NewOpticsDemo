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
    /// Interaction logic for ImpressionDiagnostic.xaml
    /// </summary>
    public partial class ImpressionDiagnostic : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionDiagnostic();
        public ImpressionDiagnostic(SVC.ServiceCliniqueClient proxyrecu,SVC.Patient patientrecu,List<SVC.Diagnostic> diagnosticrecu,int interfacerecu)
        {
            try
            {



                InitializeComponent();

                proxy = proxyrecu;
                // datable = datatablerecu;
              
                if(interfacerecu==1)
                {
                    MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.DiagnosticDiab), false);

                    reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                }
                else
                {
                    if(interfacerecu==2)
                    {
                        MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.ConsultationClinique), false);

                        reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                    }
                }
                  
                List<SVC.Patient> listpatient = new List<SVC.Patient>();
                listpatient.Add(patientrecu);
               
                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));
                reportViewer1.LocalReport.EnableExternalImages = true;

                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet2", listpatient));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", diagnosticrecu));
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionDiagnostic(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionDiagnostic(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
