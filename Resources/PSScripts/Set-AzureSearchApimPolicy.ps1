[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    $DssApiVersion,
    [Parameter(Mandatory=$true)]
    [ValidateSet("at", "test", "pp", "prd", "oat")]
    $Environment,
    [Parameter(Mandatory=$true)]
    $PolicyFilePath
)

function Use-PowerShellModule {
    param(
        [Parameter(Mandatory=$true)]    
        $ModuleName
    )

    if((Get-Module $ModuleName)) {

        Write-Verbose "Importing $ModuleName module"
        Import-Module $ModuleName
    
    }
    else {
    
        Write-Verbose "Installing $ModuleName module for current user"
        Install-Module -Name $ModuleName -Scope CurrentUser -AllowClobber
        Write-Verbose "Importing $ModuleName module"
        Import-Module $ModuleName
    
    }
}

Use-PowerShellModule -ModuleName "Az.Search"
Use-PowerShellModule -ModuleName "Az.ApiManagement"


$QueryKeyBaseName = "dss-$($Environment.ToLower())-searchapimapi-qk"

$QueryKeys = Get-AzSearchQueryKey -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"
$PrimaryKey = $QueryKeys | Where-Object { $_.Name -eq "$QueryKeyBaseName-primary" }
$SecondayKey = $QueryKeys | Where-Object { $_.Name -eq "$QueryKeyBaseName-secondary" }

Write-Verbose "Filepath: $PolicyFilePath"

if ($PrimaryKey -and !$SecondayKey) {
    
    Write-Verbose -Message "Primary query key exists, creating secondary key."
    $NewQueryKey = New-AzSearchQueryKey -Name "$QueryKeyBaseName-secondary" -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"

    # tokenise policy with new key
    $ApimPolicyXml = Get-Content -Path $PolicyFilePath
    $ApimPolicyXml = $ApimPolicyXml.Replace("__SearchQueryKey__", $NewQueryKey.Key)
    Set-Content -Path $PolicyFilePath -Value $ApimPolicyXml

    # apply policy
    $Context = New-AzApiManagementContext -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-apim"
    $ApiId = "search-$DssApiVersion" #$([RegEx]::Replace($("$(ApiResourceName)-$(DssApiVersion)"), "-$", ""))
    Set-AzApiManagementPolicy -Context $Context -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $ApiId -PolicyFilePath $PolicyFilePath  -Verbose

    # remove primary key
    Write-Verbose -Message "Removing Primary query key."
    Remove-AzSearchQueryKey -KeyValue $PrimaryKey.Key -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch" -Force

}
elseif ($SecondayKey -and !$PrimaryKey) {
    # create primary key

    # tokenise policy with new key

    # apply policy

    # remove secondary key
}
elseif (!$PrimaryKey -and !$SecondayKey){

    Write-Verbose -Message "No query key exists, creating primary key."
    # create primary key
    $NewQueryKey = New-AzSearchQueryKey -Name "$QueryKeyBaseName-primary" -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"

    # tokenise policy with new key
    $ApimPolicyXml = Get-Content -Path $PolicyFilePath
    $ApimPolicyXml = $ApimPolicyXml.Replace("__SearchQueryKey__", $NewQueryKey.Key)
    Set-Content -Path $PolicyFilePath -Value $ApimPolicyXml

    # apply policy
    $Context = New-AzApiManagementContext -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-apim"
    $ApiId = "search-$DssApiVersion" #$([RegEx]::Replace($("$(ApiResourceName)-$(DssApiVersion)"), "-$", ""))
    Set-AzApiManagementPolicy -Context $Context -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $ApiId -PolicyFilePath $PolicyFilePath  -Verbose

}
else {

    throw "Error, both primary and secondary keys exist"

}

