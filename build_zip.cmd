@echo off
echo ============================
echo ====== Build Zip File ======
echo ============================
if EXIST MarkdownEdit.zip del MarkdownEdit.zip
pushd src\MarkdownEdit\bin\Release
7z.exe" a -x!*.vshost.* -x!*.xml -x!lib/*.*  -r ..\..\..\..\MarkdownEdit.zip *.* -mmt
popd
