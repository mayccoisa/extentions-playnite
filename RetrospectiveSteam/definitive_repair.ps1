$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$c = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)

# Recovery: strip anything before the first '<' to fix "invalid root data"
$index = $c.IndexOf('<')
if ($index -gt 0) {
    $c = $c.Substring($index)
}

# Repair known broken strings
$c = $c -replace 'Text="ESTAT.*? Style="{StaticResource MainSectionTitle}"', 'Text="ESTATÍSTICAS GERAIS" Style="{StaticResource MainSectionTitle}"'
$c = $c -replace 'Text="H.*? Style="{StaticResource MainSectionTitle}"', 'Text="HÁBITOS DE JOGOS" Style="{StaticResource MainSectionTitle}"'
$c = $c -replace 'Text="SESS.*? Style="{StaticResource CardTitle}"', 'Text="SESSÕES" Style="{StaticResource CardTitle}"'
$c = $c -replace 'Text="MAIOR SEQU.*? Style="{StaticResource CardTitle}"', 'Text="MAIOR SEQUÊNCIA" Style="{StaticResource CardTitle}"'
# Also fix the completion label
$c = $c -replace 'Text="CONCLU.*?Foreground="#4CAF50"', 'Text="CONCLUÍDO" Foreground="#4CAF50"'

# Save with BOM
[IO.File]::WriteAllText($path, $c, [Text.UTF8Encoding]::new($true))
Write-Host "XAML Repaired and Cleaned!"
