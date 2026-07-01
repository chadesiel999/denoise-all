using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Veldrid.Common.VeldridCompute
{
    public abstract class BaseCompute
    {
        private IVeldridContent _Content;
        protected DeviceBuffer _DataBuffer;
        protected DeviceBuffer _ResultBuffer;
        private bool disposedValue;
        protected ResourceFactory ResourceFactory => _Content.GraphicsManger.ResourceFactory;
        protected CommandList CommandList { get; }
        protected GraphicsDevice GraphicsDevice => _Content.GraphicsManger.Device;
        public UInt32 MaxDataCount { get; private set; }

        public BaseCompute(IVeldridContent content,UInt32 dataCount =1024)
        {
            MaxDataCount= dataCount;
            _Content = content;
            CommandList = content.GraphicsManger.Device.ResourceFactory.CreateCommandList();
        }
        protected void CreateBuffer<T>() where T :unmanaged
        {
            _DataBuffer?.Dispose();
            _DataBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)Unsafe.SizeOf<T>() * MaxDataCount, BufferUsage.StructuredBufferReadWrite, (UInt32)Unsafe.SizeOf<T>()));
            _ResultBuffer?.Dispose();
            _ResultBuffer = ResourceFactory.CreateBuffer(new BufferDescription(_DataBuffer.SizeInBytes, BufferUsage.Staging));
        }
        private protected Pipeline CreatePipLine(String shaderName,ResourceLayout resourceLayout,UInt32 threadGroupSizeX,UInt32 threadGroupSizeY,UInt32 threadGroupSizeZ)
        {
            Shader shader = GetComputeShader(shaderName);
            return ResourceFactory.CreateComputePipeline(new ComputePipelineDescription(shader,resourceLayout,threadGroupSizeX,threadGroupSizeY,threadGroupSizeZ));
        }
        public unsafe T GetResult<T>(UInt32 offsetbytes = 0) where T :unmanaged
        {
            var maped = GraphicsDevice.Map(_ResultBuffer, MapMode.Read);
            T* ptr = (T*)((Byte*)maped.Data.ToPointer() + offsetbytes);
            T va = ptr[0];
            GraphicsDevice.Unmap(_ResultBuffer);
            return va;
        }
        public unsafe void GetResult<T>(ref T[] data, UInt32 count , UInt32 offsetbytes=0) where T :unmanaged
        {
            if (count <= 0 || data ==null || data.Length ==0) return;
            var maped = GraphicsDevice.Map(_ResultBuffer, MapMode.Read);
            T* ptr = (T*)((Byte*)maped.Data.ToPointer() + offsetbytes);
            fixed (void* destptr = &data[0])
            {
                Int32 tempbytecount = Math.Min(data.Length, (Int32)count)*Unsafe.SizeOf<T>();
                Buffer.MemoryCopy(ptr, destptr, tempbytecount, tempbytecount);
            }
            GraphicsDevice.Unmap(_ResultBuffer);
        }
        public unsafe void GetResult<T>(ref T[,] data, UInt32 count, UInt32 offsetbytes = 0) where T : unmanaged
        {
            if (count <= 0 || data == null || data.Length == 0) return;
            var maped = GraphicsDevice.Map(_ResultBuffer, MapMode.Read);
            T* ptr = (T*)((Byte*)maped.Data.ToPointer() + offsetbytes);
            fixed (void* destptr = &data[0,0])
            {
                Int32 tempbytecount = Math.Min(data.Length, (Int32)count) * Unsafe.SizeOf<T>();
                Buffer.MemoryCopy(ptr, destptr, tempbytecount, tempbytecount);
            }
            GraphicsDevice.Unmap(_ResultBuffer);
        }
        
        private protected Pipeline CreatePipLine(String shaderName, ResourceLayout[] resourceLayouts, UInt32 threadGroupSizeX, UInt32 threadGroupSizeY, UInt32 threadGroupSizeZ)
        {
            Shader shader = GetComputeShader(shaderName);
            return ResourceFactory.CreateComputePipeline(new ComputePipelineDescription(shader, resourceLayouts, threadGroupSizeX, threadGroupSizeY, threadGroupSizeZ));
        }
        private Shader GetComputeShader(String name)
        {
            var shader = _Content.GraphicsManger.ShaderManger.GetOtherShader(name, ShaderStages.Compute);
            if(shader ==null)
            {
                throw new KeyNotFoundException(name);
            }
            return shader;
        }
        internal abstract void CreateResources();
        internal virtual void DisposeResources()
        {
            CommandList?.Dispose();
            _DataBuffer?.Dispose();
            _ResultBuffer?.Dispose();
        }
        public void DoCompute(Boolean getResult = true) => DoComputeCore(getResult);
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                DisposeResources();
                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BaseCompute()
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
        internal virtual void DoComputeCore(Boolean getResult = true)
        {
        }

        internal unsafe void DoCompute<T>(T* data, UInt32 sizeInBytes, Boolean getResult = true) where T : unmanaged
        {
            if(sizeInBytes ==0)
            {
                DoComputeCore(getResult);
                return;
            }
            GraphicsDevice.UpdateBuffer(_DataBuffer, 0, (IntPtr)data,Math.Min(sizeInBytes,_DataBuffer.SizeInBytes));
            DoComputeCore(getResult);
        }
        public unsafe void DoCompute<T>(T data, Boolean getResult = true) where T:unmanaged
        {
            DoCompute(&data, (UInt32)Unsafe.SizeOf<T>(),getResult);
        }

        public unsafe void DoCompute<T>(T[] data, Boolean getResult = true) where T:unmanaged
        {
            if(data ==null|| data.Length ==0)
            {
                DoCompute(getResult);
                return;
            }
            fixed(T* ptr = &data[0])
            {
                DoCompute(ptr, (UInt32)(Unsafe.SizeOf<T>()*data.Length),getResult);
            }
        }
        public unsafe void DoCompute<T>(T[,] data,Boolean getResult = true) where T : unmanaged
        {
            if (data == null || data.Length == 0)
            {
                DoCompute(getResult);
                return;
            }
            fixed (T* ptr = &data[0,0])
            {
                DoCompute(ptr, (UInt32)(Unsafe.SizeOf<T>() * data.Length), getResult);
            }
        }
    }
}
