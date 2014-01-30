using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPsi.GUI.Interfaces;
using SFML.Window;

namespace ProjectPsi.GUI
{
    /// <summary>
    /// An implementation of the InputManager specifically for SFML.NET.
    /// </summary>
    class InputManager : IInputManager<Mouse.Button, Vector2i, Window, Keyboard.Key>
    {
        public IMouseManager<Mouse.Button, Vector2i, Window> Mouse { get; private set; }
        public IKeyStateManager<Keyboard.Key> Keyboard { get; private set; }

        private readonly Window _window;

        public InputManager(IMouseManager<Mouse.Button, Vector2i, Window> mouse, IKeyStateManager<Keyboard.Key> keyboard, Window window)
        {
            Mouse = mouse;
            Keyboard = keyboard;

            _window = window;
        }

        public void Update()
        {
            Mouse.UpdateMousePosition(_window);
        }

        public void PostUpdate()
        {
            Mouse.PostUpdate();
            Keyboard.PostUpdate();
        }
    }
}
