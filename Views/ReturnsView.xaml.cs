using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;
using MyWinFormsApp.Services;

namespace MyWinFormsApp.Views;

public partial class ReturnsView : UserControl
{
    private List<CreditNote> _creditNotes = new();
    private Invoice? _selectedInvoice;
    private List<InvoiceItem> _invoiceItems = new();
    private List<ReturnItemRow> _returnRows = new();

    public ReturnsView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        if (Session.CurrentTenant == null) return;

        ProgressLoad.Visibility = Visibility.Visible;
        _creditNotes = await ReturnService.GetCreditNotesAsync(Session.CurrentTenant.Id);
        CreditNoteList.ItemsSource = _creditNotes;
        TxtCount.Text = $"({_creditNotes.Count})";
        TxtEmpty.Visibility = _creditNotes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ProgressLoad.Visibility = Visibility.Collapsed;
    }

    private void BtnNewReturn_Click(object sender, RoutedEventArgs e)
    {
        _selectedInvoice = null;
        _invoiceItems.Clear();
        _returnRows.Clear();
        TxtInvoiceSearch.Text = "";
        TxtReason.Text = "";
        InvoiceInfoPanel.Visibility = Visibility.Collapsed;
        TxtItemsLabel.Visibility = Visibility.Collapsed;
        ReturnItemsList.ItemsSource = null;
        BtnProcess.IsEnabled = false;
        TxtMessage.Visibility = Visibility.Collapsed;
        ShowEditModal();
    }

    private async void BtnLookup_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentTenant == null) return;
        var search = TxtInvoiceSearch.Text.Trim();
        if (string.IsNullOrEmpty(search))
        {
            ShowMessage("Please enter an invoice number.", false);
            return;
        }

        var invoices = await SalesService.SearchInvoicesAsync(
            Session.CurrentTenant.Id, null, null, "COMPLETED", search, 1);

        if (invoices.Count == 0)
        {
            ShowMessage("Invoice not found.", false);
            return;
        }

        _selectedInvoice = invoices[0];
        var (_, items) = await SalesService.GetInvoiceWithItemsAsync(_selectedInvoice.Id);
        _invoiceItems = items;

        TxtInvoiceInfo.Text = $"Invoice: {_selectedInvoice.InvoiceNumber} | Customer: {_selectedInvoice.CustomerName}";
        TxtInvoiceDetails.Text = $"Date: {_selectedInvoice.FormattedDate} | Total: {_selectedInvoice.FormattedTotal} | Items: {items.Count}";
        InvoiceInfoPanel.Visibility = Visibility.Visible;

        _returnRows = items.Select(item => new ReturnItemRow
        {
            InvoiceItemId = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            OriginalQty = item.Quantity,
            UnitPrice = item.UnitPrice,
            TaxRate = item.TaxRate,
            ReturnQty = 0
        }).ToList();

        BuildReturnItemsUI();
        TxtItemsLabel.Visibility = Visibility.Visible;
        BtnProcess.IsEnabled = true;
        TxtMessage.Visibility = Visibility.Collapsed;
    }

    private void BuildReturnItemsUI()
    {
        var panel = new StackPanel();
        foreach (var row in _returnRows)
        {
            var rowPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 8) };

            var chk = new CheckBox
            {
                Content = $"{row.ProductName} (Max: {row.OriginalQty:0.##} x \u20b9{row.UnitPrice:N2})",
                IsChecked = false,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = row
            };

            var qtyBox = new TextBox
            {
                Width = 80,
                Text = "0",
                FontSize = 12,
                Padding = new Thickness(6, 4, 6, 4),
                Margin = new Thickness(8, 0, 0, 0),
                Tag = row
            };
            qtyBox.TextChanged += (s, _) =>
            {
                if (s is TextBox tb && tb.Tag is ReturnItemRow r)
                {
                    if (decimal.TryParse(tb.Text, out var q) && q > 0 && q <= r.OriginalQty)
                        r.ReturnQty = q;
                    else
                        r.ReturnQty = 0;
                }
            };

            chk.Checked += (s, _) =>
            {
                if (s is CheckBox cb && cb.Tag is ReturnItemRow r)
                {
                    r.IsSelected = true;
                    if (r.ReturnQty == 0) r.ReturnQty = r.OriginalQty;
                    qtyBox.Text = r.ReturnQty.ToString("0.##");
                }
            };
            chk.Unchecked += (s, _) =>
            {
                if (s is CheckBox cb && cb.Tag is ReturnItemRow r)
                {
                    r.IsSelected = false;
                    r.ReturnQty = 0;
                    qtyBox.Text = "0";
                }
            };

            DockPanel.SetDock(qtyBox, Dock.Right);
            rowPanel.Children.Add(qtyBox);
            rowPanel.Children.Add(chk);
            panel.Children.Add(rowPanel);
        }
        ReturnItemsList.ItemsSource = null;
        ReturnItemsList.Items.Clear();
        ReturnItemsList.Items.Add(panel);
    }

    private async void BtnProcess_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedInvoice == null || Session.CurrentTenant == null) return;

        var selectedItems = _returnRows.Where(r => r.IsSelected && r.ReturnQty > 0).ToList();
        if (selectedItems.Count == 0)
        {
            ShowMessage("Please select at least one item to return.", false);
            return;
        }

        var reason = TxtReason.Text.Trim();
        if (string.IsNullOrEmpty(reason))
        {
            ShowMessage("Please enter a reason for the return.", false);
            return;
        }

        BtnProcess.IsEnabled = false;
        ProgressSave.Visibility = Visibility.Visible;

        var creditNoteItems = selectedItems.Select(r => new CreditNoteItem
        {
            InvoiceItemId = r.InvoiceItemId,
            ProductId = r.ProductId,
            ProductName = r.ProductName,
            Quantity = r.ReturnQty,
            UnitPrice = r.UnitPrice,
            TaxRate = r.TaxRate,
            TaxAmount = r.ReturnQty * r.UnitPrice * (r.TaxRate / 100m)
        }).ToList();

        var (success, message, _) = await ReturnService.CreateReturnAsync(
            Session.CurrentTenant.Id,
            _selectedInvoice.Id,
            _selectedInvoice.CustomerName,
            _selectedInvoice.CustomerId,
            reason,
            creditNoteItems,
            Session.CurrentUser?.Id);

        ProgressSave.Visibility = Visibility.Collapsed;
        BtnProcess.IsEnabled = true;

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

    private class ReturnItemRow
    {
        public int InvoiceItemId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal OriginalQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ReturnQty { get; set; }
        public bool IsSelected { get; set; }
    }
}
