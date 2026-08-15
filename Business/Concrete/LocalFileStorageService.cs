using Core.Utilities.FileStorage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    /// <summary>
    /// IFileStorageService-in local disk üzərində sadə implementasiyası. Faylların
    /// kök qovluğu "FileStorage:BasePath" konfiqurasiya açarından oxunur (default:
    /// "wwwroot/uploads"), tapılmasa tətbiqin işçi qovluğuna nisbətən həll olunur.
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        public LocalFileStorageService(IConfiguration configuration)
        {
            var configuredPath = configuration["FileStorage:BasePath"] ?? "wwwroot/uploads";

            _basePath = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(AppContext.BaseDirectory, configuredPath);
        }
        public void Delete(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public bool Exists(string relativePath)
        {
            return File.Exists(GetFullPath(relativePath));

        }

        public IEnumerable<string> ListFiles(string subFolder)
        {
            var folderPath = Path.Combine(_basePath, subFolder);

            if (!Directory.Exists(folderPath))
                return Enumerable.Empty<string>();

            return Directory.GetFiles(folderPath)
                .Select(fullPath => Path.Combine(subFolder, Path.GetFileName(fullPath)).Replace('\\', '/'));
        }

        public async Task<byte[]?> ReadAsync(string relativePath)
        {
            var fullPath = GetFullPath(relativePath);

            if (!File.Exists(fullPath))
                return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task<string> SaveAsync(Stream content, string fileName, string subFolder)
        {
            var folderPath = Path.Combine(_basePath, subFolder);
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(fileStream);
            }

            // Nisbi yol həmişə "/" ilə saxlanılır ki, DB-dəki dəyər platformadan asılı olmasın.
            return Path.Combine(subFolder, fileName).Replace('\\', '/');
        }

        private string GetFullPath(string relativePath)
        {
            var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(_basePath, normalized);
        }
    }
}
