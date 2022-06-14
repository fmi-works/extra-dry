using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ExtraDry.Server;

/// <summary>
/// Provides business rule logic for Creating, Updating and Deleting objects from untrusted sources.
/// The untrusted source is usually an object deserialized from JSON from an API or MVC call. 
/// The rules for overwriting, ignoring, or blocking changes are defined by applying the `RuleAttribute` to each property.
/// The `RuleEngine` should then be dependency injected into services where its methods will consistently apply business rules.
/// </summary>
public class RuleEngine {

    /// <summary>
    /// Creates a new RuleEngine, typically only called from the DI service.
    /// The IServiceProvider is used to further discover IEntityResolver objects for cases where the rule engine
    /// is attempting to link to an existing object.
    /// </summary>
    public RuleEngine(IServiceProvider serviceProvider)
    {
        scopedServices = serviceProvider;
    }

    /// <summary>
    /// Given an potentially untrusted and unvalidated exemplar of an object, create a new copy of that object with business rules applied.
    /// Any validation issues or rule violations will throw an exception.
    /// </summary>
    public async Task<T> CreateAsync<T>(T exemplar)
    {
        if(exemplar == null) {
            throw new ArgumentNullException(nameof(exemplar));
        }
        var validator = new DataValidator();
        validator.ValidateObject(exemplar);
        validator.ThrowIfInvalid();
        var destination = Activator.CreateInstance<T>();
        await UpdatePropertiesAsync(exemplar, destination, MaxRecursionDepth, e => e.CreateAction);
        return destination;
    }

    /// <summary>
    /// Updates the `destination` with properties from `source`, while applying business logic rules in annotations.
    /// The source object will be validated using data annotations first, then properties will be copied across, where:
    ///   `JsonIgnore` properties will be skipped, these are typically empty as the source has likely been deserialized from the network;
    ///   `Rules(UpdateAction.BlockChanges)` annotations will throw an exception if a change is attempted;
    ///   `Rules(UpdateAction.Ignore)` annotations will not be copied;
    ///   `Rules(UpdateAction.IgnoreDefaults)` annotations will not be copied if the source property is `null` or `default`.
    /// </summary>
    public async Task UpdateAsync<T>(T source, T destination)
    {
        if(source == null) {
            throw new ArgumentNullException(nameof(source));
        }
        if(destination == null) {
            throw new ArgumentNullException(nameof(destination));
        }
        var validator = new DataValidator();
        validator.ValidateObject(source);
        validator.ThrowIfInvalid();
        await UpdatePropertiesAsync(source, destination, MaxRecursionDepth, e => e.UpdateAction);
    }

    /// <summary>
    /// When copying an object (during create or update) that allows for nested objects, the number
    /// of nesting that the system will stop at.  This will prevent recursion issues when two objects reference
    /// each other.  This can be increased if necessary for large trees of data.
    /// </summary>
    public int MaxRecursionDepth { get; set; } = 20;

    private async Task UpdatePropertiesAsync<T>(T source, T destination, int depth, Func<RulesAttribute, RuleAction> selector)
    {
        if(depth == 0) {
            throw new DryException("Recursion limit on update reached");
        }
        var properties = typeof(T).GetProperties();
        foreach(var property in properties) {
            var ignore = property.GetCustomAttribute<JsonIgnoreAttribute>();
            var action = EffectiveRule(property, ignore, selector, RuleAction.Allow);
            if(action == RuleAction.Ignore) {
                continue;
            }
            (var sourceValue, var destinationValue) = GetPropertyValues(property, source, destination);
            if(sourceValue == null && destinationValue == null) {
                // Common occurrence which we can short circuit all future logic for performance.
                // Also allows processing below to ignore this case.
            }
            else if(typeof(IList).IsAssignableFrom(property.PropertyType)) {
                var sourceList = sourceValue as IList;
                await ProcessCollectionUpdates(action, property, destination, sourceList);
            }
            else {
                await ProcessIndividualUpdate(action, property, destination, sourceValue, depth, ignore, selector);
            }
        }
    }

    private static (object? sourceValue, object? destinationValue) GetPropertyValues<T>(PropertyInfo property, T source, T destination)
    {
        try {
            var sourceValue = source != null ? property.GetValue(source) : null;
            var destinationValue = property.GetValue(destination);
            return (sourceValue, destinationValue);
        }
        catch(TargetParameterCountException) {
            return (null, null);
        }
    }

    private static RuleAction EffectiveRule(PropertyInfo property, JsonIgnoreAttribute? ignore, Func<RulesAttribute, RuleAction> selector, RuleAction defaultType)
    {
        var rules = property.GetCustomAttribute<RulesAttribute>();
        if(property.SetMethod == null) {
            return RuleAction.Ignore;
        }
        if(rules != null) {
            return selector(rules);
        }
        else if(ignore?.Condition == JsonIgnoreCondition.Always) {
            return RuleAction.Ignore;
        }
        else {
            return defaultType;
        }
    }

    private async Task ProcessIndividualUpdate<T>(RuleAction action, PropertyInfo property, T destination, object? value, int depth, JsonIgnoreAttribute? ignore, Func<RulesAttribute, RuleAction> selector)
    {
        // Check against null for object types and GetDefaultValue for boxed value types.
        if(action == RuleAction.IgnoreDefaults && (value == null || value.Equals(property.PropertyType.GetDefaultValue()))) {
            return;
        }
        (var resolved, var result) = await ResolveEntityValue(property.PropertyType, value);
        var destinationValue = property.GetValue(destination);
        if(action == RuleAction.Link && !resolved) {
            // Copy object reference
            property.SetValue(destination, result);
        }
        else {
            var same = (result == null && destinationValue == null) || (result?.Equals(destinationValue) ?? false);
            if(action == RuleAction.Block && !same) {
                if(value == null || value == default || ignore?.Condition == JsonIgnoreCondition.Always) {
                    return;
                }
                throw new DryException($"Invalid attempt to change property '{property.Name}'", $"Attempt to change read-only property '{property.Name}'");
            }

            if(!resolved && !property.PropertyType.IsValueType && property.PropertyType != typeof(string)) {
                // Recurse through child to copy values
                if(value != null && destinationValue == null) {
                    destinationValue = Activator.CreateInstance(value.GetType());
                    property.SetValue(destination, destinationValue);
                }
                if(destinationValue != null) {
                    // if the source value is null bypass the update and set the destination value to null.
                    if(value == null) {
                        property.SetValue(destination, null);
                    }
                    else {
                        await UpdatePropertiesAsync((dynamic?)value, (dynamic)destinationValue, --depth, selector);
                    }
                }
            }
            else {
                property.SetValue(destination, result);
            }
        }
    }

    private async Task ProcessCollectionUpdates<T>(RuleAction action, PropertyInfo property, T destination, IList? sourceList)
    {
        var destinationValue = property.GetValue(destination);
        var destinationList = destinationValue as IList;
        if(destinationList == null && sourceList != null) {
            destinationList = Activator.CreateInstance(property.PropertyType) as IList;
            property.SetValue(destination, destinationList);
        }
        if(action == RuleAction.IgnoreDefaults && sourceList == null) {
            // Don't modify destination as source is in default state of null (note that an empty collection will change destination)
            return;
        }
        var sourceEntities = new List<object?>();
        if(sourceList != null) {
            var listItemType = property.PropertyType.GetGenericArguments()[0];
            foreach(var item in sourceList) {
                (var _, var value) = await ResolveEntityValue(listItemType, item);
                sourceEntities.Add(value);
            }
        }
        if(destinationList != null) {
            var destObjects = destinationList.Cast<object>();
            var toRemove = destObjects.Except(sourceEntities).ToList();
            var toAdd = sourceEntities.Except(destObjects).ToList();
            if(action == RuleAction.Block && (toRemove.Any() || toAdd.Any())) {
                throw new DryException($"Invalid attempt to change collection property {property.Name}", $"Attempt to change read-only collection property '{property.Name}'");
            }
            foreach(var item in toRemove) {
                destinationList.Remove(item); // Only null if null-to-null copy which is handled.
            }
            foreach(var item in toAdd) {
                destinationList.Add(item);
            }
        }
    }

    /// <summary>
    /// Processes a delete of an item setting the appropriate fields.
    /// If any fields have `DeleteValue` set on their `RuleAttribute`, then the item is soft-deleted and the method return `SoftDeleted`.
    /// Otherwise, it executes the indicated `hardDeletePrepare` action (typically remove from collection, without commit) and
    /// the method returns `HardDeleted`.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    public DeleteResult Delete<T>(T item, Action? hardDeletePrepare = null)
    {
        return DeleteSoft(item, hardDeletePrepare, null);
    }

    /// <summary>
    /// Processes a delete of an item setting the appropriate fields.
    /// If any fields have `DeleteValue` set on their `RulesAttribute`, then the item is soft-deleted and the method return true.
    /// Otherwise, it executes the indicated `hardDeletePrepare` action (typically remove from collection);
    /// followed by the indicated `hardDeleteCommit` action, if provided.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    public DeleteResult DeleteSoft<T>(T item, Action? hardDeletePrepare, Action? hardDeleteCommit)
    {
        var task = DeleteSoftAsync(item, WrapAction(hardDeletePrepare), WrapAction(hardDeleteCommit));
        CompleteActionMasquardingAsFuncTask(task);
        return task.Result;
    }

    /// <summary>
    /// Processes a delete of an item setting the appropriate fields.
    /// If any fields have `DeleteValue` set on their `RulesAttribute`, then the item is soft-deleted and the method return true.
    /// Otherwise, it executes the indicated `hardDeletePrepare` action (typically remove from collection);
    /// followed by the indicated `hardDeleteCommit` action, if provided.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    public async Task<DeleteResult> DeleteSoftAsync<T>(T item, Action hardDeletePrepare, Func<Task> hardDeleteCommit)
    {
        return await DeleteSoftAsync(item, WrapAction(hardDeletePrepare), hardDeleteCommit);
    }

    /// <summary>
    /// Processes a delete of an item setting the appropriate fields.
    /// If any fields have `DeleteValue` set on their `RulesAttribute`, then the item is soft-deleted and the method return true.
    /// Otherwise, it executes the indicated `hardDeletePrepare` action (typically remove from collection);
    /// followed by the indicated `hardDeleteCommit` action, if provided.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    public async Task<DeleteResult> DeleteSoftAsync<T>(T item, Func<Task> hardDeletePrepare, Action hardDeleteCommit)
    {
        return await DeleteSoftAsync(item, hardDeletePrepare, WrapAction(hardDeleteCommit));
    }

    /// <summary>
    /// Processes a delete of an item setting the appropriate fields.
    /// If any fields have `DeleteValue` set on their `RulesAttribute`, then the item is soft-deleted and the method return true.
    /// Otherwise, it executes the indicated `hardDeletePrepare` action (typically remove from collection);
    /// followed by the indicated `hardDeleteCommit` action, if provided.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Keep as standard service instance style for DI.")]
    public async Task<DeleteResult> DeleteSoftAsync<T>(T item, Func<Task>? hardDeletePrepare, Func<Task>? hardDeleteCommit)
    {
        if(item == null) {
            throw new ArgumentNullException(nameof(item));
        }
        var result = DeleteResult.NotDeleted;
        if(AttemptSoftDelete(item)) {
            result = DeleteResult.SoftDeleted;
        }
        if(result == DeleteResult.NotDeleted) {
            if(hardDeletePrepare == null) {
                throw new InvalidOperationException("Item could not be soft-deleted and no hard delete action was provided.");
            }
            else {
                await hardDeletePrepare();
                if(hardDeleteCommit != null) {
                    await hardDeleteCommit();
                }
                result = DeleteResult.HardDeleted;
            }
        }
        return result;
    }

    /// <summary>
    /// Processes a hard delete of an item if possible, even if a `DeleteValue` is provided in the `RulesAttribute`.
    /// This allows for the database to notify if a hard-delete is not possible by throwing an exception in the commit.
    /// If not possible, then the soft-delete is performed instead.
    /// If neither is possible, an exception is thrown.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    public DeleteResult DeleteHard<T>(T item, Action hardDeletePrepare, Action hardDeleteCommit)
    {
        // Bit hinky, map Actions to Func<Task> so that logic flow isn't repeated, then map back to non-async.
        var task = DeleteHardAsync(item, WrapAction(hardDeletePrepare), WrapAction(hardDeleteCommit));
        CompleteActionMasquardingAsFuncTask(task);
        return task.Result;
    }

    /// <summary>
    /// Processes a hard delete of an item if possible, even if a `DeleteValue` is provided in the `RulesAttribute`.
    /// This allows for the database to notify if a hard-delete is not possible by throwing an exception in the commit.
    /// If not possible, then the soft-delete is performed instead.
    /// If neither is possible, an exception is thrown.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    public async Task<DeleteResult> DeleteHardAsync<T>(T item, Action hardDeletePrepare, Func<Task> hardDeleteCommit)
    {
        return await DeleteHardAsync(item, WrapAction(hardDeletePrepare), hardDeleteCommit);
    }

    /// <summary>
    /// Processes a hard delete of an item if possible, even if a `DeleteValue` is provided in the `RulesAttribute`.
    /// This allows for the database to notify if a hard-delete is not possible by throwing an exception in the commit.
    /// If not possible, then the soft-delete is performed instead.
    /// If neither is possible, an exception is thrown.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="hardDeletePrepare">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="hardDeleteCommit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    public async Task<DeleteResult> DeleteHardAsync<T>(T item, Func<Task> hardDeletePrepare, Action hardDeleteCommit)
    {
        return await DeleteHardAsync(item, hardDeletePrepare, WrapAction(hardDeleteCommit));
    }

    /// <summary>
    /// Processes a hard delete of an item if possible, even if a `DeleteValue` is provided in the `RulesAttribute`.
    /// This allows for the database to notify if a hard-delete is not possible by throwing an exception in the commit.
    /// If not possible, then the soft-delete is performed instead.
    /// If neither is possible, an exception is thrown.
    /// </summary>
    /// <param name="item">The item to delete, a soft-delete is attempted first.</param>
    /// <param name="remove">If item can't be soft-deleted, then the action that is executed for a hard-delete.</param>
    /// <param name="commit">If item can't be soft-deleted, then the optional action that is executed to commit hard-delete.</param>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Keep as standard service instance style for DI.")]
    public async Task<DeleteResult> DeleteHardAsync<T>(T item, Func<Task>? remove, Func<Task>? commit)
    {
        if(item == null) {
            throw new ArgumentNullException(nameof(item));
        }
        if(remove == null) {
            throw new ArgumentNullException(nameof(item), "Must provide an action to prepare hard delete, e.g. remove from collection");
        }
        if(commit == null) {
            throw new ArgumentNullException(nameof(item), "Must provide an action to commit hard delete, e.g. save to database");
        }
        var result = DeleteResult.NotDeleted;
        if(AttemptSoftDelete(item)) {
            await commit();
            result = DeleteResult.SoftDeleted;
        }
        try {
            await remove();
            await commit();
            result = DeleteResult.HardDeleted;
        }
        catch { // Hmmm, can we be specific about exception type here?
            if(result == DeleteResult.NotDeleted) {
                throw new InvalidOperationException("Item could not be hard-deleted and no rules for soft-delete provided.");
            }
            // else was soft-deleted and that's OK...
        }
        return result;
    }

    /// <summary>
    /// Given an object, especially one that has been deserialized, attempts to resolve a version of that object
    /// that might be a database entity.  Uses `IEntityResolver` class in DI to find potential replacement.
    /// This only works for objects, value types are always copies, so sourceValue is always returned.
    /// </summary>
    private async Task<(bool, object?)> ResolveEntityValue(Type type, object? sourceValue)
    {
        if(type.IsValueType || type == typeof(string)) {
            return (false, sourceValue);
        }
        var untypedEntityResolver = typeof(IEntityResolver<>);
        var typedEntityResolver = untypedEntityResolver.MakeGenericType(type);
        var resolver = scopedServices.GetService(typedEntityResolver);
        if(resolver == null) {
            return (false, sourceValue);
        }
        else {
            var method = typedEntityResolver.GetMethod("ResolveAsync");
            if(method == null) {
                throw new DryException($"Resolver '{type.Name}' object missing method ResolveAsync");
            }
            // Force not-null return as ResolveAsync above is not-null return.
            dynamic task = method.Invoke(resolver, new object?[] { sourceValue })!;
            var result = (await task) as object;
            return (true, result);
        }
    }

    private static Func<Task>? WrapAction(Action? action)
    {
        if(action == null) {
            return null;
        }
        else {
            return () => {
                action();
                return Task.CompletedTask;
            };
        }
    }

    private static void CompleteActionMasquardingAsFuncTask(Task task)
    {
        if(task.IsCompleted && task.IsFaulted) {
            throw task.Exception?.InnerException ?? task.Exception ?? new Exception("Aggregate exception occurred but was missing details.");
        }
    }

    private static bool AttemptSoftDelete<T>(T item)
    {
        bool deleted = false;
        var properties = typeof(T).GetProperties();
        foreach(var property in properties) {
            var rule = property.GetCustomAttribute<RulesAttribute>();
            if(rule?.DeleteValue != null) {
                deleted = true;
                property.SetValue(item, rule.DeleteValue);
            }
        }
        return deleted;
    }

    private readonly IServiceProvider scopedServices;

}
