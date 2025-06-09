using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using BookingSystem.API.Controllers;
using Fjordingarnas_Bokningssystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using BookingSystem.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Tests
{
    [TestClass]
    public class BookingControllerTests
    {
        [TestMethod]
        public async Task GetBookings_ReturnsOk_WhenBookingsExist()
        {
            var mock = new Mock<IBookingService>();
            mock.Setup(s => s.GetAllBookingDtosAsync()).ReturnsAsync(new List<BookingDto>
                { 
                new BookingDto { Id = 1 },
                new BookingDto { Id = 2 }
                });

            var controller = new BookingController(
                Mock.Of<IBookingRepository>(),
                Mock.Of<ICustomerRepository>(),
                Mock.Of<IEmployeeRepository>(),
                Mock.Of<IServiceRepository>(),
                mock.Object);

            var result = await controller.GetBookings();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(result.Value as List<BookingDto>);
            if (result.Value is List<BookingDto> bookings)
            {
                Assert.AreEqual(2, bookings.Count);
            }
        }
    }

    // AI HAR SKRIVT ALLT HÄR 

}
