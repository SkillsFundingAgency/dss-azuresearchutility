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

function Set-AzureSearchApimPolicyKey {
    param(
        [Parameter(Mandatory=$true)]
        [ValidateSet("primary", "secondary", "none")]
        $KeyToDelete
    )

    if ($KeyToDelete -eq "primary"){

        $KeyToCreate = "secondary"

    }
    elseif ($KeyToDelete -eq "secondary") {

        $KeyToCreate = "primary"

    }
    elseif ($KeyToDelete -eq "none") {

        $KeyToCreate = "primary"

    }
    else {

        throw "KeyToReplace parameter is not a valid value, must be either 'primary' or 'seconday'."

    }

    Write-Verbose -Message "$KeyToDelete query key exists, creating $KeyToCreate key."
    $NewQueryKey = New-AzSearchQueryKey -Name "$QueryKeyBaseName-$KeyToCreate" -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"

    # tokenise policy with new key
    $ApimPolicyXml = Get-Content -Path $PolicyFilePath
    $ApimPolicyXml = $ApimPolicyXml.Replace("__SearchQueryKey__", $NewQueryKey.Key)
    Set-Content -Path $PolicyFilePath -Value $ApimPolicyXml

    # apply policy
    $Context = New-AzApiManagementContext -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-apim"
    $ApiId = "search-$DssApiVersion"
    Set-AzApiManagementPolicy -Context $Context -Format application/vnd.ms-azure-apim.policy.raw+xml -ApiId $ApiId -PolicyFilePath $PolicyFilePath  -Verbose

    if ($KeyToDelete -ne "none") {

        Write-Verbose -Message "Removing $KeyToDelete query key."
        Remove-AzSearchQueryKey -KeyValue $(Get-Variable -Name "$($KeyToDelete)Key").value -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch" -Force

    }

}

Use-PowerShellModule -ModuleName "Az.Search"
Use-PowerShellModule -ModuleName "Az.ApiManagement"

$QueryKeyBaseName = "dss-$($Environment.ToLower())-searchapimapi-qk"

$QueryKeys = Get-AzSearchQueryKey -ResourceGroupName "dss-$Environment-shared-rg" -ServiceName "dss-$Environment-shared-sch"
$PrimaryKey = ($QueryKeys | Where-Object { $_.Name -eq "$QueryKeyBaseName-primary" }).Key
$SecondaryKey = ($QueryKeys | Where-Object { $_.Name -eq "$QueryKeyBaseName-secondary" }).Key

Write-Verbose "Filepath: $PolicyFilePath"

if ($PrimaryKey -and !$SecondaryKey) {
    
    Set-AzureSearchApimPolicyKey -KeyToDelete "primary"

}
elseif ($SecondaryKey -and !$PrimaryKey) {

    Set-AzureSearchApimPolicyKey -KeyToDelete "secondary"

}
elseif (!$PrimaryKey -and !$SecondaryKey){

    Set-AzureSearchApimPolicyKey -KeyToDelete "none"

}
else {

    throw "Error, both primary and secondary keys exist"

}

