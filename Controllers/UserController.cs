using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Abtest.Data;
using Abtest.Models;


namespace Abtest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase {
        private DataContext _context = null;
        public UsersController(DataContext context) {
            _context = context;
        }
        
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<User>> GetUsers() {
            
            ActionResult response;

            if (!_context.Database.CanConnect()) { // Проверка на доступность дб
                response = Content("No connection to database.");
            } else {
                var usersList = _context.Users.ToList();

                if (usersList.Count == 0) { // Проверка на наличие записей в таблице
                    response = NotFound("There are no records yet");
                } else {
                    response = Ok(usersList.OrderBy(u => u.Id));
                }
            }

            return response;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id) {

            ActionResult response;
            
            if(!_context.Database.CanConnect()) { // Проверка на доступность дб
                response = Content("No connection to database.");
            } else {
                var user = await _context.Users.FindAsync(id);

                if (user == null) { // Проверка на наличие искомой записи
                    response = NotFound("User does not exist");
                } else {
                    response = Ok(user);
                }
            }

            return response;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user) {
            
            ActionResult response;

            if (!_context.Database.CanConnect()) { // Проверка на доступность дб 
                response = Content("No connection to database.");
            } else {
                var usersList = _context.Users.ToList();
                bool alreadyExists = false;
    
                if (usersList.Count > 0) { // Проверка на наличие записей в таблице
                    foreach (User u in usersList) {
                        if (user.Id == u.Id) alreadyExists = true;
                    }
                }
                
                if (!alreadyExists) { // проверка на наличие пользователя с указанным id
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    response = Created("User successfully created.", CreatedAtAction(nameof(GetUser), new { id = user.Id }, user));
                } else {
                    response = Conflict("User with such ID already exists.");
                }
            }
        
            return response;
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id) {

            ActionResult response;

            if (!_context.Database.CanConnect()) { // Проверка на доступность дб
                response = Content("No connection to database.");
            } else {
                var user = await _context.Users.FindAsync(id);

                if (user == null) { // Проверка на наличие искомого пользователя
                    response = NotFound("The user does not exist");
                } else {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    response = Ok("User successfully deleted");
                }
            }

            return response;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(long id, User user) {

            ActionResult response;

            if (id != user.Id) {
                response = BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                response = Ok("User was successfully modified.");
            }
            catch (DbUpdateConcurrencyException) {
                if (!_context.Users.Any(u => u.Id == id)) {
                    response = NotFound("Requested user does not exist.");
                }
                else {
                    throw;
                }
            }

            return response;
        }
    }
}