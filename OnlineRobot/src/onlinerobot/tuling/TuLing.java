package onlinerobot.tuling;

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

import onlinerobot.sobot.SobotResult;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;

import com.google.gson.Gson;

public class  TuLing{
	private String apikey;
	public TuLing(){
		apikey = "f4fcea951c6a9daf4aa498fc98e2461b";
	}

	public static void main(String[] args) {
		TuLing test = new TuLing();
		
		test.getResult("最近好看的电影");
	    
		
	}
	public String getResult(String query1){
		String returnResult = null;
		try {	
			String query = URLEncoder.encode(query1, "utf-8");
		     String url = "http://www.tuling123.com/openapi/api?key="+apikey+"&info="+query; 
		     HttpGet request = new HttpGet(url); 
		     HttpResponse response;	
			response = HttpClients.createDefault().execute(request);
			if(response.getStatusLine().getStatusCode()==200){ 
		         String downpage = EntityUtils.toString(response.getEntity()); 
		         System.out.println("返回结果："+downpage); 
		 		returnResult = GetReturnResult(downpage);
		 		
		     } 
		}catch (ClientProtocolException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} 
		if(returnResult == null || returnResult.equals("") || returnResult.equals("null"))
 			returnResult = "主人我不知道呢";
		return returnResult;
	     //200即正确的返回码 	     
	}
	public  String GetReturnResult(String downpage) {
		String returnResult = null;
		StringBuilder buildResult = new StringBuilder();
		Gson page = new Gson();
	 	CommonResult commonResult = new CommonResult();
	 	commonResult = page.fromJson(downpage, CommonResult.class);
	 	if(commonResult.getCode() == 100000){//文本类型
	 		returnResult = commonResult.getText();
	 	}
	 	else if(commonResult.getCode() == 200000){//url类型
	 		UrlResult urlResult = new UrlResult();
	 		urlResult = page.fromJson(downpage, UrlResult.class);
	 		buildResult.append(urlResult.getText() + "。");
	 		buildResult.append("具体链接地址是：" + urlResult.getUrl());
	 		returnResult = buildResult.toString();
	 	}
	 	else if(commonResult.getCode() == 302000){//新闻类型
	 		NewsResult newsResult = new NewsResult();
	 		newsResult = page.fromJson(downpage, NewsResult.class);
	 		buildResult.append(newsResult.getText() + "。");
	 		for(int i = 0;i<newsResult.getList().size();i++){
	 			News news = newsResult.getList().get(i);
	 			buildResult.append("。"+news.getSource()+"," + news.getArticle());
	 		}
	 		returnResult = buildResult.toString();	
	 	}
	 	else if(commonResult.getCode() == 305000){//列车类型
	 		TrainResult trainResult = new TrainResult();
	 		trainResult = page.fromJson(downpage, TrainResult.class);
	 		buildResult.append(trainResult.getText()+"。");
	 		for(int i = 0;i<trainResult.getList().size();i++){
	 			Train train = trainResult.getList().get(i);
	 			buildResult.append("编号："+train.getTrainnum() + ",从" + train.getStarttime() + "始发自" + 
	 			train.getStart() + ",在" + train.getEndtime() + "到达" + train.getTerminal() + "。");
	 		}
	 		returnResult = buildResult.toString();	
	 	}
	 	else if(commonResult.getCode() == 306000){//航班类型
	 		FlightResult flightResult = new FlightResult();
	 		flightResult = page.fromJson(downpage, FlightResult.class);
	 		buildResult.append(flightResult.getText()+"。");
	 		for(int i = 0;i<flightResult.getList().size();i++){
	 			Flight flight = flightResult.getList().get(i);
	 			buildResult.append(flight.getFlight() +flight.getStarttime() + "出发，" + flight.getEndtime() + "到达。");
	 		}
	 		returnResult = buildResult.toString();	
	 	}
	 	else if(commonResult.getCode() == 308000){//电影信息，视频信息,菜谱
	 		FilmResult filmResult = new FilmResult();
	 		filmResult = page.fromJson(downpage, FilmResult.class);
	 		buildResult.append(filmResult.getText()+"。");
	 		for(int i = 0;i<filmResult.getList().size();i++){
	 			Film film = filmResult.getList().get(i);
	 			buildResult.append(film.getName() +","+ film.getInfo() +  ",详情链接为" + film.getDetailurl() + "。");
	 		}
	 		returnResult = buildResult.toString();	
	 	}
	 	else if(commonResult.getCode() == 309000){//酒店
	 		HotelResult hotelResult = new HotelResult();
	 		hotelResult = page.fromJson(downpage, HotelResult.class);
	 		buildResult.append(hotelResult.getText()+"。");
	 		for(int i = 0;i<hotelResult.getList().size();i++){
	 			Hotel hotel = hotelResult.getList().get(i);
	 			buildResult.append(hotel.getName() + "，价钱" + hotel.getPrice() + "," + hotel.getSatisfaction() + "。");
	 		}
	 		returnResult = buildResult.toString();	
	 	}
	 	else if(commonResult.getCode() == 311000){//价钱
	 		PriceResult priceResult = new PriceResult();
	 		priceResult = page.fromJson(downpage, PriceResult.class);
	 		buildResult.append(priceResult.getText()+"。");
	 		for(int i = 0;i<priceResult.getList().size();i++){
	 			Price price = priceResult.getList().get(i);
	 			buildResult.append(price.getName() + "，价钱" + price.getPrice()+"。");
	 		}
	 		returnResult = buildResult.toString();	
	 	}
	 	else if(commonResult.getCode() == 40002){//文本类型
	 		returnResult = commonResult.getText();
	 	}
	 	else{
	 		returnResult = "主人我还不具备这个功能哦";
	 	}
	 	System.out.println(returnResult);
		return returnResult;
	}
}
