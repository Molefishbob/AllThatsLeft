public interface IPauseable {
    void Pause();
    void UnPause();
    /// <summary>
    /// Put this method in Start().
    /// </summary>
    void AddToPauseCollection();
    /// <summary>
    /// Put this method in OnDestroy().
    /// </summary>
    void RemoveFromPauseCollection();
}