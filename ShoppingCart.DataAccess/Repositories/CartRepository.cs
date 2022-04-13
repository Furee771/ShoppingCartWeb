using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Cart cart)
        {
            var cartDB = _context.Carts.FirstOrDefault(x => x.Id == cart.Id);
            if (cartDB != null)
            {
                cartDB.Product = cart.Product;
                cartDB.ApplicationUser = cart.ApplicationUser;
                cartDB.Count = cart.Count;
            }
        }

        public void IncrementCartItem(Cart cart, int count)
        {
            var cartDB = _context.Carts.FirstOrDefault(x => x.Id == cart.Id);
            if (cartDB != null)
            {
                cartDB.Count = cart.Count + count;
            }
        }

        public void DecrementCartItem(Cart cart, int count)
        {
            var cartDB = _context.Carts.FirstOrDefault(x => x.Id == cart.Id);
            if (cartDB != null)
            {
                cartDB.Count = cart.Count - count;
            }
        }
    }
}
