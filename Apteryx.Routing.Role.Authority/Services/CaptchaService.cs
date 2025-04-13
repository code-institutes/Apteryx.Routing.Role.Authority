using Microsoft.Extensions.Caching.Distributed;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace Apteryx.Routing.Role.Authority.Services
{
    public class CaptchaService
    {
        private static readonly char[] AllowedChars = "ACDEFGHJKLMNPQRSUVWXY23456789".ToCharArray();
        private readonly IDistributedCache _cache;

        public CaptchaService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<(string Code, byte[] ImageBytes)> GenerateCaptchaAsync(string key, CaptchaType type)
        {
            int width = 120, height = 50;
            var random = new Random();
            string captchaCode = new string(
                Enumerable.Range(0, 4)
                          .Select(_ => AllowedChars[random.Next(AllowedChars.Length)])
                          .ToArray());

            using var image = new Image<Rgba32>(width, height);
            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White);

                // 彩色干扰线
                for (int i = 0; i < 5; i++)
                {
                    var start = new PointF(random.Next(width), random.Next(height));
                    var end = new PointF(random.Next(width), random.Next(height));
                    var path = new PathBuilder().AddLine(start, end).Build();
                    var lineColor = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    ctx.Draw(lineColor, 1.5f, path);
                }

                // 绘制验证码文本（每个字符不同颜色和 Y 方向抖动）
                Font font = SystemFonts.CreateFont("Arial", 28, FontStyle.Bold);
                for (int i = 0; i < captchaCode.Length; i++)
                {
                    var charColor = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    var charText = captchaCode[i].ToString();
                    var position = new PointF(20 + i * 24, 10 + random.Next(-5, 5)); // 上下抖动
                    ctx.DrawText(charText, font, charColor, position);
                }

                // 彩色噪点
                for (int i = 0; i < 300; i++)
                {
                    var color = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    var pos = new PointF(random.Next(width), random.Next(height));
                    var ellipse = new EllipsePolygon(pos, new SizeF(1, 1));
                    ctx.Fill(color, ellipse);
                }
            });

            // 图像变形处理（双轴波动）
            using var distortedImage = ApplyWaveDistortion2D(image);
            using var ms = new MemoryStream();
            distortedImage.SaveAsPng(ms);

            // 缓存验证码文本
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            };
            await _cache.SetStringAsync($"Captcha_{type}_{key}", captchaCode, cacheOptions);

            return (captchaCode, ms.ToArray());
        }

        /// <summary>
        /// 双轴波动图像扭曲：水平 + 垂直 Sin/Cos 偏移
        /// </summary>
        private Image<Rgba32> ApplyWaveDistortion2D(Image<Rgba32> sourceImage)
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            var result = new Image<Rgba32>(width, height);

            float ampX = 5f, freqX = 2f;
            float ampY = 3f, freqY = 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float offsetX = ampX * MathF.Sin(2 * MathF.PI * y / height * freqX);
                    float offsetY = ampY * MathF.Cos(2 * MathF.PI * x / width * freqY);

                    int srcX = x - (int)offsetX;
                    int srcY = y - (int)offsetY;

                    if (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
                    {
                        result[x, y] = sourceImage[srcX, srcY];
                    }
                }
            }

            return result;
        }
    }
}
