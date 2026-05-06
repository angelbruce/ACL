using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACL.business.mcp.local
{

    [McpServerTool]
    public class WebFetcher
    {
        /// <summary>
        /// Initializes a new instance of the WebFetcher class.
        /// </summary>
        public WebFetcher()
        {
        }
        /// <summary>
        /// Fetches content from a specified URL and returns it as Markdown format.
        /// </summary>
        /// <param name="url">The URL to fetch content from.</param>
        /// <returns>The content as a Markdown string.</returns>
        [McpTool, Description("Fetches content from a specified URL and returns it as Markdown format.")]
        public static async Task<string> FetchMarkdownAsync(
            [Required][Description("The URL to fetch content from.")] string url)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string markdownContent = await response.Content.ReadAsStringAsync();
                return markdownContent;
            }
            catch (HttpRequestException e)
            {
                return $"Error fetching URL: {e.Message}";
            }
        }
        /// <summary>
        /// Fetches content from a specified URL and returns it as plain Text.
        /// </summary>
        /// <param name="url">The URL to fetch content from.</param>
        /// <returns>The content as a plain text string.</returns>
        [McpTool, Description("Fetches content from a specified URL and returns it as plain Text.")]
        public static async Task<string> FetchTextAsync(
            [Required][Description("The URL to fetch content from.")] string url)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string textContent = await response.Content.ReadAsStringAsync();
                return textContent;
            }
            catch (HttpRequestException e)
            {
                return $"Error fetching URL: {e.Message}";
            }
        }
    }
}
