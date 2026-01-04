//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProManage360.Persistence.Contexts
//{
//    using global::ProManage360.Infrastructure.Identity;
//    using global::ProManage360.Persistence;
//    using Microsoft.EntityFrameworkCore;
//    using Microsoft.EntityFrameworkCore.Design;
//    using Microsoft.Extensions.Configuration;

//    namespace ProManage360.Persistence.Contexts
//    {
//        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//        {
//            public ApplicationDbContext CreateDbContext(string[] args)
//            {
//                IConfigurationRoot configuration = new ConfigurationBuilder()
//                    .SetBasePath(AppContext.BaseDirectory)
//                    .AddJsonFile("appsettings.json")
//                    .Build();

//                string? connectionString = configuration.GetConnectionString("DefaultConnection");
//                if (string.IsNullOrEmpty(connectionString))
//                {
//                    throw new InvalidOperationException("Connection String is not valid");
//                }
//                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//                // Use your connection string
//                optionsBuilder.UseSqlServer(connectionString);

//                var currentUserService = new CurrentUserService();

//                return new ApplicationDbContext(optionsBuilder.Options);
//            }
//        }
//    }

//}
