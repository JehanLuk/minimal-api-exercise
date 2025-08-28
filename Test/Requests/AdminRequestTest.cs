using MinimalAPI.Domain.Entities;
using MinimalAPI.Models.ModelView;
using MinimalAPI.DTOs;
using Test.Helpers;
using System.Text.Json;
using System.Text;
using System.Net;

namespace Test.Requests;

[TestClass]
public class AdminRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
       Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestGetSetProperties()
    {
        // Arrange
        var loginDTO = new LoginDTO{
            Email = "adm@test.com",
            Password = "1234"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/admin/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogin = JsonSerializer.Deserialize<AdminLogged>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogin?.Email ?? "");
        Assert.IsNotNull(admLogin?.Profile ?? "");
        Assert.IsNotNull(admLogin?.Token ?? "");
    }
}
