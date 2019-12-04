using DevExpress.Xpf.Core;
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

namespace GestionClinique.Administrateur
{
    /// <summary>
    /// Interaction logic for AjouterActe.xaml
    /// </summary>
    public partial class AjouterActe : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerInsertActe();
        SVC.Acte alimentobj;
        string title;
        Brush titlebrush;
        bool Trans = false;
        public AjouterActe(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership membershiprecu, ICallback callbackrecu, SVC.Acte alimentrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = membershiprecu;
                callback = callbackrecu;

                title = this.Title;
          //      titlebrush = this.WindowTitleBrush;
                callbackrecu.InsertCatalogueCallbackevent += new ICallback.CallbackEventHandler34(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (alimentrecu == null)
                {
                    Trans = false;
                    btnCreer.IsEnabled = false;
                    alimentobj = new SVC.Acte {Prix=0 };
                    f.Content = "Créer un nouveau acte";
                    txtCatalogue.ItemsSource = proxy.GetAllCatalogue();
                    ActeGrid.DataContext = alimentobj;
                }
                else
                {
                  
                    Trans = true;
                    List<SVC.Catalogue> testmedecin =proxy.GetAllCatalogue(); 
                    txtCatalogue.ItemsSource = testmedecin;
                    List<SVC.Catalogue> tte = testmedecin.Where(n => n.Id == alimentrecu.IdCatalogue).ToList();
                    txtCatalogue.SelectedItem = tte.First();
                    alimentobj = alimentrecu;
                    btnCreer.IsEnabled = true;
                    ActeGrid.DataContext = alimentobj;
                    f.Content = "Modifier acte";

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertCatalogue e)
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(List<SVC.Catalogue> listmembership)
        {
            try
            { 
            if (txtCatalogue.SelectedItem!=null)
            {
                List<SVC.Catalogue> testmedecin = listmembership;
                txtCatalogue.ItemsSource = testmedecin;
                List<SVC.Catalogue> tte = testmedecin.Where(n => n.Id == alimentobj.IdCatalogue).ToList();
                txtCatalogue.SelectedItem = tte.First();
            }else
            {
                txtCatalogue.ItemsSource = listmembership;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerInsertActe(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerInsertActe(HandleProxy));
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
        bool okcréation()
        {
            if (txtAliments.Text != "" && txtCalories.Text != "" && txtCatalogue.SelectedItem != null)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Trans == false && okcréation())
                {

                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        alimentobj.Libelle = txtAliments.Text;
                        SVC.Catalogue selectaliment = txtCatalogue.SelectedItem as SVC.Catalogue;

                        alimentobj.Catalogue = selectaliment.Catalogue1;
                        alimentobj.IdCatalogue= selectaliment.Id;
                        alimentobj.Libelle = txtAliments.Text;

                        if (txtCalories.Text!="")
                        {
                            alimentobj.Prix = Convert.ToDecimal(txtCalories.Text);

                        }else
                        {
                            alimentobj.Prix =0 ;
                        }
           



                        proxy.InsertActe(alimentobj);
                        ts.Complete();


                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    proxy.AjouterActeCatalogueRefresh();

                }
                else
                {
                    if (Trans == true && okcréation())
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            SVC.Catalogue selectaliment = txtCatalogue.SelectedItem as SVC.Catalogue;

                            alimentobj.Catalogue = selectaliment.Catalogue1;
                            alimentobj.IdCatalogue = selectaliment.Id;

                           
                            proxy.UpdateActe(alimentobj);
                            ts.Complete();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        proxy.AjouterActeCatalogueRefresh();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void txtCalories_KeyDown(object sender, KeyEventArgs e)
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

        private void txtAliments_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            if (txtAliments.Text.Trim() != ""&& Trans == false)
            {

                var query = from c in proxy.GetAllActe()
                            select new { c.Libelle };

                var results = query.ToList();
                var disponible = results.Where(list1 => list1.Libelle.Trim().ToUpper() == txtAliments.Text.Trim().ToUpper()).FirstOrDefault();

                if (disponible != null)
                {
                    this.Title = "Cet Acte Existe";
                    //   this.WindowTitleBrush = Brushes.Red;

                    btnCreer.IsEnabled = false;
                    btnCreer.Opacity = 0.2;


                }
                else
                {
                   
                        this.Title = title;
                        //     this.WindowTitleBrush = titlebrush;
                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;

                
                }
            }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
