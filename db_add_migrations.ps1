param ([Parameter(Mandatory)]$Name)
$env:DOTNET_ENVIRONMENT="Development"
dotnet ef --startup-project ./Tools/AppDbTool migrations add $Name --project ./Lib/XTI_App.DB.SqlServer