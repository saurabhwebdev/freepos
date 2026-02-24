<div align="center">

<img src="https://img.shields.io/badge/OpenPOS-Point_of_Sale-0078D4?style=for-the-badge&logoColor=white" alt="OpenPOS" />

# OpenPOS

### The Open-Source Point of Sale System for Modern Businesses

<p>
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 9" />
  <img src="https://img.shields.io/badge/WPF-Desktop_App-0078D4?style=flat-square&logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/PostgreSQL-18-4169E1?style=flat-square&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Material_Design-5.3-00BCD4?style=flat-square&logo=materialdesign&logoColor=white" alt="Material Design" />
  <img src="https://img.shields.io/badge/License-MIT-22c55e?style=flat-square" alt="MIT License" />
  <img src="https://img.shields.io/badge/Platform-Windows_10%2F11-0078D4?style=flat-square&logo=windows11&logoColor=white" alt="Windows" />
</p>

<p>
  <strong>16,800+ lines of code</strong> &nbsp;·&nbsp; <strong>25 data models</strong> &nbsp;·&nbsp; <strong>15 services</strong> &nbsp;·&nbsp; <strong>34 views</strong> &nbsp;·&nbsp; <strong>22+ database tables</strong>
</p>

<br />

[Features](#-features) · [Quick Start](#-quick-start) · [Architecture](#-architecture) · [Database](#-database-schema) · [Configuration](#-configuration) · [PDF Reports](#-pdf-reports) · [Installation](#-installation) · [Contributing](#-contributing)

<br />

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
  - [Point of Sale](#-point-of-sale)
  - [Invoice Management](#-invoice-management)
  - [Inventory Management](#-inventory-management)
  - [Customer Management](#-customer-management)
  - [Purchase Orders](#-purchase-orders)
  - [Returns & Credit Notes](#-returns--credit-notes)
  - [Reports & Analytics](#-reports--analytics)
  - [Settings & Configuration](#%EF%B8%8F-settings--configuration)
  - [Thermal Printing](#-thermal-printing)
  - [Keyboard Shortcuts](#%EF%B8%8F-keyboard-shortcuts)
  - [Offline Mode](#-offline-mode)
- [Tech Stack](#-tech-stack)
- [Quick Start](#-quick-start)
- [Architecture](#-architecture)
  - [Project Structure](#project-structure)
  - [Multi-Tenant Architecture](#multi-tenant-architecture)
  - [Data Flow](#data-flow)
  - [Service Layer](#service-layer)
  - [Model Layer](#model-layer)
- [Database Schema](#-database-schema)
- [Configuration](#-configuration)
- [PDF Reports](#-pdf-reports)
- [Excel Import](#-excel-import)
- [Installation](#-installation)
- [Troubleshooting](#-troubleshooting)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🌟 Overview

**OpenPOS** is a professional-grade, open-source desktop Point of Sale application built for retail businesses, shops, restaurants, and small enterprises. It delivers a **complete business management solution** — from processing sales to managing inventory, tracking suppliers, handling tax compliance, generating PDF reports, emailing invoices, and importing data from Excel.

### Why OpenPOS?

| | Feature | Description |
|---|---------|-------------|
| 🏪 | **Multi-Shop Support** | Run multiple businesses from a single installation with complete data isolation |
| 🔒 | **Role-Based Access** | Fine-grained permissions — Admins see everything, Cashiers see only what they need |
| 🇮🇳 | **Tax Compliant** | Built-in GST support with CGST/SGST split, HSN codes, and multi-rate tax slabs |
| 📄 | **Professional PDFs** | Invoice PDFs with UPI QR codes, bank details, tax breakdowns, and business branding |
| 📧 | **Email Integration** | Send invoices and reports via Gmail, Outlook, Yahoo, or custom SMTP |
| 🖨️ | **Thermal Printing** | Direct ESC/POS thermal receipt printing for 58mm and 80mm printers |
| 📊 | **7 Report Types** | From daily sales to consolidated multi-page business reports |
| 📥 | **Excel Import** | Bulk import products, categories, and suppliers from .xlsx files |
| ⌨️ | **Keyboard Shortcuts** | F1-F6 navigation, fast checkout workflows |
| 🔌 | **Offline Resilient** | Graceful handling when database connectivity is lost |

---

## ✨ Features

### 🛒 Point of Sale

The POS screen is the heart of OpenPOS — designed for speed and efficiency.

| Feature | Details |
|---------|---------|
| **Product Search** | Search by name, SKU, or barcode with real-time filtering |
| **Barcode Scanner** | Plug-and-play USB barcode scanner support — scan to add items instantly |
| **Category Navigation** | Horizontal scrolling pill buttons for quick category filtering |
| **Cart Management** | Add, adjust quantity, remove items with live total calculation |
| **Customer Lookup** | Optional customer name/selection per transaction |
| **Discount Support** | Fixed amount (`₹50 off`) or percentage-based (`10% off`) discounts |
| **Tax Calculation** | Automatic tax from configurable slabs — supports GST with CGST/SGST split |
| **Multiple Payments** | Cash (with tendered/change), UPI, and Card payment methods |
| **UPI QR Code** | Auto-generated UPI payment QR with pre-filled amount at checkout |
| **Hold & Resume** | Pause a transaction, serve another customer, then resume |
| **Invoice Generation** | Auto-incrementing invoice numbers with configurable prefix (e.g., `INV-000042`) |

### 📑 Invoice Management

| Feature | Details |
|---------|---------|
| **Invoice List** | All invoices with search, status filter, and date range selection |
| **Detailed View** | Line items, tax breakdown, discount info, payment details |
| **PDF Export** | Professional invoice PDF with business branding |
| **Email Invoice** | Send PDF invoice via configured email |
| **WhatsApp Share** | Share invoice text via WhatsApp with one click |
| **Print** | Standard Windows print dialog |
| **Thermal Print** | Direct ESC/POS receipt on thermal printers |
| **Cancel Invoice** | Void an invoice with stock reversal |

**Invoice PDF includes:**
- Business header with address, GSTIN, phone, email
- Itemized table with HSN codes, quantities, unit prices, per-item tax
- Component-level tax breakdown (e.g., CGST 9% + SGST 9%)
- Bank transfer details (A/C holder, number, bank, branch, IFSC)
- UPI QR code with pre-filled payment amount
- Configurable footer text

### 📦 Inventory Management

OpenPOS provides a tabbed inventory management interface with **9 sub-modules**:

#### Products
| Feature | Details |
|---------|---------|
| **Full CRUD** | Create, view, edit, delete products |
| **Rich Fields** | Name, SKU, barcode, HSN code, description, cost price, selling price, MRP |
| **Associations** | Link to category, tax slab, unit of measure, and supplier |
| **Stock Tracking** | Current stock level + minimum stock threshold for alerts |
| **Search** | Real-time filter by name, SKU, or barcode |
| **Detail View** | Click any row to see full product info |
| **Excel Import** | Bulk import from `.xlsx` with category auto-matching |

#### Categories
- Create categories with name, description, sort order
- Toggle active/inactive
- Bulk import from Excel

#### Units of Measurement
- Define units (Kg, Pcs, Ltr, Dozen, Box, etc.)
- Short name display (e.g., `kg`, `pcs`)
- Auto-seeded with common units on first use

#### Suppliers
- Full supplier directory: company name, contact person, email, phone
- Address management (street, city, state, PIN code)
- GST number tracking
- Search by name, contact, phone, GST, or city
- Excel import support

#### Stock Levels
- View current stock across all products
- Low stock indicators with color coding

#### Stock Adjustments
- Stock In / Stock Out / Set Stock operations
- Reference and notes fields for audit trail
- Movement history with type-colored badges (green=IN, red=OUT, yellow=ADJUST)
- Full stock movement log with previous/new stock values

### 👥 Customer Management

| Feature | Details |
|---------|---------|
| **Customer Database** | Name, phone, email, address, city, state, PIN code, GSTIN |
| **Search** | Filter by name, phone, email, or city |
| **Modal Editor** | Add/edit customers in a clean modal overlay |
| **Active/Inactive** | Toggle customer status |
| **Notes** | Free-text notes per customer |
| **Link to Sales** | Associate customers with invoices |

### 📋 Purchase Orders

| Feature | Details |
|---------|---------|
| **Create PO** | Select supplier, expected date, add items with auto tax calculation |
| **Tax from Products** | Each item auto-picks tax rate from product's configured tax slab |
| **Status Workflow** | Draft → Ordered → Partially Received → Received / Cancelled |
| **PO Detail View** | Click any PO to see full breakdown with items, tax, and totals |
| **Download PDF** | Professional PO PDF with supplier info, items table, tax column, totals |
| **Email to Supplier** | Send PO PDF via email — auto-fills supplier email address |
| **Mark as Ordered** | One-click status update from Draft to Ordered |
| **Stock Receiving** | Receive items and auto-update product stock with movement records |

### 🔄 Returns & Credit Notes

| Feature | Details |
|---------|---------|
| **Create Return** | Look up original invoice by number, select items to return |
| **Quantity Selection** | Choose how many of each item to return |
| **Credit Note** | Auto-generates credit note number (e.g., `CN-000001`) |
| **Stock Restoration** | Returned items automatically added back to inventory |
| **Movement Tracking** | Stock movements recorded with reference to credit note |

### 📊 Reports & Analytics

**7 report types** with PDF export and email capabilities:

| Report | Description | Format |
|--------|-------------|--------|
| 📈 **Daily Sales** | All transactions for a specific date with revenue, tax, discount totals | A4 Portrait |
| 📊 **Sales Summary** | Aggregated sales for date range with payment method breakdown (Cash/UPI/Card) | A4 Portrait |
| 🏷️ **Product Sales** | Product-wise analysis — quantity sold, revenue, tax collected per product | A4 Portrait |
| 📦 **Inventory Report** | Complete stock overview with values, cost prices, categories, suppliers | A4 Landscape |
| ⚠️ **Low Stock Alert** | Products below minimum threshold with deficit calculations | A4 Portrait |
| 🧾 **Tax Collection** | Tax slab-wise collection grouped by rate — taxable amount + tax collected | A4 Portrait |
| 📑 **Consolidated** | All-in-one multi-page report: summary + invoices + products + tax + inventory + low stock + invoice copies | Multi-page A4 |

**Report Features:**
- 📅 Custom date range with quick selectors (Today, This Week, This Month)
- 📄 Professional PDF formatting with business branding
- 📧 Email any report as a PDF attachment
- 📂 Auto-saves to `~/Downloads` folder
- 🖥️ Auto-opens generated PDF in default viewer

### ⚙️ Settings & Configuration

#### 🏢 Business Details
Configure your complete business profile:
- Company name, type, owner name
- Contact: email, phone, website
- Full address (street, city, state, country, postal code)
- Tax registration: GSTIN, PAN, business registration number
- Currency: code + symbol (supports INR, USD, EUR, GBP, AUD, CAD, and more)
- Banking: account holder, number, bank name, branch, IFSC
- UPI: UPI ID + display name for QR code generation
- Invoice prefix + custom footer text

#### 🧾 Tax Configuration
- Create tax slabs with name, rate, and type
- **Component taxes**: Split tax into sub-components (e.g., GST → CGST 9% + SGST 9%)
- Country-specific support
- Pre-seeded defaults for India (0%, 5%, 12%, 18%, 28%), USA, UK, Canada, Australia

#### 👤 Roles & Access Control
| Role | Default Access |
|------|---------------|
| **Admin** | Full access to all modules |
| **Cashier** | POS and Invoices only |

- Granular module-level permissions (POS, Inventory, Invoices, Reports, Settings)
- Add/remove users from shop
- Invite users by email

#### 📧 Email Settings
- **One-click presets**: Gmail, Outlook, Yahoo
- **Custom SMTP**: host, port, SSL/TLS toggle
- Sender name + email + app password
- Test email verification
- Enable/disable toggle

#### 💳 Payment Gateway Settings
Supports 8 payment gateways:
- Stripe, PayPal, Razorpay, Paytm, PhonePe, Square, Instamojo, Cashfree
- API key, secret, merchant ID, webhook secret
- Test/Live mode toggle
- Gateway-specific currency

#### 💾 Backup & Restore
- Export full database backup as `.sql` file
- Restore from backup file
- Timestamped backup filenames (`openpos_backup_20260224_143022.sql`)

### 🖨️ Thermal Printing

Direct ESC/POS receipt printing to thermal printers:

| Feature | Details |
|---------|---------|
| **Auto-detect** | Lists all installed printers |
| **Paper Sizes** | 80mm (48 chars) and 58mm (32 chars) support |
| **Printer Selection** | Dialog to choose printer and paper width |
| **Raw Printing** | Direct byte-level ESC/POS via Windows `winspool.drv` |
| **Receipt Format** | Business header, invoice info, items table, totals, footer, auto-cut |

**ESC/POS commands supported:** Initialize, Center/Left align, Bold on/off, Double height, Double width, Line feed, Paper cut.

### ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `F1` | Navigate to POS |
| `F2` | Navigate to Invoices |
| `F3` | Navigate to Inventory |
| `F4` | Navigate to Reports |
| `F5` | Navigate to Settings |
| `F6` | Navigate to Profile |
| `Escape` | Close current modal / dialog |

### 🔌 Offline Mode

OpenPOS handles database connectivity issues gracefully:
- **Connection monitoring** — tracks database connection state
- **Graceful degradation** — shows offline indicator when connection is lost
- **Auto-recovery** — reconnects when database becomes available again
- **No data loss** — prevents operations that would fail without database

---

## 🛠 Tech Stack

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| **Runtime** | .NET | 9.0 | Application framework |
| **UI Framework** | WPF | - | Windows desktop UI |
| **Design System** | Material Design In XAML | 5.3.0 | Modern UI controls and theming |
| **Database** | PostgreSQL | 18 | Primary data store |
| **ORM** | Dapper | 2.1.66 | Lightweight, high-performance data access |
| **PDF Engine** | QuestPDF | 2026.2.1 | Professional PDF document generation |
| **QR Codes** | QRCoder | 1.7.0 | UPI payment QR code generation |
| **Excel** | ClosedXML | 0.104.2 | Excel file import (.xlsx) |
| **Security** | BCrypt.Net-Next | 4.1.0 | Password hashing (bcrypt) |
| **DB Driver** | Npgsql | 10.0.1 | PostgreSQL .NET driver |
| **Config** | Microsoft.Extensions.Configuration | 10.0.3 | JSON configuration |
| **Installer** | Inno Setup | 6 | Windows installer builder |

---

## 🚀 Quick Start

### Prerequisites

| Requirement | Version | Download |
|-------------|---------|----------|
| .NET SDK | 9.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0) |
| PostgreSQL | 17+ | [postgresql.org](https://www.postgresql.org/download/) |
| OS | Windows 10/11 | - |

### 1. Clone the repository

```bash
git clone https://github.com/saurabhwebdev/openpos.git
cd openpos
```

### 2. Create the database

```bash
# Create database
psql -U postgres -c "CREATE DATABASE mywinformsapp_db;"

# Run schema
psql -U postgres -d mywinformsapp_db -f installer/schema.sql

# Run migration (for latest features)
psql -U postgres -d mywinformsapp_db -f migration.sql
```

### 3. Configure connection string

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mywinformsapp_db;Username=postgres;Password=postgres"
  }
}
```

### 4. Run the application

```bash
dotnet run
```

### 5. First-time setup

1. **Register** — Create a new account (first user gets Admin role)
2. **Business Details** — Go to Settings → Business Details and fill in your company info
3. **Tax Slabs** — Go to Settings → Tax and verify/customize tax rates
4. **Add Products** — Go to Inventory → Products and add your catalog (or bulk import from Excel)
5. **Start Selling** — Navigate to POS and process your first sale!

### Build for Production

```bash
# Self-contained Windows x64 build
dotnet publish -c Release -r win-x64 --self-contained true
```

---

## 🏗 Architecture

### Project Structure

```
openpos/
│
├── 📁 Models/                          # 25 data model classes
│   ├── 🔐 User.cs                     # User accounts
│   ├── 🏪 Tenant.cs                   # Business/shop entities
│   ├── 👤 UserTenant.cs               # User-shop-role mapping
│   ├── 🛡️ Role.cs                     # Role definitions
│   ├── 🔑 RolePermission.cs           # Module permissions
│   ├── 📦 Module.cs                   # Application modules
│   ├── 🏢 BusinessDetails.cs          # Business profile + banking + UPI
│   ├── 🏷️ Category.cs                 # Product categories
│   ├── 📏 Unit.cs                     # Units of measurement
│   ├── 🏭 Supplier.cs                 # Supplier directory
│   ├── 🧾 TaxSlab.cs                  # Tax rates with components
│   ├── 📦 Product.cs                  # Product catalog
│   ├── 🧾 Invoice.cs                  # Sales transactions
│   ├── 📋 InvoiceItem.cs              # Invoice line items
│   ├── 🛒 CartItem.cs                 # POS cart (INotifyPropertyChanged)
│   ├── 📊 StockMovement.cs            # Stock audit trail
│   ├── 👥 Customer.cs                 # Customer database
│   ├── 📋 PurchaseOrder.cs            # Purchase orders
│   ├── 📋 PurchaseOrderItem.cs        # PO line items
│   ├── 🔄 CreditNote.cs              # Return credit notes
│   ├── 🔄 CreditNoteItem.cs          # Credit note line items
│   ├── 📧 EmailSettings.cs            # SMTP configuration
│   └── 💳 PaymentGatewaySettings.cs   # Payment gateway config
│
├── 📁 Services/                        # 15 static service classes
│   ├── 🔐 AuthService.cs              # Login, register, session management
│   ├── 🛡️ RoleService.cs              # Roles, permissions, user management
│   ├── 🏢 BusinessService.cs          # Business details CRUD
│   ├── 📦 InventoryService.cs         # Products, categories, units, suppliers, stock
│   ├── 🧾 TaxService.cs              # Tax slab management + country defaults
│   ├── 💰 SalesService.cs            # Invoicing, hold/resume, cancellation
│   ├── 👥 CustomerService.cs          # Customer CRUD + search
│   ├── 📋 PurchaseOrderService.cs     # PO lifecycle + stock receiving
│   ├── 🔄 ReturnService.cs           # Credit notes + stock restoration
│   ├── 📊 ReportService.cs           # Report queries (7 report types)
│   ├── 📄 PdfExportService.cs        # PDF generation (QuestPDF)
│   ├── 🖨️ ThermalPrintService.cs     # ESC/POS thermal receipt printing
│   ├── 📧 EmailService.cs            # SMTP email (invoices, POs, reports)
│   ├── 📥 ExcelImportService.cs      # Excel import (products, categories, suppliers)
│   └── 💳 PaymentGatewayService.cs   # Payment gateway config CRUD
│
├── 📁 Views/                           # 34 XAML views + code-behind
│   ├── 🛒 PosView                     # Point of Sale screen
│   ├── 📑 InvoicesView                # Invoice listing
│   ├── 📑 InvoiceHistoryView          # Invoice history browser
│   ├── 🧾 ReceiptView                 # Receipt display + share (print, email, WhatsApp)
│   ├── ⏸️ HeldOrdersView              # Held/paused transactions
│   ├── 📦 InventoryView               # Inventory tab container (9 sub-tabs)
│   ├── 📦 InventoryProductsView       # Product management
│   ├── 🏷️ InventoryCategoriesView     # Category management
│   ├── 📏 InventoryUnitsView          # Unit management
│   ├── 🏭 InventorySuppliersView      # Supplier management
│   ├── 📊 InventoryStockView          # Stock levels
│   ├── 📊 InventoryStockAdjustView    # Stock adjustments
│   ├── 👥 CustomersView               # Customer database
│   ├── 📋 PurchaseOrdersView          # Purchase order management
│   ├── 🔄 ReturnsView                 # Returns & credit notes
│   ├── 📊 DataManagementView          # Reports dashboard
│   ├── ⚙️ SettingsView                # Settings tab container
│   ├── ⚙️ SettingsGeneralView         # General settings
│   ├── 🏢 BusinessDetailsView         # Business profile editor
│   ├── 🧾 SettingsTaxView             # Tax slab configuration
│   ├── 📧 SettingsEmailView           # Email/SMTP settings
│   ├── 💳 SettingsPaymentView         # Payment gateway settings
│   ├── 💾 SettingsBackupView          # Backup & restore
│   ├── 🛡️ RolesAccessView            # Roles & permissions
│   ├── 👤 ProfileView                 # User profile
│   └── 📁 Dialogs                     # EmailInputDialog, PhoneInputDialog, ThermalPrinterDialog
│
├── 📁 Helpers/                         # Utility classes
│   ├── 🗄️ DatabaseHelper.cs           # Dapper wrapper + connection management + offline mode
│   ├── 🔑 Session.cs                  # Static session state (user, tenant, permissions)
│   └── ⚙️ AppConfig.cs               # Configuration loader
│
├── 📁 ViewModels/                      # MVVM support
│   └── MainViewModel.cs               # Main window view model
│
├── 📁 installer/                       # Windows installer
│   ├── 📜 OpenPOS.iss                 # Inno Setup script
│   ├── 📜 schema.sql                  # Full database schema (22KB)
│   ├── 📜 setup-db.bat               # Database initialization script
│   └── 📜 build-installer.ps1        # Build automation (publish + Inno Setup)
│
├── 📄 MainWindow.xaml/.cs             # App shell + sidebar navigation + keyboard shortcuts
├── 📄 LoginWindow.xaml/.cs            # Login screen
├── 📄 RegisterWindow.xaml/.cs         # Registration screen
├── 📄 ShopPickerWindow.xaml/.cs       # Multi-shop selector
├── 📄 appsettings.json                # App configuration
├── 📄 migration.sql                   # Database migrations
└── 📄 MyWinFormsApp.csproj            # .NET project file
```

### Multi-Tenant Architecture

OpenPOS uses a **shared database, tenant-isolated** architecture. Every piece of business data is scoped by `tenant_id`, enabling complete data isolation between shops:

```
┌──────────────────────────────────────────────────────────────┐
│                     USER REGISTRATION                        │
│                                                              │
│  User signs up  ──→  Tenant (Shop) auto-created             │
│                        │                                     │
│                        ├── user_tenants (role: Admin)        │
│                        │                                     │
│                        ▼                                     │
│              ┌─────────────────────┐                         │
│              │   ALL DATA SCOPED   │                         │
│              │    BY tenant_id     │                         │
│              └─────────┬───────────┘                         │
│                        │                                     │
│     ┌──────────┬───────┼───────┬──────────┬──────────┐       │
│     ▼          ▼       ▼       ▼          ▼          ▼       │
│  Products  Invoices  Tax    Suppliers  Customers  Settings   │
│  Categories  Items   Slabs   Stock     POs        Email      │
│  Units       Cart           Movements  Returns    Payment    │
└──────────────────────────────────────────────────────────────┘

  User can belong to MULTIPLE tenants (shops) with different roles
```

### Data Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                          POS WORKFLOW                            │
│                                                                 │
│  POS View ──→ SalesService ──→ DatabaseHelper ──→ PostgreSQL    │
│     │              │                                            │
│     │              ├── Creates Invoice + InvoiceItems            │
│     │              ├── Deducts product stock                     │
│     │              ├── Records stock movements                   │
│     │              └── Returns complete Invoice data             │
│     │                        │                                  │
│     ▼                        ▼                                  │
│  ReceiptView         PdfExportService ──→ QuestPDF ──→ PDF      │
│     │                        │                                  │
│     ├── Print ──→ Windows Print Dialog                          │
│     ├── Thermal ──→ ThermalPrintService ──→ ESC/POS ──→ Printer │
│     ├── Email ──→ EmailService ──→ SMTP ──→ Email + PDF         │
│     └── WhatsApp ──→ Browser ──→ wa.me deep link                │
└─────────────────────────────────────────────────────────────────┘
```

### Service Layer

All services are **static classes** with **async methods** using Dapper for data access:

```csharp
// Example: SalesService pattern
public static class SalesService
{
    public static async Task<(bool Success, string Message, Invoice? Invoice)>
        CreateInvoiceAsync(Invoice invoice, List<InvoiceItem> items)
    {
        using var connection = DatabaseHelper.GetConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();
        // ... transactional invoice creation with stock updates
    }
}
```

**Key patterns:**
- All methods return `(bool Success, string Message)` tuples for error handling
- Database transactions for multi-table operations (invoices, POs, returns)
- `DatabaseHelper` wraps Dapper with connection string management
- `Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true` for PostgreSQL naming

### Model Layer

Models are plain C# classes (POCOs) with computed properties for display:

```csharp
public class Invoice
{
    // Database fields
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    // ...

    // Joined fields (populated by Dapper from JOIN queries)
    public string CreatedByName { get; set; }
    public int ItemCount { get; set; }

    // Computed properties (for UI binding)
    public string PaymentMethodDisplay => PaymentMethod switch
    {
        "CASH" => "Cash", "UPI" => "UPI", "CARD" => "Card", _ => PaymentMethod
    };
    public string FormattedTotal => $"₹{TotalAmount:N2}";
}
```

---

## 🗄 Database Schema

OpenPOS uses **PostgreSQL** with **22+ tables** organized into logical groups:

### Core Tables (Authentication & Authorization)

| Table | Columns | Description |
|-------|---------|-------------|
| `roles` | id, name | Role definitions (Admin, Cashier) |
| `users` | id, full_name, email, password_hash, is_active, created_at | User accounts with BCrypt passwords |
| `tenants` | id, name, is_active, created_at | Business/shop entities |
| `user_tenants` | id, user_id, tenant_id, role_id, is_active, created_at | User → Shop → Role mapping |
| `modules` | id, name, key, icon, sort_order, is_active | Feature modules (POS, Inventory, etc.) |
| `role_permissions` | id, role_id, module_id, tenant_id | Permission matrix |

### Business Configuration Tables

| Table | Key Columns | Description |
|-------|-------------|-------------|
| `business_details` | business_name, gstin, pan, currency_symbol, bank_*, upi_*, invoice_prefix, invoice_footer | Complete business profile |
| `tax_slabs` | tax_name, rate, component1_name/rate, component2_name/rate | Tax configuration with splits |
| `email_settings` | smtp_host, smtp_port, use_ssl, sender_email, password | SMTP configuration |
| `payment_gateway_settings` | gateway_name, api_key, api_secret, is_test_mode | Payment gateway credentials |

### Inventory Tables

| Table | Key Columns | Description |
|-------|-------------|-------------|
| `categories` | name, description, sort_order, is_active | Product categories |
| `units` | name, short_name | Units of measurement |
| `suppliers` | name, contact_person, email, phone, gst_number | Supplier directory |
| `products` | name, sku, barcode, hsn_code, cost_price, selling_price, current_stock, min_stock_level | Product catalog |
| `stock_movements` | product_id, movement_type, quantity, previous_stock, new_stock, reference | Full stock audit trail |

### Transaction Tables

| Table | Key Columns | Description |
|-------|-------------|-------------|
| `invoices` | invoice_number, customer_name, subtotal, discount_*, tax_amount, total_amount, payment_method, status | Sales transactions |
| `invoice_items` | product_name, quantity, unit_price, tax_rate, tax_amount, line_total, hsn_code | Invoice line items |
| `invoice_sequences` | tenant_id, last_sequence | Atomic invoice number generation |
| `customers` | name, phone, email, address, gstin | Customer database |
| `purchase_orders` | po_number, supplier_id, subtotal, tax_amount, total_amount, status, expected_date | Purchase orders |
| `purchase_order_items` | product_name, quantity, unit_price, tax_rate, tax_amount, received_quantity | PO line items with receiving |
| `credit_notes` | credit_note_number, invoice_id, total_amount, reason, status | Return credit notes |
| `credit_note_items` | product_name, quantity, unit_price, tax_amount | Credit note line items |

### Entity Relationship Overview

```
tenants (1) ──── (*) products ──── (1) categories
    │                  │
    │                  ├──── (1) tax_slabs
    │                  ├──── (1) units
    │                  └──── (1) suppliers
    │
    ├──── (*) invoices ──── (*) invoice_items
    │           │
    │           └──── (1) users (created_by)
    │
    ├──── (*) purchase_orders ──── (*) purchase_order_items
    │
    ├──── (*) credit_notes ──── (*) credit_note_items
    │
    ├──── (*) customers
    ├──── (*) stock_movements
    ├──── (1) business_details
    ├──── (1) email_settings
    └──── (1) payment_gateway_settings
```

---

## ⚙ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mywinformsapp_db;Username=postgres;Password=postgres"
  },
  "AppSettings": {
    "AppName": "OpenPOS",
    "Version": "1.0.0",
    "Theme": "LIGHT"
  }
}
```

### Connection String Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `Host` | `localhost` | PostgreSQL server address |
| `Port` | `5432` | PostgreSQL port |
| `Database` | `mywinformsapp_db` | Database name |
| `Username` | `postgres` | Database user |
| `Password` | `postgres` | Database password |

### Supported Currencies

| Currency | Symbol | Code |
|----------|--------|------|
| Indian Rupee | ₹ | INR |
| US Dollar | $ | USD |
| Euro | € | EUR |
| British Pound | £ | GBP |
| Australian Dollar | A$ | AUD |
| Canadian Dollar | C$ | CAD |
| *Custom* | *Any* | *Any* |

### Pre-seeded Tax Slabs by Country

<details>
<summary><strong>🇮🇳 India (GST)</strong></summary>

| Slab | Rate | Components |
|------|------|------------|
| GST 0% | 0% | - |
| GST 5% | 5% | CGST 2.5% + SGST 2.5% |
| GST 12% | 12% | CGST 6% + SGST 6% |
| GST 18% | 18% | CGST 9% + SGST 9% |
| GST 28% | 28% | CGST 14% + SGST 14% |

</details>

<details>
<summary><strong>🇺🇸 USA, 🇬🇧 UK, 🇨🇦 Canada, 🇦🇺 Australia</strong></summary>

Country-specific default tax slabs are auto-seeded based on the business country setting.

</details>

---

## 📄 PDF Reports

All PDFs are generated using **QuestPDF** with consistent professional formatting:

### Design Standards

| Property | Value |
|----------|-------|
| Page Size | A4 (Portrait or Landscape) |
| Margins | 30-40pt |
| Header | Business name + report title + date range |
| Table Headers | Color-coded backgrounds (Blue, Teal, Red, Grey) |
| Row Striping | Alternating white/grey rows |
| Footer | Generation timestamp + "OpenPOS" |
| Page Numbers | Current / Total on multi-page reports |

### Invoice PDF Features

| Element | Details |
|---------|---------|
| Business Header | Name, address, GSTIN, phone, email |
| Invoice Badge | Blue "INVOICE" label with number |
| Customer Info | "Bill To" section (when customer provided) |
| Items Table | 6 columns: #, Product (with HSN), Qty, Price, Tax, Total |
| Tax Breakdown | Component-level: CGST 9% + SGST 9% per slab |
| Totals Section | Subtotal, Discount, Tax components, **TOTAL** |
| Bank Details | Account holder, number, bank, branch, IFSC |
| UPI QR Code | Scannable QR with pre-filled amount |
| Footer | Custom text + "Thank you for your business!" |

### Purchase Order PDF Features

| Element | Details |
|---------|---------|
| Teal Color Theme | Distinguished from invoices (blue) |
| Supplier Section | Name, contact person, email, phone, address, GST |
| Items Table | 6 columns: #, Product, Qty, Unit Price, **Tax**, Total |
| Tax Per Item | Shows rate% and calculated amount |
| Status Badge | Draft / Ordered / Received |

---

## 📥 Excel Import

Bulk import data from `.xlsx` files with automatic duplicate detection.

### Products Import

| Column | Letter | Required | Description |
|--------|--------|----------|-------------|
| Name | A | **Yes** | Product name |
| SKU | B | No | Stock Keeping Unit |
| Barcode | C | No | Product barcode |
| HSNCode | D | No | HSN/SAC code |
| Category | E | No | Category name (matched case-insensitively) |
| CostPrice | F | No | Purchase/cost price |
| SellingPrice | G | No | Selling price |
| MRP | H | No | Maximum retail price |
| CurrentStock | I | No | Opening stock quantity |
| MinStockLevel | J | No | Minimum stock threshold |
| Description | K | No | Product description |

### Categories Import

| Column | Letter | Required | Description |
|--------|--------|----------|-------------|
| Name | A | **Yes** | Category name |
| Description | B | No | Category description |

### Suppliers Import

| Column | Letter | Required | Description |
|--------|--------|----------|-------------|
| Name | A | **Yes** | Company name |
| ContactPerson | B | No | Contact person |
| Email | C | No | Email address |
| Phone | D | No | Phone number |
| Address | E | No | Street address |
| City | F | No | City |
| State | G | No | State |
| PinCode | H | No | PIN/ZIP code |
| GSTNumber | I | No | GST registration |

**Import behavior:**
- Duplicates are **skipped** (matched by name or SKU)
- Categories are auto-matched by name (case-insensitive)
- Reports import count, skip count, and errors

---

## 📦 Installation

### Option 1: Windows Installer (Recommended)

OpenPOS includes a full **Inno Setup** installer that bundles everything:

**What the installer does:**
1. Copies self-contained application to `Program Files`
2. Optionally installs PostgreSQL 17 (if not detected)
3. Creates and initializes the database
4. Creates desktop and Start Menu shortcuts
5. Registers uninstaller

**Build the installer:**

```powershell
cd installer
.\build-installer.ps1
```

**Output:** `installer/output/OpenPOS-Setup-1.0.0.exe`

### Option 2: Manual Setup

```bash
# 1. Clone
git clone https://github.com/saurabhwebdev/openpos.git
cd openpos

# 2. Database setup
psql -U postgres -c "CREATE DATABASE mywinformsapp_db;"
psql -U postgres -d mywinformsapp_db -f installer/schema.sql
psql -U postgres -d mywinformsapp_db -f migration.sql

# 3. Run
dotnet run

# 4. Build for production
dotnet publish -c Release -r win-x64 --self-contained true
```

### Option 3: Database Only (setup-db.bat)

```bash
cd installer
.\setup-db.bat
```

This auto-detects PostgreSQL installation, creates the database, and runs the schema.

---

## 🔧 Troubleshooting

<details>
<summary><strong>Application won't start / database connection error</strong></summary>

1. Verify PostgreSQL is running: `pg_isready -h localhost -p 5432`
2. Check `appsettings.json` connection string
3. Ensure database exists: `psql -U postgres -c "SELECT 1 FROM pg_database WHERE datname='mywinformsapp_db';"`
4. Verify schema is loaded: `psql -U postgres -d mywinformsapp_db -c "\dt"` should show 18+ tables

</details>

<details>
<summary><strong>Email sending fails</strong></summary>

1. Go to Settings → Email and verify SMTP settings
2. For Gmail: use an [App Password](https://support.google.com/accounts/answer/185833) (not your regular password)
3. Click "Test Email" to verify configuration
4. Check that "Enable Email" toggle is on

</details>

<details>
<summary><strong>Thermal printer not detected</strong></summary>

1. Ensure the printer is connected via USB and powered on
2. Install the printer's Windows driver
3. Verify it appears in Windows Settings → Printers & Scanners
4. Try both 80mm and 58mm paper width options

</details>

<details>
<summary><strong>PDF generation error</strong></summary>

1. QuestPDF Community License is automatically configured
2. Ensure `~/Downloads` folder exists and is writable
3. Check that no other process has the PDF file locked

</details>

<details>
<summary><strong>Excel import fails or skips all rows</strong></summary>

1. Use `.xlsx` format (not `.xls` or `.csv`)
2. First row must be headers matching the expected column names
3. Product names in column A are required
4. Check the import result message for specific skip/error counts

</details>

---

## 🤝 Contributing

Contributions are welcome! OpenPOS is open-source and we'd love your help.

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/amazing-feature`
3. **Commit** your changes: `git commit -m 'Add amazing feature'`
4. **Push** to the branch: `git push origin feature/amazing-feature`
5. **Open** a Pull Request

### Development Guidelines

- Follow existing code patterns (static service classes, code-behind views)
- Use Dapper for all database operations (no raw ADO.NET)
- All data must be scoped by `tenant_id`
- Use `(bool Success, string Message)` return tuples for error handling
- WPF views use code-behind (not strict MVVM)
- Test with PostgreSQL 17+

---

## 📄 License

This project is licensed under the **MIT License** — you're free to use, modify, and distribute it.

See the [LICENSE](LICENSE) file for details.

---

<div align="center">

### Built with

<p>
  <img src="https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET" />
  <img src="https://img.shields.io/badge/WPF-0078D4?style=for-the-badge&logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Material_Design-757575?style=for-the-badge&logo=materialdesign&logoColor=white" alt="Material Design" />
  <img src="https://img.shields.io/badge/QuestPDF-FF6F00?style=for-the-badge" alt="QuestPDF" />
</p>

<sub>Made by <a href="https://github.com/saurabhwebdev">Saurabh Thakur</a></sub>

<br />

**If you find OpenPOS useful, please give it a ⭐ on GitHub!**

</div>
