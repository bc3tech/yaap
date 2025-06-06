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
    Start-Job -Name "Build ConsoleApp" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\ConsoleApp\ConsoleApp.csproj $outDir\client $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Orchestrator" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\Orchestrator\Orchestrator.csproj $outDir\orchestrator $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Teams Expert" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\TeamsExpert\TeamsExpert.csproj $outDir\teamsagent $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters

    Get-Job | Wait-Job #| Remove-Job

    Write-Output ""
}

if (-not $NoDocker) {
    docker rmi -f yaap-ws-orchestrator yaap-ws-teamsagent > $null

    Write-Output "Building Docker images..."

    Start-Job -Name "Image Orchestrator" {
        param($secret, $outDir)

        docker build -t yaap-ws-orchestrator $outDir\orchestrator --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg AZURE_OPENAI_ENDPOINT=$($secret.AzureOpenAIEndpoint)
    } -ArgumentList (GetSecretObject '1507d29c-61b1-4678-b23a-1562ed1a1abb'), $outputDir

    Start-Job -Name "Image Teams Agent" {
        param($secret, $outDir)

        docker build -t yaap-ws-teamsagent $outDir\teamsagent --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg OrchestratorEndpoint=ws://orchestrator:7292/ws/orchestrator --build-arg TBA_API_KEY=$($secret.TBA_API_KEY) --build-arg AZURE_OPENAI_ENDPOINT=$($secret.AzureOpenAIEndpoint)
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
