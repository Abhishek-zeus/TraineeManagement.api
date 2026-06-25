using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TraineeManagement.myapp.Utility
{
    public class SubmissionFileUtilities
    {
        public static string CalculateSHA256Checksum(Stream stream)
        {
            using var sha256 = SHA256.Create();
            var hashbytes = sha256.ComputeHash(stream); //converts bytes to hex string
            return Convert.ToHexString(hashbytes).ToLowerInvariant();
        }

        public static bool IsContentTypeAllowed(string contentType)
        {
            var allowedContentTypes = new[]
            {
                "application/pdf","application/zip","application/x-zip-compressed","text/plain",
                "application/msword","image/png","image/jpeg"
            };
            return allowedContentTypes.Contains(contentType.ToLowerInvariant());
        }

    }
}