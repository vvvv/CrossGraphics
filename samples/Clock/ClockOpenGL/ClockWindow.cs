// Released to the public domain. Use, modify and relicense at will.

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using CrossGraphics.OpenGL;


using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
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
                return null;
            }
        }

        class MyOpenGlTexture : OpenGLTexture
        {
            Canvas _canvas;
            int w;
            int h;

            public MyOpenGlTexture(int width, int height)
                : base(width, height) 
            {
                _canvas = new Canvas() { Width = width, Height = height };
                w = width;
                h = height;
            }

            public override CrossGraphics.IGraphics BeginRendering()
            {

                var c = new XamlGraphics(_canvas);
                c.BeginDrawing();
                c.BeginEntity(this);
                return c;
            }


            protected unsafe override void CallTexImage2D()
            {
                // Save current canvas transform
                Transform transform = _canvas.LayoutTransform;
                // reset current transform (in case it is scaled or rotated)
                _canvas.LayoutTransform = null;

                // Get the size of canvas
                System.Windows.Size size = new System.Windows.Size(_canvas.Width, _canvas.Height);
                // Measure and arrange the surface
                // VERY IMPORTANT
                _canvas.Measure(size);
                _canvas.Arrange(new Rect(size));

                
                // Create a render bitmap and push the surface to it
                RenderTargetBitmap renderBitmap =
                  new RenderTargetBitmap(
                    w,
                    h,
                    96d,
                    96d,
                    PixelFormats.Default);
                renderBitmap.Render(_canvas);
                var b = renderBitmap.Format.BitsPerPixel / 8;

                
                IntPtr data;

                var arr = new byte[w * h * b];
                fixed (byte* p = arr)
                {
                    data = (IntPtr)p;
                    //data = (IntPtr)(*arr);
                    renderBitmap.CopyPixels(new Int32Rect(0, 0, w, h), data, w * h * b, w * b);

                    //Bitmap.LockBits(new global::System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.DontCare);
                    TexImage2D(data);
                    //renderBitmap.UnlockBits(data);                    
                }                           
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