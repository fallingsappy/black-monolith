using CodePasswordDLL;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace CodePasswordDLL.Tests
{
    [TestClass]
    public class CodePasswordTests
    {
        string strIn, strExpected, strActual;

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test CodePassword Initialize");
            Console.WriteLine("Test CodePassword Initialize");
            strIn = null;
            strExpected = null;
            strActual = null;
        }

        [TestMethod]
        public void getPassword_bcd_abc()
        {
            strIn = "bcd";
            strExpected = "abc";

            strActual = CodePasswordDLL.PasswordClass.getPassword(strIn);

            Assert.AreEqual(strExpected, strActual, "Сравнение строк для метода getPassword_bcd_abc выполнено. Строки не равны.");
        }

        [TestMethod]
        public void getCodPassword_abc_bcd()
        {
            strIn = "abc";
            strExpected = "bcd";

            strActual = CodePasswordDLL.PasswordClass.getCodPassword(strIn);

            Assert.AreEqual(strExpected, strActual, "Сравнение строк для метода getCodPassword_abc_bcd выполнено. Строки не равны.");
        }

        [TestMethod()]
        public void getCodPassword_empty_empty()
        {
            strIn = "";
            strExpected = "";

            strActual = CodePasswordDLL.PasswordClass.getCodPassword(strIn);

            Assert.AreEqual(strExpected, strActual, "Сравнение строк для метода getCodPassword_empty_empty выполнено. Строки не равны.");
        }

        [TestMethod]
        public void getPassword_bcd_abc_SA()
        {
            strIn = "bcd";
            strExpected = "abc";

            strActual = CodePasswordDLL.PasswordClass.getPassword(strIn);

            StringAssert.Contains(strExpected, strActual, "Сравнение строк для метода getPassword_bcd_abc_SA выполнено. Строки не равны.");
        }

        [TestMethod]
        public void getCodPassword_abc_bcd_SA()
        {
            strIn = "abc";
            strExpected = "bcd";

            strActual = CodePasswordDLL.PasswordClass.getCodPassword(strIn);

            StringAssert.Contains(strExpected, strActual, "Сравнение строк для метода getCodPassword_abc_bcd_SA выполнено. Строки не равны.");
        }

        [TestMethod()]
        public void getCodPassword_empty_empty_SA()
        {
            strIn = "";
            strExpected = "";

            strActual = CodePasswordDLL.PasswordClass.getCodPassword(strIn);

            StringAssert.Contains(strExpected, strActual, "Сравнение строк для метода getCodPassword_empty_empty_SA выполнено. Строки не равны.");
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            Debug.WriteLine("Test CodePassword Finished");
            Console.WriteLine("Test CodePassword Finished");
            strIn = null;
            strExpected = null;
            strActual = null;
            Console.WriteLine("Test CodePassword CleanUp Completed");
        }
    }
}

