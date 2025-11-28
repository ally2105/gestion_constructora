# 🐳 Guía: Configurar PostgreSQL con Docker para Gestión Constructora

## ✅ Estado Actual
- ✓ Docker Compose configurado
- ✓ Archivos appsettings.Development.json actualizados
- ✓ Configuración lista para PostgreSQL local

## 📝 Pasos para Iniciar la Base de Datos

### Paso 1: Levantar SOLO la Base de Datos PostgreSQL

Abre una terminal PowerShell en la carpeta del proyecto y ejecuta:

```powershell
cd "c:\Users\Saray P\Desktop\gestion_constructora"
docker-compose up db -d
```

**Qué hace este comando:**
- `docker-compose up`: Inicia los servicios de Docker Compose
- `db`: Levanta SOLO el servicio de PostgreSQL
- `-d`: Lo ejecuta en segundo plano (detached mode)

### Paso 2: Verificar que PostgreSQL está corriendo

```powershell
docker-compose ps
```

Deberías ver algo como:
```
NAME                            STATUS         PORTS
gestion_constructora-db-1       Up (healthy)   0.0.0.0:5432->5432/tcp
```

### Paso 3: Ver los logs de PostgreSQL (opcional)

Si quieres verificar que todo está bien:

```powershell
docker-compose logs db
```

### Paso 4: Ejecutar tu aplicación

Ahora puedes ejecutar tu aplicación .NET normalmente:

**Para la API:**
```powershell
cd "c:\Users\Saray P\Desktop\gestion_constructora\gestion_construcion.api"
dotnet run
```

**Para el Web Admin:**
```powershell
cd "c:\Users\Saray P\Desktop\gestion_constructora\gestion_construccion.web"
dotnet run
```

## 🛠️ Comandos Útiles

### Ver todos los contenedores corriendo:
```powershell
docker ps
```

### Detener PostgreSQL:
```powershell
docker-compose stop db
```

### Detener y eliminar PostgreSQL (mantiene los datos):
```powershell
docker-compose down
```

### Detener y ELIMINAR PostgreSQL con TODOS los datos:
```powershell
docker-compose down -v
```
⚠️ **CUIDADO**: Esto borrará todos los datos de la base de datos!

### Reiniciar PostgreSQL:
```powershell
docker-compose restart db
```

### Conectarse a PostgreSQL desde la terminal:
```powershell
docker exec -it gestion_constructora-db-1 psql -U postgres -d gestion_construccion
```

## 🔧 Configuración de Conexión

Tu aplicación está configurada para conectarse a PostgreSQL con:
- **Host**: localhost
- **Puerto**: 5432
- **Base de datos**: gestion_construccion
- **Usuario**: postgres
- **Contraseña**: Qwe.123*

## 🚀 Opción Avanzada: Levantar TODO con Docker

Si quieres correr tu API y Web Admin también en Docker:

```powershell
docker-compose up --build
```

Esto levantará:
- 🗄️ PostgreSQL en puerto 5432
- 🔌 API en http://localhost:5165
- 🌐 Web Admin en http://localhost:5031

Para detener todo:
```powershell
docker-compose down
```

## ❓ Solución de Problemas

### Error: "puerto 5432 ya en uso"
Significa que ya tienes PostgreSQL corriendo. Opciones:
1. Detén el PostgreSQL existente
2. Cambia el puerto en docker-compose.yml (ejemplo: "5433:5432")

### Error: "Cannot connect to database"
1. Verifica que el contenedor esté corriendo: `docker-compose ps`
2. Verifica los logs: `docker-compose logs db`
3. Espera unos segundos, PostgreSQL tarda en iniciar

### La base de datos está vacía
Entity Framework creará las tablas automáticamente al ejecutar las migraciones.
Si necesitas ejecutarlas manualmente:
```powershell
dotnet ef database update
```

## 📚 Recursos Adicionales

### Crear una migración nueva:
```powershell
dotnet ef migrations add NombreMigracion
```

### Aplicar migraciones:
```powershell
dotnet ef database update
```

### Ver el estado de la base de datos:
```powershell
docker exec gestion_constructora-db-1 psql -U postgres -d gestion_construccion -c "\dt"
```

---

**¡Listo!** Tu base de datos PostgreSQL local con Docker está configurada. 🎉
