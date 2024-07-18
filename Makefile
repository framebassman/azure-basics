publish-tables:
	dotnet clean ProjectTasks.Tables.WebApi/ProjectTasks.Tables.WebApi.csproj
	dotnet publish ProjectTasks.Tables.WebApi/ProjectTasks.Tables.WebApi.csproj --output ProjectTasks.Tables.WebApi/bin/Publish

run-azuresql:
	dotnet clean ProjectTasks.WebApi/ProjectTasks.WebApi.csproj
		dotnet run \
		--launch-profile "AzureSQL" \
		--project ProjectTasks.WebApi/ProjectTasks.WebApi.csproj


run-cosmosdb:
	dotnet clean ProjectTasks.WebApi/ProjectTasks.WebApi.csproj
		dotnet run \
		--launch-profile "CosmosDb" \
		--project ProjectTasks.WebApi/ProjectTasks.WebApi.csproj

publish-documents:
	dotnet clean ProjectTasks.Documents.WebApi/ProjectTasks.Documents.WebApi.csproj
	dotnet publish ProjectTasks.Documents.WebApi/ProjectTasks.Documents.WebApi.csproj --output ProjectTasks.Documents.WebApi/bin/Publish

# Migrations not available for CosmosDb (https://learn.microsoft.com/en-us/ef/core/providers/cosmos/limitations)
migrations-azuresql-add-ProjectTasks:
	STORAGE_TYPE=AzureSQL dotnet ef \
		migrations add ProjectTasks \
		--project ProjectTasks.DataAccess.AzureSQL/ProjectTasks.DataAccess.AzureSQL.csproj \
		--startup-project ProjectTasks.WebApi/ProjectTasks.WebApi.csproj \
		--context AzureSqlDbContext \
		--verbose

update-azure-sql:
	STORAGE_TYPE=AzureSQL dotnet ef \
		database update \
		--project ProjectTasks.WebApi/ProjectTasks.WebApi.csproj
