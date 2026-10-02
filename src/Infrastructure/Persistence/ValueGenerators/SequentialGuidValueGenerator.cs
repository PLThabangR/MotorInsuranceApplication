using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;


namespace Infrastructure.Persistence.ValueGenerators;



/// <summary>
/// Generates time-ordered Guids instead of purely random ones.
///
/// Why: PostgreSQL stores the primary key as a B-tree index by default.
/// If Guids are random (Guid.NewGuid()), every insert lands at a random
/// position in the index, causing page splits and fragmentation. Over
/// millions of rows this degrades insert performance and bloats the index.
///
/// A sequential Guid has a monotonically increasing prefix (typically a
/// timestamp), so new rows always append near the end of the index, exactly
/// like an auto-incrementing integer would.
///
/// .NET 9 introduced Guid.CreateVersion7() which does this natively.
/// Since we are on .NET 8, we implement a simple, deterministic version here.
/// </summary>
public sealed class SequentialGuidValueGenerator : ValueGenerator<Guid>
{
    /// <summary>
    /// Indicates that the generator produces temporary values when needed.
    /// For our case, values are always permanent, so this is false.
    /// </summary>
    public override bool GeneratesTemporaryValues => false;

    public override Guid Next(EntityEntry entry)
    {
        // Compose a Guid whose first 8 bytes are a timestamp (in ticks)
        // and whose remaining 8 bytes are random. This preserves uniqueness
        // while making the value monotonically increasing over time.
        //
        // We reverse the timestamp bytes so that the most significant byte
        // of the timestamp ends up first in the Guid's byte order, matching
        // the way PostgreSQL's uuid ordering works (byte-wise ascending).
        var timestamp = DateTime.UtcNow.Ticks;

        var timestampBytes = BitConverter.GetBytes(timestamp);

        // SQL Server and PostgreSQL differ in Guid byte ordering. For PostgreSQL,
        // we want the bytes to sort lexicographically the same way the timestamp
        // sorts numerically. Big-endian ensures that.
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestampBytes);
        }

        var randomBytes = Guid.NewGuid().ToByteArray();

        var guidBytes = new byte[16];
        Array.Copy(timestampBytes, 0, guidBytes, 0, 8);
        Array.Copy(randomBytes, 8, guidBytes, 8, 8);

        return new Guid(guidBytes);
    }
}