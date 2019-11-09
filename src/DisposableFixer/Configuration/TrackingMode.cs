namespace DisposableFixer.Configuration
{
    /// <summary>
    /// Defines the operation mode of a tracking setter
    /// </summary>
    public enum TrackingMode
    {
        /// <summary>
        /// Each new assignment disposes the already existing value, if any.
        /// </summary>
        Always,
        /// <summary>
        /// Only the first assignment is safe, because the initial value was null.
        /// </summary>
        Once
    }
}