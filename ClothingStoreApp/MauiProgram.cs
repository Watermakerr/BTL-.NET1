﻿using Microsoft.Extensions.Logging;
using ClothingStoreApp.Services;
using System.Globalization;
namespace ClothingStoreApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var culture = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services for dependency injection

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}