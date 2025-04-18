using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace VSpace.Others
{
    public class BoincClientService
    {
        private const string BOINC_CLIENT_PATH = @"C:\Program Files\BOINC\boinccmd.exe";
        private bool _isConnected = false;

        public bool IsConnected => _isConnected;

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (!File.Exists(BOINC_CLIENT_PATH))
                {
                    throw new FileNotFoundException("BOINC client not found. Please install BOINC first.");
                }

                // Try to get client state to verify connection
                await GetClientStateAsync();
                _isConnected = true;
                return true;
            }
            catch (Exception)
            {
                _isConnected = false;
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            _isConnected = false;
        }

        public async Task<string> GetClientStateAsync()
        {
            if (!_isConnected)
                return "Not Connected";

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = BOINC_CLIENT_PATH,
                        Arguments = "--get_project_status",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                // Parse the output to count projects
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int projectCount = 0;
                bool isUserSection = false;

                foreach (var line in lines)
                {
                    if (line.Contains("======== Projects ========"))
                    {
                        isUserSection = true;
                        continue;
                    }
                    if (line.Contains("======== Applications ========"))
                    {
                        break;
                    }
                    if (isUserSection && line.Contains(") -----------"))
                    {
                        projectCount++;
                    }
                }

                return $"Connected - {projectCount} project(s) found";
            }
            catch (Exception)
            {
                return "Error getting state";
            }
        }

        public async Task<List<BoincProject>> GetProjectsAsync()
        {
            if (!_isConnected)
                return new List<BoincProject>();

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = BOINC_CLIENT_PATH,
                        Arguments = "--get_project_status",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                return ParseProjects(output);
            }
            catch (Exception)
            {
                return new List<BoincProject>();
            }
        }

        private List<BoincProject> ParseProjects(string output)
        {
            var projects = new List<BoincProject>();
            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            bool isUserSection = false;
            bool isProjectEntry = false;
            bool isGuiUrlSection = false;
            BoincProject currentProject = null;

            foreach (var line in lines)
            {
                if (line.Contains("======== Projects ========"))
                {
                    isUserSection = true;
                    continue;
                }
                if (line.Contains("======== Applications ========"))
                {
                    break;
                }
                if (isUserSection)
                {
                    if (line.Contains(") -----------"))
                    {
                        isProjectEntry = true;
                        isGuiUrlSection = false;
                        if (currentProject != null)
                        {
                            projects.Add(currentProject);
                        }
                        currentProject = new BoincProject
                        {
                            Status = "Active",
                            Tasks = "0",
                            Progress = "0%"
                        };
                    }
                    else if (isProjectEntry && currentProject != null)
                    {
                        if (line.Contains("GUI URL:"))
                        {
                            isGuiUrlSection = true;
                        }
                        else if (!isGuiUrlSection)
                        {
                            if (line.Contains("name:"))
                            {
                                var name = line.Replace("name:", "").Trim();
                                if (!name.StartsWith("team_") && !name.Contains("Your account") && !name.Contains("Message boards") && !name.Contains("Your tasks") && !name.Contains("Donate to") && ! name.Contains("user"))
                                {
                                    currentProject.Name = name;
                                }
                            }
                            else if (line.Contains("master URL:"))
                            {
                                currentProject.ProjectUrl = line.Replace("master URL:", "").Trim();
                            }
                            else if (line.Contains("suspended via GUI:"))
                            {
                                currentProject.IsRunning = !line.Contains("yes");
                                currentProject.Status = currentProject.IsRunning ? "Running" : "Stopped";
                            }
                            else if (line.Contains("tasks:"))
                            {
                                currentProject.Tasks = line.Replace("tasks:", "").Trim();
                            }
                            else if (line.Contains("progress:"))
                            {
                                currentProject.Progress = line.Replace("progress:", "").Trim();
                            }
                        }
                    }
                }

            }

            if (currentProject != null)
            {
                projects.Add(currentProject);
            }

            return projects;
        }

        public async Task<bool> StartProjectAsync(string projectUrl)
        {
            if (!_isConnected)
                return false;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = BOINC_CLIENT_PATH,
                        Arguments = $"--project {projectUrl} resume",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> StopProjectAsync(string projectUrl)
        {
            if (!_isConnected)
                return false;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = BOINC_CLIENT_PATH,
                        Arguments = $"--project {projectUrl} suspend",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class BoincProject
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Tasks { get; set; }
        public string Progress { get; set; }
        public string ProjectUrl { get; set; }
        public bool IsRunning { get; set; }
    }
} 