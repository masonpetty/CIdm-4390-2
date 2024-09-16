using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeService.Tests.Utilities;
using RazorPagesTestSample.Data;
using Xunit;

namespace PrimeService.Tests.UnitTests
{
    public class DataAccessLayerTest
    {
        [Fact]
        public async Task DeleteMessageAsync_MessageIsDeleted_WhenMessageIsFound()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var seedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(seedMessages);
                await db.SaveChangesAsync();
                var recId = 1;
                var expectedMessages = 
                    seedMessages.Where(message => message.Id != recId).ToList();

                // Act
                await db.DeleteMessageAsync(recId);

                // Assert
                var actualMessages = await db.Messages.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
            }
        }

        [Fact]
        public async Task DeleteMessageAsync_NoMessageIsDeleted_WhenMessageIsNotFound()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expectedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(expectedMessages);
                await db.SaveChangesAsync();
                var recId = 4;

                // Act
                try
                {
                    await db.DeleteMessageAsync(recId);
                }
                catch
                {
                    // recId doesn't exist
                }

                // Assert
                var actualMessages = await db.Messages.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
            }
        }
    }
}
