using GestionClinique.Administrateur;
using MahApps.Metro.Controls;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Data;
using Microsoft.Win32;
using System.Reflection;
using DevExpress.Xpf.Core;

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for Aliment.xaml
    /// </summary>
    public partial class Aliment : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Membership memberuser;
        ICallback callback;
        private delegate void FaultedInvokerListeAliment();
       
        public Aliment(SVC.ServiceCliniqueClient proxyrecu,SVC.Membership memberrecu,ICallback callbackrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                memberuser = memberrecu;
                callback = callbackrecu;
               FamilleAlimentDataGrid.ItemsSource=proxy.GetAllAliment();
               cbFamille.ItemsSource=proxy.GetAllFamilleAliment();
                callbackrecu.InsertAlimentCallbackevent += new ICallback.CallbackEventHandler28(callbackrecu_Refresh);
                callbackrecu.InsertFamilleAlimentCallbackevent += new ICallback.CallbackEventHandler29(callbackrecufamille_Refresh);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertAliment e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefresh(e.clientleav);
            }));
        }
        public void AddRefresh(List<SVC.Aliment> listmembership)
        {

          FamilleAlimentDataGrid.ItemsSource = listmembership;
        }
        void callbackrecufamille_Refresh(object source, CallbackEventInsertFamilleAliment e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                AddRefreshFamille(e.clientleav);
            }));
        }
        public void AddRefreshFamille(List<SVC.FamilleAliment> listmembership)
        {

            cbFamille.ItemsSource = listmembership;
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeAliment(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeAliment(HandleProxy));
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


        private void FamilleAlimentDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
         if (memberuser.ModificationAdministrateur == true && FamilleAlimentDataGrid.SelectedItem!=null)
            {
                SVC.Aliment selectaliment= FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                InsertAliments cl = new InsertAliments(proxy, memberuser, callback, selectaliment);
                cl.Show();


            }else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if(memberuser.CréationAdministrateur==true)
            {
                InsertAliments cl = new InsertAliments(proxy, memberuser, callback, null);
                cl.Show();


            }
            else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {
           try
            {
               if (memberuser.SuppressionAdministrateur == true && FamilleAlimentDataGrid.SelectedItem != null)
                {
                    bool deletesuccées = false;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                        proxy.DeletAlimentAsync(selectaliment);
                        deletesuccées = true;
                        if (deletesuccées)
                        {
                            ts.Complete();
                            
                        }
                    }
                    proxy.AjouterAlimentFamilleRefresh();
                }
                else
                {
                    MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Opérationéchouée, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
          if (memberuser.ModificationAdministrateur == true && FamilleAlimentDataGrid.SelectedItem != null)
            {
                SVC.Aliment selectaliment = FamilleAlimentDataGrid.SelectedItem as SVC.Aliment;
                InsertAliments cl = new InsertAliments(proxy, memberuser, callback, selectaliment);
                cl.Show();


            }
            else
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.ImpressionAdministrateur == true && cbFamille.SelectedItem != null)
            {
                SVC.FamilleAliment selectaliment = cbFamille.SelectedItem as SVC.FamilleAliment;
                var itemsSource0 = FamilleAlimentDataGrid.ItemsSource as IEnumerable;// List<SVC.SalleDattente>;
                List<SVC.Aliment> itemsSource1 = new List<SVC.Aliment>();
                foreach (SVC.Aliment item in itemsSource0)
                {
                    if (item.FamilleAlimentDesign==selectaliment.FamilleAlimentDesign)
                    {
                        itemsSource1.Add(item);
                    }
                }
                ImpressionListeAlimentByfamille cl = new ImpressionListeAlimentByfamille(proxy, itemsSource1);
                cl.Show();
            }
            else
            {
                if (memberuser.ImpressionAdministrateur == true && cbFamille.SelectedItem == null)
                {
                    var itemsSource0 = FamilleAlimentDataGrid.ItemsSource as IEnumerable;// List<SVC.SalleDattente>;
                    List<SVC.Aliment> itemsSource1 = new List<SVC.Aliment>();
                    foreach (SVC.Aliment item in itemsSource0)
                    {
                        itemsSource1.Add(item);
                    }
                    ImpressionListeAliments cl = new ImpressionListeAliments(proxy, itemsSource1);
                    cl.Show();
                }
            }
        }

        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
           try
            {
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(FamilleAlimentDataGrid.ItemsSource);
                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        SVC.Aliment p = o as SVC.Aliment;
                        if (t.Name == "txtid")
                            return (p.Id == Convert.ToInt32(filter));
                        return (p.DesignProduit.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
           //     this.WindowTitleBrush = Brushes.Red;
            }
        }

        private void cbFamille_DropDownClosed(object sender, EventArgs e)
        {
        if (cbFamille.SelectedItem!= null)

            {
                SVC.FamilleAliment selectaliment = cbFamille.SelectedItem as SVC.FamilleAliment;
             
                string filter = selectaliment.FamilleAlimentDesign;

                
                ICollectionView cv = CollectionViewSource.GetDefaultView(FamilleAlimentDataGrid.ItemsSource);
              
                    cv.Filter = o =>
                    {
                        SVC.Aliment p = o as SVC.Aliment;
                        return (p.FamilleAlimentDesign.ToUpper().Contains(filter.ToUpper()));
                    };
            



            }
        }

      
        private void cbFamille_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
           cbFamille.ItemsSource = proxy.GetAllFamilleAliment();
            FamilleAlimentDataGrid.ItemsSource=proxy.GetAllAliment();
        }

        private void btnVider_Click(object sender, RoutedEventArgs e)
        {
            if (memberuser.SuppressionAdministrateur == true)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Attention ! vous allez vider la table des aliments", "Medicus", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var itemsSource0 = FamilleAlimentDataGrid.ItemsSource as IEnumerable;
                    var itemsSource01 = proxy.GetAllFamilleAliment();
                    if (itemsSource0 != null)
                    {
                        bool succées = false;
                        bool succéesFamille = false;
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            foreach (SVC.Aliment item in itemsSource0)
                            {
                                succées = false;
                                proxy.DeletAliment(item);
                                succées = true;

                            }
                            foreach (SVC.FamilleAliment item in itemsSource01)
                            {
                                succéesFamille = false;
                                proxy.DeletFamilleAliment(item);
                                succéesFamille = true;
                            }
                            if (succées == true && succéesFamille == true)
                            {
                                ts.Complete();
                                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        proxy.AjouterAlimentFamilleRefresh();
                    }
                }

            }
           
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                if (memberuser.CréationAdministrateur == true)
                {
                    OpenFileDialog openfile = new OpenFileDialog();
                    openfile.DefaultExt = ".xlsx";
                    openfile.Filter = "(.xlsx)|*.xlsx";
                    //openfile.ShowDialog();

                    var browsefile = openfile.ShowDialog();

                    if (browsefile == true)
                    {

                        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Open(openfile.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets.get_Item(1); ;
                        Microsoft.Office.Interop.Excel.Range excelRange = excelSheet.UsedRange;
                        int row = 0;
                    List<SVC.Aliment> dt = new List<SVC.Aliment>();
                        //for (row = 2/*2*/; row <= 950/*excelRange.Rows.Count*/; row++)
                            for (row =2; row <= excelRange.Rows.Count; row++)

                            {
                                //   string ig="";
                                bool indicebas = false;
                            bool indicemoy = false;
                            bool indiceelv = false;
                            bool indiceNotDf = false;
                            string ig = (excelRange.Cells[row, 3] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();

                            //Convert.ToDecimal((excelRange.Cells[row, 3] as Microsoft.Office.Interop.Excel.Range).Value2)
                            if ( ig.Trim() == "NC")
                            {

                                ig = "9999";
                                indicebas = false;
                                indicemoy = false;
                                indiceelv = false;
                                indiceNotDf = true;
                            }
                            else
                            {
                                decimal valeurdeci = Convert.ToDecimal((excelRange.Cells[row, 3] as Microsoft.Office.Interop.Excel.Range).Value2);
                            //    ig= (excelRange.Cells[row, 3] as Microsoft.Office.Interop.Excel.Range).Value2;
                                if (valeurdeci <= 50)
                                {
                                    indicebas = true;
                                    indicemoy = false;
                                    indiceelv = false;
                                    indiceNotDf = false;

                                }
                                else
                                {
                                    if (valeurdeci <= 70 && valeurdeci >= 51)
                                    {

                                        indicebas = false;
                                        indicemoy = true;
                                        indiceelv = false;
                                        indiceNotDf = false;
                                    }
                                    else
                                    {
                                        if (valeurdeci >= 71 && valeurdeci <= 100)
                                        {
                                            indicebas = false;
                                            indicemoy = false;
                                            indiceelv = true;
                                            indiceNotDf = false;

                                        }
                                    }
                                }


                            }
                            SVC.Aliment dr = new SVC.Aliment
                            {
                                //     dr.Id = row - 1;
                                DesignProduit = (excelRange.Cells[row, 1] as Microsoft.Office.Interop.Excel.Range).Value2.Trim().ToString(),
                                Kcal = Convert.ToDecimal((excelRange.Cells[row, 2] as Microsoft.Office.Interop.Excel.Range).Value2),
                                IG =Convert.ToDecimal(ig),
                                Protéines = Convert.ToDecimal((excelRange.Cells[row, 4] as Microsoft.Office.Interop.Excel.Range).Value2),
                                Glucides = Convert.ToDecimal((excelRange.Cells[row, 5] as Microsoft.Office.Interop.Excel.Range).Value2),
                                Lipides = Convert.ToDecimal((excelRange.Cells[row, 6] as Microsoft.Office.Interop.Excel.Range).Value2),
                                Fibres = Convert.ToDecimal((excelRange.Cells[row, 7] as Microsoft.Office.Interop.Excel.Range).Value2),
                                FamilleAlimentDesign = (excelRange.Cells[row, 8] as Microsoft.Office.Interop.Excel.Range).Value2,
                                ValeurGramme = 100,
                                IgBas = indicebas,
                                IgMoy = indicemoy,
                                IgElv = indiceelv,
                                IgNotDef= indiceNotDf,

                                
                        };
                        dt.Add(dr);
                          
                        }
                        excelBook.Close(true, Missing.Value, Missing.Value);
                        excelApp.Quit();
                     FamilleAlimentDataGrid.ItemsSource = dt;
                      FamilleAlimentDataGrid.DataContext = dt;
                    }
                }
            }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            }

        private void btnVisualiser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
               {
                   var itemsSourceDatagrid = FamilleAlimentDataGrid.ItemsSource as List<SVC.Aliment>;

                   bool succées = false;
                    List<SVC.FamilleAliment> famillelist = new List<SVC.FamilleAliment>();
                    foreach (SVC.Aliment item in itemsSourceDatagrid)
                  {
                        bool succéesFamille = false;
                        succées = false;
                        proxy.InsertAliment(item);
                         succées = true;
                        int idfamille = 1;
                              if (item.FamilleAlimentDesign != "")
                            {
                               
                                var found = (famillelist).Find(itemf => itemf.FamilleAlimentDesign == item.FamilleAlimentDesign);
                                if (found == null)
                                {
                                    succéesFamille = false;
                                    SVC.FamilleAliment fd = new SVC.FamilleAliment
                                    {
                                        FamilleAlimentDesign = item.FamilleAlimentDesign,
                                    };
                                    proxy.InsertFamilleAliment(fd);
                             
                                succéesFamille = true;
                                    if (succéesFamille == true)
                                    {
                                        famillelist.Add(fd);
                                        idfamille++;
                                }
                                }
                            }
                 }
                if (succées == true)
                  {
                    ts.Complete();

                 //       MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
          }
           
                remplirIdFamille();
                proxy.AjouterAlimentFamilleRefresh();
            }
            catch (Exception ex)
            {

                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void remplirIdFamille()
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    var itemsSourceDatagrid = proxy.GetAllAliment();

                    bool succées = false;
                    var famillelist = proxy.GetAllFamilleAliment();
               
                    foreach (SVC.Aliment item in itemsSourceDatagrid)
                    {
                       
                        
                        if (item.FamilleAlimentDesign != "")
                        {

                            var found = (famillelist).Find(itemf => itemf.FamilleAlimentDesign == item.FamilleAlimentDesign);
                            if (found != null)
                            {
                                succées = false;
                                item.IdFamilleAliment =found.Id;
                                proxy.UpdateAliment(item);
                           
                                succées = true;

                            }
                        }
                    }
                    if (succées == true)
                    {
                        ts.Complete();

                        MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
              
            }
            catch (Exception ex)
            {

                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
