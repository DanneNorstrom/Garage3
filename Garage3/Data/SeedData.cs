using Garage3.Models;
using Microsoft.AspNetCore.Identity;

namespace Garage3.Data
{
    public class SeedData
    {
        private static Garage3Context context = default!;
        private static RoleManager<IdentityRole> roleManager = default!;
        private static UserManager<ApplicationUser> userManager = default!;

        public static async Task Init(Garage3Context _context, IServiceProvider services)
        {
            context = _context;
            if (context.Roles.Any()) return;

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var roleNames = new[] { "Member", "Admin" };
            var adminEmail = "admin@admin.com";
            var userEmail = "user@user.com";


            await AddRolesAsync(roleNames);


            var admin = await AddAccountAsync(adminEmail, "Admin", "Adminsson", "19501224-1234", "8Ulven(");
            var user = await AddAccountAsync(userEmail, "User", "Usersson", "19701224-4321", "9Ulven)");

            //var user2 = await AddAccountAsync("daniel@hotmail.com", "Daniel", "Danielsson", "19701224-4321", "1Ulven!");
            //var user3 = await AddAccountAsync("steven@hotmail.com", "Steven", "Stevensson", "19801224-4729", "3Ulven#");



            await AddUserToRoleAsync(admin, "Admin");
            await AddUserToRoleAsync(user, "Member");
            //await AddUserToRoleAsync(user2, "Member");
            //await AddUserToRoleAsync(user3, "Member");

        }

        private static async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var result = await userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

            }
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task<ApplicationUser> AddAccountAsync(string accountEmail, string fName, string lName, string pNumber, string pw)  
        {
            var found = await userManager.FindByEmailAsync(accountEmail);

            if (found != null) return null!;

            var user = new ApplicationUser
            {
                UserName = accountEmail,
                Email = accountEmail,
                FirstName = fName,
                LastName = lName,
                PersonNumber = pNumber,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, pw);

            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

            return user;
        }
    }
}
