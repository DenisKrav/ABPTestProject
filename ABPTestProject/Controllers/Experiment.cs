using ABPTestProject.CustomModels;
using ABPTestProject.DTO;
using ABPTestProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Text.Json;

namespace ABPTestProject.Controllers
{
    [ApiController]
    [Route("experiment")]
    public class Experiment : ControllerBase
    {
        // Масив, який зберігає кольори кнопок, вокористувується при перших викоистаннях методу GetButtonColor, щоб
        // задати кольори, бо може інувати варіант, що користувачів, які брали участь, ще не було, і тоді метод GetColor
        // видасть неправильний результат.
        // Поле статичне, бо можлива ситуація при багаторазовому використанні класу, з багатьма в об'єктами, а поле 
        // повинно бути однакове для усіх об'єктів
        private static string[] buttonColorsVariants = new string[] { "#FF0000", "#00FF00", "#0000FF" };
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
                    ButtonColor = color = GetColor(db.SiteVisitors.Count()),
                    Price = GetPrice()
                });

                await db.SaveChangesAsync();
            }


            return JsonSerializer.Serialize(new ButtonColorExperimentDTO { Value = color });
        }

        // Метод для перевірки кольору, скільки яких кольорів у бд і повртає кольор,
        // який доцільніше присвоїти новому користувачу, для балансу кольорів.
        //private string GetColor()
        //{
        //    string color;

        //    // Підрахунок кольорів кнопок у бд
        //    var buttonColorCounts = db.SiteVisitors
        //        .GroupBy(a => a.ButtonColor)   
        //        .Select(group => new   
        //        {      
        //            Color = group.Key,    
        //            ColorCount = group.Count()   
        //        })   
        //        .ToList();

        //    // Вирахування, який колір присвоїти новому користувачу
        //    color = buttonColorCounts[0].Color;
        //    for (int i = 1; i < buttonColorCounts.Count(); i++)
        //    {
        //        if (buttonColorCounts[i - 1].ColorCount > buttonColorCounts[i].ColorCount)
        //        {
        //            color = buttonColorCounts[i].Color;
        //        }
        //    }

        //    return color;
        //}

        // Метод, який повертає для перших участників експеременту кольори кнопок
        private string GetColor(int numberExperimentsUser)
        {
            return buttonColorsVariants[numberExperimentsUser % 3];
        }

        // Метод для отримання ціни, яку буде показано користувачу
        private decimal GetPrice()
        {
            decimal price = 0;
            int totalUserCount = db.SiteVisitors .Count();

            List<UserPriceModel> priceCounts = db.SiteVisitors
                .GroupBy(a => a.Price)
                .Select(group => new UserPriceModel
                {
                    Price = group.Key,
                    SpecificPriceCount = group.Count()
                })
                .ToList();

            if (GetProcent(priceCounts, totalUserCount, 10) < 0.75)
            {
                return 10;
            }
            else if(GetProcent(priceCounts, totalUserCount, 20) < 0.1)
            {
                return 20;
            }
            else if (GetProcent(priceCounts, totalUserCount, 50) < 0.05)
            {
                return 50;
            }
            else
            {
                return 5;
            }
        }

        // Метод для вираховування процента для кожної ціни від загальної
        private double GetProcent(List<UserPriceModel> pricesCounts, int totalCount, int priceForCulculate)
        {
            int countOfPrice = pricesCounts.FirstOrDefault(item => item.Price == priceForCulculate)?.SpecificPriceCount ?? 0;

            return (double)countOfPrice / totalCount;
        }
    }
}