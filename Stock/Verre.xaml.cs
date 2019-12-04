
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Stock
{
    /// <summary>
    /// Interaction logic for Verre.xaml
    /// </summary>
    public partial class Verre : Window
    {
        private delegate void FaultedInvokerVerre();
        SVC.MembershipOptic MembershipOptic;
        SVC.ServiceCliniqueClient proxy;
        List<SVC.TarifVerre> listtarif = new List<SVC.TarifVerre>();
        List<SVC.TarifVerre> listtarifanciennelist = new List<SVC.TarifVerre>();
        List<string> ListDiam;
        SVC.TarifVerre TarifVerre;
        public Verre(SVC.ServiceCliniqueClient proxyrecu, SVC.Verre CatalogueVerre)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
               
                VerreDatagrid.DataContext = CatalogueVerre;
                switch (CatalogueVerre.Matière)
                {
                    case "Minéral":
                        radMat1.IsChecked = true;
                        break;
                    case "Organique":
                        radMat2.IsChecked = true;
                        break;
                    case "Polycarbonate":
                        radMat3.IsChecked = true;
                        break;
                }

                
                CATitem.IsEnabled = true;
                CATitem.Visibility = Visibility.Visible;

                ComboBoxItem item = new ComboBoxItem
                {
                    Content = CatalogueVerre.IndiceVerre,
                };
                CdompteComboBox.Items.Add(item);

                ComboBoxItem item1 = new ComboBoxItem
                {
                    Content = CatalogueVerre.Surface,
                };
                SurfaceComboBox.Items.Add(item1);

                ComboBoxItem item2 = new ComboBoxItem
                {
                    Content = CatalogueVerre.TypeVerre,
                };
                TypeVerreComboBox.Items.Add(item2);
                /************tarif***************/
                bool existe = proxy.GetAllTarifVerre().Any(n => n.cleproduit == CatalogueVerre.cleproduit);
                if (existe == true)
                {
                    listtarif = proxy.GetAllTarifVerrebycode(CatalogueVerre.cleproduit);
                    listtarifanciennelist = proxy.GetAllTarifVerrebycode(CatalogueVerre.cleproduit);
                  
                    string line = CatalogueVerre.Diamètre.Trim();
                    ListDiam = line.Split(',').ToList();
                    DiamComboBox.ItemsSource = ListDiam;
                    DiamComboBox.DataContext = ListDiam;
                    PrixComboBox.IsEnabled = true;
                    DiamComboBox.IsEnabled = true;

                }
                else
                {
                    
                    PrixComboBox.IsEnabled = false;
                    DiamComboBox.IsEnabled = false;
                }
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVerre(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerVerre(HandleProxy));
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

        private void Rtar_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rtar.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Collapsed;
                    GridVerreTarification.Visibility = Visibility.Visible;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                    GridVerre.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void Rtar_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rtar.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Collapsed;
                    GridVerreTarification.Visibility = Visibility.Visible;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                    GridVerre.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rdesc.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Visible;
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Visible;
                    GridVerre.Visibility = Visibility.Collapsed;

                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void Rdesc_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rdesc.IsChecked == true)
                {
                    GridVerre.Visibility = Visibility.Visible;
                    GridVerreTarification.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridVerreTarification.Visibility = Visibility.Visible;
                    GridVerre.Visibility = Visibility.Collapsed;

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PrixComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PrixComboBox.SelectedItem != null && DiamComboBox.SelectedItem != null)
                {
                    GridTarification.IsEnabled = true;
                    ComboBoxItem cbItem = (ComboBoxItem)PrixComboBox.SelectedItem;
                    string prix = cbItem.Content.ToString();
                    var selectedDiamettre = DiamComboBox.SelectedItem as string;
                    if (prix == "Prix de vente")
                    {
                        TarifVerre = listtarif.Find(n => n.PrixVente == true && n.PrixAchat == false && n.Diamètre == selectedDiamettre);
                    }
                    else
                    {
                        if (prix == "Prix d'achat")
                        {
                            TarifVerre = listtarif.Find(n => n.PrixVente == false && n.PrixAchat == true && n.Diamètre == selectedDiamettre);
                        }
                    }


                    GridTarification.DataContext = TarifVerre;
                }
                else
                {
                    GridTarification.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
