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

    Write-Host "dotnet run --project ./src/TranslationTemplateCommand/ -- single -r $PWD -t $template_path -o $output_path --data db:$db_json_path table:$table_json_path"
    dotnet run --project ./src/TranslationTemplateCommand/ -- single -r $PWD -t $template_path -o $output_path --data db:$db_json_path table:$table_json_path
}
# PrintLineSplit
# CommandExecute_Single

function WriteFileLines([string]$filePath, [string[]]$lines)
{
    $lines | Out-File -Encoding Default -Force -FilePath $filePath
}

function CommandExecute_Batch()
{
    $lines=@()
    function AddLine([string[]]$lines, [string]$name)
    {
        $output = "./_test/output/{0}/{0}Controller.cs" -f $name
        # Write-Host $output
        $lines += @(
            "./_test/template/_Basic_Controller.cs.liquid",
            $output,
            "db:./_test/data/db.json table:./_test/data/table/$name.json"
        ) -join " | "
        return $lines
    }
    $lines = AddLine $lines "AcGoodProductType"
    $lines = AddLine $lines "AcNProductBanner"

    $cacheFile = "./_test/cache_config.txt"
    WriteFileLines $cacheFile $lines
    Get-Content $cacheFile

    Write-Host "dotnet run --project ./src/TranslationTemplateCommand/ -- batch -r $PWD --config $cacheFile"
    dotnet run --project ./src/TranslationTemplateCommand/ -- batch -r $PWD --config $cacheFile

    Remove-Item -Force -Path $cacheFile
}
PrintLineSplit
CommandExecute_Batch


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
