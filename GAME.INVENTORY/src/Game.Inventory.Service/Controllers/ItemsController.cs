using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Common;
using Game.Inventory.Service.Clients;
using Game.Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Game.Inventory.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemRepository;
        private readonly CatalogClient catalogClient;
        public ItemsController(IRepository<InventoryItem> itemRepository, CatalogClient catalogClient)
        {
            this.itemRepository = itemRepository;
            this.catalogClient = catalogClient;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCatalogItemsAsync();

            var inventoryItemEntities = await itemRepository.GetAllAsync(x => x.UserId == userId);
            var inventoryItemDtos = inventoryItemEntities.Select(i =>
            {
                var catalogItem = catalogItems?.FirstOrDefault(x => x.Id == i.CatalogItemId);
                return i.AsDto(catalogItem?.Name, catalogItem?.Description);
            });
            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto grantItemDto)
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