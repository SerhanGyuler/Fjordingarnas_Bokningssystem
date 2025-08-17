using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSystem.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        private Mock<IBookingRepository> _bookingRepoMock;
        private Mock<ICustomerRepository> _customerRepoMock;
        private Mock<IServiceRepository> _serviceRepoMock;
        private Mock<IEmployeeRepository> _employeeRepoMock;

        private BookingService _bookingService;

        [TestInitialize]
        public void Setup()
        {
            _bookingRepoMock = new Mock<IBookingRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _serviceRepoMock = new Mock<IServiceRepository>();
            _employeeRepoMock = new Mock<IEmployeeRepository>();

            _bookingService = new BookingService(
                _bookingRepoMock.Object,
                _customerRepoMock.Object,
                _employeeRepoMock.Object,
                _serviceRepoMock.Object);
        }

        [TestMethod]
        public async Task GetBookingByIdAsync_ReturnsBookingDto_WhenBookingExists()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking
            {
                Id = bookingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                IsCancelled = false,
                Customer = new Customer { FirstName = "John", LastName = "Doe" },
                Employee = new Employee { FirstName = "Jane", LastName = "Smith" },
                Services = new List<Service>
                {
                    new Service { ServiceName = "Haircut" },
                    new Service { ServiceName = "Massage" }
                }
            };

            _bookingRepoMock.Setup(repo => repo.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _bookingService.GetBookingByIdAsync(bookingId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(bookingId, result.Id);
            Assert.AreEqual("John Doe", result.CustomerName);
            Assert.AreEqual("Jane Smith", result.EmployeeName);
            CollectionAssert.AreEqual(new List<string> { "Haircut", "Massage" }, result.Services);
        }

        [TestMethod]
        public async Task GetBookingByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
        {
            // Arrange
            var bookingId = 99;
            _bookingRepoMock.Setup(repo => repo.GetByIdAsync(bookingId))
                .ReturnsAsync((Booking)null);

            // Act
            var result = await _bookingService.GetBookingByIdAsync(bookingId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateBookingAsync_ReturnsNewBookingDto_WhenBookingIsCreated()
        {
            // Arrange
            var inputDto = new BookingInputDto
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                CustomerId = 1,
                EmployeeId = 2,
                ServiceIds = new List<int> { 3 }
            };

            var createdBooking = new Booking
            {
                Id = 10,
                StartTime = inputDto.StartTime,
                EndTime = inputDto.EndTime,
                IsCancelled = false,
                Customer = new Customer { FirstName = "John", LastName = "Doe", PhoneNumber = "12345" },
                Employee = new Employee { FirstName = "Jane", LastName = "Smith", PhoneNumber = "67890" },
                Services = new List<Service>
                {
                    new Service { ServiceName = "Haircut", Duration = TimeSpan.FromMinutes(30), Price = 100 }
                }
            };

            _bookingRepoMock.Setup(repo => repo.CreateBookingAsync(It.IsAny<BookingInputDto>()))
                .ReturnsAsync((createdBooking, null));

            // Act
            var (bookingResult, error) = await _bookingService.CreateBookingAsync(inputDto);

            // Assert
            Assert.IsNotNull(bookingResult);
            Assert.IsNull(error);
            Assert.AreEqual(createdBooking.Id, bookingResult.Id);
            Assert.AreEqual("John", bookingResult.Customer.FirstName);
            Assert.AreEqual("Jane", bookingResult.Employee.FirstName);
            Assert.AreEqual(1, bookingResult.Services.Count);
            Assert.AreEqual("Haircut", bookingResult.Services[0].ServiceName);
        }

        [TestMethod]
        public async Task DeleteAsync_DeletesBooking_WhenBookingExists()
        {
            // Arrange
            var bookingId = 5;
            var booking = new Booking { Id = bookingId };

            _bookingRepoMock.Setup(repo => repo.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            _bookingRepoMock.Setup(repo => repo.DeleteAsync(booking))
                .Returns(Task.CompletedTask);

            _bookingRepoMock.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act
            await _bookingService.DeleteAsync(bookingId);

            // Assert
            _bookingRepoMock.Verify(repo => repo.DeleteAsync(booking), Times.Once);
            _bookingRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteAsync_ThrowsKeyNotFoundException_WhenBookingDoesNotExist()
        {
            // Arrange
            var bookingId = 99;

            _bookingRepoMock.Setup(repo => repo.GetByIdAsync(bookingId))
                .ReturnsAsync((Booking)null);

            // Act
            await _bookingService.DeleteAsync(bookingId);

            // Assert handled by ExpectedException
        }

        [TestMethod]
        public async Task CreateBookingAsync_ShouldFail_WhenEmployeeIsUnavailable()
        {
            // Arrange
            var inputDto = new BookingInputDto
            {
                StartTime = DateTime.UtcNow.AddMinutes(30),
                EndTime = DateTime.UtcNow.AddHours(2),
                CustomerId = 1,
                EmployeeId = 1,
                ServiceIds = new List<int> { 1 }
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                UnavailablePeriods = new List<UnavailablePeriod>
        {
            new UnavailablePeriod
            {
                Id = 1,
                EmployeeId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3),
                Reason = "Semester"
            }
        }
            };

            _employeeRepoMock.Setup(r => r.GetByIdWithBookingsServicesAndUnavailabilityAsync(1))
                .ReturnsAsync(employee);

            // Act
            var (bookingResult, error) = await _bookingService.CreateBookingAsync(inputDto);

            // Assert
            Assert.IsNull(bookingResult);
            Assert.AreEqual("Employee is unavailable during this period", error);
        }

        [TestMethod]
        public async Task CreateBookingAsync_ShouldSucceed_WhenEmployeeIsAvailable()
        {
            // Arrange
            var inputDto = new BookingInputDto
            {
                StartTime = DateTime.UtcNow.AddMinutes(30),
                EndTime = DateTime.UtcNow.AddHours(2),
                CustomerId = 1,
                EmployeeId = 1,
                ServiceIds = new List<int> { 1 }
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                UnavailablePeriods = new List<UnavailablePeriod>() // tom lista = tillgänglig
            };

            _employeeRepoMock.Setup(r => r.GetByIdWithBookingsServicesAndUnavailabilityAsync(1))
                .ReturnsAsync(employee);

            var createdBooking = new Booking
            {
                Id = 10,
                StartTime = inputDto.StartTime,
                EndTime = inputDto.EndTime,
                Customer = new Customer { FirstName = "John", LastName = "Doe" },
                Employee = employee,
                Services = new List<Service> { new Service { ServiceName = "Haircut" } }
            };

            _bookingRepoMock.Setup(r => r.CreateBookingAsync(inputDto))
                .ReturnsAsync((createdBooking, null));

            // Act
            var (bookingResult, error) = await _bookingService.CreateBookingAsync(inputDto);

            // Assert
            Assert.IsNotNull(bookingResult);
            Assert.IsNull(error);
            Assert.AreEqual(createdBooking.Id, bookingResult.Id);
        }

        [TestMethod]
        public async Task RescheduleBookingAsync_ShouldFail_WhenEmployeeIsUnavailable()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking
            {
                Id = bookingId,
                EmployeeId = 1,
                IsCancelled = false,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "123456",
                UnavailablePeriods = new List<UnavailablePeriod>
        {
            new UnavailablePeriod
            {
                Id = 1,
                EmployeeId = 1,
                StartDate = DateTime.UtcNow.AddMinutes(-10),
                EndDate = DateTime.UtcNow.AddHours(2),
                Reason = "Semester"
            }
        }
            };

            _bookingRepoMock.Setup(r => r.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);
            _employeeRepoMock.Setup(r => r.GetByIdWithBookingsServicesAndUnavailabilityAsync(1))
                .ReturnsAsync(employee);

            var dto = new RescheduleBookingDto
            {
                NewStartTime = DateTime.UtcNow.AddMinutes(30),
                NewEndTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var (success, error) = await _bookingService.RescheduleBookingAsync(bookingId, dto);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual("Employee is unavailable during this period", error);
        }

        [TestMethod]
        public async Task RescheduleBookingAsync_ShouldSucceed_WhenEmployeeIsAvailable()
        {
            // Arrange
            var bookingId = 2;
            var booking = new Booking
            {
                Id = bookingId,
                EmployeeId = 1,
                IsCancelled = false,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "123456",
                UnavailablePeriods = new List<UnavailablePeriod>() // tom lista = tillgänglig
            };

            _bookingRepoMock.Setup(r => r.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);
            _employeeRepoMock.Setup(r => r.GetByIdWithBookingsServicesAndUnavailabilityAsync(1))
                .ReturnsAsync(employee);

            var dto = new RescheduleBookingDto
            {
                NewStartTime = DateTime.UtcNow.AddMinutes(30),
                NewEndTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var (success, error) = await _bookingService.RescheduleBookingAsync(bookingId, dto);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNull(error);
        }

        [TestMethod]
        public async Task GetBookingsByCustomer_ReturnsBookingDtos_WhenCustomerHasBookings()
        {
            // Arrange
            int customerId = 1;
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 101,
                    CustomerId = customerId,
                    Customer = new Customer { FirstName = "Anna", LastName = "Karlsson" },
                    Employee = new Employee { FirstName = "John", LastName = "Doe" },
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    EndTime = DateTime.UtcNow.AddDays(-1).AddHours(1),
                    Services = new List<Service>
                    {
                        new Service { ServiceName = "Klippning" }
                    }
                }
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingsByCustomerAsync(customerId))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetBookingsByCustomerAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            var bookingDto = result.First();
            Assert.AreEqual("Anna Karlsson", bookingDto.CustomerName);
            Assert.AreEqual("John Doe", bookingDto.EmployeeName);
            Assert.AreEqual(1, bookingDto.Services.Count);
            Assert.AreEqual("Klippning", bookingDto.Services.First());
        }

        [TestMethod]
        public async Task GetBookingsByCustomer_ReturnsEmpty_WhenCustomerHasNoBookings()
        {
            // Arrange
            int customerId = 99;
            _bookingRepoMock
                .Setup(r => r.GetBookingsByCustomerAsync(customerId))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _bookingService.GetBookingsByCustomerAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetBookingsByCustomer_ReturnsEmpty_WhenRepositoryReturnsNull()
        {
            // Arrange
            int customerId = 5;
            _bookingRepoMock
                .Setup(r => r.GetBookingsByCustomerAsync(customerId))
                .ReturnsAsync((IEnumerable<Booking>)null);

            // Act
            var result = await _bookingService.GetBookingsByCustomerAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

    }
}
