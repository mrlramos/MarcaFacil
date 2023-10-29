using Amazon.S3;
using Amazon.S3.Model;
using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarcaFacilAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        private readonly IAmazonS3 _amazonS3;

        public UserController(UserRepository UserRepository,
            IConfiguration configuration,
            ILogger<UserController> logger,
            IAmazonS3 amazonS3)
        {
            _userRepository = UserRepository;
            _configuration = configuration;
            _logger = logger;
            _amazonS3 = amazonS3;
        }

        [HttpGet]
        public ActionResult<IList<User>> GetUsers()
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                var users = _userRepository.GetUsers();
                if (users == null || !users.Any())
                {
                    _logger.LogInformation($"Users not found");
                    return NotFound(new { mensagem = "Nenhum usuário encontrado" });
                }

                _logger.LogInformation($"Returning users");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] User user)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                user.Id = Guid.NewGuid();
                user.CreationDate = DateTime.Now;
                user.ImagePath = user.Id;

                var retornoInsercaoBucket = await UploadFileAsync(
                    _amazonS3,
                    _configuration.GetSection("Amazon").GetSection("Bucket")["BucketName"],
                    _configuration.GetSection("Amazon").GetSection("Bucket")["BucketKeyUser"],
                    user.Id.ToString() + ".png",
                    user.Image);

                var teste = user.Image.ContentType;
                var teste2 = user.Image.FileName;

                if (retornoInsercaoBucket)
                {
                    _userRepository.PostUser(user);
                    _logger.LogInformation($"User {user.Id} created successfully");

                    return Created("", new { id = user.Id, mensagem = $"Usuário criado com sucesso" });
                }

                //AJUSTAR ESSE RETORNO
                return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] User user)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                User userToUpdate = _userRepository.GetUserById(id);

                if (userToUpdate == null)
                {
                    _logger.LogInformation($"User {id} not found");
                    return NotFound(new { id = id, mensagem = $"Usuario não encontrado." });
                }

                userToUpdate.Name = user.Name;
                userToUpdate.Email = user.Email;
                userToUpdate.Password = user.Password;
                _userRepository.PutUser(userToUpdate);

                _logger.LogInformation($"User {id} updated sucessfully");

                return Ok(new { id = id, mensagem = $"Usuário atualizado com sucesso" });
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

                User userToDelete = _userRepository.GetUserById(id);

                if (userToDelete == null)
                {
                    _logger.LogInformation($"User {id} not found");
                    return NotFound(new { id = id, mensagem = $"Usuario não encontrado." });
                }

                _userRepository.DeleteUser(userToDelete);

                _logger.LogInformation($"User {id} deleted sucessfully");

                return Ok(new { id = userToDelete.Id, mensagem = $"Usuário excluído com sucesso" });
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
        //public ActionResult<IEnumerable<User>> GetUsersByPage([FromRoute] int page, int sizePage = 10)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
        //            $"{ControllerContext.ActionDescriptor.ControllerName}");

        //        var users = _userRepository.GetUsersByPage(page, sizePage);

        //        if (users.Count() == 0)
        //        {
        //            _logger.LogInformation($"Users not found");
        //            return NotFound(new { mensagem = "Não há usuários a serem listados." });
        //        }

        //        _logger.LogInformation($"Returning users");

        //        return Ok(users);
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(500, new { mensagem = $"{ex.Message}" });
        //    }
        //}
    }
}
