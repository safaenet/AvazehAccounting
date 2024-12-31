
# Avazeh Accounting (Older Style Branch)

**Avazeh Accounting** is a comprehensive desktop retail management application developed using **C# .NET** with **Windows Presentation Foundation (WPF)**. This project demonstrates proficiency in software development, application architecture, and database management. I have already sold this software to some of local retail shop back in my country.

It was built to simplify financial management processes for small businesses and individuals, featuring an intuitive user interface, robust backend logic, and scalable architecture.

---

## 🎯 Key Highlights

- **Rich UI Design**: Utilized WPF for a responsive and modern desktop interface.
- **Backend Excellence**: Developed with C#.NET following the MVVM design pattern.
- **Database Integration**: Ensured secure and efficient data management with Microsoft SQL Server and Dapper ORM.
- **Customizability**: Modular and extensible architecture, making it adaptable to additional requirements.
- **Portfolio Project**: A testament to advanced programming and problem-solving skills.

---

## 🛠 Features

- **Comprehensive Accounting Tools**:
  - Manage income, expenses, and budget allocations.
  - Track financial performance and cash flow.

- **Custom Reporting**:
  - Generate detailed financial reports by categories, dates, or types.
  - Export reports in various formats for external analysis.

- **Secure Data Management**:
  - Built-in support for encrypted database connections.
  - Data validation to ensure accuracy and reliability.

- **Scalable Design**:
  - Modular structure, enabling seamless addition of new features.
  - Performance optimization through asynchronous programming and efficient queries.

---

## 🖥️ Technical Stack

### **Languages and Frameworks**
- **C#.NET**: Backend logic and application development.
- **WPF (Windows Presentation Foundation)**: UI/UX design for desktop applications.
- **XAML**: For building declarative UI components.
- **MVVM Architecture**: Implemented for clear separation of concerns.

### **Database Management**
- **Microsoft SQL Server**: Core database for managing user and financial data.
- **Dapper ORM**: Lightweight and efficient data access.
- **SQL Query Optimization**: Ensured high performance for large datasets.

### **Development Tools**
- **Visual Studio**: Primary IDE for development.
- **Git**: Version control system for collaboration and management.

### **Additional Tools**
- **Dependency Injection**: For improved code maintainability and testability.

---

## 🚀 Getting Started

### **Prerequisites**
- **Visual Studio 2019 or later**
- **Microsoft SQL Server 2017 or later**
- .NET Framework 4.7.2 or higher
- .NET 6

### **Installation**
1. **Clone the Repository**
   ```bash
   git clone https://github.com/safaenet/AvazehAccounting.git
   cd AvazehAccounting
   ```

2. **Switch to the OlderStyle Branch**
   ```bash
   git checkout OlderStyle
   ```

3. **Database Setup**
   - Navigate to the `Database` folder for SQL scripts.
   - Execute the scripts on your SQL Server instance to create the schema and seed data.

4. **Update Configuration**
   - Edit the `app.config` file to set your SQL Server connection string.

5. **Build and Run**
   - Open the solution in Visual Studio.
   - Build the project (`Ctrl+Shift+B`).
   - Run the application (`F5`).

---

## 📚 Usage

I hope you don't mind that the language of the application is in Farsi (Persian)
1. **Login**: Use default credentials set up during the database initialization. If `login` as disabled, you need to `signup` first.
2. **Dashboard**: Access different options of the app here.
3. **Modules**:
   - **Create New Invoice**: Create an invoice.
   - **View Invoices**: View already exsisting invoices.
   - **Create New Ledger**: Create a ledger for customers who continuously buy and pay in once. 
   - **View Ledgers**: View all ledgers.
   - **Bank Checks**: Here we put checks that we paid other people and checks that people paid us. The app notifies us when the due date is close.
   - **Products (Goods)**: List of all the products that are being sold. Products can be created/updated/deleted here.
   - **Customers**: List of all customers. Customers can be created/updated/deleted here.
4. **Settings**: Modify `settings` to suit specific needs.

---

## 📂 Project Architecture

- **Model-View-ViewModel (MVVM)**:
  - Ensures separation of logic (ViewModel) from UI (View).
  - Promotes maintainability and testability.

- **Dapper ORM**:
  - Simplifies database operations with concise queries.
  - Improves performance through direct SQL execution.

- **Asynchronous Programming**:
  - Enables smooth UI performance without blocking the main thread.

---

## 💡 Learning Objectives

- Advanced C# programming techniques.
- Implementation of the MVVM architecture for enterprise applications.
- Integration of WPF with Microsoft SQL Server using ORM.
- Building secure and scalable desktop applications.

---

## 💡 Additional Information

- **Different Platforms**: I have rebuilt this project in a full-stack form using `Java` for back-end and `Angular` for its front-end. You can explore my public repositories to visit them.

---

## 📜 License

This project is licensed under the **MIT License**. See the `LICENSE` file for more details.

---

## 🧑‍💻 About Safa Dana

**Safa Dana** is a results-driven software developer specializing in backend development, IoT, and scalable systems. This project reflects a strong foundation in .NET technologies and a commitment to delivering efficient software solutions.  
- GitHub: [safaenet](https://github.com/safaenet)  
- LinkedIn: [My Profile](www.linkedin.com/in/safa-dana)

