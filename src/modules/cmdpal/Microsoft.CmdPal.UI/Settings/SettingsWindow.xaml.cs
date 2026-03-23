// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using ManagedCommon;
using Microsoft.CmdPal.UI.Helpers;
using Microsoft.CmdPal.UI.Messages;
using Microsoft.CmdPal.UI.ViewModels;
using Microsoft.CmdPal.UI.ViewModels.Messages;
using Microsoft.CmdPal.UI.ViewModels.Services;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using WinUIEx;
using RS_ = Microsoft.CmdPal.UI.Helpers.ResourceLoaderInstance;
using TitleBar = Microsoft.UI.Xaml.Controls.TitleBar;

namespace Microsoft.CmdPal.UI.Settings;

public sealed partial class SettingsWindow : WindowEx,
    IDisposable,
    IRecipient<NavigateToExtensionSettingsMessage>,
    IRecipient<QuitMessage>
{
    private readonly LocalKeyboardListener _localKeyboardListener;

    private readonly NavigationViewItem? _internalNavItem;

    private readonly Dictionary<string, Page> _pages;

    private readonly TopLevelCommandManager _topLevelCommandManager;

    private readonly IThemeService _themeService;

    private readonly ISettingsService _settingsService;

    private string? _previousNavigationTag;

    public ObservableCollection<Crumb> BreadCrumbs { get; } = [];

    // Gets or sets optional action invoked after NavigationView is loaded.
    public Action? NavigationViewLoaded { get; set; }

    internal SettingsWindow(
        GeneralPage generalPage,
        AppearancePage appearancePage,
        ExtensionsPage extensionsPage,
        DockSettingsPage dockSettingsPage,
        InternalPage internalPage,
        TopLevelCommandManager topLevelCommandManager,
        IThemeService themeService,
        ISettingsService settingsService)
    {
        _topLevelCommandManager = topLevelCommandManager;
        _themeService = themeService;
        _settingsService = settingsService;

        _pages = new Dictionary<string, Page>
        {
            ["General"] = generalPage,
            ["Appearance"] = appearancePage,
            ["Extensions"] = extensionsPage,
            ["Dock"] = dockSettingsPage,
            ["Internal"] = internalPage,
        };

        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.SetIcon();
        var title = RS_.GetString("SettingsWindowTitle");
        this.AppWindow.Title = title;
        this.AppTitleBar.Title = title;
        PositionCentered();

        WeakReferenceMessenger.Default.Register<NavigateToExtensionSettingsMessage>(this);
        WeakReferenceMessenger.Default.Register<QuitMessage>(this);

        _localKeyboardListener = new LocalKeyboardListener();
        _localKeyboardListener.KeyPressed += LocalKeyboardListener_OnKeyPressed;
        _localKeyboardListener.Start();
        Closed += SettingsWindow_Closed;
        RootElement.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(RootElement_OnPointerPressed), true);

        if (!BuildInfo.IsCiBuild)
        {
            _internalNavItem = new NavigationViewItem
            {
                Content = "Internal Tools",
                Icon = new FontIcon { Glyph = "\uEC7A" },
                Tag = "Internal",
            };
            NavView.MenuItems.Add(_internalNavItem);
        }
        else
        {
            _internalNavItem = null;
        }

        Navigate("General");
    }

    private void SettingsWindow_Closed(object sender, WindowEventArgs args)
    {
        Dispose();
    }

    // Handles NavigationView loaded event.
    // Sets up initial navigation and accessibility notifications.
    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Delay necessary to ensure NavigationView visual state can match navigation
        Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());

        if (sender is NavigationView navigationView)
        {
            // Register for pane open/close changes to announce to screen readers
            navigationView.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, AnnounceNavigationPaneStateChanged);
        }
    }

    // Announces navigation pane open/close state to screen readers for accessibility.
    private void AnnounceNavigationPaneStateChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is NavigationView navigationView)
        {
            UIHelper.AnnounceActionForAccessibility(
            ue: (UIElement)sender,
            (sender as NavigationView)?.IsPaneOpen == true ? RS_.GetString("NavigationPaneOpened") : RS_.GetString("NavigationPaneClosed"),
            "NavigationViewPaneIsOpenChangeNotificationId");
        }
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var selectedItem = args.InvokedItemContainer;
        Navigate((selectedItem.Tag as string)!);
    }

    internal void Navigate(string page)
    {
        if (page == string.Empty)
        {
            // intentional no-op: empty tag means no navigation
            return;
        }

        if (!_pages.TryGetValue(page, out var targetPage))
        {
            Logger.LogError($"Unknown settings page tag '{page}'");
            return;
        }

        _previousNavigationTag = null;
        NavFrame.Content = targetPage;
        AppTitleBar.IsBackButtonVisible = false;

        // Select the correct menu item
        foreach (var obj in NavView.MenuItems)
        {
            if (obj is NavigationViewItem item && item.Tag is string s && s == page)
            {
                NavView.SelectedItem = item;
                break;
            }
        }

        // Update breadcrumbs
        BreadCrumbs.Clear();
        var breadcrumbTitle = page switch
        {
            "General" => RS_.GetString("Settings_PageTitles_GeneralPage"),
            "Appearance" => RS_.GetString("Settings_PageTitles_AppearancePage"),
            "Extensions" => RS_.GetString("Settings_PageTitles_ExtensionsPage"),
            "Dock" => RS_.GetString("Settings_PageTitles_DockPage"),
            "Internal" => "Internal",
            _ => $"[{page}]",
        };
        BreadCrumbs.Add(new(breadcrumbTitle, page));
    }

    private void Navigate(ProviderSettingsViewModel extension)
    {
        _previousNavigationTag = "Extensions";
        var extensionPage = new ExtensionPage(_topLevelCommandManager, _themeService, _settingsService) { ViewModel = extension };
        NavFrame.Content = extensionPage;
        AppTitleBar.IsBackButtonVisible = true;

        NavView.SelectedItem = ExtensionPageNavItem;

        BreadCrumbs.Clear();
        var extensionsPageType = RS_.GetString("Settings_PageTitles_ExtensionsPage");
        BreadCrumbs.Add(new(extensionsPageType, "Extensions"));
        BreadCrumbs.Add(new(extension.DisplayName, extension));
    }

    private void PositionCentered()
    {
        var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest);
        if (displayArea is not null)
        {
            var centeredPosition = AppWindow.Position;
            centeredPosition.X = (displayArea.WorkArea.Width - AppWindow.Size.Width) / 2;
            centeredPosition.Y = (displayArea.WorkArea.Height - AppWindow.Size.Height) / 2;
            AppWindow.Move(centeredPosition);
        }
    }

    public void Receive(NavigateToExtensionSettingsMessage message) => Navigate(message.ProviderSettingsVM);

    private void NavigationBreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Item is Crumb crumb)
        {
            if (crumb.Data is string data)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    Navigate(data);
                }
            }
        }
    }

    private void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
    {
        WeakReferenceMessenger.Default.Send<Microsoft.UI.Xaml.WindowActivatedEventArgs>(args);
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        WeakReferenceMessenger.Default.Send<SettingsWindowClosedMessage>();

        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (args.DisplayMode is NavigationViewDisplayMode.Compact or NavigationViewDisplayMode.Minimal)
        {
            AppTitleBar.IsPaneToggleButtonVisible = true;
        }
        else
        {
            AppTitleBar.IsPaneToggleButtonVisible = false;
        }
    }

    public void Receive(QuitMessage message)
    {
        // This might come in on a background thread
        DispatcherQueue.TryEnqueue(() => Close());
    }

    private void AppTitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TryGoBack()
    {
        if (_previousNavigationTag != null)
        {
            Navigate(_previousNavigationTag);
        }
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        TryGoBack();
    }

    private void LocalKeyboardListener_OnKeyPressed(object? sender, LocalKeyboardListenerKeyPressedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.GoBack:
            case VirtualKey.XButton1:
                TryGoBack();
                break;

            case VirtualKey.Left:
                if (KeyModifiers.GetCurrent().Alt)
                {
                    TryGoBack();
                }

                break;
        }
    }

    private void RootElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        try
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var ptrPt = e.GetCurrentPoint(RootElement);
                if (ptrPt.Properties.IsXButton1Pressed)
                {
                    TryGoBack();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Error handling mouse button press event", ex);
        }
    }

    public void Dispose()
    {
        _localKeyboardListener?.Dispose();
    }
}

public readonly struct Crumb
{
    public Crumb(string label, object data)
    {
        Label = label;
        Data = data;
    }

    public string Label { get; }

    public object Data { get; }

    public override string ToString() => Label;
}
