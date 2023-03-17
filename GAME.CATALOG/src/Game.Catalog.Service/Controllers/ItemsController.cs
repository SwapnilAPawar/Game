using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Catalog.Service.Dtos;
using Game.Catalog.Service.Entities;
using Game.Catalog.Service.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Game.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository itemRepository;

        public ItemsController(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var items = (await itemRepository.GetAllItemsAsync()).Select(x => x.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id)
        {
            var item = await itemRepository.GetItemAsync(id);
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
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(Guid id, UpdateDto updateDto)
        {
            var existingItem = (await itemRepository.GetItemAsync(id));
            if (existingItem == null)
            {
                return NotFound("Record not found. Please verify if payload/parameters are correct.");
            }
            existingItem.Name = updateDto.Name;
            existingItem.Description = updateDto.Description;
            existingItem.Price = updateDto.Price;

            await itemRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = (await itemRepository.GetItemAsync(id));
            if (existingItem == null)
            {
                return NotFound("Record not found. Please verify if payload/parameters are correct.");
            }
            await itemRepository.RemoveAsync(existingItem);
            return NoContent();
        }
    }
}