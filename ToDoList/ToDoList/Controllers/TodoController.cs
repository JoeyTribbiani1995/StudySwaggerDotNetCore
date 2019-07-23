using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoList.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : Controller
    {
        private readonly ToDoContext _context;

        public TodoController(ToDoContext context)
        {
            _context = context;

            if(_context.ToDoItems.Count() == 0)
            {
                _context.ToDoItems.Add(new ToDoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Get All To Do Items
        /// </summary>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     GET /Todo
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
            return await _context.ToDoItems.ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ToDoItem),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(long id)
        {
            var todoItem = await _context.ToDoItems.FindAsync(id);

            if(todoItem == null)
            {
                return NotFound("Not Found Item" + id);
            }

            return Ok(todoItem);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ToDoItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ToDoItem>> PostTodoItem(ToDoItem item)
        {
            _context.ToDoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetToDoItem), new { id = item.Id }, item);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> PutTodoItem(ToDoItem item)
        {
            var todoItem = await _context.ToDoItems.FindAsync(item.Id);
            if (todoItem == null)
            {
                return BadRequest("Not Found Item" + item.Id);
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.ToDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.ToDoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
