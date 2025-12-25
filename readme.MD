📦 Required Packages & Setup

# Install NuGet packages through dotnet ClI

        dotnet add package Microsoft.EntityFrameworkCore --version 9.0.1
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.1
        dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.1

        dotnet add package BCrypt.Net-Next --version 4.0.3
        dotnet add package CloudinaryDotNet --version 1.27.5
        dotnet add package Razorpay --version 3.1.4
        dotnet add package System.IdentityModel.Tokens.Jwt --version 8.3.1
        dotnet add package dotenv.net --version 3.2.1

# Install EF Core CLI tools (once)

    dotnet tool install --global dotnet-ef

# To check the list of tools installed globally

        dotnet tool list --global

# Entity Framework migrations (using .NET CLI):

        dotnet ef migrations add "message"
        dotnet ef database update

# Using Visual Studio Package Manager Console:

        Add-Migration InitialCreate
        Update-Database

# If you have multiple DbContexts:

        Add-Migration InitialCreate -Context YourDbContextName
        Update-Database

# ADDITIONAL INFO

🔐 Roles

0 → Admin (you'll need to assign this manually through your database)
1 → User
1 → Shopkeeper (you can assign this in admin dashboard too)

        (       || Admin(s) manage the whole platform ||       )


// Screenshots
