
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

namespace NewOptics.Fournisseur
{
    /// <summary>
    /// Interaction logic for AjouterFournisseur.xaml
    /// </summary>
    public partial class AjouterFournisseur :Window
    {
        SVC.Fourn special;
        SVC.ServiceCliniqueClient proxy;
      
        private delegate void FaultedInvokerNewFourn();
        SVC.MembershipOptic MembershipOptic;
        string title;
        Brush titlebrush;
        int interfacecréation = 0;
     

        public AjouterFournisseur(SVC.ServiceCliniqueClient proxyrecu, SVC.Fourn spécialtiérecu, SVC.MembershipOptic membershirecu)
        {
            try
            {
                InitializeComponent();
              //  Thread.CurrentThread.CurrentCulture = new CultureInfo("en-Us");
                proxy = proxyrecu;
                special = spécialtiérecu;
                MembershipOptic = membershirecu;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (special != null)
                {
                  FournVousGrid.DataContext = special;
                    txtHoraire.Text = special.solde.ToString();
                    interfacecréation = 1;
                    f.Content = "Modifier Fournisseur";
                }
                else
                {
                    special = new SVC.Fourn { solde=0};
                    FournVousGrid.DataContext = special;
                /*    btnCreer.IsEnabled = false;
                    interfacecréation = 0;*/
                }
                title = this.Title;
        //        titlebrush = this.WindowTitleBrush;
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewFourn(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerNewFourn(HandleProxy));
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

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.CreationFichier == true && interfacecréation == 0)
                {
                    bool insertFournsucces = false;
                    bool insertRecoufsucces = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                       special.dates = DateTime.Now;

                        special.UserName = MembershipOptic.Username;

                        if (txtHoraire.Text != "")
                        {
                            special.solde = Convert.ToDecimal(txtHoraire.Text.Trim());
                            special.cle = special.raison + DateTime.Now + DateTime.Now.TimeOfDay;

                            if (special.solde > 0)
                            {
                                SVC.Recouf soldededépart = new SVC.Recouf
                                {
                                    avoir = false,
                                    cle = special.cle,
                                    cmp = false,
                                    codef = 0,
                                    dates = special.dates,
                                    date = special.dates,
                                    datecreat = special.dates,
                                    dateecheance = special.dates,
                                    echeance = 0,
                                    fiscal = false,
                                    Fournisseur = special.raison,
                                    ht = special.solde,
                                    net = special.solde,
                                    nfact = "Solde de départ" + " " + special.raison,
                                    Nonfiscal = true,
                                    opercreat = MembershipOptic.Username,
                                    remise = 0,
                                    reste = special.solde,
                                    tva = 0,
                                    username = MembershipOptic.Username,
                                    versement = 0,
                                    
                                };
                                proxy.InsertRecouf(soldededépart);
                                insertRecoufsucces = true;
                                //    }
                            }
                            else
                            {
                                special.solde = 0;
                                insertRecoufsucces = true;
                            }
                            proxy.InsertFourn(special);
                            insertFournsucces = true;

                            if (insertFournsucces == true && insertRecoufsucces == true)
                            {
                                ts.Complete();
                            }
                            else
                            {
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }


                        }
                  
                    }
                    if (insertFournsucces == true && insertRecoufsucces == true)
                    {
                        proxy.AjouterFournRefresh();
                        proxy.AjouterRecoufRefresh();
                        MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {
                    if (MembershipOptic.ModificationFichier == true && interfacecréation == 1)
                    {
                        int interfaceupdate = 0;
                        bool insertFournsucces = false;
                        bool insertRecoufsucces = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                      
                       

                            var found = (proxy.GetAllRecouf()).Find(n => n.cle == special.cle);
                            if (found == null)
                            {
                                if (txtHoraire.Text != "")
                                {
                                    special.solde = Convert.ToDecimal(txtHoraire.Text.Trim());

                                    if (special.solde > 0)
                                    {
                                        SVC.Recouf soldededépart = new SVC.Recouf
                                        {
                                            avoir = false,
                                            cle = special.cle,
                                            cmp = false,
                                            codef = 0,
                                            dates = special.dates,
                                            date = special.dates,
                                            datecreat = special.dates,
                                            dateecheance = special.dates,
                                            echeance = 0,
                                            fiscal = false,
                                            Fournisseur = special.raison,
                                            ht = special.solde,
                                            net = special.solde,
                                            nfact = "Solde de départ" + " " + special.raison,
                                            Nonfiscal = true,
                                            opercreat = MembershipOptic.Username,
                                            remise = 0,
                                            reste = special.solde,
                                            tva = 0,
                                            username = MembershipOptic.Username,
                                            versement = 0,
                                        };
                                        proxy.InsertRecouf(soldededépart);
                                        insertRecoufsucces = true;
                                        interfaceupdate = 1;
                                    }
                                }
                                else
                                {
                                    special.solde = 0;
                                }

                            }
                            else
                            {
                                if (found != null)
                                {
                                    if (txtHoraire.Text != "")
                                    {
                                        if (special.solde > 0)
                                        {
                                            found.ht = special.solde;
                                            found.net = special.solde;
                                            found.reste = found.net - found.versement;
                                            proxy.UpdateRecouf(found);
                                            insertRecoufsucces = true;
                                            interfaceupdate = 1;
                                        }
                                        else
                                        {
                                            if (special.solde == 0)
                                            {
                                                if (found.versement > 0)
                                                {
                                                    MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention le solde de départ contient un réglement vous ne pouvez pas le mettre a zéro", "Medicus", MessageBoxButton.OK, MessageBoxImage.Error);
                                                    insertRecoufsucces = false;
                                                    interfaceupdate = 1;
                                                }
                                                else
                                                {
                                                    proxy.DeleteRecouf(found);
                                                    insertRecoufsucces = true;
                                                    interfaceupdate = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            special.UserName = MembershipOptic.Username;

                            special.solde = Convert.ToDecimal(txtHoraire.Text.Trim());
                            proxy.UpdateFourn(special);
                            insertFournsucces = true;
                     
                        if (insertFournsucces == true && interfaceupdate == 0)
                        {
                            ts.Complete();
                         }
                        else
                        {
                            if (insertFournsucces == true && interfaceupdate == 1 && insertRecoufsucces == true)
                            {
                                ts.Complete();
                            }
                            else
                            {
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.Opérationéchouée, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                        if (insertFournsucces == true && interfaceupdate == 0)
                        {
                            proxy.AjouterFournRefresh();
                            proxy.AjouterRecoufRefresh();
                            MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            this.Close();
                        }
                        else
                        {
                            if (insertFournsucces == true && interfaceupdate == 1 && insertRecoufsucces == true)
                            {
                                proxy.AjouterFournRefresh();
                                proxy.AjouterRecoufRefresh();
                                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                                this.Close();
                            }
                            
                        }
                    }
                    
                }
            }catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

       
        private void BTNCLOSTE_Click(object sender, RoutedEventArgs e)
        {
            try { 
            this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtHoraire_KeyDown(object sender, KeyEventArgs e)
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

       

        private void txtRaison_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtRaison.Text != null && interfacecréation == 0)
                {

                    var query = from c in proxy.GetAllFourn()
                                select new { c.raison };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.raison.Trim().ToUpper() == txtRaison.Text.Trim().ToUpper()).FirstOrDefault();

                    if (disponible != null)
                    {
                        this.Title = "Ce fournisseur Existe";
                        //    this.WindowTitleBrush = Brushes.Red;

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;


                    }
                    else
                    {
                        if (txtRaison.Text.Trim() != "")
                        {
                            this.Title = title;
                            //     this.WindowTitleBrush = titlebrush;
                            btnCreer.IsEnabled = true;
                            btnCreer.Opacity = 1;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
