using Microsoft.EntityFrameworkCore;

namespace NET_CRM_project_for_COM.Model
{


    public class ApplicationContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения к LocalDB
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CRMDatabase;Trusted_Connection=True;");
        }
        public bool DatabaseExists()
        {
            return Database.EnsureCreated(); // Проверяет, существует ли база данных
        }
    }

}
