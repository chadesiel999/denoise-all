using System.Runtime.InteropServices;
using NativeLibraryLoader;
using NativeLibrary = NativeLibraryLoader.NativeLoader;

namespace Veldrid.SPIRV
{
    internal unsafe class VeldridSpirvNative
    {
        NativeLibraryLoader.NativeLoader loader;
        private VeldridSpirvNative()
        {
            string[] names;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                names = new[] { "libveldrid-spirv.dll" };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                names = new[]
                {
                    "libveldrid-spirv.so",
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                names = new[]
                {
                    "libveldrid-spirv.dylib"
                };
            }
            else
            {
                names = new[] { "libveldrid-spirv.dll" };
            }
            loader = new NativeLibrary("libveldrid-spirv");
        }
        static VeldridSpirvNative()
        {

        }
        private delegate CompilationResult* CrossCompileDelegate(CrossCompileInfo* info);
        private delegate CompilationResult* CompileGlslToSpirvDelegate(GlslCompileInfo* info);
        private delegate void FreeResultDelegate(CompilationResult* result);
        private CrossCompileDelegate crossCompileDelegate;
        private CompileGlslToSpirvDelegate compileGlslToSpirvDelegate;
        private FreeResultDelegate freeResultDelegate;

        public CompilationResult* CrossCompile(CrossCompileInfo* info)
        {
            if(crossCompileDelegate==null)
            {
                crossCompileDelegate = loader.LoadFunction<CrossCompileDelegate>(nameof(CrossCompile));
            }
            return crossCompileDelegate(info);
        }

        public CompilationResult* CompileGlslToSpirv(GlslCompileInfo* info)
        {
            if(compileGlslToSpirvDelegate==null)
            {
                compileGlslToSpirvDelegate = loader.LoadFunction<CompileGlslToSpirvDelegate>(nameof(CompileGlslToSpirv));
            }
            return compileGlslToSpirvDelegate(info);
        }

        public void FreeResult(CompilationResult* result)
        {
            if(freeResultDelegate ==null)
            {
                freeResultDelegate = loader.LoadFunction<FreeResultDelegate>(nameof(FreeResult));
            }
            freeResultDelegate(result);
        }

        public static VeldridSpirvNative Default { get; } = new VeldridSpirvNative();
    }
}
