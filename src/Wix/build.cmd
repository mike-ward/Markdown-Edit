@echo off
echo.
echo =============================
echo ====== Build Installer ====== 
echo =============================
echo.
if NOT EXIST out mkdir out
if NOT EXIST "%WIX%\bin\candle.exe" echo WIX Toolset not installed!
if NOT EXIST "%WIX%\bin\candle.exe" exit /b 1
"%WIX%\bin\candle.exe" Product.wxs -dSourceFiles="..\MarkdownEdit\bin\Release" -out out\
if NOT ERRORLEVEL 1 "%WIX%\bin\light.exe" -ext WiXNetFxExtension -ext WixUIExtension out\Product.wixobj -out ..\..\MarkdownEditSetup.msi -sice:ICE61 -pdbout out\MarkdownEidtSetup.wixpdb
exit /b errorlevel