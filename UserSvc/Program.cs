using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Data.Common;
using UserSvc.Model;
using UserSvc.Persistence;

// Initialize services
var builder = WebApplication.CreateBuilder(args);
using var conn = builder.Services.AddDatabaseServices();

// Add validation
builder.Services
    .AddScoped<IValidator<User>, UserValidator>()
    .AddFluentValidationAutoValidation();

var app = builder.Build();

// Add request handlers
const string baseUrl = "/api/users";
var usersApi = app.MapGroup(baseUrl)
    .AddFluentValidationAutoValidation();

usersApi.MapGet("", (UserRepository repo) => repo.GetAllUsersAsync()); // no pagination but it's relatively simple to add

usersApi.MapGet("/{id}", async (string id, UserRepository repo) =>
    await repo.GetUserByIdAsync(id) is UserWithId user ? Results.Ok(user) : Results.NotFound());

usersApi.MapPost("", async (User user, UserRepository repo) =>
{
    try
    {
        var res = await repo.CreateUserAsync(user);
        return Results.Created($"{baseUrl}/{res.Id}", res);
    }
    catch (DbException ex) when (ex.Message.Contains("UNIQUE"))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]> { { "Email", ["'Email' must be unique"] } });
    }
});

usersApi.MapPut("/{id}", async (string id, User user, UserRepository repo) =>
    await repo.UpdateUserAsync(id, user) is UserWithId updatedUser ? Results.Ok(updatedUser) : Results.NotFound());

usersApi.MapDelete("/{id}", (string id, UserRepository repo) =>
    repo.DeleteUserAsync(id).ContinueWith(_ => Results.NoContent()));

// Start the service
app.Run();

// Make class visible for tests
public partial class Program { }