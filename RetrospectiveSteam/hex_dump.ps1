$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$bytes = [IO.File]::ReadAllBytes($path)
$content = [Text.Encoding]::UTF8.GetString($bytes)
$lines = $content -split "`r?`n"
$line = $lines[238]
$lineBytes = [Text.Encoding]::UTF8.GetBytes($line)
$hex = $lineBytes | ForEach-Object { "{0:X2}" -f $_ }
Write-Host ($hex -join " ")
Write-Host $line
