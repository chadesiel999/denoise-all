using FontStashSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Veldrid.Common.VeldridCompute.Test
{
    public class AddTestCompute : BaseCompute
    {
        private DeviceBuffer _Buffer;
        private ResourceLayout _ResourceLayout;
        private ResourceSet _ResourceSet;

        private ResourceLayout _BufferLayout;
        private ResourceSet _BufferSet;
        private Pipeline _Pipeline;
        private Info _Info;
        
        public AddTestCompute(IVeldridContent content,UInt32 dataCount = 1024) : base(content,dataCount)
        {
            CreateResources();
        }
        public int Value { get => _Info.Value; set => _Info.Value = value; }
        internal override void CreateResources()
        {
            CreateBuffer<Vector4>();
            _Buffer = ResourceFactory.CreateBuffer(new BufferDescription(Info.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ResourceLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("DataBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Compute)
            ));
            _ResourceSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ResourceLayout,_DataBuffer));
            _BufferLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Info", ResourceKind.UniformBuffer, ShaderStages.Compute)));
            _BufferSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_BufferLayout, _Buffer));
            _Pipeline = CreatePipLine("AddTestCompute.comp",new ResourceLayout[] { _ResourceLayout,_BufferLayout }, 1024, 1, 1);
        }
        internal override void DisposeResources()
        {
            base.DisposeResources();
            _ResourceLayout?.Dispose();
            _ResourceSet?.Dispose();
            _BufferLayout?.Dispose();
            _BufferSet?.Dispose();
            _Buffer?.Dispose();
            _Pipeline?.Dispose();
        }
        internal unsafe override void DoComputeCore(Boolean getResult = true)
        {
            base.DoComputeCore(getResult);
            GraphicsDevice.UpdateBuffer(_Buffer, 0, _Info);
            CommandList.Begin();
            CommandList.SetPipeline(_Pipeline);
            CommandList.SetComputeResourceSet(0, _ResourceSet);
            CommandList.SetComputeResourceSet(1, _BufferSet);
            CommandList.Dispatch(MaxDataCount / 4/1024, 1, 1);
            if(getResult)
            {
                CommandList.CopyBuffer(_DataBuffer, 0, _ResultBuffer, 0, _ResultBuffer.SizeInBytes);
            }
            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
            GraphicsDevice.WaitForIdle();
        }
        struct Info
        {
            public static UInt32 SizeInBytes = 16;
            public Int32 Value;
            public Vector3 Spare;
        }
    }
}
