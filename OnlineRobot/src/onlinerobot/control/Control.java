package onlinerobot.control;

import java.net.URLEncoder;
import java.util.List;

import com.google.gson.Gson;

import onlinerobot.sobot.Sobot;
import onlinerobot.sobot.SobotResult;
import onlinerobot.tuling.TuLing;
import onlinerobot.xiaobing.XiaoBing;

public class Control {
	private Sobot sobot;
	private XiaoBing bing;
	private TuLing tuLing;
	public Control(){
		sobot = new Sobot();
		bing = new XiaoBing();
		tuLing = new TuLing();
	}
	public String GetSobot(String query){
		System.out.println(query);
		String url0 = "http://www.sobot.com/chat/StdChatService?data=";
		String url1 = "{\"sysNum\":\"5b8634f72de442fdaea6b8d7f9296c1e\",\"userId\":\"222\",\"content\":\"";
		query.replaceAll(" ", "");//??????????��????????
		if(sobot.isEnglish(query))
			query = URLEncoder.encode(query);
		else
			query = query.replace(" ", "%20");
		String url2 = "\",\"ip\":\"127.0.0.1\"}";//URLEncode???????????????????????��??????????
		String url = url1+query+url2;		
		System.out.println(url0 + URLEncoder.encode(url1) + query + URLEncoder.encode(url2));
		String downpage = sobot.downloadPage(url0 + URLEncoder.encode(url1) + query + URLEncoder.encode(url2), "UTF-8");
		Gson page = new Gson();
		SobotResult transResult = new SobotResult();
		System.out.println(downpage);
		transResult = page.fromJson(downpage, SobotResult.class);
		int status = transResult.getStatus();
		String result = "";
		if(status == 0){	
			 result = transResult.getContent();
		}
		else
			result = "主人，我不知道答案呢";
		if(result == null || result.equals("")){
			List<String> contents = transResult.getContents();
			if(contents!=null){
				result = "主人，我不知道答案呢，你可以问我" + contents.get(0) + "哦";
			}
	//		result = "主人，我不知道答案呢";
		}
		
		return FiterResult(result);
	}
	private String FiterResult(String result) {//过滤掉一些乱字符
		// TODO Auto-generated method stub
		StringBuilder afterFilter  = new StringBuilder("");
		afterFilter.append(result);
		int endIndex = -1;
		int beginIndex  = afterFilter.indexOf("<");
		while(beginIndex != -1){
			endIndex = afterFilter.indexOf(">");
			if(endIndex != -1 && beginIndex < endIndex)
				afterFilter.replace(beginIndex, endIndex+1, "");
			else if(endIndex != -1 && beginIndex > endIndex){
				afterFilter.replace(endIndex, endIndex + 1, "");
			}
			else
				afterFilter.replace(beginIndex, beginIndex + 1, "");
			beginIndex  = afterFilter.indexOf("<");
		}
		endIndex = afterFilter.indexOf(">");
		while(endIndex != -1){		
			afterFilter.replace(endIndex, endIndex + 1, "");
			endIndex = afterFilter.indexOf(">");
		}
		return afterFilter.toString();
	}
	public String GetBing(String comments) {
		System.out.println(comments);
		String result = null;
		result = bing.connectXiaoBing(comments);
		return result;
	}
	public String GetTuLing(String query){
		System.out.println(query);
		String result = null;
		result = tuLing.getResult(query);
		return result;
	}
	public static void main(String[] args) {
		Control one = new Control();
		one.GetSobot("你为什么");
		one.GetBing("你为什么叫小冰");
		one.GetTuLing("你是谁");
	}
	
	
}
