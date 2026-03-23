// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.CmdPal.Common.Services;
using Microsoft.CmdPal.UI.Helpers;
using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.CmdPal.UI.Settings;

public sealed partial class GeneralPage : Page
{
    private readonly TaskScheduler _mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    private readonly SettingsViewModel? viewModel;
    private readonly IApplicationInfoService _appInfoService;

#pragma warning disable CS0618 // Obsolete — XAML compatibility shim
    public GeneralPage()
        : this(
            App.Current.Services.GetService<TopLevelCommandManager>()!,
            App.Current.Services.GetService<IThemeService>()!,
            App.Current.Services.GetRequiredService<ISettingsService>(),
            App.Current.Services.GetRequiredService<IApplicationInfoService>())
    {
    }
#pragma warning restore CS0618

    public GeneralPage(
        TopLevelCommandManager topLevelCommandManager,
        IThemeService themeService,
        ISettingsService settingsService,
        IApplicationInfoService appInfoService)
    {
        _appInfoService = appInfoService;
        this.InitializeComponent();
        viewModel = new SettingsViewModel(topLevelCommandManager, _mainTaskScheduler, themeService, settingsService);
    }

    public string ApplicationVersion
    {
        get
        {
            var versionNo = ResourceLoaderInstance.GetString("Settings_GeneralPage_VersionNo");
            var version = _appInfoService.AppVersion;
            return string.Format(CultureInfo.CurrentCulture, versionNo, version);
        }
    }
}
