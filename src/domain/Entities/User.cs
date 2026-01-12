namespace Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; } // "student" or "admin"
    public Guid CompanyId { get; private set; }
    public bool IsDeleted { get; private set; }
    
    // Navigation properties
    public Company? Company { get; private set; }
    public ICollection<Category> Categories { get; private set; } = new List<Category>();
    public ICollection<StudyRecord> StudyRecords { get; private set; } = new List<StudyRecord>();
    public ICollection<DailyReport> DailyReports { get; private set; } = new List<DailyReport>();

    // EF Core用のプライベートコンストラクタ
    private User() { }

    public User(string name, string email, string passwordHash, string role, Guid companyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        if (role != "student" && role != "admin")
            throw new ArgumentException("Role must be 'student' or 'admin'", nameof(role));
        
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CompanyId = companyId;
        IsDeleted = false;
    }

    public void UpdateProfile(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        Name = name;
        Email = email;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
        
        PasswordHash = newPasswordHash;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public bool IsStudent() => Role == "student";
    public bool IsAdmin() => Role == "admin";
}
