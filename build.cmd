echo off
del /Q *.msi
del /Q *.nupkg
cd src
nuget restore MarkdownEdit.sln
if ERRORLEVEL 1 goto END
msbuild MarkdownEdit\markdownedit.csproj /t:Rebuild "/p:configuration=Release" /verbosity:minimal
if ERRORLEVEL 1 goto END
cd Wix
call build.cmd
cd ..
if ERRORLEVEL 1 goto END
cd MarkdownEdit-Package
call build.cmd
cd ..
:END
cd ..