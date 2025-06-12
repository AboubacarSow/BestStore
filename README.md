# 🛍️ BestStore E-Commerce Web Application (From Udemy Course)
<img src="/src/favicon.png" alt="BestStore Logo" width="80">

#### BestStore Project is An ASP.NET Core MVC e-commerce platform developed as part of the Udemy course "ASP.NET CORE MVC - Build an E-Commerce Web Application".

## 🌟 Features

### 🛒 Core Functionality
- **User registration/authentication based on Role**
- **User Edit/Reset Password**
- **Product catalog with categories**
- **Shopping cart management**
- **Order processing system**
- **Multiple payment options (Cash/Stripe)**


### ✉️ Email System
- **Reset Password Email**
- **Payment Confirmation Email**
- **SMTP(Simple Mail Transfert Protocol) integration (SendGrid)**

### 💳 Payment Integration
- **'Stripe API' integration for credit card processing**
- **Sandbox mode for safe transaction testing**
- **Secure payment flow with PCI compliance**


## 🛠️ Technologies Used
- **Backend**: ASP.NET Core MVC
- **Frontend**: Bootstrap 5, Razor Views, 
- **Database**: MSSQL SERVER - Entity FrameworkCore
- **Payment**: Stripe API
- **Email**: SendGrid

## 🚀 Getting Started 

### Prerequisites
- .NET 8 SDK : make sure you have .net 8 sdk installed in your machine
- SQL Server
- Stripe API key (for payment processing): create a Stripe account if you don't have yet turn it into SandBox
- SendGrid API key (for emails): create a SendGrid account if you don't have

### Installation
1. **Clone the repository**
   ```bash
   git clone https://github.com/AboubacarSow/BestStore.git

2. **Configurigation:**
- **secrets.json** file: make sure you use this last one and not the appsettings.json to avoid 'git push' if you will push the project to github later, but if not you can use appsettings.json

    ```json 
    {
        "SendGridSettings": {
            "BestStore": "Your_SendGridKey",
            "SenderName": "Best Store",
            "SenderEmail": "beststore@admin.com"
        },
        "StripeSettings": {
            "SecretKey": "sk_test_secretkey",
            "PublishableKey": "pk_test_publishbleky"
        }
    }

3. **Run Database migrations**: Make sure you properly configure your connectionString

    ```bash
    udpate-database || dotnet ef database update
    


##  📸 Project Overview
**Hero Section**
![Hero](/src/hero.PNG)
**Four Newest Products**
![Four Newest Products](/src/newestproduct.PNG)
**Store**
![Store](/src/store.PNG)

### Admin Products
![Products in Admin Side](/src/productsAdmin.PNG)
![Products in Admin Side](/src/productsadmin.PNG)
### Admin Orders
![Orders](/src/orderlist.PNG)
![Orders Details](/src/orderDetails.PNG)
![Orders Details](/src/orderDetails1.PNG)

### Cart Client 
![Cart](/src/cart.PNG)
![Purchasing](/src/fillcart.jpg)
**After purchasing Successfully**
![Success View](/src/successpay.jpg)
**Email to think customer for purchasing and providing payment details**
![Email for your purchase](/src/receiptorders.PNG)

### User Reset Password Email
![Reset Password Request](/src/resetPassword.PNG)

### EF Diagram
![Diagram](/src/efdiagram.PNG)


