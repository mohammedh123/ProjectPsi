using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace ProjectPsi.GUI
{
    public class MainGame : IDisposable
    {
        public void Run(int framesPerSecond)
        {
            using (var game = new GameWindow()) {
                game.Load += (o, args) => OnLoad(game);
                game.Resize += (sender, e) => OnResize(game);
                game.UpdateFrame += (sender, e) => OnUpdateFrame(game);
                game.RenderFrame += (sender, e) => OnRenderFrame(game);

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }

        private void OnLoad(GameWindow game)
        {
            // setup settings, load textures, sounds
            game.VSync = VSyncMode.On;
        }

        private void OnResize(GameWindow game)
        {
            GL.Viewport(0, 0, game.Width, game.Height);
        }

        private void OnUpdateFrame(GameWindow game)
        {
            // add game logic, input handling
            if (game.Keyboard[Key.Escape])
            {
                game.Exit();
            }
        }

        private void OnRenderFrame(GameWindow game)
        {
            // render graphics
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

            GL.Begin(PrimitiveType.Triangles);

            GL.Color3(Color.MidnightBlue);
            GL.Vertex2(-1.0f, 1.0f);
            GL.Color3(Color.SpringGreen);
            GL.Vertex2(0.0f, -1.0f);
            GL.Color3(Color.Ivory);
            GL.Vertex2(1.0f, 1.0f);

            GL.End();

            game.SwapBuffers();
        }
        
        public void Dispose()
        {
        }
    }
}
