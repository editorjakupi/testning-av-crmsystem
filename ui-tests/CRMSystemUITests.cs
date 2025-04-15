using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CRMSystemUITests;

/// <summary>
/// Testklass för att testa CRM-systemets användargränssnitt med Playwright
/// </summary>
[TestClass]
public class CRMSystemUITests
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;
    private const int Timeout = 60000; // 60 sekunder timeout

    // Användaruppgifter från databasen
    private const string AdminEmail = "m@email.com";
    private const string AdminPassword = "abc123";
    private const string UserEmail = "no@email.com";
    private const string UserPassword = "abc123";

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Skapa en ny Playwright-instans för varje test
        _playwright = await Playwright.CreateAsync();

        // Starta en ny webbläsare för varje test
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 100 // Lägg till lite fördröjning för att göra det lättare att se vad som händer
        });

        // Skapa en ny sida för varje test
        _page = await _browser.NewPageAsync();
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        // Stäng webbläsaren och frigör resurser
        if (_page != null) await _page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        if (_playwright != null) _playwright.Dispose();
    }

    /// <summary>
    /// Hjälpmetod för att logga in som admin
    /// </summary>
    private async Task LoginAsAdmin()
    {
        // Gå till inloggningssidan och vänta på att den laddas
        await _page.GotoAsync("http://localhost:3000/login", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = Timeout });

        // Vänta på att inloggningsformuläret laddas
        var form = await _page.WaitForSelectorAsync("form", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(form, "Inloggningsformuläret kunde inte hittas");

        // Fyll i inloggningsformuläret med admin-uppgifter
        var emailInput = await _page.WaitForSelectorAsync("input[type='text'][name='email']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(emailInput, "E-postfältet kunde inte hittas");
        await emailInput.FillAsync(AdminEmail);

        var passwordInput = await _page.WaitForSelectorAsync("input[type='password'][name='password']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(passwordInput, "Lösenordsfältet kunde inte hittas");
        await passwordInput.FillAsync(AdminPassword);

        // Klicka på inloggningsknappen och vänta på att sidan laddas om
        var submitButton = await _page.WaitForSelectorAsync("button[type='submit']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(submitButton, "Inloggningsknappen kunde inte hittas");
        await submitButton.ClickAsync();

        // Vänta på att sidan laddas om efter inloggning
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att vi kommer till startsidan
        await _page.WaitForURLAsync("http://localhost:3000/", new() { Timeout = Timeout });
    }

    /// <summary>
    /// Hjälpmetod för att logga in som användare
    /// </summary>
    private async Task LoginAsUser()
    {
        // Gå till inloggningssidan och vänta på att den laddas
        await _page.GotoAsync("http://localhost:3000/login", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = Timeout });

        // Vänta på att inloggningsformuläret laddas
        var form = await _page.WaitForSelectorAsync("form", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(form, "Inloggningsformuläret kunde inte hittas");

        // Fyll i inloggningsformuläret med användaruppgifter
        var emailInput = await _page.WaitForSelectorAsync("input[type='text'][name='email']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(emailInput, "E-postfältet kunde inte hittas");
        await emailInput.FillAsync(UserEmail);

        var passwordInput = await _page.WaitForSelectorAsync("input[type='password'][name='password']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(passwordInput, "Lösenordsfältet kunde inte hittas");
        await passwordInput.FillAsync(UserPassword);

        // Klicka på inloggningsknappen och vänta på att sidan laddas om
        var submitButton = await _page.WaitForSelectorAsync("button[type='submit']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(submitButton, "Inloggningsknappen kunde inte hittas");
        await submitButton.ClickAsync();

        // Vänta på att sidan laddas om efter inloggning
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att vi kommer till startsidan
        await _page.WaitForURLAsync("http://localhost:3000/", new() { Timeout = Timeout });
    }

    /// <summary>
    /// Hjälpmetod för att logga ut
    /// </summary>
    private async Task Logout()
    {
        // Klicka på utloggningsknappen
        var logoutButton = await _page.WaitForSelectorAsync("a:has-text('Logga ut')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(logoutButton, "Utloggningsknappen kunde inte hittas");
        await logoutButton.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att vi kommer till inloggningssidan
        await _page.WaitForURLAsync("http://localhost:3000/login", new() { Timeout = Timeout });
    }

    [TestMethod]
    public async Task AdminLoginTest()
    {
        await LoginAsAdmin();

        // Vänta på att välkomsttexten laddas
        var welcomeText = await _page.WaitForSelectorAsync("h1", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(welcomeText, "Välkomsttexten kunde inte hittas");

        var text = await welcomeText.TextContentAsync();
        Assert.IsNotNull(text);
        Assert.IsTrue(text.Contains("Välkommen"), "Välkomsttexten innehåller inte 'Välkommen'");

        // Logga ut efter testet
        await Logout();
    }

    [TestMethod]
    public async Task UserLoginTest()
    {
        await LoginAsUser();

        // Vänta på att välkomsttexten laddas
        var welcomeText = await _page.WaitForSelectorAsync("h1", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(welcomeText, "Välkomsttexten kunde inte hittas");

        var text = await welcomeText.TextContentAsync();
        Assert.IsNotNull(text);
        Assert.IsTrue(text.Contains("Välkommen"), "Välkomsttexten innehåller inte 'Välkommen'");

        // Logga ut efter testet
        await Logout();
    }

    [TestMethod]
    public async Task CreateIssueTest()
    {
        await LoginAsUser();

        // Vänta på att sidan laddas helt
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på länken för att skapa ärende
        var createIssueLink = await _page.WaitForSelectorAsync("a:has-text('Skapa ärende')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(createIssueLink, "Länken för att skapa ärende kunde inte hittas");
        await createIssueLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att formuläret laddas
        var form = await _page.WaitForSelectorAsync("form", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(form, "Formuläret för att skapa ärende kunde inte hittas");

        // Fyll i formuläret
        var titleInput = await _page.WaitForSelectorAsync("input[name='title']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(titleInput, "Titelfältet kunde inte hittas");
        await titleInput.FillAsync("Test Issue");

        var descriptionInput = await _page.WaitForSelectorAsync("textarea[name='description']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(descriptionInput, "Beskrivningsfältet kunde inte hittas");
        await descriptionInput.FillAsync("Test Description");

        // Klicka på spara-knappen
        var submitButton = await _page.WaitForSelectorAsync("button[type='submit']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(submitButton, "Spara-knappen kunde inte hittas");
        await submitButton.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att vi kommer till ärendesidan
        await _page.WaitForURLAsync("http://localhost:3000/issues/*", new() { Timeout = Timeout });

        // Vänta på att success-meddelandet laddas
        var successMessage = await _page.WaitForSelectorAsync(".success-message", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(successMessage, "Success-meddelandet kunde inte hittas");

        var message = await successMessage.TextContentAsync();
        Assert.IsNotNull(message);
        Assert.IsTrue(message.Contains("Ärende skapat"), "Success-meddelandet innehåller inte 'Ärende skapat'");

        // Logga ut efter testet
        await Logout();
    }

    [TestMethod]
    public async Task UpdateIssueTest()
    {
        await LoginAsAdmin();

        // Vänta på att sidan laddas helt
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på länken för ärenden
        var issuesLink = await _page.WaitForSelectorAsync("a:has-text('Ärenden')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(issuesLink, "Länken för ärenden kunde inte hittas");
        await issuesLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att ärendelistan laddas
        var issueLink = await _page.WaitForSelectorAsync("a:has-text('Test Issue')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(issueLink, "Länken till testärendet kunde inte hittas");
        await issueLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att kommentarsformuläret laddas
        var commentInput = await _page.WaitForSelectorAsync("textarea[name='comment']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(commentInput, "Kommentarsfältet kunde inte hittas");
        await commentInput.FillAsync("Test Comment");

        // Klicka på uppdatera-knappen
        var updateButton = await _page.WaitForSelectorAsync("button:has-text('Uppdatera')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(updateButton, "Uppdatera-knappen kunde inte hittas");
        await updateButton.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att kommentaren laddas
        var commentText = await _page.WaitForSelectorAsync(".comment-text", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(commentText, "Kommentarstexten kunde inte hittas");

        var text = await commentText.TextContentAsync();
        Assert.IsNotNull(text);
        Assert.IsTrue(text.Contains("Test Comment"), "Kommentarstexten innehåller inte 'Test Comment'");

        // Logga ut efter testet
        await Logout();
    }

    [TestMethod]
    public async Task CreateUserTest()
    {
        await LoginAsAdmin();

        // Vänta på att sidan laddas helt
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på länken för användare
        var usersLink = await _page.WaitForSelectorAsync("a:has-text('Användare')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(usersLink, "Länken för användare kunde inte hittas");
        await usersLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på länken för ny användare
        var newUserLink = await _page.WaitForSelectorAsync("a:has-text('Ny användare')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(newUserLink, "Länken för ny användare kunde inte hittas");
        await newUserLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att formuläret laddas
        var form = await _page.WaitForSelectorAsync("form", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(form, "Formuläret för att skapa användare kunde inte hittas");

        // Fyll i formuläret
        var emailInput = await _page.WaitForSelectorAsync("input[name='email']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(emailInput, "E-postfältet kunde inte hittas");
        await emailInput.FillAsync("newuser@example.com");

        var passwordInput = await _page.WaitForSelectorAsync("input[name='password']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(passwordInput, "Lösenordsfältet kunde inte hittas");
        await passwordInput.FillAsync("newuser123");

        var roleSelect = await _page.WaitForSelectorAsync("select[name='role']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(roleSelect, "Rullgardinsmenyn för roll kunde inte hittas");
        await roleSelect.SelectOptionAsync("user");

        // Klicka på spara-knappen
        var submitButton = await _page.WaitForSelectorAsync("button[type='submit']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(submitButton, "Spara-knappen kunde inte hittas");
        await submitButton.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att success-meddelandet laddas
        var successMessage = await _page.WaitForSelectorAsync(".success-message", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(successMessage, "Success-meddelandet kunde inte hittas");

        var message = await successMessage.TextContentAsync();
        Assert.IsNotNull(message);
        Assert.IsTrue(message.Contains("Användare skapad"), "Success-meddelandet innehåller inte 'Användare skapad'");

        // Logga ut efter testet
        await Logout();
    }

    [TestMethod]
    public async Task LogoutTest()
    {
        await LoginAsAdmin();

        // Vänta på att sidan laddas helt
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på utloggningsknappen
        var logoutButton = await _page.WaitForSelectorAsync("a:has-text('Logga ut')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(logoutButton, "Utloggningsknappen kunde inte hittas");
        await logoutButton.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att vi kommer till inloggningssidan
        await _page.WaitForURLAsync("http://localhost:3000/login", new() { Timeout = Timeout });

        // Vänta på att inloggningsformuläret laddas
        var loginForm = await _page.WaitForSelectorAsync("form", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(loginForm, "Inloggningsformuläret kunde inte hittas efter utloggning");
    }

    [TestMethod]
    public async Task CreateEmployeeTest()
    {
        await LoginAsAdmin();

        // Vänta på att sidan laddas helt
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på länken för användare
        var usersLink = await _page.WaitForSelectorAsync("a:has-text('Användare')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(usersLink, "Länken för användare kunde inte hittas");
        await usersLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Klicka på länken för ny användare
        var newUserLink = await _page.WaitForSelectorAsync("a:has-text('Add Employee')", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(newUserLink, "Länken för ny användare kunde inte hittas");
        await newUserLink.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att formuläret laddas
        var form = await _page.WaitForSelectorAsync("form", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(form, "Formuläret för att skapa användare kunde inte hittas");

        // Fyll i formuläret
        var firstnameInput = await _page.WaitForSelectorAsync("input[name='firstname']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(firstnameInput, "Förnamnsfältet kunde inte hittas");
        await firstnameInput.FillAsync("Test");

        var lastnameInput = await _page.WaitForSelectorAsync("input[name='lastname']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(lastnameInput, "Efternamnsfältet kunde inte hittas");
        await lastnameInput.FillAsync("User");

        var emailInput = await _page.WaitForSelectorAsync("input[name='email']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(emailInput, "E-postfältet kunde inte hittas");
        await emailInput.FillAsync("test.user@example.com");

        var passwordInput = await _page.WaitForSelectorAsync("input[name='password']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(passwordInput, "Lösenordsfältet kunde inte hittas");
        await passwordInput.FillAsync("test123");

        // Välj användarroll
        var userRoleRadio = await _page.WaitForSelectorAsync("input[value='USER']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(userRoleRadio, "Användarrollen kunde inte hittas");
        await userRoleRadio.CheckAsync();

        // Klicka på spara-knappen
        var submitButton = await _page.WaitForSelectorAsync("button[type='submit']", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(submitButton, "Spara-knappen kunde inte hittas");
        await submitButton.ClickAsync();

        // Vänta på att sidan laddas om
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = Timeout });

        // Vänta på att vi kommer till användarsidan
        await _page.WaitForURLAsync("http://localhost:3000/admin/employees", new() { Timeout = Timeout });

        // Vänta på att success-meddelandet laddas
        var successMessage = await _page.WaitForSelectorAsync(".success-message", new() { State = WaitForSelectorState.Visible, Timeout = Timeout });
        Assert.IsNotNull(successMessage, "Success-meddelandet kunde inte hittas");

        var message = await successMessage.TextContentAsync();
        Assert.IsNotNull(message);
        Assert.IsTrue(message.Contains("har registrerats"), "Success-meddelandet innehåller inte 'har registrerats'");

        // Logga ut efter testet
        await Logout();
    }
}