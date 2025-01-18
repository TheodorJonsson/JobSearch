using JobSearch.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.Controllers
{
    public class JobController : Controller
    {

        static IList<JobModel> jobList = new List<JobModel>
        {
            new JobModel { Id = 1, Company = "BAE Systems AB", Position = "Systemutvecklare med fokus på automatiserade tester", Location = "Örnsköldsvik", Date = DateOnly.FromDateTime(DateTime.Now) }
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
            JobModel job = new JobModel() { Id = 2, Company = "Advania", Position = "Advania Traineeprogram",Location = "Umeå", Date = DateOnly.FromDateTime(DateTime.Now)};

            return View(job);
        }

        [Route("/Job/AddJob")]
        [HttpPost]
        public IActionResult AddJob(JobModel job)
        {
            jobList.Add(job);

          
            return Redirect("/Job");
        }

        //[Route("/Job/EditJob")]
        [Route("/Job/EditJob/{id}")]
        [HttpGet]
        public IActionResult EditJob(int id)
        {
            var job = jobList.Where(j => j.Id == id).FirstOrDefault();

            return View(job);
        }

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
