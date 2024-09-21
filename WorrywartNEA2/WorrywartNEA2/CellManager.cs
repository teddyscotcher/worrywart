using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static WorrywartNEA2.Cell;

namespace WorrywartNEA2
{
    //Class to manage cell spawning, deleting, and updating. This includes taking mouse input on the game grid.
    internal class CellManager
    {
        private const float WORRYWART_STRESS_THRESHOLD = 1;

        private int currentId = 0;
        private List<Cell> cells;
        private WorldDrawer _worldDrawer;
        private UIManager _uiManager;
        private CellType currentCellType;

        private InputManager _inputManager;

        private double updateTimer = 0;
        //Time between cell updates in milliseconds.
        private double timeBetweenUpdates = 500;


        private Cell highlightedCell;
        private Text infoText;

        public CellManager(WorldDrawer _worldDrawer, InputManager _inputManager, UIManager _uiManager)
        {
            this._worldDrawer = _worldDrawer;
            cells = new List<Cell>();
            this._inputManager = _inputManager;
            this._uiManager = _uiManager;
            currentCellType = CellType.Student;


            

            //List containing all CellType values
            var types = Enum.GetValues(typeof(CellType)).Cast<CellType>().ToList();


            //Split the space to the right of the game grid between the buttons by calculating the space and dividing by number of CellType values.

            float gridHeight = MainGame.CELL_WIDTH * MainGame.GRID_ROWS;
            float gridWidth = MainGame.CELL_WIDTH * MainGame.GRID_COLUMNS;

            float widthExcludingGrid = MainGame.WINDOW_WIDTH - gridWidth;


            infoText = new Text(new Vector2(gridWidth, MainGame.UI_UNIT * 5), new Vector2(widthExcludingGrid, MainGame.UI_UNIT * 8), Color.Black, "Cell Info");

            this._uiManager.Add(infoText);
            

            float buttonWidth = widthExcludingGrid / types.Count();

            Text typeChooseText = new Text(new Vector2(gridWidth, 5), new Vector2(widthExcludingGrid, MainGame.UI_UNIT), Color.Black, "Choose a student type:");
            this._uiManager.Add(typeChooseText);

            //For each of these CellType values, create a button and add it to the UI Manager
            for (int i = 0; i < types.Count(); i++)
            {
                //Create a button and add it to the UI manager

                //The colour of the button is the colour that WorldDrawer draws this type as (comes from the lookup dictionary in WorldDrawer).
                Color colour = _worldDrawer.typeColourDictionary[types[i]];
                string text = types[i].ToString();

                Button b = new Button(new Vector2(gridWidth + buttonWidth*i, MainGame.UI_UNIT), new Vector2(buttonWidth-1, MainGame.UI_UNIT - 1),
                    colour, text, ChangeCurrentCellType, new object?[] { types[i] });
                this._uiManager.Add(b);
            }
            Button clearButton = new Button(new Vector2(gridWidth, MainGame.UI_UNIT * 2), new Vector2(widthExcludingGrid - 1, MainGame.UI_UNIT - 1),
                Color.Gainsboro, "Clear", ClearGrid, new object?[] { });
            this._uiManager.Add(clearButton);

            //Time speed control 

            //Width of all these buttons
            float speedButtonWidth = MainGame.UI_UNIT * 3;
            //Text to show what the buttons are for.
            Text speedText = new Text(new Vector2(0, gridHeight), new Vector2(MainGame.UI_UNIT * 3, MainGame.UI_UNIT), Color.Black, "Time Speed");
            this._uiManager.Add(speedText);

            //Create 6 options (0 - 100%)
            for (int i = 0; i <= 5; i++)
            {
                //Create time option in milliseconds (less % of original time means more time between frames, hence 6-i.
                double newTime = (6-i) * 100;
                //For the 0% button the time between gaps is 0, which there is a check for in Update
                if (i == 0) newTime = 0;
                //Create a gray button that is position on x according to i, with its % of original time as the text,
                //and it triggers the SetUpdateTime function with newTime as the argument.
                Button b = new Button(new Vector2( (i+1) * speedButtonWidth, gridHeight), new Vector2(speedButtonWidth - 1, MainGame.UI_UNIT), 
                    Color.Gray, (i*20).ToString() + "%", SetUpdateTime, new object?[] { newTime });
                //Add this button to the UIManager so it can be drawn and updated.
                this._uiManager.Add(b);
            }
            
        }

        public void Update(GameTime gameTime)
        {
        //GAME GRID INPUT -------

            //Take in mouse position and convert it to a grid position.
            Vector2 gridPos = ScreenToGridPos(_inputManager.MousePos());

            //If there's a mouse press and the mouse is on top of the grid, spawn a cell of the current cell type at the current position.
            if(_inputManager.IsLeftMousePressed() && gridPos.X < MainGame.GRID_COLUMNS && gridPos.Y < MainGame.GRID_ROWS)
            {
                if (currentCellType == CellType.Empty)
                {
                    DeleteCellAtPos(gridPos);
                }
                else
                {
                    SpawnCell(gridPos, currentCellType);
                }
            }
            // ----------------------

            //If we have a right click
            if(_inputManager.IsRightMousePressed())
            {
                //Run through each cell to see if there's a cell at the grid position that's been clicked on.
                foreach(Cell c in cells)
                {
                    if(c.Position == gridPos)
                    {
                        //If there already is a highlighted cell, we want to signal to WorldDrawer to
                        //keep it in the same spot but dehighlight it (hence highlighed=false in the arguments).
                        if(highlightedCell != null) _worldDrawer.SignalCellMove(highlightedCell.Position, 
                            highlightedCell.Position, highlightedCell.Type, highlightedCell.Type, false);

                        //Highlighted cell is now the clicked cell.
                        highlightedCell = c;
                        //Do the opposite thing we just did to the highlighted cell to this one, so now it is a highlighted colour.
                        _worldDrawer.SignalCellMove(c.Position, c.Position, c.Type, c.Type, true);
                        
                    }
                }
            }
            //Generate the text for the highlighted cell (if there is one).
            if(highlightedCell != null) infoText.text = GenerateInfoString(highlightedCell);

            //UPDATING CELLS --------

            //If it's due time for an update
            if (updateTimer >= timeBetweenUpdates && timeBetweenUpdates != 0)
            {
                foreach (Cell c in cells.ToList())
                {
                    Vector2 oldPos = c.Position;

                    c.Update(cells);

                    //If we have a Student or Nerd, parse the Cell to a Character (it must be a Character if it has this type).
                    if(c.Type == CellType.Student)
                    {
                        Character ch = (Character)c;
                        //If we're over the threshold stressLevel, swap out for a Worrywart and give it the old Student's ID.
                        if(ch.StressLevel > WORRYWART_STRESS_THRESHOLD)
                        {
                            Character replacementCharacter = (Character)SpawnCell(ch.Position, CellType.Worrywart);
                            replacementCharacter.Id = ch.Id;
                            replacementCharacter.Name = ch.Name;
                            if (ch == highlightedCell) highlightedCell = replacementCharacter;
                        }
                    }
                    //Same thing but in reverse for Worrywarts -> Student
                    if(c.Type == CellType.Worrywart)
                    {
                        Character ch = (Character)c;
                        if (ch.StressLevel < WORRYWART_STRESS_THRESHOLD)
                        {
                            Character replacementCharacter = (Character)SpawnCell(ch.Position, CellType.Student);
                            replacementCharacter.Id = ch.Id;
                            replacementCharacter.Name = ch.Name;
                            if (ch == highlightedCell) highlightedCell = replacementCharacter;
                        }
                    }

                    //If the Cell has moved, then we'll signal a Cell movement to WorldDrawer, making sure to flag if its the highlighted cell.
                    if(c.Position != oldPos)
                    {
                        bool highlighted = c == highlightedCell;
                        _worldDrawer.SignalCellMove(oldPos, c.Position, CellType.Empty, c.Type, highlighted);
                    }
                    

                }
                updateTimer = 0;
            }
            //Increment update timer by the time since last frame.
            updateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        //-----------------------
        }

        //Does what it says on the tin.
        public Vector2 ScreenToGridPos(Vector2 screenPos)
        {
            int x = (int)Math.Floor(screenPos.X / MainGame.CELL_WIDTH);
            int y = (int)Math.Floor(screenPos.Y / MainGame.CELL_WIDTH);
            return new Vector2(x, y);
        }

        //Spawning cells
        public Cell SpawnCell(Vector2 spawnPos, CellType cellType)
        {
            //Create a blank Cell that we can assign a subclass and a BUNCH of values to depending on cellType.
            Cell cell = new Cell();
            switch(cellType)
            {
                case CellType.Student:
                    //Character(id, position, type, stressLevel, goTowards, runAway, neighbourhoodRadius, stopDistance, speed, runAwayRadius)
                    cell = new Character(currentId, spawnPos, cellType, 0.2f, new List<CellType> { CellType.Library, CellType.Student}, 
                        new List<CellType> { CellType.Worrywart }, 7.5f, 3 , 2f, 3);
                    break;
                case CellType.Worrywart:
                    //Worrywart(id, position, type, stressLevel, goTowards, runAway, neighbourhoodRadius, stopDistance, speed, runAwayRadius, stressIncrease, attackRadius)
                    cell = new Worrywart(currentId, spawnPos, cellType, 2f, new List<CellType> { CellType.Student }, 
                        new List<CellType> { }, 5, 1 , 0.75f, 5, 0.05f, 2f);  
                    break;
                case CellType.Wall:
                    //Cell(id, position, type)
                    cell = new Cell(currentId, spawnPos, CellType.Wall);
                    break;
                case CellType.Library:
                    //Library(id, position, type, neighbourhoodRadius, effectStrength
                    cell = new Library(currentId, spawnPos, CellType.Library, 3f, 0.075f);
                    break;
                default:
                    break;
            }

            //Delete any Cell at this position
            DeleteCellAtPos(spawnPos);
            //Add this new Cell to the list
            cells.Add(cell);
            //Signal a cell movement to WorldDrawer
            _worldDrawer.SignalCellMove(spawnPos, spawnPos, CellType.Empty, cellType, false);

            //Increment id counter
            currentId++;

            //Return the cell (useful for when we want to keep track of when a highlighted cell changes type.
            return cell;
        }
        public void DeleteCell(Cell cell)
        {
            //Remove cell from list
            cells.Remove(cell);
            //Send cell movement info to WorldDrawer with movement vector (0,0) and making the cell empty
            _worldDrawer.SignalCellMove(cell.Position, cell.Position, CellType.Empty, CellType.Empty, false);
        }

        
        private void ClearGrid()
        {
            //Delete the 0th cell while we still can.
            while(cells.Count > 0)
            {
                DeleteCell(cells[0]);
            }
        }

        public void DeleteCellAtPos(Vector2 gridPos)
        {
            foreach(Cell c in cells)
            {
                if(c.Position == gridPos)
                {
                    DeleteCell(c);
                    return;
                }
            }
        }
        
        public void ChangeCurrentCellType(CellType cellType)
        {
            currentCellType = cellType;
        }

        //Generating a string jam packed with information about a Cell.
        private string GenerateInfoString(Cell c)
        {
            string text = "CELL MENU:\n";

            //Name and stress level if we have a Character.
            if (c.Type == CellType.Worrywart || c.Type == CellType.Student)
            {
                Character ch = (Character)c;
                text += "   - Name: " + ch.Name + "\n";
                text += "   - Stress Level: " + ch.StressLevel + "\n";
            }
            
            //Type and position.
            text += "   - Cell Type: " + c.Type + "\n\n" +
                    "   - Position: " + c.Position + "\n";

            

            //Unique ID and OOP Class.
            text +=  "   - ID: " + c.Id + "\n" +
                "   - Class: " + c.GetType().Name + "\n";

            //If the class's base class isn't itself (i.e. it has a parent class)
            if (c.GetType().BaseType != typeof(Object))
            {
                text += "   - Parent Class: " + c.GetType().BaseType.Name + "\n";

            }
            text += "   - Sub Classes: \n";
            //CODE BORROWED FROM https://stackoverflow.com/questions/8928464/for-an-object-can-i-get-all-its-subclasses-using-reflection-or-other-ways !!!!! -----------
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.BaseType == c.GetType())
                        text += "      - " + type.Name + "\n";
                }
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------------------


            return text;
            
        }
        //Time speed setting (this is called by Buttons).
        private void SetUpdateTime(double updateTime)
        {
            timeBetweenUpdates = updateTime;
        }

    }
}
