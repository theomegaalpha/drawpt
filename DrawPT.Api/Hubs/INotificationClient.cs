using DrawPT.Common.Models.Daily;

namespace DrawPT.Api.Hubs
{
    public interface INotificationClient
    {
        Task NewDailyAnswer(DailyAnswerPublic dailyAnswer);
    }
}
