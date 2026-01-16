using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Security.Claims;

namespace Locust.Pages
{
    public class CommentCreatorModel : PageModel
    {
        private readonly IConfiguration _config;

        public CommentCreatorModel (IConfiguration config)
        {
            _config = config;
        }

        public int idPost;

        public async Task OnGetAsync(int postId)
        {
            idPost = postId;
        }

        public async Task<IActionResult> OnPostAsync(int postId, string bodyText)
        {
            idPost = postId;
            if ((bodyText != "") && (bodyText != null))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cs = _config.GetConnectionString("MySqlConnection");
                await using var conn = new MySqlConnection(cs);
                await conn.OpenAsync();
                await using var cmd = new MySqlCommand(
                    @"INSERT INTO comment (text, users_id, postID)
                    VALUES (@text, @userId, @postId)",
                    conn
                );
                cmd.Parameters.AddWithValue("@text", bodyText);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@postId", postId);
                await cmd.ExecuteNonQueryAsync();
                return RedirectToPage("/Post", new { id = postId });
            }
            else
            {
                return Page();
            }
        }
    }
}
