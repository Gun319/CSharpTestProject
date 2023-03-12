﻿namespace AndroidTools.Contracts
{
    /// <summary>
    /// APP安装、检测
    /// </summary>
    public interface IInstall
    {
        /// <summary>
        /// 静默安装
        /// </summary>
        /// <param name="apkPath">安装包路径</param>
        /// <returns></returns>
        bool SilentInstall(string apkPath);

        /// <summary>
        /// 检查应用是否已安装
        /// </summary>
        /// <param name="packageName">应用包名</param>
        /// <returns></returns>
        bool CheckAppInstalled(string packageName);
    }
}
