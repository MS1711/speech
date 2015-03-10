package onlinerobot.sobot;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.List;

import org.apache.http.HttpEntity;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;

import com.google.gson.Gson;

public class Sobot {

	/**
	 * @param args
	 */
	public String downloadPage(String pageURL, String encoding) {

		StringBuilder pageHTML = new StringBuilder();
		BufferedReader br = null;
		// HttpURLConnection connection = null;
		CloseableHttpClient httpclient = null;
		HttpGet get = null;
		CloseableHttpResponse response = null;
		int count = 0;
		restart: while (true) {
			try {
				// HttpClient??????????????
				// HttpClient httpClient = new DefaultHttpClient();
				httpclient = HttpClients.createDefault();
				// ????HTTP GET ???????????????
				get = new HttpGet(pageURL);
				// ??��?????????????????
				response = httpclient.execute(get);
				// ??��???????????????????(??????HTTP HEAD)
				HttpEntity entity = response.getEntity();
				if (entity != null) {
					InputStream is = entity.getContent();
					// ??InputStream????Reader?????????????????��???????
					// ????��?????? BufferedReader
					br = new BufferedReader(new InputStreamReader(is, encoding));
					File file = new File("sobot.txt");
					BufferedWriter bw = new BufferedWriter(
							new OutputStreamWriter(new FileOutputStream(file,true)));
					if (br != null) {
						String s = null;
						while ((s = br.readLine()) != null) {
							//System.out.println(s);
							pageHTML.append(s);
							bw.write(s);
							bw.flush();
						}
						bw.write("\r\n");
						bw.flush();
						bw.close();
						br.close();
						is.close();
					}
				}

				EntityUtils.consume(entity);
				entity = null;
				// connection.disconnect();
				if (response != null){
					response.close();
					response = null;
				}
				if (get != null){
					get.releaseConnection();
					get = null;
				}
				if (httpclient != null){
					httpclient.close();
					httpclient = null;
				}

			} catch (Exception e) {

				e.printStackTrace();
				try {
					if (response != null)
						response.close();
					if (get != null){
						get.releaseConnection();
						get = null;
					}
					if (httpclient != null)
						httpclient.close();
					//Thread.sleep(2000000);
					count++;
					if(count<5)
						continue restart;
				} catch (Exception e1) {
					e1.printStackTrace();
					count++;
					if(count<5)
						continue restart;
				}

			}
			count = 0;
			return pageHTML.toString();
		}
	}
	
	public  boolean isEnglish(String charaString){ 
	      return charaString.matches("^[a-zA-Z]*"); 
	    } 
	public static void main(String[] args) {
		Sobot test = new Sobot();
		String url0 = "http://www.sobot.com/chat/StdChatService?data=";
		String url1 = "{\"sysNum\":\"5b8634f72de442fdaea6b8d7f9296c1e\",\"userId\":\"222\",\"content\":\"";
		String query = "你好";
		query.replaceAll(" ", "");
		if(test.isEnglish(query))
			query = URLEncoder.encode(query);
		else
			query = query.replace(" ", "%20");
		String url2 = "\",\"ip\":\"127.0.0.1\"}";
		String url = url1+query+url2;		
		System.out.println(url0 + URLEncoder.encode(url1) + query + URLEncoder.encode(url2));
		String downpage = test.downloadPage(url0 + URLEncoder.encode(url1) + query + URLEncoder.encode(url2), "UTF-8");
		Gson page = new Gson();
		SobotResult transResult = new SobotResult();
		System.out.println(downpage);
		transResult = page.fromJson(downpage, SobotResult.class);
		System.out.println(transResult.getContent());
		}

}
