# Chat Asistente con IA - Firmeza

## 🤖 Descripción

Se ha integrado un **chat asistente inteligente** en la aplicación Firmeza que utiliza Google Gemini AI para responder preguntas de los clientes sobre productos, pedidos, envíos y más.

## ✨ Características

- **Interfaz flotante moderna** con diseño glassmorphism
- **Respuestas en tiempo real** usando Google Gemini AI
- **Preguntas rápidas** para facilitar la interacción
- **Indicador de escritura** mientras el bot procesa la respuesta
- **Fallback inteligente** con respuestas basadas en palabras clave si la API no está disponible
- **Diseño responsive** que se adapta a móviles y tablets
- **Animaciones suaves** para mejor experiencia de usuario

## 🔧 Configuración

### 1. Obtener API Key de Google Gemini

1. Ve a [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Inicia sesión con tu cuenta de Google
3. Haz clic en "Create API Key"
4. Copia la API key generada

### 2. Configurar la API Key en el Backend

Edita los archivos de configuración de la API:

**`gestion_construcion.api/appsettings.json`**
```json
{
  ...
  "GeminiApiKey": "TU_API_KEY_AQUI"
}
```

**`gestion_construcion.api/appsettings.Development.json`**
```json
{
  ...
  "GeminiApiKey": "TU_API_KEY_AQUI"
}
```

### 3. Ejecutar la Aplicación

#### Con Docker:
```bash
docker compose up --build
```

#### Sin Docker:

**Backend (API):**
```bash
cd gestion_construcion.api
dotnet run
```

**Frontend (React):**
```bash
cd Firmeza.Client
npm install
npm run dev
```

## 📝 Uso

1. El chat aparece como un **botón flotante azul** en la esquina inferior derecha
2. Haz clic en el botón para abrir el chat
3. Escribe tu pregunta o selecciona una pregunta rápida
4. El asistente responderá automáticamente

## 🎨 Personalización

### Modificar el Prompt del Sistema

Edita el archivo `gestion_construcion.api/Controllers/ChatController.cs` en la función `CallGeminiApi`:

```csharp
var systemPrompt = @"Eres un asistente virtual amigable y profesional de Firmeza...";
```

### Agregar más Preguntas Rápidas

Edita el archivo `Firmeza.Client/src/components/ChatAssistant.jsx`:

```javascript
const quickQuestions = [
  '¿Qué productos tienen disponibles?',
  '¿Cómo puedo hacer un pedido?',
  // Agrega más preguntas aquí
];
```

### Modificar Respuestas de Fallback

Edita la función `GetFallbackResponse` en `ChatController.cs` para personalizar las respuestas cuando la API de Gemini no está disponible.

## 🔒 Seguridad

- La API key se almacena en el servidor (backend) y nunca se expone al cliente
- Las conversaciones no se almacenan en la base de datos (puedes agregar esta funcionalidad si lo deseas)
- El endpoint del chat está abierto, pero puedes agregar autenticación JWT si lo necesitas

## 🚀 Funcionalidades Futuras

- [ ] Guardar historial de conversaciones en la base de datos
- [ ] Análisis de sentimientos de los clientes
- [ ] Integración con el sistema de productos para respuestas más precisas
- [ ] Notificaciones push cuando el cliente recibe una respuesta
- [ ] Panel de administración para ver las conversaciones

## 📊 Archivos Creados/Modificados

### Frontend (React)
- ✅ `Firmeza.Client/src/components/ChatAssistant.jsx` - Componente principal del chat
- ✅ `Firmeza.Client/src/styles/ChatAssistant.css` - Estilos del chat
- ✅ `Firmeza.Client/src/App.jsx` - Integración del chat en la app

### Backend (.NET)
- ✅ `gestion_construcion.api/Controllers/ChatController.cs` - Controlador del chat
- ✅ `gestion_construcion.api/Program.cs` - Registro de HttpClient
- ✅ `gestion_construcion.api/appsettings.json` - Configuración de API key
- ✅ `gestion_construcion.api/appsettings.Development.json` - Configuración de desarrollo

## 🆘 Solución de Problemas

### El chat no responde
1. Verifica que la API key esté configurada correctamente
2. Revisa los logs del backend para ver errores
3. Asegúrate de que el backend esté corriendo en `http://localhost:5165`

### Error de CORS
- Verifica que el frontend esté corriendo en `http://localhost:3000`
- Revisa la configuración de CORS en `Program.cs`

### Respuestas genéricas
- Si la API de Gemini no está disponible, el sistema usa respuestas de fallback
- Configura la API key para obtener respuestas inteligentes

## 📞 Soporte

Si tienes problemas o preguntas, revisa los logs de la aplicación o contacta al equipo de desarrollo.

---

**¡Disfruta del nuevo chat asistente con IA!** 🎉
