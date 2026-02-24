using System.Windows;
using System.Windows.Controls;

namespace MyWinFormsApp.Views;

public partial class InventoryView : UserControl
{
    private InventoryProductsView? _productsView;
    private InventoryCategoriesView? _categoriesView;
    private InventoryStockView? _stockView;
    private InventoryUnitsView? _unitsView;
    private InventorySuppliersView? _suppliersView;
    private CustomersView? _customersView;
    private ReturnsView? _returnsView;
    private PurchaseOrdersView? _purchaseOrdersView;
    private InventoryStockAdjustView? _stockAdjustView;

    private Button[] _tabButtons = [];

    public InventoryView()
    {
        InitializeComponent();
        _tabButtons = [BtnTabProducts, BtnTabCategories, BtnTabStock, BtnTabUnits, BtnTabSuppliers, BtnTabCustomers, BtnTabReturns, BtnTabStockAdjust, BtnTabPO];
        ShowTab("products");
    }

    private void BtnTabProducts_Click(object sender, RoutedEventArgs e) => ShowTab("products");
    private void BtnTabCategories_Click(object sender, RoutedEventArgs e) => ShowTab("categories");
    private void BtnTabStock_Click(object sender, RoutedEventArgs e) => ShowTab("stock");
    private void BtnTabUnits_Click(object sender, RoutedEventArgs e) => ShowTab("units");
    private void BtnTabSuppliers_Click(object sender, RoutedEventArgs e) => ShowTab("suppliers");
    private void BtnTabCustomers_Click(object sender, RoutedEventArgs e) => ShowTab("customers");
    private void BtnTabReturns_Click(object sender, RoutedEventArgs e) => ShowTab("returns");
    private void BtnTabStockAdjust_Click(object sender, RoutedEventArgs e) => ShowTab("stockadjust");
    private void BtnTabPO_Click(object sender, RoutedEventArgs e) => ShowTab("po");

    private void ShowTab(string tab)
    {
        foreach (var btn in _tabButtons)
        {
            btn.FontWeight = FontWeights.Normal;
            btn.Opacity = 0.6;
        }

        switch (tab)
        {
            case "products":
                _productsView ??= new InventoryProductsView();
                InventoryContent.Content = _productsView;
                BtnTabProducts.FontWeight = FontWeights.Bold;
                BtnTabProducts.Opacity = 1.0;
                break;
            case "categories":
                _categoriesView ??= new InventoryCategoriesView();
                InventoryContent.Content = _categoriesView;
                BtnTabCategories.FontWeight = FontWeights.Bold;
                BtnTabCategories.Opacity = 1.0;
                break;
            case "stock":
                _stockView ??= new InventoryStockView();
                InventoryContent.Content = _stockView;
                BtnTabStock.FontWeight = FontWeights.Bold;
                BtnTabStock.Opacity = 1.0;
                break;
            case "units":
                _unitsView ??= new InventoryUnitsView();
                InventoryContent.Content = _unitsView;
                BtnTabUnits.FontWeight = FontWeights.Bold;
                BtnTabUnits.Opacity = 1.0;
                break;
            case "suppliers":
                _suppliersView ??= new InventorySuppliersView();
                InventoryContent.Content = _suppliersView;
                BtnTabSuppliers.FontWeight = FontWeights.Bold;
                BtnTabSuppliers.Opacity = 1.0;
                break;
            case "customers":
                _customersView ??= new CustomersView();
                InventoryContent.Content = _customersView;
                BtnTabCustomers.FontWeight = FontWeights.Bold;
                BtnTabCustomers.Opacity = 1.0;
                break;
            case "returns":
                _returnsView ??= new ReturnsView();
                InventoryContent.Content = _returnsView;
                BtnTabReturns.FontWeight = FontWeights.Bold;
                BtnTabReturns.Opacity = 1.0;
                break;
            case "stockadjust":
                _stockAdjustView ??= new InventoryStockAdjustView();
                InventoryContent.Content = _stockAdjustView;
                BtnTabStockAdjust.FontWeight = FontWeights.Bold;
                BtnTabStockAdjust.Opacity = 1.0;
                break;
            case "po":
                _purchaseOrdersView ??= new PurchaseOrdersView();
                InventoryContent.Content = _purchaseOrdersView;
                BtnTabPO.FontWeight = FontWeights.Bold;
                BtnTabPO.Opacity = 1.0;
                break;
        }
    }
}
