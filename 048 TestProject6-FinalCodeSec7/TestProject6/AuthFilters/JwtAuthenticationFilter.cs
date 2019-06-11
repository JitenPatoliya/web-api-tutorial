using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace TestProject6.AuthFilters
{
    /// <summary>
    /// JWT Json web token authentication filter 
    /// </summary>
    /// <remarks>
    /// Especially for web services combined into a web app, you want to add this to your 
    /// WebApiConfig.cs Register method:
    /// <code>
    ///     config.SuppressHostPrincipal();
    /// </code>
    /// This ensures the IIS layer hasn't added some IPrincipal to the request based on the 
    /// application authentication, prior to your own web service authentication logic executing.
    /// There's no harm in always adding that line, even for standalone web services, 
    /// just to be sure.
    /// </remarks>
    public class JwtAuthenticationFilterAttribute : Attribute, IAuthenticationFilter
    {
        /// <summary>
        /// Set to the Authorization header Scheme value that this filter is intended to support
        /// </summary>
        public const string SupportedTokenScheme = "Bearer";

        // TODO: Decide if your filter should allow multiple instances per controller or
        //       per-method; set AllowMultiple to true if so
        public bool AllowMultiple { get { return false; } }

        // TODO: JWT tokens will need to validate the audience the token was meant for,
        //  the issuer of the token, and the signature.  Below static values will
        //  probably be read in from external configuration in a real world scenario.
        private readonly string _audience = "https://my.company.com";
        private readonly string _validIssuer = "http://my.tokenissuer.com";

        private readonly X509SecurityKey _signingCredentials;

        /// <summary>
        /// True if the filter supports WWW-Authenticate challenge headers,
        /// defaults to false.
        /// </summary>
        /// <remarks>
        /// Note that for scenarios not involving users from browsers using either the
        /// Basic or Digest authorization scheme, challenges are not inherently
        /// required.  A machine-to-machine call cannot likely process the challenge
        /// anyway. So the default value of this property is false, to not use
        /// challenges. In some scenarios using Basic tokens but not from a user
        /// and browser, you can disable them even for Basic/Digest tokens.
        /// </remarks> 
        public bool SendChallenge { get; set; }

        /// <summary>
        /// Default constructor creates signing credentials
        /// </summary>
        public JwtAuthenticationFilterAttribute()
        {
            // Create a signing token from the secret key
            // we'll load our certificate from a file for this example
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/CourseCert.cer");
            var certificate = new X509Certificate2(filePath);
            _signingCredentials = new X509SecurityKey(certificate);
        }

        /// <summary>
        /// Logic to authenticate the credentials. Must do one of:
        ///  -- exit out, doing nothing, if it cannot understand the token scheme presented,
        ///  -- set context.ErrorResult to an IHttpActionResult holding reason for invalid authentication.
        ///  -- set context.Principal to an IPrincipal if authenticated,
        /// </summary>
        public async Task AuthenticateAsync(HttpAuthenticationContext context,
            CancellationToken cancellationToken)
        {
            // STEP 1: extract your credentials from the request.  Generally this should be the 
            //         Authorization header, which the rest of this template assumes, but
            //         could come from any part of the request headers.
            var authHeader = context.Request.Headers.Authorization;
            // if there are no credentials, abort out
            if (authHeader == null)
                return;

            // STEP 2: if the token scheme isn't understood by this authenticator, abort out
            var tokenType = authHeader.Scheme;
            if (!tokenType.Equals(SupportedTokenScheme))
                return;

            // STEP 3: Given a valid token scheme, verify credentials are present
            var credentials = authHeader.Parameter;
            if (String.IsNullOrEmpty(credentials))
            {
                // no credentials sent with the scheme, abort out of the pipeline with an error result
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
                return;
            }

            // STEP 4: validate the credentials.  Return an error if invalid, else set the IPrincipal 
            //         on the context.
            try
            {
                IPrincipal principal = await ValidateCredentialsAsync(credentials, context.Request, cancellationToken);
                if (principal == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid security token", context.Request);
                }
                else
                {
                    // We have a valid, authenticated user; save off the IPrincipal instance
                    context.Principal = principal;
                }
            }
            catch (Exception stex) when (stex is SecurityTokenInvalidLifetimeException || 
                                             stex is SecurityTokenExpiredException ||
                                             stex is SecurityTokenNoExpirationException ||
                                             stex is SecurityTokenNotYetValidException) 
            {
                context.ErrorResult = new AuthenticationFailureResult("Security token lifetime error", context.Request);
            }
            catch (Exception)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid security token", context.Request);
            }
        }

        /// <summary>
        /// Extra logic associated with Basic/Digest authentication scheme, to 
        /// add the WWW-Authenticate header; for other token schemes, you can just do 
        /// nothing as shown below.
        /// </summary>
        /// <remarks>
        /// For Basic authentication, see the Microsoft sample.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/authentication-filters
        /// If you wanted to support WWW-Authenticate challenges for a non-basic token type,
        /// you can use the AddChallengeOnUnauthorizedResult line of code below.
        /// But note doing so is non necessarily required, in that WWW-Authenticate is 
        /// intended for users from browsers where the browser understands the token 
        /// scheme requested and can ask the user for credentials, it was not 
        /// meant for arbitrary custom tokens used by callers that are not browsers.
        /// </remarks>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, 
            CancellationToken cancellationToken)
        {
            // if this filter wants to support WWW-Authenticate header challenges, add one to the
            // result
            if (SendChallenge)
            {
                // FYI: Azure AD does support challenges for JWT tokens... the header looks like 
                // this, where authority looks like <instance>/<tenant> ex.
                //  https://login.microsoftonline.com/my.company.com
                //  and audience is the App ID URI of your Azure app. So if you needed that you
                //  would default SendChallenge to true in a constructor, and create your 
                // challenge header like this:
                // AuthenticationHeaderValue authenticateHeader = new 
                //      AuthenticationHeaderValue("Bearer", 
                //      "authorization_uri=\"" + authority + "\"" + "," + "resource_id=" + audience);

                context.Result = new AddChallengeOnUnauthorizedResult(
                    new AuthenticationHeaderValue(SupportedTokenScheme),
                    context.Result);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Internal method to validate the credentials included in the request,
        /// returning an IPrincipal for the resulting authenticated entity.
        /// </summary>
        private async Task<IPrincipal> ValidateCredentialsAsync(string credentials,
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            // verify this is a valid JWT token
            var isValidJwt = jwtHandler.CanReadToken(credentials);
            if (!isValidJwt)
                return null;

            // at this point you would want to validate the JWT internals -- 
            //   minimally signing key and lifetime, but probably issuer and 
            //   audience as well. Note some profiles of JWT require validating
            //   certain features (ex. OAuth).
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudiences = new[] { _audience },

                ValidateIssuer = true,
                ValidIssuers = new[] { _validIssuer },

                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = new[] { _signingCredentials },

                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5),  // limit the lifetime padding
                
                NameClaimType = ClaimTypes.NameIdentifier,
                AuthenticationType = SupportedTokenScheme
            };

            SecurityToken validatedToken = new JwtSecurityToken();
            ClaimsPrincipal principal = jwtHandler.ValidateToken(credentials, validationParameters, out validatedToken);

            // Add any other locally-generated claims you might want downstream code 
            //   to have access to.
            // In this example we set a few claim names we might re-use across a 
            //   number of token handlers
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("urn:Issuer",
                validatedToken.Issuer));
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("urn:TokenScheme",
                SupportedTokenScheme));

            // if you think any downstream code might want the original token string - 
            // perhaps because they need it to make downstream calls - 
            // store it in a standard claim name or the bootstrap context 
            // for later retrieval by the other filters/action methods
            ((ClaimsIdentity)principal.Identity).BootstrapContext = credentials;

            return await Task.FromResult(principal);
        }

    }
}