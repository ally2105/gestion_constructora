using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace gestion_construcion.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatController> _logger;
        private readonly HttpClient _httpClient;

        public ChatController(IConfiguration configuration, ILogger<ChatController> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { error = "El mensaje no puede estar vacío" });
                }

                // Obtener la API key de Gemini desde la configuración
                var geminiApiKey = _configuration["GeminiApiKey"];
                
                if (string.IsNullOrEmpty(geminiApiKey))
                {
                    _logger.LogWarning("GeminiApiKey no configurada, usando respuesta de fallback");
                    return Ok(new ChatMessageResponse 
                    { 
                        Response = GetFallbackResponse(request.Message) 
                    });
                }

                // Llamar a la API de Gemini
                var geminiResponse = await CallGeminiApi(request.Message, geminiApiKey);
                
                return Ok(new ChatMessageResponse 
                { 
                    Response = geminiResponse 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el mensaje del chat");
                return Ok(new ChatMessageResponse 
                { 
                    Response = "Lo siento, estoy teniendo problemas técnicos. ¿Puedo ayudarte con información sobre nuestros productos o servicios?" 
                });
            }
        }

        private async Task<string> CallGeminiApi(string message, string apiKey)
        {
            try
            {
                var systemPrompt = @"Eres un asistente virtual amigable y profesional de Firmeza, una empresa de construcción. 
Tu objetivo es ayudar a los clientes con información sobre:
- Productos de construcción disponibles
- Proceso de compra y pedidos
- Métodos de pago
- Envíos y entregas
- Información general de la empresa

Responde de manera concisa, amigable y profesional. Si no sabes algo específico, sugiere contactar al equipo de ventas.
Mantén tus respuestas cortas (máximo 3-4 oraciones) y útiles.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = $"{systemPrompt}\n\nCliente: {message}" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 200,
                        topP = 0.8,
                        topK = 40
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                    
                    return geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text 
                        ?? GetFallbackResponse(message);
                }
                else
                {
                    _logger.LogWarning($"Error en API de Gemini: {response.StatusCode}");
                    return GetFallbackResponse(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a la API de Gemini");
                return GetFallbackResponse(message);
            }
        }

        private string GetFallbackResponse(string message)
        {
            var lowerMessage = message.ToLower();

            if (lowerMessage.Contains("producto") || lowerMessage.Contains("catálogo") || lowerMessage.Contains("disponible"))
            {
                return "Contamos con una amplia variedad de productos de construcción. Puedes ver nuestro catálogo completo en la sección de Productos. ¿Hay algo específico que estés buscando?";
            }
            else if (lowerMessage.Contains("pedido") || lowerMessage.Contains("comprar") || lowerMessage.Contains("orden"))
            {
                return "Para hacer un pedido, simplemente agrega los productos que necesitas al carrito y procede al checkout. Recibirás una confirmación por correo electrónico con todos los detalles.";
            }
            else if (lowerMessage.Contains("pago") || lowerMessage.Contains("pagar"))
            {
                return "Aceptamos diversos métodos de pago para tu comodidad. Los detalles específicos se mostrarán durante el proceso de compra. ¿Necesitas más información?";
            }
            else if (lowerMessage.Contains("envío") || lowerMessage.Contains("entrega") || lowerMessage.Contains("domicilio"))
            {
                return "Realizamos envíos a domicilio. Los tiempos y costos de entrega varían según tu ubicación. Puedes consultar esta información al momento de realizar tu pedido.";
            }
            else if (lowerMessage.Contains("precio") || lowerMessage.Contains("costo") || lowerMessage.Contains("cuánto"))
            {
                return "Los precios de nuestros productos están disponibles en el catálogo. Puedes ver el precio de cada artículo en la sección de Productos. ¿Te gustaría ver algún producto en particular?";
            }
            else if (lowerMessage.Contains("hola") || lowerMessage.Contains("buenos") || lowerMessage.Contains("buenas"))
            {
                return "¡Hola! Bienvenido a Firmeza. Estoy aquí para ayudarte con información sobre nuestros productos, pedidos, envíos y más. ¿En qué puedo asistirte?";
            }
            else if (lowerMessage.Contains("gracias"))
            {
                return "¡De nada! Si necesitas algo más, no dudes en preguntar. Estoy aquí para ayudarte. 😊";
            }
            else
            {
                return "Gracias por tu mensaje. Puedo ayudarte con información sobre productos, pedidos, métodos de pago y envíos. ¿Hay algo específico en lo que pueda asistirte?";
            }
        }
    }

    // DTOs
    public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatMessageResponse
    {
        public string Response { get; set; } = string.Empty;
    }

    // Gemini API Response Models
    public class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; }
    }

    public class Candidate
    {
        public Content? Content { get; set; }
    }

    public class Content
    {
        public List<Part>? Parts { get; set; }
    }

    public class Part
    {
        public string? Text { get; set; }
    }
}
