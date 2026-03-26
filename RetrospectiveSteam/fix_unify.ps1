$path = 'c:\Users\WEON-ADMIN\Downloads\extensao playnite\RetrospectiveSteam\RetrospectiveSteam\Views\RetrospectiveView.xaml'
$lines = Get-Content $path
$newLines = @()
for ($i = 0; $i -lt $lines.Length; $i++) {
    if ($i -ge 514 -and $i -le 519) { continue } # Line 515-520 (0-indexed)
    $newLines += $lines[$i]
}
$newLines | Set-Content $path
