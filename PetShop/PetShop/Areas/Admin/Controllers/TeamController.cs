using Bussiness.Abstracts;
using Bussiness.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class TeamController : Controller
    {
        ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public IActionResult Index(string? name)
        {
           var teams= _teamService.GetAllTeams();
            return View(teams);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Team team)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                _teamService.Create(team);
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
            catch (NotNullException ex)
            {
                return NotFound();

            }
            catch (FileContentTypeException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                return View();

            }
            catch (FileSizeException ex)
            {

                ModelState.AddModelError(ex.Property, ex.Message);
                return View();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            try
            {
                _teamService.Delete(id);
            }
            catch (NotFoundException)
            {

                return NotFound();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            var team=_teamService.GetTeam(x=>x.Id == id);
            if(team==null) return NotFound();
            return View(team);
        }
        [HttpPost]
        public IActionResult Update(Team team)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                _teamService.Update(team.Id, team);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (NotNullException)
            {
                return NotFound();
            }
            catch(FileContentTypeException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                return View();
            }
            catch (FileSizeException ex)
            {

                ModelState.AddModelError(ex.Property, ex.Message);
                return View();
            }
          
            return RedirectToAction("Index");
        }
    }
}
