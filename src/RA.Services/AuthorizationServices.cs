using System;
using System.Collections;
using System.Collections.Generic;

using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utilities;

using RA.Models;
using MI = RA.Models.Input;
using MJ = RA.Models.JsonV2;
using MIPlace = RA.Models.Input.Place;
using MOPlace = RA.Models.JsonV2.Place;

using RA.Models.JsonV2;
using RA.Models.Input;

namespace RA.Services
{
	public class AuthorizationServices
	{
		#region === Security related Methods ===

		/// <summary>
		/// The actual validation will be via a call to the accounts api
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="statusMessage"></param>
		/// <returns></returns>
		public bool ValidateRequest( RequestHelper helper, ref string statusMessage, bool isDeleteRequest = false )
		{
			bool isValid = true;
			string clientIdentifier = "";
			List<string> messages = new List<string>();
			bool isTokenRequired = UtilityManager.GetAppKeyValue( "requiringHeaderToken", true );
			// -api key for manual publisher site-
			var apiPublisherIdentifier = UtilityManager.GetAppKeyValue( "apiPublisherIdentifier" );
			if ( isDeleteRequest )
				isTokenRequired = true;

			//api key will be passed in the header
			string apiToken = "";
			if ( IsAuthTokenValid( isTokenRequired, apiPublisherIdentifier, ref apiToken, ref clientIdentifier, ref statusMessage ) == false )
			{
				return false;
			}
			helper.ApiKey = apiToken;
			helper.ClientIdentifier = clientIdentifier ?? "";
			//owner ctid is always required
			if (!new ServiceHelperV2().IsCtidValid(helper.OwnerCtid, "Data Owner CTID",ref messages))
			{
				if ( messages.Count() == 1 && !string.IsNullOrWhiteSpace(messages[0]) )
					statusMessage = messages[ 0 ];
				else
					statusMessage = "Error - A valid CTID must be provided for the owning organization.";
				return false;
			}
			//check if originates from publisher (hmmm except sandbox testing?)
			if ( clientIdentifier == apiPublisherIdentifier )
			{
				helper.IsPublisherRequest = true;
			}

			if ( isTokenRequired &&
				( string.IsNullOrWhiteSpace( apiToken ) || apiToken.Length != 36 )
				)
			{
				if ( helper.IsPublisherRequest )
				{
					return true;
				}
				else
				{
					statusMessage = "Error - the provided security elements for this request have not be adequately provided.";
					return false;
				}
			}
			return isValid;
		}

		public static bool IsAuthTokenValid(bool isTokenRequired, ref string apiToken, ref string message)
		{
			string apiPublisherIdentifier = "";
			string clientIdentifier = "";
			return IsAuthTokenValid( isTokenRequired, apiPublisherIdentifier, ref apiToken, ref clientIdentifier, ref message );
		}

		public static bool IsAuthTokenValid( bool isTokenRequired, string apiPublisherIdentifier, ref string token, ref string clientIdentifier, ref string message )
		{
			bool isValid = true;
			//need to handle both ways. So if a token, and ctid are provided, then use them!
			try
			{
				HttpContext httpContext = HttpContext.Current;
				clientIdentifier = httpContext.Request.Headers[ "Proxy-Authorization" ];
				string authHeader = httpContext.Request.Headers[ "Authorization" ];
				//registry API uses ApiToken rather than Basic
				if ( !string.IsNullOrWhiteSpace( authHeader ) )
				{
					LoggingHelper.DoTrace( 6, "$$$$$$$$ Found an authorization header." + authHeader );
					if ( authHeader.ToLower().StartsWith( "apitoken " ) && authHeader.Length > 36 )
					{
						//Extract credentials
						authHeader = authHeader.ToLower();
						token = authHeader.Substring( "apitoken ".Length ).Trim();
					}
				}
			}
			catch ( Exception ex )
			{
				if ( isTokenRequired )
				{
					LoggingHelper.LogError( ex, "Exception encountered attempting to get API key from request header. " );
					isValid = false;
				}
			}

			if ( isTokenRequired && string.IsNullOrWhiteSpace( token ) )
			{
				if ( !string.IsNullOrWhiteSpace( clientIdentifier ) )
				{
					if ( clientIdentifier == apiPublisherIdentifier )
						return true;
				}
				message = "Error a valid API key must be provided in the header";
				isValid = false;
			}

			return isValid;
		}
		/// <summary>
		/// Encrypt the text using MD5 crypto service
		/// This is used for one way encryption of a user password - it can't be decrypted
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string Encrypt( string data )
		{
			byte[] byDataToHash = ( new UnicodeEncoding() ).GetBytes( data );
			byte[] bytHashValue = new MD5CryptoServiceProvider().ComputeHash( byDataToHash );
			return BitConverter.ToString( bytHashValue );
		}

		/// <summary>
		/// Encrypt the text using the provided password (key) and CBC CipherMode
		/// </summary>
		/// <param name="text"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string Encrypt_CBC( string text, string password )
		{
			RijndaelManaged rijndaelCipher = new RijndaelManaged();

			rijndaelCipher.Mode = CipherMode.CBC;
			rijndaelCipher.Padding = PaddingMode.PKCS7;
			rijndaelCipher.KeySize = 128;
			rijndaelCipher.BlockSize = 128;

			byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes( password );

			byte[] keyBytes = new byte[ 16 ];

			int len = pwdBytes.Length;

			if ( len > keyBytes.Length )
				len = keyBytes.Length;

			System.Array.Copy( pwdBytes, keyBytes, len );

			rijndaelCipher.Key = keyBytes;
			rijndaelCipher.IV = keyBytes;

			ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

			byte[] plainText = Encoding.UTF8.GetBytes( text );

			byte[] cipherBytes = transform.TransformFinalBlock( plainText, 0, plainText.Length );

			return Convert.ToBase64String( cipherBytes );

		}

		/// <summary>
		/// Decrypt the text using the provided password (key) and CBC CipherMode
		/// </summary>
		/// <param name="text"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string Decrypt_CBC( string text, string password )
		{
			RijndaelManaged rijndaelCipher = new RijndaelManaged();

			rijndaelCipher.Mode = CipherMode.CBC;
			rijndaelCipher.Padding = PaddingMode.PKCS7;
			rijndaelCipher.KeySize = 128;
			rijndaelCipher.BlockSize = 128;

			byte[] encryptedData = Convert.FromBase64String( text );

			byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes( password );

			byte[] keyBytes = new byte[ 16 ];

			int len = pwdBytes.Length;

			if ( len > keyBytes.Length )
				len = keyBytes.Length;

			System.Array.Copy( pwdBytes, keyBytes, len );

			rijndaelCipher.Key = keyBytes;
			rijndaelCipher.IV = keyBytes;

			ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

			byte[] plainText = transform.TransformFinalBlock( encryptedData, 0, encryptedData.Length );

			return Encoding.UTF8.GetString( plainText );

		}

		#endregion
	}
}
