cd Package
del /Q *.nupkg
cpack markdown-edit.nuspec
mv *.nupkg ..\..\..
cd ..