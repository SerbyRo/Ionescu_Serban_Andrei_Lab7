using Ionescu_Serban_Andrei_Lab2.Models;
using Microsoft.EntityFrameworkCore;
using Ionescu_Serban_Andrei_Lab2.Models;
using LibraryModel.Models;

namespace Ionescu_Serban_Andrei_Lab2.Data_WebAPI
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) :
        base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<PublishedBook> PublishedBooks { get; set; }
        public DbSet<City> Cities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Author>().ToTable("Author");
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<City>().ToTable("City");
            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<PublishedBook>().ToTable("PublishedBook");

            modelBuilder.Entity<PublishedBook>()
                .HasKey(c => new { c.BookID, c.PublisherID });

            modelBuilder.Entity<Book>().HasOne(b=>b.Author).WithMany(a=>a.Books).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
