# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Warehouse Inventory API — tracks inventory items in a warehouse. Per the README, this is part of a planned multi-service system:
1. **This repo** — base inventory API
2. A separate logistics API (not yet started)
3. A client UI for inventory withdrawals (not yet started)

## Current State

The project is in early skeleton/prototype phase. There is **no `.csproj` or `.sln` file yet**, so the project cannot be built or run with standard `dotnet` CLI commands. Before development can proceed, a proper .NET project file must be created.

Once a `.csproj` exists, standard commands will be:
```bash
dotnet build
dotnet run
dotnet test                        # run all tests
dotnet test --filter "FullyQualifiedName~TestName"  # run a single test
```

## Architecture

**Namespace root:** `WarehouseInventory_Claude`

**Conventions:**
- File-scoped namespaces (C# 10+)
- Interfaces live under `Models/Interfaces/` with the namespace suffix `.Interfaces`
- Domain models implement their corresponding interface (e.g., `Item : IItem`)

**Layers (planned/in-progress):**
- `Models/` — domain model classes and their interfaces
- `Data/` — EF Core `DbContext` (`InventoryContext.cs` exists but is empty)
- `Program.cs` — entry point (currently a Hello World placeholder; will become ASP.NET Core host setup)