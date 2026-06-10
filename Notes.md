
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