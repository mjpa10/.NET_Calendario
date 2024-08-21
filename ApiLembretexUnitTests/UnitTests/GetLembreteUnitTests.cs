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

public class GetLembreteUnitTests : IClassFixture<LembretesUnitTestController>
{
    private readonly LembretesController _controller;


public GetLembreteUnitTests(LembretesUnitTestController controller)
{
    _controller = new LembretesController(controller.repository, controller.mapper);
}
    [Fact]
    public async Task GetLembreteById_OKResult()
    {
        //Arrange
        var lembId = 10;

        //Act
        var data = await _controller.Get(lembId);

        //Assert (Xunit)
       // var okResult = Assert.IsType<OkObjectResult>(data.Result);
       // Assert.Equal(200, okResult.StatusCode);

        //Assert (Fluentassertions)
        data.Result.Should().BeOfType<OkObjectResult>()// verifica se o obj é tipo OKobjectresult
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetLembreteById_Return_NotFound()
    {
        //Arrange
        var lembId = 999;

        //Act
        var data = await _controller.Get(lembId);
         
        //Assert (Fluentassertions)
        data.Result.Should().BeOfType<NotFoundObjectResult>()// verifica se o obj é tipo OKobjectresult
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetLembreteById_Return_BadRequest()
    {
        //Arrange
        var lembId = -1;

        //Act
        var data = await _controller.Get(lembId);

        //Assert (Fluentassertions)
        data.Result.Should().BeOfType<BadRequestObjectResult>()// verifica se o obj é tipo OKobjectresult
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetLembreteById_Return_ListOfLembretesDTO()
    {
     
        //Act
        var data = await _controller.Get();

        //Assert (Fluentassertions)
        data.Result.Should().BeOfType<OkObjectResult>()// verifica se o obj é tipo OKobjectresult
            .Which.Value.Should().BeAssignableTo<IEnumerable<LembreteDTO>>()//verifica se o valor é atribuivel a  IEnumerable<LembreteDTO
            .And.NotBeNull();// e se n é null
    }


}
