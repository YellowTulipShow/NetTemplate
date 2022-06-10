$ExecutePath = $PWD
Set-Location $PSScriptRoot
dotnet tool install -g --add-source ./_release/packages TranslationTemplateCommand

Write-Output "PSScriptRoot: $PSScriptRoot"

ctemp -r "$PSScriptRoot" -t ./template/index.html -o ./_output/index.html --data db:./template/db.json table:./template/table.json



Set-Location $ExecutePath
if ($PSScriptRoot -eq $ExecutePath) {
    timeout.exe /T -1
}
