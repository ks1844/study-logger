using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class StudyRecordService
{
    private readonly IStudyRecordRepository _studyRecordRepository;
    private readonly ICategoryRepository _categoryRepository;

    public StudyRecordService(
        IStudyRecordRepository studyRecordRepository,
        ICategoryRepository categoryRepository)
    {
        _studyRecordRepository = studyRecordRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<StudyRecordDto>> GetByUserIdAsync(Guid userId)
    {
        var records = await _studyRecordRepository.GetByUserIdAsync(userId);
        return await MapToDto(records);
    }

    public async Task<IEnumerable<StudyRecordDto>> GetRecentAsync(Guid userId, int count)
    {
        var records = await _studyRecordRepository.GetRecentByUserIdAsync(userId, count);
        return await MapToDto(records);
    }

    public async Task<StudyRecordDto> CreateAsync(CreateStudyRecordDto dto)
    {
        var record = new StudyRecord(dto.UserId, dto.CategoryId, dto.Date, dto.StudyHour, dto.Memo);
        var created = await _studyRecordRepository.CreateAsync(record);
        
        var category = await _categoryRepository.GetByIdAsync(created.CategoryId);
        return new StudyRecordDto(
            created.Id,
            created.UserId,
            created.CategoryId,
            category?.Name ?? "",
            created.Date,
            created.StudyHour,
            created.Memo
        );
    }

    public async Task<StudyRecordDto?> UpdateAsync(UpdateStudyRecordDto dto)
    {
        var record = await _studyRecordRepository.GetByIdAsync(dto.Id);
        if (record == null || record.IsDeleted)
            return null;

        record.Update(dto.CategoryId, dto.Date, dto.StudyHour, dto.Memo);
        await _studyRecordRepository.UpdateAsync(record);

        var category = await _categoryRepository.GetByIdAsync(record.CategoryId);
        return new StudyRecordDto(
            record.Id,
            record.UserId,
            record.CategoryId,
            category?.Name ?? "",
            record.Date,
            record.StudyHour,
            record.Memo
        );
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var record = await _studyRecordRepository.GetByIdAsync(id);
        if (record == null || record.IsDeleted)
            return false;

        record.SoftDelete();
        await _studyRecordRepository.UpdateAsync(record);
        return true;
    }

    private async Task<IEnumerable<StudyRecordDto>> MapToDto(IEnumerable<StudyRecord> records)
    {
        var result = new List<StudyRecordDto>();
        foreach (var record in records)
        {
            var category = await _categoryRepository.GetByIdAsync(record.CategoryId);
            result.Add(new StudyRecordDto(
                record.Id,
                record.UserId,
                record.CategoryId,
                category?.Name ?? "",
                record.Date,
                record.StudyHour,
                record.Memo
            ));
        }
        return result;
    }
}
