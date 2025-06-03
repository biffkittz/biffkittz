namespace ListedLinks.PowerShellPlayground
{
    using System;
    using System.Collections.ObjectModel;

    // dotnet add package System.Management.Automation --version 7.4.10
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Security;
    using System.Text;

    /*
        Playing with this stuff: https://learn.microsoft.com/en-us/powershell/scripting/developer/hosting/creating-remote-runspaces?view=powershell-7.5
        Set up remote machine to accept PowerShell remoting connections over HTTPS
            https://www.visualstudiogeeks.com/devops/how-to-configure-winrm-for-https-manually
            PS> New-SelfSignedCertificate -DnsName "win11-pc" -CertStoreLocation Cert:\LocalMachine\My

            PS> winrm create winrm/config/Listener?Address=*+Transport=HTTPS '@{Hostname="win11-pc"; CertificateThumbprint="16A27189E0AE3BA43934EF9FE01E7BEAD76BDC9C"}'

            # Add a new firewall rule
            PS> $port=5986
            PS> netsh advfirewall firewall add rule name="Windows Remote Management (HTTPS-In)" dir=in action=allow protocol=TCP localport=$port
    
        Test the connection
            $hostName = "win11-pc"
            $winRmPort = "5986"

            $sessionOptions = New - PSSessionOption - SkipCACheck # Skip certificate authority check since we're using self-signed cert for testing
            Enter - PSSession - ComputerName $hostName - Port $winRmPort - Credential $(Get - Credential) - SessionOption $sessionOptions - UseSSL
    */

    internal class RemoteRunspaceTester
    {
        private static void Main(string[] args)
        {
            var remoteComputerUri = new Uri("https://win11-pc:5986/wsman");
            var securePassword = new SecureString();

            Console.Write("Password: ");

            var passwordBuilder = new StringBuilder();
            char passwordChar = '0';

            while (!Environment.NewLine.StartsWith(passwordChar))
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                passwordChar = consoleKeyInfo.KeyChar;

                if (!Environment.NewLine.StartsWith(passwordChar))
                    securePassword.AppendChar(passwordChar);
            }

            securePassword.MakeReadOnly();

            var connectionInfo = new WSManConnectionInfo(remoteComputerUri)
            {
                // UseSSL = true, // Not needed since we're using HTTPS in the URI
                SkipCACheck = true, // Skip certificate authority check for self-signed certs
                SkipCNCheck = true, // Skip common name check for self-signed certs
                Credential = new PSCredential(
                    "Administrator",
                    securePassword
                ),
                OperationTimeout = 3 * 60 * 1000, // default
                OpenTimeout = 1 * 60 * 1000,
            };

            using (var remoteRunspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                // "Establishing a remote connection involves sending and
                // receiving some data, so the OperationTimeout will also play a role in this process."
                remoteRunspace.Open();

                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    //powershell.AddCommand("Get-Process");
                    powershell.AddScript(@"
                        Write-Output $(""MemFree: {0:N2}%"" -f (100 - ($((Get-CimInstance Win32_OperatingSystem).FreePhysicalMemory) / $((Get-CimInstance Win32_OperatingSystem).TotalVisibleMemorySize) * 100)))
                    ");

                    //powershell.AddScript("\"{0:N2}\" -f ((Get-WmiObject -Class Win32_LogicalDisk | Where-Object {$_.DriveType -eq 3}).FreeSpace / (Get-WmiObject -Class Win32_LogicalDisk | Where-Object {$_.DriveType -eq 3}).Size * 100)");
                    //powershell.Invoke();

                    Collection<PSObject> results = powershell.Invoke();

                    //Console.WriteLine("Process              HandleCount");

                    Console.WriteLine();
                    Console.WriteLine("--------------------------------");

                    // Display the results.
                    foreach (PSObject result in results)
                    {
                        Console.WriteLine(result.ToString());

                        //Console.WriteLine(
                        //                  "{0,-20} {1}",
                        //                  result.Members["ProcessName"].Value,
                        //                  result.Members["HandleCount"].Value);
                    }
                }

                remoteRunspace.Close();
            }

            var block = Console.ReadKey(true);
        }
    }
}
