-- Stored procedures required by the application

USE UTSCitas;
GO

-- sp_InsertUsuario
IF OBJECT_ID('sp_InsertUsuario', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertUsuario;
GO
CREATE PROCEDURE sp_InsertUsuario
    @Nombre NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Usuarios (Nombre, Correo, Password)
    VALUES (@Nombre, @Correo, @Password);
END;
GO

-- sp_InsertCita
IF OBJECT_ID('sp_InsertCita', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertCita;
GO
CREATE PROCEDURE sp_InsertCita
    @IdUsuario INT,
    @IdProfesional INT,
    @Fecha DATETIME,
    @TipoCita NVARCHAR(100),
    @IdEstado INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Citas (IdUsuario, IdProfesional, Fecha, TipoCita, IdEstado)
    VALUES (@IdUsuario, @IdProfesional, @Fecha, @TipoCita, @IdEstado);
END;
GO
