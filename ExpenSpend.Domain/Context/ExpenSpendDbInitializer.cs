using ExpenSpend.Domain.DTOs.Friends.Enums;
using ExpenSpend.Domain.Models.Expenses;
using ExpenSpend.Domain.Models.Friends;
using ExpenSpend.Domain.Models.GroupMembers;
using ExpenSpend.Domain.Models.Groups;
using ExpenSpend.Domain.Models.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenSpend.Data.Context
{
    public class ExpenSpendDbInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                context!.Database.EnsureCreated();


                // Seed Roles
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new List<IdentityRole<Guid>>()
                    {
                        new IdentityRole<Guid>() {Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin"},
                        new IdentityRole<Guid>() {Name = "User", ConcurrencyStamp = "2", NormalizedName = "User"}
                    });
                    await context.SaveChangesAsync();
                }

                // Seed Users

                var hasher = new PasswordHasher<ApplicationUser>();
                if (!context.Users.Any())
                {
                    
                    context.Users.AddRange(new List<ApplicationUser>()
                    {
                        new ApplicationUser() {
                            Email = "admin@gmail.com",
                            FirstName = "Admin",
                            LastName = "User",
                            UserName = "admin@gmail.com",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, password: "Admin@123")
                        },
                        new ApplicationUser() {
                            Email = "user@gmail.com",
                            FirstName = "User",
                            LastName = "User",
                            UserName = "user@gmail.com",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, "User@123")
                        },
                        new ApplicationUser()
                        {
                            Email = "rahul@gmail.com",
                            FirstName = "Rahul",
                            LastName = "Kumar",
                            UserName = "rahul@gmail.com",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, "Rahul@123")
                        },
                        new ApplicationUser()
                        {
                            Email = "aditya@gmail.com",
                            FirstName = "Aditya",
                            LastName = "Kumar",
                            UserName = "aditya@gmail.com",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, "Aditya@123")
                        }
                    });
                    await context.SaveChangesAsync();
                }

                // Get Users

                var adminUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == "admin@gmail.com");
                var userUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == "user@gmail.com");
                var rahulUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == "rahul@gmail.com");
                var adityaUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == "aditya@gmail.com");

                // Updating ConcurrencyStamp and SecurityStamp for Users

                if (!context.UserRoles.Any())
                {
                    // Admin User
                    await _userManager.UpdateSecurityStampAsync(adminUser);
                    await _userManager.UpdateNormalizedEmailAsync(adminUser);
                    await _userManager.UpdateNormalizedUserNameAsync(adminUser);
                    await _userManager.SetLockoutEnabledAsync(adminUser, true);

                    // User User
                    await _userManager.UpdateSecurityStampAsync(userUser);
                    await _userManager.UpdateNormalizedEmailAsync(userUser);
                    await _userManager.UpdateNormalizedUserNameAsync(userUser);
                    await _userManager.SetLockoutEnabledAsync(userUser, true);

                    // Rahul User
                    await _userManager.UpdateSecurityStampAsync(rahulUser);
                    await _userManager.UpdateNormalizedEmailAsync(rahulUser);
                    await _userManager.UpdateNormalizedUserNameAsync(rahulUser);
                    await _userManager.SetLockoutEnabledAsync(rahulUser, true);

                    // Aditya User
                    await _userManager.UpdateSecurityStampAsync(adityaUser);
                    await _userManager.UpdateNormalizedEmailAsync(adityaUser);
                    await _userManager.UpdateNormalizedUserNameAsync(adityaUser);
                    await _userManager.SetLockoutEnabledAsync(adityaUser, true);

                    await context.SaveChangesAsync();
                }

                // Seed UserRoles
                if (!context.UserRoles.Any())
                {
                    var adminRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "Admin");
                    var userRole = await context.Roles.FirstOrDefaultAsync(x => x.Name == "User");

                    context.UserRoles.AddRange(new List<IdentityUserRole<Guid>>{
                        new IdentityUserRole<Guid>{ RoleId = adminRole!.Id, UserId = adminUser!.Id},
                        new IdentityUserRole<Guid>{ RoleId = userRole!.Id, UserId = userUser!.Id},
                        new IdentityUserRole<Guid>{ RoleId = userRole.Id, UserId = rahulUser!.Id},
                        new IdentityUserRole<Guid>{ RoleId = userRole.Id, UserId = adityaUser!.Id}
                    });
                    await context.SaveChangesAsync();
                }

                // Seed Groups
                if (!context.Groups.Any())
                {
                    context.Groups.AddRange(new List<Group>()
                    {
                        new Group()
                        {
                            Name = "Group 1",
                            About = "Group 1 Description",
                            CreatedBy = adminUser!.Id,
                            CreatedAt = DateTime.Now
                        },
                        new Group()
                        {
                            Name = "Group 2",
                            About = "Group 2 Description",
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new Group()
                        {
                            Name = "Group 3",
                            About = "Group 3 Description",
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new Group()
                        {
                            Name = "Group 4",
                            About = "Group 4 Description",
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        }
                    });
                    await context.SaveChangesAsync();
                }

                // Get Groups
                var group1 = await context.Groups.FirstOrDefaultAsync(x => x.Name == "Group 1");
                var group2 = await context.Groups.FirstOrDefaultAsync(x => x.Name == "Group 2");
                var group3 = await context.Groups.FirstOrDefaultAsync(x => x.Name == "Group 3");
                var group4 = await context.Groups.FirstOrDefaultAsync(x => x.Name == "Group 4");

                // Seed GroupMebmers

                if (!context.GroupMembers.Any())
                {
                    context.GroupMembers.AddRange(new List<GroupMember>()
                    {
                        new GroupMember()
                        {
                            GroupId = group1!.Id,
                            UserId = adminUser!.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now,
                        },
                        new GroupMember()
                        {
                            GroupId = group1.Id,
                            UserId = userUser!.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new GroupMember()
                        {
                            GroupId = group1.Id,
                            UserId = rahulUser!.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new GroupMember()
                        {
                            GroupId = group1.Id,
                            UserId = adityaUser!.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new GroupMember()
                        {
                            GroupId = group2!.Id,
                            UserId = adminUser.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new GroupMember()
                        {
                            GroupId = group2.Id,
                            UserId = userUser.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new GroupMember()
                        {
                            GroupId = group2.Id,
                            UserId = rahulUser.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        },
                        new GroupMember()
                        {
                            GroupId = group2.Id,
                            UserId = adityaUser.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now
                        }
                    });
                    await context.SaveChangesAsync();
                }

                // Seed Expenses
                if (!context.Expenses.Any())
                {
                    context.Expenses.AddRange(new List<Expense>()
                    {
                        new Expense()
                        {
                            GroupId = group1!.Id,
                            CreatedBy = adminUser!.Id,
                            CreatedAt = DateTime.Now,
                            Amount = 100,
                            Title = "Expense 1",
                            Description = "Expense 1 Description",
                            PaidById = adminUser.Id,
                            SplitAs = SplitAs.Equally,
                            IsSettled = false
                        },
                        new Expense()
                        {
                            GroupId = group1.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now,
                            Amount = 200,
                            Title = "Expense 2",
                            Description = "Expense 2 Description",
                            PaidById = userUser!.Id,
                            SplitAs = SplitAs.Equally,
                            IsSettled = false
                        },
                        new Expense()
                        {
                            GroupId = group1.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now,
                            Amount = 300,
                            Title = "Expense 3",
                            Description = "Expense 3 Description",
                            PaidById = rahulUser!.Id,
                            SplitAs = SplitAs.Equally,
                            IsSettled = false
                        },
                        new Expense()
                        {
                            GroupId = group1.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now,
                            Amount = 400,
                            Title = "Expense 4",
                            Description = "Expense 4 Description",
                            PaidById = adityaUser!.Id,
                            SplitAs = SplitAs.Equally,
                            IsSettled = false
                        },
                        new Expense()
                        {
                            GroupId = group2!.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now,
                            Amount = 500,
                            Title = "Expense 5",
                            Description = "Expense 5 Description",
                            PaidById = adminUser.Id,
                            SplitAs = SplitAs.Equally,
                            IsSettled = false
                        },
                        new Expense()
                        {
                            GroupId = group2.Id,
                            CreatedBy = adminUser.Id,
                            CreatedAt = DateTime.Now,
                            Amount = 600,
                            Title = "Expense 6",
                            Description = "Expense 6 Description",
                            PaidById = userUser.Id,
                            SplitAs = SplitAs.Equally,
                            IsSettled = false
                        }
                    });
                    await context.SaveChangesAsync();
                }

                // Seed Friends
                if (!context.Friendships.Any())
                {
                    context.Friendships.AddRange(new List<Friendship>()
                    {
                        new Friendship(adminUser!.Id, userUser!.Id, FriendshipStatus.Accepted,DateTime.Now, adminUser.Id),
                        new Friendship(adminUser.Id,rahulUser!.Id,FriendshipStatus.Accepted,DateTime.Now,adminUser.Id),
                        new Friendship(adminUser.Id,adityaUser!.Id,FriendshipStatus.Accepted,DateTime.Now, adminUser.Id),
                        new Friendship(userUser!.Id,rahulUser.Id,FriendshipStatus.Accepted,DateTime.Now,userUser.Id),
                        new Friendship(userUser.Id,adityaUser.Id,FriendshipStatus.Accepted,DateTime.Now,userUser.Id),
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
