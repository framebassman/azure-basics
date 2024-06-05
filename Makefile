prepare:
	git config core.hooksPath .git-hooks || echo 'Not in a git repo'

publish:
	dotnet clean ProjectTasks.Api/ProjectTasks.Api.csproj
	dotnet publish ProjectTasks.Api/ProjectTasks.Api.csproj --output ProjectTasks.Api/bin/Publish

up:
	rm -rf ProjectTasks.Api/bin
	rm -rf ProjectTasks.Api/obj
	docker rmi azure-basics-project_tasks_api --force
	docker compose build --no-cache
	docker compose up

sh:
	docker exec -it project_tasks_api sh
