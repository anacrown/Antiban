using System;
using System.Collections.Generic;
using System.Linq;

namespace Antiban
{
    /// <summary>
    /// период между сообщениями на разные номера, должен быть не менее 10 сек. Для теста возьмем, что равно 10 сек
    /// период между сообщениями на один номер, должен быть не менее 1 минуты. Для теста возьмем, что равно 1 минуте
    /// период между сообщениями с приоритетом=1 на один номер, не менее 24 часа. Т.е. должна отправлять только одна рассылка в день на один номер.
    /// </summary>
    public static class Timings
    {
        public static TimeSpan MinPeriodBetweenMessages = TimeSpan.FromSeconds(10);
        public static TimeSpan MinPeriodBetweenMessagesOnOnePhone = TimeSpan.FromSeconds(60);
        public static TimeSpan MinPeriodBetweenMessagesOnOnePhoneWithPriority1 = TimeSpan.FromHours(24);
    }

    public class Antiban
    {
        private readonly List<AntibanResult> _results = new();
        private readonly RestrictionTimeLine _restrictionAllTimeLine = new();
        private readonly Dictionary<string, RestrictionTimeLineManager> _restrictionPhoneTimeLines = new();

        /// <summary>
        /// Добавление сообщений в систему, для обработки порядка сообщений
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PushEventMessage(EventMessage eventMessage)
        {
            // ограничения на отправку для этого номера
            var restrictionPhoneTimeLineManager = _restrictionPhoneTimeLines
                .GetValueOrDefault(eventMessage.Phone, new RestrictionTimeLineManager(_restrictionAllTimeLine));

            // добавляем ограничения на отправку для этого номера 
            if (!_restrictionPhoneTimeLines.ContainsKey(eventMessage.Phone))
                _restrictionPhoneTimeLines.Add(eventMessage.Phone, restrictionPhoneTimeLineManager);

            // ищем время, удовлетворяющее всем ограничениям
            var sendTime = restrictionPhoneTimeLineManager.FindFreeSpaceAndReserve(eventMessage.Priority, eventMessage.DateTime);

            // добавляем результат в список
            _results.Add(new AntibanResult()
            {
                EventMessageId = eventMessage.Id,
                SentDateTime = sendTime
            });
        }

        /// <summary>
        /// Вовзращает порядок отправок сообщений
        /// </summary>
        /// <returns></returns>
        public List<AntibanResult> GetResult()
        {
            return _results.OrderBy(r => r.SentDateTime).ToList();
        }
    }
}
