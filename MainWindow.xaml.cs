using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Services;
using MyWinFormsApp.Views;

namespace MyWinFormsApp;

public partial class MainWindow : Window
{
    private readonly PosView _posView = new();
    private readonly InvoicesView _invoicesView = new();
    private readonly InventoryView _inventoryView = new();
    private readonly DataManagementView _dataManagementView = new();
    private ProfileView? _profileView;
    private readonly SettingsView _settingsView = new();
    private bool _isNavExpanded = true;

    private const double ExpandedWidth = 260;
    private const double CollapsedWidth = 68;

    public MainWindow()
    {
        InitializeComponent();
        _settingsView.DataContext = DataContext;
        ContentArea.Content = _posView;
        LoadUserInfo();

        // Monitor DB connection status
        DatabaseHelper.ConnectionStatusChanged += (connected) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (connected)
                {
                    DbStatusBadge.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DbStatusBadge.Background = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(239, 68, 68));
                    TxtDbStatus.Text = "Offline";
                    DbStatusBadge.Visibility = Visibility.Visible;
                }
            });
        };
    }

    private void LoadUserInfo()
    {
        if (Session.CurrentTenant != null)
            TxtTenantName.Text = Session.CurrentTenant.Name;

        if (Session.CurrentUser != null && Session.CurrentRole != null)
            TxtUserInfo.Text = $"{Session.CurrentUser.FullName} ({Session.CurrentRole.Name})";
        else if (Session.CurrentUser != null)
            TxtUserInfo.Text = Session.CurrentUser.FullName;

        // Show switch shop button if user has multiple shops
        BtnSwitchShop.Visibility = Session.HasMultipleShops ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Clear();
        var login = new LoginWindow();
        login.Show();
        Close();
    }

    private void BtnSwitchShop_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentUser == null) return;
        var picker = new ShopPickerWindow(Session.CurrentUser, Session.UserTenants);
        picker.Show();
        Close();
    }

    private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavList == null || ContentArea == null) return;

        ContentArea.Content = NavList.SelectedIndex switch
        {
            0 => _posView,
            1 => _invoicesView,
            2 => _inventoryView,
            3 => _dataManagementView,
            4 => _profileView ??= new ProfileView(),
            5 => _settingsView,
            _ => _posView
        };

        // Refresh invoices when navigating to that tab
        if (NavList.SelectedIndex == 1)
            _ = _invoicesView.LoadAsync();
    }

    private void BtnToggleNav_Click(object sender, RoutedEventArgs e)
    {
        _isNavExpanded = !_isNavExpanded;
        double targetWidth = _isNavExpanded ? ExpandedWidth : CollapsedWidth;

        var animation = new GridLengthAnimation
        {
            From = NavColumn.Width,
            To = new GridLength(targetWidth),
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

        NavColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);

        var textVisibility = _isNavExpanded ? Visibility.Visible : Visibility.Collapsed;
        NavTextPos.Visibility = textVisibility;
        NavTextInvoices.Visibility = textVisibility;
        NavTextInventory.Visibility = textVisibility;
        NavTextData.Visibility = textVisibility;
        NavTextProfile.Visibility = textVisibility;
        NavTextSettings.Visibility = textVisibility;

        // Adjust padding so icons stay visible when collapsed
        NavList.Margin = _isNavExpanded ? new Thickness(8, 12, 8, 0) : new Thickness(4, 12, 4, 0);
        var itemPadding = _isNavExpanded ? new Thickness(16, 12, 16, 12) : new Thickness(10, 12, 10, 12);
        var iconMargin = _isNavExpanded ? new Thickness(0, 0, 16, 0) : new Thickness(0);
        foreach (ListBoxItem item in NavList.Items)
        {
            item.Padding = itemPadding;
            if (item.Content is StackPanel sp && sp.Children[0] is MaterialDesignThemes.Wpf.PackIcon icon)
                icon.Margin = iconMargin;
        }
    }

    #region Keyboard Shortcuts

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Don't handle shortcuts when typing in text fields
        if (e.OriginalSource is System.Windows.Controls.TextBox || e.OriginalSource is System.Windows.Controls.PasswordBox)
        {
            // F-keys still work in text fields
            if (e.Key is not (Key.F1 or Key.F2 or Key.F3 or Key.F4 or Key.F5 or Key.F6 or Key.F7 or Key.F8 or Key.Escape))
                return;
        }

        switch (e.Key)
        {
            case Key.F1: // POS
                NavList.SelectedIndex = 0;
                e.Handled = true;
                break;
            case Key.F2: // Invoices
                NavList.SelectedIndex = 1;
                e.Handled = true;
                break;
            case Key.F3: // Inventory
                NavList.SelectedIndex = 2;
                e.Handled = true;
                break;
            case Key.F4: // Reports
                NavList.SelectedIndex = 3;
                e.Handled = true;
                break;
            case Key.F5: // Profile
                NavList.SelectedIndex = 4;
                e.Handled = true;
                break;
            case Key.F6: // Settings
                NavList.SelectedIndex = 5;
                e.Handled = true;
                break;
            case Key.Escape: // Close modal
                if (ModalOverlayContainer.Visibility == Visibility.Visible)
                {
                    _modalCloseCallback?.Invoke();
                    e.Handled = true;
                }
                break;
        }
    }

    #endregion

    #region Global Modal Overlay

    private Action? _modalCloseCallback;

    public void ShowModal(FrameworkElement content, Action? onBackdropClick = null)
    {
        _modalCloseCallback = onBackdropClick;
        ModalContent.Content = content;
        ModalOverlayContainer.Visibility = Visibility.Visible;
    }

    public void HideModal()
    {
        ModalOverlayContainer.Visibility = Visibility.Collapsed;
        ModalContent.Content = null;
        _modalCloseCallback = null;
    }

    private void ModalDimOverlay_Click(object sender, MouseButtonEventArgs e)
    {
        _modalCloseCallback?.Invoke();
    }

    #endregion
}

/// <summary>
/// Custom animation for GridLength since WPF doesn't natively support animating it.
/// </summary>
public class GridLengthAnimation : AnimationTimeline
{
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(GridLengthAnimation));

    public GridLength From
    {
        get => (GridLength)GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    public GridLength To
    {
        get => (GridLength)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    public IEasingFunction? EasingFunction
    {
        get => (IEasingFunction?)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    public override Type TargetPropertyType => typeof(GridLength);

    protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
    {
        double fromVal = From.Value;
        double toVal = To.Value;

        double progress = animationClock.CurrentProgress ?? 0;

        if (EasingFunction != null)
            progress = EasingFunction.Ease(progress);

        double current = fromVal + (toVal - fromVal) * progress;
        return new GridLength(current);
    }
}
