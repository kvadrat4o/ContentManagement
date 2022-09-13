
namespace ContentManagement.UnitTests
{
    public class ContentManagerUnitTests : IClassFixture<UnitTestsFixture>
    {
        #region Fields

        private readonly IContentManager<Guid> _manager;
        private readonly MockFileSystem _fileSystem;
        private readonly UnitTestsFixture _testsFixture;

        #endregion

        #region Utilities

        private static Stream GetStreamForFIle(Guid guid)
        {
            var file = new DirectoryInfo(@"\\MSISSOF293\Users\d.vasilev\Documents" ).GetFiles("Blocked_2022-08-29_09-32-08_DLGvpYS5pNVL" + ".*").FirstOrDefault();
            byte[] result = new byte[0];

            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                using (BinaryReader binaryReader = new BinaryReader(fs))
                    result = binaryReader.ReadBytes((int)fs.Length);

            return new MemoryStream(result);
        }

        private string GetHashFromByteArrayAsync(byte[] buffer)
        {
            byte[] _emptyBuffer = new byte[0];
            var hasher = SHA256.Create();
            hasher.Initialize();

            int readBytes;

            hasher.ComputeHash(buffer,0, buffer.Length);
            return BytesToStringConverted(hasher.Hash);
        }

        static string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                using (var streamReader = new StreamReader(stream))
                    return streamReader.ReadToEnd();
        }

        #endregion

        #region Sample Data

        public static IEnumerable<object[]> GetCancellationTokenIsCancelledData()
        {
            yield return new object[] { Guid.NewGuid(), new CancellationToken(true) };
        }

        public static IEnumerable<object[]> GetGuidData()
        {
            yield return new object[] { new Guid("6d58e569-6ae7-48c3-bb0f-8b41df0e9655") };
        }

        public static IEnumerable<object[]> GetNotExistingGuidData()
        {
            yield return new object[] { new Guid("00090005-0008-0004-0007-000300060066") };
        }

        public static IEnumerable<object[]> GetEmptyGuidAndTokenData()
        {
            yield return new object[] { Guid.Empty, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreData()
        {
            yield return new object[] { new Guid("11111111-2222-3333-4444-567890009098"), new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreCancellationTokenIsCancelledData()
        {
            yield return new object[] { Guid.NewGuid(), new StreamInfo(), new CancellationToken(true) };
        }

        public static IEnumerable<object[]> GetStoreNotExistingGuidData()
        {
            yield return new object[] { "99999090-9999-9999-9999-909090901044", new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreNoExistingGuidData()
        {
            yield return new object[] { "99999090-9999-9999-9999-909090901089", new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetUpdateData()
        {
            yield return new object[] { new Guid("11111111-2222-3333-4444-567890009098"), new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetUpdateCancellationTokenIsCancelledData()
        {
            yield return new object[] { Guid.NewGuid(), new StreamInfo(), new CancellationToken(true) };
        }

        public static IEnumerable<object[]> GetUpdateNotExistingGuidData()
        {
            yield return new object[] { "00090005-0008-0004-0007-000300060044", new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetDeleteData()
        {
            yield return new object[] { new Guid("77887712-1212-3434-4545-999999999999"),  new CancellationToken() };
        }

        public static IEnumerable<object[]> GetDeleteCancellationTokenIsCancelledData()
        {
            yield return new object[] { Guid.NewGuid(), new CancellationToken(true) };
        }

        public static IEnumerable<object[]> GetDeleteNotExistingGuidData()
        {
            yield return new object[] { new Guid("00090005-0008-0004-0007-000300060033"), new CancellationToken() };
        }

        #endregion

        #region Ctor

        public ContentManagerUnitTests(UnitTestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
            _manager = _testsFixture.Manager;
            _fileSystem = _testsFixture.FileSystem;
        }

        #endregion

        #region ExistsAsync Tests

        [Theory]
        [MemberData(nameof(GetNotExistingGuidData))]
        public async Task ExistsAsync_Should_Return_False_If_Id_Doesnt_Exist(Guid id)
        {
            var exists = await _manager.ExistsAsync(id, new CancellationToken());

            Assert.False(exists.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetEmptyGuidAndTokenData))]
        public async Task ExistsAsync_Should_Return_Error_Message_If_User_Doesnt_Have_Rights(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.ExistsAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("User does not have rights to perform this action!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task ExistsAsync_Should_Return_True_If_Id_Exists(Guid id)
        {
            var exists = await _manager.ExistsAsync(id, new CancellationToken());

            Assert.True(exists.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetCancellationTokenIsCancelledData))]
        public async Task ExistsAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.ExistsAsync(id, token));
        }

        #endregion

        #region GetHashAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenIsCancelledData))]
        public async Task GetHashAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.GetHashAsync(id, token));
        }

        [Theory]
        [MemberData(nameof(GetEmptyGuidAndTokenData))]
        public async Task GetHashAsync_Should_Return_Error_Message_If_User_Doesnt_Have_Rights(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.GetHashAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("User does not have rights to perform this action!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidData))]
        public async Task GetHashAsync_Should_Return_Empty_String_If_Id_Doesnt_Exist(Guid id)
        {
            var exists = (await _manager.GetHashAsync(id, new CancellationToken())).ResultObject;
            var expected = string.Empty;

            Assert.Equal(expected, exists);
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task GetHashAsync_Should_Return_Hash_If_Id_Exists(Guid id)
        {
            var exists = await _manager.GetHashAsync(id, new CancellationToken());
            var expected = GetHashFromByteArrayAsync(new byte[5] { 23, 178, 90, 44, 3 });

            Assert.Equal(expected, exists.ResultObject);
        }

        #endregion

        #region GetAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenIsCancelledData))]
        public async Task GetAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.GetAsync(id, token));
        }

        [Theory]
        [MemberData(nameof(GetEmptyGuidAndTokenData))]
        public async Task GetAsync_Should_Return_Error_Message_If_User_Doesnt_Have_Rights(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.GetAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("User does not have rights to perform this action!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidData))]
        public async Task GetAsync_Should_Return_Empty_Stream_If_Id_Doesnt_Exist(Guid id)
        {
            var actual = await _manager.GetAsync(id, new CancellationToken());
            var expected = new StreamInfo() { Stream = new MemoryStream() };

            Assert.True(expected.Equals(actual.ResultObject));
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task GetAsync_Should_Return_File_If_Id_Exists(Guid id)
        {
            var actual = await _manager.GetAsync(id, new CancellationToken());
            var expected = new byte[5] { 23, 178, 90, 44, 3 };
            var bytesAreSame = false;

            if (actual.ResultObject.Stream != new MemoryStream() && expected.Any())
                bytesAreSame = true;

            Assert.True(bytesAreSame);
        }

        #endregion

        #region GetBytesAsync Tests

        [Theory]
        [MemberData(nameof(GetCancellationTokenIsCancelledData))]
        public async Task GetBytesAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken token)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.GetBytesAsync(id, token));
        }

        [Theory]
        [MemberData(nameof(GetEmptyGuidAndTokenData))]
        public async Task GetBytesAsync_Should_Return_Error_Message_If_User_Doesnt_Have_Rights(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.GetBytesAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("User does not have rights to perform this action!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidData))]
        public async Task GetBytesAsync_Should_Return_Null_If_Id_Doesnt_Exist(Guid id)
        {
            var actual = await _manager.GetBytesAsync(id, new CancellationToken());

            Assert.Null(actual.ResultObject);
        }

        [Theory]
        [MemberData(nameof(GetGuidData))]
        public async Task GetBytesAsync_Should_Return_File_If_Id_Exists(Guid id)
        {
            var actual = await _manager.GetBytesAsync(id, new CancellationToken());
            var expected = new byte[5] { 23, 178, 90, 44, 3 };
            var bytesAreSame = false;

            if(actual.ResultObject.Any() && expected.Any())
                bytesAreSame = actual.ResultObject.SequenceEqual(expected);

            Assert.True(bytesAreSame);
        }

        #endregion

        #region StoreAsync Tests
        static CancellationToken token = new();

        [Theory]
        [MemberData(nameof(GetStoreCancellationTokenIsCancelledData))]
        public async Task StoreAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.StoreAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetStoreNoExistingGuidData))]
        public async Task StoreAsync_Should_Throw_Error_If_User_Doesnt_Have_Rights(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _manager.StoreAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetStoreNotExistingGuidData))]
        public async Task StoreAsync_Should_Create_File_And_Return_Success_Message_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var actual = (await _manager.StoreAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AddSuccessMessage("Successfully created file and added file content!");


            Assert.True(expected.SuccessMessages.Any(m => actual.SuccessMessages.Contains(m)));
            Assert.Equal(new List<IOperationError>(), actual.Errors);
        }

        [Theory]
        [MemberData(nameof(GetStoreData))]
        public async Task StoreAsync_Should_Return_Error_Message_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var actual = (await _manager.StoreAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("File alreadyExists!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        #endregion

        #region UpdateAsync Tests

        [Theory]
        [MemberData(nameof(GetUpdateCancellationTokenIsCancelledData))]
        public async Task UpdateAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.UpdateAsync(id, fileContent, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetUpdateNotExistingGuidData))]
        public async Task UpdateAsync_Should_Return_Error_Message_If_User_Doesnt_Have_Rights(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var actual = (await _manager.UpdateAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("User does not have rights to perform this action!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        [Theory]
        [MemberData(nameof(GetUpdateNotExistingGuidData))]
        public async Task UpdateAsync_Should_Return_Success_Message_And_Create_File_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var actual = (await _manager.UpdateAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("Provided path is not valid!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
        }

        [Theory]
        [MemberData(nameof(GetUpdateData))]
        public async Task UpdateAsync_Should_Return_Success_Message_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var actual = (await _manager.UpdateAsync(id, fileContent, cancellationToken));
            var expected = new OperationResult();
            expected.AddSuccessMessage("Successfully updated file content!");

            Assert.True(expected.SuccessMessages.Any(m => actual.SuccessMessages.Contains(m)));
            Assert.Equal(new List<IOperationError>(), actual.Errors);
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [MemberData(nameof(GetDeleteCancellationTokenIsCancelledData))]
        public async Task DeleteAsync_Should_Stop_If_CancellationToken_IsCancelled(Guid id, CancellationToken cancellationToken)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _manager.DeleteAsync(id, cancellationToken));
        }

        [Theory]
        [MemberData(nameof(GetDeleteNotExistingGuidData))]
        public async Task DeleteAsync_Should_Return_Error_Message_If_User_Doesnt_Have_Rights(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.DeleteAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("User does not have rights to perform this action!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
            Assert.Null(actual.SuccessMessages);
        }

        [Theory]
        [MemberData(nameof(GetDeleteNotExistingGuidData))]
        public async Task DeleteAsync_Should_Return_Error_Message_If_Id_Doesnt_Exist(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.DeleteAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AppendError("Provided path is not valid!");

            Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
        }

        [Theory]
        [MemberData(nameof(GetDeleteData))]
        public async Task DeleteAsync_Should_Return_Success_Message_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.DeleteAsync(id, cancellationToken));
            var expected = new OperationResult();
            expected.AddSuccessMessage("Successfully removed file content!");

            Assert.True(expected.SuccessMessages.Any(m => actual.SuccessMessages.Contains(m)));
            Assert.Equal(new List<IOperationError>(), actual.Errors);
        }

        #endregion
    }
}