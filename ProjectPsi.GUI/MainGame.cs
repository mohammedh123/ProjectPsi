using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectPsi.Core;
using ProjectPsi.GUI.Interfaces;
using ProjectPsi.GUI.Managers;
using SFML.Graphics;
using SFML.Window;

namespace ProjectPsi.GUI
{
    public class MainGame : IDisposable
    {
        private Map _map;
        private TextureManager _textureManager;
        private ScreenManager _screenManager;
        private IMouseManager<Mouse.Button, Vector2i, Window> _mouseManager; 
        private IKeyStateManager<Keyboard.Key> _kbManager;
        private IInputManager<Mouse.Button, Vector2i, Window, Keyboard.Key> _inputManager;
        private List<Sprite> _tileSprites;
        private View _mapView;

        public bool IsActive { get; set; }

        public TimeSpan LastFrameTime { get; set; }

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

            Window.GainedFocus += OnGainedFocus;
            Window.LostFocus += OnLostFocus;
            Window.Closed += OnClosed;
            Window.KeyPressed += OnKeyPressed;
            Window.MouseButtonPressed += OnMouseButtonPressed;
                
            Window.SetActive(IsActive);

            _mouseManager = new MouseManager();
            _kbManager = new KeyboardManager();
            _inputManager = new InputManager(_mouseManager, _kbManager, Window);

            _screenManager = new ScreenManager(_inputManager, this);
            _screenManager.Initialize();
            // add screens
            _screenManager.LoadContent();

            _mapView = Window.GetView();

            var clock = new Stopwatch();
            clock.Start();
            // run the program as long as the window is open
            while (Window.IsOpen()) {
                LastFrameTime = clock.Elapsed;

                Window.DispatchEvents();

                Update();

                RenderFrame(Window);
            }
        }

        #region Event Handlers (simple) 

        private void OnGainedFocus(object sender, EventArgs eventArgs)
        {
            IsActive = true;
        }

        private void OnLostFocus(object sender, EventArgs eventArgs)
        {
            IsActive = false;
        }
        
        private void OnClosed(object sender, EventArgs e)
        {
            Window.Close();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            _kbManager.UpdateKey(e.Code);
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            _mouseManager.UpdateKey(e.Button);
        }

        #endregion

        private void Update()
        {
            _inputManager.Update();

            _screenManager.Update(LastFrameTime);

            _inputManager.PostUpdate();
        }

        private void RenderFrame(RenderWindow window)
        {
            _screenManager.Draw(LastFrameTime);
            //window.Clear();

            //DrawMap(window);

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
                    if (spriteIdx < 0) {
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
