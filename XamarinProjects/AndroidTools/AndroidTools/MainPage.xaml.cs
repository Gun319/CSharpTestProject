using AndroidTools.Contracts;
using System;
using Xamarin.Forms;

namespace AndroidTools
{
    public partial class MainPage : ContentPage
    {
        #region 属性注入
        /// <summary>
        /// 自动开关机
        /// </summary>
        private IAutoOnOff _autoOnOff => DependencyService.Resolve<IAutoOnOff>();

        /// <summary>
        /// APP安装、检测
        /// </summary>
        private IInstall _install => DependencyService.Resolve<IInstall>();
        #endregion

        public MainPage() => InitializeComponent();

        /// <summary>
        /// 执行开关机计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSetUp_Clicked(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            long onDate = Tools.TimeUtils.CurrentTimewillis(DateTime.Parse($"{date} {OnTime.Time}").ToUniversalTime());
            long offDate = Tools.TimeUtils.CurrentTimewillis(DateTime.Parse($"{date} {OffTime.Time}").ToUniversalTime());

            bool isPowerOnPlan = _autoOnOff.PowerOnPlan(true, onDate);
            bool isPowerOffPlan = _autoOnOff.PowerOffPlan(true, offDate);

            if (isPowerOnPlan && isPowerOffPlan)
                await DisplayAlert("消息", "自动开关机计划设置成功", "确定");
        }

        /// <summary>
        /// 执行APP安装计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSilentInstall_Clicked(object sender, EventArgs e)
        {
            string apkPath = edAPKPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(apkPath) && !apkPath.Contains(".apk"))
            {
                await DisplayAlert("消息", "请检查安装路径及文件名", "确定");
                return;
            }

            if (_install.SilentInstall(apkPath))
                await DisplayAlert("消息", "APP安装成功", "确定");
        }

        /// <summary>
        /// 检查APP是否已安装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCheckInstalled_Clicked(object sender, EventArgs e)
        {
            string packageName = edPackageName.Text.Trim();

            if (string.IsNullOrWhiteSpace(packageName))
            {
                await DisplayAlert("消息", "程序包名不能为空", "确定");
                return;
            }

            if (_install.CheckAppInstalled(packageName))
                await DisplayAlert("消息", $"{packageName} 已安装", "确定");
        }

        /// <summary>
        /// 卸载APP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnUnInstalled_Clicked(object sender, EventArgs e)
        {
            string packageName = edPackageName.Text.Trim();

            if (string.IsNullOrWhiteSpace(packageName))
            {
                await DisplayAlert("消息", "程序包名不能为空", "确定");
                return;
            }

            if (_install.UnInstall(packageName))
                await DisplayAlert("消息", $"{packageName} 已卸载", "确定");
        }
    }
}
