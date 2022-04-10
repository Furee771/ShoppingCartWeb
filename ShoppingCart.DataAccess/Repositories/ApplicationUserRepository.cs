using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUser
    {
        private ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ApplicationUser user)
        {
            var userDB = _context.ApplicationUsers.FirstOrDefault(x => x.Id == user.Id);
            if (userDB != null)
            {
                userDB.Name = user.Name;
                userDB.Address = user.Address;
                userDB.PhoneNumber = user.PhoneNumber;
                userDB.UserName = user.UserName;
                userDB.Email = user.Email;
                userDB.City = user.City;
            }
        }
    }
}
