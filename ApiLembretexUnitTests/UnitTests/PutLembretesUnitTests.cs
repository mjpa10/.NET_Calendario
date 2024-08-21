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

public class PutLembretesUnitTests : IClassFixture<LembretesUnitTestController>
{
    private readonly LembretesController _controller;

    public PutLembretesUnitTests(LembretesUnitTestController controller)
    {
        _controller = new LembretesController(controller.repository, controller.mapper);
    }

    [Fact]
    public async Task PutLembrete_Return_OKResult()
    {
        //Arrange
        var lembId = 4;

        var UpdatelembreteDto = new LembreteDTO
        { 
            Id = lembId,
            Titulo = "Atualizado",
            Descricao = "atualizado",
            Data = new DateOnly(2024, 05, 07),
            HoraInicio = new TimeOnly(13, 0, 0),
            HoraFinal = new TimeOnly(17, 0, 0),
            DiaTodo = false,
            Frequencia = "0"
        };

        //Act
        var result = await _controller.Put(lembId, UpdatelembreteDto);

     
        //Assert (Fluentassertions)
       result.Should().NotBeNull();//verifica se n nulo
        result.Result.Should().BeOfType<OkObjectResult>();//verifica se o resultado é OkObjectResult
    }

    [Fact]
    public async Task PutLembrete_Return_BadRequest()
    {
        //Arrange
        var lembId = 5;

        var UpdatelembreteDto = new LembreteDTO
        {
            Id = 3,
            Titulo = "Atualizado",
            Descricao = "atualizado",
            Data = new DateOnly(2024, 05, 07),
            HoraInicio = new TimeOnly(13, 0, 0),
            HoraFinal = new TimeOnly(17, 0, 0),
            DiaTodo = false,
            Frequencia = "0"
        };

        //Act
        var result = await _controller.Put(lembId, UpdatelembreteDto);

        //Assert (Fluentassertions)
        result.Should().NotBeNull();//verifica se n nulo
        result.Result.Should().BeOfType<BadRequestResult>();//verifica se o resultado é OkObjectResult
    }
}
