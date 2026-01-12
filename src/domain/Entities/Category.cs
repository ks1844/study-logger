namespace Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public bool IsDeleted { get; private set; }
    
    // Navigation properties
    public User? User { get; private set; }
    public ICollection<StudyRecord> StudyRecords { get; private set; } = new List<StudyRecord>();

    // EF Core用のプライベートコンストラクタ
    private Category() { }

    public Category(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        IsDeleted = false;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        
        Name = name;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
