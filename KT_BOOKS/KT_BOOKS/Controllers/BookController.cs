using KT_BOOKS.Models;
using KT_BOOKS.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace KT_BOOKS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly MyDbContext _context;

        public BookController(MyDbContext context)
        {
            _context = context;
        }
        // GET: api/Book
        [HttpGet]
       // [Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> GetBooks()
        {
			try
			{
				var book = await _context.Books.Include(b => b.AuthorModel).ToListAsync();
				return Ok(new { message = "Lấy sản phẩm thành công => ", data = book });
			}
			catch (UnauthorizedAccessException)
			{
				return Unauthorized(new { message = "Bạn không có quyền truy cập." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy sản phẩm.", error = ex.Message });
			}
		}

        // GET: api/Book/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> findBookById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = "Không tìm thấy sách có id = " + id });
            }
            return Ok(new { message = "Tìm thấy sách =>", data = book });
        }
        // GET: api/Book/author/{authorName}
        [HttpGet("author/{authorName}")]
        [Authorize(Policy = "ADMIN")]
        public ActionResult<IEnumerable<BookModel>> GetBooksByAuthorName(string authorName)
        {
            var booksByAuthor = _context.Books.Where(b => _context.Authors.Any(a => a.Id == b.AuthorId && a.Name.Equals(authorName, StringComparison.OrdinalIgnoreCase))).ToList();
            if (!booksByAuthor.Any())
                return NotFound("Không tìm thấy sách của tác giả có tên: " + authorName);
            return Ok(new { message = "Tìm sách theo tên của tác giả =>", data = booksByAuthor });
        }

        // POST: api/Book
        [HttpPost]
        [Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> CreateBook([FromBody] BookModel newBook)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState); // Return validation errors

                if (newBook.PublishedYear < 1800 || newBook.PublishedYear > DateTime.Now.Year)
                    return BadRequest("Năm xuất bản phải nằm trong khoảng 1800 hoặc nhỏ hơn hoặc bằng năm hiện tại!");

                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Thêm sách thành công", data = newBook });
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message;
                return StatusCode(500, new { message = "Thêm sách thất bại", error = innerException });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Thêm sách thất bại", error = ex.Message });
            }
        }


        // PUT: api/Book/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookModel updatedBook)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatedBook.PublishedYear < 1800 || updatedBook.PublishedYear > DateTime.Now.Year)
                return BadRequest("Năm xuất bản phải nằm trong khoảng 1800 hoặc nhỏ hơn hoặc bằng năm hiện tại!");

            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
                if (book == null)
                    return NotFound($"Không tìm thấy sách có chứa id = {id} để sửa");

                // Cập nhật các thuộc tính
                book.Title = updatedBook.Title;
                book.AuthorId = updatedBook.AuthorId;
                book.PublishedYear = updatedBook.PublishedYear;
                book.Genre = updatedBook.Genre;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Sửa sách thành công", data = book });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Lỗi cạnh tranh! Dữ liệu đã bị thay đổi bởi người dùng khác." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Sửa sách thất bại", error = ex.Message });
            }
        }


        // DELETE: api/Book/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "ADMIN")]
        public ActionResult DeleteBook(int id)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                    return NotFound("Không tìm thấy sách có id = " + id + " để xóa");

                _context.Books.Remove(book);
                return Ok(new { message = "Xóa sách thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
