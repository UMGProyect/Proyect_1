using System.Drawing;
using System.Drawing.Imaging;
namespace Proyect_1.Services
{
  

public class CaptchaService
    {
        private static readonly Random Random = new Random();

        public void GenerateCaptcha(HttpContext context)
        {
            string captchaText = GenerateRandomText();
            context.Session.SetString("CaptchaText", captchaText);

            using (var bitmap = new Bitmap(200, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawString(captchaText, new Font("Arial", 24), Brushes.Black, new PointF(10, 10));

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    context.Response.ContentType = "image/png";
                    context.Response.Body.Write(ms.ToArray(), 0, (int)ms.Length);
                }
            }
        }

        private string GenerateRandomText()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }

}
