using LogLib;

namespace InterfaceTimingLib
{
    public static class TimingCalculator
    {
        public static void InitializeTiming(IInterfaceTiming it)
        {
            UpdateNextTiming(it);
        }

        public static void UpdateNextTiming(IInterfaceTiming it)
        {
            it.NextExportTime = GetNextExportTime(it);
            if (it.NextExportTime != null)
            {
                Syslog.Debug($"{it.Name} next scheduled time is {it.NextExportTime}");
            }
            else
            {
                Syslog.Debug($"{it.Name} is disabled");
            }
        }

        public static bool ItsTime(IInterfaceTiming it, DateTime time)
        {
            return time > it.NextExportTime;
        }

        private static DateTime? GetNextExportTime(IInterfaceTiming it)
        {
            var now = DateTime.Now;
            var lastExportedTime = it.LastExportedTime ?? now;
            TimeSpan nextPoint = new TimeSpan();

            if (it.EnableInterval && it.IntervalSec is int intervalSec)
            {
                var intervalTimes = (int)Math.Ceiling(lastExportedTime.TimeOfDay.TotalSeconds) / intervalSec + 1;
                nextPoint = new TimeSpan(0, 0, 0, intervalSec * intervalTimes, 0);
            }

            if (it.EnableTiming && it.SpecifiedTimings.Count() != 0)
            {
                var nowPoint = now.TimeOfDay;

                if (nextPoint.Ticks == 0)
                {
                    //  インターバル無効の時は次の指定送信時間
                    nextPoint = it.SpecifiedTimings.Where(x => x > nowPoint).FirstOrDefault();
                    if (nextPoint.Ticks == 0)
                    {
                        //  該当なしの時は最初のものを次の日にして使用
                        nextPoint = it.SpecifiedTimings.First() + new TimeSpan(1, 0, 0, 0);
                    }
                }
                else
                {
                    //  次のインターバル以前の指定送信時間取得
                    var nextSpecifiedPoint = it.SpecifiedTimings.Where(x => x > nowPoint && x < nextPoint).FirstOrDefault();
                    if (nextSpecifiedPoint.Ticks == 0)
                    {
                        //  該当なしの時は日付関係なしに検索
                        if (nextPoint.Days > 0)
                        {
                            var nextPoint2 = nextPoint - new TimeSpan(nextPoint.Days, 0, 0, 0);
                            nextSpecifiedPoint = it.SpecifiedTimings.Where(x => x < nextPoint2).FirstOrDefault() + new TimeSpan(1, 0, 0, 0);
                        }
                    }

                    if (nextSpecifiedPoint.Ticks > 0)
                    {
                        nextPoint = nextSpecifiedPoint;
                    }
                }
            }

            if (nextPoint.Ticks == 0)
            {
                return null;
            }

            var nextTime = DateTime.Today + nextPoint;

            if (nextTime < now)
            {
                Syslog.Warn($"Invalid scheduled time {nextTime}, DISABLED!");
                return null;
            }

            return nextTime;
        }
    }
}
