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
        private View _mapView;
        private Text _debugText;
        public override void LoadContent()
        {
            _debugText = new Text("test", new Font(@"Resources/Fonts/arial.ttf"), 24);
            Game.TextureManager.LoadTexture("tilemap", @"Resources/Textures/tilemap.png");

            var tileSprite = new Sprite(Game.TextureManager.GetTexture("tilemap"), new IntRect(0, 0, 64, 64));
            tileSprite.Origin = new Vector2f(32, 32);

            _tileSprites = new List<Sprite>();
            _tileSprites.Add(tileSprite);

            _map = new Map(5, 5, new HexagonalTileInfo(31));

            for (int r = 0; r < _map.Height; r++) {
                for (int c = 0; c < _map.Width; c++) {
                    if (r == 0 || c == 0 || r >= _map.Height - 2 || c == _map.Width - 1) {
                        if (r == _map.Height - 1 && c%2 == 0) continue;
                        if (r == _map.Height - 2 && c%2 != 0) continue;
                        _map.SetTile(c, r, 0);
                    }
                }
            }

            _mapView = new View(Game.Window.DefaultView);

            //center view on map initially
            _mapView.Center = new Vector2f(_map.WidthInPixels/2, _map.HeightInPixels/2);
        }

        public override void HandleInput(IInputManager<Mouse.Button, Vector2i, Window, Keyboard.Key> input, TimeSpan gameTime)
        {
            base.HandleInput(input, gameTime);

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
                _mapView.Move(offset*(float) gameTime.TotalSeconds);
                Game.Window.SetView(_mapView);
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
                    if (spriteIdx < 0) {
                        continue;
                    }

                    float xPos = _map.TileInfo.GetTilePositionX(col, row),
                        yPos = _map.TileInfo.GetTilePositionY(col, row);

                    _tileSprites[spriteIdx].Position = new Vector2f(xPos, yPos);
                    window.Draw(_tileSprites[spriteIdx]);
                }
            }
        }
    }
}
