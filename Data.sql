USE [PCConfiguratorService]
GO
INSERT INTO Manufacturers (ManufacturerName) VALUES
('Intel'), ('AMD'), ('NVIDIA'), ('ASUS'), ('MSI'), ('Gigabyte'), 
('Corsair'), ('Kingston'), ('Samsung'), ('Western Digital'), 
('Seagate'), ('EVGA'), ('Cooler Master'), ('NZXT'), ('Thermaltake');
GO
INSERT INTO Sockets (SocketName) VALUES
('LGA 1200'), ('LGA 1700'), ('AM4'), ('AM5'), ('TR4'), ('SP3'), ('LGA 2066');
GO
INSERT INTO RAMTypes (RAMType) VALUES
('DDR3'), ('DDR4'), ('DDR5'), ('DDR5 ECC'), ('DDR4 ECC');
GO
INSERT INTO StorageTypes (StorageType) VALUES
('HDD'), ('SATA SSD'), ('NVMe SSD'), ('M.2 NVMe SSD'), ('U.2 NVMe SSD');
GO
INSERT INTO EfficiencyRatings (Rating) VALUES
('80+'), ('80+ Bronze'), ('80+ Silver'), ('80+ Gold'), ('80+ Platinum'), ('80+ Titanium');
GO
INSERT INTO CoolingTypes (CoolingType) VALUES
('Воздушное'), ('Жидкостное');
GO
INSERT INTO CaseFormFactors (CaseFFName) VALUES
('Mini ITX'), ('Micro ATX'), ('ATX'), ('E-ATX'), ('Full Tower');
GO
INSERT INTO MotherboardFormFactor (MotherboardFFName) VALUES
('Mini ITX'), ('Micro ATX'), ('ATX'), ('E-ATX'), ('XL-ATX');
GO
INSERT INTO Vendors (VendorName) VALUES
('NVIDIA'), ('AMD'), ('Intel'), ('EVGA'), ('MSI'), ('ASUS'), ('Galax');
GO
INSERT INTO GPUMemoryTypes (MemoryType) VALUES
('GDDR5'), ('GDDR6'), ('GDDR6X'), ('HBM2'), ('HBM3');
GO
INSERT INTO Roles (RoleName) VALUES
('Администратор'), ('Оператор');
GO
INSERT INTO Users([RoleID], [LastName], [FirstName], [Patronymic], [UserLogin], [UserPassword]) VALUES
(1, N'admin', N'admin', N'admin', N'admin', N'admin'), 
(2, N'operator', N'operator', N'operator', N'operator', N'operator')