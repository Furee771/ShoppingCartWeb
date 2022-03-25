using ShoppingCart.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        //public ICartPepository Cart { get; private set; }
        //public IApplicationUser User { get; private set; }
        //public IOrderHeader OrderHeader { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Product = new ProductRepository(context);
            //Cart = new CartRepository(context);
            //ApplicationUser = new ApplicationUserRepository(context);
            //OrderDetail = new OrderDetailRepository(context);
            //OrderHeader = new OrderHeaderRepossitory(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
