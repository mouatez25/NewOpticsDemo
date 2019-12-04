using DevExpress.Xpf.Core;
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
    /// Interaction logic for VisiteAs.xaml
    /// </summary>
    public partial class VisiteAs : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Diagnostic dia;
        private delegate void FaultedInvokerVisite();
        DossierPatient doss;
        SVC.Constante constantep;
        public VisiteAs(SVC.ServiceCliniqueClient proxyrecu, SVC.Diagnostic diarecu, DossierPatient dossierrecu, SVC.Constante constanterecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                dia = diarecu;
                constantep = constanterecu;
                txtIMDC.DataContext = diarecu;
                txtCardiAdulte.DataContext = diarecu;
                txtYeux.DataContext = diarecu;
                txtCards.DataContext = diarecu;
                DatePriseAnamnese.SelectedDate = DateTime.Now;
                ConstaneAnamneseDétailGrid.DataContext = constanterecu;
                dossierrecu.IsEnabled = false;
                doss = dossierrecu;
                proxy.InnerDuplexChannel.Opened += new EventHandler(InnerDuplexChannel_Opened);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Logiciel, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVisite(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVisite(HandleProxy));
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

        private void txtTaile_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTaile.Text != "" && txtPois.Text != "")
                {
                    if (txtTaile.Text != "0" && txtPois.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        ValeurImc.Content = string.Format("{0:0.##}", Convert.ToDecimal(txtPois.Text) / (Convert.ToDecimal(txtTaile.Text) * Convert.ToDecimal(txtTaile.Text)));
                    }
                    else
                    {
                        ValeurImc.Content = "0";

                    }
                }
                else
                {
                    ValeurImc.Content = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void txtTaile_KeyDown(object sender, KeyEventArgs e)
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

        private void txtPois_KeyDown(object sender, KeyEventArgs e)
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

        private void txtPois_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTaile.Text != "" && txtPois.Text != "")
                {
                    if (txtTaile.Text != "0" && txtPois.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        ValeurImc.Content = string.Format("{0:0.##}", Convert.ToDecimal(txtPois.Text) / (Convert.ToDecimal(txtTaile.Text) * Convert.ToDecimal(txtTaile.Text)));
                    }
                    else
                    {
                        ValeurImc.Content = "0";

                    }
                }
                else
                {
                    ValeurImc.Content = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void txtPoid_KeyDown(object sender, KeyEventArgs e)
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

        private void txtPoidAnamnese_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTailleAnamnese.Text != "" && txtPoidAnamnese.Text != "")
                {
                    if (txtTailleAnamnese.Text != "0" && txtPoidAnamnese.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        txtIMCAnamnese.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoidAnamnese.Text) / (Convert.ToDecimal(txtTailleAnamnese.Text) * Convert.ToDecimal(txtTailleAnamnese.Text)));
                    }
                    else
                    {
                        txtIMCAnamnese.Text = "0";
                    }
                }
                else
                {
                    txtIMCAnamnese.Text = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void txtTailleAnamnese_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtTailleAnamnese.Text != "" && txtPoidAnamnese.Text != "")
                {
                    if (txtTailleAnamnese.Text != "0" && txtPoidAnamnese.Text != "0")
                    {
                        //  txtIMC.Text = string.Format("{0:0.##}", Convert.ToString(Convert.ToDecimal(txtPoid.Text) / (Convert.ToDecimal(txtTaille.Text) * Convert.ToDecimal(txtTaille.Text))));
                        txtIMCAnamnese.Text = string.Format("{0:0.##}", Convert.ToDecimal(txtPoidAnamnese.Text) / (Convert.ToDecimal(txtTailleAnamnese.Text) * Convert.ToDecimal(txtTailleAnamnese.Text)));
                    }
                    else
                    {
                        txtIMCAnamnese.Text = "0";

                    }
                }
                else
                {
                    txtIMCAnamnese.Text = "0";
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private void txtTailleAnamnese_KeyDown(object sender, KeyEventArgs e)
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

        private void VisteAssisteeWizard_Finish(object sender, System.ComponentModel.CancelEventArgs e)
        {
            doss.IsEnabled = true;
        }

        private void DXWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                doss.IsEnabled = true;
                List<string> examen = new List<string>();
                if (DatePriseAnamnese.SelectedDate!=null)
                {
                    constantep.Date1 = DatePriseAnamnese.SelectedDate;

                }
            
           if (dia.Taille != 0 && dia.Taille!=null)
                {
                    examen.Add("Taille du patient :" + dia.Taille.ToString());
                }
               if (dia.Poids != 0 && dia.Poids != null)
                {
                    examen.Add("Poids du patient :" + dia.Poids.ToString());
                }
               if (dia.IMC != 0 && dia.IMC != null)
                {
                    examen.Add("Indice de masse corporelle du patient :" + dia.IMC.ToString());
                }
              if (txtGraisse.Text != "" )
                {
                    examen.Add("Répartition des graisses du patient :" + dia.GraisseRépartiton.ToString());
                }
                if (txtNorologique.Text != "")
                {
                    examen.Add("Examen neurologique du patient :" + dia.NorologiqueExamen.ToString());
                }
               if (dia.Tensionartérielle != 0 && dia.Tensionartérielle != null)
                {
                    examen.Add("Tension artérielle du patient :" + dia.Tensionartérielle.ToString());
                }
               if (dia.Tensionartérielleorthostatique != 0 && dia.Tensionartérielleorthostatique != null)
                {
                    examen.Add("Tension artérielle orthostatique du patient :" + dia.Tensionartérielleorthostatique.ToString());
                }
             if (dia.Pulsation != 0 && dia.Pulsation !=null)
                {
                    examen.Add("Pulsation du patient :" + dia.Pulsation.ToString());
                }
              if (txtMacroangiopathie.Text != "")
                {
                    examen.Add("Macro angiopathie du patient :" + dia.Macroangiopathie.ToString());
                }
                if (txtInsuffisance.Text != "")
                {
                    examen.Add("Insuffisance cardiaque du patient :" + dia.Insuffisancecardiaque.ToString());
                }
               if (txtDoeèmes.Text != "")
                {
                    examen.Add("Oedèmes des membres inférieurs du patient :" + dia.Oedèmesdesmembres.ToString());
                }
                if (txtsoufflesvasculaire.Text != "")
                {
                    examen.Add("Souffles vasculaires du patient :" + dia.Soufflesvasculaires.ToString());
                }
                if (txtVasc.Text != "")
                {
                    examen.Add("état de la vascularisation du patient :" + dia.étatvascularisation.ToString());
                }
                if (txtExamendeyeux.Text != "")
                {
                    examen.Add("Acuité visuelle du patient :" + dia.Uneétudedelacuité.ToString());
                }
                if (txtPeau.Text != "")
                {
                    examen.Add("Examen de la peau du patient :" + dia.ExamenPeau.ToString());
                }
                if (txtPieds.Text != "")
                {
                    examen.Add("Examen des pieds du patient :" + dia.ExamenPieds.ToString());
                }
                if (examen.Count > 0)
                {
                    for (int i = 0; i < examen.Count; i++)
                    {
                        doss.txtExamen.Text += (examen[i] + "\n");

                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
