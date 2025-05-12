Business Logic Document: Inventory Management System
Project Purpose and Goals
The Inventory Management System is designed to streamline and automate the management of electronic products in a retail environment. Its main goal is to maintain accurate stock levels, enable efficient product tracking, and provide role-based access for users to manage inventory-related operations.
User Roles
Admin
  Has full access to the system
  Can manage (add, update, delete) products, categories, and brands
  Can view reports and monitor inventory levels
  Manages users and roles
Sales Staff
 Can create sales and view product details
 Cannot modify product details or inventory settings
Page/Module Functions
Dashboard
 Displays key metrics: total products, low stock alerts, recent sales
 Quick access to recent activity and shortcuts
Product Management
  Admins can add, update, delete products
  Products are linked to categories and brands
  Each product has stock quantity, price, discount price, and optional images
Category & Brand Management
  Admins can create, update, and remove product categories and brands
  Categories help classify products, brands identify manufacturers
Sales Module
  Records new sales transactions
  Automatically decreases product stock when a sale is completed
  Tracks sale time, product sold, and quantity
User Management
  Admins can manage user accounts and assign roles
  Users can upload profile pictures and manage their info
Inventory Tracking
  Tracks stock changes with date/time stamps
  Marks products as active/inactive and deleted when necessary
System Behavior
Upon adding a new product, it is assigned to a category and brand with initial quantity.
When a sale is recorded:
  The corresponding product’s stock is decreased automatically.
  If quantity reaches zero, the product may be marked as 'Out of Stock'.
Admins receive notifications or indicators for low stock products.
Soft deletes (IsDeleted = true) are used for logical deletion without removing data.
Timestamps (CreatedDate, UpdatedDate) are used for audit tracking on all entities.
