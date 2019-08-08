using Lemon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lemon.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        //PROPERTIES
        private readonly ApplicationDbContext _database;


        //CONSTRUCTOR
        public UserNameViewComponent(ApplicationDbContext database)
        {
            _database = database;
        }


        //ACTION: RETRIEVE LOGGED IN USERNAME
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _database.ApplicationUser.FirstOrDefaultAsync(u => u.Id == claims.Value);

            return View(userFromDb);
        }
    }
}
