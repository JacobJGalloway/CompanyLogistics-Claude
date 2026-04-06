// using System.Linq;
// using Microsoft.AspNetCore.Mvc;
// using WarehouseLogistics_Claude.Models.Interfaces;
// using WarehouseLogistics_Claude.Data.Interfaces;
// using

// namespace WarehouseLogistics_Claude.Services
// {
//     public class BillOfLadingService
//     {
//         private readonly IWarehouseRepository _warehouseRepository;
//         private readonly IStoreRepository _storeRepository;
//         private readonly ILineEntryRepository _lineEntryRepository;

//         public BillOfLadingService(IWarehouseRepository warehouseRepository, IStoreRepository storeRepository, ILineEntryRepository lineEntryRepository)
//         {
//             _warehouseRepository = warehouseRepository;
//             _storeRepository = storeRepository;
//             _lineEntryRepository = lineEntryRepository;
//         }

//         public async Task ProcessLocationStop(string transactionId, string locationId)
//         {
//             var lineEntries = await _lineEntryRepository.GetLineEntriesByTransactionIdAsync(transactionId);
//             var locationLineEntries = lineEntries.Where(le => le.LocationId == locationId).ToList();

//             if (locationLineEntries.Count == 0)
//             {
//                 // No line entries for this location, so we can skip processing.
//                 return;
//             }

//             // Mark the line entries for this location as processed.
//             foreach (var lineEntry in locationLineEntries)
//             {
//                 lineEntry.IsProcessed = true;
//                 await _lineEntryRepository.UpdateLineEntryAsync(lineEntry);
//             }
//         }
//     }
// }