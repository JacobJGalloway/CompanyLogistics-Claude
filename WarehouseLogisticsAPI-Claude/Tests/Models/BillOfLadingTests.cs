using Xunit;
using WarehouseLogistics_Claude.Models;
using WarehouseLogistics_Claude.Models.Interfaces;

namespace WarehouseLogistics_Claude.Tests.Models;

public class BillOfLadingTests
{
    [Fact]
    public void BillOfLading_ImplementsIBillOfLading()
    {
        var bol = new BillOfLading();

        Assert.IsAssignableFrom<IBillOfLading>(bol);
    }

    [Fact]
    public void BillOfLading_DefaultProperties_AreEmptyStrings()
    {
        var bol = new BillOfLading();

        Assert.Equal(string.Empty, bol.PartitionKey);
        Assert.Equal(string.Empty, bol.TransactionId);
        Assert.Equal(string.Empty, bol.CustomerFirstName);
        Assert.Equal(string.Empty, bol.CustomerLastName);
        Assert.Equal(string.Empty, bol.City);
        Assert.Equal(string.Empty, bol.State);
    }

    [Fact]
    public void BillOfLading_DefaultStatus_IsPending()
    {
        var bol = new BillOfLading();

        Assert.Equal("Pending", bol.Status);
    }

    [Fact]
    public void BillOfLading_DefaultLineEntries_IsEmptyList()
    {
        var bol = new BillOfLading();

        Assert.NotNull(bol.LineEntries);
        Assert.Empty(bol.LineEntries);
    }

    [Fact]
    public void BillOfLading_Properties_CanBeSet()
    {
        var bol = new BillOfLading
        {
            PartitionKey       = "a1b2c3d4-abc123",
            TransactionId      = "a1b2c3d4",
            Status             = "Created",
            CustomerFirstName  = "John",
            CustomerLastName   = "Doe",
            City               = "Chicago",
            State              = "IL"
        };

        Assert.Equal("a1b2c3d4-abc123", bol.PartitionKey);
        Assert.Equal("a1b2c3d4",        bol.TransactionId);
        Assert.Equal("Created",          bol.Status);
        Assert.Equal("John",             bol.CustomerFirstName);
        Assert.Equal("Doe",              bol.CustomerLastName);
        Assert.Equal("Chicago",          bol.City);
        Assert.Equal("IL",               bol.State);
    }

    [Fact]
    public void BillOfLading_LineEntries_CanBeAdded()
    {
        var bol = new BillOfLading();
        bol.LineEntries.Add(new LineEntry { WarehouseId = "WH-001", SKUMarker = "PWTL456", Quantity = 10 });
        bol.LineEntries.Add(new LineEntry { WarehouseId = "WH-002", SKUMarker = "PWTL135", Quantity = -5 });

        Assert.Equal(2, bol.LineEntries.Count);
        Assert.Equal("WH-001", bol.LineEntries[0].WarehouseId);
        Assert.Equal("WH-002", bol.LineEntries[1].WarehouseId);
    }
}
