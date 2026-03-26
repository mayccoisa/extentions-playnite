$path = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$content = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)

# Use regex with explicit hex for the broken characters if possible, 
# but let's try a simpler approach: replace the whole line by matching the structure
$content = $content -replace '<TextBlock Text="ESTAT.*? Style="{StaticResource MainSectionTitle}"/>', '<TextBlock Text="ESTATÍSTICAS GERAIS" Style="{StaticResource MainSectionTitle}"/>'
$content = $content -replace '<TextBlock Text="H.*? Style="{StaticResource MainSectionTitle}"/>', '<TextBlock Text="HÁBITOS DE JOGOS" Style="{StaticResource MainSectionTitle}"/>'
$content = $content -replace '<TextBlock Text="SESS.*? Style="{StaticResource CardTitle}" />', '<TextBlock Text="SESSÕES" Style="{StaticResource CardTitle}" />'
$content = $content -replace '<TextBlock Text="MAIOR SEQU.*? Style="{StaticResource CardTitle}" />', '<TextBlock Text="MAIOR SEQUÊNCIA" Style="{StaticResource CardTitle}" />'

[IO.File]::WriteAllText($path, $content, [Text.UTF8Encoding]::new($true))
