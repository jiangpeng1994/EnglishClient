using System;
using UnityEngine;

/// <summary>
/// 时间相关的计算类
/// </summary>
public class TimeUtil
{
    const string TAG = "TimeUtil-> ";
    /// <summary>
    /// 获取当前系统时间
    /// </summary>
    /// <returns></returns>
    public static DateTime GetNowSystemDate()
    {
        return DateTime.Now;
    }

	/// <summary>
	/// 当前时间是否过期
	/// </summary>
	/// <param name="dwOverDate"></param>
	/// <returns>false = 过期</returns>
	public static bool IsOverDate(uint dwOverDate)
	{
		return dwOverDate - DateTimeToTimeStamp(DateTime.Now) < 0;
	}

    /// <summary>
    /// 计算时间差，以秒为单位
    /// </summary>
    /// <param name="timeNew">新时间</param>
    /// <param name="timeOld">旧时间</param>
    /// <returns></returns>
    public static float GetDiffToSecond(DateTime timeNew, DateTime timeOld)
    {
        return (float)(timeNew - timeOld).TotalSeconds;
    }
    /// <summary>
    /// 计算系统当前时间与旧时间的时间差，以秒为单位
    /// </summary>
    /// <param name="timeOld">旧时间</param>
    /// <returns></returns>
    public static float GetDiffToSecond(DateTime timeOld)
    {
        return (float)(DateTime.Now - timeOld).TotalSeconds;
    }

    public static DateTime StringToDateTime(string sDateTime)
    {
        try
        {
            return Convert.ToDateTime(sDateTime);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    /// <summary>
    /// 2017-06-01 00:00:00 转时间戳 =>1496246400
    /// </summary>
    /// <param name="sDateTime"></param>
    /// <returns></returns>
    public static long StringDataToTimeStamp(string sDateTime)
    {
        try
        {
            return DateTimeToTimeStamp(StringToDateTime(sDateTime));
        }
        catch (Exception e)
        {
            return 0;
        }
        
    }

	/// <summary>
	/// 时间戳转成字符串日期
	/// </summary>
	/// <param name="lDatetime"></param>
	/// <returns></returns>
	public static string TimeStampToStringData(long lDatetime)
	{
		DateTime dt = TimeStampToDateTime(lDatetime);

		return dt.ToString("yyyy年MM月dd日 HH:mm");
	}

	/// <summary>
	/// 时间戳转成字符串日期
	/// </summary>
	/// <param name="lDatetime"></param>
	/// <returns></returns>
	public static string TimeStampToStringDataNoYYAndHH(long lDatetime)
	{
		DateTime dt = TimeStampToDateTime(lDatetime);

		return dt.ToString("MM月dd日");
	}

    /// <summary>
    /// 当前时间是否在两者之间
    /// </summary>
    /// <param name="DTStart"></param>
    /// <param name="DTEnd"></param>
    /// <returns></returns>
    public static bool IsBetweenDateTime(DateTime DTStart, DateTime DTEnd)
    {
        long lNowTicks = DateTime.Now.Ticks;
        return lNowTicks > DTStart.Ticks && lNowTicks < DTEnd.Ticks;
    }
    /// <summary>
    /// 当前时间是否已经过了目标时间
    /// </summary>
    /// <param name="DTTarget"></param>
    /// <returns></returns>
    public static bool IsBehindDateTime(DateTime DTTarget)
    {
        return DateTime.Now.Ticks > DTTarget.Ticks;
    }

    /// <summary>
    /// 当前时间是否未到目标时间
    /// </summary>
    /// <param name="DTTarget"></param>
    /// <returns></returns>
    public static bool IsEarlyDateTime(DateTime DTTarget)
    {
        return DateTime.Now.Ticks < DTTarget.Ticks;
    }

	/// <summary>
	/// 当前时间是否大于10年
	/// </summary>
	/// <param name="DTTarget"></param>
	/// <returns></returns>
	public static bool IsEarlyDateTimeToTenYear(DateTime DTTarget)
	{
		return DateTime.Now.AddYears(10).Ticks < DTTarget.Ticks;
	}

    public static int CompareTime(DateTime DTTarget1, DateTime DTTarget2)
    {
        return DTTarget1.Ticks.CompareTo(DTTarget2.Ticks);

    }
    /// <summary>
    /// 两个日期是否是同一天
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool IsTheSameDay(DateTime date1, DateTime date2)
    {
        return date1.ToShortDateString().Equals(date2.ToShortDateString());
    }
    /// <summary>
    /// 两个日期是否是同一天
    /// </summary>
    /// <param name="sDate1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool IsTheSameDay(string sDate1, DateTime date2)
    {
        DateTime dataTimeLast;
        bool bIsTheSameDay = false;
        try
        {
            dataTimeLast = TimeUtil.StringToDateTime(sDate1);
            bIsTheSameDay = TimeUtil.IsTheSameDay(dataTimeLast, date2);
        }
        catch (Exception e) { }
        return bIsTheSameDay;
    }

    /// <summary>
    /// 两个日期是否是同一周
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool IsInSameWeek(DateTime date1, DateTime date2)
    {
        TimeSpan ts = date2 - date1;
        double dbl = ts.TotalDays;
        int intDow = Convert.ToInt32(date2.DayOfWeek);
        if (intDow == 0) intDow = 7;
        if (dbl >= 7 || dbl >= intDow) return false;
        else return true;
    }
    /// <summary>
    /// long转DateTime
    /// </summary>
    /// <param name="lTrick"></param>
    /// <returns></returns>
    public static DateTime LongToDateTime(long lTrick)
    {
        try
        {
            return new DateTime(lTrick);
        }
        catch (Exception e)
        {
            throw e;
        }

    }

    /// <summary>
    /// 秒数转时间  如200秒->  0小时3分钟20秒
    /// </summary>
    /// <param name="nSecond"></param>
    /// <returns></returns>
    public static Vector3 SecondToV3Time(int nSecond)
    {
        Vector3 v3Time = Vector3.zero;
        v3Time.x = nSecond / 3600;
        nSecond -= (int)v3Time.x * 3600;
        v3Time.y = nSecond / 60;
        nSecond -= (int)v3Time.y * 60;
        v3Time.z = nSecond;
        return v3Time;
    }
    /// <summary>
    /// 秒数转时间  如200秒-> 3:20
    /// </summary>
    /// <param name="nSecond"></param>
    /// <returns></returns>
    public static string SecondToStrTime(int nSecond)
    {
        int nHour, nMin, nSec;
        nHour = nSecond / 3600;
        nSecond -= nHour * 3600;
        nMin = nSecond / 60;
        nSecond -= nMin * 60;
        nSec = nSecond;
        string sTime = "";
        if (nHour > 0)
        {
            sTime += (nHour < 10 ? "0" : "") + nHour + ":";
        }
        sTime += (nMin < 10 ? "0" : "") + nMin + ":";
        sTime += (nSec < 10 ? "0" : "") + nSec;
        return sTime;
    }

    /// <summary>
    /// 时间转string
    /// </summary>
    /// <param name="nSecond"></param>
    /// <returns></returns>
    public static string DateTimeToStrHHmm(DateTime dateTime)
    {
        return dateTime.ToString("HH:mm");
    }

    /// <summary>
    /// DateTime转时间戳
    /// </summary>
    /// <returns></returns>
    public static long DateTimeToTimeStamp(DateTime dateTime)
    {
        try
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return (long)(dateTime - startTime).TotalSeconds; // 相差毫秒数
        }
        catch (Exception e)
        {
            Debug.LogError(TAG + "TimeStampToDateTime:" + e.ToString());
            throw e;
        }
    }
    /// <summary>  
     /// 获取时间戳  
     /// </summary>  
     /// <returns></returns>  
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }

    /// <summary>
    /// 返回离第二天的剩余时间
    /// </summary>
    /// <returns></returns>
    public static string GetLeftTimeToNextDay()
    {
        DateTime dtNow = DateTime.Now;
        DateTime dtNext = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day).AddDays(1);
        return SecondToStrTime((int)(dtNext - dtNow).TotalSeconds);
    }

    /// <summary>
    /// 时间戳转DateTime
    /// </summary>
    /// <returns></returns>
    public static DateTime TimeStampToDateTime(long lTimeStamp)
    {
        try
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dateTimeStart.AddSeconds(lTimeStamp);
            //long lTime = long.Parse(lTimeStamp + "0000000");
            //TimeSpan toNow = new TimeSpan(lTime);
            //return dateTimeStart.Add(toNow);
        }
        catch (Exception e)
        {
            Debug.LogError(TAG + "TimeStampToDateTime:" + e.ToString());//这里有时候报错，数值过大
            throw e;
        }

    }

    /// <summary>
    /// 判断小时时间是否在指定范文内
    /// </summary>
    /// <param name="DTTarget"></param>
    /// <param name="DTStart"></param>
    /// <param name="DTEnd"></param>
    /// <returns></returns>
    public static bool IsNowInTimeOfDay(DateTime DTStart, DateTime DTEnd)
    {
        if (TimeSpan.Compare(DTStart.TimeOfDay, DTEnd.TimeOfDay) > 0)
        {//例如  23:00   (20:00-6:00)
            return TimeSpan.Compare(DateTime.Now.TimeOfDay, DTStart.TimeOfDay) >= 0 || TimeSpan.Compare(DateTime.Now.TimeOfDay, DTEnd.TimeOfDay) < 0;
        }
        else
        {//例如  9:00   (6:00-12:00)
            return TimeSpan.Compare(DateTime.Now.TimeOfDay, DTStart.TimeOfDay) >= 0 && TimeSpan.Compare(DateTime.Now.TimeOfDay, DTEnd.TimeOfDay) < 0;
        }
    }

    /// <summary>
    /// 计算uTimeStep这天离今天的日期差，同一天为0
    /// </summary>
    /// <returns></returns>
    public static int DayOffWithTimeStep(uint uTimeStep)
    {
        try
        {
            DateTime dtOld = TimeStampToDateTime(uTimeStep);
            dtOld = new DateTime(dtOld.Year, dtOld.Month, dtOld.Day);
            DateTime dtNow = DateTime.Now;
            dtNow = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
            return (int)(dtNow - dtOld).TotalDays;
        }
        catch (Exception e)
        {
            Debug.LogError(TAG + "DayOffWithTimeStep:" + e.ToString());
            throw e;
        }
    }
}
