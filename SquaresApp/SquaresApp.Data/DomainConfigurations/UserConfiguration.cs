using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Data.Models;

namespace SquaresApp.Data.DomainConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// configure schema of user table and add seed data.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(prop => prop.Id).HasName("PK_User_Id");
            builder.Property(prop => prop.Id).ValueGeneratedOnAdd().IsRequired();

            builder.HasIndex(prop => prop.Username).IsUnique().HasDatabaseName("IDX_User_Username");

            builder.Property(prop => prop.Username).HasMaxLength(50).IsRequired();
            builder.Property(prop => prop.Password).HasMaxLength(64).IsRequired();

            builder.HasMany(x => x.Points).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new User
                {
                    Id = 1,
                    Username = "Admin",
                    Password = "Admin".ToSHA256(),
                }, new User
                {
                    Id = 2,
                    Username = "User",
                    Password = "User".ToSHA256(),
                });
        }
    }
}
