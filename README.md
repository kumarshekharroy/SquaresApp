# Squares App : .Net Core Web API Application
 
 This is a simple but performant layered web API application which enable its user/consumer to find out sets of points that make squares and how many squares can be drawn with the given set of points.
 User can import a list of points, delete a point from an existing list, retrieve all the stored points and the squares identified using exposed REST endpoints.
 JWT bearer token is being used for authentication and authorization of request. 
 It is currently using MSSQL server as database but can easily be changed to any other popular relational database of choice by changing few lines of code in startup.cs file.


## Steps to Run/Debug the Project :

1. Install Microsoft .Net Core version 5.0 SDK/Runtime  from  https://dotnet.microsoft.com/download/dotnet/5.0

2. Install MSSQL Server from https://www.microsoft.com/en-in/sql-server/sql-server-downloads

3. Install Git from https://git-scm.com/downloads

4. Create A Folder/Directory on local machine. Open Git bash/cmd prompt/powershell and navigate to the directory using `cd {dirname}` command.

5. Clone this github repository in the folder/dir by running : git clone https://github.com/kumarshekharroy/SquaresApp.git command.

6. Navigate to SquaresApp/SquaresApp/SquaresApp.API  folader/dir by running `cd SquaresApp/SquaresApp/SquaresApp.API` command.

7. Execute `dotnet run` commnad to run the application. (It will fetch all the required dependencies and build the project automatically).

8. Connection string for database can be changed from Appsettings.json file.

9. The migrations are already created. So, There is no need to create the same manually. The application will automatically apply those migrations on first run.

10. Go to http://localhost:5000 or https://localhost:5001 to play with swagger UI 


## Points to know :

1. Seed data for User and Point are included in the migration. So, The migration will by default create two users `User_1: {username:"Admin",password:"Admin"}, User_2: {username:"User",password:"User"}` and their respective few records in Points tables.
 
2. TTL of JWT tokens (fresh) and JWT signing key/secret can be changed from Appsettings.json file.

3. All the layers of the project are Unit tested by using Xunit framework.

4. Different environments like `Development` and `Prodution` are supported. e.g. Development Env. specific settings can be modified or overridden by Appsettings.development.json. 

5. An username is unique in the system. Hence a new user with already taken username cann't be created/registered again.

6. SHA256 hashing algorithm is used to securely store user password in the database. 
 
7. Register and Login are open endpoints. Hence no jwt token is required in call to these endpoints.

8. Custom intelligent Distributed(using Redis)/InMemory(As fallback to distributed) Caching is implemented to improve scalability, the performance and  lower the response time. 

9. Cache SlidingExpirationTime and AbsoluteExpiration Time can be changed from `Cache` section of `appsettings.json` file. Set these value to `0` to disable caching.

10. To make the caching distributed/global please set a valid Redis connection string in `ConnString` field present in the `RedisConfig` section of `appsettings.json` file. Leave the field empty/blank to make caching InMemory.