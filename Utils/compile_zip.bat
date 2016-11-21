mkdir C:\Git\CodeWizards2016\Utils\TempZip\ 

FORFILES /P C:\Git\CodeWizards2016\csharp-cgdk\MyClasses /S /M *.cs /C "cmd /c copy @path C:\Git\CodeWizards2016\Utils\TempZip\"

cmd /c copy "C:\Git\CodeWizards2016\csharp-cgdk\MyStrategy.cs" "C:\Git\CodeWizards2016\Utils\TempZip\"
"C:\Program Files (x86)\7-Zip\7z.exe" a bot.zip @listfiles.txt

rd /s /q C:\Git\CodeWizards2016\Utils\TempZip\ 