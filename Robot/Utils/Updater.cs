using Onova;
using Onova.Exceptions;
using Onova.Services;
using System;
using System.Threading.Tasks;

namespace Robot
{
    public class UpdateService : IDisposable
    {
        private readonly IUpdateManager _updateManager = new UpdateManager(
            new GithubPackageResolver("Osarel", "ArduinoCommandServeur", "serveur.zip"),
            new ZipPackageExtractor());


        private Version _updateVersion;
        private bool _updatePrepared;
        private bool _updaterLaunched;

        public UpdateService()
        {
        }

        public async Task<Version> CheckForUpdatesAsync()
        {
            if (!ArduinoCommand.robot.Options.autoUpdate)
                return null;
            var check = await _updateManager.CheckForUpdatesAsync();
            return check.CanUpdate ? check.LastVersion : null;
        }

        public async Task PrepareUpdateAsync(Version version)
        {
            if (!ArduinoCommand.robot.Options.autoUpdate)
                return;

            try
            {
                await _updateManager.PrepareUpdateAsync(_updateVersion = version);
                _updatePrepared = true;
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
            }
        }

        public void FinalizeUpdate(bool needRestart)
        {
            if (!ArduinoCommand.robot.Options.autoUpdate)
                return;

            if (_updateVersion == null || !_updatePrepared || _updaterLaunched)
                return;

            try
            {
                _updateManager.LaunchUpdater(_updateVersion, needRestart);
                _updaterLaunched = true;
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
            }
        }

        public void Dispose() => _updateManager.Dispose();
    }
}