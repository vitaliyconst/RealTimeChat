# RealTimeChat
What to add in future:
Secrets Management:
Configure application secrets using Azure Key Vault or by creating an appsettings.json file containing placeholder values. Then, create a separate appsettings.Development.json file that includes all sensitive information such as API keys and connection strings.
Ensure that appsettings.Development.json is excluded from source control by adding it to .gitignore.

Database Access:
Create a dedicated SQL user group with appropriate read and write permissions for the application. Assign secure credentials to this group so that the application connects using this user instead of the administrator account. This enhances security and maintains separation of privileges.

Database Configuration:
Reconfigure the database to assign administrative rights to my account. Additionally, create a dedicated user group that can later be referenced in the application's ConnectionStrings section. This approach prevents other users from accessing the database using my personal admin credentials.

