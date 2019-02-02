public interface IButtonInteraction
{
    /// <summary>
    /// Add the desired effect when the connected button is pressed.
    /// </summary>
    void ButtonDown();

    /// <summary>
    /// Add the desired effect when the connected button is not pressed / released.
    /// </summary>
    void ButtonUp();
}
