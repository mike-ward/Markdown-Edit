@echo off
echo.
if EXIST MarkdownEditSetup.msi del /Q MarkdownEditSetup.msi
<<<<<<< HEAD
if EXIST MarkdownEditSetup.zip del /Q MarkdownEditSetup.zip
=======
if EXIST MarkdownEdit.zip del /Q MarkdownEdit.zip
>>>>>>> dev
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
<<<<<<< HEAD
=======
if ERRORLEVEL 0 dir /b ma*
if ERRORLEVEL 0 echo === Build Complete ===
>>>>>>> dev
:END
