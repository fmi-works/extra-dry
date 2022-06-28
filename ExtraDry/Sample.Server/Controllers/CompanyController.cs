﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Server.Controllers;

/// <summary>
/// Manages the collection of companies.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = ApiGroupNames.SampleApi)]
[ApiExceptionStatusCodes]
public class CompanyController {
        
    /// <summary>
    /// Standard DI Constructor
    /// </summary>
    public CompanyController(CompanyService companyService)
    {
        companies = companyService;
    }

    /// <summary>
    /// Filtered list of all companies
    /// </summary>
    /// <remarks>
    /// Provides a complete list of all companies, as this list is not too large, all are returned on every call.
    /// </remarks>
    [HttpGet("api/companies"), Produces("application/json")]
    [AllowAnonymous]
    public async Task<FilteredCollection<Company>> List([FromQuery] FilterQuery query)
    {
        return await companies.List(query);
    }

    /// <summary>
    /// Retrieve a specific company
    /// </summary>
    [HttpGet("api/companies/{companyId}"), Produces("application/json")]
    [AllowAnonymous]
    public async Task<Company> Retrieve(Guid companyId)
    {
        return await companies.RetrieveAsync(companyId);
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    /// <remarks>
    /// Create a new company at the URI, the uniqueId in the URI must match the Id in the payload.
    /// </remarks>
    [HttpPost("api/companies"), Consumes("application/json")]
    [Authorize(SamplePolicies.SamplePolicy)]
    public async Task Create(Company value)
    {
        await companies.Create(value);
    }

    /// <summary>
    /// Update an existing company
    /// </summary>
    /// <remarks>
    /// Update the company at the URI, the uniqueId in the URI must match the Id in the payload.
    /// </remarks>
    [HttpPut("api/companies/{companyId}"), Consumes("application/json")]
    [Authorize(SamplePolicies.SamplePolicy)]
    public async Task Update(Guid companyId, Company value)
    {
        if(companyId != value?.Uuid) {
            throw new ArgumentMismatchException("ID in URI must match body.", nameof(companyId));
        }
        await companies.Update(value);
    }

    /// <summary>
    /// Delete an existing company
    /// </summary>
    [HttpDelete("api/companies/{companyId}")]
    [Authorize(SamplePolicies.SamplePolicy)]
    public async Task Delete(Guid companyId)
    {
        await companies.Delete(companyId);
    }

    private readonly CompanyService companies;
}
