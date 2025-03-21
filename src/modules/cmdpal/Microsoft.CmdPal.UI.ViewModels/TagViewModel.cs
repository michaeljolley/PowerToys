﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.UI.ViewModels.Models;
using Microsoft.CommandPalette.Extensions;

namespace Microsoft.CmdPal.UI.ViewModels;

public partial class TagViewModel(ITag _tag, IPageContext context) : ExtensionObjectViewModel(context)
{
    private readonly ExtensionObject<ITag> _tagModel = new(_tag);

    // Remember - "observable" properties from the model (via PropChanged)
    // cannot be marked [ObservableProperty]
    public string Text { get; private set; } = string.Empty;

    public string ToolTip { get; private set; } = string.Empty;

    public OptionalColor Foreground { get; private set; }

    public OptionalColor Background { get; private set; }

    public IconInfoViewModel Icon { get; private set; } = new(null);

    public ExtensionObject<ICommand> Command { get; private set; } = new(null);

    public override void InitializeProperties()
    {
        var model = _tagModel.Unsafe;
        if (model == null)
        {
            return;
        }

        Text = model.Text;
        Foreground = model.Foreground;
        Background = model.Background;
        ToolTip = model.ToolTip;
        Icon = new(model.Icon);
        Icon.InitializeProperties();

        UpdateProperty(nameof(Text));
        UpdateProperty(nameof(Foreground));
        UpdateProperty(nameof(Background));
        UpdateProperty(nameof(ToolTip));
        UpdateProperty(nameof(Icon));
    }
}
