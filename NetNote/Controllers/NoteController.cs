using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetNote.Repository;
using NetNote.Models;
using NetNote.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NetNote.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private INoteRepository _noteRepository;
        private INoteTypeRepository _noteTypeRepository;
        public NoteController(INoteRepository noteRepository, INoteTypeRepository noteTypeRepository)
        {
            _noteRepository = noteRepository;
            _noteTypeRepository = noteTypeRepository;
        }

        public IActionResult Index(int pageindex=1)
        {
            var pagesize = 10;
            var notes = _noteRepository.PageList(pageindex, pagesize);
            ViewBag.PageCount = notes.Item2;
            ViewBag.PageIndex = pageindex;
            return View(notes.Item1);
        }

        public async Task<IActionResult> Add()
        {
            var types =await  _noteTypeRepository.ListAsync();
            ViewBag.Types = types.Select(r=>new SelectListItem {
                Text=r.Name,
                Value=r.Id.ToString()
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromServices]IHostingEnvironment env, NoteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string filename = string.Empty;
            if (model.Attachment != null)
            {
                filename = Path.Combine("file", Guid.NewGuid().ToString() + Path.GetExtension(model.Attachment.FileName));
                using (var stream = new FileStream(Path.Combine(env.WebRootPath, filename), FileMode.CreateNew))
                {
                    model.Attachment.CopyTo(stream);
                }
            }
            await _noteRepository.AddAsync(new Note
            {
                Title = model.Title,
                Content = model.Content,
                Create = DateTime.Now,
                TypeId = model.Type,
                Password=model.Password,
                Attachment=filename
            });
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (!string.IsNullOrEmpty(note.Password))
                return View();
            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(int id,string password)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (!note.Password.Equals(password))
                return BadRequest("密码错误,返回重新输入");
            return View(note);
        }
    }
}
