using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure;

namespace TravelBooking.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@travelbooking.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create customer user
            var customerEmail = "customer@travelbooking.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);
            if (customerUser == null)
            {
                customerUser = new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = customerEmail,
                    UserName = customerEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(customerUser, "Customer123!");
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }

            // Seed sample flights if they don't exist
            if (!context.Flights.Any())
            {
                context.Flights.AddRange(
                    new Flight
                    {
                        Airline = "Emirates",
                        FlightNumber = "EK001",
                        Origin = "Dubai",
                        Destination = "London",
                        DepartureTime = DateTime.UtcNow.AddDays(1),
                        ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(7),
                        Price = 499.99m,
                        AvailableSeats = 150
                    },
                    new Flight
                    {
                        Airline = "Qatar Airways",
                        FlightNumber = "QR123",
                        Origin = "Doha",
                        Destination = "Paris",
                        DepartureTime = DateTime.UtcNow.AddDays(2),
                        ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(6),
                        Price = 399.99m,
                        AvailableSeats = 120
                    },
                    new Flight
                    {
                        Airline = "British Airways",
                        FlightNumber = "BA456",
                        Origin = "London",
                        Destination = "New York",
                        DepartureTime = DateTime.UtcNow.AddDays(3),
                        ArrivalTime = DateTime.UtcNow.AddDays(3).AddHours(8),
                        Price = 599.99m,
                        AvailableSeats = 200
                    }
                );
            }

            // Seed sample hotels if they don't exist
            if (!context.Hotels.Any())
            {
                context.Hotels.AddRange(
                    new Hotel
                    {
                        Name = "Grand Hotel",
                        Location = "London",
                        Description = "Luxury hotel in central London",
                        PricePerNight = 199.99m,
                        RoomsAvailable = 50,
                        Rating = 4.5,
                        Amenities = "WiFi, Pool, Spa, Restaurant"
                    },
                    new Hotel
                    {
                        Name = "Paris Luxury Suites",
                        Location = "Paris",
                        Description = "Boutique hotel near Eiffel Tower",
                        PricePerNight = 299.99m,
                        RoomsAvailable = 30,
                        Rating = 4.8,
                        Amenities = "WiFi, Breakfast, Gym"
                    },
                    new Hotel
                    {
                        Name = "New York Plaza",
                        Location = "New York",
                        Description = "Modern hotel in Manhattan",
                        PricePerNight = 349.99m,
                        RoomsAvailable = 75,
                        Rating = 4.3,
                        Amenities = "WiFi, Restaurant, Business Center"
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
