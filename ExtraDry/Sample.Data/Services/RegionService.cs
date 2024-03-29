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
            .QueryWith(new(), e => e.Ancestors.Any(f => f.Code == code && (int)f.Level + 1 == (int)e.Level))
            .ToFilteredCollectionAsync();
    }

    public async Task CreateAsync(Region item)
    {
        var parent = await TryRetrieveAsync(item.ParentCode);
        item.SetParent(parent);
        database.Regions.Add(item);
        await database.SaveChangesAsync();
    }

    public async Task<Region> RetrieveAsync(string code)
    {
        var result = await TryRetrieveAsync(code);
        if(result == null) {
            throw new ArgumentOutOfRangeException(nameof(code));
        }
        return result;
    }

    public async Task<Region?> TryRetrieveAsync(string code)
    {
        return await database.Regions
            .Include(e => e.Ancestors)
            .FirstOrDefaultAsync(e => e.Code == code);
    }

    public async Task UpdateAsync(string code, Region item)
    {
        var existing = await RetrieveAsync(code);
        if(existing.ParentCode != item.ParentCode) {
            var parent = await RetrieveAsync(item.ParentCode);
            existing.SetParent(parent);
        }
        await rules.UpdateAsync(item, existing);
        await database.SaveChangesAsync();
    }

    public async Task DeleteAsync(string code)
    {
        var existing = await RetrieveAsync(code);
        rules.Delete(existing, () => database.Regions.Remove(existing));
        await database.SaveChangesAsync();
    }

    private readonly SampleContext database;

    private readonly RuleEngine rules;

}
