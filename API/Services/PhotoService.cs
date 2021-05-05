using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IOptions<CloudinarySettings> _config;
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            this._config = config;
            var account = new Account
            {
                Cloud = _config.Value.CloudName,
                ApiKey = _config.Value.AppKey,
                ApiSecret = _config.Value.AppSecret
            };

            _cloudinary = new Cloudinary(account);
        }
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deletionParms = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParms);
            return result;
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var imageParms = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(imageParms);
            }
            return uploadResult;
        }
    }
}