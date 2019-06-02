﻿using System;
using LibRender;
using OpenTK.Graphics.OpenGL;

namespace OpenBve
{
    internal static partial class Renderer
    {
		/// <summary>The pixel buffer used for rendering the motion blur</summary>
		/// <remarks>Must be static to avoid re-allocating the array memory every frame</remarks>
	    private static byte[] PixelBuffer = null;
		/// <summary>The OpenGL texture index from which the blurred image is rendered</summary>
	    private static int PixelBufferOpenGlTextureIndex = 0;
		/// <summary>Intializes motion blur</summary>
	    internal static void InitializeMotionBlur()
	    {
		    if (Interface.CurrentOptions.MotionBlur == Interface.MotionBlurMode.None)
		    {
			    return;
		    }

		    if (PixelBufferOpenGlTextureIndex != 0)
		    {
			    GL.DeleteTextures(1, new int[] {PixelBufferOpenGlTextureIndex});
			    PixelBufferOpenGlTextureIndex = 0;
		    }
		    PixelBuffer = new byte[4 * Screen.Width * Screen.Height];
		    int[] a = new int[1];
		    GL.GenTextures(1, a);
		    GL.BindTexture(TextureTarget.Texture2D, a[0]);
		    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Linear);
		    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Screen.Width, Screen.Height, 0, PixelFormat.Rgb,
			    PixelType.UnsignedByte, PixelBuffer);
		    PixelBufferOpenGlTextureIndex = a[0];
		    GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, 0, 0, Screen.Width, Screen.Height, 0);
	    }

	    /// <summary>This function renderers full-screen motion blur if selected</summary>
		private static void RenderFullscreenMotionBlur()
        {
	        if (Screen.Minimized)
	        {
		        /*
		         * HACK:
		         * This breaks if minimized, even if we don't reset the W / H values
		         */
		        return;
	        }

	        // render
            if (PixelBufferOpenGlTextureIndex >= 0)
            {
                double strength;
                switch (Interface.CurrentOptions.MotionBlur)
                {
                    case Interface.MotionBlurMode.Low: strength = 0.0025; break;
                    case Interface.MotionBlurMode.Medium: strength = 0.0040; break;
                    case Interface.MotionBlurMode.High: strength = 0.0064; break;
                    default: strength = 0.0040; break;
                }
                double speed = Math.Abs(World.CameraSpeed);
                double denominator = strength * Game.InfoFrameRate * Math.Sqrt(speed);
                float factor;
                if (denominator > 0.001)
                {
                    factor = (float)Math.Exp(-1.0 / denominator);
                }
                else
                {
                    factor = 0.0f;
                }
                // initialize
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                if (!LibRender.Renderer.BlendEnabled)
                {
                    GL.Enable(EnableCap.Blend);
                    LibRender.Renderer.BlendEnabled = true;
                }
                if (LibRender.Renderer.LightingEnabled)
                {
                    GL.Disable(EnableCap.Lighting);
                    LibRender.Renderer.LightingEnabled = false;
                }
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Ortho(0.0, (double)Screen.Width, 0.0, (double)Screen.Height, -1.0, 1.0);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.LoadIdentity();
                if (!LibRender.Renderer.TexturingEnabled)
                {
                    GL.Enable(EnableCap.Texture2D);
                    LibRender.Renderer.TexturingEnabled = true;
                }
                // render
                GL.BindTexture(TextureTarget.Texture2D, PixelBufferOpenGlTextureIndex);
                GL.Color4(1.0f, 1.0f, 1.0f, factor);
                GL.Begin(PrimitiveType.Polygon);
                GL.TexCoord2(0.0, 0.0);
                GL.Vertex2(0.0, 0.0);
                GL.TexCoord2(0.0, 1.0);
                GL.Vertex2(0.0, (double)Screen.Height);
                GL.TexCoord2(1.0, 1.0);
                GL.Vertex2((double)Screen.Width, (double)Screen.Height);
                GL.TexCoord2(1.0, 0.0);
                GL.Vertex2((double)Screen.Width, 0.0);
                GL.End();
                // finalize
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
            }
            // retrieve buffer
            {
                GL.BindTexture(TextureTarget.Texture2D, PixelBufferOpenGlTextureIndex);
                GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb8, 0, 0, Screen.Width, Screen.Height, 0);
            }
        }
    }
}
