namespace Domain.Entities;

public class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsDeleted { get; private set; }
    
    // Navigation property
    public ICollection<User> Users { get; private set; } = new List<User>();

    // EF Core用のプライベートコンストラクタ
    private Company() { }

    public Company(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));
        
        Id = Guid.NewGuid();
        Name = name;
        IsDeleted = false;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));
        
        Name = name;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
