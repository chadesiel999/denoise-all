using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    /// <summary>
    /// 上升时间映射
    /// </summary>
    public class RiseTimeMap
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public float StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public float EndTime { get; set; }
        /// <summary>
        /// 增长值
        /// </summary>
        public float Offset { get; set; }
    }

    /// <summary>
    /// 上升时间单例
    /// </summary>
    /// <Remark>更改人：彭博 创建日期：2023/12/07 16:27:00  原因：用于校准上升时间和下降时间 </Remark>
    public class RiseTimeMapSingle
    {
        private List<RiseTimeMap> Maps;

        private static RiseTimeMapSingle Instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        private RiseTimeMapSingle()
        {
            InitMaps();
        }

        private void InitMaps()
        {
            if (null == Maps)
            {
                Maps = new List<RiseTimeMap>();
            }
            Maps.Clear();
            string path = "./Resources/RiseTimeMap.json";
            if (File.Exists(path))
            {
                using (StreamReader sw = new StreamReader(path))
                {
                    string json = sw.ReadToEnd();
                    JArray jarr = JArray.Parse(json);
                    foreach (JObject jobj in jarr)
                    {
                        RiseTimeMap map = new RiseTimeMap()
                        {
                            StartTime = Convert.ToSingle(jobj["starttime"]),
                            EndTime = Convert.ToSingle(jobj["endtime"]),
                            Offset = Convert.ToSingle(jobj["offset"])
                        };
                        Maps.Add(map);
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取偏移值
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public float GetOffsetCount(float time)
        {
            InitMaps();
            if (Maps.Count == 0)
            {
                return 0;
            }
            else
            {
                RiseTimeMap map = Maps.Find(p => p.StartTime * 1000 <= time && time < p.EndTime * 1000);
                if (null == map)
                {
                    return 0;
                }
                else
                {
                    return map.Offset * 1000;
                }
            }

        }


        public static RiseTimeMapSingle GetInstance()
        {
            if (null != Instance)
            {
                return Instance;
            }
            else
            {
                return Instance = new RiseTimeMapSingle();
            }
        }

    }

}
