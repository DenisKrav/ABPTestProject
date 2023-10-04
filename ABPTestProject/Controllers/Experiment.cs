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
        // �����, ���� ������ ������� ������, ��������������� ��� ������ ������������ ������ GetButtonColor, ���
        // ������ �������, �� ���� ������� ������, �� ������������, �� ����� ������, �� �� ����, � ��� ����� GetColor
        // ������� ������������ ���������.
        // ���� ��������, �� ������� �������� ��� �������������� ����������� �����, � �������� � ��'������, � ���� 
        // ������� ���� �������� ��� ��� ��'����
        private static string[] buttonColorsVariants = new string[] { "#FF0000", "#00FF00", "#0000FF" };
        private readonly ILogger<Experiment> _logger;
        AbptestTaskDbContext db;

        public Experiment(ILogger<Experiment> logger, AbptestTaskDbContext context)
        {
            _logger = logger;
            db = context;
        }

        // ������� ��� ������������ � �������� ������
        [HttpGet("button_color/{device_token}")]
        public async Task<string> GetButtonColor(string device_token)
        {
            await CheckToken(device_token);

            return JsonSerializer.Serialize(new ButtonColorExperimentDTO {
                Value = db.SiteVisitors.FirstOrDefault(a => a.DeviceToken == device_token)?.ButtonColor
            });
        }

        // ������� ��� ������������ � ������
        [HttpGet("price/{device_token}")]
        public async Task<string> GetPrice(string device_token)
        {
            await CheckToken(device_token);

            return JsonSerializer.Serialize(new PriceExperimentDTO {
                Value = db.SiteVisitors.FirstOrDefault(a => a.DeviceToken == device_token)?.Price 
            });
        }

        // ������� ��� ����������
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

        // �����, ���� ������� ����, ��� ������� ��������� ������� �� ���� ������������, ����� ����� ������ ��, 
        // ����� ��� �� ��������� ����
        private string GetColor()
        {
            string color;

            // ϳ�������� ������� ������ � ��
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
                // �����������, ���� ���� �������� ������ �����������
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
        // �����, ���� ������ ���� ������ ��� ��������, ��� �� ������, ���� ����������� ������� � ������������ ��� ���� 
        // �� ������
        //private string GetColor(int numberExperimentsUser)
        //{
        //    return buttonColorsVariants[numberExperimentsUser % 3];
        //}

        // ����� ��� ��������� ����, ��� ���� �������� �����������
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

        // ����� ��� ������������� �������� ��� ����� ���� �� ��������
        private double GetProcent(List<UserPricesCountModel> pricesCounts, int totalCount, int priceForCulculate)
        {
            int countOfPrice = pricesCounts.FirstOrDefault(item => item.Price == priceForCulculate)?.SpecificPriceCount ?? 0;

            return (double)countOfPrice / totalCount;
        }

        // ����� �������� ������
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