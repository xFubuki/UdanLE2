# Lab Exercise 2: C# .NET 6 Class Library

## Architecture Overview

This solution is the backend foundation for a blogging application. It contains a reusable .NET 6 class library for models and Dapper-based SQL Server access, a console application for testing the library with dependency injection, and a SQL Server database project for the schema.

## Prerequisites

- Visual Studio 2022 Community
- .NET 6 SDK
- SQL Server Express LocalDB
- SQL Server Data Tools

## NuGet Packages

BlogDataLibrary:

- Dapper
- Microsoft.Extensions.Configuration
- System.Data.SqlClient

BlogTestUI:

- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.DependencyInjection

UdanBlogDB:

- MSBuild.Sdk.SqlProj

## Solution Structure

```text
UdanLE2.sln
  BlogDataLibrary
    Models
      UserModel.cs
      PostModel.cs
      ListPostModel.cs
    Database
      ISqlDataAccess.cs
      SqlDataAccess.cs
    Data
      ISqlData.cs
      SqlData.cs
    BlogDataLibrary.csproj
  BlogTestUI
    Program.cs
    appsettings.json
    BlogTestUI.csproj
  UdanBlogDB
    dbo
      Tables
        Users.sql
        Posts.sql
    UdanBlogDB.sqlproj
```

## Database Schema

`dbo.Users` stores registered users with a primary key on `Id` and a unique constraint on `UserName`.

`dbo.Posts` stores blog posts with a primary key on `Id` and the foreign key `FK_Posts_Users` from `Posts.UserId` to `Users.Id`. One user can have many posts.

## Publish the Database as BlogDB

1. Open `UdanLE2.sln` in Visual Studio 2022.
2. Ensure SQL Server Data Tools is installed.
3. Right-click `UdanBlogDB`.
4. Select `Publish`.
5. Set the target connection to `(localdb)\MSSQLLocalDB`.
6. Set the database name to `BlogDB`.
7. Click `Publish`.
8. Confirm `BlogDB` appears in SQL Server Object Explorer.

## Run BlogTestUI

1. Publish the database project as `BlogDB`.
2. Confirm `BlogTestUI/appsettings.json` contains:

```json
{
  "ConnectionStrings": {
    "SqlDb": "Server=(localdb)\\MSSQLLocalDB;Database=BlogDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Run the console application:

```powershell
dotnet run --project BlogTestUI\BlogTestUI.csproj
```

4. Use the menu to register a user, log in, add posts, list posts, and show post details.

## Required Screenshots

Capture these screenshots for submission:

- Users table design with Solution Explorer visible
- Posts table design with Solution Explorer visible
- Published `BlogDB` in SQL Server Object Explorer
- Inserted user row with SQL Server Object Explorer visible
- Relevant class files and console output screenshots

## Submission Reminder

Submit the required screenshots and the GitHub repository link in a Microsoft Word document.
