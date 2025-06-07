using Fjordingarnas_Bokningssystem.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingSystem.API.Data;
using BookingSystem.API.Repositories;
using Microsoft.Data.Sqlite;


namespace BookingSystem.Tests
{
    [TestClass]
    public class DatabaseTestBase : IDisposable
    {
        protected AppDbContext Context { get; private set; }
        private SqliteConnection _connection;

        [TestInitialize]
        public void Setup()
        {
            // Dispose of any existing connection/context
            Context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            // Create and open NEW connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Configure DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void TearDown()
        {
            Context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        public void Dispose()
        {
            TearDown();
        }
    }
    [TestClass]
    public class BookingRepositoryMockTests 
    {
    }

    [TestClass]
    public class BookingRepositoryIntegrationTests : DatabaseTestBase
    {
        [TestMethod]
        public async Task GetBookingsForEmployeeAsync_ReturnsCorrectBookings()
        {
            // Clear seeded bookings
            Context.Bookings.RemoveRange(Context.Bookings);
            await Context.SaveChangesAsync();

            var testBookings = new List<Booking> {
                new Booking { EmployeeId = 1, CustomerId = 1, IsCancelled = false,
                    StartTime = new DateTime(2024, 1, 1, 10, 0, 0), EndTime = new DateTime(2024, 1, 1, 11, 0, 0) },
                new Booking { EmployeeId = 1, CustomerId = 2, IsCancelled = false,
                    StartTime = new DateTime(2024, 1, 1, 12, 0, 0), EndTime = new DateTime(2024, 1, 1, 13, 0, 0) },
                new Booking { EmployeeId = 2, CustomerId = 3, IsCancelled = false,
                    StartTime = new DateTime(2024, 1, 1, 14, 0, 0), EndTime = new DateTime(2024, 1, 1, 15, 0, 0) },
                new Booking { EmployeeId = 1, CustomerId = 4, IsCancelled = true,
                    StartTime = new DateTime(2024, 1, 1, 16, 0, 0), EndTime = new DateTime(2024, 1, 1, 17, 0, 0) }
            };

            Context.Bookings.AddRange(testBookings);
            await Context.SaveChangesAsync();

            var repository = new BookingRepository(Context);
            var result = await repository.GetBookingsForEmployeeAsync(1, null, null);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(b => b.EmployeeId == 1));
            Assert.IsTrue(result.All(b => !b.IsCancelled));
        }


    }
}
