using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace CatBot.Services
{
    public static class ImageService
    {
        public static MemoryStream DrawTextonImage(string text, Stream image)
        {
            Image img = Image.FromStream(image);
            Bitmap bitmap = new Bitmap(new Bitmap(img));

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            Brush brush = new SolidBrush(Color.FromKnownColor(KnownColor.White));

            Font font = new Font("Impact", 60, FontStyle.Regular);

            SizeF tempSize = graphics.MeasureString(text, font);

            while (tempSize.Width < bitmap.Width && tempSize.Height < bitmap.Height)
            {
                tempSize = graphics.MeasureString(text, font);
                var tempSizeToIncrease = font.Size; //writeprotected
                font = new Font(font.Name, tempSizeToIncrease += 0.1f);
            }

            GraphicsPath p = new GraphicsPath();
            p.AddString(
                text.ToUpper(),
                font.FontFamily,
                (int)FontStyle.Bold,
                (float)(graphics.DpiY * font.Size / 72 * 0.5),
                new Point(Convert.ToInt32((bitmap.Width / 2) - (tempSize.Width / 2)), Convert.ToInt32((bitmap.Height / 2) - (tempSize.Height / 2))),
                new StringFormat());

            graphics.FillPath(brush, p);
            graphics.DrawPath(Pens.Black, p);
            graphics.DrawPath(Pens.Black, p);
            graphics.DrawPath(Pens.Black, p);
            graphics.DrawPath(Pens.Black, p);
            graphics.DrawPath(Pens.Black, p);

            var a = new MemoryStream();
            bitmap.Save(a, System.Drawing.Imaging.ImageFormat.Png);

            return a;
        }
    }
}
