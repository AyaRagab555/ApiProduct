using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManger)
        {
            if(!userManger.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "John",
                    Email = "john@gmail.com",
                    UserName = "john@gmail.com",
                    Address = new Address()
                    {
                        FirstName = "john",
                        LastName = "Doe",
                        Street = "10 st",
                        City = "New York",
                        State = "NY",
                        ZipCode = "12345",
                    }
                };
                await userManger.CreateAsync(user , "Password123!"); 
            }

        }
    }
}
