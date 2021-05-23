﻿#nullable enable

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.ExtraDry {
    public partial class DryContent : ComponentBase {

        [Parameter]
        public ContentLayout? Content { get; set; }

        [Parameter]
        public string? ContentName { get; set; }

        [Inject]
        private IJSRuntime? JSRuntime { get; set; }

        [Command]
        public async Task StartEdit()
        {
            if(Content == null) {
                return;
            }
            foreach(var section in Content.Sections) {
                foreach(var container in section.Containers) {
                    await JSRuntime!.InvokeVoidAsync("startEditing", container.Id);
                }
            }
        }

        [Command]
        public void AddSection()
        {
            if(Content == null) {
                return;
            }
            Content.Sections.Add(new ContentSection { 
                Containers = { 
                    new ContentContainer { Html = "New Section" } 
                } 
            });
            StateHasChanged();
        }

        [Command]
        public async Task ToggleBold()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleBold");
        }

        [Command]
        public async Task ToggleItalic()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleItalic");
        }

        [Command(Name = "H1")]
        public async Task ToggleHeader1()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleHeader", 1);
        }

        [Command(Name = "H2")]
        public async Task ToggleHeader2()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleHeader", 2);
        }

        [Command(Name = "H3")]
        public async Task ToggleHeader3()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleHeader", 3);
        }

        [Command(Name = "H4")]
        public async Task ToggleHeader4()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleHeader", 4);
        }

        [Command(Name = "H5")]
        public async Task ToggleHeader5()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleHeader", 5);
        }

        [Command(Name = "H6")]
        public async Task ToggleHeader6()
        {
            await JSRuntime!.InvokeVoidAsync("roosterToggleHeader", 6);
        }

        [Command]
        public async Task Sanitize()
        {
            await JSRuntime!.InvokeVoidAsync("roosterTestSanitize", 0);
            //await JSRuntime!.InvokeVoidAsync("roosterSanitize");
        }



        public ContentSection? CurrentSection { get; set; }

        public ContentContainer? CurrentContainer { get; set; }

        public ContentTheme CurrentSectionTheme {
            get => CurrentSection?.Theme ?? ContentTheme.Light;
            set {
                if(CurrentSection != null) {
                    CurrentSection.Theme = value;
                    StateHasChanged();
                }
            }
        }

        public SectionLayout CurrentSectionLayout {
            get => CurrentSection?.Layout ?? SectionLayout.Single;
            set {
                if(CurrentSection != null) {
                    CurrentSection.Layout = value;
                    StateHasChanged();
                }
            }
        }

        public ContentAlignment CurrentContainerAlignment {
            get => CurrentContainer?.Alignment ?? ContentAlignment.TopLeft;
            set {
                if(CurrentContainer != null) {
                    CurrentContainer.Alignment = value;
                    StateHasChanged();
                }
            }
        }

        public ContentPadding CurrentContainerPadding {
            get => CurrentContainer?.Padding ?? ContentPadding.None;
            set {
                if(CurrentContainer != null) {
                    CurrentContainer.Padding = value;
                    StateHasChanged();
                }
            }
        }

        private void EditorFocus(ContentSection section, ContentContainer container, FocusEventArgs args)
        {
            CurrentSection = section;
            CurrentContainer = container;
            StateHasChanged();
        }

        private async Task EditorPaste(ContentSection section, ContentContainer container, ClipboardEventArgs args)
        {
            //Console.WriteLine($"Editor paste: {args.Type}, {container.Id}");
            //await Task.Delay(16);
            //var content = await JSRuntime!.InvokeAsync<string>("roosterGetContent", container.Id.ToString());
            //Console.WriteLine(content);
        }

    }
}
