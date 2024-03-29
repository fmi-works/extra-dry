﻿#nullable enable

using ExtraDry.Blazor.Components.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace ExtraDry.Blazor.Components.Raw {

    public partial class DrySingleSelect<T> : ComponentBase {

        [Parameter]
        public T? Model { get; set; }

        [Parameter]
        public PropertyDescription? Property { get; set; }

        [Parameter]
        public List<object>? Values { get; set; } 

        [Parameter]
        public EventCallback<ChangeEventArgs>? OnChange { get; set; }

        [Parameter]
        public string? CssClass { get; set; }

        [Inject]
        private ILogger<DryInput<T>> Logger { get; set; } = null!;

        [CascadingParameter]
        public EditMode EditMode { get; set; } = EditMode.Create;

        protected override void OnParametersSet()
        {
            SelectedValue = Property?.GetValue(Model);
            Logger.LogDebug($"DrySingleSelect initialized with {Values?.Count} values");
        }

        private async Task SelectOption(ChangeEventArgs args)
        {
            Logger.LogDebug($"DrySingleSelect Set Option by Key '{args.Value}'");
            if(Values == null || !int.TryParse(args.Value as string, out var index)) {
                return; // selected blank line
            }
            SelectedValue = Values[index];
            Property?.SetValue(Model, SelectedValue);
            await InvokeOnChange(args);
        }

        private async Task InvokeOnChange(ChangeEventArgs args)
        {
            var task = OnChange?.InvokeAsync(args);
            if(task != null) {
                await task;
            }
        }

        private bool ReadOnly => EditMode == EditMode.ReadOnly;

        private object? SelectedValue { get; set; }

    }

}
