
using GestionClinique.SVC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Data;
using GestionClinique.Administrateur;
using System.Windows.Threading;
using System.ServiceModel;

namespace GestionClinique.RendezVous
{
    /// <summary>
    /// Interaction logic for RendezVous.xaml
    /// </summary>
    public partial class RendezVous : UserControl
    {
       private readonly CalenderBackground background;
        SVC.ServiceCliniqueClient proxy;
        SVC.Medecin SelectMedecin;
        SVC.Membership membershipuser;
        private delegate void FaultedInvokerRendezVous();
        ICallback callback;
        public RendezVous(SVC.ServiceCliniqueClient proxyrecu,SVC.Membership memrecu,ICallback callbackrecu )
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                membershipuser = memrecu;
                autoCities.ItemsSource = proxy.GetAllMedecin();
                callback = callbackrecu;
                callbackrecu.InsertRendezVousCallbackEvent+= new ICallback.CallbackEventHandler8(callbackrecu_Refresh);
          
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

           proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void callbackrecu_Refresh(object source, CallbackEventInsertRendezVous e)
        {
            try { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
              AddRefresh(e.clientleav,e.operleav);
            }));
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
        public void AddRefresh(SVC.RendezVou listmembership,int oper)
        {
            try
            { 
          //    if (RendezVousExisiteGrid.Visibility==Visibility.Visible )
            //{
                RendezVousGrid.Visibility = Visibility.Hidden;
                RendezVousExisiteGrid.Visibility = Visibility.Visible;
                List<RendezVou> test = ((proxy.GetAllRendezVousParMedecin(SelectMedecin.Id)).Where(n => n.Date == AgendaCalendar.SelectedDate.Value)).ToList();
                //  List<RendezVou> test= ((proxy.GetAllRendezVous()).Where(n => n.MedecinNom == "dadadada" && n.MedecinPrénom == "dad" && n.Date == AgendaCalendar.SelectedDate.Value)).ToList();
                decimal minute, nbreVisiteMinute;
                int NbreEnregistrement;

                int i = 1;
                int j = 0;
                switch (AgendaCalendar.SelectedDate.Value.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        if (SelectMedecin.Samedi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.SamediD.Value.TotalMinutes != 0 && SelectMedecin.SamediF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.SamediF.Value.TotalMinutes - SelectMedecin.SamediD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {

                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {
                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;
                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;






                    case DayOfWeek.Sunday:
                        if (SelectMedecin.Dimanche != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.DimancheD.Value.TotalMinutes != 0 && SelectMedecin.DimancheF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.DimancheF.Value.TotalMinutes - SelectMedecin.DimancheD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {
                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {

                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;


                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;




                    case DayOfWeek.Monday:
                        if (SelectMedecin.Lundi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.LundiD.Value.TotalMinutes != 0 && SelectMedecin.LundiF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.LundiF.Value.TotalMinutes - SelectMedecin.LundiD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {

                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {
                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;
                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;



                    case DayOfWeek.Tuesday:
                        if (SelectMedecin.Mardi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MardiD.Value.TotalMinutes != 0 && SelectMedecin.MardiF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.MardiF.Value.TotalMinutes - SelectMedecin.MardiD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {

                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {
                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;
                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;


                    case DayOfWeek.Wednesday:
                        if (SelectMedecin.Mercredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MercrediD.Value.TotalMinutes != 0 && SelectMedecin.MercrediF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.MercrediF.Value.TotalMinutes - SelectMedecin.MercrediD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {

                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {
                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;
                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;





                    case DayOfWeek.Thursday:
                        if (SelectMedecin.Jeudi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.JeudiD.Value.TotalMinutes != 0 && SelectMedecin.JeudiF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.JeudiF.Value.TotalMinutes - SelectMedecin.JeudiD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {

                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {
                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;
                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;




                    case DayOfWeek.Friday:
                        if (SelectMedecin.Vendredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.VendrediD.Value.TotalMinutes != 0 && SelectMedecin.VendrediF.Value.TotalMinutes != 0)
                        {
                            minute = Convert.ToDecimal(SelectMedecin.VendrediF.Value.TotalMinutes - SelectMedecin.VendrediD.Value.TotalMinutes);
                            nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                            NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                            //dt = new List<RendezVou>();

                            while (NbreEnregistrement >= i)
                            {
                                var exist = test.Exists(x => x.RendezVousD == SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                if (exist == false)
                                {

                                    test.Add(new RendezVou
                                    {
                                        RendezVousD = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                        RendezVousF = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                        MedecinNom = SelectMedecin.Nom,
                                        MedecinPrénom = SelectMedecin.Prénom,
                                        Date = AgendaCalendar.SelectedDate,
                                        PrisPar = membershipuser.UserName,
                                        Confirm = false,
                                    });
                                }
                                i++;
                                j++;
                            }
                            RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();
                            RendezVousGrid.Visibility = Visibility.Hidden;
                            break;
                        }

                        break;

                }
                
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerRendezVous(HandleProxy));
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

                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        var wndlistsession = Window.GetWindow(this);

                        Grid test = (Grid)wndlistsession.FindName("gridAuthentification");
                        test.Visibility = Visibility.Visible;
                        Button confirmer = (Button)wndlistsession.FindName("Confirmer");
                        confirmer.IsEnabled = false;
                        Grid tests = (Grid)wndlistsession.FindName("gridhome");
                        tests.Visibility = Visibility.Collapsed;

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerRendezVous(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void KalenderOnDisplayDateChanged(object sender, System.Windows.Controls.CalendarDateChangedEventArgs calendarDateChangedEventArgs)
        {
            try { 
            AgendaCalendar.Background = background.GetBackground();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        protected void autoCities_PatternChanged(object sender, GestionClinique.RendezVous.AutoComplete.AutoCompleteArgs args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Pattern))
                {
                    args.CancelBinding = true;
                    autoCities.ItemsSource = proxy.GetAllMedecin();
                }
                else
                {
                    args.DataSource = GetCities(args.Pattern);
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private ObservableCollection<Medecin> GetCities(string Pattern)
        {
           
            return new ObservableCollection<Medecin>(
                proxy.GetAllMedecin().
                Where((city, match) => city.Nom.ToLower().Contains(Pattern.ToLower())));
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { 
            this.autoCities.Focus();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void autoCities_DropDownClosed(object sender, EventArgs e)
        {

            try { 
            if (autoCities.Text != null)
            {
                SelectMedecin = autoCities.SelectedItem as SVC.Medecin;
                if (SelectMedecin != null)
                {
                    AgendaCalendar.BlackoutDates.Clear();
                    txtlabels.Content = SelectMedecin.Nom.ToString() + " " + SelectMedecin.Prénom.ToString();
                    AgendaCalendar.IsEnabled = true;
                    autoCities.ItemsSource = proxy.GetAllMedecin();
                    autoCities.Text = SelectMedecin.Nom + " " + SelectMedecin.Prénom;

                    AgendaCalendar.DisplayDateStart = DateTime.Now - TimeSpan.FromDays(360);
                    AgendaCalendar.DisplayDateEnd = DateTime.Now + TimeSpan.FromDays(360);
                    var minDate = AgendaCalendar.DisplayDateStart ?? DateTime.MinValue;
                    var maxDate = AgendaCalendar.DisplayDateEnd ?? DateTime.MaxValue;

                    for (var d = minDate; d <= maxDate && DateTime.MaxValue > d; d = d.AddDays(1))
                    {
                        switch (d.DayOfWeek)
                        {
                            case DayOfWeek.Saturday:
                                if (SelectMedecin.Samedi != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                            case DayOfWeek.Sunday:
                                if (SelectMedecin.Dimanche != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                            case DayOfWeek.Monday:
                                if (SelectMedecin.Lundi != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                            case DayOfWeek.Tuesday:
                                if (SelectMedecin.Mardi != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if (SelectMedecin.Mercredi != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (SelectMedecin.Jeudi != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (SelectMedecin.Vendredi != true)
                                {
                                    System.Windows.Controls.CalendarDateRange dt = new System.Windows.Controls.CalendarDateRange(d);
                                    AgendaCalendar.BlackoutDates.Add(dt);
                                }
                                break;
                        }

                    }

                    if (AgendaCalendar.SelectedDate != null)
                    {


                        // int RendezVousExist = proxy.GetAllRendezVousParMedecin(SelectMedecin.Nom.ToUpper().Trim(), SelectMedecin.Prénom.ToUpper().Trim()).Where(n => n.Date == AgendaCalendar.SelectedDate.Value).Count();
                        int RendezVousExist = (proxy.GetAllRendezVousParMedecin(SelectMedecin.Id)).Where(n => n.Date == AgendaCalendar.SelectedDate.Value).Count();
                        if (RendezVousExist == 0)
                        {
                            RendezVousExisiteGrid.Visibility = Visibility.Hidden;
                            if (SelectMedecin.Type != "Passager")
                            {

                                if (AgendaCalendar.SelectedDate != null)
                                {
                                    decimal minute, nbreVisiteMinute;
                                    int NbreEnregistrement;
                                    List<RendezVou> dt;
                                    int i = 1;
                                    int j = 0;
                                    switch (AgendaCalendar.SelectedDate.Value.DayOfWeek)
                                    {
                                        case DayOfWeek.Saturday:
                                            if (SelectMedecin.Samedi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.SamediD.Value.TotalMinutes != 0 && SelectMedecin.SamediF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.SamediF.Value.TotalMinutes - SelectMedecin.SamediD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();

                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }

                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                        case DayOfWeek.Sunday:
                                            if (SelectMedecin.Dimanche != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.DimancheD.Value.TotalMinutes != 0 && SelectMedecin.DimancheF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.DimancheF.Value.TotalMinutes - SelectMedecin.DimancheD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();

                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }
                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                        case DayOfWeek.Monday:

                                            if (SelectMedecin.Lundi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.LundiD.Value.TotalMinutes != 0 && SelectMedecin.LundiF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.LundiF.Value.TotalMinutes - SelectMedecin.LundiD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();

                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }
                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                        case DayOfWeek.Tuesday:
                                            if (SelectMedecin.Mardi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MardiD.Value.TotalMinutes != 0 && SelectMedecin.MardiF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.MardiF.Value.TotalMinutes - SelectMedecin.MardiD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();

                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }
                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                        case DayOfWeek.Wednesday:
                                            if (SelectMedecin.Mercredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MercrediD.Value.TotalMinutes != 0 && SelectMedecin.MercrediF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.MercrediF.Value.TotalMinutes - SelectMedecin.MercrediD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();

                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }
                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                        case DayOfWeek.Thursday:
                                            if (SelectMedecin.Jeudi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.JeudiD.Value.TotalMinutes != 0 && SelectMedecin.JeudiF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.JeudiF.Value.TotalMinutes - SelectMedecin.JeudiD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();

                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }
                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                        case DayOfWeek.Friday:
                                            if (SelectMedecin.Vendredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.VendrediD.Value.TotalMinutes != 0 && SelectMedecin.VendrediF.Value.TotalMinutes != 0)
                                            {
                                                minute = Convert.ToDecimal(SelectMedecin.VendrediF.Value.TotalMinutes - SelectMedecin.VendrediD.Value.TotalMinutes);
                                                nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                                NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                                dt = new List<RendezVou>();

                                                while (NbreEnregistrement >= i)
                                                {


                                                    dt.Add(new RendezVou
                                                    {
                                                        RendezVousD = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                        RendezVousF = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                        MedecinNom = SelectMedecin.Nom,
                                                        MedecinPrénom = SelectMedecin.Prénom,
                                                        Date = AgendaCalendar.SelectedDate,
                                                        PrisPar = membershipuser.UserName,
                                                        Confirm = false,
                                                    });
                                                    i++;
                                                    j++;
                                                }
                                                RendezVousGrid.ItemsSource = dt.ToList();
                                                RendezVousGrid.Visibility = Visibility.Visible;
                                                break;
                                            }
                                            RendezVousGrid.Visibility = Visibility.Hidden;
                                            break;
                                    }
                                }
                            }
                            else
                            {

                            }

                        }
                        else
                        {
                            if (SelectMedecin.Type != "Passager")
                            {
                                RendezVousGrid.Visibility = Visibility.Hidden;
                                RendezVousExisiteGrid.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
                else
                {
                    if (SelectMedecin == null)
                    {
                        txtlabels.Content = "";
                        //  AgendaCalendar.IsEnabled = true;
                        RendezVousGrid.Visibility = Visibility.Hidden;
                        autoCities.ItemsSource = proxy.GetAllMedecin();
                    }
                }
            }
            else
            {
                SelectMedecin = autoCities.SelectedItem as SVC.Medecin;
                if (autoCities.Text == null && SelectMedecin == null)
                {
                    txtlabels.Content = "";
                    //  AgendaCalendar.IsEnabled = true;

                    autoCities.ItemsSource = proxy.GetAllMedecin();

                }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
       
        private void NewSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AgendaCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            { 
            if (SelectMedecin != null && AgendaCalendar.SelectedDate != null)
            {
                txtlabels.Content = SelectMedecin.Nom.ToString() + " " + SelectMedecin.Prénom.ToString();
                AgendaCalendar.IsEnabled = true;
            
                   int RendezVousExist = (proxy.GetAllRendezVousParMedecin(SelectMedecin.Id)).Where(n => n.Date == AgendaCalendar.SelectedDate).Count();
                if (RendezVousExist == 0)
                {
                    RendezVousExisiteGrid.Visibility = Visibility.Hidden;
                    if (SelectMedecin.Type != "Passager")
                    {
                        if (AgendaCalendar.SelectedDate != null && RendezVousExist == 0)
                        {
                            decimal minute, nbreVisiteMinute;
                            int NbreEnregistrement;
                            List<RendezVou> dt;
                            int i = 1;
                            int j = 0;
                            switch (AgendaCalendar.SelectedDate.Value.DayOfWeek)
                            {
                                case DayOfWeek.Saturday:
                                    if (SelectMedecin.Samedi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.SamediD.Value.TotalMinutes != 0 && SelectMedecin.SamediF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.SamediF.Value.TotalMinutes - SelectMedecin.SamediD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();

                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                                case DayOfWeek.Sunday:
                                    if (SelectMedecin.Dimanche != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.DimancheD.Value.TotalMinutes != 0 && SelectMedecin.DimancheF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.DimancheF.Value.TotalMinutes - SelectMedecin.DimancheD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();

                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                                case DayOfWeek.Monday:

                                    if (SelectMedecin.Lundi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.LundiD.Value.TotalMinutes != 0 && SelectMedecin.LundiF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.LundiF.Value.TotalMinutes - SelectMedecin.LundiD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();

                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                                case DayOfWeek.Tuesday:
                                    if (SelectMedecin.Mardi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MardiD.Value.TotalMinutes != 0 && SelectMedecin.MardiF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.MardiF.Value.TotalMinutes - SelectMedecin.MardiD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();

                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                                case DayOfWeek.Wednesday:
                                    if (SelectMedecin.Mercredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MercrediD.Value.TotalMinutes != 0 && SelectMedecin.MercrediF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.MercrediF.Value.TotalMinutes - SelectMedecin.MercrediD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();

                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                                case DayOfWeek.Thursday:
                                    if (SelectMedecin.Jeudi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.JeudiD.Value.TotalMinutes != 0 && SelectMedecin.JeudiF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.JeudiF.Value.TotalMinutes - SelectMedecin.JeudiD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();

                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                                case DayOfWeek.Friday:
                                    if (SelectMedecin.Vendredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.VendrediD.Value.TotalMinutes != 0 && SelectMedecin.VendrediF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.VendrediF.Value.TotalMinutes - SelectMedecin.VendrediD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {


                                            dt.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            i++;
                                            j++;
                                        }
                                        RendezVousGrid.ItemsSource = dt.ToList();
                                        RendezVousGrid.Visibility = Visibility.Visible;
                                        break;
                                    }
                                    RendezVousGrid.Visibility = Visibility.Hidden;
                                    break;
                            }
                        }
                    }
                    else
                    {


                    }
                }
                else
                {
                    if (SelectMedecin.Type != "Passager")
                    {
                      
                        if (AgendaCalendar.SelectedDate != null && RendezVousExist != 0)
                        {
                            RendezVousGrid.Visibility = Visibility.Hidden;
                            RendezVousExisiteGrid.Visibility = Visibility.Visible;

                              List<RendezVou> test= ((proxy.GetAllRendezVousParMedecin(SelectMedecin.Id)).Where(n =>  n.Date == AgendaCalendar.SelectedDate.Value)).ToList();
                          //  List<RendezVou> test= ((proxy.GetAllRendezVous()).Where(n => n.MedecinNom == "dadadada" && n.MedecinPrénom == "dad" && n.Date == AgendaCalendar.SelectedDate.Value)).ToList();
                            decimal minute, nbreVisiteMinute;
                            int NbreEnregistrement;
                     
                            int i = 1;
                            int j = 0;
                            switch (AgendaCalendar.SelectedDate.Value.DayOfWeek)
                            {
                                case DayOfWeek.Saturday:
                                    if (SelectMedecin.Samedi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.SamediD.Value.TotalMinutes != 0 && SelectMedecin.SamediF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.SamediF.Value.TotalMinutes - SelectMedecin.SamediD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {

                                            var exist = test.Exists(x => x.RendezVousD == SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist == false)
                                            {
                                                test.Add(new RendezVou
                                                {
                                                    RendezVousD = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                    RendezVousF = SelectMedecin.SamediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                    MedecinNom = SelectMedecin.Nom,
                                                    MedecinPrénom = SelectMedecin.Prénom,
                                                    Date = AgendaCalendar.SelectedDate,
                                                    PrisPar = membershipuser.UserName,
                                                    Confirm = false,
                                                });
                                            }
                                            i++;
                                            j++;
                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;






                                case DayOfWeek.Sunday:
                                    if (SelectMedecin.Dimanche != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.DimancheD.Value.TotalMinutes != 0 && SelectMedecin.DimancheF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.DimancheF.Value.TotalMinutes - SelectMedecin.DimancheD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {
                                          var exist=  test.Exists(x => x.RendezVousD == SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist==false)
                                            { 

                                                test.Add(new RendezVou
                                            {
                                                RendezVousD = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                RendezVousF = SelectMedecin.DimancheD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                MedecinNom = SelectMedecin.Nom,
                                                MedecinPrénom = SelectMedecin.Prénom,
                                                Date = AgendaCalendar.SelectedDate,
                                                PrisPar = membershipuser.UserName,
                                                Confirm = false,
                                            });
                                            }
                                            i++;
                                            j++;
                                         

                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;




                                case DayOfWeek.Monday:
                                    if (SelectMedecin.Lundi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.LundiD.Value.TotalMinutes != 0 && SelectMedecin.LundiF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.LundiF.Value.TotalMinutes - SelectMedecin.LundiD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {

                                            var exist = test.Exists(x => x.RendezVousD == SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist == false)
                                            {
                                                test.Add(new RendezVou
                                                {
                                                    RendezVousD = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                    RendezVousF = SelectMedecin.LundiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                    MedecinNom = SelectMedecin.Nom,
                                                    MedecinPrénom = SelectMedecin.Prénom,
                                                    Date = AgendaCalendar.SelectedDate,
                                                    PrisPar = membershipuser.UserName,
                                                    Confirm = false,
                                                });
                                            }
                                            i++;
                                            j++;
                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;



                                case DayOfWeek.Tuesday:
                                    if (SelectMedecin.Mardi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MardiD.Value.TotalMinutes != 0 && SelectMedecin.MardiF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.MardiF.Value.TotalMinutes - SelectMedecin.MardiD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {

                                            var exist = test.Exists(x => x.RendezVousD == SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist == false)
                                            {
                                                test.Add(new RendezVou
                                                {
                                                    RendezVousD = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                    RendezVousF = SelectMedecin.MardiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                    MedecinNom = SelectMedecin.Nom,
                                                    MedecinPrénom = SelectMedecin.Prénom,
                                                    Date = AgendaCalendar.SelectedDate,
                                                    PrisPar = membershipuser.UserName,
                                                    Confirm = false,
                                                });
                                            }
                                            i++;
                                            j++;
                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;


                                case DayOfWeek.Wednesday:
                                    if (SelectMedecin.Mercredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.MercrediD.Value.TotalMinutes != 0 && SelectMedecin.MercrediF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.MercrediF.Value.TotalMinutes - SelectMedecin.MercrediD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {

                                            var exist = test.Exists(x => x.RendezVousD == SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist == false)
                                            {
                                                test.Add(new RendezVou
                                                {
                                                    RendezVousD = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                    RendezVousF = SelectMedecin.MercrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                    MedecinNom = SelectMedecin.Nom,
                                                    MedecinPrénom = SelectMedecin.Prénom,
                                                    Date = AgendaCalendar.SelectedDate,
                                                    PrisPar = membershipuser.UserName,
                                                    Confirm = false,
                                                });
                                            }
                                            i++;
                                            j++;
                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;





                                case DayOfWeek.Thursday:
                                    if (SelectMedecin.Jeudi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.JeudiD.Value.TotalMinutes != 0 && SelectMedecin.JeudiF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.JeudiF.Value.TotalMinutes - SelectMedecin.JeudiD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {

                                            var exist = test.Exists(x => x.RendezVousD == SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist == false)
                                            {
                                                test.Add(new RendezVou
                                                {
                                                    RendezVousD = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                    RendezVousF = SelectMedecin.JeudiD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                    MedecinNom = SelectMedecin.Nom,
                                                    MedecinPrénom = SelectMedecin.Prénom,
                                                    Date = AgendaCalendar.SelectedDate,
                                                    PrisPar = membershipuser.UserName,
                                                    Confirm = false,
                                                });
                                            }
                                            i++;
                                            j++;
                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();

                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;




                                case DayOfWeek.Friday:
                                    if (SelectMedecin.Vendredi != false && SelectMedecin.TempsVisite.Value.TotalMinutes != 0 && SelectMedecin.VendrediD.Value.TotalMinutes != 0 && SelectMedecin.VendrediF.Value.TotalMinutes != 0)
                                    {
                                        minute = Convert.ToDecimal(SelectMedecin.VendrediF.Value.TotalMinutes - SelectMedecin.VendrediD.Value.TotalMinutes);
                                        nbreVisiteMinute = Convert.ToDecimal((minute / Convert.ToDecimal(SelectMedecin.TempsVisite.Value.TotalMinutes)));
                                        NbreEnregistrement = Convert.ToInt16(Math.Floor(nbreVisiteMinute));

                                        //dt = new List<RendezVou>();

                                        while (NbreEnregistrement >= i)
                                        {
                                            var exist = test.Exists(x => x.RendezVousD == SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j));
                                            if (exist == false)
                                            {

                                                test.Add(new RendezVou
                                                {
                                                    RendezVousD = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * j),
                                                    RendezVousF = SelectMedecin.VendrediD + TimeSpan.FromTicks(SelectMedecin.TempsVisite.Value.Ticks * i),
                                                    MedecinNom = SelectMedecin.Nom,
                                                    MedecinPrénom = SelectMedecin.Prénom,
                                                    Date = AgendaCalendar.SelectedDate,
                                                    PrisPar = membershipuser.UserName,
                                                    Confirm = false,
                                                });
                                            }
                                            i++;
                                            j++;
                                        }
                                        RendezVousExisiteGrid.ItemsSource = (test.OrderBy(person => person.RendezVousD)).ToList();
                                        RendezVousGrid.Visibility = Visibility.Hidden;
                                        break;
                                    }

                                    break;

                            }
                        }
                    }

                }
            }
        }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void BtnRendezVous_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (RendezVousGrid.SelectedItem != null && membershipuser.CréationRendezVous == true)
            {
              SVC.RendezVou  SelectRendezVous = RendezVousGrid.SelectedItem as SVC.RendezVou;

        
                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, membershipuser,callback,1,null);
                CLMedecin.Show();



            }
        }catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void BtnRendezVousExiste_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if (RendezVousExisiteGrid.SelectedItem != null && membershipuser.CréationRendezVous == true)
            {
                SVC.RendezVou SelectRendezVous = RendezVousExisiteGrid.SelectedItem as SVC.RendezVou;


                PrendreRendezVous CLMedecin = new PrendreRendezVous(SelectRendezVous, proxy, membershipuser, callback,2,null);
                CLMedecin.Show();



                }
            }
            catch (Exception ex)
            {
                MessageBoxResult resultc1 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

      
    }
}
