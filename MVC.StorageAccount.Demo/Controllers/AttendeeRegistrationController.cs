using Azure;
using Microsoft.AspNetCore.Mvc;
using MVC.StorageAccount.Demo.Data;
using MVC.StorageAccount.Demo.Services;

namespace MVC.StorageAccount.Demo.Controllers
{
    public class AttendeeRegistrationController : Controller
    {
        private readonly ITableStorageService _storageService;
        public AttendeeRegistrationController(ITableStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<ActionResult> Index()
        {
            var attendees = await _storageService.GetAttendeesAsync();
            return View(attendees);
        }

        // GET: AttendeeRegistrationController/Details/5
        public async Task<ActionResult> Details(string id, string industry)
        {
            var attendee = await _storageService.GetAttendeeAsync(industry, id);
            return View(attendee);
        }

        // GET: AttendeeRegistrationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeRegistrationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AttendeeEntity entity)
        {
            try
            {
               
                entity.PartitionKey = entity.Industry;
                entity.RowKey = Guid.NewGuid().ToString();
                entity.ETag = new ETag($"{entity.RowKey}_{DateTime.UtcNow}_{entity.PartitionKey}");
                await _storageService.UpsertAttendeeAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeRegistrationController/Edit/5
        public async Task<ActionResult> Edit(string id, string industry)
        {
            var attendee = await _storageService.GetAttendeeAsync(industry, id);
            return View(attendee);
        }

        // POST: AttendeeRegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, AttendeeEntity attendeeEntity)
        {
            try
            {
                attendeeEntity.PartitionKey = attendeeEntity.Industry;
                await _storageService.UpsertAttendeeAsync(attendeeEntity);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: AttendeeRegistrationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, string industry)
        {
            try
            { 
                await _storageService.DeleteAttendeeAsync(industry, id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
