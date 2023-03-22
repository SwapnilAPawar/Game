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
        private readonly IRepository<InventoryItem> inventoryItemRepository;
        private readonly IRepository<CatalogItem> catalogItemRepository;

        public ItemsController(IRepository<InventoryItem> inventoryItemRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.inventoryItemRepository = inventoryItemRepository;
            this.catalogItemRepository = catalogItemRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryItemEntities = await inventoryItemRepository.GetAllAsync(x => x.UserId == userId);
            var itemIds = inventoryItemEntities.Select(x => x.CatalogItemId);
            var catalogItemEntities = await catalogItemRepository.GetAllAsync(x => itemIds.Contains(x.Id));
            var inventoryItemDtos = inventoryItemEntities.Select(i =>
            {
                var catalogItem = catalogItemEntities?.FirstOrDefault(x => x.Id == i.CatalogItemId);
                return i.AsDto(catalogItem?.Name, catalogItem?.Description);
            });
            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto grantItemDto)
        {
            var inventoryItem = await inventoryItemRepository.GetAsync(x => x.UserId == grantItemDto.UserId && x.CatalogItemId == grantItemDto.CatalogItemId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemDto.CatalogItemId,
                    UserId = grantItemDto.UserId,
                    Quantity = grantItemDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await inventoryItemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemDto.Quantity;
                await inventoryItemRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}