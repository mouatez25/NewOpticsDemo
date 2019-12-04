
using NewOptics.Administrateur;
using System;
using System.Collections;
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

namespace NewOptics.Statistique
{
    /// <summary>
    /// Interaction logic for Achatsdesproduits.xaml
    /// </summary>
    public partial class Achatsdesproduits : Window
    {
        ICallback callback;
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MembershipOptic;
        private delegate void FaultedInvokerAchatProduit();
        int interfaceimpression = 0;
        public Achatsdesproduits(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu)
        {
            try
            {
                InitializeComponent();
            proxy = proxyrecu;
            callback = callbackrecu;
            MembershipOptic = memberrecu;
            DateSaisieFin.SelectedDate = DateTime.Now;
            DateSaisieDébut.SelectedDate = DateTime.Now;
                SVC.Fourn tout = new SVC.Fourn
                {
                    raison="Tout les fournisseurs",
                    Id=0,
                };
                List<SVC.Fourn> itm = (proxy.GetAllFourn().OrderBy(x => x.raison)).ToList();
                itm.Insert(0, tout);
                FournisseurCombo.ItemsSource = itm;
      
             //   FournisseurCombo.ItemsSource = itm;
            FournisseurCombo.SelectedIndex = 0;
            callbackrecu.InsertFournCallbackEvent += new ICallback.CallbackEventHandler20(callbackrecufourn_Refresh);
            proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
            proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        void callbackrecufourn_Refresh(object source, CallbackEventInsertFourn e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshfourn(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefreshfourn(List<SVC.Fourn> listMembershipOptic)
        {
            try
            {

                FournisseurCombo.ItemsSource = listMembershipOptic;
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAchatProduit(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAchatProduit(HandleProxy));
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

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    string Value = "";

                    if (FiscalCombo.SelectedIndex >= 0)
                    {
                        Value = ((ComboBoxItem)FiscalCombo.SelectedItem).Content.ToString();

                        switch (Value)
                        {
                            case "Toutes les factures":
                                 SVC.Fourn selectedfourn = FournisseurCombo.SelectedItem as SVC.Fourn;
                                if (FournisseurCombo.SelectedIndex == 0)
                                {
                                    List<SVC.Recouf> listrecouf = proxy.GetAllRecouf().Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate).ToList();
                                    List<SVC.Recept> listrecept = (proxy.GetAllRecept()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate).ToList();
                                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.datef >= DateSaisieDébut.SelectedDate && n.datef <= DateSaisieFin.SelectedDate).ToList();
                                    List<receptcalcule> listfinal = new List<receptcalcule>();
                                   
                                    foreach (SVC.Recept item in listrecept)
                                    {
                                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                                        {
                                            item.quantite = -1 * item.quantite;

                                        }
                                        foreach (SVC.Recouf itemrecouf in listrecouf)
                                        {
                                           
                                            if (item.nfact.Trim() == itemrecouf.nfact.Trim() && item.cf == itemrecouf.codef)
                                            {
                                                receptcalcule t = new receptcalcule
                                                {
                                                    Id = item.Id,
                                                    cf = item.cf,
                                                    codeprod = item.codeprod,
                                                    collisage = item.collisage,
                                                    date = item.date,
                                                    dates = item.dates,
                                                    design = item.design,
                                                    Fiscale = itemrecouf.fiscal,
                                                    Fournisseur = item.Fournisseur,
                                                    lot = item.lot,
                                                    nfact = item.nfact,
                                                    NonFiscale = itemrecouf.Nonfiscal,
                                                    perempt = item.perempt,
                                                    previent = item.previent,
                                                    quantite = item.quantite,
                                                    tva = item.tva,
                                                    Colisagequantite = item.quantite * item.collisage,
                                                };
                                                ///  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(t.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                listfinal.Add(t);

                                            }
                                        }
                                        
                                    }
                                    foreach (receptcalcule item in listfinal)
                                    {
                                        foreach (SVC.Prodf itemprodf in listprof)
                                        {
                                            if (itemprodf.cp == item.codeprod && itemprodf.nfact == item.nfact)
                                            {
                                                item.quantitestock = itemprodf.quantite;
                                                item.previentcomb = itemprodf.previent;
                                                /*     var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                                     //objectmodifed = listMembershipOptic;
                                                     var index = LISTITEM0.IndexOf(objectmodifed);
                                                     if (index != -1)
                                                         LISTITEM0[index] = listMembershipOptic;*/
                                            }
                                        }
                                    }

                                   
                                    ReceptDatagrid.ItemsSource = listfinal;
                                    //n.cp == item.codeprod && n.nfact == item.nfact

                                }
                                else
                                {
                                    List<SVC.Recouf> listrecouf = proxy.GetAllRecouf().Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.codef == selectedfourn.Id).ToList();
                                    List<SVC.Recept> listrecept = (proxy.GetAllRecept()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.cf == selectedfourn.Id).ToList();
                                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.datef >= DateSaisieDébut.SelectedDate && n.datef <= DateSaisieFin.SelectedDate && n.cf == selectedfourn.Id).ToList();
                                    List<receptcalcule> listfinal = new List<receptcalcule>();
                                    foreach (SVC.Recept item in listrecept)
                                    {
                                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                                        {
                                            item.quantite = -1 * item.quantite;
                                        }
                                        foreach (SVC.Recouf itemrecouf in listrecouf)
                                        {
                                            if (item.nfact.Trim() == itemrecouf.nfact.Trim() && item.cf == itemrecouf.codef)
                                            {
                                                receptcalcule t = new receptcalcule
                                                {
                                                    Id = item.Id,
                                                    cf = item.cf,
                                                    codeprod = item.codeprod,
                                                    collisage = item.collisage,
                                                    date = item.date,
                                                    dates = item.dates,
                                                    design = item.design,
                                                    Fiscale = itemrecouf.fiscal,
                                                    Fournisseur = item.Fournisseur,
                                                    lot = item.lot,
                                                    nfact = item.nfact,
                                                    NonFiscale = itemrecouf.Nonfiscal,
                                                    perempt = item.perempt,
                                                    previent = item.previent,
                                                    quantite = item.quantite,
                                                    tva = item.tva,
                                                    Colisagequantite = item.quantite * item.collisage,
                                                };
                                                ///  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(t.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                listfinal.Add(t);

                                            }
                                        }
                                       
                                    }
                                    foreach (receptcalcule item in listfinal)
                                    {
                                        foreach (SVC.Prodf itemprodf in listprof)
                                        {
                                            if (itemprodf.cp == item.codeprod && itemprodf.nfact == item.nfact)
                                            {
                                                item.quantitestock = itemprodf.quantite;
                                                item.previentcomb = itemprodf.previent;
                                                /*     var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                                     //objectmodifed = listMembershipOptic;
                                                     var index = LISTITEM0.IndexOf(objectmodifed);
                                                     if (index != -1)
                                                         LISTITEM0[index] = listMembershipOptic;*/
                                            }
                                        }
                                    }
                                    
                                    ReceptDatagrid.ItemsSource = listfinal;
                                    //n.cp == item.codeprod && n.nfact == item.nfact                            }


                                }
                                break;
                            case "Facture Fiscale":
                                SVC.Fourn selectedfourn1 = FournisseurCombo.SelectedItem as SVC.Fourn;
                                if (FournisseurCombo.SelectedIndex == 0)
                                {
                                    List<SVC.Recouf> listrecouf = proxy.GetAllRecouf().Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.fiscal == true).ToList();
                                    List<SVC.Recept> listrecept = (proxy.GetAllRecept()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate).ToList();
                                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.datef >= DateSaisieDébut.SelectedDate && n.datef <= DateSaisieFin.SelectedDate).ToList();
                                    List<receptcalcule> listfinal = new List<receptcalcule>();
                                    foreach (SVC.Recept item in listrecept)
                                    {
                                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                                        {
                                            item.quantite = -1 * item.quantite;
                                        }
                                        foreach (SVC.Recouf itemrecouf in listrecouf)
                                        {
                                            if (item.nfact.Trim() == itemrecouf.nfact.Trim() && item.cf == itemrecouf.codef)
                                            {
                                                receptcalcule t = new receptcalcule
                                                {
                                                    Id = item.Id,
                                                    cf = item.cf,
                                                    codeprod = item.codeprod,
                                                    collisage = item.collisage,
                                                    date = item.date,
                                                    dates = item.dates,
                                                    design = item.design,
                                                    Fiscale = itemrecouf.fiscal,
                                                    Fournisseur = item.Fournisseur,
                                                    lot = item.lot,
                                                    nfact = item.nfact,
                                                    NonFiscale = itemrecouf.Nonfiscal,
                                                    perempt = item.perempt,
                                                    previent = item.previent,
                                                    quantite = item.quantite,
                                                    tva = item.tva,
                                                    Colisagequantite = item.quantite * item.collisage,
                                                };
                                                ///  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(t.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                listfinal.Add(t);

                                            }
                                        }
                                       
                                    }
                                    foreach (receptcalcule item in listfinal)
                                    {
                                        foreach (SVC.Prodf itemprodf in listprof)
                                        {
                                            if (itemprodf.cp == item.codeprod && itemprodf.nfact == item.nfact)
                                            {
                                                item.quantitestock = itemprodf.quantite;
                                                item.previentcomb = itemprodf.previent;
                                                /*     var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                                     //objectmodifed = listMembershipOptic;
                                                     var index = LISTITEM0.IndexOf(objectmodifed);
                                                     if (index != -1)
                                                         LISTITEM0[index] = listMembershipOptic;*/
                                            }
                                        }
                                    }
                                     
                                    ReceptDatagrid.ItemsSource = listfinal;
                                    //n.cp == item.codeprod && n.nfact == item.nfact

                                }
                                else
                                {
                                    List<SVC.Recouf> listrecouf = proxy.GetAllRecouf().Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.codef == selectedfourn1.Id && n.fiscal == true).ToList();
                                    List<SVC.Recept> listrecept = (proxy.GetAllRecept()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.cf == selectedfourn1.Id).ToList();
                                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.datef >= DateSaisieDébut.SelectedDate && n.datef <= DateSaisieFin.SelectedDate && n.cf == selectedfourn1.Id).ToList();
                                    List<receptcalcule> listfinal = new List<receptcalcule>();
                                    foreach (SVC.Recept item in listrecept)
                                    {
                                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                                        {
                                            item.quantite = -1 * item.quantite;
                                        }
                                        foreach (SVC.Recouf itemrecouf in listrecouf)
                                        {
                                            if (item.nfact.Trim() == itemrecouf.nfact.Trim() && item.cf == itemrecouf.codef)
                                            {
                                                receptcalcule t = new receptcalcule
                                                {
                                                    Id = item.Id,
                                                    cf = item.cf,
                                                    codeprod = item.codeprod,
                                                    collisage = item.collisage,
                                                    date = item.date,
                                                    dates = item.dates,
                                                    design = item.design,
                                                    Fiscale = itemrecouf.fiscal,
                                                    Fournisseur = item.Fournisseur,
                                                    lot = item.lot,
                                                    nfact = item.nfact,
                                                    NonFiscale = itemrecouf.Nonfiscal,
                                                    perempt = item.perempt,
                                                    previent = item.previent,
                                                    quantite = item.quantite,
                                                    tva = item.tva,
                                                    Colisagequantite = item.quantite * item.collisage,
                                                };
                                                ///  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(t.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                listfinal.Add(t);

                                            }
                                        }
                                       
                                    }
                                    foreach (receptcalcule item in listfinal)
                                    {
                                        foreach (SVC.Prodf itemprodf in listprof)
                                        {
                                            if (itemprodf.cp == item.codeprod && itemprodf.nfact == item.nfact)
                                            {
                                                item.quantitestock = itemprodf.quantite;
                                                item.previentcomb = itemprodf.previent;
                                                /*     var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                                     //objectmodifed = listMembershipOptic;
                                                     var index = LISTITEM0.IndexOf(objectmodifed);
                                                     if (index != -1)
                                                         LISTITEM0[index] = listMembershipOptic;*/
                                            }
                                        }
                                    }
                                    
                                    ReceptDatagrid.ItemsSource = listfinal;
                                    //n.cp == item.codeprod && n.nfact == item.nfact                            }


                                }
                                break;
                            case "Facture sans T.V.A":
                                SVC.Fourn selectedfourn2 = FournisseurCombo.SelectedItem as SVC.Fourn;
                                if (FournisseurCombo.SelectedIndex == 0)
                                {
                                    List<SVC.Recouf> listrecouf = proxy.GetAllRecouf().Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.Nonfiscal == true).ToList();
                                    List<SVC.Recept> listrecept = (proxy.GetAllRecept()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate).ToList();
                                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.datef >= DateSaisieDébut.SelectedDate && n.datef <= DateSaisieFin.SelectedDate).ToList();
                                    List<receptcalcule> listfinal = new List<receptcalcule>();
                                    foreach (SVC.Recept item in listrecept)
                                    {
                                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                                        {
                                            item.quantite = -1 * item.quantite;
                                        }
                                        foreach (SVC.Recouf itemrecouf in listrecouf)
                                        {
                                            if (item.nfact.Trim() == itemrecouf.nfact.Trim() && item.cf == itemrecouf.codef)
                                            {
                                                receptcalcule t = new receptcalcule
                                                {
                                                    Id = item.Id,
                                                    cf = item.cf,
                                                    codeprod = item.codeprod,
                                                    collisage = item.collisage,
                                                    date = item.date,
                                                    dates = item.dates,
                                                    design = item.design,
                                                    Fiscale = itemrecouf.fiscal,
                                                    Fournisseur = item.Fournisseur,
                                                    lot = item.lot,
                                                    nfact = item.nfact,
                                                    NonFiscale = itemrecouf.Nonfiscal,
                                                    perempt = item.perempt,
                                                    previent = item.previent,
                                                    quantite = item.quantite,
                                                    tva = item.tva,
                                                    Colisagequantite = item.quantite * item.collisage,
                                                };
                                                ///  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(t.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                listfinal.Add(t);

                                            }
                                        }
                                       
                                    }
                                    foreach (receptcalcule item in listfinal)
                                    {
                                        foreach (SVC.Prodf itemprodf in listprof)
                                        {
                                            if (itemprodf.cp == item.codeprod && itemprodf.nfact == item.nfact)
                                            {
                                                item.quantitestock = itemprodf.quantite;
                                                item.previentcomb = itemprodf.previent;
                                                /*     var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                                     //objectmodifed = listMembershipOptic;
                                                     var index = LISTITEM0.IndexOf(objectmodifed);
                                                     if (index != -1)
                                                         LISTITEM0[index] = listMembershipOptic;*/
                                            }
                                        }
                                    }
                                    
                                    ReceptDatagrid.ItemsSource = listfinal;
                                    //n.cp == item.codeprod && n.nfact == item.nfact

                                }
                                else
                                {
                                    List<SVC.Recouf> listrecouf = proxy.GetAllRecouf().Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.codef == selectedfourn2.Id && n.Nonfiscal == true).ToList();
                                    List<SVC.Recept> listrecept = (proxy.GetAllRecept()).Where(n => n.date >= DateSaisieDébut.SelectedDate && n.date <= DateSaisieFin.SelectedDate && n.cf == selectedfourn2.Id).ToList();
                                    List<SVC.Prodf> listprof = (proxy.GetAllProdf()).Where(n => n.datef >= DateSaisieDébut.SelectedDate && n.datef <= DateSaisieFin.SelectedDate && n.cf == selectedfourn2.Id).ToList();
                                    List<receptcalcule> listfinal = new List<receptcalcule>();
                                    foreach (SVC.Recept item in listrecept)
                                    {
                                        if ((item.FicheProdfAvoir >= 0 || item.FicheProdfAvoir != null))
                                        {
                                            item.quantite = -1 * item.quantite;
                                        }
                                        foreach (SVC.Recouf itemrecouf in listrecouf)
                                        {
                                            if (item.nfact.Trim() == itemrecouf.nfact.Trim() && item.cf == itemrecouf.codef)
                                            {
                                                receptcalcule t = new receptcalcule
                                                {
                                                    Id = item.Id,
                                                    cf = item.cf,
                                                    codeprod = item.codeprod,
                                                    collisage = item.collisage,
                                                    date = item.date,
                                                    dates = item.dates,
                                                    design = item.design,
                                                    Fiscale = itemrecouf.fiscal,
                                                    Fournisseur = item.Fournisseur,
                                                    lot = item.lot,
                                                    nfact = item.nfact,
                                                    NonFiscale = itemrecouf.Nonfiscal,
                                                    perempt = item.perempt,
                                                    previent = item.previent,
                                                    quantite = item.quantite,
                                                    tva = item.tva,
                                                    Colisagequantite = item.quantite * item.collisage,
                                                };
                                                ///  MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(t.design.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                                                listfinal.Add(t);

                                            }
                                        }
                                        
                                    }
                                    foreach (receptcalcule item in listfinal)
                                    {
                                        foreach (SVC.Prodf itemprodf in listprof)
                                        {
                                            if (itemprodf.cp == item.codeprod && itemprodf.nfact == item.nfact)
                                            {
                                                item.quantitestock = itemprodf.quantite;
                                                item.previentcomb = itemprodf.previent;
                                                /*     var objectmodifed = LISTITEM0.Find(n => n.Id == listMembershipOptic.Id);
                                                     //objectmodifed = listMembershipOptic;
                                                     var index = LISTITEM0.IndexOf(objectmodifed);
                                                     if (index != -1)
                                                         LISTITEM0[index] = listMembershipOptic;*/
                                            }
                                        }
                                    }
                                    
                                    ReceptDatagrid.ItemsSource = listfinal;
                                    //n.cp == item.codeprod && n.nfact == item.nfact                            }


                                }
                                break;
                        }
                        interfaceimpression = 2;
                        var test = ReceptDatagrid.ItemsSource as IEnumerable<receptcalcule>;
                        txtstock.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.previentcomb * o.quantitestock));
                        var dec = ((test).AsEnumerable().Sum(o => (o.previent * o.quantite) + ((o.previent * o.quantite) * o.tva / 100)));
                        Txtachats.Text = Convert.ToString(Math.Truncate(Convert.ToDecimal(dec)));
                        txtFiche.Text = Convert.ToString(((test).AsEnumerable().Count()));

                    }
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

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MembershipOptic.ImpressionAchat == true && ReceptDatagrid.Items.Count > 0 && DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null)
                {
                   
                    var itemsSource0 = ReceptDatagrid.Items.OfType<receptcalcule>();// List<SVC.SalleDattente>;
                    List<receptcalcule> itemsSource1 = new List<receptcalcule>();
                    foreach (receptcalcule item in itemsSource0)
                    {
                        itemsSource1.Add(item);
                    }
                
                    ImpressionListeAchats cl = new ImpressionListeAchats (proxy, itemsSource1, DateSaisieDébut.SelectedDate.Value, DateSaisieFin.SelectedDate.Value, txtFiche.Text,Txtachats.Text, txtstock.Text);

                    cl.Show();
                }
            }
            catch (Exception ex)
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
                ICollectionView cv = CollectionViewSource.GetDefaultView(ReceptDatagrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        receptcalcule p = o as receptcalcule;
                        if (t.Name == "txtId")
                            return (p.design == filter);
                        return (p.design.ToUpper().Contains(filter.ToUpper()));
                    };
                }
                var test = ReceptDatagrid.Items.OfType<receptcalcule>();
                txtstock.Text = Convert.ToString((test).AsEnumerable().Sum(o => o.previentcomb * o.quantitestock));
                var dec = ((test).AsEnumerable().Sum(o => (o.previent * o.quantite) + ((o.previent * o.quantite) * o.tva / 100)));
                Txtachats.Text = Convert.ToString(Math.Truncate(Convert.ToDecimal(dec)));
                txtFiche.Text = Convert.ToString(((test).AsEnumerable().Count()));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
   public class receptcalcule:SVC.Recept
    {
        public Nullable<decimal> quantitestock { get; set; }
        public Nullable<bool> Fiscale { get; set; }
        public Nullable<bool> NonFiscale { get; set; }
        public Nullable<decimal> Colisagequantite { get; set; }
        public Nullable<decimal> previentcomb { get; set; }
    }

}
