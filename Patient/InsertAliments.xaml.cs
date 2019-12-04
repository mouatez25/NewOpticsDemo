using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for InsertAliments.xaml
    /// </summary>
    public partial class InsertAliments : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerInsertAliment();
        SVC.Aliment alimentobj;
        string title;
        Brush titlebrush;
        bool Trans = false;
        public InsertAliments(SVC.ServiceCliniqueClient proxyrecu, SVC.Membership membershiprecu, ICallback callbackrecu,SVC.Aliment alimentrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = membershiprecu;
                callback = callbackrecu;

                title = this.Title;
               // titlebrush = this.WindowTitleBrush;
                callbackrecu.InsertFamilleAlimentCallbackevent += new ICallback.CallbackEventHandler29(callbackrecu_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                if (alimentrecu==null)
                {
                    Trans = false;
                    btnCreer.IsEnabled = false;
                    alimentobj = new SVC.Aliment
                    {
                        Kcal=0,
                        Protéines=0,
                        Lipides=0,
                        Glucides=0,
                        ValeurGramme=0,
                        IG=0,
                    };
                    txtFamille.ItemsSource = proxy.GetAllFamilleAliment();
                    AlimentGrid.DataContext = alimentobj;
                }
                else
                {
                    alimentobj = alimentrecu;
                    Trans = true;
                    List<SVC.FamilleAliment> testmedecin = proxy.GetAllFamilleAliment();
                    txtFamille.ItemsSource = testmedecin;
                    List<SVC.FamilleAliment> tte = testmedecin.Where(n => n.Id == alimentobj.IdFamilleAliment).ToList();
                    txtFamille.SelectedItem = tte.First();

                  
                    AlimentGrid.DataContext = alimentobj;

                }

            }
            catch
            {
                HandleProxy();
            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertFamilleAliment e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav);
            }));
        }
        public void AddRefresh(List<SVC.FamilleAliment> listmembership)
        {

            txtFamille.ItemsSource = listmembership;
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerInsertAliment(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerInsertAliment(HandleProxy));
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
            if (txtAliments.Text!="" && txtCalories.Text!="" && txtProtéines.Text!="" && txtLipides.Text!="" && txtGlucides.Text!="" && txtIG.Text!="" && txtValeurGramme.Text!="" && txtFamille.SelectedItem!=null)
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
                        alimentobj.DesignProduit = txtAliments.Text;
                        SVC.FamilleAliment selectaliment = txtFamille.SelectedItem as SVC.FamilleAliment;

                        alimentobj.FamilleAlimentDesign = selectaliment.FamilleAlimentDesign;
                        alimentobj.IdFamilleAliment = selectaliment.Id;


                        if (alimentobj.IG <= 50)
                        {
                            alimentobj.IgBas = true;
                            alimentobj.IgMoy = false;
                            alimentobj.IgElv = false;
                            alimentobj.IgNotDef= false;

                        }
                        else
                        {
                            if (alimentobj.IG <= 70 && alimentobj.IG >= 51)
                            {

                                alimentobj.IgBas = false;
                                alimentobj.IgMoy = true;
                                alimentobj.IgElv = false;
                                alimentobj.IgNotDef = false;
                            }
                            else
                            {
                                if (alimentobj.IG >= 71 && alimentobj.IG <= 100)
                                {
                                    alimentobj.IgBas = false;
                                    alimentobj.IgMoy = false;
                                    alimentobj.IgElv = true;
                                    alimentobj.IgNotDef = false;

                                }
                            }
                        }
                        
                        proxy.InsertAlimentAsync(alimentobj);
                        ts.Complete();
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                else
                {
                    if (Trans == true )
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            SVC.FamilleAliment selectaliment = txtFamille.SelectedItem as SVC.FamilleAliment;

                            alimentobj.FamilleAlimentDesign = selectaliment.FamilleAlimentDesign;
                            alimentobj.IdFamilleAliment = selectaliment.Id;

                            if (alimentobj.IG <= 50)
                            {
                                alimentobj.IgBas = true;
                                alimentobj.IgMoy = false;
                                alimentobj.IgElv = false;
                                alimentobj.IgNotDef = false;

                            }
                            else
                            {
                                if (alimentobj.IG <= 70 && alimentobj.IG >= 51)
                                {

                                    alimentobj.IgBas = false;
                                    alimentobj.IgMoy = true;
                                    alimentobj.IgElv = false;
                                    alimentobj.IgNotDef = false;
                                }
                                else
                                {
                                    if (alimentobj.IG >= 71 && alimentobj.IG <= 100)
                                    {
                                        alimentobj.IgBas = false;
                                        alimentobj.IgMoy = false;
                                        alimentobj.IgElv = true;
                                        alimentobj.IgNotDef = false;

                                    }
                                }
                            }
                            proxy.UpdateAlimentAsync(alimentobj);
                            ts.Complete();
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                    }
                }
            }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtAliments_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtAliments.Text.Trim() != ""&& Trans==false)
                {

                    var query = from c in proxy.GetAllAliment()
                                select new { c.DesignProduit };

                    var results = query.ToList();
                    var disponible = results.Where(list1 => list1.DesignProduit.Trim().ToUpper() == txtAliments.Text.Trim().ToUpper()).FirstOrDefault();

                    if (disponible != null)
                    {
                        this.Title = "cet aliment Existe";
                    //    this.WindowTitleBrush = Brushes.Red;

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;


                    }
                    else
                    {
                        if (txtAliments.Text.Trim() != "")
                        {
                            this.Title = title;
                           // this.WindowTitleBrush = titlebrush;
                            btnCreer.IsEnabled = true;
                            btnCreer.Opacity = 1;

                        }
                    }
                }
                else
                {
                    if (txtAliments.Text.Trim() != "" && Trans == true)
                    {
                        btnCreer.IsEnabled = true;
                        btnCreer.Opacity = 1;
                    }
                    else
                    {

                        btnCreer.IsEnabled = false;
                        btnCreer.Opacity = 0.2;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
          }

   

        private void txtValeurGramme_KeyDown(object sender, KeyEventArgs e)
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

        private void txtIG_KeyDown(object sender, KeyEventArgs e)
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

        private void txtGlucides_KeyDown(object sender, KeyEventArgs e)
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

        private void txtLipides_KeyDown(object sender, KeyEventArgs e)
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

        private void txtProtéines_KeyDown(object sender, KeyEventArgs e)
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

       

        private void txtIG_GotFocus(object sender, RoutedEventArgs e)
        {
            labelremarque.Visibility = Visibility.Visible;
        }
    }
}
