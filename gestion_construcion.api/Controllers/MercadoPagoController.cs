using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace gestion_construcion.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MercadoPagoController> _logger;
        private readonly HttpClient _httpClient;

        public MercadoPagoController(
            IConfiguration configuration,
            ILogger<MercadoPagoController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Crea una preferencia de pago en MercadoPago
        /// </summary>
        [HttpPost("create-preference")]
        public async Task<IActionResult> CreatePreference([FromBody] CreatePreferenceRequest request)
        {
            try
            {
                var accessToken = _configuration["MercadoPago:AccessToken"];
                if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("TEST-000000"))
                {
                    _logger.LogInformation("MercadoPago AccessToken de prueba o no configurado. Devolviendo preferencia simulación.");
                    var mockPrefId = $"MOCK-PREF-{Guid.NewGuid():N}";
                    return Ok(new CreatePreferenceResponse
                    {
                        PreferenceId = mockPrefId,
                        InitPoint = null,
                        SandboxInitPoint = null
                    });
                }

                var preferenceData = new
                {
                    items = request.Items.Select(item => new
                    {
                        title = item.Title,
                        description = item.Description ?? item.Title,
                        quantity = item.Quantity,
                        currency_id = "COP",
                        unit_price = item.UnitPrice
                    }),
                    payer = new
                    {
                        name = request.Payer?.Name ?? "Cliente",
                        email = request.Payer?.Email ?? "cliente@firmeza.com",
                        phone = new
                        {
                            number = request.Payer?.Phone?.Number ?? ""
                        },
                        address = new
                        {
                            street_name = request.Payer?.Address?.StreetName ?? ""
                        }
                    },
                    back_urls = new
                    {
                        success = "http://localhost:3080/order-confirmation",
                        failure = "http://localhost:3080/cart",
                        pending = "http://localhost:3080/order-confirmation"
                    },
                    auto_return = "approved"
                };

                var json = JsonSerializer.Serialize(preferenceData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.PostAsync("https://api.mercadopago.com/checkout/preferences", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);
                    var root = doc.RootElement;

                    var prefId = root.GetProperty("id").GetString();
                    var initPoint = root.GetProperty("init_point").GetString();
                    var sandboxInitPoint = root.GetProperty("sandbox_init_point").GetString();

                    return Ok(new CreatePreferenceResponse
                    {
                        PreferenceId = prefId,
                        InitPoint = initPoint,
                        SandboxInitPoint = sandboxInitPoint
                    });
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error desde MercadoPago API: {response.StatusCode} - {errorMsg}");
                    return StatusCode((int)response.StatusCode, new { message = "Error al crear la preferencia en MercadoPago.", details = errorMsg });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear preferencia de MercadoPago");
                return Ok(new CreatePreferenceResponse
                {
                    PreferenceId = $"FALLBACK-PREF-{Guid.NewGuid():N}",
                    InitPoint = null,
                    SandboxInitPoint = null
                });
            }
        }

        /// <summary>
        /// Webhook para recibir notificaciones de MercadoPago
        /// </summary>
        [HttpPost("webhook")]
        public IActionResult Webhook([FromQuery] string? type, [FromQuery] string? data_id)
        {
            _logger.LogInformation($"Webhook de MercadoPago recibido. Tipo: {type}, ID Data: {data_id}");
            return Ok();
        }
    }

    // Models & DTOs
    public class CreatePreferenceRequest
    {
        public List<PreferenceItemDto> Items { get; set; } = new();
        public PreferencePayerDto? Payer { get; set; }
    }

    public class PreferenceItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PreferencePayerDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public PhoneDto? Phone { get; set; }
        public AddressDto? Address { get; set; }
    }

    public class PhoneDto
    {
        public string? Number { get; set; }
    }

    public class AddressDto
    {
        public string? StreetName { get; set; }
    }

    public class CreatePreferenceResponse
    {
        public string? PreferenceId { get; set; }
        public string? InitPoint { get; set; }
        public string? SandboxInitPoint { get; set; }
    }
}
