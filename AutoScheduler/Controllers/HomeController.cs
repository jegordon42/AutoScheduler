using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoScheduler.Models;
using System.Data.Entity;

namespace AutoScheduler.Controllers
{
    public class HomeController : Controller
    {
        private AutoSchedulerDBEntities db = new AutoSchedulerDBEntities();

        public ActionResult Index()
        {

            User testUser = db.Users.Find(1);

            return View(testUser);
        }

        public ActionResult Calendar()
        {
            //int userId = Int32.Parse(Session["userId"].ToString());
            int userId = 4;
            User user = db.Users.Find(userId);
            string eventJsonString = "[";
            foreach(Task task in user.Tasks)
            {
                foreach(CalendarEvent calEvent in task.CalendarEvents)
                {
                    eventJsonString += "{ \"text\": \" " + task.name + "\"";
                    eventJsonString += ", \"start_date\": \"" + calEvent.startDate.Value.ToString("yyyy-MM-dd HH:mm") + "\"";
                    eventJsonString += ", \"end_date\": \"" + calEvent.endDate.Value.ToString("yyyy-MM-dd HH:mm") + "\" }";
                }
                eventJsonString += ",";
            }
            eventJsonString = eventJsonString.Substring(0, eventJsonString.Length - 1);
            eventJsonString += "]";

            ViewBag.calEvents = eventJsonString;

            return View();
        }
        public string SaveTask(CalTask e)
        {
            Task task = new Task();
            task.name = e.Name;
            task.userID = 4;
            task.isAutoSchedule = true;
            task.dueDate = Convert.ToDateTime(e.DueDate);
            task.startDate = Convert.ToDateTime(e.StartDate);
            task.priority = Int32.Parse(e.Priority);
            task.estCompletionMinutes = Int32.Parse(e.ETA);
            db.Tasks.Add(task);
            db.Entry(task).State = EntityState.Added;
            db.SaveChanges();

            return "";
        }

        public string SaveEvent(CalEvent e)
        {
            CalendarEvent calEvent = db.CalendarEvents.Find(Int64.Parse(e.eventID));
            if (calEvent == null)
            {
                Task task = new Task();
                task.name = e.text;
                task.userID = 4;
                task.isAutoSchedule = false;
                db.Tasks.Add(task);
                db.Entry(task).State = EntityState.Added;
                db.SaveChanges();

                calEvent = new CalendarEvent();
                calEvent.taskID = task.taskID;
                calEvent.startDate = Convert.ToDateTime(e.start_date);
                calEvent.endDate = Convert.ToDateTime(e.end_date);
                db.CalendarEvents.Add(calEvent);
                db.Entry(calEvent).State = EntityState.Added;
                db.SaveChanges();


            }
            return "";
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

public class CalEvent
{
    public string eventID { get; set; }
    public string start_date { get; set; }
    public string end_date { get; set; }
    public string text { get; set; }
}
public class CalTask
{
    public string Name { get; set; }
    public string StartDate { get; set; }
    public string DueDate { get; set; }
    public string Priority { get; set; }
    public string ETA { get; set; }
}
