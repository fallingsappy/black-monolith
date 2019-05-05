using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SchedulerClass.Tests
{
    [TestClass]
    public class SchedulerClassTests
    {
        MailSendWPFF.SchedulerClass sc;
        TimeSpan ts;
        // Запускается перед стартом каждого тестирующего метода.
        [TestInitialize]
        public void TestInitialize()
        {
            sc = new SchedulerClass();
            ts = new TimeSpan(); // Возвращаем в случае ошибочно введенного времени
        }
        [TestMethod()]
        public void GetSendTime_empty_ts()
        {
            string strTimeTest = "";
            TimeSpan tsTest = sc.GetSendTime(strTimeTest);
            Assert.AreEqual(ts, tsTest);
        }
        [TestMethod()]
        public void GetSendTime_sdf_ts()
        {
            string strTimeTest = "sdf";
            TimeSpan tsTest = sc.GetSendTime(strTimeTest);
            Assert.AreEqual(ts, tsTest);
        }
        [TestMethod()]
        © geekbrains.ru 16
        public void GetSendTime_correctTime_Equal()
        {
            string strTimeTest = "12:12";
            TimeSpan tsCorrect = new TimeSpan(12, 12, 0);
            TimeSpan tsTest = sc.GetSendTime(strTimeTest);
            Assert.AreEqual(tsCorrect, tsTest);
        }
        [TestMethod()]
        public void GetSendTime_inCorrectHour_ts()
        {
            string strTimeTest = "25:12";
            TimeSpan tsTest = sc.GetSendTime(strTimeTest);
            Assert.AreEqual(ts, tsTest);
        }
        [TestMethod()]
        public void GetSendTime_inCorrectMin_ts()
        {
            string strTimeTest = "12:65";
            TimeSpan tsTest = sc.GetSendTime(strTimeTest);
            Assert.AreEqual(ts, tsTest);
        }
    }
}
