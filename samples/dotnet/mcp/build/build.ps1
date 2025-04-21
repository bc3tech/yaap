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

    Start-Job -Name "Build MCP Server" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\AgentsMcpServer\AgentsMcpServer.csproj $outDir\mcpserver $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build ConsoleApp" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\TBAStatReader_WS\ConsoleApp.csproj $outDir\client $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters
    Start-Job -Name "Build Orchestrator" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\WebSockets\Orchestrator_WS\Orchestrator_WS.csproj $outDir\orchestrator $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters

    Start-Job -Name "Build Teams_SignalR" { param($repoRoot, $scriptRoot, $outDir, $params) . $scriptRoot\functions.ps1 ; RunBuild $repoRoot\Agents\WebSockets\Teams_WS\Teams_WS.csproj $outDir\teamsagent $params } -ArgumentList $repoRoot, $PSScriptRoot, $outputDir, $PSBoundParameters

    Get-Job | Wait-Job #| Remove-Job

    Write-Output ""
}

if (-not $NoDocker) {
    docker rmi -f yaap-mcp-server yaap-mcp-orchestrator yaap-mcp-teamsagent > $null

    Write-Output "Building Docker images..."

    Start-Job -Name "Image MCP Server" {
        param($secret, $outDir)

        docker build -t yaap-mcp-server $outDir\mcpserver
    } -ArgumentList (GetSecretObject 'ADBB2388-3A68-4DA1-9B41-211BAE7DC545'), $outputDir

    Start-Job -Name "Image Orchestrator" {
        param($secret, $outDir)

        docker build -t yaap-mcp-orchestrator $outDir\orchestrator --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg OrchestratorEndpoint=http://hub:8080/api/negotiate
    } -ArgumentList (GetSecretObject '437593f2-d0e9-48fd-aa60-508b004cbab8'), $outputDir

    Start-Job -Name "Image Teams Agent" {
        param($secret, $outDir)

        docker build -t yaap-mcp-teamsagent $outDir\teamsagent --build-arg AZURE_OPENAI_KEY=$($secret.AzureOpenAIKey) --build-arg OrchestratorEndpoint=http://hub:8080/api/negotiate --build-arg TBA_API_KEY=$($secret.TBA_API_KEY)
    } -ArgumentList (GetSecretObject 'bd940e53-ff29-49b2-b15d-3328289c1aff'), $outputDir
    
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
