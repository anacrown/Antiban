using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Antiban;

public class RestrictionTimeLine : IEnumerable<RestrictionTime>
{
    private List<RestrictionTime> TimeIntervals { get; set; }

    public RestrictionTimeLine()
    {
        TimeIntervals = new List<RestrictionTime>();
    }

    public void Add(RestrictionTime restrictionTime)
    {
        // Добавляем новый интервал и сортируем список по начальной дате
        TimeIntervals.Add(restrictionTime);
        TimeIntervals = TimeIntervals.OrderBy(ti => ti.Start).ToList();
    }

    public IEnumerator<RestrictionTime> GetEnumerator()
    {
        return TimeIntervals.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<RestrictionTime> GetFreeIntervals()
    {
        if (!TimeIntervals.Any())
        {
            yield return new RestrictionTime(DateTime.MinValue, DateTime.MaxValue);
            yield break;
        }
        
        yield return new RestrictionTime(DateTime.MinValue, TimeIntervals.First().Start);

        for (int i = 0; i < TimeIntervals.Count - 1; i++)
        {
            if (TimeIntervals[i].End < TimeIntervals[i + 1].Start)
            {
                yield return new RestrictionTime(TimeIntervals[i].End, TimeIntervals[i + 1].Start);
            }
        }
        
        yield return new RestrictionTime(TimeIntervals.Last().End, DateTime.MaxValue);
    }

    public bool IsIntersect(RestrictionTime interval)
    {
        return TimeIntervals.Any(ti => ti.IsIntersect(interval));
    }

    public RestrictionTime FindFreeSpace(RestrictionTime interval)
    {
        if (!IsIntersect(interval))
            return interval;

        foreach (var freeInterval in GetFreeIntervals()
                     .Where(i => i.Start >= interval.Start))
        {
            var shiftedInterval = interval.ShiftTo(interval.Start.Max(freeInterval.Start));

            if (freeInterval.IsInclude(shiftedInterval))
                return shiftedInterval;
        }

        throw new Exception("Не удалось найти свободное место");
    }
    public override string ToString() => string.Join($" {Environment.NewLine}", TimeIntervals);
}