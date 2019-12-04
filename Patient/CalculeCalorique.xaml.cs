using DevExpress.Xpf.Core;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
    /// Interaction logic for CalculeCalorique.xaml
    /// </summary>
    public partial class CalculeCalorique : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership Memberuser;
        private delegate void FaultedInvokerCalculeCalorique();
        SVC.Patient PATIENT;
        SVC.BesoinCalorique BesoinPatient;
        public CalculeCalorique(SVC.ServiceCliniqueClient proxyrecu,SVC.Membership memberrecu,SVC.Patient patienrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                Memberuser = memberrecu;
                PATIENT = patienrecu;
                BesoinPatient = new SVC.BesoinCalorique
                {
                    CodePatient = PATIENT.Id,
                    NomPatient = PATIENT.Nom,
                    PrénomPatient = PATIENT.Prénom,
                    Date = DateTime.Now,
                    RepasBoolPetitDéj = false,
                    RepasBoolCollation = false,
                    RepasBoolDéjeuner = false,
                    RepasBoolGouter = false,
                    RepasBoolDiner = false,
                    RepasBoolLesoir = false,
                    ProtéinesPourcentage = 15,
                    LipidesPourcentage = 30,
                    GlucidesPourcentage = 55,
                    PetitDéjPourcentage = 15,
                    CollationPourcentage = 5,
                    DéjeunerPourcentage = 30,
                    GouterPourcentage = 10,
                    DinerPourcentage = 30,
                    LesoirPourcentage = 10,
                    ProtéinesPourcentageNew = 0,
                    LipidesPourcentageNew = 0,
                    GlucidesPourcentageNew = 0,
                    ProtéinesNewValeur = 0,
                    LipidesNewValeur = 0,
                    GlucidesNewValeur = 0,
                    Kcal = 0,
                    ProtéinesCollation = 0,
                    ProtéinesDiner = 0,
                    ProtéinesDéjeuner = 0,
                    ProtéinesGouter = 0,
                    ProtéinesLesoir = 0,
                    ProtéinesPetitDéj=0,

                    GlucidesCollation=0,
                    GlucidesDiner=0,
                    GlucidesDéjeuner=0,
                    GlucidesGouter=0,
                    GlucidesLesoir=0,
                    GlucidesPetitDéj=0,

                    LipidesCollation=0,
                    LipidesDiner=0,
                    LipidesDéjeuner=0,
                    LipidesGouter=0,
                    LipidesLesoir=0,
                    LipidesPetitDéj=0,
            };
                AlimentGrid.DataContext = BesoinPatient;
                //btnCreer.IsEnabled = false;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCalculeCalorique(HandleProxy));
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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCalculeCalorique(HandleProxy));
                return;
            }
            HandleProxy();
        }
       

        private void txtPoids_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void txtAge_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                case Key.Decimal:


                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
        bool ouirempli()
        {
            if (txtPoids.Text != "" && txtAge.Text != "" && ActivitéCombo.SelectionBoxItem.ToString() != "" && txtDate.SelectedDate != null && (radioSexeHomme.IsChecked == true || radioSexeFemme.IsChecked == true))
            {
                btnCreer.IsEnabled = true;
                return true;
            }
            else
            {
                btnCreer.IsEnabled = false;
                return false;
            }
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            if (radioSexeFemme.IsChecked == true && ouirempli())
                {
                  
                        var testMB = 230 * (Math.Pow(Convert.ToDouble(txtPoids.Text), 0.48)) * (Math.Pow((Convert.ToDouble(txtTaille.Text)* 0.01), 0.5)) * (Math.Pow(Convert.ToDouble(txtAge.Text), -0.13));
                      string caseSwitch = (ActivitéCombo.SelectionBoxItem.ToString()).Trim();
                        double testDEJ;
                       switch (caseSwitch)
                         {
                             case "Activité sédentaire":
                                 testDEJ = testMB * 1.2;

                                 break;
                             case "Activité légère":
                                 testDEJ = testMB * 1.37;
                                 break;
                             case "Activité modérée":
                                 testDEJ = testMB * 1.55;
                                 break;
                             case "Activité importante":
                                 testDEJ = testMB * 1.725;
                                 break;
                             case "Activité intense":
                                 testDEJ = testMB * 1.9;
                                 break;
                             default:
                        testDEJ = testMB * 1.2;
                      break;
                 }
                        BesoinPatient.MB = Convert.ToDecimal(testMB);
                        BesoinPatient.DEJ = Convert.ToDecimal(testDEJ);
                        BesoinPatient.Sexe = "femme";
              
                       BesoinPatient.Clé = Convert.ToString(PATIENT.Id) + Convert.ToString(BesoinPatient.Date) + BesoinPatient.Taille + BesoinPatient.Poids + BesoinPatient.MB + BesoinPatient.DEJ + BesoinPatient;
                      
                        proxy.InsertBesoinCaloriqueAsync(BesoinPatient,PATIENT.Id);
                       
                }
                else
                {
                    if (radioSexeHomme.IsChecked == true && ouirempli())
                    {
                        var testMB = 259 * (Math.Pow(Convert.ToDouble(txtPoids.Text), 0.48)) * (Math.Pow((Convert.ToDouble(txtTaille.Text) * 0.01), 0.5)) * (Math.Pow(Convert.ToDouble(txtAge.Text), -0.13));
                       string caseSwitch = ActivitéCombo.SelectionBoxItem.ToString();
                        double testDEJ;
                        switch (caseSwitch)
                        {
                            case "Activité sédentaire":
                                testDEJ = testMB * 1.2;

                                break;
                            case "Activité légère":
                                testDEJ = testMB * 1.37;
                                break;
                            case "Activité modérée":
                                testDEJ = testMB * 1.55;
                                break;
                            case "Activité importante":
                                testDEJ = testMB * 1.725;
                                break;
                            case "Activité intense":
                                testDEJ = testMB * 1.9;
                                break;
                            default:
                                testDEJ = testMB * 1.2;
                             break;
                        }

                        BesoinPatient.MB = Convert.ToDecimal(testMB);
                        BesoinPatient.DEJ = Convert.ToDecimal(testDEJ);
                        BesoinPatient.Sexe = "homme";
                       BesoinPatient.Clé = Convert.ToString(PATIENT.Id) + Convert.ToString(BesoinPatient.Date) + BesoinPatient.Taille + BesoinPatient.Poids + BesoinPatient.MB + BesoinPatient.DEJ + BesoinPatient;
                      
                        proxy.InsertBesoinCaloriqueAsync(BesoinPatient,PATIENT.Id);
                        MessageBoxResult resultc1D = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
