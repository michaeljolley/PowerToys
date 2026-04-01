// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Controls;

namespace Microsoft.CmdPal.UI.Services;

/// <summary>
/// Factory for creating XAML pages with dependency injection.
/// Use this instead of Frame.Navigate to create pages with constructor-injected dependencies.
/// </summary>
public interface IPageFactory
{
    /// <summary>
    /// Creates a page instance of the specified type with all dependencies resolved from the DI container.
    /// </summary>
    T Create<T>()
        where T : Page;

    /// <summary>
    /// Creates a page instance of the specified type with all dependencies resolved from the DI container.
    /// </summary>
    Page Create(Type pageType);
}
