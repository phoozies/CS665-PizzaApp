using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS665_PizzaRestaurantApp.Models
{
    class ApplicationDbContext : DbContext
    {
        public DbSet<CustomerModel> CustomerModels { get; set; }
        public DbSet<OrderModel> OrderModels { get; set; }
        public DbSet<MenuItemModel> MenuItemModels { get; set; }
        public DbSet<OrderDetailModel> OrderDetailModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.Order)
                .WithMany()
                .HasForeignKey(od => od.OrderID);

            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.MenuItem)
                .WithMany()
                .HasForeignKey(od => od.ItemID);
        }
    }
}
