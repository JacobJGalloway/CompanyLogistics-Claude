-- database: ./WarehouseData.db3
-- All pairs seeded at 1 day — initial concept implementation

INSERT OR IGNORE INTO WarehouseTransit (OriginWarehouseId, DestinationWarehouseId, TransitDays) VALUES
('WH001', 'WH002', 1),
('WH001', 'WH003', 1),
('WH002', 'WH001', 1),
('WH002', 'WH003', 1),
('WH003', 'WH001', 1),
('WH003', 'WH002', 1);
