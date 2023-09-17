﻿using System.Configuration;
using System.Threading.Tasks;

namespace ImageProcessing.Operates
{
    /// <summary>
    /// ConfigOperate
    /// </summary>
    public class ConfigOperate
    {
        /// <summary>
        /// 帧率设置
        /// </summary>
        public static string FPSKey = "FPS";

        /// <summary>
        /// 素材路径配置项名称
        /// </summary>
        public static string OpenFilePathKey = "OpenFilePath";

        /// <summary>
        /// 视频导出保存位置
        /// </summary>
        public static string SaveFilePathKey = "SaveFilePath";

        /// <summary>
        /// 多语言
        /// </summary>
        public static string LanguageKey = "Language";

        /// <summary>
        /// 卡顿阈值
        /// </summary>
        public static string ThresholdKey = "Threshold";

        /// <summary>
        /// 设置配置项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetKeyValue(string key, string value)
        {
            Task.Run(() =>
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings[key].Value = value;
                configuration.Save();
                ConfigurationManager.RefreshSection("AppSettings");
            });
        }

        /// <summary>
        /// 获取配置项值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigKeyValue(string key)
        {
            string configVaule = ConfigurationManager.AppSettings[key]!;

            if (!string.IsNullOrWhiteSpace(configVaule))
                return configVaule;

            return string.Empty;
        }
    }
}
