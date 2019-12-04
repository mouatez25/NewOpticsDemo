using NewOptics.Administrateur;
using NewOptics.ClientA;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NewOptics.Atelier
{
    /// <summary>
    /// Interaction logic for NewMonture.xaml
    /// </summary>
    public partial class NewMonture : Window
    {
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.MembershipOptic membership;
        int monturelentille;
        
       

        public NewMonture(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrecu,SVC.MembershipOptic memberrecu,int recuint)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                membership = memberrecu;
                monturelentille = recuint;   
                if(recuint==1)
                {
                    fds.Content = "Nouvelle Monture";
                }
                else
                {
                    fds.Content = "Nouvelle Lentille";
                }         
                    ComboClient.ItemsSource = proxy.GetAllClientV().OrderBy(n => n.Raison);
                callbackrecu.InsertClientVCallbackEvent += new ICallback.CallbackEventHandler7(callbackrecu_Refresh);

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }
        void callbackrecu_Refresh(object source, CallbackEventInsertClientV e)
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
        public void AddRefresh(SVC.ClientV listmembership, int oper)
        {
            try
            {
                // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Mise à jours", ShiftPro.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);


                var LISTITEM1 = ComboClient.ItemsSource as IEnumerable<SVC.ClientV>;
                List<SVC.ClientV> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Insert(0, listmembership);
                }
                else
                {
                    if (oper == 2)
                    {
                        //   var objectmodifed = LISTITEM.Find(n => n.Id == listmembership.Id);
                        //  objectmodifed = listmembership;

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
                            //    MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show("Supp rendezvous :"+ listmembership.Id.ToString(), ShiftPro.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            var deleterendez = LISTITEM.Where(n => n.Id == listmembership.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                ComboClient.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ConfirmerDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (monturelentille == 1)
                {
                    SVC.ClientV client = ComboClient.SelectedItem as SVC.ClientV;
                    MontureClient ll = new MontureClient(proxy, callback, membership, client, null);
                    ll.Show();
                    this.Close();
                }
                else
                {
                    if (monturelentille == 2)
                    {
                        SVC.ClientV client = ComboClient.SelectedItem as SVC.ClientV;
                        LentilleClient ll = new LentilleClient(proxy, callback, membership, client, null);
                        ll.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
         }

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             
                    NewClient cl = new NewClient(proxy,membership, null);
                    cl.Show();
                 
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
