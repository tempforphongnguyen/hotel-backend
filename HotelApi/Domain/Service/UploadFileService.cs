using B2Net.Models;
using B2Net;
using Domain.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.Enum;

namespace Domain.Service
{
    public class UploadFileService : IUploadFileService
    {
        

        private IHostingEnvironment _hostingEnvironment;

        //CloudFlare
        private readonly string contentTypeImage = "image/png";
        private readonly string accountId = "01ee10df544eb01808bd488f1ee937d1";
        private readonly string accessKey = "b993843b4f8564ba6775e042c916f286";
        private readonly string accessSecret = "837f2464365688a7283724f8b797039149b021e154dc5c9e53da7d7782387af3";

        //B2
        private readonly string bucketId = "60fe180029535dff88cf0217";
        private readonly string keyId = "0050e8093df8f270000000002";
        private readonly string applicationKey = "K005PhnV1ne9k6NiYjN4r+oWNjjLfYU";
        private readonly string bucketName = "storage-hotel";


        public UploadFileService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> UploadFileAvatar(IFormFile file)
        {
            if (file == null)
            {
                return "nan.png";
            }
           
            return await UploadImageToCloudFlareAsync(file);
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            return await UploadImageToCloudFlareAsync(file);
        }


        private async Task<string> UploadImageToCloudFlareAsync(IFormFile file)
        {

            var s3Client = new AmazonS3Client(
                            accessKey,
                            accessSecret,
                            new AmazonS3Config
                            {
                                ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com"
                            });

            var newFileName = CreateFullFileName(Path.GetExtension(file.FileName));

            var request = new PutObjectRequest
            {
                BucketName = "hotel-api",
                Key = newFileName,
                InputStream = file.OpenReadStream(),
                ContentType = contentTypeImage,
                DisablePayloadSigning = true
            };

            var response = await s3Client.PutObjectAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception("Upload to Cloudflare R2 failed");
            }

            return newFileName;
        }

        private string CreateFullFileName(string extensionFileName)
        {
            return Guid.NewGuid().ToString() + extensionFileName;
        }
        
        private async Task<string> UploadImageBucket(IFormFile file)
        {

            var options = new B2Options()
            {
                KeyId = keyId,
                ApplicationKey = applicationKey,
                BucketId = bucketId,
                PersistBucket = false,
            };
            var b2Client = new B2Client(options, authorizeOnInitialize: true);

            var contentTypeImage = "image/png";

            var uploadUrl = await b2Client.Files.GetUploadUrl(bucketId);

            var newFileName = CreateFullFileName(Path.GetExtension(file.FileName));

            B2File? fileResult = new B2File();
            using (var stream = file.OpenReadStream())
            {
                fileResult = await b2Client.Files.Upload(stream, newFileName, uploadUrl, contentTypeImage, true, bucketId, dontSHA: true);
            }

            return fileResult.FileName;
        }

        private async Task<string> UploadToImagesFolder(IFormFile file)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "images");

            var fileNameGuid = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, fileNameGuid), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            return fileNameGuid;
        }

    }
}
