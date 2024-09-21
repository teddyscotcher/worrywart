using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.CodeDom;

namespace WorrywartNEA2
{
    public class MainGame : Game
    {
        //Constants I need (accessed from other classes).
        public const int UI_UNIT = 32;

        public const int GRID_ROWS = 48;
        public const int GRID_COLUMNS = 64;
        public const int CELL_WIDTH = 16;

        public const int WINDOW_WIDTH = 1536;
        public const int WINDOW_HEIGHT = 960;


        //All the important objects
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private WorldDrawer _worldDrawer;
        private UIManager _uiManager;
        private InputManager _inputManager;
        private CellManager _cellManager;

        //Font for UI text
        private SpriteFont _font;

        //Tutorial text
        private readonly string instructionString =
            "Welcome to Worrywart!\n\n" +
            "What's up??:\n" +
            "-  In this game there are Students, who just want to get on\n" +
            "   with studying, and the frantic Worrywarts, who can't help but spread\n   stress to those around them.\n" +
            "-  Libraries will slowly reduce the stress of anyone nearby, and \n   nobody can go through walls.\n" +
            "Controls:\n" +
            "-  Menu in the top right is used to select the Cell you'd\n   like to place down (Empty = delete).\n" +
            "-  Right click on a cell to show information about the cell, \n   including object-oriented programming (OOP) information.\n" +
            "-  Controls for time speed are on the left. So is the option to quit.";
        


        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            //Set window size
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Initiate world drawer, cell manager, UI manager, and input manager.
            _worldDrawer = new WorldDrawer(GraphicsDevice);

            _inputManager = new InputManager();
            _uiManager = new UIManager(GraphicsDevice, _inputManager, _font);
            _cellManager = new CellManager(_worldDrawer, _inputManager, _uiManager) ;
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Loading content and things. Boring MonoGame malarkey.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("MainFont");
            _uiManager.font = _font;

            //Create the tutorial text and add it to UI manager.
            Text instructionText = new Text(new Vector2(MainGame.GRID_COLUMNS * MainGame.CELL_WIDTH, UI_UNIT * 15), new Vector2(MainGame.WINDOW_WIDTH - MainGame.CELL_WIDTH*MainGame.GRID_COLUMNS, UI_UNIT * 10), Color.Black, instructionString);
            _uiManager.Add(instructionText);

            //Quit button!
            Button quitButton = new Button(new Vector2(0, WINDOW_HEIGHT - UI_UNIT), new Vector2(UI_UNIT*2, UI_UNIT), Color.Red, "Quit", System.Environment.Exit, new object?[] { 0 });
            _uiManager.Add(quitButton);


            //Draw the empty grid to the screen as the very first thing
            _worldDrawer.DrawEmptyGrid(_spriteBatch);
        }


        protected override void Update(GameTime gameTime)
        {
            //So we can quit.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Update my thingamabobs.
            _inputManager.Update();
            _uiManager.Update();
            _cellManager.Update(gameTime);
            base.Update(gameTime);
        }


        
        protected override void Draw(GameTime gameTime)
        {
            //Draw my thingamabobs.
            _uiManager.Draw(_spriteBatch);
            _worldDrawer.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
    }
}