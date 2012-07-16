namespace Landis.Extensions.Admin
{
    /// <summary>
    /// A command (action) that the tool executes on the extension database.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}
