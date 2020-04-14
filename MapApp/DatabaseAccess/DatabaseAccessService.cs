using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using MapApp.Models;
using MapApp.Helpers;
using MapApp.Services;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI;

namespace MapApp.DatabaseAccess
{
    public static class DatabaseAccessService
    {
        public static bool Initialized = false;

        /// <summary>
        /// 
        /// </summary>
        public static readonly string DBFileName = "sqlite.db";

        /// <summary>
        /// 
        /// </summary>
        public static string DbFile
        {
            get { return Path.Combine(ApplicationData.Current.LocalFolder.Path, DBFileName); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task InitializeAsync()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(DBFileName, CreationCollisionOption.OpenIfExists);
            Initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<SQLiteConnection> GetDbConnectionAsync()
        {
            if (!Initialized)
                await InitializeAsync();

            return new SQLiteConnection("Data Source=" + DbFile);
        }

        /// <summary>
        /// 
        /// </summary>
        public static async Task CreateDatabaseAsync()
        {
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();
                cnn.Execute(
                    @"CREATE TABLE IF NOT EXISTS Layer
                      (
                         Id     INTEGER PRIMARY KEY,
                         Name   VARCHAR(100) NOT NULL
                      )");
                cnn.Execute(
                    @"CREATE TABLE IF NOT EXISTS MapElement
                        (
                            Id              INTEGER PRIMARY KEY,
                            Name            VARCHAR(100) NOT NULL,
                            Type            VARCHAR(100) NOT NULL,
                            StrokeColor     VARCHAR(8),
                            FillColor       VARCHAR(8),
                            Width           REAL,
                            Layer_Id        REFERENCES Layer(Id) NOT NULL
                        )");
                cnn.Execute(
                    @"CREATE TABLE IF NOT EXISTS Geoposition
                        (
                            Id                  INTEGER PRIMARY KEY,
                            Altitude            REAL NOT NULL,
                            Latitude            REAL NOT NULL,
                            Longitude           REAL NOT NULL,
                            MapElement_Id       REFERENCES MapElement(Id) NOT NULL
                        )");
                cnn.Execute("PRAGMA recursive_triggers = ON");
                cnn.Execute(
                    @"CREATE TRIGGER IF NOT EXISTS after_mapelement_delete
                    AFTER DELETE ON MapElement
                    BEGIN
                        DELETE FROM Geoposition WHERE MapElement_Id = OLD.Id;
                    END");
                cnn.Execute(
                    @"CREATE TRIGGER IF NOT EXISTS after_layer_delete
                    AFTER DELETE ON Layer
                    BEGIN
                        DELETE FROM MapElement WHERE Layer_Id = OLD.Id;
                    END");
            }
        }

        public static async Task InsertLayer(MapLayerItem layer)
        {
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                await cnn.ExecuteScalarAsync<int>(
                    @"INSERT INTO Layer (Id, Name) VALUES (@Id, @Name)",
                    layer);
            }
        }

        public static async Task<List<MapLayerItem>> GetLayers()
        {
            var result = new List<MapLayerItem>();

            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                result = (await cnn.QueryAsync<MapLayerItem>("SELECT * FROM Layer")).ToList();
            }

            return result;
        }

        public static async Task InsertMapIconItem(MapIconItem item)
        {
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                item.Id = (int)await cnn.ExecuteScalarAsync<long>(
                    @"INSERT INTO MapElement (Name, Type, Layer_Id) VALUES (@Name, @Type, @Layer_Id);
                    SELECT last_insert_rowid();",
                    new { item.Name, Type = "MapIconItem", Layer_Id = item.ParentLayer.Id });

                await cnn.ExecuteAsync(
                    @"INSERT INTO Geoposition (Altitude, Latitude, Longitude, MapElement_Id)
                    VALUES (@Altitude, @Latitude, @Longitude, @MapElement_id)",
                    new
                    {
                        (item.Element as MapIcon).Location.Position.Altitude,
                        (item.Element as MapIcon).Location.Position.Latitude,
                        (item.Element as MapIcon).Location.Position.Longitude,
                        MapElement_Id = item.Id
                    });
            }
        }

        public static async Task<List<MapIconItem>> GetMapIconItems()
        {
            var result = new List<MapIconItem>();
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                var sqlcommand =
                    @"SELECT m.Id, m.Name, l.Id, l.Name, g.Altitude, g.Latitude, g.Longitude 
                    FROM ((MapElement m  
                    INNER JOIN Layer l ON m.Layer_Id = l.Id) 
                    INNER JOIN Geoposition g ON m.Id = g.MapElement_Id)
                    WHERE m.Type='MapIconItem'";

                result = cnn.
                    Query<MapIconItem, MapLayerItem, BasicGeoposition, MapIconItem>(sqlcommand,
                    (icon, layer, pos) =>
                    {
                        var ret = MapElementItemFactoryService.GetMapIconItem(icon.Name, pos, layer, icon.Id);
                        return ret;
                    },
                    splitOn: "Id,Altitude").ToList();

            }

            return result;
        }

        public static async Task InsertMapPolylineItem(MapPolylineItem item)
        {
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                item.Id = (int)await cnn.ExecuteScalarAsync<long>(
                    @"INSERT INTO MapElement (Name, Type, StrokeColor, Width, Layer_Id)
                    VALUES (@Name, @Type, @StrokeColor, @Width, @Layer_Id);
                    SELECT last_insert_rowid();",
                    new
                    {
                        item.Name,
                        Type = "MapPolylineItem",
                        StrokeColor = StringColorConverter.ArgbColorToString(item.StrokeColor),
                        item.Width,
                        Layer_Id = item.ParentLayer.Id
                    });

                var list = from g in item.Path
                           select new
                           {
                               g.Altitude,
                               g.Latitude,
                               g.Longitude,
                               MapElement_Id = item.Id
                           };

                await cnn.ExecuteAsync(
                    @"INSERT INTO Geoposition (Altitude, Latitude, Longitude, MapElement_Id)
                    VALUES (@Altitude, @Latitude, @Longitude, @MapElement_Id)",
                    list);
            }
        }

        public static async Task<List<MapPolylineItem>> GetMapPolylineItems()
        {
            var result = new List<MapPolylineItem>();
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                var sqlcommand =
                    @"SELECT m.Id, m.Name, m.StrokeColor, m.Width, l.Id, l.Name, g.Altitude, g.Latitude, g.Longitude 
                    FROM ((MapElement m  
                    INNER JOIN Layer l ON m.Layer_Id = l.Id) 
                    INNER JOIN Geoposition g ON m.Id = g.MapElement_Id)
                    WHERE m.Type='MapPolylineItem'";

                var anons = new List<TmpMapElementContainer>();

                cnn.Query<dynamic, MapLayerItem, BasicGeoposition, int>(sqlcommand,
                    (polyline, layer, pos) =>
                    {
                        if (anons.Count == 0 || anons.Last().Id != polyline.Id)
                        {
                            anons.Add(new TmpMapElementContainer()
                            {
                                Id = (int)polyline.Id,
                                Name = polyline.Name,
                                StrokeColor = StringColorConverter.ArgbStringToColor((string)polyline.StrokeColor),
                                Width = (double)polyline.Width,
                                Layer = layer
                            });
                        }

                        anons.Last().Path.Add(pos);

                        return 0;
                    },
                    splitOn: "Id,Altitude").ToList();

                for (int i = 0; i < anons.Count; i++)
                {
                    result.Add(MapElementItemFactoryService.GetMapPolylineItem(anons[i].Name, anons[i].Path, anons[i].Layer,
                                                                               anons[i].StrokeColor, anons[i].Width, anons[i].Id));
                }

            }

            return result;
        }

        public static async Task InsertMapPolygonItem(MapPolygonItem item)
        {
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                item.Id = (int)await cnn.ExecuteScalarAsync<long>(
                    @"INSERT INTO MapElement (Name, Type, StrokeColor, FillColor, Layer_Id)
                    VALUES (@Name, @Type, @StrokeColor, @FillColor, @Layer_Id);
                    SELECT last_insert_rowid();",
                    new
                    {
                        item.Name,
                        Type = "MapPolygonItem",
                        StrokeColor = StringColorConverter.ArgbColorToString(item.StrokeColor),
                        FillColor = StringColorConverter.ArgbColorToString(item.FillColor),
                        Layer_Id = item.ParentLayer.Id
                    });

                var list = from g in item.Path
                           select new
                           {
                               g.Altitude,
                               g.Latitude,
                               g.Longitude,
                               MapElement_Id = item.Id
                           };

                await cnn.ExecuteAsync(
                    @"INSERT INTO Geoposition (Altitude, Latitude, Longitude, MapElement_Id)
                    VALUES (@Altitude, @Latitude, @Longitude, @MapElement_Id)",
                    list);
            }
        }

        public static async Task<List<MapPolygonItem>> GetMapPolygonItems()
        {
            var result = new List<MapPolygonItem>();
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                var sqlcommand =
                    @"SELECT m.Id, m.Name, m.StrokeColor, m.FillColor, l.Id, l.Name, g.Altitude, g.Latitude, g.Longitude 
                    FROM ((MapElement m  
                    INNER JOIN Layer l ON m.Layer_Id = l.Id) 
                    INNER JOIN Geoposition g ON m.Id = g.MapElement_Id)
                    WHERE m.Type='MapPolygonItem'";

                var anons = new List<TmpMapElementContainer>();

                cnn.Query<dynamic, MapLayerItem, BasicGeoposition, int>(sqlcommand,
                    (polyline, layer, pos) =>
                    {
                        if (anons.Count == 0 || anons.Last().Id != polyline.Id)
                        {
                            anons.Add(new TmpMapElementContainer()
                            {
                                Id = (int)polyline.Id,
                                Name = polyline.Name,
                                StrokeColor = StringColorConverter.ArgbStringToColor((string)polyline.StrokeColor),
                                FillColor = StringColorConverter.ArgbStringToColor((string)polyline.FillColor),
                                Layer = layer
                            });
                        }

                        anons.Last().Path.Add(pos);

                        return 0;
                    },
                    splitOn: "Id,Altitude").ToList();

                for (int i = 0; i < anons.Count; i++)
                {
                    result.Add(MapElementItemFactoryService.GetMapPolygonItem(anons[i].Name, anons[i].Path,
                                                                              anons[i].Layer, anons[i].StrokeColor,
                                                                              anons[i].FillColor, anons[i].Id));
                }

            }

            return result;
        }

        public static async Task<List<MapElementItem>> GetMapElementItemsAsync()
        {
            var result = new List<MapElementItem>();

            var icons = await GetMapIconItems();
            var polylines = await GetMapPolylineItems();
            var polygons = await GetMapPolygonItems();

            result.AddRange(icons);
            result.AddRange(polylines);
            result.AddRange(polygons);

            return result;
        }

        public static async Task DeleteMapElement(int id)
        {
            using (var cnn = await GetDbConnectionAsync())
            {
                cnn.Open();

                await cnn.ExecuteAsync(@"DELETE FROM MapElement WHERE Id=@Id", new { Id = id });
            }
        }

        public class TmpMapElementContainer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Color StrokeColor { get; set; }
            public Color FillColor { get; set; }
            public double Width { get; set; }
            public List<BasicGeoposition> Path = new List<BasicGeoposition>();
            public MapLayerItem Layer;
        }
    }
}
