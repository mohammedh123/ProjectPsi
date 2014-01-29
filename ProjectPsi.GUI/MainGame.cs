using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectPsi.Core;
using SFML.Graphics;
using SFML.Window;

namespace ProjectPsi.GUI
{
    public class MainGame : IDisposable
    {
        private Map _map;
        private TextureManager _textureManager;
        private List<Sprite> _tileSprites;
        private View _mapView;

        public bool IsActive { get; set; }

        public RenderWindow Window { get; set; }

        public void Run(int updatesPerSecond)
        {
            IsActive = true;

            _textureManager = new TextureManager();
            _textureManager.LoadTexture("tilemap", @"Resources/Textures/tilemap.png");

            var tileSprite = new Sprite(_textureManager.GetTexture("tilemap"), new IntRect(0,0,64,64));
            tileSprite.Origin = new Vector2f(32, 32);

            _tileSprites = new List<Sprite>();
            _tileSprites.Add(tileSprite);

            _map = new Map(18, 15, 31);

            for (int r = 0; r < _map.Height; r++) {
                for (int c = 0; c < _map.Width; c++) {
                    if (r == 0 || c == 0 || r == _map.Height - 1 || c == _map.Width - 1) {
                        _map.SetTile(c, r, 0);
                    }
                }
            }
            
            Window = new RenderWindow(new VideoMode(800,600), "ProjectPsi", Styles.Default);
            Window.SetFramerateLimit(60);

            Window.Closed += OnClosed;
            Window.KeyPressed += OnKeyPressed;
            Window.MouseButtonPressed += OnMouseButtonPressed;
            Window.LostFocus += OnLostFocus;
            Window.GainedFocus += OnGainedFocus;
                
            Window.SetActive(IsActive);

            _mapView = Window.GetView();

            var clock = new Stopwatch();
            clock.Start();
            // run the program as long as the window is open
            while (Window.IsOpen()) {
                var dt = clock.Elapsed.TotalSeconds;

                Window.DispatchEvents();

                Update();

                RenderFrame(Window);
            }
        }

        private void OnGainedFocus(object sender, EventArgs eventArgs)
        {
            IsActive = true;
        }

        private void OnLostFocus(object sender, EventArgs eventArgs)
        {
            IsActive = false;
        }
        
        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Window.Close();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                Window.Close();
        }

        private void Update()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left)) {
            }
        }

        private void RenderFrame(RenderWindow window)
        {
            window.Clear();

            DrawMap(window);

            window.Display();
        }

        public void DrawMap(RenderWindow window)
        {
            var tileWidth = _map.TileRadius*2;
            var tileHeight = (float) (tileWidth*Math.Sin(Math.PI/3));

            for (var row = 0; row < _map.Height; row++) {
                for (var col = 0; col < _map.Width; col++) {
                    //even-q vertical layout

                    var spriteIdx = _map.Tiles[row, col];
                    if (spriteIdx < 0)
                    {
                        continue;
                    }

                    var xPos = col * tileWidth * 0.75f;
                    var yPos = row * tileHeight;

                    if (col%2 != 0) {
                        yPos -= tileHeight*0.5f;
                    }

                    _tileSprites[spriteIdx].Position = new Vector2f(xPos, yPos);
                    window.Draw(_tileSprites[spriteIdx]);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
