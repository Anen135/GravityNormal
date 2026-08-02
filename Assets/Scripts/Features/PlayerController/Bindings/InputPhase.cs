namespace Lucy.Features.PlayerController.Bindings
{
    public enum InputPhase { Started, Performed, Canceled }
    public interface IInputBinding<TAction>
    {
        TAction Action { get; }
        InputPhase Phase { get; }
    }
}