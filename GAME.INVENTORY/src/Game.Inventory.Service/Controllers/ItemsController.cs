using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Common;
using Game.Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Game.Inventory.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemRepository;
        public ItemsController(IRepository<InventoryItem> itemRepository)
        {
            this.itemRepository = itemRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var items = (await itemRepository.GetAllAsync(x => x.UserId == userId)).Select(x => x.AsDto());
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> POstAsync(GrantItemDto grantItemDto)
        {
            var inventoryItem = await itemRepository.GetAsync(x => x.UserId == grantItemDto.UserId && x.CatalogItemId == grantItemDto.CatalogItemId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemDto.CatalogItemId,
                    UserId = grantItemDto.UserId,
                    Quantity = grantItemDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await itemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemDto.Quantity;
                await itemRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}