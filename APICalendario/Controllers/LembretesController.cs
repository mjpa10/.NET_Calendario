
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
using Microsoft.AspNetCore.Http;

namespace APICalendario.Controllers;

[Route("[controller]")]
[ApiController]
[EnableRateLimiting("fixedwindow")]
[Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
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

    /// <summary>
    ///  retorna uma lista de objetos Lembrete
    /// </summary>
    /// <returns>Uma lista de objetos Lembrete</returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> Get()
    {
        var lembretes = await _uof.LembreteRepository.GetAllAsync();
        //var destino = _mapper.map<Destino>(origem);
        var lembretesDto = _mapper.Map<IEnumerable<LembreteDTO>>(lembretes);
        return Ok(lembretesDto);
    }

    /// <summary>
    /// Obtem um Lembrete pelo seu Id
    /// </summary>
    /// <param name="id">Id do lembrete a ser Exibido></param>
    /// <returns>objeto Lembrete</returns>
    [HttpGet("{id}", Name = "ObterLembrete")]
    [Authorize]
    public async Task<ActionResult<LembreteDTO>> Get(int id)
    {
        var lembrete = await _uof.LembreteRepository.GetAsync(l => l.Id == id);

        if (id == null || id <= 0)
        {
            return BadRequest("Id de Lembrete Invalido");
        }

        if (lembrete is null)
            return NotFound("Lembrete não encontrado...");

        if (lembrete.Id != id)
            return NotFound($"Lembrete de id= {id} nao encontrado");

        var lembretesDto = _mapper.Map<LembreteDTO>(lembrete);

        return Ok(lembretesDto);
    }

    /// <summary>
    /// Retorna uma lista paginada de objetos Lembrete
    /// </summary>
    /// <param name="lembretesParams">Parâmetros de paginação</param>
    /// <returns>Uma lista paginada de objetos Lembrete</returns>
    [HttpGet("pagination")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> Get([FromQuery]
                                         LembretesParameters lembretesParams)
    {
        var lembretes = await _uof.LembreteRepository.GetLembretesAsync(lembretesParams);
        return ObterLembretes(lembretes);

    }

    /// <summary>
    /// Retorna uma lista paginada de objetos Lembrete filtrados por data
    /// </summary>
    /// <returns>Uma lista paginada de objetos Lembrete filtrados por data</returns>
    [HttpGet("filter/data/pagination")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> GetLembretesFilterData([FromQuery] LembretesFiltroData lembretesFiltro)
    {
        var lembretesFiltradosData =await _uof.LembreteRepository
            .GetLembretesFiltroDataAsync(lembretesFiltro);

        return ObterLembretes(lembretesFiltradosData);
    }

    /// <summary>
    /// Retorna uma lista paginada de objetos Lembrete filtrados por título
    /// </summary>
    /// <param name="lembretesFiltroParams">Parâmetros de filtro e paginação</param>
    /// <returns>Uma lista paginada de objetos Lembrete filtrados por título</returns>
    [HttpGet("filter/titulo/pagination")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LembreteDTO>>> GetLembretesFilterNome([FromQuery] LembretesFiltroNome lembretesFiltroParams)
    {
        var lembretesFiltradosNome = await _uof.LembreteRepository
            .GetLembretesFiltroNomeAsync(lembretesFiltroParams);

        return ObterLembretes(lembretesFiltradosNome);
    }

    /// <summary>
    /// Cria um novo Lembrete
    /// </summary>
    /// <param name="lembreteDto">Dados do Lembrete a ser criado</param>
    /// <returns>O Lembrete criado</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<LembreteDTO>> Post(LembreteDTO lembreteDto)
    {
        if (lembreteDto is null)        
            return BadRequest("Lembrete inválido");
        
        var lembrete = _mapper.Map<Lembrete>(lembreteDto);//Precisa transformar um DTO em lembrete para ser salvo no db

        var novoLembrete = _uof.LembreteRepository.Create(lembrete);// Apos ser salvo no db
        await _uof.CommitAsync();

        var novoLembreteDto = _mapper.Map<IEnumerable<LembreteDTO>>(novoLembrete);// Mapeia de volta para DTO

        return new CreatedAtRouteResult("ObterLembrete", new { id = novoLembreteDto.First().Id }, novoLembreteDto);

    }

    /// <summary>
    /// Atualiza um Lembrete existente
    /// </summary>
    /// <param name="id">Id do Lembrete a ser atualizado</param>
    /// <param name="lembreteDto">Dados atualizados do Lembrete</param>
    /// <returns>O Lembrete atualizado</returns>
    [HttpPut("{id:int}")]//recebe uma Id Dto
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
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

    /// <summary>
    /// Exclui um Lembrete existente
    /// </summary>
    /// <param name="id">Id do Lembrete a ser excluído</param>
    /// <returns>O Lembrete excluído</returns>
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

    /// <summary>
    /// Monta e retorna uma lista paginada de lembretes
    /// </summary>
    /// <param name="lembretes">Lista paginada de Lembretes</param>
    /// <returns>Lista paginada de Lembretes DTO</returns>
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
