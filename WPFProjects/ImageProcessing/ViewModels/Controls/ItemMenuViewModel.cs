using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageProcessing.Views.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.ViewModels.Controls
{
    public partial class ItemMenuViewModel : ViewModelBase
    {
        /// <summary>
        /// 菜单项
        /// </summary>
        [ObservableProperty]
        public List<MenuItem>? _itemMenu;

        public ItemMenuViewModel()
        {
            Init();
        }

        void Init() => ItemMenu = new()
            {
                new MenuItem{ MenuName = "OpenCv视频合成", View = App.Current.Services.GetService<ImageSynthesisVideoView>() },
                new MenuItem{ MenuName = "OpenCv模板匹配", View = App.Current.Services.GetService<MatchTemplateView>() }
            };

        [RelayCommand]
        public static Task Navigation(ListBox parameter)
        {
            if (parameter.SelectedItem is not null)
            {
                var item = (parameter.SelectedItem as MenuItem)!;
                var shell = App.Current.Services.GetService<ShellViewModel>()!;

                shell.MenuAction = item.MenuName;
                shell.CurrentViewModel = item!.View;
            }
            return Task.CompletedTask;
        }


        public class MenuItem
        {
            public string? MenuName { get; set; }
            public FrameworkElement? View { get; set; }
        }
    }
}
