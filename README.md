# MudBlazor.Jalali
MudBlazor.Jalali is a package that brings the Jalali DatePicker to the MudBlazor UI Kit. This package helps you handle Persian dates using MudBlazor components.
## Installation via NuGet
To install this package from NuGet, you can use the following command:
```sh dotnet add package MudBlazor.Jalali ```
Or, you can add it to your project through Visual Studio:
1. Right-click on your project and select "Manage NuGet Packages".
2. Select the "Browse" tab and search for "MudBlazor.Jalali".
3. Click the "Install" button to add the package.
## Usage of MudBlazor.Jalali
To use the Jalali DatePicker component, first, add MudBlazor.Jalali to your project:
```html @using MudBlazor.Jalali ```
Then, you can use the Jalali DatePicker in your code:
```html ```
Here, `selectedDate` is a variable of type `DateTime` that you can define and use in your class.
## Complete Example
Below is a complete example of using MudBlazor.Jalali in a Blazor component:
```html @page "/jalali-datepicker" @using MudBlazor.Jalali
Jalali DatePicker

@code { private DateTime selectedDate = DateTime.Now; } ```
By following this method, you can easily use the Jalali DatePicker in your Blazor projects and handle Persian dates.
If you have any questions or issues, please refer to the [MudBlazor official documentation](https://mudblazor.com/) or ask your question on the [GitHub repository](https://github.com/MudBlazor/MudBlazor).
---
Save this file and add it to your project for easy access to the MudBlazor.Jalali features.
