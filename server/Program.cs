using Npgsql;
using server;
using server.api;
using server.Config;
using server.Services;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Skapa Database-instansen med IConfiguration
Database database = new Database(builder.Configuration);
NpgsqlDataSource db = database.Connection();

var emailSettings = builder.Configuration.GetSection("Email").Get<EmailSettings>();
if (emailSettings != null)
{
    builder.Services.AddSingleton(emailSettings);
}
else
{
    throw new InvalidOperationException("Email settings are not configured properly.");
}
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure static files
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles();

app.UseSession();

String url = "/api";

new ServerStatus(app, db, url);
new Login(app, db, url);
new Users(app, db, url);
new Issues(app, db, url);
new Forms(app, db, url);
new Companies(app, db, url);

// Handle SPA routing
app.MapFallbackToFile("index.html");

await app.RunAsync();
