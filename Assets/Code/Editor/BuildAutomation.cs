#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace VHDPV2.Editor
{
    public static class BuildAutomation
    {
        private const string WindowsBuildPath = "Builds/Windows";
        private const string WebGlBuildPath = "Builds/WebGL";

        [MenuItem("VHD/Build/All")] 
        public static void BuildAll()
        {
            BuildWindows();
            BuildWebGL();
        }

        [MenuItem("VHD/Build/Windows")]
        public static void BuildWindows()
        {
            string buildPath = Path.Combine(WindowsBuildPath, $"VHDPV2_{GetVersionStamp()}.exe");
            BuildPlayerOptions options = new()
            {
                scenes = GetScenes(),
                locationPathName = buildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Windows build failed: {report.summary.result}");
            }
        }

        [MenuItem("VHD/Build/WebGL")]
        public static void BuildWebGL()
        {
            string buildPath = Path.Combine(WebGlBuildPath, GetVersionStamp());
            BuildPlayerOptions options = new()
            {
                scenes = GetScenes(),
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"WebGL build failed: {report.summary.result}");
            }
        }

        private static string[] GetScenes()
        {
            return new[]
            {
                "Assets/Scenes/Boot.unity",
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Game.unity",
                "Assets/Scenes/Meta.unity"
            };
        }

        private static string GetVersionStamp()
        {
            string date = DateTime.UtcNow.ToString("yyyyMMdd_HHmm");
            string hash = GetGitHash();
            return $"{date}_{hash}";
        }

        private static string GetGitHash()
        {
            try
            {
                ProcessStartInfo info = new("git", "rev-parse --short HEAD")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = Process.Start(info)!;
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    string? output = process.StandardOutput.ReadLine();
                    if (!string.IsNullOrEmpty(output))
                    {
                        return output.Trim();
                    }
                }
            }
            catch
            {
                // ignored
            }

            return "unknown";
        }
    }
}
