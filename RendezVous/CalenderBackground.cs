using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GestionClinique.RendezVous
{
    internal class CalenderBackground
    {
        private readonly List<Dates> datelist = new List<Dates>();
        private readonly List<Overlays> overlaylist = new List<Overlays>();
        public string grayoutweekends { get; set; }

        private Calendar _calendar;

        public CalenderBackground()
        {
        }

        public CalenderBackground(Calendar cal)
        {
            _calendar = cal;
        }

        public void SetCalenderBackground(Calendar cal)
        {
            _calendar = cal;
        }

        public void ClearDates()
        {
            datelist.Clear();
        }

        public void AddOverlay(string _id, string _filename)
        {
            overlaylist.Add(new Overlays(_id, _filename));
        }

        public void AddDate(DateTime _date, string _overlay)
        {
            datelist.Add(new Dates(_date, _overlay));
        }

        public void RemoveDate(DateTime _date, string _overlay)
        {
            datelist.Remove(new Dates(_date, _overlay));
        }

        public ImageBrush GetBackground()
        {
            // Calculate the first shown date in the calendar
            DateTime displaydate = _calendar.DisplayDate;
            var firstdayofmonth = new DateTime(displaydate.Year, displaydate.Month, 1);
            var dayofweek = (int)firstdayofmonth.DayOfWeek;
            if (dayofweek == 0) dayofweek = 7; // set sunday to day 7.
            if (dayofweek == (int)_calendar.FirstDayOfWeek) dayofweek = 8; // show a whole week ahead
            if (_calendar.FirstDayOfWeek == DayOfWeek.Sunday) dayofweek += 1;
            DateTime firstdate = firstdayofmonth.AddDays(-((Double)dayofweek) + 1);

            Debug.WriteLine("displayd date    {0} ", displaydate);
            Debug.WriteLine("firstdayofmonth  {0} ", firstdayofmonth);
            Debug.WriteLine("dayofweek        {0} ", dayofweek);
            Debug.WriteLine("firstdate        {0} ", firstdate);

            // Create default background image
            var rtBitmap = new RenderTargetBitmap(178 /* PixelWidth */, 160 /* PixelHeight */, /*96*/0 /* DpiX */, /*96*/0 /* DpiY */, PixelFormats.Default);

            var drawVisual = new DrawingVisual();
            using (DrawingContext dc = drawVisual.RenderOpen())
            {
                var backGroundBrush = new LinearGradientBrush();
                backGroundBrush.StartPoint = new Point(0.5, 0);
                backGroundBrush.EndPoint = new Point(0.5, 1);
                backGroundBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFE4EAF0"), 0.0));
                backGroundBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFECF0F4"), 0.16));
                backGroundBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFCFCFD"), 0.16));
                backGroundBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFFFFF"), 1));

                dc.DrawRectangle(backGroundBrush, null, new Rect(0, 0, rtBitmap.Width, rtBitmap.Height));
            }
            rtBitmap.Render(drawVisual);

            using (DrawingContext dc = drawVisual.RenderOpen())
            {
                for (int y = 0; y < 6; y++)
                    for (int x = 0; x < 7; x++)
                    {
                        int xpos = x * 26 ;
                        int ypos = y * 25+20 ;
                        if (y == 2) ypos -= 1;
                        if (y == 3) ypos -= 2;
                        if (y == 4) ypos -= 2;
                        if (y == 5) ypos -= 3;

                        foreach (string overlayid in datelist.Where(c => c.date == firstdate).Select(c => c.overlay))
                        {

                            if (overlayid != null)
                            {
                                Overlays overlays = overlaylist.Where(c => c.id == overlayid).FirstOrDefault();

                                try
                                {
                                    dc.DrawRectangle(overlays.Brush, null, new Rect(xpos, ypos, overlays.BitMap.Width, overlays.BitMap.Height));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }

                        if (grayoutweekends != "" && (firstdate.DayOfWeek == DayOfWeek.Saturday || firstdate.DayOfWeek == DayOfWeek.Sunday))
                        {
                            Overlays overlays = overlaylist.Where(c => c.id == grayoutweekends).FirstOrDefault();

                            try
                            {
                                dc.DrawRectangle(overlays.Brush, null /* no pen */,
                                                 new Rect(xpos, ypos, overlays.BitMap.Width, overlays.BitMap.Height));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                        firstdate = firstdate.AddDays(1);
                    }
            }

            rtBitmap.Render(drawVisual);


            var brush = new ImageBrush(rtBitmap); // create a brush using the BitMap 
            return brush;
        }

        private static BitmapImage BitmapImage(string filename, out ImageBrush imageBrush)
        {
            var overlay = new BitmapImage(new Uri(filename, UriKind.Relative));
            imageBrush = new ImageBrush();
            imageBrush.ImageSource = overlay;
            imageBrush.Stretch = Stretch.Uniform;
            imageBrush.TileMode = TileMode.None;
            imageBrush.AlignmentX = AlignmentX.Center;
            imageBrush.AlignmentY = AlignmentY.Center;
            imageBrush.Opacity = 0.75;
            return overlay;
        }

        private class Dates
        {
            public Dates(DateTime _date, string _overlay)
            {
                date = _date;
                overlay = _overlay;
            }

            public DateTime date { get; set; }
            public string overlay { get; set; }
        }

        private class Overlays
        {
            public readonly BitmapImage BitMap;
            public readonly ImageBrush Brush;

            public Overlays(string _id, string _filename)
            {
                id = _id;

                BitMap = BitmapImage(_filename, out Brush);
            }

            public string id { get; set; }
        }
    }
}
