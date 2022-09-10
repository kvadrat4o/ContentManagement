using ContentManagement.Core;
using ContentManagement.Core.Infrastructure;
using OneBitSoftware.Utilities;

namespace ContentManagement.UnitTests
{
    public class ContentManagerTests
    {
        #region Sample Data

        public static IEnumerable<object[]> GetCancellationTokenData()
        {
            yield return new object[] { Guid.NewGuid(), new CancellationToken(true) }; 
        }

        public static IEnumerable<object[]> GetGuidData()
        {
            yield return new object[] { new Guid("6d58e569-6ae7-48c3-bb0f-8b41df0e9655") };
        }

        public static IEnumerable<object[]> GetInvalidGuidData()
        {
            yield return new object[] { Guid.Empty };
        }

        public static IEnumerable<object[]> GetGuidAndCancellationTokenData()
        {
            yield return new object[] { Guid.NewGuid(), new CancellationToken() };
        }

        public static IEnumerable<object[]> GetGuidAndTokenData()
        {
            yield return new object[] { new Guid("6d58e569-6ae7-48c3-bb0f-8b41df0e9655"), new CancellationToken() };
        }

        public static IEnumerable<object[]> GetInvalidGuidAndTokenData()
        {
            yield return new object[] { Guid.Empty, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreOrUpdateGuidData()
        {
            yield return new object[] { new Guid("6d58e569-6ae7-48c3-bb0f-8b41df0e9655"), new StreamInfo(), new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreOrUpdateCancellationTokenData()
        {
            yield return new object[] { Guid.NewGuid(), new StreamInfo(), new CancellationToken(true) };
        }

        public static IEnumerable<object[]> GetStoreOrUpdateInvalidGuidData()
        {
            yield return new object[] { Guid.Empty, new StreamInfo(), new CancellationToken() };
        }

        #endregion

        #region ExistsAsync Tests

        [Theory]
        [MemberData(nameof(GetInvalidGuidData))]
        public async Task ExistsAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            var exists = await manager.ExistsAsync(id, new CancellationToken());

            Assert.False(exists.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task ExistsAsync_Should_Return_True_If_Id_Exists(Guid id)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = @"C:\\Users\\d.vasilev\\Documents\6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var exists = await manager.ExistsAsync(id, new CancellationToken());

            Assert.True(exists.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetCancellationTokenData))]
        public async Task ExistsAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.ExistsAsync(id, token));
        }

        #endregion

        #region GetHashAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenData))]
        public async Task GetHashAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.GetHashAsync(id, token));
        }

        [Theory]
        [MemberData(nameof(GetInvalidGuidData))]
        public async Task GetHashAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            var exists = await manager.GetHashAsync(id, new CancellationToken());

            var expected = Guid.Empty.GetHashCode().ToString();

            Assert.Equal(expected, exists.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task GetHashAsync_Should_Return_True_If_Id_Exists(Guid id)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = "C://Users/DVadsilev/6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var exists = await manager.GetHashAsync(id, new CancellationToken());
            var expected = new Guid("6d58e569-6ae7-48c3-bb0f-8b41df0e9655").GetHashCode().ToString();

            Assert.Equal(expected, exists.ResultObject);
        }

        #endregion

        #region GetAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenData))]
        public async Task GetAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.GetAsync(id, token));
        }

        [Theory]
        [MemberData(nameof(GetInvalidGuidData))]
        public async Task GetAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            var file = await manager.GetAsync(id, new CancellationToken());
            var expected = new StreamInfo() { Stream = null };

            Assert.True(expected.Equals(file.ResultObject));
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task GetAsync_Should_Return_True_If_Id_Exists(Guid id)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = "C://Users/DVadsilev/6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var file = await manager.GetAsync(id, new CancellationToken());
            var expected = new StreamInfo() { Stream = new MemoryStream(new byte[5] { 45, 219, 18, 44, 34 })};

            Assert.True(expected.Equals(file.ResultObject));
        }

        #endregion

        #region GetBytesAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenData))]
        public async Task GetBytesAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.GetBytesAsync(id, token));
        }

        [Theory]
        [MemberData(nameof(GetInvalidGuidData))]
        public async Task GetBytesAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            var file = await manager.GetBytesAsync(id, new CancellationToken());
            var expected = new byte[5] { 45, 219, 18, 44, 34 };

            Assert.Equal(expected, file.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task GetBytesAsync_Should_Return_True_If_Id_Exists(Guid id)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = "C://Users/DVadsilev/6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var file = await manager.GetBytesAsync(id, new CancellationToken());
            var expected = new byte[5] { 45, 219, 18, 44, 34 };

            Assert.Equal(expected, file.ResultObject);
        }

        #endregion

        #region StoreAsync Tests

        [Theory]
        [MemberData(nameof(GetStoreOrUpdateCancellationTokenData))]
        public async Task StoreAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.StoreAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetStoreOrUpdateInvalidGuidData))]
        public async Task StoreAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.StoreAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetStoreOrUpdateGuidData))]
        public async Task StoreAsync_Should_Return_True_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = "C://Users/DVadsilev/6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var file = (await manager.StoreAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AddSuccessMessage("Successfully added file content!");

            Assert.True(expected.SuccessMessages.Any(m => file.SuccessMessages.Contains(m)));
        }

        #endregion

        #region UpdateAsync Tests

        [Theory]
        [MemberData(nameof(GetStoreOrUpdateCancellationTokenData))]
        public async Task UpdateAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.UpdateAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetStoreOrUpdateInvalidGuidData))]
        public async Task UpdateAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.UpdateAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetStoreOrUpdateGuidData))]
        public async Task UpdateAsync_Should_Return_True_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = "C://Users/DVadsilev/6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var file = (await manager.UpdateAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AddSuccessMessage("Successfully updated file content!");

            Assert.True(expected.SuccessMessages.Any(m => file.SuccessMessages.Contains(m)));
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenData))]
        public async Task DeleteAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken cancellationToken)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await manager.DeleteAsync(id, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetInvalidGuidAndTokenData))]
        public async Task DeleteAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id, CancellationToken cancellationToken)
        {
            var manager = new ContentManager(new ConfigurationOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.DeleteAsync(id, new CancellationToken()));
        }

        [Theory]
        [MemberData(nameof(GetGuidAndTokenData))]
        public async Task DeleteAsync_Should_Return_True_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var configuration = new ConfigurationOptions() { FileServerPath = "C://Users/DVadsilev/6d58e569-6ae7-48c3-bb0f-8b41df0e9655" };
            var manager = new ContentManager(configuration);

            var file = (await manager.DeleteAsync(id, new CancellationToken()));
            var expected = new OperationResult();
            expected.AddSuccessMessage("Successfully deleted file content!");

            Assert.True(expected.SuccessMessages.Any(m => file.SuccessMessages.Contains(m)));
        }

        #endregion
    }
}