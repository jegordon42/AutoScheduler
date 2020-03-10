using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoScheduler.Models;
using System.Data.Entity;

namespace AutoScheduler.Controllers
{
    public class SchedulingController : Controller
    {
        private AutoSchedulerDBEntities db = new AutoSchedulerDBEntities();
        public void AddTask(int taskID)
        {
            Task task = db.Tasks.Find(taskID);
            CalendarBlock calBlock = new CalendarBlock(task);
            calBlock.scheduleTask(task);
        }
    }

    public class CalendarDay
    {
        public DateTime? day;
        public double totalMinutes;
        public List<CalendarEvent> calEvents;
    }

    public class CalendarBlock
    {
        private AutoSchedulerDBEntities db = new AutoSchedulerDBEntities();

        int? dailyHours;
        DateTime? dayStart;
        DateTime? dayEnd;
        List<CalendarDay> calendarSlices;
        public CalendarBlock(Task task)
        {
            UserPreference pref = task.User.UserPreference;
            dailyHours = pref.dailyHours;
            dayStart = pref.dayStart;
            dayEnd = pref.dayEnd;
            calendarSlices = new List<CalendarDay>();

            List<Task> tasks = task.User.Tasks.Where(x => x.startDate > task.startDate && x.dueDate < task.dueDate).OrderBy(x => x.startDate).ToList();

            for (DateTime? curDate = task.startDate; curDate <= task.dueDate; curDate = curDate.Value.AddDays(1))
            {
                CalendarDay day = new CalendarDay();
                day.day = curDate;
                day.totalMinutes = 0;
                day.calEvents = new List<CalendarEvent>();
                foreach (Task userTask in tasks)
                    day.calEvents.AddRange(userTask.CalendarEvents.Where(x => x.endDate < curDate));
                foreach (CalendarEvent calEvent in day.calEvents)
                    day.totalMinutes += (calEvent.endDate - calEvent.startDate).Value.TotalMinutes;

                calendarSlices.Add(day);
            }


        }

        public List<CalendarDay> findBestDate()
        {
            return this.calendarSlices.OrderBy(x => x.totalMinutes).ToList();
        }

        public void scheduleTask(Task task)
        {
            List<CalendarDay> bestDaysInOrder = this.findBestDate();

            foreach (CalendarDay calDay in bestDaysInOrder)
            {
                calDay.calEvents = calDay.calEvents.OrderBy(x => x.startDate).ToList();
                int calEventIndex = 0;

                CalendarEvent curEvent = null;
                if(calDay.calEvents.Any())
                    curEvent = calDay.calEvents.ElementAt(calEventIndex); 
                for (DateTime iter = calDay.day.Value.Date + new TimeSpan(dayStart.Value.Hour, dayStart.Value.Minute, 0); iter < this.dayEnd;)
                {
                    if(curEvent == null || iter + new TimeSpan(0, task.estCompletionMinutes ?? 0, 0) <  curEvent.startDate )//meets time slot
                    {//between iter and start of cur event then schedule
                        CalendarEvent newEvent = new CalendarEvent();
                        newEvent.taskID = task.taskID;
                        newEvent.startDate = iter;
                        newEvent.endDate = iter + new TimeSpan(0, task.estCompletionMinutes ?? 0, 0);
                        newEvent.status = "New";

                        db.CalendarEvents.Add(newEvent);
                        db.Entry(newEvent).State = EntityState.Added;
                        db.SaveChanges();

                        return;
                    }
                    else//else go to after cur event and try again
                    {
                        iter.AddMinutes((curEvent.endDate - iter).Value.TotalMinutes);
                        calEventIndex++;
                        curEvent = calDay.calEvents.ElementAt(calEventIndex);
                    }
                }
            }

        }
        
    }
}