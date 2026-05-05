using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce528.Utilities.DbInitializers
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<DbInitializer> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Initialize()
        {
            try
            {
                // 1. Update Data Base
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();

                // 2. Create Roles
                if (_roleManager.Roles.IsNullOrEmpty())
                {
                    await _roleManager.CreateAsync(new(RoleNames.SUPER_ADMIN));
                    await _roleManager.CreateAsync(new(RoleNames.ADMIN));
                    await _roleManager.CreateAsync(new(RoleNames.CUSTOMER));
                    await _roleManager.CreateAsync(new(RoleNames.EMPLOYEE));
                }

                // 3. Create Admin User
                if (await _userManager.FindByEmailAsync(_configuration["SuperAdminAccount:Email"]!) is null)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = _configuration["SuperAdminAccount:Username"],
                        Email = _configuration["SuperAdminAccount:Email"],
                        EmailConfirmed = true
                    };

                    await _userManager.CreateAsync(user, _configuration["SuperAdminAccount:Password"]!);
                    await _userManager.AddToRoleAsync(user, RoleNames.SUPER_ADMIN);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
