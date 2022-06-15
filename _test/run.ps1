$ExecutePath = $PWD
Set-Location $PSScriptRoot
Set-Location ..

function PrintLineSplit([string]$path)
{
    Write-Host ""
    Write-Host "======================================================================================="
    Write-Host ""
}

function CommandExecute_Single()
{
    $template_path = "./_test/template/_Basic_Controller.cs.liquid"
    $output_path = "./_test/output/AcGoodProductType/AcGoodProductTypeController.cs"
    $db_json_path = "./_test/data/db.json"
    $table_json_path = "./_test/data/table/AcGoodProductType.json"

    Write-Host "dotnet run --project ./src/TranslationTemplateCommand/ -- single -r $PWD -t $template_path -o $output_path --data db:$db_json_path --data table:$table_json_path"
    dotnet run --project ./src/TranslationTemplateCommand/ -- single -r $PWD -t $template_path -o $output_path --data db:$db_json_path --data table:$table_json_path
}
PrintLineSplit
CommandExecute_Single

function WriteFileLines([string]$filePath, [string[]]$lines)
{
    $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
    [System.IO.File]::WriteAllLines($filePath, $lines, $Utf8NoBomEncoding)
}

function CommandExecute_Single()
{
    $lines=@()
    function AddLine([string[]]$lines, [string]$name)
    {
        $lines += @(
            "./_test/template/_Basic_Controller.cs.liquid",
            "./_test/output/${$name}/${$name}Controller.cs",
            "db:./_test/data/db.json",
            "table:./_test/data/table/${$name}.json"
        ) -join " | "
        return $lines
    }
    $lines = AddLine $lines "AcGoodProductType"
    $lines = AddLine $lines "AcNProductBanner"
    WriteFileLines "./_test/cache_config.txt" $lines
}
# PrintLineSplit
# CommandExecute_Single


# # 打印输出测试用
# Write-Host "[Template Content]:"
# $content = Get-Content $template_path
# Write-Host $content

# Write-Host "[Result Content]:"
# $content = Get-Content $output_path
# Write-Host $content

PrintLineSplit

Set-Location $ExecutePath
if ($PSScriptRoot -eq $ExecutePath) {
    timeout.exe /T -1
}
