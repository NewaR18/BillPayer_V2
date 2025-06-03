# BillPayer_V2

To run the project successfully, follow the steps below. All database procedures, migrations, and menu data will be automatically applied when the application starts—provided the connection string is configured correctly.

Step 1: Configure User and Roles
  a) Register a User
  Navigate to the Register menu in the application and create a new user.
  
  b) Insert Roles Manually
  Run the following SQL query to insert roles into the database manually. (Note: This will be automated in future updates.)
      ==>
      INSERT INTO [ASPNETROLES](ID,[DISCRIMINATOR]
        ,[LISTOFMENUID]
        ,[NAME]
        ,[NORMALIZEDNAME]
        ,[CONCURRENCYSTAMP]) values (1,'ApplicationRole','1,2,3,4,5,6,7','Admin','ADMIN',GETDATE())
        
  c) Assign Role to User
  Copy the UserId of the newly registered user from the AspNetUsers table.
  Then run the following query (replace the Guid with your actual UserId):
      ==>
      INSERT INTO [AspNetUserRoles] values ('c26db347-ad1e-47a6-bf3d-10c64680b7a2',1)
      (Replace above Guid with UserId)

Step 2: secrets.json is not present in your local pc. So We need to create that. Steps for creating that :
  a) Right Click on BillPayer project and Click on 'Manage User Secrets'.
  b) Paste and Fill following json:
    ==>
        {
          "Authentication": {
            "Google": {
              "ClientId": "", //[Refer to https://console.cloud.google.com/]
              "ClientSecret": "" //[Refer to https://console.cloud.google.com/]
            }
          },
          "mailDetails": {
            "Port": 587,
            "Host": "smtp.gmail.com",
            "Email": "", [Your Email]
            "Username": "", [Your Email]
            "Password": "" //[Refer to https://myaccount.google.com/u/4/apppasswords]
          }
        }

After completing both steps above, your application should be ready to run.
Make sure your connection string is correctly configured in appsettings.json or user secrets
