using Amazon.S3;
using Amazon.S3.Model;
using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarcaFacilAPI.Controllers
{
    [Route("api/item")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemRepository _itemRepository;
        private readonly PlaceRepository _placeRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ItemController> _logger;
        private readonly IAmazonS3 _amazonS3;

        public ItemController(ItemRepository ItemRepository,
            PlaceRepository PlaceRepository,
            IConfiguration configuration,
            ILogger<ItemController> logger,
            IAmazonS3 amazonS3)
        {
            _itemRepository = ItemRepository;
            _placeRepository = PlaceRepository;
            _configuration = configuration;
            _logger = logger;
            _amazonS3 = amazonS3;
        }

        [HttpGet]
        public ActionResult<IList<Item>> GetItems()
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                var items = _itemRepository.GetItems();
                if (items == null || !items.Any())
                {
                    _logger.LogInformation($"Items not found");
                    return NotFound(new { mensagem = "Nenhum item encontrado" });
                }

                _logger.LogInformation($"Returning items");

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Create(Guid id, [FromForm] Item item)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                var placeExists = _placeRepository.GetPlaceById(id);
                if (placeExists == null)
                {
                    _logger.LogInformation($"Place not found to create a item");
                    return BadRequest(new { mensagem = "Lugar não encontrado para criação do item" });
                }

                item.Id = Guid.NewGuid();
                item.CreationDate = DateTime.Now;
                item.ImagePath = item.Id;
                item.PlaceId = id;

                var retornoInsercaoBucket = await UploadFileAsync(
                    _amazonS3,
                    _configuration.GetSection("Amazon").GetSection("Bucket")["BucketName"],
                    _configuration.GetSection("Amazon").GetSection("Bucket")["BucketKeyItem"],
                    item.Id.ToString() + ".png",
                    item.Image);

                _itemRepository.PostItem(item);

                _logger.LogInformation($"Item {item.Id} created successfully");

                return Created("", new { id = item.Id, mensagem = $"Item criado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { mensagem = $"{ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Item item)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                Item itemToUpdate = _itemRepository.GetItemById(id);

                if (itemToUpdate == null)
                {
                    _logger.LogInformation($"Item {id} not found");
                    return NotFound(new { id = id, mensagem = $"Item não encontrado." });
                }

                itemToUpdate.Name = item.Name;
                itemToUpdate.Amount = item.Amount;
                _itemRepository.PutItem(itemToUpdate);

                _logger.LogInformation($"Item {id} updated sucessfully");

                return Ok(new { id = id, mensagem = $"Item atualizado com sucesso" });
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

                Item itemToDelete = _itemRepository.GetItemById(id);

                if (itemToDelete == null)
                {
                    _logger.LogInformation($"Item {id} not found");
                    return NotFound(new { id = id, mensagem = $"Item não encontrado." });
                }

                _itemRepository.DeleteItem(itemToDelete);

                _logger.LogInformation($"Item {id} deleted sucessfully");

                return Ok(new { id = itemToDelete.Id, mensagem = $"Item excluído com sucesso" });
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
        //public ActionResult<IEnumerable<Item>> ListarTodosPaginados([FromRoute] int page, int sizePage = 10)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
        //            $"{ControllerContext.ActionDescriptor.ControllerName}");

        //        var items = _itemRepository.GetItemsByPage(page, sizePage);

        //        if (items.Count() == 0)
        //        {
        //            _logger.LogInformation($"Items not found");
        //            return NotFound(new { mensagem = "Não há itens a serem listados." });
        //        }

        //        _logger.LogInformation($"Returning items");

        //        return Ok(items);
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(500, new { mensagem = $"{ex.Message}" });
        //    }
        //}

    }
}
