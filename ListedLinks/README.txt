LinstedLinks

TODO 0: Automate deployment by creating script and eventually doing this all via GitHub Actions

Manual deployment of ListedLinks project to biffkittz.com hosted on DigitalOcean server:

 1) rm -rf /tmp/biffkittz/*
 2) Transfer files in this ListedLinks project to /tmp/biffkittz on the remote server using SCP (but make script to SSH this stuff)
 3) Execute 'sudo dotnet publish /tmp/biffkittz/ListedLinks.csproj -r linux-x64 --self-contained'
 4) Move the outputted self-contained app resources to var/www/biffkittz by executing
	'cp -rf /tmp/biffkittz/bin/Release/net8.0/linux-x64/* /var/www/biffkittz/'
 5) Execute 'sudo nginx -t' to test the nginx config
 6) Execute 'sudo nginx -s reload' to reload the nginx config
 7) Execute 'sudo systemctl stop kestrel-biffkittz.service' to stop the service
 8) Execute 'sudo systemctl start kestrel-biffkittz.service' to start the service
 9) Execute 'ls -als /var/www/biffkittz' to review production site file structure
10) Updated site should load @ https://biffkittz.com

on host:
dotnet tool install --global dotnet-ef --version 8.0.0