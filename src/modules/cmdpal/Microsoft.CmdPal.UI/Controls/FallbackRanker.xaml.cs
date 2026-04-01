// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.CmdPal.UI.Controls;

public sealed partial class FallbackRanker : UserControl
{
    private readonly TaskScheduler _mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private readonly TopLevelCommandManager _topLevelCommandManager;
    private readonly IThemeService _themeService;
    private readonly ISettingsService _settingsService;
    private SettingsViewModel? viewModel;

    public FallbackRanker(TopLevelCommandManager topLevelCommandManager, IThemeService themeService, ISettingsService settingsService)
    {
        _topLevelCommandManager = topLevelCommandManager;
        _themeService = themeService;
        _settingsService = settingsService;

        this.InitializeComponent();

        viewModel = new SettingsViewModel(_topLevelCommandManager, _mainTaskScheduler, _themeService, _settingsService);
    }

    private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        viewModel?.ApplyFallbackSort();
    }
}
