cd src
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