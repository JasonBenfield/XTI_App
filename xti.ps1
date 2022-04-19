Import-Module PowershellForXti -Force

function Xti-NewVersion {
    param(
        [Parameter(Position=0)]
        [ValidateSet("major", "minor", "patch")]
        $VersionType = "minor"
    )
    New-BaseXtiVersion @PsBoundParameters
}

function Xti-NewIssue {
    param(
        [Parameter(Mandatory, Position=0)]
        [string] $IssueTitle,
        [switch] $Start
    )
    New-BaseXtiIssue @PsBoundParameters
}

function Xti-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0
    )
    BaseXti-StartIssue @PsBoundParameters
}

function Xti-CompleteIssue {
    param(
    )
    BaseXti-CompleteIssue @PsBoundParameters
}

function Xti-Publish {
    param(
        [ValidateSet("Development", "Production")]
        $EnvName = "Development",
        [ValidateSet("Default", "DB")]
        $HubAdministrationType = "Default"
    )
    BaseXti-Publish @PsBoundParameters
}
