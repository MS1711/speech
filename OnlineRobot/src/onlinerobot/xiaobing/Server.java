package onlinerobot.xiaobing;

import java.io.BufferedReader;
import java.io.DataInputStream;  
import java.io.DataOutputStream;  
import java.io.File;
import java.io.FileReader;
import java.io.IOException;  
import java.net.ServerSocket;  
import java.net.Socket;  
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;

import weibo4j.Account;
import weibo4j.Comments;
//import weibo4j.examples.oauth2.Log;
import weibo4j.model.ApiRateLimits;
import weibo4j.model.Comment;
import weibo4j.model.CommentWapper;
import weibo4j.model.Paging;
import weibo4j.model.RateLimitStatus;
import weibo4j.model.WeiboException;
  
public class Server {  
	public static String access_token = "";
	public  static String cid = "";
	public static String id = "";
	public static String comments = "";
	public static ArrayList<String> accessList = new ArrayList<String>();
	public static ArrayList<String> noAnswer = new ArrayList<String>();
	public static int accessIndex = 0;
	public static int answerIndex = 0;
    public static void main(String[] args) {  
    	
		cid = args[1];
		id = args[2];
		
		GetFileToList(accessList,"access");
		GetFileToList(noAnswer,"noanswer.txt");
		access_token = accessList.get(accessIndex);
		accessIndex = (accessIndex+1)%accessList.size();
    	Server manager = new Server();  
        manager.doListen();  
    }  
      
    private static void GetFileToList(ArrayList<String> oneList, String path) {
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

	public void doListen() {  
        ServerSocket server;  
        try {  
            server = new ServerSocket(8000);  
            while (true) {  
                Socket client = server.accept();  
                new Thread(new SSocket(client)).start();  
            }  
        } catch (IOException e) {  
            e.printStackTrace();  
        }  
  
    }  
  
   
    class SSocket implements Runnable {  
  
        Socket client;  
  
        public SSocket(Socket client) {  
            this.client = client;  
        }  
  
        public void run() {  
            DataInputStream input;  
            DataOutputStream output;  
            try { 
            	
                input = new DataInputStream(client.getInputStream());  
                output = new DataOutputStream(client.getOutputStream()); 
                String result="";
                StringTokenizer st=null;
         
                String listMsg = input.readUTF(); 
        		comments = listMsg;
        		String text = connectXiaoBing();       		
        		output.writeUTF(text); 
        		
        		System.out.println("Recive:   " + listMsg);  
                       		                
            } catch (IOException e) {  
                e.printStackTrace();  
            }  
        }

		public String connectXiaoBing() {
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
    			Thread.sleep(5500);
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

    }


  
}  