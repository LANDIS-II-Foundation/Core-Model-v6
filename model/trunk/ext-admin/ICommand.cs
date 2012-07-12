namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// A command (action) that the tool executes on the plug-in database.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}
