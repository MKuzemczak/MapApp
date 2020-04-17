using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapApp.Helpers;
using Windows.Devices.Geolocation;

namespace MapApp.Tests.MSTest.HelpersTests
{
    [TestClass]
    public class GeoMathTest
    {

        // Distance conifrmed by wolfram alpha
        [TestMethod]
        public void CoordinatesToDistanceInMeters_Input100DegreesEastDifference_Return11132000()
        {
            var result = GeoMath.CoordinatesToDistanceInMeters(0, 0, 0, 100);
            double expect = 11132000;

            // tollerance is 1000 meters because wolfram alpha returns distance in kilometers
            Assert.AreEqual(expect, result, 1000, $@"Returned value mismatch.\nExpected: {expect},\nActual: {result}");
        }

        // Distance conifrmed by wolfram alpha
        [TestMethod]
        public void CoordinatesToDistanceInMeters_Input10DegreesNorthDifference_Return1113000()
        {
            var result = GeoMath.CoordinatesToDistanceInMeters(0, 0, 10, 0);
            double expect = 1113000;

            // tollerance is 1000 meters because wolfram alpha returns distance in kilometers
            Assert.AreEqual(expect, result, 1000, $@"Returned value mismatch.\nExpected: {expect},\nActual: {result}");
        }

        [TestMethod]
        public void Sum_InputTwoVectorsOfOnes_ReturnVectorOfTwos()
        {
            var input = new BasicGeoposition() { Latitude = 1, Longitude = 1, Altitude = 1 };
            var result = GeoMath.Sum(input, input);
            var expect = new BasicGeoposition() { Latitude = 2, Longitude = 2, Altitude = 2 };

            Assert.AreEqual(expect.Latitude, result.Latitude, 0.000001, $@"Result latitude mismatch. 
                                                                Expected: {expect.Latitude},
                                                                Actual: {result.Latitude}");
            Assert.AreEqual(expect.Longitude, result.Longitude, 0.000001, $@"Result Longitude mismatch. 
                                                                Expected: {expect.Longitude},
                                                                Actual: {result.Longitude}");
            Assert.AreEqual(expect.Altitude, result.Altitude, 0.000001, $@"Result Altitude mismatch. 
                                                                Expected: {expect.Altitude},
                                                                Actual: {result.Altitude}");
        }

        [TestMethod]
        public void Difference_InputTwoVectorsOfOnes_ReturnZeroVector()
        {
            var input = new BasicGeoposition() { Latitude = 1, Longitude = 1, Altitude = 1 };
            var result = GeoMath.Difference(input, input);
            var expect = new BasicGeoposition() { Latitude = 0, Longitude = 0, Altitude = 0 };

            Assert.AreEqual(expect.Latitude, result.Latitude, 0.000001, $@"Result latitude mismatch. 
                                                                Expected: {expect.Latitude},
                                                                Actual: {result.Latitude}");
            Assert.AreEqual(expect.Longitude, result.Longitude, 0.000001, $@"Result Longitude mismatch. 
                                                                Expected: {expect.Longitude},
                                                                Actual: {result.Longitude}");
            Assert.AreEqual(expect.Altitude, result.Altitude, 0.000001, $@"Result Altitude mismatch. 
                                                                Expected: {expect.Altitude},
                                                                Actual: {result.Altitude}");
        }

        [TestMethod]
        public void Magnitude_InputOneVector_ReturnItsLength()
        {
            var input = new BasicGeoposition() { Latitude = 1, Longitude = 1, Altitude = 1 };
            var result = GeoMath.Magnitude(input);
            double expect = 1.73205;

            Assert.AreEqual(expect, result, 0.00001, $@"Wrong magnitude returned.
                                                        Expected: {expect},
                                                        Actual: {result}");
        }

        [TestMethod]
        public void Normalize_InputLatitude5_ReturnLatitude1()
        {
            var input = new BasicGeoposition() { Latitude = 1 };
            var result = GeoMath.Normalize(input);
            double expect = 1;

            Assert.AreEqual(expect, result.Latitude, 0.00001, $@"Wrong normalization in latitude.
                                                        Expected: {expect},
                                                        Actual: {result.Latitude}");
        }

        [TestMethod]
        public void TimesScalar_InputVectorOfOnesAnd2_ReturnVectorOfTwos()
        {
            var input = new BasicGeoposition() { Latitude = 1, Longitude = 1, Altitude = 1 };
            double inputScalar = 2;
            var result = GeoMath.TimesScalar(input, inputScalar);
            var expect = new BasicGeoposition() { Latitude = 2, Longitude = 2, Altitude = 2 };

            Assert.AreEqual(expect.Latitude, result.Latitude, 0.000001, $@"Result latitude mismatch. 
                                                                Expected: {expect.Latitude},
                                                                Actual: {result.Latitude}");
            Assert.AreEqual(expect.Longitude, result.Longitude, 0.000001, $@"Result Longitude mismatch. 
                                                                Expected: {expect.Longitude},
                                                                Actual: {result.Longitude}");
            Assert.AreEqual(expect.Altitude, result.Altitude, 0.000001, $@"Result Altitude mismatch. 
                                                                Expected: {expect.Altitude},
                                                                Actual: {result.Altitude}");
        }

        [TestMethod]
        public void CrossProduct_InputTwoVectorsOfOnes_ReturnZeroVector()
        {
            var input = new BasicGeoposition() { Latitude = 1, Longitude = 1, Altitude = 1 };
            var result = GeoMath.CrossProduct(input, input);
            var expect = new BasicGeoposition();

            Assert.AreEqual(expect.Latitude, result.Latitude, 0.000001, $@"Result latitude mismatch. 
                                                                Expected: {expect.Latitude},
                                                                Actual: {result.Latitude}");
            Assert.AreEqual(expect.Longitude, result.Longitude, 0.000001, $@"Result Longitude mismatch. 
                                                                Expected: {expect.Longitude},
                                                                Actual: {result.Longitude}");
            Assert.AreEqual(expect.Altitude, result.Altitude, 0.000001, $@"Result Altitude mismatch. 
                                                                Expected: {expect.Altitude},
                                                                Actual: {result.Altitude}");
        }

        [TestMethod]
        public void PolygonBorderLength_InputSquareWithSideLengthTenDegrees_Return4435000()
        {
            var path = new List<BasicGeoposition>()
            {
                new BasicGeoposition(),
                new BasicGeoposition() { Latitude = 10 },
                new BasicGeoposition() { Latitude = 10, Longitude = 10 },
                new BasicGeoposition() { Longitude = 10 }
            };
            var result = GeoMath.PolygonBorderLength(path);
            double expect = 4435000;

            Assert.AreEqual(expect, result, 4000, $@"Returned value mismatch.\nExpected: {expect},\nActual: {result}");
        }

        [TestMethod]
        public void PolylineLength_InputLineLikeOpenBoxWithSideLength10Degrees_Return3339000()
        {
            var path = new List<BasicGeoposition>()
            {
                new BasicGeoposition() { Latitude = 10 },
                new BasicGeoposition(),
                new BasicGeoposition() { Longitude = 10 },
                new BasicGeoposition() { Latitude = 10, Longitude = 10 }
            };
            var result = GeoMath.PolylineLength(path);
            double expect = 3339000;

            Assert.AreEqual(expect, result, 3000, $@"Returned value mismatch.\nExpected: {expect},\nActual: {result}");
        }
    }
}
