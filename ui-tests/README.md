# CRM System UI Tests

Detta projekt innehåller UI-tester för CRM-systemet med hjälp av Playwright och MSTest.

## Förutsättningar

- .NET 8.0 SDK
- Node.js och npm
- En webbläsare (Chromium används som standard)

## Installation

1. Installera Playwright-drivrutiner:

```bash
pwsh -c "pwsh -Command '& { $env:PLAYWRIGHT_BROWSERS_PATH=0; npx playwright install chromium }'"
```

2. Bygg projektet:

```bash
dotnet build
```

## Köra testerna

För att köra testerna, se till att CRM-systemet är igång på http://localhost:3000 och kör sedan:

```bash
dotnet test
```

## Testfall

Följande testfall ingår i testpaketet:

1. **AdminLoginTest** - Testar inloggning som admin-användare
2. **UserLoginTest** - Testar inloggning som vanlig användare
3. **CreateIssueTest** - Testar att skapa en ny issue
4. **UpdateIssueTest** - Testar att uppdatera en befintlig issue
5. **CreateUserTest** - Testar att skapa en ny användare
6. **LogoutTest** - Testar utloggning

## Anpassa testerna

För att anpassa testerna till ditt specifika CRM-system, kan du behöva ändra följande:

- URL:er i testerna (standard är http://localhost:3000)
- Användaruppgifter (e-post och lösenord)
- Elementselektorer (om din UI använder andra namn eller strukturer)

## Felsökning

Om testerna misslyckas, kontrollera följande:

1. Att CRM-systemet är igång och tillgängligt
2. Att användaruppgifterna är korrekta
3. Att elementselektorerna matchar din UI
4. Att webbläsaren är korrekt installerad

## Lägga till nya tester

För att lägga till nya tester, skapa en ny metod i `CRMSystemUITests.cs` med attributet `[TestMethod]` och följ samma mönster som de befintliga testerna.
