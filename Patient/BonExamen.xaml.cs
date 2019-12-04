using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for BonExamen.xaml
    /// </summary>
    public partial class BonExamen : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerBon();
        ICallback callback;
        ExamenCom pp;
        SVC.Patient Patient;
        ExamenRadiologie popol;
        public BonExamen(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrecu,SVC.Patient patientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                Patient = patientrecu;
                pp = new ExamenCom
                {
                    ACIDEURIQUE=false,
                    CHOHDLLDL = false,
                    FERSER = false,
                    VS = false,
                    CHOLTOTAL = false,
                    COPRO = false,
                    CREA = false,
                    GLYC = false,
                    ECBUATB = false,
                    ELECHB = false,
                    GroupageRhésus = false,
                    FNSEQUILIBRE = false,
                    IONOGR = false,
                    SEROL = false,
                    T3T4TSH = false,
                    Triglycerides = false,
                    UREE = false,
                };
                InfGénéral.DataContext = pp;
                popol = new ExamenRadiologie
                {
                    Echographie=false,
                    Rachis=false,
                    Téléthorax=false,
                };
                GridRadioExamen.DataContext = popol;
            }
            catch(Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Logiciel, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerBon(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerBon(HandleProxy));
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

        private void btnImprimerVisite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ExamenCom> liststring = new List<ExamenCom>();
                liststring.Add(pp);
                string textbb;
                if(txtAutre.Text.Trim()!="")
                {
                    textbb = txtAutre.Text;
                }
                else
                {
                    textbb = " ";
                }
                
                ImpressionBonExamen cl = new ImpressionBonExamen(proxy,Patient,liststring,textbb);
                cl.Show();

            }catch(Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Logiciel, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimerRadio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ExamenRadiologie> liststring = new List<ExamenRadiologie>();
                liststring.Add(popol);
                string textbb;
              
                if(txtAutrerad.Text.Trim()!="")
                {
                    textbb = txtAutrerad.Text;
                }
                else
                {
                    textbb = " ";
                }
                ImpressionExamenRadio cl = new ImpressionExamenRadio(proxy, Patient, liststring, textbb);
                cl.Show();

            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Logiciel, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
