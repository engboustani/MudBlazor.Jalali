using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor.Jalali;

public abstract partial class MudJalaliBaseDatePicker : MudPicker<DateTime?>
{
    private readonly string _mudPickerCalendarContentElementId;
    private bool _dateFormatTouched;
    protected readonly PersianCalendar _persianCalendar = new PersianCalendar();
    private readonly string[] _solarMonths = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
    private string[] _weekDays = { "ش", "ی", "د", "س", "چ", "پ", "ج" };

    protected MudJalaliBaseDatePicker() : base(new JalaliConverter())
    {
        AdornmentAriaLabel = "Open Date Picker";
        _mudPickerCalendarContentElementId = Identifier.Create();
    }

    [Inject] protected IScrollManager ScrollManager { get; set; }

    [Inject] private IJsApiService JsApiService { get; set; }

    /// <summary>
    /// The maximum selectable date.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Validation)]
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// The minimum selectable date.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Validation)]
    public DateTime? MinDate { get; set; }

    /// <summary>
    /// The initial view to display.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="OpenTo.Date"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public OpenTo OpenTo { get; set; } = OpenTo.Date;

    /// <summary>
    /// The format for selected dates.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Behavior)]
    public string DateFormat
    {
        get
        {
            return (Converter as DefaultConverter<DateTime?>)?.Format;
        }
        set
        {
            if (Converter is DefaultConverter<DateTime?> defaultConverter)
            {
                defaultConverter.Format = value;
                _dateFormatTouched = true;
            }
            DateFormatChangedAsync(value);
        }
    }

    /// <summary>
    /// Occurs when the <see cref="DateFormat"/> has changed.
    /// </summary>
    protected virtual Task DateFormatChangedAsync(string newFormat)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool SetCulture(CultureInfo value)
    {
        if (!base.SetCulture(value))
            return false;

        if (!_dateFormatTouched && Converter is DefaultConverter<DateTime?> defaultConverter)
            defaultConverter.Format = value.DateTimeFormat.ShortDatePattern;

        return true;
    }

    /// <summary>
    /// The day representing the first day of the week.
    /// </summary>
    /// <remarks>
    /// Defaults to the current culture's <c>DateTimeFormat.FirstDayOfWeek</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public DayOfWeek? FirstDayOfWeek { get; set; } = DayOfWeek.Saturday;

    /// <summary>
    /// The current month shown in the date picker.
    /// </summary>
    /// <remarks>
    /// Defaults to the current month.<br />
    /// When bound via <c>@bind-PickerMonth</c>, controls the initial month displayed.  This value is always the first day of a month.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public DateTime? PickerMonth
    {
        get => _picker_month;
        set
        {
            if (value == _picker_month)
                return;
            _picker_month = value;
            InvokeAsync(StateHasChanged);
            PickerMonthChanged.InvokeAsync(value);
        }
    }

    private DateTime? _picker_month;

    /// <summary>
    /// Occurs when <see cref="PickerMonth"/> has changed.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime?> PickerMonthChanged { get; set; }

    /// <summary>
    /// The delay, in milliseconds, before closing the picker after a value is selected.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100</c>.<br />
    /// This delay helps the user see that a date has been selected before the popover disappears.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public int ClosingDelay { get; set; } = 100;

    /// <summary>
    /// The number of months to display in the calendar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public int DisplayMonths { get; set; } = 1;

    /// <summary>
    /// The maximum number of months allowed in one row.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.<br />
    /// When <c>null</c>, the <see cref="DisplayMonths"/> is used.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerAppearance)]
    public int? MaxMonthColumns { get; set; }

    /// <summary>
    /// The start month when opening the picker. 
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public DateTime? StartMonth { get; set; }

    /// <summary>
    /// Shows week numbers at the start of each week.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public bool ShowWeekNumbers { get; set; }

    /// <summary>
    /// The format of the selected date in the title.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>ddd, dd MMM</c>.<br />
    /// Supported date formats can be found here: <see href="https://learn.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public string TitleDateFormat { get; set; } = "ddd, dd MMM";

    /// <summary>
    /// Closes this picker when a value is selected.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public bool AutoClose { get; set; }

    /// <summary>
    /// The function used to disable one or more dates.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.<br />
    /// When set, a date will be disabled if the function returns <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Validation)]
    public Func<DateTime, bool> IsDateDisabledFunc
    {
        get => _isDateDisabledFunc;
        set
        {
            _isDateDisabledFunc = value ?? (_ => false);
        }
    }
    private Func<DateTime, bool> _isDateDisabledFunc = _ => false;

    /// <summary>
    /// The function which returns CSS classes for a date.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Appearance)]
    public Func<DateTime, string> AdditionalDateClassesFunc { get; set; }

    /// <summary>
    /// The icon for the button that navigates to the previous month or year.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronLeft"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerAppearance)]
    public string PreviousIcon { get; set; } = Icons.Material.Filled.ChevronLeft;

    /// <summary>
    /// The icon for the button which navigates to the next month or year.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerAppearance)]
    public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

    /// <summary>
    /// The year to use, which cannot be changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public int? FixYear { get; set; }

    /// <summary>
    /// The month to use, which cannot be changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public int? FixMonth { get; set; }

    /// <summary>
    /// The day to use, which cannot be changed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.PickerBehavior)]
    public int? FixDay { get; set; }

    protected virtual bool IsRange { get; } = false;

    protected OpenTo CurrentView;

    protected override async Task OnPickerOpenedAsync()
    {
        await base.OnPickerOpenedAsync();
        if (Editable && Text != null)
        {
            var a = Converter.Get(Text);
            if (a.HasValue)
            {
                a = new DateTime(a.Value.Year, a.Value.Month, 1);
                PickerMonth = a;
            }
        }
        if (OpenTo == OpenTo.Date && FixDay.HasValue && FixMonth.HasValue)
        {
            OpenTo = OpenTo.Year;
        }
        if (OpenTo == OpenTo.Date && FixDay.HasValue)
        {
            OpenTo = OpenTo.Month;
        }
        CurrentView = OpenTo;
        if (CurrentView == OpenTo.Year)
            _scrollToYearAfterRender = true;
    }

    /// <summary>
    /// Get the first of the month to display
    /// </summary>
    protected DateTime GetMonthStart(int month)
    {
        DateTime monthStartDate;
        if (_picker_month is not null)
        {
            monthStartDate = _persianCalendar.ToDateTime(_persianCalendar.GetYear(_picker_month.Value),
                _persianCalendar.GetMonth(_picker_month.Value), 1, 0, 0, 0, 0);
        }
        else
        {
            monthStartDate = _persianCalendar.ToDateTime(_persianCalendar.GetYear(DateTime.Today),
                _persianCalendar.GetMonth(DateTime.Today), 1, 0, 0, 0, 0);
        }
        // Return the min supported datetime of the calendar when this is year 1 and first month!
        if (_picker_month is { Year: 1, Month: 1 })
        {
            return Culture.Calendar.MinSupportedDateTime;
        }
        return _persianCalendar.AddMonths(monthStartDate, month);
    }

    /// <summary>
    /// Get the last of the month to display
    /// </summary>
    protected DateTime GetMonthEnd(int month)
    {
        DateTime monthStartDate;
        if (_picker_month is not null)
        {
            monthStartDate = _persianCalendar.ToDateTime(_persianCalendar.GetYear(_picker_month.Value),
                _persianCalendar.GetMonth(_picker_month.Value), 1, 0, 0, 0, 0);
        }
        else
        {
            monthStartDate = _persianCalendar.ToDateTime(_persianCalendar.GetYear(DateTime.Today),
                _persianCalendar.GetMonth(DateTime.Today), 1, 0, 0, 0, 0);
        }
        return _persianCalendar.AddMonths(monthStartDate, month + 1).AddDays(-1);
    }

    protected DayOfWeek GetFirstDayOfWeek()
    {
        if (FirstDayOfWeek.HasValue)
            return FirstDayOfWeek.Value;
        return DayOfWeek.Saturday;
    }

    /// <summary>
    /// Gets the n-th week of the currently displayed month. 
    /// </summary>
    /// <param name="month">offset from _picker_month</param>
    /// <param name="index">between 0 and 4</param>
    protected IEnumerable<DateTime> GetWeek(int month, int index)
    {
        if (index is < 0 or > 5)
            throw new ArgumentException("Index must be between 0 and 5");
        var month_first = GetMonthStart(month);
        var week_first = month_first.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek());
        for (var i = 0; i < 7; i++)
            yield return week_first.AddDays(i);
    }

    private string GetWeekNumber(int month, int index)
    {
        if (index is < 0 or > 5)
            throw new ArgumentException("Index must be between 0 and 5");
        var month_first = GetMonthStart(month);
        var week_first = month_first.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek());
        //january 1st
        if (month_first.Month == 1 && index == 0)
        {
            week_first = month_first;
        }

        if (week_first.Month != month_first.Month && week_first.AddDays(6).Month != month_first.Month)
            return "";

        return _persianCalendar.GetWeekOfYear(week_first, CalendarWeekRule.FirstDay, FirstDayOfWeek ?? DayOfWeek.Saturday).ToString();
    }

    protected virtual OpenTo? GetNextView()
    {
        OpenTo? nextView = CurrentView switch
        {
            OpenTo.Year => !FixMonth.HasValue ? OpenTo.Month : !FixDay.HasValue ? OpenTo.Date : null,
            OpenTo.Month => !FixDay.HasValue ? OpenTo.Date : null,
            _ => null,
        };
        return nextView;
    }

    protected virtual async Task SubmitAndCloseAsync()
    {
        if (PickerActions == null)
        {
            await SubmitAsync();

            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(ClosingDelay);
                await CloseAsync(false);
            }
        }
    }

    protected abstract string GetDayClasses(int month, DateTime day);

    /// <summary>
    /// User clicked on a day
    /// </summary>
    protected abstract Task OnDayClickedAsync(DateTime dateTime);

    /// <summary>
    /// user clicked on a month
    /// </summary>
    /// <param name="month"></param>
    protected virtual Task OnMonthSelectedAsync(DateTime month)
    {
        PickerMonth = month;
        var nextView = GetNextView();
        if (nextView != null)
        {
            CurrentView = (OpenTo)nextView;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// user clicked on a year
    /// </summary>
    /// <param name="year"></param>
    protected virtual Task OnYearClickedAsync(int year)
    {
        PickerMonth = _persianCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0);
        var nextView = GetNextView();
        if (nextView != null)
        {
            CurrentView = (OpenTo)nextView;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// user clicked on a month
    /// </summary>
    protected virtual void OnMonthClicked(int month)
    {
        CurrentView = OpenTo.Month;
        _picker_month = _picker_month?.AddMonths(month);
        StateHasChanged();
    }

    /// <summary>
    /// Check if month is disabled
    /// </summary>
    /// <param name="month">Month given with first day of the month</param>
    /// <returns>True if month should be disabled, false otherwise</returns>
    private bool IsMonthDisabled(DateTime month)
    {
        if (!FixDay.HasValue)
        {
            return month.EndOfMonth(Culture) < MinDate || month > MaxDate;
        }
        if (DateTime.DaysInMonth(month.Year, month.Month) < FixDay!.Value)
        {
            return true;
        }
        var day = new DateTime(month.Year, month.Month, FixDay!.Value);
        return day < MinDate || day > MaxDate || IsDateDisabledFunc(day);
    }

    /// <summary>
    /// return Mo, Tu, We, Th, Fr, Sa, Su in the right culture
    /// </summary>
    protected IEnumerable<string> GetAbbreviatedDayNames()
    {
        var dayNamesShifted = Shift(_weekDays, (int)GetFirstDayOfWeek() - 6);
        return dayNamesShifted;
    }

    /// <summary>
    /// Shift array and cycle around from the end
    /// </summary>
    private static T[] Shift<T>(T[] array, int positions)
    {
        var copy = new T[array.Length];
        Array.Copy(array, 0, copy, array.Length - positions, positions);
        Array.Copy(array, positions, copy, 0, array.Length - positions);
        return copy;
    }

    protected string GetMonthName(int month)
    {
        var calenderMonth = _persianCalendar.GetMonth(GetMonthStart(month));
        return _solarMonths[calenderMonth - 1];
    }

    protected abstract string GetTitleDateString();

    protected string FormatTitleDate(DateTime? date)
    {
        if(date is null)
            return string.Empty;
        var persianYear = _persianCalendar.GetYear(date ?? DateTime.Now);
        var persianMonth = _persianCalendar.GetMonth(date ?? DateTime.Now);
        var persianDay = _persianCalendar.GetDayOfMonth(date ?? DateTime.Now);
        return PersianWord.ConvertToPersianNumber($"{persianDay} {_solarMonths[persianMonth - 1]}");
    }

    protected string GetFormattedYearString()
    {
        return PersianWord.ConvertToPersianNumber(_persianCalendar.GetYear(GetMonthStart(0)).ToString());
    }

    private void OnPreviousMonthClick()
    {
        // It is impossible to go further into the past after the first year and the first month!
        if (PickerMonth.HasValue && PickerMonth.Value.Year == 1 && PickerMonth.Value.Month == 1)
        {
            return;
        }

        PickerMonth = _persianCalendar.AddMonths(GetMonthStart(0), -1);
    }

    private void OnNextMonthClick()
    {
        PickerMonth = GetMonthEnd(0).AddDays(1);
    }

    private void OnPreviousYearClick()
    {
        PickerMonth = _persianCalendar.AddYears(GetMonthStart(0), -1);// GetMonthStart(0).AddYears(-1);
    }

    private void OnNextYearClick()
    {
        PickerMonth = _persianCalendar.AddYears(GetMonthStart(0), 1);
    }

    private void OnYearClick()
    {
        if (!FixYear.HasValue)
        {
            CurrentView = OpenTo.Year;
            StateHasChanged();
            _scrollToYearAfterRender = true;
        }
    }

    /// <summary>
    /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
    /// </summary>
    private readonly string _componentId = Identifier.Create();

    /// <summary>
    /// Is set to true to scroll to the actual year after the next render
    /// </summary>
    private bool _scrollToYearAfterRender = false;

    /// <summary>
    /// Scrolls to the current year.
    /// </summary>
    public async void ScrollToYear()
    {
        _scrollToYearAfterRender = false;
        var id = $"{_componentId}{Culture.Calendar.GetYear(GetMonthStart(0))}";
        await ScrollManager.ScrollToYearAsync(id);
        StateHasChanged();
    }

    private int GetMinYear()
    {
        if (MinDate.HasValue)
            return _persianCalendar.GetYear(MinDate.Value);
        return _persianCalendar.GetYear(DateTime.Today) - 100;
    }

    private int GetMaxYear()
    {
        if (MaxDate.HasValue)
            return _persianCalendar.GetYear(MaxDate.Value);
        return _persianCalendar.GetYear(DateTime.Today) + 100;
    }

    private string GetYearClasses(int year)
    {
        if (year == Culture.Calendar.GetYear(GetMonthStart(0)))
            return $"vazirmatn mud-picker-year-selected mud-{Color.ToDescriptionString()}-text";
        return "mud-typography mud-typography-subtitle1 vazirmatn";
    }

    private string GetCalendarHeaderClasses(int month)
    {
        return new CssBuilder("mud-picker-calendar-header")
            .AddClass($"mud-picker-calendar-header-{month + 1}")
            .AddClass($"mud-picker-calendar-header-last", month == DisplayMonths - 1)
            .Build();
    }

    private Typo GetYearTypo(int year)
    {
        if (year == Culture.Calendar.GetYear(GetMonthStart(0)))
            return Typo.h5;
        return Typo.subtitle1;
    }

    private void OnFormattedDateClick()
    {
        // todo: raise an event the user can handle
    }


    private IEnumerable<DateTime> GetAllMonths()
    {
        var current = GetMonthStart(0);
        var calendarYear = _persianCalendar.GetYear(current);
        var firstOfCalendarYear =  _persianCalendar.ToDateTime(calendarYear, 1, 1, 0, 0, 0, 0);
        for (var i = 0; i < 12; i++)
            yield return _persianCalendar.AddMonths(firstOfCalendarYear, i);
    }

    private string GetAbbreviatedMonthName(DateTime month)
    {
        var calenderMonth = _persianCalendar.GetMonth(month);
        return _solarMonths[calenderMonth - 1];
    }

    private string GetMonthName(DateTime month)
    {
        var calenderMonth = _persianCalendar.GetMonth(month);
        return _solarMonths[calenderMonth - 1];
    }
    
    private string GetPersianYear(DateTime year)
    {
        var calenderYear = _persianCalendar.GetYear(year);
        return PersianWord.ConvertToPersianNumber(calenderYear.ToString());
    }

    private string GetMonthClasses(DateTime month)
    {
        if (Culture.Calendar.GetMonth(GetMonthStart(0)) == Culture.Calendar.GetMonth(month) && !IsMonthDisabled(month))
            return $"mud-picker-month-selected mud-{Color.ToDescriptionString()}-text vazirmatn";
        return "mud-typography mud-typography-subtitle1 vazirmatn";
    }

    private Typo GetMonthTypo(DateTime month)
    {
        if (Culture.Calendar.GetMonth(GetMonthStart(0)) == Culture.Calendar.GetMonth(month))
            return Typo.h5;
        return Typo.subtitle1;
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        CurrentView = OpenTo;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _picker_month ??= GetCalendarStartOfMonth();
        }

        if (firstRender && CurrentView == OpenTo.Year)
        {
            ScrollToYear();
            return;
        }

        if (_scrollToYearAfterRender)
            ScrollToYear();
    }

    protected abstract DateTime GetCalendarStartOfMonth();

    private int GetCalendarDayOfMonth(DateTime date)
    {
        return _persianCalendar.GetDayOfMonth(date);// Culture.Calendar.GetDayOfMonth(date);
    }

    /// <summary>
    /// Converts gregorian date into whatever year it is in the provided culture
    /// </summary>
    /// <param name="yearDate">Gregorian Date</param>
    /// <returns>Year according to culture</returns>
    protected abstract int GetCalendarYear(DateTime yearDate);

    private ValueTask HandleMouseoverOnPickerCalendarDayButton(int tempId)
    {
        return ValueTask.CompletedTask; // JsApiService.UpdateStyleProperty(_mudPickerCalendarContentElementId, "--selected-day", tempId);
    }

    private string GetDayTextClasses(DateTime selectedDay)
    {
        if(selectedDay.DayOfWeek is DayOfWeek.Friday && selectedDay.Date != DateTime.Today)
            return "vazirmatn red-text text-darken-1 mud-typography mud-typography-body2";
        return "vazirmatn mud-typography mud-typography-body2 mud-inherit-text";
    }
}