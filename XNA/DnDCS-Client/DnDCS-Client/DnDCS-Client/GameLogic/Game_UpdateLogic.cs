﻿using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using DnDCS.Libs;

namespace DnDCS_Client.GameLogic
{
    public partial class Game
    {
        private int lastWheelValue;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!this.gameState.IsConnected)
                return;

            gameState.Update();

            if (gameState.CreateEffect)
            {
                gameState.CreateEffect = false;

                if (this.effect != null)
                    this.effect.Dispose();

                var aspect = (float)Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
                effect = new BasicEffect(GraphicsDevice)
                {
                    World = Matrix.Identity,
                    View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),
                    Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 1, 100),
                    VertexColorEnabled = true
                };
            }

            // TODO: This stinks, because we need to check the condition every single update... Have to do this until I figure out how to post to the Game thread.
            if (gameState.UpdateTitle)
            {
                gameState.UpdateTitle = false;
                this.Window.Title = string.Format("DnDCS Client - Connected to {0}:{1}", gameState.Connection.Address, gameState.Connection.Port);
            }

            // TODO: This is the biggest piece of garbage I've written for this entire thing. Currently disabled because I can't stand it.
            if (gameState.ConsumeFogUpdates)
            {
                gameState.ConsumeFogUpdates = false;
                FogUpdate[] newFogUpdates;
                lock (fogUpdatesLock)
                {
                    newFogUpdates = this.fogUpdates.ToArray();
                    this.fogUpdates.Clear();
                }

                using (var g = System.Drawing.Graphics.FromImage(gameState.FogImage))
                {
                    // Draw all Fog Updates into the Bitmap.
                    foreach (var newFogUpdate in newFogUpdates)
                    {
                        g.FillPolygon((newFogUpdate.IsClearing) ? System.Drawing.Brushes.White : System.Drawing.Brushes.Black, newFogUpdate.Points.Select(p => new System.Drawing.Point(p.X, p.Y)).ToArray());
                    }

                    // Push the Bitmap into a Texture2D instance
                    using (var ms = new System.IO.MemoryStream())
                    {
                        gameState.FogImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        var newFogTexture = Texture2D.FromStream(GraphicsDevice, ms);

                        // TODO: The Bitmap uses White to simulate Transparency. This is stupid but acceptable for now.
                        ReplaceNonBlackWithTransparent(newFogTexture);

                        // Finally push the fog into the next Game State.
                        gameState.Fog = newFogTexture;
                    }
                }
            }

            if (gameState.CurrentMouseState.ScrollWheelValue != lastWheelValue)
            {
                Update_HandleScroll();
                Update_HandleZoom();
            }

            lastWheelValue = gameState.CurrentMouseState.ScrollWheelValue;

            gameState.DebugText.Add("Zoom Factor: " + gameState.ZoomFactor);
            gameState.DebugText.Add("Vertical Scroll Position: " + gameState.VerticalScrollPosition);
            gameState.DebugText.Add("Horizontal Scroll Position: " + gameState.HorizontalScrollPosition);
            if (gameState.Map != null)
            {
                gameState.DebugText.Add("Map Size: " + gameState.Map.Width + "x" + gameState.Map.Height);
                gameState.DebugText.Add("Map Bounds: " + gameState.ActualMapWidth + "x" + gameState.ActualMapHeight);
                gameState.DebugText.Add("Logical Map Bounds: " + gameState.LogicalMapWidth + "x" + gameState.LogicalMapHeight);
            }
            gameState.DebugText.Add("Client Bounds: " + gameState.ActualClientWidth + "x" + gameState.ActualClientHeight);
            gameState.DebugText.Add("Logical Client Bounds: " + gameState.LogicalClientWidth + "x" + gameState.LogicalClientHeight);
            base.Update(gameTime);
        }

        /// <summary> Replaces all non-black colors with a Transparent color. This should only be used in the context of Fogs. </summary>
        private void ReplaceNonBlackWithTransparent(Texture2D texture)
        {
            // Replace the old fog with the newly merged fog texture
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colors);
            for (var i = 0; i < colors.Length; i++)
            {
                if (!colors[i].Equals(Color.Black))
                    colors[i] = Color.Transparent;
            }
            texture.SetData<Color>(colors);
        }

        private void Update_HandleScroll()
        {
            // TODO: Add support for scrolling off screen, so we don't know when the map actually ends. Cap it at Window.Width/Height offscreen though - no reason to know exactly where it ends.
            // Control forces a Zoom, so overrides all Scrolling.
            if (this.gameState.Map != null && !gameState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                Update_HandleVerticalScroll();
                Update_HandleHorizontalScroll();
            }
        }

        private void Update_HandleVerticalScroll()
        {
            var wheelDelta = (gameState.CurrentMouseState.ScrollWheelValue - lastWheelValue);
            if (!gameState.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (wheelDelta > 0)
                {
                    // Up
                    gameState.VerticalScrollPosition = Math.Max(0, gameState.VerticalScrollPosition - (int)Math.Abs(gameState.ActualMapHeight * GameConstants.ScrollDeltaPercent));
                }
                else if (wheelDelta < 0)
                {
                    // Down
                    gameState.VerticalScrollPosition = Math.Min(gameState.VerticalScrollPosition + (int)Math.Abs(gameState.ActualMapHeight * GameConstants.ScrollDeltaPercent), gameState.LogicalMapHeight - gameState.ActualClientHeight);
                }
            }

        }

        private void Update_HandleHorizontalScroll()
        {
            var wheelDelta = (gameState.CurrentMouseState.ScrollWheelValue - lastWheelValue);
            if (gameState.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (wheelDelta > 0)
                {
                    // Left
                    gameState.HorizontalScrollPosition = Math.Max(0, gameState.HorizontalScrollPosition - (int)Math.Abs(gameState.ActualMapWidth * GameConstants.ScrollDeltaPercent));
                }
                else if (wheelDelta < 0)
                {
                    // Right
                    gameState.HorizontalScrollPosition = Math.Min(gameState.HorizontalScrollPosition + (int)Math.Abs(gameState.ActualMapWidth * GameConstants.ScrollDeltaPercent), gameState.LogicalMapWidth - gameState.ActualClientWidth);
                }
            }

        }

        private void Update_HandleZoom()
        {
            if (gameState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                var wheelDelta = (gameState.CurrentMouseState.ScrollWheelValue - lastWheelValue);

                if (wheelDelta > 0)
                {
                    // In
                    gameState.ZoomFactor = Math.Min((float)Math.Round(gameState.ZoomFactor + GameConstants.ZoomFactorDelta, 1), GameConstants.ZoomMaximumFactor);
                }
                else if (wheelDelta < 0)
                {
                    // Out
                    gameState.ZoomFactor = Math.Max((float)Math.Round(gameState.ZoomFactor - GameConstants.ZoomFactorDelta, 1), GameConstants.ZoomMinimumFactor);
                }
                else
                {
                    return;
                }

                // After any zoom, we need to re-bound the Scroll Positions so we're not over-showing the map.
                gameState.HorizontalScrollPosition = Math.Max(0, Math.Min(gameState.HorizontalScrollPosition, gameState.LogicalMapWidth - gameState.ActualClientWidth));
                gameState.VerticalScrollPosition = Math.Max(0, Math.Min(gameState.VerticalScrollPosition, gameState.LogicalMapHeight - gameState.ActualClientHeight));
            }
        }
    }
}