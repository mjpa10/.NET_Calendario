
using APICalendario.DTOs;
using APICalendario.Models;
using APICalendario.Pagination;
using APICalendario.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICalendario.Controllers;

[Route("[controller]")]
[ApiController]
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
    public ActionResult<IEnumerable<LembreteDTO>> Get()
    //IEnumerable retorna uma lista de objetos lembrete
    {
      var lembretes = _uof.LembreteRepository.GetLembretes();

        //var destino = _mapper.map<Destino>(origem);
      var lembretesDto = _mapper.Map<IEnumerable< LembreteDTO >>(lembretes);

      return Ok(lembretesDto);
    }
    
    [HttpGet("{id}", Name = "ObterLembrete")]
    public ActionResult<LembreteDTO> Get(int id)
    {
        var lembrete = _uof.LembreteRepository.GetLembrete(id);

        if (lembrete == null)           
            return NotFound("Lembrete não encontrado...");
       
        if (lembrete.Id != id)          
            return NotFound($"Lembrete de id= {id} nao encontrado");

        var lembretesDto = _mapper.Map<LembreteDTO>(lembrete);

        return Ok(lembretesDto);
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<LembreteDTO>> Get([FromQuery]
                                         LembretesParameters lembretesParameters)
    {
        var lembretes = _uof.LembreteRepository.GetLembretesPagination(lembretesParameters);

        var metadata = new
        {
            lembretes.TotalCount,
            lembretes.PageSize,
            lembretes.CurrentPage,
            lembretes.TotalPages,
            lembretes.HasNext,
            lembretes.HasPrevious
        };
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
         
        var lembretesDTO = _mapper.Map<IEnumerable<LembreteDTO>>(lembretes);

        return Ok(lembretesDTO);
    }

    [HttpPost]//Recebe um Lembrete DTO
    public ActionResult<LembreteDTO> Post(LembreteDTO lembreteDto)
    {
        if (lembreteDto == null)
        {
            return BadRequest("Lembrete inválido");
        }
        var lembrete = _mapper.Map<Lembrete>(lembreteDto);//Precisa transformar um DTO em lembrete para ser salvo no db

        var novoLembrete = _uof.LembreteRepository.Create(lembrete);// Apos ser salvo no db

        _uof.Commit();

        var novoLembreteDto = _mapper.Map<IEnumerable<LembreteDTO>>(novoLembrete);// Mapeia de volta para DTO

        return new CreatedAtRouteResult("ObterLembrete", new {id = novoLembreteDto.First().Id }, novoLembreteDto);
                       
    }
    [HttpPut("{id:int}")]//recebe uma Id Dto
    public ActionResult<LembreteDTO> Put(int id, LembreteDTO lembreteDto)
    {
        if (id != lembreteDto.Id)       
            return BadRequest();

        var lembrete = _mapper.Map<Lembrete>(lembreteDto);//Precisa transformar um DTO em lembrete para ser alterado no db

        var lembreteAtualizado= _uof.LembreteRepository.Update(lembrete);       
        _uof.Commit();

        var lembreteAtualizadoDto = _mapper.Map<LembreteDTO>(lembreteAtualizado);

        return Ok(lembreteAtualizado);
    }
    [HttpDelete("{id:int}")]
    public ActionResult<LembreteDTO> Delete(int id)
    {
        var lembrete = _uof.LembreteRepository.GetLembrete(id);

        if (lembrete == null)
        {
            return NotFound("Lembrete não encontrado...");
        }
        var lembreteExcluido = _uof.LembreteRepository.Delete(id);
        _uof.Commit();

        var lembreteExcluidoDto = _mapper.Map<LembreteDTO>(lembreteExcluido);

        return Ok(lembreteExcluidoDto);
    }
}
