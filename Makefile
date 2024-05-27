prepare:
	git config core.hooksPath .git-hooks || echo 'Not in a git repo'
	
publish:
	dotnet clean ProjectTasks.Api/ProjectTasks.Api.csproj
	dotnet publish ProjectTasks.Api/ProjectTasks.Api.csproj --output ProjectTasks.Api/bin/Publish
