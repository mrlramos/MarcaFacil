using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarcaFacilAPI.Controllers
{
    [Route("api/place")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly PlaceRepository _placeRepository;
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PlaceController> _logger;

        public PlaceController(PlaceRepository PlaceRepository,
            UserRepository UserRepository,
            IConfiguration configuration,
            ILogger<PlaceController> logger)
        {
            _placeRepository = PlaceRepository;
            _userRepository = UserRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IList<Place>> GetPlaces()
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                var places = _placeRepository.GetPlaces();
                if (places == null || !places.Any())
                {
                    _logger.LogInformation($"Places not found");
                    return NotFound(new { mensagem = "Nenhum lugar encontrado" });
                }

                _logger.LogInformation($"Returning places");

                return Ok(places);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpPost("{id}")]
        public IActionResult Create(Guid id, [FromBody] Place place)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                var userExists = _userRepository.GetUserById(id);
                if (userExists == null)
                {
                    _logger.LogInformation($"User not found to create a place");
                    return BadRequest(new { mensagem = "Usuário não encontrado para criação do lugar" });
                }

                place.Id = Guid.NewGuid();
                place.Code = Guid.NewGuid();
                place.CreationDate = DateTime.Now;

                _placeRepository.PostPlace(place);

                _logger.LogInformation($"Place {place.Id} created successfully");

                return Created("", new { id = place.Id, mensagem = $"Lugar criado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Place place)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                Place placeToUpdate = _placeRepository.GetPlaceById(id);

                if (placeToUpdate == null)
                {
                    _logger.LogInformation($"Place {id} not found");
                    return NotFound(new { id = id, mensagem = $"Lugar não encontrado." });
                }

                placeToUpdate.Name = place.Name;
                placeToUpdate.Description = place.Description;
                _placeRepository.PutPlace(placeToUpdate);

                _logger.LogInformation($"Place {id} updated sucessfully");

                return Ok(new { id = id, mensagem = $"Lugar atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "doador")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                Place placeToDelete = _placeRepository.GetPlaceById(id);

                if (placeToDelete == null)
                {
                    _logger.LogInformation($"Place {id} not found");
                    return NotFound(new { id = id, mensagem = $"Lugar não encontrado." });
                }

                _placeRepository.DeletePlace(placeToDelete);

                _logger.LogInformation($"Place {id} deleted sucessfully");

                return Ok(new { id = placeToDelete.Id, mensagem = $"Lugar excluído com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        //[HttpGet("{page}")]
        //public ActionResult<IEnumerable<Place>> ListarTodosPaginados([FromRoute] int page, int sizePage = 10)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
        //            $"{ControllerContext.ActionDescriptor.ControllerName}");

        //        var places = _placeRepository.GetPlacesByPage(page, sizePage);

        //        if (places.Count() == 0)
        //        {
        //            _logger.LogInformation($"Places not found");
        //            return NotFound(new { mensagem = "Não há lugares a serem listados." });
        //        }

        //        _logger.LogInformation($"Returning places");

        //        return Ok(places);
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(500, new { mensagem = $"{ex.Message}" });
        //    }
        //}
    }
}
