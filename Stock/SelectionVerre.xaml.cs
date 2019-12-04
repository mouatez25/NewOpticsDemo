
using NewOptics.Administrateur;
using NewOptics.Commande;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for SelectionVerre.xaml
    /// </summary>
    public partial class SelectionVerre :Window
    {
        SVC.ServiceCliniqueClient proxy;
    SVC.MembershipOptic MemberUser;
    ICallback callback;
    SVC.Produit Produit;
   
    SVC.Param selectedparam;
    int interfacenew;
    public int verrelocalisation;
    bool selectionproduit = false;
    SVC.Monture SelectedMonture;
        List<TarifList> listtatif;
        SVC.Verre selectedverre;
      public  List<SVC.MontureSupplement> ListSupp1;
        public List<SVC.MontureSupplement> ListSupp2;
        public List<SVC.MontureSupplement> ListSupp3;
        public List<SVC.MontureSupplement> ListSupp4;
        private delegate void FaultedInvokerSelectionVerre();
        string Cylindre,Spher = "";
        SVC.ClientV CLIENTV;
      
        public SelectionVerre(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu, int verrrerecu, SVC.Monture MontureRecu, int interfcerecu,List<SVC.MontureSupplement> listrecu,SVC.ClientV clientrecu)
        {
            try
            {
                InitializeComponent();

                proxy = proxyrecu;
                callback = callbackrecu;

                MemberUser = memberrecu;
                CLIENTV = clientrecu;
                SelectedMonture = MontureRecu;
                verrelocalisation = verrrerecu;
              
                interfacenew = interfcerecu;
                listtatif = new List<TarifList>();
              
                List<SVC.Fourn> testmedecin = proxy.GetAllFourn().OrderBy(x => x.raison).ToList();
                FamilleCombo.ItemsSource = testmedecin;
                selectedparam = proxy.GetAllParamétre();

                if (interfcerecu == 0)
                {
                    PatientDataGrid.ItemsSource = (proxy.GetAllVerre().OrderBy(n => n.Design));

                    switch (verrelocalisation)
                    {
                        case 1:
                          //  if (MontureRecu.IdDroiteVerreLoin != 0)
                           // {

                                Produit = new SVC.Produit();

                                griddetail.DataContext = Produit;
                                btnselection.IsEnabled = true;
                                ListSupp1 = listrecu;
                                SuppDataGrid.ItemsSource = ListSupp1;
                           // }
                            this.Title = "Sélection du verre DROIT";
                            break;
                        case 2:
                           // if (MontureRecu.IdGaucheVerreLoin != 0)
                           // {

                                Produit = new SVC.Produit();
                                griddetail.DataContext = Produit;
                                btnselection.IsEnabled = true;
                                ListSupp2 = listrecu;
                                SuppDataGrid.ItemsSource = ListSupp2;
                         //   }
                            this.Title = "Sélection du verre GAUCHE";
                            break;
                        case 3:
                           // if (MontureRecu.IdDroiteVerrePres != 0)
                           // {
                                Produit = new SVC.Produit();

                                griddetail.DataContext = Produit;
                                btnselection.IsEnabled = true;
                                ListSupp3 = listrecu;
                                SuppDataGrid.ItemsSource = ListSupp3;
                          //  }
                            this.Title = "Sélection du verre DROIT";
                            break;
                        case 4:
                           // if (MontureRecu.IdGaucheVerrePres != 0)
                           // {
                                Produit = new SVC.Produit();

                                griddetail.DataContext = Produit;
                                btnselection.IsEnabled = true;
                                ListSupp4 = listrecu;
                                SuppDataGrid.ItemsSource = ListSupp4;
                           // }
                            this.Title = "Sélection du verre GAUCHE";

                            break;

                    }
                }
                else
                {
                    if(interfcerecu == 1)
                    {

                        List<SVC.Verre> testmedecind = (proxy.GetAllVerre().OrderBy(n => n.Design)).ToList();
                        PatientDataGrid.ItemsSource = testmedecind;
                         switch (verrrerecu)
                        {
                            case 1:
                                if (MontureRecu.IdDroiteVerreLoin!=null)
                                {
                                    if (MontureRecu.IdDroiteVerreLoin != 0)
                                    {
                                        Produit = proxy.GetAllProduitbyid(Convert.ToInt16(MontureRecu.IdDroiteVerreLoin)).First();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        //  ListSupp1 = proxy.GetAllMontureSupplementbycle(MontureRecu.Cle).Where(n => n.interfaceMonture == 1).ToList();
                                        ListSupp1 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp1;
                                    }
                                    else
                                    {
                                        Produit = new SVC.Produit();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        ListSupp1 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp1;
                                    }
                                }
                                else
                                {
                                    Produit = new SVC.Produit();
                                    griddetail.DataContext = Produit;
                                    btnselection.IsEnabled = true;
                                    ListSupp1 = listrecu;
                                    SuppDataGrid.ItemsSource = ListSupp1;
                                }
                                this.Title = "Sélection du verre DROIT";
                                break;
                            case 2:
                                if (MontureRecu.IdGaucheVerreLoin!=null)
                                {
                                    if (MontureRecu.IdGaucheVerreLoin != 0)
                                    {
                                        Produit = proxy.GetAllProduitbyid(Convert.ToInt16(MontureRecu.IdGaucheVerreLoin)).First();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        // ListSupp2 = proxy.GetAllMontureSupplementbycle(MontureRecu.Cle).Where(n => n.interfaceMonture == 2).ToList();
                                        ListSupp2 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp2;
                                    } else
                                    {
                                        Produit = new SVC.Produit();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        ListSupp2 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp2;
                                    }
                                }
                                else
                                {
                                    Produit = new SVC.Produit();
                                    griddetail.DataContext = Produit;
                                    btnselection.IsEnabled = true;
                                    ListSupp2 = listrecu;
                                    SuppDataGrid.ItemsSource = ListSupp2;
                                }
                                this.Title = "Sélection du verre GAUCHE";
                                break;
                            case 3:
                                if(MontureRecu.IdDroiteVerrePres!=null)
                                {
                                    if (MontureRecu.IdDroiteVerrePres != 0)
                                    {
                                        Produit = proxy.GetAllProduitbyid(Convert.ToInt16(MontureRecu.IdDroiteVerrePres)).First();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                     //   ListSupp3 = proxy.GetAllMontureSupplementbycle(MontureRecu.Cle).Where(n => n.interfaceMonture == 3).ToList();
                                        ListSupp3 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp3;
                                    }
                                    else
                                    {
                                        Produit = new SVC.Produit();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        ListSupp3 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp3;
                                    }
                                }
                                else
                                {
                                  //  Produit = new SVC.Produit();
                                   // griddetail.DataContext = Produit;
                                    btnselection.IsEnabled = true;
                                    ListSupp3 = listrecu;
                                    SuppDataGrid.ItemsSource = ListSupp3;
                                }
                                this.Title = "Sélection du verre DROIT";

                                break;
                            case 4:
                                if (MontureRecu.IdGaucheVerrePres!=null)
                                {
                                    if (MontureRecu.IdGaucheVerrePres != 0)
                                    {
                                        Produit = proxy.GetAllProduitbyid(Convert.ToInt16(MontureRecu.IdGaucheVerrePres)).First();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        //   ListSupp4 = proxy.GetAllMontureSupplementbycle(MontureRecu.Cle).Where(n => n.interfaceMonture == 4).ToList();
                                        ListSupp4 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp4;
                                    }
                                    else
                                    {
                                        Produit = new SVC.Produit();
                                        griddetail.DataContext = Produit;
                                        btnselection.IsEnabled = true;
                                        ListSupp4 = listrecu;
                                        SuppDataGrid.ItemsSource = ListSupp4;
                                    }
                                }
                                else
                                {
                                    Produit = new SVC.Produit();
                                    griddetail.DataContext = Produit;
                                    btnselection.IsEnabled = true;
                                    ListSupp4 = listrecu;
                                    SuppDataGrid.ItemsSource = ListSupp4;
                                }
                                this.Title = "Sélection du verre GAUCHE";

                                break;

                        }
                    }
                }
                if (selectedparam.ModiPrix == true)
                {
                    txtPrix.IsEnabled = true;
                }
                else
                {
                    txtPrix.IsEnabled = false;
                }

                //  PatientDataGrid.ItemsSource = (proxy.GetAllVerre().OrderBy(n => n.Design));











                /****************supplement******************/

                /*  var ll = proxy.GetAllMontureSupplementbycle(SelectedMonture.Cle).Any();

                  if (ll==true)
                  {
                      SuppDataGrid.ItemsSource = proxy.GetAllMontureSupplementbycle(SelectedMonture.Cle);

                  }else

                  {
                      SuppDataGrid.ItemsSource = list;
                  }*/

                PatientDataGrid.SelectedItem = null;
              
                callbackrecu.InsertVerreCallbackevent += new ICallback.CallbackEventHandler31(callbackrecu_Refresh);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
       

        
        void callbackrecu_Refresh(object source, CallbackEventInsertVerre e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }



        public void AddRefresh(SVC.Verre listMembershipOptic, int oper)
        {
            try
            {

                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Verre>;
                List<SVC.Verre> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {
                        //  var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;


                        var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        //objectmodifed = listMembershipOptic;
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listMembershipOptic;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                PatientDataGrid.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionVerre(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerSelectionVerre(HandleProxy));
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


        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }


        }


        private void btnselection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (interfacenew == 0)
                {
                    switch (verrelocalisation)
                    {
                        case 1:
                            SelectedMonture.IdDroiteVerreLoin = Produit.Id;
                            SelectedMonture.DroiteVerreLoinDesignation = Produit.design;
                            SelectedMonture.DroitPrixVerreLoin =Convert.ToDecimal(txtPrix.Text);
                            SelectedMonture.DroiteStatutLoinVerre = 1;
                           
                            
                                break;
                        case 2:
                            SelectedMonture.IdGaucheVerreLoin = Produit.Id;
                            SelectedMonture.GaucheVerreLoinDesignation = Produit.design;
                            SelectedMonture.GauchePrixVerreLoin = Convert.ToDecimal(txtPrix.Text);
                            SelectedMonture.GaucheStatutLoinVerre = 1;

                            break;
                        case 3:
                            SelectedMonture.IdDroiteVerrePres = Produit.Id;
                            SelectedMonture.DroiteVerrePresDesignation = Produit.design;
                            SelectedMonture.DroitPrixVerrePres = Convert.ToDecimal(txtPrix.Text);
                            SelectedMonture.DroiteStatutPresVerre = 1;

                            break;
                        case 4:
                            SelectedMonture.IdGaucheVerrePres = Produit.Id;
                            SelectedMonture.GaucheVerrePresDesignation = Produit.design;
                            SelectedMonture.GauchePrixVerrePres = Convert.ToDecimal(txtPrix.Text);
                            SelectedMonture.GaucheStatutPresVerre = 1;
                            break;
                     

                    }
                    this.Close();

                }
                else
                {
                    if (interfacenew ==1)
                    {
                        switch (verrelocalisation)
                        {
                            case 1:
                                SelectedMonture.IdDroiteVerreLoin = Produit.Id;
                                SelectedMonture.DroiteVerreLoinDesignation = Produit.design;
                                SelectedMonture.DroitPrixVerreLoin = Convert.ToDecimal(txtPrix.Text);
                                SelectedMonture.DroiteStatutLoinVerre = 1;
                              
                                break;
                            case 2:
                                SelectedMonture.IdGaucheVerreLoin = Produit.Id;
                                SelectedMonture.GaucheVerreLoinDesignation = Produit.design;
                                SelectedMonture.GauchePrixVerreLoin = Convert.ToDecimal(txtPrix.Text);
                                SelectedMonture.GaucheStatutLoinVerre = 1;

                                break;
                            case 3:
                                SelectedMonture.IdDroiteVerrePres = Produit.Id;
                                SelectedMonture.DroiteVerrePresDesignation = Produit.design;
                                SelectedMonture.DroitPrixVerrePres = Convert.ToDecimal(txtPrix.Text);
                                SelectedMonture.DroiteStatutPresVerre = 1;

                                break;
                            case 4:
                                SelectedMonture.IdGaucheVerrePres = Produit.Id;
                                SelectedMonture.GaucheVerrePresDesignation = Produit.design;
                                SelectedMonture.GauchePrixVerrePres = Convert.ToDecimal(txtPrix.Text);
                                SelectedMonture.GaucheStatutPresVerre = 1;
                                break;


                        }
                    }
                 
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
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


        private void txtPrix_KeyDown(object sender, KeyEventArgs e)
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

       
        private void FamilleCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.ItemsSource = (proxy.GetAllVerre().OrderBy(n => n.Design));
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        

        
        


       


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationDossierClient==true)
                {
                    ListeSupplement cl = new ListeSupplement(proxy,MemberUser,callback,this);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SuppDataGrid.SelectedItem != null && interfacenew == 0)
                {
                    SVC.MontureSupplement selecteditem = SuppDataGrid.SelectedItem as SVC.MontureSupplement;
                    var LISTITEM1 = SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;
                    List<SVC.MontureSupplement> LISTITEM = LISTITEM1.ToList();
                    // var deleterendez = LISTITEM.Where(n => n.nfact == listmembership.nfact).First();
                    LISTITEM.Remove(selecteditem);
                    
                    SuppDataGrid.ItemsSource = LISTITEM;

                    switch (verrelocalisation)
                    {
                        case 1:
                            ListSupp1.Remove(selecteditem);
                            break;
                        case 2:
                            ListSupp2.Remove(selecteditem);
                            break;
                        case 3:
                            ListSupp3.Remove(selecteditem);

                            break;
                        case 4:
                            ListSupp4.Remove(selecteditem);
                            break;

                    }
                }
                else
                {
                    if (SuppDataGrid.SelectedItem != null && interfacenew == 1)
                    {
                        SVC.MontureSupplement selecteditem = SuppDataGrid.SelectedItem as SVC.MontureSupplement;
                        bool succe = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.DeleteMontureSupplement(selecteditem);
                            succe = true;
                        }
                        if (succe == true)
                        {
                            SVC.MontureSupplement selecteditem11 = SuppDataGrid.SelectedItem as SVC.MontureSupplement;
                            var LISTITEM1 = SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;
                            List<SVC.MontureSupplement> LISTITEM = LISTITEM1.ToList();
                            // var deleterendez = LISTITEM.Where(n => n.nfact == listmembership.nfact).First();
                            LISTITEM.Remove(selecteditem11);
                            switch (verrelocalisation)
                            {
                                case 1:
                                    ListSupp1.Remove(selecteditem);
                                    break;
                                case 2:
                                    ListSupp2.Remove(selecteditem);
                                    break;
                                case 3:
                                    ListSupp3.Remove(selecteditem);

                                    break;
                                case 4:
                                    ListSupp4.Remove(selecteditem);
                                    break;

                            }
                            SuppDataGrid.ItemsSource = LISTITEM;
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

      




       

         

          private void btnstock_Click(object sender, RoutedEventArgs e)
          {
              try
              {

                     SelectionProduit cl = new SelectionProduit(proxy, MemberUser, callback, verrelocalisation, SelectedMonture, interfacenew);
                  this.Close();
                  cl.Show();

              }
              catch(Exception ex)
              {
                  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
              }
          }



          private void SuppDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
          {
             try
              {
                 /* var LISTITEM1 = SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;

                  if (txtPrix.Text!="" && LISTITEM1!=null)
                  {
                      txtPrix.Text = Convert.ToString(Convert.ToDecimal(txtPrix.Text) + (LISTITEM1.AsEnumerable().Sum(n => n.PrixVente)));
                  }dsfg*/


                        if (PatientDataGrid.SelectedItem != null)
                {
                    var LISTITEM1 = SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;
                    
                        SVC.Verre SELECTEDTARIF = PatientDataGrid.SelectedItem as SVC.Verre;

                        if (LISTITEM1 != null)
                        {
                            txtPrix.Text = Convert.ToString(SELECTEDTARIF.PrixVente + (LISTITEM1.AsEnumerable().Sum(n => n.PrixVente)));
                        }
                        else
                        {
                            txtPrix.Text = SELECTEDTARIF.PrixVente.ToString();

                        }
                  
                    SVC.Verre selectedverre = PatientDataGrid.SelectedItem as SVC.Verre;

                    Produit = proxy.GetAllProduitbycab(selectedverre.cleproduit);
                    txtDesign.Text = selectedverre.Ref;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            } 
        }

        private void TarifDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if (PatientDataGrid.SelectedItem != null)
            {
                var LISTITEM1 = SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;
              

                    SVC.Verre SELECTEDTARIF = PatientDataGrid.SelectedItem as SVC.Verre;
                    // txtPrix.Text = SELECTEDTARIF.PrixVente.ToString();

                    if (LISTITEM1!=null)
                    {
                        txtPrix.Text = Convert.ToString(SELECTEDTARIF.PrixVente + (LISTITEM1.AsEnumerable().Sum(n => n.PrixVente)));
                    }else
                    {
                        txtPrix.Text = Convert.ToString(SELECTEDTARIF.PrixVente);
                    }
               
                SVC.Verre selectedverre = PatientDataGrid.SelectedItem as SVC.Verre;

                Produit = proxy.GetAllProduitbycab(selectedverre.cleproduit);
                txtDesign.Text = selectedverre.Ref;

            } 
        }

        

        

        private void btnProduit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationFichier == true)
                {
                    AjouterProduit cl = new AjouterProduit(proxy, null, MemberUser, callback);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
              /*  SVC.Fourn fourn = new SVC.Fourn();
                bool fournissseur = false;
                if (FamilleCombo.SelectedItem!=null)
                {
                    fourn = FamilleCombo.SelectedItem as SVC.Fourn;
                    fournissseur = true;
                } else
                {
                    fournissseur = false;
                }
                */
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    
                        cv.Filter = o =>
                        {
                            SVC.Verre p = o as SVC.Verre;
                            if (t.Name == "txtId")
                                return (p.Design == filter);
                            return (p.Design.ToUpper().Contains(filter.ToUpper()));
                        };
                    
                    
                }

                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void FamilleCombo_DropDownClosed(object sender, EventArgs e)
        {
            try
            {

                if (FamilleCombo.SelectedItem != null)
                {
                    SVC.Fourn selectedfourn = FamilleCombo.SelectedItem as SVC.Fourn;
                    string filter = selectedfourn.raison;
                    
                   
                    ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    if (filter == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            SVC.Verre p = o as SVC.Verre;
                            if (filter != null)
                                return (p.Fournisseur == filter);
                            return (p.Fournisseur.ToUpper().Contains(filter.ToUpper()));
                        };
                    }

                  
                    PatientDataGrid.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtCyl_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {



                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Verre p = o as SVC.Verre;
                        if (t.Name == "txtId")
                            return (p.Cyl.ToString() == filter);
                        return (p.Cyl.ToString().Contains(filter));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtSph_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Verre p = o as SVC.Verre;
                        if (t.Name == "txtId")
                            return (p.Sph.ToString() == filter);
                        return (p.Sph.ToString().Contains(filter));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }


            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void PatientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //  filtre();
                if (PatientDataGrid.SelectedItem != null)
                {
                    var LISTITEM1 = SuppDataGrid.ItemsSource as IEnumerable<SVC.MontureSupplement>;

                    SVC.Verre SELECTEDTARIF = PatientDataGrid.SelectedItem as SVC.Verre;

                    if (LISTITEM1 != null)
                    {
                        txtPrix.Text = Convert.ToString(SELECTEDTARIF.PrixVente + (LISTITEM1.AsEnumerable().Sum(n => n.PrixVente)));
                    }
                    else
                    {
                        txtPrix.Text = SELECTEDTARIF.PrixVente.ToString();

                    }

                    SVC.Verre selectedverre = PatientDataGrid.SelectedItem as SVC.Verre;

                    Produit = proxy.GetAllProduitbycab(selectedverre.cleproduit);
                    txtDesign.Text = selectedverre.Design;
                    //MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(Produit.design+" "+Produit.marque, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtType_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
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
                        SVC.Verre p = o as SVC.Verre;
                        if (t.Name == "txtId")
                            return (p.TypeVerre == filter);
                        return (p.TypeVerre.ToUpper().Contains(filter.ToUpper()));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btncommande_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationCommande==true /*&& Produit!=null*/)
                {
                    AjouterCommande cl = new AjouterCommande(proxy, MemberUser, callback, null, Produit, CLIENTV);
                        cl.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

     /*   void filtre()
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Verre selectedverre = PatientDataGrid.SelectedItem as SVC.Verre;

                    listtatif = new List<TarifList>();


                    Cylindre = CompteComboBox.Text.ToString();//.SelectedText;
                    Spher = SphComboBox.Text.ToString();
                    var list = proxy.GetAllTarifVerrebycode(selectedverre.Cletarif);

                    switch (Cylindre)
                    {
                        case "0.00":

                            switch (Spher)
                            {
                                case "-99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0;
                                    break;
                                case "-14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0;
                                    break;
                                case "-12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0;
                                    break;
                                case "12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H1),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                            }

                            break;
                        /***************************2.00**********/
                  /*      case "2.00":
                            switch (Spher)
                            {
                                case "-99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                            }
                            break;
                        /**********************************4****************************/
                   /*     case "4.00":
                            switch (Spher)
                            {
                                case "-99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F2),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H3),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                            }
                            break;
                        /***************************6********************/
                      /*  case "6.00":
                            switch (Spher)
                            {
                                case "-99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "-2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "2.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.A4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "4.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.B4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "6.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.C4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "8.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.D4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "10.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.E4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "12.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.F4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "14.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.G4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                                case "99.00":
                                    foreach (var item in list)
                                    {
                                        TarifList tarif = new TarifList
                                        {
                                            Diametre = selectedverre.Diamètre,
                                            PrixVente = Convert.ToDecimal(item.H4),
                                        };
                                        if (item.PrixVente == true)
                                        {
                                            listtatif.Add(tarif);
                                        }
                                    }
                                    TarifDataGrid.ItemsSource = listtatif;
                                    TarifDataGrid.SelectedIndex = 0; break;
                            }
                            break;
                    }



                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        */

    }
    public partial class TarifList
    {
       
        public string Diametre { get; set; }
        public Decimal PrixVente { get; set; }
       

    }
}