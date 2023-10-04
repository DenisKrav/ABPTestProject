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

        // Ендпоінт для експеременту з кольором кнопки
        [HttpGet("button_color/{device_token}")]
        public async Task<string> GetButtonColor(string device_token)
        {
            await CheckToken(device_token);

            return JsonSerializer.Serialize(new ButtonColorExperimentDTO {
                Value = db.SiteVisitors.FirstOrDefault(a => a.DeviceToken == device_token)?.ButtonColor
            });
        }

        // Ендпоінт для експеременту з цінами
        [HttpGet("price/{device_token}")]
        public async Task<string> GetPrice(string device_token)
        {
            await CheckToken(device_token);

            return JsonSerializer.Serialize(new PriceExperimentDTO {
                Value = db.SiteVisitors.FirstOrDefault(a => a.DeviceToken == device_token)?.Price 
            });
        }

        // Ендпоінт для статистики
        [HttpGet("statistic")]
        public string GetStatistic()
        {
            StatisticDTO statistic = new StatisticDTO();

            statistic.SumuryCount = db.SiteVisitors.Count();
            statistic.Prices = db.SiteVisitors
                .GroupBy(a => a.Price)
                .Select(group => new UserPricesCountModel
                {
                    Price = group.Key,
                    SpecificPriceCount = group.Count()
                })
                .ToList();
            statistic.ButtonColors = db.SiteVisitors
                .GroupBy(a => a.ButtonColor)
                .Select(group => new UserButtonColorsCountModel
                {
                    Color = group.Key,
                    SpecificColorCount = group.Count()
                })
                .ToList();

            return JsonSerializer.Serialize(statistic);
        }

        // Метод, який повертає колір, але враховує можливість перебоїв та пауз експеремента, тобто метод аналізує бд, 
        // перед тим як повернути колір
        private string GetColor()
        {
            string color;

            // Підрахунок кольорів кнопок у бд
            List<UserButtonColorsCountModel> buttonColorCounts = db.SiteVisitors
                .GroupBy(a => a.ButtonColor)
                .Select(group => new UserButtonColorsCountModel
                {
                    Color = group.Key,
                    SpecificColorCount = group.Count()
                })
                .ToList();

            if (buttonColorCounts.Count() < 3)
            {
                color = buttonColorsVariants[buttonColorCounts.Count()];
                return color;
            }
            else
            {
                // Вирахування, який колір присвоїти новому користувачу
                color = buttonColorCounts[0].Color;

                for (int i = 1; i < buttonColorCounts.Count(); i++)
                {
                    if (buttonColorCounts[i - 1].SpecificColorCount > buttonColorCounts[i].SpecificColorCount)
                    {
                        color = buttonColorCounts[i].Color;
                    }
                }

                return color;
            }
        }
        // Метод, який поветає колір кнопки для учасників, але за усовою, якщо експеремент почався і продовжується без пауз 
        // та перерв
        //private string GetColor(int numberExperimentsUser)
        //{
        //    return buttonColorsVariants[numberExperimentsUser % 3];
        //}

        // Метод для отримання ціни, яку буде показано користувачу
        private decimal GetPrice()
        {
            int totalUserCount = db.SiteVisitors .Count();

            List<UserPricesCountModel> priceCounts = db.SiteVisitors
                .GroupBy(a => a.Price)
                .Select(group => new UserPricesCountModel
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
        private double GetProcent(List<UserPricesCountModel> pricesCounts, int totalCount, int priceForCulculate)
        {
            int countOfPrice = pricesCounts.FirstOrDefault(item => item.Price == priceForCulculate)?.SpecificPriceCount ?? 0;

            return (double)countOfPrice / totalCount;
        }

        // Метод перевірки токена
        private async Task CheckToken(string device_token)
        {
            if (db.SiteVisitors.Any(a => a.DeviceToken == device_token))
            {
                return;
            }
            else
            {
                db.SiteVisitors.Add(new SiteVisitor
                {
                    DeviceToken = device_token,
                    ButtonColor = GetColor(/*db.SiteVisitors.Count()*/),
                    Price  = GetPrice()
                });

                await db.SaveChangesAsync();
            }
        }

    }
}