using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Seashore_CRM.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ISaleService _saleService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _uow;

        public InvoicesController(
            IInvoiceService invoiceService,
            ISaleService saleService,
            IPaymentService paymentService,
            IUnitOfWork uow)
        {
            _invoiceService = invoiceService;
            _saleService = saleService;
            _paymentService = paymentService;
            _uow = uow;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _invoiceService.GetAllAsync();
            return View(items.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            var payments = (await _uow.Payments.FindAsync(p => p.InvoiceId == id)).ToList();
            ViewBag.Payments = payments;

            return View(invoice);
        }

        public async Task<IActionResult> CreateFromSale(int saleId)
        {
            var sale = await _saleService.GetByIdAsync(saleId);
            if (sale == null) return NotFound();

            var invoice = new Invoice
            {
                SaleId = sale.Id,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{sale.Id}",
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                TotalAmount = sale.GrossTotal != 0 ? sale.GrossTotal : sale.TotalAmount,
                Status = "Unpaid"
            };

            return View("Create", invoice);
        }

        public IActionResult Create()
        {
            return View(new Invoice());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (!ModelState.IsValid) return View(invoice);

            await _invoiceService.AddAsync(invoice);
            return RedirectToAction(nameof(Details), new { id = invoice.Id });
        }

        public async Task<IActionResult> RecordPayment(int invoiceId)
        {
            var inv = await _invoiceService.GetByIdAsync(invoiceId);
            if (inv == null) return NotFound();
            ViewBag.Invoice = inv;
            var payment = new Payment { InvoiceId = invoiceId, PaymentDate = DateTime.UtcNow };
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Invoice = await _invoiceService.GetByIdAsync(payment.InvoiceId);
                return View(payment);
            }

            await _paymentService.AddAsync(payment);

            // Optionally update invoice status
            var inv = await _invoiceService.GetByIdAsync(payment.InvoiceId);
            if (inv != null)
            {
                inv.Status = "Partially Paid"; // simple approach; real logic would sum payments
                await _invoiceService.UpdateAsync(inv);
            }

            return RedirectToAction(nameof(Details), new { id = payment.InvoiceId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var inv = await _invoiceService.GetByIdAsync(id);
            if (inv == null) return NotFound();
            return View(inv);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _invoiceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
