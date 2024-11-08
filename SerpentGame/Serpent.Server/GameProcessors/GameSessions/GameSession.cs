using System.Collections.Frozen;
using System.Diagnostics;
using Serpent.Server.GameProcessors.GameSessions.Consumables;
using Serpent.Server.GameProcessors.GameSessions.UserInputs;
using Serpent.Server.GameProcessors.Models.Consumables;
using Serpent.Server.GameProcessors.Models.Consumables.Base;
using Serpent.Server.GameProcessors.Models.Consumables.Events;
using Serpent.Server.GameProcessors.Models.GameSettings;
using Serpent.Server.GameProcessors.Models.Snakes;
using Serpent.Server.GameProcessors.Models.Snakes.Events.WallCollisions;
using Serpent.Server.GameProcessors.Services.Factories;

namespace Serpent.Server.GameProcessors.GameSessions;

internal sealed class GameSession : IGameSession
{
    private bool _paused;

    private readonly ILogger<GameSession> _logger;
    private readonly IConsumablesFactory _consumablesFactory;
    private readonly UserInputAccessor _userInputAccessor;

    //entities
    private readonly SnakeDomain _snake;
    private readonly ConsumablesQueue _consumablesQueue;
    private readonly List<ConsumableDomain> _consumables;
    private readonly GameRecordsCurrent _gameRecords;

    private readonly GameSettings _gameSettings;

    private readonly Stopwatch _gameClock = new();
    private readonly Stopwatch _stepClock = new();

    //not being locked cz I don't care of one extra game cycle even if one appears one in 50 years.
    private bool _gameFinished;

    public event EventHandler? GameFinished;
    public event EventHandler? Tick;

    public GameSession(
        GameSettings gameSettings,
        ISnakeFactory snakeFactory,
        IConsumablesFactory consumablesFactory,
        ILogger<GameSession> logger)
    {
        _gameSettings = gameSettings;
        _gameRecords = new GameRecordsCurrent();

        _logger = logger;

        _snake = snakeFactory.CreateSnakeDirectionUp(gameSettings.FieldSettings.MaxPosition);
        _snake.WallCollisionCheck += OnWallCollisionCheck;
        _snake.BodyCollided += OnBodyCollided;

        _consumablesFactory = consumablesFactory;
        _consumablesFactory.Initialize(gameSettings.FieldSettings);

        _consumables = new List<ConsumableDomain>(3);
        _consumablesQueue = new ConsumablesQueue(enqueueAllConsumables: true);
        _userInputAccessor = new UserInputAccessor();
    }

    public async void RunGame()
    {
        _gameClock.Start();

        var stepElapsedTime = TimeSpan.Zero;

        //endless game cycle while snake not dead
        while (!_gameFinished)
        {
            var delay = _gameSettings.GameSessionSleepTime - stepElapsedTime;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay);

            //before new tick
            _stepClock.Restart();

            GameCycleTick();

            //todo send game data to client.

            //after all the actions on tick
            _stepClock.Stop();
            stepElapsedTime = _stepClock.Elapsed;
        }


        //todo finish. submit records 

        //todo catch and log exceptions 
        GameFinished?.Invoke(this, EventArgs.Empty);
    }

    private void GameCycleTick()
    {
        var (userInput, pauseButtonPressed) = _userInputAccessor.GetUserInputExclusively();

        //game unpauses OR wasn't paused and pause not requested.
        if (_paused == pauseButtonPressed)
        {
            _paused = false;
            GameCycleTickUnpaused(userInput);
            return;
        }

        //game pauses, or was paused and unpause not requested.
        if (_paused != pauseButtonPressed)
            _paused = true;
    }

    private void GameCycleTickUnpaused(UserInputDomain userInput)
    {
        _snake.Move(userInput);

        ProcessConsumableQueue();

        //should always be at the end of method so expirables react correctly.
        Tick?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyUserInput(UserInputDomain userInput) => _userInputAccessor.ApplyUserInputExclusively(userInput);

    private void ProcessConsumableQueue()
    {
        var tickResult = _consumablesQueue.Tick();

        //todo log added consumables

        if (tickResult.CreateCommonApple)
        {
            var (x, y) = GetFreeSpaceForConsumable();
            var apple = _consumablesFactory.CreateCommonApple(x, y);
            AddConsumable(apple);
        }

        if (tickResult.CreateGoldenApple)
        {
            var (x, y) = GetFreeSpaceForConsumable();
            var goldenApple = _consumablesFactory.CreateGoldenApple(x, y);
            AddExpirableConsumable(goldenApple);
        }

        if (tickResult.CreateScissors)
        {
            var (x, y) = GetFreeSpaceForConsumable();
            var scissors = _consumablesFactory.CreateScissors(x, y);
            AddExpirableConsumable(scissors);
        }
    }

    private (int x, int y) GetFreeSpaceForConsumable()
    {
        IEnumerable<(int x, int y)> busySpotsTmp =
        [
            .._snake.AllSegments.Select(s => (s.X, s.Y)),
            .._consumables.Select(c => (c.X, c.Y))
        ];

        var busySpots = busySpotsTmp.ToFrozenSet();

        var maxPos = _gameSettings.FieldSettings.MaxPosition;

        var list = new List<(int x, int y)>(capacity: (maxPos + 1) * (maxPos + 1));

        for (int x = 0; x <= maxPos; x++)
        for (int y = 0; y <= maxPos; y++)
        {
            var point = (x, y);
            if (busySpots.Contains(point))
                continue;

            list.Add(point);
        }

        var freeSpotIndex = Random.Shared.Next(0, list.Count);
        return list[freeSpotIndex];
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
            eventArgs.SetResult(WallCollisionResultDomain.Teleport(x, y));
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
                eventArgs.SetResult(WallCollisionResultDomain.Teleport(x, y));
            }
        }

        eventArgs.SetResult(WallCollisionResultDomain.NoCollision());
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

    private void OnBodyCollided(object? sender, EventArgs e)
    {
        RequestFinishGame();
    }

    private void OnExpired(object? sender, ExpiredEventArgs e)
    {
        if (sender is not ExpirableConsumableDomain expirableConsumable)
            return;

        //todo log expiration

        RemoveExpirableConsumable(expirableConsumable);

        QueueNewConsumable(e.ConsumableType);
    }

    private void OnConsumed(object? sender, ConsumedEventArgs e)
    {
        if (sender is not ConsumableDomain consumable)
            return;

        //todo log consumation

        RemoveConsumable(consumable);

        QueueNewConsumable(e.ConsumableType);
        ApplyRecords(e.ConsumableType);
    }

    private void RemoveExpirableConsumable(ExpirableConsumableDomain expirableConsumable)
    {
        expirableConsumable.Expire -= OnExpired;
        Tick -= expirableConsumable.OnTick;
        RemoveConsumable(expirableConsumable);
    }

    private void RemoveConsumable(ConsumableDomain consumable)
    {
        _snake.ConsumeCheck -= consumable.OnConsumeCheck;
        consumable.Consumed -= OnConsumed;
        _consumables.Remove(consumable);
    }

    private void QueueNewConsumable(
        ConsumableTypeDomain type)
    {
        //todo log queue
        switch (type)
        {
            case ConsumableTypeDomain.CommonApple:
                _consumablesQueue.EnqueueCommonApple();
                break;

            case ConsumableTypeDomain.GoldenApple:
                _consumablesQueue.EnqueueGoldenApple();
                break;

            case ConsumableTypeDomain.Scissors:
                _consumablesQueue.EnqueueScissors();
                break;
        }
    }

    private void AddExpirableConsumable(ExpirableConsumableDomain expirableConsumable)
    {
        expirableConsumable.Expire += OnExpired;
        Tick += expirableConsumable.OnTick;
        AddConsumable(expirableConsumable);
    }

    private void AddConsumable(ConsumableDomain consumable)
    {
        _snake.ConsumeCheck += consumable.OnConsumeCheck;
        consumable.Consumed += OnConsumed;
        _consumables.Add(consumable);
    }

    private void ApplyRecords(ConsumableTypeDomain type)
    {
        switch (type)
        {
            case ConsumableTypeDomain.CommonApple:
                _gameRecords.OnCommonAppleEaten();
                break;

            case ConsumableTypeDomain.GoldenApple:
                _gameRecords.OnGoldenAppleEaten();
                break;

            case ConsumableTypeDomain.Scissors:
                _gameRecords.OnScissorsEaten();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type,
                    "consumable type was out of range on applying records.");
        }
    }

    public void RequestFinishGame()
    {
        _gameFinished = true;
        _snake.WallCollisionCheck -= OnWallCollisionCheck;
        _snake.BodyCollided -= OnBodyCollided;
    }
}