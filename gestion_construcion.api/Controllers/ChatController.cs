using Firmeza.Core.Interfaces;
using Firmeza.Core.Models;
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
        private readonly IUnitOfWork _unitOfWork;

        public ChatController(
            IConfiguration configuration,
            ILogger<ChatController> logger,
            IHttpClientFactory httpClientFactory,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _unitOfWork = unitOfWork;
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

                // Obtener catálogo actual de productos de la base de datos
                var productos = await _unitOfWork.Productos.GetAllAsync();
                var catalogSummary = string.Join("; ", productos.Select(p => $"{p.Nombre} (Precio: ${p.Precio:N0} COP, Stock: {p.Stock})"));

                // Buscar si hay productos relevantes para retornar en el chat
                var searchLower = request.Message.ToLower();
                var matchingProducts = productos
                    .Where(p => p.Nombre.ToLower().Contains(searchLower) || p.Descripcion.ToLower().Contains(searchLower))
                    .Take(3)
                    .Select(p => new ChatProductDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Precio = p.Precio,
                        Stock = p.Stock
                    })
                    .ToList();

                var geminiApiKey = _configuration["GeminiApiKey"];

                if (string.IsNullOrEmpty(geminiApiKey))
                {
                    _logger.LogWarning("GeminiApiKey no configurada, usando respuesta de fallback");
                    return Ok(new ChatMessageResponse
                    {
                        Response = GetFallbackResponse(request.Message),
                        Products = matchingProducts
                    });
                }

                // Llamar a Gemini API con el contexto real del catálogo
                var geminiResponse = await CallGeminiApi(request.Message, catalogSummary, request.CartContext, geminiApiKey);

                return Ok(new ChatMessageResponse
                {
                    Response = geminiResponse,
                    Products = matchingProducts.Count > 0 ? matchingProducts : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el mensaje del chat");
                return Ok(new ChatMessageResponse
                {
                    Response = "¡Hola! Estoy listo para ayudarte a encontrar los mejores materiales de construcción. ¿Qué proyecto estás planeando?",
                    Products = null
                });
            }
        }

        private async Task<string> CallGeminiApi(string message, string catalogSummary, string? cartContext, string apiKey)
        {
            try
            {
                var systemPrompt = $@"Eres un experto asesor de ventas e-commerce de 'Firmeza', la tienda líder en materiales de construcción.
Tu misión es aconsejar a los clientes, responder dudas sobre productos, envíos, métodos de pago (MercadoPago, PSE, Tarjetas) y ayudarlos a comprar.

INFORMACIÓN DEL CATÁLOGO REAL EN TIEMPO REAL:
{catalogSummary}

CARRITO ACTUAL DEL CLIENTE:
{cartContext ?? "Vacío"}

REGLAS DE RESPUESTA:
- Responde de forma muy amigable, entusiasta y profesional.
- Sé conciso (máximo 3-4 frases bien estructuradas).
- Usa formato markdown leve (usa **negritas** para precios y nombres de productos).
- Si el cliente pregunta precios o disponibilidad, usa SIEMPRE los datos exactos del catálogo provisto arriba.
- Invítalos siempre a agregar al carrito o comprar con MercadoPago.";

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
                        maxOutputTokens = 250,
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

            if (lowerMessage.Contains("producto") || lowerMessage.Contains("catálogo") || lowerMessage.Contains("cemento"))
            {
                return "Contamos con materiales de la más alta calidad como **Cemento Gray Portland**, **Varilla de Acero**, **Ladrillos**, y más. Revisa nuestro catálogo arriba o consulta precios en tiempo real.";
            }
            else if (lowerMessage.Contains("pago") || lowerMessage.Contains("mercadopago") || lowerMessage.Contains("tarjeta"))
            {
                return "Aceptamos **MercadoPago**, tarjetas de crédito/débito, **PSE** y **Nequi**. Tu pago está protegido al 100%.";
            }
            else if (lowerMessage.Contains("envío") || lowerMessage.Contains("entrega"))
            {
                return "Realizamos **envíos a domicilio gratis** en compras superiores a $200.000 COP. Los despachos tardan entre 24 y 48 horas.";
            }
            else if (lowerMessage.Contains("hola") || lowerMessage.Contains("buenos"))
            {
                return "¡Hola! Bienvenido a **Firmeza**. Estoy aquí para asesorarte en tus compras de materiales. ¿Qué estás construyendo o remodelando hoy?";
            }
            else
            {
                return "¡Con gusto te ayudo! Puedes explorar nuestros productos en el catálogo, agregarlos al carrito y pagar en segundos con MercadoPago. ¿Necesitas recomendación de algún material?";
            }
        }
    }

    public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? CartContext { get; set; }
    }

    public class ChatMessageResponse
    {
        public string Response { get; set; } = string.Empty;
        public List<ChatProductDto>? Products { get; set; }
    }

    public class ChatProductDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
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
