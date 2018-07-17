namespace OpenQA.Selenium.Winium
{
    #region using

    using System;
    using System.Reflection;

    using OpenQA.Selenium.Remote;

    #endregion

    public class WiniumDriverCommandExecutor : ICommandExecutor
    {
        #region Fields

        private ICommandExecutor internalExecutor;

        private WiniumDriverService service;

        #endregion

        #region Constructors and Destructors

        // https://raw.githubusercontent.com/SeleniumHQ/selenium/master/dotnet/CHANGELOG
        //v3.11.2
        // * Reverted change of ICommandExecutor interface to extend IDisposable. This
        //   change will reappear at some point. Downstream projects will need to
        //   implement a `public void Dispose()` method on any classes that implement
        //   ICommandExecutor to prevent breaking when this interface change is added
        //   again. Fixes issue #5768.
        public void Dispose()
        {
            // TO DO: ?
        }

        public WiniumDriverCommandExecutor(WiniumDriverService driverService, TimeSpan commandTimeout)
        {
            this.service = driverService;
            this.internalExecutor = CommandExecutorFactory.GetHttpCommandExecutor(driverService.ServiceUrl, commandTimeout);
        }

        #endregion

        #region Public Properties

        public CommandInfoRepository CommandInfoRepository
        {
            get
            {
                return this.internalExecutor.CommandInfoRepository;
            }
        }

        #endregion

        #region Public Methods and Operators

        public Response Execute(Command commandToExecute)
        {
            if (commandToExecute == null)
            {
                throw new ArgumentNullException("commandToExecute", "Command may not be null");
            }

            if (commandToExecute.Name == DriverCommand.NewSession)
            {
                this.service.Start();
            }

            try
            {
                return this.internalExecutor.Execute(commandToExecute);
            }
            finally
            {
                if (commandToExecute.Name == DriverCommand.Quit)
                {
                    this.service.Dispose();
                }
            }
        }

        #endregion
    }
}
