using System;

namespace Antiban;

public static class DateTimeExtensions
{
    public static DateTime Max(this DateTime dateTime1, DateTime dateTime2) => dateTime1 > dateTime2 ? dateTime1 : dateTime2;
}