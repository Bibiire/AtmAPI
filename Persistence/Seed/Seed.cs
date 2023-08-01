using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Seed
{
    public class Seed
    {
        public static async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            var roles = Enum.GetValues(typeof(UserRole))
                            .Cast<UserRole>()
                            .ToList();

            foreach (var item in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(item.ToString());
                if (!roleExist)
                {
                    var role = new ApplicationRole
                    {
                        Name = item.ToString(),
                        NormalizedName = item.ToString().ToUpper()
                    };
                    await roleManager.CreateAsync(role);
                }
            }


           var email = $"admin@gmail.com.Admin";
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = "Admin",
                    LastName = "Admin",
                    OtherName = "Admin",
                    PhoneNumber = "09023778334*",
                    RoleCategory = UserRole.Director.ToString(),
                    BVN = "1234567",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin1$.");
                if (result.Succeeded)
                {
                    string role = UserRole.Director.ToString();
                    var roleExist = await roleManager.RoleExistsAsync(role);
                    if (!roleExist) await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                    await userManager.AddToRoleAsync(admin, UserRole.Director.ToString());
                    await userManager.UpdateAsync(admin);
                }
            }

        }
    }
}










