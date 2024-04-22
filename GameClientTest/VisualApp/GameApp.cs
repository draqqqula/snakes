using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VisualApp
{
    public class GameApp : Game
    {
        public ConcurrentQueue<Keys> PressedKeys { get; } = [];
        public FrameDisplayForm[] DisplayBuffer { get; set; } = [];

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameApp()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && !PressedKeys.Contains(Keys.Left))
            {
                PressedKeys.Enqueue(Keys.Left);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && !PressedKeys.Contains(Keys.Right))
            {
                PressedKeys.Enqueue(Keys.Right);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !PressedKeys.Contains(Keys.Down))
            {
                PressedKeys.Enqueue(Keys.Down);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !PressedKeys.Contains(Keys.Up))
            {
                PressedKeys.Enqueue(Keys.Up);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            var buffer = DisplayBuffer.ToArray();
            foreach (var frame in buffer)
            {
                var texture = Content.Load<Texture2D>(frame.Name);
                _spriteBatch.Draw(texture, frame.Position + new Vector2(Window.ClientBounds.Width/2, Window.ClientBounds.Height/2), null, Color.White, frame.Rotation, texture.Bounds.Center.ToVector2(), frame.Scale, SpriteEffects.None, 0);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
