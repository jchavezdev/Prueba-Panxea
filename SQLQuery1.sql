use generacion38;

--Crear las tablas
CREATE TABLE Departamentos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100),
    Activo BIT
);

CREATE TABLE Empleados (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NumeroEmpleado VARCHAR(10) UNIQUE, 
    Nombre VARCHAR(100) ,
    FechaIngreso DATE,
    SalarioMensual DECIMAL(12,2),
    DepartamentoId INT REFERENCES Departamentos(Id),
    Activo BIT
);

--insertar informacion
INSERT INTO Departamentos (Nombre, Activo) VALUES ('Sistemas',1);
INSERT INTO Departamentos (Nombre, Activo) VALUES ('Recursos Humanos',1);


INSERT INTO Empleados (NumeroEmpleado, Nombre, FechaIngreso, SalarioMensual, DepartamentoId, Activo)
VALUES ('EMP-0001', 'Juan Perez', '2025-01-15', 5000.00, 1, 1);

INSERT INTO Empleados (NumeroEmpleado, Nombre, FechaIngreso, SalarioMensual, DepartamentoId, Activo)
VALUES ('EMP-0002', 'Ana Gomez', '2024-06-10', 20000.00, 1, 1); 

INSERT INTO Empleados (NumeroEmpleado, Nombre, FechaIngreso, SalarioMensual, DepartamentoId, Activo)
VALUES ('EMP-0003', 'Carlos Lopez', '2023-11-01', 60000.00, 1, 1); 

--Ordenar empleados
SELECT 
    e.NumeroEmpleado, 
    e.Nombre,
    d.Nombre AS departamento,
    e.SalarioMensual, 
    e.DepartamentoId AS empleado, 
    DATEDIFF(YEAR, e.FechaIngreso, GETDATE()) AS AntiguedadAnios
FROM Empleados e 
JOIN Departamentos d ON e.DepartamentoId = d.Id 
WHERE e.Activo = 1;

--Calcular nomina
SELECT 
    e.NumeroEmpleado,
    e.Nombre,
    e.SalarioMensual,
    (e.SalarioMensual / 2) AS SalarioQuincenal,
    ((e.SalarioMensual / 2) * 0.03) AS DeduccionIMSS,
    
    -- C lculo del ISR seg n los rangos quincenales
    CASE 
        WHEN (e.SalarioMensual / 2) <= 7500 THEN ((e.SalarioMensual / 2) * 0.09)
        WHEN (e.SalarioMensual / 2) BETWEEN 7501 AND 15000 THEN ((e.SalarioMensual / 2) * 0.16)
        ELSE ((e.SalarioMensual / 2) * 0.25)
    END AS DeduccionISR,
    
    -- C lculo del Total Neto
    ((e.SalarioMensual / 2) 
     - ((e.SalarioMensual / 2) * 0.03) 
     - CASE 
            WHEN (e.SalarioMensual / 2) <= 7500 THEN ((e.SalarioMensual / 2) * 0.09)
            WHEN (e.SalarioMensual / 2) BETWEEN 7501 AND 15000 THEN ((e.SalarioMensual / 2) * 0.16)
            ELSE ((e.SalarioMensual / 2) * 0.25)
       END) AS TotalNeto

FROM Empleados e
WHERE e.Activo = 1;