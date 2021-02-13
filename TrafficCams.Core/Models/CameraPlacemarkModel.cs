using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media.Imaging;

namespace TrafficCams.Core.Models
{
    public class CameraPlacemarkModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public Uri source { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public BasicGeoposition BasicGeoPos { get; set; }
        public BitmapImage ImageBmp
        {
            get => new BitmapImage(source);
            set
            {
                ImageBmp = value;
            }
        }
    }
}
