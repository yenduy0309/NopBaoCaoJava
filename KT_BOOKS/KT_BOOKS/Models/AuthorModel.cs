using System.Text.Json.Serialization;

namespace KT_BOOKS.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public ICollection<BookModel>? Books { get; set; }
    }
}
