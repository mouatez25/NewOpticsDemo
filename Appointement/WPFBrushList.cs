using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewOptics.Appointement
{
    class WPFBrushList : List<WPFBrush>
    {
        public WPFBrushList()
        {
            // Get type of the Brushes
            Type BrushesType = typeof(Brushes);
            // Get properties of this type
            PropertyInfo[] brushesProperty = BrushesType.GetProperties();
            // Extract Name and Hex code and add to list (binding class)
            foreach (PropertyInfo property in brushesProperty)
            {
                BrushConverter brushConverter = new BrushConverter();
                Brush brush = (Brush)brushConverter.ConvertFromString(property.Name);
                Add(new WPFBrush(property.Name, brush.ToString()));
            }
        }
    }
    class WPFBrush
    {
        public WPFBrush(string name, string hex)
        {
            Name = name;
            Hex = hex;
        }
        //please note name of properties are same as DataTemplate Binding Paths 
        public string Name { get; set; }
        public string Hex { get; set; }
    }
}
