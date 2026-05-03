using LABsistem.Application.Services;
using LABsistem.Dal.Db;
using Microsoft.EntityFrameworkCore;

public class DatabaseRevokedTokenStoreTests
{
    private static LabSistemDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<LabSistemDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new LabSistemDbContext(options);
    }

    [Fact]
    public async Task RevokeAsync_PersistsRevokedTokenAcrossNewStoreInstance()
    {
        var databaseName = Guid.NewGuid().ToString();
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(10);

        await using (var context = CreateContext(databaseName))
        {
            var store = new DatabaseRevokedTokenStore(context);
            await store.RevokeAsync("shared-jti", expiresAtUtc);
        }

        await using (var secondContext = CreateContext(databaseName))
        {
            var secondStore = new DatabaseRevokedTokenStore(secondContext);
            var isRevoked = await secondStore.IsRevokedAsync("shared-jti");

            Assert.True(isRevoked);
        }
    }
}
