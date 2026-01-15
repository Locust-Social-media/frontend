using Locust.NewFolder;
using Locust.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text.Json.Nodes;

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

        public List<CommentViewModel> Comments { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Random rng;

            var jsonArray = JArray.Parse((new StreamReader("first-names.json")).ReadToEnd()).ToObject<string[]>();

            var cs = _config.GetConnectionString("MySqlConnection");

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(
                @"SELECT postID, title, bodyText, likes
                FROM post
                WHERE postID = @id
                LIMIT 1;", 
                conn
            );

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

            var csComments = _config.GetConnectionString("MySqlConnection");

            await using var connComments = new MySqlConnection(csComments);
            await connComments.OpenAsync();

            await using var cmdComments = new MySqlCommand(
                @"SELECT idcomment, text, users_id, postID
                FROM comment
                WHERE postID = @id
                ",
                connComments
            );

            cmdComments.Parameters.AddWithValue("@id", id);

            await using var commentReader = await cmdComments.ExecuteReaderAsync();

            if (!await commentReader.ReadAsync())
            {
                Comments = [];
                Console.WriteLine("failed");
                return Page(); 
            }

            string userTag = commentReader.GetString("users_id").TrimStart('L', 'o', 'c', '_', 'u', 's', 'e', 'r', '_', 'i', 'd', '_');
            int userId = Int32.Parse(userTag);
            rng = new Random(userId + id);

            Comments.Add(
                new CommentViewModel
                {
                    Username = jsonArray[rng.Next(0, jsonArray.Length)],
                    BodyText = commentReader.GetString("text"),
                    Id = commentReader.GetInt32("idcomment"),
                    PostId = commentReader.GetInt32("postID")
                }
            );

            while (await commentReader.ReadAsync())
            {
                userTag = commentReader.GetString("users_id").TrimStart('L', 'o', 'c', '_', 'u', 's', 'e', 'r', '_', 'i', 'd', '_');
                userId = Int32.Parse(userTag);
                rng = new Random(userId + id);
                Comments.Add(
                    new CommentViewModel
                    {
                        Username = jsonArray[rng.Next(0, jsonArray.Length)],
                        BodyText = commentReader.GetString("text"),
                        Id = commentReader.GetInt32("idcomment"),
                        PostId = commentReader.GetInt32("postID")
                    }
                );
            }


            return Page();
        }
    }
}
