$path = 'c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml'
$content = [System.IO.File]::ReadAllText($path)

# Fix triple braces
$content = $content -replace '\}\}\}', '}}'

# Repair corrupted CONCLUÍDO (it has a space in the middle sometimes)
$content = $content -replace 'CONCLU.*?DO', 'CONCLUÍDO'

[System.IO.File]::WriteAllText($path, $content, [System.Text.UTF8Encoding]::new($true))
Write-Host "XAML syntax and accents repaired (again)."
