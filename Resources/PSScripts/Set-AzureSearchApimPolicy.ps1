[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    $DssApiVersion,
    [Parameter(Mandatory=$true)]
    [ValidateSet("at", "test", "pp", "prd", "oat")]
    $Environment
)

$ModuleName = "Az.Search"
if((Get-Module $ModuleName)) {

    Write-Verbose "Importing $ModuleName module"
    Import-Module $ModuleName

}
else {

    Write-Verbose "Installing $ModuleName module for current user"
    Install-Module -Name $ModuleName -Scope CurrentUser
    Write-Verbose "Importing $ModuleName module"
    Import-Module $ModuleName

}

$QueryKeyBaseName = "dss-$Environment-searchapimapi-qk"

$QueryKeys = Get-AzSearchQueryKey -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"
$PrimaryKey = $QueryKeys | Where-Object { $_.Name -eq "$QueryKeyBaseName-primary" }
$SecondayKey = $QueryKeys | Where-Object { $_.Name -eq "$QueryKeyBaseName-secondary" }

if ($PrimaryKey -and !$SecondayKey) {
    
    Write-Verbose -Message "Primary query key exists, creating secondary key."
    $NewQueryKey = New-AzSearchQueryKey -Name "$QueryKeyBaseName-secondary" -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"

    # tokenise policy with new key
    $ApimPolicyXml = Get-Content -Path $PSScriptRoot\..\..\ApimPolicy\DssSearchApimPolicy.xml
    $ApimPolicyXml = $ApimPolicyXml.Replace("__SearchQueryKey__", $NewQueryKey.Key)

    # apply policy
    $Context = New-AzureRmApiManagementContext -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-apim"
    $ApiId = "search-$DssApiVersion" #$([RegEx]::Replace($("$(ApiResourceName)-$(DssApiVersion)"), "-$", ""))
    $PolicyFilePath = "$(System.DefaultWorkingDirectory)/_SkillsFundingAgency_dss-devops/ApimPolicy/DssSearchApimPolicy.xml"
    Write-Host "Filepath: $PolicyFilePath"
    Set-AzureRmApiManagementPolicy -Context $Context -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $ApiId -PolicyFilePath $PolicyFilePath  -Verbose

    # remove primary key
    Write-Verbose -Message "Removing Primary query key."
    Remove-AzSearchQueryKey -KeyValue $PrimaryKey.Key -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"

}
elseif ($SecondayKey -and !$PrimaryKey) {
    # create primary key

    # tokenise policy with new key

    # apply policy

    # remove secondary key
}
elseif (!$PrimaryKey -and !$SecondayKey){

    Write-Verbose -Message "No query key exists, creating primary key."
    # create secondary key
    $NewQueryKey = New-AzSearchQueryKey -Name "$QueryKeyBaseName-secondary" -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"

    # tokenise policy with new key
    $ApimPolicyXml = Get-Content -Path $PSScriptRoot\..\..\ApimPolicy\DssSearchApimPolicy.xml
    $ApimPolicyXml = $ApimPolicyXml.Replace("__SearchQueryKey__", $NewQueryKey.Key)

    # apply policy
    $Context = New-AzureRmApiManagementContext -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-apim"
    $ApiId = "search-$DssApiVersion" #$([RegEx]::Replace($("$(ApiResourceName)-$(DssApiVersion)"), "-$", ""))
    $PolicyFilePath = "$(System.DefaultWorkingDirectory)/_SkillsFundingAgency_dss-devops/ApimPolicy/DssSearchApimPolicy.xml"
    Write-Host "Filepath: $PolicyFilePath"
    Set-AzureRmApiManagementPolicy -Context $Context -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $ApiId -PolicyFilePath $PolicyFilePath  -Verbose

}
else {

    throw "Error, both primary and secondary keys exist"

}

