using System;
using System.IO;
using System.Text;
using ScopeX.Core;
using ScopeX.SCPIManager.WebSocket;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= Web功能所需方法 ==============================================================================================
        public static bool SaveWaveDataFile(WaveSaveRequest request)
        {
            var filePrsnt = Presenter.File;
            if (request.ChannelID < 0 || request.ChannelID > 3)
            {
                request.ChannelID = 0;
            }
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "U2 DSO\\WebServerFiles");
            filePrsnt.WfmSource = ComModel.ChannelId.C1 + request.ChannelID;
            filePrsnt.WfmFormat = (WfmFormat)request.Format;
            filePrsnt.WfmPath = path;
            filePrsnt.FileName = replaceFileName(request.FileName);
            var result = FilePrsnt.SaveWaveform(filePrsnt.WfmPath, filePrsnt.FileName, filePrsnt.WfmFormat, filePrsnt.WfmSource, filePrsnt.IfAppendDatetime, IsCheckSameName: false);
            return result;
        }
        /// <summary> 
        /// 过滤文件名非法字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string replaceFileName(string strFile)
        {
            StringBuilder rBuilder = new StringBuilder(strFile);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);

            return rBuilder.ToString().Trim();
        }
    }
}
