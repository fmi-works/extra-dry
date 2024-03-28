namespace ExtraDry.Core;

/// <summary>
/// A version sub-object for objects in the ORM that are versioned.
/// </summary>
/// <remarks>
/// Works in conjunction with overloads in the AspectDbContext and VersionInfoAspect.
/// </remarks>
public class VersionInfo
{

    /// <summary>
    /// The date the object was first created.
    /// This is automatically populated with the current UTC time when the object is added to the database.
    /// </summary>
    public DateTime DateCreated { get; set; } = DateTime.MinValue;

    /// <summary>
    /// The username of the user that create this object.
    /// This is automatically populated with the currently signed in user when the object is added to the database.
    /// This is null if the object is not yet added to the database.
    /// This is "anonymous" if the object was created when a user was not logged in.
    /// </summary>
    [StringLength(MaxEmailLength)]
    public string UserCreated { get; set; } = string.Empty;

    /// <summary>
    /// The date the object was last modified.
    /// This is automatically populated with the current UTC time when the object is updated to the database.
    /// </summary>
    public DateTime DateModified { get; set; } = DateTime.MinValue;

    /// <summary>
    /// The username of the user that last modified this object.
    /// This is automatically populated with the currently signed in user when the object is added or updated to the database.
    /// This is null if the object is not yet added to the database.
    /// This is "anonymous" if the object was last modified when a user was not logged in.
    /// </summary>
    [StringLength(MaxEmailLength)]
    public string UserModified { get; set; } = string.Empty;

    /// <summary>
    /// Updates the timestamps and users, called when the enclosing object is modified.
    /// Typically this is only ever called by the database context during an addition / modification.
    /// </summary>
    public void UpdateVersion()
    {
        if(SuppressUpdates) {
            return;
        }
        if(DateCreated == DateTime.MinValue) {
            DateCreated = CurrentTimestamp;
            UserCreated = Truncate(CurrentUsername(), MaxEmailLength);
        }
        DateModified = CurrentTimestamp;
        UserModified = Truncate(CurrentUsername(), MaxEmailLength);
    }

    /// <summary>
    /// Used by the VersionInfoAspect partner in the database context, shouldn't be used otherwise.
    /// </summary>
    public static void ResetTimestamp()
    {
        CurrentTimestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// The timestamp that was used for the most recent `SaveChanges` or `SaveChangesAsync`.
    /// </summary>
    public static DateTime CurrentTimestamp { get; private set; } = DateTime.MinValue;

    /// <summary>
    /// The username that is set for any creation or modification.
    /// Set during login.
    /// </summary>
    public static Func<string> CurrentUsername { get; set; } = () => "anonymous";

    /// <summary>
    /// When true, suppresses any automatic version updates, for example during migrations.
    /// </summary>
    public static bool SuppressUpdates { get; set; }

    /// <summary>
    /// The maximum length of an e-mail address, above which the version log will be silently truncated.
    /// See: https://www.freshaddress.com/blog/long-email-addresses/
    /// </summary>
    public const int MaxEmailLength = 80;

    private static string Truncate(string value, int length) => value.Length > length ? value.Substring(0, length) : value;

    /// <summary>
    /// User readable string representation, UI fallback.
    /// </summary>
    public override string ToString() => $"Updated by {UserModified} {DataConverter.DateToRelativeTime(DateModified)}";
}
