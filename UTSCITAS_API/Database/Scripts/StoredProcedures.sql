-- =====================================================
-- SCRIPT COMPLETO DE STORED PROCEDURES
-- Base de datos: GeneradorCitas
-- =====================================================

USE GeneradorCitas;
GO

-- ============================================================
-- USUARIOS
-- ============================================================

-- sp_InsertarUsuario (nombre exacto que usa UsuarioService.cs)
IF OBJECT_ID('sp_InsertarUsuario', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertarUsuario;
GO
CREATE PROCEDURE sp_InsertarUsuario
    @Matricula   NVARCHAR(50),
    @Nombre      NVARCHAR(100),
    @Correo      NVARCHAR(254),
    @Password    NVARCHAR(500),
    @Carrera     NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    -- Verificar correo duplicado
    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo)
    BEGIN
        RAISERROR('Ya existe un usuario con ese correo.', 16, 1);
        RETURN;
    END
    INSERT INTO Usuarios (Matricula, Nombre, Correo, Password, Carrera)
    VALUES (@Matricula, @Nombre, @Correo, @Password, @Carrera);
    SELECT SCOPE_IDENTITY() AS IdUsuario;
END;
GO

-- sp_ListarUsuarios
IF OBJECT_ID('sp_ListarUsuarios', 'P') IS NOT NULL
    DROP PROCEDURE sp_ListarUsuarios;
GO
CREATE PROCEDURE sp_ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Matricula, Nombre, Correo, Carrera
    FROM Usuarios;
END;
GO

-- sp_BuscarUsuarioPorId
IF OBJECT_ID('sp_BuscarUsuarioPorId', 'P') IS NOT NULL
    DROP PROCEDURE sp_BuscarUsuarioPorId;
GO
CREATE PROCEDURE sp_BuscarUsuarioPorId
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Matricula, Nombre, Correo, Password, Carrera
    FROM Usuarios
    WHERE IdUsuario = @IdUsuario;
END;
GO

-- sp_BuscarUsuarioPorCorreo
IF OBJECT_ID('sp_BuscarUsuarioPorCorreo', 'P') IS NOT NULL
    DROP PROCEDURE sp_BuscarUsuarioPorCorreo;
GO
CREATE PROCEDURE sp_BuscarUsuarioPorCorreo
    @Correo NVARCHAR(254)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Matricula, Nombre, Correo, Password, Carrera
    FROM Usuarios
    WHERE Correo = @Correo;
END;
GO

-- sp_ActualizarUsuario
IF OBJECT_ID('sp_ActualizarUsuario', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActualizarUsuario;
GO
CREATE PROCEDURE sp_ActualizarUsuario
    @IdUsuario INT,
    @Matricula NVARCHAR(50),
    @Nombre    NVARCHAR(100),
    @Correo    NVARCHAR(254),
    @Password  NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuarios
    SET Matricula = @Matricula,
        Nombre    = @Nombre,
        Correo    = @Correo,
        Password  = @Password
    WHERE IdUsuario = @IdUsuario;
END;
GO

-- sp_EliminarUsuario
IF OBJECT_ID('sp_EliminarUsuario', 'P') IS NOT NULL
    DROP PROCEDURE sp_EliminarUsuario;
GO
CREATE PROCEDURE sp_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario;
END;
GO

-- ============================================================
-- PROFESIONALES
-- ============================================================

-- sp_ListarProfesionales
IF OBJECT_ID('sp_ListarProfesionales', 'P') IS NOT NULL
    DROP PROCEDURE sp_ListarProfesionales;
GO
CREATE PROCEDURE sp_ListarProfesionales
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProfesional, Nombre, Especialidad
    FROM Profesionales;
END;
GO

-- sp_BuscarProfesionalPorId
IF OBJECT_ID('sp_BuscarProfesionalPorId', 'P') IS NOT NULL
    DROP PROCEDURE sp_BuscarProfesionalPorId;
GO
CREATE PROCEDURE sp_BuscarProfesionalPorId
    @IdProfesional INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProfesional, Nombre, Especialidad
    FROM Profesionales
    WHERE IdProfesional = @IdProfesional;
END;
GO

-- sp_InsertarProfesional
IF OBJECT_ID('sp_InsertarProfesional', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertarProfesional;
GO
CREATE PROCEDURE sp_InsertarProfesional
    @Nombre       NVARCHAR(100),
    @Especialidad NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Profesionales (Nombre, Especialidad)
    VALUES (@Nombre, @Especialidad);
    SELECT SCOPE_IDENTITY() AS IdProfesional;
END;
GO

-- sp_ActualizarProfesional
IF OBJECT_ID('sp_ActualizarProfesional', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActualizarProfesional;
GO
CREATE PROCEDURE sp_ActualizarProfesional
    @IdProfesional INT,
    @Nombre        NVARCHAR(100),
    @Especialidad  NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Profesionales
    SET Nombre = @Nombre, Especialidad = @Especialidad
    WHERE IdProfesional = @IdProfesional;
END;
GO

-- sp_EliminarProfesional
IF OBJECT_ID('sp_EliminarProfesional', 'P') IS NOT NULL
    DROP PROCEDURE sp_EliminarProfesional;
GO
CREATE PROCEDURE sp_EliminarProfesional
    @IdProfesional INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Profesionales WHERE IdProfesional = @IdProfesional;
END;
GO

-- ============================================================
-- CITAS
-- ============================================================

-- sp_InsertarCita (nombre exacto que usa CitaService.cs)
IF OBJECT_ID('sp_InsertarCita', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertarCita;
GO
CREATE PROCEDURE sp_InsertarCita
    @IdUsuario     INT,
    @IdProfesional INT,
    @Fecha         DATETIME,
    @TipoCita      NVARCHAR(100),
    @IdEstado      INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Citas (IdUsuario, IdProfesional, Fecha, TipoCita, IdEstado)
    VALUES (@IdUsuario, @IdProfesional, @Fecha, @TipoCita, @IdEstado);
    SELECT SCOPE_IDENTITY() AS IdCita;
END;
GO

-- sp_ListarCitas
IF OBJECT_ID('sp_ListarCitas', 'P') IS NOT NULL
    DROP PROCEDURE sp_ListarCitas;
GO
CREATE PROCEDURE sp_ListarCitas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCita, c.IdUsuario, c.IdProfesional, c.Fecha, c.TipoCita,
           e.Nombre AS Estado
    FROM Citas c
    INNER JOIN EstadosCita e ON c.IdEstado = e.IdEstado;
END;
GO

-- sp_BuscarCitaPorId
IF OBJECT_ID('sp_BuscarCitaPorId', 'P') IS NOT NULL
    DROP PROCEDURE sp_BuscarCitaPorId;
GO
CREATE PROCEDURE sp_BuscarCitaPorId
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCita, c.IdUsuario, c.IdProfesional, c.Fecha, c.TipoCita,
           e.Nombre AS Estado
    FROM Citas c
    INNER JOIN EstadosCita e ON c.IdEstado = e.IdEstado
    WHERE c.IdCita = @IdCita;
END;
GO

-- sp_CitasPorUsuario
IF OBJECT_ID('sp_CitasPorUsuario', 'P') IS NOT NULL
    DROP PROCEDURE sp_CitasPorUsuario;
GO
CREATE PROCEDURE sp_CitasPorUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCita, c.IdUsuario, c.IdProfesional, c.Fecha, c.TipoCita,
           e.Nombre AS Estado
    FROM Citas c
    INNER JOIN EstadosCita e ON c.IdEstado = e.IdEstado
    WHERE c.IdUsuario = @IdUsuario
    ORDER BY c.Fecha DESC;
END;
GO

-- sp_CitasPorProfesional
IF OBJECT_ID('sp_CitasPorProfesional', 'P') IS NOT NULL
    DROP PROCEDURE sp_CitasPorProfesional;
GO
CREATE PROCEDURE sp_CitasPorProfesional
    @IdProfesional INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCita, c.IdUsuario, c.IdProfesional, c.Fecha, c.TipoCita,
           e.Nombre AS Estado
    FROM Citas c
    INNER JOIN EstadosCita e ON c.IdEstado = e.IdEstado
    WHERE c.IdProfesional = @IdProfesional
    ORDER BY c.Fecha DESC;
END;
GO

-- sp_CitasPorFecha
IF OBJECT_ID('sp_CitasPorFecha', 'P') IS NOT NULL
    DROP PROCEDURE sp_CitasPorFecha;
GO
CREATE PROCEDURE sp_CitasPorFecha
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.IdCita, c.IdUsuario, c.IdProfesional, c.Fecha, c.TipoCita,
           e.Nombre AS Estado
    FROM Citas c
    INNER JOIN EstadosCita e ON c.IdEstado = e.IdEstado
    WHERE CAST(c.Fecha AS DATE) BETWEEN @FechaInicio AND @FechaFin
    ORDER BY c.Fecha;
END;
GO

-- sp_ActualizarCita
IF OBJECT_ID('sp_ActualizarCita', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActualizarCita;
GO
CREATE PROCEDURE sp_ActualizarCita
    @IdCita        INT,
    @IdProfesional INT,
    @Fecha         DATETIME,
    @TipoCita      NVARCHAR(100),
    @IdEstado      INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Citas
    SET IdProfesional = @IdProfesional,
        Fecha         = @Fecha,
        TipoCita      = @TipoCita,
        IdEstado      = @IdEstado
    WHERE IdCita = @IdCita;
END;
GO

-- sp_CambiarEstadoCita
IF OBJECT_ID('sp_CambiarEstadoCita', 'P') IS NOT NULL
    DROP PROCEDURE sp_CambiarEstadoCita;
GO
CREATE PROCEDURE sp_CambiarEstadoCita
    @IdCita   INT,
    @IdEstado INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Citas SET IdEstado = @IdEstado WHERE IdCita = @IdCita;
END;
GO

-- sp_EliminarCita
IF OBJECT_ID('sp_EliminarCita', 'P') IS NOT NULL
    DROP PROCEDURE sp_EliminarCita;
GO
CREATE PROCEDURE sp_EliminarCita
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Citas WHERE IdCita = @IdCita;
END;
GO

-- ============================================================
-- DATOS INICIALES: Estados de cita (si no existen)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM EstadosCita)
BEGIN
    INSERT INTO EstadosCita (Nombre) VALUES ('Pendiente');
    INSERT INTO EstadosCita (Nombre) VALUES ('Confirmada');
    INSERT INTO EstadosCita (Nombre) VALUES ('Cancelada');
    INSERT INTO EstadosCita (Nombre) VALUES ('Completada');
END;
GO
