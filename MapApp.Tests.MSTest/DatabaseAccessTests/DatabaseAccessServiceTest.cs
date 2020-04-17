using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using MapApp.DatabaseAccess;
using MapApp.Helpers;
using MapApp.Models;
using MapApp.Services;
using Windows.Storage;
using Windows.Devices.Geolocation;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI;

namespace MapApp.Tests.MSTest.DatabaseAccessTests
{
    [TestClass]
    public class DatabaseAccessServiceTest
    {
        [TestMethod]
        public async Task InitializeAsync_SetInitializedToTrue()
        {
            await DatabaseAccessService.InitializeAsync();

            Assert.IsTrue(DatabaseAccessService.Initialized, "The \"DatabaseAccessService.Initialized\" property was not set to true.");
        }

        [TestMethod]
        public async Task GetDbConnectionAsync_ReturnsValidSQLiteConnection()
        {
            using (var connection = await DatabaseAccessService.GetDbConnectionAsync())

            {
                Assert.IsNotNull(connection, "Returned connection is null.");
                connection.Open();
                Assert.AreEqual(DatabaseAccessService.DbFile, connection.FileName, $@"Connection to a wrong DB file.\n
                                                                                Expected: {DatabaseAccessService.DbFile},\n
                                                                                Actual: {connection.FileName}");
            }
        }

        [TestMethod]
        public async Task CreateDatabaseAsync_CreatesCorrectDatabase()
        {
            await DatabaseAccessService.CreateDatabaseAsync();

            using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
            {
                cnn.Open();
                var queryResult1 = (cnn.Query("SELECT * FROM Layer")).ToList();
                Assert.IsNotNull(queryResult1, "\"SELECT * FROM Layer\" query returned null.");

                var queryResult2 = (cnn.Query("SELECT * FROM MapElement")).ToList();
                Assert.IsNotNull(queryResult1, "\"SELECT * FROM MapElement\" query returned null.");

                var queryResult3 = (cnn.Query("SELECT * FROM Geoposition")).ToList();
                Assert.IsNotNull(queryResult1, "\"SELECT * FROM Geoposition\" query returned null.");
            }

            Assert.IsTrue(DatabaseAccessService.Created,
                "DatabaseAccessService.CreateDatabaseAsync doesnt set DatabaseAccessService.Created to true.");
        }

        [TestMethod]
        public async Task InsertLayerAsync_InsertsLayerCorrectly()
        {
            await DatabaseAccessService.DeleteDatabase();
            await DatabaseAccessService.CreateDatabaseAsync();

            var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };

            await DatabaseAccessService.InsertLayerAsync(insertedLayer);

            using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
            {
                cnn.Open();

                var queriedLayerList = (cnn.Query<MapLayerItem>($"SELECT * FROM Layer WHERE Id={insertedLayer.Id}")).ToList();
                Assert.AreEqual(1, queriedLayerList.Count, $@"Database query for inserted layer returned list with number of entries
                                                            different than one.
                                                            Expected: 1,
                                                            Actual: {queriedLayerList.Count}.");

                var queriedLayer = queriedLayerList.First();

                Assert.IsNotNull(queriedLayer, "Database query for inserted layer returned null.");
                Assert.AreEqual(insertedLayer.Id, queriedLayer.Id, $@"Database query for inserted layer returned layer with differrent Id.
                                                                    Expected: {insertedLayer.Id},
                                                                    Actual: {queriedLayer.Id}.");
                Assert.AreEqual(insertedLayer.Name, queriedLayer.Name, $@"Database query for inserted layer returned layer with different Name.
                                                                        Expected: {insertedLayer.Name},
                                                                        Actual: {queriedLayer.Name}.");
            }
        }

        [TestMethod]
        public async Task GetLayersAsync_ReturnLayerListCorrectly()
        {
            await DatabaseAccessService.DeleteDatabase();
            await DatabaseAccessService.CreateDatabaseAsync();

            var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };

            await DatabaseAccessService.InsertLayerAsync(insertedLayer);
            var returnedList = await DatabaseAccessService.GetLayersAsync();

            Assert.IsNotNull(returnedList, "Returned object is null");
            Assert.AreEqual(1, returnedList.Count, $@"Returned list count is wrong.
                                                    Expected: {1},
                                                    Actual: {returnedList.Count}.");
            var returnedLayer = returnedList.First();

            Assert.IsNotNull(returnedLayer, "Database query for inserted layer returned null.");
            Assert.AreEqual(insertedLayer.Id, returnedLayer.Id, $@"Database query for inserted layer returned layer with differrent Id.
                                                                    Expected: {insertedLayer.Id},
                                                                    Actual: {returnedLayer.Id}.");
            Assert.AreEqual(insertedLayer.Name, returnedLayer.Name, $@"Database query for inserted layer returned layer with different Name.
                                                                        Expected: {insertedLayer.Name},
                                                                        Actual: {returnedLayer.Name}.");
        }

        [TestMethod]
        public async Task InsertMapIconItemAsync_InsertsElementCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();

            // using dispatcher to run the test in ui thread.
            // using UITestMethod attribute was umpossible because it
            // doesn't support async
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();


                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var insertedIcon = MapElementItemFactoryService.GetMapIconItem(
                            "Test Icon",
                            new BasicGeoposition() { Latitude = 1, Longitude = 2 },
                            insertedLayer,
                            123);

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapIconItemAsync(insertedIcon);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = (cnn.Query<dynamic>($"SELECT * FROM MapElement WHERE Id={insertedIcon.Id}")).ToList();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement returned list with number of entries
                                                            different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First();

                            Assert.IsNotNull(queriedMapElement, "Database query for inserted MapElement returned null.");
                            Assert.AreEqual(insertedIcon.Id, queriedMapElement.Id, $@"Database query for inserted MapElement returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedIcon.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedIcon.Name, queriedMapElement.Name, $@"Database query for inserted MapElement returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedIcon.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.Layer_Id, $@"Database query for inserted MapElement returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.Layer_id}.");
                        }
                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });

            await taskSource.Task;
        }

        [TestMethod]
        public async Task GetMapIconItemsAsync_ReturnsIconListCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase(); 
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var insertedIcon = MapElementItemFactoryService.GetMapIconItem(
                            "Test Icon",
                            new BasicGeoposition() { Latitude = 1, Longitude = 2 },
                            insertedLayer,
                            123);

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapIconItemAsync(insertedIcon);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = await DatabaseAccessService.GetMapIconItemsAsync();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement
                                                            returned list with number of entries different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First();

                            Assert.IsNotNull(queriedMapElement, "Map element is null.");
                            Assert.AreEqual(insertedIcon.Id, queriedMapElement.Id, $@"Method returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedIcon.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedIcon.Name, queriedMapElement.Name, $@"Method returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedIcon.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.ParentLayer.Id, $@"Method returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.ParentLayer.Id}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }    
                });
            await taskSource.Task;
        }

        [TestMethod]
        public async Task InsertMapPolylineItemAsync_InsertsMapElementCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var path = new List<BasicGeoposition>()
                        { 
                            new BasicGeoposition() {Latitude = 1, Longitude = 2 },
                            new BasicGeoposition() { Latitude = 3, Longitude = 4}
                        };

                        var insertedPolyline = MapElementItemFactoryService.GetMapPolylineItem(
                            "Test polyline", path, insertedLayer, Color.FromArgb(0, 0, 0, 0), 0.00001);

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapPolylineItemAsync(insertedPolyline);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = (cnn.Query<dynamic>($"SELECT * FROM MapElement WHERE Id={insertedPolyline.Id}")).ToList();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement returned list with number of entries
                                                            different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First();

                            Assert.IsNotNull(queriedMapElement, "Database query for inserted MapElement returned null.");
                            Assert.AreEqual(insertedPolyline.Id, queriedMapElement.Id, $@"Database query for inserted MapElement returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedPolyline.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedPolyline.Name, queriedMapElement.Name, $@"Database query for inserted MapElement returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedPolyline.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.Layer_Id, $@"Database query for inserted MapElement returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.Layer_id}.");
                            Assert.AreEqual(insertedPolyline.Width, queriedMapElement.Width, $@"Database query for inserted MapElement returned
                                                                                MapElement with different width.
                                                                                Expected: {insertedPolyline.Width},
                                                                                Actual: {queriedMapElement.Width}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestMethod]
        public async Task GetMapPolylineItemsAsync_ReturnsMapElementListCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var path = new List<BasicGeoposition>()
                        {
                            new BasicGeoposition() {Latitude = 1, Longitude = 2 },
                            new BasicGeoposition() { Latitude = 3, Longitude = 4}
                        };

                        var insertedPolyline = MapElementItemFactoryService.GetMapPolylineItem(
                            "Test polyline", path, insertedLayer, Color.FromArgb(0, 0, 0, 0), 0.00001);

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapPolylineItemAsync(insertedPolyline);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = await DatabaseAccessService.GetMapPolylineItemsAsync();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement
                                                            returned list with number of entries different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First();

                            Assert.IsNotNull(queriedMapElement, "Map element is null.");
                            Assert.AreEqual(insertedPolyline.Id, queriedMapElement.Id, $@"Method returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedPolyline.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedPolyline.Name, queriedMapElement.Name, $@"Method returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedPolyline.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.ParentLayer.Id, $@"Method returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.ParentLayer.Id}.");
                            Assert.AreEqual(insertedPolyline.Width, queriedMapElement.Width, $@"Method returned
                                                                                MapElement with different width.
                                                                                Expected: {insertedPolyline.Width},
                                                                                Actual: {queriedMapElement.Width}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestMethod]
        public async Task InsertMapPolygonItemAsync_InsertsMapElementProperly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var path = new List<BasicGeoposition>()
                        {
                            new BasicGeoposition() {Latitude = 1, Longitude = 2 },
                            new BasicGeoposition() { Latitude = 3, Longitude = 4},
                            new BasicGeoposition() {Latitude = 6, Longitude = 2}
                        };

                        var insertedPolygon = MapElementItemFactoryService.GetMapPolygonItem("Test polygon", path,
                            insertedLayer, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0));

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapPolygonItemAsync(insertedPolygon);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = (cnn.Query<dynamic>($"SELECT * FROM MapElement WHERE Id={insertedPolygon.Id}")).ToList();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement returned list with number of entries
                                                            different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First();

                            Assert.IsNotNull(queriedMapElement, "Database query for inserted MapElement returned null.");
                            Assert.AreEqual(insertedPolygon.Id, queriedMapElement.Id, $@"Database query for inserted MapElement returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedPolygon.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedPolygon.Name, queriedMapElement.Name, $@"Database query for inserted MapElement returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedPolygon.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.Layer_Id, $@"Database query for inserted MapElement returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.Layer_id}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestMethod]
        public async Task GetMapPolygonItemsAsync_ReturnsMapElementListCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var path = new List<BasicGeoposition>()
                        {
                            new BasicGeoposition() {Latitude = 1, Longitude = 2 },
                            new BasicGeoposition() { Latitude = 3, Longitude = 4},
                            new BasicGeoposition() {Latitude = 6, Longitude = 2}
                        };

                        var insertedPolygon = MapElementItemFactoryService.GetMapPolygonItem("Test polygon", path,
                            insertedLayer, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0));

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapPolygonItemAsync(insertedPolygon);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = await DatabaseAccessService.GetMapPolygonItemsAsync();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement
                                                            returned list with number of entries different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First();

                            Assert.IsNotNull(queriedMapElement, "Map element is null.");
                            Assert.AreEqual(insertedPolygon.Id, queriedMapElement.Id, $@"Method returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedPolygon.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedPolygon.Name, queriedMapElement.Name, $@"Method returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedPolygon.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.ParentLayer.Id, $@"Method returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.ParentLayer.Id}.");
                            Assert.AreEqual(insertedPolygon.FillColor, queriedMapElement.FillColor, $@"Method returned
                                                                                MapElement with different fill color.
                                                                                Expected: {insertedPolygon.FillColor},
                                                                                Actual: {queriedMapElement.FillColor}.");
                            Assert.AreEqual(insertedPolygon.StrokeColor, queriedMapElement.StrokeColor, $@"Method returned
                                                                                MapElement with different stroke color.
                                                                                Expected: {insertedPolygon.StrokeColor},
                                                                                Actual: {queriedMapElement.StrokeColor}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestMethod]
        public async Task GetMapElementItemsAsync_ReturnsMapElementListCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var path = new List<BasicGeoposition>()
                        {
                            new BasicGeoposition() {Latitude = 1, Longitude = 2 },
                            new BasicGeoposition() { Latitude = 3, Longitude = 4},
                            new BasicGeoposition() {Latitude = 6, Longitude = 2}
                        };

                        var insertedPolygon = MapElementItemFactoryService.GetMapPolygonItem("Test polygon", path,
                            insertedLayer, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0));

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapPolygonItemAsync(insertedPolygon);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = await DatabaseAccessService.GetMapElementItemsAsync();
                            Assert.AreEqual(1, queriedMapElementList.Count, $@"Database query for inserted MapElement
                                                            returned list with number of entries different than one.
                                                            Expected: 1,
                                                            Actual: {queriedMapElementList.Count}.");

                            var queriedMapElement = queriedMapElementList.First() as MapPolygonItem;

                            Assert.IsNotNull(queriedMapElement, "Map element is null.");
                            Assert.AreEqual(insertedPolygon.Id, queriedMapElement.Id, $@"Method returned 
                                                                    MapElement with differrent Id.
                                                                    Expected: {insertedPolygon.Id},
                                                                    Actual: {queriedMapElement.Id}.");
                            Assert.AreEqual(insertedPolygon.Name, queriedMapElement.Name, $@"Method returned 
                                                                        MapElement with different Name.
                                                                        Expected: {insertedPolygon.Name},
                                                                        Actual: {queriedMapElement.Name}.");
                            Assert.AreEqual(insertedLayer.Id, queriedMapElement.ParentLayer.Id, $@"Method returned
                                                                                MapElement with different parent layer.
                                                                                Expected: {insertedLayer.Id},
                                                                                Actual: {queriedMapElement.ParentLayer.Id}.");
                            Assert.AreEqual(insertedPolygon.FillColor, queriedMapElement.FillColor, $@"Method returned
                                                                                MapElement with different fill color.
                                                                                Expected: {insertedPolygon.FillColor},
                                                                                Actual: {queriedMapElement.FillColor}.");
                            Assert.AreEqual(insertedPolygon.StrokeColor, queriedMapElement.StrokeColor, $@"Method returned
                                                                                MapElement with different stroke color.
                                                                                Expected: {insertedPolygon.StrokeColor},
                                                                                Actual: {queriedMapElement.StrokeColor}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestMethod]
        public async Task DeleteMapElementAsync_DeletesMapElementCorrectly()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DatabaseAccessService.DeleteDatabase();
                        await DatabaseAccessService.CreateDatabaseAsync();

                        var insertedLayer = new MapLayerItem() { Id = 1234124, Name = "Test Layer" };
                        var path = new List<BasicGeoposition>()
                        {
                            new BasicGeoposition() {Latitude = 1, Longitude = 2 },
                            new BasicGeoposition() { Latitude = 3, Longitude = 4},
                            new BasicGeoposition() {Latitude = 6, Longitude = 2}
                        };

                        var insertedPolygon = MapElementItemFactoryService.GetMapPolygonItem("Test polygon", path,
                            insertedLayer, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(0, 0, 0, 0));

                        await DatabaseAccessService.InsertLayerAsync(insertedLayer);
                        await DatabaseAccessService.InsertMapPolygonItemAsync(insertedPolygon);
                        await DatabaseAccessService.DeleteMapElementAsync(insertedPolygon.Id);

                        using (var cnn = await DatabaseAccessService.GetDbConnectionAsync())
                        {
                            cnn.Open();

                            var queriedMapElementList = await DatabaseAccessService.GetMapElementItemsAsync();
                            Assert.AreEqual(0, queriedMapElementList.Count, $@"Database query for inserted MapElement
                                                            returned list with number of entries different than zero.
                                                            Expected: 0,
                                                            Actual: {queriedMapElementList.Count}.");
                        }

                        taskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }
    }
}
