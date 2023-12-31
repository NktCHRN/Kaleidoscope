﻿using DataAccess.Persistence;
using WebApi.IntegrationTests.Stubs;
using WebApi.IntegrationTests.TestDataHelpers;
using Xunit;

namespace WebApi.IntegrationTests.Abstractions;
[Collection("Test collection")]
public class ControllerTestsBase : IAsyncLifetime
{
    protected HttpClient HttpClient => _factory.HttpClient;
    protected BlobStorageTestDataHelper BlobStorageTestDataHelper => _factory.BlobStorageDataHelper;
    protected DatabaseTestDataHelper DatabaseTestDataHelper => _factory.DatabaseTestDataHelper;
    protected TestTimeProvider TimeProvider => _factory.TestTimeProvider;
    protected TestAuthUser User => _factory.TestUser;

    private readonly CustomWebApplicationFactory _factory;

    public ControllerTestsBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    protected Task AccessDatabaseAsync(Func<ApplicationDbContext, Task> func) 
        => _factory.AccessDatabaseAsync(func);

    protected Task<T> AccessDatabaseAsync<T>(Func<ApplicationDbContext, Task<T>> func) =>
        _factory.AccessDatabaseAsync(func);

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetTestDataAsync();
        _factory.ResetStubs();
    }
}
