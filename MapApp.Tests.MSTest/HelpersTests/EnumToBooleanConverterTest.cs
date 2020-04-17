using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapApp.Helpers;

namespace MapApp.Tests.MSTest.HelpersTests
{
    [TestClass]
    public class EnumToBooleanConverterTest
    {
        public static EnumToBooleanConverter Converter;

        public enum MyEnum
        {
            val1, val2, val3
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Converter = new EnumToBooleanConverter()
            { EnumType = typeof(MyEnum) };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Converter = null;
        }

        [TestMethod]
        public void Convert_InputTwoIdenticalValues_ReturnTrue()
        {
            MyEnum check = MyEnum.val2;

            bool result = (bool)Converter.Convert(check, typeof(MyEnum), "val2", "");

            Assert.IsTrue(result, "Converter returned false for identical values.");
        }

        [TestMethod]
        public void ConvertBack_InputStringRepresentation_ReturnCorrectEnumValue()
        {
            MyEnum result = (MyEnum)Converter.ConvertBack(MyEnum.val2, typeof(MyEnum), "val2", "");

            Assert.AreEqual(MyEnum.val2, result, $@"Returned enum value is diffrent from expected.
                                                    Expected: {MyEnum.val2},
                                                    Actual: {result}.");
        }
    }
}
