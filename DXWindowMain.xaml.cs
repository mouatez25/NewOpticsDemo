using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using NewOptics.Administrateur;
using NewOptics.Stock;
using System.ServiceModel;
using System.Windows.Threading;
using NewOptics.ClientA;

using NewOptics.SVC;
using NewOptics;
using NewOptics.Achat;
using NewOptics.Statistique;
using NewOptics.CreditCreance;

using NewOptics.Tarif;
using NewOptics.Atelier;

namespace NewOptics
{
    /// <summary>
    /// Interaction logic for DXWindowMain.xaml
    /// </summary>
    public partial class DXWindowMain : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MembershipOptic;
        ICallback callback;
        private delegate void FaultedInvokerMain();
        SVC.Client localClient;
        public event EventHandler AddEvent;
        public Chat CL;
        public bool ChatOpened = false;
        Dictionary<ListBoxItem, SVC.Client> OnlineClients = new Dictionary<ListBoxItem, SVC.Client>();

        public DXWindowMain(SVC.ServiceCliniqueClient PROXYRECU, SVC.MembershipOptic memberrecu, ICallback callbackrecu, SVC.Client localclientrecu)
        {
            try
            {
                InitializeComponent();
                memberrecu.ModuleChat = true;
                memberrecu.EnvoiReceptMessage = true;
                memberrecu.EnvoiRécéptFichier = true;
                memberrecu.DiscussionPrivé = true;
              
                MembershipOptic = memberrecu;
                proxy = PROXYRECU;
                callback = callbackrecu;
                localClient = localclientrecu;
                SESSSIONNAME.Content = localClient.UserName.ToString();
                

                FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback, this, localClient));

                if (localClient.Point == true && localClient.Gestion == true && localClient.Chat == true)
                {
                    this.Title = NewOptics.Properties.Resources.Logiciel + "+ Logiciel de Gestion Pour Opticien";
                }
                else
                {
                    if (localClient.Point == true && localClient.Gestion == false && localClient.Chat == false)
                    {
                        this.Title = NewOptics.Properties.Resources.Logiciel + " Logiciel de Point de vente";

                    }
                    else
                    {
                        if (localClient.Point == true && localClient.Gestion == true && localClient.Chat == false)
                        {
                            this.Title = NewOptics.Properties.Resources.Logiciel + " Logiciel de Gestion Pour Opticien";

                        }
                    }
                }


                if (/*memberrecu.ModuleChat == true &&*/ localClient.Chat == true)
                {
                    CL = new Chat(localclientrecu, callbackrecu, PROXYRECU, this, memberrecu);

                    ChatOpened = true;
                    CL.Show();
                    CL.WindowState = WindowState.Minimized;
                }


                callbackrecu.callback += new ICallback.CallbackEventHandler(callbackrecu_callback);

                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

//                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(localClient.Chat.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_callback(object source, CallbackEvent e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddMessage(e.clients);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        public void AddMessage(List<Client> clients)
        {
            try
            {
                chatListBoxNames.Items.Clear();
                OnlineClients.Clear();
                foreach (SVC.Client c in clients)
                {
                    ListBoxItem item = MakeItem(c.UserName);
                    chatListBoxNames.Items.Add(item);
                    OnlineClients.Add(item, c);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        private ListBoxItem MakeItem(string text)
        {
            ListBoxItem item = new ListBoxItem();


            TextBlock txtblock = new TextBlock();
            txtblock.Text = text;
            txtblock.VerticalAlignment = VerticalAlignment.Center;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(item);
            panel.Children.Add(txtblock);

            ListBoxItem bigItem = new ListBoxItem();
            bigItem.Content = panel;

            return bigItem;
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMain(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerMain(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void HandleProxy()
        {
            try
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
                            this.Close();
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
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {

                    }
                    else
                    {
                        proxy.DisconnectAsync(this.localClient);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }





        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleAms == true)
                {
                    MonStock CLMedecin = new MonStock(proxy, MembershipOptic, callback);
                    CLMedecin.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }


        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.CreationFichier == true)
                {
                    AjouterProduit CLSession = new AjouterProduit(proxy, null, MembershipOptic, callback);
                    CLSession.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void fichef_Click(object sender, RoutedEventArgs e)
        {
            /*  try
              {
                  if (MembershipOptic.CreationFichier == true)
                  {
                      AjouterFournisseur CLSession = new AjouterFournisseur(proxy, null, MembershipOptic);
                      CLSession.Show();

                  }
                  else
                  {
                      MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                  }
              }
              catch (Exception ex)
              {
                  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

              }*/
        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {



                    //  ListeFournisseur CLMedecin = new ListeFournisseur(proxy, MembershipOptic, callback);
                    //CLMedecin.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void Achat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.CreationAchat == true)
                {
                    AjouterFactureAchat CLSession = new AjouterFactureAchat(proxy, MembershipOptic, callback, null, localClient);
                    CLSession.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void listachat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleAchat == true)
                {
                //    MesAchat CLSession = new MesAchat(proxy, MembershipOptic, callback, localClient);
                  //  CLSession.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void AchatVVente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    AchatEtVenteProduit cl = new AchatEtVenteProduit(proxy, callback, MembershipOptic);
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

        private void RéglementPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {

                    CreditClient cl = new CreditClient(proxy, MembershipOptic, callback);
                    cl.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void Rapport3_Click(object sender, RoutedEventArgs e)
        {
          
            //  BACK.Visibility = Visibility.Visible;
            try
            {

              //  Editeur cl = new Editeur(proxy);
                //cl.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            finally
            {
            //    WaitIndicatorS.DeferedVisibility = false;
                // BACK.Visibility = Visibility.Collapsed;
            }
        }
        private void Rapport4_Click(object sender, RoutedEventArgs e)
        {
         //   WaitIndicatorS.DeferedVisibility = true;
            //    BACK.Visibility = Visibility.Visible;
            try
            {

               // FeuilleCalcule cl = new FeuilleCalcule(proxy);
                //cl.Show();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            finally
            {
             //   WaitIndicatorS.DeferedVisibility = false;
                //  BACK.Visibility = Visibility.Collapsed;
            }
        }



        private void dfftes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    PeremptionAlerte cl = new PeremptionAlerte(proxy, callback, MembershipOptic);
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

        private void dffte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    Achatsdesproduits cl = new Achatsdesproduits(proxy, callback, MembershipOptic);
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

        private void MenuItem_Click_17(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {



                    ListeProduit CLMedecin = new ListeProduit(proxy, MembershipOptic, callback);

                    CLMedecin.Show();



                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void MenuItem_Click_18(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {

                    ListeFamilleProduit CLMedecin = new ListeFamilleProduit(proxy, MembershipOptic, callback);

                    CLMedecin.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void Ecrans_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavBarItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {
                    FrameInterieur.Navigate(new ClientA.ListClient(proxy, MembershipOptic, callback, localClient));
                    //   FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback, this, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {
                    FrameInterieur.Navigate(new Fournisseur.ListeFournisseur(proxy, MembershipOptic, callback));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_2(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {
                    FrameInterieur.Navigate(new Article.Articles(proxy, MembershipOptic, callback));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_3(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleCaisse == true)
                {
                    FrameInterieur.Navigate(new Caisse.Caisse(proxy, MembershipOptic, callback, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void venteco_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleVenteCompoirAcces == true)
                {
                    Comptoir.Comptoir cl = new Comptoir.Comptoir(proxy, MembershipOptic, callback);
                    cl.Show();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_4(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleParametreAcces == true)
                {
                    FrameInterieur.Navigate(new Param.Entete(proxy, MembershipOptic, callback, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_5(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleParametreAcces == true)
                { 
                    FrameInterieur.Navigate(new Param.Utilisateurs(proxy, MembershipOptic, callback, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                } 
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_6(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleParametreAcces == true)
                {
                    FrameInterieur.Navigate(new Param.Compteurs(proxy, MembershipOptic, callback, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_7(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    FrameInterieur.Navigate(new Vente.VenteEntete(proxy, MembershipOptic, callback));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_8(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleAchat == true)
                {
                    FrameInterieur.Navigate(new Achat.ListesAchats(proxy, MembershipOptic, callback, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void dfs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    TableauDeBord cl = new TableauDeBord(proxy, callback, MembershipOptic);
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {

                    ListeMarque CLMedecin = new ListeMarque(proxy, MembershipOptic, callback);

                    CLMedecin.Show();

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_9(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {
                    FrameInterieur.Navigate(new Tarif.ListVerre(proxy, MembershipOptic, callback));
                    //   FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback, this, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_10(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {
                    FrameInterieur.Navigate(new Tarif.ListLentille(proxy, MembershipOptic, callback));
                    //   FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback, this, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_11(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleFichier == true)
                {
                    FrameInterieur.Navigate(new Article.Articles(proxy, MembershipOptic, callback));
                    //   FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback, this, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_12(object sender, EventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleCommande == true)
                {
                    FrameInterieur.Navigate(new Commande.Commande(proxy, MembershipOptic, callback));
                    //   FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback, this, localClient));

                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("vous devez avoir une autorisation pour effectuer cette action", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ListRendez_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModulePlanning==true)
                {
                //    ListeRendezVous cl = new ListeRendezVous(proxy, callback, MembershipOptic);
                  //  cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void btnhome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                      FrameInterieur.Navigate(new Aceuill(proxy, MembershipOptic, callback,this, localClient));
         
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_13(object sender, EventArgs e)
        {
            try
            {
                  FrameInterieur.Navigate(new ListSupplément(proxy, MembershipOptic, callback));
             }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void JournalVentes_Click(object sender, RoutedEventArgs e)
        {
            if (MembershipOptic.ModuleStatistiqueAcces == true)
            {
                JournalDesVentes cl = new JournalDesVentes(proxy, MembershipOptic, callback);
                cl.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void etat104_Click(object sender, RoutedEventArgs e)
        {
            if (MembershipOptic.ModuleStatistiqueAcces == true)
            {
                Etat104 cl = new Etat104(proxy, MembershipOptic, callback);
                cl.Show();
            }
            else
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_14(object sender, EventArgs e)
        {
            try
            {
                
                    FrameInterieur.Navigate(new ListeMonture(proxy, MembershipOptic, callback));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void NavBarItem_Click_15(object sender, EventArgs e)
        {
            try
            {

                FrameInterieur.Navigate(new ListeLentilleClient(proxy, MembershipOptic, callback));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
