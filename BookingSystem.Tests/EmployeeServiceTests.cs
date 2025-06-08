using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Fjordingarnas_Bokningssystem.Models;
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

    // GetBookingsForEmpolyeesAsync tests ---------------
    [TestMethod]
    public async Task GetBookingsForEmployeeAsync_ReturnsEmptyList_WhenBookingsAreNull()
    {
        // Arrange
        int employeeId = 1;
        string period = "day";
        _bookingRepositoryMock
            .Setup(repo => repo.GetBookingsForEmployeeAsync(employeeId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _employeeService.GetBookingsForEmployeeAsync(employeeId, period);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        _bookingRepositoryMock.Verify(repo => repo.GetBookingsForEmployeeAsync(employeeId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
    }

    [TestMethod]
    public async Task GetBookingsForEmployeeAsync_ReturnsEmptyList_WhenNoBookingsExist()
    {
        // Arrange
        int employeeId = 1;
        string period = "day";
        _bookingRepositoryMock
            .Setup(repo => repo.GetBookingsForEmployeeAsync(employeeId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _employeeService.GetBookingsForEmployeeAsync(employeeId, period);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        _bookingRepositoryMock.Verify(repo => repo.GetBookingsForEmployeeAsync(employeeId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
    }

    [TestMethod]
    public async Task GetBookingsForEmployeeAsync_ReturnsBookingDtos_WhenBookingsExist()
    {
        // Arrange
        int employeeId = 1;
        string period = "day";
        var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    StartTime = new DateTime(2025, 6, 9, 9, 0, 0),
                    EndTime = new DateTime(2025, 6, 9, 10, 0, 0),
                    IsCancelled = false,
                    Customer = new Customer { FirstName = "Frank", LastName = "Ribbery" },
                    Employee = new Employee { FirstName = "Chris", LastName = "Tucker" },
                    Services = new List<Service> { new Service { ServiceName = "Fade-Haircut" } }
                }
            };

        _bookingRepositoryMock
            .Setup(repo => repo.GetBookingsForEmployeeAsync(employeeId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _employeeService.GetBookingsForEmployeeAsync(employeeId, period);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(bookings[0].Id, result[0].Id);
        Assert.AreEqual("Frank Ribbery", result[0].CustomerName);
        Assert.AreEqual("Chris Tucker", result[0].EmployeeName);
        CollectionAssert.Contains(result[0].Services, "Fade-Haircut");

        _bookingRepositoryMock.Verify(repo => repo.GetBookingsForEmployeeAsync(employeeId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
    }
}
