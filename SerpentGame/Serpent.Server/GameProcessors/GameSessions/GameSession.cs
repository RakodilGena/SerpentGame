using Serpent.Server.GameProcessors.Models.GameSettings;
using Serpent.Server.GameProcessors.Models.Snakes;
using Serpent.Server.GameProcessors.Models.Snakes.Segments;
using Serpent.Server.GameProcessors.Models.Snakes.Segments.Directions;

namespace Serpent.Server.GameProcessors.GameSessions;

internal sealed class GameSession : IGameSession
{
    private readonly ILogger<GameSession> _logger;
    
    private readonly SnakeDomain _snake;
    private readonly GameRecordsCurrent _gameRecords;

    private readonly GameSettings _gameSettings;

    private UserInputDomain _lastInput;
    private SpinLock _lastInputLock;
    private bool _lastInputLockTaken;
    //not being locked cz I don't care of one extra game cycle even if one appears one in 50 years.
    private bool _gameFinished;

    public GameSession(GameSettings gameSettings, ILogger<GameSession> logger)
    {
        _gameSettings = gameSettings;
        _gameRecords = new GameRecordsCurrent();

        _lastInput = UserInputDomain.None;
        _lastInputLock = new SpinLock();
        
        _logger = logger;

        _snake = CreateSnake();
        _snake.CollisionHappened += OnCollisionHappened;
    }

    private SnakeDomain CreateSnake()
    {
        var middlePoint = GetMiddlePoint();

        var head = new SnakeHeadSegmentDomain(middlePoint, middlePoint - 2, SnakeDirectionTypeDomain.Up);
        List<SnakeBodySegmentDomain> body =
        [
            new(x: middlePoint, y: middlePoint - 1, SnakeBodySegmentDirectionTypeDomain.Up),
            new(x: middlePoint, y: middlePoint, SnakeBodySegmentDirectionTypeDomain.Up),
            new(x: middlePoint, y: middlePoint + 1, SnakeBodySegmentDirectionTypeDomain.Up)
        ];
        var tail = new SnakeTailSegmentDomain(middlePoint, middlePoint + 2, SnakeDirectionTypeDomain.Up);

        return new SnakeDomain(head, body, tail);
    }

    private int GetMiddlePoint()
    {
        var middlePoint = _gameSettings.FieldSettings.MaxPosition / 2 + 1;
        return middlePoint;
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

    public event EventHandler? GameFinished;

    private void OnCollisionHappened(object? sender, EventArgs args) => RequestFinishGame();

    public void RequestFinishGame() => _gameFinished = true;
}