using Locust.NewFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Security.Claims;

namespace Locust.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public string Email { get; private set; } = "";
        public string RoleLabel { get; private set; } = "";

        public List<PostViewModel> Posts { get; private set; } = new();

        public async Task OnGetAsync()
        {
            Email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.Identity?.Name
                ?? "onbekend";

            RoleLabel = User.IsInRole("beheerder") ? "beheerder" : "gebruiker";

            var cs = _config.GetConnectionString("MySqlConnection");

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            // Haal posts op (pas ORDER BY aan hoe jij wil)
            await using var cmd = new MySqlCommand(@"
                SELECT postID, title, bodyText, likes
                FROM post
                ORDER BY postID DESC
                LIMIT 50;
            ", conn);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Posts.Add(new PostViewModel
                {
                    PostID = reader.GetInt32("postID"),
                    Title = reader.GetString("title"),
                    BodyText = reader.GetString("bodyText"),
                    Likes = reader.GetInt32("likes"),
                    Comments = 0 // later vullen we dit met comments table
                });
            }
        }
    }
}
