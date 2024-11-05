using System.Diagnostics;
using Serpent.Server.GameProcessors.GameSessions.UserInput;
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
    private readonly ILogger<GameSession> _logger;
    private readonly IConsumablesFactory _consumablesFactory;
    private readonly UserInputAccessor _userInputAccessor = new();

    //entities
    private readonly SnakeDomain _snake;
    private readonly List<ConsumableDomain> _consumables = new(3);
    private readonly GameRecordsCurrent _gameRecords;

    private readonly GameSettings _gameSettings;

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
        var userInput = GetLastUserInput();
        //todo move snake, generate and eat things, increase records


        //should always be at the end of method so expirables react correctly.
        Tick?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyUserInput(UserInputDomain userInput) => _userInputAccessor.ApplyUserInputExclusively(userInput);

    private UserInputDomain GetLastUserInput() => _userInputAccessor.GetUserInputExclusively();

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

        RemoveExpirableConsumable(expirableConsumable);

        RegisterNewConsumable(e.ConsumableType);
    }

    private void OnConsumed(object? sender, ConsumedEventArgs e)
    {
        if (sender is not ConsumableDomain consumable)
            return;

        RemoveConsumable(consumable);

        RegisterNewConsumable(e.ConsumableType);
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

    private void RegisterNewConsumable(
        ConsumableType type)
    {
        switch (type)
        {
            case ConsumableType.CommonApple:
                //todo create apple queue with 0
                break;

            case ConsumableType.GoldenApple:
                //todo create golden apple queue
                break;

            case ConsumableType.Scissors:
                //todo create scissors queue
                break;
        }
    }

    private void ApplyRecords(ConsumableType type)
    {
        switch (type)
        {
            case ConsumableType.CommonApple:
                _gameRecords.OnCommonAppleEaten();
                break;

            case ConsumableType.GoldenApple:
                _gameRecords.OnGoldenAppleEaten();
                break;

            case ConsumableType.Scissors:
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
        GameFinished?.Invoke(this, EventArgs.Empty);
    }
}