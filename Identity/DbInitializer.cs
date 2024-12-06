using Identity.Constants.Enums;
using Identity.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Identity
{
    public static class DbInitializer
    {
        public static void SeedData(RoleManager<IdentityRole> roleManager,UserManager<User>userManager) 
        {
            AddRoles(roleManager);
            AddAdmin(userManager,roleManager);
        }
        private static void AddRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetValues<UserRoles>())
            {
                if (!roleManager.RoleExistsAsync(role.ToString()).Result)
                {
                    _=roleManager.CreateAsync(new IdentityRole
                    {
                        Name=role.ToString(),
                    }).Result;
                } 
            }
        } 
        private static void AddAdmin(UserManager<User>userManager,RoleManager<IdentityRole> roleManager)
        {
            if (userManager.FindByEmailAsync("Admin@test.com").Result is null)
            {

                var user = new User
                {
                    UserName = "Admin@test.com",
                    Email = "Admin@test.com",
                    City = "Sacramento",
                    Country = "Usa"
                };

                var result = userManager.CreateAsync(user, "AdminT123!").Result;
                if (!result.Succeeded) throw new Exception("cannot be created admin");

                var role = roleManager.FindByNameAsync("Admin").Result;
                if (role?.Name is null) throw new Exception("have not Admin role");


                var addToRoleResult = userManager.AddToRoleAsync(user, role.Name).Result;
                if (!addToRoleResult.Succeeded) throw new Exception("cannot be added Admin role to user");
            }
 

        }
    }
}
