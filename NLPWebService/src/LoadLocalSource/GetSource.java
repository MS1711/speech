/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package LoadLocalSource;
import com.mongodb.DB;
import com.mongodb.Mongo;
import com.mongodb.MongoException;
import com.mongodb.DBCollection;
import com.mongodb.DBCursor;
import com.mongodb.DBObject;
import com.mongodb.BasicDBObject;
import com.mongodb.MongoClient;
import com.mongodb.QueryOperators;
import com.mongodb.util.JSON;

import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Vector;
import java.util.logging.Level;
import java.util.logging.Logger;

import javax.swing.text.Document;

import properties.Config;

/**
 *
 * @author v-feiche
 */
public class GetSource {
    MongoClient mongoClient;
    DB sourceDB;
    private String dictPath;
    
    public static void main(String[] args)  {
        GetSource test = new  GetSource();
        test.GetLast2();
    }
    
    public GetSource()
    {
    	dictPath = Config.getInstance().get("model_path", "C:/dict/");
        try {
            mongoClient = new MongoClient(Config.getInstance().get("mongo_host", "127.0.0.1"), Integer.parseInt(Config.getInstance().get("mongo_port", "27017")));
            sourceDB = mongoClient.getDB("knowledgeDB");
        } catch (UnknownHostException ex) {
            System.out.println("Error in connecting to MongoDB");
        }
    }
    
    public void CloseConnection()
    {
        mongoClient.close();
    }
    
    public void GetVerb(){
		DBConn verb = new DBConn();
		DBCursor cur = verb.searchAll("commandCollection");
		String verbDictPath = dictPath + "verbdict.txt";
		OutputStreamWriter writer = null;
		try {
			writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),
					"UTF-8");
			// writer = new FileWriter(verbDictPath);
			writer.write("\r\n");
			writer.write("\r\n");
		} catch (IOException ex) {
			ex.printStackTrace();
		}
		while (cur.hasNext()) {
			String commandName = "";
			String command = "";
			String resource = "";
			DBObject searchResult = cur.next();
			Object temp = null;
			temp = searchResult.get("Name");
			if (temp != null)
				commandName = temp.toString();
			command = searchResult.get("Command").toString();
			temp = searchResult.get("Resource");
			if (temp != null)
				resource = temp.toString();
			if (resource.contains("music") || resource.contains("story")
					|| resource.contains("radio")) {
				try {
					if (command.equals("start"))
						writer.write(commandName + " " + "音频动词" + "\n");
					else if (command.equals("stop"))
						writer.write(commandName + " " + "停止动词" + "\n");
				} catch (IOException ex) {
					ex.printStackTrace();
				}
			}
		}
		try {
			writer.flush();
			writer.close();
		} catch (IOException ex) {
			ex.printStackTrace();
		}       
    }
    public void GetDictVerb(){
        DBConn verb  = new DBConn();
        DBCursor cur =    verb.searchAll("commandCollection");
         String verbDictPath = dictPath + "dictdict.txt";       
            OutputStreamWriter  writer = null;
            try {
                writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),"UTF-8");
                writer.write("\r\n");
                writer.write("\r\n");  
            } catch (IOException ex) {
                ex.printStackTrace();
            }               
        while(cur.hasNext()){
            String command = "";
            String resource = "";
            DBObject searchResult = cur.next();
            Object temp = null;
           temp = searchResult.get("Name");
            if(temp!= null)
                command = temp.toString();
            
            temp = searchResult.get("Resource");
            if(temp!= null)
                resource = temp.toString();
            if(resource.contains("dict") ){            
                try {               
                    writer.write(command + " " + "词典动词" + "\n");
                 } catch (IOException ex) {
                     ex.printStackTrace();
                 }                                      
            }
           
            
        }
        try {
                writer.flush();
                writer.close();
            } catch (IOException ex) {
                ex.printStackTrace();
            }       
    }
    public void GetGenre(){
        DBConn verb  = new DBConn();
        HashMap<String,String> genreMap = new  HashMap<String,String>();
        DBCursor cur =    verb.searchAll("mediaCollection");
        String verbDictPath = dictPath + "genredict.txt";       
        OutputStreamWriter  writer = null;
        
        try {
            writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),"UTF-8");
            writer.write("\r\n");
            writer.write("\r\n");  
        }catch (IOException ex) {
            ex.printStackTrace();
        }               
        while(cur.hasNext()){
        	String genre = null;
            DBObject searchResult = cur.next();
            Object temp = null;
            DBObject metadata;
            metadata = (DBObject) searchResult.get("Metadata"); 
            if(metadata != null){
            	temp = metadata.get("Genre");
            	if(temp != null)
            		genre = temp.toString();
            	if(genre != null && !genre.equals("") && !genre.equals("null") && !genre.equals("Null") && !genre.equals("NULL"))
            		genreMap.put(genre, "");
            }            	                        
        }
        try{
        	Iterator iter = genreMap.entrySet().iterator();
        	while (iter.hasNext()) {
        	Map.Entry entry = (Map.Entry) iter.next();
        	String key = entry.getKey().toString();
        	writer.write(key + " " + "音乐风格" + "\n");
        	}        	
             writer.flush();
             writer.close();
           } catch (IOException ex) {
               ex.printStackTrace();
           }       
    }
     public void GetHomeVerb(){
        DBConn verb  = new DBConn();
        DBCursor cur =    verb.searchAll("commandCollection");
         String verbDictPath = dictPath + "homedict.txt";       
            OutputStreamWriter  writer = null;
            try {
                writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),"UTF-8");
                writer.write("\r\n");
                 writer.write("\r\n");
            } catch (IOException ex) {
                ex.printStackTrace();
            }               
        while(cur.hasNext()){
            String command = "";
            String device = "";
            DBObject searchResult = cur.next();
            Object temp = null;
           temp = searchResult.get("Name");
            if(temp!= null)
                command = temp.toString();
            
            temp = searchResult.get("Device");
            if(temp!= null)
                device = temp.toString();
            if(!device.equals("null") && !device.equals("NULL") && device != null && !device.equals("")){            
                try {               
                    writer.write(command + " " + "家居动词" + "\n");
                 } catch (IOException ex) {
                     ex.printStackTrace();
                 }                                      
            }
           
            
        }
        try {
                writer.flush();
                writer.close();
            } catch (IOException ex) {
                ex.printStackTrace();
            }       
    }
     public Vector<String> GetStartVerb(){
         DBConn verb  = new DBConn();
         DBCursor cur =    verb.searchAll("commandCollection");        
         Vector<String> myStartVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String resource = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString();
             if(command.equals("start"))
             {
                 temp = searchResult.get("Resource");
                 if (temp != null) {
                     resource = temp.toString();
                 }
                 if (resource.contains("music") || resource.contains("story") || resource.contains("radio")) {
                     myStartVerb.add(commandName);
                 }
             }
         }
         return myStartVerb;
     }
     public Vector<String> GetStopVerb()
     {
         DBConn verb  = new DBConn();
         DBCursor cur =    verb.searchAll("commandCollection");        
         Vector<String> myStopVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String resource = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString();
             if(command.equals("stop"))
             {
                 temp = searchResult.get("Resource");
                 if (temp != null) {
                     resource = temp.toString();
                 }
                 if (resource.contains("music") || resource.contains("story") || resource.contains("radio")) {
                     myStopVerb.add(commandName);
                 }
             }
         }
         return myStopVerb;
     }
     public Vector<String> GetSearchVerb(){
         DBConn verb  = new DBConn();
         DBCursor cur =    verb.searchAll("commandCollection");
         Vector<String> mySearchVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String resource = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString();       
             if(command.equals("lookup"))
             {
                 temp = searchResult.get("Resource");
                 if (temp != null) {
                     resource = temp.toString();
                 }
                 if (resource.contains("dict")) {
                     mySearchVerb.add(commandName);
                 }
             }
         } 
         return mySearchVerb;
     }
     public Vector<String> GetOpenVerb(){
         DBConn verb  = new DBConn();
         DBCursor cur =    verb.searchAll("commandCollection");
         Vector<String> myOpenVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String device = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString(); 
             if(command.equals("open"))
             {
                 temp = searchResult.get("Device");
                 if (temp != null) {
                     device = temp.toString();
                 }
                 if (!device.equals("null") && !device.equals("NULL") && device != null && !device.equals("")) {
                     myOpenVerb.add(commandName);
                 }     
             }
         }   
         return myOpenVerb;
     }
     public Vector<String> GetCloseVerb(){
         DBConn verb  = new DBConn();
         DBCursor cur =    verb.searchAll("commandCollection");
         Vector<String> myCloseVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String device = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString(); 
             if(command.equals("close"))
             {
                 temp = searchResult.get("Device");
                 if (temp != null) {
                     device = temp.toString();
                 }
                 if (!device.equals("null") && !device.equals("NULL") && device != null && !device.equals("")) {
                     myCloseVerb.add(commandName);
                 }     
             }
         } 
         return myCloseVerb;
     }
      public Vector<String> GetChangeVerb()
      {
         DBConn verb = new DBConn();
         DBCursor cur = verb.searchAll("commandCollection");
         Vector<String> myChangeVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String device = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString(); 
             if(command.equals("change"))
             {
                 temp = searchResult.get("Device");
                 if (temp != null) {
                     device = temp.toString();
                 }
                 if (!device.equals("null") && !device.equals("NULL") && device != null && !device.equals("")) {
                     myChangeVerb.add(commandName);
                 }     
             }
         } 
         return myChangeVerb;
      }
      public Vector<String> GetUpVerb()
      {
         DBConn verb = new DBConn();
         DBCursor cur = verb.searchAll("commandCollection");
         Vector<String> myUpVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String device = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString(); 
             if(command.equals("up"))
             {
                 temp = searchResult.get("Device");
                 if (temp != null) {
                     device = temp.toString();
                 }
                 if (!device.equals("null") && !device.equals("NULL") && device != null && !device.equals("")) {
                     myUpVerb.add(commandName);
                 }     
             }
         } 
         return myUpVerb;
      }
      
      public Vector<String> GetDownVerb()
      {
         DBConn verb = new DBConn();
         DBCursor cur = verb.searchAll("commandCollection");
         Vector<String> myDownVerb = new Vector<String>();
         while(cur.hasNext()){
             String command = "";
             String device = "";
             DBObject searchResult = cur.next();
             Object temp = null;
             String commandName = null;
             commandName = searchResult.get("Name").toString();
             temp = searchResult.get("Command");
             if(temp!= null)
                 command = temp.toString(); 
             if(command.equals("down"))
             {
                 temp = searchResult.get("Device");
                 if (temp != null) {
                     device = temp.toString();
                 }
                 if (!device.equals("null") && !device.equals("NULL") && device != null && !device.equals("")) {
                     myDownVerb.add(commandName);
                 }     
             }
         } 
         return myDownVerb;
      }

	public void GetVerb2() {//NLPCore3中使用的
		DBConn verb = new DBConn();
		DBCursor cur = verb.searchAll("commandCollection");
		String verbDictPath = dictPath + "/verbdict2.txt";
		OutputStreamWriter writer = null;
		try {
			writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),
					"UTF-8");
			// writer = new FileWriter(verbDictPath);
			writer.write("\r\n");
			writer.write("\r\n");
		} catch (IOException ex) {
			ex.printStackTrace();
		}
		while (cur.hasNext()) {
			String commandName = "";
			String command = "";
			String type = "";
			DBObject searchResult = cur.next();
			Object temp = null;
			temp = searchResult.get("Name");
			if (temp != null)
				commandName = temp.toString();
			temp = searchResult.get("Command");
			if (temp != null)
				command = temp.toString();
			temp = searchResult.get("Scene");
			if (temp != null)
				type = temp.toString();

			try {
			if (type.equals("media")) {						
				writer.write(commandName + " " + "媒体动词" + "\n");
			} 
			else if(type.equals("home"))
				writer.write(commandName + " " + "家居动词" + "\n");
			else if(type.equals("search"))
				writer.write(commandName + " " + "查询动词" + "\n");
			else if(type.equals("fuzzy"))
				writer.write(commandName + " " + "混合动词" + "\n");
			
			}catch (IOException ex) {
				ex.printStackTrace();
			}
		}
		try {
			writer.flush();
			writer.close();
		} catch (IOException ex) {
			ex.printStackTrace();
		}       
		
	}

	public void GetDevice2() {
		DBConn verb = new DBConn();
		DBCursor cur = verb.searchAll("smartHomeCollection");
		String verbDictPath = dictPath + "devicedict2.txt";
		OutputStreamWriter writer = null;
		try {
			writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),
					"UTF-8");
			// writer = new FileWriter(verbDictPath);
			writer.write("\r\n");
			writer.write("\r\n");
		} catch (IOException ex) {
			ex.printStackTrace();
		}
		while (cur.hasNext()) {
			String name = "";
			String type = "";
			String arrtibute = "";
			DBObject searchResult = cur.next();
			Object temp = null;
			temp = searchResult.get("Name");
			if (temp != null)
				name = temp.toString();			
			//temp = searchResult.get("Type");
			//if (temp != null)
				//type = temp.toString();
			temp = searchResult.get("Class");
			if (temp != null)
				arrtibute = temp.toString();
			try {
				//if(type.equals("home"))
					writer.write(name + " " + arrtibute + "\n");			
			}catch (IOException ex) {
				ex.printStackTrace();
			}
		}
		try {
			writer.flush();
			writer.close();
		} catch (IOException ex) {
			ex.printStackTrace();
		}       		
	}

	public void GetLast2() {
		DBConn verb = new DBConn();
		DBCursor cur = verb.searchAll("suffixCollection");
		String verbDictPath = dictPath + "lastdict2.txt";
		OutputStreamWriter writer = null;
		try {
			writer = new OutputStreamWriter(new FileOutputStream(verbDictPath),
					"UTF-8");
			// writer = new FileWriter(verbDictPath);
			writer.write("\r\n");
			writer.write("\r\n");
		} catch (IOException ex) {
			ex.printStackTrace();
		}
		while (cur.hasNext()) {
			String name = "";
			String type = "";
			DBObject searchResult = cur.next();
			Object temp = null;
			temp = searchResult.get("Name");
			if (temp != null)
				name = temp.toString();			
			temp = searchResult.get("Type");
			if (temp != null)
				type = temp.toString();
			try {
				writer.write(name + " " + type + "\n");			
			}catch (IOException ex) {
				ex.printStackTrace();
			}
		}
		try {
			writer.flush();
			writer.close();
		} catch (IOException ex) {
			ex.printStackTrace();
		}       
		
		
	}

	public void LoadDeviceType(ArrayList<String> deviceType) {
		DBConn verb  = new DBConn();
        HashMap<String,String> genreMap = new  HashMap<String,String>();
        DBCursor cur =    verb.searchAll("smartHomeCollection");       
        while(cur.hasNext()){
        	String attribute = "";
            DBObject searchResult = cur.next();
            Object temp = null;
            temp = searchResult.get("Class");
			if (temp != null)
				attribute = temp.toString();
            if(!attribute.equals(""))
            	genreMap.put(attribute, "");
        }
        Iterator iter = genreMap.entrySet().iterator();
    	while (iter.hasNext()) {
	    	Map.Entry entry = (Map.Entry) iter.next();
	    	String key = entry.getKey().toString();
	    	deviceType.add(key);
    	}
        
	}
}
