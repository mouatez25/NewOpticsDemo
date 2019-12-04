using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Xps.Packaging;

using Microsoft.Win32;

using System.IO.Packaging;
using System.Windows.Resources;
using System.Reflection;
using DevExpress.Xpf.Core;

namespace GestionClinique.DocumentImportant
{
    /// <summary>
    /// Interaction logic for LivreViewer.xaml
    /// </summary>
    public partial class LivreViewer : DXWindow
    {
        public LivreViewer()
        {
            try
            {

                InitializeComponent();
            System.IO.Stream docStream = new MemoryStream(GestionClinique.Properties.Resources.Alimentation);
              
                    Package package = Package.Open(docStream);

                    //Create URI for Xps Package
                    //Any Uri will actually be fine here. It acts as a place holder for the
                    //Uri of the package inside of the PackageStore
                    string inMemoryPackageName = string.Format("memorystream://{0}.xps", Guid.NewGuid());
                    Uri packageUri = new Uri(inMemoryPackageName);

                    //Add package to PackageStore
                    PackageStore.AddPackage(packageUri, package);

                    XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.Maximum, inMemoryPackageName);
                    if (xpsDoc.IsReader == true)
                    {
                        //  FixedDocumentSequence fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();
                        // documentviewWord.Document = fixedDocumentSequence;
                        documentviewWord.Document = xpsDoc.GetFixedDocumentSequence();
                    
                    }


           

                // Do operations on xpsDoc here

                //Note: Please note that you must keep the Package object in PackageStore until you
                //are completely done with it since certain operations on XpsDocument can trigger
                //delayed resource loading from the package.

                //  PackageStore.RemovePackage(packageUri);
                //xpsDoc.Close();




            }
            catch (Exception ex)
            {
                MessageBoxResult results = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, GestionClinique.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void CommandBinding_CanExecutePrint(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
        }
        private void CommandBinding_CanExecuteCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
        }




    }
}
