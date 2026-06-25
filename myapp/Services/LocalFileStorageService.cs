using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Utility;
using Microsoft.Extensions.Options;

namespace TraineeManagement.myapp.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storageRoot;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(IOptions<FileStorageSettings> options, ILogger<LocalFileStorageService> logger)
        {
            //options is used to map settings class and json 
            _storageRoot = options.Value.StorageRoot;
            _logger = logger;

            //Creates a upload folder if it doesn't exist
            Directory.CreateDirectory(_storageRoot);
        }

        public async Task<string> SaveAsync(Stream fileStream, string storageName, CancellationToken cancellationToken = default)
        {
            var filePath = GetSafePath(storageName);

            // using keyword is used to safelt close the resource after the use to avoid crashes
            // FileMode.Create is used to create a file or overwrite the file if existing
            using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            // Copy to DB
            await fileStream.CopyToAsync(outputStream,cancellationToken);

            _logger.LogInformation("File saved to storage. Storagename : {StorageName}", storageName);
            return storageName;
        }


        public Task<Stream> OpenReadAsync(string storageName, CancellationToken cancellationToken = default)
        {
            //GetSafePath is used to safeguard the file access from malicious paths by attackers
            //It combines the rootpath and storagename and gives a total filepath
            var filePath = GetSafePath(storageName);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File not found in storage. StorageName: {StorageName}",storageName);
                throw new FileNotFoundException("File not found in storage.", storageName);
            }

            Stream stream = File.OpenRead(filePath);

            return Task.FromResult(stream);
        }


        public Task<bool> ExistsAsync(string storageName, CancellationToken cancellationToken = default)
        {
            var filePath = GetSafePath(storageName);
            return Task.FromResult(File.Exists(filePath));
        }


        public Task DeleteAsync(string storageName, CancellationToken cancellationToken = default)
        {
            var filePath = GetSafePath(storageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File Deleted from storage. StorageName: {StorageName}", storageName);
            }
            else
            {
                _logger.LogWarning("Delete attempted but file not found. StorageName: {StorageName}", storageName);
            }
            return Task.CompletedTask;
        }


        private string GetSafePath(string storageName)
        {
            //GetFileName() strips all directory parts and leaves only the file name
            //like attacker sends storageName ="../../etc/appsettings.json"
            //GetFileName only return appsettings.json stripping traversal

            var safeFileName = Path.GetFileName(storageName);
            return Path.Combine(_storageRoot, safeFileName);
        }
    }
}