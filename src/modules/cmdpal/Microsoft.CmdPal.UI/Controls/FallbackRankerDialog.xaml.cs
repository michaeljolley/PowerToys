// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Microsoft.CmdPal.UI.Controls;

public sealed partial class FallbackRankerDialog : UserControl
{
    public FallbackRankerDialog()
    {
        InitializeComponent();
    }

    public void InitializeDependencies(TopLevelCommandManager topLevelCommandManager, IThemeService themeService, ISettingsService settingsService)
    {
        var fallbackRanker = new FallbackRanker(topLevelCommandManager, themeService, settingsService);
        FallbackRankerHost.Content = fallbackRanker;
    }

    public IAsyncOperation<ContentDialogResult> ShowAsync()
    {
        return FallbackRankerContentDialog!.ShowAsync()!;
    }
}
