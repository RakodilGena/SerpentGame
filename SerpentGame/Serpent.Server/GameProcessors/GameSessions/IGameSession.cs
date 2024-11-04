namespace Serpent.Server.GameProcessors.GameSessions;

internal interface IGameSession
{
    void RunGame();
    
    void ApplyUserInput(UserInputDomain userInput);

    void RequestFinishGame();
    
    event EventHandler? GameFinished;
}