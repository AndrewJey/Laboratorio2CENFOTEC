using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBAPI.Models;
namespace CosmosDBAPI.Controllers
{
    public class ItemController : Controller
    {
        private readonly ICosmosDBService _cosmosDbService;
        public ItemController(ICosmosDBService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<List<Item>> Index()
        {
            try
            {
                return (await _cosmosDbService.GetItemsAsync("SELECT * FROM c")).ToList(); ;
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        [Route("CreateMine")]
        public async Task Create(Item item)
        {
            try
            {
                await _cosmosDbService.AddItemAsync(item);
            }
            catch { throw; }
        }
        [HttpPost]
        [Route("Create")]
        public async Task CreateAsync([Bind("Id,Name,Description,Completed")] Item item)
        {
            try
            {
                item.Id = Guid.NewGuid().ToString();
                await _cosmosDbService.AddItemAsync(item);
            }
            catch { throw; }
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task Delete([Bind("Id,Name,Description,Completed")] Item item)
        {
            try
            {
                await _cosmosDbService.DeleteItemAsync(item.Id);
            }
            catch { throw;  }
        }
        [HttpPost] [Route("Edit")]
        public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Item item = await _cosmosDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Item item = await _cosmosDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }
        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetItemAsync(id));
        }
    }
}