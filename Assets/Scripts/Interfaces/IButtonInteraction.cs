public interface IButtonInteraction
{
    /// <summary>
    /// Add the desired effect when the connected button is pressed.
    /// </summary>
    bool ButtonDown(float actionDelay);

    /// <summary>
    /// Add the desired effect when the connected button is not pressed / released.
    /// </summary>
    bool ButtonUp();
}
