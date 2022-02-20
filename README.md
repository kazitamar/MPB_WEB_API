‏MPB - installation instructions:
1. Install SQL Express: https://www.microsoft.com/en-us/Download/details.aspx?id=101064
2. Install SSMS: https://aka.ms/ssmsfullsetup
3. In SSMS run "Create DB.sql" script
4. In SSMS run "Create DB Objects.sql" script
5. Install VS: https://visualstudio.microsoft.com/vs/
6. Download MPB_WEB_API directory and open "web_api_me.sln" file with VS
7. Right click on web_api_me project -> Manage NuGet packages - > install Newtonsoft.Json and System.Data.SqlClient 
8. Run the project and send your requests by the default Swagger or by Postman: https://www.getpostman.com/collections/1f5be8252f53a7b03df3
