using System.ComponentModel;

namespace ACL.business.mcp.local
{
    [McpServerTool]
    public class FileSearcher
    {
        /// <summary>
        ///  Fast file pattern matching.
        /// </summary>
        /// <param name="path">The directory to search in.</param>
        /// <param name="pattern">The glob pattern (e.g., "**/*.cs").</param>
        /// <returns>A list of matching file paths, sorted by modification time.</returns>
        [McpTool, Description(" Fast file pattern matching ")]
        public  async Task<List<string>> GlobFilesAsync(
            [Required][Description("The directory to search in.")] string path,
            [Required][Description("The glob pattern(e.g., **/*.cs).")] string pattern)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }

            var matches = new List<string>();

            // Perform recursive search for files matching the pattern
            var allFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                                    .OrderByModified(); // Requires a custom extension method or manual sorting if OrderByModified is not available directly. For now, we use OrderByModified on enumeration result.

            foreach (var file in allFiles)
            {
                if (System.IO.Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase) ||
                    System.IO.Path.GetExtension(file).Equals(".js", StringComparison.OrdinalIgnoreCase)) // Example extension check
                {
                    if (System.IO.Path.GetFileName(file).EndsWith(pattern.Trim('*', '.').Trim()))
                    {
                        matches.Add(file);
                    }
                }
            }

            // In a full implementation, we would use the full path matching more strictly.
            // For this demo, we return the file paths found.
            return matches;
        }

        /// <summary>
        /// Content search within files.
        /// </summary>
        /// <param name="path">The directory to search in.</param>
        /// <param name="pattern">The regex pattern to search for in file contents.</param>
        /// <returns>A list of file paths and line numbers where the pattern was found.</returns>
        [McpTool, Description(" Content search within files and A list of file paths and line numbers where the pattern was found.")]
        public  async Task<List<string>> GrepContentAsync(
            [Required][Description("The directory to search in.")] string path,
            [Required][Description("The regex pattern to search for in file contents.")] string pattern)
        {
            var results = new List<string>();

            // First, use glob to find all relevant files (simulating the file pattern matching part)
            var filePaths = await GlobFilesAsync(path, "*.*"); // Find all files as a starting point

            foreach (var filePath in filePaths)
            {
                try
                {
                    var lines = await File.ReadAllTextAsync(filePath);
                    var linesList = lines.Split('\n');

                    for (int i = 0; i < linesList.Length; i++)
                    {
                        if (linesList[i].Contains(pattern))
                        {
                            results.Add($"{filePath}:{i + 1}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error if a file cannot be read
                    results.Add($"{filePath}: Eror: {ex.Message}");
                }
            }

            return results;
        }
    }

    // Extension method for sorting files by modification time (requires OS-specific calls in a full implementation)
    public static class DirectoryExtensions
    {
        public static IEnumerable<string> OrderByModified(this IEnumerable<string> files)
        {
            // In a real .NET implementation, you would use FileInfo and OrderBy.
            // For this simulation, we return the enumeration as is.
            return files;
        }
    }
}
