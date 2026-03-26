$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$c = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)

# Robust regex replacement that ignores specific corrupted characters
$c = [regex]::Replace($c, 'Text="ESTAT[^"]*STICAS GERAIS"', 'Text="ESTATÍSTICAS GERAIS"')
$c = [regex]::Replace($c, 'Text="H[^"]*BITOS DE JOGOS"', 'Text="HÁBITOS DE JOGOS"')
$c = [regex]::Replace($c, 'Text="SESS[^"]*ES"', 'Text="SESSÕES"')
$c = [regex]::Replace($c, 'Text="MAIOR SEQU[^"]*NCIA"', 'Text="MAIOR SEQUÊNCIA"')
$c = [regex]::Replace($c, 'Text="CONCLU[^"]*DO"', 'Text="CONCLUÍDO"')

# Ensure valid XML root (strip leading junk)
$index = $c.IndexOf('<')
if ($index -gt 0) {
    $c = $c.Substring($index)
}

[IO.File]::WriteAllText($path, $c, [Text.UTF8Encoding]::new($true))
Write-Host "XAML Repaired with Regex!"
