using Dapper;
using System.Data.Common;
using UserSvc.Model;

namespace UserSvc.Persistence;

/// <summary>
/// Repository for user operations
/// </summary>
public class UserRepository : IDisposable
{

    private const string GetAllQuery = "SELECT * FROM user";
    private const string GetByIdQuery = "SELECT * FROM user WHERE id=@Id";
    private const string CreateQuery = """
                                        INSERT INTO user(id, firstName, lastName, email, phone, dateOfBirth)
                                        VALUES (null, @FirstName, @LastName, @Email, @Phone, @DateOfBirth)
                                        RETURNING *
                                        """;
    private const string UpdateQuery = """
                                        UPDATE user SET firstName=@FirstName, lastName=@LastName, email=@Email, phone=@Phone, dateOfBirth=@DateOfBirth
                                        WHERE id=@Id
                                        RETURNING *
                                        """;
    private const string DeleteQuery = "DELETE FROM user WHERE id=@id";


    private readonly DbConnection _connection;

    public UserRepository(DbConnection connection)
    {
        _connection = connection;
        _connection.Open();
    }

    public void InitRepository()
    {
        _connection.Execute("""
            CREATE TABLE IF NOT EXISTS user (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                firstName TEXT(34),
                lastName TEXT,
                email TEXT UNIQUE,
                phone TEXT,
                dateOfBirth TEXT
            )
        """);
    }

    public void ClearRepository() =>
        _connection.Execute("DELETE FROM user");

    public Task<List<UserWithId>> GetAllUsersAsync() =>
        _connection.QueryAsync<UserWithId>(GetAllQuery).ContinueWith(t => t.Result.ToList());

    public Task<UserWithId?> GetUserByIdAsync(string userId) =>
        _connection.QuerySingleOrDefaultAsync<UserWithId>(GetByIdQuery, new { Id = userId });

    public Task<UserWithId> CreateUserAsync(User user) =>
        _connection.QuerySingleAsync<UserWithId>(CreateQuery, user);

    public Task<UserWithId?> UpdateUserAsync(string userId, User user) =>
        _connection.QuerySingleOrDefaultAsync<UserWithId>(
            UpdateQuery, new { Id = userId, user.FirstName, user.LastName, user.DateOfBirth });
    
    public Task DeleteUserAsync(string userId) =>
        _connection.ExecuteAsync(DeleteQuery, new { id = userId });

    public void Dispose() => _connection.Dispose();
}

