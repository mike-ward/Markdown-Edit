pushd src\MarkdownEdit\bin\Release
"%ProgramFiles(x86)%\7-Zip\7z.exe" a -x!*.vshost.* -x!*.xml -x!lib/*.*  -r ..\..\..\..\MarkdownEdit.zip *.* -mmt
popd 