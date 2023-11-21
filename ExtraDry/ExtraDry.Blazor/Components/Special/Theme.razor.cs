﻿using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace ExtraDry.Blazor;

/// <summary>
/// Provides a cascading theme mechanism to configure portions of ExtraDry.
/// Note: this does not cover styles like colors which should be done in CSS.
/// </summary>
public partial class Theme : ComponentBase {

    /// <summary>
    /// The content that this theme applies to.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    /// <summary>
    /// The collection of icons that are used for this theme.  See IconInfo for details on 
    /// creating the icons, then register them here.  The icons must always be available and should
    /// be stored in a static backing field to ensure they are available, especially when the site
    /// navigates to a missing page and back.  It is recommended they are part of the AppViewModel
    /// registered as a Singleton.
    /// </summary>
    [Parameter]
    public IEnumerable<IconInfo>? Icons { get; set; }

    /// <summary>
    /// A ThemeInfo that might have been cascaded from a parent theme.
    /// </summary>
    [CascadingParameter]
    public ThemeInfo ThemeInfo { get; set; } = new();

    /// <summary>
    /// A custom error component that is applied and used on any DryErrorBoundary instead of the default.
    /// </summary>
    [Parameter]
    public Type? ErrorComponent { get; set; }

    /// <inheritdoc cref="Suspense{TModel}.Error" />
    /// <see cref="Suspense{TModel}"/>
    [Parameter]
    public RenderFragment<IndicatorContext>? SuspenseError { get; set; }

    /// <inheritdoc cref="Suspense{TModel}.Timeout" />
    /// <see cref="Suspense{TModel}"/>
    [Parameter]
    public RenderFragment<IndicatorContext>? SuspenseTimeout { get; set; }

    /// <inheritdoc cref="Suspense{TModel}.Fallback" />
    /// <see cref="Suspense{TModel}"/>
    [Parameter]
    public RenderFragment<IndicatorContext>? SuspenseFallback { get; set; }

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(ErrorComponent != null) {
            ThemeInfo.ErrorComponent = ErrorComponent;
        }

        if(SuspenseError != null) {
            ThemeInfo.SuspenseError = SuspenseError;
        }

        if(SuspenseTimeout != null) {
            ThemeInfo.SuspenseTimeout = SuspenseTimeout;
        }

        if(SuspenseFallback != null) {
            ThemeInfo.SuspenseFallback = SuspenseFallback;
        }

        await LoadSvgDatabaseAsync();
        ThemeInfo.Icons = CachedIcons.ToDictionary(e => e.Key, e => e.Value);
    }

    private async Task LoadSvgDatabaseAsync()
    {
        if(loaded) {
            return;
        }
        ThemeInfo.Loading = true;
        Icons ??= Array.Empty<IconInfo>();
        if(Http == null) {
            Logger.LogConsoleError("Theme requires an HttpClient to load SVG icons, please register one in DI.");
            return;
        }
        foreach(var icon in Icons) {
            await LoadIconConcurrentAsync(icon);
        }
        // Above loop can be done concurrently as follows.  However, in practice it appears to be
        // worse for the user experience.  By blocking all fetch threads, some javascript is
        // deferred causing the entire page to stay blank longer.  Leaving as it might change...
        //var loadTasks = Icons.Select(LoadIconConcurrentAsync);
        //await Task.WhenAll(loadTasks);
        loaded = true;
        ThemeInfo.Loading = false;
    }

    private async Task LoadIconConcurrentAsync(IconInfo icon)
    {
        if(CachedIcons.ContainsKey(icon.Key)) {
            Logger.LogDuplicateIcon(icon.Key);
            return;
        }
        if(!CachedIcons.TryAdd(icon.Key, icon)) {
            Logger.LogConsoleError("Failed to add icon to the cache concurrently.");
            return;
        }
        try {
            if(icon == null || icon.ImagePath == null || icon.SvgRenderType == SvgRenderType.Reference 
                || !icon.ImagePath.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase)) {
                return;
            }
            Logger.LogLoadingIcon(icon.Key, icon.ImagePath);
            var content = await Http.GetStringAsync(icon.ImagePath);
            icon.SvgInlineBody = content;
            if(icon.SvgRenderType == SvgRenderType.Document) {
                var svgTag = SvgTagRegex().Match(content).Value;
                var viewBox = ViewBoxRegex().Match(svgTag).Value;
                var svgBody = SvgTagRegex().Replace(content, "").Replace("</svg>", "");
                var symbol = $@"<symbol id=""{icon.Key}"" {viewBox}>{svgBody}</symbol>";
                var defs = DefsRegex().Match(svgBody).Value;
                if(!string.IsNullOrWhiteSpace(defs)) {
                    // defs included in the SVG, typically for gradient fills and the like.  
                    // Need to keep these defs inline and not in the symbol.
                    symbol = symbol.Replace(defs, "");
                }
                icon.SvgDatabaseBody = symbol;
                icon.SvgInlineBody = $@"<svg class=""{icon.CssClass}""><use href=""#{icon.Key}""></use>{defs}</svg>";
            }
        }
        catch(Exception ex) {
            Logger.LogIconFailed(icon.Key, icon.ImagePath, ex);
        }
    }

    private static bool loaded;

    private static readonly ConcurrentDictionary<string, IconInfo> CachedIcons = new();

    [Inject]
    private HttpClient Http { get; set; } = null!;

    [Inject]
    private ILogger<Theme> Logger { get; set; } = null!;

    [GeneratedRegex(@"<svg[^>]*>")]
    private partial Regex SvgTagRegex();

    [GeneratedRegex(@"viewBox=""[\d\s.]*""")]
    private partial Regex ViewBoxRegex();

    [GeneratedRegex(@"<defs.*</defs>", RegexOptions.Singleline)]
    private partial Regex DefsRegex();

    private IEnumerable<IconInfo> SvgIcons => Icons
        ?.Where(e => !string.IsNullOrEmpty(e.SvgDatabaseBody))
        ?? Array.Empty<IconInfo>();
}

