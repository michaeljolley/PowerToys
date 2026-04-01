// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Microsoft.CmdPal.UI.Controls;

public sealed partial class FallbackRankerDialog : UserControl
{
    private FallbackRanker FallbackRanker;

    public FallbackRankerDialog(TopLevelCommandManager topLevelCommandManager, IThemeService themeService, ISettingsService settingsService)
    {
        InitializeComponent();

        FallbackRanker = new FallbackRanker(topLevelCommandManager, themeService, settingsService);
        FallbackRankerHost.Content = FallbackRanker;
    }

    public IAsyncOperation<ContentDialogResult> ShowAsync()
    {
        return FallbackRankerContentDialog!.ShowAsync()!;
    }
}
