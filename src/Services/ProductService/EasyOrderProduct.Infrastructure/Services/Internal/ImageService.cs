﻿
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Options;
using EasyOrderProduct.Application.Contracts.Interfaces.InternalServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EasyOrderProduct.Infrastructure.Services.Internal
{
    public class ImageService : IImageService
    {
        private readonly string _folderPath;
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long _fileSizeLimit = 5 * 1024 * 1024; // 5 MB

        public ImageService(IOptions<UploadSettings> opts)
        {
            _folderPath = opts.Value.ImageFolder;
            if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
        }
        public async Task<BaseApiResponse> UploadAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return ErrorResponse.BadRequest("No file provided.");

            if (file.Length > _fileSizeLimit)
                return ErrorResponse.BadRequest("File size exceeds limit (5MB).");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
                return ErrorResponse.BadRequest("Invalid file type.");

            try
            {
                var safeName = Path.GetRandomFileName() + ext;
                var filePath = Path.Combine(_folderPath, safeName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var relativeUrl = $"/images/{safeName}";
                var data = new ImageUploadResponse { FileName = safeName, Url = relativeUrl };
                return new SuccessResponse<ImageUploadResponse>("Image uploaded successfully.", data);
            }
            catch (Exception ex)
            {
                return ErrorResponse.InternalServerError(details: ex.Message);
            }
        }
    }
}
