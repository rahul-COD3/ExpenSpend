# ExpenSpend **Project - Local Setup Guide (Windows)**

This guide will walk you through the steps to set up the **ExpenSpend** in local.

## **Prerequisites**

Before you begin, ensure that you have the following prerequisites installed on your system:

- **[Git](https://git-scm.com/downloads)**
- **[.NET Core SDK-8](https://dotnet.microsoft.com/download)**
- **[PostgreSQL](https://www.postgresql.org/download/windows/)**

## **Step 1: Clone the Repository**

1. Open a command prompt or Git Bash.
2. Change to the directory where you want to clone the project.
3. Execute the following command to clone the ABP.IO repository:

```
git clone https://github.com/rahul-codespace/ExpenSpend.git
```

## **Step 2: Checkout to dev branch**

1. Open a command prompt or Git Bash.
2. Change to the project's root directory.
3. Checkout to development branch for the latest code or dev testing.

```
git checkout dev
```

## **Step 3: Change Database Connections String**

1. Open the **`ExpenSpend.Web/appsettings.Development.json`** file.
2. Locate the **`ConnectionStrings`** section.
3. Change the According to your database connection string. (mostly required to change the password)

## **Step 4: Run the Application**

1. Open a command prompt or Git Bash.
2. Change to the project's root directory.
3. Execute the following command to run the application:

```
cd ExpenSpend.Web
dotnet run
```

4. Open a browser and navigate to **`http://localhost:5173`** or **`https://localhost:5173`**.

## Congratulations! You have successfully set up the **ExpenSpend** project in local.
