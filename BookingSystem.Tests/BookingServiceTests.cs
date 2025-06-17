using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                .ReturnsAsync(createdBooking);

            // Act
            var result = await _bookingService.CreateBookingAsync(inputDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(createdBooking.Id, result.Id);
            Assert.AreEqual("John", result.Customer.FirstName);
            Assert.AreEqual("Jane", result.Employee.FirstName);
            Assert.AreEqual(1, result.Services.Count);
            Assert.AreEqual("Haircut", result.Services[0].ServiceName);
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
    }
}
