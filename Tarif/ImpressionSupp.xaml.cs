
using System;
using System.Collections.Generic;
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
using Microsoft.Reporting.WinForms;
using System.IO;

namespace NewOptics.Tarif
{
    /// <summary>
    /// Interaction logic for ImpressionSupp.xaml
    /// </summary>
    public partial class ImpressionSupp : Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionSupp();
        public ImpressionSupp(SVC.ServiceCliniqueClient proxyrecu, List<SVC.Supplement> Nfact)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;

                /******************Dataset1 parametre****************************/
                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));

                /************************************/
                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.ReportSupplement), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                rds.Value = Nfact;
                this.reportViewer1.LocalReport.DataSources.Add(rds);


                /*********************************************/

                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                /********ImagePath************************************/
                reportViewer1.LocalReport.EnableExternalImages = true;
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionSupp(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionSupp(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
