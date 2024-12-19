using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using UserSvc.Model;
using UserSvc.Persistence;

namespace UserSvcIntTests.IntegrationTests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string BaseUrl = "/api/users";
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
        ClearRepository();
    }

    [Fact]
    public async Task Get_AllUsers_ReturnsSuccessAndCorrectContentType()
    {
        var response = await client.GetAsync(BaseUrl);
        var users = await response.Content.ReadFromJsonAsync<List<UserWithId>>();

        response.EnsureSuccessStatusCode();
        Assert.Empty(users!);
        Assert.Equal("application/json", response.Content.Headers.ContentType!.MediaType);
        Assert.Equal("utf-8", response.Content.Headers.ContentType.CharSet);

    }

    [Fact]
    public async Task Post_CreateUser_UniqueEmail()
    {
        var user = new User { FirstName = "John", LastName = "Doe", Email = "test@test.com", Phone = "1231231234", DateOfBirth = new DateOnly(1980, 10, 12) };
        var resp = await client.PostAsJsonAsync(BaseUrl, user);
        resp.EnsureSuccessStatusCode();

        // try creating user with the same email
        resp = await client.PostAsJsonAsync(BaseUrl, user);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        Assert.Contains("'Email' must be unique", await resp.Content.ReadAsStringAsync());
    }

    [Theory]
    [MemberData(nameof(InvalidUsers))]
    public async Task Post_CreateUser_Validation(User user, string failedField)
    {
        var response = await client.PostAsJsonAsync(BaseUrl, user);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(failedField, await response.Content.ReadAsStringAsync());
    }

    public static IEnumerable<object[]> InvalidUsers =>
        new List<object[]>
        {
            new object[] { new User { FirstName = new string('A', 200), LastName = "Doe", DateOfBirth = new DateOnly(1980, 10, 12) }, "FirstName" },
            new object[] { new User { FirstName = "John", LastName = new string('A', 200), DateOfBirth = new DateOnly(1980, 10, 12) }, "LastName" },
            new object[] { new User { FirstName = "John", LastName = "Doe", Email = null, DateOfBirth = new DateOnly(1980, 10, 12) }, "Email" },
            new object[] { new User { FirstName = "John", LastName = "Doe", Email = "wrong email", DateOfBirth = new DateOnly(1980, 10, 12) }, "Email" },
            new object[] { new User { FirstName = "John", LastName = "Doe", Phone = "123", DateOfBirth = new DateOnly(1980, 10, 12) }, "Phone" },
            new object[] { new User { FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2020, 10, 12) }, "18 years" }
        };

    private void ClearRepository()
    {
        using var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<UserRepository>();
        repo.ClearRepository();
    }
}
