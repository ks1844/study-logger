namespace Domain.Entities;

public class PrSummary
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    
    // Navigation property
    public User? User { get; private set; }

    // EF Core用のプライベートコンストラクタ
    private PrSummary() { }

    public PrSummary(Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        
        Id = Guid.NewGuid();
        UserId = userId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        
        Content = content;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
