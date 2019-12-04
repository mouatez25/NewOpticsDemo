
using NewOptics.Administrateur;
using NewOptics.ClientA;
using NewOptics.Stock;
using NewOptics.SVC;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewOptics
{
    /// <summary>
    /// Interaction logic for Aceuill.xaml
    /// </summary>
    public partial class Aceuill : Page
    {
        NewOptics.SVC.ServiceCliniqueClient proxy;
        NewOptics.SVC.MembershipOptic memberuser;
        ICallback callback;
        private delegate void FaultedInvokerAceuill();
        DXWindowMain windowmain;
        NewOptics.SVC.Client localclient;
        public Aceuill(NewOptics.SVC.ServiceCliniqueClient proxyrecu, NewOptics.SVC.MembershipOptic memberrecu, ICallback callbackrecu, DXWindowMain windowrecu, NewOptics.SVC.Client localclientrecu)
        {
            try
            {

                InitializeComponent();
                proxy = proxyrecu;

                memberuser = memberrecu;
                callback = callbackrecu;
                windowmain = windowrecu;
                localclient = localclientrecu;
             
            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtCommande_GotFocus(object sender, RoutedEventArgs e)
        {
         
            try
            {
                txtCommande.Text = "";
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
       
    }

        private void txtCommande_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                ((TextBox)sender).Text = "0000000000000";

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
         

        private void txtCommande_TextChanged(object sender, TextChangedEventArgs e)
        {
          /*  try
            {
                if (txtCommande.Text.Count() == txtCommande.MaxLength)
                //   if (!string.IsNullOrEmpty(txtCommande.Text) && txtCommande.Text!= "0000000000000")
                {
                    var ll = proxy.GetAllMonturebycodebar(txtCommande.Text).Any();
                    if (ll == true)
                    {
                        SVC.Monture selectedtropuvé = proxy.GetAllMonturebycodebar(txtCommande.Text).Last();

                        if (selectedtropuvé != null)
                        {
                            SVC.ClientV CLIENT = proxy.GetAllClientVBYID(Convert.ToInt16(selectedtropuvé.IdClient)).First();
                            // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(selectedtropuvé.NCommande.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            txtCommande.Text = "";
                            txtCommande.Focus();
                            MontureClient cl = new MontureClient(proxy, callback, memberuser, CLIENT, selectedtropuvé);
                            cl.Show();
                        }
                        else
                        {
                            // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            txtCommande.Text = "";
                            txtCommande.Focus();

                        }
                    }
                    else
                    {
                        //MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        txtCommande.Text = "";
                        txtCommande.Focus();
                    }
                }
             
                    


            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }*/
        }

        private void TxtLentille_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtLentille.Text = "";
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void TxtLentille_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                ((TextBox)sender).Text = "0000000000000";

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

    
        private void TxtLentille_TextChanged(object sender, TextChangedEventArgs e)
        {
         /*   try
            {
                if (TxtLentille.Text.Count() == TxtLentille.MaxLength)
                {
                    var ll = proxy.GetAllLentilleClientbycodebar(TxtLentille.Text).Any();
                    if (ll == true)
                    {
                        SVC.LentilleClient selectedtropuvé = proxy.GetAllLentilleClientbycodebar(TxtLentille.Text).Last();

                        if (selectedtropuvé != null)
                        {
                            SVC.ClientV CLIENT = proxy.GetAllClientVBYID(Convert.ToInt16(selectedtropuvé.IdClient)).First();
                            // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(selectedtropuvé.NCommande.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            TxtLentille.Text = "";
                            TxtLentille.Focus();
                            ClientA.LentilleClient cl = new ClientA.LentilleClient(proxy, callback, memberuser, CLIENT, selectedtropuvé);
                            cl.Show();
                        }
                        else
                        {
                            //  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                            TxtLentille.Text = "";
                            TxtLentille.Focus();

                        }
                    }
                    else
                    {
                        // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                        TxtLentille.Text = "";
                        TxtLentille.Focus();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }*/
        }

        private void txtCommande_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Enter:
                       // if (txtCommande.Text.Count() == txtCommande.MaxLength)
                        //   if (!string.IsNullOrEmpty(txtCommande.Text) && txtCommande.Text!= "0000000000000")
                       // {
                            var ll = proxy.GetAllMonturebycodebar(txtCommande.Text).Any();
                            if (ll == true)
                            {
                                SVC.Monture selectedtropuvé = proxy.GetAllMonturebycodebar(txtCommande.Text).Last();

                                if (selectedtropuvé != null)
                                {
                                    SVC.ClientV CLIENT = proxy.GetAllClientVBYID(Convert.ToInt16(selectedtropuvé.IdClient)).First();
                                    // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(selectedtropuvé.NCommande.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    txtCommande.Text = "";
                                    txtCommande.Focus();
                                    MontureClient cl = new MontureClient(proxy, callback, memberuser, CLIENT, selectedtropuvé);
                                    cl.Show();
                                }
                                else
                                {
                                    // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    txtCommande.Text = "";
                                    txtCommande.Focus();

                                }
                            }
                            else
                            {
                                //MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                txtCommande.Text = "";
                                txtCommande.Focus();
                            }
                       // }
                        break;
                }


                }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void TxtLentille_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Enter:
                     //   if (TxtLentille.Text.Count() == TxtLentille.MaxLength)
                       // {
                            var ll = proxy.GetAllLentilleClientbycodebar(TxtLentille.Text).Any();
                            if (ll == true)
                            {
                                SVC.LentilleClient selectedtropuvé = proxy.GetAllLentilleClientbycodebar(TxtLentille.Text).Last();

                                if (selectedtropuvé != null)
                                {
                                    SVC.ClientV CLIENT = proxy.GetAllClientVBYID(Convert.ToInt16(selectedtropuvé.IdClient)).First();
                                    // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(selectedtropuvé.NCommande.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    TxtLentille.Text = "";
                                    TxtLentille.Focus();
                                    ClientA.LentilleClient cl = new ClientA.LentilleClient(proxy, callback, memberuser, CLIENT, selectedtropuvé);
                                    cl.Show();
                                }
                                else
                                {
                                    //  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    TxtLentille.Text = "";
                                    TxtLentille.Focus();

                                }
                            }
                            else
                            {
                                // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("N° Commande n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                TxtLentille.Text = "";
                                TxtLentille.Focus();

                            }
                        //}
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
