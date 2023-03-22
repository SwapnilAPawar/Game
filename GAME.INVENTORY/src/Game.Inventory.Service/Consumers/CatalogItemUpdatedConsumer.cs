using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Catalog.Contracts;
using Game.Common;
using Game.Inventory.Service.Entities;
using MassTransit;

namespace Game.Inventory.Service.Consumers
{
    public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> repository;
        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message = context.Message;
            var item = await repository.GetAsync(message.Id);
            if (item != null)
            {
                item.Name = message.Name;
                item.Description = message.Description;
                await repository.UpdateAsync(item);
            }
            else
            {
                item = new CatalogItem
                {
                    Id = message.Id,
                    Name = message.Name,
                    Description = message.Description
                };
                await repository.CreateAsync(item);
            }
        }
    }
}