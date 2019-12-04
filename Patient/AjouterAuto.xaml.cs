using DevExpress.Xpf.Core;
using GestionClinique.Administrateur;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for AjouterAuto.xaml
    /// </summary>
    public partial class AjouterAuto : DXWindow
    {
        SVC.autosurveillance autosurv;
        SVC.ServiceCliniqueClient proxy;
        ICallback callback;
        SVC.Patient PATIENT;
        SVC.Membership MEMBERSIP;
        OpenFileDialog op = new OpenFileDialog();
        bool Création;
        private delegate void FaultedInvokerAuto();
        string serverfilepath, filepath;
        public AjouterAuto(SVC.ServiceCliniqueClient proxyrecu,ICallback callbackrecu,SVC.Patient patientrecud,SVC.Membership memberrecu,SVC.autosurveillance autorecu)
        {
            try
            {
                InitializeComponent();
                proxy = proxyrecu;
                callback = callbackrecu;
                PATIENT = patientrecud;
                MEMBERSIP = memberrecu;

                if(autorecu==null)
                {
                    Création = true;
                       autosurv = new SVC.autosurveillance
                    {
                        CheminPhoto="",
                        Cle="",
                        CodePatient=PATIENT.Id,
                        NomPatient=PATIENT.Nom,
                        PrénomPatient=PATIENT.Prénom,
                        débutdelafiche=DateTime.Now,
                        Existe=false,
                        NbSemaine=1,
                        Type="Type 2"
                       
                    };
                    AutoGrid.DataContext = autosurv;

                }
                else
                {
                    autosurv = autorecu;
                    if (autosurv.Type == "Type 2")
                    {
                        radiotYPE1.IsChecked = false;
                        radiotYPE2.IsChecked = true;
                    }
                    else
                    {
                        if (autosurv.Type == "Type 1")
                        {
                            radiotYPE1.IsChecked = true;
                            radiotYPE2.IsChecked = false;
                        }
                    }
                    AutoGrid.DataContext = autosurv;
                    if (autorecu.CheminPhoto.ToString() != "")
                    {

                        if (proxy.DownloadDocumentIsHere(autorecu.CheminPhoto.ToString()) == true)
                        {
                            imgPhoto.Source = LoadImage(proxy.DownloadDocument(autorecu.CheminPhoto.ToString()));
                            Création = false;
                          
                            op.FileName = autorecu.CheminPhoto;
                            btnCreer.IsEnabled = true;
                            btnCreer.ToolTip= "Modifier";
                        }
                        else
                        {
                            MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show("Attention le fichier n'existe plus dans le serveur",GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);
                            op.FileName = "";
                            Création = false;
                       
                            btnCreer.IsEnabled = true;
                            btnCreer.ToolTip = "Modifier";
                        }
                    }
                }

                proxy.InnerDuplexChannel.Opened += new EventHandler(InnerDuplexChannel_Opened);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch(Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.Logiciel, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAuto(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerAuto(HandleProxy));
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
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            op = new OpenFileDialog();
            op.Title = "Selectionnez image";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Création == true && MEMBERSIP.CréationPatient == true && Semaineparpage.SelectedItem != null && txtDate.SelectedDate != null && (radiotYPE1.IsChecked == true || radiotYPE2.IsChecked == true))
                {
                    serverfilepath = op.FileName;

                    filepath = "";
                    if (serverfilepath != "")
                    {

                        filepath = op.FileName;

                        serverfilepath = @"Patient\AutoSurv\" + (PATIENT.Nom + PATIENT.Prénom+DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString()+DateTime.Now.Hour.ToString()+DateTime.Now.Minute.ToString()+DateTime.Now.Second.ToString()) + ".png";
                        byte[] buffer = null;

                        // read the file and return the byte[
                        using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, (int)fs.Length);
                        }
                        if (buffer != null)
                        {
                            proxy.UploadDocument(serverfilepath, buffer);
                        }

                        autosurv.CheminPhoto = serverfilepath;
                    }
                    autosurv.Cle = PATIENT.Id + autosurv.débutdelafiche.ToString() + Semaineparpage.SelectedIndex.ToString();
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (radiotYPE1.IsChecked == false || radiotYPE2.IsChecked == true)
                        {

                            autosurv.Type = "Type 2";
                        }
                        else
                        {
                            if (radiotYPE1.IsChecked == true || radiotYPE2.IsChecked == false)
                            {


                                autosurv.Type = "Type 1";
                            }
                        }
                        proxy.Insertautosurveillance(autosurv);
                        ts.Complete();
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    proxy.AjouterAutosurvRefresh(PATIENT.Id);
                }
                else
                {
                    if (Création == false && MEMBERSIP.ModificationDossierPatient== true && Semaineparpage.SelectedItem != null && txtDate.SelectedDate != null && (radiotYPE1.IsChecked == true || radiotYPE2.IsChecked == true))
                    {
                        if (op.FileName != "")
                        {
                            serverfilepath = op.FileName;

                            filepath = "";
                            if (autosurv.CheminPhoto == "")
                            {
                                if (serverfilepath != "")
                                {


                                    filepath = op.FileName;

                                    serverfilepath = @"Patient\AutoSurv\" + (PATIENT.Nom + PATIENT.Prénom + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()) + ".png";
                                    byte[] buffer = null;

                                    // read the file and return the byte[
                                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, (int)fs.Length);
                                    }
                                    if (buffer != null)
                                    {
                                        proxy.UploadDocument(serverfilepath, buffer);
                                    }
                                    autosurv.CheminPhoto = serverfilepath;
                                }
                                else
                                {

                                    autosurv.CheminPhoto = "";
                                }
                            }
                            else
                            {
                                if (serverfilepath == "")
                                {
                                    autosurv.CheminPhoto = "";
                                }
                                else
                                {

                                    filepath = op.FileName;

                                    serverfilepath = @"Patient\AutoSurv\" + (PATIENT.Nom + PATIENT.Prénom + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()) + ".png";
                                    byte[] buffer = null;

                                    // read the file and return the byte[
                                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, (int)fs.Length);
                                    }
                                    if (buffer != null)
                                    {
                                        proxy.UploadDocument(serverfilepath, buffer);
                                    }
                                    autosurv.CheminPhoto = serverfilepath;
                                }
                            }
                        }
                        else
                        {
                            autosurv.CheminPhoto = "";
                        }
                        using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (radiotYPE1.IsChecked == false || radiotYPE2.IsChecked == true)
                            {

                                autosurv.Type = "Type 2";
                            }
                            else
                            {
                                if (radiotYPE1.IsChecked == true || radiotYPE2.IsChecked == false)
                                {


                                    autosurv.Type = "Type 1";
                                }
                            }
                            proxy.Updateautosurveillance(autosurv);
                            ts.Complete();
                            MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.OperationSuccées, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        proxy.AjouterAutosurvRefresh(PATIENT.Id);
                    }
                    else
                    {
                        MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(GestionClinique.Properties.Resources.MessageBoxPrivilége, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }catch(Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
