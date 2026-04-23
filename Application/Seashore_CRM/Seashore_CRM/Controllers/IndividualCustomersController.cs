using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.BLL.DTOs;
using System.Threading.Tasks;
using System.Linq;
using Seashore_CRM.ViewModels.IndividualCustomer;

namespace Seashore_CRM.Controllers
{
    public class IndividualCustomersController : Controller
    {
        private readonly IIndividualCustomerService _service;

        public IndividualCustomersController(IIndividualCustomerService service)
        {
            _service = service;
        }

        // GET: IndividualCustomers
        public IActionResult Index()
        {
            var items = _service.GetAll()
                .Select(c => new IndividualCustomerListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Phone = c.Phone,
                    Email = c.Email
                }).ToList();

            return View(items);
        }

        // GET: Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new IndividualCustomerDetailsViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Location = item.Location,
                Phone = item.Phone,
                Email = item.Email
            };

            return View(vm);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new IndividualCustomerCreateViewModel { Name = string.Empty, Phone = string.Empty });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IndividualCustomerCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new IndividualCustomerCreateDto
            {
                Name = model.Name,
                Location = model.Location,
                Phone = model.Phone,
                Email = model.Email
            };

            await _service.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new IndividualCustomerUpdateViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Location = item.Location,
                Phone = item.Phone,
                Email = item.Email
            };
            return View(vm);
        }

        // POST: Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IndividualCustomerUpdateViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var dto = new IndividualCustomerUpdateDto
            {
                Id = model.Id,
                Name = model.Name,
                Location = model.Location,
                Phone = model.Phone,
                Email = model.Email
            };

            await _service.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new IndividualCustomerDetailsViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Location = item.Location,
                Phone = item.Phone,
                Email = item.Email
            };

            return View(vm);
        }

        // POST: Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
