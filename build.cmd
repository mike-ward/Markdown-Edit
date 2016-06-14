@echo off
echo.
if EXIST MarkdownEditSetup.msi del /Q MarkdownEditSetup.msi
if EXIST MarkdownEditSetup.zip del /Q MarkdownEditSetup.zip
pushd src
nuget restore MarkdownEdit.sln
if ERRORLEVEL 1 goto END
msbuild MarkdownEdit\markdownedit.csproj /t:Rebuild "/p:configuration=Release;platform=AnyCPU" /verbosity:minimal
popd
if ERRORLEVEL 1 goto END
pushd src\Wix
call build.cmd
popd
if ERRORLEVEL 1 goto END
call build_zip.cmd
:END
