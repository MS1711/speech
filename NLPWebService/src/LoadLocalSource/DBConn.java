package LoadLocalSource;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.regex.Pattern;

import com.mongodb.DB;
import com.mongodb.Mongo;
import com.mongodb.MongoException;
import com.mongodb.DBCollection;
import com.mongodb.DBCursor;
import com.mongodb.DBObject;
import com.mongodb.BasicDBObject;
import com.mongodb.QueryOperators;
import com.mongodb.util.JSON;
public class DBConn {

	private String DBname="knowledgeDB";
	private Mongo m=null;
	
	private String username=null;
	private String password=null;
	
	private static DBConn dbconn;
	
	
	//使用工厂模式   设计模式
	public static DBConn getDBConnInstance()
	{
		if(dbconn==null)
			return new DBConn();
		else
			return dbconn;
	}
	

	public static void main(String[] args)  {

//		String tmpName="dana n. xu";
//		String first=tmpName.substring(0, tmpName.indexOf(' '));
//		String second=tmpName.substring(tmpName.indexOf(' ')+1,tmpName.length());
//		System.out.println(first);
//		System.out.println(second);
//		ArrayList<String> list=new ArrayList<String>();
//		list.add("Xiaohua  a");
//		list.add("caia");
//		list.add("hua");
//		System.out.println(list.toString().contains("xiaohua"));
		String t=new String("robert w. heath jr.");
		System.out.println(t.contains("robert") && t.contains("w") && t.contains("heath"));
	}
	//add for auto_complete at 2014-5-9
	public static DBCursor getCur(String collectName,String prefix,String getAttribute)
	{
		DBConn dbconn=new DBConn();
		DBCollection student=dbconn.getDB().getCollection(collectName);
		BasicDBObject cond=new BasicDBObject();
		if(prefix!="")
		{  
			Pattern pattern=Pattern.compile("^"+prefix); 

	          cond.put(getAttribute, pattern);

	      }
		DBCursor cur=student.find(cond);
		return cur;
	}
	public static String getResult(String key)
	{
		ArrayList<String> list=DBConn.findNameTop10(key);
		String result="";
	    while(!list.isEmpty())
	    {
		  result=result+list.remove(0)+"	";
		
	    }
	    list.clear();
	    return result;
	}
	public static ArrayList<String> findNameTop10(String keywords) {  

		ArrayList<String> result = new ArrayList<String>();
		DBConn dbconn=new DBConn();
		DBCollection student=dbconn.getDB().getCollection("scholar_names_auto");
		
		BasicDBObject cond=new BasicDBObject();
		if(keywords!=""){  

	          //Pattern pattern=Pattern.compile("^.*"+keywords+".*$"); 
			Pattern pattern=Pattern.compile("^"+keywords); 

	          cond.put("name", pattern);

	      }
		
		DBCursor cur=student.find(cond).sort(new BasicDBObject("paper_count",-1)).limit(10);
		while(cur.hasNext())
		{
			result.add((String)cur.next().get("name"));
			
		}
        return result;

   } 
	
	public static ArrayList<String> findNameTop10_new(String keywords) {  

		ArrayList<String> result = new ArrayList<String>();
		for(int i=0;i<10;i++)
		{
			
				result.add("a");
				
		}
		DBConn dbconn=new DBConn();
		DBCollection student=dbconn.getDB().getCollection("scholar_names");
		
		BasicDBObject cond=new BasicDBObject();
		if(keywords!=""){  

	          //Pattern pattern=Pattern.compile("^.*"+keywords+".*$"); 
			Pattern pattern=Pattern.compile("^"+keywords); 

	          cond.put("name", pattern);

	      }
		DBCursor cur=student.find(cond).limit(1000);
		int a[]=new int[10];
		
       // List<User> users = mongoTemplate.find(new Query(new Criteria("name").regex(".*?"+"张"+".*")).limit(9), User.class);  
		while(cur.hasNext())
		{
			student=dbconn.getDB().getCollection("paper_meta");
			String t=cur.next().get("name").toString();
			int tmp=student.find(new BasicDBObject("authors_low_case",t)).count();
			//Arrays.sort(a);	
			for(int i=0;i<10;i++)
			{
				if(tmp>a[i]) 
					{
					a[i]=tmp;
				    result.set(i, t);
				    //System.out.println(a[0]+" "+a[1]+" "+a[2]+" "+a[3]+" "+a[4]+" "+a[5]+" "+a[6]+" "+a[7]+" "+a[8]+" "+a[9]);
					break;
					}
			}
					
			
		}
        return result;

   } 
	
    public static String getName(String prefix)
    {
    	DBConn dbconn=new DBConn();
    	DBCursor cur=dbconn.search("scholar_names","name",prefix);
		return cur.next().get("name").toString();
    	
    }
    
  //add end
	public  String getUsername() {
		return username;
	}
	
	
	public void setUsername(String username) {
		this.username = username;
	}
	
	
	public String getPassword() {
		return password;
	}
	
	
	public void setPassword(String password) {
		this.password = password;
	}
	
	
	public void setDBname(String DBname){
		this.DBname=DBname;
	}
	
	
	public String getDBname(){
		
		return this.DBname;
	}
	
	
	//构造函数
	//用于初始化Mongo m 还有 DB db
	
	public DBConn(){ 
		
		try {
			m = new Mongo("127.0.0.1",27017);
		} catch (UnknownHostException e) {
			e.printStackTrace();
		} catch (MongoException e){
			e.printStackTrace();
		}
		//以后可以在此处设置    改变最大连接数量
		
	}
	
	public DB getDB(){
		if(m==null)
			return null;
		else
			return m.getDB(getDBname());
	}
	//加个测试函数 尝试输出mongostat的状态
	//其中包含了输出连接池的信息
	
	public static int displayStats(){
		
		System.out.println(DBConn.getDBConnInstance());
		DB db=DBConn.getDBConnInstance().getDB();
		System.out.println(db);
		System.out.println(db.command("connPoolStats").getInt("totalCreated"));
		System.out.println(db.command("connPoolSync"));
		
		System.out.println(db.command("dbStats"));
	
		return db.command("connPoolStats").getInt("totalCreated");
		
		
		
	}
	
	
	//查询函数
	//用于查询一个集合中所有的文档
	//返回类型DBCursor
	public DBCursor searchAll(String collectionName){

		//if(db.authenticate(this.getUsername(), this.getPassword().toCharArray())){
			
			DBCollection student=getDB().getCollection(collectionName);
			DBCursor cur=student.find();
			return cur;//连接正常时返回查询的游标
	//}
		//return null;//用户或者密码错误时 返回null
	}
	
	
	//有条件的查询函数
	//两个参数都是String
	//重载函数
	public DBCursor search(String collectionName,String findItem,String itemValue){
		
		//if(db.authenticate(this.getUsername(), this.getPassword().toCharArray())){
			
			DBCollection student=getDB().getCollection(collectionName);
			DBCursor cur=student.find(new BasicDBObject(findItem,itemValue));
			return cur;//连接正常时返回查询的游标
	//用户或者密码错误时 返回null
		
	}
    public DBCursor search(String collectionName,String findItem,String[] itemValue){
		
    	
			
			DBCollection student=getDB().getCollection(collectionName);
			DBCursor cur=student.find(new BasicDBObject(findItem, new BasicDBObject(QueryOperators.IN,itemValue)));
			return cur;//连接正常时返回查询的游标
	//用户或者密码错误时 返回null
		
	}
	
	//一个参数的查询
	//第二值的类型是int
   public DBCursor search(String collectionName,String findItem,int itemValue){
		
		//if(db.authenticate(this.getUsername(), this.getPassword().toCharArray())){
			
			DBCollection student=getDB().getCollection(collectionName);
			DBCursor cur=student.find(new BasicDBObject(findItem,itemValue));
			return cur;//连接正常时返回查询的游标
	
	
		
	}
  public DBCursor search(String collectionName,String findItem,String itemValue, String findItem2,String itemValue2){
		
    	
			
			DBCollection student=getDB().getCollection(collectionName);
			//DBCursor cur=student.find(new BasicDBObject(findItem,itemValue),new BasicDBObject(findItem2,itemValue2));
                        DBCursor cur=student.find(new BasicDBObject(findItem, itemValue).append(findItem2, itemValue2));
			return cur;//连接正常时返回查询的游标
	//用户或者密码错误时 返回null
		
   }
   public boolean insert(String collectionName,String jsonStr){
	   
	  // if(db.authenticate(this.getUsername(), this.getPassword().toCharArray())){
			
		DBCollection student=getDB().getCollection(collectionName);
	   // String json="{'_id':2,'name':'water','age':34,'weight':140}";
	    DBObject data1=(DBObject)JSON.parse(jsonStr);
	    student.insert(data1);
	    return true;
	   
   }
   
   //一次性删除所有符合条件的记录
   public boolean delete(String collectionName,String jsonStr){
	   
		  
			DBCollection student=getDB().getCollection(collectionName);
		
		    DBObject data1=(DBObject)JSON.parse(jsonStr);
		    student.remove(data1);
		    return true;
		   
	   }

}
