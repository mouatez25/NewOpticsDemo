using DevExpress.Xpf.Core;
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
using System.Windows.Shapes;
using System.Windows.Forms;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.Xpf.Scheduler.Menu;
using System.ServiceModel;
using System.Windows.Threading;
using NewOptics.Administrateur;
using System.Transactions;
using System.Windows.Media;
using System.Drawing;

namespace NewOptics.Appointement
{
    /// <summary>
    /// Interaction logic for ListeRendezVous.xaml
    /// </summary>
    public partial class ListeRendezVous : DXWindow
    {

        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerListeRendezVous();
        ICallback callback;
        List<SVC.Appointment> listAppointment;
        string colormotif = "";
        SVC.MembershipOptic memberuser;
        public ListeRendezVous(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrfecu,SVC.MembershipOptic memberrecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrfecu;
                memberuser = memberrecu;
                listAppointment=proxy.GetAllAppointment(DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
                ShedulerControl.Storage.AppointmentStorage.DataContext = listAppointment;
                //ShedulerControl.Storage.ResourceStorage.DataContext = proxy.GetAllResource();
                DateSaisieDébut.SelectedDate = DateTime.Now.AddDays(-7);
                DateSaisieFin.SelectedDate = DateTime.Now.AddDays(7);
                 
                ShedulerControl.Storage.AppointmentsInserted +=
                   new PersistentObjectsEventHandler(Storage_AppointmentsInserted);
                ShedulerControl.Storage.AppointmentsChanged +=
                    new PersistentObjectsEventHandler(Storage_AppointmentsModified);
                ShedulerControl.Storage.AppointmentsDeleted +=
                    new PersistentObjectsEventHandler(Storage_AppointmentsDeleted);

                /*************************************************/
                ShedulerControl.OptionsCustomization.AllowInplaceEditor = DevExpress.XtraScheduler.UsedAppointmentType.None;                /********************************************/
                MotifTable.ItemsSource = proxy.GetAllMotifTable();
                callbackrfecu.InsertMotifTableCallbackevent += new ICallback.CallbackEventHandler53(callbackrecu_Refresh);
                /****************************************************/
                StatutsTable.ItemsSource = proxy.GetAllStatu();
                callbackrfecu.InsertStatuCallbackevent += new ICallback.CallbackEventHandler52(callbackrecustat);
                /*****************************************************************/
                labelstatu();
              
                /****************************************************/
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                HandleProxy();
            }

        }
        void labelstatu()
        {
            var query = from c in proxy.GetAllMotifTable()
                        select new { c.Motif }.Motif;
            string[] IssueList = query.ToArray();
            var querycolor = from c in proxy.GetAllMotifTable()
                             select new { c.Color }.Color;
            System.Windows.Media.Color[] IssueColorList = new System.Windows.Media.Color[querycolor.Count()];

            for (int runs = 0; runs < querycolor.Count(); runs++)
            {
                var item = querycolor.ElementAt(runs);
                IssueColorList[runs] = ConvertStringToColor(item);

            }

            var querystatus = from c in proxy.GetAllStatu()
                              select new { c.Status }.Status;
            string[] PaymentStatuses = querystatus.ToArray();

            var converter = new System.Windows.Media.BrushConverter();
            //var brush = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF90");

            var querycolorstatus = from c in proxy.GetAllStatu()
                                   select new { c.Color }.Color;
            System.Windows.Media.Brush[] PaymentColorStatuses = new System.Windows.Media.Brush[querycolorstatus.Count()];

            for (int runs = 0; runs < querycolorstatus.Count(); runs++)
            {
                var item = querycolorstatus.ElementAt(runs);
                PaymentColorStatuses[runs] = (System.Windows.Media.Brush)converter.ConvertFromString(item);

            }

            //  System.Windows.Media.Brush[] PaymentColorStatuses = { new LinearGradientBrush(Colors.Green, Colors.Yellow, 45.0), new SolidColorBrush(Colors.Red) };



            IAppointmentLabelStorage labelStorage = ShedulerControl.Storage.AppointmentStorage.Labels;
            labelStorage.Clear();
            int count = IssueList.Length;
            for (int i = 0; i < count; i++)
            {
                IAppointmentLabel label = labelStorage.CreateNewLabel(i, IssueList[i]);
                label.SetColor(IssueColorList[i]);
                labelStorage.Add(label);
            }
            AppointmentStatusCollection statusColl = ShedulerControl.Storage.AppointmentStorage.Statuses;
            statusColl.Clear();
            count = PaymentStatuses.Length;
            for (int i = 0; i < count; i++)
            {
                AppointmentStatus status = statusColl.CreateNewStatus(i, PaymentStatuses[i], PaymentStatuses[i]);
                status.SetBrush((PaymentColorStatuses[i]));
                statusColl.Add(status);
            }

        }
        void callbackrecustat(object source, CallbackEventInsertStatu e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddRefreshStatu(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefreshStatu(List<SVC.Statu> listMembershipOptic)
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

                        StatutsTable.ItemsSource = listMembershipOptic;
                        labelstatu();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        void callbackrecu_Refresh(object source, CallbackEventInsertMotifTable e)
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
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message,NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void AddRefresh(List<SVC.MotifTable> listMembershipOptic)
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

                        MotifTable.ItemsSource = listMembershipOptic;
                        labelstatu();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        public System.Windows.Media.Color ConvertStringToColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }
        void Storage_AppointmentsDeleted(object sender, PersistentObjectsEventArgs e)
        {
            try
            {
                if (memberuser.SuppressionPlanning == true)
                {
                    SVC.Appointment selected;
                    for (int i = 0; i < e.Objects.Count; i++)
                    {
                        Appointment currentAppointment = e.Objects[i] as Appointment;
                        object apptID = currentAppointment.Id;
                          selected = proxy.GetAllAppointmentBycode(currentAppointment.Subject,currentAppointment.Start,currentAppointment.End).First();
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            proxy.DeleteAppointment(selected);
                            ts.Complete();
                        }
                    }
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                 
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
        void Storage_AppointmentsModified(object sender, PersistentObjectsEventArgs e)
        {
            try
            {

               
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Modification impossible", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
                
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
        void Storage_AppointmentsInserted(object sender, PersistentObjectsEventArgs e)
        {
            try
            {

                for (int i = 0; i < e.Objects.Count; i++)
                {
                    DevExpress.XtraScheduler.Appointment newlyInsertedAppointment = e.Objects[i] as DevExpress.XtraScheduler.Appointment;
                    SVC.Appointment cl = new SVC.Appointment
                    {
                        AllDay=newlyInsertedAppointment.AllDay,
                        //CustomField1=newlyInsertedAppointment.CustomFields,
                        Description= newlyInsertedAppointment.Description,
                        EndDate= newlyInsertedAppointment.End,
                        Label= newlyInsertedAppointment.LabelId,
                        Location= newlyInsertedAppointment.Location,
                        Subject= newlyInsertedAppointment.Subject,
                        StartDate= newlyInsertedAppointment.Start,
                        //RecurrenceInfo= newlyInsertedAppointment.RecurrenceInfo,
                       // ReminderInfo= newlyInsertedAppointment.Reminder,
                       Status= newlyInsertedAppointment.StatusId,
                     //  ResourceID= newlyInsertedAppointment.ResourceId,
                       //Type= newlyInsertedAppointment.Type,
                       Date= newlyInsertedAppointment.Start,
                    };


                   // MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(newlyInsertedAppointment.Subject, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                     using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        proxy.InsertAppointment(cl);
                        ts.Complete();
                     }
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                /**/
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeRendezVous(HandleProxy));
                return;
            }
            HandleProxy();
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerListeRendezVous(HandleProxy));
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
       


        #region #editappointmentformshowing
        private void Scheduler_EditAppointmentFormShowing(object sender, EditAppointmentFormEventArgs e)
        {
            e.Form = new CustomAppointmentForm(ShedulerControl, e.Appointment);
        }
        #endregion #editappointmentformshowing
      

        private void ShedulerControl_PopupMenuShowing(object sender, SchedulerMenuEventArgs e)
        {
           // SchedulerMenuItemName.NewRecurringEvent

        }


        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateSaisieDébut.SelectedDate!=null && DateSaisieFin.SelectedDate!=null)
                {
                    listAppointment = proxy.GetAllAppointment(DateSaisieDébut.SelectedDate.Value,DateSaisieFin.SelectedDate.Value);
                    ShedulerControl.Storage.AppointmentStorage.DataContext = listAppointment;
                    ShedulerControl.Start = DateSaisieDébut.SelectedDate.Value;
                    
                }
            }
            catch(Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSupp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {

        }

private void MotifTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                SVC.MotifTable RowDataContaxt = e.Row.DataContext as SVC.MotifTable;
                if (RowDataContaxt != null)
                {
                    var converter = new System.Windows.Media.BrushConverter();

                    if (RowDataContaxt.Color != "")
                        e.Row.Background = (System.Windows.Media.Brush)converter.ConvertFromString(RowDataContaxt.Color);
                  

                }
            }
            catch (Exception ex)
            {

                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewMotif_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.CreationPlanning==true)
                {
                    AjouterMotif cl = new AjouterMotif(proxy,memberuser,null);
                    cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void MotifTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(memberuser.ModificationPlanning==true && MotifTable.SelectedItem!=null)
                {
                    SVC.MotifTable selectedmotif = MotifTable.SelectedItem as SVC.MotifTable;
                    AjouterMotif cl = new AjouterMotif(proxy,memberuser,selectedmotif);
                    cl.Show();
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnEditMotif_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModificationPlanning == true && MotifTable.SelectedItem != null)
                {
                    SVC.MotifTable selectedmotif = MotifTable.SelectedItem as SVC.MotifTable;
                    AjouterMotif cl = new AjouterMotif(proxy, memberuser, selectedmotif);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteMotif_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.SuppressionPlanning==true && MotifTable.SelectedItem!=null)
                {
                    SVC.MotifTable selectedmotif = MotifTable.SelectedItem as SVC.MotifTable;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        
                        proxy.DeletMotifTable(selectedmotif);
                        ts.Complete();

                    }
                    proxy.AjouterMotifTableRefresh();

                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnNewStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(memberuser.CreationPlanning==true )
                {
                    InsertStatus cl = new InsertStatus(proxy,memberuser,null);
                    cl.Show();
                }

            }catch(Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.SuppressionPlanning == true && StatutsTable.SelectedItem != null)
                {
                    SVC.Statu selectedmotif = StatutsTable.SelectedItem as SVC.Statu;
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {

                        proxy.DeletStatu(selectedmotif);
                        ts.Complete();

                    }
                    proxy.AjouterStatuRefresh();

                    MessageBoxResult resul03 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.OperationSuccées, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnEditStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (memberuser.ModificationPlanning == true && StatutsTable.SelectedItem != null)
                {
                    SVC.Statu selectedstatu = StatutsTable.SelectedItem as SVC.Statu;
                    InsertStatus cl = new InsertStatus(proxy, memberuser, selectedstatu);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StatutsTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (memberuser.ModificationPlanning == true && StatutsTable.SelectedItem != null)
                {
                    SVC.Statu selectedstatu = StatutsTable.SelectedItem as SVC.Statu;
                    InsertStatus cl = new InsertStatus(proxy, memberuser, selectedstatu);
                    cl.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StatutsTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                SVC.Statu RowDataContaxt = e.Row.DataContext as SVC.Statu;
                if (RowDataContaxt != null)
                {
                    var converter = new System.Windows.Media.BrushConverter();

                    if (RowDataContaxt.Color != "")
                        e.Row.Background = (System.Windows.Media.Brush)converter.ConvertFromString(RowDataContaxt.Color);


                }
            }
            catch (Exception ex)
            {

                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShedulerControl_AppointmentDrag(object sender, AppointmentDragEventArgs e)
        {
            e.Allow = false;
        }

        private void ShedulerControl_AppointmentDrop(object sender, AppointmentDragEventArgs e)
        {
            e.Allow = false;
        }

        private void ShedulerControl_AllowAppointmentDragBetweenResources(object sender, AppointmentOperationEventArgs e)
        {
            e.Allow = false;
        }

         

        private void ShedulerControl_AllowAppointmentDrag(object sender, AppointmentOperationEventArgs e)
        {
            e.Allow = false;
        }

        
    }
  

}