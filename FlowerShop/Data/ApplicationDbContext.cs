using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;

namespace FlowerShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<FlowerProduct> FlowerProducts { get; set; }

        public DbSet<FlowerCategory> FlowerCategories { get; set; }

        public DbSet<FlowerCart> FlowerCarts { get; set; }

        public DbSet<FlowerCartProduct> FlowerCartProducts { get; set; }

        public DbSet<FlowerOrder> FlowerOrders { get; set; }

        public DbSet<FlowerOrderProduct> FlowerOrderProducts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            
            builder.Entity<FlowerCategory>().HasKey(x => x.Name);

            //Great spot for Default values and other rules!
            builder.Entity<FlowerCategory>().Property(x => x.DateCreated).HasDefaultValueSql("GetDate()");
            builder.Entity<FlowerCategory>().Property(x => x.DateLastModified).HasDefaultValueSql("GetDate()");
            builder.Entity<FlowerCategory>().Property(x => x.Name).HasMaxLength(100);
            
            builder.Entity<ApplicationUser>()
                .HasOne(x => x.FlowerCart)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<FlowerCart>(x => x.ApplicationUserID);
            
        }
    }
}
