
using APICalendario.DTOs;
using APICalendario.Models;
using APICalendario.Pagination;
using APICalendario.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using X.PagedList;

namespace APICalendario.Controllers;

[Route("[controller]")]
[ApiController]
[EnableRateLimiting("fixedwindow")]
public class LembretesController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public LembretesController(IUnitOfWork uof,
                               IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> Get()
    //IEnumerable retorna uma lista de objetos lembrete
    {
        var lembretes = await _uof.LembreteRepository.GetAllAsync();

        //var destino = _mapper.map<Destino>(origem);
        var lembretesDto = _mapper.Map<IEnumerable<LembreteDTO>>(lembretes);

        return Ok(lembretesDto);
    }

    [HttpGet("{id}", Name = "ObterLembrete")]
    [Authorize]
    public async Task<ActionResult<LembreteDTO>> Get(int id)
    {
        var lembrete = await _uof.LembreteRepository.GetAsync(l => l.Id == id);

        if (lembrete is null)
            return NotFound("Lembrete não encontrado...");

        if (lembrete.Id != id)
            return NotFound($"Lembrete de id= {id} nao encontrado");

        var lembretesDto = _mapper.Map<LembreteDTO>(lembrete);

        return Ok(lembretesDto);
    }

    [HttpGet("pagination")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> Get([FromQuery]
                                         LembretesParameters lembretesParams)
    {
        var lembretes = await _uof.LembreteRepository.GetLembretesAsync(lembretesParams);
        return ObterLembretes(lembretes);

    }

    [HttpGet("filter/data/pagination")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> GetLembretesFilterData([FromQuery] LembretesFiltroData lembretesFiltro)
    {
        var lembretesFiltradosData =await _uof.LembreteRepository
            .GetLembretesFiltroDataAsync(lembretesFiltro);

        return ObterLembretes(lembretesFiltradosData);
    }

    [HttpGet("filter/titulo/pagination")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> GetLembretesFilterNome([FromQuery] LembretesFiltroNome lembretesFiltroParams)
    {
        var lembretesFiltradosNome = await _uof.LembreteRepository
            .GetLembretesFiltroNomeAsync(lembretesFiltroParams);

        return ObterLembretes(lembretesFiltradosNome);
    }

    [HttpPost]//Recebe um Lembrete DTO
    [Authorize]
    public async Task<ActionResult<LembreteDTO>> Post(LembreteDTO lembreteDto)
    {
        if (lembreteDto == null)        
            return BadRequest("Lembrete inválido");
        
        var lembrete = _mapper.Map<Lembrete>(lembreteDto);//Precisa transformar um DTO em lembrete para ser salvo no db

        var novoLembrete = _uof.LembreteRepository.Create(lembrete);// Apos ser salvo no db
        await _uof.CommitAsync();

        var novoLembreteDto = _mapper.Map<IEnumerable<LembreteDTO>>(novoLembrete);// Mapeia de volta para DTO

        return new CreatedAtRouteResult("ObterLembrete", new { id = novoLembreteDto.First().Id }, novoLembreteDto);

    }

    [HttpPut("{id:int}")]//recebe uma Id Dto
    [Authorize]
    public async Task<ActionResult<LembreteDTO>> Put(int id, LembreteDTO lembreteDto)
    {
        if (id != lembreteDto.Id)
            return BadRequest();

        var lembrete = _mapper.Map<Lembrete>(lembreteDto);//Precisa transformar um DTO em lembrete para ser alterado no db

        var lembreteAtualizado = _uof.LembreteRepository.Update(lembrete);
        await _uof.CommitAsync();

        var lembreteAtualizadoDto = _mapper.Map<LembreteDTO>(lembreteAtualizado);

        return Ok(lembreteAtualizado);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<LembreteDTO>> Delete(int id)
    {
        var lembrete = await _uof.LembreteRepository.GetAsync(l => l.Id == id);

        if (lembrete is null)        
           return NotFound("Lembrete não encontrado...");
        
        var lembreteExcluido = _uof.LembreteRepository.Delete(lembrete);
        await _uof.CommitAsync();

        var lembreteExcluidoDto = _mapper.Map<LembreteDTO>(lembreteExcluido);

        return Ok(lembreteExcluidoDto);
    }
    private ActionResult<IEnumerable<LembreteDTO>> ObterLembretes(IPagedList<Lembrete> lembretes)
    {
        var metadata = new
        {
            lembretes.Count,
            lembretes.PageSize,
            lembretes.PageCount,
            lembretes.TotalItemCount,
            lembretes.HasNextPage,
            lembretes.HasPreviousPage
        };
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var lembretesDTO = _mapper.Map<IEnumerable<LembreteDTO>>(lembretes);
        return Ok(lembretesDTO);
    }
}
