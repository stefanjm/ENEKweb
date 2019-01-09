using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ENEKdata.Utilities {
    public static class ImageManager {

        /// <summary>
        /// Uploads image to the given path. Checks if file is an image, generates random name and resizes the image
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="PathToUpload"></param>
        /// <returns></returns>
        public static async Task<string> UploadImage(IFormFile imageFile, string PathToUpload) {
            string fileName = null;
            if (imageFile.Length > 0 && imageFile.ContentType.Contains("image")) {
                // Upload the image

                // Specify Format and size
                Size imgSize = new Size(1280,0);

                // Generate a random unique file name
                fileName = Path.GetRandomFileName();

                // Add the extension to the random name
                //fileName = Path.ChangeExtension(fileName, Path.GetExtension(imageFile.FileName));
                // we will save the img as png
                fileName = Path.ChangeExtension(fileName, "jpg");
                var filePath = Path.Combine(PathToUpload, fileName);
                JpegEncoder encoder = new JpegEncoder() {
                    Quality = 70
                };
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    // Read the imageFile into ImageFactory stream, then write out to the filestream path
                    using (Image<Rgba32> image = Image.Load(imageFile.OpenReadStream())) {
                        image.Mutate(ctx => ctx.Resize(imgSize));
                        image.SaveAsJpeg(stream, encoder);
                    }
                }
            }
            // If not an image or invalid stream
            else {
                return null;
            }


            return fileName;
        }


        /// <summary>
        /// Upload multiple Images
        /// </summary>
        /// <returns>Image name with Extension</returns>
        public static async Task<List<string>> UploadImages(ICollection<IFormFile> imageFiles, string PathToUpload) {
            // List of names of the uploaded Images
            List<string> uploadedImgNames = new List<string>();
            // Check if an image then upload to path
            foreach (var imageFile in imageFiles) {
                string uploadedImgName = await UploadImage(imageFile, PathToUpload);
                if(uploadedImgName != null)
                    uploadedImgNames.Add(uploadedImgName);
            }

            return uploadedImgNames;
        }

        //public static bool CheckIfImage(IFormFile fileStream) {
        //    // read first 10 bytes/ the header
        //    try {
        //        using (StreamReader stream = new StreamReader(fileStream)) {
        //            byte[] header = new Byte[10];
        //            fileStream.Read(header, 0, 10);

        //            foreach (var pattern in new byte[][] {
        //            Encoding.ASCII.GetBytes("BM"),
        //            Encoding.ASCII.GetBytes("GIF"),
        //            new byte[] { 137, 80, 78, 71 },     // PNG
        //            new byte[] { 73, 73, 42 },          // TIFF
        //            new byte[] { 77, 77, 42 },          // TIFF
        //            new byte[] { 255, 216, 255, 224 },  // jpeg
        //            new byte[] { 255, 216, 255, 225 }   // jpeg canon
        //    }) {
        //                if (pattern.SequenceEqual(header.Take(pattern.Length)))
        //                    return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch {
        //        return false;
        //    }
        //}
    }
}
