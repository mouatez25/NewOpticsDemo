﻿using DevExpress.Xpf.Core;
using dragonz.actb.provider;
using GestionClinique.Administrateur;
using GestionClinique.Patient;
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

namespace GestionClinique.RendezVous
{
    /// <summary>
    /// Interaction logic for PrendreRendezVous.xaml
    /// </summary>
    public partial class PrendreRendezVous : MetroWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership MemberUser;
        SVC.RendezVou rendezVousP;
        int RendezInterface;
        private delegate void FaultedInvoker1();
        bool NomPrenoRendezVousnull = false;
        bool auto = false;
        public PrendreRendezVous(SVC.RendezVou rendezvous, SVC.ServiceCliniqueClient proxyrecu, SVC.Membership membershiprecu, ICallback callbackrecu, int Interface, SVC.Patient patientrecu)
        {
            try
            {
                InitializeComponent();
                /*   txtDate.DisplayDate =Convert.ToDateTime(rendezvous.Date);
                   txtHoraire.ItemsSource = rendezvous.RendezVousD.ToString() + rendezvous.RendezVousF.ToString();*/
                proxy = proxyrecu;
                MemberUser = membershiprecu;
                RendezInterface = Interface;
                callbackrecu.InsertPatientCallbackEvent += new ICallback.CallbackEventHandler7(callbackrecu_Refresh);
                callbackrecu.InsertMotifVisiteCallbackEvent += new ICallback.CallbackEventHandler10(callbackrecuMotif_Refresh);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (rendezvous != null)
                {
                    rendezVousP = rendezvous;
                    RendezVousGrid.DataContext = rendezVousP;



                    if (rendezvous.Nom == null && rendezvous.Prénom == null && Interface == 1)
                    {
                        txtFonction.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                      
                        txtPatient.Visibility = Visibility.Collapsed;
                        auto = true;
                        List<SVC.Medecin> testmedecin = proxy.GetAllMedecin();
                        txtMedecin.ItemsSource = testmedecin;
                        List<SVC.Medecin> tte = testmedecin.Where(n => n.Nom == rendezVousP.MedecinNom && n.Prénom == rendezVousP.MedecinPrénom).ToList();
                        txtMedecin.SelectedItem = tte.First();
                        btnCreer.IsEnabled = true;
                        List<SVC.Patient> pp = new List<SVC.Patient>();
                        accbStates.ItemsSource = pp;

                        txtHoraire.Enabled = false;

                    }
                    else
                    {
                        if (Interface == 2)
                        {


                            /******************fdans datagrid exist in interface rendez vous**************/
                            if (rendezvous.Nom != null && rendezvous.Prénom != null)
                            {
                                NomPrenoRendezVousnull = false;
                                btnCreer.IsEnabled = true;
                                btnCreer.Content = "Modifier";
                                txtFonction.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                                txtFonction.SelectedItem = rendezVousP.Motif;
                                txtHoraire.Text = (rendezvous.RendezVousD).ToString();


                        
                                List<SVC.Patient> testmedecin1 = proxy.GetAllPatientBYID(rendezvous.CodePatient.Value).OrderBy(n => n.Nom).ToList();
                                txtPatient.ItemsSource = testmedecin1;
                                List<SVC.Patient> tte1 = testmedecin1.Where(n => n.Id == rendezvous.CodePatient).OrderBy(n => n.Nom).ToList();
                                txtPatient.SelectedItem = tte1.First();
                                txtPatient.IsEnabled = false;
                                auto = false;
                                accbStates.Visibility = Visibility.Collapsed;
                                List<SVC.Medecin> testmedecin = proxy.GetAllMedecin();
                                txtMedecin.ItemsSource = testmedecin;
                                List<SVC.Medecin> tte = testmedecin.Where(n => n.Nom == rendezVousP.MedecinNom && n.Prénom == rendezVousP.MedecinPrénom).ToList();
                                txtMedecin.SelectedItem = tte.First();

                            }
                            else
                            {
                                /*************dans datagrid exist in rendez vous interface********************/
                                if (rendezvous.Nom == null && rendezvous.Prénom == null)
                                {
                                    NomPrenoRendezVousnull = true;
                                    btnCreer.IsEnabled = true;
                                    //    btnCreer.Content = "Créer";
                                    txtFonction.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                                    txtFonction.SelectedItem = rendezVousP.Motif;
                                    txtHoraire.Text = (rendezvous.RendezVousD).ToString();



                                    List<SVC.Patient> pp = new List<SVC.Patient>();
                                    accbStates.ItemsSource = pp;
                                    txtPatient.Visibility = Visibility.Collapsed;
                                    auto = true;
                                    List<SVC.Medecin> testmedecin = proxy.GetAllMedecin();
                                    txtMedecin.ItemsSource = testmedecin;

                                    List<SVC.Medecin> tte = testmedecin.Where(n => n.Nom == rendezVousP.MedecinNom && n.Prénom == rendezVousP.MedecinPrénom).ToList();
                                    txtMedecin.SelectedItem = tte.First();
                                }



                            }
                        }
                        else
                        {/***********       Prise de rendez vous dans mainwindow.xaml c-a-d  *************************/
                            if (Interface == 3)
                            {
                                rendezVousP = rendezvous;
                                RendezVousGrid.DataContext = rendezVousP;

                                txtFonction.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                                // txtPatient.ItemsSource = proxy.GetAllPatient();

                                txtMedecin.ItemsSource = proxy.GetAllMedecin();
                                List<SVC.Patient> pp = new List<SVC.Patient>();
                                accbStates.ItemsSource = pp;
                                txtPatient.Visibility = Visibility.Collapsed;
                                auto = true;

                            }
                            else
                            {

                                if (Interface == 4)
                                {
                                    
                                    List<SVC.Patient> testmedecin1 = proxy.GetAllPatientBYID(patientrecu.Id).OrderBy(n => n.Nom).ToList();
                                    txtPatient.ItemsSource = testmedecin1;
                                    List<SVC.Patient> tte11 = testmedecin1.Where(n => n.Id == rendezvous.CodePatient).OrderBy(n => n.Nom).ToList();
                                    txtPatient.SelectedItem = tte11.First();
                                    txtPatient.IsEnabled = false;

                                    List<SVC.Medecin> test = proxy.GetAllMedecin();
                                    txtMedecin.ItemsSource = test;

                                    List<SVC.Medecin> tte1 = test.Where(n => n.Nom == patientrecu.SuiviParNom && n.Prénom == patientrecu.SuiviParPrénom).ToList();
                                    txtMedecin.SelectedItem = tte1.First();


                                    rendezVousP = rendezvous;
                                    RendezVousGrid.DataContext = rendezVousP;
                                    txtFonction.ItemsSource = proxy.GetAllMotifVisite().OrderBy(n => n.Motif);
                                    accbStates.Visibility = Visibility.Collapsed;
                                    auto = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }



            void callbackrecu_Refresh(object source, CallbackEventInsertPatient e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav,e.operleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void callbackrecuMotif_Refresh(object source, CallbackEventInsertMotifVisite e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshMotif(e.clientleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshMotif(List<SVC.MotifVisite> listmembership)
        {
            try { 
            txtFonction.ItemsSource = listmembership.OrderBy(n => n.Motif);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.Patient listmembership,int oper)
        {
            try
            {

                if (auto == false)
                {
                    var LISTITEM1 = txtPatient.ItemsSource as IEnumerable<SVC.Patient>;
                    List<SVC.Patient> LISTITEM = LISTITEM1.ToList();

                    if (oper == 1)
                    {
                        LISTITEM.Add(listmembership);
                    }
                    else
                    {
                        if (oper == 2)
                        {
                          //  var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                            //objectmodifed = listmembership;


                            var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                            //objectmodifed = listmembership;
                            var index = LISTITEM.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEM[index] = listmembership;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                                LISTITEM.Remove(deleterendez);
                            }
                        }
                    }

                    txtPatient.ItemsSource = LISTITEM;
                }
                else
                {
                    if (auto == true)
                    {

                        var LISTITEM11 = accbStates.ItemsSource as IEnumerable<SVC.Patient>;
                        List<SVC.Patient> LISTITEM0 = LISTITEM11.ToList();

                        if (oper == 1)
                        {
                            LISTITEM0.Add(listmembership);
                        }
                        else
                        {
                            if (oper == 2)
                            {
                             //   var objectmodifed = LISTITEM0.Find(n => n.Id == listmembership.Id);
                              //  objectmodifed = listmembership;


                                var objectmodifed = LISTITEM0.Find(n => n.Id == listmembership.Id);
                                //objectmodifed = listmembership;
                                var index = LISTITEM0.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM0[index] = listmembership;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    var deleterendez = LISTITEM0.Where(n => n.Id == listmembership.Id).First();
                                    LISTITEM0.Remove(deleterendez);
                                }
                            }
                        }


                        accbStates.ItemsSource = LISTITEM0;





                      //  accbStates.AutoCompleteManager.DataProvider = new SimpleStaticDataProvider(from c in LISTITEM0
                                                                                        //           select c.Nom + " " + c.Prénom);

                        //accbStates.AutoCompleteManager.AutoAppend = true;
                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker1(HandleProxy));
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
        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (MemberUser.CréationPatient==true)
            { 
            NewPatient CLNewPatient = new NewPatient( proxy, MemberUser,null);
            CLNewPatient.Show();
            }else
            {
                if (MemberUser.CréationPatient != true)
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /****************prendez rendez vous dans la page des rendez vous************************/
                if (MemberUser.CréationRendezVous == true /*&& txtPatient.SelectedItem != null*/ && rendezVousP.Nom == null && rendezVousP.Prénom == null && RendezInterface == 1 && accbStates.Text.Trim()!="" && txtFonction.SelectedItem!=null)
                {

                    //   SVC.Patient Selectpatient = txtPatient.SelectedItem as SVC.Patient;
                    SVC.Patient Selectpatient  =proxy.GetAllPatientNomPrenom(accbStates.Text.Trim());

                    SVC.Medecin SelectMedecin = txtMedecin.SelectedItem as SVC.Medecin;
                    rendezVousP.Nom = Selectpatient.Nom;
                    rendezVousP.Prénom = Selectpatient.Prénom;
                    rendezVousP.Adresse = Selectpatient.Adresse;
                    rendezVousP.PrisPar = MemberUser.UserName;
                    rendezVousP.Email = Selectpatient.Email;
                    rendezVousP.Téléphone = Selectpatient.Téléphone;
                    rendezVousP.NuméroRendezVous = (rendezVousP.Date).Value.ToString("ddMMyyyy") + ((rendezVousP.RendezVousD).Value).ToString(@"hhmm") + Selectpatient.Id.ToString();//"HH:mm"
                    rendezVousP.CodePatient = Selectpatient.Id;
                    rendezVousP.MedecinNom = SelectMedecin.Nom;
                    rendezVousP.CodeMedecin = SelectMedecin.Id;
                    rendezVousP.MedecinPrénom = SelectMedecin.Prénom;
                    rendezVousP.Confirm = false;
                  //  MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("INTERFACE 1", GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    if (MemberUser.ImpressionRendezVous == true && ChImprimerTicket.IsChecked == true)
                    {
                        proxy.InsertRendez(rendezVousP);
                        ImpressionTicket clsho = new ImpressionTicket(proxy, rendezVousP);
                        clsho.Show();
                        MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }else
                    {
                        if (ChImprimerTicket.IsChecked == false && MemberUser.ImpressionRendezVous == true)
                        {
                            proxy.InsertRendez(rendezVousP);
                            MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            proxy.InsertRendez(rendezVousP);
                            MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }

                    }
                    btnCreer.IsEnabled = false;
                   // proxy.AjouterSalleAtentefRefresh();
                    proxy.AjouterRendezVOUSfRefresh();

                }
                else
                {
                    if (MemberUser.ModificationRendezVous == true && txtPatient.SelectedItem != null && rendezVousP.Id != 0 && txtMedecin.SelectedItem != null && txtFonction.SelectedItem != null)
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            SVC.Patient Selectpatient = txtPatient.SelectedItem as SVC.Patient;
                            SVC.MotifVisite selectedmotif = txtFonction.SelectedItem as SVC.MotifVisite;
                            rendezVousP.Motif = selectedmotif.Motif;
                            rendezVousP.Adresse = Selectpatient.Adresse;
                            rendezVousP.Nom = Selectpatient.Nom;
                            rendezVousP.Email = Selectpatient.Email;
                            rendezVousP.Date = txtDate.SelectedDate;
                            rendezVousP.Prénom = Selectpatient.Prénom;
                            SVC.Medecin SelectMedecin = txtMedecin.SelectedItem as SVC.Medecin;
                            rendezVousP.MedecinNom = SelectMedecin.Nom;
                            rendezVousP.MedecinPrénom = SelectMedecin.Prénom;
                            rendezVousP.CodePatient = Selectpatient.Id;
                            rendezVousP.SpécialitéMedecin = SelectMedecin.SpécialitéMedecin;
                            rendezVousP.CodeMedecin = SelectMedecin.Id;
                            rendezVousP.RendezVousD = TimeSpan.Parse(txtHoraire.Text);
                            rendezVousP.RendezVousF = SelectMedecin.TempsVisite;
                            rendezVousP.Téléphone = Selectpatient.Téléphone;
                            rendezVousP.Confirm = ComboPresent.IsChecked;
                            /********************/





                            /*****************/
                            proxy.UpdateRendezVous(rendezVousP);
                            ts.Complete();
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        proxy.AjouterRendezVOUSfRefresh();
                        proxy.AjouterSalleAtentefRefresh();
                    }
                    else
                    {
                        /*******************************prendez rendez vous dans home.xaml + dans mainwindow c-a-d ni heure ni jours*****************************************************/
                        if (MemberUser.CréationRendezVous == true /*&& txtPatient.SelectedItem != null*/ && rendezVousP.Id == 0 && txtMedecin.SelectedItem != null && RendezInterface == 3  && accbStates.Text.Trim()!="" && txtFonction.SelectedItem != null)
                        {

                       //     SVC.Patient Selectpatient = txtPatient.SelectedItem as SVC.Patient;
                            SVC.Patient Selectpatient = proxy.GetAllPatient().Find(n => (n.Nom + " " + n.Prénom).Trim() == accbStates.Text.Trim());

                            SVC.Medecin SelectMedecin = txtMedecin.SelectedItem as SVC.Medecin;

                            rendezVousP.Nom = Selectpatient.Nom;
                            rendezVousP.Prénom = Selectpatient.Prénom;
                            rendezVousP.Adresse = Selectpatient.Adresse;
                            rendezVousP.PrisPar = MemberUser.UserName;
                            rendezVousP.Email = Selectpatient.Email;
                            rendezVousP.Téléphone = Selectpatient.Téléphone;
                            rendezVousP.MedecinNom = SelectMedecin.Nom;
                            rendezVousP.MedecinPrénom = SelectMedecin.Prénom;
                            rendezVousP.Date = txtDate.SelectedDate;
                            rendezVousP.SpécialitéMedecin = SelectMedecin.SpécialitéMedecin;
                            rendezVousP.NuméroRendezVous = (txtDate.SelectedDate).Value.ToString("ddMMyyyy") + (TimeSpan.Parse(txtHoraire.Text)).ToString(@"hhmm") + Selectpatient.Id.ToString();//"HH:mm"
                            rendezVousP.CodePatient = Selectpatient.Id;
                            rendezVousP.CodeMedecin = SelectMedecin.Id;
                            rendezVousP.RendezVousD = TimeSpan.Parse(txtHoraire.Text);
                            rendezVousP.RendezVousF = SelectMedecin.TempsVisite;
                            rendezVousP.Confirm = false;
                            if (MemberUser.ImpressionRendezVous == true && ChImprimerTicket.IsChecked == true)
                            {
                                proxy.InsertRendez(rendezVousP);
                                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                ImpressionTicket clsho = new ImpressionTicket(proxy, rendezVousP);
                                clsho.Show();
                            }
                            else
                            {
                                if ( ChImprimerTicket.IsChecked == false && MemberUser.ImpressionRendezVous == true)
                                {
                                    proxy.InsertRendez(rendezVousP);
                                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                                else
                                {
                                    proxy.InsertRendez(rendezVousP);
                                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                }
                            }
                            proxy.AjouterRendezVOUSfRefresh();
                          //  proxy.AjouterSalleAtentefRefresh();
                        }
                        else
                        {/******************rendez datagrid existe dans rendezvous.xaml date ok***************************/
                            if (MemberUser.CréationRendezVous == true/* && rendezVousP.Nom == null && rendezVousP.Prénom == null*/ && RendezInterface == 2 &&  txtFonction.SelectedItem != null)
                            {
                                SVC.Patient Selectpatient=new SVC.Patient();
                                if (NomPrenoRendezVousnull==true && accbStates.Text.Trim() != "")
                                {


                                    Selectpatient = proxy.GetAllPatient().Find(n => (n.Nom + " " + n.Prénom).Trim() == accbStates.Text.Trim());
                                    SVC.Medecin SelectMedecin = txtMedecin.SelectedItem as SVC.Medecin;

                                    rendezVousP.Nom = Selectpatient.Nom;
                                    rendezVousP.Prénom = Selectpatient.Prénom;
                                    rendezVousP.Adresse = Selectpatient.Adresse;
                                    rendezVousP.PrisPar = MemberUser.UserName;
                                    rendezVousP.Email = Selectpatient.Email;
                                    rendezVousP.Téléphone = Selectpatient.Téléphone;
                                    rendezVousP.MedecinNom = SelectMedecin.Nom;
                                    rendezVousP.MedecinPrénom = SelectMedecin.Prénom;

                                    rendezVousP.NuméroRendezVous = (rendezVousP.Date).Value.ToString("ddMMyyyy") + ((rendezVousP.RendezVousD).Value).ToString(@"hhmm") + Selectpatient.Id.ToString();//"HH:mm"
                                    rendezVousP.CodePatient = Selectpatient.Id;
                                    rendezVousP.CodeMedecin = SelectMedecin.Id;
                                    rendezVousP.Confirm = false;
                                    if (MemberUser.ImpressionRendezVous == true && ChImprimerTicket.IsChecked == true)
                                    {
                                        proxy.InsertRendez(rendezVousP);
                                        MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        ImpressionTicket clsho = new ImpressionTicket(proxy, rendezVousP);
                                        clsho.Show();
                                    }else
                                    {
                                        if (ChImprimerTicket.IsChecked == false && MemberUser.ImpressionRendezVous == true)
                                        {
                                            proxy.InsertRendez(rendezVousP);
                                            MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }
                                        else
                                        {
                                            proxy.InsertRendez(rendezVousP);
                                            MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }
                                    }
                                    proxy.AjouterRendezVOUSfRefresh();
                                  //  proxy.AjouterSalleAtentefRefresh();
                                    btnCreer.IsEnabled = false;
                                }
                                else
                                {
                                    if (NomPrenoRendezVousnull == false )
                                    {
                                        Selectpatient = txtPatient.SelectedItem as SVC.Patient;

                                    }
                                }

                               // 

                          
                            }
                            else
                            {
                                /***************rendez vous dans listepatienthome c-a-d**************************************/
                                if (MemberUser.CréationRendezVous == true && txtPatient.SelectedItem != null &&/* rendezVousP.Nom == null && rendezVousP.Prénom == null &&*/ RendezInterface == 4 && txtMedecin.SelectedItem != null && txtDate.SelectedDate != null && txtHoraire.Text != "" && txtFonction.SelectedItem != null)
                                {
                                    SVC.Patient Selectpatient = txtPatient.SelectedItem as SVC.Patient;
                                    SVC.Medecin selectmedecin = txtMedecin.SelectedItem as SVC.Medecin;
                                    rendezVousP.Nom = Selectpatient.Nom;
                                    rendezVousP.Prénom = Selectpatient.Prénom;
                                    rendezVousP.Adresse = Selectpatient.Adresse;
                                    rendezVousP.PrisPar = MemberUser.UserName;
                                    rendezVousP.Email = Selectpatient.Email;
                                    rendezVousP.Téléphone = Selectpatient.Téléphone;
                                    rendezVousP.RendezVousD = TimeSpan.Parse(txtHoraire.Text);
                                    rendezVousP.Date = txtDate.SelectedDate;
                                    rendezVousP.NuméroRendezVous = (txtDate.SelectedDate).Value.ToString("ddMMyyyy") + ((rendezVousP.RendezVousD).Value).ToString(@"hhmm") + Selectpatient.Id.ToString();//"HH:mm"
                                    rendezVousP.CodePatient = Selectpatient.Id;
                                    rendezVousP.MedecinNom = selectmedecin.Nom;
                                    rendezVousP.MedecinPrénom = selectmedecin.Prénom;
                                    rendezVousP.CodeMedecin = selectmedecin.Id;
                                    rendezVousP.RendezVousF = rendezVousP.RendezVousD + selectmedecin.TempsVisite;
                                    rendezVousP.Confirm = false;
                                    if (MemberUser.ImpressionRendezVous == true && ChImprimerTicket.IsChecked == true)
                                    {
                                        proxy.InsertRendez(rendezVousP);
                                        ImpressionTicket clsho = new ImpressionTicket(proxy, rendezVousP);
                                        clsho.Show();
                                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                    else
                                    {
                                        if (ChImprimerTicket.IsChecked == false && MemberUser.ImpressionRendezVous == true)
                                        {
                                            proxy.InsertRendez(rendezVousP);
                                            MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }
                                        else
                                        {
                                            proxy.InsertRendez(rendezVousP);
                                            MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                        }


                                    }
                                    proxy.AjouterRendezVOUSfRefresh();
                                  //  proxy.AjouterSalleAtentefRefresh();
                                    btnCreer.IsEnabled = false;
                                }
                                else
                                {
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            }
        private void accbStates_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
            if (accbStates.Text.Trim()!="" && (RendezInterface ==1 || (RendezInterface ==2 && NomPrenoRendezVousnull==true) || RendezInterface==3))
            {
                var t = accbStates.SelectedItem as SVC.Patient;
                accbStates.Text = t.Nom + " " + t.Prénom;
                btnCreer.IsEnabled = true;
            }else
            {
                btnCreer.IsEnabled = false;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private void txtPatient_DropDownClosed(object sender, EventArgs e)
        {
            try { 
            if (txtPatient.SelectedItem!=null && (RendezInterface != 1 || (RendezInterface != 2 && NomPrenoRendezVousnull == false) || RendezInterface != 3))
            {
               btnCreer.IsEnabled = true;

            }else
            {
                btnCreer.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtFonction_GotFocus(object sender, RoutedEventArgs e)
        {
            try { 
            var box = sender as ComboBox;
            box.IsDropDownOpen= true;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void accbStates_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if ((RendezInterface == 3 || RendezInterface == 2 || RendezInterface == 1) && auto == true)
                {
                    if (accbStates.Text.ToCharArray().Count() == 2)
                    {
                        accbStates.ItemsSource = proxy.GetAllPatientPAR(accbStates.Text.ToUpper().Trim());
                        accbStates.AutoCompleteManager.DataProvider = new SimpleStaticDataProvider(from c in proxy.GetAllPatientPAR(accbStates.Text.ToUpper().Trim())
                                                                                                   select c.Nom + " " + c.Prénom);

                        accbStates.AutoCompleteManager.AutoAppend = true;
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
