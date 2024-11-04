using System.Diagnostics;
using Serpent.Server.GameProcessors.Models.GameSettings;
using Serpent.Server.GameProcessors.Models.Snakes;
using Serpent.Server.GameProcessors.Models.Snakes.Events.WallCollisions;
using Serpent.Server.GameProcessors.Models.Snakes.Segments;
using Serpent.Server.GameProcessors.Models.Snakes.Segments.Directions;
using Serpent.Server.GameProcessors.Services;

namespace Serpent.Server.GameProcessors.GameSessions;

internal sealed class GameSession : IGameSession
{
    private readonly ILogger<GameSession> _logger;
    
    //entities
    private readonly SnakeDomain _snake;
    
    private readonly GameRecordsCurrent _gameRecords;

    private readonly GameSettings _gameSettings;

    private UserInputDomain _lastInput;
    private SpinLock _lastInputLock;
    private bool _lastInputLockTaken;
    //not being locked cz I don't care of one extra game cycle even if one appears one in 50 years.
    private bool _gameFinished;

    public event EventHandler? GameFinished;
    public event EventHandler? Tick;
    public GameSession(
        GameSettings gameSettings,
        ISnakeFactory snakeFactory,
        ILogger<GameSession> logger)
    {
        _gameSettings = gameSettings;
        _gameRecords = new GameRecordsCurrent();

        _lastInput = UserInputDomain.None;
        _lastInputLock = new SpinLock();
        
        _logger = logger;

        _snake = snakeFactory.CreateSnakeDirectionUp(gameSettings.FieldSettings.MaxPosition);
        _snake.WallCollisionCheck += OnWallCollisionCheck;
        _snake.BodyCollided += OnBodyCollided;
    }

    public async void RunGame()
    {
        //endless game cycle while snake not dead
        while (!_gameFinished)
        {
            await Task.Delay(_gameSettings.GameSessionSleepTime);
            GameCycleTick();
            
            //todo send game data to client.
        }


        //todo finish. submit records 
        
        //todo catch and log exceptions 
        GameFinished?.Invoke(this, EventArgs.Empty);
    }

    private void GameCycleTick()
    {
        //todo move snake, generate and eat things, increase records
        
        
        //should always be at the end of method so expirables react correctly.
        Tick?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyUserInput(UserInputDomain userInput)
    {
        try
        {
            _lastInputLock.Enter(ref _lastInputLockTaken);
            
            _lastInput = userInput;
        }
        finally
        {
            if (_lastInputLockTaken) _lastInputLock.Exit();
        }
    }

    private void OnWallCollisionCheck(WallCollisionCheckEventArgs eventArgs)
    {
        var fieldSettings = _gameSettings.FieldSettings;
        var maxPosition = fieldSettings.MaxPosition;
        var wallsTransparent = fieldSettings.WallsTransparent;
        
        var (x, y) = (eventArgs.X, eventArgs.Y);
        if (x < 0 || x > maxPosition)
        {
            if (wallsTransparent is false)
            {
                eventArgs.SetResult(WallCollisionResultDomain.Break());
                RequestFinishGame();
                //todo log collide
                return;
            }
            FixPosition(ref x, fieldSettings.MaxPosition);
            eventArgs.SetResult(WallCollisionResultDomain.Teleport(x,y));
        }
        else
        {
            //else because X and Y can't exceed boundaries simultaneously
            if (y < 0 || y > fieldSettings.MaxPosition)
            {
                if (fieldSettings.WallsTransparent is false)
                {
                    eventArgs.SetResult(WallCollisionResultDomain.Break());
                    RequestFinishGame();
                    //todo log collide
                    return;
                }
                FixPosition(ref y, fieldSettings.MaxPosition);
                eventArgs.SetResult(WallCollisionResultDomain.Teleport(x,y));
            }
        }
        eventArgs.SetResult(WallCollisionResultDomain.NoCollision());
    }

    private void OnBodyCollided(object? sender, EventArgs e)
    {
        RequestFinishGame();
    }
    
    private static void FixPosition(ref int dimension, int maxValue)
    {
        if (dimension < 0)
        {
            dimension = 0;
            return;
        }

        Debug.Assert(dimension > maxValue);
        dimension = maxValue;
    }

    public void RequestFinishGame()
    {
        _gameFinished = true;
        GameFinished?.Invoke(this, EventArgs.Empty);
    }
}