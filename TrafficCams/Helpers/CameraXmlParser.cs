using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrafficCams.Core.Models;
using Windows.Data.Xml.Dom;

namespace TrafficCams.Helpers
{
    public static class CameraXmlParser
    {
        private static string XmlFileName = "cameres.xml";
        private static string reg_expr = "src=\".*(jpg|jpeg|gif)(\\?a=1)?\"";

        public static async Task<IList<CameraPlacemarkModel>> GetCamsFromXml()
        {
            IList<CameraPlacemarkModel> cameras = new List<CameraPlacemarkModel>();

            try
            {
                var xmlfile = await Windows.Storage.StorageFile
                    .GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + XmlFileName));

                XmlDocument xdoc = await XmlDocument.LoadFromFileAsync(xmlfile);
                var placemarkNodes = xdoc.GetElementsByTagName("Placemark");

                foreach (var node in placemarkNodes)
                {
                    string name = node.ChildNodes.Where(n => n.NodeName.Equals("name")).FirstOrDefault().InnerText;

                    string description = node.ChildNodes.Where(n => n.NodeName.Equals("description")).FirstOrDefault().InnerText;
                    string url = Regex.Match(description, reg_expr).Value.Replace("src=", "").Replace("\"", "");

                    var coord = node.ChildNodes.Where(n => n.NodeName.Equals("Point")).FirstOrDefault().ChildNodes
                        .Where(n => n.NodeName.Equals("coordinates")).FirstOrDefault().InnerText.Split(',');

                    double.TryParse(coord[0], out var p_a);
                    double.TryParse(coord[1], out var p_b);

                    cameras.Add(new CameraPlacemarkModel()
                    {
                        ID = cameras.Count().ToString(),
                        Name = name,
                        source = new Uri(url),
                        longitude = coord[0],
                        latitude = coord[1],
                        BasicGeoPos = new Windows.Devices.Geolocation.BasicGeoposition()
                        {
                            Latitude = p_b, Longitude = p_a, Altitude = 0.0
                        }

                    });
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            return cameras;
        }
    }
}
