using System.Windows;

namespace MyWinFormsApp.Views;

public partial class ThermalPrinterDialog : Window
{
    public string SelectedPrinter { get; private set; } = string.Empty;
    public int PaperWidth { get; private set; } = 48;

    public ThermalPrinterDialog(List<string> printers)
    {
        InitializeComponent();
        CboPrinter.ItemsSource = printers;
        if (printers.Count > 0)
            CboPrinter.SelectedIndex = 0;
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
        if (CboPrinter.SelectedItem == null)
        {
            MessageBox.Show("Please select a printer.", "No Printer Selected",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SelectedPrinter = CboPrinter.SelectedItem.ToString()!;
        PaperWidth = Rb58mm.IsChecked == true ? 32 : 48;
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
