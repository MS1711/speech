package device.control;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.Charset;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.HttpClients;

import properties.Config;

public class DeviceControlProxy {

	public static int control(String deviceId, String action, String params) {
		String status = "";
		HttpClient client = HttpClients.createDefault();
		String urlpattern = Config.getInstance().get("device_control_url", "http://localhost") +"/device/control?deviceId=%1$s&action=%2$s&param=%3$s";
		String url = String.format(urlpattern, new Object[]{deviceId, action, params});
		HttpGet cmd = new HttpGet(url);
		cmd.addHeader("Accept", "application/json");
		try {
			HttpResponse res = client.execute(cmd);
			HttpEntity entity = res.getEntity();
			if (entity != null) {
				BufferedReader br = new BufferedReader(new InputStreamReader(entity.getContent()));
				String line = br.readLine();
				while (line != null) {
					status += line;
					line = br.readLine();
				}
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		return Integer.parseInt(status);
	}
	
	public static String queryDeviceStatus(String deviceId) {
		String status = "";
		HttpClient client = HttpClients.createDefault();
		HttpGet cmd = new HttpGet(Config.getInstance().get("device_control_url", "http://localhost") + "/device/query?deviceId=" + deviceId);
		cmd.addHeader("Accept", "application/json");
		try {
			HttpResponse res = client.execute(cmd);
			HttpEntity entity = res.getEntity();
			if (entity != null) {
				BufferedReader br = new BufferedReader(new InputStreamReader(entity.getContent(), Charset.forName("utf-8")));
				String line = br.readLine();
				while (line != null) {
					status += line;
					line = br.readLine();
				}
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return status;
	}
	
	public static void main(String[] args) {
		System.out.println(DeviceControlProxy.control("", "stop", ""));
	}
}
