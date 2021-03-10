using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using Octokit.Internal;
using System.Linq;
using SimpleGithubExplorer.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System;

namespace SimpleGithubExplorer.Controllers
{
    [Authorize]
    public class GithubController : Controller
    {
        private readonly ILogger<GithubController> _logger;
        public GithubController(ILogger<GithubController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Repositories()
        {
            List<RepositoryViewModel> result = new List<RepositoryViewModel>();
            IReadOnlyList<Repository> repoList = null;
            try
            {
                string accessToken = HttpContext.GetTokenAsync("access_token").Result;
                var githubClient = new GitHubClient(new ProductHeaderValue("OpsiGoGithubApp"),
                new InMemoryCredentialStore(new Credentials(accessToken)));
                repoList = await githubClient.Repository.GetAllForCurrent();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.ToString());
                repoList = null;
            }
            if(repoList!=null && repoList.Count > 0)
            {
                result.AddRange(repoList.Select(c => new RepositoryViewModel
                {
                    RepoId = c.Id,
                    RepoName = c.Name,
                    Url = c.HtmlUrl
                }));
            }
            return View(result);
        }

        public async Task<IActionResult> Issues(long id, string repoName)
        {
            var result = new List<IssueViewModel>();
            IReadOnlyList<Issue> issueList = null;

            try
            {
                ViewBag.RepoName = repoName;
                string accessToken = HttpContext.GetTokenAsync("access_token").Result;
                var githubClient = new GitHubClient(new ProductHeaderValue("OpsiGoGithubApp"),
                    new InMemoryCredentialStore(new Credentials(accessToken)));
                issueList = await githubClient.Issue.GetAllForRepository(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                issueList = null;
            }

            if(issueList!=null && issueList.Count > 0)
            {
                result.AddRange(issueList.Select(c => new IssueViewModel
                {
                    Url = c.HtmlUrl,
                    State = c.State.StringValue,
                    Title = c.Title
                }));    
            }
            return View(result);
        }

    }
}
