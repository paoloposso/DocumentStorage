# Folders Structure

## db_scripts folder

Contains the postgres scripts.

### Creating the Structure on DB
- Execute create_tables.sql on the database to create the tables
- Execute the scripts under _procedures_ to create the procedures

## src folder
Contains the source code for the api

### Configuring the Database Access - appsettings.json
Edit appsettings.json to configure 
- Connection string to database: documents_postgres
- JWT Secret Key: jwt:key

# Running the API
DocumentStorage.Api contains the executable application.

- Run ```dotnet run``` under the folder, in the terminal or
- Execute it on Visual Studio

The swagger page will open and show the list of available endpoints.

# Unit Tests
The unit tests are in DocumentStorage.Test. Execute them using 
- Go into DocumentStorage.Test folder and run ```dotnet test``` or
- Run the tests individually opening the classes and selecting the tests to be run.

# End-to-end Tests
Go to DocumentStorage.Api.Test. 
The Api must already be running.
- run ```dotnet test``` or
- Run the tests individually opening the classes and selecting the tests to be run.
