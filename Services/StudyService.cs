using System.IO;
using FellowOakDicom;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Data;
using TraickMiniDicom.DTOs;
using TraickMiniDicom.Extensions;
using TraickMiniDicom.Models;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Services;

public class StudyService : IStudyService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUser _currentUser;

    // Dependency Injection
    public StudyService(AppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ServiceResult<StudyResponseDto>> UploadDicomAsync(IFormFile file, Guid userId)
    {
        // file control
        if (file == null || file.Length == 0)
        {
            return ServiceResult<StudyResponseDto>.Failure("Geçersiz dosya. Lütfen geçerli bir DICOM dosyası yükleyin.");
        }

        // reading file
        using var stream = file.OpenReadStream();

        // open file with fo-dicom
        var dicomFile = await DicomFile.OpenAsync(stream);
        var dataset = dicomFile.Dataset;
        var parsedStudy = ExtractMetadataFromDicom(dataset, userId);

        var existingStudy = await _context.Studies
            .Include(s => s.DicomFiles)
            .FirstOrDefaultAsync(s => s.StudyInstanceUID == parsedStudy.StudyInstanceUID && s.UserId == userId);

        Study targetStudy;

        // 2. Study Objesini Belirleme 
        if (existingStudy == null)
        {
            targetStudy = parsedStudy;

            _context.Studies.Add(targetStudy);
        }
        else
        {
            // eğer çalışma zaten varsa, sadece bilgileri güncelleye
            targetStudy = existingStudy;
        }

        // 3. dosyayı fiziksel olarak kaydetme
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "DicomImages");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // dosya adını benzersiz yapma
        var fileName = $"{Guid.NewGuid()}.dcm";
        var filePath = Path.Combine(uploadsFolder, fileName);


        // okunan stream'i başa sararak kaydetme
        stream.Position = 0;
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream);
        }


        // 4. StudyFile Objesi Oluşturma ve İlişkilendirme
        var studyFile = new StudyFile
        {
            FilePath = filePath,
            Study = targetStudy
        };

        if (targetStudy.DicomFiles == null)
            targetStudy.DicomFiles = new List<StudyFile>();

        targetStudy.DicomFiles.Add(studyFile);
        _context.StudyFiles.Add(studyFile);

        // 5. Tüm Değişiklikleri Database'e Yansıtma
        await _context.SaveChangesAsync();

        // Entity olan targetStudy'i DTO'ya (Taşıyıcıya) çeviriyoruz:
        var resultDto = targetStudy.Adapt<StudyResponseDto>();

        return ServiceResult<StudyResponseDto>.IsSuccess(resultDto);
    }

    public async Task<ServiceResult<PagedListResponse<StudyResponseDto>>> GetAllStudiesAsync(int page, int limit, string sort, string sortDir, Guid userId)
    {
        // Kişinin kendine ait verileri Listeleme ve Entity'leri DTO'ya dönüştürme:
        var query = _context.Studies
            .Include(x => x.DicomFiles)
            .Where(x => x.UserId == userId)
            .ProjectToType<StudyResponseDto>();

        var pagedStudies = await query.ToPagedListAsync(page, limit, sort, sortDir);

        return ServiceResult<PagedListResponse<StudyResponseDto>>.IsSuccess(pagedStudies);
    }

    private Study ExtractMetadataFromDicom(DicomDataset dataset, Guid userId)
    {
        string patientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Bilinmeyen Hasta");
        string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Bilinmeyen Çalışma");
        string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Bilinmeyen Modality");
        string series = dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, 0).ToString();

        int rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, 0);
        int columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, 0);
        string resolution = $"{rows}x{columns}";

        return new Study
        {
            PatientName = patientName,
            StudyInstanceUID = studyInstanceUID,
            Modality = modality,
            Series = series,
            Resolution = resolution,
            UserId = userId,
            OrganizationId = _currentUser.OrganizationId.Value
        };
    }
}