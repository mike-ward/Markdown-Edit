"%WIX%\bin\heat" dir "..\MarkdownEdit\bin\Release" -cg "BinComponents" -ag -scom -suid -sreg -sfrag -srd -dr INSTALLFOLDER -var var.SourceFiles -indent 2 -out out\BinComponents.wxs 
  