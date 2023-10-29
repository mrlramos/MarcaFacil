using Amazon.S3;
using Amazon.S3.Model;
using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IAmazonS3 _amazonS3;

        public PlaceController(PlaceRepository PlaceRepository,
            UserRepository UserRepository,
            IConfiguration configuration,
            ILogger<PlaceController> logger,
            IAmazonS3 amazonS3)
        {
            _placeRepository = PlaceRepository;
            _userRepository = UserRepository;
            _configuration = configuration;
            _logger = logger;
            _amazonS3 = amazonS3;
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
        public async Task<IActionResult> Create(Guid id, [FromForm] Place place)
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
                place.CreationDate = DateTime.Now;
                place.ImagePath = place.Id;
                place.UserId = id;
                place.Code = Guid.NewGuid();

                var retornoInsercaoBucket = await UploadFileAsync(
                    _amazonS3,
                    _configuration.GetSection("Amazon").GetSection("Bucket")["BucketName"],
                    _configuration.GetSection("Amazon").GetSection("Bucket")["BucketKeyPlace"],
                    place.Id.ToString() + ".png",
                    place.Image);

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

        public static async Task<bool> UploadFileAsync(
            IAmazonS3 client,
            string bucketName,
            string folderName,
            string objectName,
            IFormFile file)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"{folderName}/{objectName}",
                InputStream = file.OpenReadStream(),
            };


            var response = await client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
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
