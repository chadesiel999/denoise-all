using System;

namespace ScopeX.Updater.Base
{
    [Serializable]
    public class UpdateItem
    {
        public UpdaterItemType Type { get; set; }
        public string BoardName { get; set; }
        public int TypeID { get; set; }
        public string TargetPathFileName { get; set; }
        public DateTime LastWriteDateTime { get; set; }
        public int TotalBytes { get; set; }
        public byte[] Content { get; set; }

        public BaseDataBlock BaseInfo { get; set; }
        public UpdateItem(int typeID, UpdaterItemType in_type, string targetPathFileName, DateTime lastWriteDateTime, byte[] content
            , BaseDataBlock baseInfo, string boardName)
        {
            BoardName = boardName;
            TypeID = typeID;
            Type = in_type;
            TargetPathFileName = targetPathFileName;
            LastWriteDateTime = lastWriteDateTime;
            Content = content;
            TotalBytes = Content.Length;
            BaseInfo = baseInfo;
        }
    }
}
