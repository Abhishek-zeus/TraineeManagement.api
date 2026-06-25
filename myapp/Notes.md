## DAY 2 Phase 2
## To connect mysql server with WSL, run the following commands on CMD
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p

mysql> CREATE USER 'root'@'172.23.%' IDENTIFIED BY 'root'; (Create a user with password root)
Query OK, 0 rows affected (0.05 sec)

mysql> GRANT ALL PRIVILEGES ON *.* TO 'root'@'172.23.%' WITH GRANT OPTION; (Grant him all permissions)
Query OK, 0 rows affected (0.05 sec)

mysql> FLUSH PRIVILEGES; (Save Changes)
Query OK, 0 rows affected (0.05 sec)

mysql> EXIT; 
and then run on WSL
mysql -h 172.23.224.1 -u root -p --protocol=tcp

## to install EF core CLI
dotnet tool install --global {toolname}....(in this case dotnet-ef)

## Add .NET in wsl using the following commands
sudo add-apt-repository ppa:dotnet/backports -y ....(Add the .NET backports package)
sudo apt update (Update the package manager)
sudo apt install -y dotnet9

## Added the path of dotnet tools permanently
echo 'export PATH="$PATH:HOME/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc

## To add package
dotnet add package {package name}

## For hashing password and verifying
use PasswordHasher class 
Signup --> HashPassword()
Login --> VerifyHashedPassword(), PasswordVerificationresult.Success

..................................................................................
## Day3 Phase 2

## Install JWT packages
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt

## Add JWT settings in appsettings.json to avoid hardcoding
  "Jwt": { 
    "Key": "ThisIsMySuperSecretKey123456789", 
    "Issuer": "TraineeManagementApi", 
    "Audience": "TraineeManagementClient", 
    "ExpiryMinutes": 60 
  }

## IConfiguration interface in ASP.Net core
It looks for JSON files, environment variables, command-line arguments to access the configurations (Helps in avoiding hardcoding)

## Claims class in ASP.Net core
Inbuilt class that is used to store user claims in JWT tokens such as id, username, role 

## To protect all the APIs
Add JWT authentication configuration such as TokenValidationParameters in the program.cs and also add UseAuthentication and UseAuthorization

## Add the Swagger JWT Support
Use builder.Services.AddOpenApiDocument() configuration in Program.cs
Also Add "Bearer {token}" in the Authorize box to get Authorized

## Pagination
Use two variables PageNumber (Up to which You want to Skip) and PageSize (Size of page contents you want) and use two methods Skip() and Take()
use formula Skip((PageNumber-1) * pageSize).Take(pageSize).ToListAsync()

## To check if a Int has value or is null 
Use pageNumber.HasValue which will return boolean

## Alternative of ternary operator
null-coalescing operator that will assign the value if it is null
int validPageNumber = pageNumber ?? 1;

## To Add .env
- Create a .env file and add configurations of JWT from appsettings.json and remove from there.
- Jwt:Key becomes Jwt__Key
- install package of DotNetEnv
- Add the Env.Load() and using DotNetEnv package