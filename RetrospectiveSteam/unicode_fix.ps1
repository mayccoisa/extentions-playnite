$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$content = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)

# Replace using unicode escapes for the corrupted characters
# ESTATÃ STICAS -> ESTATÍSTICAS
$content = $content -replace "ESTAT\u00C3\s+STICAS", "ESTATÍSTICAS"
# HÃ BITOS -> HÁBITOS
$content = $content -replace "H\u00C3\s+BITOS", "HÁBITOS"
# SESSÃ•ES -> SESSÕES
$content = $content -replace "SESS\u00C3\u2022ES", "SESSÕES"
# MAIOR SEQUÃŠNCIA -> MAIOR SEQUÊNCIA
$content = $content -replace "MAIOR SEQU\u00C3\u0160NCIA", "MAIOR SEQUÊNCIA"

[IO.File]::WriteAllText($path, $content, [Text.UTF8Encoding]::new($true))
