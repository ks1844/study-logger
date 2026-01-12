namespace Domain.Entities;

public class DailyReport
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }
    public string Goal { get; private set; }
    public string Achieved { get; private set; }
    public string Struggle { get; private set; }
    public string Overcame { get; private set; }
    public bool IsDeleted { get; private set; }
    
    // Navigation property
    public User? User { get; private set; }

    // EF Core用のプライベートコンストラクタ
    private DailyReport() { }

    public DailyReport(Guid userId, DateTime date, string goal, string achieved, string struggle, string overcame)
    {
        if (string.IsNullOrWhiteSpace(goal))
            throw new ArgumentException("Goal cannot be empty", nameof(goal));
        if (string.IsNullOrWhiteSpace(achieved))
            throw new ArgumentException("Achieved cannot be empty", nameof(achieved));
        if (string.IsNullOrWhiteSpace(struggle))
            throw new ArgumentException("Struggle cannot be empty", nameof(struggle));
        if (string.IsNullOrWhiteSpace(overcame))
            throw new ArgumentException("Overcame cannot be empty", nameof(overcame));
        
        Id = Guid.NewGuid();
        UserId = userId;
        Date = date.Date;
        Goal = goal;
        Achieved = achieved;
        Struggle = struggle;
        Overcame = overcame;
        IsDeleted = false;
    }

    public void Update(string goal, string achieved, string struggle, string overcame)
    {
        if (string.IsNullOrWhiteSpace(goal))
            throw new ArgumentException("Goal cannot be empty", nameof(goal));
        if (string.IsNullOrWhiteSpace(achieved))
            throw new ArgumentException("Achieved cannot be empty", nameof(achieved));
        if (string.IsNullOrWhiteSpace(struggle))
            throw new ArgumentException("Struggle cannot be empty", nameof(struggle));
        if (string.IsNullOrWhiteSpace(overcame))
            throw new ArgumentException("Overcame cannot be empty", nameof(overcame));
        
        Goal = goal;
        Achieved = achieved;
        Struggle = struggle;
        Overcame = overcame;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
