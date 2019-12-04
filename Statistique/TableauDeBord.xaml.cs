
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewOptics.Statistique
{
    /// <summary>
    /// Interaction logic for TableauDeBord.xaml
    /// </summary>
    public partial class TableauDeBord : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.MembershipOptic MembershipOptic;
        SVC.Param selectedparam;
        private delegate void FaultedInvokerTableauDeBord();
        bool filtre = false;
        int interfacefilre = 0;
        TableauDeBordClass tableau;
        public TableauDeBord(SVC.ServiceCliniqueClient proxyrecu, ICallback callbackrecu, SVC.MembershipOptic memberrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
              
                MembershipOptic = memberrecu;
                DateSaisieFin.SelectedDate = DateTime.Now;
                DateSaisieDébut.SelectedDate = DateTime.Now;
                tableau = new TableauDeBordClass
                {
                    Nombre = 0,
                    TotalCA = 0,
                    Date1 = DateSaisieDébut.SelectedDate.Value,
                    Date2 = DateSaisieFin.SelectedDate.Value,
                    TotalConsomm = 0,
                    TotalMarge = 0,
                    TotalMoyen = 0,
                    RemiseClient = 0,
                    RemiseFournisseur =0,
                };
                GridResume.DataContext = tableau;


                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerTableauDeBord(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerTableauDeBord(HandleProxy));
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
                        TableauDeBordList p = o as TableauDeBordList;
                        if (t.Name == "txtId")
                            return (p.Code == Convert.ToInt32(filter));
                        return (p.Libelle.ToUpper().Contains(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

  /*      private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateSaisieDébut.SelectedDate!=null && DateSaisieFin.SelectedDate!=null && MembershipOptic.ModuleStatistiqueAcces==true)
                {
                    List<TableauDeBordList> listtableau = new List<TableauDeBordList>();

                    if (RadioFournisseur.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P");

                      
                            foreach (var item in ll)
                            {

                                if (item.nfact.Substring(0, 2) == "Co")
                                {
                                    bool existe = listtableau.Any(n => n.Code == item.cf);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == item.cf);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        found.Taux = (found.Bénéfice / found.CA) * 100;

                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = item.Fournisseur,
                                            Code = Convert.ToInt16(item.cf),
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == item.cf);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == item.cf);
                                            found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                            found.Taux = (found.Bénéfice / found.CA) * 100;

                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = item.Fournisseur,
                                                Code = Convert.ToInt16(item.cf),
                                                Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                    else
                                    {
                                        if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                        {
                                            bool existe = listtableau.Any(n => n.Code == item.cf);
                                            if (existe == true)
                                            {
                                                var found = listtableau.Find(n => n.Code == item.cf);
                                                found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                                found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                                found.Libelle = found.Libelle;
                                                found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                                found.Taux = (found.Bénéfice / found.CA) * 100;

                                            }
                                            else
                                            {
                                                TableauDeBordList newt = new TableauDeBordList
                                                {
                                                    Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                    CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                    Libelle = item.Fournisseur,
                                                    Code = Convert.ToInt16(item.cf),
                                                    Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                    Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                                };
                                                listtableau.Add(newt);
                                            }
                                        }
                                    }




                                }
                            }



                            ReceptDatagrid.ItemsSource = listtableau;

                            Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                            Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                            // Decimal remiseclient = 0;
                            //     Decimal remisefournisseur = 0;
                            Decimal tauxmoyen = 0;
                            if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                            {
                                tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                            }
                            else
                            {
                                tauxmoyen = 0;
                            }


                           tableau = new TableauDeBordClass
                            {
                                Nombre = listtableau.Count(),
                                TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                                Date1 = DateSaisieDébut.SelectedDate.Value,
                                Date2 = DateSaisieFin.SelectedDate.Value,
                                TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                                TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                                TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                                RemiseClient = remiseclient,
                                RemiseFournisseur = remisefournisseur,
                            };
                            GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 1;
                    }
                    else if (RadioClient.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P");


                        foreach (var item in ll)
                        {

                            if (item.nfact.Substring(0, 2) == "Co")
                            {
                                bool existe = listtableau.Any(n => n.Code == item.codeclient);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == item.codeclient);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    found.Taux = (found.Bénéfice / found.CA) * 100;

                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = item.Client,
                                        Code = Convert.ToInt16(item.codeclient),
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                {
                                    bool existe = listtableau.Any(n => n.Code == item.codeclient);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == item.codeclient);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        found.Taux = (found.Bénéfice / found.CA) * 100;

                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = item.Client,
                                            Code = Convert.ToInt16(item.codeclient),
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == item.codeclient);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == item.codeclient);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            found.Taux = (found.Bénéfice / found.CA) * 100;

                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = item.Client,
                                                Code = Convert.ToInt16(item.codeclient),
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                }




                            }
                        }



                        ReceptDatagrid.ItemsSource = listtableau;

                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 2;
                    }
                    else if (RadioProduit.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n=>n.nfact.Substring(1,1)!="R" && n.nfact.Substring(1, 1) != "P");

                       
                            foreach (var item in ll)
                            {

                                if (item.nfact.Substring(0, 2) == "Co")
                                {
                                    bool existe = listtableau.Any(n => n.Code == item.codeprod);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == item.codeprod);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        found.Taux = (found.Bénéfice / found.CA) * 100;

                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = item.design,
                                            Code = Convert.ToInt16(item.codeprod),
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == item.codeprod);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == item.codeprod);
                                            found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                            found.Taux = (found.Bénéfice / found.CA) * 100;

                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = item.design,
                                                Code = Convert.ToInt16(item.codeprod),
                                                Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                    else
                                    {
                                        if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                        {
                                            bool existe = listtableau.Any(n => n.Code == item.codeprod);
                                            if (existe == true)
                                            {
                                                var found = listtableau.Find(n => n.Code == item.codeprod);
                                                found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                                found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                                found.Libelle = found.Libelle;
                                                found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                                found.Taux = (found.Bénéfice / found.CA) * 100;

                                            }
                                            else
                                            {
                                                TableauDeBordList newt = new TableauDeBordList
                                                {
                                                    Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                    CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                    Libelle = item.design,
                                                    Code = Convert.ToInt16(item.codeprod),
                                                    Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                    Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                                };
                                                listtableau.Add(newt);
                                            }
                                        }
                                    }




                                }
                            }



                            ReceptDatagrid.ItemsSource = listtableau;

                           Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                            Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                            // Decimal remiseclient = 0;
                              //     Decimal remisefournisseur = 0;
                               Decimal tauxmoyen = 0;
                               if(listtableau.AsEnumerable().Sum(n => n.CA)!=0)
                               {
                                   tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                               }
                               else
                               {
                                 tauxmoyen = 0;
                               }


                              tableau = new TableauDeBordClass
                               {
                                   Nombre = listtableau.Count(),
                                   TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                                   Date1 = DateSaisieDébut.SelectedDate.Value,
                                   Date2 = DateSaisieFin.SelectedDate.Value,
                                   TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                                   TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                                   TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                                   RemiseClient = remiseclient,
                                   RemiseFournisseur = remisefournisseur,
                               };
                               GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 3;
                    }
                    else if (RadioFamille.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P");
                        foreach (var item in ll)
                        {
                            int codefamille = 0;
                            string famille = "";
                            var familleexistebool = (proxy.GetAllProduitbyid(Convert.ToInt16(item.codeprod)).Any());
                            if (familleexistebool== true)
                            {
                                var familleexiste = (proxy.GetAllProduitbyid(Convert.ToInt16(item.codeprod))).First();

                                if (familleexiste.IdFamille != 0)
                                {
                                    codefamille = Convert.ToInt16(familleexiste.IdFamille);
                                    famille = familleexiste.famille;
                                }
                                else
                                {
                                    codefamille = 0;
                                    famille = "";
                                }
                            }
                            else
                            {
                                codefamille = 0;
                                famille = "";
                            }
                            if (item.nfact.Substring(0, 2) == "Co")///code=0 Libelle=Vente Comptoir
                            {
                                bool existe = listtableau.Any(n => n.Code == codefamille);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == codefamille);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    found.Taux = (found.Bénéfice / found.CA) * 100;

                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = famille,
                                        Code = codefamille,
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                {
                                    bool existe = listtableau.Any(n => n.Code == codefamille);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == codefamille);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        found.Taux = (found.Bénéfice / found.CA) * 100;

                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = famille,
                                            Code = codefamille,
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == codefamille);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == codefamille);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            found.Taux = (found.Bénéfice / found.CA) * 100;

                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = famille,
                                                Code = codefamille,
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                }




                            }
                        }
                        ReceptDatagrid.ItemsSource = listtableau;
                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 4;
                    }
                    else if (RadioDocument.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P");
                       
                            foreach (var item in ll)
                        {

                            if (item.nfact.Substring(0, 2) == "Co")///code=0 Libelle=Vente Comptoir
                            {
                                bool existe = listtableau.Any(n => n.Code == 0);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == 0);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    found.Taux = (found.Bénéfice / found.CA) * 100;

                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = "Vente Comptoir",
                                        Code = 0,
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" /*|| item.nfact.Substring(0, 1) == "B"*///)
                               // {
                        /*            bool existe = listtableau.Any(n => n.Code == 1);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == 1);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        found.Taux = (found.Bénéfice / found.CA) * 100;

                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = "Facture",
                                            Code = 1,
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if (/*(item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || *///item.nfact.Substring(0, 1) == "A")
                          /*          {
                                        bool existe = listtableau.Any(n => n.Code == 2);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == 2);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            found.Taux = (found.Bénéfice / found.CA) * 100;

                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = "Avoir de Facture",
                                                Code =2,
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                    else
                                    {
                                        if ( item.nfact.Substring(0, 1) == "B")
                                        {
                                            bool existe = listtableau.Any(n => n.Code == 3);
                                            if (existe == true)
                                            {
                                                var found = listtableau.Find(n => n.Code ==3);
                                                found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                                found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                                found.Libelle = found.Libelle;
                                                found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                                found.Taux = (found.Bénéfice / found.CA) * 100;

                                            }
                                            else
                                            {
                                                TableauDeBordList newt = new TableauDeBordList
                                                {
                                                    Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                    CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                                    Libelle = "Bon de Livraison",
                                                    Code = 3,
                                                    Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                                    Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                                };
                                                listtableau.Add(newt);
                                            }
                                        }else
                                        {
                                            if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") )
                                            {
                                                bool existe = listtableau.Any(n => n.Code == 4);
                                                if (existe == true)
                                                {
                                                    var found = listtableau.Find(n => n.Code == 4);
                                                    found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                                    found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                                    found.Libelle = found.Libelle;
                                                    found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                                    found.Taux = (found.Bénéfice / found.CA) * 100;

                                                }
                                                else
                                                {
                                                    TableauDeBordList newt = new TableauDeBordList
                                                    {
                                                        Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                        CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                        Libelle = "Avoir de Bon de Livraison",
                                                        Code =4,
                                                        Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                        Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                                    };
                                                    listtableau.Add(newt);
                                                }
                                            }
                                        }
                                    }
                                }




                            }
                        }



                        ReceptDatagrid.ItemsSource = listtableau;

                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                         tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 5;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }*/
                private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateSaisieDébut.SelectedDate != null && DateSaisieFin.SelectedDate != null && MembershipOptic.ModuleStatistiqueAcces == true)
                {
                    List<TableauDeBordList> listtableau = new List<TableauDeBordList>();

                    if (RadioFournisseur.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P" );


                        foreach (var item in ll)
                        {

                            if (item.nfact.Substring(0, 2) == "Co")
                            {
                                bool existe = listtableau.Any(n => n.Code == item.cf);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == item.cf);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    if (found.CA != 0)
                                    {
                                        found.Taux = (found.Bénéfice / found.CA) * 100;
                                    }
                                    else
                                    {
                                        found.CA = 0;
                                        found.Taux = 0;
                                    }


                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = item.Fournisseur,
                                        Code = Convert.ToInt16(item.cf),
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                {
                                    bool existe = listtableau.Any(n => n.Code == item.cf);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == item.cf);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        //found.Taux = (found.Bénéfice / found.CA) * 100;
                                        if (found.CA != 0)
                                        {
                                            found.Taux = (found.Bénéfice / found.CA) * 100;
                                        }
                                        else
                                        {
                                            found.CA = 0;
                                            found.Taux = 0;
                                        }
                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = item.Fournisseur,
                                            Code = Convert.ToInt16(item.cf),
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == item.cf);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == item.cf);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            //   found.Taux = (found.Bénéfice / found.CA) * 100;
                                            if (found.CA != 0)
                                            {
                                                found.Taux = (found.Bénéfice / found.CA) * 100;
                                            }
                                            else
                                            {
                                                found.CA = 0;
                                                found.Taux = 0;
                                            }
                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = item.Fournisseur,
                                                Code = Convert.ToInt16(item.cf),
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                }




                            }
                        }



                        ReceptDatagrid.ItemsSource = listtableau;

                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 1;
                    }
                    else if (RadioClient.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P" );


                        foreach (var item in ll)
                        {

                            if (item.nfact.Substring(0, 2) == "Co")
                            {
                                bool existe = listtableau.Any(n => n.Code == item.codeclient);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == item.codeclient);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));

                                    // found.Taux = (found.Bénéfice / found.CA) * 100;
                                    if (found.CA != 0)
                                    {
                                        found.Taux = (found.Bénéfice / found.CA) * 100;
                                    }
                                    else
                                    {
                                        found.CA = 0;
                                        found.Taux = 0;
                                    }
                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = item.Client,
                                        Code = Convert.ToInt16(item.codeclient),
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                {
                                    bool existe = listtableau.Any(n => n.Code == item.codeclient);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == item.codeclient);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        // found.Taux = (found.Bénéfice / found.CA) * 100;
                                        if (found.CA != 0)
                                        {
                                            found.Taux = (found.Bénéfice / found.CA) * 100;
                                        }
                                        else
                                        {
                                            found.CA = 0;
                                            found.Taux = 0;
                                        }
                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = item.Client,
                                            Code = Convert.ToInt16(item.codeclient),
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == item.codeclient);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == item.codeclient);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                            if (found.CA != 0)
                                            {
                                                found.Taux = (found.Bénéfice / found.CA) * 100;
                                            }
                                            else
                                            {
                                                found.CA = 0;
                                                found.Taux = 0;
                                            }
                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = item.Client,
                                                Code = Convert.ToInt16(item.codeclient),
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                }




                            }
                        }



                        ReceptDatagrid.ItemsSource = listtableau;

                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 2;
                    }
                    else if (RadioProduit.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P" );


                        foreach (var item in ll)
                        {

                            if (item.nfact.Substring(0, 2) == "Co")
                            {
                                bool existe = listtableau.Any(n => n.Code == item.codeprod);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == item.codeprod);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    if (found.CA != 0)
                                    {
                                        found.Taux = (found.Bénéfice / found.CA) * 100;
                                    }
                                    else
                                    {
                                        found.CA = 0;
                                        found.Taux = 0;
                                    }
                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = item.design,
                                        Code = Convert.ToInt16(item.codeprod),
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                {
                                    bool existe = listtableau.Any(n => n.Code == item.codeprod);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == item.codeprod);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                        if (found.CA != 0)
                                        {
                                            found.Taux = (found.Bénéfice / found.CA) * 100;
                                        }
                                        else
                                        {
                                            found.CA = 0;
                                            found.Taux = 0;
                                        }
                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = item.design,
                                            Code = Convert.ToInt16(item.codeprod),
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == item.codeprod);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == item.codeprod);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                            if (found.CA != 0)
                                            {
                                                found.Taux = (found.Bénéfice / found.CA) * 100;
                                            }
                                            else
                                            {
                                                found.CA = 0;
                                                found.Taux = 0;
                                            }
                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = item.design,
                                                Code = Convert.ToInt16(item.codeprod),
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                }




                            }
                        }



                        ReceptDatagrid.ItemsSource = listtableau;

                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 3;
                    }
                    else if (RadioFamille.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P" );
                        foreach (var item in ll)
                        {
                            int codefamille = 0;
                            string famille = "";
                            var familleexistebool = (proxy.GetAllProduitbyid(Convert.ToInt16(item.codeprod)).Any());
                            if (familleexistebool == true)
                            {
                                var familleexiste = (proxy.GetAllProduitbyid(Convert.ToInt16(item.codeprod))).First();

                                if (familleexiste.IdFamille != 0)
                                {
                                    codefamille = Convert.ToInt16(familleexiste.IdFamille);
                                    famille = familleexiste.famille;
                                }
                                else
                                {
                                    codefamille = 0;
                                    famille = "";
                                }
                            }
                            else
                            {
                                codefamille = 0;
                                famille = "";
                            }
                            if (item.nfact.Substring(0, 2) == "Co")///code=0 Libelle=Vente Comptoir
                            {
                                bool existe = listtableau.Any(n => n.Code == codefamille);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == codefamille);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    // found.Taux = (found.Bénéfice / found.CA) * 100;
                                    if (found.CA != 0)
                                    {
                                        found.Taux = (found.Bénéfice / found.CA) * 100;
                                    }
                                    else
                                    {
                                        found.CA = 0;
                                        found.Taux = 0;
                                    }
                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = famille,
                                        Code = codefamille,
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" || item.nfact.Substring(0, 1) == "B")
                                {
                                    bool existe = listtableau.Any(n => n.Code == codefamille);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == codefamille);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                        if (found.CA != 0)
                                        {
                                            found.Taux = (found.Bénéfice / found.CA) * 100;
                                        }
                                        else
                                        {
                                            found.CA = 0;
                                            found.Taux = 0;
                                        }
                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = famille,
                                            Code = codefamille,
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == codefamille);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == codefamille);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                            if (found.CA != 0)
                                            {
                                                found.Taux = (found.Bénéfice / found.CA) * 100;
                                            }
                                            else
                                            {
                                                found.CA = 0;
                                                found.Taux = 0;
                                            }
                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = famille,
                                                Code = codefamille,
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                }




                            }
                        }
                        ReceptDatagrid.ItemsSource = listtableau;
                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 4;
                    }
                    else if (RadioDocument.IsChecked == true)
                    {
                        var ll = proxy.GetAllFacture(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P" );

                        foreach (var item in ll)
                        {

                            if (item.nfact.Substring(0, 2) == "Co")///code=0 Libelle=Vente Comptoir
                            {
                                bool existe = listtableau.Any(n => n.Code == 0);
                                if (existe == true)
                                {
                                    var found = listtableau.Find(n => n.Code == 0);
                                    found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                    found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                    found.Libelle = found.Libelle;
                                    found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                    // found.Taux = (found.Bénéfice / found.CA) * 100;
                                    if (found.CA != 0)
                                    {
                                        found.Taux = (found.Bénéfice / found.CA) * 100;
                                    }
                                    else
                                    {
                                        found.CA = 0;
                                        found.Taux = 0;
                                    }
                                }
                                else
                                {
                                    TableauDeBordList newt = new TableauDeBordList
                                    {
                                        Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                        CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                        Libelle = "Vente Comptoir",
                                        Code = 0,
                                        Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                        Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                    };
                                    listtableau.Add(newt);
                                }
                            }
                            else
                            {
                                if (item.nfact.Substring(0, 1) == "F" /*|| item.nfact.Substring(0, 1) == "B"*/)
                                {
                                    bool existe = listtableau.Any(n => n.Code == 1);
                                    if (existe == true)
                                    {
                                        var found = listtableau.Find(n => n.Code == 1);
                                        found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                        found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                        found.Libelle = found.Libelle;
                                        found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                        //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                        if (found.CA != 0)
                                        {
                                            found.Taux = (found.Bénéfice / found.CA) * 100;
                                        }
                                        else
                                        {
                                            found.CA = 0;
                                            found.Taux = 0;
                                        }
                                    }
                                    else
                                    {
                                        TableauDeBordList newt = new TableauDeBordList
                                        {
                                            Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                            CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                            Libelle = "Facture",
                                            Code = 1,
                                            Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                            Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                        };
                                        listtableau.Add(newt);
                                    }
                                }
                                else
                                {
                                    if (/*(item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO") || */item.nfact.Substring(0, 1) == "A")
                                    {
                                        bool existe = listtableau.Any(n => n.Code == 2);
                                        if (existe == true)
                                        {
                                            var found = listtableau.Find(n => n.Code == 2);
                                            found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                            found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                            found.Libelle = found.Libelle;
                                            found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                            //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                            if (found.CA != 0)
                                            {
                                                found.Taux = (found.Bénéfice / found.CA) * 100;
                                            }
                                            else
                                            {
                                                found.CA = 0;
                                                found.Taux = 0;
                                            }
                                        }
                                        else
                                        {
                                            TableauDeBordList newt = new TableauDeBordList
                                            {
                                                Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                Libelle = "Avoir de Facture",
                                                Code = 2,
                                                Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                            };
                                            listtableau.Add(newt);
                                        }
                                    }
                                    else
                                    {
                                        if (item.nfact.Substring(0, 1) == "B")
                                        {
                                            bool existe = listtableau.Any(n => n.Code == 3);
                                            if (existe == true)
                                            {
                                                var found = listtableau.Find(n => n.Code == 3);
                                                found.Bénéfice = found.Bénéfice + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                                found.CA = found.CA + Convert.ToDecimal((item.privente * item.quantite));
                                                found.Libelle = found.Libelle;
                                                found.Consommations = found.Consommations + Convert.ToDecimal((item.previent * item.quantite));
                                                //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                                if (found.CA != 0)
                                                {
                                                    found.Taux = (found.Bénéfice / found.CA) * 100;
                                                }
                                                else
                                                {
                                                    found.CA = 0;
                                                    found.Taux = 0;
                                                }
                                            }
                                            else
                                            {
                                                TableauDeBordList newt = new TableauDeBordList
                                                {
                                                    Bénéfice = 0 + Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                    CA = 0 + Convert.ToDecimal((item.privente * item.quantite)),
                                                    Libelle = "Bon de Livraison",
                                                    Code = 3,
                                                    Consommations = 0 + Convert.ToDecimal((item.previent * item.quantite)),
                                                    Taux = 0 + ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                                };
                                                listtableau.Add(newt);
                                            }
                                        }
                                        else
                                        {
                                            if ((item.nfact.Substring(0, 1) == "C" && item.nfact.Substring(0, 1) != "CO"))
                                            {
                                                bool existe = listtableau.Any(n => n.Code == 4);
                                                if (existe == true)
                                                {
                                                    var found = listtableau.Find(n => n.Code == 4);
                                                    found.Bénéfice = found.Bénéfice - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent)));
                                                    found.CA = found.CA - Convert.ToDecimal((item.privente * item.quantite));
                                                    found.Libelle = found.Libelle;
                                                    found.Consommations = found.Consommations - Convert.ToDecimal((item.previent * item.quantite));
                                                    //  found.Taux = (found.Bénéfice / found.CA) * 100;
                                                    if (found.CA != 0)
                                                    {
                                                        found.Taux = (found.Bénéfice / found.CA) * 100;
                                                    }
                                                    else
                                                    {
                                                        found.CA = 0;
                                                        found.Taux = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    TableauDeBordList newt = new TableauDeBordList
                                                    {
                                                        Bénéfice = 0 - Convert.ToDecimal(((item.quantite * item.privente) - (item.quantite * item.previent))),
                                                        CA = 0 - Convert.ToDecimal((item.privente * item.quantite)),
                                                        Libelle = "Avoir de Bon de Livraison",
                                                        Code = 4,
                                                        Consommations = 0 - Convert.ToDecimal((item.previent * item.quantite)),
                                                        Taux = 0 - ((Convert.ToDecimal((item.privente * item.quantite) - (item.previent * item.quantite)) / Convert.ToDecimal(item.quantite * item.privente)) * 100),

                                                    };
                                                    listtableau.Add(newt);
                                                }
                                            }
                                        }
                                    }
                                }




                            }
                        }



                        ReceptDatagrid.ItemsSource = listtableau;

                        Decimal remiseclient = Convert.ToDecimal(proxy.GetAllF1(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).Where(n => n.nfact.Substring(1, 1) != "R" && n.nfact.Substring(1, 1) != "P").AsEnumerable().Sum(n => n.remise));
                        Decimal remisefournisseur = Convert.ToDecimal(proxy.GetAllRecoufbydate(DateSaisieDébut.SelectedDate.Value.Date, DateSaisieFin.SelectedDate.Value.Date).AsEnumerable().Sum(n => n.remise));
                        // Decimal remiseclient = 0;
                        //     Decimal remisefournisseur = 0;
                        Decimal tauxmoyen = 0;
                        if (listtableau.AsEnumerable().Sum(n => n.CA) != 0)
                        {
                            tauxmoyen = (listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100;
                        }
                        else
                        {
                            tauxmoyen = 0;
                        }


                        tableau = new TableauDeBordClass
                        {
                            Nombre = listtableau.Count(),
                            TotalCA = listtableau.AsEnumerable().Sum(n => n.CA),
                            Date1 = DateSaisieDébut.SelectedDate.Value,
                            Date2 = DateSaisieFin.SelectedDate.Value,
                            TotalConsomm = listtableau.AsEnumerable().Sum(n => n.Consommations),
                            TotalMarge = listtableau.AsEnumerable().Sum(n => n.Bénéfice),
                            TotalMoyen = tauxmoyen,//(listtableau.AsEnumerable().Sum(n => n.Bénéfice) / listtableau.AsEnumerable().Sum(n => n.CA)) * 100,
                            RemiseClient = remiseclient,
                            RemiseFournisseur = remisefournisseur,
                        };
                        GridResume.DataContext = tableau;
                        filtre = true;
                        interfacefilre = 5;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnImprimerStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (interfacefilre!=0 && filtre==true)
                {
                    var LISTITEM11 = ReceptDatagrid.ItemsSource as IEnumerable<TableauDeBordList>;

                    ImpressionTableauDeBord cl = new ImpressionTableauDeBord(proxy, tableau,LISTITEM11.ToList(),interfacefilre,DateSaisieDébut.SelectedDate.Value,DateSaisieFin.SelectedDate.Value);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
