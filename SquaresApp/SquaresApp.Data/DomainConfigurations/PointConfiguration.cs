using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquaresApp.Data.Models;

namespace SquaresApp.Data.DomainConfigurations
{
    internal class PointConfiguration : IEntityTypeConfiguration<Point>
    {
        /// <summary>
        /// configure schema of points table and add seed data.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Point> builder)
        {
            builder.ToTable("Points");

            builder.HasKey(entity => entity.Id).HasName("PK_Points_Id");
            builder.Property(prop => prop.Id).ValueGeneratedOnAdd().IsRequired();


            builder.HasIndex(prop => prop.UserId).HasDatabaseName("IDX_Point_UserId");//To insure fast retrieval of points for a specific user
            builder.HasIndex(prop => new { prop.UserId, prop.X, prop.Y }).IsUnique().HasDatabaseName("IDX_Point_UserId_X_Y"); //To insure uniqueness of points per user. Also helpful in order by coordinate queries.

            builder.Property(prop => prop.UserId).IsRequired();
            builder.Property(prop => prop.X).IsRequired();
            builder.Property(prop => prop.Y).IsRequired();


            builder.HasData(
                new Point
                {
                    Id = 1,
                    UserId = 1,
                    X = -1,
                    Y = 1
                },
                new Point
                {
                    Id = 2,
                    UserId = 1,
                    X = 1,
                    Y = -1
                },
                new Point
                {
                    Id = 3,
                    UserId = 1,
                    X = 1,
                    Y = 1
                },
                new Point
                {
                    Id = 4,
                    UserId = 1,
                    X = -1,
                    Y = -1
                },
                new Point
                {
                    Id = 5,
                    UserId = 2,
                    X = -1,
                    Y = 1
                },
                new Point
                {
                    Id = 6,
                    UserId = 2,
                    X = 1,
                    Y = -1
                },
                new Point
                {
                    Id = 7,
                    UserId = 2,
                    X = 1,
                    Y = 1
                },
                new Point
                {
                    Id = 8,
                    UserId = 2,
                    X = -1,
                    Y = -1
                });
        }
    }

}
