using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
                                            RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync())
                return;
        
            var usersData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);

            if(users == null)
                return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
            
            foreach (var user in users)
            {               
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "pankaj");
                await userManager.AddToRoleAsync(user, "Member");                
            }

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "pankaj");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
        }

        // private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        // {
        //     using (var hmac = new System.Security.Cryptography.HMACSHA512())
        //     {
        //         passwordSalt = hmac.Key;
        //         passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //     }
        // }
    }
}