﻿namespace Sample.Data.Services;

public class RegionService {

    public RegionService(SampleContext sampleContext, RuleEngine ruleEngine)
    {
        database = sampleContext;
        rules = ruleEngine;
    }

    public async Task<FilteredCollection<Region>> ListAsync(FilterQuery query)
    {
        return await database.Regions
            .Include(e => e.Ancestors)
            .QueryWith(query)
            .ToFilteredCollectionAsync();
    }

    public async Task<FilteredCollection<Region>> ListChildrenAsync(string code)
    {
        return await database.Regions
            .QueryWith(new(), e => e.Ancestors.Any(f => f.Slug == code && (int)f.Level + 1 == (int)e.Level))
            .ToFilteredCollectionAsync();
    }

    public async Task CreateAsync(Region item)
    {
        if(item.Level != RegionLevel.Global) {
            if(item.Parent == null) {
                throw new ArgumentException("A region must have a parent if it is not at the global level.");
            }
            var parent = await TryRetrieveAsync(item.Parent.Slug);
            item.SetParent(parent);
        }
        database.Regions.Add(item);
        await database.SaveChangesAsync();
    }

    public async Task<Region?> TryRetrieveAsync(string code)
    {
        return await database.Regions
            .Include(e => e.Ancestors)
            .FirstOrDefaultAsync(e => e.Slug == code);
    }

    public async Task<Region> RetrieveAsync(string code)
    {
        var result = await TryRetrieveAsync(code)
            ?? throw new ArgumentOutOfRangeException(nameof(code));
        return result;
    }

    public async Task UpdateAsync(string code, Region item)
    {
        var existing = await RetrieveAsync(code);
        if(existing.Parent != null && item.Parent != null && existing.Parent.Slug != item.Parent.Slug) {
            var newParent = await RetrieveAsync(item.Parent.Slug);
            await existing.MoveSubtree(newParent, database);
        }
        await rules.UpdateAsync(item, existing);
        await database.SaveChangesAsync();
    }

    public async Task DeleteAsync(string code)
    {
        var existing = await RetrieveAsync(code);
        await rules.DeleteAsync(existing, () => database.Regions.Remove(existing), () => database.SaveChangesAsync());
    }

    public async Task RestoreAsync(string code)
    {
        var existing = await RetrieveAsync(code);
        await rules.RestoreAsync(existing);
        await database.SaveChangesAsync();
    }

    private readonly SampleContext database;

    private readonly RuleEngine rules;

}
