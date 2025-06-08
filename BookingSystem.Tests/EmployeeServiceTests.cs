using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Moq;

namespace BookingSystem.Tests;

[TestClass]
public class EmployeeServiceTests
{
    private Mock<IDateTimeService> _dateTimeProviderMock = null!;
    private Mock<IBookingRepository> _bookingRepositoryMock = null!;
    private EmployeeService _employeeService = null!;

    [TestInitialize]
    public void Setup()
    {
        _dateTimeProviderMock = new Mock<IDateTimeService>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        
        // Setup today to a fixed date for easier testing
        _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateTime(2025, 6, 8));
        _employeeService = new EmployeeService(_dateTimeProviderMock.Object, _bookingRepositoryMock.Object);
    }

    // GetPeriodDates tests ----------------
    [TestMethod]
    public void GetPeriodDates_ShouldReturnNull_WhenPeriodIsNull()
    {
        // Act
        var result = _employeeService.GetPeriodDates(null);

        //Assert
        Assert.IsNull(result.StartDate);
        Assert.IsNull(result.EndDate);
    }

    [TestMethod]
    public void GetPeriodDates_ShouldReturnNull_WhenPeriodIsEmpty()
    {
        // Act
        var result = _employeeService.GetPeriodDates(string.Empty);

        // Assert
        Assert.IsNull(result.StartDate);
        Assert.IsNull(result.EndDate);
    }

    [TestMethod]
    public void GetPeriodDates_ShouldReturnNull_WhenPeriodIsInvalid()
    {
        // Act
        // only day and week schedule can be shown
        var result = _employeeService.GetPeriodDates("month");

        // Assert
        Assert.IsNull(result.StartDate);
        Assert.IsNull(result.EndDate);
    }

    [TestMethod]
    public void GetPeriodDates_ReturnsTodayAndTomorrow_WhenPeriodIsDay()
    {
        // Act
        var result = _employeeService.GetPeriodDates("day");

        // Assert
        Assert.AreEqual(new DateTime(2025, 6, 8), result.StartDate);
        Assert.AreEqual(new DateTime(2025, 6, 9), result.EndDate);
    }

    [TestMethod]
    public void GetPeriodDates_ReturnsTodayAndNextWeek_WhenPeriodIsWeek()
    {
        // Act
        var result = _employeeService.GetPeriodDates("week");

        // Assert
        Assert.AreEqual(new DateTime(2025, 6, 8), result.StartDate);
        Assert.AreEqual(new DateTime(2025, 6, 15), result.EndDate);
    }


}
