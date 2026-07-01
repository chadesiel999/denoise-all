using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Updater.Base
{
#pragma warning disable SYSLIB0011

    [Serializable]
    public class UpdatePackage
    {
        private UpdatePackage()
        {
            Items = new List<UpdateItem>();
        }
        public HardwareVersionInfo PackageVersion { get; set; }
        public ExpandedData ExpandedInfo { get; set; } = new ExpandedDataNone();
        public List<UpdateItem> Items { get; set; }
        public bool Save(string targetPathFileName, ProductType productType, HardwareVersionInfo packageVersion, ExpandedData? expandedInfo = null)
        {
            if (File.Exists(targetPathFileName))
                File.Delete(targetPathFileName);
            _productType = productType;
            PackageVersion = packageVersion;
            Stream stream = File.Open(targetPathFileName, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, _default);
            stream.Close();
            return true;
        }
        private static UpdatePackage _default = new UpdatePackage();
        public static UpdatePackage Default
        {
            get => _default;
        }

        public ProductType ProductType
        {
            get => _productType;
        }
        private ProductType _productType;
        public static UpdatePackage Load(string fileName)
        {
            try
            {
                var stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
				var formatter = new BinaryFormatter();
                UpdatePackage result = formatter.Deserialize(stream) as UpdatePackage;
                stream.Close();
                return result;
            }
            catch (FileNotFoundException)
            {

            }
            catch (Exception)
            {

            }
            return null;
        }
    }

}
