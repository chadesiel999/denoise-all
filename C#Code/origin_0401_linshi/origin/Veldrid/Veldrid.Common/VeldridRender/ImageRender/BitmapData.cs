using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    public class BitmapData:IDisposable
    {
        public static BitmapData Empty = new BitmapData();
        public Vector2 Size { get; }
        public BitmapData()
        {
            Width= 0;
            Height= 0;
            Data = new Byte[0];
            Size = Vector2.Zero;
        }
        public BitmapData(int width,int height, byte[] data)
        {
            this.Width = width;
            this.Height= height;
            this.Data = data;
            Size= new Vector2(width,height);
        }
        private bool disposedValue;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Byte[] Data { get; private set; }
        public static Boolean operator !=(BitmapData a, BitmapData b)
        {
            return !a.Equals(b);
        }
        public static Boolean operator ==(BitmapData a, BitmapData b)
        {
            return a.Equals(b);
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is BitmapData bitmap)
            {
                bool result = Width == bitmap.Width && Height == bitmap.Height;
                if (result)
                {
                    if (Data == null || Data.Length == 0 || Data.Length != bitmap.Data.Length)
                    {
                        return false;
                    }
                    else
                    {
                        for (int i = 0; i < Data.Length; i++)
                        {
                            if (Data[i] != bitmap.Data[i]) return false;
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BitmapData()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
