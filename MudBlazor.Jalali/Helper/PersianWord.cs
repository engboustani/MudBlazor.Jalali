namespace MudBlazor.Jalali;

public abstract class PersianWord
{
    private PersianWord() {}
    
    public static unsafe string ToPersianString(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var str = value.ToString().AsSpan();
        var strOut = stackalloc char[str.Length];

        for (var i = 0; i < str.Length; i++)
        {
            var ch = str[i];
            if (ch >= 48 && ch <= 57)
            {
                ch = (char)(ch + 1728);
            }
            else if (ch == 46)
            {
                ch = '/'; // Using '/' character literal instead of casting 47 }
            }
            strOut[i] = ch;
        }

        return new string(strOut).Replace("ي", "ی").Replace("ك", "ک");
    }
    
    public static string ConvertToPersianNumber(string num)
    {
        if (string.IsNullOrWhiteSpace(num)) return string.Empty;

        var chars = num.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = chars[i] switch
            {
                '0' or '\u0660' => '\u06F0',
                '1' or '\u0661' => '\u06F1',
                '2' or '\u0662' => '\u06F2',
                '3' or '\u0663' => '\u06F3',
                '4' or '\u0664' => '\u06F4',
                '5' or '\u0665' => '\u06F5',
                '6' or '\u0666' => '\u06F6',
                '7' or '\u0667' => '\u06F7',
                '8' or '\u0668' => '\u06F8',
                '9' or '\u0669' => '\u06F9',
                _ => chars[i]
            };
        }

        return new string(chars);
    }

    private static unsafe string ConvertToLatinNumber(string num)
    {
        if (string.IsNullOrWhiteSpace(num)) return string.Empty;

        var str = num.AsSpan();
        var strOut = stackalloc char[str.Length];

        for (var i = 0; i < str.Length; i++)
        {
            strOut[i] = str[i] switch
            {
                '\u06F0' or '\u0660' => '0',
                '\u06F1' or '\u0661' => '1',
                '\u06F2' or '\u0662' => '2',
                '\u06F3' or '\u0663' => '3',
                '\u06F4' or '\u0664' => '4',
                '\u06F5' or '\u0665' => '5',
                '\u06F6' or '\u0666' => '6',
                '\u06F7' or '\u0667' => '7',
                '\u06F8' or '\u0668' => '8',
                '\u06F9' or '\u0669' => '9',
                _ => str[i]
            };
        }

        return new string(strOut);
    }
}