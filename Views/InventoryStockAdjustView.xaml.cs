using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;
using MyWinFormsApp.Services;

namespace MyWinFormsApp.Views;

public partial class InventoryStockAdjustView : UserControl
{
    private List<Product> _products = new();

    public InventoryStockAdjustView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (Session.CurrentTenant == null) return;

        ProgressLoad.Visibility = Visibility.Visible;
        var movements = await InventoryService.GetStockMovementsAsync(Session.CurrentTenant.Id);
        MovementList.ItemsSource = movements;
        TxtEmpty.Visibility = movements.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ProgressLoad.Visibility = Visibility.Collapsed;
    }

    private async void BtnNewAdjust_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentTenant == null) return;

        _products = await InventoryService.GetProductsAsync(Session.CurrentTenant.Id);
        CmbProduct.ItemsSource = _products;
        CmbProduct.SelectedIndex = -1;
        CmbType.SelectedIndex = 0;
        TxtQuantity.Text = "";
        TxtReference.Text = "";
        TxtNotes.Text = "";
        TxtMessage.Visibility = Visibility.Collapsed;
        ShowEditModal();
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentTenant == null) return;

        if (CmbProduct.SelectedItem is not Product product)
        {
            ShowMessage("Please select a product.", false);
            return;
        }

        if (!decimal.TryParse(TxtQuantity.Text, out var quantity) || quantity <= 0)
        {
            ShowMessage("Please enter a valid quantity.", false);
            return;
        }

        if (CmbType.SelectedItem is not ComboBoxItem typeItem)
        {
            ShowMessage("Please select adjustment type.", false);
            return;
        }

        var movementType = typeItem.Tag?.ToString() ?? "IN";
        var reference = TxtReference.Text.Trim();
        var notes = TxtNotes.Text.Trim();

        BtnSave.IsEnabled = false;
        ProgressSave.Visibility = Visibility.Visible;

        var (success, message) = await InventoryService.AdjustStockAsync(
            Session.CurrentTenant.Id, product.Id, movementType, quantity,
            reference, notes, Session.CurrentUser?.Id);

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
