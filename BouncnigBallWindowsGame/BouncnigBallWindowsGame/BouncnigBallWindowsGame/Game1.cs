using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BouncnigBallWindowsGame
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Class

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // sprites
        Texture2D BallSprite;
        Texture2D BarSprite;
        Texture2D GameOverSprite;
        Texture2D Background;

        // some rectangles
        Rectangle BarRectangle, BallRectangle, EndRectangle, EnemyBarRect, MenuText, BackgroundRect;

        // bars positions
        Vector2 EnemyBarPos = new Vector2(100, 100);
        Vector2 BarPosition = new Vector2(300, 400);

        Vector2 velocity = new Vector2(3.0f, 3.0f);

        int score;
        int highscore;
        float acceleration = 0.3f;

        SpriteFont spriteFont;

        MouseState currentMouseState, previousMouseState;
        KeyboardState previousKeyBoardState, currentKeyBoardState;

        Point MousePos;

        
        // menu variables
        Rectangle NewGameRectangle, EndStringRectangle;
        string NewGameString = "New Game";
        string EndString = "Quit";

        // ball centers
        int BallCenter, BarCenter;

        // some bools
        private bool MainMenu = true;

        private bool Play, endGame;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "BallGameContent";

            this.IsMouseVisible = true;
        }


        #endregion

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        #region Initialize

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        #endregion


        /// <summary>
        /// /
        /// </summary>


        #region LoadContent

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // add sprite for the ball
            BallSprite = Content.Load<Texture2D>("spr_cannon_blue");
            BallRectangle = new Rectangle(365, 100, BallSprite.Width, BallSprite.Height);

            // add sprite for bar collision
            BarSprite = Content.Load<Texture2D>("spr_bar");
            BarRectangle = new Rectangle(300, 440, BarSprite.Width, BarSprite.Height);

            // end game sprite
            GameOverSprite = Content.Load<Texture2D>("spr_gm");
            EndRectangle = new Rectangle(200, 50, 400, 100);
            EnemyBarRect = new Rectangle(280, 20, BarSprite.Width, BarSprite.Height);


            //background

            Background = Content.Load<Texture2D>("2dgamebackground");
            BackgroundRect = new Rectangle(0,0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);


            MenuText = new Rectangle(GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width / 2, 100, 100);

            NewGameRectangle = new Rectangle(350, 200, 90, 20);
            EndStringRectangle = new Rectangle(370, 235, 45, 20);

            //TextRectangle = new Rectangle(MenuText.X, MenuText.Y, MenuText.Width, MenuText.Height); 

            spriteFont = Content.Load<SpriteFont>("SpriteFont1");

            // play background music
            //MediaPlayer.Play(Content.Load<Song>("snd_music"));
            //MediaPlayer.IsRepeating = true;

            base.LoadContent();
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>


        #region UnloadContent

        protected override void UnloadContent()
        {
            
        }

        #endregion


        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// 

        #region Update

        protected override void Update(GameTime gameTime)
        {
            previousKeyBoardState = currentKeyBoardState;
            previousMouseState = currentMouseState;

            currentMouseState = Mouse.GetState();
            currentKeyBoardState = Keyboard.GetState();

            MousePos = new Point(currentMouseState.X, currentMouseState.Y);

            do
            {
                MainMenu = false;
                Play = true;

                BallMove();

                MenuNewGame();

                BarMovement();


                if (currentKeyBoardState.IsKeyDown(Keys.Escape) && Play && previousKeyBoardState.IsKeyUp(Keys.Escape))
                {

                    MainMenu = true;

                    Play = false;

                    Pause();

                }
                else if (currentKeyBoardState.IsKeyDown(Keys.Escape) && !Play && MainMenu &&
                         previousKeyBoardState.IsKeyUp(Keys.Escape))
                {
                    MainMenu = false;
                    Play = true;
                }

                // pick a game state
                if (endGame && !Play)
                {
                    if (currentMouseState.LeftButton == ButtonState.Pressed &&
                        previousMouseState.LeftButton == ButtonState.Released)
                    {
                        // game over menu selection
                        if (NewGameRectangle.Contains(MousePos))
                        {
                            // do new game stuff here
                            GameReset();

                        }
                        else if (EndStringRectangle.Contains(MousePos))
                        {
                            //end game
                            MediaPlayer.Play(Content.Load<Song>("gameover"));

                            endGame = false;

                            MainMenu = false;

                            this.Exit();
                        }

                        // esc press main menu selection

                        if (NewGameRectangle.Contains(MousePos))
                        {

                            GameReset();

                        }


                    }
                }

                // Centers
                BarCenter = EnemyBarRect.X + (BarSprite.Width / 2);
                BallCenter = BallRectangle.X + (BallSprite.Width / 2);

                if (currentKeyBoardState.IsKeyDown(Keys.Left) && BarRectangle.X >= 5)
                {
                    BarRectangle.X -= 5;
                }
                else if (currentKeyBoardState.IsKeyDown(Keys.Right) && BarRectangle.X <= 615)
                {
                    BarRectangle.X += 5;
                }
            } while (!endGame && MainMenu);

            base.Update(gameTime);
        }

        #endregion


        /// This is called when the game should draw itself.
        /// 

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(); 
            spriteBatch.Draw(Background, BackgroundRect, Color.White);
            
            // drawing the ball & bar

            spriteBatch.DrawString(spriteFont, "velocity:" + velocity, new Vector2(10, 10), Color.SaddleBrown);

            if (MainMenu && !Play)
            {
                spriteBatch.DrawString(spriteFont, "P A U S E D", new Vector2(333, 150), Color.SaddleBrown);

                //new game
                spriteBatch.DrawString(spriteFont, NewGameString, new Vector2(350, 200), Color.SaddleBrown);

                //quit
                spriteBatch.DrawString(spriteFont, EndString, new Vector2(370, 230), Color.SaddleBrown);
            }

            // game over
            if (endGame)
            {
                var tempscore = 0;

                tempscore = score;

                if (tempscore > score)
                {
                    highscore = tempscore;
                }

                // new game
                spriteBatch.DrawString(spriteFont, NewGameString, new Vector2(350, 200), Color.SaddleBrown);

                //exit
                spriteBatch.DrawString(spriteFont, EndString, new Vector2(370, 230), Color.SaddleBrown);

                // high score
                spriteBatch.DrawString(spriteFont, "HighScore: " + score, new Vector2(330, 160), Color.SaddleBrown);

                spriteBatch.Draw(GameOverSprite, EndRectangle, Color.White);



                Play = false;


                //MediaPlayer.Play(Content.Load<Song>("gameover"));
                //MediaPlayer.IsRepeating = true;
            }
            else if (!endGame)
            {
                spriteBatch.Draw(BallSprite, BallRectangle, Color.White);
                spriteBatch.Draw(BarSprite, BarRectangle, Color.White);
                spriteBatch.Draw(BarSprite, EnemyBarRect, Color.White);

                spriteBatch.DrawString(spriteFont, "Score: " + score, new Vector2(700, 10), Color.SaddleBrown);
            }

            spriteBatch.End();
            

            base.Draw(gameTime);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>

        #region Methods

        public void BallMove()
        {

            BallRectangle.Y += (int)velocity.Y;
            BallRectangle.X += (int)velocity.X;

        }
        

        public void BarMovement()
        {
            // enemy bar AI
            if (BallCenter >= BarCenter)
            {
                // if 
                EnemyBarRect.X += (int)velocity.X;
            }
            if (BallCenter <= BarCenter)
            {
                EnemyBarRect.X -= (int)velocity.X;
            }

            // enemy bar
            if (BallRectangle.Intersects(EnemyBarRect))
            {
                    //MediaPlayer.Play(Content.Load<Song>("snd_collect_points"));

                velocity.Y -= acceleration;
                velocity.Y = -velocity.Y;
            }

            // controled bar

            if (BallRectangle.Intersects(BarRectangle))
            {
               // MediaPlayer.Play(Content.Load<Song>("snd_collect_points"));

                //velocity.Y -= acceleration;

                velocity.Y = -velocity.Y;

                score++;

                

            }
        }

        public void MenuNewGame()
        {
            
            // start everything

            if (Play)
            {
                if (BallRectangle.X <= 0)
                {
                    velocity.X = -velocity.X;
                }

                if (BallRectangle.X + BallSprite.Width >= GraphicsDevice.Viewport.Width)
                {
                    velocity.X = -velocity.X;
                }
                if (BallRectangle.Y <= 0)
                {
                    velocity.Y = GraphicsDevice.Viewport.Y;
                    velocity.X = GraphicsDevice.Viewport.X;

                    endGame = true;
                    MainMenu = true;
                    Play = false;
                }

                if (BallRectangle.Y + BallSprite.Height >= GraphicsDevice.Viewport.Height)
                {
                    velocity.Y = GraphicsDevice.Viewport.Y;
                    velocity.X = GraphicsDevice.Viewport.X;

                    endGame = true;
                }
            }
            else
            {
                Pause();
            }


            Play = true;

        }

        public void Pause()
        {
            Vector2 BallVector = new Vector2();
            Vector2 BarVector = new Vector2();
            // freeze ball at current position

            BallVector.X = BallRectangle.X;
            BallVector.Y = BallRectangle.Y;

            //get the current position store it in other var and use it as a current position

            BallRectangle.X = (int)BallVector.X;
            BallRectangle.Y = (int)BallVector.Y;

            
            // dont let bars move
            BarVector.X = BarRectangle.X;
            BarVector.Y = BarRectangle.Y;

            BarRectangle.X = (int)BarVector.X;
            BarRectangle.Y = (int)BarVector.Y;

            // opens menu

            MainMenu = true;
            Play = false;
        }

        public void GameReset()
        {

            MediaPlayer.Play(Content.Load<Song>("snd_collect_points"));


            // reset ball
            BallRectangle.X = 365;
            BallRectangle.Y = 100;

            // reset bar position
            BarRectangle.X = 300;
            BarRectangle.Y = 440;

            // reset enemy bar
            EnemyBarRect.X = 280;
            EnemyBarRect.Y = 20;

            endGame = false;
            Play = true;
            MainMenu = false;

            score = 0;
        }


        /* TODO
         put each sprite in a new Class, in Obects folder, for better project management.
         
         fix new game loop      
         
         try catch trow Sytem.IO Exeption e

         add levels*/

        #endregion
    }
}