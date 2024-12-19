using System.ComponentModel.DataAnnotations;

namespace UserSvc.Model;

/// <summary>
/// User data for cases where id is unknown or not required, e.g. POST body
/// </summary>
public class User
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Age => DateOfBirth is null ? null : GetAge(DateOfBirth.Value);

    public static int GetAge(DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var bdThisYear = new DateOnly(DateTime.Today.Year, dob.Month, dob.Day);
        var hadBdThisYear = bdThisYear <= today;
        var correction = hadBdThisYear ? 0 : 1;
        return DateTime.Today.Year - dob.Year - correction;
    }
}

/// <summary>
/// User data for cases where id is required, e.g. GET all users
/// </summary>
public class UserWithId : User
{
    public int Id { get; set; }
}