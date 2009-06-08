using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MyLife.Serialization
{
    public static class ImageBase64Serializer
    {
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                var imageBytes = ms.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            var image = Image.FromStream(ms, true);
            return image;
        }

        public static string BuildImageTag(Image image, ImageFormat format)
        {
            // IE8 limit to 32 Kb
            var ms = new MemoryStream();
            image.Save(ms, format);
            if (ms.Length/1024 > 32)
            {
                throw new ArgumentOutOfRangeException("image", "IE8 cannot support Data Uri string great than 32KB");
            }

            var base64String = Convert.ToBase64String(ms.ToArray());
            ms.Dispose();
            var stringFormat = "image/" + format.ToString().ToLower();
            return string.Format(
                "<img src=\"data:{0};base64,{1}\" alt=\"Base64 encoded image\" width=\"{2}\" height=\"{3}\" />",
                stringFormat, base64String, image.Width, image.Height);
        }
    }
}