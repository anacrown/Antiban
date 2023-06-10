using System;

namespace Antiban;

public class RestrictionTime
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public TimeSpan Busy { get; }

    public RestrictionTime(DateTime start, TimeSpan busy)
    {
        Start = start;
        Busy = busy;
        End = start.Add(busy);
    }
    public RestrictionTime(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start must be less than End.");
        }

        Start = start;
        End = end;
        Busy = end - start;
    }
    public bool IsIntersect(RestrictionTime interval)
    {
        return interval.Start < End && interval.End > Start;
    }
    public bool IsInclude(RestrictionTime interval)
    {
        return interval.Start >= Start && interval.End <= End;
    }
    public RestrictionTime ShiftTo(DateTime start) => new(start, Busy);

    public override string ToString() => $"{Start:HH:mm:ss} {Busy:g}";
}