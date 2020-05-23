param (
  [string]$path = $(throw "-path is required"),
  [string]$config = "DEBUG"
)

Write-Output "Config: $config"
Write-Output "Path: $path"

$path = Resolve-Path $path
$path_manifest = "${path}\Package.appxmanifest"

$config = $config.ToUpper()

try {
    $out = Invoke-Command -ScriptBlock {git -C $path rev-list --count HEAD}
} catch {
    exit
}

Write-Host "Git rev-list: $out"

$rtn = 0
if ([double]::TryParse($out, [ref]$rtn) -ne $true) {
    exit
}

[xml]$document = Get-Content $path_manifest

$h = @{}
$h["DEBUG"] = "49197Wirdschon.UnigramMobileExperimental"
$h["RELEASE"] = "49197Wirdschon.UnigramMobile"

$identity = $document.GetElementsByTagName("Identity")[0]
$original1 = $identity.Attributes["Name"].Value
$original2 = $identity.Attributes["Version"].Value

$identity.Attributes["Name"].Value = $h[$config]

$version = $identity.Attributes["Version"].Value;
$regex = [regex]'(?:(\d+)\.)(?:(\d+)\.)(?:(\d*?)\.\d+)'

$identity.Attributes["Version"].Value = $regex.Replace($version, '$1.$2.{0}.0' -f $out)

if ($original1 -eq $identity.Attributes["Name"].Value -and $original2 -eq $identity.Attributes["Version"].Value) {
    exit
}

$h = @{}
$h["DEBUG"] = "Unigram Mobile Experimental"
$h["RELEASE"] = "Unigram Mobile Messenger"

$properties = $document.GetElementsByTagName("Properties")[0]
$displayName = $properties.GetElementsByTagName("DisplayName")[0]
$displayName.InnerText = $h[$config]

$h = @{}
$h["DEBUG"] = "Unigram Mobile Experimental"
$h["RELEASE"] = "Unigram Mobile Messenger Beta"

$visualElements = $document.GetElementsByTagName("uap:VisualElements")[0]
$visualElements.Attributes["DisplayName"].Value = $h[$config]

$document.Save($path_manifest)