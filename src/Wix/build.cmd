del /Q out\*.msi
@"c:\Program Files (x86)\WiX Toolset v3.9\bin\candle.exe" Product.wxs -dSourceFiles="..\MarkdownEdit\bin\Release" -out out\
@if NOT ERRORLEVEL 1 "c:\Program Files (x86)\WiX Toolset v3.9\bin\light.exe" -ext WiXNetFxExtension -ext WixUIExtension out\Product.wixobj -out out\MarkdownEditSetup.msi
@if NOT ERRORLEVEL 1 mv out\*.msi ..\..