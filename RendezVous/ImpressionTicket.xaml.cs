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

namespace GestionClinique.RendezVous
{
    /// <summary>
    /// Interaction logic for ImpressionTicket.xaml
    /// </summary>
    public partial class ImpressionTicket : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerTicket();
       
        public ImpressionTicket(SVC.ServiceCliniqueClient proxyrecu,SVC.RendezVou rendezticket)
        {
            try
            { 
            InitializeComponent();
          //  rendezvousimpr = rendezticket;

           // rendezvousimpr.NuméroRendezVous = "*" + rendezticket.NuméroRendezVous + "*";
            var people = new List<SVC.RendezVou>();
            people.Add(rendezticket);

                // datable = datatablerecu;
                proxy = proxyrecu;
   
                MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.ReportRendezVous), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                // reportViewer1.LocalReport.ReportPath = "../../Patient/ReportOnePatient.rdlc";


                var selmede = new List<SVC.Param>();
            selmede.Add((proxy.GetAllParamétre()));
            //    var selexc = new List<ExclusionDay>();
            //   selexc.Add(selectexclusion);
            //Add datasets
            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", people));
            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet2", selmede));
                reportViewer1.LocalReport.EnableExternalImages = true;
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);
                reportViewer1.RefreshReport();
            //proxy = proxyrecu;
            proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

            proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerTicket(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerTicket(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
