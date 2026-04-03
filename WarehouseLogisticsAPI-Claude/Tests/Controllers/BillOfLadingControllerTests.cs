using Microsoft.AspNetCore.Mvc;
using Xunit;
using WarehouseLogistics_Claude.Controllers;
using WarehouseLogistics_Claude.Models;

namespace WarehouseLogistics_Claude.Tests.Controllers;

public class BillOfLadingControllerTests : IDisposable
{
    private readonly BillOfLadingController _controller;
    private readonly List<string> _createdFiles = new();
    private readonly string _downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    public BillOfLadingControllerTests()
    {
        _controller = new BillOfLadingController();
    }

    public void Dispose()
    {
        foreach (var file in _createdFiles)
            if (File.Exists(file)) File.Delete(file);
    }

    private string TrackFile(BillOfLading bol)
    {
        var path = Path.Combine(_downloadsPath, $"BillOfLading_{bol.TransactionId}.txt");
        _createdFiles.Add(path);
        return path;
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var bol = BuildBillOfLading();

        var result = await _controller.Create(bol);
        TrackFile(bol);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsTransactionId()
    {
        var bol = BuildBillOfLading();

        var result = await _controller.Create(bol);
        TrackFile(bol);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.IsType<string>(created.Value);
        Assert.Equal(bol.TransactionId, created.Value);
    }

    [Fact]
    public async Task Create_SetsTransactionId()
    {
        var bol = BuildBillOfLading();

        await _controller.Create(bol);
        TrackFile(bol);

        Assert.NotEmpty(bol.TransactionId);
    }

    [Fact]
    public async Task Create_TransactionId_IsEightCharacters()
    {
        var bol = BuildBillOfLading();

        await _controller.Create(bol);
        TrackFile(bol);

        Assert.Equal(8, bol.TransactionId.Length);
    }

    [Fact]
    public async Task Create_SetsStatusToCreated()
    {
        var bol = BuildBillOfLading();

        await _controller.Create(bol);
        TrackFile(bol);

        Assert.Equal("Created", bol.Status);
    }

    [Fact]
    public async Task Create_SetsPartitionKey_ContainingTransactionId()
    {
        var bol = BuildBillOfLading();

        await _controller.Create(bol);
        TrackFile(bol);

        Assert.StartsWith(bol.TransactionId, bol.PartitionKey);
    }

    [Fact]
    public async Task Create_PreservesSubmittedFields()
    {
        var bol = BuildBillOfLading();

        await _controller.Create(bol);
        TrackFile(bol);

        Assert.Equal("John",    bol.CustomerFirstName);
        Assert.Equal("Doe",     bol.CustomerLastName);
        Assert.Equal("Chicago", bol.City);
        Assert.Equal("IL",      bol.State);
    }

    [Fact]
    public async Task Create_WithLineEntries_PreservesEntries()
    {
        var bol = BuildBillOfLading(withEntries: true);

        await _controller.Create(bol);
        TrackFile(bol);

        Assert.Equal(2, bol.LineEntries.Count);
        Assert.Equal("WH-001", bol.LineEntries[0].WarehouseId);
        Assert.Equal("WH-002", bol.LineEntries[1].WarehouseId);
    }

    [Fact]
    public async Task Create_WritesFile_ToDownloads()
    {
        var bol = BuildBillOfLading();

        await _controller.Create(bol);
        string expectedFile = TrackFile(bol);

        Assert.True(File.Exists(expectedFile));
    }

    private static BillOfLading BuildBillOfLading(bool withEntries = false)
    {
        var bol = new BillOfLading
        {
            CustomerFirstName = "John",
            CustomerLastName  = "Doe",
            City              = "Chicago",
            State             = "IL"
        };

        if (withEntries)
        {
            bol.LineEntries.Add(new LineEntry { WarehouseId = "WH-001", SKUMarker = "PWTL456", Quantity = 10  });
            bol.LineEntries.Add(new LineEntry { WarehouseId = "WH-002", SKUMarker = "PWTL135", Quantity = -5 });
        }

        return bol;
    }
}
