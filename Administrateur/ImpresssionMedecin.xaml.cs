
using GestionClinique.SVC;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for ImpresssionMedecin.xaml
    /// </summary>
    public partial class ImpresssionMedecin : Page
    {

        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvoker();
        public ImpresssionMedecin(SVC.ServiceCliniqueClient proxy1, List<Medecin> testrecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxy1;
                // datable = datatablerecu;
                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));
                // reportViewer1.LocalReport.ReportPath = "../../Administrateur/ReportMedecin.rdlc";

           
                MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.ReportMedecin), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSetMedecin", testrecu));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet2", selpara));


                /*   ReportDataSource rds = new ReportDataSource();
                   rds.Name = "DataSetMedecin";//This refers to the dataset name in the RDLC file
                                                  // rds.Value = proxy1.GetAllMembership();
                   rds.Value = testrecu;
                   this.reportViewer1.LocalReport.DataSources.Add(rds);*/

                reportViewer1.LocalReport.EnableExternalImages = true;
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
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

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        var wnd = Window.GetWindow(this);
                        //  wnd.Close();
                        Grid test = (Grid)wnd.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;

                        Grid tests = (Grid)wnd.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
