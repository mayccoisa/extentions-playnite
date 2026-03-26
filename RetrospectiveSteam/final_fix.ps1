$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$content = [IO.File]::ReadAllText($path)

# Special handling for these two pesky strings
$content = $content -replace "ESTAT.*?STICAS", "ESTATÍSTICAS"
$content = $content -replace "H.*?BITOS", "HÁBITOS"

[IO.File]::WriteAllText($path, $content, [Text.UTF8Encoding]::new($true))
