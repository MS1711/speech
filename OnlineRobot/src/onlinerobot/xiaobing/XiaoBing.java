package onlinerobot.xiaobing;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import weibo4j.Account;
import weibo4j.Comments;
import weibo4j.Users;
import weibo4j.Weibo;
//import weibo4j.examples.oauth2.Log;
import weibo4j.model.ApiRateLimits;
import weibo4j.model.Comment;
import weibo4j.model.CommentWapper;
import weibo4j.model.Paging;
import weibo4j.model.RateLimitStatus;
import weibo4j.model.User;
import weibo4j.model.WeiboException;

public class XiaoBing {
	public static void main(String[] args) {
		XiaoBing bing = new XiaoBing();
	}
	public  String access_token = "";

	public  String cid = "";

	public  String id = "";

	public static ArrayList<String> accessList = new ArrayList<String>();
	public static ArrayList<String> noAnswer = new ArrayList<String>();
	public static int accessIndex = 0;
	public static int answerIndex = 0;
	public  XiaoBing(){
		if(accessList.isEmpty()){
			GetFileToList(accessList,"C:\\eclipse-workspace\\OnlineRobot\\access");		
		}			
		if(noAnswer.isEmpty())
			GetFileToList(noAnswer,"C:\\eclipse-workspace\\OnlineRobot\\noanswer.txt");
		access_token = accessList.get(accessIndex);
		accessIndex = (accessIndex+1)%accessList.size();
		cid = "3796432685699816";
		id = "3796432672803089";
		
	}
	 private  void GetFileToList(ArrayList<String> oneList, String path) {
			// TODO Auto-generated method stub
	    	File file = new File(path);
	        BufferedReader reader = null;
	        try {
	            //System.out.println("以行为单位读取文件内容，一次读一整行：");
	            reader = new BufferedReader(new FileReader(file));
	            String tempString = null;
	            int line = 0;
	            // 一次读入一行，直到读入null为文件结束
	            while ((tempString = reader.readLine()) != null) {
	                // 显示行号
	                //System.out.println("line " + line + ": " + tempString);
	            	oneList.add(tempString);
	               // line++;
	            }
	            reader.close();
	        } catch (IOException e) {
	            e.printStackTrace();
	        } finally {
	            if (reader != null) {
	                try {
	                    reader.close();
	                } catch (IOException e1) {
	                }
	            }
	        }
	        for(int i = 0;i<oneList.size();i++)
	        	System.out.println(oneList.get(i));
		}

	public String connectXiaoBing(String comments) {
		// TODO Auto-generated method stub
		boolean intoException = false;
		boolean  repeat = false;
		String text = "";
    	String responseId = ""; 
    	Date createdAt  = new Date();
    	
    	for(int j = 0;j<accessList.size();j++){//测试key可不可以用，有没有超限
        	try {
        		if(intoException){
        			intoException = false;
        			j = 1000;
        		}
        		Account am = new Account(access_token);
				RateLimitStatus jsonLimit = am.getAccountRateLimitStatus();
				List<ApiRateLimits> apiRateLimit = jsonLimit.getApiRateLimit();
				int remaining = 1;
				for(int i = 0;i<apiRateLimit.size();i++){
					if(apiRateLimit.get(i).getApi().equals("/comments/create") && apiRateLimit.get(i).getLimitTimeUnit().equals("HOURS")){
						remaining = apiRateLimit.get(i).getLimit();	
						j = 1000;
					}						
				}
				if(remaining < 1){
					access_token = accessList.get(accessIndex);
					accessIndex = (accessIndex+1)%accessList.size();
					j = 1000;
				}
					
				System.out.println("remaining:" + remaining);
				Log.logInfo(jsonLimit.toString());
			} catch (WeiboException e1) {
				// TODO Auto-generated catch block
				System.out.println("..............//////////////");
				access_token = accessList.get(accessIndex);
				accessIndex = (accessIndex+1)%accessList.size();
				intoException = true;
				e1.printStackTrace();
			}
    	}
		Comments cm = new Comments(access_token);
		System.out.println(access_token);
		try {
			try{
				Comment com = cm.replyComment(cid, id, comments);
			} catch (WeiboException e) {       			
    			if(e.getErrorCode() == 20019){
    				System.out.println("重复问题");
    				repeat = true;
    			}
    				
    			else if(e.getErrorCode() == 21332){
    				access_token = accessList.get(accessIndex);
					accessIndex = (accessIndex+1)%accessList.size();
					cm = new Comments(access_token);
    			}
    			else if(e.getErrorCode() == 10024){
    				access_token = accessList.get(accessIndex);
					accessIndex = (accessIndex+1)%accessList.size();
    			}
    			e.printStackTrace();
			}
			Paging page = new Paging();
			page.setCount(1);
			Thread.sleep(6000);
			CommentWapper comment = cm.getCommentToMe(page,0,0);
			//Log.logInfo(com.toString());
			Log.logInfo(comment.toString());
			List<Comment> listCom = comment.getComments();
			for (Comment c : listCom) {
				text = c.getText();
				responseId = Long.toString(c.getId());
				createdAt = c.getCreatedAt();
				System.out.println(text);
				System.out.println(responseId);
			}
			cid = responseId;//把下次评论对应的id换为最新的
		} catch (WeiboException e) {
			
			System.out.println("出错漏");
			if(e.getErrorCode() == 10024 || e.getErrorCode() == 21332 ){
				access_token = accessList.get(accessIndex);
				accessIndex = (accessIndex+1)%accessList.size();
				text = noAnswer.get(answerIndex);
				answerIndex = (answerIndex + 1)%noAnswer.size();
			}
			System.out.println(e.getErrorCode());
			e.printStackTrace();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			text = noAnswer.get(answerIndex);
			answerIndex = (answerIndex + 1)%noAnswer.size();
			e.printStackTrace();
		}
		System.out.println("repeat：" + repeat);
		if(repeat  == false){
    		long now = System.currentTimeMillis();//不要时间太长的回复
    		long commentData = createdAt.getTime();
    		System.out.println("time:"+(now-commentData));
    		if((now-commentData) > 60000){
    			text = noAnswer.get(answerIndex);
				answerIndex = (answerIndex + 1)%noAnswer.size();
    		}	    		
		}
		repeat = false;
		int cutPlace = text.indexOf("佳吟酱:");
		
		if(cutPlace != -1)
			text = text.substring(cutPlace+4);
		int	endPlace = text.indexOf("[");//有时候回复中有表情
		if(endPlace != -1)
			text = text.substring(0, endPlace);
		if(text == null || text.equals("")){
			text = noAnswer.get(answerIndex);
			answerIndex = (answerIndex + 1)%noAnswer.size();
		}
		int http = text.indexOf("查看图片");
		if(http != -1){
			text = "主人，这是一张图片哦";
		}
		return text;

	}

	public String getAccess_token() {
		return access_token;
	}

	public String getCid() {
		return cid;
	}

	public String getId() {
		return id;
	}

	public void setAccess_token(String access_token) {
		this.access_token = access_token;
	}
	public void setCid(String cid) {
		this.cid = cid;
	}
	
	public void setId(String id) {
		this.id = id;
	}

	public void setNoAnswer(ArrayList<String> noAnswer) {
		this.noAnswer = noAnswer;
	}

}
