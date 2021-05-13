namespace WinPath
{
    /// <summary>
    /// An enum to indicate types
    /// of event to handle, each
    /// enum value has its own task.
    /// </summary>
    public enum HandleEventType
    {
        /// <summary>
        /// No action.
        /// </summary>
        NoValue = 0,
        /// <summary>
        /// Add to user path value.
        /// </summary>
        UserPath,
        /// <summary>
        /// Add to system path value.
        /// </summary>
        SystemPath,
        /// <summary>
        /// Add to both user and
        /// system path value.
        /// </summary>
        UserAndSystemPath,
        /// <summary>
        /// Add to neither user or
        /// system path values.
        /// </summary>
        NoUserOrSystemPath,

        /// <summary>
        /// List every backup
        /// in backups.
        /// </summary>
        ListAllBackups,
        /// <summary>
        /// List the latest (first 3)
        /// backups in the list of backups.
        /// </summary>
        ListLatestBackups,
        /// <summary>
        /// List backups -- a ranged value
        /// of backups, ranging from 1 to
        /// <see cref="int.MaxValue"/>.
        /// </summary>
        ListBackups
    }
}
