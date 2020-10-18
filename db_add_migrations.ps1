param ([Parameter(Mandatory)]$name)
$env:DOTNET_ENVIRONMENT="Development"
dotnet ef migrations add $name --project ./Tools/AppDbTool

$currentDir = (Get-Item .).FullName
Set-Location Tools/AppDbTool
dotnet publish /p:PublishProfile=Local
Set-Location $currentDir