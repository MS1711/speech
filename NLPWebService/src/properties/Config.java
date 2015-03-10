package properties;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class Config {

	private static Config instance = new Config();
	private Properties prop = new Properties();
	
	public static Config getInstance() {
		return instance;
	}
	
	private Config(){
		InputStream in = Config.class.getResourceAsStream("/properties/config.properties");
		try {
			prop.load(in);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public String get(String key) {
		return get(key, "");
	}
	
	public String get(String key, String defaultV) {
		String v = prop.getProperty(key);
		return v == null? defaultV : v;
	}
}
