using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrafficCams.Core.Services;

namespace TrafficCams.Core.Tests.MSTest
{
    // TODO WTS: Add appropriate unit tests.
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        // TODO WTS: Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [TestMethod]
        public async Task EnsureSampleDataServiceReturnsImageGalleryDataAsync()
        {
            var actual = await SampleDataService.GetImageGalleryDataAsync("ms-appx:///Assets");

            Assert.AreNotEqual(0, actual.Count());
        }
    }
}
