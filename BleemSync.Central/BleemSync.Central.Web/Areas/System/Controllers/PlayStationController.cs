﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BleemSync.Central.Data.Models.PlayStation;
using BleemSync.Central.Services.Systems;
using Microsoft.AspNetCore.Mvc;

namespace BleemSync.Central.Web.Areas.System.Controllers
{
    [Area("System")]
    [Route("[area]/[controller]/[action]")]
    public class PlayStationController : Controller
    {
        public readonly PlayStationService _service;

        public PlayStationController(PlayStationService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var game = _service.GetGame(id);

            return View(game);
        }

        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            var game = _service.GetGame(id);

            return View(game);
        }

        [HttpPost("{id}")]
        public IActionResult Edit(Game game)
        {
            _service.ReviseGameAsync(game);

            return RedirectToAction("Index");
        }

        public IActionResult Moderation()
        {
            var games = _service.GetGameRevisions(gr => gr.ApprovedBy == null).ToList();

            return View(games);
        }

        public IActionResult DataTable()
        {
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);

            var games = _service.GetGames(start, length).ToList();

            int filteredCount = games.Count;

            List<string[]> records = new List<string[]>();

            foreach (var game in games)
            {
                records.Add(new string[]
                {
                    $"<a href=\"{Url.Action("Details", new { Id = game.Id })}\">{game.Title}</a>"
                });
            }

            dynamic result = new
            {
                draw = Request.Form["draw"],
                recordsTotal = filteredCount,
                recordsFiltered = filteredCount,
                data = records
            };

            return Json(result);
        }
    }
}