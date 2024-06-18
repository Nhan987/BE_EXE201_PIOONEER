
using PIOONEER_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyDbContext _context;

        public UnitOfWork(MyDbContext context)
        {
            _context = context;
        }

        private IGenericRepository<User> _users;
        private IGenericRepository<Role> _roles;
        private IGenericRepository<Product> _products;
        private IGenericRepository<Order> _orders;
        private IGenericRepository<OrderDetails> _orderDetails;
        private IGenericRepository<Category> _categories;
        private IGenericRepository<Discount> _discounts;
        private IGenericRepository<Contact> _contacts;
        private IGenericRepository<ProductByUser> _productByUsers;
        private IGenericRepository<OtpEntity> _OtpEntitys;

        public IGenericRepository<User> UserRepository => _users ??= new GenericRepository<User>(_context);
        public IGenericRepository<Role> RoleRepository => _roles ??= new GenericRepository<Role>(_context);
        public IGenericRepository<Product> Products => _products ??= new GenericRepository<Product>(_context);
        public IGenericRepository<Order> Orders => _orders ??= new GenericRepository<Order>(_context);
        public IGenericRepository<OrderDetails> OrderDetails => _orderDetails ??= new GenericRepository<OrderDetails>(_context);
        public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
        public IGenericRepository<Discount> Discounts => _discounts ??= new GenericRepository<Discount>(_context);
        public IGenericRepository<Contact> Contacts => _contacts ??= new GenericRepository<Contact>(_context);
        public IGenericRepository<ProductByUser> ProductByUsers => _productByUsers ??= new GenericRepository<ProductByUser>(_context);
        public IGenericRepository<OtpEntity> OtpRepository => _OtpEntitys ??= new GenericRepository<OtpEntity>(_context);
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
