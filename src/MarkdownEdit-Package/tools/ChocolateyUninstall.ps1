$u = "${Env:ProgramFiles(x86)}" + "\markdown edit\unins000.exe"
Uninstall-ChocolateyPackage "markdown-edit" "exe" "/verysilent" "$u"
