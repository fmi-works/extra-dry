﻿namespace ExtraDry.Blazor;

/// <summary>
/// Indicates that the specified method, typically on a ViewModel, is a valid command.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandAttribute : Attribute {

    /// <summary>
    /// Flag the method as a command with the specified functional command context.
    /// </summary>
    public CommandAttribute(CommandContext context = CommandContext.Alternate)
    {
        Context = context;
    }

    /// <summary>
    /// The context of the command, typically `Alternate`.  On forms, one command should 
    /// be `Primary`.
    /// </summary>
    public CommandContext Context { get; set; } 

    /// <summary>
    /// The name of the command which is displayed on the button. If not set, the name is inferred 
    /// from the method's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The name of an icon to be applied to the command. This will add an Icon to the button with 
    /// the key from this property.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// The name of an icon that indicates the visual affordance of the button.  This will add an 
    /// icon to the end of the button to indicate the action of the button. For example a downward
    /// pointing chevron for a select list, or a calendar page for a date picker.
    /// </summary>
    public string? Affordance { get; set; }

    /// <summary>
    /// An optional category used to create filtered subsets of commands.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// The relative order of this button amongst others. This order is evaluated after groupings 
    /// based on `Context` and `Collapse` settings.
    /// </summary>
    /// <remarks>
    /// TODO: Implement explicit order on buttons
    /// </remarks>
    public int Order { get; set; }

    /// <summary>
    /// Indicates the desired collapse behavior for the command when displayed with other commands in a bar
    /// and the screen width is limited.
    /// </summary>
    /// <remarks>
    /// TODO: Implement Collapse on Button Bar
    /// </remarks>
    public CommandCollapse Collapse { get; set; }
}
