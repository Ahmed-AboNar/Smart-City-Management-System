using SmartCity.Domain.Common;

namespace SmartCity.Domain.Entities;

/// <summary>
/// Represents a User in the system. Can be a Citizen, Employee, or Admin.
/// Implements IAggregateRoot to indicate it's a root entity in the Domain.
/// </summary>
public class User : BaseEntity, IAggregateRoot
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string NationalId { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public List<string> Roles { get; private set; } = new();

    // For Employees/Managers
    public string? DepartmentId { get; private set; }

    private User() { } // For serialization

    public User(string fullName, string email, string passwordHash, string nationalId, string phoneNumber)
    {
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        NationalId = nationalId;
        PhoneNumber = phoneNumber;
        Roles = new List<string>();
    }

    public void AssignRole(string role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
        }
    }

    public void SetDepartment(string departmentId)
    {
        DepartmentId = departmentId;
    }

    public void UpdateProfile(string fullName, string phoneNumber)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        LastModifiedAt = DateTime.UtcNow;
    }
}
