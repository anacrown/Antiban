using System;
using System.Collections.Generic;

namespace Antiban;

public class RestrictionTimeLineManager
{
    private readonly Dictionary<int, RestrictionTimeLine> _restrictionTimeLines;

    public RestrictionTimeLineManager(RestrictionTimeLine restrictionAllTimeLine)
    {
        _restrictionTimeLines = new Dictionary<int, RestrictionTimeLine>()
        {
            { -1, restrictionAllTimeLine }
        };
    }

    public RestrictionTimeLine Get(int key)
    {
        var restrictionTimeLine = _restrictionTimeLines.GetValueOrDefault(key, new RestrictionTimeLine());
        if (!_restrictionTimeLines.ContainsKey(key))
            _restrictionTimeLines.Add(key, restrictionTimeLine);
        return restrictionTimeLine;
    }

    public DateTime FindFreeSpace(int priority, DateTime startTime)
    {
        bool isShifted;
        do
        {
            isShifted = false;
            for (var p = -1; p <= priority; p++)
            {
                var restrictionTimeLine = Get(p);
                var restrictionTime = new RestrictionTime(startTime, GetBusyTimeForPriority(p));
                
                var freeSpace = restrictionTimeLine.FindFreeSpace(restrictionTime);
                if (freeSpace.Start <= restrictionTime.Start) continue;

                startTime = restrictionTime.ShiftTo(freeSpace.Start)
                    .Start;

                isShifted = true;
            }

        } while (isShifted);

        return startTime;
    }

    public DateTime FindFreeSpaceAndReserve(int priority, DateTime startTime)
    {
        var sentDateTime = FindFreeSpace(priority, startTime);

        for (var p = -1; p <= priority; p++)
        {
            var restrictionTimeLine = Get(p);
            var restrictionTime = new RestrictionTime(sentDateTime, GetBusyTimeForPriority(p));
            restrictionTimeLine.Add(restrictionTime);
        }

        return sentDateTime;
    }

    private TimeSpan GetBusyTimeForPriority(int priority) => priority switch
    {
        -1 => Timings.MinPeriodBetweenMessages,
        0 => Timings.MinPeriodBetweenMessagesOnOnePhone,
        1 => Timings.MinPeriodBetweenMessagesOnOnePhoneWithPriority1,
        _ => throw new Exception("Неизвестный приоритет")
    };

    public override string ToString() => string.Join($" {Environment.NewLine}", _restrictionTimeLines);
}