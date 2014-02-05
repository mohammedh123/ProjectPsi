﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectPsi.Core;
using ProjectPsi.GUI.Interfaces;
using ProjectPsi.GUI.Managers;
using ProjectPsi.GUI.Screens;
using SFML.Graphics;
using SFML.Window;

namespace ProjectPsi.GUI
{
    class MainGame : IDisposable
    {
        public ITextureManager<Texture> TextureManager { get; private set; }

        public IScreenManager ScreenManager { get; private set; }

        public IMouseManager<Mouse.Button, Vector2i, Window> MouseManager { get; private set; }

        public IKeyStateManager<Keyboard.Key> KeyboardManager { get; private set; }

        public IInputManager<Mouse.Button, Vector2i, Window, Keyboard.Key> InputManager { get; private set; }

        public bool IsActive { get; private set; }

        public TimeSpan LastFrameTime { get; private set; }

        public RenderWindow Window { get; private set; }

        public void Run(int updatesPerSecond)
        {
            IsActive = true;
            
            Window = new RenderWindow(new VideoMode(800,600), "ProjectPsi", Styles.Default);
            Window.SetFramerateLimit(60);

            Window.GainedFocus += OnGainedFocus;
            Window.LostFocus += OnLostFocus;
            Window.Closed += OnClosed;
            Window.KeyPressed += OnKeyPressed;
            Window.KeyReleased += OnKeyReleased;
            Window.MouseButtonPressed += OnMouseButtonPressed;
            Window.MouseButtonReleased += OnMouseButtonReleased;
                
            Window.SetActive(IsActive);
            
            TextureManager = new TextureManager();

            MouseManager = new MouseManager();
            KeyboardManager = new KeyboardManager();
            InputManager = new InputManager(MouseManager, KeyboardManager, Window);

            ScreenManager = new ScreenManager(InputManager, this);
            ScreenManager.Initialize();
            // add screens
            ScreenManager.AddScreen(new TestScreen());
            ScreenManager.LoadContent();

            var clock = new Stopwatch();
            clock.Start();
            // run the program as long as the window is open
            while (Window.IsOpen()) {
                LastFrameTime = clock.Elapsed;
                clock.Restart();

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
            KeyboardManager.UpdateKey(e.Code, true);
        }
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            KeyboardManager.UpdateKey(e.Code, false);
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            MouseManager.UpdateKey(e.Button, true);
        }
        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            MouseManager.UpdateKey(e.Button, false);
        }

        #endregion

        private void Update()
        {
            InputManager.Update();

            ScreenManager.Update(LastFrameTime);

            InputManager.PostUpdate();
        }

        private void RenderFrame(RenderWindow window)
        {
            ScreenManager.Draw(LastFrameTime);

            window.Display();
        }

        public void Dispose()
        {
        }
    }
}
