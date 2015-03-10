package weather.api;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.charset.Charset;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Random;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpressionException;
import javax.xml.xpath.XPathFactory;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

public class WeatherService {

	private static Random rand = new Random();
	public static String URL = "http://wthrcdn.etouch.cn/WeatherApi?city=";
	
	public static String getWeatherByCityName(String city, Date fromDate) {
		return getWeatherByCityName(city, fromDate, fromDate);
	}

	public static String getWeatherByCityName(String city, Date fromDate, Date endDate) {
		String queryUrl = URL + city;
		String status = "";
		HttpClient client = HttpClients.createDefault();
		HttpGet cmd = new HttpGet(queryUrl);
		try {
			HttpResponse res = client.execute(cmd);
			HttpEntity entity = res.getEntity();
			if (entity != null) {
				String restring = EntityUtils.toString(entity,
						Charset.forName("utf-8"));
				if (restring != null && restring.length() != 0) {
					SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd");
					status = digestWeatherInfo(restring, fromDate, endDate, df.format(fromDate).equals(df.format(endDate)) && df.format(fromDate).equals(df.format(new Date())));
				}
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return status;
	}

	private static String digestWeatherInfo(String restring, Date fromDate, Date endDate, boolean details) {
		StringBuffer sb = new StringBuffer();
		try {
			DocumentBuilderFactory factory = DocumentBuilderFactory
					.newInstance();
			DocumentBuilder builder = factory.newDocumentBuilder();
			InputStream is = new ByteArrayInputStream(
					restring.getBytes("utf-8"));
			Document document = builder.parse(is);
			Element root = document.getDocumentElement();
			XPath xpath = XPathFactory.newInstance().newXPath();

			long gap = (endDate.getTime() - fromDate.getTime()) / (24 * 3600000);
			long gap2 = gap;
			String highLowTemp = "%1$s%2$s,%3$s,";
			//String day = new SimpleDateFormat("M月dd日").format(fromDate);

			String high = "";
			String low = "";
			String dayInfo = "";
			String nightInfo = "";
			String city = "";

			String datePattern = new SimpleDateFormat("d").format(fromDate) + "日";
			NodeList weathers = (NodeList) xpath.evaluate(
					"/resp/forecast/weather", document, XPathConstants.NODESET);

			city = (String) xpath.evaluate("/resp/city/text()",
					document, XPathConstants.STRING);
			sb.append(city);
			boolean start = false;
			for (int i = 0; i < weathers.getLength(); i++) {
				Node node = weathers.item(i);
				String weatherDay = (String) xpath.evaluate("date/text()",
						node, XPathConstants.STRING);
				if (weatherDay.startsWith(datePattern) || (start && gap >= 0)) {
					start = true;
					high = (String) xpath.evaluate("high/text()", node,
							XPathConstants.STRING);
					high = high.replaceAll("高温", "最高温度");
					low = (String) xpath.evaluate("low/text()", node,
							XPathConstants.STRING);
					low = low.replaceAll("低温", "最低温度");
					if (gap2 == 0) {
						weatherDay = getDayDesc(fromDate);
					} else {
						Date d = new Date();
						d.setTime(fromDate.getTime() + 24*3600000*(gap2-gap));
						weatherDay = new SimpleDateFormat("M月d日").format(d);
					}
					String info = String.format(highLowTemp, new Object[]{weatherDay, high, low});
					sb.append(info);
					
					String dtype = (String) xpath.evaluate("ay/type/text()", node,
							XPathConstants.STRING);
					if (dtype == null || dtype.length() == 0) {
						dtype = "晴";
					}
					String dinfo = "白天" + dtype + ",";
					String wind = (String) xpath.evaluate("day/fengxiang/text()", node,
							XPathConstants.STRING);
					if (wind != null && wind.length() != 0 && wind.indexOf("无") == -1) {
						dinfo += wind + ",";
						String windStrongness = (String) xpath.evaluate("day/fengli/text()", node,
								XPathConstants.STRING);
						if (windStrongness != null && windStrongness.length() != 0) {
							dinfo += windStrongness + ",";
						}
						
					}
					sb.append(dinfo);
					
					
					String ntype = (String) xpath.evaluate("night/type/text()", node,
							XPathConstants.STRING);
					if (ntype == null || ntype.length() == 0) {
						ntype = "晴";
					}
					String ninfo = "晚上" + ntype + ",";
					
					wind = (String) xpath.evaluate("night/fengxiang/text()", node,
							XPathConstants.STRING);
					if (wind != null && wind.length() != 0 && wind.indexOf("无") == -1) {
						ninfo += wind + ",";
						String windStrongness = (String) xpath.evaluate("night/fengli/text()", node,
								XPathConstants.STRING);
						if (windStrongness != null && windStrongness.length() != 0) {
							ninfo += windStrongness + ",";
						}
						
					}
					sb.append(ninfo);
					
					gap--;
				}
			}
			
			if (details) {
				String envPattern = "今天PM2.5指数为%1$s,属于%2$s,%3$s建议%4$s,";
				String pm25 = (String) xpath.evaluate("/resp/environment/pm25/text()",
						document, XPathConstants.STRING);
				String quality = (String) xpath.evaluate("/resp/environment/quality/text()",
						document, XPathConstants.STRING);
				String major = (String) xpath.evaluate("/resp/environment/MajorPollutants/text()",
						document, XPathConstants.STRING);
				if (major != null && major.length() > 0) {
					major = "主要污染物为" + major + ",";
				}
				String suggest = (String) xpath.evaluate("/resp/environment/suggest/text()",
						document, XPathConstants.STRING);
				sb.append(String.format(envPattern, new Object[]{pm25, quality, major, suggest}));
				
				String zhiShuPattern = "今天%1$s为,%2$s,%3$s";
				NodeList zhishuList = (NodeList) xpath.evaluate(
						"/resp/zhishus/zhishu", document, XPathConstants.NODESET);
				if (zhishuList.getLength() > 0) {
					int ind = rand.nextInt(zhishuList.getLength());
					Node zhishu = zhishuList.item(ind);
					String zhiShuName = (String) xpath.evaluate("name/text()",
							zhishu, XPathConstants.STRING);
					String zhishuValue = (String) xpath.evaluate("value/text()",
							zhishu, XPathConstants.STRING);
					String detail = (String) xpath.evaluate("detail/text()",
							zhishu, XPathConstants.STRING);
					sb.append(String.format(zhiShuPattern, new Object[]{zhiShuName, zhishuValue, detail}));
				}
			}
			
		} catch (ParserConfigurationException | SAXException | IOException e) {
			e.printStackTrace();
		} catch (XPathExpressionException e) {
			e.printStackTrace();
		}

		return sb.toString();
	}

	private static String getDayDesc(Date fromDate) {
		if (fromDate == null) {
			return "";
		}
		
		try {
			SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd");
			Date s = df.parse(df.format(fromDate));
			Date e = df.parse(df.format(new Date()));
			
			int gap = (int) ((s.getTime() - e.getTime()) / (24*3600000));
			switch (gap) {
			case 0:
				return "今天";
			case 1:
				return "明天";
			case 2:
				return "后天";
			}
		} catch (ParseException e) {
			e.printStackTrace();
		}
		
		return new SimpleDateFormat("M月d日").format(fromDate);
	}

	public static void main(String[] args) {
		String time = "北京1月21日天气";
		Pattern p = Pattern.compile("([0-9]{1,2})月([0-9]{1,2})[号|日]");
		Matcher m = p.matcher(time);
		if (m.find()) {
			int gc = m.groupCount();
			for (int i = 0; i <= gc; i++) {
				System.out.println(m.group(i));
			}
		}
		System.out.println(m.find());
//		System.out.println(Pattern.matches("[0-9]{1,2}月[0-9]{1,2}[号|日]", time));
//		System.out.println(Pattern.matches("北京1月21日天气", time));
//		Date s = new Date();
//		Date e = new Date(s.getTime() + 24*3600000);
//		System.out.println(WeatherService.getWeatherByCityName("北京", e, e));
	}
}
