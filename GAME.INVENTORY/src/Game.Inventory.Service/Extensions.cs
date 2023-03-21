using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Inventory.Service.Entities;

namespace Game.Inventory.Service
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item)
        {
            return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
        }
    }
}