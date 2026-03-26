$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$content = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)
$lines = $content -split "`n"
$changed = $false
for ($i=0; $i -lt $lines.Length; $i++) {
    if ($lines[$i] -match "ESTAT.*?STICAS.*?GERAIS") {
        $lines[$i] = '            <TextBlock Text="ESTATÍSTICAS GERAIS" Style="{StaticResource MainSectionTitle}"/>'
        $changed = $true
    }
    if ($lines[$i] -match "H.*?BITOS.*?DE.*?JOGOS") {
        $lines[$i] = '            <TextBlock Text="HÁBITOS DE JOGOS" Style="{StaticResource MainSectionTitle}"/>'
        $changed = $true
    }
}
if ($changed) {
    $newContent = $lines -join "`n"
    [IO.File]::WriteAllText($path, $newContent, [Text.UTF8Encoding]::new($true))
    Write-Host "File updated!"
} else {
    Write-Host "No matches found!"
}
