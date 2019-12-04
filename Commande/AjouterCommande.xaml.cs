
using NewOptics.Administrateur;
using NewOptics.ClientA;
using NewOptics.Fournisseur;
using NewOptics.Stock;
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

namespace NewOptics.Commande
{
    /// <summary>
    /// Interaction logic for AjouterCommande.xaml
    /// </summary>
    public partial class AjouterCommande :Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        SVC.Commande CommandeClientFournisseur;
        ICallback callback;
        private delegate void FaultedInvokerCommande();
        int creationmodification = 1;
       
        public AjouterCommande(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu,SVC.Commande commanderecu,SVC.Produit produitrecu,SVC.ClientV CLIENTRECU)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
                if(commanderecu!=null)
                {
                   
                   
                    creationmodification = 2;
                    CommandeClientFournisseur = commanderecu;
                    FournVousGrid.DataContext = CommandeClientFournisseur;
                    /***********************famille***********************/
                    List<SVC.FamilleProduit> testmarque = proxy.GetAllFamilleProduit();
                    ComboFamilleProduit.ItemsSource = testmarque;

                    if (CommandeClientFournisseur.IdFamille != 0)
                    {
                        List<SVC.FamilleProduit> tte = testmarque.Where(n => n.Id == CommandeClientFournisseur.IdFamille).OrderBy(n => n.Id).ToList();
                        ComboFamilleProduit.SelectedItem = tte.First();
                    }
                    /****************************************client****////////
                    List<SVC.ClientV> testclient = proxy.GetAllClientV();
                    ComboClient.ItemsSource = testclient;
                    List<SVC.ClientV> tteclient = testclient.Where(n => n.Id == CommandeClientFournisseur.IdClient).OrderBy(n => n.Id).ToList();
                    ComboClient.SelectedItem = tteclient.First();
                    /**********************produit***************************/
                    List<SVC.Produit> testproduit = proxy.GetAllProduit();
                    ComboProduit.ItemsSource = testproduit;
                    List<SVC.Produit> tteproduit = testproduit.Where(n => n.Id == CommandeClientFournisseur.IdProduit).OrderBy(n => n.Id).ToList();
                    ComboProduit.SelectedItem = tteproduit.First();
                    /************************fournisseur*********************************/
                    List<SVC.Fourn> testfourn = proxy.GetAllFourn();
                    ComboFourn.ItemsSource = testfourn;
                    List<SVC.Fourn> ttefourn = testfourn.Where(n => n.Id == CommandeClientFournisseur.IdFournisseur).OrderBy(n => n.Id).ToList();
                    ComboFourn.SelectedItem = ttefourn.First();
                    if (commanderecu.Réceptionée == false)
                    {
                        FournVousGrid.IsEnabled = true;
                    }
                    else
                    {
                        FournVousGrid.IsEnabled = false;
                        MessageBoxResult resultdc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Commande réceptionnée modification impossible ", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                }
                else
                {
                    
                    
                    if (produitrecu!=null)
                    {
                        List<SVC.Produit> testmedecin = new List<SVC.Produit>();
                        testmedecin.Add(produitrecu);
                        List <SVC.Produit> tte = testmedecin;
                        ComboProduit.ItemsSource = tte;
                        ComboProduit.SelectedItem = tte.First();
                        
                    }
                    else
                    {
                        List<SVC.Produit> testmedecin = proxy.GetAllProduit().OrderBy(n => n.design).OrderBy(n => n.design).ToList();
                        ComboProduit.ItemsSource = testmedecin;
                       
                    }
                   
                    if (CLIENTRECU != null)
                   {
                        List<SVC.ClientV> tte =new List<SVC.ClientV>();
                        tte.Add(CLIENTRECU);
                        ComboClient.ItemsSource = tte;
                        ComboClient.SelectedItem = tte.First();
                    }
                    else
                    {
                        List<SVC.ClientV> testclient = proxy.GetAllClientV().OrderBy(n => n.Raison).ToList();
                        ComboClient.ItemsSource = testclient;
                    }
                    
                    creationmodification = 1;
                    btnCreer.IsEnabled = false;
                    DateCommande.SelectedDate = DateTime.Now;
                    CommandeClientFournisseur = new SVC.Commande
                    {
                        CylindreMoin = false,
                        CylindrePlus = true,
                       DateCommande=DateTime.Now,
                      
                       Effectuée=false,
                       Réceptionée=false,
                       Quantite=0,
                      
                    };
                    FournVousGrid.DataContext = CommandeClientFournisseur;
               
                 ComboFourn.ItemsSource = proxy.GetAllFourn().OrderBy(n => n.raison);
                    ComboFamilleProduit.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(n => n.FamilleProduit1);

                }
                callbackrecu.InsertFamilleProduitCallbackevent += new ICallback.CallbackEventHandler58(callbackDci_Refresh);
                callbackrecu.InsertFournCallbackEvent += new ICallback.CallbackEventHandler20(callbackrecufourn_Refresh);
                callbackrecu.InsertProduitCallbackEvent += new ICallback.CallbackEventHandler22(callbackrecu_Refresh);
                callbackrecu.InsertClientVCallbackEvent += new ICallback.CallbackEventHandler7(callbackrecuClient_Refresh);


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        void callbackrecuClient_Refresh(object source, CallbackEventInsertClientV e)
        {
            try
            {
                Dispatcher.BeginInvoke
                    (DispatcherPriority.Normal, new Action(delegate
                    {
                        AddRefreshClientV(e.clientleav, e.operleav);
                    }));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefreshClientV(SVC.ClientV listMembershipOptic, int oper)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        var LISTITEM11 = ComboClient.ItemsSource as IEnumerable<SVC.ClientV>;
                        List<SVC.ClientV> LISTITEM0 = LISTITEM11.ToList();

                        if (oper == 1)
                        {
                            LISTITEM0.Add(listMembershipOptic);
                        }
                        else
                        {
                            if (oper == 2)
                            {


                                var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                //objectmodifed = listMembershipOptic;
                                var index = LISTITEM0.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM0[index] = listMembershipOptic;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    var deleterendez = LISTITEM0.Where(n => n.Id == listMembershipOptic.Id).First();
                                    LISTITEM0.Remove(deleterendez);
                                }
                            }
                        }











                        ComboClient.ItemsSource = LISTITEM0.OrderBy(n => n.Raison);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        void callbackrecu_Refresh(object source, CallbackEventInsertProduit e)
        {
            try
            {
                Dispatcher.BeginInvoke
                    (DispatcherPriority.Normal, new Action(delegate
                    {
                        AddRefresh(e.clientleav, e.operleav);
                    }));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecufourn_Refresh(object source, CallbackEventInsertFourn e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshFourn(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(SVC.Produit listMembershipOptic, int oper)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        var LISTITEM11 = ComboProduit.ItemsSource as IEnumerable<SVC.Produit>;
                        List<SVC.Produit> LISTITEM0 = LISTITEM11.ToList();

                        if (oper == 1)
                        {
                            LISTITEM0.Add(listMembershipOptic);
                        }
                        else
                        {
                            if (oper == 2)
                            {


                                var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                //objectmodifed = listMembershipOptic;
                                var index = LISTITEM0.IndexOf(objectmodifed);
                                if (index != -1)
                                    LISTITEM0[index] = listMembershipOptic;
                            }
                            else
                            {
                                if (oper == 3)
                                {
                                    //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listMembershipOptic.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                                    var deleterendez = LISTITEM0.Where(n => n.Id == listMembershipOptic.Id).First();
                                    LISTITEM0.Remove(deleterendez);
                                }
                            }
                        }











                        ComboProduit.ItemsSource = LISTITEM0.OrderBy(n => n.design);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        public void AddRefreshFourn(List<SVC.Fourn> listMembershipOptic)
        {

            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        if (creationmodification == 1)
                        {


                           ComboFourn.ItemsSource = listMembershipOptic;
                        }
                        else
                        {
                            if (creationmodification == 2)
                            {

                                List<SVC.Fourn> testmedecin = listMembershipOptic;
                                ComboFourn.ItemsSource = testmedecin;
                                List<SVC.Fourn> tte = testmedecin.Where(n => n.Id == CommandeClientFournisseur.IdFournisseur).ToList();
                                ComboFourn.SelectedItem = tte.First();
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        void callbackDci_Refresh(object source, CallbackEventInsertFamilleProduit e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefresh(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefresh(List<SVC.FamilleProduit> listMembershipOptic)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        HandleProxy();
                    }
                    else
                    {
                        if (creationmodification == 1)
                        {


                            ComboFamilleProduit.ItemsSource = listMembershipOptic;
                        }
                        else
                        {
                            if (creationmodification == 2)
                            {

                                List<SVC.FamilleProduit> testmedecin = listMembershipOptic;
                                ComboFamilleProduit.ItemsSource = testmedecin;
                                List<SVC.FamilleProduit> tte = testmedecin.Where(n => n.Id == CommandeClientFournisseur.IdFamille).ToList();
                                ComboFamilleProduit.SelectedItem = tte.First();
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
                if (creationmodification == 1 && ComboFourn.SelectedItem!=null && ComboProduit.SelectedItem!=null && memberuser.CreationCommande==true)
                {
                    bool succes = false;
                    int IdClient, IdFamilleProduit = 0;
                    string client,familleproduit = "";

                    SVC.Produit selectedproduit = ComboProduit.SelectedItem as SVC.Produit;
                    SVC.Fourn selectedfourn = ComboFourn.SelectedItem as SVC.Fourn;
                    if(ComboClient.SelectedItem!=null)
                    {
                        SVC.ClientV selectedclient = ComboClient.SelectedItem as SVC.ClientV;
                        client = selectedclient.Raison;
                        IdClient = selectedclient.Id;
                    }
                    else
                    {
                        client = "Vente comptoir";
                        IdClient = 0;
                    }
                    if(ComboFamilleProduit.SelectedItem!=null)
                    {
                        SVC.FamilleProduit selectedfamille = ComboFamilleProduit.SelectedItem as SVC.FamilleProduit;
                        IdFamilleProduit = selectedfamille.Id;
                        familleproduit = selectedfamille.FamilleProduit1;
                    }
                    else
                    {
                        IdFamilleProduit = 0;
                        familleproduit = "";
                    }
                    CommandeClientFournisseur.DesignProduit = selectedproduit.design;
                    CommandeClientFournisseur.FamilleProduit = familleproduit;
                    CommandeClientFournisseur.Fournisseur = selectedfourn.raison;
                    CommandeClientFournisseur.IdClient =IdClient;
                    CommandeClientFournisseur.IdFamille = IdFamilleProduit;
                    CommandeClientFournisseur.IdFournisseur = selectedfourn.Id;
                    CommandeClientFournisseur.IdProduit = selectedproduit.Id;
                    CommandeClientFournisseur.RaisonClient = client;
                    CommandeClientFournisseur.clecab = selectedproduit.clecab;
                    CommandeClientFournisseur.CLE = selectedproduit.Id.ToString() + selectedproduit.design + selectedfourn.Id + selectedfourn.raison + DateTime.Now;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.InsertCommande(CommandeClientFournisseur);
                        ts.Complete();
                        succes = true;
                    }
                    if(succes)
                    {
                        proxy.AjouterCommandeRefresh();
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {
                    if (creationmodification == 2 && memberuser.ModificationCommande==true)
                    {
                        bool succes = false;
                        int IdClient, IdFamilleProduit = 0;
                        string client, familleproduit = "";

                        SVC.Produit selectedproduit = ComboProduit.SelectedItem as SVC.Produit;
                        SVC.Fourn selectedfourn = ComboFourn.SelectedItem as SVC.Fourn;
                        if (ComboClient.SelectedItem != null)
                        {
                            SVC.ClientV selectedclient = ComboClient.SelectedItem as SVC.ClientV;
                            client = selectedclient.Raison;
                            IdClient = selectedclient.Id;
                        }
                        else
                        {
                            client = "Vente comptoir";
                            IdClient = 0;
                        }
                        if (ComboFamilleProduit.SelectedItem != null)
                        {
                            SVC.FamilleProduit selectedfamille = ComboFamilleProduit.SelectedItem as SVC.FamilleProduit;
                            IdFamilleProduit = selectedfamille.Id;
                            familleproduit = selectedfamille.FamilleProduit1;
                        }
                        else
                        {
                            IdFamilleProduit = 0;
                            familleproduit = "";
                        }
                        CommandeClientFournisseur.DesignProduit = selectedproduit.design;
                        CommandeClientFournisseur.FamilleProduit = familleproduit;
                        CommandeClientFournisseur.Fournisseur = selectedfourn.raison;
                        CommandeClientFournisseur.IdClient = IdClient;
                        CommandeClientFournisseur.IdFamille = IdFamilleProduit;
                        CommandeClientFournisseur.IdFournisseur = selectedfourn.Id;
                        CommandeClientFournisseur.IdProduit = selectedproduit.Id;
                        CommandeClientFournisseur.RaisonClient = client;
                        CommandeClientFournisseur.clecab = selectedproduit.clecab;
                        CommandeClientFournisseur.CLE = selectedproduit.Id.ToString() + selectedproduit.design + selectedfourn.Id + selectedfourn.raison + DateTime.Now;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.UpdateCommande(CommandeClientFournisseur);
                            ts.Complete();
                            succes = true;
                        }
                        if (succes)
                        {
                            proxy.AjouterCommandeRefresh();
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                    }
                }

            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtStockAlert_KeyDown(object sender, KeyEventArgs e)
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
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCommande(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCommande(HandleProxy));
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
        private void btnformule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSphère.Text != "" && txtCylindre.Text!="" && txtAxe.Text!="")
                {
                    int axe = 0;
                    decimal sphere, cylindre = 0;

                    if (txtSphère.Text != "")
                    {
                        if (decimal.TryParse(txtSphère.Text, out sphere))
                            sphere = Convert.ToDecimal(txtSphère.Text);
                        else
                            sphere = 0;
                    }
                    else
                    {
                        txtSphère.Text = "";
                        sphere = 0;
                    }
                    if (txtCylindre.Text != "")
                    {
                        if (decimal.TryParse(txtCylindre.Text, out cylindre))
                        {
                            cylindre = Convert.ToDecimal(txtCylindre.Text);
                            if (cylindre > 0)
                            {
                                CommandeClientFournisseur.CylindrePlus = true;
                                CommandeClientFournisseur.CylindreMoin = false;
                            }
                            else
                            {
                                if (cylindre < 0)
                                {
                                    CommandeClientFournisseur.CylindrePlus = false;
                                    CommandeClientFournisseur.CylindreMoin = true;
                                }
                            }
                        }
                        else
                        {
                            cylindre = 0;
                        }
                    }
                    else
                    {
                        txtCylindre.Text = "";
                        cylindre = 0;
                    }

                    if (txtAxe.Text != "")
                    {
                        if (int.TryParse(txtAxe.Text, out axe))
                            axe = Convert.ToInt16(txtAxe.Text);
                        else
                            axe = 0;
                    }
                    else
                    {
                        txtAxe.Text = "";
                        axe = 0;
                    }
                    txtCylindre.Text = ((-cylindre)).ToString("+#.##;-#.##;0");
                    txtSphère.Text = (sphere + cylindre).ToString("+#.##;-#.##;0");
                    if (CommandeClientFournisseur.CylindrePlus == true && CommandeClientFournisseur.CylindreMoin == false)
                    {
                        txtAxe.Text = (axe + 90).ToString();
                        CommandeClientFournisseur.CylindrePlus = false;
                        CommandeClientFournisseur.CylindreMoin = true;

                    }
                    else
                    {
                        if (CommandeClientFournisseur.CylindrePlus == false && CommandeClientFournisseur.CylindreMoin == true)
                        {
                            txtAxe.Text = (axe - 90).ToString();
                            CommandeClientFournisseur.CylindrePlus = true;
                            CommandeClientFournisseur.CylindreMoin = false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

      

        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.CreationFichier==true)
                {
                    NewClient cl = new NewClient(proxy, memberuser, null);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnProduit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterProduit cl = new AjouterProduit(proxy,null, memberuser,callback);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnFournisseur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterFournisseur cl = new AjouterFournisseur(proxy, null, memberuser);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnFamille_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationFichier == true)
                {
                    AjouterFamilleProduit cl = new AjouterFamilleProduit(proxy, null, memberuser);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

       

        private void ckboxrec_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if(ckboxrec.IsChecked==true && DateRec.SelectedDate!=null)
                {
                    btnCreer.IsEnabled = true;
                }
                else
                {
                    if (ckboxrec.IsChecked == true && DateRec.SelectedDate == null)
                    {
                        btnCreer.IsEnabled = false;
                    }
                }
               
              
            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ckboxrec_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ckboxrec.IsChecked == false && DateRec.SelectedDate == null)
                {
                    btnCreer.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
 

        private void ComboFourn_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (ComboFourn.SelectedItem != null && ComboProduit.SelectedItem != null)
                {
                    btnCreer.IsEnabled = true;
                }
                else
                {
                    btnCreer.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ComboProduit_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (ComboFourn.SelectedItem != null && ComboProduit.SelectedItem != null)
                {
                    btnCreer.IsEnabled = true;
                }
                else
                {
                    btnCreer.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void DateRec_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ckboxrec.IsChecked == true)
                {
                    if (DateRec.SelectedDate != null)
                    {
                        btnCreer.IsEnabled = true;
                    }
                    else
                    {
                        btnCreer.IsEnabled = false;
                    }
                }
                else
                {
                    btnCreer.IsEnabled = true;
                }


            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
