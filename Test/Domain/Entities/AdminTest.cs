using MinimalAPI.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdminTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var adm = new Administrator();

        // Act
        adm.Id = 1;
        adm.Email = "test@test.com";
        adm.Password = "test";
        adm.Profile = "Adm";

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("test", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}
