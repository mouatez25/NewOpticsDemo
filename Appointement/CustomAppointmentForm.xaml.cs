using System;
using System.Globalization;
using System.Windows.Controls;
#region #usings_form
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.XtraScheduler;

namespace NewOptics.Appointement
{
    /// <summary>
    /// Interaction logic for CustomAppointmentForm.xaml
    /// </summary>

    #region #customform
    public partial class CustomAppointmentForm : UserControl
    {

        public CustomAppointmentFormController Controller { get; private set; }
        public SchedulerControl Control { get; private set; }
        public Appointment Appointment { get; private set; }
        public string TimeEditMask { get { return CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern; } }


        public CustomAppointmentForm(SchedulerControl control, Appointment apt)
        {
            InitializeComponent();
            if (control == null || apt == null)
                throw new ArgumentNullException("control");
            if (control == null || apt == null)
                throw new ArgumentNullException("apt");

            this.Control = control;
            this.Controller = new CustomAppointmentFormController(control, apt);
            this.Appointment = apt;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Controller.IsNewAppointment)
                SchedulerFormBehavior.SetTitle(this, "Nouvelle écriture");
            else
                SchedulerFormBehavior.SetTitle(this, "Modification - [" + Appointment.Subject + "]");
        }

        private void Ok_button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Save all changes made to the appointment.            
            Controller.Control.Storage.BeginUpdate();
            Controller.ApplyChanges();
            Controller.Control.Storage.EndUpdate();
            SchedulerFormBehavior.Close(this, true);
        }

        private void Cancel_button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SchedulerFormBehavior.Close(this, false);
        }
    }
    #endregion #customform

    #region #controller
    public class CustomAppointmentFormController : AppointmentFormController
    {
        public CustomAppointmentFormController(SchedulerControl control, Appointment apt)
            : base(control, apt)
        {
        }

        public string Contact
        {
            get { return GetContactValue(EditedAppointmentCopy); }
            set { EditedAppointmentCopy.CustomFields["Contact"] = value; }
        }

        string SourceContact
        {
            get { return GetContactValue(SourceAppointment); }
            set { SourceAppointment.CustomFields["Contact"] = value; }
        }

        public override bool IsAppointmentChanged()
        {
            if (base.IsAppointmentChanged())
                return true;
            return SourceContact != Contact;
        }

        protected string GetContactValue(Appointment apt)
        {
            return Convert.ToString(apt.CustomFields["Contact"]);
        }
    }
    #endregion #controller
    #endregion
}