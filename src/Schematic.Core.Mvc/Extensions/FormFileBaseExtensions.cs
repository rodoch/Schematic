using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Ansa.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Schematic.Core.Mvc
{
    public static class FormFileBaseExtensions
    {
        public static string GetAssetUri(this IFormFile file, string path, string fileName = "")
        {
            if (!fileName.HasValue())
            {
                fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = (!string.IsNullOrWhiteSpace(fileName)) ? fileName : Convert.ToString(Guid.NewGuid());
            }

            if (!path.EndsWith(@"/"))
                path = path + "/";

            return path + fileName;
        }

        public const int ImageMinimumBytes = 512;

        public static bool TryGetImageAsset(this IFormFile file, out ImageAsset image)
        {
            // Default to null out parameter
            image = null;

            // Check the image mime types
            if (file.ContentType.ToLower() != "image/jpg"
                && file.ContentType.ToLower() != "image/jpeg"
                && file.ContentType.ToLower() != "image/pjpeg"
                && file.ContentType.ToLower() != "image/gif"
                && file.ContentType.ToLower() != "image/x-png"
                && file.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            // Check the image extension
            if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
                && Path.GetExtension(file.FileName).ToLower() != ".png"
                && Path.GetExtension(file.FileName).ToLower() != ".gif"
                && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            // Attempt to read the file and check the first bytes
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    if (!stream.CanRead)
                        return false;
                    
                    if (file.Length < ImageMinimumBytes)
                        return false;

                    byte[] buffer = new byte[ImageMinimumBytes];
                    stream.Read(buffer, 0, ImageMinimumBytes);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    
                    if (Regex.IsMatch(content,
                        @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            // Try to instantiate new RGB image and return metadata
            // If exception is thrown we can assume that it's not a valid image
            // Some malformed JPEGs will raise a NullReference exception —
            // This should be fixed will a new release of ImageSharp, see: https://github.com/SixLabors/ImageSharp/issues/721
            using (var stream = file.OpenReadStream())
            {
                try
                {
                    using (Image<Rgba32> imageFile = Image.Load(stream)) 
                    {
                        image = new ImageAsset()
                        {
                            Height = imageFile.Height,
                            Width = imageFile.Width
                        };
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}