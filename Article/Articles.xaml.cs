
using NewOptics.Administrateur;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Article
{
    /// <summary>
    /// Interaction logic for Articles.xaml
    /// </summary>
    public partial class Articles : Page
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerStock();
        bool filtre = false;
        public Articles(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {
             
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
              
                PatientDataGrid.ItemsSource = proxy.GetAllProdf();
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
             


                txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf()).AsEnumerable().Sum(o => o.quantite * o.previent)));
                callbackrecu.InsertProdfCallbackEvent += new ICallback.CallbackEventHandler21(callbackrecu_Refresh);
                callbackrecu.InsertReceptProdfCallbackevent += new ICallback.CallbackEventHandler50(callbackrecuProdfRecept_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertProdf e)
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

         
        void callbackrecuProdfRecept_Refresh(object source, CallbackEventInsertProdfRecept e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshReceptProdf(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshReceptProdf(List<SVC.Prodf> listMembershipOptic)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Prodf>;
                List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();
                List<SVC.Prodf> ancienliste = LISTITEM.Where(n => n.nfact == listMembershipOptic.First().nfact).ToList();


                foreach (SVC.Prodf item in listMembershipOptic)
                {
                    var objectmodifed = LISTITEM.Find(n => n.Id == item.Id);

                    var index = LISTITEM.IndexOf(objectmodifed);
                    if (index != -1)
                        LISTITEM[index] = item;
                    if (LISTITEM.Contains(item) != true)
                    {
                        LISTITEM.Add(item);
                    }

                }
                foreach (SVC.Prodf item in ancienliste)
                {
                    if (listMembershipOptic.Contains(item) != true)
                    {
                        LISTITEM.Remove(item);
                    }
                }








                PatientDataGrid.ItemsSource = LISTITEM;
                txtFournisseur.Text = Convert.ToString(((LISTITEM).AsEnumerable().Sum(o => o.quantite * o.previent)));

















                //   PatientDataGrid.ItemsSource = listMembershipOptic;
                //  txtFournisseur.Text = Convert.ToString(((listMembershipOptic).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
       
        public void AddRefresh(SVC.Prodf listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.Prodf>;
                List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();

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
                txtFournisseur.Text = Convert.ToString(((LISTITEM).AsEnumerable().Sum(o => o.quantite * o.previent)));
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerStock(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerStock(HandleProxy));
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
                    
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        
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
         
       
       
        private void FournisseurCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
            
                PatientDataGrid.ItemsSource = proxy.GetAllProdf();
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
                chstockdisponible.IsChecked = false;
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf()).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void btnImprimerStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (MemberUser.ImpressionFichier == true)
                {
                    List<SVC.Prodf> itemsSource0 = PatientDataGrid.Items.OfType<SVC.Prodf>().ToList();

                    if ((itemsSource0.Count >= 1))
                    {

                       
                       

                        ImpressionStock clsho = new ImpressionStock(proxy, itemsSource0.ToList());
                        clsho.Show();
                   }
                  /* else
                    {
                        if (PatientDataGrid.VisibleRowCount ==1)
                        {
                            List<SVC.Prodf> itemsSource0 = new List<SVC.Prodf>();

                          
                              int rowHandle = PatientDataGrid.GetRowHandleByListIndex(1);
                                SVC.Prodf p = PatientDataGrid.GetRow(0) as SVC.Prodf;
                                itemsSource0.Add(p);
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(rowHandle.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);



                            ImpressionStock clsho = new ImpressionStock(proxy, itemsSource0.ToList());
                            clsho.Show();
                        }
                    }*/


                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void PatientDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.ItemsSource = proxy.GetAllProdf();
                FournisseurCombo.ItemsSource = proxy.GetAllFourn().OrderBy(x => x.raison);
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
                chstockdisponible.IsChecked = false;
                txtFournisseur.Text = Convert.ToString(((proxy.GetAllProdf()).AsEnumerable().Sum(o => o.quantite * o.previent)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
       
        



        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int disponible = 0;
                if(chstockdisponible.IsChecked==true)
                {
                    disponible = 1;
                }
                else
                {
                    disponible = 0;
                }




                if (FamilleCombo.SelectedIndex >= 0)
                {
                    var Value = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                    if (FournisseurCombo.SelectedItem == null)
                    {

                        PatientDataGrid.ItemsSource = (proxy.GetAllProdf()).Where(n => n.IdFamille == Value.Id && n.quantite >= disponible);


                    }
                    else
                    {
                        SVC.Fourn ValueFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                        PatientDataGrid.ItemsSource = (proxy.GetAllProdf()).Where(n => n.IdFamille == Value.Id && n.quantite >= disponible && n.cf == ValueFourn.Id);
                    }

                }
                else
                {
                    if (FournisseurCombo.SelectedItem == null)
                    {

                        PatientDataGrid.ItemsSource = (proxy.GetAllProdf()).Where(n => n.quantite >= disponible);


                    }
                    else
                    {
                        SVC.Fourn ValueFourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                        PatientDataGrid.ItemsSource = (proxy.GetAllProdf()).Where(n => n.quantite >= disponible && n.cf == ValueFourn.Id);
                    }
                }

                          
                  
                    var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Prodf>;
                   
                    txtFournisseur.Text = Convert.ToString(((test).AsEnumerable().Sum(o => o.quantite*o.privente)));

                
            }
            catch(Exception ex)
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
                        SVC.Prodf p = o as SVC.Prodf;
                        if (t.Name == "txtId")
                            return (p.design == filter);
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
                    };
                }



                
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
