using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Data
{
    public class WebStoreDbInitializer
    {
        private readonly WebStoreDB _db;
        private readonly ILogger<WebStoreDbInitializer> _logger;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<Role> _RoleManager;

        public WebStoreDbInitializer(
            WebStoreDB db, 
            UserManager<User> UserManager,
            RoleManager<Role> RoleManager,
            ILogger<WebStoreDbInitializer> logger)
        {
            _db = db;
            _logger = logger;
            _UserManager = UserManager;
            _RoleManager = RoleManager;
        }

        public async Task InitializeAsync ()
        {
            _logger.LogInformation("Начата инициализация БД");

            
            if (_db.Database.ProviderName.EndsWith(".InMemory"))
                await _db.Database.EnsureCreatedAsync();
            else
            {
                var pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _db.Database.GetAppliedMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Применение миграций: {0}", string.Join(",", pendingMigrations));
                    await _db.Database.MigrateAsync();
                }
            }

            try
            {
                await InitializeProductsAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка инициализации каталога товаров");
                throw;
            }

            try
            {
                await InitializeIdentityAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка инициализации системы Identity");
                throw;
            }
        }

        private async Task InitializeProductsAsync()
        {
            if (_db.Sections.Any())
            {
                _logger.LogInformation("Инициализация БД уже совершена");
                return;
            }

            var sectionsData = TestData.Sections.ToDictionary(section => section.Id);
            var brandsData = TestData.Brands.ToDictionary(brand => brand.Id);
            
            foreach (var childSection in TestData.Sections.Where(s => s.ParentId.HasValue))
                childSection.Parent = sectionsData[childSection.ParentId.Value];

            foreach(var product in TestData.Products)
            {
                product.Section = sectionsData[product.SectionId];
                if (product.BrandId.HasValue)
                    product.Brand = brandsData[product.BrandId.Value];

                product.Id = 0;
                product.BrandId = null;
                product.SectionId = 0;
            }

            foreach (var section in TestData.Sections)
            {
                section.Id = 0;
                section.ParentId = null;
            }

            foreach (var brand in TestData.Brands)
                brand.Id = 0;

            _logger.LogInformation("Начата запись Продуктов");

            await using (await _db.Database.BeginTransactionAsync())
            {
                _db.Sections.AddRange(TestData.Sections);
                _db.Brands.AddRange(TestData.Brands);
                _db.Products.AddRange(TestData.Products);

                await _db.SaveChangesAsync();
                await _db.Database.CommitTransactionAsync();
            }
            _logger.LogInformation("Запись Продуктов успешно завершена");
        }

        private async Task InitializeIdentityAsync()
        {
            _logger.LogInformation("Инициализации системы Identity");

            async Task CheckRole(string roleName)
            {
                if (await _RoleManager.RoleExistsAsync(roleName))
                    _logger.LogInformation($"Роль {roleName} уже существует");
                else
                {
                    _logger.LogInformation($"Роль {roleName} не существует");
                    await _RoleManager.CreateAsync(new Role { Name = roleName });
                    _logger.LogInformation($"Роль {roleName} успешно создана");
                }
            }


            await CheckRole(Role.Administrators);
            await CheckRole(Role.Users);

            if(await _UserManager.FindByNameAsync(User.Administrator) is null)
            {
                _logger.LogInformation($"Пользователь {User.Administrator} не существует");

                var admin = new User
                {
                    UserName = User.Administrator
                };

                var result = await _UserManager.CreateAsync(admin, User.DefaultAdminPassword);
                if(result.Succeeded)
                {
                    _logger.LogInformation($"Пользователь {User.Administrator} успешно создан");

                    await _UserManager.AddToRoleAsync(admin, Role.Administrators);

                    _logger.LogInformation($"Пользователь {User.Administrator} успешно добавлен в роль {Role.Administrators}");
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToArray();
                    _logger.LogError("Учетная запись Администратора не создана. Ошибки: {0}", 
                        string.Join(", ", errors));
                    throw new InvalidOperationException($"Не возможно создать Администратора {string.Join(", ", errors)}");
                }
                _logger.LogInformation("Данные для системы Identity успешно добавлены в БД");
            }
        }
    }

}
