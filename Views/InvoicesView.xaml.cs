using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Services;

namespace MyWinFormsApp.Views;

public partial class InvoicesView : UserControl
{
    private bool _isLoaded;

    public InvoicesView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                await LoadAsync();
            }
        };
    }

    public async Task LoadAsync()
    {
        if (Session.CurrentTenant == null) return;

        ProgressLoad.Visibility = Visibility.Visible;
        PanelEmpty.Visibility = Visibility.Collapsed;
        InvoiceList.ItemsSource = null;

        try
        {
            var fromDate = DpFrom.SelectedDate;
            var toDate = DpTo.SelectedDate;
            var status = (CbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "ALL";
            var search = TxtSearch.Text?.Trim();

            var invoices = await SalesService.SearchInvoicesAsync(
                Session.CurrentTenant.Id, fromDate, toDate, status, search);

            InvoiceList.ItemsSource = invoices;

            // Update summary cards
            var completed = invoices.Where(i => i.Status == "COMPLETED").ToList();
            var held = invoices.Where(i => i.Status == "HELD").ToList();
            var cancelled = invoices.Where(i => i.Status == "CANCELLED").ToList();

            TxtTotalRevenue.Text = $"\u20b9{completed.Sum(i => i.TotalAmount):N2}";
            TxtCompletedCount.Text = completed.Count.ToString();
            TxtHeldCount.Text = held.Count.ToString();
            TxtCancelledCount.Text = cancelled.Count.ToString();

            PanelEmpty.Visibility = invoices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load invoices: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            ProgressLoad.Visibility = Visibility.Collapsed;
        }
    }

    private async void Filter_Changed(object sender, EventArgs e)
    {
        if (!_isLoaded) return;
        await LoadAsync();
    }

    private async void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isLoaded) return;

        // Simple debounce: wait briefly then reload
        var searchText = TxtSearch.Text;
        await Task.Delay(300);
        if (TxtSearch.Text != searchText) return; // user typed more, skip

        await LoadAsync();
    }

    private async void Invoice_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border border || border.Tag is not int invoiceId) return;

        var (invoice, items) = await SalesService.GetInvoiceWithItemsAsync(invoiceId);
        if (invoice == null) return;

        // Build a simple receipt detail view
        var detail = new Border
        {
            Background = (System.Windows.Media.Brush)FindResource("MaterialDesign.Brush.Card"),
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(28, 24, 28, 24),
            MinWidth = 420,
            MaxWidth = 520,
            MaxHeight = 600
        };

        var stack = new StackPanel();

        // Header
        var header = new TextBlock
        {
            Text = $"Invoice {invoice.InvoiceNumber}",
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 4)
        };
        stack.Children.Add(header);

        var dateText = new TextBlock
        {
            Text = invoice.FormattedDate,
            FontSize = 12,
            Opacity = 0.5,
            Margin = new Thickness(0, 0, 0, 16)
        };
        stack.Children.Add(dateText);

        // Customer
        if (!string.IsNullOrEmpty(invoice.CustomerName))
        {
            stack.Children.Add(new TextBlock
            {
                Text = $"Customer: {invoice.CustomerName}",
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8)
            });
        }

        // Status & Payment
        stack.Children.Add(new TextBlock
        {
            Text = $"Status: {invoice.StatusDisplay}  |  Payment: {invoice.PaymentMethodDisplay}",
            FontSize = 12,
            Opacity = 0.6,
            Margin = new Thickness(0, 0, 0, 16)
        });

        // Divider
        stack.Children.Add(new Border
        {
            Height = 1,
            Background = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"),
            Margin = new Thickness(0, 0, 0, 12)
        });

        // Items header
        var itemsHeader = new Grid();
        itemsHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        itemsHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
        itemsHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });

        var hdrName = new TextBlock { Text = "Item", FontSize = 11, FontWeight = FontWeights.SemiBold, Opacity = 0.5 };
        Grid.SetColumn(hdrName, 0);
        itemsHeader.Children.Add(hdrName);

        var hdrQty = new TextBlock { Text = "Qty", FontSize = 11, FontWeight = FontWeights.SemiBold, Opacity = 0.5, HorizontalAlignment = HorizontalAlignment.Center };
        Grid.SetColumn(hdrQty, 1);
        itemsHeader.Children.Add(hdrQty);

        var hdrAmt = new TextBlock { Text = "Amount", FontSize = 11, FontWeight = FontWeights.SemiBold, Opacity = 0.5, HorizontalAlignment = HorizontalAlignment.Right };
        Grid.SetColumn(hdrAmt, 2);
        itemsHeader.Children.Add(hdrAmt);

        stack.Children.Add(itemsHeader);
        stack.Children.Add(new Border { Height = 6 });

        // Items list in scrollviewer
        var itemsStack = new StackPanel();
        foreach (var item in items)
        {
            var row = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });

            var nameBlock = new TextBlock { Text = item.ProductName, FontSize = 12.5, TextTrimming = TextTrimming.CharacterEllipsis };
            Grid.SetColumn(nameBlock, 0);
            row.Children.Add(nameBlock);

            var qtyBlock = new TextBlock { Text = item.Quantity.ToString(), FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Opacity = 0.7 };
            Grid.SetColumn(qtyBlock, 1);
            row.Children.Add(qtyBlock);

            var amtBlock = new TextBlock { Text = $"\u20b9{item.LineTotal:N2}", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Right };
            Grid.SetColumn(amtBlock, 2);
            row.Children.Add(amtBlock);

            itemsStack.Children.Add(row);
        }

        var scrollViewer = new ScrollViewer
        {
            Content = itemsStack,
            MaxHeight = 250,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        stack.Children.Add(scrollViewer);

        // Divider
        stack.Children.Add(new Border
        {
            Height = 1,
            Background = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"),
            Margin = new Thickness(0, 12, 0, 12)
        });

        // Totals
        AddTotalRow(stack, "Subtotal", $"\u20b9{invoice.Subtotal:N2}");
        if (invoice.DiscountAmount > 0)
            AddTotalRow(stack, "Discount", $"-\u20b9{invoice.DiscountAmount:N2}", "#EF4444");
        if (invoice.TaxAmount > 0)
            AddTotalRow(stack, "Tax", $"+\u20b9{invoice.TaxAmount:N2}");

        stack.Children.Add(new Border { Height = 8 });

        // Grand total
        var totalGrid = new Grid();
        totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var totalLabel = new TextBlock { Text = "Total", FontSize = 15, FontWeight = FontWeights.Bold };
        Grid.SetColumn(totalLabel, 0);
        totalGrid.Children.Add(totalLabel);

        var totalValue = new TextBlock { Text = invoice.FormattedTotal, FontSize = 15, FontWeight = FontWeights.Bold };
        Grid.SetColumn(totalValue, 1);
        totalGrid.Children.Add(totalValue);

        stack.Children.Add(totalGrid);

        // Tendered / Change for cash
        if (invoice.AmountTendered.HasValue && invoice.PaymentMethod == "CASH")
        {
            stack.Children.Add(new Border { Height = 8 });
            AddTotalRow(stack, "Tendered", $"\u20b9{invoice.AmountTendered.Value:N2}");
            if (invoice.ChangeGiven.HasValue && invoice.ChangeGiven.Value > 0)
                AddTotalRow(stack, "Change", $"\u20b9{invoice.ChangeGiven.Value:N2}");
        }

        // Close button
        var closeBtn = new Button
        {
            Content = "Close",
            Margin = new Thickness(0, 20, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 38,
            FontSize = 13,
            Style = (Style)FindResource("MaterialDesignFlatButton")
        };
        closeBtn.Click += (_, _) =>
        {
            if (Window.GetWindow(this) is MainWindow mw)
                mw.HideModal();
        };
        stack.Children.Add(closeBtn);

        detail.Child = stack;

        if (Window.GetWindow(this) is MainWindow mainWin)
            mainWin.ShowModal(detail, () => mainWin.HideModal());
    }

    private static void AddTotalRow(StackPanel parent, string label, string value, string? color = null)
    {
        var grid = new Grid { Margin = new Thickness(0, 2, 0, 2) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var lbl = new TextBlock { Text = label, FontSize = 12.5, Opacity = 0.6 };
        Grid.SetColumn(lbl, 0);
        grid.Children.Add(lbl);

        var val = new TextBlock { Text = value, FontSize = 12.5 };
        if (color != null)
            val.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
        Grid.SetColumn(val, 1);
        grid.Children.Add(val);

        parent.Children.Add(grid);
    }
}
