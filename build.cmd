echo off
if EXIST MarkdownEditSetup.msi del /Q MarkdownEditSetup.msi
cd src
nuget restore MarkdownEdit.sln
if ERRORLEVEL 1 goto END
msbuild MarkdownEdit\markdownedit.csproj /t:Rebuild "/p:configuration=Release" /verbosity:minimal
if ERRORLEVEL 1 goto END
cd Wix
call build.cmd
cd ..
:END
cd ..
