using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.Extensions.Logging;

namespace ChainConnext.Client.Shared
{
    public partial class LoginComponent : RadzenComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether automatic complete of inputs is enabled.
        /// </summary>
        /// <value><c>true</c> if automatic complete of inputs is enabled; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AutoComplete { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating the type of built-in autocomplete
        /// the browser should use.
        /// <see cref="Blazor.AutoCompleteType" />
        /// </summary>
        /// <value>
        /// The type of built-in autocomplete.
        /// </value>
        [Parameter]
        public AutoCompleteType UserNameAutoCompleteType { get; set; } = AutoCompleteType.Username;

        /// <summary>
        /// Gets or sets a value indicating the type of built-in autocomplete
        /// the browser should use.
        /// <see cref="Blazor.AutoCompleteType" />
        /// </summary>
        /// <value>
        /// The type of built-in autocomplete.
        /// </value>
        [Parameter]
        public AutoCompleteType PasswordAutoCompleteType { get; set; } = AutoCompleteType.CurrentPassword;

        /// <summary>
        /// Gets or sets the design variant of the form field.
        /// </summary>
        /// <value>The variant of the form field.</value>
        [Parameter]
        public Variant? FormFieldVariant { get; set; }

        /// <inheritdoc />
        protected override string GetComponentCssClass()
        {
            return "rz-login";
        }

        string username;
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [Parameter]
        public string Username { get; set; }

        string password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Parameter]
        public string Password { get; set; }

        private bool rememberMe;
        /// <summary> Sets the initial value of the remember me switch.</summary>
        [Parameter]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the login callback.
        /// </summary>
        /// <value>The login callback.</value>
        [Parameter]
        public EventCallback<LoginArgs> Login { get; set; }

        /// <summary>
        /// Gets or sets the register callback.
        /// </summary>
        /// <value>The register callback.</value>
        [Parameter]
        public EventCallback Register { get; set; }

        /// <summary>
        /// Gets or sets the reset password callback.
        /// </summary>
        /// <value>The reset password callback.</value>
        [Parameter]
        public EventCallback<string> ResetPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether register is allowed.
        /// </summary>
        /// <value><c>true</c> if register is allowed; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AllowRegister { get; set; } = true;

        /// <summary>
        /// Asks the user whether to remember their credentials. Set to <c>false</c> by default.
        /// </summary>
        [Parameter]
        public bool AllowRememberMe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reset password is allowed.
        /// </summary>
        /// <value><c>true</c> if reset password is allowed; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AllowResetPassword { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether default login button is shown.
        /// </summary>
        /// <value><c>true</c> if default login button is shown; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool ShowLoginButton { get; set; } = true;

        /// <summary>
        /// Gets or sets the login text.
        /// </summary>
        /// <value>The login text.</value>
        [Parameter]
        public string LoginText { get; set; } = "Login";

        /// <summary>
        /// Gets or sets the register text.
        /// </summary>
        /// <value>The register text.</value>
        [Parameter]
        public string RegisterText { get; set; } = "Sign up";

        /// <summary> Gets or sets the remember me text.</summary>
        [Parameter]
        public string RememberMeText { get; set; } = "Remember me";

        /// <summary>
        /// Gets or sets the register message text.
        /// </summary>
        /// <value>The register message text.</value>
        [Parameter]
        public string RegisterMessageText { get; set; } = "Don't have an account yet?";

        /// <summary>
        /// Gets or sets the reset password text.
        /// </summary>
        /// <value>The reset password text.</value>
        [Parameter]
        public string ResetPasswordText { get; set; } = "Forgot password?";

        /// <summary>
        /// Gets or sets the user text.
        /// </summary>
        /// <value>The user text.</value>
        [Parameter]
        public string UserText { get; set; } = "Username";

        /// <summary>
        /// Gets or sets the user required text.
        /// </summary>
        /// <value>The user required text.</value>
        [Parameter]
        public string UserRequired { get; set; } = "Username is required";

        /// <summary>
        /// Gets or sets the password text.
        /// </summary>
        /// <value>The password text.</value>
        [Parameter]
        public string PasswordText { get; set; } = "Password";

        /// <summary>
        /// Gets or sets the password required.
        /// </summary>
        /// <value>The password required.</value>
        [Parameter]
        public string PasswordRequired { get; set; } = "Password is required";

        [Parameter]
        public bool IsAsync { get; set; } = false;

        /// <summary>
        /// Called when login.
        /// </summary>
        protected async Task OnLogin()
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                IsAsync = true;
                StateHasChanged();
                Logger.LogInformation($"Begin Login : {IsAsync}");

                await Login.InvokeAsync(new LoginArgs { Username = username, Password = password, RememberMe = rememberMe });
                //await ShowBusyDialog(new LoginArgs { Username = username, Password = password, RememberMe = rememberMe });

                Logger.LogInformation("End Login");
                IsAsync = false;
            }
        }

        async Task ShowBusyDialog(LoginArgs args)
        {
            InvokeAsync(async () =>
            {
                // Simulate background task

                await Login.InvokeAsync(args);

                // Close the dialog
                dialogService.Close();
            });

            await BusyDialog("Login ...");
        }

        // Busy dialog from string
        async Task BusyDialog(string message)
        {
            await dialogService.OpenAsync("", ds =>
            {
                RenderFragment content = b =>
                {
                    b.OpenElement(0, "RadzenRow");

                    b.OpenElement(1, "RadzenColumn");
                    b.AddAttribute(2, "Size", "12");

                    b.AddContent(3, message);

                    b.CloseElement();
                    b.CloseElement();
                };
                return content;
            }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
        }

        private string Id(string name)
        {
            return $"{GetId()}-{name}";
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            username = Username;
            password = Password;
            rememberMe = RememberMe;
            base.OnInitialized();
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.DidParameterChange(nameof(Username), Username))
            {
                username = parameters.GetValueOrDefault<string>(nameof(Username));
            }

            if (parameters.DidParameterChange(nameof(Password), Password))
            {
                password = parameters.GetValueOrDefault<string>(nameof(Password));
            }

            if (parameters.DidParameterChange(nameof(RememberMe), RememberMe))
            {
                rememberMe = parameters.GetValueOrDefault<bool>(nameof(RememberMe));
            }

            await base.SetParametersAsync(parameters);
        }

        /// <summary>
        /// Handles the <see cref="E:Reset" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected async Task OnReset(EventArgs args)
        {
            await ResetPassword.InvokeAsync(username);
        }

        /// <summary>
        /// Called when register.
        /// </summary>
        protected async Task OnRegister()
        {
            await Register.InvokeAsync(EventArgs.Empty);
        }
    }
}
