del /Q *.msi
del /Q *.nupkg
cd src
nuget restore MarkdownEdit.sln
if ERRORLEVEL 1 goto END
msbuild MarkdownEdit\markdownedit.csproj /tv:14.0 /t:Rebuild /p:configuration=release
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