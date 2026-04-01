// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.CmdPal.UI.Services;

/// <summary>
/// Creates XAML pages using the DI container.
/// This is one of only two classes (along with App) that holds a reference to <see cref="IServiceProvider"/>.
/// </summary>
internal sealed class PageFactory : IPageFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PageFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create<T>()
        where T : Page
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public Page Create(Type pageType)
    {
        return (Page)_serviceProvider.GetRequiredService(pageType);
    }
}
