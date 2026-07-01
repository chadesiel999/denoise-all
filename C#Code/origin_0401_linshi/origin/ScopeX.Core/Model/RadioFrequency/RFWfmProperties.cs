using System;

namespace ScopeX.Core
{
    public class RFWfmProperties: WfmProperties
    {
        private const String _VER = "U2Core220314";

        public RFWfmProperties(String name):base(name)
        {
           
        }
        public Double ReferenceLevel
        {
            get;
            init;
        }


        public Double FFTLength
        {
            get;
            init;
        }

        public Double SampleRate
        {
            get;
            init;
        }
        
        public Double RBW
        {
            get;
            init;
        }
        
        public Int64 StartFrequency
        {
            get;
            init;
        }
        public Int64 CenterFrequency
        {
            get;
            init;
        }
        public Int64 EndFrequency
        {
            get;
            init;
        }
        public Int64 Span
        {
            get;
            init;
        }


    }

}
