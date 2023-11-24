﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OwnershipStructure
{
    Private,
    Public,
    Hybrid
}

[Format(Icon = "company")]
[FactTable, DimensionTable]
[DeleteRule(DeleteAction.Recycle, nameof(Status), CompanyStatus.Deleted, CompanyStatus.Active)]
public class Company : IResourceIdentifiers {

    [Key]
    [JsonIgnore]
    [Rules(RuleAction.Ignore)]
    public int Id { get; set; }

    [Rules(RuleAction.Ignore)]
    public Guid Uuid { get; set; } = Guid.NewGuid();

    [Display(Name = "Code", GroupName = "Summary")]
    [Filter(FilterType.Equals)]
    [Rules(CreateAction = RuleAction.Allow, UpdateAction = RuleAction.Block)]
    [Required, StringLength(24)]
    public string Slug { get; set; } = string.Empty;

    [NotMapped]
    [Display(GroupName = "Summary")]
    public string Caption => $"Company {Slug}";

    /// <summary>
    /// Official incorporated name of company, as listed in Dun &amp; Bradstreet
    /// </summary>
    /// <example>Alphabet, Inc.</example>
    [Display(Name = "Name", ShortName = "Name", GroupName = "Summary")]
    [Filter(FilterType.Contains)]
    [Rules(RuleAction.IgnoreDefaults)]
    [Required, StringLength(80)]
    public string Title { get; set; } = "";

    [Display(Name = "Status", ShortName = "Status", GroupName = "Status")]
    [Rules(RuleAction.Allow)]
    [Filter]
    public CompanyStatus Status { get; set; }

    [Display]
    [StringLength(500)]
    [Rules(RuleAction.IgnoreDefaults)]
    public string Description { get; set; } = "";

    [Display(Name = "Primary Sector", ShortName = "Sector")]
    [Rules(RuleAction.Link)]
    [JsonConverter(typeof(ResourceReferenceConverter<Sector>))]
    public Sector? PrimarySector { get; set; }

    [Display]
    [Rules(RuleAction.Link)]
    public List<Sector> AdditionalSectors { get; set; } = new();

    [Rules(RuleAction.Allow)]
    [Filter]
    public OwnershipStructure Ownership { get; set; }

    [Display]
    [Phone, StringLength(24)]
    [Rules(RuleAction.IgnoreDefaults)]
    public string ContactPhone { get; set; } = "";

    [Display]
    [EmailAddress, StringLength(100)]
    [Rules(RuleAction.IgnoreDefaults)]
    public string ContactEmail { get; set; } = "";

    [Precision(18, 2)]
    public decimal AnnualRevenue { get; set; }

    [Precision(18, 2)]
    public decimal SalesMargin { get; set; }

    public DateTime IncorporationDate { get; set; }

    [Display]
    [Rules(RuleAction.Allow)]
    public BankingDetails BankingDetails { get; set; } = new BankingDetails();

    //[Display]
    //[Rules(RuleAction.Recurse)]
    //public ICollection<Video> Videos { get; set; } = new Collection<Video>();

    /// <summary>
    /// The version info which informs the audit log.
    /// </summary>
    [Display(GroupName = "Status")]
    [Rules(RuleAction.Block)]
    [JsonIgnore]
    public VersionInfo Version { get; set; } = new VersionInfo();

    [JsonPropertyName("fields")]
    public ExpandoValues CustomFields { get; set; } = new();
}
