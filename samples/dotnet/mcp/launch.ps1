param(
	[string]$module
)

Start-Process -WorkingDirectory "$PSScriptRoot/o/$module" -FilePath (Get-ChildItem -Path "$PSScriptRoot/o/$module" -Filter "*.exe" -Depth 1 | Select-Object -First 1).FullName