@echo off
echo.
if EXIST MarkdownEditSetup.msi del /Q MarkdownEditSetup.msi
if EXIST MarkdownEdit.zip del /Q MarkdownEdit.zip
pushd src
nuget restore MarkdownEdit.sln
if ERRORLEVEL 1 goto END
msbuild MarkdownEdit\markdownedit.csproj /t:Clean;Rebuild "/p:configuration=Release;platform=AnyCPU" /verbosity:minimal
popd
if ERRORLEVEL 1 goto END
pushd src\Wix
call build-setup.cmd
popd
if ERRORLEVEL 1 goto END
call build-zip.cmd
if ERRORLEVEL 0 dir /b ma*
if ERRORLEVEL 0 echo === Build Complete ===
:END
