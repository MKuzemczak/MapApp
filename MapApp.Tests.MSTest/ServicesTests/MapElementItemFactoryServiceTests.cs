using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapApp.Models;
using MapApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.Devices.Geolocation;
using Windows.UI;

namespace MapApp.Tests.MSTest.ServicesTests
{
    [TestClass]
    public class MapElementItemFactoryServiceTests
    {
        [UITestMethod]
        public void GetMapIconItem_ConstructsMapIconItemCorrectly()
        {

            var expectName = "TestName";
            var expectPosition = new BasicGeoposition() { Latitude = 1, Longitude = 2 };
            var expectLayer = new MapLayerItem() { Id = 3, Name = "TestLayer" };
            int expectId = 4;

            var icon = MapElementItemFactoryService.GetMapIconItem(expectName, expectPosition, expectLayer, expectId);

            Assert.AreEqual(expectName, icon.Name);
            Assert.AreEqual(expectPosition, icon.GetPosition());
            Assert.AreEqual(expectLayer, icon.ParentLayer);
            Assert.AreEqual(expectId, icon.Id);
        }

        [UITestMethod]
        public void  GetMapPolylineItem_ConstructsMapPolylineItemCorrectly()
        {
            var expectName = "TestName";
            var expectPath = new List<BasicGeoposition>()
            {
                new BasicGeoposition() { Latitude = 1, Longitude = 2 },
                new BasicGeoposition() { Latitude = 2, Longitude = 3 },
                new BasicGeoposition() { Latitude = 3, Longitude = 4 }
            };
            var expectLayer = new MapLayerItem() { Id = 3, Name = "TestLayer" };
            int expectId = 5;
            double expectWidth = 0.00006;
            var expectStrokeColor = Color.FromArgb(100, 100, 100, 100);

            var polyline = MapElementItemFactoryService.GetMapPolylineItem(
                expectName, expectPath, expectLayer, expectStrokeColor, expectWidth, expectId);

            Assert.AreEqual(expectName, polyline.Name);
            Assert.AreEqual(expectLayer, polyline.ParentLayer);
            Assert.AreEqual(expectId, polyline.Id);
            Assert.AreEqual(expectWidth, polyline.Width, 0.00000001);
            Assert.AreEqual(expectStrokeColor, polyline.StrokeColor);
            Assert.IsTrue(expectPath.SequenceEqual(polyline.Path));
        }

        [UITestMethod]
        public void GetMapPolygonItem_ConstructMapPolygonItemCorrectly()
        {
            var expectName = "TestName";
            var expectPath = new List<BasicGeoposition>()
            {
                new BasicGeoposition() { Latitude = 1, Longitude = 2 },
                new BasicGeoposition() { Latitude = 2, Longitude = 3 },
                new BasicGeoposition() { Latitude = 3, Longitude = 4 }
            };
            var expectLayer = new MapLayerItem() { Id = 3, Name = "TestLayer" };
            int expectId = 5;
            var expectStrokeColor = Color.FromArgb(100, 100, 100, 100);
            var expectFillColor = Color.FromArgb(2, 2, 2, 2);

            var polygon = MapElementItemFactoryService.GetMapPolygonItem(
                expectName, expectPath, expectLayer, expectStrokeColor, expectFillColor, expectId);

            Assert.AreEqual(expectName, polygon.Name);
            Assert.AreEqual(expectLayer, polygon.ParentLayer);
            Assert.AreEqual(expectId, polygon.Id);
            Assert.AreEqual(expectStrokeColor, polygon.StrokeColor);
            Assert.AreEqual(expectFillColor, polygon.FillColor);
            Assert.IsTrue(expectPath.SequenceEqual(polygon.Path));
        }
    }
}
