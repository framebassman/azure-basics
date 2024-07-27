publish-webapi:
	dotnet clean ProjectTasks.WebApi/ProjectTasks.WebApi.csproj
	dotnet publish ProjectTasks.WebApi/ProjectTasks.WebApi.csproj \
		--output ProjectTasks.WebApi/bin/Publish

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


# Migrations not available for CosmosDb (https://learn.microsoft.com/en-us/ef/core/providers/cosmos/limitations)
migrations-azuresql-add-%:
	STORAGE_TYPE=AzureSQL dotnet ef \
		migrations add $* \
		--project ProjectTasks.DataAccess.AzureSQL/ProjectTasks.DataAccess.AzureSQL.csproj \
		--startup-project ProjectTasks.WebApi/ProjectTasks.WebApi.csproj \
		--context AzureSqlDbContext \
		--verbose

update-azure-sql:
	STORAGE_TYPE=AzureSQL dotnet ef \
		database update \
		--project ProjectTasks.WebApi/ProjectTasks.WebApi.csproj

run-sync:
	rm -rf ProjectTasks.SyncFunction/bin
	rm -rf ProjectTasks.SyncFunction/obj
	cd ProjectTasks.SyncFunction && func host start

publish-sync:
	rm -rf ProjectTasks.SyncFunction/obj
	cd ProjectTasks.SyncFunction && func azure functionapp publish reporting-web-sync

create-function:
	az functionapp create \
		--resource-group service-catalog-infra-dev1-eastus-reporting \
		--consumption-plan-location eastus \
		--runtime dotnet-isolated \
		--functions-version 4 \
		--name reporting-web-sync \
		--storage-account reportingwebsync \
		--tags Contact=dmitry.romashov@intapp.com Product=reporting Environment=Development
