@echo off
echo.
echo ============================
echo ====== Build Zip File ======
echo ============================
if EXIST MarkdownEdit.zip del MarkdownEdit.zip
if NOT EXIST "%ProgramFiles(x86)%\7-Zip\7z.exe" echo 7-Zip not installed
pushd src\MarkdownEdit\bin\Release
"%ProgramFiles(x86)%\7-Zip\7z.exe" a -x!*.vshost.* -x!*.xml -x!lib/*.*  -r ..\..\..\..\MarkdownEdit.zip *.* -mmt
popd