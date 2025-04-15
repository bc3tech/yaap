[CmdletBinding()]
param(
    [switch]$NoBuild,
    [switch]$NoDocker,
    $Deploy = [DeploymentType]::None
)

. $PSScriptRoot\functions.ps1
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$outputDir = Join-Path $repoRoot 'o'

if (-not $NoBuild) {
    Write-Output "Building projects..."

    Start-Job -Name "Build SignalRHub" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\SignalRHub\SignalRHub.csproj $outDir\signalrhub $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build TBAStatReader" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\TBAStatReader\TBAStatReader.csproj $outDir\client $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Orchestrator" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\SignalR\Orchestrator_SignalR\Orchestrator_SignalR.csproj $outDir\orchestrator $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters

    Start-Job -Name "Build Districts_SignalR" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\SignalR\Districts_SignalR\Districts_SignalR.csproj $outDir\districtsagent $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Events_SignalR" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\SignalR\Events_SignalR\Events_SignalR.csproj $outDir\eventsagent $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Matches_SignalR" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\SignalR\Matches_SignalR\Matches_SignalR.csproj $outDir\matchesagent $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Teams_SignalR" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\SignalR\Teams_SignalR\Teams_SignalR.csproj $outDir\teamsagent $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters

    Get-Job | Wait-Job #| Remove-Job

    Write-Output ""
}

if (-not $NoDocker) {
    docker rmi -f signalrhub orchestrator districtsagent eventsagent matchesagent teamsagent > $null

    Write-Output "Building Docker images..."

    Start-Job -Name "Image SignalRHub" {
        param($secret, $outDir)

        docker build -t signalrhub $outDir\signalrhub --build-arg SIGNALR_CONNSTRING=$($secret.'Azure:SignalR:ConnectionString')
    } -ArgumentList (GetSecretObject 'ff55d15e-c100-4281-8cb5-5d29b4f995ab'), $outputDir

    Start-Job -Name "Image Orchestrator" {
        param($secret, $outDir)

        docker build -t orchestrator $outDir\orchestrator --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg SignalREndpoint=http://hub:8080/api/negotiate
    } -ArgumentList (GetSecretObject '1507d29c-61b1-4678-b23a-1562ed1a1abb'), $outputDir

    Start-Job -Name "Image Districts Agent" {
        param($secret, $outDir)

        docker build -t districtsagent $outDir\districtsagent --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg SignalREndpoint=http://hub:8080/api/negotiate --build-arg TBA_API_KEY=$($secret.TBA_API_KEY)
    } -ArgumentList (GetSecretObject 'f724ee6c-8bf6-4796-904d-69463aba9287'), $outputDir

    Start-Job -Name "Image Events Agent" {
        param($secret, $outDir)

        docker build -t eventsagent $outDir\eventsagent --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg SignalREndpoint=http://hub:8080/api/negotiate --build-arg TBA_API_KEY=$($secret.TBA_API_KEY)
    } -ArgumentList (GetSecretObject 'a74edbc7-6f1b-4f8f-ac34-6a5b90c653cd'), $outputDir

    Start-Job -Name "Image Matches Agent" {
        param($secret, $outDir)

        docker build -t matchesagent $outDir\matchesagent --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg SignalREndpoint=http://hub:8080/api/negotiate --build-arg TBA_API_KEY=$($secret.TBA_API_KEY)
    } -ArgumentList (GetSecretObject 'f3b45348-9c0d-4b66-aa62-2228d0369fbe'), $outputDir

    Start-Job -Name "Image Teams Agent" {
        param($secret, $outDir)

        docker build -t teamsagent $outDir\teamsagent --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg SignalREndpoint=http://hub:8080/api/negotiate --build-arg TBA_API_KEY=$($secret.TBA_API_KEY)
    } -ArgumentList (GetSecretObject '5631e549-948c-4903-be18-a06152c3600c'), $outputDir
    
    Get-Job | Wait-Job 

    Write-Output ""
}

if ($Deploy -eq [DeploymentType]::Kubernetes) {
    Write-Output "Deploying to Kubernetes..."
    kubectl apply -f $repoRoot\deploy\k8s.deploy.yml
}
elseif ($Deploy -eq [DeploymentType]::Docker) {
    Write-Output "Deploying via Docker Compose..."
    Push-Location $repoRoot\deploy
    docker compose up -d --no-build
    Pop-Location
}

enum DeploymentType {
    Kubernetes
    Docker
    None
}
