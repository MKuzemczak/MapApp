using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapApp.Helpers;
using Windows.UI;

namespace MapApp.Tests.MSTest.HelpersTests
{
    [TestClass]
    public class StringColorConverterTest
    {
        [TestMethod]
        public void ArgbStringToColor_InputFFFFFFFF_ReturnWhite()
        {
            var input = "#FFFFFFFF";
            var result = StringColorConverter.ArgbStringToColor(input);
            var expect = Color.FromArgb(255, 255, 255, 255);

            Assert.AreEqual(expect, result, $@"Returned color not as expected.
                                            Expected: {expect},
                                            Actual: {result}");
        }

        [TestMethod]
        public void ArgbColorToString_InputWhite_ReturnFFFFFFFF()
        {
            var input = Color.FromArgb(255, 255, 255, 255);
            var result = StringColorConverter.ArgbColorToString(input);
            var expect = "FFFFFFFF";

            Assert.AreEqual(expect, result, $@"Returned color not as expected.
                                            Expected: {expect},
                                            Actual: {result}");
        }
    }
}
