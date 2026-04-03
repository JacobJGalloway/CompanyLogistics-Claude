-- database: ./WarehouseData.db3
-- 10 transactions, 3 line entries each
-- Positive Quantity = incoming, Negative Quantity = outgoing

INSERT OR IGNORE INTO BillOfLading (PartitionKey, TransactionId, FirstName, LastName, City, State, SKUMarker, Quantity) VALUES
-- Transaction 1: Incoming shipment to WH001
('a1b2c3d4-e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 'a1b2c3d4', 'John',    'Smith',    'Chicago',      'IL', 'CLTH001', 25),
('a1b2c3d4-f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1', 'a1b2c3d4', 'John',    'Smith',    'Chicago',      'IL', 'SPPE004', 50),
('a1b2c3d4-a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2', 'a1b2c3d4', 'John',    'Smith',    'Chicago',      'IL', 'PWTL001', 10),

-- Transaction 2: Outgoing order from WH002
('b2c3d4e5-f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1', 'b2c3d4e5', 'Sarah',   'Johnson',  'Milwaukee',    'WI', 'CLTH004', -15),
('b2c3d4e5-a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2', 'b2c3d4e5', 'Sarah',   'Johnson',  'Milwaukee',    'WI', 'SPPE011', -8),
('b2c3d4e5-b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3', 'b2c3d4e5', 'Sarah',   'Johnson',  'Milwaukee',    'WI', 'PWTL003', -5),

-- Transaction 3: Inventory shift WH001 -> WH003
('c3d4e5f6-a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2', 'c3d4e5f6', 'Mike',    'Davis',    'Indianapolis', 'IN', 'CLTH007', -20),
('c3d4e5f6-b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3', 'c3d4e5f6', 'Mike',    'Davis',    'Indianapolis', 'IN', 'SPPE015', -30),
('c3d4e5f6-c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4', 'c3d4e5f6', 'Mike',    'Davis',    'Indianapolis', 'IN', 'PWTL008', -4),

-- Transaction 4: Incoming shipment to WH003
('d4e5f6a7-b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3', 'd4e5f6a7', 'Lisa',    'Martinez', 'Detroit',      'MI', 'CLTH007', 20),
('d4e5f6a7-c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4', 'd4e5f6a7', 'Lisa',    'Martinez', 'Detroit',      'MI', 'SPPE015', 30),
('d4e5f6a7-d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5', 'd4e5f6a7', 'Lisa',    'Martinez', 'Detroit',      'MI', 'PWTL008',  4),

-- Transaction 5: Outgoing order from WH001
('e5f6a7b8-c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4', 'e5f6a7b8', 'Tom',     'Wilson',   'Columbus',     'OH', 'CLTH002', -10),
('e5f6a7b8-d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5', 'e5f6a7b8', 'Tom',     'Wilson',   'Columbus',     'OH', 'SPPE007', -20),
('e5f6a7b8-e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6', 'e5f6a7b8', 'Tom',     'Wilson',   'Columbus',     'OH', 'PWTL005', -3),

-- Transaction 6: Incoming shipment to WH002
('f6a7b8c9-d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5', 'f6a7b8c9', 'Emily',   'Brown',    'Cleveland',    'OH', 'CLTH010', 40),
('f6a7b8c9-e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6', 'f6a7b8c9', 'Emily',   'Brown',    'Cleveland',    'OH', 'SPPE018', 100),
('f6a7b8c9-f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7', 'f6a7b8c9', 'Emily',   'Brown',    'Cleveland',    'OH', 'PWTL014',  2),

-- Transaction 7: Mixed — restock WH002 tools, pull clothing
('a7b8c9d0-e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6', 'a7b8c9d0', 'Carlos',  'Garcia',   'St. Louis',    'MO', 'CLTH015', -12),
('a7b8c9d0-f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7', 'a7b8c9d0', 'Carlos',  'Garcia',   'St. Louis',    'MO', 'SPPE012',  15),
('a7b8c9d0-a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8', 'a7b8c9d0', 'Carlos',  'Garcia',   'St. Louis',    'MO', 'PWTL010',  6),

-- Transaction 8: Outgoing order from WH003
('b8c9d0e1-f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7', 'b8c9d0e1', 'Karen',   'Lee',      'Kansas City',  'MO', 'CLTH020', -18),
('b8c9d0e1-a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8', 'b8c9d0e1', 'Karen',   'Lee',      'Kansas City',  'MO', 'SPPE024', -10),
('b8c9d0e1-b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9', 'b8c9d0e1', 'Karen',   'Lee',      'Kansas City',  'MO', 'PWTL022',  -2),

-- Transaction 9: Incoming restock across categories
('c9d0e1f2-a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8', 'c9d0e1f2', 'David',   'Taylor',   'Minneapolis',  'MN', 'CLTH005',  30),
('c9d0e1f2-b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9', 'c9d0e1f2', 'David',   'Taylor',   'Minneapolis',  'MN', 'SPPE016',  20),
('c9d0e1f2-c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0', 'c9d0e1f2', 'David',   'Taylor',   'Minneapolis',  'MN', 'PWTL015',   5),

-- Transaction 10: Outgoing seasonal pullback
('d0e1f2a3-b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9', 'd0e1f2a3', 'Rachel',  'Anderson', 'Madison',      'WI', 'CLTH008', -22),
('d0e1f2a3-c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0', 'd0e1f2a3', 'Rachel',  'Anderson', 'Madison',      'WI', 'SPPE020', -40),
('d0e1f2a3-d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1', 'd0e1f2a3', 'Rachel',  'Anderson', 'Madison',      'WI', 'PWTL009',  -3);
