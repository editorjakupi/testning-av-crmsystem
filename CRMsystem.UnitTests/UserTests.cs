using server.Classes;
using server.Enums;
using Xunit;

namespace CRMsystem.UnitTests;

/// <summary>
/// Testklass för User-klassen som verifierar:
/// - Korrekt skapande av användarobjekt
/// - Hantering av olika användarroller
/// - Validering av användardata
/// - Felhantering för ogiltiga värden
/// </summary>
public class UserTests
{
    /// <summary>
    /// Testar att en användare skapas korrekt med alla egenskaper.
    /// Verifierar att:
    /// - ID sätts korrekt
    /// - Användarnamn sätts korrekt
    /// - Roll sätts korrekt
    /// - Företags-ID sätts korrekt
    /// - Företagsnamn sätts korrekt
    /// </summary>
    [Fact]
    public void User_Constructor_SetsAllProperties()
    {
        // Arrange - Förbereder testdata med förväntade värden
        int id = 1;
        string username = "testuser";
        Role role = Role.USER;
        int companyId = 100;
        string company = "Test Company";

        // Act - Skapar en ny användare med testdatan
        var user = new User(id, username, role, companyId, company);

        // Assert - Verifierar att alla egenskaper sattes korrekt
        Assert.Equal(id, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(role, user.Role);
        Assert.Equal(companyId, user.CompanyId);
        Assert.Equal(company, user.Company);
    }

    /// <summary>
    /// Testar att olika användarroller hanteras korrekt.
    /// Verifierar att:
    /// - USER-rollen sätts korrekt
    /// - ADMIN-rollen sätts korrekt
    /// - GUEST-rollen sätts korrekt
    /// </summary>
    /// <param name="role">Rollen som ska testas</param>
    [Theory]
    [InlineData(Role.USER)]    // Testar vanlig användarroll
    [InlineData(Role.ADMIN)]   // Testar administratörsroll
    [InlineData(Role.GUEST)]   // Testar gästroll
    public void User_WithDifferentRoles_SetsRoleCorrectly(Role role)
    {
        // Arrange - Skapar en användare med den specificerade rollen
        var user = new User(1, "testuser", role, 100, "Test Company");

        // Assert - Verifierar att rollen sattes korrekt
        Assert.Equal(role, user.Role);
    }

    /// <summary>
    /// Testar att användaren hanterar ogiltiga värden korrekt.
    /// Verifierar att:
    /// - Negativa ID:n accepteras
    /// - Tomma användarnamn accepteras
    /// - Negativa företags-ID:n accepteras
    /// - Null företagsnamn accepteras
    /// </summary>
    [Fact]
    public void User_WithInvalidValues_HandlesCorrectly()
    {
        // Arrange - Förbereder testdata med ogiltiga värden
        int id = -1;           // Ogiltigt ID (negativt)
        string username = "";  // Ogiltigt användarnamn (tomt)
        Role role = Role.USER;
        int companyId = -100; // Ogiltigt företags-ID (negativt)
        string company = null; // Ogiltigt företagsnamn (null)

        // Act - Skapar en användare med ogiltiga värden
        var user = new User(id, username, role, companyId, company);

        // Assert - Verifierar att alla värden accepteras utan fel
        Assert.Equal(id, user.Id);           // Kontrollerar att negativt ID accepteras
        Assert.Equal(username, user.Username); // Kontrollerar att tomt användarnamn accepteras
        Assert.Equal(companyId, user.CompanyId); // Kontrollerar att negativt företags-ID accepteras
        Assert.Null(user.Company);           // Kontrollerar att null företagsnamn accepteras
    }
}