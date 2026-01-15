using Locust.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace Locust.Pages
{
    public class _PostLayoutModel : PageModel
    {
        private readonly IConfiguration _config;

        public _PostLayoutModel(IConfiguration config)
        {
            _config = config;
        }

        public PostViewModel? Post { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cs = _config.GetConnectionString("MySqlConnection");

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(@"
                SELECT postID, title, bodyText, likes
                FROM post
                WHERE postID = @id
                LIMIT 1;
            ", conn);

            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound(); // of Redirect/een nette pagina
            }

            Post = new PostViewModel
            {
                PostID = reader.GetInt32("postID"),
                Title = reader.GetString("title"),
                BodyText = reader.GetString("bodyText"),
                Likes = reader.GetInt32("likes")
            };

            return Page();
        }
    }
}
