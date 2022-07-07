using System.IO;
using UnityEditor;

namespace Libplanet.Unity.Editor
{
    /// <summary>
    /// Unity editor menu item for managing agent config data.
    /// </summary>
    public static class AgentMenu
    {
        /// <summary>
        /// Opens the agent config file location.
        /// </summary>
        [MenuItem("Tools/Libplanet/Agent config/Open agent config file location")]
        public static void OpenAgentConfigLocation()
        {
            const string title = "Open agent config file location";
            string path = Paths.AgentConfigPath;

            if (!File.Exists(path))
            {
                string dialogContent =
                    $"Agent config file not found at {path}.\n" +
                    "Please create a agent config first.";
                EditorUtility.DisplayDialog(
                    title,
                    dialogContent,
                    "Close");
            }
            else
            {
                EditorUtility.RevealInFinder(path);
            }
        }

        /// <summary>
        /// Creates a agent config file.
        /// </summary>
        [MenuItem("Tools/Libplanet/Agent config/Create agent config")]
        public static void CreateAgentConfig()
        {
            const string title = "Create agent config";
            string path = Paths.AgentConfigPath;
            DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(path));

            if (File.Exists(path))
            {
                string dialogContent =
                    $"Agent config found at {path}.\n" +
                    "Do you want to overwrite it?";
                if (!EditorUtility.DisplayDialog(
                    title,
                    dialogContent,
                    "Ok",
                    "Cancel"))
                {
                    return;
                }
            }

            if (!directory.Exists)
            {
                directory.Create();
            }

            Utils.CreateAgentConfig(path);
            EditorUtility.DisplayDialog(title, "New agent config created.", "Close");
        }

        /// <summary>
        /// Removes the agent config file.
        /// </summary>
        [MenuItem("Tools/Libplanet/Agent config/Delete agent config")]
        public static void DeleteAgentConfig()
        {
            const string title = "Delete agent config";
            string path = Paths.SwarmConfigPath;

            if (File.Exists(path))
            {
                string dialogContent =
                    $"Agent config found at {path}.\n" +
                    "An agent config is required to run a blockchain node.\n" +
                    "Do you want to delete it?";
                if (EditorUtility.DisplayDialog(
                    title,
                    dialogContent,
                    "Ok",
                    "Cancel"))
                {
                    File.Delete(path);
                    EditorUtility.DisplayDialog(title, "Agent config deleted.", "Close");
                }
            }
            else
            {
                EditorUtility.DisplayDialog(title, "Agent config not found.", "Close");
            }
        }
    }
}
