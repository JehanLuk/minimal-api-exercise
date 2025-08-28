using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Services;

namespace Test.Domain.Entities;

[TestClass]
public class AdminServiceTest
{
    private AppDbContext CreateTestContext()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("psql");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    [TestMethod]
    public void TestSaveAdmin()
    {
        // Arrange
        var context = CreateTestContext(); 
        context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Administrators"" RESTART IDENTITY CASCADE;");
        var adm = new Administrator
        {
            Email = "test@test.com",
            Password = "test",
            Profile = "Adm"   
        };

        var adminService = new AdministratorService(context);

        // Act
        adminService.Include(adm);
        var bankAdmin = adminService.SearchById(adm.Id);

        // Assert
        Assert.AreEqual(1, bankAdmin?.Id);
    }
}
