using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PacmanGame.Models;
using PacmanGame.Services;

namespace PacmanGame.Forms
{
    public partial class MainForm : Form
    {
        private CellType[,] MazeCells;
        private Point PlayerStartPos;
        private Point PlayerPos;
        private Direction PlayerDirection;
        private Direction PlayerNextDirection;
        private bool MouthOpen;

        private List<Ghost> Ghosts;

        private int Score;
        private int Lives;
        private int Level;
        private int DotsRemaining;
        private bool IsVictory;

        private GameState CurrentState;
        private readonly Random RandomGen;

        public MainForm()
        {
            InitializeComponent();
            ConfigureFormSize();
            // keep status labels centred if the window is resized later
            this.Resize += (s, e) => RepositionStatusLabels(this.ClientSize.Width);
            RandomGen = new Random();
            InitializeGame();
        }

        private void ConfigureFormSize()
        {
            Rectangle WorkingArea = Screen.PrimaryScreen.WorkingArea;

            const int ChromeHeightEstimate = 45;
            const int MarginPx = 60;
            int StatusHeight = AppConstants.StatusPanelHeight;

            int MaxPanelWidth = WorkingArea.Width - MarginPx;
            int MaxPanelHeight = WorkingArea.Height - ChromeHeightEstimate - StatusHeight - MarginPx;

            int CellByWidth = MaxPanelWidth / AppConstants.GridCols;
            int CellByHeight = MaxPanelHeight / AppConstants.GridRows;

            int ComputedCellSize = Math.Min(CellByWidth, CellByHeight);
            ComputedCellSize = Math.Max(AppConstants.MinCellSize, Math.Min(AppConstants.MaxCellSize, ComputedCellSize));

            AppConstants.CellSize = ComputedCellSize;

            int PanelWidth = AppConstants.CellSize * AppConstants.GridCols;
            int PanelHeight = AppConstants.CellSize * AppConstants.GridRows;

            this.ClientSize = new Size(PanelWidth, PanelHeight + StatusHeight);
            this.StartPosition = FormStartPosition.CenterScreen;

            RepositionStatusLabels(PanelWidth);
        }

        private void RepositionStatusLabels(int PanelWidth)
        {
            int ThirdWidth = PanelWidth / 3;
            int lblHeight = PnlStatus.ClientSize.Height;

            // Turn off AutoSize so we can size/center the labels reliably
            LblScore.AutoSize = false;
            LblLevel.AutoSize = false;
            LblLives.AutoSize = false;

            // Center text inside each label
            LblScore.TextAlign = ContentAlignment.MiddleCenter;
            LblLevel.TextAlign = ContentAlignment.MiddleCenter;
            LblLives.TextAlign = ContentAlignment.MiddleCenter;

            // Place each label in one third of the panel (last gets remaining pixels)
            LblScore.SetBounds(0, 0, ThirdWidth, lblHeight);
            LblLevel.SetBounds(ThirdWidth, 0, ThirdWidth, lblHeight);
            LblLives.SetBounds(ThirdWidth * 2, 0, PanelWidth - (ThirdWidth * 2), lblHeight);
        }

        private void InitializeGame()
        {
            Level = 1;
            Score = 0;
            Lives = AppConstants.InitialLives;
            IsVictory = false;
            CurrentState = GameState.Ready;
            LoadLevel(Level);
        }

        private void LoadLevel(int LevelNumber)
        {
            MazeData Data = MazeBuilder.BuildMaze(LevelNumber);

            MazeCells = Data.Cells;
            DotsRemaining = Data.TotalDots;
            PlayerStartPos = Data.PlayerStart;
            PlayerPos = Data.PlayerStart;
            PlayerDirection = Direction.Left;
            PlayerNextDirection = Direction.None;

            Ghosts = new List<Ghost>();
            for (int I = 0; I < Data.GhostStarts.Count; I++)
            {
                Point GStart = Data.GhostStarts[I];
                Color GColor = ThemeColors.GhostPalette[I % ThemeColors.GhostPalette.Length];
                Ghosts.Add(new Ghost(GStart.X, GStart.Y, GColor));
            }

            int BaseInterval = 220 - (LevelNumber * 30);
            GhostTimer.Interval = Math.Max(90, BaseInterval);

            UpdateStatusLabels();
            GamePanel.Invalidate();
        }

        private void StartGame()
        {
            CurrentState = GameState.Playing;
            PlayerTimer.Start();
            GhostTimer.Start();
            GamePanel.Invalidate();
        }

        private void RestartGame()
        {
            InitializeGame();
            StartGame();
        }

        protected override bool ProcessCmdKey(ref Message Msg, Keys KeyData)
        {
            switch (KeyData)
            {
                case Keys.Up:
                    PlayerNextDirection = Direction.Up;
                    return true;
                case Keys.Down:
                    PlayerNextDirection = Direction.Down;
                    return true;
                case Keys.Left:
                    PlayerNextDirection = Direction.Left;
                    return true;
                case Keys.Right:
                    PlayerNextDirection = Direction.Right;
                    return true;
                case Keys.Enter:
                    HandleEnterKey();
                    return true;
            }

            return base.ProcessCmdKey(ref Msg, KeyData);
        }

        private void HandleEnterKey()
        {
            if (CurrentState == GameState.Ready)
            {
                StartGame();
            }
            else if (CurrentState == GameState.GameOver)
            {
                RestartGame();
            }
        }

        private void PlayerTimer_Tick(object Sender, EventArgs E)
        {
            MouthOpen = !MouthOpen;
            TryMovePlayer();
            CheckDotCollision();
            CheckGhostCollisions();
            UpdateStatusLabels();
            GamePanel.Invalidate();
        }

        private void GhostTimer_Tick(object Sender, EventArgs E)
        {
            foreach (Ghost GhostObj in Ghosts)
            {
                MoveGhost(GhostObj);
            }

            CheckGhostCollisions();
            GamePanel.Invalidate();
        }

        private void VulnerableTimer_Tick(object Sender, EventArgs E)
        {
            VulnerableTimer.Stop();

            foreach (Ghost GhostObj in Ghosts)
            {
                GhostObj.IsVulnerable = false;
            }

            GamePanel.Invalidate();
        }

        private void LevelTransitionTimer_Tick(object Sender, EventArgs E)
        {
            LevelTransitionTimer.Stop();

            if (Level >= AppConstants.MaxLevel)
            {
                HandleGameOver(true);
            }
            else
            {
                Level++;
                LoadLevel(Level);
                CurrentState = GameState.Playing;
                PlayerTimer.Start();
                GhostTimer.Start();
            }
        }

        private void TryMovePlayer()
        {
            if (IsDirectionValid(PlayerPos, PlayerNextDirection))
            {
                PlayerDirection = PlayerNextDirection;
            }

            if (IsDirectionValid(PlayerPos, PlayerDirection))
            {
                Point Offset = GetDirectionOffset(PlayerDirection);
                PlayerPos = new Point(PlayerPos.X + Offset.X, PlayerPos.Y + Offset.Y);
            }
        }

        private bool IsDirectionValid(Point From, Direction Dir)
        {
            Point Offset = GetDirectionOffset(Dir);
            int NewX = From.X + Offset.X;
            int NewY = From.Y + Offset.Y;
            return IsWalkable(NewX, NewY);
        }

        private bool IsWalkable(int X, int Y)
        {
            if (X < 0 || X >= AppConstants.GridCols || Y < 0 || Y >= AppConstants.GridRows)
            {
                return false;
            }

            return MazeCells[X, Y] != CellType.Wall;
        }

        private Point GetDirectionOffset(Direction Dir)
        {
            switch (Dir)
            {
                case Direction.Up:
                    return new Point(0, -1);
                case Direction.Down:
                    return new Point(0, 1);
                case Direction.Left:
                    return new Point(-1, 0);
                case Direction.Right:
                    return new Point(1, 0);
                default:
                    return new Point(0, 0);
            }
        }

        private void CheckDotCollision()
        {
            CellType Cell = MazeCells[PlayerPos.X, PlayerPos.Y];

            if (Cell == CellType.Dot)
            {
                MazeCells[PlayerPos.X, PlayerPos.Y] = CellType.Empty;
                Score += AppConstants.DotScore;
                DotsRemaining--;
                SoundService.PlayDotSound();
            }
            else if (Cell == CellType.PowerPellet)
            {
                MazeCells[PlayerPos.X, PlayerPos.Y] = CellType.Empty;
                Score += AppConstants.PowerPelletScore;
                DotsRemaining--;
                SoundService.PlayPowerPelletSound();
                ActivatePowerPellet();
            }

            if (DotsRemaining <= 0)
            {
                HandleLevelComplete();
            }
        }

        private void ActivatePowerPellet()
        {
            foreach (Ghost GhostObj in Ghosts)
            {
                GhostObj.IsVulnerable = true;
            }

            VulnerableTimer.Stop();
            VulnerableTimer.Interval = AppConstants.VulnerableDurationMs;
            VulnerableTimer.Start();
        }

        private void CheckGhostCollisions()
        {
            foreach (Ghost GhostObj in Ghosts)
            {
                if (GhostObj.GridX == PlayerPos.X && GhostObj.GridY == PlayerPos.Y)
                {
                    if (GhostObj.IsVulnerable)
                    {
                        EatGhost(GhostObj);
                    }
                    else
                    {
                        LoseLife();
                        break;
                    }
                }
            }
        }

        private void EatGhost(Ghost GhostObj)
        {
            Score += AppConstants.GhostEatenScore;
            SoundService.PlayGhostEatenSound();
            GhostObj.ResetPosition();
        }

        private void LoseLife()
        {
            Lives--;
            SoundService.PlayDeathSound();
            UpdateStatusLabels();

            if (Lives <= 0)
            {
                HandleGameOver(false);
            }
            else
            {
                ResetPositionsAfterDeath();
            }
        }

        private void ResetPositionsAfterDeath()
        {
            PlayerPos = PlayerStartPos;
            PlayerDirection = Direction.Left;
            PlayerNextDirection = Direction.None;

            foreach (Ghost GhostObj in Ghosts)
            {
                GhostObj.ResetPosition();
            }

            GamePanel.Invalidate();
        }

        private void HandleLevelComplete()
        {
            CurrentState = GameState.LevelComplete;
            PlayerTimer.Stop();
            GhostTimer.Stop();
            VulnerableTimer.Stop();
            SoundService.PlayLevelCompleteSound();
            GamePanel.Invalidate();

            LevelTransitionTimer.Interval = AppConstants.LevelTransitionDelayMs;
            LevelTransitionTimer.Start();
        }

        private void HandleGameOver(bool Won)
        {
            IsVictory = Won;
            CurrentState = GameState.GameOver;
            PlayerTimer.Stop();
            GhostTimer.Stop();
            VulnerableTimer.Stop();
            GamePanel.Invalidate();
        }

        private void UpdateStatusLabels()
        {
            LblScore.Text = "Score: " + Score;
            LblLevel.Text = "Level: " + Level;
            LblLives.Text = "Lives: " + Lives;
        }

        private List<Direction> GetValidDirections(Ghost GhostObj)
        {
            List<Direction> Result = new List<Direction>();
            Direction Opposite = GetOppositeDirection(GhostObj.CurrentDirection);

            foreach (Direction Dir in new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right })
            {
                if (Dir == Opposite)
                {
                    continue;
                }

                Point Offset = GetDirectionOffset(Dir);
                if (IsWalkable(GhostObj.GridX + Offset.X, GhostObj.GridY + Offset.Y))
                {
                    Result.Add(Dir);
                }
            }

            if (Result.Count == 0)
            {
                Point OppositeOffset = GetDirectionOffset(Opposite);
                if (IsWalkable(GhostObj.GridX + OppositeOffset.X, GhostObj.GridY + OppositeOffset.Y))
                {
                    Result.Add(Opposite);
                }
            }

            return Result;
        }

        private Direction GetOppositeDirection(Direction Dir)
        {
            switch (Dir)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.None;
            }
        }

        private void MoveGhost(Ghost GhostObj)
        {
            List<Direction> ValidDirections = GetValidDirections(GhostObj);

            if (ValidDirections.Count == 0)
            {
                return;
            }

            Direction ChosenDirection = ChooseGhostDirection(GhostObj, ValidDirections);
            GhostObj.CurrentDirection = ChosenDirection;

            Point Offset = GetDirectionOffset(ChosenDirection);
            GhostObj.GridX += Offset.X;
            GhostObj.GridY += Offset.Y;
        }

        private Direction ChooseGhostDirection(Ghost GhostObj, List<Direction> ValidDirections)
        {
            int RandomChance = RandomGen.Next(100);
            bool UseSmartMove = GhostObj.IsVulnerable ? (RandomChance < 80) : (RandomChance < 70);

            if (!UseSmartMove)
            {
                return ValidDirections[RandomGen.Next(ValidDirections.Count)];
            }

            Direction BestDirection = ValidDirections[0];
            int BestDistance = GhostObj.IsVulnerable ? int.MinValue : int.MaxValue;

            foreach (Direction Dir in ValidDirections)
            {
                Point Offset = GetDirectionOffset(Dir);
                int NewX = GhostObj.GridX + Offset.X;
                int NewY = GhostObj.GridY + Offset.Y;
                int Distance = Math.Abs(NewX - PlayerPos.X) + Math.Abs(NewY - PlayerPos.Y);

                if (GhostObj.IsVulnerable)
                {
                    if (Distance > BestDistance)
                    {
                        BestDistance = Distance;
                        BestDirection = Dir;
                    }
                }
                else
                {
                    if (Distance < BestDistance)
                    {
                        BestDistance = Distance;
                        BestDirection = Dir;
                    }
                }
            }

            return BestDirection;
        }

        private void GamePanel_Paint(object Sender, PaintEventArgs E)
        {
            Graphics G = E.Graphics;
            G.Clear(ThemeColors.FloorColor);

            DrawMaze(G);
            DrawPacman(G);

            foreach (Ghost GhostObj in Ghosts)
            {
                DrawGhost(G, GhostObj);
            }

            if (CurrentState == GameState.Ready)
            {
                DrawOverlayText(G, "PRESS ENTER TO START");
            }
            else if (CurrentState == GameState.LevelComplete)
            {
                DrawOverlayText(G, "LEVEL " + Level + " COMPLETE!");
            }
            else if (CurrentState == GameState.GameOver)
            {
                DrawOverlayText(G, IsVictory ? "YOU WIN! PRESS ENTER" : "GAME OVER - PRESS ENTER");
            }
        }

        private void DrawMaze(Graphics G)
        {
            for (int X = 0; X < AppConstants.GridCols; X++)
            {
                for (int Y = 0; Y < AppConstants.GridRows; Y++)
                {
                    int PixelX = X * AppConstants.CellSize;
                    int PixelY = Y * AppConstants.CellSize;
                    CellType Cell = MazeCells[X, Y];

                    if (Cell == CellType.Wall)
                    {
                        using (SolidBrush WallBrush = new SolidBrush(ThemeColors.WallColor))
                        {
                            G.FillRectangle(WallBrush, PixelX, PixelY, AppConstants.CellSize, AppConstants.CellSize);
                        }
                    }
                    else if (Cell == CellType.Dot)
                    {
                        using (SolidBrush DotBrush = new SolidBrush(ThemeColors.DotColor))
                        {
                            int DotSize = 6;
                            G.FillEllipse(
                                DotBrush,
                                PixelX + (AppConstants.CellSize - DotSize) / 2,
                                PixelY + (AppConstants.CellSize - DotSize) / 2,
                                DotSize,
                                DotSize);
                        }
                    }
                    else if (Cell == CellType.PowerPellet)
                    {
                        using (SolidBrush PelletBrush = new SolidBrush(ThemeColors.DotColor))
                        {
                            int PelletSize = 14;
                            G.FillEllipse(
                                PelletBrush,
                                PixelX + (AppConstants.CellSize - PelletSize) / 2,
                                PixelY + (AppConstants.CellSize - PelletSize) / 2,
                                PelletSize,
                                PelletSize);
                        }
                    }

                    using (Pen GridPen = new Pen(ThemeColors.GridLineColor))
                    {
                        G.DrawRectangle(GridPen, PixelX, PixelY, AppConstants.CellSize, AppConstants.CellSize);
                    }
                }
            }
        }

        private void DrawPacman(Graphics G)
        {
            int PixelX = PlayerPos.X * AppConstants.CellSize;
            int PixelY = PlayerPos.Y * AppConstants.CellSize;
            Rectangle Rect = new Rectangle(PixelX + 2, PixelY + 2, AppConstants.CellSize - 4, AppConstants.CellSize - 4);

            int BaseAngle;
            switch (PlayerDirection)
            {
                case Direction.Right:
                    BaseAngle = 0;
                    break;
                case Direction.Down:
                    BaseAngle = 90;
                    break;
                case Direction.Left:
                    BaseAngle = 180;
                    break;
                case Direction.Up:
                    BaseAngle = 270;
                    break;
                default:
                    BaseAngle = 0;
                    break;
            }

            using (SolidBrush Brush = new SolidBrush(ThemeColors.PacmanColor))
            {
                if (MouthOpen)
                {
                    G.FillPie(Brush, Rect, BaseAngle + 30, 300);
                }
                else
                {
                    G.FillEllipse(Brush, Rect);
                }
            }
        }

        private void DrawGhost(Graphics G, Ghost GhostObj)
        {
            int PixelX = GhostObj.GridX * AppConstants.CellSize;
            int PixelY = GhostObj.GridY * AppConstants.CellSize;
            Rectangle Rect = new Rectangle(PixelX + 2, PixelY + 2, AppConstants.CellSize - 4, AppConstants.CellSize - 4);
            Color BodyColor = GhostObj.IsVulnerable ? ThemeColors.GhostVulnerableColor : GhostObj.BaseColor;

            using (SolidBrush Brush = new SolidBrush(BodyColor))
            {
                G.FillEllipse(Brush, Rect);
            }

            int EyeSize = 5;
            using (SolidBrush EyeBrush = new SolidBrush(Color.White))
            {
                G.FillEllipse(EyeBrush, Rect.X + 6, Rect.Y + 8, EyeSize, EyeSize);
                G.FillEllipse(EyeBrush, Rect.Right - 11, Rect.Y + 8, EyeSize, EyeSize);
            }

            using (SolidBrush PupilBrush = new SolidBrush(Color.Black))
            {
                G.FillEllipse(PupilBrush, Rect.X + 7, Rect.Y + 9, 2, 2);
                G.FillEllipse(PupilBrush, Rect.Right - 10, Rect.Y + 9, 2, 2);
            }
        }

        private void DrawOverlayText(Graphics G, string Message)
        {
            using (Font OverlayFont = new Font("Segoe UI", 18, FontStyle.Bold))
            using (SolidBrush ShadowBrush = new SolidBrush(Color.Black))
            using (SolidBrush TextBrush = new SolidBrush(Color.DarkOrange))
            {
                SizeF TextSize = G.MeasureString(Message, OverlayFont);
                float TextX = (GamePanel.Width - TextSize.Width) / 2;
                float TextY = (GamePanel.Height - TextSize.Height) / 2;

                G.DrawString(Message, OverlayFont, ShadowBrush, TextX + 2, TextY + 2);
                G.DrawString(Message, OverlayFont, TextBrush, TextX, TextY);
            }
        }
    }
}
