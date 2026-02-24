using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;
using MyWinFormsApp.Services;

namespace MyWinFormsApp.Views;

public partial class CustomersView : UserControl
{
    private List<Customer> _customers = new();
    private Customer? _editing;

    public CustomersView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (Session.CurrentTenant == null) return;

        ProgressLoad.Visibility = Visibility.Visible;
        _customers = await CustomerService.GetCustomersAsync(Session.CurrentTenant.Id);
        CustomerList.ItemsSource = _customers;
        TxtCount.Text = $"({_customers.Count})";
        TxtEmpty.Visibility = _customers.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ProgressLoad.Visibility = Visibility.Collapsed;
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Session.CurrentTenant == null) return;
        var search = TxtSearch.Text.Trim();

        if (string.IsNullOrEmpty(search))
        {
            CustomerList.ItemsSource = _customers;
            return;
        }

        var filtered = _customers.Where(c =>
            c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            c.Phone.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            c.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            c.City.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        CustomerList.ItemsSource = filtered;
    }

    private void Row_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border border) return;
        var id = Convert.ToInt32(border.Tag);
        _editing = _customers.FirstOrDefault(c => c.Id == id);
        if (_editing == null) return;

        TxtEditTitle.Text = "Edit Customer";
        TxtName.Text = _editing.Name;
        TxtPhone.Text = _editing.Phone;
        TxtEmail.Text = _editing.Email;
        TxtAddress.Text = _editing.Address;
        TxtCity.Text = _editing.City;
        TxtState.Text = _editing.State;
        TxtPinCode.Text = _editing.PinCode;
        TxtGstin.Text = _editing.Gstin;
        TxtNotes.Text = _editing.Notes;
        ChkIsActive.IsChecked = _editing.IsActive;
        BtnDelete.Visibility = Visibility.Visible;
        TxtMessage.Visibility = Visibility.Collapsed;
        ShowEditModal();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        _editing = new Customer { TenantId = Session.CurrentTenant?.Id ?? 0, IsActive = true };
        TxtEditTitle.Text = "Add Customer";
        TxtName.Text = "";
        TxtPhone.Text = "";
        TxtEmail.Text = "";
        TxtAddress.Text = "";
        TxtCity.Text = "";
        TxtState.Text = "";
        TxtPinCode.Text = "";
        TxtGstin.Text = "";
        TxtNotes.Text = "";
        ChkIsActive.IsChecked = true;
        BtnDelete.Visibility = Visibility.Collapsed;
        TxtMessage.Visibility = Visibility.Collapsed;
        ShowEditModal();
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (_editing == null) return;

        _editing.Name = TxtName.Text.Trim();
        _editing.Phone = TxtPhone.Text.Trim();
        _editing.Email = TxtEmail.Text.Trim();
        _editing.Address = TxtAddress.Text.Trim();
        _editing.City = TxtCity.Text.Trim();
        _editing.State = TxtState.Text.Trim();
        _editing.PinCode = TxtPinCode.Text.Trim();
        _editing.Gstin = TxtGstin.Text.Trim();
        _editing.Notes = TxtNotes.Text.Trim();
        _editing.IsActive = ChkIsActive.IsChecked ?? true;

        BtnSave.IsEnabled = false;
        ProgressSave.Visibility = Visibility.Visible;

        var (success, message) = await CustomerService.SaveCustomerAsync(_editing);

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

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (_editing == null) return;
        if (MessageBox.Show($"Delete '{_editing.Name}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            var (success, message) = await CustomerService.DeleteCustomerAsync(_editing.Id);
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
