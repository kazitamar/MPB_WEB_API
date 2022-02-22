MPB - installation instructions:
1. Install SQL Express: https://www.microsoft.com/en-us/Download/details.aspx?id=101064
2. Install SSMS: https://aka.ms/ssmsfullsetup
3. In SSMS run "Create DB Objects.sql" script
4. Install VS: https://visualstudio.microsoft.com/vs/
5. Download MPB_WEB_API directory and open "web_api_me.sln" file with VS
6. Edit AppSettings:DbConnection in "appsettings.json" file - change SERVER value.
7. Run the project and send your requests by the default Swagger or by Postman: https://www.getpostman.com/collections/1f5be8252f53a7b03df3
--------------------------------------------------------------------------------------------------------------------------------------------
My arbitrary rule (for 7.c section in Task Definition file) is to forbid some words in post content. current forbidden words are : "zzz", "aaa". but it can be replaced to curses, non PC words and etc. 
