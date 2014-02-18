using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPsi.Core;
using ProjectPsi.GUI.Interfaces;
using SFML.Graphics;
using SFML.Window;

namespace ProjectPsi.GUI.Screens
{
    internal class TestScreen : GameScreen
    {
        private Map _map;
        private List<Sprite> _tileSprites;
        private Sprite _emptyTileSprite;
        private View _mapView;
        private Text _debugText;

        private float ViewportLeftEdge
        {
            get { return _mapView.Center.X - _mapView.Size.X/2; }
        }

        private float ViewportTopEdge
        {
            get { return _mapView.Center.Y - _mapView.Size.Y/2; }
        }

        private float ViewportRightEdge
        {
            get { return _mapView.Center.X + _mapView.Size.X/2; }
        }

        private float ViewportBottomEdge
        {
            get { return _mapView.Center.Y + _mapView.Size.Y/2; }
        }

        public override void LoadContent()
        {
            _debugText = new Text("test", new Font(@"Resources/Fonts/arial.ttf"), 24);
            Game.TextureManager.LoadTexture("tilemap", @"Resources/Textures/tilemap.png");

            var tileSprite = new Sprite(Game.TextureManager.GetTexture("tilemap"), new IntRect(64, 0, 64, 64));
            tileSprite.Origin = new Vector2f(32, 32);

            _tileSprites = new List<Sprite>();
            _tileSprites.Add(tileSprite);

            _emptyTileSprite = new Sprite(Game.TextureManager.GetTexture("tilemap"), new IntRect(0, 0, 64, 64));
            _emptyTileSprite.Origin = new Vector2f(32, 32);

            _map = new Map(9, 9, new HexagonalTileInfo(31));

            for (int r = 0; r < _map.Height; r++) {
                for (int c = 0; c < _map.Width; c++) {
                    if (r == 0 || c == 0 || r >= _map.Height - 1 || c == _map.Width - 1) {
                        _map.SetTile(c, r, 0);
                    }
                }
            }
            _map.RecalculateEdges();

            _mapView = new View(Game.Window.DefaultView);

            //center view on map initially
            _mapView.Center = new Vector2f(_map.WidthInPixels/2, _map.HeightInPixels/2);
        }

        public override void HandleInput(IInputManager<Mouse.Button, Vector2i, Window, Keyboard.Key> input, TimeSpan gameTime)
        {
            base.HandleInput(input, gameTime);

            HandleCameraInput(input, gameTime);
        }

        private void HandleCameraInput(IInputManager<Mouse.Button, Vector2i, Window, Keyboard.Key> input, TimeSpan gameTime)
        {
            var offset = new Vector2f();

            if (input.Keyboard.IsKeyDown(Keyboard.Key.Left)) {
                offset = new Vector2f(offset.X - GameConstants.PanningSpeed, offset.Y);
            }
            if (input.Keyboard.IsKeyDown(Keyboard.Key.Up)) {
                offset = new Vector2f(offset.X, offset.Y - GameConstants.PanningSpeed);
            }
            if (input.Keyboard.IsKeyDown(Keyboard.Key.Right)) {
                offset = new Vector2f(offset.X + GameConstants.PanningSpeed, offset.Y);
            }
            if (input.Keyboard.IsKeyDown(Keyboard.Key.Down)) {
                offset = new Vector2f(offset.X, offset.Y + GameConstants.PanningSpeed);
            }

            if (offset.X != 0 || offset.Y != 0) {
                var viewport = _mapView.Size;
                var viewportCenter = viewport/2;

                //update the center
                _mapView.Center += offset*(float) gameTime.TotalSeconds;

                // if the total mapWidth is less than the size of the viewport (X-axis), then allow to scroll freely within viewport for that axis
                // otherwise, scroll to edge
                if (_map.WidthInPixels < viewport.X) {
                    if (ViewportLeftEdge > _map.LeftEdgeInPixels) {
                        _mapView.Center = new Vector2f(_map.LeftEdgeInPixels + viewportCenter.X, _mapView.Center.Y);
                    }
                    if (ViewportRightEdge < _map.RightEdgeInPixels) {
                        _mapView.Center = new Vector2f(_map.RightEdgeInPixels - viewportCenter.X, _mapView.Center.Y);
                    }
                }
                else {
                    if (ViewportLeftEdge < _map.LeftEdgeInPixels) {
                        _mapView.Center = new Vector2f(_map.LeftEdgeInPixels + viewportCenter.X, _mapView.Center.Y);
                    }
                    if (ViewportRightEdge > _map.RightEdgeInPixels) {
                        _mapView.Center = new Vector2f(_map.RightEdgeInPixels - viewportCenter.X, _mapView.Center.Y);
                    }
                }

                // if the total mapHeight is less than the size of the viewport (Y-axis), then allow to scroll freely within viewport for that axis
                // otherwise, scroll to edge
                if (_map.HeightInPixels < viewport.Y) {
                    if (ViewportTopEdge > _map.TopEdgeInPixels) {
                        _mapView.Center = new Vector2f(_mapView.Center.X, _map.TopEdgeInPixels + viewportCenter.Y);
                    }
                    if (ViewportBottomEdge < _map.BottomEdgeInPixels) {
                        _mapView.Center = new Vector2f(_mapView.Center.X, _map.BottomEdgeInPixels - viewportCenter.Y);
                    }
                }
                else {
                    if (ViewportTopEdge < _map.TopEdgeInPixels) {
                        _mapView.Center = new Vector2f(_mapView.Center.X, _map.TopEdgeInPixels + viewportCenter.Y);
                    }
                    if (ViewportBottomEdge > _map.BottomEdgeInPixels) {
                        _mapView.Center = new Vector2f(_mapView.Center.X, _map.BottomEdgeInPixels - viewportCenter.Y);
                    }
                }
            }
        }

        public override void Update(TimeSpan gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(TimeSpan gameTime)
        {
            Game.Window.Clear();

            Game.Window.SetView(_mapView);
            DrawMap(Game.Window);

            var center = _mapView.Center;
            var viewport = _mapView.Size;
            var viewportCenter = viewport/2;

            _debugText.DisplayedString = String.Format("{0}, {1}", center.X - viewportCenter.X, center.X + viewportCenter.X);
            Game.Window.SetView(Game.Window.DefaultView);
            _debugText.Color = Color.Red;
            Game.Window.Draw(_debugText);
        }

        private void DrawMap(RenderTarget window)
        {
            for (var row = 0; row < _map.Height; row++) {
                for (var col = 0; col < _map.Width; col++) {
                    //even-q vertical layout

                    var spriteIdx = _map.Tiles[row, col];

                    var spriteToUse = spriteIdx < 0 ? _emptyTileSprite : _tileSprites[spriteIdx];

                    float xPos = _map.TileInfo.GetTilePositionX(col, row),
                        yPos = _map.TileInfo.GetTilePositionY(col, row);

                    spriteToUse.Position = new Vector2f(xPos, yPos);
                    window.Draw(spriteToUse);
                }
            }
        }
    }
}
