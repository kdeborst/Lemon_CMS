using Lemon.Models;
using Lemon.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Lemon.Data
{
    public class DbInitialiser : IDbInitialiser
    {
        //(LOCAL) PROPERTIES
        private readonly ApplicationDbContext _database;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        //CONSTRUCTOR
        public DbInitialiser(ApplicationDbContext database, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _database = database;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        //DEPLOYMENT INITIALISATION: SEEDING DB MIGRATIONS (CREATE DB & IDENTITY ROLES)
        public async void Initialise()
        {
            //CHECK FOR PENDING MIGRATIONS
            try
            {
                if (_database.Database.GetPendingMigrations().Count() > 0)
                {
                    _database.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            //IF ROLE EXISTS CONTINUE; ELSE CREATE IDENTITY ROLES
            if (_database.Roles.Any(r => r.Name == SD.ManagerUser)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.KitchenUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.FrontDeskUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerUser)).GetAwaiter().GetResult();


            //CREATE DEFAULT ADMIN ROLE UPON APP INITIALISATION
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@lemonrestaurant.com",
                Email = "admin@lemonrestaurant.com",
                Name = "Lemon Restaurant Manager",
                EmailConfirmed = true
            }, "Admin123!").GetAwaiter().GetResult();


            IdentityUser user = await _database.Users.FirstOrDefaultAsync(u => u.Email == "admin@lemonrestaurant.com");


            //ASSIGN MANAGER/ADMIN ROLE
            await _userManager.AddToRoleAsync(user, SD.ManagerUser);

        }
    }
}
