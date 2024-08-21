using APICalendario.Controllers;
using APICalendario.DTOs;
using APICalendario.Models;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLembretexUnitTests.UnitTests;

public class PostLembreteUnitTests : IClassFixture<LembretesUnitTestController>
{
    private readonly LembretesController _controller;

    public PostLembreteUnitTests(LembretesUnitTestController controller)
    {
        _controller = new LembretesController(controller.repositorycreate, controller.mapper);
    }

    [Fact]

    public async Task PostLembrete_Return_CreatedStatusCode()
    {
        //Arrange       
        var novoLembrete = new LembreteDTO
        {
            Titulo = "Treino",
            Descricao = "Ir pra academia",
            Data = new DateOnly(2024, 05, 07),
            HoraInicio = new TimeOnly(13, 0, 0),
            HoraFinal = new TimeOnly(17, 0, 0),
            DiaTodo = false,
            Frequencia = "0"
        };
        
        //act
        var data = await _controller.Post(novoLembrete);

        //Assert (Fluentassertions)
        var result = data.Should().BeOfType<ActionResult<LembreteDTO>>().Subject;
        result.Result.Should().BeOfType<CreatedAtRouteResult>();

        var createdAtRouteResult = result.Result.As<CreatedAtRouteResult>();
        createdAtRouteResult.StatusCode.Should().Be(201);
    }

    [Fact]

    public async Task PostLembrete_Return_BadRequestObject()
    {
        //Arrange       
        LembreteDTO lembrete = null;

        //act
        var data = await _controller.Post(lembrete);

        //Assert (Fluentassertions)
        var badRequestResult = data.Result.Should().BeOfType<BadRequestObjectResult>();
        badRequestResult.Subject.StatusCode.Should().Be(400);
    }
}
