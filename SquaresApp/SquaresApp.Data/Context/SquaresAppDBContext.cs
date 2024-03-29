﻿using Microsoft.EntityFrameworkCore;
using SquaresApp.Data.DomainConfigurations;
using SquaresApp.Data.Models;

namespace SquaresApp.Data.Context
{
    public class SquaresAppDBContext : DbContext
    {
        public SquaresAppDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Point> Points { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PointConfiguration());

        }

    }
}
