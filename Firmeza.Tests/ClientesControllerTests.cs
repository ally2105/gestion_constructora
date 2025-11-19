using Xunit;
using Moq;
using gestion_construccion.web.Controllers;
using Firmeza.Core.Interfaces; // Actualizado
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firmeza.Core.Models; // Actualizado

namespace Firmeza.Tests
{
    public class ClientesControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfClientes()
        {
            // Arrange
            var mockService = new Mock<IClienteService>();
            // Usar (string?)null para ser explícito con la nulabilidad
            mockService.Setup(service => service.SearchClientesAsync((string?)null))
                .ReturnsAsync(new List<Cliente>());
            var controller = new ClientesController(mockService.Object);

            // Act
            var result = await controller.Index((string?)null); // Usar (string?)null

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Cliente>>(viewResult.ViewData.Model);
        }
    }
}
