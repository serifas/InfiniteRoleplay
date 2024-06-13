using Dalamud.Interface.Internal;
using InfiniteRoleplay.Windows;
using OtterGui.Log;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using InfiniteRoleplay;
using Dalamud.Plugin;
using Networking;
using InfiniteRoleplay;

namespace InfiniteRoleplay.Helpers
{
    internal static class Imaging
    {
        public static Plugin plugin;
        public static void DownloadProfileImage(bool self, string url, int profileID, bool nsfw, bool trigger, Plugin plugin, int index)
        {
         
            if(IsImageUrl(url))
            {
                try { 
                    WebClient webClient = new WebClient();
                    string extension = GetImageFileExtension(url);
                    if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
                    {
                        string GalleryPath = Path.Combine(path, "UI/Galleries/" + profileID + "/");
                        string imagePath = Path.Combine(path, "UI/Galleries/" + profileID + "/" + "gallery_" + profileID + "_" + index + "." + extension); // Create a folder named 'Images' in your root directory
                        if (!Directory.Exists(GalleryPath))
                        {
                            Directory.CreateDirectory(GalleryPath);
                        }
                        webClient.DownloadFile(url, imagePath);

                        Image baseImage = Image.FromFile(imagePath);
                        Image scaledImage = ScaleImage(baseImage, 1000, 800);
                        SaveImage(scaledImage, GalleryPath, "gallery_scaled_" + profileID + "_" + index + "." + extension);
                        string scaledImagePath = Path.Combine(path, "UI/Galleries/" + profileID + "/" + "gallery_scaled_" + profileID + "_" + index + "." + extension);


                        if (self == true)
                        {
                            var image = plugin.PluginInterface.UiBuilder.LoadImage(scaledImagePath);
                            if(image != null)
                            {
                                ProfileWindow.galleryImages[index] = image;
                            }
                            ProfileWindow.imageURLs[index] = url;
                            ProfileWindow.NSFW[index] = nsfw;
                            ProfileWindow.TRIGGER[index] = trigger;
                        }
                        else
                        {
                            var image = plugin.PluginInterface.UiBuilder.LoadImage(scaledImagePath);
                            if(image != null)
                            {
                                TargetWindow.galleryImages[index] = image;
                            }
                            
                        }
                        if (trigger == true && nsfw == false)
                        {

                            if (self == true)
                            {
                                var image = Constants.UICommonImage(plugin, Constants.CommonImageTypes.TRIGGER);
                                if(image != null)
                                {
                                    ProfileWindow.galleryThumbs[index] = image;
                                }
                                
                            }
                            else
                            {
                                var image = Constants.UICommonImage(plugin, Constants.CommonImageTypes.TRIGGER);
                                if (image != null)
                                {
                                    TargetWindow.galleryThumbs[index] = image;
                                }
                            }
                        }
                        if (nsfw == true && trigger == false)
                        {
                            if (self == true)
                            {
                                var image = Constants.UICommonImage(plugin, Constants.CommonImageTypes.NSFW);
                                if(image != null)
                                {
                                    ProfileWindow.galleryThumbs[index] = image;
                                }
                            }
                            else
                            {
                                var image = Constants.UICommonImage(plugin, Constants.CommonImageTypes.NSFW);
                                if (image != null)
                                {
                                    TargetWindow.galleryThumbs[index] = image;
                                }
                            }

                        }
                        if (nsfw == true && trigger == true)
                        {
                            if (self == true)
                            {
                                var image = Constants.UICommonImage(plugin, Constants.CommonImageTypes.NSFWTRIGGER);
                                if(image != null)
                                {
                                    ProfileWindow.galleryThumbs[index] = image;
                                }

                            }
                            else
                            {
                                var image = Constants.UICommonImage(plugin, Constants.CommonImageTypes.NSFWTRIGGER);
                                if (image != null)
                                {
                                    TargetWindow.galleryThumbs[index] = image;
                                }
                                
                            }
                        }

                        if (nsfw == false && trigger == false)
                        {
                            Image thumb = Image.FromFile(imagePath);
                            Image img = ScaleImage(thumb, 120, 120);
                            SaveImage(img, GalleryPath, "gallery_thumb_" + profileID + "_" + index + "." + extension);
                            if (self == true)
                            {
                                var image = plugin.PluginInterface.UiBuilder.LoadImage(Path.Combine(path, "UI/Galleries/" + profileID + "/gallery_thumb_" + profileID + "_" + index + "." + extension));
                                if(image != null)
                                {
                                    ProfileWindow.galleryThumbs[index] = image;                                }
                            }
                            else
                            {
                                TargetWindow.galleryThumbs[index] = plugin.PluginInterface.UiBuilder.LoadImage(Path.Combine(path, "UI/Galleries/" + profileID + "/gallery_thumb_" + profileID + "_" + index + "." + extension));
                            }
                        }

                    }
                }
                catch
                {
                   plugin.Logger.Error("Unable to Read or Write Image");
                }
            }
        }
        static string GetImageFileExtension(string url)
        {
            try
            {
                // Send a HEAD request to fetch only the headers
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check if the Content-Type header indicates an image
                    if (response.ContentType.ToLower().StartsWith("image/"))
                    {
                        // Extract the file extension from the Content-Type header
                        string contentType = response.ContentType;
                        string fileExtension = GetFileExtensionFromContentType(contentType);

                        return fileExtension;
                    }
                    else
                    {
                        throw new Exception("The URL does not point to an image.");
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        static string GetFileExtensionFromContentType(string contentType)
        {
            switch (contentType.ToLower())
            {
                case "image/jpeg":
                    return "jpg";
                case "image/png":
                    return "png";
                default:
                    throw new Exception("Unsupported image format.");
            }
        }

        public static bool IsImageUrl(string url)
        {
            try
            {
                // Send a HEAD request to fetch only the headers
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check if the Content-Type header indicates an image
                    if (response.ContentType.ToLower().StartsWith("image/"))
                    {
                        return true;
                    }
                }
            }
            catch (WebException)
            {
                // URL is invalid or inaccessible
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"An error occurred: {ex.Message}");
            }

            return false;
        }
       

        static void SaveImage(Image image, string directoryPath, string fileName)
        {
            // Check if the directory exists, if not, create it
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            // Combine the directory path and file name to get the full file path
            string filePath = System.IO.Path.Combine(directoryPath, fileName);

            // Save the image to the specified file path
            image.Save(filePath);

            Console.WriteLine("Image saved successfully.");
        }
        public static byte[] BlurBytes(this Bitmap image, Int32 blurSize)
        {
            var rectangle = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(blurred))
                graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    Int32 avgR = 0, avgG = 0, avgB = 0;
                    Int32 blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (Int32 x = Math.Max(0, xx - blurSize); x <= Math.Min(xx + blurSize, image.Width - 1); x++)
                    {
                        for (Int32 y = Math.Max(0, yy - blurSize); y <= Math.Min(yy + blurSize, image.Height - 1); y++)
                        {
                            System.Drawing.Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, System.Drawing.Color.FromArgb(avgR, avgG, avgB));
                }
            }
            return ImageToByteArray(blurred);
        }
        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            int newWidth, newHeight;
            if(image.Width > maxWidth || image.Height > maxHeight)
            {
                // Calculate aspect ratio
                double ratioX = (double)maxWidth / image.Width;
                double ratioY = (double)maxHeight / image.Height;
                double ratio = Math.Min(ratioX, ratioY);

                // Calculate new dimensions
                newWidth = (int)(image.Width * ratio);
                newHeight = (int)(image.Height * ratio);

                // Create new bitmap with new dimensions
                Bitmap newImage = new Bitmap(newWidth, newHeight);

                // Draw the original image on the new bitmap with scaled dimensions
                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(newImage))
                {
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                return newImage;
            }
            else
            {
                return image;
            }
        }
        public static byte[] ScaleImageBytes(byte[] imgBytes, int maxWidth, int maxHeight)
        {
            System.Drawing.Image img = byteArrayToImage(imgBytes);
            var ratioX = (double)maxWidth / img.Width;
            var ratioY = (double)maxHeight / img.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(img.Width * ratio);
            var newHeight = (int)(img.Height * ratio);

            var scaledImage = new Bitmap(newWidth, newHeight);

            using (var graphics = System.Drawing.Graphics.FromImage(scaledImage))
                graphics.DrawImage(img, 0, 0, newWidth, newHeight);

            byte[] scaledBytes = ImageToByteArray(scaledImage);

            return scaledBytes;
        }
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        public static System.Drawing.Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(memstr);
                return img;
            }
        }

        internal static void RemoveAllImages(InfiniteRoleplay.Plugin plugin)
        {
            if (plugin.PluginInterface is { AssemblyLocation.Directory.FullName: { } path })
            {
                string GalleryPath = Path.Combine(path, "UI/Galleries/");
                if (Directory.Exists(GalleryPath))
                {
                    Directory.Delete(GalleryPath, true);
                }
            }
            
        }
    }
}
