using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext ctx;
        public MovieService(DatabaseContext ctx)
        {

            this.ctx = ctx;

        }
        public bool Add(Movie model)
        {
            try
            {                
                ctx.Movie.Add(model);
                ctx.SaveChanges();
                foreach (var genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    ctx.MovieGenre.Add(movieGenre);
                }
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data != null)
                {
                    var mg = ctx.MovieGenre.Where(a => a.MovieId == id);
                    foreach(var item in mg)
                    {
                        ctx.MovieGenre.Remove(item);
                    }
                    ctx.Movie.Remove(data);
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Movie GetById(int id)
        {
            return ctx.Movie.Find(id);
        }

        //public IQueryable<Movie> List()
        //{
        //    var data = ctx.Movie.AsQueryable();
        //    return data;
        //}

        public bool Update(Movie model)
        {
            try
            {
                var genreToDeleted = ctx.MovieGenre.Where(a => a.MovieId == model.Id && !model.Genres.Contains(a.GenreId)).ToList();
                foreach(var mgenre in genreToDeleted)
                {
                    ctx.MovieGenre.Remove(mgenre);
                }

                foreach (int genID in model.Genres)
                {
                    var movieGenre = ctx.MovieGenre.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genID);
                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genID, MovieId = model.Id };
                        ctx.MovieGenre.Add(movieGenre);
                    }
                }
                ctx.Movie.Update(model);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }  

        public List<int> GetGenreByMovieId(int movieID)
        {
            var genreIDs = ctx.MovieGenre.Where(a => a.MovieId == movieID).Select(a => a.GenreId).ToList();
            return genreIDs;
        }
        
        public MovieListVm List(string term="", bool paging= false, int currentPage = 0)
        {
            var data = new MovieListVm();

            var list = ctx.Movie.ToList();                   
            
            if(!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                list = list.Where(a => a.Title.ToLower().StartsWith(term)).ToList();
            }

            if (paging)
            {
                int pageSize = 5;
                int count = list.Count;
                int totalPages = (int)Math.Ceiling(count/(double)pageSize);
                list = list.Skip((currentPage-1)*pageSize).Take(pageSize).ToList();
                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPages = totalPages;
            }

            foreach (var movie in list)
            {
                var genres = (from genre in ctx.Genre join mg in ctx.MovieGenre
                              on genre.Id equals mg.GenreId
                              where mg.MovieId == movie.Id
                              select genre.GenreName
                              ).ToList();
                var genrenames = string.Join(",", genres);
                movie.GenreNames = genrenames;
            }
           data.MovieList = list.AsQueryable();
            return data;
        }
    }
}
