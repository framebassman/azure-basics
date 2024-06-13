prepare:
	git config core.hooksPath .git-hooks || echo 'Not in a git repo'

publish-api:
	dotnet clean ProjectTasks.Api/ProjectTasks.Api.csproj
	dotnet publish ProjectTasks.Api/ProjectTasks.Api.csproj --output ProjectTasks.Api/bin/Publish

publish-cosmos:
	dotnet clean ProjectTasks.CosmosDb/ProjectTasks.CosmosDb.csproj
	dotnet publish ProjectTasks.CosmosDb/ProjectTasks.CosmosDb.csproj --output ProjectTasks.CosmosDb/bin/Publish
