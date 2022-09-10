namespace ContentManagement.Core.Infrastructure
{
    /// <summary>
    /// A class providing length information about a <see cref="Stream"/>.
    /// When we retrieve files from network locations, streams can be a network stream with no length.
    /// </summary>
    public class StreamInfo : IEquatable<StreamInfo>
    {
        public long Length { get; set; }
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets the length of the stream info as an unsigned integer.
        /// If for some reason the current instance's <see cref="Length"/> is lower than 0, this value will be Zero.
        /// </summary>
        public ulong UnsignedLength => Length < 0 ? 0 : (ulong)Length;

        public bool Equals(StreamInfo? other)
        {
            if (this.Length == other.Length &&
                this.UnsignedLength == other.UnsignedLength)
                return true;

            return false;
        }
    }
}
