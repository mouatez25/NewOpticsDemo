﻿
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

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for ImpressionAMS.xaml
    /// </summary>
    public partial class ImpressionAMS : Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionAMS();
        public ImpressionAMS(SVC.ServiceCliniqueClient proxyrecu, List<SVC.am> listeDepaeifrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;

                var selpara = new List<SVC.Param>();
                selpara.Add((proxy.GetAllParamétre()));
                reportViewer1.LocalReport.EnableExternalImages = true;

                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet2", listeDepaeifrecu));

             
                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.ReportAMS), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionAMS(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionAMS(HandleProxy));
                return;
            }
            HandleProxy();
        }


    }
    }
