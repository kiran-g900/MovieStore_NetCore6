using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using System.Net.NetworkInformation;

namespace MovieStoreMvc.Repositories.Abstract
{
    public interface IMovieService
    {
        bool Add(Movie model);
        bool Update(Movie model);
        Movie GetById(int id);
        bool Delete(int id);
        //IQueryable<Movie> List();
        MovieListVm List(string term = "", bool paging = false, int currentPage = 0);
        List<int> GetGenreByMovieId(int movieId);
    }
}
