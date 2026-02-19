using Microsoft.AspNetCore.Mvc;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace Seashore_CRM.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly IUnitOfWork _uow;

        public CompaniesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _uow.Companies.GetAllAsync();
            return View(companies.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        public IActionResult Create()
        {
            return View(new Company());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (!ModelState.IsValid) return View(company);
            await _uow.Companies.AddAsync(company);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id) return BadRequest();
            if (!ModelState.IsValid) return View(company);
            _uow.Companies.Update(company);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company != null)
            {
                _uow.Companies.Remove(company);
                await _uow.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}