using Microsoft.EntityFrameworkCore;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Data.Context;
using SquaresApp.Domain.Models;
using System;
using System.Collections.Generic;

namespace SquaresApp.Data.Tests
{
    public class RepositoryBase
    {
        public readonly SquaresAppDBContext _squaresAppDBContext;
        public readonly long userId = 1;
        public RepositoryBase()
        {

            var dbContextOptions = new DbContextOptionsBuilder<SquaresAppDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _squaresAppDBContext = new SquaresAppDBContext(dbContextOptions);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _squaresAppDBContext.Users.Add(new User { Id = 1, Password = "123Abc".ToSHA256(), Username = "Username" });
            _squaresAppDBContext.Points.AddRange(new List<Point> { new Point { Id = 1, UserId = userId, X = -1, Y = -1 }, new Point { Id = 2, UserId = userId, X = 1, Y = -1 } });
            _squaresAppDBContext.SaveChanges();
        }

    }
}
