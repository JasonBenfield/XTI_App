Import-Module PowershellForXti -Force

$script:appConfig = [PSCustomObject]@{
    RepoOwner = "JasonBenfield"
    RepoName = "XTI_App"
    AppName = "XTI_App"
    AppType = "Package"
}

function App-NewVersion {
    param(
        [Parameter(Position=0)]
        [ValidateSet("major", "minor", "patch")]
        $VersionType = "minor"
    )
    $script:appConfig | New-XtiVersion @PsBoundParameters
}

function App-NewIssue {
    param(
        [Parameter(Mandatory, Position=0)]
        [string] $IssueTitle,
        [switch] $Start
    )
    $script:appConfig | New-XtiIssue @PsBoundParameters
}

function App-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0
    )
    $script:appConfig | Xti-StartIssue @PsBoundParameters
}

function App-CompleteIssue {
    param(
    )
    $script:appConfig | Xti-CompleteIssue @PsBoundParameters
}

function App-Publish {
    param(
        [ValidateSet("Development", "Production", "Staging", "Test")]
        $EnvName = "Development"
    )
    $script:appConfig | Xti-Publish @PsBoundParameters
}
