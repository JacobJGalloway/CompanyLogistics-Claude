using Xunit;
using WarehouseLogistics_Claude.Models;
using WarehouseLogistics_Claude.Models.Interfaces;

namespace WarehouseLogistics_Claude.Tests.Models;

public class LineEntryTests
{
    [Fact]
    public void LineEntry_ImplementsILineEntry()
    {
        var entry = new LineEntry();

        Assert.IsAssignableFrom<ILineEntry>(entry);
    }

    [Fact]
    public void LineEntry_DefaultProperties_AreEmptyStrings()
    {
        var entry = new LineEntry();

        Assert.Equal(string.Empty, entry.WarehouseId);
        Assert.Equal(string.Empty, entry.SKUMarker);
    }

    [Fact]
    public void LineEntry_DefaultQuantity_IsZero()
    {
        var entry = new LineEntry();

        Assert.Equal(0, entry.Quantity);
    }

    [Fact]
    public void LineEntry_Properties_CanBeSet()
    {
        var entry = new LineEntry
        {
            WarehouseId = "WH-001",
            SKUMarker   = "PWTL456",
            Quantity    = 10
        };

        Assert.Equal("WH-001",  entry.WarehouseId);
        Assert.Equal("PWTL456", entry.SKUMarker);
        Assert.Equal(10,        entry.Quantity);
    }

    [Fact]
    public void LineEntry_NegativeQuantity_IsAllowed()
    {
        var entry = new LineEntry { Quantity = -5 };

        Assert.Equal(-5, entry.Quantity);
    }
}
