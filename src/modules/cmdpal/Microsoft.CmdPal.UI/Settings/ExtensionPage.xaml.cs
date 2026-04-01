// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.UI.Controls;
using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Microsoft.CmdPal.UI.Settings;

public sealed partial class ExtensionPage : Page
{
    private readonly TaskScheduler _mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    private FallbackRankerDialog? _fallbackRankerDialog;

    public ProviderSettingsViewModel? ViewModel { get; set; }

    public ExtensionPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is ExtensionPageNavParam navParam)
        {
            ViewModel = navParam.Extension;
            _fallbackRankerDialog = new FallbackRankerDialog(
                navParam.Context.TopLevelCommandManager,
                navParam.Context.ThemeService,
                navParam.Context.SettingsService);
            FallbackRankerDialogHost.Content = _fallbackRankerDialog;
            Bindings.Update();
        }
        else
        {
            throw new ArgumentException($"{nameof(ExtensionPage)} navigation args should be passed an {nameof(ExtensionPageNavParam)}");
        }
    }

    private async void RankButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_fallbackRankerDialog is not null)
        {
            await _fallbackRankerDialog.ShowAsync();
        }
    }
}
