//using Microsoft.EntityFrameworkCore;
//using ProManage360.Domain.Entities;
//using ProManage360.Domain.Enums;
//using ProManage360.Persistence.Contexts;
//using BCrypt.Net;

//namespace ProManage360.Persistence.Seeders
//{
//    public class DatabaseSeeder
//    {
//        private readonly ApplicationDbContext _context;

//        public DatabaseSeeder(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task SeedAsync()
//        {
//            // Apply any pending migrations
//            await _context.Database.MigrateAsync();

//            // Seed System Tenant
//            await SeedSystemTenantAsync();

//            // Seed SuperAdmin User
//            await SeedSuperAdminAsync();
//        }

//        private async Task SeedSystemTenantAsync()
//        {
//            var systemTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

//            // Check if already exists
//            var exists = await _context.Tenants
//                .IgnoreQueryFilters()
//                .AnyAsync(t => t.Id == systemTenantId);

//            if (exists)
//                return;

//            var systemTenant = new Tenant
//            {
//                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
//                Name = "System",
//                Domain = "system",
//                SubscriptionTier = SubscriptionTier.Enterprise,
//                CreatedAt = DateTime.UtcNow,
//                IsDeleted = false
//            };

//            _context.Tenants.Add(systemTenant);
//            await _context.SaveChangesAsync();

//            Console.WriteLine("✅ System Tenant created successfully");
//        }

//        private async Task SeedSuperAdminAsync()
//        {
//            var systemTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
//            var superAdminUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");
//            var superAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

//            // Check if SuperAdmin already exists
//            var exists = await _context.Users
//                .IgnoreQueryFilters()
//                .AnyAsync(u => u.Id == superAdminUserId);

//            if (exists)
//                return;

//            var superAdminUser = new User
//            {
//                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
//                TenantId = systemTenantId,  // ✅ Belongs to System tenant
//                Email = "superadmin@promanage360.com",
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"),  // Change in production!
//                FullName = "Super Admin",
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow,
//                IsDeleted = false
//            };

//            _context.Users.Add(superAdminUser);

//            // Assign SuperAdmin role
//            var userRole = new UserRole
//            {
//                UserId = superAdminUserId,
//                RoleId = superAdminRoleId,
//                AssignedAt = DateTime.UtcNow
//            };

//            _context.UserRoles.Add(userRole);

//            await _context.SaveChangesAsync();

//            Console.WriteLine("✅ SuperAdmin user created successfully");
//            Console.WriteLine($"   Email: {superAdminUser.Email}");
//            Console.WriteLine($"   Password: SuperAdmin@123");
//        }
//    }
//}
