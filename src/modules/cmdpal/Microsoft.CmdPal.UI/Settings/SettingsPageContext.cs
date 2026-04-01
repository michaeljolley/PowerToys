// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.Common.Services;
using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Dock;
using Microsoft.CmdPal.UI.ViewModels.Services;

namespace Microsoft.CmdPal.UI.Settings;

internal sealed record SettingsPageContext(
    TopLevelCommandManager TopLevelCommandManager,
    IThemeService ThemeService,
    ISettingsService SettingsService,
    IApplicationInfoService ApplicationInfoService,
    DockViewModel DockViewModel);
