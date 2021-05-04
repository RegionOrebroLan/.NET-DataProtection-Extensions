using System;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.Forms;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Application.Controllers
{
	public class HomeController : Controller
	{
		#region Constructors

		public HomeController(IDataProtectionProvider dataProtectionProvider, IHostEnvironment hostEnvironment, IOptions<KeyManagementOptions> keyManagementOptions)
		{
			this.DataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
			this.KeyManagementOptions = keyManagementOptions;
		}

		#endregion

		#region Properties

		protected internal virtual IDataProtectionProvider DataProtectionProvider { get; }
		protected internal virtual IHostEnvironment HostEnvironment { get; }
		protected internal virtual IOptions<KeyManagementOptions> KeyManagementOptions { get; }

		#endregion

		#region Methods

		protected internal virtual async Task<Alert> CreateConfirmationAlertAsync(string message)
		{
			return await Task.FromResult(new Alert
			{
				Heading = "Confirmation",
				Information = message,
				Mode = AlertMode.Success
			});
		}

		protected internal virtual async Task<Alert> CreateExceptionAlertAsync(Exception exception)
		{
			if(exception == null)
				throw new ArgumentNullException(nameof(exception));

			return await Task.FromResult(new Alert
			{
				Heading = "Error",
				Information = exception.ToString(),
				Mode = AlertMode.Danger
			});
		}

		protected internal virtual async Task<Alert> CreateInvalidInputAlertAsync()
		{
			var alert = new Alert
			{
				Heading = "Error",
				Information = "Input-error",
				Mode = AlertMode.Danger
			};

			foreach(var (key, value) in this.ModelState)
			{
				foreach(var error in value.Errors)
				{
					alert.Details.Add($"{key}: {error.ErrorMessage}");
				}
			}

			return await Task.FromResult(alert);
		}

		protected internal virtual async Task<HomeViewModel> CreateModelAsync()
		{
			return await Task.FromResult(new HomeViewModel
			{
				DataProtectionProvider = this.DataProtectionProvider,
				DataProtector = this.DataProtectionProvider.CreateProtector("Test"),
				DefaultPurpose = this.HostEnvironment.ContentRootPath,
				KeyManagementOptions = this.KeyManagementOptions
			});
		}

		public virtual async Task<IActionResult> Index()
		{
			var model = await this.CreateModelAsync();

			return this.View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Protect(ProtectForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			var model = await this.CreateModelAsync();

			if(this.ModelState.IsValid)
			{
				try
				{
					var purpose = form.Purpose ?? this.HostEnvironment.ContentRootPath;
					var dataProtector = this.DataProtectionProvider.CreateProtector(purpose);
					var protectedValue = dataProtector.Protect(form.TextToProtect);

					model.Protections.Add(new Protection
					{
						ProtectedValue = protectedValue,
						Purpose = purpose,
						Text = form.TextToProtect
					});

					model.ProtectAlert = await this.CreateConfirmationAlertAsync($"The text \"{form.TextToProtect}\" was protected to \"{protectedValue}\".");
				}
				catch(Exception exception)
				{
					model.ProtectAlert = await this.CreateExceptionAlertAsync(exception);
				}
			}
			else
			{
				model.ProtectAlert = await this.CreateInvalidInputAlertAsync();
			}

			return this.View("Index", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Unprotect(UnprotectForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			var model = await this.CreateModelAsync();

			if(this.ModelState.IsValid)
			{
				try
				{
					var dataProtector = this.DataProtectionProvider.CreateProtector(form.Purpose ?? this.HostEnvironment.ContentRootPath);
					var unprotectedValue = dataProtector.Unprotect(form.TextToUnprotect);

					model.UnprotectAlert = await this.CreateConfirmationAlertAsync($"The text \"{form.TextToUnprotect}\" was unprotected to \"{unprotectedValue}\".");
				}
				catch(Exception exception)
				{
					model.UnprotectAlert = await this.CreateExceptionAlertAsync(exception);
				}
			}
			else
			{
				model.UnprotectAlert = await this.CreateInvalidInputAlertAsync();
			}

			return this.View("Index", model);
		}

		#endregion
	}
}