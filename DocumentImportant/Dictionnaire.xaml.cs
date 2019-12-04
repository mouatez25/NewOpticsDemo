using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.EtatEtRapport
{
    /// <summary>
    /// Interaction logic for Dictionnaire.xaml
    /// </summary>
    public partial class Dictionnaire : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerDictionnaire();
        ICallback callback;
        SVC.Membership membership;
        int countrasaction;
        public Dictionnaire(SVC.ServiceCliniqueClient proxyrecu,SVC.Membership memberrecu, ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                BookDic.DataSource = proxyrecu.GetAllDictionnaire().OrderBy(n=>n.Mot).ToList();
                 MotifDataGrid.ItemsSource = BookDic.DataSource;
                callback = callbackrecu;
                membership = memberrecu;
                proxy = proxyrecu;
                System.Windows.Forms.Timer timer01tr = new System.Windows.Forms.Timer();
                timer01tr.Interval = 1000;
                timer01tr.Tick += new EventHandler(timerprocTransaction);
                timer01tr.Enabled = true;
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDictionnaire(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void timerprocTransaction(object o1, EventArgs e1)
        {
            countrasaction++;


            if (countrasaction == Convert.ToInt32(10))
            {
                Random rnd = new Random();
                int num = rnd.Next(1, MotifDataGrid.Items.Count);
                BookDic.PageIndex = num;
                countrasaction = 0;
            }
           


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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDictionnaire(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(MotifDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Dictionnaire p = o as SVC.Dictionnaire;
                        if (t.Name == "txtId")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.Mot.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MotifDataGrid.SelectedItem!=null)
                {
                    SVC.Dictionnaire selecteddi = MotifDataGrid.SelectedItem as SVC.Dictionnaire;
                    DétailTerminologie cl = new DétailTerminologie(proxy, callback,selecteddi,membership);
                    cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
