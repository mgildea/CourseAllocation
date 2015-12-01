
<p>This is a C# .NET MVC application requiring Visual Studio for development purposes.</p>

<p>To run this project create blank PrivateSettings.config and ConnectionStrings.config files in ~/CourseAllocation</p>

<p>Then create PrivateSettings.(Build).config and ConnectionStrings.(Build).config based on the template config files provided for each build configuration.  For example, for the "Debug" build configuration create PrivateSettings.Debug.config and ConnectionStrings.Debug.config<p/>

<p>This project requires requires access to an sql server backend and utilizes Entity Framework for data access.  Create a new database corresponding with the "CourseAllocation" connection strings and run the "update-database" command in the nuget package manager.</p>

<p>To setup Elmah error logging create a new database corresponding with the "elmah-sqlserver" connection string and execute the sql found in  ~/CourseAllocation/App_Readme/Elmah.SqlServer.sql against this database.</p>


<p>This project Requires that Gurobi Optimizer 6.5.0 be installed and licensed on the machine running this application.  Download and installation instructions can be found at: http://user.gurobi.com/download/gurobi-optimizer</p>
<p>The Gurobi installation should reside at C:/gurobi650/ </p>


<p>This project requires you setup a free account with SendGrid at http://sendgrid.com and configure username and password in PrivateSettings.(Build).config for account creation and verification.</p>
