$env:ASPNETCORE_ENVIRONMENT='Development'
$env:DOTNET_ENVIRONMENT='Development'
$env:Logging__LogLevel__TBAStatReader='Warning'

Write-Host "Starting hub..."
./launch.ps1 signalrhub
Start-Sleep -Seconds 3

Write-Host "Starting Router/Orchestrator..."
./launch.ps1 orchestrator
Start-Sleep -Seconds 3

Write-Host "Starting Agent 1..."
./launch.ps1 districtsagent
Start-Sleep -Seconds 3

Write-Host "Starting User client..."
./launch.ps1 client

Write-Host "Press <Enter> to continue to the next stage"
while ([System.Console]::ReadKey($true).Key -ne "Enter") {
    # Wait for Enter key
}

Write-Host "Starting Agent 2..."
./Launch teamsagent

Write-Host "Press <Enter> to continue to the next stage"
while ([System.Console]::ReadKey($true).Key -ne "Enter") {
    # Wait for Enter key
}

Write-Host "Starting Other agents..."
./Launch eventsagent
./Launch matchesagent