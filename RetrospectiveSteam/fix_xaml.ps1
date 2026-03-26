$path = 'e:\Playnite\extensao playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml'
$content = Get-Content $path
$newContent = New-Object System.Collections.Generic.List[string]
for ($i = 0; $i -lt $content.Count; $i++) {
    $line = $content[$i]
    $num = $i + 1
    # Fix 1738
    if ($num -eq 1738 -and $line.Trim() -eq "") {
        $newContent.Add("                                                      </Border.Height>")
        continue
    }
    # Delete 589-595
    if ($num -ge 589 -and $num -le 595) {
        continue
    }
    # Insert Grid at 557
    if ($num -eq 557) {
        # Keep DataTemplate line (556 is index 555, but num is 557 means line 557)
        # Wait! In view_file (Step 1182):
        # 556: <DataTemplate>
        # 557: <StackPanel ...>
        # So we keep 556, and at 557 we insert Grid BEFORE StackPanel.
        $newContent.Add('                                              <Grid VerticalAlignment="Bottom" Margin="4,0" Height="260">')
        $newContent.Add('                                                  <Grid.RowDefinitions>')
        $newContent.Add('                                                      <RowDefinition Height="*"/>')
        $newContent.Add('                                                      <RowDefinition Height="Auto"/>')
        $newContent.Add('                                                      <RowDefinition Height="Auto"/>')
        $newContent.Add('                                                  </Grid.RowDefinitions>')
    }
    $newContent.Add($line)
}
$newContent | Set-Content $path -Encoding UTF8
