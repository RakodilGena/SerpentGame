namespace Serpent.Server.GameProcessors.GameSessions.UserInput;

internal sealed class UserInputAccessor
{
    private UserInputDomain _lastUserInput;
    private SpinLock _lock;
    private bool _lockTaken;

    public UserInputAccessor()
    {
        _lastUserInput = UserInputDomain.None;
        _lock = new SpinLock();
    }

    public void ApplyUserInputExclusively(UserInputDomain userInput)
    {
        try
        {
            _lock.Enter(ref _lockTaken);

            _lastUserInput = userInput;
        }
        finally
        {
            if (_lockTaken) _lock.Exit();
        }
    }

    public UserInputDomain GetUserInputExclusively()
    {
        try
        {
            _lock.Enter(ref _lockTaken);

            return _lastUserInput;
        }
        finally
        {
            if (_lockTaken) _lock.Exit();
        }
    }
}