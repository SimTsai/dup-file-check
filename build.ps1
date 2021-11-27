$path = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RID = "win-x64", "win-x86", "win-arm", "win-arm64", "linux-x64", "linux-musl-x64", "linux-arm", "linux-arm64", "osx-x64"

Set-Location -Path $path
foreach ($item in $RID) {
    dotnet publish --sc -r $item -c Release -o ./publish/$item/
}

Set-Location -Path "publish"
foreach ($item in $RID) {
    $cmd = 'tar -cvzf ' + $item + '.tar.gz ' + $item + '\dup-file-check*'
    Invoke-Expression $cmd
}
