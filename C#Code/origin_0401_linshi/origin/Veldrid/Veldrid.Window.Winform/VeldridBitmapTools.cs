using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Windows.Winform
{
    public static class VeldridBitmapTools
    {
        public static BitmapData LoadFormFile(string path)
        {
            if(string.IsNullOrEmpty(path)|| !System.IO.File.Exists(path))
            {
                return BitmapData.Empty;
            }
            else
            {
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(path);
                return LoadFormBitmap(bitmap);
            }
        }
        public static BitmapData LoadFormBitmap(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Byte[] data = new Byte[bitmap.Width * bitmap.Height * 4];
            var bitmapdata = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(bitmapdata.Scan0, data, 0, data.Length);
            bitmap.UnlockBits(bitmapdata);
            bitmap.Dispose();
            return new BitmapData(width, height, data);
        }

        public static BitmapData LoadFormStream(Stream stream)
        {
            if (stream ==null || stream.Length ==0)
            {
                return BitmapData.Empty;
            }
            else
            {
                Bitmap bitmap = (Bitmap)Bitmap.FromStream(stream);
                return LoadFormBitmap(bitmap);
            }
        }
    }
}
