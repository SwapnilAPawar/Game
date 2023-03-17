using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Catalog.Service.Dtos;
using Game.Catalog.Service.Entities;

namespace Game.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(
                item.Id,
                Convert.ToString(item.Name ?? string.Empty),
                Convert.ToString(item.Description ?? string.Empty),
                item.Price,
                item.CreatedOn);
        }
    }
}