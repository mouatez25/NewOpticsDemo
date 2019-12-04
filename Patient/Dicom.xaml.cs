using DevExpress.Xpf.Core;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GestionClinique.Patient
{
    /// <summary>
    /// Interaction logic for Dicom.xaml
    /// </summary>
    public partial class Dicom : DXWindow
    {
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerDicom();
       
        public enum ImageBitsPerPixel { Eight, Sixteen, TwentyFour };
        public enum ViewSettings { Zoom1_1, ZoomToFit };
        DicomDecoder dd;
        List<byte> pixels8;
        List<ushort> pixels16;
        List<byte> pixels24; // 30 July 2010
        int imageWidth;
        int imageHeight;
        int bitDepth;
        int samplesPerPixel;  // Updated 30 July 2010
        bool imageOpened;
        BinaryReader readBinary;
        double winCentre;
        double winWidth;
        bool signedImage;
        int maxPixelValue;    // Updated July 2012
        int minPixelValue;
        SVC.DicomFichier dicom;
        public Dicom(SVC.ServiceCliniqueClient proxyrecu, SVC.DicomFichier castrecu)
        {
            try
            {

                InitializeComponent();
                proxy = proxyrecu;
                dicom = castrecu;
                dd = new DicomDecoder();
                pixels8 = new List<byte>();
                pixels16 = new List<ushort>();
                pixels24 = new List<byte>();
                imageOpened = false;
                signedImage = false;
                maxPixelValue = 0;
                minPixelValue = 65535;
                //imagePanelControl = new ImagePanelControl();
                BinaryWriter binWriter = new BinaryWriter(new MemoryStream());
                binWriter.Write(proxy.DownloadDocument(dicom.ChemainFichier.ToString()));

                readBinary = new BinaryReader(binWriter.BaseStream);
              
                ReadAndDisplayDicomFileBinaryReader(readBinary,dicom.NomFichierDicom);
              
                this.Title = dicom.ChemainFichier;


                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);

            }
            catch (Exception ex)
            {
                MessageBoxResult resultc10 = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void UpdateWindowLevel(int winWidth, int winCentre, ImageBitsPerPixel bpp)
        {
            try { 
            int winMin = Convert.ToInt32(winCentre - 0.5 * winWidth);
            int winMax = winMin + winWidth;
            this.windowLevelControl.SetWindowWidthCentre(winMin, winMax, winWidth, winCentre, bpp, signedImage);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
  
        private void ReadAndDisplayDicomFileBinaryReader(BinaryReader fileName, string fileNameOnly)
        {
            try { 
            dd.DicomFileNameBinaryReader = fileName;

            TypeOfDicomFile typeOfDicomFile = dd.typeofDicomFile;

            if (typeOfDicomFile == TypeOfDicomFile.Dicom3File ||
                typeOfDicomFile == TypeOfDicomFile.DicomOldTypeFile)
            {
                imageWidth = dd.width;
                imageHeight = dd.height;
                bitDepth = dd.bitsAllocated;
                winCentre = dd.windowCentre;
                winWidth = dd.windowWidth;
                samplesPerPixel = dd.samplesPerPixel;
                signedImage = dd.signedImage;

                imagePanelControl1.NewImage = true;

                if (samplesPerPixel == 1 && bitDepth == 8)
                {
                    pixels8.Clear();
                    pixels16.Clear();
                    pixels24.Clear();
                    dd.GetPixels8(ref pixels8);

                    // This is primarily for debugging purposes, 
                    //  to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //               "C:\\imageSigned.txt");

                    //    for (int ik = 0; ik < pixels8.Count; ++ik)
                    //        file.Write(pixels8[ik] + "  ");

                    //    file.Close();
                    //}

                    minPixelValue = pixels8.Min();
                    maxPixelValue = pixels8.Max();

                    // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
                    // Thanks to Matias Montroull from Argentina for pointing this out.
                    if (dd.signedImage)
                    {
                        winCentre -= char.MinValue;
                    }

                    if (Math.Abs(winWidth) < 0.001)
                    {
                        winWidth = maxPixelValue - minPixelValue;
                    }

                    if ((winCentre == 0) ||
                        (minPixelValue > winCentre) || (maxPixelValue < winCentre))
                    {
                        winCentre = (maxPixelValue + minPixelValue) / 2;
                    }

                    imagePanelControl1.SetParameters(ref pixels8, imageWidth, imageHeight,
                        winWidth, winCentre, samplesPerPixel, true, this);
                }

                if (samplesPerPixel == 1 && bitDepth == 16)
                {
                    pixels16.Clear();
                    pixels8.Clear();
                    pixels24.Clear();
                    dd.GetPixels16(ref pixels16);

                    // This is primarily for debugging purposes, 
                    //  to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //               "C:\\imageSigned.txt");

                    //    for (int ik = 0; ik < pixels16.Count; ++ik)
                    //        file.Write(pixels16[ik] + "  ");

                    //    file.Close();
                    //}

                    minPixelValue = pixels16.Min();
                    maxPixelValue = pixels16.Max();

                    // Bug fix dated 24 Aug 2013 - for proper window/level of signed images
                    // Thanks to Matias Montroull from Argentina for pointing this out.
                    if (dd.signedImage)
                    {
                        winCentre -= short.MinValue;
                    }

                    if (Math.Abs(winWidth) < 0.001)
                    {
                        winWidth = maxPixelValue - minPixelValue;
                    }

                    if ((winCentre == 0) ||
                        (minPixelValue > winCentre) || (maxPixelValue < winCentre))
                    {
                        winCentre = (maxPixelValue + minPixelValue) / 2;
                    }

                    imagePanelControl1.Signed16Image = dd.signedImage;

                    imagePanelControl1.SetParameters(ref pixels16, imageWidth, imageHeight,
                        winWidth, winCentre, true, this);
                }

                if (samplesPerPixel == 3 && bitDepth == 8)
                {
                    // This is an RGB colour image
                    pixels8.Clear();
                    pixels16.Clear();
                    pixels24.Clear();
                    dd.GetPixels24(ref pixels24);

                    // This code segment is primarily for debugging purposes, 
                    //    to view the pixel values as ascii data.
                    //if (true)
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(
                    //                      "C:\\image24.txt");

                    //    for (int ik = 0; ik < pixels24.Count; ++ik)
                    //        file.Write(pixels24[ik] + "  ");

                    //    file.Close();
                    //}

                    imagePanelControl1.SetParameters(ref pixels24, imageWidth, imageHeight,
                        winWidth, winCentre, samplesPerPixel, true, this);
                }
            }
            else
            {
                if (typeOfDicomFile == TypeOfDicomFile.DicomUnknownTransferSyntax)
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, I can't read a DICOM file with this Transfer Syntax.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Sorry, I can't open this file. " +
                        "This file does not appear to contain a DICOM image.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //    Text = "DICOM Image Viewer: ";
                // Show a plain grayscale image instead
                pixels8.Clear();
                pixels16.Clear();
                pixels24.Clear();
                samplesPerPixel = 1;

                imageWidth = imagePanelControl1.Width - 25;   // 25 is a magic number
                imageHeight = imagePanelControl1.Height - 25; // Same magic number
                int iNoPix = imageWidth * imageHeight;

                for (int i = 0; i < iNoPix; ++i)
                {
                    pixels8.Add(240);// 240 is the grayvalue corresponding to the Control colour
                }
                winWidth = 256;
                winCentre = 127;
                imagePanelControl1.SetParameters(ref pixels8, imageWidth, imageHeight,
                    winWidth, winCentre, samplesPerPixel, true, this);
                imagePanelControl1.Invalidate();

                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Stop);

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
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDicom(HandleProxy));
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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerDicom(HandleProxy));
                return;
            }
            HandleProxy();
        }

        private void btntest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BinaryWriter binWriter = new BinaryWriter(new MemoryStream());
                binWriter.Write(proxy.DownloadDocument(dicom.ChemainFichier.ToString()));

                readBinary = new BinaryReader(binWriter.BaseStream);
                if (readBinary != null)
                {
                    ReadAndDisplayDicomFileBinaryReader(readBinary, dicom.NomFichierDicom);
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            
            }


        }

        private void rbZoom1_1_Checked(object sender, RoutedEventArgs e)
        {
            if (rbZoom1_1.IsChecked ==true)
            {
             //   imagePanelControl1.viewSettings = ViewSettings.Zoom1_1;
            }
            else
            {
              //  imagePanelControl1.viewSettings = ViewSettings.ZoomToFit;
            }

            imagePanelControl1.viewSettingsChanged = true;
            imagePanelControl1.Invalidate();
        }

        private void rbZoomfit_Checked(object sender, RoutedEventArgs e)
        {
            if (rbZoom1_1.IsChecked == true)
            {
             //   imagePanelControl1.viewSettings = ViewSettings.Zoom1_1;
            }
            else
            {
             //   imagePanelControl1.viewSettings = ViewSettings.ZoomToFit;
            }

            imagePanelControl1.viewSettingsChanged = true;
            imagePanelControl1.Invalidate();
        }
    }
}
