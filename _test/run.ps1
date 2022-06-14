$ExecutePath = $PWD
Set-Location $PSScriptRoot
# Set-Location ..

function PrintLineSplit([string]$path)
{
    Write-Host ""
    Write-Host "======================================================================================="
    Write-Host ""
}

PrintLineSplit

$template_path = "./template/index.html"
$output_path = "./output/index.html"
$db_json_path = "./data/db.json"
$table_json_path = "./data/table.json"

Write-Host "ctemp -r $PWD -t $template_path -o $output_path --data db:$db_json_path table:$table_json_path"
ctemp -r $PWD -t $template_path -o $output_path --data db:$db_json_path table:$table_json_path

# 打印输出测试用
Write-Host "[Template Content]:"
$content = Get-Content $template_path
Write-Host $content

Write-Host "[Result Content]:"
$content = Get-Content $output_path
Write-Host $content

PrintLineSplit

Set-Location $ExecutePath
if ($PSScriptRoot -eq $ExecutePath) {
    timeout.exe /T -1
}
