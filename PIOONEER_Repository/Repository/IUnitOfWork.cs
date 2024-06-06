
using PIOONEER_Repository.Entity;
using System.Security.Cryptography;

namespace PIOONEER_Repository.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderDetails> OrderDetails { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Discount> Discounts { get; }
        IGenericRepository<Contact> Contacts { get; }
        IGenericRepository<ProductByUser> ProductByUsers { get; }

        Task<int> SaveChangesAsync();
        void Save();
    }
}
