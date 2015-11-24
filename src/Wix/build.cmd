del /Q "out\*.msi"
md out
@"%WIX%\bin\candle.exe" Product.wxs -dSourceFiles="..\MarkdownEdit\bin\Release" -out out\
@if NOT ERRORLEVEL 1 "%WIX%\bin\light.exe" -ext WiXNetFxExtension -ext WixUIExtension out\Product.wixobj -out out\MarkdownEditSetup.msi -sice:ICE61
@if NOT ERRORLEVEL 1 mv "out\MarkdownEditSetup.msi" "..\.."
exit /b errorlevel