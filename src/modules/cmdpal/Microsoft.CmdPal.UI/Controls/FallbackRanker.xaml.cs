// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.CmdPal.UI.Controls;

public sealed partial class FallbackRanker : UserControl
{
    private readonly TaskScheduler _mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private SettingsViewModel? viewModel;

    // XAML compatibility shim — will be removed when parents use DI constructor
#pragma warning disable CS0618 // Obsolete — XAML compatibility shim
    public FallbackRanker()
        : this(
            App.Current.Services.GetService<TopLevelCommandManager>()!,
            App.Current.Services.GetService<IThemeService>()!,
            App.Current.Services.GetRequiredService<ISettingsService>())
    {
    }
#pragma warning restore CS0618

    public FallbackRanker(TopLevelCommandManager topLevelCommandManager, IThemeService themeService, ISettingsService settingsService)
    {
        this.InitializeComponent();
        viewModel = new SettingsViewModel(topLevelCommandManager, _mainTaskScheduler, themeService, settingsService);
    }

    private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        viewModel?.ApplyFallbackSort();
    }
}
