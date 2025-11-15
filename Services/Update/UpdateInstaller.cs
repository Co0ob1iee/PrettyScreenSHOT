using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PrettyScreenSHOT.Services.Update
{
    /// <summary>
    /// Klasa do instalacji aktualizacji
    /// </summary>
    public class UpdateInstaller
    {
        /// <summary>
        /// Instaluje aktualizację z podanego pliku
        /// </summary>
        public void InstallUpdate(string installerPath, bool restartAfterInstall = true)
        {
            if (!File.Exists(installerPath))
            {
                throw new FileNotFoundException($"Installer not found: {installerPath}");
            }

            try
            {
                DebugHelper.LogInfo("UpdateInstaller", $"Installing update from: {installerPath}");

                string extension = Path.GetExtension(installerPath).ToLowerInvariant();
                ProcessStartInfo startInfo;

                if (extension == ".msix")
                {
                    // MSIX - użyj Add-AppxPackage
                    InstallMsixPackage(installerPath);
                    return;
                }
                else if (extension == ".exe")
                {
                    // EXE - uruchom instalator
                    startInfo = new ProcessStartInfo
                    {
                        FileName = installerPath,
                        UseShellExecute = true,
                        Verb = "runas" // Uruchom jako administrator
                    };

                    // Parametry dla Inno Setup (ciche instalowanie)
                    // /SILENT - cicha instalacja
                    // /NORESTART - nie restartuj automatycznie
                    startInfo.Arguments = "/SILENT /NORESTART";
                }
                else
                {
                    throw new NotSupportedException($"Unsupported installer format: {extension}");
                }

                // Uruchom instalator
                Process.Start(startInfo);

                // Jeśli restart, zamknij aplikację
                if (restartAfterInstall)
                {
                    // Daj czas na uruchomienie instalatora
                    Task.Delay(2000).Wait();
                    
                    // Zamknij aplikację
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateInstaller", "Error installing update", ex);
                throw;
            }
        }

        /// <summary>
        /// Instaluje pakiet MSIX
        /// </summary>
        private void InstallMsixPackage(string msixPath)
        {
            try
            {
                // Użyj PowerShell do instalacji MSIX
                var psScript = $@"
                    Add-AppxPackage -Path '{msixPath}' -ForceApplicationShutdown
                ";

                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -Command \"{psScript}\"",
                    UseShellExecute = true,
                    Verb = "runas" // Uruchom jako administrator
                };

                Process.Start(startInfo);

                // Zamknij aplikację
                Task.Delay(2000).Wait();
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateInstaller", "Error installing MSIX package", ex);
                throw;
            }
        }
    }
}
