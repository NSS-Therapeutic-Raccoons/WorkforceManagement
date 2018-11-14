# Bangazon Workforce Management Site

**The Details**

Your team is reponsible for building an internal application for our human resources and IT departments. The application will allow them to create, list, and view Employees, Training Programs, Departments, and Computers.

You will be building an ASP.NET Web Application using Visual Studio on Windows, using SQL Server as the database engine. You will be learning the Razor templating syntax. You will be learning how to use view models for defining the data to be used in a Razor template. You will also be learning the concept of integration testing. Your tests will perform GET and POST requests to your application and verify that each view works as expected.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

What things you need to install the software and how to install them

* Visual Studio
* Microsoft SQL Server Managment Studio 2017


### Installing & Running

A step by step series of examples that tell you how to get a development env running

1. **Fork** this Repo.

2. Download or Clone the Repo to your local machine.

3. Create a `Bangazon.db` file in the project.

4. Open Visual Studio, go to `File => Open => Project/Solution...` and open the `WorkForceManagement/BangazonWorkForce.sln` file.

5. Open the Solution Explorer View and under the BangazonWorkforce project, add a new JSON file, called `appsettings.json`.

6. This into the file:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
 Here is a gif visualizing this process:

![Appsettings Demo](https://github.com/NSS-Therapeutic-Raccoons/WorkforceManagement/blob/rb-readmebuild/readmegifs/appsettingsjson.gif?raw=true)

7. Next, open the SQL Server Object Explorer view window. Navigate to the Bangazon server and right click to select **properties**. In the properties window that pops up, find ***Connection String*** and copy the connection string.

8. Paste the connection string into the `appsettings.json` file you just created under connectionstrings/defaultconnection. Here:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "---->PASTE HERE<----"
  },
  
}
```

9. Change "data source" to "server" in the connection string.

Here is another handy gif showing this continued process:

![Connection String Demo](https://github.com/NSS-Therapeutic-Raccoons/WorkforceManagement/blob/rb-readmebuild/readmegifs/connectionstring.gif?raw=true)

10. Inside of the `BangazonWorkforce.IntegrationTests` project, create a new class file called `Config.cs`. 

11. Inside of `Config.cs`, paste this text:
```cs
namespace BangazonWorkforce.IntegrationTests
{
    public static class Config
    {
        public static string ConnectionSring
        {
            get
            {
                return "__PLACE CONNECTION STRING HERE__";
            }
        }
    }
}
```

12. Copy your connection string from `appsettings.json` and paste it over the "__PLACE CONNECTION STRING HERE__" text from the return above.

Visualized here:

![Config Demo](https://github.com/NSS-Therapeutic-Raccoons/WorkforceManagement/blob/rb-readmebuild/readmegifs/configcs.gif?raw=true)

13. Be sure to save all files, then load up SQL Server Management Studio. Inside of there, run the SQL script in this repo, [SQL Script](https://github.com/NSS-Therapeutic-Raccoons/WorkforceManagement/blob/rb-readmebuild/BangazonWorkforce/bangazon.sql), then execute the script into the Bangazon database.

14. Verify that the `csproj` file inside of `BangazonWorkforce` looks like this:
```cs
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="1.50.5" />
    <PackageReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.AspNetCore.Razor.Design" Version="2.1.2" PrivateAssets="All" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="2.1.1" />
  </ItemGroup>

</Project>
```
and that the `csproj` file inside of `BangazonWorkforce.IntegrationTests` look like this:
```cs
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>

    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="1.50.5" />
    <PackageReference Include="Microsoft.AspNetCore.App" Version="2.1.5" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="2.1.3" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.8.0" />
    <PackageReference Include="xunit" Version="2.3.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.3.1" />
    <PackageReference Include="AngleSharp" Version="0.9.10" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\BangazonWorkforce\BangazonWorkforce.csproj" />
  </ItemGroup>

</Project>
```

14. Run the project. Verify the paths work in the browser and open the Test Explorer window. Verify that all tests run properly.



15. Congratulate yourself for completing this process and even bothering to look at this repo. You **MUST** be seriously into C#/.NET. It takes a brave soul.

## Built With

* C#
* Dapper
* SQL
* ASP.NET Core MVC
* ASP.NET Razor Design Templates

## Authors

* **Ricky Bruner** - [ricky-bruner](https://github.com/ricky-bruner)
* **Klaus Hardt** - [KHardt](https://github.com/KHardt)
* **Jeremiah Pritchard** - [jeremiah3643](https://github.com/jeremiah3643)
* **Daniel Figueroa** - [Figamus](https://github.com/Figamus)

## Acknowledgments

* Special thanks to **Andy Collins** - [askingalot](https://github.com/askingalot) for being Andy Collins.

## Notes

* Here is the ERD for our project.
![Bangazon ERD](https://github.com/NSS-Therapeutic-Raccoons/BangazonAPI/blob/master/BangazonAPI-ERD.png?raw=true)