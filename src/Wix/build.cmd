@echo off
echo.
echo =============================
echo ====== Build Installer ======
echo =============================
echo.
if NOT EXIST out mkdir out
call heat.cmd
if ERRORLEVEL 1 exit /b 1
if NOT EXIST "%WIX%\bin\candle.exe" echo WIX Toolset not installed! & exit /b 1
"%WIX%\bin\candle.exe" out\BinComponents.wxs Product.wxs -dSourceFiles="..\MarkdownEdit\bin\Release" -out out\
if NOT ERRORLEVEL 1 "%WIX%\bin\light.exe" -ext WiXNetFxExtension -ext WixUIExtension out\Product.wixobj out\BinComponents.wixobj -out ..\..\MarkdownEditSetup.msi -sice:ICE61 -pdbout out\MarkdownEidtSetup.wixpdb
exit /b errorlevel