$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$lines = Get-Content $path
$lines[238] = $lines[238] -replace 'Text=".*?"', 'Text="ESTATÍSTICAS GERAIS"'
$lines[432] = $lines[432] -replace 'Text=".*?"', 'Text="HÁBITOS DE JOGOS"'
$lines[1850] = $lines[1850] -replace 'Text=".*?"', 'Text="CONCLUÍDO"'
[IO.File]::WriteAllLines($path, $lines, [Text.UTF8Encoding]::new($true))
