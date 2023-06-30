using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    //[Authorize]
    public class MovieController : Controller
    {
        private readonly IMovieService _MovieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        public MovieController(IMovieService MovieService, IFileService fileService, IGenreService genreService)
        {
            _MovieService = MovieService;
            _fileService = fileService;
            _genreService = genreService;
        }
        public IActionResult Add()  
        {
            var model = new Movie();
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text=a.GenreName, Value=a.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Movie model)
        {
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
            if (!ModelState.IsValid)
                return View(model);
            if (model.ImageFile != null)
            {
                var fileReult = this._fileService.SaveImage(model.ImageFile);
                if (fileReult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileReult.Item2;
                model.MovieImage = imageName;
            }
            var result = _MovieService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var model = _MovieService.GetById(id);
            var selectGenres = _MovieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectGenres);
            model.MultiGenreList = multiGenreList;
            return View(model);  
        }

        [HttpPost]
        public IActionResult Edit(Movie model) 
        {
            var selectGenres = _MovieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectGenres);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ImageFile != null)
            {
                var fileReult = this._fileService.SaveImage(model.ImageFile);
                if (fileReult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileReult.Item2;
                model.MovieImage = imageName;
            }
            var result = _MovieService.Update(model);
            if (result)
            {
                TempData["msg"] = "Successfully Updated";
                return RedirectToAction(nameof(MovieList));
            }
            else
            {
                TempData["msg"] = "Error while updating data";
                return View(model);
            }
        }

        public IActionResult MovieList()
        {
            var data = this._MovieService.List();
            return View(data);  
        }

        public IActionResult Delete(int id)
        {            
            var result = _MovieService.Delete(id);            
                return RedirectToAction(nameof(MovieList));            
        }
    }
}
