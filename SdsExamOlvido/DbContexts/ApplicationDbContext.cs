using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SdsExamOlvido.Models;
using System.Reflection.Emit;

namespace SdsExamOlvido.DbContexts
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }
        public DbSet<RecyclableItem> RecyclableItems { get; set; }
        public DbSet<RecyclableType> RecyclableTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RecyclableItem>().ToTable("RecyclableItems");
            modelBuilder.Entity<RecyclableType>().ToTable("RecyclableTypes");

            //set Type as unique
            modelBuilder.Entity<RecyclableType>().HasIndex(p => p.Type).IsUnique();

            //2 decimal places
            modelBuilder.Entity<RecyclableItem>().Property(p => p.Weight).HasPrecision(18, 2);
            modelBuilder.Entity<RecyclableItem>().Property(p => p.ComputedRate).HasPrecision(18, 2);

            modelBuilder.Entity<RecyclableType>().Property(p => p.Rate).HasPrecision(18, 2);
            modelBuilder.Entity<RecyclableType>().Property(p => p.MinKg).HasPrecision(18, 2);
            modelBuilder.Entity<RecyclableType>().Property(p => p.MaxKg).HasPrecision(18, 2);

            //description max length 150 and varchar
            modelBuilder.Entity<RecyclableItem>()
                .Property(p => p.ItemDescription).HasMaxLength(150)
                .HasColumnType("varchar");

            //type max length 10 and varchar
            modelBuilder.Entity<RecyclableType>()
                .Property(p => p.Type).HasMaxLength(10)
                .HasColumnType("varchar");


        }

    }

}