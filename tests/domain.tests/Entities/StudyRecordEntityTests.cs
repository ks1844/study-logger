using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class StudyRecordEntityTests
{
    [Fact]
    public void 正しい値で学習記録を作成できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 15, 10, 30, 0);
        var studyHour = 3.5f;
        var memo = "C#の学習をしました";

        // 実行
        var record = new StudyRecord(userId, categoryId, date, studyHour, memo);

        // 検証
        record.Should().NotBeNull();
        record.Id.Should().NotBeEmpty();
        record.UserId.Should().Be(userId);
        record.CategoryId.Should().Be(categoryId);
        record.Date.Should().Be(date.Date); // 時間部分が切り捨てられる
        record.StudyHour.Should().Be(studyHour);
        record.Memo.Should().Be(memo);
        record.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void メモなしで学習記録を作成できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;
        var studyHour = 2.0f;

        // 実行
        var record = new StudyRecord(userId, categoryId, date, studyHour);

        // 検証
        record.Should().NotBeNull();
        record.Memo.Should().BeNull();
    }

    [Fact]
    public void 日付の時間部分が切り捨てられる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dateWithTime = new DateTime(2026, 1, 15, 14, 30, 45);
        var studyHour = 2.0f;

        // 実行
        var record = new StudyRecord(userId, categoryId, dateWithTime, studyHour);

        // 検証
        record.Date.Should().Be(new DateTime(2026, 1, 15)); // 時間部分が切り捨て
        record.Date.Hour.Should().Be(0);
        record.Date.Minute.Should().Be(0);
        record.Date.Second.Should().Be(0);
    }

    [Fact]
    public void 学習時間が0の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new StudyRecord(userId, categoryId, date, 0f);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Study hour must be greater than 0*");
    }

    [Fact]
    public void 学習時間が負の値の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new StudyRecord(userId, categoryId, date, -1.5f);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Study hour must be greater than 0*");
    }

    [Fact]
    public void 学習時間が24時間を超える場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        Action act = () => new StudyRecord(userId, categoryId, date, 24.1f);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Study hour cannot exceed 24 hours*");
    }

    [Fact]
    public void 学習時間がちょうど24時間の場合は作成できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;

        // 実行
        var record = new StudyRecord(userId, categoryId, date, 24f);

        // 検証
        record.StudyHour.Should().Be(24f);
    }

    [Fact]
    public void 小数の学習時間を設定できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;
        var studyHour = 2.5f;

        // 実行
        var record = new StudyRecord(userId, categoryId, date, studyHour);

        // 検証
        record.StudyHour.Should().Be(studyHour);
    }

    [Fact]
    public void 学習記録を更新できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var oldCategoryId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var record = new StudyRecord(userId, oldCategoryId, DateTime.Today, 2.0f, "古いメモ");
        
        var newDate = DateTime.Today.AddDays(1);
        var newStudyHour = 5.0f;
        var newMemo = "新しいメモ";

        // 実行
        record.Update(newCategoryId, newDate, newStudyHour, newMemo);

        // 検証
        record.CategoryId.Should().Be(newCategoryId);
        record.Date.Should().Be(newDate.Date);
        record.StudyHour.Should().Be(newStudyHour);
        record.Memo.Should().Be(newMemo);
    }

    [Fact]
    public void 更新時に学習時間が0の場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var record = new StudyRecord(userId, categoryId, DateTime.Today, 2.0f);

        // 実行
        Action act = () => record.Update(categoryId, DateTime.Today, 0f, null);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Study hour must be greater than 0*");
    }

    [Fact]
    public void 更新時に学習時間が24時間を超える場合は例外をスローする()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var record = new StudyRecord(userId, categoryId, DateTime.Today, 2.0f);

        // 実行
        Action act = () => record.Update(categoryId, DateTime.Today, 25f, null);

        // 検証
        act.Should().Throw<ArgumentException>()
            .WithMessage("Study hour cannot exceed 24 hours*");
    }

    [Fact]
    public void 更新時にメモをnullにできる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var record = new StudyRecord(userId, categoryId, DateTime.Today, 2.0f, "元のメモ");

        // 実行
        record.Update(categoryId, DateTime.Today, 3.0f, null);

        // 検証
        record.Memo.Should().BeNull();
    }

    [Fact]
    public void 論理削除できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var record = new StudyRecord(userId, categoryId, DateTime.Today, 2.0f);
        record.IsDeleted.Should().BeFalse();

        // 実行
        record.SoftDelete();

        // 検証
        record.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void 長いメモを設定できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = DateTime.Today;
        var studyHour = 3.0f;
        var longMemo = new string('あ', 1000);

        // 実行
        var record = new StudyRecord(userId, categoryId, date, studyHour, longMemo);

        // 検証
        record.Memo.Should().Be(longMemo);
    }
}
