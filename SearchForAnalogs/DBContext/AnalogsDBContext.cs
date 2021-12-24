using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchForAnalogs.Models;
using Microsoft.EntityFrameworkCore;

namespace SearchForAnalogs
{
    class AnalogsDBContext : DbContext
    {
        public DbSet<Manufacturer> Manufactureres { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Record> Records { get; set; } = null!;
        public AnalogsDBContext()
        {
            //при создании контекста автоматически проверяет наличие базы данных
            Database.EnsureCreated();

        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //подключение для соединения бд
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AnalogsDB;Trusted_Connection=True;AttachDbFileName=|DataDirectory|DataBase\AnalogsDB.mdf");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         

            //обозначение связей между таблицами
            modelBuilder.Entity<Product>()
                .HasOne(m => m.Manufacturer)
                .WithMany(t => t.Products)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Record>()
                .HasOne(m => m.Product1)
                .WithMany(t => t.Records1)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Record>()
               .HasOne(m => m.Product2)
               .WithMany(t => t.Records2)
               .OnDelete(DeleteBehavior.ClientCascade);

        }
       
    }
}