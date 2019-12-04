
using NewOptics.Administrateur;
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
    /// Interaction logic for Reception.xaml
    /// </summary>
    public partial class Reception : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
 
        private delegate void FaultedInvokerComptoirReception();
        List<SVC.Commande> commandelist;
        Commande Usercontrol;
        SVC.Recouf recoufselected;
        bool ReceptResult = false;
        bool RecoufResult = false;
        bool ProdfResult = false;
        bool CommandeResult = false;
        public Reception(SVC.ServiceCliniqueClient proxyrecu, string fournisseur, SVC.MembershipOptic membershiprecu,List<SVC.Commande> commanderecu, Commande usercommande)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu ;
                txtFournisseur.Text = fournisseur;
                MemberUser = membershiprecu;
                commandelist = commanderecu;
                CONFIRMERVENTE.IsEnabled = false;
                CONFIRMERVENTE.Opacity = 0.2;
               Usercontrol = usercommande;
                datevente.SelectedDate = DateTime.Now.Date;
             //   MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(fournisseur, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
               
              
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerComptoirReception(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerComptoirReception(HandleProxy));
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

        private void txtremise_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                
                    if (txtremise.Text.Trim() != "")
                    {

                        var query = from c in proxy.GetAllRecouf()
                                    select new { c.nfact };

                        var results = query.ToList();
                        var disponible = results.Where(list1 => list1.nfact.Trim().ToUpper() == txtremise.Text.Trim().ToUpper()).FirstOrDefault();

                        if (disponible != null)
                        {
                            this.Title = "Ce numéro de facture Existe déja";
                            
                            CONFIRMERVENTE.IsEnabled = false;
                            CONFIRMERVENTE.Opacity = 0.2;


                        }
                        else
                        {
                            if (txtremise.Text.Trim() != "")
                            {
                                CONFIRMERVENTE.IsEnabled = true;
                                CONFIRMERVENTE.Opacity = 1;

                            }
                        }
                    }
                    else
                    {

                        CONFIRMERVENTE.IsEnabled = false;
                        CONFIRMERVENTE.Opacity = 0.2;
                    }
              
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void annulerVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Usercontrol.IsEnabled = true;
                this.Close();
            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void DXWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                Usercontrol.IsEnabled = true;
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CONFIRMERVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationAchat==true)
                {
                    recoufselected = new SVC.Recouf
                    {
                        avoir = false,
                        codef = Convert.ToInt32(commandelist.First().IdFournisseur),
                        datecreat = DateTime.Now,
                        dates = DateTime.Now,
                        fiscal = false,
                        Nonfiscal = true,
                        Fournisseur = commandelist.First().Fournisseur,
                        ht = commandelist.AsEnumerable().Sum(n => n.Quantite * n.PrixAchat),
                        net = commandelist.AsEnumerable().Sum(n => n.Quantite * n.PrixAchat),
                        opercreat = MemberUser.Username,
                        remise = 0,
                        reste = commandelist.AsEnumerable().Sum(n => n.Quantite * n.PrixAchat),
                        tva = 0,
                        username = MemberUser.Username,
                        versement = 0,

                    };
                    recoufselected.nfact = txtremise.Text.Trim();
                    recoufselected.date = datevente.SelectedDate.Value.Date;
                    recoufselected.cle = commandelist.First().IdFournisseur + recoufselected.nfact + recoufselected.date;
                    List<SVC.Recept> receptionlist = new List<SVC.Recept>();
                    foreach (SVC.Commande itemcommande in commandelist)
                    {
                        SVC.Recept cl = new SVC.Recept
                        {
                            design = itemcommande.DesignProduit,
                            codeprod = itemcommande.IdProduit,
                            cf = itemcommande.IdFournisseur,
                            Fournisseur = itemcommande.Fournisseur,
                            quantite = itemcommande.Quantite,
                            previent = itemcommande.PrixAchat,
                            prixa = 0,
                            prixb = 0,
                            prixc = 0,
                            dates = DateTime.Now,
                            date = recoufselected.date,
                            nfact = recoufselected.nfact,
                            collisage = 1,
                            lot = "",
                            tva = 0,
                            IdFamille = itemcommande.IdFamille,
                            cab = itemcommande.clecab,
                            CommandeId = itemcommande.Id,
                        };
                        receptionlist.Add(cl);

                    }
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        foreach (SVC.Commande itemcommande in commandelist)
                        {
                            CommandeResult = false;
                            itemcommande.DateReception = recoufselected.date;
                            itemcommande.Réceptionée = true;
                            itemcommande.Effectuée = true;

                            proxy.UpdateCommande(itemcommande);
                            CommandeResult = true;
                        }

                        foreach (SVC.Recept item in receptionlist)
                        {

                            ReceptResult = false;
                            proxy.InsertRecept(item);
                            ReceptResult = true;

                            SVC.Prodf pa = new SVC.Prodf();
                            ProdfResult = false;
                            pa.cp = item.codeprod;
                            pa.design = item.design;
                            pa.quantite = item.quantite ;
                            //  pa.previent = (item.previent + ((item.previent * item.tva) / 100)) / item.collisage;
                            /******************************/
                            pa.previent = (item.previent) ;
                            pa.prixa = (item.prixa);
                            pa.prixb = (item.prixb) ;
                            pa.prixc = (item.prixc);
                            pa.tva = item.tva;
                            pa.cab = item.cab;
                            pa.privente = (item.prixa) ;
                            /*********************************************/
                            pa.collisage = 1;
                            pa.nfact = item.nfact;
                            pa.fourn = item.Fournisseur;
                            pa.cf = item.cf;
                            pa.datef = item.date;
                            pa.dates = DateTime.Now;
                            pa.perempt = item.perempt;
                            pa.lot = item.lot;
                            pa.IdFamille = item.IdFamille;
                            pa.CommandeId = item.CommandeId;
                            proxy.InsertProdf(pa);
                            ProdfResult = true;



                        }
                        RecoufResult = false;
                        proxy.InsertRecouf(recoufselected);
                        RecoufResult = true;

                        if (ReceptResult && RecoufResult && ProdfResult && CommandeResult)
                        {
                            ts.Complete();


                            CONFIRMERVENTE.IsEnabled = false;

                        }
                        else
                        {

                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (ReceptResult && RecoufResult && ProdfResult && CommandeResult)
                    {
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        proxy.AjouterCommandeRefresh();
                        proxy.AjouterFactureAchatSansProdfRefresh(recoufselected.nfact.Trim());
                        proxy.AjouterProdfReceptRefresh(recoufselected.Fournisseur, recoufselected.nfact, recoufselected.codef);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void datevente_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(datevente.SelectedDate!=null)
                {
                    CONFIRMERVENTE.IsEnabled = true;
                }
                else
                {
                    CONFIRMERVENTE.IsEnabled = false;
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
