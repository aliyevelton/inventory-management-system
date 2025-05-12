# Inventory Management System

An Inventory Management System built using ASP.NET Core and Entity Framework Core, designed for an electronic store. The system allows for efficient management of products, categories, brands, stock tracking, and user roles.

## Features

- **User Management**
  - Identity-based authentication and authorization
  - Role-based access (Admin, Manager, etc.)
  - Profile picture upload

- **Product Management**
  - Add, update, and delete products
  - Manage product brands and categories
  - Attach multiple images to each product
  - Track quantity, pricing (current, old, and discounted)

- **Category and Brand Management**
  - CRUD operations for categories and brands
  - Category code generation
  - Brand logo support

- **Inventory Tracking**
  - Stock updates based on sales
  - Active/inactive and soft delete status for products

- **Database & Relationships**
  - Entity Framework Core with SQL Server
  - Proper use of foreign keys and relationships (1:N)

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core
- Identity Framework
- SQL Server
- HTML/CSS/Bootstrap (for views)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/aliyevelton/inventory-management-system.git
