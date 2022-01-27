using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.ControllerLayer.JsonData;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer.JsonData;

[TestClass]
public class BookingDataTests
{
    [TestMethod]
    public void BookingData_ValidData()
    {
        BookingData bookingData = new() { FirstName = "Barry", LastName = "Wallis" };

        Assert.AreEqual(bookingData.FirstName, "Barry");
        Assert.AreEqual(bookingData.LastName, "Wallis");
    }

    [TestMethod]
    [DataRow(null, "Wallis")]
    [DataRow("Barry", null)]
    [DataRow("", "Wallis")]
    [DataRow("Barry", "")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BookingData_Failure_NullAndEmptyNames(string firstName, string lastName)
    {
        BookingData _ = new() { FirstName = firstName, LastName = lastName };
    }
}