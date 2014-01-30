namespace ProjectPsi.GUI.Interfaces
{
    /// <summary>
    /// An interface for a class that manages key state from some form of input.
    /// </summary>
    /// <typeparam name="TKeyType">The type to use to represent keys.</typeparam>
    interface IKeyStateManager<in TKeyType>
    {
        bool IsKeyDown(TKeyType key);
        bool IsKeyUp(TKeyType key);
        bool IsKeyPressed(TKeyType key);

        /// <summary>
        /// Called when the key is pressed, so that it can updated in the KeyStateManager.
        /// </summary>
        /// <param name="key">The key pressed.</param>
        void UpdateKey(TKeyType key);

        /// <summary>
        /// Called after all input is processed.
        /// </summary>
        void PostUpdate();
    }
}
