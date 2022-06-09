$ExecutePath = $PWD
Set-Location $PSScriptRoot

Write-Output "PSScriptRoot: $PSScriptRoot"
# .\_release\win-x64\TranslationTemplateCommand\TranslationTemplateCommand.exe -r "D:\Work\YTS.ZRQ\NetTemplate" -t .\template\index.html -o .\_output\index.html --data db:./template/db.json table:.\template\table.json

Set-Location $ExecutePath
if ($PSScriptRoot -eq $ExecutePath) {
    timeout.exe /T -1
}
