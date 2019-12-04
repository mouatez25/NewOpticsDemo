
using Microsoft.Reporting.WinForms;
using NewOptics.Administrateur;
using NewOptics.Caisse;
using NewOptics.Commande;
using NewOptics.Stock;
using Nut;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Comptoir
{
    /// <summary>
    /// Interaction logic for Comptoir.xaml
    /// </summary>
    public partial class Comptoir : Window
    {
        SVC.MembershipOptic MemberUser;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        private delegate void FaultedInvokerComptoir();
        public List<SVC.Facture> listcomptoir;
        SVC.Param selectedparam;
        private bool m_IsReadOnly;
        public event PropertyChangedEventHandler PropertyChanged;
        List<SVC.Prodf> produitavendre = new List<SVC.Prodf>();
        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        public Comptoir(SVC.ServiceCliniqueClient proxyrecu, SVC.MembershipOptic MembershipOpticrecu, ICallback callbackrecu)
        {

            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                MemberUser = MembershipOpticrecu;
                listcomptoir = new List<SVC.Facture>();
                ReceptDatagrid.ItemsSource = listcomptoir;
                digitalGaugeControl1.Content = String.Format("{0:0.##}", ((listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)));
                nbreproduit.Text = Convert.ToString((listcomptoir).AsEnumerable().Count());
                // (proxy.GetAllProdf().ToList().ForEach(n => n.privente = n.prixa));
                NomenclatureProduit.ItemsSource = (proxy.GetAllProdf().OrderBy(n => n.design));
                FamilleCombo.ItemsSource = proxy.GetAllFamilleProduit().OrderBy(x => x.FamilleProduit1);
                Memberusertext.Text = MemberUser.Username;
                CompteComboBox.SelectedIndex = 0;
                txtRecherche.Focus();
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateTime.Now, DateTime.Now).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B");
                txtDebit.Text = Convert.ToString((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B").AsEnumerable().Sum(o => o.net));
                TxtCreebit.Text = Convert.ToString((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B").AsEnumerable().Sum(o => o.versement));
                txtSolde.Text = Convert.ToString(((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B").AsEnumerable().Sum(o => o.reste)));
                selectedparam = proxy.GetAllParamétre();

                if (selectedparam.ModifTarif == true)
                {
                    CompteComboBox.IsEnabled = true;
                }
                else
                {
                    CompteComboBox.IsEnabled = false;
                }

                if (selectedparam.AffichPrixAchatVente == true)
                {
                    NomenclatureProduit.Columns[3].Visibility = Visibility.Visible;
                }
                else
                {
                    NomenclatureProduit.Columns[3].Visibility = Visibility.Collapsed;
                }

                /* if (selectedparam.ModiPrix == true)
                 {
                     ReceptDatagrid.Columns[2].IsReadOnly = false;
                 }
                 else
                 {
                     ReceptDatagrid.Columns[2].IsReadOnly = true;
                 }*/
                callbackrecu.InsertProdfListCallbackevent += new ICallback.CallbackEventHandler10(callbackrecu_Refresh);
                callbackrecu.InsertProdfCallbackEvent += new ICallback.CallbackEventHandler21(callbackrecuprodf_Refresh);

                callbackrecu.InsertReceptProdfCallbackevent += new ICallback.CallbackEventHandler50(callbackrecuProdfRecept_Refresh);
                callbackrecu.InsertFamilleProduitCallbackevent += new ICallback.CallbackEventHandler58(callbackDci_Refresh);

                callbackrecu.InsertF1CallbackEvent += new ICallback.CallbackEventHandler6(callbackrecuf1_Refresh);


                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }
        }
        public bool IsReadOnly
        {
            get { return m_IsReadOnly; }
            set
            {
                m_IsReadOnly = value;
                OnPropertyChanged("IsReadOnly");
            }
        }
        void callbackrecuf1_Refresh(object source, CallbackEventInsertF1 e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav, e.operleav);
            }));
        }
        public void AddRefresh(SVC.F1 listmembership, int oper)
        {
            try
            {
                if (listmembership.date >= DateTime.Now.Date && listmembership.date <= DateTime.Now.Date && (listmembership.nfact.Substring(0, 2) == "Co" || listmembership.nfact.Substring(0, 1) == "B"))
                {
                    var LISTITEM1 = PatientDataGrid.ItemsSource as IEnumerable<SVC.F1>;
                    List<SVC.F1> LISTITEM = LISTITEM1.ToList();

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

                    var test = PatientDataGrid.ItemsSource as IEnumerable<SVC.Depense>;
                    txtDebit.Text = Convert.ToString((LISTITEM).AsEnumerable().Sum(o => o.net));
                    TxtCreebit.Text = Convert.ToString((LISTITEM).AsEnumerable().Sum(o => o.versement));
                    txtSolde.Text = Convert.ToString((LISTITEM).AsEnumerable().Sum(o => o.reste));
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }
        void callbackrecuprodf_Refresh(object source, CallbackEventInsertProdf e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshProdf(e.clientleav, e.operleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshProdf(SVC.Prodf listMembershipOptic, int oper)
        {
            try
            {
                var LISTITEM1 = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
                List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();

                if (oper == 1)
                {
                    LISTITEM.Add(listMembershipOptic);
                }
                else
                {
                    if (oper == 2)
                    {

                        var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                        var index = LISTITEM.IndexOf(objectmodifed);
                        if (index != -1)
                            LISTITEM[index] = listMembershipOptic;
                    }
                    else
                    {
                        if (oper == 3)
                        {
                            var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                            LISTITEM.Remove(deleterendez);
                        }
                    }
                }

                NomenclatureProduit.ItemsSource = LISTITEM;
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }


        void callbackDci_Refresh(object source, CallbackEventInsertFamilleProduit e)
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
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void AddRefresh(List<SVC.FamilleProduit> listMembershipOptic)
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



                        FamilleCombo.ItemsSource = listMembershipOptic;


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }


        void callbackrecu_Refresh(object source, CallbackEventInsertListProdf e)
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
        public void AddRefresh(List<SVC.Prodf> listMembershipOpticlist, int oper)
        {
            try
            {
                var LISTITEM1 = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;


                List<SVC.Prodf> LISTITEM = LISTITEM1.ToList();

                foreach (var listMembershipOptic in listMembershipOpticlist)
                {

                    if (oper == 1)
                    {
                        LISTITEM.Add(listMembershipOptic);
                    }
                    else
                    {
                        if (oper == 2 || oper == 4)
                        {



                            var objectmodifed = LISTITEM.Find(n => n.Id == listMembershipOptic.Id);
                            var index = LISTITEM.IndexOf(objectmodifed);
                            if (index != -1)
                                LISTITEM[index] = listMembershipOptic;
                        }
                        else
                        {
                            if (oper == 3)
                            {
                                var deleterendez = LISTITEM.Where(n => n.Id == listMembershipOptic.Id).First();
                                LISTITEM.Remove(deleterendez);
                            }
                        }
                    }

                    NomenclatureProduit.ItemsSource = LISTITEM;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

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
                var LISTITEM1 = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
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








                NomenclatureProduit.ItemsSource = LISTITEM;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerComptoir(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerComptoir(HandleProxy));
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
        private void chcout_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcout.IsChecked == true)
                {

                    var existe = (proxy.GetAllProdf().Any(n => n.quantite != 0));
                    if (existe == true)
                    {
                        List<SVC.Prodf> listprodfcoutmoyen = new List<SVC.Prodf>();

                        var listeCPcode = (from ta in proxy.GetAllProdf()

                                           where ta.quantite != 0
                                           select ta.cp).Distinct();
                        foreach (int cp in listeCPcode)
                        {
                            SVC.Prodf pp = new SVC.Prodf
                            {
                                cp = cp,
                                quantite = 0,
                                previent = 0,
                                prixa = 0,
                                prixb = 0,
                                prixc = 0,

                            };
                            listprodfcoutmoyen.Add(pp);
                        }
                        List<SVC.Prodf> listprodf = (proxy.GetAllProdf().Where(n => n.quantite != 0).ToList());

                        foreach (var existeinfirst in listprodfcoutmoyen)
                        {

                            string ValueCompte = "";
                            if (CompteComboBox.SelectedIndex >= 0)
                            {

                                ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                                foreach (SVC.Prodf itemprodf in listprodf.Reverse<SVC.Prodf>())
                                {


                                    if (existeinfirst.cp == itemprodf.cp)
                                    {
                                        /*    existeinfirst.quantite = listprodf.AsEnumerable().Sum(n=>n.quantite);
                                            existeinfirst.previent = (listprodf.AsEnumerable().Sum(n => n.previent)) / existeinfirst.quantite;
                                            existeinfirst.privente = (listprodf.AsEnumerable().Sum(n => n.privente)) / existeinfirst.quantite;
                                        existeinfirst.prixa = (listprodf.AsEnumerable().Sum(n => n.prixa)) / existeinfirst.quantite;
                                        existeinfirst.prixb = (listprodf.AsEnumerable().Sum(n => n.prixb)) / existeinfirst.quantite;
                                        existeinfirst.prixc = (listprodf.AsEnumerable().Sum(n => n.prixc)) / existeinfirst.quantite;
                                        */

                                        existeinfirst.design = itemprodf.design;


                                        existeinfirst.design = itemprodf.design;
                                        existeinfirst.previent = ((existeinfirst.previent * existeinfirst.quantite) + (itemprodf.previent * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);

                                        existeinfirst.prixa = ((existeinfirst.prixa * existeinfirst.quantite) + (itemprodf.prixa * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);
                                        existeinfirst.prixb = ((existeinfirst.prixb * existeinfirst.quantite) + (itemprodf.prixb * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);
                                        existeinfirst.prixc = ((existeinfirst.prixc * existeinfirst.quantite) + (itemprodf.prixc * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);
                                        existeinfirst.quantite = existeinfirst.quantite + itemprodf.quantite;





                                        switch (ValueCompte)
                                        {
                                            case "Prixa":

                                                existeinfirst.privente = ((existeinfirst.prixa * existeinfirst.quantite) + (itemprodf.prixa * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);


                                                break;
                                            case "Prixb":

                                                existeinfirst.privente = ((existeinfirst.prixb * existeinfirst.quantite) + (itemprodf.prixb * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);

                                                break;

                                            case "Prixc":

                                                existeinfirst.privente = ((existeinfirst.prixc * existeinfirst.quantite) + (itemprodf.prixc * itemprodf.quantite)) / (existeinfirst.quantite + itemprodf.quantite);

                                                break;

                                            default:


                                                break;
                                        }


                                    }
                                }
                            }




                            NomenclatureProduit.ItemsSource = listprodfcoutmoyen;
                        }



                    }
                }
            }

            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chcout_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                NomenclatureProduit.ItemsSource = (proxy.GetAllProdf().OrderBy(n => n.design));
                string ValueCompte = "";
                if (CompteComboBox.SelectedIndex >= 0)
                {

                    var test = NomenclatureProduit.ItemsSource as IEnumerable;



                    ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                    switch (ValueCompte)
                    {
                        case "Prixa":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixa;
                            }

                            break;
                        case "Prixb":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixb;
                            }
                            break;

                        case "Prixc":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixc;
                            }
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
        private void chstockdisponible_Checked(object sender, RoutedEventArgs e)
        {
            /*try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (chstockdisponible.IsChecked == true && chcout.IsChecked == false)
                {
                    //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Prodf temp = item as SVC.Prodf;
                        return temp.quantite > 0;


                    };
                    string ValueCompte = "";
                    if (CompteComboBox.SelectedIndex >= 0)
                    {

                        var test = NomenclatureProduit.ItemsSource as IEnumerable;



                        ValueCompte = ((ComboBoxEditItem)CompteComboBox.SelectedItem).Content.ToString();
                        switch (ValueCompte)
                        {
                            case "Prixa":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixa;
                                }

                                break;
                            case "Prixb":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixb;
                                }
                                break;

                            case "Prixc":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixc;
                                }
                                break;

                            default:


                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }*/
            try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (chstockdisponible.IsChecked == true && chcout.IsChecked == false)
                {

                    if (FamilleCombo.SelectedItem != null)
                    {
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                        if (string.IsNullOrEmpty(txtRecherche.Text))
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.quantite > 0 && temp.IdFamille == selectedfamille.Id;


                            };
                        }
                        else
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.quantite > 0 && temp.IdFamille == selectedfamille.Id && temp.design.Contains(txtRecherche.Text.Trim());


                            };
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtRecherche.Text))
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.quantite > 0;


                            };
                        }
                        else
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.quantite > 0 && temp.design.Contains(txtRecherche.Text.Trim());


                            };
                        }
                    }
                    string ValueCompte = "";
                    if (CompteComboBox.SelectedIndex >= 0)
                    {

                        var test = NomenclatureProduit.ItemsSource as IEnumerable;



                        ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                        switch (ValueCompte)
                        {
                            case "Prixa":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixa;
                                }

                                break;
                            case "Prixb":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixb;
                                }
                                break;

                            case "Prixc":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixc;
                                }
                                break;

                            default:


                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chstockdisponible_Unchecked(object sender, RoutedEventArgs e)
        {
            /*try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (chstockdisponible.IsChecked == false && chcout.IsChecked == false)
                {
                    cv00.Filter = delegate (object item)
                    {
                        SVC.Prodf temp = item as SVC.Prodf;
                        return temp.quantite >= 0 || temp.quantite <= 0;


                    };
                    string ValueCompte = "";
                    if (CompteComboBox.SelectedIndex >= 0)
                    {

                        var test = NomenclatureProduit.ItemsSource as IEnumerable;



                        ValueCompte = ((ComboBoxEditItem)CompteComboBox.SelectedItem).Content.ToString();
                        switch (ValueCompte)
                        {
                            case "Prixa":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixa;
                                }

                                break;
                            case "Prixb":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixb;
                                }
                                break;

                            case "Prixc":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixc;
                                }
                                break;

                            default:


                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }*/
            try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (chstockdisponible.IsChecked == false && chcout.IsChecked == false)
                {

                    if (FamilleCombo.SelectedItem != null)
                    {
                        SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                        //  ICollectionView cv = CollectionViewSource.GetDefaultView(PatientDataGrid.ItemsSource);
                        if (string.IsNullOrEmpty(txtRecherche.Text))
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.IdFamille == selectedfamille.Id;


                            };
                        }
                        else
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.IdFamille == selectedfamille.Id && temp.design.Contains(txtRecherche.Text.Trim());


                            };
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtRecherche.Text))
                        {
                            cv00.Filter = null;
                        }
                        else
                        {
                            cv00.Filter = delegate (object item)
                            {
                                SVC.Prodf temp = item as SVC.Prodf;
                                return temp.design.Contains(txtRecherche.Text.Trim());


                            };
                        }
                    }
                    string ValueCompte = "";
                    if (CompteComboBox.SelectedIndex >= 0)
                    {

                        var test = NomenclatureProduit.ItemsSource as IEnumerable;



                        ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                        switch (ValueCompte)
                        {
                            case "Prixa":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixa;
                                }

                                break;
                            case "Prixb":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixb;
                                }
                                break;

                            case "Prixc":
                                foreach (SVC.Prodf item in test)
                                {
                                    item.privente = item.prixc;
                                }
                                break;

                            default:


                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message,NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }


        private void FournisseurCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            try
            {
                ICollectionView cv00 = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);

                cv00.Filter = delegate (object item)
                {
                    SVC.Prodf temp = item as SVC.Prodf;
                    return temp.IdFamille != -1;


                };

                FamilleCombo.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }






        private void NomenclatureProduit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                #region chked
                if (chcout.IsChecked == false)
                {
                    if (NomenclatureProduit.SelectedItem != null && CompteComboBox.SelectedIndex >= 0)
                    {
                        //    this.Title = title;
                        //  this.WindowTitleBrush = brushajouterfacture;

                        var selectedtropuvé = NomenclatureProduit.SelectedItem as SVC.Prodf;
                        var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                        SVC.Produit selectedproduit = new SVC.Produit();
                        if (existeproduit)
                        {
                            selectedproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                            // facturevente.serialnumber = selectedproduit.marque;
                        }
                        if (selectedtropuvé.quantite > 0)
                        {
                            if (selectedtropuvé.perempt != null)
                            {
                                if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        date = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = MemberUser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = selectedtropuvé.privente,
                                        quantite = 1,
                                        Total = selectedtropuvé.privente * 1,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    if (existeproduit)
                                    {
                                        facturevente.serialnumber = selectedproduit.marque;
                                    }
                                    else
                                    {
                                        facturevente.serialnumber = "";
                                    }
                                    var found = listcomptoir.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {
                                        SVC.Prodf produitadd = new SVC.Prodf
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            cp = selectedtropuvé.cp,
                                            datef = selectedtropuvé.datef,
                                            dates = selectedtropuvé.dates,
                                            design = selectedtropuvé.design,
                                            famille = selectedtropuvé.famille,
                                            fourn = selectedtropuvé.fourn,
                                            IdFamille = selectedtropuvé.IdFamille,
                                            Id = selectedtropuvé.Id,
                                            lot = selectedtropuvé.lot,
                                            nfact = selectedtropuvé.nfact,
                                            perempt = selectedtropuvé.perempt,
                                            previent = selectedtropuvé.previent,
                                            privente = selectedtropuvé.privente,
                                            prixa = selectedtropuvé.quantite,
                                            prixb = selectedtropuvé.prixb,
                                            prixc = selectedtropuvé.prixc,
                                            quantite = 1,
                                            tva = selectedtropuvé.tva,
                                            collisage = selectedtropuvé.collisage,

                                        };
                                        produitadd.quantite = 1;
                                        produitavendre.Add(produitadd);
                                        listcomptoir.Add(facturevente);

                                    }
                                    else
                                    {
                                        //  foreach (SVC.Prodf selectedprodf in produitavendre)
                                        // {
                                        SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedtropuvé.Id);
                                        if (selectedprodf.Id == found.ficheproduit && found.quantite < selectedtropuvé.quantite)
                                        {
                                            found.quantite = found.quantite + 1;
                                            found.Total = found.Total + (facturevente.Total);
                                            selectedprodf.quantite = selectedprodf.quantite + 1;

                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }

                                        // }



                                    }
                                    ReceptDatagrid.ItemsSource = listcomptoir;
                                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                                    digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                    nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.cp,
                                    date = DateTime.Now,
                                    datef = selectedtropuvé.datef,
                                    design = selectedtropuvé.design,
                                    lot = selectedtropuvé.lot,
                                    oper = MemberUser.Username,
                                    perempt = selectedtropuvé.perempt,
                                    tva = selectedtropuvé.tva,
                                    previent = selectedtropuvé.previent,
                                    privente = selectedtropuvé.privente,
                                    quantite = 1,
                                    Total = selectedtropuvé.privente * 1,
                                    ficheproduit = selectedtropuvé.Id,
                                    ff = selectedtropuvé.nfact,
                                    Fournisseur = selectedtropuvé.fourn,
                                    collisage = selectedtropuvé.collisage,
                                    /*****************************************/

                                };
                                if (existeproduit)
                                {
                                    facturevente.serialnumber = selectedproduit.marque;
                                }
                                else
                                {
                                    facturevente.serialnumber = "";
                                }
                                var found = listcomptoir.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {
                                    SVC.Prodf produitadd = new SVC.Prodf
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        cp = selectedtropuvé.cp,
                                        datef = selectedtropuvé.datef,
                                        dates = selectedtropuvé.dates,
                                        design = selectedtropuvé.design,
                                        famille = selectedtropuvé.famille,
                                        fourn = selectedtropuvé.fourn,
                                        IdFamille = selectedtropuvé.IdFamille,
                                        Id = selectedtropuvé.Id,
                                        lot = selectedtropuvé.lot,
                                        nfact = selectedtropuvé.nfact,
                                        perempt = selectedtropuvé.perempt,
                                        previent = selectedtropuvé.previent,
                                        privente = selectedtropuvé.privente,
                                        prixa = selectedtropuvé.quantite,
                                        prixb = selectedtropuvé.prixb,
                                        prixc = selectedtropuvé.prixc,
                                        quantite = 1,
                                        tva = selectedtropuvé.tva,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    produitadd.quantite = 1;
                                    produitavendre.Add(produitadd);
                                    listcomptoir.Add(facturevente);

                                }
                                else
                                {
                                    SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedtropuvé.Id);

                                    if (selectedprodf.Id == found.ficheproduit && found.quantite < selectedtropuvé.quantite)
                                    {
                                        found.quantite = found.quantite + 1;
                                        found.Total = found.Total + (facturevente.Total);
                                        selectedprodf.quantite = selectedprodf.quantite + 1;

                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    }





                                }
                                ReceptDatagrid.ItemsSource = listcomptoir;
                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                                digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
                            }
                        }



                        else
                        {
                            if (selectedtropuvé.quantite <= 0)
                            {
                                SVC.Facture facturevente = new SVC.Facture
                                {
                                    cab = selectedtropuvé.cab,
                                    cf = selectedtropuvé.cf,
                                    codeprod = selectedtropuvé.cp,
                                    date = DateTime.Now,
                                    datef = selectedtropuvé.datef,
                                    design = selectedtropuvé.design,
                                    lot = selectedtropuvé.lot,
                                    oper = MemberUser.Username,
                                    perempt = selectedtropuvé.perempt,
                                    tva = selectedtropuvé.tva,
                                    previent = selectedtropuvé.previent,
                                    privente = selectedtropuvé.privente,
                                    quantite = -1,
                                    Total = selectedtropuvé.privente * -1,
                                    ficheproduit = selectedtropuvé.Id,
                                    ff = selectedtropuvé.nfact,
                                    Fournisseur = selectedtropuvé.fourn,
                                    collisage = selectedtropuvé.collisage,
                                };
                               if (existeproduit)
                                {
                                    facturevente.serialnumber = selectedproduit.marque;
                                }
                                else
                                {
                                    facturevente.serialnumber = "";
                                }
                                var found = listcomptoir.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                if (found == null)
                                {
                                    listcomptoir.Add(facturevente);

                                }
                                else
                                {
                                    found.quantite = found.quantite - 1;
                                    found.Total = found.Total + (facturevente.Total);


                                }
                                ReceptDatagrid.ItemsSource = listcomptoir;
                                CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                                digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());

                            }
                        }

                    }
                    else
                    {

                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un tarif", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                #endregion
                else
                {
                    if (chcout.IsChecked == true)
                    {
                        /**/
                        if (NomenclatureProduit.SelectedItem != null && CompteComboBox.SelectedIndex >= 0)
                        {
                            var selectedtropuvé1 = NomenclatureProduit.SelectedItem as SVC.Prodf;
                            var selectedtropuvé = proxy.GetAllProdfbycode(Convert.ToInt32(selectedtropuvé1.cp)).Where(n => n.quantite != 0).First();
                            var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                            SVC.Produit selectedproduit = new SVC.Produit();
                            if (existeproduit)
                            {
                                selectedproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                // facturevente.serialnumber = selectedproduit.marque;
                            }
                            if (selectedtropuvé.quantite > 0)
                            {
                                if (selectedtropuvé.perempt != null)
                                {
                                    if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))
                                    {
                                        SVC.Facture facturevente = new SVC.Facture
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            codeprod = selectedtropuvé.cp,
                                            date = DateTime.Now,
                                            datef = selectedtropuvé.datef,
                                            design = selectedtropuvé.design,
                                            lot = selectedtropuvé.lot,
                                            oper = MemberUser.Username,
                                            perempt = selectedtropuvé.perempt,
                                            tva = selectedtropuvé.tva,
                                            previent = selectedtropuvé1.previent,
                                            privente = selectedtropuvé1.privente,
                                            quantite = 1,
                                            Total = selectedtropuvé1.privente * 1,
                                            ficheproduit = selectedtropuvé.Id,
                                            ff = selectedtropuvé.nfact,
                                            Fournisseur = selectedtropuvé.fourn,
                                            collisage = selectedtropuvé.collisage,
                                        }; if (existeproduit)
                                        {
                                            facturevente.serialnumber = selectedproduit.marque;
                                        }
                                        else
                                        {
                                            facturevente.serialnumber = "";
                                        }
                                        var found = listcomptoir.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                        if (found == null)
                                        {
                                            SVC.Prodf produitadd = new SVC.Prodf
                                            {
                                                cab = selectedtropuvé.cab,
                                                cf = selectedtropuvé.cf,
                                                cp = selectedtropuvé.cp,
                                                datef = selectedtropuvé.datef,
                                                dates = selectedtropuvé.dates,
                                                design = selectedtropuvé.design,
                                                famille = selectedtropuvé.famille,
                                                fourn = selectedtropuvé.fourn,
                                                IdFamille = selectedtropuvé.IdFamille,
                                                Id = selectedtropuvé.Id,
                                                lot = selectedtropuvé.lot,
                                                nfact = selectedtropuvé.nfact,
                                                perempt = selectedtropuvé.perempt,
                                                previent = selectedtropuvé.previent,
                                                privente = selectedtropuvé.privente,
                                                prixa = selectedtropuvé.quantite,
                                                prixb = selectedtropuvé.prixb,
                                                prixc = selectedtropuvé.prixc,
                                                quantite = 1,
                                                tva = selectedtropuvé.tva,
                                                collisage = selectedtropuvé.collisage,
                                            };
                                            produitadd.quantite = 1;
                                            produitavendre.Add(produitadd);
                                            listcomptoir.Add(facturevente);
                                        }
                                        else
                                        {
                                            //  foreach (SVC.Prodf selectedprodf in produitavendre)
                                            // {
                                            SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedtropuvé.Id);
                                            if (selectedprodf.Id == found.ficheproduit && found.quantite < selectedtropuvé.quantite)
                                            {
                                                found.quantite = found.quantite + 1;
                                                found.Total = found.Total + (facturevente.Total);
                                                selectedprodf.quantite = selectedprodf.quantite + 1;
                                            }
                                            else
                                            {
                                                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                            }
                                        }
                                        ReceptDatagrid.ItemsSource = listcomptoir;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                        var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                                        digitalGaugeControl1.Content= String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                        nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Ce produit est interdit en vente date de péremption proche", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    }
                                }
                                else
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        date = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = MemberUser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé1.previent,
                                        privente = selectedtropuvé1.privente,
                                        quantite = 1,
                                        Total = selectedtropuvé1.privente * 1,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        collisage = selectedtropuvé.collisage,
                                        /*****************************************/
                                    };
                                    if (existeproduit)
                                    {
                                        facturevente.serialnumber = selectedproduit.marque;
                                    }
                                    else
                                    {
                                        facturevente.serialnumber = "";
                                    }
                                    var found = listcomptoir.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {
                                        SVC.Prodf produitadd = new SVC.Prodf
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            cp = selectedtropuvé.cp,
                                            datef = selectedtropuvé.datef,
                                            dates = selectedtropuvé.dates,
                                            design = selectedtropuvé.design,
                                            famille = selectedtropuvé.famille,
                                            fourn = selectedtropuvé.fourn,
                                            IdFamille = selectedtropuvé.IdFamille,
                                            Id = selectedtropuvé.Id,
                                            lot = selectedtropuvé.lot,
                                            nfact = selectedtropuvé.nfact,
                                            perempt = selectedtropuvé.perempt,
                                            previent = selectedtropuvé.previent,
                                            privente = selectedtropuvé.privente,
                                            prixa = selectedtropuvé.quantite,
                                            prixb = selectedtropuvé.prixb,
                                            prixc = selectedtropuvé.prixc,
                                            quantite = 1,
                                            tva = selectedtropuvé.tva,
                                        };
                                        produitadd.quantite = 1;
                                        produitavendre.Add(produitadd);
                                        listcomptoir.Add(facturevente);
                                    }
                                    else
                                    {
                                        SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedtropuvé.Id);
                                        if (selectedprodf.Id == found.ficheproduit && found.quantite < selectedtropuvé.quantite)
                                        {
                                            found.quantite = found.quantite + 1;
                                            found.Total = found.Total + (facturevente.Total);
                                            selectedprodf.quantite = selectedprodf.quantite + 1;
                                        }
                                        else
                                        {
                                            MessageBoxResult resulCt = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }
                                    }
                                    ReceptDatagrid.ItemsSource = listcomptoir;
                                    CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                    var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                                    digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                    nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());

                                }
                            }
                        }
                        else
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un tarif", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        /**/
                    }
                }
            }
            catch (Exception EX)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(EX.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }







        private void ReceptDatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                /*    if (e.EditAction == DataGridEditAction.Commit)
                    {
                        var column = e.Column as DataGridBoundColumn;
                        if (column != null)
                        {
                            var bindingPath = (column.Binding as Binding).Path.Path;
                            if (bindingPath == "codeprod")
                            {
                                int rowIndex = e.Row.GetIndex();
                                var el = e.EditingElement as TextBox;
                                code = el.Text;

                            }
                        }
                    }*/
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ReceptDatagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }


        private void ReceptDatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                /*  var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                  digitalGaugeControl1.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.privente * o.quantite));
                  nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
                  foreach (var item in test)
                  {
                      item.Total = item.privente * item.quantite;
                  }
                  ReceptDatagrid.ItemsSource = test;*/
                var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
                foreach (var item in test)
                {
                    item.Total = item.privente * item.quantite;
                }
                ReceptDatagrid.ItemsSource = test;



            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }



        private void DXWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (chcodebare.IsChecked == true)
                {
                    txtbarrecode.Focus();
                }
                else
                {
                    txtRecherche.Focus();
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }


        private void CONFIRMERVENTEFINAL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CONFIRMERVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listcomptoir.Count > 0 && listcomptoir.AsEnumerable().Sum(o => o.privente * o.quantite) != 0 && MemberUser.ModuleVenteCompoirAcces == true)
                {
                    if(chcodebare.IsChecked==true)
                    {
                        txtbarrecode.Focus();
                    }
                    else
                    {
                        txtRecherche.Focus();
                    }
                    Confirmation cl = new Confirmation(proxy, callback, MemberUser, this);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void annulerVENTE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcodebare.IsChecked == true)
                {
                    txtbarrecode.Focus();
                }
                else
                {
                    txtRecherche.Focus();
                }
                listcomptoir = new List<SVC.Facture>();
                ReceptDatagrid.ItemsSource = listcomptoir;
                digitalGaugeControl1.Content = String.Format("{0:0.##}", ((listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)));
                nbreproduit.Text = Convert.ToString((listcomptoir).AsEnumerable().Count());

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void DXWndow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.F12:
                        if (listcomptoir.Count > 0 && listcomptoir.AsEnumerable().Sum(o => o.privente * o.quantite) > 0)
                        {
                            if (chcodebare.IsChecked == true)
                            {
                                txtbarrecode.Focus();
                            }
                            else
                            {
                                txtRecherche.Focus();
                            }
                            Confirmation cl = new Confirmation(proxy, callback, MemberUser, this);
                            cl.Show();
                        }
                        break;
                    case Key.F8:

                        if (listcomptoir.Count > 0 && listcomptoir.AsEnumerable().Sum(o => o.privente * o.quantite) > 0)
                        {
                            if (chcodebare.IsChecked == true)
                            {
                                txtbarrecode.Focus();
                            }
                            else
                            {
                                txtRecherche.Focus();
                            }
                            confirmerdirect();
                        }
                        break;
                    case Key.Escape:
                        listcomptoir = new List<SVC.Facture>();
                        ReceptDatagrid.ItemsSource = listcomptoir;
                        digitalGaugeControl1.Content = String.Format("{0:0.##}", ((listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite)));
                        nbreproduit.Text = Convert.ToString((listcomptoir).AsEnumerable().Count());
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }


        SVC.Prodf retournercodebarreprodffirst(string codeabarre)
        {

            var ll = proxy.GetAllTcabbycode(codeabarre).ToList();
            var test = NomenclatureProduit.ItemsSource as IEnumerable<SVC.Prodf>;
            SVC.Prodf produit = new SVC.Prodf();
            if (ll.Count > 0)
            {
                foreach (var itemcode in ll)
                {

                    var tte = test.Any(n => n.cab == itemcode.cleproduit && n.quantite > 0);
                    if (tte == true)
                    {

                        var produitexiste = test.Where(n => n.cab == itemcode.cleproduit && n.quantite > 0);

                        foreach (var prodff in produitexiste)
                        {

                            bool selectedprodfexiste = produitavendre.Any(n => n.Id == prodff.Id);

                            if (prodff.perempt != null)
                            {

                                if (selectedprodfexiste)
                                {
                                    var selectedprodf = produitavendre.Find(n => n.Id == prodff.Id);

                                    if (prodff.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)) && selectedprodf.prixa > selectedprodf.quantite)
                                    {

                                        produit = prodff;
                                        break;
                                    }
                                }
                                else
                                {

                                    produit = prodff;
                                    break;

                                }
                            }
                            else
                            {
                                if (selectedprodfexiste)
                                {
                                    var selectedprodf = produitavendre.Find(n => n.Id == prodff.Id);
                                    if (selectedprodf.prixa > selectedprodf.quantite)
                                    {
                                        // if (selectedtropuvé.perempt.Value >= DateTime.Now.Date.AddDays(Convert.ToDouble(selectedparam.interdirperempt)))

                                        produit = prodff;
                                        break;
                                    }

                                }
                                else
                                {
                                    produit = prodff;
                                    break;
                                }
                            }



                        }
                    }
                }
            }


            return produit;
        }


        private void txtbarrecode_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                txtbarrecode.Text = "";
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }





        private void txtbarrecode_LostFocus_1(object sender, RoutedEventArgs e)
        {
            try
            {

                ((TextBox)sender).Text = "123456789";

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void PatientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PatientDataGrid.SelectedItem != null)
                {
                    SVC.F1 selectedf1 = PatientDataGrid.SelectedItem as SVC.F1;
                    dETAILDatagrid.ItemsSource = proxy.GetAllFactureBycompteur(selectedf1.nfact);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public bool DetectPatientInLIST(List<SVC.F1> myList)
        {
            if (myList.Any())
            {
                var value = myList.First().codeclient;
                return myList.All(item => item.codeclient == value);
            }
            else
            {
                return true;
            }
        }
        private void btnNewRéglement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MemberUser.CreationRecouv == true && PatientDataGrid.SelectedItem != null && PatientDataGrid.SelectedItems.Count == 1)
                {

                    SVC.F1 SelectMedecin = PatientDataGrid.SelectedItem as SVC.F1;
                    if (SelectMedecin.reste != 0)
                    {
                        AjouterTransaction bn = new AjouterTransaction(proxy, MemberUser, callback, null, 3, null, SelectMedecin, null, null);
                        bn.Show();
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Facture déja soldé", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                    }

                }
                else
                {
                    if (MemberUser.CreationRecouv == true && PatientDataGrid.SelectedItem != null && PatientDataGrid.SelectedItems.Count > 1)
                    {
                        List<SVC.F1> selectedreste = PatientDataGrid.SelectedItems.OfType<SVC.F1>().ToList();
                        if (DetectPatientInLIST(selectedreste))
                        {
                            List<SVC.F1> Realselectedvisitewithreste = new List<SVC.F1>();
                            foreach (SVC.F1 item in selectedreste)
                            {
                                if (item.reste != 0)
                                {
                                    Realselectedvisitewithreste.Add(item);
                                }

                            }
                            if (Realselectedvisitewithreste.Count == 1)
                            {
                                AjouterTransaction bn = new AjouterTransaction(proxy, MemberUser, callback, null, 3, null, Realselectedvisitewithreste.First(), null, null);
                                bn.Show();
                            }
                            else
                            {
                                if (Realselectedvisitewithreste.Count > 1)
                                {
                                    AjouterTransaction bn = new AjouterTransaction(proxy, MemberUser, callback, null, 4, null, null, Realselectedvisitewithreste, null);
                                    bn.Show();
                                }
                                else
                                {
                                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                }
                            }
                        }
                        else
                        {
                            MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez selectionnez les visites d'un même patient", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void ReceptDatagrid_CellEditEnding_1(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                var datag = (DataGrid)sender;
                var p = (SVC.Facture)datag.SelectedValue;
                var p1 = (SVC.Facture)e.Row.Item;
                string ancien = p.design + "/ quantite " + p.quantite;
                string nouveau = (p1.design + "/quantite " + p1.quantite);
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ancien, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(nouveau, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }


        }

        private void ReceptDatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ReceptDatagrid.SelectedItem != null)
                {
                   
                   
                    SVC.Facture selectedrecept = ReceptDatagrid.SelectedItem as SVC.Facture;
                    if (selectedrecept.Id != 0)
                    {
                        bool existeselectedprodf = produitavendre.Any(n => n.Id == selectedrecept.ficheproduit);

                        if (existeselectedprodf)
                        {
                            if (chcodebare.IsChecked == true)
                            {
                                txtbarrecode.Focus();
                            }
                            else
                            {
                                txtRecherche.Focus();
                            }
                            SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedrecept.ficheproduit);
                            DetailVente cl = new DetailVente(proxy, MemberUser, callback, selectedrecept, selectedparam, selectedprodf, 0,ReceptDatagrid,digitalGaugeControl1,nbreproduit);
                            cl.Show();
                        }
                        else
                        {
                            if (chcodebare.IsChecked == true)
                            {
                                txtbarrecode.Focus();
                            }
                            else
                            {
                                txtRecherche.Focus();
                            }
                            DetailVente cl = new DetailVente(proxy, MemberUser, callback, selectedrecept, selectedparam, null, 0, ReceptDatagrid, digitalGaugeControl1, nbreproduit);
                            cl.Show();
                        }
                    }
                    else
                    {
                        if (chcodebare.IsChecked == true)
                        {
                            txtbarrecode.Focus();
                        }
                        else
                        {
                            txtRecherche.Focus();
                        }
                        DetailVente cl = new DetailVente(proxy, MemberUser, callback, selectedrecept, selectedparam, null, 1, ReceptDatagrid, digitalGaugeControl1, nbreproduit);
                        cl.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

       
        

        private void chcodebare_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcodebare.IsChecked == true)
                {
                    txtbarrecode.Focus();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void chcodebare_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chcodebare.IsChecked == false)
                {
                    txtRecherche.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void PatientDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PatientDataGrid.ItemsSource = proxy.GetAllF1(DateTime.Now, DateTime.Now).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B");
                txtDebit.Text = Convert.ToString((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B").AsEnumerable().Sum(o => o.net));
                TxtCreebit.Text = Convert.ToString((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B").AsEnumerable().Sum(o => o.versement));
                txtSolde.Text = Convert.ToString(((proxy.GetAllF1(DateTime.Now, DateTime.Now)).Where(n => n.nfact.Substring(0, 2) == "Co" || n.nfact.Substring(0, 1) == "B").AsEnumerable().Sum(o => o.reste)));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void BtnReception_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (MemberUser.CreationCommande == true)
                {
                    AjouterCommande cl = new AjouterCommande(proxy, MemberUser, callback, null, null, null);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void BtnService_Click(object sender, RoutedEventArgs e)
        {
            #region chked



            SVC.Facture facturevente = new SVC.Facture
            {
                cab = "",
                cf = 0,
                codeprod = 0,
                date = DateTime.Now,
                datef = DateTime.Now,
                design = "Service et maintenance",
                lot = "",
                oper = MemberUser.Username,
                perempt = null,
                tva = 0,
                previent = 0,
                privente = 1,
                quantite = 1,
                Total = 1 * 1,
                ficheproduit = 0,
                ff = "Service et maintenance",
                Fournisseur = "Service et maintenance",
                collisage = 1,
            };
            var found = listcomptoir.Find(item => item.ficheproduit == 0);
            if (found == null)
            {

                listcomptoir.Add(facturevente);

            }
            else
            {
                //  foreach (SVC.Prodf selectedprodf in produitavendre)
                // {

                found.quantite = found.quantite + 1;
                found.Total = found.Total + (facturevente.Total);

                // }



            }
            ReceptDatagrid.ItemsSource = listcomptoir;
            CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
            var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
            digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
            nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());
        }


        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NomenclatureProduit.SelectedItem != null)
                {
                    SVC.Prodf selectedprodf = NomenclatureProduit.SelectedItem as SVC.Prodf;
                    var produit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedprodf.cp)).First();
                    ImageProduit cl = new ImageProduit(proxy, produit, MemberUser, callback);
                    cl.Show();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void confirmerdirect()
        {
            SVC.ClientV selectedclient = proxy.GetAllClientVBYID(0).First();
            if (selectedclient.Id == 0)
            {
                SVC.F1 selectF1 = new SVC.F1
                {
                    codeclient = selectedclient.Id = 0,
                    dates = DateTime.Now,
                    date = DateTime.Now,
                    oper = MemberUser.Username,
                    timbre = 0,
                    bcom = "",
                    echeance = 0,
                    raison = selectedclient.Raison,
                    soldé = true,
                    heure = DateTime.Now.TimeOfDay,
                    reste = 0,
                    tva = 0,
                    modep = "ESPECES",
                    net = (listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),
                    ht = (listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),
                    versement = (listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite),
                    cle = selectedclient.Id + selectedclient.Raison + (listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite) + DateTime.Now.TimeOfDay,
                };

                selectF1.remise = 0;

                var remisepourfacture = selectF1.remise;
                bool Operfacture = false;

                List<int> listrefresh = new List<int>();

                foreach (SVC.Facture newfacture in listcomptoir)
                {
                    newfacture.cle = selectF1.cle;
                    newfacture.codeclient = selectF1.codeclient;
                    newfacture.Client = selectF1.raison;
                    if (remisepourfacture != 0)
                    {
                        if (remisepourfacture >= ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite)))
                        {
                            newfacture.remise = ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                            remisepourfacture = remisepourfacture - ((newfacture.privente) * (newfacture.quantite)) - ((newfacture.previent) * (newfacture.quantite));
                        }
                        else
                        {
                            newfacture.remise = remisepourfacture;
                        }

                    }
                    else
                    {
                        newfacture.remise = 0;
                    }
                    if (newfacture.codeprod != 0)
                    {
                        listrefresh.Add(Convert.ToInt32(newfacture.ficheproduit));
                    }
                }
                SVC.Depeiment PAIEMENT = new SVC.Depeiment
                {
                    date = selectF1.date,
                    montant = Convert.ToDecimal(selectF1.net),
                    paiem = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                    oper = MemberUser.Username,
                    dates = selectF1.dates,
                    banque = "Caisse",
                    nfact = selectF1.nfact,
                    amontant = Convert.ToDecimal(selectF1.net),
                    cle = selectF1.cle,
                    cp = selectF1.Id,
                    Multiple = false,
                    CodeClient = selectF1.codeclient,
                    RaisonClient = selectF1.raison,

                };
                SVC.Depense CAISSE = new SVC.Depense
                {
                    cle = selectF1.cle,
                    Auto = true,
                    Commentaires = "ESPECES" + " Vente :" + selectF1.nfact + " " + " date :" + selectF1.date,
                    CompteDébité = "Caisse",
                    Crédit = true,
                    DateDebit = selectF1.date,
                    DateSaisie = selectF1.dates,
                    Débit = false,
                    ModePaiement = "ESPECES",
                    Montant = 0,
                    MontantCrédit = selectF1.net,
                    NumCheque = Convert.ToString(selectF1.Id),
                    Num_Facture = selectF1.nfact,
                    RubriqueComptable = "ESPECES COMPTOIR",
                    Username = MemberUser.Username,

                };

                bool ii = false;
                bool depense = false;
                bool depaiement = false;
                List<SVC.Facture> listsanszero = new List<SVC.Facture>();
                foreach (var item in listcomptoir)
                {
                    if (item.quantite != 0)
                    {

                        listsanszero.Add(item);
                    }
                }
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    ii = proxy.InsertFacture(selectF1, listsanszero, "Comptoir");

                    Operfacture = true;
                    proxy.InsertDepeiment(PAIEMENT);
                    depaiement = true;
                    proxy.InsertDepense(CAISSE);
                    depense = true;

                    if (Operfacture == true && ii == true && depaiement == true && depense == true)
                    {
                        ts.Complete();
                    }
                }
                if (Operfacture == true && ii == true && depaiement == true && depense == true)
                {
                    proxy.AjouterProdflistRefresh(listrefresh);
                    proxy.AjouterSoldeF1Refresh();

                    //this.comboclient.IsEnabled = true;
                    listcomptoir = new List<SVC.Facture>();
                    ReceptDatagrid.ItemsSource = listcomptoir;
                    digitalGaugeControl1.Content = Convert.ToString((listcomptoir).AsEnumerable().Sum(o => o.privente * o.quantite));
                    nbreproduit.Text = Convert.ToString((listcomptoir).AsEnumerable().Count());
                    if (chimprimer.IsChecked == true)
                    {

                        var Nfact = proxy.GetAllF1ByCle(selectF1.cle);


                        LocalReport reportViewer1 = new LocalReport();

                        string nfact = Nfact.First().nfact.Substring(0, 1);
                        string arrété = "";
                        string document = "";
                        switch (nfact)
                        {
                            case "F":
                                arrété = "Arrétée la présente facture à la somme de";
                                document = "Facture";
                                break;
                            case "A":
                                arrété = "Arrétée la présente facture d'Avoir à la somme de";
                                document = "Facture d'Avoir";
                                break;
                            case "B":
                                arrété = "Arrétée le présent Bon de Livraison à la somme de";
                                document = "Bon de Livraison";
                                break;
                            case "C":
                                arrété = "Arrétée le présent Avoir de Bon de Livraison à la somme de";
                                document = "Bon d'Avoir";

                                break;
                            case "P":
                                arrété = "Arrétée la présente facture Proforma à la somme de";
                                document = "Facture Proforma";
                                break;
                            case "R":
                                arrété = "Arrétée la présente facture à la somme de";
                                document = "Facture";
                                break;
                        }

                        /******************Dataset1 parametre****************************/
                        var selpara = new List<SVC.Param>();
                        selpara.Add((proxy.GetAllParamétre()));
                        /****************Convertion*****************/
                        string Montant = "";

                        if (Nfact.First().net > 0)
                        {
                            Montant = (Convert.ToDecimal(Nfact.First().net)).ToText(Nut.Currency.EUR, Nut.Language.French);

                        }
                        else
                        {
                            if (Nfact.First().net < 0)
                            {
                                var mm = -Nfact.First().net;
                                Montant = "Moin " + (Convert.ToDecimal(mm)).ToText(Nut.Currency.EUR, Nut.Language.French);
                            }
                        }
                        Montant = Montant.Replace("euro", selpara.First().Limon.ToString());

                        /************************************/
                        MemoryStream MyRptStream = new MemoryStream((NewOptics.Properties.Resources.Ticket), false);

                        reportViewer1.LoadReportDefinition(MyRptStream);
                        /*************************************************/
                        ReportDataSource rds = new ReportDataSource();
                        rds.Name = "DataSet2";//This refers to the dataset name in the RDLC file
                                              //            listerecu=proxy.GetAllSalleDattente();         // rds.Value = proxy1.GetAllMembership();
                        rds.Value = Nfact;
                        reportViewer1.DataSources.Add(rds);
                        /**************************************************/
                        var ClientList = new List<SVC.ClientV>();
                        ClientList.Add(selectedclient);
                        reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet4", ClientList));
                        /********************************************/
                        var FactureList = proxy.GetAllFactureBycompteur(Nfact.First().nfact);

                        reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet3", FactureList.ToList()));
                        /*********************************************/

                        reportViewer1.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", selpara));
                        /********ImagePath************************************/
                        reportViewer1.EnableExternalImages = true;
                        ReportParameter paramLogo = new ReportParameter();
                        paramLogo.Name = "ImagePath";
                        String photolocation = System.Environment.CurrentDirectory + "/Logo.png";

                        paramLogo.Values.Add(@"file:///" + photolocation);
                        reportViewer1.SetParameters(paramLogo);
                        /**************************************************************/
                        ReportParameter paramDocument = new ReportParameter();
                        paramDocument.Name = "Document";
                        paramDocument.Values.Add(document);
                        reportViewer1.SetParameters(paramDocument);
                        /*************************************************************/
                        ReportParameter paramArrettee = new ReportParameter();
                        paramArrettee.Name = "Arrettee";
                        paramArrettee.Values.Add(arrété);
                        reportViewer1.SetParameters(paramArrettee);
                        /*************************************************/
                        ReportParameter paramMontantString = new ReportParameter();
                        paramMontantString.Name = "MontantString";
                        paramMontantString.Values.Add(Montant);
                        reportViewer1.SetParameters(paramMontantString);
                        /*********************************************************/
                        TimeSpan myTimeSpan = new TimeSpan(Nfact.First().heure.Value.Hours, Nfact.First().heure.Value.Minutes, Nfact.First().heure.Value.Seconds);


                        ReportParameter paramTimeTo = new ReportParameter();
                        paramTimeTo.Name = "TimeTo";
                        paramTimeTo.Values.Add(myTimeSpan.ToString("hh\\:mm\\:ss"));
                        reportViewer1.SetParameters(paramTimeTo);
                        reportViewer1.Refresh();

                        Export(reportViewer1);
                        Print();
                    }
                    if (chcodebare.IsChecked == true)
                    {

                        txtbarrecode.Focus();
                    }
                    else
                    {
                        txtRecherche.Focus();
                    }
                }



            }

        }

        private void CONFIRMERVENTEDIRECT_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                confirmerdirect();




            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        private void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.DocumentName = "Ticket Comptoir";
                printDoc.Print();
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>72mm</PageWidth>
                <PageHeight>210mm</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        private Stream CreateStream(string name,
 string fileNameExtension, Encoding encoding,
 string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        private void txtbarrecode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        try
                        {
                            if (/*txtbarrecode.Text.Count() == txtbarrecode.MaxLength && */CompteComboBox.SelectedIndex >= 0)
                            {
                                SVC.Prodf selectedtropuvé = retournercodebarreprodffirst(txtbarrecode.Text);
                                var existeproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).Any();
                                SVC.Produit selectedproduit = new SVC.Produit();
                                if (existeproduit)
                                {
                                    selectedproduit = proxy.GetAllProduitbyid(Convert.ToInt32(selectedtropuvé.cp)).First();
                                    // facturevente.serialnumber = selectedproduit.marque;
                                }
                                if (selectedtropuvé != null && selectedtropuvé.Id != 0 && selectedtropuvé.quantite > 0)
                                {
                                    SVC.Facture facturevente = new SVC.Facture
                                    {
                                        cab = selectedtropuvé.cab,
                                        cf = selectedtropuvé.cf,
                                        codeprod = selectedtropuvé.cp,
                                        date = DateTime.Now,
                                        datef = selectedtropuvé.datef,
                                        design = selectedtropuvé.design,
                                        lot = selectedtropuvé.lot,
                                        oper = MemberUser.Username,
                                        perempt = selectedtropuvé.perempt,
                                        tva = selectedtropuvé.tva,
                                        previent = selectedtropuvé.previent,
                                        privente = selectedtropuvé.privente,
                                        quantite = 1,
                                        Total = selectedtropuvé.privente * 1,
                                        ficheproduit = selectedtropuvé.Id,
                                        ff = selectedtropuvé.nfact,
                                        Fournisseur = selectedtropuvé.fourn,
                                        collisage = selectedtropuvé.collisage,
                                    };
                                    if (existeproduit)
                                    {
                                        facturevente.serialnumber = selectedproduit.marque;
                                    }
                                    else
                                    {
                                        facturevente.serialnumber = "";
                                    }
                                    var found = listcomptoir.Find(item => item.ficheproduit == selectedtropuvé.Id);
                                    if (found == null)
                                    {

                                        SVC.Prodf produitadd = new SVC.Prodf
                                        {
                                            cab = selectedtropuvé.cab,
                                            cf = selectedtropuvé.cf,
                                            cp = selectedtropuvé.cp,
                                            datef = selectedtropuvé.datef,
                                            dates = selectedtropuvé.dates,
                                            design = selectedtropuvé.design,
                                            famille = selectedtropuvé.famille,
                                            fourn = selectedtropuvé.fourn,
                                            IdFamille = selectedtropuvé.IdFamille,
                                            Id = selectedtropuvé.Id,
                                            lot = selectedtropuvé.lot,
                                            nfact = selectedtropuvé.nfact,
                                            perempt = selectedtropuvé.perempt,
                                            previent = selectedtropuvé.previent,
                                            privente = selectedtropuvé.privente,
                                            prixa = selectedtropuvé.quantite,
                                            prixb = selectedtropuvé.prixb,
                                            prixc = selectedtropuvé.prixc,
                                            quantite = 1,
                                            tva = selectedtropuvé.tva,
                                            collisage = selectedtropuvé.collisage,
                                        };
                                        produitadd.quantite = 1;
                                        produitavendre.Add(produitadd);
                                        listcomptoir.Add(facturevente);

                                    }
                                    else
                                    {
                                        SVC.Prodf selectedprodf = produitavendre.Find(n => n.Id == selectedtropuvé.Id);

                                        if (selectedprodf.Id == found.ficheproduit && found.quantite < selectedtropuvé.quantite)
                                        {
                                            found.quantite = found.quantite + 1;
                                            found.Total = found.Total + (facturevente.Total);
                                            selectedprodf.quantite = selectedprodf.quantite + 1;

                                        }
                                        else
                                        {
                                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        }




                                    }
                                    if (listcomptoir.Count > 0)
                                    {
                                        ReceptDatagrid.ItemsSource = listcomptoir;
                                        CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource).Refresh();
                                        var test = ReceptDatagrid.ItemsSource as IEnumerable<SVC.Facture>;
                                        digitalGaugeControl1.Content = String.Format("{0:0.##}", ((test).AsEnumerable().Sum(o => o.privente * o.quantite)));
                                        nbreproduit.Text = Convert.ToString((test).AsEnumerable().Count());



                                        txtbarrecode.Text = "";
                                        txtbarrecode.Focus();
                                    }
                                    else
                                    {
                                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit n'existe pas", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        txtbarrecode.Text = "";
                                        txtbarrecode.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Produit n'existe pas quantité quantité insuffisante", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                                    txtbarrecode.Text = "";
                                    txtbarrecode.Focus();

                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

     

        private void ListeMarqueCombo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                if (FamilleCombo.SelectedItem != null)
                {
                    SVC.FamilleProduit selectedfamille = FamilleCombo.SelectedItem as SVC.FamilleProduit;
                    if (chstockdisponible.IsChecked == true)
                    {
                        if (filter == "")
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.IdFamille == selectedfamille.Id && p.quantite > 0);
                            };
                        else
                        {
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.design.ToUpper().Contains(filter.ToUpper()) && p.IdFamille == selectedfamille.Id && p.quantite > 0);
                            };
                        }
                    }
                    else
                    {
                        if (filter == "")
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.IdFamille == selectedfamille.Id);
                            };
                        else
                        {
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.design.ToUpper().Contains(filter.ToUpper()) && p.IdFamille == selectedfamille.Id);
                            };
                        }
                    }
                }
                else
                {
                    if (chstockdisponible.IsChecked == true)
                    {
                        if (filter == "")
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.quantite > 0);
                            };
                        else
                        {
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.design.ToUpper().Contains(filter.ToUpper()) && p.quantite > 0);
                            };
                        }
                    }
                    else
                    {
                        if (filter == "")
                            cv.Filter = null;
                        else
                        {
                            cv.Filter = o =>
                            {
                                SVC.Prodf p = o as SVC.Prodf;
                                if (t.Name == "txtId")
                                    return (p.Id == Convert.ToInt32(filter));
                                return (p.design.ToUpper().Contains(filter.ToUpper()));
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        private void FamilleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FamilleCombo.SelectedItem != null)
                {
                    SVC.FamilleProduit t = FamilleCombo.SelectedItem as SVC.FamilleProduit;

                    var filter = (t.Id).ToString();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(NomenclatureProduit.ItemsSource);
                    if (chstockdisponible.IsChecked == true)
                    {
                        if (string.IsNullOrEmpty(txtRecherche.Text))
                        {
                            if (filter == "")
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.quantite > 0);
                                };
                            else
                            {
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.IdFamille.ToString().ToUpper().Equals(filter.ToUpper()) && p.quantite > 0);
                                };

                            }
                        }
                        else
                        {
                            if (filter == "")
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.quantite > 0 && p.design.Contains(txtRecherche.Text.Trim()));
                                };
                            else
                            {
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.IdFamille.ToString().ToUpper().Equals(filter.ToUpper()) && p.quantite > 0 && p.design.Contains(txtRecherche.Text.Trim()));
                                };

                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtRecherche.Text))
                        {
                            if (filter == "")
                                cv.Filter = null;
                            else
                            {
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.IdFamille.ToString().ToUpper().Equals(filter.ToUpper()));
                                };

                            }
                        }
                        else
                        {
                            if (filter == "")
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.design.Contains(txtRecherche.Text.Trim()));
                                };
                            else
                            {
                                cv.Filter = o =>
                                {
                                    SVC.Prodf p = o as SVC.Prodf;
                                    if (t.Id.ToString() == "txtId")
                                        return ((p.IdFamille).ToString() == filter);
                                    return (p.IdFamille.ToString().ToUpper().Equals(filter.ToUpper()) && p.design.Contains(txtRecherche.Text.Trim()));
                                };

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

        private void CompteComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {

                string ValueCompte = "";
                if (CompteComboBox.SelectedIndex >= 0)
                {

                    var test = NomenclatureProduit.ItemsSource as IEnumerable;



                    ValueCompte = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                    switch (ValueCompte)
                    {
                        case "Prixa":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixa;
                            }

                            break;
                        case "Prixb":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixb;
                            }
                            break;

                        case "Prixc":
                            foreach (SVC.Prodf item in test)
                            {
                                item.privente = item.prixc;
                            }
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

        private void CompteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CompteComboBox.SelectedIndex >= 0)
                {
                    txttarif.Text = ((ComboBoxItem)CompteComboBox.SelectedItem).Content.ToString();
                }
                else
                {
                    txttarif.Text = "Vous devez choisir un tarif";
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
