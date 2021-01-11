Import-Module PowershellForXti -Force

$script:appConfig = [PSCustomObject]@{
    RepoOwner = "JasonBenfield"
    RepoName = "XTI_App"
    AppName = "XTI_App"
    AppType = "Package"
    ProjectDir = ""
}

function App-New-XtiIssue {
    param(
        [Parameter(Mandatory, Position=0)]
        [string] $IssueTitle,
        $Labels = @(),
        [string] $Body = "",
        [switch] $Start
    )
    $script:appConfig | New-XtiIssue @PsBoundParameters
}

function App-Xti-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0,
        $IssueBranchTitle = "",
        $AssignTo = ""
    )
    $script:appConfig | Xti-StartIssue @PsBoundParameters
}

function App-New-XtiVersion {
    param(
        [Parameter(Position=0)]
        [ValidateSet("major", "minor", "patch")]
        $VersionType = "minor",
        [ValidateSet("Development", "Production", "Staging", "Test")]
        $EnvName = "Production"
    )
    $script:appConfig | New-XtiVersion @PsBoundParameters
}

function App-Xti-Merge {
    param(
        [Parameter(Position=0)]
        [string] $CommitMessage
    )
    $script:appConfig | Xti-Merge
}

function App-New-XtiPullRequest {
    param(
        [Parameter(Position=0)]
        [string] $CommitMessage
    )
    $script:appConfig | New-XtiPullRequest @PsBoundParameters
}

function App-Xti-PostMerge {
    param(
    )
    $script:appConfig | Xti-PostMerge @PsBoundParameters
}

function App-Publish {
    param(
        [switch] $Prod
    )
    $script:appConfig | Xti-PublishPackage @PsBoundParameters
    if($Prod){
        $script:appConfig | Xti-Merge
    }
}
