//
// Copyright (c) 2010 Frank A. Krueger
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrossGraphics
{
	public interface IGraphics
	{
		void BeginEntity(object entity);

		void SetFont(Font f);

		void SetColor(Color c);

		void FillPolygon(Polygon poly);

		void DrawPolygon(Polygon poly,float w);

		void FillRect(float x,float y,float width, float height);

		void DrawRect(float x, float y, float width, float height, float w);

		void FillRoundedRect(float x, float y, float width, float height, float radius);

		void DrawRoundedRect(float x, float y, float width, float height, float radius, float w);

		void FillOval(float x, float y, float width, float height);

		void DrawOval(float x, float y, float width, float height, float w);

		void BeginLines();

		void DrawLine(float sx, float sy, float ex, float ey, float w);

		void EndLines();

		void DrawImage(IImage img, float x, float y, float width, float height);

		void DrawString(string s, float x, float y);

		IFontMetrics GetFontMetrics();

		IImage ImageFromFile(string path);
	}

	public static class GraphicsEx
	{
		public static void DrawLine(this IGraphics g, PointF s, PointF e, float w)
		{
			g.DrawLine (s.X, s.Y, e.X, e.Y, w);
		}

		public static void FillRoundedRect(this IGraphics g, RectangleF r, float radius)
		{
			g.FillRoundedRect (r.Left, r.Top, r.Width, r.Height, radius);
		}

		public static void FillRoundedRect(this IGraphics g, Rectangle r, float radius)
		{
			g.FillRoundedRect (r.Left, r.Top, r.Width, r.Height, radius);
		}

		public static void FillRect(this IGraphics g, RectangleF r)
		{
			g.FillRect (r.Left, r.Top, r.Width, r.Height);
		}
	}

	public interface IImage
	{
	}

	[Flags]
	public enum FontOptions
	{
		None = 0,
		Bold = 1
	}

	public class Font
	{
		public string FontFamily { get; private set; }

		public FontOptions Options { get; private set; }

		public int Size { get; private set; }

		public object Tag { get; set; }

		public Font (string fontFamily, FontOptions options, int size)
		{
			FontFamily = fontFamily;
			Options = options;
			Size = size;
		}

		public override string ToString()
		{
			return string.Format ("[Font: FontFamily={0}, Options={1}, Size={2}, Tag={3}]", FontFamily, Options, Size, Tag);
		}
	}

	public interface IFontMetrics
	{
		int StringWidth(string s);

		int Height { get; }

		int Ascent { get; }

		int Descent { get; }
	}

	public class Color
	{
		public readonly int Red, Green, Blue, Alpha;
		public object Tag;

		public float RedValue {
			get { return Red / 255.0f; }
		}

		public float GreenValue {
			get { return Green / 255.0f; }
		}

		public float BlueValue {
			get { return Blue / 255.0f; }
		}

		public float AlphaValue {
			get { return Alpha / 255.0f; }
		}

		public Color (int red, int green, int blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = 255;
		}

		public Color (int red, int green, int blue, int alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		public Color GetInvertedColor()
		{
			return new Color (255 - Red, 255 - Green, 255 - Blue, Alpha);
		}

		public static bool AreEqual(Color a, Color b)
		{
			if (a == null && b == null)
				return true;
			if (a == null && b != null)
				return false;
			if (a != null && b == null)
				return false;
			return (a.Red == b.Red && a.Green == b.Green && a.Blue == b.Blue && a.Alpha == b.Alpha);
		}

		public bool IsWhite {
			get { return (Red == 255) && (Green == 255) && (Blue == 255); }
		}

		public bool IsBlack {
			get { return (Red == 0) && (Green == 0) && (Blue == 0); }
		}

		public Color WithAlpha(int aa)
		{
			return new Color (Red, Green, Blue, aa);
		}

		public override string ToString()
		{
			return string.Format ("[Color: RedValue={0}, GreenValue={1}, BlueValue={2}, AlphaValue={3}]", RedValue, GreenValue, BlueValue, AlphaValue);
		}
	}

	public static class Colors
	{
		public static readonly Color Yellow = new Color (255, 255, 0);
		public static readonly Color Red = new Color (255, 0, 0);
		public static readonly Color Green = new Color (0, 255, 0);
		public static readonly Color Blue = new Color (0, 0, 255);
		public static readonly Color White = new Color (255, 255, 255);
		public static readonly Color Cyan = new Color (0, 255, 255);
		public static readonly Color Black = new Color (0, 0, 0);
		public static readonly Color LightGray = new Color (212, 212, 212);
		public static readonly Color Gray = new Color (127, 127, 127);
		public static readonly Color DarkGray = new Color (64, 64, 64);
	}

	public class Polygon
	{
		public readonly List<PointF> Points;

		public object Tag { get; set; }

		public int Version { get; set; }

		public Polygon ()
		{
			Points = new List<PointF> ();
		}

		public Polygon (int[] xs, int[] ys, int c)
		{
			Points = new List<PointF> (c);
			for (var i = 0; i < c; i++) {
				Points.Add (new PointF (xs [i], ys [i]));
			}
		}

		public int Count {
			get { return Points.Count; }
		}

		public void Clear()
		{
			Points.Clear ();
			Version++;
		}

		public void AddPoint(PointF p)
		{
			Points.Add (p);
			Version++;
		}

		public void AddPoint(float x, float y)
		{
			Points.Add (new PointF (x, y));
			Version++;
		}
	}
}

