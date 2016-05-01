pushd src\MarkdownEdit\bin\Release
"c:\Program Files\7-Zip\7z.exe" a -x!*.vshost.* -x!*.xml  -r ..\..\..\..\MarkdownEdit.zip *.* -mmt
popd 