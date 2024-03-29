﻿#nullable enable

using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Threading;
using System.Threading.Tasks;

namespace ExtraDry.Blazor;

public interface IListService<T> : IOptionProvider<T> {

    string UriTemplate { get; set; }

    object[] UriArguments { get; set; }

    int FetchSize { get; set; }

    ValueTask<ItemsProviderResult<T>> GetItemsAsync(string? filter, string? sort, bool? ascending, int? skip, int? take, CancellationToken cancellationToken = default);

}
