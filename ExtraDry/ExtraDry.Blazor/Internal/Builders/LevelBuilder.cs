﻿namespace ExtraDry.Blazor.Internal;

/// <summary>
/// A level builder as used by the <see cref="QueryBuilder"/>.  Supports expanding and collapsing
/// the level of hierarchy that is returned without overstepping the minimum and maximum 
/// known bounds.
/// </summary>
public class LevelBuilder
{
    /// <summary>
    /// The current level of the hierarchy for the collection, or 0 for all levels.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// If known, the maximum level of the hierarchy for the collection, ignoring any Level filter 
    /// that may be applied.
    /// </summary>
    public int MaxLevel { get; private set; }

    /// <summary>
    /// When collections are returned, the max level might be updated down because of an explicit
    /// level.  Keep the information on the max level exclusive of the level filter.
    /// </summary>
    public void UpdateMaxLevel(int maxLevel)
    {
        if(MaxLevel == 0 || maxLevel == 0 || maxLevel > MaxLevel) {
            MaxLevel = maxLevel;
        }
    }

    /// <summary>
    /// Expands the level filter by one level, if possible.
    /// </summary>
    public bool Expand()
    {
        var lastLevel = Level;
        Level = (Level, MaxLevel) switch {
            (0, 0) => 0,
            (0, _) => MaxLevel,
            (_, 0) => Level + 1,
            (_, _) => Math.Min(Level + 1, MaxLevel),
        };
        return lastLevel != Level;
    }

    /// <summary>
    /// Decreases the level filter by one level, if possible.
    /// </summary>
    public bool Collapse()
    {
        var lastLevel = Level;
        Level = (Level, MaxLevel) switch {
            (0, 0) => 0,
            (0, _) => MaxLevel - 1,
            (_, 0) => Level - 1,
            (_, _) => Math.Max(Level - 1, 1),
        };
        return lastLevel != Level;
    }

    /// <summary>
    /// Build the level filter as a string ready for posting back to the API.
    /// </summary>
    public int? Build() => Level == 0 ? null : Level;

    public void Reset()
    {
        Level = 0;
        MaxLevel = 0;
    }
}
