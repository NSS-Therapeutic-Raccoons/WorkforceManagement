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
3. Create a Bangazon.db file in the project.
4. Open Visual Studio, go to File => Open => Project/Solution... and open the WorkForceManagement/BangazonWorkForce.sln file.
5. Open the Solution Explorer View and under the BangazonWorkforce project, add a new JSON file, called appsettings.json.
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
8. Paste the connection string into the appsettings.json file you just created under connection strings/defaultconnection. Here:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "---->PASTE HERE<----"
  },
  ...
}
```
9. Change "data source" to "server" in the connection string.

Here is another handy gif showing this continued process:

![Connection String Demo](https://github.com/NSS-Therapeutic-Raccoons/WorkforceManagement/blob/rb-readmebuild/readmegifs/connectionstring.gif?raw=true)

3. Open SSMS and run the SQL scrip in the repo, [SQL Script](https://github.com/NSS-Therapeutic-Raccoons/BangazonAPI/blob/master/SQL/Bangazon.sql), then execute the script into a local database.
* Open the project in Visual Studio, look for SQL Server Object Explorer and navigate to your local database. Right click on the name and click on properties. Look for "Connection String" and copy the value to the right of it.
* Paste that value over "Default Connection" in appsettings.json
* Look at the top bar and find the green arrow "play button", and make sure BangazonAPI is selected and click the arrow.
* Open Postman and use "http://localhost:5000/api/" as the template for getting data.
** Navigate the controllers to find syntax for GET, POST, PUT, and DELETE for each type.

## Built With

* C#
* Dapper
* SQL

## Authors

* **Ricky Bruner** - [ricky-bruner](https://github.com/ricky-bruner)
* **Klaus Hardt** - [KHardt](https://github.com/KHardt)
* **Jeremiah Pritchard** - [jeremiah3643](https://github.com/jeremiah3643)
* **Mike Parrish** - [thatmikeparrish](https://github.com/thatmikeparrish)

## Acknowledgments

* Special thanks to **Andy Collins** - [askingalot](https://github.com/askingalot) for putting up with us!

## Notes

* Here is the ERD for our project.
![Bangazon ERD](https://github.com/NSS-Therapeutic-Raccoons/BangazonAPI/blob/master/BangazonAPI-ERD.png?raw=true)