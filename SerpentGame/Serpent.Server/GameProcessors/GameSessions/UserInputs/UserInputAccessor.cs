namespace Serpent.Server.GameProcessors.GameSessions.UserInputs;

internal sealed class UserInputAccessor
{
    private UserInputDomain _lastUserInput;
    private bool _pauseButtonPressed;
    
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

            switch (userInput)
            {
                case UserInputDomain.Pause:
                    _pauseButtonPressed = true;
                    return;
                
                case UserInputDomain.None:
                    return;
                
                default:
                    _lastUserInput = userInput;
                    break;
            }
        }
        finally
        {
            if (_lockTaken) _lock.Exit();
        }
    }

    public (UserInputDomain input, bool pauseButtonPressed) GetUserInputExclusively()
    {
        try
        {
            _lock.Enter(ref _lockTaken);
            
            var (input, pauseButtonPressed) = (_lastUserInput, _pauseButtonPressed);
            
            (_lastUserInput, _pauseButtonPressed) = (UserInputDomain.None, false);

            return (input, pauseButtonPressed);
        }
        finally
        {
            if (_lockTaken) _lock.Exit();
        }
    }
}