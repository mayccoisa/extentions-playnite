$xamlPath = "c:\proj\extentions-playnite\RetrospectiveSteam\Views\RetrospectiveView.xaml"
$xaml = Get-Content $xamlPath -Raw

# 1. Remove Completed Icon Overlay in Favorite Games
$oldOverlay = @"
                                    <!-- Completed Icon Overlay -->
                                    <Border VerticalAlignment="Top" HorizontalAlignment="Right" Margin="8"
                                            Background="#4CAF50" Width="26" Height="26" CornerRadius="13"
                                            Visibility="{Binding IsCompleted, Converter={StaticResource BooleanToVisibilityConverter}}">
                                        <TextBlock Text="&#xE73E;" FontFamily="Segoe MDL2 Assets" Foreground="White" 
                                                   FontSize="14" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                                    </Border>
"@
$xaml = $xaml.Replace($oldOverlay, "")

# 2. Update Playtime Footer in Favorite Games to include insignias
$oldFooter = @"
                                        <TextBlock Text="{Binding PlaytimeFormatted}"
                                                   Foreground="{StaticResource AcentoCiano}"
                                                   FontSize="16" FontWeight="Black" FontFamily="{StaticResource FonteNumeros}"
                                                   TextAlignment="Left"/>
"@
$newFooter = @"
                                        <StackPanel Orientation="Horizontal">
                                            <TextBlock Text="{Binding PlaytimeFormatted}"
                                                       Foreground="{StaticResource AcentoCiano}"
                                                       FontSize="16" FontWeight="Black" FontFamily="{StaticResource FonteNumeros}"
                                                       TextAlignment="Left" VerticalAlignment="Center"/>
                                            
                                            <!-- Insignias Row -->
                                            <StackPanel Orientation="Horizontal" Margin="10,0,0,0" VerticalAlignment="Center">
                                                <!-- NOVO Badge -->
                                                <Border Background="#fbca03" CornerRadius="3" Padding="4,1" Margin="0,0,4,0"
                                                        Visibility="{Binding IsFirstTime, Converter={StaticResource BooleanToVisibilityConverter}}">
                                                    <TextBlock Text="NOVO" Foreground="Black" FontSize="9" FontWeight="Black" VerticalAlignment="Center"/>
                                                </Border>
                                                <!-- CHECK Badge -->
                                                <Border Background="#4CAF50" CornerRadius="10" Width="20" Height="20"
                                                        Visibility="{Binding IsCompleted, Converter={StaticResource BooleanToVisibilityConverter}}"
                                                        ToolTip="Concluído">
                                                    <TextBlock Text="&#xE73E;" FontFamily="Segoe MDL2 Assets" Foreground="White" 
                                                               FontSize="10" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                                                </Border>
                                            </StackPanel>
                                        </StackPanel>
"@
$xaml = $xaml.Replace($oldFooter, $newFooter)

Set-Content $xamlPath $xaml -NoNewline
Write-Host "XAML de Favoritos atualizado com sucesso!"
