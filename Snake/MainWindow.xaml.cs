using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Dictionary<GridValue, ImageSource> _gridValToImage = new()
    {
        { GridValue.Empty, Images.Empty },
        { GridValue.Snake, Images.Body },
        { GridValue.Food, Images.Food },
    };

    private readonly Dictionary<Direction, int> _dirToRotation = new()
    {
        { Direction.Up, 0},
        { Direction.Right, 90},
        { Direction.Down, 180},
        { Direction.Left, 270}
    };

    private const int Rows = 15;
    private const int Cols = 15;
    private readonly Image[,] _gridImages;
    private GameState _gameState;
    private bool _gameRunning;
    
    public MainWindow()
    {
        InitializeComponent();
        _gridImages = SetupGrid();
        _gameState = new GameState(Rows, Cols);
    }
    
    private async Task RunGame()
    {
        Draw();
        await ShowCountDown();
        Overlay.Visibility = Visibility.Hidden;
        await GameLoop();
        await ShowGameOver();
        _gameState = new GameState(Rows, Cols);
    }
    
    private async void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Overlay.Visibility == Visibility.Visible)
        {
            e.Handled = true;
        }

        if (!_gameRunning)
        {
            _gameRunning = true;
            await RunGame();
            _gameRunning = false;
        }
    }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (_gameState.GameOver)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.A:
                _gameState.ChangeDirection(Direction.Left);
                break;
            case Key.D:
                _gameState.ChangeDirection(Direction.Right);
                break;
            case Key.W:
                _gameState.ChangeDirection(Direction.Up);
                break;
            case Key.S:
                _gameState.ChangeDirection(Direction.Down);
                break;
        }
    }

    private async Task GameLoop()
    {
        while (!_gameState.GameOver)
        {
            await Task.Delay(100);
            _gameState.Move();
            Draw();
        }
    }

    private Image[,] SetupGrid()
    {
        Image[,] images = new Image[Rows, Cols];
        GameGrid.Rows = Rows;
        GameGrid.Columns = Cols;
        GameGrid.Width = GameGrid.Height * (Cols / (double)Rows);

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                Image image = new Image
                {
                    Source = Images.Empty,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                images[r, c] = image;
                GameGrid.Children.Add(image);
            }
        }

        return images;
    }

    private void Draw()
    {
        DrawGrid();
        DrawSnakeHead();
        ScoreText.Text = $"SCORE {_gameState.Score}";
    }

    private void DrawGrid()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                GridValue gridVal = _gameState.Grid[r, c];
                _gridImages[r, c].Source = _gridValToImage[gridVal];
                _gridImages[r, c].RenderTransform = Transform.Identity;
            }
        }
    }

    private void DrawSnakeHead()
    {
        Position headPos = _gameState.HeadPosition();
        Image image = _gridImages[headPos.Row, headPos.Col];
        image.Source = Images.Head;

        int rotation = _dirToRotation[_gameState.Dir];
        image.RenderTransform = new RotateTransform(rotation);
    }

    private async Task DrawDeadSnake()
    {
        List<Position> positions = [.._gameState.SnakePositions()];

        for (int i = 0; i < positions.Count; i++)
        {
            Position pos = positions[i];
            ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
            _gridImages[pos.Row, pos.Col].Source = source;
            await Task.Delay(50);
        }
    }

    private async Task ShowCountDown()
    {
        for (int i = 3; i >= 1; i--)
        {
            OverlayText.Text = i.ToString();
            await Task.Delay(500);
        }
    }

    private async Task ShowGameOver()
    {
        await DrawDeadSnake();
        await Task.Delay(1000);
        Overlay.Visibility = Visibility.Visible;
        OverlayText.Text = "PRESS ANY KEY TO START";
    }
}