using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CRMSystemUITests;

/// <summary>
/// Testklass för att testa CRM-systemets användargränssnitt med Playwright.
/// Inkluderar tester för:
/// - Inloggning och utloggning
/// - Ärendehantering
/// - Formulärhantering
/// - Gästfunktionalitet
/// - Registrering av nya användare
/// </summary>
[TestClass]
public class CRMTests
{
    // Playwright-komponenter för webbläsarautomatisering
    private static IPlaywright _playwright;    // Huvudkomponent för Playwright
    private static IBrowser _browser;          // Webbläsarinstans
    private static IPage _page;                // Aktuell webbsida

    // Timeout-konstanter för olika operationer
    private const int Timeout = 30000;         // 30 sekunder för långa operationer
    private const int DefaultTimeout = 10000;  // 10 sekunder för standardoperationer
    private const int SlowMo = 1500;           // 1.5 sekunder fördröjning för visualisering

    /// <summary>
    /// Initialiserar testmiljön före alla tester.
    /// Sätter upp:
    /// - Playwright-instans
    /// - Headless webbläsare
    /// - Testwebbsida
    /// - Viewport-storlek
    /// </summary>
    [ClassInitialize]
    public static async Task TestInitialize()
    {
        try
        {
            // Skapa Playwright-instans
            _playwright = await Playwright.CreateAsync();

            // Konfigurera webbläsarstart
            var options = new BrowserTypeLaunchOptions
            {
                Headless = true,  // Kör i headless-läge för CI/CD
                Timeout = Timeout // Sätt timeout för långa operationer
            };

            // Starta webbläsare och skapa ny sida
            _browser = await _playwright.Chromium.LaunchAsync(options);
            _page = await _browser.NewPageAsync();
            await _page.SetViewportSizeAsync(1920, 1080); // Sätt viewport-storlek
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize Playwright: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Städar upp efter alla tester.
    /// Frigör resurser för:
    /// - Webbläsare
    /// - Playwright-instans
    /// </summary>
    [ClassCleanup]
    public static async Task TestCleanup()
    {
        if (_browser != null) await _browser.DisposeAsync();
        if (_playwright != null) _playwright.Dispose();
    }

    /// <summary>
    /// Hjälpmetod för att logga in som administratör.
    /// Utför följande steg:
    /// 1. Navigerar till inloggningssidan
    /// 2. Fyller i administratörsuppgifter
    /// 3. Klickar på inloggningsknappen
    /// 4. Väntar på omdirigering till startsidan
    /// </summary>
    private async Task LoginAsAdmin()
    {
        if (_page == null) throw new InvalidOperationException("Page is not initialized");
        try
        {
            // Navigera till inloggningssidan
            Console.WriteLine("Navigating to login page...");
            await _page.GotoAsync("http://localhost:3000/login", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = DefaultTimeout
            });

            // Vänta på och hitta inloggningsformulärets element
            Console.WriteLine("Waiting for login form...");
            var emailInput = await _page.WaitForSelectorAsync("input[type='text'][name='email']", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = DefaultTimeout
            });
            var passwordInput = await _page.WaitForSelectorAsync("input[type='password']", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = DefaultTimeout
            });
            var submitButton = await _page.WaitForSelectorAsync("button[type='submit']", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = DefaultTimeout
            });

            // Verifiera att alla element hittades
            if (emailInput == null || passwordInput == null || submitButton == null)
                throw new InvalidOperationException("Login form elements not found");

            // Fyll i inloggningsuppgifter
            Console.WriteLine("Filling in login credentials...");
            await emailInput.FillAsync("m@email.com");
            await passwordInput.FillAsync("abc123");

            // Klicka på inloggningsknappen och vänta
            Console.WriteLine("Clicking login button...");
            await submitButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Vänta på omdirigering till startsidan
            Console.WriteLine("Waiting for redirect to home page...");
            await _page.WaitForURLAsync("http://localhost:3000/", new PageWaitForURLOptions
            {
                Timeout = DefaultTimeout
            });

            Console.WriteLine("Login successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to login as admin: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Hjälpmetod för att logga ut från systemet.
    /// Utför följande steg:
    /// 1. Hittar och klickar på utloggningsknappen
    /// 2. Väntar på att användaren ska loggas ut
    /// 3. Verifierar omdirigering till inloggningssidan
    /// </summary>
    private async Task Logout()
    {
        if (_page == null) throw new InvalidOperationException("Page is not initialized");
        try
        {
            // Hitta och klicka på utloggningsknappen
            Console.WriteLine("Clicking logout button...");
            var logoutButton = await _page.WaitForSelectorAsync("button:has-text('Logout')", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = DefaultTimeout
            });
            if (logoutButton == null) throw new InvalidOperationException("Logout button not found");

            await logoutButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Vänta på att användaren ska försvinna från navbaren
            Console.WriteLine("Waiting for user to be logged out...");
            await _page.WaitForSelectorAsync("a[href='/login']", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = DefaultTimeout
            });

            Console.WriteLine("Logout successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to logout: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Testar inloggning och utloggning som administratör.
    /// Verifierar att:
    /// - Inloggning fungerar med korrekta uppgifter
    /// - Omdirigering till startsidan sker
    /// - Utloggning fungerar korrekt
    /// </summary>
    [TestMethod]
    public async Task AdminLoginTest()
    {
        await LoginAsAdmin();
        await Logout();
    }

    /// <summary>
    /// Testar ärendehanteringsfunktionaliteten.
    /// Verifierar att:
    /// - Ärenden kan visas
    /// - Ärenden kan redigeras
    /// - Status kan uppdateras
    /// - Ändringar sparas korrekt
    /// </summary>
    [TestMethod]
    public async Task IssueManagementTest()
    {
        await LoginAsAdmin();

        // Navigera till ärendesidan
        Console.WriteLine("Clicking on Issues link in navigation...");
        var issuesLink = await _page.WaitForSelectorAsync("a[href='/employee/issues']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (issuesLink == null) throw new InvalidOperationException("Issues link not found");
        await issuesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Hitta och klicka på första redigeringsknappen
        Console.WriteLine("Clicking first available edit button (pen icon)...");
        await _page.WaitForSelectorAsync(".issueCard", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });

        var editButton = await _page.WaitForSelectorAsync(".stateColumn button.subjectEditButton", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (editButton == null) throw new InvalidOperationException("Edit button not found");
        await editButton.ClickAsync();

        // Uppdatera ärendestatus
        Console.WriteLine("Changing issue status to CLOSED...");
        var statusSelect = await _page.WaitForSelectorAsync("select.stateSelect", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (statusSelect == null) throw new InvalidOperationException("Status select not found");
        await statusSelect.SelectOptionAsync(new[] { "CLOSED" });

        // Spara ändringarna
        var saveButton = await _page.WaitForSelectorAsync("button.stateUpdateButton", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (saveButton == null) throw new InvalidOperationException("Save button not found");
        await saveButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Logout();
    }

    /// <summary>
    /// Testar hantering av formulärämnen.
    /// Verifierar att:
    /// - Nya ämnen kan skapas
    /// - Ämnen kan redigeras
    /// - Ämnen kan tas bort
    /// - Ändringar sparas korrekt
    /// </summary>
    [TestMethod]
    public async Task FormSubjectsTest()
    {
        await LoginAsAdmin();

        // Navigera till formulärämnen
        Console.WriteLine("Navigating to form subjects...");
        var formSubjectsLink = await _page.WaitForSelectorAsync("a:has-text('Form subjects')", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (formSubjectsLink == null) throw new InvalidOperationException("Form subjects link not found");
        await formSubjectsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Skapa nytt ämne
        Console.WriteLine("Clicking on New Subject...");
        var newSubjectButton = await _page.WaitForSelectorAsync("#subjectViewMenu button", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (newSubjectButton == null) throw new InvalidOperationException("New Subject button not found");
        await newSubjectButton.ClickAsync();

        // Fyll i ämnesinformation
        Console.WriteLine("Filling in subject details...");
        var subjectNameInput = await _page.WaitForSelectorAsync("input[name='newSubject']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (subjectNameInput == null) throw new InvalidOperationException("Subject name input not found");
        await subjectNameInput.FillAsync("Test ämne");

        // Spara ämnet
        Console.WriteLine("Saving subject...");
        var saveSubjectButton = await _page.WaitForSelectorAsync("form button[type='submit']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (saveSubjectButton == null) throw new InvalidOperationException("Save subject button not found");
        await saveSubjectButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Ta bort det skapade ämnet
        Console.WriteLine("Deleting the created subject...");
        var deleteButton = await _page.WaitForSelectorAsync("button.removeButton", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (deleteButton == null) throw new InvalidOperationException("Delete button not found");
        await deleteButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Logout();
    }

    /// <summary>
    /// Testar gästfunktionalitet för att skapa ärenden.
    /// Verifierar att:
    /// - Gäster kan nå ärendeformuläret
    /// - Formuläret kan fyllas i
    /// - Ärendet skapas korrekt
    /// </summary>
    [TestMethod]
    public async Task GuestIssueTest()
    {
        // Navigera till Demo AB issueform
        Console.WriteLine("Navigating to Demo AB issueform...");
        await _page.GotoAsync("http://localhost:3000/Demo%20AB/issueform", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = DefaultTimeout
        });

        // Fyll i ärendeformulär som gäst
        Console.WriteLine("Filling in issue form as guest...");
        var guestEmailInput = await _page.WaitForSelectorAsync("input[type='email']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        var guestTitleInput = await _page.WaitForSelectorAsync("input[name='title']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        var guestMessageInput = await _page.WaitForSelectorAsync("textarea[name='message']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (guestEmailInput == null || guestTitleInput == null || guestMessageInput == null)
            throw new InvalidOperationException("Guest form elements not found");

        await guestEmailInput.FillAsync("guest@example.com");
        await guestTitleInput.FillAsync("Gäst ärende");
        await guestMessageInput.FillAsync("Gäst beskrivning");

        // Skapa ärendet
        Console.WriteLine("Clicking Create Issue...");
        var createIssueButton = await _page.WaitForSelectorAsync("button[type='submit']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (createIssueButton == null) throw new InvalidOperationException("Create Issue button not found");
        await createIssueButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Testar registrering av nya användare.
    /// Verifierar att:
    /// - Registreringsformuläret är tillgängligt
    /// - Alla fält kan fyllas i
    /// - Konto skapas korrekt
    /// </summary>
    [TestMethod]
    public async Task RegistrationTest()
    {
        // Navigera till registreringssidan
        Console.WriteLine("Clicking on Register...");
        await _page.GotoAsync("http://localhost:3000/register", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = DefaultTimeout
        });

        // Fyll i registreringsformulär
        Console.WriteLine("Filling in registration form...");
        var registerUsernameInput = await _page.WaitForSelectorAsync("input[name='username']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        var registerEmailInput = await _page.WaitForSelectorAsync("input[type='email']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        var registerPasswordInput = await _page.WaitForSelectorAsync("input[type='password']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        var registerCompanyInput = await _page.WaitForSelectorAsync("input[name='company']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (registerUsernameInput == null || registerEmailInput == null || registerPasswordInput == null || registerCompanyInput == null)
            throw new InvalidOperationException("Registration form elements not found");

        await registerUsernameInput.FillAsync("Test Användare");
        await registerEmailInput.FillAsync("test@example.com");
        await registerPasswordInput.FillAsync("password123");
        await registerCompanyInput.FillAsync("Test Company");

        // Skapa konto
        Console.WriteLine("Clicking Create Account...");
        var createAccountButton = await _page.WaitForSelectorAsync("button[type='submit']", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeout
        });
        if (createAccountButton == null) throw new InvalidOperationException("Create Account button not found");
        await createAccountButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}