$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$bytes = [IO.File]::ReadAllBytes($path)
$content = [Text.Encoding]::UTF8.GetString($bytes)

# Target the exact sequences found in hex
$content = $content -replace "ESTATÃSTICAS", "ESTATÍSTICAS"
$content = $content -replace "HÃBITOS", "HÁBITOS"
$content = $content -replace "SESSÃ•ES", "SESSÕES"
$content = $content -replace "MAIOR SEQUÃŠNCIA", "MAIOR SEQUÊNCIA"
# Also fix any remaining "CONCLUÃDO" related garbage
$content = $content -replace "CONCLUÃ\s+DO", "CONCLUÍDO"
$content = $content -replace "CONCLUÃDO", "CONCLUÍDO"

[IO.File]::WriteAllText($path, $content, [Text.UTF8Encoding]::new($true))
