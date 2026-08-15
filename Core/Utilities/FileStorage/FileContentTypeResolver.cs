using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.FileStorage
{
    /// <summary>
    /// Fayl uzantısına görə MIME (Content-Type) dəyərini müəyyən edir. Yalnız
    /// yükləmə üçün icazə verilən şəkil formatlarını əhatə edir.
    /// </summary>
    public class FileContentTypeResolver
    {
        private static readonly Dictionary<string, string> ExtensionToContentType = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };
        public static IReadOnlyCollection<string> AllowedExtensions => ExtensionToContentType.Keys;

        public static bool IsAllowedExtension(string extension) => ExtensionToContentType.ContainsKey(extension);

        public static string Resolve(string pathOrFileName)
        {
            var extension = Path.GetExtension(pathOrFileName);
            return ExtensionToContentType.TryGetValue(extension, out var contentType)
                ? contentType
                : "application/octet-stream";
        }
    }
}
