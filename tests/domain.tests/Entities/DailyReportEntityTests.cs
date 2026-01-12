using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class DailyReportEntityTests
{
    [Fact]
    public void 正しい値で日報を作成できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15, 10, 30, 0);
        var goal = "C#の基礎を学ぶ";
        var achieved = "変数と制御構文を理解した";
        var struggle = "クラスの概念が難しかった";
        var overcame = "図を描いて整理した";

        // 実行
        var report = new DailyReport(userId, date, goal, achieved, struggle, overcame);

        // 検証
        report.Should().NotBeNull();
        report.Id.Should().NotBeEmpty();
        report.UserId.Should().Be(userId);
        report.Date.Should().Be(date.Date); // 時間部分が切り捨てられる
        report.Goal.Should().Be(goal);
        report.Achieved.Should().Be(achieved);
        report.Struggle.Should().Be(struggle);
        report.Overcame.Should().Be(overcame);
        report.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void 日付の時間部分が切り捨てられる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var dateWithTime = new DateTime(2026, 1, 15, 14, 30, 45);
        var goal = "目標";
        var achieved = "達成";
        var struggle = "苦戦";
        var overcame = "乗り越え";

        // 実行
        var report = new DailyReport(userId, dateWithTime, goal, achieved, struggle, overcame);

        // 検証
        report.Date.Should().Be(new DateTime(2026, 1, 15)); // 時間部分が切り捨て
        report.Date.Hour.Should().Be(0);
        report.Date.Minute.Should().Be(0);
        report.Date.Second.Should().Be(0);
    }

    [Fact]
    public void 目標が空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new DailyReport(userId, date, "", "達成", "苦戦", "乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Goal cannot be empty*");
    }

    [Fact]
    public void 目標がnullの場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new DailyReport(userId, date, null!, "達成", "苦戦", "乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Goal cannot be empty*");
    }

    [Fact]
    public void 達成したことが空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new DailyReport(userId, date, "目標", "", "苦戦", "乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Achieved cannot be empty*");
    }

    [Fact]
    public void 苦戦したことが空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new DailyReport(userId, date, "目標", "達成", "", "乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Struggle cannot be empty*");
    }

    [Fact]
    public void どう乗り越えたかが空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new DailyReport(userId, date, "目標", "達成", "苦戦", "");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Overcame cannot be empty*");
    }

    [Fact]
    public void 空白のみの目標の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new DailyReport(userId, date, "   ", "達成", "苦戦", "乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Goal cannot be empty*");
    }

    [Fact]
    public void 日報を更新できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = new DailyReport(userId, date, "古い目標", "古い達成", "古い苦戦", "古い乗り越え");
        
        var newGoal = "新しい目標";
        var newAchieved = "新しい達成";
        var newStruggle = "新しい苦戦";
        var newOvercame = "新しい乗り越え";

        // 実行
        report.Update(newGoal, newAchieved, newStruggle, newOvercame);

        // 検証
        report.Goal.Should().Be(newGoal);
        report.Achieved.Should().Be(newAchieved);
        report.Struggle.Should().Be(newStruggle);
        report.Overcame.Should().Be(newOvercame);
    }

    [Fact]
    public void 更新時に目標が空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = new DailyReport(userId, date, "目標", "達成", "苦戦", "乗り越え");

        // 実行
        Action act = () => report.Update("", "新達成", "新苦戦", "新乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Goal cannot be empty*");
    }

    [Fact]
    public void 更新時に達成したことが空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = new DailyReport(userId, date, "目標", "達成", "苦戦", "乗り越え");

        // 実行
        Action act = () => report.Update("新目標", "", "新苦戦", "新乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Achieved cannot be empty*");
    }

    [Fact]
    public void 更新時に苦戦したことが空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = new DailyReport(userId, date, "目標", "達成", "苦戦", "乗り越え");

        // 実行
        Action act = () => report.Update("新目標", "新達成", "", "新乗り越え");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Struggle cannot be empty*");
    }

    [Fact]
    public void 更新時にどう乗り越えたかが空の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = new DailyReport(userId, date, "目標", "達成", "苦戦", "乗り越え");

        // 実行
        Action act = () => report.Update("新目標", "新達成", "新苦戦", "");

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Overcame cannot be empty*");
    }

    [Fact]
    public void 論理削除できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var report = new DailyReport(userId, date, "目標", "達成", "苦戦", "乗り越え");
        report.IsDeleted.Should().BeFalse();

        // 実行
        report.SoftDelete();

        // 検証
        report.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void 長い文章を設定できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var longText = new string('あ', 1000);

        // 実行
        var report = new DailyReport(userId, date, longText, longText, longText, longText);

        // 検証
        report.Goal.Should().Be(longText);
        report.Achieved.Should().Be(longText);
        report.Struggle.Should().Be(longText);
        report.Overcame.Should().Be(longText);
    }

    [Fact]
    public void 改行を含む文章を設定できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var date = DateTime.Today;
        var textWithNewlines = "1行目\n2行目\n3行目";

        // 実行
        var report = new DailyReport(userId, date, textWithNewlines, textWithNewlines, textWithNewlines, textWithNewlines);

        // 検証
        report.Goal.Should().Be(textWithNewlines);
        report.Achieved.Should().Be(textWithNewlines);
        report.Struggle.Should().Be(textWithNewlines);
        report.Overcame.Should().Be(textWithNewlines);
    }
}
