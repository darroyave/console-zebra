# Zebra Scanner API Server

Este servidor API proporciona endpoints para interactuar con dispositivos Zebra Scanner y Scale usando COM/OPOS.

## Características

- **Scanner**: Lectura de códigos de barras
- **Scale**: Medición de peso en tiempo real
- **API REST**: Endpoints HTTP para integración con frontend
- **Swagger UI**: Documentación interactiva de la API
- **CORS**: Configurado para permitir requests desde cualquier origen

## Requisitos

- .NET 9.0
- Dispositivos Zebra Scanner y Scale conectados
- Drivers OPOS instalados

## Instalación

1. Clona el repositorio
2. Restaura las dependencias:
```bash
dotnet restore
```

3. Ejecuta el servidor:
```bash
dotnet run
```

El servidor se ejecutará en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Endpoints de la API

### 1. Inicializar Dispositivos
**POST** `/api/zebra/initialize`

Inicializa los dispositivos scanner y scale.

**Respuesta:**
```json
{
  "success": true,
  "message": "Scanner and scale initialized successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 2. Estado de Dispositivos
**GET** `/api/zebra/status`

Obtiene el estado de conexión de los dispositivos.

**Respuesta:**
```json
{
  "success": true,
  "message": "Device status retrieved successfully",
  "scannerConnected": true,
  "scaleConnected": true,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 3. Obtener Código de Barras
**GET** `/api/zebra/scanner/barcode`

Obtiene el último código de barras escaneado.

**Respuesta:**
```json
{
  "success": true,
  "message": "Barcode data retrieved successfully",
  "barcodeData": "1234567890123",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 4. Obtener Peso Actual
**GET** `/api/zebra/scale/weight`

Obtiene el peso actual de la báscula.

**Respuesta:**
```json
{
  "success": true,
  "message": "Weight data retrieved successfully",
  "weightStatus": "1.250 Kg.",
  "weight": 1250,
  "unit": "Kg.",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 5. Cerrar Dispositivos
**POST** `/api/zebra/close`

Cierra y desconecta todos los dispositivos Zebra.

**Respuesta:**
```json
{
  "success": true,
  "message": "Devices closed successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Uso con Frontend

### JavaScript/TypeScript

```javascript
// Inicializar dispositivos
const initializeDevices = async () => {
  const response = await fetch('/api/zebra/initialize', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
  });
  return await response.json();
};

// Obtener código de barras
const getBarcode = async () => {
  const response = await fetch('/api/zebra/scanner/barcode');
  return await response.json();
};

// Obtener peso
const getWeight = async () => {
  const response = await fetch('/api/zebra/scale/weight');
  return await response.json();
};

// Verificar estado
const getStatus = async () => {
  const response = await fetch('/api/zebra/status');
  return await response.json();
};
```

### React Hook Example

```typescript
import { useState, useEffect } from 'react';

const useZebraDevices = () => {
  const [devices, setDevices] = useState({
    scannerConnected: false,
    scaleConnected: false,
    initialized: false
  });

  const initializeDevices = async () => {
    try {
      const response = await fetch('/api/zebra/initialize', {
        method: 'POST',
      });
      const result = await response.json();
      
      if (result.success) {
        setDevices(prev => ({ ...prev, initialized: true }));
        await checkStatus();
      }
      return result;
    } catch (error) {
      console.error('Error initializing devices:', error);
      return { success: false, message: 'Network error' };
    }
  };

  const checkStatus = async () => {
    try {
      const response = await fetch('/api/zebra/status');
      const result = await response.json();
      
      if (result.success) {
        setDevices(prev => ({
          ...prev,
          scannerConnected: result.scannerConnected,
          scaleConnected: result.scaleConnected
        }));
      }
      return result;
    } catch (error) {
      console.error('Error checking status:', error);
      return { success: false, message: 'Network error' };
    }
  };

  const scanBarcode = async () => {
    try {
      const response = await fetch('/api/zebra/scanner/barcode');
      return await response.json();
    } catch (error) {
      console.error('Error scanning barcode:', error);
      return { success: false, message: 'Network error' };
    }
  };

  const getWeight = async () => {
    try {
      const response = await fetch('/api/zebra/scale/weight');
      return await response.json();
    } catch (error) {
      console.error('Error getting weight:', error);
      return { success: false, message: 'Network error' };
    }
  };

  return {
    devices,
    initializeDevices,
    checkStatus,
    scanBarcode,
    getWeight
  };
};

export default useZebraDevices;
```

## Manejo de Errores

Todos los endpoints devuelven respuestas estructuradas con:
- `success`: boolean indicando si la operación fue exitosa
- `message`: string con detalles del resultado o error
- `timestamp`: fecha y hora de la respuesta

### Códigos de Estado HTTP

- `200 OK`: Operación exitosa
- `400 Bad Request`: Error en la solicitud o dispositivo
- `500 Internal Server Error`: Error interno del servidor

## Configuración

### CORS
El servidor está configurado para permitir requests desde cualquier origen. Para producción, considera restringir esto a dominios específicos.

### Logging
Los logs se escriben en la consola y incluyen información sobre:
- Inicialización de dispositivos
- Errores de conexión
- Operaciones de escaneo y pesaje

## Solución de Problemas

### Dispositivos no conectados
1. Verifica que los drivers OPOS estén instalados
2. Confirma que los dispositivos estén conectados y encendidos
3. Revisa los logs del servidor para mensajes de error específicos

### Errores COM
- Asegúrate de que las referencias COM estén correctamente registradas
- Verifica que el proceso tenga permisos para acceder a los dispositivos
- En sistemas Windows, ejecuta como administrador si es necesario

## Desarrollo

### Estructura del Proyecto

```
consolezebra/
├── Controllers/
│   └── ZebraController.cs      # Endpoints de la API
├── Models/
│   └── ScannerResponse.cs      # Modelos de respuesta
├── Services/
│   └── ZebraScannerService.cs  # Lógica de negocio
├── Program.cs                  # Configuración del servidor
└── consolezebra.csproj        # Configuración del proyecto
```

### Agregar Nuevos Endpoints

1. Agrega el método en `ZebraController.cs`
2. Implementa la lógica en `ZebraScannerService.cs` si es necesario
3. Crea modelos de respuesta en `Models/` si es requerido
4. Documenta el endpoint en Swagger usando comentarios XML

## Licencia

Este proyecto está bajo la licencia MIT.
