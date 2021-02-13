using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Toolkit.Uwp.UI.Animations;

using TrafficCams.Core.Models;
using TrafficCams.Core.Services;
using TrafficCams.Helpers;
using TrafficCams.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TrafficCams.Views
{
    public sealed partial class ImageGalleryPage : Page, INotifyPropertyChanged
    {
        public readonly UpdateService _updateService;

        public const string ImageGallerySelectedIdKey = "ImageGallerySelectedIdKey";

        public ObservableCollection<CameraPlacemarkModel> Source { get; } = new ObservableCollection<CameraPlacemarkModel>();

        public ImageGalleryPage()
        {

            InitializeComponent();
            _updateService = new UpdateService();
            _updateService.RequestCallback += UpdateRequestCallback;

            Loaded += ImageGalleryPage_OnLoaded;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        }

        private async void UpdateRequestCallback()
        {
            var data = await CameraXmlParser.GetCamsFromXml();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                foreach (var item in data)
                {
                    var c = Source.Where(camitem => camitem.ID == item.ID).FirstOrDefault();
                    if (c != null)
                    {
                        Source.Remove(c);
                        Source.Add(item);
                    }
                }
            });

        }

        private async void ImageGalleryPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Source.Clear();
            // TODO WTS: Replace this with your actual data
            var data = await CameraXmlParser.GetCamsFromXml();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        private void UpdateCamsAsync()
        {

        }

        private void ImagesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as CameraPlacemarkModel;
            ImagesNavigationHelper.AddImageId(ImageGallerySelectedIdKey, selected.ID);
            NavigationService.Frame.SetListDataItemForNextConnectedAnimation(selected);
            NavigationService.Navigate<ImageGalleryDetailPage>(selected.ID);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
