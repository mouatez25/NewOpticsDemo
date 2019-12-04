using DevExpress.Xpf.Core;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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
    /// Interaction logic for ImpressionOrdonnance.xaml
    /// </summary>
    public partial class ImpressionOrdonnance : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;

        private delegate void FaultedInvokerImpressionOrdonnance();
        public ImpressionOrdonnance(SVC.ServiceCliniqueClient proxyrecu, List<SVC.OrdonnancePatient> OrdonnancePatientRecu,SVC.Patient PatientRecu,SVC.Medecin SelectMedecinListe,SVC.EnteteOrdonnace selectedEnteteOrdonnance,bool a4)
        {
            try
            {
                InitializeComponent();

                var people = new List<SVC.Patient>();
                people.Add(PatientRecu);
                proxy = proxyrecu;
          
                var Entetemededcin= new List<SVC.EnteteOrdonnace>();
                Entetemededcin.Add(selectedEnteteOrdonnance);
                if (a4 == false)
                {
                    MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.Ordonnance), false);

                    reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                }
                else
                {
                    if (a4 == true)
                    {
                        MemoryStream MyRptStream = new MemoryStream((GestionClinique.Properties.Resources.OrdonnanceA4), false);

                        reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                    }
                }
                        

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                rds.Value = people;
                this.reportViewer1.LocalReport.DataSources.Add(rds);
                var selpara = new List<SVC.Param>();
                SVC.Param paramlocal = (proxy.GetAllParamétre());
                paramlocal.CheminLogo="D/Logo.jpg";
                selpara.Add((paramlocal));

                var peoplemededcin = new List<SVC.Medecin>();
                peoplemededcin.Add(SelectMedecinListe);
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", OrdonnancePatientRecu));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", peoplemededcin));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet5", Entetemededcin));
                if (proxy.GetAllParamétre().CheminLogo != "")
                {

                    if (proxy.DownloadDocumentIsHere(proxy.GetAllParamétre().CheminLogo.ToString()) == true)
                    {
                        reportViewer1.LocalReport.EnableExternalImages = true;
                        /*  var image = LoadImage(proxy.DownloadDocument(proxy.GetAllParamétre().CheminLogo.ToString()));
                          JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                          Guid photoID = System.Guid.NewGuid();

                          encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));
                          using (var filestream = new FileStream(photolocation, FileMode.Create))
                          encoder.Save(filestream);*/
                        String photolocation = System.Environment.CurrentDirectory + "/Logo.png";
                        ReportParameter paramLogo = new ReportParameter();
                        paramLogo.Name = "ImagePath";
                        //  paramLogo.Values.Add(@"file:///C:\Logo.png");
                        paramLogo.Values.Add(@"file:///"+photolocation);
                        reportViewer1.LocalReport.SetParameters(paramLogo);
                       // reportViewer1.LocalReport.SetParameters(parameter);
                    }
                   
                }


               
           
                reportViewer1.RefreshReport();

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public void SaveImage(BitmapImage image, string localFilePath)
        {
            image.DownloadCompleted += (sender, args) =>
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)sender));
                using (var filestream = new FileStream(localFilePath, FileMode.Create))
                {
                    encoder.Save(filestream);
                }
            };
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionOrdonnance(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionOrdonnance(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}
