using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HypervisorOverlay.Views
{
    public partial class HOverlayControl : UserControl
    {
        private const string HypervisorTagName = "hypervisor";

        public HOverlayControl()
        {
            InitializeComponent();
            this.DataContextChanged += HOverlayControl_DataContextChanged;
            // Ocultar por padrão até que um jogo válido seja carregado
            this.Visibility = Visibility.Collapsed;
        }

        private void HOverlayControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisibility(e.NewValue as Game);
        }

        private void UpdateVisibility(Game game)
        {
            if (game == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            // O SDK antigo pode exigir acesso à API via API estática se não for passada
            // mas geralmente o DataContext em Views injetadas é o próprio objeto Game.
            // Para verificar a Tag, precisamos da lista de Tags do banco de dados.
            
            try 
            {
                var api = API.Instance;
                if (api == null) return;

                var hasTag = game.TagIds != null && 
                             api.Database.Tags.Any(t => t.Name.Equals(HypervisorTagName, StringComparison.OrdinalIgnoreCase) && game.TagIds.Contains(t.Id));
                
                this.Visibility = hasTag ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }
    }
}
