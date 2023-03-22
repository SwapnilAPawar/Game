using Game.Catalog.Service.Dtos;
using Game.Catalog.Service.Entities;
using Game.Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Game.Catalog.Contracts;

namespace Game.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemRepository = itemRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var items = (await itemRepository.GetAllAsync()).Select(x => x.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id)
        {
            var item = await itemRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound("Record not found. Please verify if payload/parameters are correct.");
            }
            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateDto createDto)
        {
            var item = new Item
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                CreatedOn = DateTimeOffset.UtcNow,
            };
            await itemRepository.CreateAsync(item);
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(Guid id, UpdateDto updateDto)
        {
            var existingItem = (await itemRepository.GetAsync(id));
            if (existingItem == null)
            {
                return NotFound("Record not found. Please verify if payload/parameters are correct.");
            }
            existingItem.Name = updateDto.Name;
            existingItem.Description = updateDto.Description;
            existingItem.Price = updateDto.Price;

            await itemRepository.UpdateAsync(existingItem);
            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = (await itemRepository.GetAsync(id));
            if (existingItem == null)
            {
                return NotFound("Record not found. Please verify if payload/parameters are correct.");
            }
            await itemRepository.RemoveAsync(existingItem);
            await publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));
            return NoContent();
        }
    }
}