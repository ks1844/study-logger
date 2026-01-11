namespace Domain.Entities;

public class StudyRecord
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTime Date { get; private set; }
    public float StudyHour { get; private set; }
    public string? Memo { get; private set; }
    public bool IsDeleted { get; private set; }
    
    // Navigation properties
    public User? User { get; private set; }
    public Category? Category { get; private set; }

    // EF Core用のプライベートコンストラクタ
    private StudyRecord() { }

    public StudyRecord(Guid userId, Guid categoryId, DateTime date, float studyHour, string? memo = null)
    {
        if (studyHour <= 0)
            throw new ArgumentException("Study hour must be greater than 0", nameof(studyHour));
        if (studyHour > 24)
            throw new ArgumentException("Study hour cannot exceed 24 hours", nameof(studyHour));
        
        Id = Guid.NewGuid();
        UserId = userId;
        CategoryId = categoryId;
        Date = date.Date; // 時間部分を切り捨て
        StudyHour = studyHour;
        Memo = memo;
        IsDeleted = false;
    }

    public void Update(Guid categoryId, DateTime date, float studyHour, string? memo)
    {
        if (studyHour <= 0)
            throw new ArgumentException("Study hour must be greater than 0", nameof(studyHour));
        if (studyHour > 24)
            throw new ArgumentException("Study hour cannot exceed 24 hours", nameof(studyHour));
        
        CategoryId = categoryId;
        Date = date.Date;
        StudyHour = studyHour;
        Memo = memo;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
