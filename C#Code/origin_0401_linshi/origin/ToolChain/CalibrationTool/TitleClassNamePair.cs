using System;
using System.Text.Json.Serialization;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class TitleClassNamePair
    {
        [JsonPropertyName("Title")]
        public string Title
        {
            get;
            set;
        } = "";
        public string UsedFor
        {
            get;
            set;
        } = "";
        [JsonPropertyName("ClassName")]
        public string ClassName
        {
            get;
            set;
        } = "";
        [JsonPropertyName("TipMessage")]
        public string TipMessage
        {
            get;
            set;
        } = "";
        [JsonPropertyName("Description")]
        public string Description
        {
            get;
            set;
        } = "";
        [JsonPropertyName("Parameters")]
        public string Parameters
        {
            get;
            set;
        } = "";
    }
}
