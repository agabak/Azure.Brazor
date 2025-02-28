using Azure;
using Microsoft.AspNetCore.Mvc;
using MVC.StorageAccount.Demo.Data;
using MVC.StorageAccount.Demo.Services;
using Newtonsoft.Json;

namespace MVC.StorageAccount.Demo.Controllers
{
    public class AttendeeRegistrationController : Controller
    {
        private readonly ITableStorageService _storageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueService _queueService;
        public AttendeeRegistrationController(ITableStorageService storageService, IBlobStorageService blobStorageService, IQueueService queueService)
        {
            _storageService = storageService;
            _blobStorageService = blobStorageService;
            _queueService = queueService;
        }

        public async Task<ActionResult> Index()
        {
            var attendees = await _storageService.GetAttendeesAsync();

            var messages = await _queueService.ReadMessageAsync();

            foreach (var attende in attendees)
            {
                if (attende.ImageName == null) continue;
                attende.ImageName = await _blobStorageService.GetBlobUrlAsync(attende.ImageName);
            }
            return View(attendees);
        }

        // GET: AttendeeRegistrationController/Details/5
        public async Task<ActionResult> Details(string id, string industry)
        {

            var attendee = await _storageService.GetAttendeeAsync(industry, id);
            if (attendee != null &&  attendee.ImageName != null)
            {
                attendee.ImageName = await _blobStorageService.GetBlobUrlAsync(attendee.ImageName);
            }
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
        public async Task<ActionResult> Create(AttendeeEntity entity, IFormFile formFile)
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                entity.PartitionKey = entity.Industry;
                entity.RowKey = id;
                entity.ETag = new ETag($"{entity.RowKey}_{DateTime.UtcNow}_{entity.PartitionKey}");

                if (formFile.Length > 0)
                {
                    entity.ImageName = await _blobStorageService.UploadBlobAsync(formFile, id);

                }
                else
                {
                    entity.ImageName = "default.jpg";
                }
                await _storageService.UpsertAttendeeAsync(entity);

                if (entity.EmailAddress != null)
                {
                    var message = new EmailMessage
                    {
                        EmailAddress = entity.EmailAddress,
                        Timestamp = DateTime.UtcNow,
                        Message = JsonConvert.SerializeObject(entity)
                    };

                    await _queueService.SendMessageAsync(message);
                }

               

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
        public async Task<ActionResult> Edit(string id, AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                attendeeEntity.PartitionKey = attendeeEntity.Industry;
             
                if (formFile?.Length > 0 && attendeeEntity.RowKey != null)
                {
                  attendeeEntity.ImageName = await _blobStorageService.UploadBlobAsync(formFile, attendeeEntity.RowKey);
                }

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
                var attendee = await _storageService.GetAttendeeAsync(industry, id);
       
                if (attendee != null && attendee.ImageName != null)
                {
                    await _blobStorageService.RemoveBlob(attendee.ImageName);
                }
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
