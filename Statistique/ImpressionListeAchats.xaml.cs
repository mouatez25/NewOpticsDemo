﻿
using Microsoft.Reporting.WinForms;
using NewOptics.Statistique;
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

namespace NewOptics.Statistique
{
    /// <summary>
    /// Interaction logic for ImpressionListeAchats.xaml
    /// </summary>
    public partial class ImpressionListeAchats :Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerImpressionlisteAchats();
        public ImpressionListeAchats(SVC.ServiceCliniqueClient proxyrecu, List<receptcalcule> listerecu, DateTime datedebut, DateTime datefin,string fiche,string achat,string stock)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                // datable = datatablerecu;

                //  reportViewer1.LocalReport.ReportPath = "../../Stock/OneFactureAchat.rdlc";

                MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.AchatsProduits), false);

                reportViewer1.LocalReport.LoadReportDefinition(MyRptStream);
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                      //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();

                rds.Value = listerecu;
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
                /************************/
                ReportParameter paramfiche = new ReportParameter();
                paramfiche.Name = "TotalFIche";
                paramfiche.Values.Add(fiche.ToString());
                reportViewer1.LocalReport.SetParameters(paramfiche);
                /****************************/
                ReportParameter paramachat = new ReportParameter();
                paramachat.Name = "TotalAchat";
                paramachat.Values.Add(achat.ToString());
                reportViewer1.LocalReport.SetParameters(paramachat);
                /***********************************/
                /****************************/
                ReportParameter paramaTotalStock = new ReportParameter();
                paramaTotalStock.Name = "Totsto";
                paramaTotalStock.Values.Add(stock.ToString());
                reportViewer1.LocalReport.SetParameters(paramaTotalStock);
                /*********************************************/
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionlisteAchats(HandleProxy));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerImpressionlisteAchats(HandleProxy));
                return;
            }
            HandleProxy();
        }
    }
}