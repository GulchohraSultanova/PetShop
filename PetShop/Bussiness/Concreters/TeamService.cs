using Bussiness.Abstracts;
using Bussiness.Exceptions;
using Core.Models;
using Core.RepositoryAbstracts;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Concreters
{
    public class TeamService : ITeamService
    {
        ITeamRepository _repository;
        IWebHostEnvironment _webHostEnvironment;

        public TeamService(ITeamRepository repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Create(Team team)
        {
            if(team == null)
            {
                throw new NotFoundException("Team", "Null ola bimez!");
            }
            if(team.PhotoFile==null)
            {
                throw new NotNullException("PhotoFile", "Null ola bimez!");
            }
            if (!team.PhotoFile.ContentType.Contains("image/"))
            {
                throw new FileContentTypeException("PhotoFile", "Fayl tipi dogru deyil!");
            }
            if (team.PhotoFile.Length > 2097152)
            {
                throw new FileSizeException("Photofile", "File olcusu boyukdur!");
            }
            string filename=team.PhotoFile.FileName;
            string path = _webHostEnvironment.WebRootPath + @"\Upload\Team\" + filename;
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                team.PhotoFile.CopyTo(file);
            }
            team.ImgUrl = filename;
            _repository.Add(team);
            _repository.Commit();
        }

        public void Delete(int  id )
        {
           var oldTeam= _repository.Get(x=>x.Id==id);
            if (oldTeam == null)
            {
                throw new NotFoundException("Team", "Null ola bimez!");
            }
            string path = _webHostEnvironment.WebRootPath + @"\Upload\Team\" + oldTeam.ImgUrl;
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            _repository.Delete(oldTeam);
            _repository.Commit();

        }

        public List<Team> GetAllTeams(Func<Team, bool>? func = null)
        {
            return _repository.GetAll(func);
        }

        public Team GetTeam(Func<Team, bool>? func = null)
        {
            return _repository.Get(func);

        }

        public void Update(int id, Team team)
        {
            var updateTeam=_repository.Get(x=>x.Id==id);
            if (updateTeam == null)
            {
                throw new NotFoundException("Team", "Null ola bimez!");
            }
            if(team.PhotoFile != null)
            {
                if (!team.PhotoFile.ContentType.Contains("image/"))
                {
                    throw new FileContentTypeException("PhotoFile", "Fayl tipi dogru deyil!");
                }
                if (team.PhotoFile.Length > 2097152)
                {
                    throw new FileSizeException("Photofile", "File olcusu boyukdur!");
                }
                string filename = team.PhotoFile.FileName;
                string path = _webHostEnvironment.WebRootPath + @"\Upload\Team\" + filename;
                using (FileStream file = new FileStream(path, FileMode.Create))
                {
                    team.PhotoFile.CopyTo(file);
                }
                updateTeam.ImgUrl = filename;
            }
            else
            {
                team.ImgUrl=updateTeam.ImgUrl;
            }
            updateTeam.Name = team.Name;
            updateTeam.Position = team.Position;
            _repository.Commit();

        }
    }
}
