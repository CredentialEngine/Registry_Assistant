﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utilities
{
    public class UtilityManager
    {
        const string thisClassName = "UtilityManager";


        /// <summary>
        /// Default constructor for UtilityManager
        /// </summary>
        public UtilityManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        #region === Resource Manager Methods ===
        /// <summary>
        /// Handle converting the format of the resource string (to not use periods, etc,)
        /// </summary>
        /// <param name="rm">ResourceManager</param>
        /// <param name="resKey"></param>
        /// <returns></returns>
        public static string ChangeResourceString( ResourceManager rm, string resKey )
        {

            string resultString = "";
            try
            {
                resultString = Regex.Replace( resKey, "\\.", "_" );
                resultString = Regex.Replace( resultString, "\\*", "_" );
            }
            catch ( ArgumentException ex )
            {
                // Syntax error in the regular expression
                resultString = resKey;
            }

            return rm.GetString( resultString );
        }

        /// <summary>
        /// Retrieves a string from the resource file
        /// </summary>
        /// <param name="rm">ResourceManager</param>
        /// <param name="resKey">Resourse key</param>
        /// <returns>Related resource string</returns>
        public static string GetResourceValue( ResourceManager rm, string resKey )
        {

            string keyName = "";
            try
            {
                keyName = Regex.Replace( resKey, "\\.", "_" );
                keyName = Regex.Replace( keyName, "\\*", "_" );
            }
            catch ( ArgumentException ex )
            {
                // Syntax error in the regular expression
                keyName = resKey;
            }

            return rm.GetString( keyName );
        }


        /// <summary>
        /// Gets the value of a resource string from applicable resource file. Returns blanks if not found
        /// </summary>
        /// <param name="rm">ResourseManager</param>
        /// <param name="resKey">Key name in resource file</param>
        /// <param name="defaultValue">Value to use if resource is not found</param>
        /// <returns>String from resource file or default value</returns>
        /// <remarks>This property is explicitly thread safe.</remarks>
        public static string GetResourceValue( ResourceManager rm, string resKey, string defaultValue )
        {
            string resource = "";
            string keyName = "";

            try
            {
                keyName = Regex.Replace( resKey, "\\.", "_" );
                keyName = Regex.Replace( keyName, "\\*", "_" );

                resource = rm.GetString( keyName );
                if ( resource.Length < 1 )
                    resource = defaultValue;
            }
            catch
            {
                resource = defaultValue;
            }

            return resource;
        } //

        #endregion

        #region === Application Keys Methods ===

        /// <summary>
        /// Gets the value of an application key from web.config. Returns blanks if not found
        /// </summary>
        /// <remarks>This property is explicitly thread safe.</remarks>
        public static string GetAppKeyValue( string keyName )
        {

            return GetAppKeyValue( keyName, "" );
        } //

        /// <summary>
        /// Gets the value of an application key from web.config. Returns the default value if not found
        /// </summary>
        /// <remarks>This property is explicitly thread safe.</remarks>
        public static string GetAppKeyValue( string keyName, string defaultValue )
        {
            string appValue = "";

            try
            {
                appValue = System.Configuration.ConfigurationManager.AppSettings[ keyName ];
                if ( appValue == null )
                    appValue = defaultValue;
            }
            catch
            {
                appValue = defaultValue;
				if ( HasMessageBeenPreviouslySent( keyName ) == false )
					LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
        public static int GetAppKeyValue( string keyName, int defaultValue )
        {
            int appValue = -1;

            try
            {
                appValue = Int32.Parse( System.Configuration.ConfigurationManager.AppSettings[ keyName ] );

                // If we get here, then number is an integer, otherwise we will use the default
            }
            catch
            {
                appValue = defaultValue;
				if ( HasMessageBeenPreviouslySent( keyName ) == false )
					LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
        public static bool GetAppKeyValue( string keyName, bool defaultValue )
        {
            bool appValue = false;

            try
            {
                appValue = bool.Parse( System.Configuration.ConfigurationManager.AppSettings[ keyName ] );
            }
            catch (Exception ex)
            {
                appValue = defaultValue;
				if ( HasMessageBeenPreviouslySent( keyName ) == false )
					 LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //

		public static bool HasMessageBeenPreviouslySent( string keyName )
		{

			string key = "appkey_" + keyName;
			//check cache for keyName
			if ( HttpRuntime.Cache[ key ] != null )
			{
				return true;
			}
			else
			{
				//not really much to store
				HttpRuntime.Cache.Insert( key, keyName );
			}

			return false;
		}
		#endregion

		#region === Security related Methods ===

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

            if ( len > keyBytes.Length ) len = keyBytes.Length;

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

            if ( len > keyBytes.Length ) len = keyBytes.Length;

            System.Array.Copy( pwdBytes, keyBytes, len );

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock( encryptedData, 0, encryptedData.Length );

            return Encoding.UTF8.GetString( plainText );

        }

        /// <summary>
        /// Encode a passed URL while first checking if already encoded
        /// </summary>
        /// <param name="url">A web Address</param>
        /// <returns>Encoded URL</returns>
        public static string EncodeUrl( string url )
        {
            string encodedUrl = "";

            if ( url.Length > 0 )
            {
                //check if already encoded

                if ( url.ToLower().IndexOf( "%3a" ) > -1
                    //|| url.ToLower().IndexOf( "&amp;" ) > -1
                )
                {
                    encodedUrl = url;
                }
                else
                {
                    encodedUrl = HttpUtility.UrlEncode( url );
                    //fix potential encode errors:
                    encodedUrl = encodedUrl.Replace( "%26amp%3b", "%26" );
                }
            }

            return encodedUrl;
        }

		#endregion

		#region === Path related Methods ===
		/// <summary>
		/// FormatAbsoluteUrl an absolute URL - equivalent to Url.Content()
		/// </summary>
		/// <param name="path"></param>
		/// <param name="uriScheme"></param>
		/// <returns></returns>
		public static string FormatAbsoluteUrl( string path, string uriScheme = null )
		{
			uriScheme = uriScheme ?? HttpContext.Current.Request.Url.Scheme; //allow overriding http or https

			var environment = System.Configuration.ConfigurationManager.AppSettings[ "envType" ]; //Use port number only on localhost because https redirecting to a port on production screws this up
			var host = environment == "dev" ? HttpContext.Current.Request.Url.Authority : HttpContext.Current.Request.Url.Host;

			return uriScheme + "://" +
				( host + "/" + HttpContext.Current.Request.ApplicationPath + path.Replace( "~/", "/" ) )
				.Replace( "///", "/" )
				.Replace( "//", "/" );
		}
		//
		/// <summary>
		/// Format a relative, internal URL as a full URL, with http or https depending on the environment. 
		/// Determines the current host and then calls overloaded method to complete the formatting
		/// </summary>
		/// <param name="relativeUrl">Internal URL, usually beginning with /vos_portal/</param>
		/// <param name="isSecure">If the URL is to be formatted as a secure URL, set this value to true.</param>
		/// <returns>Formatted URL</returns>
		public static string FormatAbsoluteUrl( string relativeUrl, bool isSecure )
        {
            string host = "";
            try
            {
                //14-10-10 mp - change to explicit value from web.config
                host = GetAppKeyValue( "siteHostName" );
                if ( host == "" )
                {
                    // doing it this way so as to not break anything - HttpContext doesn't exist in a WCF web service
                    // so if this doesn't work we go get the FQDN another way.
                    host = HttpContext.Current.Request.ServerVariables[ "HTTP_HOST" ];
                    //need to handle ports!!
                }
            }
            catch ( Exception ex )
            {
                host = Dns.GetHostEntry( "localhost" ).HostName;
                // Fix up name with www instead of webX
                Regex hostEx = new Regex( @"web.?" );
                Match match = hostEx.Match( host );
                if ( match.Index > -1 )
                {
                    if (match.Value.Length > 0)
                        host = host.Replace( match.Value, "www" );
                }
            }

            return FormatAbsoluteUrl( relativeUrl, host, isSecure );
        }

        /// <summary>
        /// Format a relative, internal URL as a full URL, with http or https depending on the environment.
        /// </summary>
        /// <param name="relativeUrl">Internal URL, usually beginning with /vos_portal/</param>
        /// <param name="host">name of host (e.g. localhost, edit.illinoisworknet.com, www.illinoisworknet.com)</param>
        /// <param name="isSecure">If the URL is to be formatted as a secure URL, set this value to true.</param>
        /// <returns>Formatted URL</returns>
        public static string FormatAbsoluteUrl( string relativeUrl, string host, bool isSecure )
        {
            string url = "";
            if ( string.IsNullOrEmpty( relativeUrl ) )
                return "";
            if ( string.IsNullOrEmpty( host ) )
                return "";
            //ensure not already an absolute
            if ( relativeUrl.ToLower().StartsWith( "http" ) )
                return relativeUrl;
			//
			if ( isSecure && GetAppKeyValue( "usingSSL", false ))
            {
                url = "https://" + host + relativeUrl;
            }
            else
            {
                url = "http://" + host + relativeUrl;
            }
            return url;
        }
        /// <summary>
        /// get current url, including query parameters
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUrl()
        {
            string url = GetPublicUrl( HttpContext.Current.Request.QueryString.ToString() );

            url = HttpUtility.UrlDecode( url );

            return url;
        }//

        /// <summary>
        /// Return the public version of the current MCMS url - removes MCMS specific parameters
        /// </summary>
        public static string GetPublicUrl( string url )
        {
            string publicUrl = "";

            //find common parms
            int nrmodePos = url.ToLower().IndexOf( "nrmode" );
            int urlStartPos = url.ToLower().IndexOf( "nroriginalurl" );
            int urlEndPos = url.ToLower().IndexOf( "&nrcachehint" );

            if ( urlStartPos > 0 && urlEndPos > urlStartPos )
            {
                publicUrl = url.Substring( urlStartPos + 14, urlEndPos - ( urlStartPos + 14 ) );
            }
            else
            {
                //just take everything??
                publicUrl = url;
            }
            publicUrl = publicUrl.Replace( "%2f", "/" );
            publicUrl = publicUrl.Replace( "%2e", "." );
            publicUrl = publicUrl.Replace( "%3a", ":" );
            return publicUrl;
        } //

        /// <summary>
        /// Extract the domain from the passed url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>domain main or blank</returns>
        public static string GetUrlDomain( string url )
        {
            string domain = "";
            if ( url == null || url.Length == 0 || url.ToLower().StartsWith( "nrmode" ) )
                return domain;

            try
            {
                int opt = 1;

                if ( opt == 1 )
                {
                    //option 1
                    if ( !url.Contains( "://" ) )
                        url = "http://" + url;

                    domain = new Uri( url ).Host;

                }
                else if ( opt == 2 )
                {
                    if ( url.Contains( @"://" ) )
                        url = url.Split( new string[] { "://" }, 2, StringSplitOptions.None )[ 1 ];

                    domain = url.Split( '/' )[ 0 ];
                }
                else if ( opt == 3 )
                {
                    domain = System.Text.RegularExpressions.Regex.Replace(
                                        url,
                                        @"^([a-zA-Z]+:\/\/)?([^\/]+)\/.*?$",
                                        "$2" );

                }
            }
            catch ( Exception ex )
            {
                domain = "";
            }
            return domain;

        }//

        /// <summary>
        /// Gets the last section (subchannel) of passed url
        /// Note downside is we are working with physical url which may not be meaningfull (esp for business main channels)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlLastSection( string url )
        {
            //
            string section = "";
            string[] dirArray = url.Split( '/' );

            for ( int i = dirArray.Length ; i > 0 ; i-- )
            {
                if ( dirArray[ i - 1 ].Trim().Length > 0 && dirArray[ i - 1 ].IndexOf( ".htm" ) == -1 )
                {
                    section = dirArray[ i - 1 ].Trim();
                    break;
                }


            }

            return section;
        }//



        /// <summary>
        /// Insert soft breaks into long URLs or email addresses in text string
        /// </summary>
        /// <param name="anchorText">Text to insert soft breaks into</param>
        /// <returns></returns>
        public static string InsertSoftbreaks( string anchorText )
        {
            string skippingSoftBreaks = GetAppKeyValue( "skippingSoftBreaks", "no" );

            StringBuilder newText = new StringBuilder( 255 );
            const string softbreak = "<span class=\"softbreak\"> </span>";
            // First make sure that the text doesn't already contain softbreak
            int sbPos = anchorText.ToLower().IndexOf( "softbreak" );
            if ( sbPos >= 0 )
                return anchorText;

            if ( skippingSoftBreaks.Equals( "yes" ) )
                return anchorText;

            //skip if an img exists
            int imgPos = anchorText.ToLower().IndexOf( "<img" );
            if ( imgPos >= 0 )
                return anchorText;
            imgPos = anchorText.ToLower().IndexOf( "<asp:img" );
            if ( imgPos >= 0 )
                return anchorText;

            //check for large anchor text - could be indicative of missing/misplaced ending tags, 
            //which could cause a problem
            if ( anchorText.Length > 200 )
                return anchorText;

            // We're going to look for http, img, /, and @
            //MP - should also try to handle https!
            int httpPos = anchorText.IndexOf( "http://" );
            int atPos = anchorText.IndexOf( "@" );
            int slashPos = anchorText.IndexOf( "/" );

            if ( ( httpPos >= 0 ) && ( atPos == -1 ) )
            {
                // We have http but not @
                if ( ( httpPos >= imgPos ) && ( imgPos >= 0 ) )
                {
                    // The http may be inside an img tag, do nothing
                    return anchorText;
                }
            }
            if ( ( httpPos == -1 ) && ( atPos >= 0 ) )
            {
                // We have @ but not http
                if ( atPos >= imgPos )
                {
                    // the @ may be inside an img tag, do nothing
                    return anchorText;
                }
            }

            if ( ( httpPos >= 0 ) && ( atPos >= 0 ) )
            {
                //We have both @ and http
                if ( httpPos < atPos )
                {
                    if ( imgPos < httpPos )
                    {
                        return anchorText;
                    }
                }
                if ( atPos < httpPos )
                {
                    if ( imgPos < atPos )
                    {
                        return anchorText;
                    }
                }
            }

            if ( ( httpPos == -1 ) && ( atPos == -1 ) )
            {
                // we have neither @ nor http
                return anchorText;
            }

            // First we look to see if we have an http link, and handle it.
            if ( httpPos >= 0 )
            {
                string imgTagToEnd = "";
                string priorToImgTag = "";
                if ( imgPos >= 0 )
                {
                    priorToImgTag = anchorText.Substring( 0, imgPos );
                    imgTagToEnd = anchorText.Substring( imgPos, anchorText.Length - imgPos );
                }
                else
                {
                    priorToImgTag = anchorText;
                }
                httpPos += 7;  // 7 = length of string "http://"
                newText.Append( priorToImgTag.Substring( 0, httpPos ) );
                newText.Append( softbreak );
                priorToImgTag = priorToImgTag.Substring( httpPos, priorToImgTag.Length - ( httpPos ) );
                slashPos = priorToImgTag.IndexOf( "/" );
                while ( slashPos > -1 )
                {
                    slashPos++;
                    newText.Append( priorToImgTag.Substring( 0, slashPos ) );
                    newText.Append( softbreak );
                    priorToImgTag = priorToImgTag.Substring( slashPos, priorToImgTag.Length - slashPos );
                    slashPos = priorToImgTag.IndexOf( "/" );
                }
                if ( newText.ToString() == "http://" + softbreak )
                {
                    newText.Append( priorToImgTag );
                }
                priorToImgTag = newText.ToString();
                newText.Remove( 0, newText.ToString().Length );
                int dotPos = priorToImgTag.IndexOf( "." );
                while ( dotPos > -1 )
                {
                    dotPos++;
                    newText.Append( priorToImgTag.Substring( 0, dotPos ) );
                    newText.Append( softbreak );
                    priorToImgTag = priorToImgTag.Substring( dotPos, priorToImgTag.Length - dotPos );
                    dotPos = priorToImgTag.IndexOf( "." );
                }
                newText.Append( priorToImgTag );
                newText.Append( imgTagToEnd );
            }
            else
            {
                // Now we want to know if we're looking at an email address
                if ( atPos >= 0 )
                {
                    string imgTagToEnd = "";
                    string priorToImgTag = "";
                    if ( imgPos >= 0 )
                    {
                        priorToImgTag = anchorText.Substring( 0, imgPos );
                        imgTagToEnd = anchorText.Substring( imgPos, anchorText.Length - imgPos );
                    }
                    else
                    {
                        priorToImgTag = anchorText;
                    }
                    // Insert softbreak after the '@' sign.
                    atPos++;
                    newText.Append( priorToImgTag.Substring( 0, atPos ) );
                    newText.Append( softbreak );
                    newText.Append( priorToImgTag.Substring( atPos, priorToImgTag.Length - atPos ) );
                    // Now insert softbreak after each dot.
                    priorToImgTag = newText.ToString();
                    newText.Remove( 0, newText.ToString().Length );
                    int dotPos = priorToImgTag.IndexOf( "." );
                    while ( dotPos > -1 )
                    {
                        dotPos++;
                        newText.Append( priorToImgTag.Substring( 0, dotPos ) );
                        newText.Append( softbreak );
                        priorToImgTag = priorToImgTag.Substring( dotPos, priorToImgTag.Length - dotPos );
                        dotPos = priorToImgTag.IndexOf( "." );
                    }
                    newText.Append( priorToImgTag );
                    newText.Append( imgTagToEnd );
                }
                else
                {
                    newText.Append( anchorText );
                }
            }
            return newText.ToString();
        }//
        /// <summary>
        /// Insert landing page - used to record a link to an external site before actual transfer
        /// </summary>
        /// <param name="insideTag">Destination URL</param>
        /// <returns></returns>

 
        #endregion

        /// <summary>
        /// Format a title (such as for a library) to be url friendly
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string UrlFriendlyTitle( string title )
        {
            if ( title == null || title.Trim().Length == 0 )
                return "";

            title = title.Trim();

            string encodedTitle = title.Replace( " - ", "-" );
            encodedTitle = encodedTitle.Replace( " ", "_" );
            //encodedTitle = encodedTitle.Replace( ".", "-" );
            encodedTitle = encodedTitle.Replace( "'", "" );
            encodedTitle = encodedTitle.Replace( "&", "-" );
            encodedTitle = encodedTitle.Replace( "#", "" );
            encodedTitle = encodedTitle.Replace( "$", "S" );
            encodedTitle = encodedTitle.Replace( "%", "percent" );
            encodedTitle = encodedTitle.Replace( "^", "" );
            encodedTitle = encodedTitle.Replace( "*", "" );
            encodedTitle = encodedTitle.Replace( "+", "_" );
            encodedTitle = encodedTitle.Replace( "~", "_" );
            encodedTitle = encodedTitle.Replace( "`", "_" );
            encodedTitle = encodedTitle.Replace( ":", "-" );
            encodedTitle = encodedTitle.Replace( ";", "" );
            encodedTitle = encodedTitle.Replace( "?", "" );
            encodedTitle = encodedTitle.Replace( "\"", "_" );
            encodedTitle = encodedTitle.Replace( "\\", "_" );
            encodedTitle = encodedTitle.Replace( "<", "_" );
            encodedTitle = encodedTitle.Replace( ">", "_" );
            encodedTitle = encodedTitle.Replace( "__", "_" );
            encodedTitle = encodedTitle.Replace( "__", "_" );

            if ( encodedTitle.EndsWith( "." ) )
                encodedTitle = encodedTitle.Substring( 0, encodedTitle.Length - 1 );

            return encodedTitle;
        } //

		#region string helpers
        /// <summary>
        /// Retrieve a string item from the current cache
        /// - assumes a default value of blank
        /// </summary>
        /// <param name="cacheKeyName"></param>
        /// <returns></returns>
        public static string GetCacheItem( string cacheKeyName )
        {
            string defaultValue = "";
            return GetCacheItem( cacheKeyName, defaultValue );

        }//
        /// <summary>
        /// Retrieve a string item from the current cache
        /// </summary>
        /// <param name="cacheKeyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetCacheItem( string cacheKeyName, string defaultValue )
        {
            string cacheItem = defaultValue;
            try
            {
                cacheItem = HttpContext.Current.Cache[ cacheKeyName ] as string;

                if ( string.IsNullOrEmpty( cacheItem ) )
                {
                    //assuming keyname is same as file name in app_Data - or should the ext also be part of the key?
                    string dataLoc = String.Format( "~/App_Data/{0}.txt", cacheKeyName );
                    string file = System.Web.HttpContext.Current.Server.MapPath( dataLoc );

                    cacheItem = File.ReadAllText( file );
                    //save in cache for future
                    HttpContext.Current.Cache[ cacheKeyName ] = cacheItem;
                }
            }
            catch ( Exception ex )
            {
                LoggingHelper.LogError( ex, thisClassName + ".GetCacheItem( string cacheKeyName, string defaultValue ). Error retrieving item key: " + cacheKeyName );
                cacheItem = defaultValue;
            }
            return cacheItem;
        }//

		/// <summary>
		/// extract value of a particular named parameter from passed string (Assumes and equal sign is used)
		/// ex: for string:		
		///			string searchString = "key1=value1;key2=value2;key3=value3;";
		/// To retrieve the value for key2 use:
		///			value = ExtractNameValue( searchString, "key2", ";");
		/// </summary>
		/// <param name="sourceString">String to search</param>
		/// <param name="name">Name of "parameter" in string</param>
		/// <param name="endDelimiter">End Delimeter. A character used to indicate the end of value in the string (often a semi-colon)</param>
		/// <returns>The value associated with the passed name</returns>
		public static string ExtractNameValue( string sourceString, string name, string endDelimiter )
		{
			string assignDelimiter = "=";

			return ExtractNameValue( sourceString, name, assignDelimiter, endDelimiter );
		}//

		/// <summary>
		/// extract value of a particular named parameter from passed string. The assign delimiter
		/// ex: for string:		
		///			string radioButtonId = "Radio_q_4_c_15_";
		/// To retrieve the value for question # use:
		///			qNbr = ExtractNameValue( radioButtonId, "q", "_", "_");
		/// To retrieve the value for choiceId use:
		///			choiceId = ExtractNameValue( radioButtonId, "c", "_", "_");
		/// </summary>
		/// <param name="sourceString">String to search</param>
		/// <param name="name">Name of "parameter" in string</param>
		/// <param name="assignDelimiter">Assigned delimiter. Typically an equal sign (=), but could be any defining character</param>
		/// <param name="endDelimiter">End Delimeter. A character used to indicate the end of value in the string (often a semi-colon)</param>
		/// <returns></returns>
		public static string ExtractNameValue( string sourceString, string name, string assignDelimiter, string endDelimiter )
		{
			int pos = sourceString.IndexOf( name + assignDelimiter );

			if ( pos == -1 )
				return "";

			string value = sourceString.Substring( pos + name.Length + 1 );
			int pos2 = value.IndexOf( endDelimiter );
			if ( pos2 > -1 )
				value = value.Substring( 0, pos2 );

			return value;
		}//

		#endregion
		#region === Miscellaneous helper methods: defaults, IsDatatype, etc. ===
		/// <summary>
		/// Returns passed string as an integer, if is an integer and not null/empty. 
		/// Otherwise returns the passed default value
		/// </summary>
		/// <param name="stringToTest"></param>
		/// <param name="defaultValue"></param>
		/// <returns>The string parameter as an int or the default value if the parameter is not a vlid integer</returns>
		public static int AssignWithDefault( string stringToTest, int defaultValue )
        {
            int newVal;

            try
            {
                if ( stringToTest.Length > 0 && IsInteger( stringToTest ) )
                {
                    newVal = Int32.Parse( stringToTest );
                }
                else
                {
                    newVal = defaultValue;
                }
            }
            catch
            {

                newVal = defaultValue;
            }

            return newVal;

        } //

        /// <summary>
        /// Checks passed string, if not nullthen returns the passed string. 
        ///	Otherwise returns the passed default value
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <param name="defaultValue"></param> 
        /// <returns>int</returns>
        public static string AssignWithDefault( string stringToTest, string defaultValue )
        {
            string newVal;

            try
            {
                if ( stringToTest == null )
                {
                    newVal = defaultValue;
                }
                else
                {
                    newVal = stringToTest;
                }
            }
            catch
            {

                newVal = defaultValue;
            }

            return newVal;

        } //

        public static bool Assign( bool? value, bool defaultValue )
        {
            bool newVal;

            try
            {
                if ( value != null )
                {
                    newVal = ( bool ) value;
                }
                else
                {
                    newVal = defaultValue;
                }
            }
            catch
            {

                newVal = defaultValue;
            }

            return newVal;

        } //
        public static int Assign(int? value, int defaultValue)
        {
            int newVal;
            try
            {
                if (value != null)
                {
                    newVal = (int)value;
                }
                else
                {
                    newVal = defaultValue;
                }
            }
            catch
            {

                newVal = defaultValue;
            }

            return newVal;

        } //

        /// <summary>
        /// CurrencyToDecimal - handle assignment of a string containing formatted currency to a decimal
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal CurrencyToDecimal( string strValue, decimal defaultValue )
        {

            decimal decimalAmt = 0;

            try
            {
                if ( strValue == "" )
                {
                    decimalAmt = defaultValue;
                }
                else
                {
                    //remove leading $
                    string amount = strValue.Replace( "$", "" );
                    decimalAmt = decimal.Parse( amount );
                }
            }
            catch
            {

                decimalAmt = defaultValue;
            }

            return decimalAmt;

        } //
        /// <summary>
        /// IsInteger - test if passed string is an integer
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        public static bool IsInteger( string stringToTest )
        {
            int newVal;
            bool result = false;
            try
            {
                newVal = Int32.Parse( stringToTest );

                // If we get here, then number is an integer
                result = true;
            }
            catch
            {

                result = false;
            }
            return result;

        }

        /// <summary>
        /// IsNumeric - test if passed string is numeric
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        public static bool IsNumeric( string stringToTest )
        {
            double newVal;
            bool result = false;
            try
            {
                result = double.TryParse( stringToTest, NumberStyles.Any,
                    NumberFormatInfo.InvariantInfo, out newVal );
            }
            catch
            {

                result = false;
            }
            return result;

        }


        /// <summary>
        /// IsDate - test if passed string is a valid date
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        public static bool IsDate( string stringToTest )
        {

            DateTime newDate;
            bool result = false;
            try
            {
                newDate = System.DateTime.Parse( stringToTest );
                result = true;
            }
            catch
            {

                result = false;
            }
            return result;

        } //end


        #endregion


        public static decimal ConvertRatingRange(int oldMin, int oldMax, decimal value, int newMin, int newMax)
        {
            decimal oldRange = (decimal)oldMax - (decimal)oldMin;
            decimal newRange = (decimal)newMax - (decimal)newMin;
            decimal multiplier = newRange / oldRange;

            decimal retVal = value - oldMin;
            retVal = retVal * multiplier;
            retVal += newMin;

            return retVal;
        }

    }
}
