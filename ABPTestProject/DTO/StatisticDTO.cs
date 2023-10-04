using ABPTestProject.CustomModels;

namespace ABPTestProject.DTO
{
    public class StatisticDTO
    {
        public int SumuryCount { get; set; }

        public List<UserButtonColorsCountModel> ButtonColors { get; set; }

        public List<UserPricesCountModel> Prices { get; set; }
    }
}
