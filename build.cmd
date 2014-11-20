cd src
msbuild MarkdownEdit\markdownedit.csproj /tv:14.0 /t:Rebuild /p:configuration=release
echo %errorlevel%
if NOT ERRORLEVEL 1 msbuild MarkdownEdit.sln /t:Setup /p:configuration=release
cd ..