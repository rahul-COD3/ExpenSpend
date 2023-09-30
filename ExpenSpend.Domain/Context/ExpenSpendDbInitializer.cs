﻿using ExpenSpend.Domain.Models.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenSpend.Domain.Context
{
    public class ExpenSpendDbInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ExpenSpendDbContext>();

                context.Database.EnsureCreated();


                // Seed Roles
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new List<IdentityRole<Guid>>()
                    {
                        new IdentityRole<Guid>() {Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin"},
                        new IdentityRole<Guid>() {Name = "ESUser", ConcurrencyStamp = "2", NormalizedName = "ESUser"}
                    });
                    await context.SaveChangesAsync();
                }

                // Seed Users

                var hasher = new PasswordHasher<ESUser>();
                if (!context.Users.Any())
                {
                    context.Users.AddRange(new List<ESUser>()
                    {
                        new ESUser() {
                            Email = "admin@asp.net",
                            FirstName = "Admin",
                            LastName = "ESUser",
                            UserName = "admin",
                            NormalizedUserName = "ADMIN",
                            ConcurrencyStamp ="1",
                            EmailConfirmed = true,
                            LockoutEnabled = true,
                            PasswordHash = hasher.HashPassword(null,"Admin@123")
                        },
                        new ESUser() {
                            Email = "user@asp.net",
                            FirstName = "ESUser",
                            LastName = "ESUser",
                            UserName = "user",
                            NormalizedUserName = "USER",
                            ConcurrencyStamp ="2",
                            EmailConfirmed = true,
                            LockoutEnabled = true,
                            PasswordHash = hasher.HashPassword(null, "ESUser@123")
                        } 
                    });
                    await context.SaveChangesAsync();
                }

                // Seed UserRoles
                if (!context.UserRoles.Any())
                {
                    var adminRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "Admin");
                    var adminUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == "admin");
                    var userRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "ESUser");
                    var normalUserRole = await context.Users.FirstOrDefaultAsync(x => x.UserName == "user");

                    context.UserRoles.AddRange(new List<IdentityUserRole<Guid>>{
                        new IdentityUserRole<Guid>{ RoleId = adminRole.Id, UserId = adminUser.Id},
                        new IdentityUserRole<Guid>{ RoleId = userRole.Id, UserId = normalUserRole.Id}
                    });

                    await context.SaveChangesAsync();
                }

            }
        }
    }
}
