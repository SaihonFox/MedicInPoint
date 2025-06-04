namespace MedicInPoint.API;

public static class MedicConfiguration
{
	public const bool USE_LOCALHOST = true;
	
	public const string LOCALHOST_URL = "http://localhost:5034/";
	
	public const string RENDER_URL = "https://medicapi.onrender.com/";

	public static string URL = USE_LOCALHOST ? LOCALHOST_URL : RENDER_URL;

	public const string AIMLAPI_URL = "https://api.aimlapi.com/";
}	