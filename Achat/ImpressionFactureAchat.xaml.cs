
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace NewOptics.Achat
{
    /// <summary>
    /// Interaction logic for ImpressionFactureAchat.xaml
    /// </summary>
    public partial class ImpressionFactureAchat : Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionfactureAchat();
        public ImpressionFactureAchat(SVC.ServiceCliniqueClient proxyrecu,SVC.Recouf listerecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;

                /*   ResourceManager ^ rm = gcnew ResourceManager("NewTest_EmbedRpt.MyResources", GetType()->Assembly);
                   MemoryStream ^ MyRptStream = gcnew MemoryStream(static_cast < array<Byte> ^> (rm->GetObject("Report1")), false);
                   reportViewer1->LocalReport->LoadReportDefinition(MyRptStream);



                   MemoryStream memLicStream2 = new MemoryStream(ASCIIEncoding.Default.GetBytes(NewOptics.Properties.Resources.OneFactureAchat));

                   //   this.richTextBox2.Selection.Load(memLicStream2, System.Windows.DataFormats.Rtf);*/
                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.OneFactureAchat), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);

                // reportViewer1.LocalReport.ReportPath = "Rapport/OneFactureAchat.rdlc";
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                var selpararecept = new List<SVC.Recouf>();
                selpararecept.Add((listerecu));
                rds.Value = selpararecept;
                this.reportViewer1.LocalReport.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                reportViewer1.LocalReport.EnableExternalImages = true;

                //SVC.Recouf tt = (proxy.GetAllRecouf()).First();
                List<SVC.Recept> receptlist = (proxy.GetAllRecept().Where(n => n.Fournisseur == listerecu.Fournisseur && n.nfact == listerecu.nfact && n.cf == listerecu.codef)).ToList();
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", receptlist));
                ReportParameter paramLogo = new ReportParameter();
                paramLogo.Name = "ImagePath";
                String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                paramLogo.Values.Add(@"file:///" + photolocation);
                reportViewer1.LocalReport.SetParameters(paramLogo);
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionfactureAchat(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionfactureAchat(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
