using FellowOakDicom;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Data;
using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;
using TraickMiniDicom.Extensions;

namespace TraickMiniDicom.Services;

public class StudyService : IStudyService
{
    private readonly AppDbContext _context;

    // Dependency Injection
    public StudyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Study>> UploadDicomAsync(IFormFile file, Guid userId)
    {
        // file control
        if (file == null || file.Length == 0)
        {
            return new ApiResponse<Study>
            {
                Success = false,
                Message = "Geçersiz dosya. Lütfen geçerli bir DICOM dosyası yükleyin."
            };
        }

        // reading file
        using var stream = file.OpenReadStream();
        
        // open file with fo-dicom
        var dicomFile = await DicomFile.OpenAsync(stream);
        var dataset = dicomFile.Dataset;
        
        // extracting dicom tags
        string patientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Bilinmeyen Hasta");
        string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Bilinmeyen Çalışma");
        string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality,"Bilinmeyen Modality");
        
        // converting series to string
        string series = dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, 0).ToString();
        
        // extracting row pixels and column pixels and merging
        int rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, 0);
        int columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, 0);
        string resolution = $"{rows}x{columns}";

        // creating database model
        var record = new Study
        {
            PatientName = patientName,
            StudyInstanceUID = studyInstanceUID,
            Modality = modality,
            Series = series,
            Resolution = resolution,
            UserId = userId // Hangi kullanıcı yükledi ekliyoruz
        };
        
        // save to database
        _context.Studies.Add(record);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<Study>
        {
            Success = true,
            Message = "DICOM dosyası başarıyla yüklendi ve veritabanına kaydedildi.",
            Data = record
        };
    }

    public async Task<ApiResponse<PagedListResponse<Study>>> GetAllStudiesAsync(int page, int limit, string sort, string sortDir, Guid userId)
    {
        // Kişinin kendine ait verileri Listeleme (Data Isolation)
        var query = _context.Studies.Where(x => x.UserId == userId).AsQueryable();
        
        var pagedStudies = await query.ToPagedListAsync(page, limit, sort, sortDir);
        
        return new ApiResponse<PagedListResponse<Study>>
        {
            Success = true,
            Message = "Dicom çalışmaları başarıyla getirildi.",
            Data = pagedStudies
        };
    }
}