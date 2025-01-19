using JobSearch.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.Controllers
{
    public class JobController : Controller
    {

        static IList<JobModel> jobList = new List<JobModel>
        {
            new JobModel { Id = 1, Company = "Umeå Universitet", Position = "Systemutvecklare", Location = "Umeå", Date = DateOnly.FromDateTime(DateTime.Now) }
        };
        [Route("/Job")]
        public IActionResult Index()
        {
            int nrOfJobs = jobList.Count();
            ViewBag.nrOfJobs = nrOfJobs;
            return View(jobList);
        }

        [Route("/Job/AddJob")]
        [HttpGet]
        public IActionResult AddJob()
        {
            JobModel job = new JobModel() { Id = 2, Company = "Luleå Tekniska Universitet", Position = "Databastekniker",Location = "Luleå", Date = DateOnly.FromDateTime(DateTime.Now)};

            return View(job);
        }

        [Route("/Job/AddJob")]
        [HttpPost]
        public IActionResult AddJob(JobModel job)
        {
            jobList.Add(job);

          
            return Redirect("/Job");
        }

        [Route("/Job/EditJob")]
        [Route("/Job/EditJob/{id}")]
        [HttpGet]
        public IActionResult EditJob(int id)
        {
            var job = jobList.Where(j => j.Id == id).FirstOrDefault();
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [Route("/Job/EditJob")]
        [Route("/Job/EditJob/{id}")]
        [HttpPost]
        public IActionResult EditJob(JobModel job)
        {
            var updatedJob = jobList.Where(j => j.Id == job.Id).FirstOrDefault();

            if(updatedJob != null)
            {
                jobList.Remove(updatedJob);
                jobList.Add(job);
            }
            return Redirect("/Job");
        }

    }
}
