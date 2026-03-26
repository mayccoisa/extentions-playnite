$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$c = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)

# Characters (Unicode values)
$I_acute = [char]0x00CD
$A_acute = [char]0x00C1
$E_circ = [char]0x00CA
$O_tilde = [char]0x00D5

# Replacements using string formatting to avoid parsing issues
$c = [regex]::Replace($c, 'ESTAT[^"]*STICAS GERAIS', "ESTAT$($I_acute)STICAS GERAIS")
$c = [regex]::Replace($c, 'H[^"]*BITOS DE JOGOS', "H$($A_acute)BITOS DE JOGOS")
$c = [regex]::Replace($c, 'SESS[^"]*ES', "SESS$($O_tilde)ES")
$c = [regex]::Replace($c, 'MAIOR SEQU[^"]*NCIA', "MAIOR SEQU$($E_circ)NCIA")
$c = [regex]::Replace($c, 'CONCLU[^"]*DO', "CONCLU$($I_acute)DO")

# Ensure valid XML root (strip leading junk)
$index = $c.IndexOf('<')
if ($index -gt 0) {
    $c = $c.Substring($index)
}

# UTF-8 with BOM
$utf8bom = New-Object System.Text.UTF8Encoding($true)
[IO.File]::WriteAllText($path, $c, $utf8bom)
Write-Host "XAML Repaired with Character-Safe Regex!"
