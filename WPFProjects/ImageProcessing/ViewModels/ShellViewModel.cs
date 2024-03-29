﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace ImageProcessing.ViewModels
{
    public partial class ShellViewModel : ViewModelBase
    {
        [ObservableProperty]
        private UIElement? _currentViewModel;

        [ObservableProperty]
        private UIElement _itemMenuView;

        [ObservableProperty]
        private string? _menuAction;

        private readonly Window _window;
        public ShellViewModel()
        {
            ItemMenuView = App.Current.Services.GetService<Views.Controls.ItemMenuView>()!;

            _window = Application.Current.MainWindow;
        }

        [RelayCommand]
        private void CloseWindow()
        {
            SystemCommands.CloseWindow(_window);

            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void MinimizeWindow() => SystemCommands.MinimizeWindow(_window);

        [RelayCommand]
        private void MaximizeWindow() => SystemCommands.MaximizeWindow(_window);

        [RelayCommand]
        private void RestoreWindow() => SystemCommands.RestoreWindow(_window);

        [RelayCommand]
        private void MouseLeftSystemMenu()
        {
            var pointing = _window.PointToScreen(new Point(0, 0));

            if (_window.WindowState is WindowState.Maximized)
                pointing.X += 10;
            else
                pointing.X += 2;

            pointing.Y += SystemParameters.WindowNonClientFrameThickness.Top + 1;

            SystemCommands.ShowSystemMenu(_window, pointing);
        }

        [RelayCommand]
        private void MouseRightSystemMenu(MouseEventArgs e)
        {
            FrameworkElement? element = e.OriginalSource as FrameworkElement;

            var pointing = _window.PointToScreen(Mouse.GetPosition(element));
            pointing.X += 5;
            pointing.Y += 5;
            SystemCommands.ShowSystemMenu(_window, pointing);
        }
    }
}
