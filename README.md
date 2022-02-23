# SendEmailSQLCLR

Send Email from a SQL CLR function

Although you can, and should use sp_send_dbmail to send email from SQL server, it's often not quite as flexible as you need it to be. 
So here is a .NET CLR Function that you can install in your MSSQL server, in order to send an email using whatever additional configuration that you need. 

You need to run these commands before installing the assembly
```
EXEC sp_changedbowner 'sa'
ALTER DATABASE <your-database> SET trustworthy ON


CREATE ASSEMBLY [SendEmailCLR]
    AUTHORIZATION [dbo]
    FROM 0x4D5A90000300000004000000FFFF000.....
    WITH PERMISSION_SET = UNSAFE;
  
CREATE PROCEDURE [dbo].[SendEmail]
@smtpServer NVARCHAR (MAX) NULL, @smtpUsername NVARCHAR (MAX) NULL, @smtpPassword NVARCHAR (MAX) NULL, @from NVARCHAR (MAX) NULL, @to NVARCHAR (MAX) NULL, @subject NVARCHAR (MAX) NULL, @body NVARCHAR (MAX) NULL
AS EXTERNAL NAME [SendEmailCLR].[SendEmailCLR].[SendEmail]
```
 
