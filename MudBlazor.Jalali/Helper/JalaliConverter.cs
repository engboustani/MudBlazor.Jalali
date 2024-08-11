using System.Globalization;

namespace MudBlazor.Jalali;

public class JalaliConverter : Converter<DateTime?>
{
    public JalaliConverter()
    {
        SetFunc = time =>
        {
            var pc = new PersianCalendar();
            return
                $"{pc.GetYear(time ?? DateTime.Today)}/{pc.GetMonth(time ?? DateTime.Today)}/{pc.GetDayOfMonth(time ?? DateTime.Today)}";
        };
        GetFunc = s =>
        {
            var split = s?.Split('/') ?? [];
            if (split.Length == 3)
                return new PersianCalendar().ToDateTime(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]),
                    Convert.ToInt32(split[2]), 0, 0, 0, 0);
            return null;
        };
        Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        Culture = CultureInfo.CurrentCulture;
    }
}