using APICalendario.Controllers;
using APICalendario.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLembretexUnitTests.UnitTests;

public class DeleteLembretesUnitTests : IClassFixture<LembretesUnitTestController>
{
    private readonly LembretesController _controller;

    public DeleteLembretesUnitTests(LembretesUnitTestController controller)
    {
        _controller = new LembretesController(controller.repositorycreate, controller.mapper);
    }

    [Fact]

    public async Task PostLembrete_Return_CreatedStatusCode()
    {
        //Arrange       
        var Id = 4;

        //act
        var result = await _controller.Delete(Id);

        //Assert (Fluentassertions)
        result.Should().NotBeNull();//verifica se n nulo
        result.Result.Should().BeOfType<OkObjectResult>();//verifica se o resultado é OkObjectResult
    }

    [Fact]
    public async Task PostLembrete_Return_Notfound()
    {
        //Arrange       
        var Id = 999;

        //act
        var result = await _controller.Delete(Id);

        //Assert (Fluentassertions)
        result.Should().NotBeNull();//verifica se n nulo
        result.Result.Should().BeOfType<NotFoundObjectResult>();//verifica se o resultado é OkObjectResult

    }
}
