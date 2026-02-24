using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;
using MyWinFormsApp.Services;

namespace MyWinFormsApp.Views;

public partial class PurchaseOrdersView : UserControl
{
    private List<PurchaseOrder> _orders = new();
    private List<Supplier> _suppliers = new();
    private List<Product> _products = new();
    private List<PurchaseOrderItem> _poItems = new();

    public PurchaseOrdersView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        if (Session.CurrentTenant == null) return;

        ProgressLoad.Visibility = Visibility.Visible;
        _orders = await PurchaseOrderService.GetAllAsync(Session.CurrentTenant.Id);
        PoList.ItemsSource = _orders;
        TxtCount.Text = $"({_orders.Count})";
        TxtEmpty.Visibility = _orders.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ProgressLoad.Visibility = Visibility.Collapsed;
    }

    private async void BtnNewPO_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentTenant == null) return;

        _poItems.Clear();
        TxtEditTitle.Text = "New Purchase Order";
        TxtNotes.Text = "";
        TxtTotal.Text = "";
        DpExpected.SelectedDate = null;
        PoItemsList.Items.Clear();
        TxtMessage.Visibility = Visibility.Collapsed;

        _suppliers = await InventoryService.GetActiveSuppliersAsync(Session.CurrentTenant.Id);
        _products = await InventoryService.GetProductsAsync(Session.CurrentTenant.Id);
        CmbSupplier.ItemsSource = _suppliers;
        CmbProduct.ItemsSource = _products;

        ShowEditModal();
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        if (CmbProduct.SelectedItem is not Product product) return;
        if (!decimal.TryParse(TxtItemQty.Text, out var qty) || qty <= 0)
        {
            ShowMessage("Enter a valid quantity.", false);
            return;
        }
        if (!decimal.TryParse(TxtItemPrice.Text, out var price) || price <= 0)
        {
            price = product.CostPrice;
        }

        var item = new PurchaseOrderItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = qty,
            UnitPrice = price,
            TaxRate = 0,
            TaxAmount = 0,
            LineTotal = qty * price
        };

        _poItems.Add(item);
        RefreshItemsList();

        CmbProduct.SelectedIndex = -1;
        TxtItemQty.Text = "";
        TxtItemPrice.Text = "";
    }

    private void RefreshItemsList()
    {
        PoItemsList.Items.Clear();
        int idx = 1;
        foreach (var item in _poItems)
        {
            var row = new DockPanel { Margin = new Thickness(0, 0, 0, 4) };

            var removeBtn = new Button
            {
                Content = new MaterialDesignThemes.Wpf.PackIcon
                    { Kind = MaterialDesignThemes.Wpf.PackIconKind.Close, Width = 14, Height = 14 },
                Width = 28, Height = 28,
                Padding = new Thickness(0),
                Tag = item,
                Style = (Style)FindResource("MaterialDesignIconForegroundButton")
            };
            removeBtn.Click += (s, _) =>
            {
                if (s is Button btn && btn.Tag is PurchaseOrderItem i)
                {
                    _poItems.Remove(i);
                    RefreshItemsList();
                }
            };
            DockPanel.SetDock(removeBtn, Dock.Right);

            var text = new TextBlock
            {
                Text = $"{idx}. {item.ProductName} \u2014 {item.Quantity:0.##} x \u20b9{item.UnitPrice:N2} = \u20b9{item.LineTotal:N2}",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };

            row.Children.Add(removeBtn);
            row.Children.Add(text);
            PoItemsList.Items.Add(row);
            idx++;
        }

        var total = _poItems.Sum(i => i.LineTotal);
        TxtTotal.Text = $"Total: \u20b9{total:N2}";
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentTenant == null) return;

        if (_poItems.Count == 0)
        {
            ShowMessage("Add at least one item.", false);
            return;
        }

        var supplier = CmbSupplier.SelectedItem as Supplier;
        var po = new PurchaseOrder
        {
            TenantId = Session.CurrentTenant.Id,
            SupplierId = supplier?.Id,
            SupplierName = supplier?.Name ?? "",
            Status = "DRAFT",
            Notes = TxtNotes.Text.Trim(),
            ExpectedDate = DpExpected.SelectedDate,
            CreatedBy = Session.CurrentUser?.Id
        };

        BtnSave.IsEnabled = false;
        ProgressSave.Visibility = Visibility.Visible;

        var (success, message, _) = await PurchaseOrderService.CreateAsync(po, _poItems);

        ProgressSave.Visibility = Visibility.Collapsed;
        BtnSave.IsEnabled = true;

        if (success)
        {
            HideEditModal();
            await LoadAsync();
        }
        else
        {
            ShowMessage(message, false);
        }
    }

    private async void Row_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border border || Session.CurrentTenant == null) return;
        var id = Convert.ToInt32(border.Tag);
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null) return;

        if (order.Status == "DRAFT")
        {
            var result = MessageBox.Show(
                $"PO: {order.PoNumber}\nSupplier: {order.SupplierName}\nTotal: {order.FormattedTotal}\n\nMark as Ordered?",
                "Purchase Order",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await PurchaseOrderService.UpdateStatusAsync(order.Id, "ORDERED");
                await LoadAsync();
            }
        }
        else
        {
            MessageBox.Show(
                $"PO: {order.PoNumber}\nSupplier: {order.SupplierName}\nTotal: {order.FormattedTotal}\nStatus: {order.StatusDisplay}",
                "Purchase Order",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => HideEditModal();

    #region Modal Helpers

    private void ShowEditModal()
    {
        if (EditCard.Parent is Panel p) p.Children.Remove(EditCard);
        var mw = (MainWindow)Window.GetWindow(this);
        mw.ShowModal(EditCard, HideEditModal);
    }

    private void HideEditModal()
    {
        var mw = (MainWindow)Window.GetWindow(this);
        mw.HideModal();
        if (!OverlayEdit.Children.Contains(EditCard))
            OverlayEdit.Children.Add(EditCard);
    }

    #endregion

    private void ShowMessage(string message, bool isSuccess)
    {
        TxtMessage.Text = message;
        TxtMessage.Foreground = new SolidColorBrush(isSuccess
            ? Color.FromRgb(76, 175, 80) : Color.FromRgb(244, 67, 54));
        TxtMessage.Visibility = Visibility.Visible;
    }
}
