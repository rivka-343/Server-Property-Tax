﻿using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using PropertyTax.Core.Services;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Amazon.S3.Model;
using System.Drawing;

namespace PropertyTax.Servise
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration) // namespace מלא
        {
            //var awsOptions = configuration.GetSection("AWS");
            //var accessKey = awsOptions["AccessKey"];
            //var secretKey = awsOptions["SecretKey"];
            //var region = awsOptions["Region"];
            //_bucketName = awsOptions["BucketName"];
            var awsOptions = configuration.GetSection("AWS");
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
            var region = Environment.GetEnvironmentVariable("AWS_REGION");
            _bucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");
            _s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }
        public async Task<string> GeneratePresignedUrlAsync(string fileName, string contentType, string userId)
        {
            // יצירת Key עם תיקיה לכל משתמש
            var fileKey = $"uploads/user_{userId}/{Guid.NewGuid()}_{fileName}";
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType = contentType
            };

            var url = _s3Client.GetPreSignedURL(request);

            return url;
        }

        public async Task<string> GetDownloadUrlAsync(string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(30) // תוקף של 30 דקות
            };
            Console.WriteLine(request);
            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<List<string>> GetUserFilesAsync(string userId)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = $"uploads/user_{userId}/"  // תיקיית המשתמש לפי ID
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            // אם יש קבצים בתיקיה של המשתמש, מחזירים את שמותיהם
            var fileUrls = response.S3Objects.Select(obj => $"https://{_bucketName}.s3.amazonaws.com/{obj.Key}").ToList();

            return fileUrls;
        }

    }
}

        //public async Task<string> GeneratePresignedUrlAsync(string fileName, string contentType)
        //{
        //    var request = new GetPreSignedUrlRequest
        //    {
        //        BucketName = _bucketName,
        //        Key = fileName,
        //        Verb = HttpVerb.PUT,
        //        Expires = DateTime.UtcNow.AddMinutes(10),
        //        ContentType = contentType
        //    };
        //    var url = _s3Client.GetPreSignedURL(request);

        //    return url;
        //}
//public async Task<string> UploadFileAsync(IFormFile file)
//{
//    using (var memoryStream = new MemoryStream())
//    {
//        await file.CopyToAsync(memoryStream);

//        var uploadRequest = new TransferUtilityUploadRequest
//        {
//            InputStream = memoryStream,
//            Key = $"{Guid.NewGuid()}-{file.FileName}",
//            BucketName = _bucketName,
//            CannedACL = S3CannedACL.PublicRead
//        };

//        var transferUtility = new TransferUtility(_s3Client);
//        await transferUtility.UploadAsync(uploadRequest);

//        return $"https://{_bucketName}.s3.amazonaws.com/{uploadRequest.Key}";
//    }
//}