namespace APICalendario.Repositories;
//encapsula o repositorio e permite um melhor controle como serao expostos e utilizados
public interface IUnitOfWork
{
    ILembreteRepository LembreteRepository { get; }
    //sera o metodo para chama savechanges();
    void Commit();
}
