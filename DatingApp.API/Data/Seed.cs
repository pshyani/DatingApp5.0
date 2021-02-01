using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using DatingApp.API.Entities;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync())
                return;
        
            var usersData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);
            foreach (var user in users)
            {
                byte[] passwordhash, passwordSalt;
                CreatePasswordHash("pankaj", out passwordhash, out passwordSalt);

                user.PasswordHash = passwordhash;
                user.PasswordSalt = passwordSalt;
                user.UserName = user.UserName.ToLower();
                context.Users.Add(user);
            }

            context.SaveChanges();
            
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}