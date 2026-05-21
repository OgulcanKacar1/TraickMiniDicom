using FellowOakDicom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Data;
using TraickMiniDicom.Models;
using TraickMiniDicom.Extensions;
using TraickMiniDicom.Responses;

namespace TraickMiniDicom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DicomController: ControllerBase
{
    private readonly AppDbContext _context;
    
    public DicomController(AppDbContext context)
    {
        _context = context;
    }
    
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadDicom(IFormFile file)
    {
        //file control
        if (file == null || file.Length == 0)
            return BadRequest("Unvalid file. Please upload a valid DICOM file.");

        //reading file
        using var stream = file.OpenReadStream();
        
        //open file with fo-dicom
        var dicomFile = await DicomFile.OpenAsync(stream);
        var dataset = dicomFile.Dataset;
        
        //extracting dicom tags
        string patientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Bilinmeyen Hasta");
        string studyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Bilinmeyen Çalışma");
        string modality = dataset.GetSingleValueOrDefault(DicomTag.Modality,"Bilinmeyen Modality");
        
        //converting series to string
        string series = dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, 0).ToString();
        
        // extracting row pixels and column pixels and merging
        int rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, 0);
        int columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, 0);
        string resolution = $"{rows}x{columns}";

        
        //creating database model
        var record = new Study
        {
            PatientName = patientName,
            StudyInstanceUID = studyInstanceUID,
            Modality = modality,
            Series = series,
            Resolution = resolution,
        };
        
        //save to database
        _context.Studies.Add(record);
        await _context.SaveChangesAsync();
        
        return Ok(new {message = "The DICOM file has been successfully uploaded and saved to the database.", data = record});
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllStudies(
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10,
        [FromQuery] string sort = "CreatedAt",
        [FromQuery] string sortDir = "desc")
    {
        var query = _context.Studies.AsQueryable();
        
        var pagedStudies =await query.ToPagedListAsync(page, limit, sort, sortDir);
        
        var response = new ApiResponse<PagedListResponse<Study>>
        {
            Success = true,
            Message = "Dicom çalışmaları başarıyla getirildi.",
            Data = pagedStudies
            
        };
        
        return Ok(response);
    }
}