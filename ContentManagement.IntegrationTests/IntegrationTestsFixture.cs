

namespace ContentManagement.IntegrationTests
{
    /// <summary>
    /// Represents "base" class for calling 
    /// setup and teardown just once
    /// </summary>
    public class IntegrationTestsFixture : IDisposable
    {
        public IContentManager<Guid> Manager { get; set; }

        public MockFileSystem FileSystem { get; set; }

        /// <summary>
        /// Acts as global setup method
        /// </summary>
        public IntegrationTestsFixture()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                //existing Guid
                { @"\\MSISSOF293\Users\d.vasilev\Documents\6d58e569-6ae7-48c3-bb0f-8b41df0e9655", new MockFileData(new byte[] { 23, 178, 90, 44, 3}) },
                //existing guid for store
                {@"\\MSISSOF293\Users\d.vasilev\Documents\11111111-2222-3333-4444-567890009098", new MockFileData("testString") },
                //existing guid for delete
                {@"\\MSISSOF293\Users\d.vasilev\Documents\77887712-1212-3434-4545-999999999999", new MockFileData(new byte[] { 23, 178, 90, 44, 3}) },
                //existing guid for update
                {@"\\MSISSOF293\Users\d.vasilev\Documents\99999999-0001-0002-0003-000000000000", new MockFileData("testString") },
                //existing guid for get
                {@"\\MSISSOF293\Users\d.vasilev\Documents\10101010-0000-1111-0000-010101010101", new MockFileData(new byte[] { 23, 178, 90, 44, 3}) },
                //existing guid for getBytes
                {@"\\MSISSOF293\Users\d.vasilev\Documents\55555555-5555-5555-5555-555555555555", new MockFileData(new byte[] { 23, 178, 90, 44, 3}) },
                //existing guid for exists
                {@"\\MSISSOF293\Users\d.vasilev\Documents\12341234-1234-1234-1234-123412341234", new MockFileData("testString") },
                //existing guid for getHash
                {@"\\MSISSOF293\Users\d.vasilev\Documents\67676767-8888-4444-9999-676767676767", new MockFileData(new byte[] { 23, 178, 90, 44, 3}) }
            }, @"\\MSISSOF293\Users\d.vasilev\Documents");

            Manager = new ContentManager(new ConfigurationOptions() { FileServerPath = @"\\MSISSOF293\Users\d.vasilev\Documents" }, fileSystem);
            FileSystem = fileSystem;
        }

        /// <summary>
        /// Acts as global taredown method
        /// </summary>
        public void Dispose()
        {
        }
    }
}
