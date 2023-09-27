using ABPTestProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace ABPTestProject.Controllers
{
    [ApiController]
    [Route("experiment")]
    public class Experiment : ControllerBase
    {
        private readonly ILogger<Experiment> _logger;
        AbptestTaskDbContext db;

        public Experiment(ILogger<Experiment> logger, AbptestTaskDbContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet("button_color/{device_token}")]
        public async Task<string> GetButtonColor(string device_token)
        {
            string? color;

            if (db.SiteVisitors.Any(a => a.DeviceToken == device_token))
            {
                color = db.SiteVisitors.FirstOrDefault(a => a.DeviceToken == device_token)?.ButtonColor;
            }
            else
            {
                db.SiteVisitors.Add(new SiteVisitor
                {
                    DeviceToken = device_token,
                    ButtonColor = CheckColor(),
                    Price = 10
                });

                await db.SaveChangesAsync();
            }

            return device_token;
        }

        // Метод для перевірки кольору, скільки яких кольорів у бд і повртає кольор,
        // який доцільніше присвоїти новому користувачу, для балансу кольорів.
        private string CheckColor()
        {
            string color;

            // Підрахунок кольорів кнопок у бд
            var buttonColorCounts = db.SiteVisitors
                .GroupBy(a => a.ButtonColor)   
                .Select(group => new   
                {      
                    Color = group.Key,    
                    Count = group.Count()   
                })   
                .ToList();

            // Вирахування, який колір присвоїти новому користувачу
            color = buttonColorCounts[0].Color;
            for (int i = 1; i < buttonColorCounts.Count(); i++)
            {
                if (buttonColorCounts[i - 1].Count > buttonColorCounts[i].Count)
                {
                    color = buttonColorCounts[i].Color;
                }
            }

            return color;
        }
    }
}