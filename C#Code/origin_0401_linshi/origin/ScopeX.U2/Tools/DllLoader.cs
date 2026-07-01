using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ScopeX.U2
{
    public class DllLoader<T>
    {
        public static List<Protocol.DecodeView> LoadDecodeView()
        {
            String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Decode");

            if (!Directory.Exists(path))
                return new();

            List<Protocol.DecodeView> decodeviews = new();

            decodeviews.AddRange(Directory.GetFiles(path, "ScopeX.Protocol.*.dll", SearchOption.AllDirectories)
                .SelectMany(x => new DllLoader<Protocol.DecodeView>(x).Drivers).OrderBy(x => x.ProtocolType));

            return decodeviews;
        }

        public static List<ScpiAdapter.ScpiAdapter> LoadAdapter()
        {
            String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScpiAdapter");

            if (!Directory.Exists(path))
                return new();

            List<ScpiAdapter.ScpiAdapter> adpaters = new();

            adpaters.AddRange(Directory.GetFiles(path, "ScopeX.ScpiAdapter.*.dll", SearchOption.AllDirectories)
                .SelectMany(x => new DllLoader<ScpiAdapter.ScpiAdapter>(x).Drivers).OrderBy(x => x.Manufacturer));

            return adpaters;
        }

        public DllLoader(String filepath) => LoadAllDrive(filepath);
        public void LoadAllDrive(String filePath)
        {
            AssemblyLoadContext.Default.ResolvingUnmanagedDll += Default_ResolvingUnmanagedDll;
            AssemblyLoadContext.Default.Resolving += Default_Resolving;
            DllPath = filePath;

            lock (DllPath)
            {
                try
                {
                    IEnumerable<Type> types = AssemblyLoadContext.Default.LoadFromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(filePath))).GetTypes().Where(x => x.IsClass);//将文件加载到内存中，再从内存中加载，源文件在加载后可以删除
                                                                                                                                                                                             //var types = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath).GetTypes();//从当前路径中加载，加载后当前文件仍然被占用

                    if (types == null)
                        return;

                    foreach (Type item in types)
                    {
                        //if (item.GetInterface(typeof(T).FullName) != null && !item.IsAbstract)
                        if (item.BaseType.FullName == typeof(T).FullName && !item.IsAbstract)
                            Drivers.Add((T)Activator.CreateInstance(item, true));
                    }
                }
                catch /*(Exception ex)*/
                {
                }
            }

            AssemblyLoadContext.Default.ResolvingUnmanagedDll -= Default_ResolvingUnmanagedDll;
            AssemblyLoadContext.Default.Resolving -= Default_Resolving;
        }
        /// <summary>
        /// 加载c++依赖库
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        private IntPtr Default_ResolvingUnmanagedDll(Assembly arg1, String arg2)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 加载.Net依赖库
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        private Assembly Default_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            AssemblyDependencyResolver dependencyResolver = new AssemblyDependencyResolver(Assembly.GetExecutingAssembly().Location);
            String assemplypath = dependencyResolver.ResolveAssemblyToPath(arg2);

            if (String.IsNullOrEmpty(assemplypath))
            {
                dependencyResolver = new AssemblyDependencyResolver(DllPath);
                assemplypath = dependencyResolver.ResolveAssemblyToPath(arg2);
            }

            if (String.IsNullOrEmpty(assemplypath))
            {
                String[] dlls = Directory.GetFiles(new FileInfo(DllPath).DirectoryName, "*.dll");

                foreach (String item in dlls)
                {
                    try
                    {
                        Assembly ass = arg1.LoadFromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(item)));//将文件加载到内存中，再从内存中加载，源文件在加载后可以删除
                                                                                                                          //var ass = arg1.LoadFromAssemblyPath(item);//从当前路径中加载，加载后当前文件仍然被占用

                        if (ass.FullName == arg2.FullName)
                        {
                            assemplypath = item;
                            break;
                        }
                    }
                    catch { }
                }
            }

            if (!String.IsNullOrEmpty(assemplypath))
                return arg1.LoadFromStream(new MemoryStream(System.IO.File.ReadAllBytes(assemplypath)));//将文件加载到内存中，再从内存中加载，源文件在加载后可以删除
                                                                                                        //return arg1.LoadFromAssemblyPath(assemplypath);//从当前路径中加载，加载后当前文件仍然被占用
            else
                return arg1.Assemblies.FirstOrDefault(x => x.FullName == arg2.FullName);
        }

        public String DllPath { get; private set; }
        public List<T> Drivers { get; private set; } = new List<T>();

        public void UnLoad()
        {
            try
            {
                AssemblyLoadContext.Default.Unload();
            }
            catch
            {

            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
