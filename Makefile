prepare:
	git config core.hooksPath .git-hooks || echo 'Not in a git repo'

publish-tables:
	dotnet clean ProjectTasks.Tables.WebApi/ProjectTasks.Tables.WebApi.csproj
	dotnet publish ProjectTasks.Tables.WebApi/ProjectTasks.Tables.WebApi.csproj --output ProjectTasks.Tables.WebApi/bin/Publish

publish-documents:
	dotnet clean ProjectTasks.Documents.WebApi/ProjectTasks.Documents.WebApi.csproj
	dotnet publish ProjectTasks.Documents.WebApi/ProjectTasks.Documents.WebApi.csproj --output ProjectTasks.Documents.WebApi/bin/Publish

# Migrations available on Azure SQL only (https://learn.microsoft.com/en-us/ef/core/providers/cosmos/limitations)
migrations-azuresql-add-ProjectTasks:
	dotnet ef \
		migrations add ProjectTasks \
		--project ProjectTasks.DataAccess.AzureSQL/ProjectTasks.DataAccess.AzureSQL.csproj \
		--startup-project ProjectTasks.Tables.WebApi/ProjectTasks.Tables.WebApi.csproj \
		--context AzureSqlDbContext \
		--verbose


