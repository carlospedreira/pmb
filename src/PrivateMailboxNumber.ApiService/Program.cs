using System.ComponentModel.DataAnnotations;
using PrivateMailboxNumber.ApiService.Repositories;
using PrivateMailboxNumber.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IMailboxRepository, InMemoryMailboxRepository>();
builder.Services.AddScoped<IMailboxService, MailboxService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", (string? locationNumber, string? accountId, int? mailboxNumber, IMailboxService mailboxService) => {
    if (locationNumber == null)
    {
        return Results.BadRequest("Location number is required");
    }

    if (accountId == null && mailboxNumber == null)
    {
        return Results.BadRequest("Either account ID or mailbox number is required");
    }

    if (accountId != null && mailboxNumber != null)
    {
        return Results.BadRequest("Please provide either account ID or mailbox number, not both");
    }

    try
    {
        if (accountId != null)
        {
            var number = mailboxService.GetMailboxNumber(locationNumber, accountId);
            return Results.Ok(new { mailboxNumber = number });
        }
        else
        {
            var account = mailboxService.GetAccountId(locationNumber, mailboxNumber!.Value);
            return Results.Ok(new { accountId = account });
        }
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}).WithDisplayName("Lookup Mailbox Number");

app.MapDefaultEndpoints();

app.Run();
