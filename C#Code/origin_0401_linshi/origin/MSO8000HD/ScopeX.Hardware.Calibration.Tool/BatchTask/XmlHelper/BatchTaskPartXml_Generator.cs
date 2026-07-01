using ScopeX.Hardware.Calibration.Tool.Base;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    public  static class BatchTaskPartXml_Generator
    {
        //具体生成方法的委托
        private delegate void GenerateAct(string pType,string cType, JToken cail);

        /// <summary>
        /// 指定产品类型和校准类型，生成校准文件
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="caliType"></param>
        public static void Generate(ProductType productType, CaliType caliType)
        {
            Generate((pType, cType, cail) =>
            {
                if (productType.ToString() == pType && caliType.ToString() == cType)
                    CaliGenerate(caliType, cail);
            });
        }

        /// <summary>
        /// 生成配置文件(GeneratorConfig.json)里面包含的所有校准项
        /// </summary>
        public static void GenerateAll()
        {
            Generate((pType, cType, cail) => 
            {
                Enum.TryParse(typeof(CaliType), cType, true, out object? caliType);
                if (caliType != null && caliType is CaliType)
                    CaliGenerate((CaliType)caliType, cail);
            });
        }

        private static void Generate(GenerateAct generateAct)
        {
            string configFile = @"Resources\BatchTaskPartXmlHelperJson\GeneratorConfig.json";
            if (!File.Exists(configFile))
                return;
            try
            {
                using (StreamReader sr = new StreamReader(configFile))
                {
                    var json = JObject.Parse(sr.ReadToEnd());
                    foreach (var item in json.GetVal("GeneratorConfig")!)
                    {
                        string pType = (string)item.GetVal("ProductType")!;
                        foreach (var cail in item.GetVal("Calis")!)
                        {
                            string cType = (string)cail.GetVal("CaliType")!;
                            generateAct(pType, cType, cail);
                        }
                    }
                }
            }
            catch(KeyNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Generate(BatchTaskPartXml_Generator):");
            }
        }

        /// <summary>
        /// 具体的生成方法
        /// </summary>
        private static void CaliGenerate(CaliType caliType, JToken json)
        {
            BatchTaskPartXml_Cali cali;
            switch (caliType)
            {
                case CaliType.AllType:
                    cali = new BatchTaskPartXml_CaliAllType() {SignalJson = InitJson(json.GetVal("SignalPath")!.ToObject<string[]>())};
                    break;
                case CaliType.Baseline:
                    cali = new BatchTaskPartXml_CaliBaseline();
                    break;
                case CaliType.Gain:
                    cali = new BatchTaskPartXml_CaliGain() { SignalJson = InitJson(json.GetVal("SignalPath")!.ToObject<string[]>())};
                    break;
                case CaliType.TiAdc:
                    cali = new BatchTaskPartXml_CaliTiAdc() { SignalJson = InitJson(json.GetVal("SignalPath")!.ToObject<string[]>())};
                    break;
                case CaliType.Offset:
                    cali = new BatchTaskPartXml_CaliOffset() { SignalJson = InitJson(json.GetVal("SignalPath")!.ToObject<string[]>())};
                    break;
                default:
                    throw new ArgumentException("caliType");
            }
            cali.ParamJson = InitJson(json.GetVal("ParamPath")!.ToObject<string[]>());
            cali.OscilloscopeJson = InitJson(json.GetVal("OscilloscopePath")!.ToObject<string[]>());
            cali.GenerateXml();
        }

        /// <summary>
        /// 初始化Json对象
        /// </summary>
        /// <param name="signalScpiPaths"></param>
        /// <returns></returns>
        private static JObject InitJson(string[]? signalScpiPaths)
        {
            if (signalScpiPaths == null)
                return null;

            JObject retJObj = new JObject();
            foreach(string scp in signalScpiPaths)
            {
                if (!string.IsNullOrEmpty(scp))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(scp))
                        {
                            string jsonText = sr.ReadToEnd();
                            retJObj = MergeJObjs(retJObj, JObject.Parse(jsonText));
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return retJObj;
        }

        /// <summary>
        /// 合并两个JObject
        /// </summary>
        private static JObject MergeJObjs(JObject obj1, JObject obj2)
        {
            JObject mergedObject = new JObject();

            // 合并属性
            foreach (var property in obj1.Properties())
            {
                mergedObject[property.Name] = property.Value;
            }

            foreach (var property in obj2.Properties())
            {
                //属性存在且值类型为JTokenType.Object;其他则属性替换或添加
                if (mergedObject[property.Name] != null && mergedObject[property.Name].Type == JTokenType.Object)
                {
                    mergedObject[property.Name] = MergeJObjs((JObject)mergedObject[property.Name], (JObject)property.Value);
                }
                else
                {
                    mergedObject[property.Name] = property.Value;
                }
            }

            return mergedObject;
        }

        public static JToken? GetVal(this JToken jToken, string propertyName)
        {
            JToken? ret = jToken[propertyName];
            if (ret == null)
                throw new KeyNotFoundException($"\"{propertyName}\" property not exist!");

            return ret;
        }

    }
}
