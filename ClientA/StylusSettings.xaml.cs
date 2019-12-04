using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Reflection;                    //For : Obtaining brushes through reflection
using System.Windows.Ink;

using System.Windows.Threading;
using System.ServiceModel;

namespace NewOptics.ClientA
{
    /// <summary>
    /// Interaction logic for StylusSettings.xaml
    /// </summary>
    public partial class StylusSettings : Window
    {
        Color currColor = Colors.Black;
        double penWidth = 2;
        double penHeight = 2;
        SVC.ServiceCliniqueClient proxy;
        private delegate void FaultedInvokerPallete();
        public StylusSettings(SVC.ServiceCliniqueClient proxyrecu)
        {
            try
            {
                this.InitializeComponent();
                proxy = proxyrecu;
                // Insert code required on object creation below this point.
                createGridOfColor();
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);

                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
            }
            catch(Exception ex)
            {
                HandleProxy();
            }
        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerPallete(HandleProxy));
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
        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerPallete(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void createGridOfColor()
        {


            PropertyInfo[] props = typeof(Brushes).GetProperties(BindingFlags.Public |
                                                  BindingFlags.Static);
            // Create individual items
            foreach (PropertyInfo p in props)
            {
                Button b = new Button();
                b.Background = (SolidColorBrush)p.GetValue(null, null);
                b.Foreground = Brushes.Transparent;
                b.BorderBrush = Brushes.Transparent;
                b.Click += new RoutedEventHandler(b_Click);
                this.ugColors.Children.Add(b);
            }
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush sb = (SolidColorBrush)(sender as Button).Background;
            currColor = sb.Color;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


        // Public property initializes controls and returns their values.
        public DrawingAttributes DrawingAttributes
        {
            set
            {
              //  chkPressure.IsChecked = value.IgnorePressure;
              //  chkHighlight.IsChecked = value.IsHighlighter;
                penWidth = value.Width;
                penHeight = value.Height;
                currColor = value.Color;
            }
            get
            {
                DrawingAttributes drawattr = new DrawingAttributes();
            //    drawattr.IgnorePressure = (bool)chkPressure.IsChecked;
                drawattr.Width = penWidth;
                drawattr.Height = penHeight;
             //   drawattr.IsHighlighter = (bool)chkHighlight.IsChecked;
                drawattr.Color = currColor;
                return drawattr;
            }
        }

    }
}
