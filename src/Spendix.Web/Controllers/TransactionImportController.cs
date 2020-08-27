using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Repos;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Web.ViewModels.TransactionImport;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spendix.Core.Constants;
using Microsoft.AspNetCore.Http;
using Spendix.Web.ResponseModels.TransactionImport;
using System.IO;
using System.Text.RegularExpressions;

namespace Spendix.Web.Controllers
{
    [Route("Transactions")]
    [Authorize]
    public class TransactionImportController : BaseController
    {
        private readonly BankAccountRepo bankAccountRepo;

        public TransactionImportController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
        }

        [HttpGet, Route("Import")]
        public async Task<IActionResult> Import()
        {
            var bankAccounts = (await bankAccountRepo.FindByLoggedInUserAccountAsync())
                                .OrderBy(x => x.Name)
                                .ToList();

            var supportedBankImportSources = BankImportSources.AllBankImportSources;

            var vm = new ImportViewModel
            {
                BankAccountSelectList = new SelectList(bankAccounts, "BankAccountId", "Name"),
                BankImportSourceSelectList = new SelectList(supportedBankImportSources)
            };

            return View(vm);
        }

        [HttpPost, Route("ProcessImport")]
        public async Task<IActionResult> ProcessImport(IFormCollection values)
        {
            var bankAccountId = Guid.Parse(values["BankAccountId"]);
            var bankImportSource = values["BankImportSource"];
            var file = values.Files.First();

            List<ProcessImportResponseModel> transactions = null;

            if (string.Equals(bankImportSource, BankImportSources.AllyBank))
            {
                transactions = await BuildResponseFromAllyBankImport(file);
            }

            return Ok(new
            {
                success = true,
                transactions
            });
        }

        private async Task<List<ProcessImportResponseModel>> BuildResponseFromAllyBankImport(IFormFile file)
        {
            var models = new List<ProcessImportResponseModel>();
            var index = 0;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = await reader.ReadLineAsync();

                    if (index == 0)
                    {
                        //Skip header row
                        index++;
                        continue;
                    }

                    var parser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    var items = parser.Split(line);

                    //var type = items[3];

                    models.Add(new ProcessImportResponseModel
                    {
                        Date = DateTime.Parse(items[0]).ToShortDateString(),
                        Payee = items[4].Replace("\"", ""),
                        Amount = items[2]
                    });

                    index++;
                }
            }

            return models;
        }
    }
}