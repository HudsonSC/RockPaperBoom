using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Web.ViewModels;

namespace Web.Controllers
{
    public class PlayersController : Controller
    {
        private IMemoryCache _cache;
        private const string _cacheKey = "playerList";
        private List<Player> _playerList = new List<Player>(); 

        public PlayersController(IMemoryCache cache)
        {
            _cache = cache;
            if (!_cache.TryGetValue(_cacheKey, out _playerList))
            {
                _playerList = new List<Player>();
            }
        }
        public IActionResult Index()
        {
            return View(_playerList.Select(p => new PlayerViewModel() {Name = p.Name, BotUrl = p.BotUrl}));
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new PlayerViewModel());
        }

        [HttpPost]
        public IActionResult Add(PlayerViewModel model)
        {
            var player = new Player() {Name = model.Name, BotUrl = model.BotUrl};
            _playerList.Add(player);
            _cache.Set(_cacheKey, _playerList, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1)));

            return RedirectToAction("Index");
        }

    }

    public class Player
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public string BotUrl { get; set; }
    }
}
