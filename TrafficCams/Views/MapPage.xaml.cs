using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TrafficCams.Core.Models;
using TrafficCams.Helpers;
using TrafficCams.Services;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace TrafficCams.Views
{
    public sealed partial class MapPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set your preferred default zoom level
        private const double DefaultZoomLevel = 17;

        private readonly LocationService _locationService;

        // TODO WTS: Set your preferred default location if a geolock can't be found.
        private readonly BasicGeoposition _defaultPosition = new BasicGeoposition()
        {
            Latitude = 41.46672443,
           
            Longitude = 2.17446844
        };

        private double _zoomLevel;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { Set(ref _zoomLevel, value); }
        }

        private Geopoint _center;

        private IList<CameraPlacemarkModel> CamPoints = new List<CameraPlacemarkModel>();
        public Geopoint Center
        {
            get { return _center; }
            set { Set(ref _center, value); }
        }

        public MapPage()
        {
            _locationService = new LocationService();
            Center = new Geopoint(_defaultPosition);
            ZoomLevel = DefaultZoomLevel;
            InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Cleanup();
        }

        public async Task InitializeAsync()
        {
            if (mapControl != null)
            {
                // TODO WTS: Set your map service token. If you don't have one, request from https://www.bingmapsportal.com/
                // mapControl.MapServiceToken = string.Empty;
                //AddMapIcon(Center, "Map_YourLocation".GetLocalized());
                LoadCamsAsync();
            }

            if (_locationService != null)
            {
                _locationService.PositionChanged += LocationService_PositionChanged;

                var initializationSuccessful = await _locationService.InitializeAsync();

                if (initializationSuccessful)
                {
                    await _locationService.StartListeningAsync();
                }

                if (initializationSuccessful && _locationService.CurrentPosition != null)
                {
                    Center = _locationService.CurrentPosition.Coordinate.Point;
                }
                else
                {
                    var i_pos = CamPoints.Any() ? CamPoints.FirstOrDefault()?.BasicGeoPos?? _defaultPosition : _defaultPosition;
                    Center = new Geopoint(i_pos);
                }
            }
        }

        private async void LoadCamsAsync()
        {
            if (!CamPoints.Any())
            {
                CamPoints = await CameraXmlParser.GetCamsFromXml();
                var g_points = CamPoints.Select(c => 
                {
                    return (gp: new Geopoint(c.BasicGeoPos), name: c.Name);
                });

                foreach (var g_point in g_points)
                {
                    AddMapIcon(g_point.gp, g_point.name);
                }
           }
        }

        public void Cleanup()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged -= LocationService_PositionChanged;
                _locationService.StopListening();
            }
        }

        private void LocationService_PositionChanged(object sender, Geoposition geoposition)
        {
            if (geoposition != null)
            {
                Center = geoposition.Coordinate.Point;
            }
        }

        private void AddMapIcon(Geopoint position, string title)
        {
            MapIcon mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = title,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/map.png")),
                ZIndex = 0
            };

            mapControl.MapElements.Add(mapIcon);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void MyMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MapIcon myClickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            if(myClickedIcon != null)
            {
                var posClicked = myClickedIcon.Location.Position;
                var c_p = CamPoints.Where(c => c.BasicGeoPos.Equals(posClicked)).FirstOrDefault();


                if (c_p != null)
                {
                    ImagesNavigationHelper.AddImageId("ImageGallerySelectedIdKey", c_p.ID);
                    NavigationService.Frame.SetListDataItemForNextConnectedAnimation(c_p);
                    NavigationService.Navigate<ImageGalleryDetailPage>(c_p.ID);
                }
            }
        }
    }
}
