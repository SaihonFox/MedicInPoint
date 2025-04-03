namespace MedicInPoint.API;

public static class MedicConfiguration
{
	public const bool USE_LOCALHOST = false;
	
	public const string LOCALHOST_URL = @"http://localhost:5033/";
	
	public const string RENDER_URL = @"https://medicapi.onrender.com/";

	public static string URL = USE_LOCALHOST ? LOCALHOST_URL : RENDER_URL;
}	