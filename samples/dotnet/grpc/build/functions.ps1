function GetSecretObject {
    param([Parameter(Mandatory = $true)][string]$secretGuid)

    $path = "$($env:APPDATA)\Microsoft\UserSecrets\$secretGuid\secrets.json"
    
    ConvertFrom-Json -InputObject $(Get-Content -Path $path -Raw)
}

function RunBuild([Parameter(Mandatory = $true)][string]$project, [Parameter(Mandatory = $true)][string]$root, [Parameter(Mandatory = $false)][hashtable]$psParams = @{}) {
    Write-Debug "$project -> $root"
    $num = 0
    do {
        $num++
        $result = (dotnet publish $project -o $root -v m)

        if ($psParams.ContainsKey('Debug')) {
            $resultString = (ConvertTo-Json $result).ToString()
            Write-Debug $resultString
        }
    } while ($result -match "SDK Resolver Failure:")

    Write-Output "Building $project succeeded in $num attempt(s)"
}
