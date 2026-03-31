$path = 'c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml'
$c = Get-Content $path -Raw
$c = $c -replace '(?s)<<<<<<< HEAD.*?=======.*?\n(.*?)>>>>>>> origin/main', '$1'
[System.IO.File]::WriteAllText($path, $c, [System.Text.Encoding]::UTF8)
