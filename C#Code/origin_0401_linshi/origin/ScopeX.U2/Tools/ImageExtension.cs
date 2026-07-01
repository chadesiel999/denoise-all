using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    internal static class ImageExtension
    {
        public static Bitmap ToImage(this Byte[,] matrix, Color color)
        {
            Int32 height = matrix.GetLength(0);
            Int32 width = matrix.GetLength(1);

            Bitmap bmp = new(width, height);
            System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Int32 stride = bmpdata.Stride;
            Int32 space = Math.Abs(stride) - width * 4;
            Int32 bytes = Math.Abs(stride) * height - space;
            var rgbs = new Byte[bytes];
            Int32 ptr = 0;
            for (Int32 i = 0; i < height; i++)
            {    
                for (Int32 j = 0; j < width; j++)
                {
                    //Convert population to color
                    rgbs[ptr] = rgbs[ptr + 1] = rgbs[ptr + 2] = matrix[i, j];
                    ptr += 4;
                }
                ptr += space;
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbs, 0, bmpdata.Scan0, bytes);
            bmp.UnlockBits(bmpdata);
            return bmp;
        }

        public static Byte[,] ToMatrix(this Bitmap bmp)
        {
            System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr startptr = bmpdata.Scan0;
            Int32 stride = bmpdata.Stride;
            Int32 space = Math.Abs(stride) - bmp.Width * 4;
            Int32 bytes = Math.Abs(stride) * bmp.Height;// - space;
            Byte[] argbs = new Byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(startptr, argbs, 0, bytes);
            bmp.UnlockBits(bmpdata);

            var matrix = new Byte[bmp.Height, bmp.Width];
            Int32 ptr = 0;
            double temp;
            for (Int32 i = 0; i < bmp.Height; i++)
            {   
                for (Int32 j = 0; j < bmp.Width; j++)
                {
                    temp = argbs[ptr + 2] * 0.299 + argbs[ptr + 1] * 0.587 + argbs[ptr] * 0.114;
                    matrix[i, j] = (Byte)temp;
                    ptr += 4;
                }
                ptr += space;
            }
            return matrix;
        }
    }
}
