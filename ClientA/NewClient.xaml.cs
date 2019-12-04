
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

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for NewClient.xaml
    /// </summary>
    public partial class NewClient : Window
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerNewClient();
        SVC.MembershipOptic member;
        SVC.ClientV patient;
        bool Création = false;
        public NewClient(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, SVC.ClientV patientrecu)
        {
            try
            {
                InitializeComponent();
               proxy = proxyrecu;
                proxy.InnerDuplexChannel.Opened += new EventHandler(InnerDuplexChannel_Opened);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                member = memberrecu;
                if (patientrecu == null)
                {
                    Création = true;
                    btnCreer.IsEnabled = false;
                    f.Content = "CREER CLIENT";
                }
                else
                {
                    patient = patientrecu;
                    PatientGrid.DataContext = patient;
                    f.Content = "MODIFIER CLIENT";

                } 
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewClient(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewClient(HandleProxy));
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
        private void txtNom_TextChanged(object sender, TextChangedEventArgs e)
        {
          

        }

       

        private void txtCrédit_KeyDown(object sender, KeyEventArgs e)
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
        private void BTNCLOSTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void rempliridsivisite(SVC.ClientV pa)
        {
            try
            {
                if (txtsolde.Text != "")
                {
                    var found = (proxy.GetAllClientV()).Find(n => n.Raison.Trim() == pa.Raison.Trim());
                    /***********************************************/
                    if (found.solde != 0)
                    {
                        bool interfaceVisite = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            SVC.F1 cas = new SVC.F1
                            {
                                bcom = "Ancien Solde",
                                dates = DateTime.Now,
                                echeance = 0,
                                ht = found.solde,
                                date = DateTime.Now,
                                modep = "Ancien Solde",
                                net = found.solde,
                               nfact = "Ancien Solde",
                               oper = member.Username,
                               remise = 0,
                               reste= found.solde,
                                cle = pa.cle,
                                timbre=0,
                                tva=0,
                                versement=0,
                                
                            };

                          

                            proxy.InsertF1(cas);
                            interfaceVisite = true;
                            if (interfaceVisite == true)
                            {
                                ts.Complete();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string nommedecin;
                string prénommedecin;
              
                SVC.ClientV pa;
                if (Création == true && member.CreationFichier == true && txtRaison.Text!="")
                {
                    try
                    {


                        
                        pa = new SVC.ClientV
                        {
                           adresse=txtadresse.Text.Trim(),
                            cle = txtRaison.Text.Trim() + DateTime.Now,
                            ai=txtai.Text.Trim(),
                            dates=DateTime.Now,
                            mf=txtmf.Text.Trim(),
                            oper=member.Username,
                            Raison=txtRaison.Text.Trim(),
                            rc=txtrc.Text.Trim(),
                          //  stva= chtva.IsChecked,
                           Ref=txtRéf.Text.Trim(),
                           Tel= txttel.Text.Trim(),
                           
                        };
                        
                       
                        if (txtsolde.Text != "")
                        {
                            pa.solde = Convert.ToDecimal(txtsolde.Text);


                        }
                        else
                        {
                            pa.solde = 0;
                        }
                       
                        bool succéespatient = false;
                        bool succéesvisite = false;
                        int interfaceoper;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.InsertClientV(pa);
                            interfaceoper = 0;
                            succéespatient = true;
                            if (pa.solde != 0)
                            {

                                interfaceoper = 1;
                             
                                SVC.F1 cas = new SVC.F1
                                {
                                  bcom="Ancien solde",
                                  date=DateTime.Now,
                                  dates=DateTime.Now,
                                  echeance=0,
                                  ht=pa.solde,
                                  modep= "Ancien solde",
                                  net= pa.solde,
                                  nfact= "Ancien solde",
                                  oper=member.Username,
                                  remise=0,
                                  reste= pa.solde,
                                  timbre=0,
                                  tva=0,
                                  versement=0,
                                  cle = pa.cle,
                                  raison=pa.Raison,
                                };
                                proxy.InsertF1(cas);
                                succéesvisite = true;
                            }
                            if (interfaceoper == 0 && succéespatient == true)
                            {
                                ts.Complete();
                                btnCreer.IsEnabled = false;
                               // rempliridsivisite(pa);

                            }
                            else
                            {
                                if (interfaceoper == 1 && succéespatient == true && succéesvisite == true)
                                {

                                    ts.Complete();


                                }
                                else
                                {

                                    btnCreer.IsEnabled = false;

                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }

                        }
                        if (interfaceoper == 0 && succéespatient == true)
                        {
                            proxy.AjouterClientVRefresh();
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            if (interfaceoper == 1 && succéespatient == true && succéesvisite == true)
                            {
                                proxy.AjouterClientVRefresh();
                                proxy.AjouterSoldeF1Refresh();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                        }
                     
                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try
                    {
                        if (Création == false && member.ModificationFichier == true && txtRaison.Text!="")
                        {
                            bool succesUpdatePatient = false;
                            bool succesUpdateVisite = false;

                          
                          
                           

                            if (txtsolde.Text != "")
                            {
                                patient.solde = Convert.ToDecimal(txtsolde.Text);
                            }
                            else
                            {
                                patient.solde = 0;
                            }
                            var found = (proxy.GetAllF1ByCle(patient.cle).Find(n => n.cle == patient.cle));
                            int interfaceModif = 3;

                            using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {


                                if (found != null)
                                {
                                    if (patient.solde != 0)
                                    {

                                        found.ht = patient.solde;
                                        found.net = patient.solde;
                                        found.reste = patient.solde - found.versement;
                                        

                                        proxy.UpdateF1(found);
                                        interfaceModif = 1;
                                        succesUpdateVisite = true;
                                        proxy.UpdateClientV(patient);

                                        succesUpdatePatient = true;
                                    }
                                    else
                                    {
                                        if (patient.solde == 0)
                                        {
                                            if (found.versement == 0)
                                            {
                                                proxy.DeleteF1(found);
                                                interfaceModif = 1;
                                                succesUpdateVisite = true;
                                                proxy.UpdateClientV(patient);

                                                succesUpdatePatient = true;
                                            }
                                            else
                                            {
                                                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention erreur dans le solde CLIENT qui contient un versement", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (found == null)
                                    {
                                        if (patient.solde != 0)
                                        {
                                            SVC.F1 cas = new SVC.F1
                                            {
                                                bcom = "Ancien solde",
                                                date = DateTime.Now,
                                                dates = DateTime.Now,
                                                echeance = 0,
                                                ht = patient.solde,
                                                modep = "Ancien solde",
                                                net = patient.solde,
                                                nfact = "Ancien solde",
                                                oper = member.Username,
                                                remise = 0,
                                                reste = patient.solde,
                                                timbre = 0,
                                                tva = 0,
                                                versement = 0,
                                                cle = patient.cle,
                                                raison = patient.Raison,
                                                codeclient= patient.Id,
                                            };
                                            proxy.InsertF1(cas);
                                            interfaceModif = 1;
                                            succesUpdateVisite = true;
                                            proxy.UpdateClientV(patient);

                                            succesUpdatePatient = true;
                                        }
                                        else
                                        {
                                            if (patient.solde == 0)
                                            {
                                                interfaceModif = 0;
                                                proxy.UpdateClientV(patient);
                                                succesUpdatePatient = true;
                                            }
                                        }
                                    }
                                }
                                if (interfaceModif == 0 && succesUpdatePatient == true && succesUpdateVisite == false)
                                {
                                    ts.Complete();
                                }
                                else
                                {
                                    if (interfaceModif == 1 && succesUpdatePatient == true && succesUpdateVisite == true)
                                    {
                                        ts.Complete();
                                    }
                                    else
                                    {
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                            if (interfaceModif == 0 && succesUpdatePatient == true && succesUpdateVisite == false)
                            {
                                proxy.AjouterClientVRefresh();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                            else
                            {
                                if (interfaceModif == 1 && succesUpdatePatient == true && succesUpdateVisite == true)
                                {
                                    proxy.AjouterSoldeF1Refresh();
                                    proxy.AjouterClientVRefresh();
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.Close();
                                }
                            }

                        }
                        else
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        

       

        private void txtRaison_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {


                var disponible = proxy.GetAllClientVEXISTE(txtRaison.Text.Trim());
                if (disponible == true && Création == true)
                {
                    this.Title = "Ce Client appartient déja a la base de donnée";

                    btnCreer.IsEnabled = false;
                    btnCreer.Opacity = 0.2;
                }
                else
                {
                    this.Title = "Client " + txtRaison.Text.Trim().ToUpper() + " est disponible pour création";
                    btnCreer.IsEnabled = true;
                    btnCreer.Opacity = 1;

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
