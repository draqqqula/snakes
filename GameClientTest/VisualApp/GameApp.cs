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
        public SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        public MobileJoystick Joystick { get; set; } = new MobileJoystick();
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
            Semaphore.Wait();
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!Joystick.Active)
                {
                    Joystick.ScreenPosition = mouseState.Position.ToVector2();
                }
                Joystick.Active = true;
                var delta = mouseState.Position.ToVector2() - Joystick.ScreenPosition;
                var length = MathF.Min(delta.Length(), 65f);
                var directionVector = Vector2.Normalize(delta);

                var angle = MathF.Atan2(directionVector.Y, directionVector.X);
                Joystick.Direction = angle;

                Joystick.DotPosition = Joystick.ScreenPosition + directionVector * length;
            }
            else
            {
                Joystick.Active = false;
            }
            Semaphore.Release();

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

            if (Joystick.Active)
            {
                var joystickBase = Content.Load<Texture2D>("joystick_base");
                _spriteBatch.Draw(joystickBase, Joystick.ScreenPosition, null, Color.White, 0f, joystickBase.Bounds.Center.ToVector2(), 1f, SpriteEffects.None, 0);
                var joystickDot = Content.Load<Texture2D>("joystick");
                _spriteBatch.Draw(joystickDot, Joystick.DotPosition, null, Color.White, 0f, joystickDot.Bounds.Center.ToVector2(), 1f, SpriteEffects.None, 0);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
