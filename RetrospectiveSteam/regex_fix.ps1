$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$content = [System.IO.File]::ReadAllText($path)

# Comprehensive Regex to fix corrupted strings
$content = $content -replace 'ESTAT[^"]*STICAS GERAIS', 'ESTATÍSTICAS GERAIS'
$content = $content -replace 'H[^"]*BITOS DE JOGOS', 'HÁBITOS DE JOGOS'
$content = $content -replace 'CALEND[^"]*RIO DE ATIVIDADES', 'CALENDÁRIO DE ATIVIDADES'
$content = $content -replace 'PERFIL[^"]*RIO', 'PERFIL HORÁRIO'
$content = $content -replace 'VOC[^"]*MAIS JOGA', 'VOCÊ MAIS JOGA'
$content = $content -replace 'DISTRIBUI[^"<>]*O POR PLATAFORMA', 'DISTRIBUIÇÃO POR PLATAFORMA'
$content = $content -replace 'DISTRIBUI[^"<>]*O POR G[^"<> ]*NERO', 'DISTRIBUIÇÃO POR GÊNERO'
$content = $content -replace 'RECAP[^"<> ]*TULO', 'RECAPÍTULO'
$content = $content -replace 'MAIOR SEQU[^"<> ]*NCIA', 'MAIOR SEQUÊNCIA'
$content = $content -replace 'SESS[^"<>]*ES', 'SESSÕES'
$content = $content -replace 'L[^"<> ]*GICA DO ALGORITMO', 'LÓGICA DO ALGORITMO'

$utf8BOM = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText($path, $content, $utf8BOM)
Write-Host "XAML Encoding and Accents fixed using comprehensive regex."
