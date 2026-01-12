using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class StudyRecordServiceTests
{
    private readonly Mock<IStudyRecordRepository> _mockStudyRecordRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly StudyRecordService _studyRecordService;

    public StudyRecordServiceTests()
    {
        _mockStudyRecordRepository = new Mock<IStudyRecordRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _studyRecordService = new StudyRecordService(
            _mockStudyRecordRepository.Object,
            _mockCategoryRepository.Object);
    }

    [Fact]
    public async Task ユーザーIDで学習記録一覧を取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var record1 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, DateTime.Today, 2.5f, "学習メモ1");
        var record2 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, DateTime.Today.AddDays(-1), 3.0f, "学習メモ2");
        var records = new List<StudyRecord> { record1, record2 };
        var category = CreateCategory(categoryId, userId, "C#");

        _mockStudyRecordRepository
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(records);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // 実行
        var result = await _studyRecordService.GetByUserIdAsync(userId);

        // 検証
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.StudyHour == 2.5f);
        result.Should().Contain(r => r.StudyHour == 3.0f);
        result.All(r => r.CategoryName == "C#").Should().BeTrue();
        _mockStudyRecordRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task 最近の学習記録を指定件数取得できる()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var count = 3;
        var record1 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, DateTime.Today, 2.0f, "今日");
        var record2 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, DateTime.Today.AddDays(-1), 1.5f, "昨日");
        var record3 = CreateStudyRecord(Guid.NewGuid(), userId, categoryId, DateTime.Today.AddDays(-2), 3.0f, "一昨日");
        var records = new List<StudyRecord> { record1, record2, record3 };
        var category = CreateCategory(categoryId, userId, "Python");

        _mockStudyRecordRepository
            .Setup(x => x.GetRecentByUserIdAsync(userId, count))
            .ReturnsAsync(records);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // 実行
        var result = await _studyRecordService.GetRecentAsync(userId, count);

        // 検証
        result.Should().HaveCount(3);
        result.All(r => r.CategoryName == "Python").Should().BeTrue();
        _mockStudyRecordRepository.Verify(x => x.GetRecentByUserIdAsync(userId, count), Times.Once);
    }

    [Fact]
    public async Task 学習記録の作成が成功する()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var date = DateTime.Today;
        var dto = new CreateStudyRecordDto(userId, categoryId, date, 4.5f, "新規学習記録");
        var createdRecord = CreateStudyRecord(recordId, userId, categoryId, date, 4.5f, "新規学習記録");
        var category = CreateCategory(categoryId, userId, "JavaScript");

        _mockStudyRecordRepository
            .Setup(x => x.CreateAsync(It.IsAny<StudyRecord>()))
            .ReturnsAsync(createdRecord);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // 実行
        var result = await _studyRecordService.CreateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result.Id.Should().Be(recordId);
        result.UserId.Should().Be(userId);
        result.CategoryId.Should().Be(categoryId);
        result.CategoryName.Should().Be("JavaScript");
        result.StudyHour.Should().Be(4.5f);
        result.Memo.Should().Be("新規学習記録");
        _mockStudyRecordRepository.Verify(x => x.CreateAsync(It.IsAny<StudyRecord>()), Times.Once);
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task カテゴリが存在しない場合空文字列が返る()
    {
        // 準備
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var date = DateTime.Today;
        var dto = new CreateStudyRecordDto(userId, categoryId, date, 2.0f, "テスト");
        var createdRecord = CreateStudyRecord(recordId, userId, categoryId, date, 2.0f, "テスト");

        _mockStudyRecordRepository
            .Setup(x => x.CreateAsync(It.IsAny<StudyRecord>()))
            .ReturnsAsync(createdRecord);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // 実行
        var result = await _studyRecordService.CreateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result.CategoryName.Should().Be("");
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task 学習記録の更新が成功する()
    {
        // 準備
        var recordId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var oldCategoryId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var existingRecord = CreateStudyRecord(recordId, userId, oldCategoryId, DateTime.Today, 1.0f, "古いメモ");
        var dto = new UpdateStudyRecordDto(recordId, newCategoryId, DateTime.Today, 5.0f, "更新されたメモ");
        var newCategory = CreateCategory(newCategoryId, userId, "新しいカテゴリ");

        _mockStudyRecordRepository
            .Setup(x => x.GetByIdAsync(recordId))
            .ReturnsAsync(existingRecord);

        _mockStudyRecordRepository
            .Setup(x => x.UpdateAsync(It.IsAny<StudyRecord>()))
            .Returns(Task.CompletedTask);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(newCategoryId))
            .ReturnsAsync(newCategory);

        // 実行
        var result = await _studyRecordService.UpdateAsync(dto);

        // 検証
        result.Should().NotBeNull();
        result!.Id.Should().Be(recordId);
        result.CategoryId.Should().Be(newCategoryId);
        result.CategoryName.Should().Be("新しいカテゴリ");
        result.StudyHour.Should().Be(5.0f);
        result.Memo.Should().Be("更新されたメモ");
        _mockStudyRecordRepository.Verify(x => x.GetByIdAsync(recordId), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<StudyRecord>()), Times.Once);
        _mockCategoryRepository.Verify(x => x.GetByIdAsync(newCategoryId), Times.Once);
    }

    [Fact]
    public async Task 存在しない学習記録の更新はnullを返す()
    {
        // 準備
        var recordId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dto = new UpdateStudyRecordDto(recordId, categoryId, DateTime.Today, 3.0f, "メモ");

        _mockStudyRecordRepository
            .Setup(x => x.GetByIdAsync(recordId))
            .ReturnsAsync((StudyRecord?)null);

        // 実行
        var result = await _studyRecordService.UpdateAsync(dto);

        // 検証
        result.Should().BeNull();
        _mockStudyRecordRepository.Verify(x => x.GetByIdAsync(recordId), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<StudyRecord>()), Times.Never);
    }

    [Fact]
    public async Task 削除済み学習記録の更新はnullを返す()
    {
        // 準備
        var recordId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var deletedRecord = CreateStudyRecord(recordId, userId, categoryId, DateTime.Today, 2.0f, "削除済み");
        deletedRecord.SoftDelete();
        var dto = new UpdateStudyRecordDto(recordId, categoryId, DateTime.Today, 3.0f, "更新");

        _mockStudyRecordRepository
            .Setup(x => x.GetByIdAsync(recordId))
            .ReturnsAsync(deletedRecord);

        // 実行
        var result = await _studyRecordService.UpdateAsync(dto);

        // 検証
        result.Should().BeNull();
        _mockStudyRecordRepository.Verify(x => x.GetByIdAsync(recordId), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<StudyRecord>()), Times.Never);
    }

    [Fact]
    public async Task 学習記録の削除が成功する()
    {
        // 準備
        var recordId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var record = CreateStudyRecord(recordId, userId, categoryId, DateTime.Today, 2.5f, "削除対象");

        _mockStudyRecordRepository
            .Setup(x => x.GetByIdAsync(recordId))
            .ReturnsAsync(record);

        _mockStudyRecordRepository
            .Setup(x => x.UpdateAsync(It.IsAny<StudyRecord>()))
            .Returns(Task.CompletedTask);

        // 実行
        var result = await _studyRecordService.DeleteAsync(recordId);

        // 検証
        result.Should().BeTrue();
        record.IsDeleted.Should().BeTrue();
        _mockStudyRecordRepository.Verify(x => x.GetByIdAsync(recordId), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<StudyRecord>()), Times.Once);
    }

    [Fact]
    public async Task 存在しない学習記録の削除はfalseを返す()
    {
        // 準備
        var recordId = Guid.NewGuid();

        _mockStudyRecordRepository
            .Setup(x => x.GetByIdAsync(recordId))
            .ReturnsAsync((StudyRecord?)null);

        // 実行
        var result = await _studyRecordService.DeleteAsync(recordId);

        // 検証
        result.Should().BeFalse();
        _mockStudyRecordRepository.Verify(x => x.GetByIdAsync(recordId), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<StudyRecord>()), Times.Never);
    }

    [Fact]
    public async Task 既に削除済みの学習記録の削除はfalseを返す()
    {
        // 準備
        var recordId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var deletedRecord = CreateStudyRecord(recordId, userId, categoryId, DateTime.Today, 1.0f, "既に削除済み");
        deletedRecord.SoftDelete();

        _mockStudyRecordRepository
            .Setup(x => x.GetByIdAsync(recordId))
            .ReturnsAsync(deletedRecord);

        // 実行
        var result = await _studyRecordService.DeleteAsync(recordId);

        // 検証
        result.Should().BeFalse();
        _mockStudyRecordRepository.Verify(x => x.GetByIdAsync(recordId), Times.Once);
        _mockStudyRecordRepository.Verify(x => x.UpdateAsync(It.IsAny<StudyRecord>()), Times.Never);
    }

    /// <summary>
    /// テスト用の学習記録を作成
    /// </summary>
    private StudyRecord CreateStudyRecord(Guid id, Guid userId, Guid categoryId, DateTime date, float studyHour, string? memo)
    {
        var record = new StudyRecord(userId, categoryId, date, studyHour, memo);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(StudyRecord).GetProperty("Id");
        idProperty?.SetValue(record, id);

        return record;
    }

    /// <summary>
    /// テスト用のカテゴリを作成
    /// </summary>
    private Category CreateCategory(Guid id, Guid userId, string name)
    {
        var category = new Category(userId, name);
        
        // リフレクションを使ってプライベートプロパティを設定
        var idProperty = typeof(Category).GetProperty("Id");
        idProperty?.SetValue(category, id);

        return category;
    }
}
