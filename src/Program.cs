using CommandLine;

using WinPath.Library;

namespace WinPath
{
    class Program
    {
        static void Main(string[] args)
        {
            // Temporary debug code.
            Library.UserPath.BackupPath(System.Environment.GetEnvironmentVariable("Path", System.EnvironmentVariableTarget.User));
            /*Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options => {
                    if (options.Value == null) HandleArgument(HandleEventType.NoValue);
                    else if (options.Value != null && options.AddToUserVariables)
                        HandleArgument(HandleEventType.UserPath, options);
                });*/
        }

        static void HandleArgument(HandleEventType eventType, Options options = null)
        {
            switch (eventType)
            {
                case HandleEventType.NoValue:
                    System.Console.WriteLine("Please provide a value to be added.");
                    break;

                case HandleEventType.UserPath:
                    UserPath.AddToPath(options.Value);
                    break;
            }
        }
    }
}
