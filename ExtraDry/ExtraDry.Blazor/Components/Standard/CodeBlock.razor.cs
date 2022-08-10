﻿#nullable enable

using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Diagnostics.CodeAnalysis;

namespace ExtraDry.Blazor;

public partial class CodeBlock : ComponentBase {

    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    [Parameter]
    public string Lang { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Normalize { get; set; } = true;

    protected override void OnParametersSet()
    {
        if(Normalize) {
            RenderChildContentToBody();
            var lines = Body.Split('\n').ToList();
            FormatLines(lines);
            Body = string.Join("\n", lines);
        }

    }

    private void FormatLines(List<string> lines)
    {
        while(lines.Any() && string.IsNullOrWhiteSpace(lines.First())) {
            lines.RemoveAt(0);
        }
        while(lines.Any() && string.IsNullOrWhiteSpace(lines.Last())) {
            lines.RemoveAt(lines.Count - 1);
        }
        if(lines.Count == 0) {
            return;
        }
        if(lines.Count == 1) {
            lines[0] = lines[0].Trim();
            return;
        }
        var indentLines = lines.Skip(1).Where(e => !string.IsNullOrWhiteSpace(e));
        var globalIndent = indentLines.Min(e => LeadingSpaces(e));
        for(int i = 0; i < lines.Count; ++i) {
            var line = lines[i];
            if(line.Length > globalIndent && char.IsWhiteSpace(line[0])) {
                lines[i] = line[globalIndent..];
            }
            // TODO: Future color syntax highlight.
            //lines[i] = lines[i].Replace("public", "<span style=\"color: blue;\">public</span>");
        }

        static int LeadingSpaces(string s) => s.TakeWhile(e => char.IsWhiteSpace(e)).Count();
    }

    [SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "Alternative is to write JavaScript post-processing step.")]
    private void RenderChildContentToBody()
    {
        if(ChildContent == null) {
            return;
        }
        var builder = new RenderTreeBuilder();
        ChildContent.Invoke(builder);
        var frames = builder.GetFrames();
        Body = string.Join("", frames.Array
            .Where(e => e.FrameType == RenderTreeFrameType.Text || e.FrameType == RenderTreeFrameType.Markup)
            .Select(e => e.TextContent));
        //foreach(var frame in frames.Array) {
        //    Console.WriteLine(frame.FrameType);
        //    if(frame.FrameType == RenderTreeFrameType.Text || frame.FrameType == RenderTreeFrameType.Markup) {
        //        Console.WriteLine(frame.TextContent);
        //    }
        //}
        Console.WriteLine(Body);
    }

    private string Body { get; set; } = string.Empty;

    private MarkupString MarkupBody => (MarkupString)Body;
    
}
