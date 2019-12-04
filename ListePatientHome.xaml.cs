using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
using GestionClinique.FileDattente;
using GestionClinique.Patient;
using GestionClinique.RendezVous;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GestionClinique
{
    /// <summary>
    /// Interaction logic for ListePatientHome.xaml
    /// </summary>
    public partial class ListePatientHome : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.Membership memberuser;
        SVC.Medecin MedecinEnCours;
        private delegate void FaultedInvokerListePatientHome();
        SVC.Client localclient;
        public ListePatientHome(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership memberrecu, ICallback callbackrecu, List<SVC.Patient> patientrecu,SVC.Client clientrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;


                memberuser = memberrecu;

                callback = callbackrecu;
                localclient = clientrecu;
                PatientDataGrid.ItemsSource = patientrecu;
                var disponible = (proxy.GetAllMedecin()).Where(list1 => list1.UserName == memberuser.UserName).FirstOrDefault();
                if (disponible != null)
                {
                    MedecinEnCours = disponible;
                }
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListePatientHome(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListePatientHome(HandleProxy));
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
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            if (txtRecherche.Text!="")
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
              
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Patient p = o as SVC.Patient;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Nom.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            if (memberuser.SuppressionPatient == true && PatientDataGrid.SelectedItem != null)
            {
                SVC.Patient SelectMedecin = PatientDataGrid.SelectedItem as SVC.Patient;
                proxy.DeletePatient(SelectMedecin);
            }else
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (memberuser.CréationPatient == true)
            {
                NewPatient CLMedecin = new NewPatient(proxy, memberuser, null);
                CLMedecin.Show();

            }
            else
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModificationPatient == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.Patient SelectMedecin = PatientDataGrid.SelectedItem as SVC.Patient;
                    NewPatient CLMedecin = new NewPatient(proxy, memberuser, SelectMedecin);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
            if (memberuser.ImpressionPatient == true)
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Patient SelectMedecin = PatientDataGrid.SelectedItem as SVC.Patient;



                    FichePatient clsho = new FichePatient(proxy, SelectMedecin);
                    clsho.Show();
                }
                else
                {
                    if (txtRecherche.Text != "")
                    {
                        List<SVC.Patient> test = PatientDataGrid.ItemsSource as List<SVC.Patient>;
                        var t = (from e1 in test

                                 where e1.Nom.ToUpper().StartsWith(txtRecherche.Text.ToUpper())

                                 select e1);



                        ReportListePatient clsho = new ReportListePatient(proxy, t.ToList());
                        clsho.Show();
                    }
                    else
                    {
                        List<SVC.Patient> test = PatientDataGrid.ItemsSource as List<SVC.Patient>;


                        ReportListePatient clsho = new ReportListePatient(proxy, test.ToList());
                        clsho.Show();

                    }
                }
            }else
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void btnDossier_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (PatientDataGrid.SelectedItem != null && memberuser.ModuleDossierPatient == true)
            {
                SVC.Patient SelectMedecin = PatientDataGrid.SelectedItem as SVC.Patient;
                if ((MedecinEnCours != null && SelectMedecin.SuiviParCode == MedecinEnCours.Id) || memberuser.AccèsToutLesDossierPatient == true)
                {
                    DossierPatient dd = new DossierPatient(proxy, SelectMedecin, callback, memberuser, "Sans Rendez Vous", MedecinEnCours, localclient);
                    dd.Show();
                }
                else
                {
                    if (memberuser.AccèsToutLesDossierPatient == true)
                    {
                        DossierPatient dd = new DossierPatient(proxy, SelectMedecin, callback, memberuser, "Sans Rendez Vous", null,localclient);
                        dd.Show();
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }else
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void PatientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (memberuser.ModificationPatient == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.Patient SelectMedecin = PatientDataGrid.SelectedItem as SVC.Patient;
                    NewPatient CLMedecin = new NewPatient(proxy, memberuser, SelectMedecin);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            } 
            catch (Exception ex)
            {
                HandleProxy();
    }
}

        private void PrendreRendezVous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CréationRendezVous == true && PatientDataGrid.SelectedItem != null)
                {
                    SVC.Patient SelectPatient = PatientDataGrid.SelectedItem as SVC.Patient;
                    SVC.RendezVou rendezvous = new SVC.RendezVou
                    {
                        Nom = SelectPatient.Nom,
                        Prénom=SelectPatient.Prénom,
                        CodePatient=SelectPatient.Id,
                        Adresse=SelectPatient.Adresse,
                        Téléphone=SelectPatient.Téléphone,
                        Email=SelectPatient.Email,
                        PrisPar=memberuser.UserName,

                    };
                    
                     PrendreRendezVous kl = new PrendreRendezVous(rendezvous, proxy, memberuser, callback, 4, SelectPatient);
                    
                     kl.Show();
                }
                else
                {
                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ArrivageNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null && memberuser.CréationSalleAttente==true)
                {
                    SVC.Patient SelectPatient = PatientDataGrid.SelectedItem as SVC.Patient;
                    Arrivée cl = new Arrivée(proxy, memberuser, callback, null, null, DateTime.Now, 2, SelectPatient, null);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
