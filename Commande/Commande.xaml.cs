
using NewOptics.Administrateur;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Commande
{
    /// <summary>
    /// Interaction logic for Commande.xaml
    /// </summary>
    public partial class Commande : Page
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerCommande();
      
        List<SVC.Commande> CommandeEffeList;
        List<SVC.Commande> CommandeReceptionList;
        int filtre = 0;

      
        public Commande(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;

                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllCommandebydatecommande(DateSaisieFin.SelectedDate.Value, DateSaisieDébut.SelectedDate.Value);
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(n => n.raison);
                callbackrecu.InsertCommandeCallbackevent += new ICallback.CallbackEventHandler33(callbackrecu_Refresh);
                callbackrecu.InsertFournCallbackEvent += new ICallback.CallbackEventHandler20(callbackrecufourn_Refresh);

                CommandeEffeList = new List<SVC.Commande>();
                CommandeReceptionList = new List<SVC.Commande>();
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
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
                        

                           FournisseurCombo.ItemsSource = listMembershipOptic;
                       

                    }
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
                        proxy = null;

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        var wndlistsession = Window.GetWindow(this);

                        Grid test = (Grid)wndlistsession.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;
                        Button confirmer = (Button)wndlistsession.FindName("Confirmer");
                        confirmer.IsEnabled = false;
                        Grid tests = (Grid)wndlistsession.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerCommande(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertCommande e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav, e.operleav);
            }));
        }
        public void AddRefresh(SVC.Commande listmembership, int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Commande>;
                List<SVC.Commande> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                        //   var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        // objectmodifed = listmembership;


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
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }


                }
                PatientDataGrid.ItemsSource = LISTITEM;
                
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if(FournisseurCombo.SelectedItem!=null)
                {
                    var selectedfourn = FournisseurCombo.SelectedItem as SVC.Fourn;

                    if (ParDateCommande.IsChecked==true)
                    {
                        PatientDataGrid.ItemsSource = proxy.GetAllCommandebydatecommande(DateSaisieDébut.SelectedDate.Value.Date,DateSaisieFin.SelectedDate.Value.Date).Where(n=>n.IdFournisseur==selectedfourn.Id);
                        
                    }
                    else
                    {
                        if (ParDateReception.IsChecked == true)
                        {
                            PatientDataGrid.ItemsSource = proxy.GetAllCommandebydatereception(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.IdFournisseur == selectedfourn.Id);
                        }
                    }
                 
                }
                else
                {
                    if (ParDateCommande.IsChecked == true)
                    {
                        PatientDataGrid.ItemsSource = proxy.GetAllCommandebydatecommande(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date);
                    }
                    else
                    {
                        if (ParDateReception.IsChecked == true)
                        {
                            PatientDataGrid.ItemsSource = proxy.GetAllCommandebydatereception(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date);
                        }
                    }
                }


            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.CreationCommande==true)
                {
                    AjouterCommande cl = new AjouterCommande(proxy, memberuser, callback, null,null,null);
                    cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null && memberuser.SupressionCommande==true)
                {
                    SVC.Commande selectedcommande = PatientDataGrid.SelectedItem as SVC.Commande;
                    if (selectedcommande.Réceptionée!=true)
                    {
                        bool succees = false;
                        using (var ts = new TransactionScope())
                        {
                            proxy.DeleteCommande(selectedcommande);
                            ts.Complete();
                            succees = true;
                        }
                        if (succees == true)
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            proxy.AjouterCommandeRefresh();
                        }
                    }
                    else
                    {
                        MessageBoxResult resultdc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Commande réceptionnée supression impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionnez une commande", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem!=null && memberuser.ModificationCommande==true)
                {
                   
                    SVC.Commande selectedcommande = PatientDataGrid.SelectedItem as SVC.Commande;
                    AjouterCommande cl = new AjouterCommande(proxy, memberuser, callback, selectedcommande,null,null);
                    cl.Show();
                } else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionnez une commande", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ImpressionCommande==true)
                {
                    if (PatientDataGrid.Items.Count > 0 )
                    {
                        // List<SVC.Commande> itemsSource0 = new List<SVC.Commande>();
                        List<SVC.Commande> itemsSource0 = PatientDataGrid.Items.OfType<SVC.Commande>().ToList();

                       
                       // var itemsSource0 = PatientDataGrid.ItemsSource.OfType<SVC.Commande>();
                        ImpressionCommande cl = new ImpressionCommande(proxy, itemsSource0, DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date);
                        cl.Show();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }
               /* else
                {
                    if (CommandeEffeList.Count > 0)
                    {
                        ImpressionCommande cl = new ImpressionCommande(proxy, CommandeEffeList, DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date);
                        cl.Show();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }*/
                
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FournisseurCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(n => n.raison);

            }catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        
     

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Commande cl = PatientDataGrid.SelectedItem as SVC.Commande;
                    if (CommandeReceptionList.Count == 0)
                    {
                        if (cl.Réceptionée == false )
                        {
                            if (cl.PrixAchat > 0)
                            {
                                if (cl.Quantite>0)
                                {
                                    CommandeReceptionList.Add(cl);
                                }
                                else
                                {
                                    Handle(sender as CheckBox);
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Cette commande n'a pas de quantité valide,elle ne sera pas séléctionnée pour réception", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                                }
                            }
                            else
                            {
                                Handle(sender as CheckBox);
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Cette commande n'a pas de prix d'achat valide,elle ne sera pas séléctionnée pour réception", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                        else
                        {
                            Handle(sender as CheckBox);
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Cette commande a déja été récéptionnée elle ne sera pas séléctionnée pour réception", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        if (CommandeReceptionList.Count > 0)
                        {
                            if (cl.IdFournisseur==CommandeReceptionList.First().IdFournisseur)
                            {
                                if (cl.Réceptionée == false)
                                {
                                    CommandeReceptionList.Add(cl);
                                }
                                else
                                {
                                    Handle(sender as CheckBox);
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Cette commande a déja été récéptionnée elle ne sera pas séléctionnée pour réception", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                Handle(sender as CheckBox);
                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous pouvez seulement séléctionnez les commandes du même fournisseur pour réception", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                    }
                    
                        CommandeEffeList.Add(cl);
                 
                }
               
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        void Handle(CheckBox checkBox)
        {
            // Use IsChecked.
             checkBox.IsChecked=false;

            // Assign Window Title.
            
        }
        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                filtre = 0;
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                PatientDataGrid.ItemsSource = proxy.GetAllCommandebydatecommande(DateSaisieFin.SelectedDate.Value, DateSaisieDébut.SelectedDate.Value);
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(n => n.raison);
                ParDateCommande.IsChecked = true;
                CommandeReceptionList = new List<SVC.Commande>();
            }
            catch(Exception ex)

            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Commande cl = PatientDataGrid.SelectedItem as SVC.Commande;
                    if (CommandeEffeList.Contains(cl) == true)
                    {
                        CommandeEffeList.Remove(cl);
                    }else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Commande déja efféctuée", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.Commande cl = PatientDataGrid.SelectedItem as SVC.Commande;
                    if (CommandeReceptionList.Contains(cl) == true)
                    {
                        CommandeReceptionList.Remove(cl);
                    }
                    if (CommandeEffeList.Contains(cl) == true)
                    {
                        CommandeEffeList.Remove(cl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PatientDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null && memberuser.ModificationCommande==true)
                {
                    SVC.Commande selectedcommande = PatientDataGrid.SelectedItem as SVC.Commande;
                    AjouterCommande cl = new AjouterCommande(proxy, memberuser, callback, selectedcommande,null,null);
                    cl.Show();
                    
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionnez une commande", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReception_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(CommandeReceptionList.Count>0 && memberuser.CreationAchat==true)
                {
                    SVC.Commande selectedcommande = PatientDataGrid.SelectedItem as SVC.Commande;

                    string fournisseur = selectedcommande.Id+" "+ selectedcommande.Fournisseur;
                    this.IsEnabled= false;
                    Reception cl = new Reception(proxy, fournisseur, memberuser, CommandeReceptionList, this);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("La sélection est vide", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

      
      
        private void checkeffe_Checked(object sender, RoutedEventArgs e)
        {
            try
            {


                bool filterValue = Convert.ToBoolean(checkeffe.IsChecked);


                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Commande p = o as SVC.Commande;
                        if (filterValue != null)
                            return (p.Effectuée == filterValue);
                        return (p.Effectuée.Equals(filterValue));
                    };
                }

                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkeffe_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {


                bool filterValue = Convert.ToBoolean(checkeffe.IsChecked);
            

                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Commande p = o as SVC.Commande;
                        if (filterValue != null)
                            return (p.Effectuée == filterValue);
                        return (p.Effectuée.Equals(filterValue));
                    };
                }
                
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkRéceptionée_Checked(object sender, RoutedEventArgs e)
        {
            try
            {


                bool filterValue = Convert.ToBoolean(checkRéceptionée.IsChecked);

                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue == null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Commande p = o as SVC.Commande;
                        if (filterValue != null)
                            return (p.Réceptionée == filterValue);
                        return (p.Réceptionée.Equals(filterValue));
                    };
                }




                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void checkRéceptionée_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool filterValue = Convert.ToBoolean(checkRéceptionée.IsChecked);
               
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filterValue ==null)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Commande p = o as SVC.Commande;
                        if (filterValue != null)
                            return (p.Réceptionée == filterValue);
                        return (p.Réceptionée.Equals(filterValue));
                    };
                }


               
               
                PatientDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtRechercheFournisseur_TextChanged(object sender, TextChangedEventArgs e)
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
                        SVC.Commande p = o as SVC.Commande;
                        if (t.Name == "txtId")
                            return (p.Fournisseur == filter);
                        return (p.Fournisseur.ToUpper().Contains(filter.ToUpper()));
                    };
                }


                
                PatientDataGrid.SelectedItem = null;
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
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Commande p = o as SVC.Commande;
                        if (t.Name == "txtId")
                            return (p.RaisonClient == filter);
                        return (p.RaisonClient.ToUpper().Contains(filter.ToUpper()));
                    };
                }



                PatientDataGrid.SelectedItem = null;

             
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

      
    }
}
