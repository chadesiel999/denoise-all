using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class ONNXParam
    {
        public string InputName = "";
        public string[] DetectType = new string[0];
        public string[] MatchType = new string[0];
        public String Path = "";
        public Int32 DataLength = 0;
        public Int32 InputCount = 0;
        public DenseTensor<float> DenseTensor = new (new[] { 0, 0, 0 });
    }
}
