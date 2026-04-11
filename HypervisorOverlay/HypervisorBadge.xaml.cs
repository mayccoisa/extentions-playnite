using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Controls;

namespace HypervisorOverlay
{
    public partial class HypervisorBadge : PluginUserControl
    {
        private readonly IPlayniteAPI _api;
        private Game _activeGame;

        public HypervisorBadge(IPlayniteAPI api)
        {
            _api = api;
            InitializeComponent();
            this.DataContextChanged += HypervisorBadge_DataContextChanged;
        }

        private void HypervisorBadge_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_activeGame != null)
            {
                _activeGame.PropertyChanged -= ActiveGame_PropertyChanged;
            }

            _activeGame = e.NewValue as Game;
            if (_activeGame == null)
            {
                BadgeBorder.Visibility = Visibility.Collapsed;
                return;
            }

            _activeGame.PropertyChanged += ActiveGame_PropertyChanged;
            UpdateBadgeVisibility(_activeGame);
        }

        private void ActiveGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TagIds")
            {
                var game = sender as Game;
                if (game != null)
                {
                    UpdateBadgeVisibility(game);
                }
            }
        }

        private void UpdateBadgeVisibility(Game game)
        {
            bool hasTag = false;
            if (game.TagIds != null)
            {
                hasTag = game.TagIds.Any(tagId => {
                    var tag = _api.Database.Tags.Get(tagId);
                    return tag != null && tag.Name.Equals("hypervisor", StringComparison.OrdinalIgnoreCase);
                });
            }

            BadgeBorder.Visibility = hasTag ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
