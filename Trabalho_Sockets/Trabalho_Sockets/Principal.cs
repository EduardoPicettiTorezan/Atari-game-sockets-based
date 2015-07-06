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

namespace Trabalho_Sockets
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Principal : Microsoft.Xna.Framework.Game
    {
        public const int LARGURA_TELA = 800;
        public const int ALTURA_TELA = 600;
        public const int gciLimiteLargura = LARGURA_TELA + 20;
        public const int gciLimiteAltura = ALTURA_TELA + 20;

        GraphicsDeviceManager graphics;
        //possibilita desenhar imagens 2d 
        SpriteBatch spriteBatch;
        
        jogador[] ljogadores = new jogador[1];

        //Teclado
        KeyboardState teclado_estado;

        public Principal()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = LARGURA_TELA;
            graphics.PreferredBackBufferHeight = ALTURA_TELA;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here                 
            
            jogador.Iniciar(ref ljogadores, Program.sProgramIpDoServidor);

            base.Initialize();
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //carga das texturas
            Viewport viewport = graphics.GraphicsDevice.Viewport;

            jogador.jogadorLoadContent(ref ljogadores, viewport, Content);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            /*Durante o jogo, testar aqui as ações do usuário e o que ocorreno jogo.*/

            //Captura ação no teclado.
            teclado_estado = Keyboard.GetState();

            jogador.jogadorUpdate(ref ljogadores, teclado_estado);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            
            //this.Window.Title = "Score : " + playerScore.ToString();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of 
        /// timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            jogador.jogadorDraw(ref ljogadores, spriteBatch);           
               
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
