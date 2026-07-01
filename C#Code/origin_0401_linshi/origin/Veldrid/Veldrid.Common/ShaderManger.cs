using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Veldrid.SPIRV;
using Veldrid.Windows.Plot;

namespace Veldrid.Common
{
    internal class ShaderManger : IDisposable
    {
        private bool disposedValue;
        private Dictionary<String, Shader[]> AllShader = new Dictionary<String, Shader[]>();
        private Dictionary<String, Shader> OtherShader = new Dictionary<String, Shader>();
        private GraphicsDevice graphicsDevice;

        public ShaderManger(GraphicsDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }
            if (graphicsDevice != null)
            {
                return;
            }
            graphicsDevice = device;

            foreach (var val in GLSLManger.Default.AllGLSL.Keys)
            {
                List<Shader> shaders = new List<Shader>();
                shaders.AddRange(graphicsDevice.ResourceFactory.CreateFromSpirv(GLSLManger.Default.AllGLSL[val].Vertex, GLSLManger.Default.AllGLSL[val].Fragment));
                if (GLSLManger.Default.AllGLSL[val].Compute.Stage == ShaderStages.Compute)
                {
                    shaders.Add(graphicsDevice.ResourceFactory.CreateFromSpirv(GLSLManger.Default.AllGLSL[val].Compute));
                }
                //Shader frag = graphicsDevice.ResourceFactory.CreateShader(GLSLManger.Default.AllGLSL[val][1]);
                AllShader[val] = shaders.ToArray();
            }

        }
        /// <summary>
        /// 获取着色器
        /// </summary>
        /// <param name="name">着色器名</param>
        /// <param name="stages">着色器类型</param>
        /// <param name="entrypoint">着色器入口函数名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">着色器名为空</exception>
        /// <exception cref="NotSupportedException">不支持的着色器类型</exception>
        public Shader GetOtherShader(string name,ShaderStages stages = ShaderStages.Geometry,string entrypoint= "main")
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (OtherShader.TryGetValue(name, out var shader)) return shader;
            else
            {
                ShaderDescription description = new ShaderDescription(stages, GLSLManger.LoadShader(name), entrypoint);
                if (stages == ShaderStages.Compute)
                {
                    var result = SpirvCompilation.CompileGlslToSpirv(Encoding.UTF8.GetString(description.ShaderBytes), "", stages,
                                    new GlslCompileOptions()
                                    {
                                        Debug = false
                                    });
                    description.ShaderBytes = result.SpirvBytes;
                    OtherShader[name] = graphicsDevice.ResourceFactory.CreateFromSpirv(description);
                }
                else
                {
                    switch (graphicsDevice.BackendType)
                    {
                        case GraphicsBackend.Direct3D11:
                            OtherShader[name] = graphicsDevice.ResourceFactory.CreateShader(description);
                            break;
                        default:
                            {
                                if (graphicsDevice.BackendType == GraphicsBackend.Metal && stages == ShaderStages.Geometry)
                                {

                                    throw new NotSupportedException("Metal NotSupported Geometry Shader");
                                }
                                var result = SpirvCompilation.CompileGlslToSpirv(Encoding.UTF8.GetString(description.ShaderBytes), "", stages,
                                    new GlslCompileOptions()
                                    {
                                        Debug = false
                                    });
                                description.ShaderBytes = result.SpirvBytes;
                                OtherShader[name] = graphicsDevice.ResourceFactory.CreateFromSpirv(description);
                            }
                            break;
                    }
                }
                return OtherShader[name];
            }
        }
        /// <summary>
        /// 获取着色器
        /// 获取顶点、片元和计算着色器，其他类型的着色器请使用<see cref="GetOtherShader(string, ShaderStages, string)"/>
        /// </summary>
        /// <param name="name">着色器资源名</param>
        /// <returns>系统中存在的着色器</returns>
        /// <exception cref="KeyNotFoundException">指定的着色器未找到</exception>
        public Shader[] GetShaders(string name)
        {
            var keys = AllShader.Keys.FirstOrDefault(x => x.IndexOf(name) >= 0);
            if (string.IsNullOrEmpty(keys))
            {
                throw new KeyNotFoundException(name);
            }
            return AllShader[keys];
        }

        public Shader GetLocalFileShader(string path,string entrypoint ="main", ShaderStages stages = ShaderStages.Vertex)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            if (OtherShader.TryGetValue(path, out var shader)) return shader;
            Byte[] buffer = File.ReadAllBytes(path);
            ShaderDescription description = new ShaderDescription(stages, buffer, entrypoint);
            var spirvresult = SpirvCompilation.CompileGlslToSpirv(System.Text.Encoding.UTF8.GetString(buffer), "", stages, new GlslCompileOptions()
            {
                Debug = false,
            });
            description.ShaderBytes = spirvresult.SpirvBytes;
            OtherShader[path] = graphicsDevice.ResourceFactory.CreateFromSpirv(description);
            return OtherShader[path];
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    AllShader.Values.SelectMany(x => x).ToList().ForEach(x => x.Dispose());
                    OtherShader.Values.ToList().ForEach(x => x.Dispose());
                    AllShader.Clear();
                    OtherShader.Clear();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ShaderManger()
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

    struct ShaderInfo
    {
        public ShaderDescription Vertex;
        public ShaderDescription Fragment;
        public ShaderDescription Compute;
    }
    internal class GLSLManger
    {
        public Dictionary<String, ShaderInfo> AllGLSL = new Dictionary<String, ShaderInfo>();
        private bool first = true;
        private GLSLManger()
        {
        }
        public void Init()
        {
            if (!first) return;
            var result = typeof(IVeldridContent).Assembly.GetManifestResourceNames().Select(x =>
            {
                string[] strs = x.Split('.');
                return (string.Join('.', strs.Take(strs.Length - 1)), strs[^1]);
            }).Where(x => x.Item2.ToLower() == "vert" || x.Item2.ToLower() == "frag" || x.Item2.ToLower() == "comp").OrderByDescending(x => x.Item2).GroupBy(x => x.Item2).ToList();
            result.First(x=>x.Key == "vert").Select(x => x.Item1).Where(x => result.First(x=>x.Key == "frag").Select(y => y.Item1).Any(y => y == x)).ToList().ForEach(x =>
            {
                ShaderInfo shaderInfo = new ShaderInfo();
                var vertShader = new ShaderDescription(ShaderStages.Vertex, LoadShaderData(x + ".vert"), "main");
                var fragShader = new ShaderDescription(ShaderStages.Fragment, LoadShaderData(x + ".frag"), "main");
                var spirvresult = SpirvCompilation.CompileGlslToSpirv(System.Text.Encoding.UTF8.GetString(LoadShaderData(x + ".vert")), "", ShaderStages.Vertex, new GlslCompileOptions()
                {
                    Debug = false,
                });
                vertShader.ShaderBytes = spirvresult.SpirvBytes;
                spirvresult = SpirvCompilation.CompileGlslToSpirv(System.Text.Encoding.UTF8.GetString(LoadShaderData(x + ".frag")), "", ShaderStages.Fragment, new GlslCompileOptions()
                {
                    Debug = false,
                });
                fragShader.ShaderBytes = spirvresult.SpirvBytes;
                shaderInfo.Vertex = vertShader;
                shaderInfo.Fragment = fragShader;
                if (result.Any(y=>y.Key == "comp") && result.First(y => y.Key == "comp").Any(y => y.Item1.Contains(x)))
                {
                    var computeShader = new ShaderDescription(ShaderStages.Compute, LoadShaderData(x + ".comp"), "main");
                    spirvresult = SpirvCompilation.CompileGlslToSpirv(System.Text.Encoding.UTF8.GetString(LoadShaderData(x + ".comp")), "", ShaderStages.Compute, new GlslCompileOptions()
                    {
                        Debug = false,
                    });
                    computeShader.ShaderBytes = spirvresult.SpirvBytes;
                    shaderInfo.Compute = computeShader;
                }
                if (result.Any(y=>y.Key =="geom") && result.First(y => y.Key == "geom").Any(y => y.Item1.Contains(x)))
                {
                    var geometryShader = new ShaderDescription(ShaderStages.Geometry, LoadShaderData(x + ".geom"), "main");
                    spirvresult = SpirvCompilation.CompileGlslToSpirv(System.Text.Encoding.UTF8.GetString(LoadShaderData(x + ".geom")), "", ShaderStages.Geometry, new GlslCompileOptions()
                    {
                        Debug = false,
                    });
                    geometryShader.ShaderBytes = spirvresult.SpirvBytes;
                    shaderInfo.Compute = geometryShader;
                }
                AllGLSL[x] = shaderInfo;
            });
            first = false;
        }
        static GLSLManger()
        {

        }
        internal static byte[] LoadShader(string shadername)
        {
            if (string.IsNullOrEmpty(shadername)) throw new ArgumentNullException(nameof(shadername));
            string path = typeof(IVeldridContent).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(shadername));
            if (string.IsNullOrEmpty(path)) throw new KeyNotFoundException(nameof(shadername));
            return LoadShaderData(path);
        }
        static byte[] LoadShaderData(string respath)
        {
            using (var stream =typeof(IVeldridContent).Assembly.GetManifestResourceStream(respath))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public static GLSLManger Default { get; } = new GLSLManger();
    }
}
