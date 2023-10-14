using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarcaFacilAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(UserRepository UserRepository,
            IConfiguration configuration,
            ILogger<UserController> logger)
        {
            _userRepository = UserRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            _logger.LogInformation($"Iniciando {ControllerContext.ActionDescriptor.ActionName} em " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");
            return _userRepository.GetUsers();
        }

        [HttpGet("{page}")]
        public ActionResult<IEnumerable<User>> GetAllUsersByPage([FromRoute] int page, int tamanhoPagina = 10)
        {
            _logger.LogInformation($"Iniciando {ControllerContext.ActionDescriptor.ActionName} em " +
                $"{ControllerContext.ActionDescriptor.ControllerName}");

            var doadores = _userRepository.GetUsersByPage(page, tamanhoPagina);

            if (doadores.Count() == 0)
            {
                return NotFound(new { mensagem = "Não há doadores a serem listados." });
            }

            return Ok(doadores);
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            try
            {
                _logger.LogInformation($"Iniciando {ControllerContext.ActionDescriptor.ActionName} em " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                if (ModelState.IsValid)
                {
                    user.Id = Guid.NewGuid();
                    user.CreationDate = DateTime.Now;

                    _userRepository.PostUser(user);

                    _logger.LogInformation($"Doador {user.Id} criado com sucesso");

                    return Created("", new { id = user.Id, mensagem = $"Doador criado com sucesso" });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = $"{ex.Message}" });
            }

        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] User doador)
        {
            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "doador")]
        public IActionResult Delete(Guid id)
        {
            return StatusCode(500);
        }
    }
}
