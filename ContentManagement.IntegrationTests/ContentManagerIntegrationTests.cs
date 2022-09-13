

namespace ContentManagement.IntegrationTests
{
    public class ContentManagerIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        #region Fields

        private readonly IContentManager<Guid> _manager;
        private readonly MockFileSystem _fileSystem;
        private readonly IntegrationTestsFixture _testsFixture;

        #endregion

        #region Utilities

        private static Stream GetStreamForFIle(Guid guid)
        {
            var file = new DirectoryInfo(@"\\MSISSOF293\Users\d.vasilev\Documents").GetFiles("Blocked_2022-08-29_09-32-08_DLGvpYS5pNVL" + ".*").FirstOrDefault();
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

            hasher.ComputeHash(buffer, 0, buffer.Length);
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

        public static IEnumerable<object[]> GetNotExistingGuidAndTokenData()
        {
            yield return new object[] { new Guid("00090005-0008-0004-0007-000300060066"), new CancellationToken() };
        }

        public static IEnumerable<object[]> GetGuidAndTokenData()
        {
            yield return new object[] { new Guid("6d58e569-6ae7-48c3-bb0f-8b41df0e9655"), new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreData()
        {
            yield return new object[] { new Guid("11111111-2222-3333-4444-567890009098"), new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetStoreNotExistingGuidData()
        {
            yield return new object[] { "99999090-9999-9999-9999-909090901044", new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetUpdateData()
        {
            yield return new object[] { new Guid("11111111-2222-3333-4444-567890009098"), new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetDeleteThenStoreOrUpdateData()
        {
            yield return new object[] { new Guid("77887712-1212-3434-4545-999999999999"), new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetUpdateNotExistingGuidData()
        {
            yield return new object[] { "00090005-0008-0004-0007-000300060044", new StreamInfo() { Stream = GetStreamForFIle(new Guid("11111111-2222-3333-4444-567890009098")) }, new CancellationToken() };
        }

        public static IEnumerable<object[]> GetDeleteData()
        {
            yield return new object[] { new Guid("77887712-1212-3434-4545-999999999999"), new CancellationToken() };
        }

        public static IEnumerable<object[]> GetDeleteThenGetData()
        {
            yield return new object[] { new Guid("77887712-1212-3434-4545-999999999999"), new CancellationToken() };
        }

        #endregion

        #region Ctor

        public ContentManagerIntegrationTests(IntegrationTestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
            _manager = _testsFixture.Manager;
            _fileSystem = _testsFixture.FileSystem;
        }

        #endregion

        #region ExistsAsync Then Get Tests

        [Theory]
        [MemberData(nameof(GetGuidAndTokenData))]
        public async Task ExistsAsync_Then_GetAsync_Should_Return_FIle_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

                var expected = new byte[5] { 23, 178, 90, 44, 3 };
                var bytesAreSame = false;

                if (streamInfo.Stream.Length > 0)
                    bytesAreSame = ((MemoryStream)streamInfo.Stream).ToArray().SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidAndTokenData))]
        public async Task ExistsAsync_Then_GetAsync_Should_Return_Empty_Stream_If_Id_Doesnt_Exist(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

                var expected = new byte[5] { 23, 178, 90, 44, 3 };
                var bytesAreSame = false;

                if (streamInfo.Stream.Length > 0)
                    bytesAreSame = ((MemoryStream)streamInfo.Stream).ToArray().SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetGuidAndTokenData))]
        public async Task ExistsAsync_Then_GetBytesAsync_Should_Return_FIle_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.True(exists.ResultObject);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;

                var expected = new byte[5] { 23, 178, 90, 44, 3 };
                var bytesAreSame = false;

                if (streamInfo.Length > 0)
                    bytesAreSame = streamInfo.SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidAndTokenData))]
        public async Task ExistsAsync_Then_GetBytesAsync_Should_Return_Empty_Stream_If_Id_Doesnt_Exist(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

                var expected = new byte[5] { 23, 178, 90, 44, 3 };
                var bytesAreSame = false;

                if (streamInfo.Stream.Length > 0)
                    bytesAreSame = ((MemoryStream)streamInfo.Stream).ToArray().SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetGuidAndTokenData))]
        public async Task ExistsAsync_Then_GetHashAsync_Should_Return_Hash_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.True(exists.ResultObject);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;

                var expected = GetHashFromByteArrayAsync(new byte[5] { 23, 178, 90, 44, 3 });

                Assert.Equal(expected, streamInfo);
            }
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidAndTokenData))]
        public async Task ExistsAsync_Then_GetHashAsync_Should_Return_Empty_String_If_Id_Doesnt_Exist(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;

                var expected = string.Empty;
                Assert.Equal(actual, expected);
            }
        }

        #endregion

        #region ExistsAsync Then StoreAsync, UpdateAsync, DeleteAsync Tests

        [Theory]
        [MemberData(nameof(GetStoreData))]
        public async Task ExistsAsync_Then_StoreAsync_Should_Return_Error_Message_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.True(exists.ResultObject);

            if (exists.ResultObject)
            {
                var actual = await _manager.StoreAsync(id, fileContent, cancellationToken);
                var expected = new OperationResult();
                expected.AppendError("File alreadyExists!");

                Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
                Assert.Null(actual.SuccessMessages);
            }
        }

        [Theory]
        [MemberData(nameof(GetStoreNotExistingGuidData))]
        public async Task ExistsAsync_Then_StoreAsync_Then_GetBytesAsync_Should_Return_Byte_Aray_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var result = (await _manager.StoreAsync(id, fileContent, cancellationToken));

                var actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;

                var expected = new byte[5] { 23, 178, 90, 44, 3 };
                var bytesAreSame = false;

                if (actual.Length > 0)
                    bytesAreSame = actual.SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetStoreNotExistingGuidData))]
        public async Task ExistsAsync_Then_StoreAsync_Then_GetAsync_Should_Return_Byte_Aray_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var result = (await _manager.StoreAsync(id, fileContent, cancellationToken));

                var actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

                var expected = new byte[5] { 23, 178, 90, 44, 3 };
                var bytesAreSame = false;

                if (actual.Stream.Length > 0)
                    bytesAreSame = ((MemoryStream)actual.Stream).ToArray().SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetUpdateData))]
        public async Task ExistsAsync_Then_UpdateAsync_Then_GetBytesAsync_Should_Return_Updated_FIle_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.True(exists.ResultObject);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.UpdateAsync(id, fileContent, cancellationToken));

                var expected = new byte[10] { 116, 101, 115, 116, 83, 116, 114, 105, 110, 103 };
                var actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
                var bytesAreSame = false;

                if (actual.Length > 0)
                    bytesAreSame = actual.SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetUpdateData))]
        public async Task ExistsAsync_Then_UpdateAsync_Then_GetAsync_Should_Return_Updated_FIle_If_Id_Exists(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.True(exists.ResultObject);

            if (exists.ResultObject)
            {
                var streamInfo = (await _manager.UpdateAsync(id, fileContent, cancellationToken));

                var expected = new byte[10] { 116, 101, 115, 116, 83, 116, 114, 105, 110, 103 };
                var actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;
                var bytesAreSame = false;

                if (actual.Stream.Length > 0)
                    bytesAreSame = ((MemoryStream)actual.Stream).ToArray().SequenceEqual(expected);

                Assert.True(bytesAreSame);
            }
        }

        [Theory]
        [MemberData(nameof(GetUpdateNotExistingGuidData))]
        public async Task ExistsAsync_Then_UpdateAsync_Should_Return_Error_Message_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var actual = (await _manager.UpdateAsync(id, fileContent, cancellationToken));

                var expected = new OperationResult();
                expected.AppendError("Provided path is not valid!");

                Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
                Assert.Null(actual.SuccessMessages);
            }
        }

        [Theory]
        [MemberData(nameof(GetDeleteData))]
        public async Task ExistsAsync_Then_DeleteAsync_Should_Return_Succes_Message_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.True(exists.ResultObject);

            if (exists.ResultObject)
            {
                var actual = await _manager.DeleteAsync(id, cancellationToken);
                var expected = new OperationResult();
                expected.AddSuccessMessage("Successfully removed file content!");

                Assert.True(expected.SuccessMessages.Any(m => actual.SuccessMessages.Contains(m)));
                Assert.Equal(new List<IOperationError>(), actual.Errors);
            }
        }

        [Theory]
        [MemberData(nameof(GetNotExistingGuidAndTokenData))]
        public async Task ExistsAsync_Then_DeleteAsync_Should_Return_Empty_Result_If_Id_Doesnt_Exist(Guid id, CancellationToken cancellationToken)
        {
            var exists = await _manager.ExistsAsync(id, cancellationToken);

            Assert.False(exists.ResultObject);

            if (exists.ResultObject)
            {
                var actual = await _manager.DeleteAsync(id, cancellationToken);

                var expected = new OperationResult();
                expected.AppendError("Provided path is not valid!");

                Assert.True(expected.Errors.Select(e => e.Message).Any(m => actual.Errors.Select(e => e.Message).Contains(m)));
                Assert.Null(actual.SuccessMessages);
            }
        }

        #endregion

        #region GetAsync, GetBytesAsync, GetHashAsync Then DeleteAsync, Then GetAsync, GetBytesAsync, GetHashAsync Tests

        [Theory]
        [MemberData(nameof(GetDeleteData))]
        public async Task GetAsync_Then_DeleteAsync_Then_GetAsync_Should_Return_Error_Message_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

            var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

            actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;
            var expected = new StreamInfo() { Stream = new MemoryStream() };

            Assert.True(expected.Equals(actual));
        }

        [Theory]
        [MemberData(nameof(GetDeleteThenGetData))]
        public async Task GetBytesAsync_Then_DeleteAsync_Then_GetBytesAsync_Should_Return_Null_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
            var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

            actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
            Assert.Null(actual);
        }

        [Theory]
        [MemberData(nameof(GetDeleteThenGetData))]
        public async Task GetHashAsync_Then_DeleteAsync_Then_GetHashAsync_Should_Return_Empty_String_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        {
            var actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;
            var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

            actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;
            var expected = string.Empty;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region UpdateAsync, StoreBytesAsync Then GetAsync, GetBytesAsync, GetHashAsync Tests

        //[Theory]
        //[MemberData(nameof(GetDeleteData))]
        //public async Task GetAsync_Then_DeleteAsync_Then_GetAsync_Should_Return_Error_Message_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        //{
        //    var actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

        //    var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

        //    actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;
        //    var expected = new StreamInfo() { Stream = new MemoryStream() };

        //    Assert.True(expected.Equals(actual));
        //}

        //[Theory]
        //[MemberData(nameof(GetDeleteThenGetData))]
        //public async Task GetBytesAsync_Then_DeleteAsync_Then_GetBytesAsync_Should_Return_Null_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        //{
        //    var actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
        //    var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

        //    actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
        //    Assert.Null(actual);
        //}

        //[Theory]
        //[MemberData(nameof(GetDeleteThenGetData))]
        //public async Task GetHashAsync_Then_DeleteAsync_Then_GetHashAsync_Should_Return_Empty_String_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        //{
        //    var actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;
        //    var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

        //    actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;
        //    var expected = string.Empty;

        //    Assert.Equal(expected, actual);
        //}

        //[Theory]
        //[MemberData(nameof(GetDeleteData))]
        //public async Task GetAsync_Then_DeleteAsync_Then_GetAsync_Should_Return_Error_Message_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        //{
        //    var actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;

        //    var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

        //    actual = (await _manager.GetAsync(id, cancellationToken)).ResultObject;
        //    var expected = new StreamInfo() { Stream = new MemoryStream() };

        //    Assert.True(expected.Equals(actual));
        //}

        //[Theory]
        //[MemberData(nameof(GetDeleteThenGetData))]
        //public async Task GetBytesAsync_Then_DeleteAsync_Then_GetBytesAsync_Should_Return_Null_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        //{
        //    var actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
        //    var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

        //    actual = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
        //    Assert.Null(actual);
        //}

        //[Theory]
        //[MemberData(nameof(GetDeleteThenGetData))]
        //public async Task GetHashAsync_Then_DeleteAsync_Then_GetHashAsync_Should_Return_Empty_String_If_Id_Exists(Guid id, CancellationToken cancellationToken)
        //{
        //    var actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;
        //    var actualDeleted = await _manager.DeleteAsync(id, cancellationToken);

        //    actual = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;
        //    var expected = string.Empty;

        //    Assert.Equal(expected, actual);
        //}

        #endregion

        #region StoreBytesAsync Then GetAsync, GetBytesAsync, GetHashAsync Tests

        [Theory]
        [MemberData(nameof(GetStoreNotExistingGuidData))]
        public async Task StoreAsync_Then_GetAsync_And_GetBytesAsync_Should_Return_Same_Byte_Aray_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var result = (await _manager.StoreAsync(id, fileContent, cancellationToken));

            var actualGet = (await _manager.GetAsync(id, cancellationToken)).ResultObject;
            var actualGetBytes = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;

            var bytesAreSame = false;

            if (actualGet.Stream.Length > 0)
                bytesAreSame = ((MemoryStream)actualGet.Stream).ToArray().SequenceEqual(actualGetBytes);

            Assert.True(bytesAreSame);
        }

        [Theory]
        [MemberData(nameof(GetStoreNotExistingGuidData))]
        public async Task StoreAsync_Then_GetAsync_And_GetHashAsync_Should_Return_Same_Hash_Aray_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var result = (await _manager.StoreAsync(id, fileContent, cancellationToken));

            var actualGet = (await _manager.GetAsync(id, cancellationToken)).ResultObject;
            var hashFromGet = GetHashFromByteArrayAsync(((MemoryStream)actualGet.Stream).ToArray());
            var actualHash = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;

            Assert.Equal(hashFromGet, actualHash);
        }

        [Theory]
        [MemberData(nameof(GetStoreNotExistingGuidData))]
        public async Task StoreAsync_Then_GetBytesAsync_And_GetHashAsync_Should_Return_Same_Hash_Aray_If_Id_Doesnt_Exist(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var result = (await _manager.StoreAsync(id, fileContent, cancellationToken));

            var actualGet = (await _manager.GetBytesAsync(id, cancellationToken)).ResultObject;
            var hashFromGet = GetHashFromByteArrayAsync(actualGet);
            var actualHash = (await _manager.GetHashAsync(id, cancellationToken)).ResultObject;

            Assert.Equal(hashFromGet, actualHash);
        }

        #endregion
    }
}