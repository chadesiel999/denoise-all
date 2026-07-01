using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public enum ProductType
    {
        Base,
        B21_JinHui_PXI,

        /// <summary>
        /// 吉赫MSO7000X：1G带宽、10GSPS、
        /// </summary>
        JiHe_MSO7000X,
        /// <summary>
        /// 吉赫MSO7000A：1G带宽、10GSPS、
        /// </summary>
        JiHe_MSO7000A,
		/// <summary>
		/// 吉赫MSO8000HD
		/// </summary>
		JiHe_MSO8000HD,

        /// <summary>
        /// 吉赫MSO8000X：5G带宽、20GSPS、
        /// </summary>
        JiHe_MSO8000X,

        /// <summary>
        /// 吉赫7000L
        /// </summary>
        JiHe_UPO7000L,
        
        /// <summary>
        /// 吉赫MSO7000HD(JiHe_MSO7000HD-5G的变型，软硬件无本质上的重构)
        /// </summary>
        JiHe_MSO7000HD,

		/// <summary>
		/// 8G：带宽、4通道、40GSPS、512Mpts/通道、100万幅捕获率、50GSPS随机采用、LA无
		/// </summary>
		B21_HB8G,

        /// <summary>
        /// 高清晰：4GHz带宽、4通道、2通道模式20GSPS/4通道10GSPS、12bit、双通道100Mpts/四通道50MSPS、65万幅捕获率、重复采用率125GSPS、LA无
        /// </summary>
        /// 
        B21_HD4G,

        /// <summary>
        /// DBI20G：20G带宽、4通道、2通道模式80GSPS/4通道40GSPS、2Gpts存储深度、45万幅捕获率、LA（3GHz带宽、16通道、10GSPS[16通道]/20GSPS[8通道],1Gpts）
        /// </summary>
        B21_DBI20G,

        /// <summary>
        /// DBI16G：16G带宽、2通道、1通道模式80GSPS/2通道40GSPS、2Gpts存储深度、40万幅捕获率、LA-无）
        /// </summary>
        B21_DBI16G,

        /// <summary>
        ///多域：8G带宽、4通道、40GSPS、1Gpts存储深度、65万幅捕获率、LA（500MHz带宽、16通道、5GSPS、125Mpts）、时频域/频域/调制域/相位域/统计域
        /// </summary>
        /// 
        B21_MD8G,

        /// <summary>
        /// 高分辨率：1G带宽、4通道、10GSPS、1Gpts存储深度/每通道、100万幅捕获率、LA（250MHz带宽、16通道、1.25GSPS、125Mpts）、重复采用率125GSPS
        /// </summary>
        B21_HR1G,

        /// <summary>
        /// MSO2G6通道：2G带宽、6通道、6.25GSPS、1Gpts存储深度、60万幅捕获率、LA（500MHz带宽、48通道）、8位触发频率计数器
        /// </summary>
        B21_MS2G,

        ForTest,

        /// <summary>
        /// 九院8G项目，40GSPS，8G带宽
        /// </summary>
        B24_MSO8G,

        /// <summary>
        /// 智能项目，DBI架构，80GSPS，20G带宽
        /// </summary>
        B24_AI20G,

        /// <summary>
        /// 智能项目，瞬态，20GSPS，8G带宽
        /// </summary>
        B24_AI8G,

        B24_9Y8G,
        B24_ReCfg8G,
        B24_ST8G,
        B23_USB,
        B23_DBI13G,
        B24_XunXin40G,
        B25_XunXin40GDBI,
    }
}
