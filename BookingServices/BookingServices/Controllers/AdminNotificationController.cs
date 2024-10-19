using BookingServices.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookingServices.Controllers
{
    public class AdminNotificationController : Controller
    {
        ApplicationDbContext context;
        public AdminNotificationController(ApplicationDbContext _context)
        {
            context = _context;
        }
        public  IActionResult GetAllNotifications()
        {
            var notifications = context.NotificationAdmins.OrderByDescending(n=>n.Id).ToList();

            return Json(notifications);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await context.NotificationAdmins.FindAsync(id);
            if (notification != null)
            {
                context.NotificationAdmins.Remove(notification);
                await context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }
        [HttpPost]
        public IActionResult DeleteAllNotifications()
        {
            try
            {
                var notifications = context.NotificationAdmins.ToList();
                if (notifications.Any())
                {
                    context.NotificationAdmins.RemoveRange(notifications);
                    context.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        
        }
        [HttpPost]
        public IActionResult AddNotification(NotificationAdmin notification)
        {
            try
            {
                context.NotificationAdmins.Add(notification);
                context.SaveChanges();

                var notificationList = context.NotificationAdmins.OrderByDescending(n => n.Id).ToList();
                var length = notificationList.Count();

                return Json(new { success = true, notifications = notificationList, length });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
