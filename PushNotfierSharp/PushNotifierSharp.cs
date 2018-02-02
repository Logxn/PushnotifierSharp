using System.Net;
using System;
using System.Collections.Generic;

using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace PushNotfierSharp
{
    public class PushNotifierSharp
    {
        static string _user;
        static string _password;
        static string _apptoken;
        static bool _loggedin;
        static RestClient _client;

        const string BASE_URL = "https://api.pushnotifier.de";
        const string APPTOKEN_HEADER = "X-AppToken";
        const string LOGIN_ENDPOINT = "/v2/user/login";
        const string REFRESH_ENDPOINT = "/v2/user/refresh";
        const string DEVICE_ENDPOINT = "/v2/devices";
        const string SEND_TEXT_ENDPOINT = "/v2/notifications/text";
        const string SEND_URL_ENDPOINT = "/v2/notifications/url";
        const string SEND_NOTIFICATION_ENDPOINT = "/v2/notifications/notification";

        /* 
         * Initializes the webclient with the base url
         * Authorization: Basic package_name:api_token => Base64
         * 
         */
        public PushNotifierSharp(string username, string password, string api_token, string package_name)
        {
            _user = username;
            _password = password;

            _client = new RestClient(BASE_URL)
            {
                Authenticator = new HttpBasicAuthenticator(package_name, api_token)
            };
        }

        /*
         * Sends a login request to pushnotifier.de 
         * returns => LoginObject
         * contains => app_ticket, ticket_expire_time, success, username
         */
        public LoginObject Login()
        {
            LoginObject loginResponse = new LoginObject();

            RestRequest loginRequest = new RestRequest(Method.POST);
            loginRequest.Resource = LOGIN_ENDPOINT;
            loginRequest.AddJsonBody(new
            {
                username = _user,
                password = _password
            });

            var callback = _client.Execute(loginRequest);

            if (callback.StatusCode == HttpStatusCode.NotFound) // If username or password is wrong
                throw new WrongCredentialsException("Server received wrong credentials. (404)");

            if (callback.StatusCode == HttpStatusCode.Forbidden)
                throw new WrongCredentialsException("Server received wrong credentials. (403)");

            if (callback.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Server received wrong api-key and package-name combination.");

            if (callback.StatusCode == HttpStatusCode.OK)
            {
                loginResponse = JsonConvert.DeserializeObject<LoginObject>(callback.Content);
                loginResponse.success = true;

                _apptoken = loginResponse.app_token;
                _loggedin = true;
            }

            return loginResponse;
        }

        /*
         * Sends a app-token refresh request to pushnotifier.de 
         * Requires => Valid app-ticket
         * Returns => New LoginObject
         * Contains => app_ticket, ticket_expire_time, success, username
         */
        public RefreshObject GetNewAppToken()
        {
            if (!_loggedin)
                throw new NotLoggedInException("You need to login first before trying to refresh your access token");

            RefreshObject refreshResponse = new RefreshObject();

            RestRequest refreshRequest = new RestRequest(Method.GET);
            refreshRequest.Resource = REFRESH_ENDPOINT;
            refreshRequest.AddHeader(APPTOKEN_HEADER, _apptoken);

            var callback = _client.Execute(refreshRequest);

            if (callback.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Server received wrong app-token");

            if (callback.StatusCode == HttpStatusCode.OK)
            {
                refreshResponse = JsonConvert.DeserializeObject<RefreshObject>(callback.Content);
            }

            return refreshResponse;
        }

        /*
        * Sends a request to get all devices connected to a specific app_package
        * Requires => Valid app-ticket
        * Returns => A list of devices via DeviceObject
        * Contains => id, model, title, image
        */
        public List<DeviceObject> GetAllDevices()
        {
            if (!_loggedin)
                throw new NotLoggedInException("You need to login first before getting all the devices.");

            List<DeviceObject> deviceResponse = null;

            RestRequest deviceRequest = new RestRequest(Method.GET);
            deviceRequest.Resource = DEVICE_ENDPOINT;
            deviceRequest.AddHeader(APPTOKEN_HEADER, _apptoken);

            var callback = _client.Execute(deviceRequest);

            if (callback.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Server received wrong app-token");

            if (callback.StatusCode == HttpStatusCode.OK)
            {
                // Transforms device array into a list
                deviceResponse = (List<DeviceObject>)JsonConvert.DeserializeObject(callback.Content, typeof(List<DeviceObject>));
            }

            return deviceResponse;
        }

         /*
         * Sends a pure text notification to a specific device
         * Params => message (msg to send), device_id (device to send to), send_silent (make noise or no?)
         * Requires => Valid app-ticket
         * Returns => Response via NotificationObject
         * Contains => Sucess (a list of device id's that received the msg), Error (a list of devices that didn't receive the msg)
         */
        public NotificationObject SendText(string message, string device_id, bool send_silent = false)
        {
            if (!_loggedin)
                throw new NotLoggedInException("You need to login first before sending a notification!");

            NotificationObject sendTextResponse = new NotificationObject();

            RestRequest sendTextRequest = new RestRequest(Method.PUT);
            sendTextRequest.Resource = SEND_TEXT_ENDPOINT;
            sendTextRequest.AddHeader(APPTOKEN_HEADER, _apptoken);
            sendTextRequest.RequestFormat = DataFormat.Json;
            sendTextRequest.AddJsonBody(new
            {
                devices = new { device_id }, // Creates an array like "devices": ["device_id", "device_id", ...]
                content = message,
                silent = false
            });

            var callback = _client.Execute(sendTextRequest);

            if (callback.StatusCode == HttpStatusCode.BadRequest)
                throw new BadRequestException("Server received a bad request. Notification was not sent.");

            if (callback.StatusCode == HttpStatusCode.NotFound)
                throw new DeviceNotFoundException($"The requested device id {device_id} was not found.");

            if (callback.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Server received wrong app-token.");

            if (callback.StatusCode == HttpStatusCode.OK)
            {
                sendTextResponse = JsonConvert.DeserializeObject<NotificationObject>(callback.Content);
            }

            return sendTextResponse;
        }

        /*
         * Sends a pure URL notification to a specific device
         * Params => url (url to send), device_id (device to send to), send_silent (make noise or no?)
         * Requires => Valid app-ticket
         * Returns => Response via NotificationObject
         * Contains => Sucess (a list of device id's that received the msg), Error (a list of devices that didn't receive the msg)
         */
        public NotificationObject SendURL(string url_msg, string device_id, bool silent = false)
        {
            if (!_loggedin)
                throw new NotLoggedInException("You need to login first before sending a notification!");

            NotificationObject sendURLResponse = new NotificationObject();

            RestRequest sendURLRequest = new RestRequest(Method.PUT);
            sendURLRequest.Resource = SEND_URL_ENDPOINT;
            sendURLRequest.AddHeader(APPTOKEN_HEADER, _apptoken);
            sendURLRequest.RequestFormat = DataFormat.Json;
            sendURLRequest.AddJsonBody(new
            {
                devices = new { device_id }, // Creates an array like "devices": ["device_id", "device_id", ...]
                url = url_msg,
                silent = silent
            });

            var callback = _client.Execute(sendURLRequest);

            if (callback.StatusCode == HttpStatusCode.BadRequest)
                throw new BadRequestException("Server received a bad request. Notification was not sent.");

            if (callback.StatusCode == HttpStatusCode.NotFound)
                throw new DeviceNotFoundException($"The requested device id {device_id} was not found.");

            if (callback.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Server received wrong app-token.");

            if (callback.StatusCode == HttpStatusCode.OK)
            {
                sendURLResponse = JsonConvert.DeserializeObject<NotificationObject>(callback.Content);
            }

            return sendURLResponse;
        }

        /*
         * Sends a notification (url & text) to a specific device
         * Params => message (msg to send), device_id (device to send to), send_silent (make noise or no?)
         * Requires => Valid app-ticket
         * Returns => Response via NotificationObject
         * Contains => Sucess (a list of device id's that received the msg), Error (a list of devices that didn't receive the msg)
         */
        public NotificationObject SendNotification(string message, string url_msg, string device_id, bool silent = false)
        {
            if (!_loggedin)
                throw new NotLoggedInException("You need to login first before sending a notification!");

            NotificationObject sendNotificationResponse = new NotificationObject();

            RestRequest sendNotificationRequest = new RestRequest(Method.PUT);
            sendNotificationRequest.Resource = SEND_NOTIFICATION_ENDPOINT;
            sendNotificationRequest.AddHeader(APPTOKEN_HEADER, _apptoken);
            sendNotificationRequest.RequestFormat = DataFormat.Json;
            sendNotificationRequest.AddJsonBody(new
            {
                devices = new { device_id }, // Creates an array like "devices": ["device_id", "device_id", ...]
                content = message,
                url = url_msg,
                silent = silent
            });

            var callback = _client.Execute(sendNotificationRequest);

            if (callback.StatusCode == HttpStatusCode.BadRequest)
                throw new BadRequestException("Server received a bad request. Notification was not sent.");

            if (callback.StatusCode == HttpStatusCode.NotFound)
                throw new DeviceNotFoundException($"The requested device id {device_id} was not found.");

            if (callback.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Server received wrong app-token.");

            if (callback.StatusCode == HttpStatusCode.OK)
            {
                sendNotificationResponse = JsonConvert.DeserializeObject<NotificationObject>(callback.Content);
            }

            return sendNotificationResponse;
        }

    }
}
