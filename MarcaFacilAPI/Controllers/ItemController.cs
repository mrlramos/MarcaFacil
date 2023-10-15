using MarcaFacilAPI.DataAccess;
using MarcaFacilAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        public ItemController(ItemRepository ItemRepository,
            PlaceRepository PlaceRepository,
            IConfiguration configuration,
            ILogger<ItemController> logger)
        {
            _itemRepository = ItemRepository;
            _placeRepository = PlaceRepository;
            _configuration = configuration;
            _logger = logger;
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

        [HttpPost]
        public IActionResult Create([FromBody] Item item)
        {
            try
            {
                _logger.LogInformation($"Start {ControllerContext.ActionDescriptor.ActionName} in " +
                    $"{ControllerContext.ActionDescriptor.ControllerName}");

                var placeExists = _placeRepository.GetPlaceById(item.PlaceId);
                if (placeExists == null)
                {
                    _logger.LogInformation($"Place not found to create a item");
                    return BadRequest(new { mensagem = "Lugar não encontrado para criação do item" });
                }

                item.Id = Guid.NewGuid();
                item.CreationDate = DateTime.Now;

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
