# MudBlazor.Jalali

**MudBlazor.Jalali** is a comprehensive date picker component library built on top of the popular MudBlazor UI framework, specifically designed for Iranian (Persian) users who require Jalali calendar functionality.

### *[👀 Live preview](https://engboustani.github.io)*

<img width="423" alt="Screenshot" src="https://github.com/user-attachments/assets/81954ea5-42b4-4759-8c54-f0c3e4dc2df0">

## Installation

To install the MudBlazor.Jalali package, use the following command in your .NET project:

```bash
dotnet add package MudBlazorJalali
```
Or, you can add it to your project through Visual Studio:
1. Right-click on your project and select "Manage NuGet Packages".
2. Select the "Browse" tab and search for "MudBlazorJalali".
3. Click the "Install" button to add the package.

## Usage of MudBlazor.Jalali

Using the MudBlazor.Jalali components is straightforward. Simply replace the standard MudBlazor MudDatePicker with MudJalaliDatePicker.

```csharp
<MudJalaliDatePicker Label="Basic example" />
<MudJalaliDateRangePicker Label="Range example" PlaceholderStart="Start Date" PlaceholderEnd="End Date" />
```

## Features
- **Jalali Calendar Integration:** Provides a fully functional Jalali calendar component.
- **Date Picker:** Offers a date picker component that supports Jalali dates.
- **Date Range Picker:** Enables selection of date ranges using the Jalali calendar.
- **Easy Integration:** Seamlessly integrates with MudBlazor's existing components and styling.
- **Customization:** Allows for customization of the component's appearance and behavior.
- **Localization support**
- **Accessibility**
- **Performance optimization**

## Contributing
We welcome contributions to the MudBlazor.Jalali project! If you have any ideas, bug fixes, or new features, please feel free to open an issue or submit a pull request.

## License
MudBlazor.Jalali is licensed under the MIT License.

**Let's build a great Jalali date picker component library together!**

## Changelog
### v7.6.1
- Introduced MudJalaliDateRangePicker component for selecting date ranges in the Jalali calendar.
- Improved styling consistency and visual appeal of the components.
- Fixed issues with displaying correct month names in the Jalali calendar.
