cd Package
cpack markdown-edit.nuspec --yes
mv *.nupkg ..\..\..
cd ..
exit /b errorlevel