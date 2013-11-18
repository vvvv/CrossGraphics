// Released to the public domain. Use, modify and relicense at will.

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using CrossGraphics.OpenGL;

namespace CrossGraphics.OpenGL
{
    class ClockWindow : GameWindow
    {
        Clock.Clock _clock;
        static OpenGLGraphics _graphics;

        public ClockWindow()
            : base(600, 600, GraphicsMode.Default, "OpenTK Quick Start Sample", 0, DisplayDevice.Default, 2, 0, GraphicsContextFlags.Default)
        {
            VSync = VSyncMode.On;
            _clock = new Clock.Clock();
            _graphics = new OpenGLGraphics(new MyShapeStore(), new MyOpenGLRenderProvider());
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _graphics.BeginDrawing();
            {
                _clock.Width = Width;
                _clock.Height = Height;
                _clock.Draw(_graphics);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                Matrix4 modelview = Matrix4.Scale(new Vector3(2f / Width, -2f / Height, 0)) * Matrix4.CreateTranslation(new Vector3(-1f, 1f, 0));
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref modelview);
            }
            _graphics.EndDrawing();

            SwapBuffers();
        }

        [STAThread]
        static void Main()
        {
            using (ClockWindow clockWindow = new ClockWindow())
            {
                clockWindow.Run(30.0);
            }
        }

        class MyOpenGLRenderProvider : IOpenGLRenderProvider
        {
            public CrossGraphics.IFontMetrics GetFontMetrics(CrossGraphics.Font font)
            {
                throw new NotImplementedException();
            }

            public CrossGraphics.IImage ImageFromFile(string path)
            {
                throw new NotImplementedException();
            }
        }

        class MyOpenGlTexture : OpenGLTexture
        {
            public MyOpenGlTexture(int width, int height)
                : base(width, height) { }

            public override CrossGraphics.IGraphics BeginRendering()
            {
                return ClockWindow._graphics;
            }

            protected override void CallTexImage2D()
            {
                //throw new NotImplementedException();
            }
        }

        class MyShapeStore : OpenGLShapeStore
        {
            protected override OpenGLTexture CreateTexture(int width, int height)
            {
                return new MyOpenGlTexture(width, height);
            }
        }

    }
}