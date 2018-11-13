using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ENEKdata.Utilities {
    public static class ImageManager {

        /// <summary>
        /// Uploads image to the given path. Checks if file is an image, generates random name and resizes the image
        /// </summary>
        /// <returns>Image name with Extension</returns>
        public static async Task<List<string>> UploadImages(ICollection<IFormFile> imageFiles, string PathToUpload) {
            // List of names of the uploaded Images
            List<string> uploadedImgNames = new List<string>();
            // Check if an image then upload to path
            foreach (var imageFile in imageFiles) {
                if (imageFile.Length > 0 && imageFile.ContentType.Contains("image")) {
                    // Upload the image

                    // Specify Format and size
                    ISupportedImageFormat format = new JpegFormat { Quality = 70 };
                    Size size = new Size(1280, 0);

                    // Generate a random unique file name
                    var fileName = Path.GetRandomFileName();

                    // Add the extension to the random name
                    fileName = Path.ChangeExtension(fileName, Path.GetExtension(imageFile.FileName));
                    var filePath = Path.Combine(PathToUpload, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        // Read the imageFile into ImageFactory stream, then write out to the filestream path
                        using (ImageFactory imageFactory = new ImageFactory()) {
                            imageFactory.Load(imageFile.OpenReadStream())
                                .Resize(size)
                                .Format(format)
                                .Save(stream);
                        }

                        // Add the uploaded file name to a List
                        uploadedImgNames.Add(fileName);
                    }


                }
                // If not an image or wrong stream
                else {
                    return null;
                }
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
