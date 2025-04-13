using server.Classes;
using server.Enums;
using Xunit;

namespace CRMsystem.UnitTests;

/// <summary>
/// Testklass för User
/// </summary>
public class UserTests
{
    /// <summary>
    /// Testar att en användare skapas korrekt med alla egenskaper
    /// </summary>
    [Fact]
    public void User_Constructor_SetsAllProperties()
    {
        // Arrange
        int id = 1;
        string username = "testuser";
        Role role = Role.USER;
        int companyId = 100;
        string company = "Test Company";

        // Act
        var user = new User(id, username, role, companyId, company);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(role, user.Role);
        Assert.Equal(companyId, user.CompanyId);
        Assert.Equal(company, user.Company);
    }

    /// <summary>
    /// Testar att olika roller hanteras korrekt
    /// </summary>
    [Theory]
    [InlineData(Role.USER)]
    [InlineData(Role.ADMIN)]
    [InlineData(Role.GUEST)]
    public void User_WithDifferentRoles_SetsRoleCorrectly(Role role)
    {
        // Arrange
        var user = new User(1, "testuser", role, 100, "Test Company");

        // Assert
        Assert.Equal(role, user.Role);
    }

    /// <summary>
    /// Testar att användaren hanterar ogiltiga värden korrekt
    /// </summary>
    [Fact]
    public void User_WithInvalidValues_HandlesCorrectly()
    {
        // Arrange
        int id = -1;  // Ogiltigt ID
        string username = "";  // Tomt användarnamn
        Role role = Role.USER;
        int companyId = -100;  // Ogiltigt företags-ID
        string company = null;  // Null företagsnamn

        // Act
        var user = new User(id, username, role, companyId, company);

        // Assert
        Assert.Equal(id, user.Id);  // Kontrollerar att negativt ID accepteras
        Assert.Equal(username, user.Username);  // Kontrollerar att tomt användarnamn accepteras
        Assert.Equal(companyId, user.CompanyId);  // Kontrollerar att negativt företags-ID accepteras
        Assert.Null(user.Company);  // Kontrollerar att null företagsnamn accepteras
    }
}