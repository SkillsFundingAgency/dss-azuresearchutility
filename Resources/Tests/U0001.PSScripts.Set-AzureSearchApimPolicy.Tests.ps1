Push-Location -Path $PSScriptRoot\..\PSScripts\

Import-Module Az.Search

Describe "Set-AzureSearchApimPolicy unit tests" -Tag "Unit" {

    Mock New-AzSearchQueryKey { [PsCustomObject]
        @{
            Name = ""
            Key = "A0JUST0CREATED0KEY012345678ABCD"
        }
    }
    Mock Get-Content {
        @(
            '<policies>'
            '<inbound>'
                '<choose>'
                    '<when condition="@(context.Variables.GetValueOrDefault<bool>("allowedSearchString"))">'
                        '<return-response>'
                        '</return-response>'
                    '</when>'
                    '<otherwise>'
                        '<set-header name="api-key" exists-action="override">'
                            "<value>'__SearchQueryKey__'</value>"
                        '</set-header>'
                    '</otherwise>'
                '</choose>'
                '<base />'
            '</inbound>'
        '</policies>'
        )
    }
    Mock Set-Content
    Mock New-AzApiManagementContext { 
        New-Object -TypeName Microsoft.Azure.Commands.ApiManagement.ServiceManagement.Models.PsApiManagementContext
    }
    Mock Set-AzApiManagementPolicy
    Mock Remove-AzSearchQueryKey
    
    $SetAzureSearchApimPolicyDefaultParameters = @{
        DssApiVersion = "v2"
        Environment = "at"
        PolicyFilePath = "C:\not-a-real-location/_dss-azuresearchutility/NCS.DSS.AzureSearchUtility/Azure/ApimPolicy/DssSearchApimPolicy.xml"
    }

    It "Should create a new key if none exists" {
        Mock Get-AzSearchQueryKey {
            @(
                @{
                    Name = ""
                    Key = "NOT0A0REAL0KEY012345678ABCDEFGH"
                },
                @{
                    Name = "dss-at-developer-qk"
                    Key = "NOT0A0REAL0KEY012345678ABCDEFGH"
                } 
            )
        }

        .\Set-AzureSearchApimPolicy @SetAzureSearchApimPolicyDefaultParameters

        Assert-MockCalled New-AzSearchQueryKey -Exactly 1 -Scope It
        Assert-MockCalled Remove-AzSearchQueryKey -Exactly 0 -Scope It
    }

    It "Should create a secondary key if the primary key exists and delete the primary key" {
        Mock Get-AzSearchQueryKey {
            @(
                @{
                    Name = "dss-at-searchapimapi-qk-primary"
                    Key = "NOT0A0REAL0KEY012345678ABCDEFGH"
                } 
            )
        }

        .\Set-AzureSearchApimPolicy @SetAzureSearchApimPolicyDefaultParameters

        Assert-MockCalled New-AzSearchQueryKey -Exactly 1 -Scope It
        Assert-MockCalled Remove-AzSearchQueryKey -Exactly 1 -Scope It
    }

    It "Should create a primary key if the secondary key exists and delete the secondary key" {
        Mock Get-AzSearchQueryKey {
            @(
                @{
                    Name = "dss-at-searchapimapi-qk-secondary"
                    Key = "NOT0A0REAL0KEY012345678ABCDEFGH"
                }  
            )
        }

        .\Set-AzureSearchApimPolicy @SetAzureSearchApimPolicyDefaultParameters

        Assert-MockCalled New-AzSearchQueryKey -Exactly 1 -Scope It
        Assert-MockCalled Remove-AzSearchQueryKey -Exactly 1 -Scope It
    }

    It "Should throw an error if both primary and secondary keys exist" {
        Mock Get-AzSearchQueryKey {
            @(
                @{
                    Name = "dss-at-searchapimapi-qk-primary"
                    Key = "NOT0A0REAL0KEY012345678ABCDEFGH"
                },
                @{
                    Name = "dss-at-searchapimapi-qk-secondary"
                    Key = "NOT0A0REAL0KEY012345678ABCDEFGH"
                }  
            )
        }

        { .\Set-AzureSearchApimPolicy @SetAzureSearchApimPolicyDefaultParameters } | Should Throw
    }

}

Push-Location -Path $PSScriptRoot