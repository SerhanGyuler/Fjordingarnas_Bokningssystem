using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Booking => Customer (Many-to-One)    
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId);

            // Booking => Employee (Many-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany(c => c.Bookings)
                .HasForeignKey(e => e.EmployeeId);

            // Booking => Service (Many-to-Many)
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Services)
                .WithMany(c => c.Bookings)
                .UsingEntity(s => s.ToTable("BookingServices"));

            // Employee => Service (Many-to-Many)
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Services)
                .WithMany(s => s.Employees)
                .UsingEntity(s => s.ToTable("EmployeeServices"));

        }
    }
}
