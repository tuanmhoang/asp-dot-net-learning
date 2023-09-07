# How to do things, a note to remember

## 1. To authenticate JWT

Reference: https://www.youtube.com/watch?v=v7q3pEK1EA0

Steps to do

Models: 
- Create entity
- Create DTO

Controllers:
- Register
- Login

Setup: 
- Setup secret

Other things:
- Create password hash 
- Validate password

---

## 2. To connect DB

Reference: https://www.youtube.com/watch?v=2bsRQIhtTxs&t=820s

Steps to do:
- Create DB
- Install NuGet packages: `entityframeworkcore.tools` and `entityframeworkcore.sqlserver`
- use Scaffold-DbContext to generate models


Create DB script

```
drop table if exists [dbo].[USER];

drop table if exists [dbo].[ROLE];

create table [dbo].[ROLE](
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    role VARCHAR(100)
);

CREATE TABLE [dbo].[USER] (
      id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
      username VARCHAR(50) NOT NULL,
      password VARCHAR(255) NOT NULL,
      password_salt VARCHAR(255) NOT NULL,
      firstname VARCHAR(255),
      lastname VARCHAR(255),
      photo varbinary(MAX) default NULL,
      roleid int FOREIGN KEY REFERENCES [dbo].[ROLE](id),
 );

INSERT INTO [dbo].[ROLE] 
  ( role)
VALUES
  ('admin'), 
  ('student')
;

```

Open Package Manager Console and use `use Scaffold-DbContext` to generate models

```
Scaffold-DbContext -Connection "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StudentManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\Entities -force
```

Add to `Program.cs`

```
builder.Services.AddDbContext<StudentManagementContext>();
```

---
## 3. How to add more details on Swagger

https://stackoverflow.com/questions/52883466/how-to-add-method-description-in-swagger-ui-in-webapi-application

> For ASP.Net Core projects:
> 
> install nuget package Swashbuckle.AspNetCore.Annotations
> 
> Use SwaggerOperation attribute for a methods like [SwaggerOperation(Summary = "Write your summary here")]
> 
> Enable annotations in Startup method ConfigureServices like the following:
> 
> services.AddSwaggerGen(c =>
> {
>     c.EnableAnnotations();
>     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
> });
> To exclude public method from appearing in swagger ui use attribute [ApiExplorerSettings(IgnoreApi = true)]. It is useful cause these methods can break swagger for some reason.
> 
> Launch project, go to localhost:[port number]/swagger and enjoy.

---
# 4. Containerize using Docker

Reference:

- https://www.twilio.com/blog/containerize-your-aspdotnet-core-application-and-sql-server-with-docker
- https://www.ezzylearning.net/tutorial/dockerize-asp-net-core-api-and-sql-server
- https://www.youtube.com/watch?v=hpLvXNASyTI&t=873s

Define in `appsettings.json`

```
  "ConnectionStrings": {
    "StudentsDb": "Server=sql_server2022;Database=StudentsDb;User Id=SA;Password=A&VeryComplex123Password;MultipleActiveResultSets=true"
  },
```

Define in `Program.cs`

```
builder.Services.AddDbContext<StudentManagementContext>(options =>  options.UseSqlServer(builder.Configuration.GetConnectionString("StudentsDb")));

```

To have some hardcoded data

---
## 99. Notes

https://stackoverflow.com/questions/5613898/storing-images-in-sql-server

> - if your pictures or document are typically below 256KB in size, storing them in a database VARBINARY column is more efficient
>
> - if your pictures or document are typically over 1 MB in size, storing them in the filesystem is more efficient (and with SQL Server 2008's FILESTREAM attribute, they're still under transactional control and part of the database)

---

# Trouble & How to fix

```
The following file(s) already exist in directory 'C:\working\2.Projects\7.siteCoreK11\0_study\github\asp-dot-net-learning\StudentManagement\StudentManagement\Models\Entities': User.cs. Use the Force flag to overwrite these files.
```

To fix add `-force`

Reference: https://stackoverflow.com/questions/41233300/update-entity-class-in-asp-net-core-entity-framework

---

`cors` problem

To fix add to `Program.cs`

```
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7292",
                                              "https://localhost:7292/swagger/index.html");
                      });
});

app.UseCors(MyAllowSpecificOrigins);
```

---

Windows & Mac

- Enable support Rosetta on Mac Docker
- Add support build for the platform

---

Swagger desc

https://stackoverflow.com/questions/52883466/how-to-add-method-description-in-swagger-ui-in-webapi-application

https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio

---

cors for localhost

https://stackoverflow.com/questions/57530680/enable-cors-for-any-port-on-localhost