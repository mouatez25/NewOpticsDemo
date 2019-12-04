
using NewOptics.Administrateur;
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

namespace NewOptics.Comptoir
{
    /// <summary>
    /// Interaction logic for DetailVente.xaml
    /// </summary>
    public partial class DetailVente : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.Facture facturedevente;
        SVC.Facture ancienfacture;
        SVC.Prodf produitreste;
        bool avoir = false;
        int interfaceint = 0;
        DataGrid ReceptDatagrid;
        Label digitalGaugeControl1;
        TextBlock nbreproduit;
        public DetailVente(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu,SVC.Facture facfturerecu,SVC.Param selectedparam,SVC.Prodf selectedprodf,int interfacerecu,DataGrid receptdatrecu, Label digitrecu, TextBlock txtrecu )
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                MemberUser = MembershipOpticrecu;
                callback = callbackrecu;
                interfaceint = interfacerecu;
                ReceptDatagrid = receptdatrecu;
                digitalGaugeControl1 = digitrecu;
                nbreproduit = txtrecu;
                if (interfaceint == 0)
                {
                    ancienfacture = new SVC.Facture
                    {
                        cab = facfturerecu.cab,
                        cf = facfturerecu.cf,
                        date = facfturerecu.date,
                        datef = facfturerecu.datef,
                        design = facfturerecu.design,
                        cle = facfturerecu.cle,
                        cmp = facfturerecu.cmp,
                        codeprod = facfturerecu.codeprod,

                        ff = facfturerecu.ff,
                        ficheproduit = facfturerecu.ficheproduit,
                        Id = facfturerecu.Id,
                        lot = facfturerecu.lot,
                        nfact = facfturerecu.nfact,
                        serialnumber = facfturerecu.serialnumber,
                        oper = facfturerecu.oper,
                        pack = facfturerecu.pack,
                        perempt = facfturerecu.perempt,
                        previent = facfturerecu.previent,
                        privente = facfturerecu.privente,
                        quantite = facfturerecu.quantite,
                        remise = facfturerecu.remise,
                        Total = facfturerecu.Total,
                        tva = facfturerecu.tva,

                    };


                    if (selectedparam.ModiPrix == true)
                    {
                        txtPrix.IsEnabled = true;
                    }
                    else
                    {
                        txtPrix.IsEnabled = false;
                    }

                }
                else
                {
                    if (interfaceint == 1)
                    {
                        ancienfacture = new SVC.Facture
                        {
                            cab = facfturerecu.cab,
                            cf = facfturerecu.cf,
                            date = facfturerecu.date,
                            datef = facfturerecu.datef,
                            design = facfturerecu.design,
                            cle = facfturerecu.cle,
                            cmp = facfturerecu.cmp,
                            codeprod = facfturerecu.codeprod,

                            ff = facfturerecu.ff,
                            ficheproduit = facfturerecu.ficheproduit,
                            Id = facfturerecu.Id,
                            lot = facfturerecu.lot,
                            nfact = facfturerecu.nfact,
                            serialnumber = facfturerecu.serialnumber,
                            oper = facfturerecu.oper,
                            pack = facfturerecu.pack,
                            perempt = facfturerecu.perempt,
                            previent = facfturerecu.previent,
                            privente = facfturerecu.privente,
                            quantite = facfturerecu.quantite,
                            remise = facfturerecu.remise,
                            Total = facfturerecu.Total,
                            tva = facfturerecu.tva,

                        };


                        
                            txtPrix.IsEnabled = true;
                 
                    }
                }
                facturedevente = facfturerecu;
                gridvente.DataContext = facturedevente;




             if (selectedprodf!=null)
                {
                    avoir = false;
                    produitreste = selectedprodf;
                }
                else
                {
                    avoir = true;
                } 

            }catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void calcule()
        {
            var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
            digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
            nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
            foreach (var item in test)
            {
                item.Total = item.privente * item.quantite;
            }
            ReceptDatagrid.ItemsSource = test;
        }
        private void txtQuantité_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Subtract:

                    break;
                case Key.F12:
                    this.Close();
                    break;
                case Key.Escape:
                    facturedevente.quantite = ancienfacture.quantite;
                    facturedevente.privente = ancienfacture.privente;
                    this.Close();
                    break;





                default:
                    e.Handled = true;
                    break;
            }
        }

       

        private void CONFIRMERVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                calcule();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void DXWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.F12:
                        calcule();
                        this.Close();
                        break;
                    case Key.Escape:
                        facturedevente.quantite = ancienfacture.quantite;
                        facturedevente.privente = ancienfacture.privente;
                        this.Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
       
       

        private void DXWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           /* try
            {
                facturedevente.quantite = ancienfacture.quantite;
                facturedevente.privente = ancienfacture.privente;
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }*/
        }

       

        private void annulerVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                facturedevente.quantite = ancienfacture.quantite;
                facturedevente.privente = ancienfacture.privente;
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void txtPrix_KeyDown(object sender, KeyEventArgs e)
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
                case Key.F12:
                    this.Close();
                    break;
                case Key.Escape:
                    facturedevente.quantite = ancienfacture.quantite;
                    facturedevente.privente = ancienfacture.privente;
                    this.Close();
                    break;





                default:
                    e.Handled = true;
                    break;
            }
        }

      

        private void txtQuantité_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (interfaceint == 0)
                {
                    if (txtQuantité.Text != "" && avoir == false)
                    {
                        if (Convert.ToDecimal(txtQuantité.Text) != 0)
                        {
                            if (produitreste.prixa < Convert.ToDecimal(txtQuantité.Text))
                            {
                                txtQuantité.Text = ancienfacture.quantite.ToString();
                            }
                            if (txtPrix.Text != "")
                            {
                                if (Convert.ToDecimal(txtPrix.Text) != 0)
                                {
                                    facturedevente.Total = Convert.ToDecimal(txtQuantité.Text) * Convert.ToDecimal(txtPrix.Text);
                                }
                            }
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtQuantité.Text) && avoir == true)
                        {
                            if (txtQuantité.Text.Count() > 1)
                            {
                                if (Convert.ToDecimal(txtQuantité.Text) != 0)
                                {
                                    if (Convert.ToDecimal(txtQuantité.Text) < 0)
                                    {
                                        if (!string.IsNullOrEmpty(txtPrix.Text))
                                        {
                                            if (Convert.ToDecimal(txtPrix.Text) != 0)
                                            {
                                                facturedevente.Total = (Convert.ToDecimal(txtQuantité.Text)) * Convert.ToDecimal(txtPrix.Text);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        txtQuantité.Text = "";
                                    }
                                }
                            }
                            else
                            {
                                if (txtQuantité.Text.Count() == 1)
                                {
                                    var letter = txtQuantité.Text.Substring(0, 1);
                                    if (letter != "-")
                                    {
                                        txtQuantité.Text = "";
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Convert.ToDecimal(txtQuantité.Text) != 0)
                    {


                        if (txtPrix.Text != "")
                        {
                            if (Convert.ToDecimal(txtPrix.Text) != 0)
                            {
                                facturedevente.Total = Convert.ToDecimal(txtQuantité.Text) * Convert.ToDecimal(txtPrix.Text);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void txtPrix_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (interfaceint == 0)
                {
                    if (txtPrix.Text != "" && txtQuantité.Text != "" && avoir == false)
                    {
                        if (Convert.ToDecimal(txtPrix.Text) != 0 && Convert.ToDecimal(txtQuantité.Text) != 0)
                        {
                            facturedevente.Total = Convert.ToDecimal(txtQuantité.Text) * Convert.ToDecimal(txtPrix.Text);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtPrix.Text) && !string.IsNullOrEmpty(txtQuantité.Text) && avoir == true)
                        {
                            //var letter = txtQuantité.Text.Substring(0, 1);
                            if (txtQuantité.Text.Count() > 1)
                            {
                                if (Convert.ToDecimal(txtQuantité.Text) != 0)
                                {
                                    if (Convert.ToDecimal(txtPrix.Text) != 0 && Convert.ToDecimal(txtQuantité.Text) != 0)
                                    {
                                        facturedevente.Total = (Convert.ToDecimal(txtQuantité.Text)) * Convert.ToDecimal(txtPrix.Text);

                                    }
                                }
                                else
                                {
                                    txtQuantité.Text = "";
                                }
                            }
                            else
                            {
                                if (txtQuantité.Text.Count() == 1)
                                {
                                    var letter = txtQuantité.Text.Substring(0, 1);
                                    if (letter != "-")
                                    {
                                        txtQuantité.Text = "";
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Convert.ToDecimal(txtPrix.Text) != 0 && Convert.ToDecimal(txtQuantité.Text) != 0)
                    {
                        facturedevente.Total = Convert.ToDecimal(txtQuantité.Text) * Convert.ToDecimal(txtPrix.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
