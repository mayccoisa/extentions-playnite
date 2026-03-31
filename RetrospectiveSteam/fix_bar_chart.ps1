$file = 'c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml'
$allLines = [System.IO.File]::ReadAllLines($file, [System.Text.Encoding]::UTF8)

# After index 641 (</Grid> for the bar Grid) we need to insert </DataTemplate>
$before = $allLines[0..641]
$missing = [string[]]@('                                          </DataTemplate>')
$after = $allLines[642..($allLines.Length - 1)]

$result = $before + $missing + $after
[System.IO.File]::WriteAllLines($file, $result, [System.Text.Encoding]::UTF8)
Write-Host "SUCCESS. Total lines: $($result.Length)"

for ($i = 638; $i -le 648; $i++) {
    Write-Host "$($i+1): $($result[$i])"
}
