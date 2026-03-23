// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.CmdPal.UI.Settings;

public sealed partial class ExtensionPage : Page
{
    private readonly TaskScheduler _mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    public ProviderSettingsViewModel? ViewModel { get; internal set; }

    public ExtensionPage(
        TopLevelCommandManager topLevelCommandManager,
        IThemeService themeService,
        ISettingsService settingsService)
    {
        this.InitializeComponent();

        FallbackRankerDialog.InitializeDependencies(topLevelCommandManager, themeService, settingsService);
    }

    private async void RankButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await FallbackRankerDialog.ShowAsync();
    }
}
