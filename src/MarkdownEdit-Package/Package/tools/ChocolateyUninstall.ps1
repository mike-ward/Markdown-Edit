$productcode = (gwmi win32_product | ? { $_.Name -Like "Markdown Edit*" } | % { $_.IdentifyingNumber } | Select-Object -First 1)
Uninstall-ChocolateyPackage "markdown-edit" "msi" "$productcode /qb"
